<template>
  <div id="message_queue_history_container">
      <row>
          <br />
          <row>
              <select-input v-model="queryParamQueue" :options="queuesFilter" :label="Resources.QUEUE45251"></select-input>
          </row>
          <row>
              <q-button
                b-style="primary"
                label="Arquivar"
                @click="Arquivar" />
          </row>

          <qtable :rows="tQueuesSend.rows"
                  :columns="tQueuesSend.columns"
                  :config="tQueuesSend.config"
                  @on-change-query="onChangeQuerySend"
                  :totalRows="tQueuesSend.total_rows">
          </qtable>
      </row>

      <br />
      <row v-if="Model.LogDatabaseExists">
          <radio-input :value="Model.LogDatabaseSelected"
                       :options="logDatabase"
                       :label="Resources.DADOS_43180"></radio-input>
          <q-button
            :label="Resources.TRANSFERIR_DADOS_PAR38484"
            @click="exportDataToHistory" />
      </row>

      <row>
          <qtable :rows="tQueuesHist.rows"
                  :columns="tQueuesHist.columns"
                  :config="tQueuesHist.config"
                  @on-change-query="onChangeQueryHist"
                  :totalRows="tQueuesHist.total_rows">
              <template #actions="props">
                  <div elem-identifier="BtnGroup">
                    <button class="b-icon b-icon--secondary dropdown" data-toggle="dropdown" title="Actions" aria-expanded="true">
                      <i class="glyphicons glyphicons-option-horizontal e-icon"></i>
                    </button>
                    <div class="pull-left dropdown-menu" x-placement="bottom-start" style="position: absolute; transform: translate3d(4px, 100px, 0px); top: 0px; left: 0px; will-change: transform;">
                        <a class="dropdown-item" @click.stop="ProcessBTClick(props.row, 'SEND')"><i class="glyphicons glyphicons-eye-open dropdown__icon"></i>Reenviar</a>
                        <a class="dropdown-item" @click.stop="ProcessBTClick(props.row, 'END')"><i class="glyphicons glyphicons-eye-open dropdown__icon"></i>Finalizar</a>
                    </div>
                  </div>
              </template>
          </qtable>
      </row>
  </div>
</template>

