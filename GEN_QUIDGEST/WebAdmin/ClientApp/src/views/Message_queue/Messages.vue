<template>
  <div id="message_queue_messages_container">
      <row>
          <br />
          <row>
              <select-input v-model="statusType" :options="statusTypes" :label="Resources.STATUS62033"></select-input>
              <select-input v-model="queueType" :options="queueTypes" :label="Resources.QUEUE45251"></select-input>
          </row>

          <qtable :rows="tQueuesMSG.rows"
                  :columns="tQueuesMSG.columns"
                  :config="tQueuesMSG.config"
                  @on-change-query="onChangeQuery"
                  :totalRows="tQueuesMSG.total_rows">
          </qtable>
      </row>
  </div>
</template>

<script>
    // @ is an alias to /src
    import { reusableMixin } from '@/mixins/mainMixin';
    import { QUtils } from '@/utils/mainUtils';
    //import bootbox from 'bootbox';

    export default {
        name: 'message_queue_messages',
        mixins: [reusableMixin],
        props: {
            Model: {
                required: true
            }
        },
        data: function () {
            var vm = this;
            return {
                statusType: '',
                statusTypes: [
                    { Text: ' ', Value: '' },
                    { Text: 'Send fail', Value: '0' },
                    { Text: 'Send in Progress', Value: '1' },
                    { Text: 'Send expired', Value: '2' },
                    { Text: 'Reply OK', Value: '3' },
                    { Text: 'Reply Reject', Value: '4' },
                    { Text: 'Reply Fail', Value: '5' },
                    { Text: 'SendReply OK', Value: '6' }
                ],
                queueType: '',
                queueTypes: [
                    { Text: ' ', Value: '' }
                ],
                tQueuesMSG: {
                    rows: [],
                    total_rows: 0,                    
                    columns: [
                        {
                            label: () => vm.$t('ACOES22599'),
                            name: "actions",
                            slot_name: "actions",
                            sort: false,
                            column_classes: "thead-actions",
                            row_text_alignment: 'text-center',
                            column_text_alignment: 'text-center'
                        },
                        {
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
                        table_title: () => vm.$t('LISTA_DE_MENSAGENS31887'),
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
                        page: 1,
                        queue: ''
                    }
                }
            };
        },
        methods: {
            fetchData: function () {
                var vm = this;
                QUtils.log("Fetch data - GetQueueMSG");
                var params = $.extend({}, vm.tQueuesMSG.queryParams, { status: vm.statusType, queue: vm.queueType });
                QUtils.FetchData(QUtils.apiActionURL('MessageQueue', 'GetQueueMSG', params)).done(function (data) {
                    QUtils.log("Fetch data - OK (GetQueueMSG)", data);
                    if (data.Success) {
                        vm.tQueuesMSG.total_rows = data.recordsTotal;
                        vm.tQueuesMSG.rows = data.data;
                    }
                    else {
                        vm.tQueuesMSG.total_rows = 0;
                        vm.tQueuesMSG.rows = [];
                    }
                });
            },
            onChangeQuery: function (queryParams) {
                var vm = this;
                $.each(queryParams, function (propName, value) { vm.tQueuesMSG.queryParams[propName] = value; });
                vm.fetchData();
            },
            fillQueueTypes: function () {
                var vm = this, acks = [];
                $.each(vm.Model.MQueues.Queues, function (index, q) {
                    vm.queueTypes.push({ Text: q.queue, Value: q.queue });
                });
                $.each(vm.Model.MQueues.Acks, function (index, ack) {
                    acks.push(ack.ackQueue);
                });
                acks = $.grep(acks, function (el, index) {
                    return index === $.inArray(el, acks);
                });
                $.each(acks, function (index, ack) {
                    vm.queueTypes.push({ Text: ack, Value: ack });
                });
            }
        },
        created: function () {
            // Ler dados
            this.fetchData();
            this.fillQueueTypes();
        },
        watch: {
            'statusType': 'fetchData',
            'queueType': 'fetchData'
        }
    };
</script>
