<template>
  <div id="audit_viewer_container">
    <div class="q-stack--column">
        <h1 class="f-header__title">
        {{ Resources.AUDITORIA_DO_SISTEMA08460 }}
        </h1>
    </div>
    <hr>

      <row v-if="!isEmptyObject(Model.ResultMsg)">
          <div class="alert alert-danger">
              <span>
                  <b class="status-message">{{ Model.ResultMsg }}</b>
              </span>
          </div>
          <br />
      </row>
      <row>
        <row v-if="!Model.LockTable">
            <select-input v-model="Model.LogTable" v-if="Model.SelectLists" :options="Model.SelectLists.LogTables" :label="Resources.SELECIONE_A_TABELA_A28300" style="width: 350px;"></select-input>
        </row>
        <row>
        <template v-if="Model.LogDatabaseExists">
            <radio-input v-model="Model.LogDatabaseSelected"
                        :options="logDatabase"
                        :label="Resources.DADOS_43180"></radio-input>
            <select-simple v-if="Model.LogTable == logTables.logPROall"
                            :side="'left'"
                            class="float-right"
                            :options="transferLogOptions"
                            :showValue="false"
                            :staticText="Resources.TRANSFERIR_DADOS_PAR38484"
                            @update:modelValue="TransferLog"></select-simple>
        </template>
        <select-simple class="float-right"
                        :side="'left'"
                        :options="logExportType"
                        :showValue="false"
                        :staticText="Resources.EXPORTAR35632"
                        @update:modelValue="LogExport"></select-simple>
        </row>
      </row>
      <br />
      <row>
          <qtable :rows="tAudit.rows"
                  :columns="tAudit.columns"
                  :config="tAudit.config"
                  @on-change-query="onChangeQuery"
                  :totalRows="tAudit.total_rows"
                  @on-single-select-row="handleSingleSelect"
                  :enableExport="true">
          </qtable>
      </row>
      <row>
          <template v-if="Model && !isEmptyObject(Model.SelectedRow)">
			  <row>
				  <h3>{{ Resources.VALORES_CURRENTES_21555 }}</h3>
			  </row>
              <template v-for="(column, index) in Model.Result.ColumnNames" :key="column">
                  <dl class="row" v-if="index >= 4">
                      <dt v-bind:class="style.dtClass">{{ column }}</dt>
                      <dd v-bind:class="style.ddClass" v-if="isEmptyObject(Model.SelectedRowValues) || isEmptyObject(Model.SelectedRowValues[index-3])">
                        <span class="glyphicons glyphicons-minus"></span>
                      </dd>
                      <dd v-bind:class="style.ddClass" v-else>{{ Model.SelectedRowValues[index-3] }}</dd>
                  </dl>
              </template>
          </template>
      </row>
  </div>
  <progress-bar :show="dataPB.show" :text="dataPB.text" :progress="dataPB.progress"></progress-bar>
</template>