<script>
    // @ is an alias to /src
    import { reusableMixin } from '@/mixins/mainMixin';
    import { QUtils } from '@/utils/mainUtils';
    import bootbox from 'bootbox';

    export default {
        name: 'message_queue_history',
        mixins: [reusableMixin],
        props: {
            Model: {
                required: true
            }
        },
        data: function () {
            var vm = this;
            return {
                statusType: [
                    { Text: ' ', Value: '' },
                    { Text: 'Send fail', Value: '0' },
                    { Text: 'Send in Progress', Value: '1' },
                    { Text: 'Send expired', Value: '2' },
                    { Text: 'Reply OK', Value: '3' },
                    { Text: 'Reply Reject', Value: '4' },
                    { Text: 'Reply Fail', Value: '5' },
                    { Text: 'SendReply OK', Value: '6' }
                ],
                queuesFilter: [
                    { Text: ' ', Value: '' }
                ],
                logDatabase: [
                    { Text: vm.$t('ATUAIS07948'), Value: false },
                    { Text: vm.$t('HISTORICO16073'), Value: true }
                ],
                tQueuesSend: {
                    rows: [],
                    total_rows: 0,
                    columns: [{
                        label: () => vm.$t('QUEUE45251'),
                        name: "1",
                        sort: true,
                        initial_sort: true,
                        initial_sort_order: "asc"
                    },
                    {
                        label: () => vm.$t('TABELA44049'),
                        name: "2",
                        sort: true
                    },
                    {
                        label: () => vm.$t('TABELA44049') + ' PK',
                        name: "3",
                        sort: true
                    },
                    {
                        label: () => vm.$t('STATUS62033'),
                        name: "4",
                        sort: true
                    },
                    {
                        label: () => vm.$t('DATA_STATUS47877'),
                        name: "5",
                        sort: true
                    },
                    {
                        label: () => vm.$t('RESPOSTA62248'),
                        name: "6",
                        sort: true
                    },
                    {
                        label: () => vm.$t('NO_DE_ENVIOS17305'),
                        name: "7",
                        sort: true
                    },
                    {
                        label: () => vm.$t('CRIADO_EM61283'),
                        name: "8",
                        sort: true
                    }],
                    config: {
                        table_title: () => vm.$t('MESAGENS_A_ENVIAR_PA25147'),
                        global_search: {
                            classes: "qtable-global-search",
                            searchOnPressEnter: true,
							showRefreshButton: true,
                            //searchDebounceRate: 1000
                        },
                        server_mode: true,
                        preservePageOnDataChange: true
                    },
                    queryParams: {
                        sort: [{ name: '1', order: 'asc' }],
                        filters: [],
                        global_search: "",
                        per_page: 10,
                        page: 1
                    }
                },
                tQueuesHist: {
                    rows: [],
                    total_rows: 0,
                    columns: [{
                        label: () => vm.$t('QUEUE45251'),
                        name: "1",
                        sort: true,
                        initial_sort: true,
                        initial_sort_order: "asc"
                    },
                    {
                        label: () => vm.$t('TABELA44049'),
                        name: "2",
                        sort: true
                    },
                    {
                        label: () => vm.$t('TABELA44049') + ' PK',
                        name: "3",
                        sort: true
                    },
                    {
                        label: () => vm.$t('STATUS62033'),
                        name: "4",
                        sort: true
                    },
                    {
                        label: () => vm.$t('DATA_STATUS47877'),
                        name: "5",
                        sort: true
                    },
                    {
                        label: () => vm.$t('RESPOSTA62248'),
                        name: "6",
                        sort: true
                    },
                    {
                        label: () => vm.$t('NO_DE_ENVIOS17305'),
                        name: "7",
                        sort: true
                    },
                    {
                        label: () => vm.$t('CRIADO_EM61283'),
                        name: "8",
                        sort: true
                    }],
                    config: {
                        table_title: () => vm.$t('MESAGENS_DE_HISTORIA25765'),
                        global_search: {
                            classes: "qtable-global-search",
                            searchOnPressEnter: true,
							showRefreshButton: true,
                            //searchDebounceRate: 1000
                        },
                        server_mode: true,
                        preservePageOnDataChange: true
                    },
                    queryParams: {
                        sort: [{ name: '1', order: 'asc' }],
                        filters: [],
                        global_search: "",
                        per_page: 10,
                        page: 1
                    }
                },
                queryParamQueue: ''
            };
        },
        computed: {
        },
        methods: {
            fetchDataSend: function () {
                var vm = this;
                var params = $.extend({}, vm.tQueuesSend.queryParams, { queue: vm.queryParamQueue });
                QUtils.log("Fetch data - Message Queue - History (A)");
                QUtils.FetchData(QUtils.apiActionURL('MessageQueue', 'GetQueueHistory', params)).done(function (data) {
                    QUtils.log("Fetch data - OK (Message Queue - History (A))", data);
                    if (data.Success) {
                        vm.tQueuesSend.rows = data.data;
                        vm.tQueuesSend.total_rows = data.recordsTotal;
                    }
                    else {
                        vm.tQueuesSend.rows = [];
                        vm.tQueuesSend.total_rows = 0;
                    }
                });
            },
            fetchDataHist: function () {
                var vm = this;
                var params = $.extend({}, vm.tQueuesHist.queryParams, { LogHistory: vm.Model.LogDatabaseSelected, queue: vm.queryParamQueue });
                QUtils.log("Fetch data - Message Queue - History (B)");
                QUtils.FetchData(QUtils.apiActionURL('MessageQueue', 'GetHistory', params)).done(function (data) {
                    QUtils.log("Fetch data - OK (Message Queue - History (B))", data);
                    if (data.Success) {
                        vm.tQueuesHist.rows = data.data;
                        vm.tQueuesHist.total_rows = data.recordsTotal;
                    }
                    else {
                        vm.tQueuesHist.rows = [];
                        vm.tQueuesHist.total_rows = 0;
                    }
                });
            },
            onChangeQuerySend: function (queryParams) {
                var vm = this;
                $.each(queryParams, function (propName, value) { vm.tQueuesSend.queryParams[propName] = value; });
                this.fetchDataSend();
            },
            onChangeQueryHist: function (queryParams) {
                var vm = this;
                $.each(queryParams, function (propName, value) { vm.tQueuesHist.queryParams[propName] = value; });
                this.fetchDataHist();
            },
            fillQueuesFilter: function() {
                var vm = this, acks = [];
                $.each(vm.Model.MQueues.Queues, function(index, q) {
                    vm.queuesFilter.push({ Text: q.queue, Value: q.queue });
                });
                $.each(vm.Model.MQueues.Acks, function (index, ack) {
                    acks.push(ack.ackQueue);
                });
                acks = $.grep(acks, function (el, index) {
                    return index === $.inArray(el, acks);
                });
                $.each(acks, function (index, ack) {
                    vm.queuesFilter.push({ Text: ack, Value: ack });
                });
            },
            Arquivar: function () {
                var vm = this;
                bootbox.confirm({
                    message: vm.Resources.DESEJA_MESMO_ARQUIVA56996,
                    buttons: {
                        confirm: {
                            label: vm.Resources.SIM28552,
                            className: 'btn-success'
                        },
                        cancel: {
                            label: vm.Resources.NAO06521,
                            className: 'btn-danger'
                        }
                    },
                    callback: function (result) {
                        if (result) {
                            QUtils.postData('MessageQueue', 'QueueProcessArquive', null, { queue: vm.queryParamQueue }, function (data) {
                                QUtils.log("QueueProcessArquive - Response", data);
                                if (data.status != "E") {
                                    vm.fetchDataHist();
                                }
                                else {
                                    bootbox.alert(data.msg);
                                }
                            });
                        }
                    }
                });
            },
            exportDataToHistory: function () {
                QUtils.FetchData(QUtils.apiActionURL('MessageQueue', 'exportDataToHistory')).done(function (data) {
                    bootbox.alert("The process started successfully.");
                });
            },
            ProcessBTClick: function(row, accao) {
                var vm = this;
                QUtils.postData('MessageQueue', 'QueueProcessActions', null, { action: accao, key: row[9] }, function (data) {
                     if (data.status != "E") {
                        vm.fetchDataHist();
                    }
                    else {
                        bootbox.alert(data.msg);
                    }
                });
            },
            reloadTables: function() {
                this.fetchDataSend();
                this.fetchDataHist();
            }
        },
        created: function () {
            // Ler dados
            this.fillQueuesFilter();
            this.fetchDataSend();
            this.fetchDataHist();
        },
        watch: {
            'queryParamQueue': function () {
                this.reloadTables();
            },
            'logDatabase': function () {
                this.reloadTables();
            },
            'Model.LogDatabaseSelected': function () {
                this.reloadTables();
            }
        }
    };
</script>
