<template>
  <div id="system_setup_scheduledjob_container">
    <!-- Modal -->
    <div class="modal fade" id="system_setup_scheduledjob" ref="system_setup_scheduledjob" tabindex="-1" role="dialog" aria-labelledby="system_setup_scheduledjob_Title" aria-hidden="true">
      <div class="modal-dialog modal-dialog-centered" role="document">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title" id="system_setup_scheduledjob_Title">{{ Resources.TAREFA_AGENDADA03399 }}</h5>
            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
              <span aria-hidden="true">&times;</span>
            </button>
          </div>
          <div class="modal-body">
            <row>
              <checkbox-input 
                v-model="Model.Enabled" 
                :label="Resources.ATIVO_00196" />
            </row>

            <row>
              <text-input 
                v-model="Model.Id"
                :label="Resources.NOME47814"
                :isReadOnly="blockInputs"
                :size="'xlarge'"
                :isRequired="true" />
            </row>
            <row>
              <select-input 
                v-model="Model.TaskType"
                :options="ScheduledJobSelect"
                :label="Resources.TIPO55111"
                :isReadOnly="blockInputs"
                :size="'xlarge'" />
            </row>
            <row>
              <text-input 
                v-model="Model.Cron"
                :label="'Cron'"
                :isReadOnly="blockInputs"
                :size="'xlarge'"
                :isRequired="true"
                placeholder="cron schedule"
                :helpText="Resources._SEGUNDO_MINUTO_HORA37214"
                 />
            </row>

            <hr/>

            <row v-for="c in JobTypeList[Model.TaskType]" :key="c.PropertyName">
              <text-input 
                v-model="Model.Options[c.PropertyName]"
                :label="c.DisplayName"
                :isReadOnly="blockInputs"
                :isRequired="!c.Optional"
                :size="'xlarge'"
                :helpText="c.Description"/>
            </row>

          </div>
          <div class="modal-footer">
            <q-button
              :label="Resources.CANCELAR49513"
              @click="close" />
            <q-button
              v-if="Model.FormMode === 'delete'"
              b-style="danger"
              :label="Resources.APAGAR04097"
              @click="SaveScheduledJob" />
            <q-button
              v-else
              b-style="primary"
              :label="Resources.GRAVAR45301"
              @click="SaveScheduledJob" />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
  import { reusableMixin } from '@/mixins/mainMixin';
  import { QUtils } from '../../utils/mainUtils';

  export default {
    name: 'scheduledjob_modal',
    emits: ['updateModal', 'close'],
    props: {
      Model: {
        required: true
      },
      JobTypeList: {
        required: true
      }
    },
    mixins: [reusableMixin],
    data: function () {
      return {
      };
    },
    mounted: function () {
        var vm = this;
        $(vm.$refs.system_setup_scheduledjob).modal('show');
        $(vm.$refs.system_setup_scheduledjob).on('hidden.bs.modal', function () {
            vm.$emit('close');
        });
    },
    methods: {
      SaveScheduledJob: function () {
        var vm = this;
        var eventData = {
          Data: vm.Model, 
          FormMode: vm.Model.FormMode
        };
        QUtils.postData('Config', 'SaveScheduledJob', eventData, null, function (data) {
            if (data.Success) {
              vm.$emit('updateModal', eventData);
              $('#system_setup_scheduledjob').modal('hide');
            }
            else {
              alert(data.Message);
            }
        });
      },
      close() {
        $(this.$refs.system_setup_scheduledjob).modal('hide');
        this.$emit('close')
      }
    },
    computed: {
      blockInputs: function () {
        return this.Model.FormMode === 'delete';
      },
      ScheduledJobSelect: function () {
        return Object.keys(this.JobTypeList).map(x => ({
           Text: x,
           Value: x
        }));
      }
    }
  };
</script>
