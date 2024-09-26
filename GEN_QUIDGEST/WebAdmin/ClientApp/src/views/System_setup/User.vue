<template>
  <div id="system_setup_user_container">
    <!-- Modal -->
    <div class="modal fade" id="system_setup_user" tabindex="-1" role="dialog" aria-labelledby="system_setup_user_Title" aria-hidden="true">
      <div class="modal-dialog modal-dialog-centered" role="document">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title" id="system_setup_user_Title">{{ Resources.UTILIZADOR_FIXO32336 }}</h5>
            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
              <span aria-hidden="true">&times;</span>
            </button>
          </div>
          <div class="modal-body">
            <row>
              <text-input v-model="Model.Name" :label="Resources.NOME47814" :isReadOnly="Model.FormMode != 'new'" :size="'xlarge'"></text-input>
            </row>
            <row>
              <select-input v-model="Model.Type" v-if="SelectLists" :options="SelectLists.DisplayUserType" :label="Resources.TIPO55111" :isReadOnly="blockInputs" :size="'xlarge'"></select-input>
            </row>
            <row>
              <checkbox-input v-model="Model.AutoLogin" :label="Resources.LOGIN_AUTOMATICO22707" :isReadOnly="blockInputs"></checkbox-input>
            </row>
            <row>
              <password-input v-model="Model.Password" :label="Resources.PASSWORD09467" :isReadOnly="blockInputs" :size="'xlarge'"></password-input>
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
              @click="SaveUserCfg" />
            <q-button
              v-else
              b-style="primary"
              :label="Resources.GRAVAR45301"
              @click="SaveUserCfg" />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
  // @ is an alias to /src
  import { reusableMixin } from '@/mixins/mainMixin';
  import { QUtils } from '../../utils/mainUtils';

  export default {
    name: 'user_modal',
    emits: ['callback', 'close'],
    props: {
      Model: {
        required: true
      },
      SelectLists: {
        required: true
      }
    },
    mixins: [reusableMixin],
    data: function () {
      return {
        temp: {}
      };
    },
    methods: {
      SaveUserCfg: function () {
        var vm = this;
        QUtils.postData('Config', 'SaveUserCfg', vm.Model, null, function (data) {
          if (data.success) {
            var eventData = { users: data.users };
            vm.$emit('callback', eventData);
            $('#system_setup_user').modal('hide');
          }
        });
      },
      close() {
        this.$emit('close')
      }
    },
    computed: {
      blockInputs: function () {
        return this.Model.FormMode === 'delete';
      }
    }
  };
</script>