<script>
  // @ is an alias to /src
  import { reusableMixin } from '@/mixins/mainMixin.js';
  import { QUtils } from '@/utils/mainUtils';
  import bootbox from 'bootbox';
  import { string_format } from '@/utils/globalUtils';

    export default {
        name: 'audit_viewer',
        mixins: [reusableMixin],
        data: function () {
            var vm = this;
            return {
                Model: {
                    LogTable: 0,
                    LogDatabaseSelected: false
                },
                dataPB: {
                    show: false,
                    text: vm.$t('TRANSFERING_LOGS___44991'),
                    progress: 100
                },
                logDatabase: [
                    { Text: vm.$t('ATUAIS07948'), Value: false },
                    { Text: vm.$t('HISTORICO16073'), Value: true }
                ],
                logTables: {
                    logPROall: 0,
                },
                logExportType: [
                    { Value: 'pdf', Text: vm.$t('FORMATO_DE_DOCUMENTO48724') },
                    { Value: 'ods', Text: vm.$t('FOLHA_DE_CALCULO__OD46941') },
                    { Value: 'xlsx', Text: vm.$t('FOLHA_DE_CALCULO_EXC59518') },
                    { Value: 'csv', Text: vm.$t('VALORES_SEPARADOS_PO10397') },
                    { Value: 'xml', Text: vm.$t('FORMATO_XML__XML_44251') }
                ],
                tAudit: {
                    rows: [],
                    total_rows: 0,
                    columns: vm.getColumns(),
                    config: {
                        global_search: {
                            classes: "qtable-global-search",
                            searchOnPressEnter: true,
							showRefreshButton: true,
                            //searchDebounceRate: 1000
                        },
                        server_mode: true,
                        preservePageOnDataChange: true,
                        single_row_selectable: true
                    },
                    queryParams: {
                        sort: [],
                        filters: [],
                        global_search: "",
                        per_page: 10,
                        page: 1
                    }
                },
                style: {
                    dtClass: 'col-sm-2 textRight',
                    ddClass: 'col-sm-10'
                },
                curSelectedRowIndex: ''
            };
        },
        computed: {
            transferLogOptions: function () {
                var vm = this,
                    options = [
                        { Text: vm.Resources.TRANSFERIR_TODOS48543, Value: true }
                    ];

                if (vm.Model.MaxLogRowDays > 0) {
                    options.push({
                        Text: string_format(vm.Resources.TRANSFERIR_DADOS_COM36042, vm.Model.MaxLogRowDays), Value: false
                    });
                }

                return options;
            }
        },
        methods: {
            fetchData: function () {
                var vm = this, params = {};
                params.param = vm.tAudit.queryParams;
                params.logTable = vm.Model.LogTable;
                params.logDatabase = vm.Model.LogDatabaseSelected;
                params.export = false;
                params.exportType = '';
                params.rowSelected = vm.Model.SelectedRow;

                QUtils.postData('AuditViewer', 'Reload', params, null, function (data) {
                    $.each(data, function (propName, value) { vm.Model[propName] = value; });
                    vm.tAudit.columns = vm.getColumns();
                    vm.tAudit.rows = [];
                    if (vm.Model && vm.Model.Result && vm.Model.Result.Values) {
                        $.each(vm.Model.Result.Values, function (index, resValue) {
                            var obj = {};
                            $.each(resValue.ColumnValues, function (name, value) { obj[name.toString()] = value });
                            vm.tAudit.rows.push(obj);
                        });
                    }

                    vm.tAudit.total_rows = vm.Model.TotalRows;
                });
            },
            getColumns: function () {
                var columns = [],
                    vm = this;

                if (vm.Model && vm.Model.Result && vm.Model.Result.ColumnNames) {
                    $.each(vm.Model.Result.ColumnNames, function (index, columnName) {
                        var col = {
                            label: columnName,
                            name: index.toString(),
                            sort: true
                        };
                        columns.push(col);
                    });
                }

                return columns;
            },
            onChangeQuery: function (queryParams) {
                this.tAudit.queryParams = queryParams;
                this.fetchData();
            },
            handleSingleSelect: function (eventData) {
                var id = eventData.row[3];
                if (this.curSelectedRowIndex == eventData.rowIndex) { this.unSelectRow(); }
                else { this.SelectRow(id, eventData.rowIndex); }
            },
            SelectRow: function (id, rowIndex) {
                if (this.Model.LogTable != 0) {
                    this.curSelectedRowIndex = rowIndex;
                    this.Model.SelectedRow = id;
                    this.DataSubmit();
                }
            },
            unSelectRow: function () {
                this.curSelectedRowIndex = '';
                this.Model.SelectedRow = '';
            },
            DataSubmit: function () {
                var vm = this;
                QUtils.postData('AuditViewer', 'Refresh', vm.Model, null, function (data) {
                    $.each(data, function (propName, value) { vm.Model[propName] = value; });
                });
            },
            RunExport: function (exportType) {
                // var msg = vm.Resources.A_EXPORTAR___20494;.
                var vm = this, params = {};
                params.param = vm.tAudit.queryParams;
                params.logTable = vm.Model.LogTable;
                params.logDatabase = vm.Model.LogDatabaseSelected;
                params.export = true;
                params.exportType = exportType;
                params.rowSelected = vm.Model.SelectedRow;

                QUtils.postData('AuditViewer', 'Reload', params, null, function (data) {
                    if (data.Success) {
                        var downloadUrl = QUtils.apiActionURL('AuditViewer', 'downloadExportFile', { id: data.fileId, type: exportType });
                        window.open(downloadUrl, "_self");
                    } else {
                        bootbox.alert(data.Message);
                    }
                });
            },
            LogExport: function (exportType) {
                var vm = this;
                if (vm.tAudit.total_rows > 5000) {
                    bootbox.confirm(string_format(vm.Resources.O_NUMERO_DE_REGISTOS44061, 5000), function (result) {
                        if (result) { vm.RunExport(exportType); }
                    });
                }
                else {
                    vm.RunExport(exportType);
                }
            },
            TransferLog: function (all) {
                var vm = this;
                QUtils.postData('AuditViewer', 'TransferLog', null, { transferAll: all }, function (data) { 
                    vm.dataPB.show = false;
                    setTimeout(vm.checkProgress(data.RequestId), 250);
                });
            },
            checkProgress: function (requestId) {
                const vm = this;

                QUtils.FetchData(QUtils.apiActionURL('AuditViewer', 'Progress', { requestId })).done(function (data) {
                    if (!data.Completed) {
                        vm.dataPB.text = data.Message;
                        vm.dataPB.progress = data.Progress;
                        vm.dataPB.show = true;
                        setTimeout(() => vm.checkProgress(requestId), 500);
                    }
                    else {
                        vm.dataPB = {
                            show: false,
                            text: '',
                            progress: 0,
                            inProcess: false
                        };
						
						bootbox.alert(data.Message, () => vm.fetchData());
                    }
                });
            },
        },
        watch: {
            // call again the method if the route changes
            '$route': 'fetchData',
            'Model.LogTable': function (newValue, oldValue) {
                if (newValue != oldValue) { this.Model.SelectedRow = ''; this.fetchData(); }
            },
            'Model.LogDatabaseSelected': function (newValue, oldValue) {
                if (newValue != oldValue) { this.Model.SelectedRow = ''; this.fetchData(); }
            }
        }
    };
</script>
