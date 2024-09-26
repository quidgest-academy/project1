<template>
  <div id="maintenance_change_year_container">
    <row v-if="!isEmptyObject(Errors)">
      <div class="alert alert-info">
        <span><b class="status-message" v-for="msg in Errors" :key="msg">{{ msg }}</b></span>
      </div>
      <br />
    </row>

    <row>
      <text-input v-model="Model.DbUser" :label="Resources.NOME_DE_UTILIZADOR58858" :isRequired="true"></text-input>
    </row>
    <row>
      <password-input v-model="Model.DbPsw" :label="Resources.PALAVRA_PASSE44126" :isRequired="true"></password-input>
    </row>
    <row v-if="Model.Years">
      <select-input v-model="Model.SrcYear" :options="Model.Years" :label="'Source database'"></select-input>
    </row>
    <row>
      <text-input v-model="Model.NewDBSchema" :label="Resources.NOME_DA_BASE_DE_DADO06329"></text-input>
    </row>
    <row>
      <checkbox-input v-model="Model.CriarBD" :label="Resources.CRIAR_A_BASE_DE_DADO55641"></checkbox-input>
    </row>
    <row>
      <text-input v-model="Model.Year" :label="Resources.ANO33022"></text-input>
    </row>
    <row>
      <text-input v-model="Model.DirFilestream" :label="Resources.DIRETORIA_DE_FILESTR39886"></text-input>
    </row>
    <row>
      <numeric-input v-model="Model.Timeout" :label="'Timeout'"></numeric-input>
    </row>

    <row>
      <q-button
        b-style="primary"
        :label="Resources.INICIAR_O_PROCESSO_D34168"
        @click="changeYear" />
    </row>

    <progress-bar :show="dataPB.show" :text="dataPB.text" :progress="dataPB.progress"></progress-bar>
  </div>
</template>

<script>
  // @ is an alias to /src
  import { reusableMixin } from '@/mixins/mainMixin';
  import { QUtils } from '@/utils/mainUtils';
  import bootbox from 'bootbox';

  export default {
    name: 'maintenance_change_year',
    mixins: [reusableMixin],
    data: function () {
      return {
        Model: {},
        Errors: [],
        dataPB: {
          show: false,
          text: '',
          progress: 0,
          inProcess: false
        }
      };
    },
    methods: {
      fetchData: function () {
        var vm = this;
        QUtils.log("Fetch data - Maintenance - Change Year");
        QUtils.FetchData(QUtils.apiActionURL('DbAdmin', 'ChangeYear')).done(function (data) {
          QUtils.log("Fetch data - OK (Maintenance - Change Year)", data);
          $.each(data.Model, function (propName, value) { vm.Model[propName] = value; });
          vm.Errors = data.Errors;
        });
      },
      changeYear: function () {
        var vm = this;
        QUtils.log("Request", QUtils.apiActionURL('DbAdmin', 'ChangeYear'));
        QUtils.postData('DbAdmin', 'ChangeYear', vm.Model, null, function (data) {
          QUtils.log("Response", data);
          $.each(data.Model, function (propName, value) { vm.Model[propName] = value; });
          vm.Errors = data.Errors;
          setTimeout(vm.checkProgress, 250);
        });
      },
      checkProgress: function () {
        // This function is the callback function which is executed in every 350 milli seconds.
        // Until the ajax call is success this method is invoked and the progressbar value is changed.
        var vm = this;
        QUtils.FetchData(QUtils.apiActionURL('DbAdmin', 'CheckChangeYearProgress')).done(function (data) {
          if (data.InProcess) {
            vm.dataPB.inProcess = true;
            vm.dataPB.text = data.Text;
            vm.dataPB.progress = data.Percent;
            vm.dataPB.show = true;
            setTimeout(vm.checkProgress, 250);
          }
          else {
            if (vm.dataPB.inProcess && !$.isEmptyObject(data.EndMsg)) {
              bootbox.alert(data.EndMsg);
            }
            vm.dataPB = {
              show: false,
              text: '',
              progress: 0,
              inProcess: false
            };
            vm.$eventHub.emit('app_updateYear');
          }
        });

      }
    },
    mounted: function () {
      setTimeout(this.checkProgress, 250);
    },
    created: function () {
      // Ler dados
      this.fetchData();
    }
  };
</script>
