<template>
  <div id="system_setup_database_container">
    <br />
    <card>
      <template #header>
        Current Data System
      </template>
      <template #body>
        <row>
          <text-input v-model="Model.Server" :label="Resources.NOME_DO_SERVIDOR_E_I58011" :isRequired="true"></text-input>
        </row>
        <row>
          <numeric-input v-model="Model.Port" :label="Resources.PORTA55707"></numeric-input>
        </row>
        <row>
          <select-input v-model="Model.ServerType" v-if="Model.SelectLists" :options="Model.SelectLists.DBMS" :label="Resources.TIPO_DE_SERVIDOR_DE_25581" :isRequired="true"></select-input>
        </row>
        <div v-if="Model.ServerType == 2">
          <row>
            <text-input v-model="Model.Service" :label="Resources.IDENTIFICADOR_DO_SER22713"></text-input>
          </row>
          <row>
            <text-input v-model="Model.ServiceName" :label="Resources.NOME_DO_SERVICO32188"></text-input>
          </row>
        </div>
        <row>
          <text-input v-model="Model.Schema" :label="Resources._SISTEMA__DIV__ANO__08355" :isRequired="true"></text-input>
        </row>
        <hr />
        <row>
          <text-input v-model="Model.DbUser" :label="Resources.LOGIN_DE_ACESSO_A_BA52816" :isRequired="true"></text-input>
        </row>
        <row>
          <password-input v-model="Model.DbPsw" :label="Resources.PALAVRA_PASSE44126" :isRequired="true" :showFiller="Model.HasDbPsw"></password-input>
        </row>
        <row>
          <password-input v-model="Model.DbCheckPsw" :label="Resources.CONFIRMAR_PALAVRA_PA30977" :isRequired="true"></password-input>
        </row>
		    <row>
          <checkbox-input v-model="Model.ConnEncrypt" :label="Resources.ENCRIPTAR_LIGACAO12834"></checkbox-input>
        </row>
		    <row>
          <checkbox-input v-model="Model.ConnWithDomainUser" :label="Resources.UTILIZADOR_DE_DOMINI41043"></checkbox-input>
        </row>
      </template>
    </card>
    <br />
    <card :isCollapse="true">
        <template #header>
            Log Data System
        </template>
        <template #body>
          <row>
            <text-input v-model="Model.Log_Server" :label="Resources.NOME_DO_SERVIDOR_E_I58011"></text-input>
          </row>
          <row>
            <numeric-input v-model="Model.Log_Port" :label="Resources.PORTA55707"></numeric-input>
          </row>
          <row>
            <select-input v-model="Model.Log_ServerType" v-if="Model.SelectLists" :options="Model.SelectLists.DBMS" :label="Resources.TIPO_DE_SERVIDOR_DE_25581"></select-input>
          </row>
          <div v-if="Model.Log_ServerType == 2">
            <row>
              <text-input v-model="Model.Log_Service" :label="Resources.IDENTIFICADOR_DO_SER22713"></text-input>
            </row>
            <row>
              <text-input v-model="Model.Log_ServiceName" :label="Resources.NOME_DO_SERVICO32188"></text-input>
            </row>
          </div>
          <row>
            <text-input v-model="Model.Log_Schema" :label="Resources._SISTEMA__DIV__ANO__08355" :isRequired="true"></text-input>
          </row>
          <hr />
          <row>
            <text-input v-model="Model.Log_DbUser" :label="Resources.LOGIN_DE_ACESSO_A_BA52816"></text-input>
          </row>
          <row>
            <password-input v-model="Model.Log_DbPsw" :label="Resources.PALAVRA_PASSE44126" :showFiller="Model.Log_HasDbPsw"></password-input>
          </row>
          <row>
            <password-input v-model="Model.Log_DbCheckPsw" :label="Resources.CONFIRMAR_PALAVRA_PA30977"></password-input>
          </row>
          <row>
            <checkbox-input v-model="Model.Log_ConnEncrypt" :label="Resources.ENCRIPTAR_LIGACAO12834"></checkbox-input>
          </row>
          <row>
            <checkbox-input v-model="Model.Log_ConnWithDomainUser" :label="Resources.UTILIZADOR_DE_DOMINI41043"></checkbox-input>
          </row>
        </template>
    </card>
    <br />
    <card :isCollapse="true">
      <template #header>
        General Settings
      </template>
      <template #body>
        <row>
          <text-input v-model="Model.DefaultYear" :label="Resources.ESPECIFICAR_ANO_OU_042147"></text-input>
        </row>
        <row>
          <checkbox-input v-model="Model.HideYears" :label="Resources.OCULTAR_ANOS03755"></checkbox-input>
        </row>
        <br />
        <row>
          <q-button
            :label="Resources.CRIAR_UM_NOVO_SISTEM49777"
            @click="CreateDataSystem">
            <q-icon icon="plus-sign" />
          </q-button>
        </row>
      </template>
    </card>
    <hr />
    <row>
      <q-button
        b-style="primary"
        :label="Resources.GRAVAR_CONFIGURACAO36308"
        @click="SaveConfigDatabase" />
    </row>

    <div class="d-none">
      <div ref="templateFormNewDataSystem">
        <row>
          <text-input v-model="newDSForm.Name" :label="Resources.ANO33022" :size="'xlarge'"></text-input>
          <text-input v-model="newDSForm.Schema" :label="'DB Schema'" :size="'xlarge'"></text-input>
        </row>
      </div>
    </div>

  </div>
</template>

<script>
  // @ is an alias to /src
  import { reusableMixin } from '@/mixins/mainMixin';
  import { QUtils } from '@/utils/mainUtils';
  import bootbox from 'bootbox';

  export default {
    name: 'database',
    props: {
      model: {
        required: true
      }
    },
    mixins: [reusableMixin],
    emits: ['updateModal'],
    data: function () {
      return {
        Model: this.model,
        newDSForm: {
          Name: '',
          Schema: ''
        }
      };
    },
    methods: {
      CreateDataSystem: function () {
        var vm = this;
        vm.newDSForm.Name = ''; vm.newDSForm.Schema = '';
        bootbox.confirm({
            title: vm.Resources.NOVA_BASE_DE_DADOS33819,
            message: vm.$refs.templateFormNewDataSystem,
            callback: function (result) {
                if (result) {
                  QUtils.log("CreateDataSystem - Request", QUtils.apiActionURL('Config', 'CreateDataSystem'));
                  QUtils.postData('Config', 'CreateDataSystem', { year: vm.newDSForm.Name, schema: vm.newDSForm.Schema }, null, function (data) {
                    QUtils.log("CreateDataSystem - Response", data);
                    vm.$router.replace({ name: 'system_setup', params: { culture: vm.currentLang, system: data.system } });
                  });
                }
            }
        });
      },
      SaveConfigDatabase: function () {
        var vm = this;
        QUtils.log("SaveConfigDatabase - Request", QUtils.apiActionURL('Config', 'SaveConfigDatabase'));
        QUtils.postData('Config', 'SaveConfigDatabase', vm.Model, null, function (data) {
          QUtils.log("SaveConfigDatabase - Response", data);
                                        vm.$emit('updateModal', data);
          vm.$eventHub.emit('fetchSysConfig', data);
        });
      }
    }
  };
</script>
