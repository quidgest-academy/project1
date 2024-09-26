<template>
  <div id="maintenance_security_container">
    <row v-if="!isEmptyObject(Model.ResultMsg)">
      <div class="alert alert-info">
        <span><b class="status-message">{{ Model.ResultMsg }}</b></span>
      </div>
      <br />
    </row>

    <row>
      <text-input v-model="Model.DbUser" :label="Resources.NOME_DE_UTILIZADOR58858"></text-input>
    </row>
    <row>
      <password-input v-model="Model.DbPsw" :label="Resources.PALAVRA_PASSE44126"></password-input>
    </row>
    <hr />

    <row>
      <card>
        <template #header>
          Transparent Data Encryption
        </template>
        <template #body>
          <row>
            <password-input v-model="Model.MasterPsw" :label="Resources.CHAVE_MESTRA09773"></password-input>
          </row>
          <row>
            <select-input v-model="Model.Encryption" v-if="Model.SelectLists" :options="Model.SelectLists.DisplayEncrypt" :label="Resources.ALGORITMO_DE_ENCRIPT09649"></select-input>
          </row>
          <row>
            <checkbox-input v-model="Model.MasterKey" :label="Resources.CRIACAO_DA_CHAVE_MES19380"></checkbox-input>
          </row>
          <row>
            <div class="q-button-container">
              <q-button
                b-style="primary"
                :label="Resources.APLICAR33981"
                @click="SaveTDE" />
              <q-button
                :label="Resources.STATUS62033"
                @click="CheckStatusTDE" />
            </div>
          </row>
        </template>
      </card>
    </row>
  </div>
</template>

<script>
  // @ is an alias to /src
  import { reusableMixin } from '@/mixins/mainMixin';
  import { QUtils } from '@/utils/mainUtils';

  export default {
    name: 'maintenance_security',
    mixins: [reusableMixin],
    data: function () {
      return {
        Model: {}
      };
    },
    methods: {
      fetchData: function () {
        var vm = this;
        QUtils.log("Fetch data - Maintenance - Secuity");
        QUtils.FetchData(QUtils.apiActionURL('DbAdmin', 'Security')).done(function (data) {
          QUtils.log("Fetch data - OK (Maintenance - Secuity)", data);
          $.each(data, function (propName, value) { vm.Model[propName] = value; });
        });
      },
      SaveTDE: function () {
        this.submitAction('SaveTDE');
      },
      CheckStatusTDE: function () {
        this.submitAction('CheckStatusTDE');
      },
      submitAction: function (action) {
        var vm = this;
        vm.showPB = true;
        QUtils.log("Request", QUtils.apiActionURL('DbAdmin', action));
        QUtils.postData('DbAdmin', action, vm.Model, null, function (data) {
          QUtils.log("Response", data);
          $.each(data, function (propName, value) { vm.Model[propName] = value; });
          vm.showPB = false;
          vm.Model.BackupItem = null;
        });
      }
    },
    created: function () {
      // Ler dados
      this.fetchData();
    },
    watch: {
      // call again the method if the route changes
      '$route': 'fetchData'
    }
  };
</script>
