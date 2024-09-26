<template>
  <div id="system_setup_core_container">
    <!-- Modal -->
    <div class="modal fade" id="system_setup_core" tabindex="-1" role="dialog" aria-labelledby="system_setup_core_Title" aria-hidden="true">
      <div class="modal-dialog modal-dialog-centered" role="document">
        <div class="modal-content">
          <div class="modal-header">
              <h5 class="modal-title" id="system_setup_core_Title">{{ Resources.MOTOR_DE_PESQUISA__E50766 }}</h5>
            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
              <span aria-hidden="true">&times;</span>
            </button>
          </div>
          <div class="modal-body">
            <row>
              <text-input v-model="Model.Index" :label="Resources.INDEX00140" :isReadOnly="blockInputs" :size="'xlarge'"></text-input>
            </row>
            <row>
              <text-input v-model="Model.Id" :label="Resources.ID36840" :isReadOnly="blockInputs" :size="'xlarge'"></text-input>
            </row>
            <row>
              <text-input v-model="Model.Area" :label="Resources.AREA19058" :isReadOnly="blockInputs" :size="'xlarge'"></text-input>
            </row>
            <row>
              <text-input v-model="Model.Urlfscrawler" :label="Resources.FSCRAWLER01982" :isReadOnly="blockInputs" :size="'xlarge'"></text-input>
            </row>
            <row>
              <text-input v-model="Model.Url" :label="Resources.URL05719" :isReadOnly="blockInputs" :size="'xlarge'"></text-input>
            </row>
            <row>
                <text-input v-model="Model.ElasticUser" :label="Resources.UTILIZADOR52387" :isReadOnly="blockInputs" :size="'xlarge'"></text-input>
            </row>
            <row>
                <password-input v-model="Model.ElasticPsw" :label="Resources.PALAVRA_PASSE44126" :isReadOnly="blockInputs" :size="'xlarge'"></password-input>
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
              @click="SaveCoreCfg" />
            <q-button
              v-else
              b-style="primary"
              :label="Resources.GRAVAR45301"
              @click="SaveCoreCfg" />
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
        name: 'core_modal',
        emits: ['updateModal', 'close'],
        props: {
            Model: {
                required: true
            }
        },
        mixins: [reusableMixin],
        data: function () {
            return {
            };
        },
        methods: {
            SaveCoreCfg: function () {
                var vm = this;
                QUtils.postData('Config', 'SaveCoreCfg', vm.Model, null, function (data) {
                    if (data.Success) {
                        vm.$emit('updateModal');
                        $('#system_setup_core').modal('hide');
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
