<template>
  <div id="maintenance_index_container">
    <row v-if="!isEmptyObject(Model.ResultMsg)">
      <div class="alert alert-info">
        <span><b class="status-message">{{ Model.ResultMsg }}</b></span>
      </div>
      <br />
    </row>

    <div class="container-fluid">
      <row>
        <text-input v-model="Model.DbUser" :label="Resources.NOME_DE_UTILIZADOR58858" :isRequired="true"></text-input>
      </row>
      <row>
        <password-input v-model="Model.DbPsw" :label="Resources.PALAVRA_PASSE44126" :isRequired="true"></password-input>
      </row>

      <div class="row" style="margin-top: 20px;">
          <!-- Info -->
          <div class="col-md-auto">
              <QGroupBoxContainer :label="Resources.INFO27076" :labelAlign="'center'" :style="'height: 205px;'">
                  <div style="padding: 4px 6px;">
                    <static-text v-model="Model.DBSchema" :label="Resources.BASE_DE_DADOS58234" :bold="true"></static-text>
                  </div>
                  <div style="padding: 4px 6px;">
                    <static-text v-model="Model.DBSize" :label="Resources.TAMANHO_DA_BD56664" :bold="true"></static-text>
                  </div>
              </QGroupBoxContainer>
          </div>

          <!-- Schema -->
          <div class="col-md-auto">
              <QGroupBoxContainer :label="Resources.SCHEMA58822" :labelAlign="'center'" :style="'height: 205px;'">
                  <div style="padding: 4px 6px;">
                    <static-text v-model="Model.VersionDb" :label="Resources.DATABASE_VERSION15344" :bold="true"></static-text>
                  </div>
                  <div style="padding: 4px 6px;">
                    <static-text v-model="Model.VersionApp" :label="Resources.VERSAO_DA_APLICACAO45955" :bold="true"></static-text>
                  </div>
                  <div style="padding: 4px 6px;">
                    <static-text v-model="Model.VersionReIdx" :label="Resources.VERSAO_DOS_SCRIPTS52566" :bold="true"></static-text>
                  </div>
              </QGroupBoxContainer>
          </div>

          <!-- Version -->
          <div class="col-md-auto">
              <QGroupBoxContainer :label="Resources.VERSAO61228" :labelAlign="'center'" :style="'height: 205px;'">
                  <div style="padding: 4px 6px;">
                    <static-text v-model="Model.VersionUpgrIndx" :label="Resources.DATABASE_VERSION15344" :bold="true"></static-text>
                  </div>

                  <div v-if="Model.VersionUpgrScripts !== 0" style="padding: 4px 6px;">
                    <static-text v-model="Model.VersionUpgrScripts" :label="Resources.APPLICATION_VERSION32207" :bold="true"></static-text>
                  </div>
              </QGroupBoxContainer>
          </div>
        </div>
  </div>




    <hr />
    <row>
      <card :isCollapse="true">
        <template #header>
          {{ Resources.OPCOES_AVANCADAS_DE_63606 }}
        </template>
        <template #body>
          <row>
            <checkbox-input v-model="Model.Zero" :label="Resources.REINDEXACAO_COMPLETA51519"></checkbox-input>
          </row>
          <template v-for="GrpItem in Reindexgroups" :key="GrpItem.Id">
            <row>
              <checkbox-input :label="GrpItem.Name" :data-id="GrpItem.Name" :clickData="GrpItem" @click="SwitchGroupCheck"></checkbox-input>
            </row>
            <div style="text-align:left; margin-left: 20px;">
              <template v-for="sqlFunc in getFiltredGroupItems(GrpItem)" :key="sqlFunc.Id">
                <row v-if="sqlFunc.Selectable">
                      <div class="row">
                          <div class="col-6">
                              <checkbox-input v-model="sqlFunc.Value"
                                  :label="sqlFunc.Description"
                                  :id="sqlFunc.Id"></checkbox-input>
                          </div>
                          <div v-if="formatDate(sqlFunc.LastRun) != '-'" class="col-6">
                              <a href="#" @click.prevent.stop="changeSelectedScript(sqlFunc)" :title="sqlFunc.Result">
                                  <span class="glyphicons glyphicons-history"></span>
                                  <span> {{formatDate(sqlFunc.LastRun)}} </span>
                                  <span class="glyphicons glyphicons-hourglass"></span>
                                  <span> {{(sqlFunc.Duration).toLocaleString('en') }} ms </span>

                                  <span v-if="sqlFunc.Result.length > 0" class="glyphicons glyphicons-alert"></span>
                                  <span v-else class="glyphicons glyphicons-ok"></span>
                              </a>
                          </div>
                      </div>
                </row>

              </template>
            </div>
          </template>
        </template>
      </card>
      <row>
        <numeric-input v-model="Model.Timeout" :label="'Timeout'"></numeric-input>
      </row>
    </row>
    <row>
      <q-button
        b-style="primary"
        :label="Resources.EXECUTAR_TAREFAS_DE_40767"
        @click="Reindex" />
    </row>

    <progress-bar :show="dataPB.show" :text="dataPB.text" :progress="dataPB.progress" :withButton="true" :buttonText="Resources.CANCELAR49513" @onButtonClick="cancelReindex"></progress-bar>
    <!-- Modal -->
    <div class="modal fade" id="database_manitenance_script_notes_container" tabindex="-1" role="dialog" aria-labelledby="database_manitenance_script_notes_Title" aria-hidden="true">
        <div v-if="selectedScript" class="modal-dialog modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="database_manitenance_script_notes_Title">{{selectedScript.Description}}</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <div><span class="glyphicons glyphicons-hand-right"></span> {{selectedScript.Origin}}
                        <span class="glyphicons glyphicons-hourglass"></span><span> {{(selectedScript.Duration).toLocaleString('en') }} ms</span>
					</div>
                    <div>{{selectedScript.Result}}</div>
                    <div v-for="files in selectedScript.Details" :key="files.ScriptId">
                        <hr />
                        <b>
                            <span class="glyphicons glyphicons-file"></span>
                            {{files.ScriptId}}
                            <span class="glyphicons glyphicons-hourglass"></span>
                            <span> {{(files.Duration).toLocaleString('en') }} ms </span>
                        </b>
                        <div v-for="block in files.ScriptDetails" :key="block.ScriptId">
                            <span class="glyphicons glyphicons-list"></span>
                            {{block.ScriptId}}
                            <span class="glyphicons glyphicons-hourglass"></span>
                            <span> {{(block.Duration).toLocaleString('en') }} ms </span>
                            <span v-if="block.Result" class="glyphicons glyphicons-alert"></span>
                        </div>
                    </div>
                </div>
            </div>
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
    name: 'maintenance_index',
    mixins: [reusableMixin],
    data: function () {
      return {
        Model: {},
        dataPB: {
          show: false,
          text: '',
          progress: 0
        },
        isCancelled: false,
        selectedScript: null
      };
    },
    computed: {
      Reindexgroups: function () {
        var vm = this;
        if (!$.isEmptyObject(vm.Model) && !$.isEmptyObject(vm.Model.reindexMenu) && !$.isEmptyObject(vm.Model.reindexMenu.Reindexgroups)) {
          return $.grep(vm.Model.reindexMenu.Reindexgroups, function (rg) { return rg.GroupItems.length > 0; });
        }
        return [];
      }
    },
    methods: {
      fetchData: function () {
        var vm = this;
        QUtils.log("Fetch data - Maintenance");
        QUtils.FetchData(QUtils.apiActionURL('DbAdmin', 'Index')).done(function (data) {
          QUtils.log("Fetch data - OK (Maintenance)", data);
          $.extend(vm.Model, data);
        });
      },
      getIndex: function (sqlFunc) {
        var
          vm = this,
          idx = vm.Model.Items.findIndex((RdxFunction) => { return RdxFunction.Id == sqlFunc; });
        return idx;
      },
      getFiltredGroupItems: function (grpItem) {
        var vm = this, gItems = [];
        $.each(grpItem.GroupItems, function (index, sqlFunc) {
          var idx = vm.getIndex(sqlFunc);
          if (idx > -1) { gItems.push(vm.Model.Items[idx]); }
        });

        return gItems;
      },
      SwitchGroupCheck: function (grp) {
	    var vm = this;
        var grpCheckbox = $('[data-id="' + grp.Name + '"]'),
          checked = grpCheckbox.is(':checked');

        $.each(grp.GroupItems, function (i, gID) {
          var idx = vm.getIndex(gID);
          if (idx > -1) {
              vm.Model.Items[idx].Value = checked;
          }
        });
      },
      GetLiteModel: function() {
        //Copy Model
        let tmpModel = Object.assign({}, this.Model);

        //Remove item details
        //The items are the heavy part of the array, but we
        //Cant get rid of them, otherwise reindexation won't work
        //Instead we just remove their details, which are heavy and basically useless
        tmpModel.Items.forEach(e => {
            //delete e.Result;
            delete e.Details;
        });

        return tmpModel;
      },
      Reindex: function () {
        var vm = this;
        QUtils.log("Request", QUtils.apiActionURL('DbAdmin', 'Start'));
        vm.dataPB.text = "Discovering database";
        vm.dataPB.progress = 2;

        if (vm.Model.Items.filter(e => e.Value === true && e.Selectable === true).length === 0) {
            bootbox.alert("No reindex items were checked.");
            return;
        }

        vm.isCancelled = false
        
        QUtils.postData('DbAdmin', 'Start', this.GetLiteModel(), null, function (data) {
          QUtils.log("Response", data)
            if(!data.Success) {
              bootbox.alert(data.Message)
              return
            }

            vm.dataPB.show = true;
            setTimeout(vm.checkProgress, 250);
        });
      },

      //Just for Genio application
      NewProject: function () {
        var vm = this;
        QUtils.log("Request", QUtils.apiActionURL('DbAdmin', 'Start'));
        vm.dataPB.text = "Discovering database";
        vm.dataPB.progress = 2;
        vm.dataPB.show = true;
        vm.dataPB.withButton = false;

        QUtils.postData('DbAdmin', 'Start', this.GetLiteModel(), null, function (data) {
            QUtils.log("Response", data);
            if (data.Completed && !$.isEmptyObject(data.Log)) {
                vm.dataPB.show = false;
                bootbox.alert(data.Log);
            }
            else if (!data.Completed) {
                setTimeout(()=> vm.checkProgress(vm.FillProjectData), 250);
            }
        });
      },

      FillProjectData: function () {
        var vm = this;
        QUtils.log("Request", QUtils.apiActionURL('DbAdmin', 'FillNewProjectData'));
        vm.dataPB.text = 'Fill new project data';
        vm.dataPB.progress = 100;

        QUtils.postData('DbAdmin', 'FillNewProjectData', this.Model, null, (data) => {
            vm.dataPB = { show: false, text: '', progress: 0, inProcess: false };
            vm.fetchData()
            bootbox.alert(data.message);
        });
      },

      checkProgress: function (callBack, firstCheck = false) {
        var vm = this;

        QUtils.FetchData(QUtils.apiActionURL('DbAdmin', 'Progress')).done(function (data) {
          if (data.Status == 'RUNNING') {
            if(!vm.isCancelled) {
              vm.dataPB.text = "Script: " + data.ActualScript
              vm.dataPB.progress = data.Count
              vm.dataPB.show = true
            }
            setTimeout(()=>vm.checkProgress(callBack), 500)
            return
          }

          if(firstCheck) return

          if(data.Status == 'CANCELLED') {
            vm.dataPB.show = false
            bootbox.alert(vm.Resources.OPERATION_CANCELLED_59653)
            vm.fetchData();
            return
          }
          if (callBack) {
            callBack()
            return
          }

          if (!$.isEmptyObject(data.Message)) {
              bootbox.alert(data.Message);                      
          }
          vm.dataPB = { show: false, text: '', progress: 0, inProcess: false };
          vm.fetchData();
        });
      },

      changeSelectedScript: function (sqlFunc) {
        if (sqlFunc) {
          this.selectedScript = sqlFunc;
          this.$nextTick(function () { $('#database_manitenance_script_notes_container').modal('show'); });
        }
        else {
          this.selectedScript = sqlFunc;
          this.$nextTick(function () { $('#database_manitenance_script_notes_container').modal('hide'); });
        }
      },

      cancelReindex: function () {
        var vm = this;
        QUtils.log("Request", QUtils.apiActionURL('DbAdmin', 'CancelReindex'));
        QUtils.FetchData(QUtils.apiActionURL('dbadmin', 'CancelReindex')).done(function (data) {
          QUtils.log("Request - OK (Maintenance - Cancel Reindexation)", data);
          if(!data.Success) {
            bootbox.alert(vm.Resources.THERE_HAS_BEEN_AN_ER33167 + ":<br />" 
            + data.Message);
            return
          }

          //Show user that the cancel operation is running
          vm.dataPB.text = 'Cancelling...'
          vm.dataPB.progress = 100
          vm.isCancelled = true
        });
      }
    },
    mounted: function () {
      // Read data
      this.fetchData()

      // Check current progress
      this.checkProgress(null, true)
    },
    watch: {
      // call again the method if the route changes
      '$route': 'fetchData'
    }
  };
</script>
