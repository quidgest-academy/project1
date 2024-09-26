<template>
  <div id="message_queue_queues_container">
      <row>
          <br />
          <qtable :rows="tQueues.rows"
                  :columns="tQueues.columns"
                  :config="tQueues.config"
                  :totalRows="tQueues.total_rows">

              <template #actions="props">
                  <q-button
                    borderless
                    :title="Resources.EXPORTAR35632"
                    @click="exportQueue(props.row)">
                    <q-icon icon="share" />
                  </q-button>
              </template>
          </qtable>
      </row>
      <!-- Modal -->
      <export_queue :Model="exportForm.data" :show="exportForm.show" @close="closeExportQueue"></export_queue>
  </div>
</template>

<script>
  // @ is an alias to /src
  import { reusableMixin } from '@/mixins/mainMixin';
  //import { QUtils } from '@/utils/mainUtils';
  import export_queue from './ExportQueue.vue';

    export default {
        name: 'message_queue_queues',
        mixins: [reusableMixin],
        components: { export_queue },
        props: {
            Model: {
                required: true
            }
        },
        data: function () {
            var vm = this;
            return {
                exportForm: {
                    show: false,
                    data: {}
                },
                tQueues: {
                    rows: [],
                    total_rows: 0,
                    columns: [{
                        label: () => vm.$t('ACOES22599'),
                        name: "actions",
                        slot_name: "actions",
                        sort: false,
                        column_classes: "thead-actions",
                        row_text_alignment: 'text-center',
                        column_text_alignment: 'text-center'
                    },
                    {
                        label: () => vm.$t('NOME_DA_QUEUE56594'),
                        name: "queue",
                        sort: true,
                        initial_sort: true,
                        initial_sort_order: "asc"
                    },
                    {
                        label: () => vm.$t('TRAJETO_DA_QUEUE07185'),
                        name: "path",
                        sort: true
                    },
                    {
                        label: () => vm.$t('ANO33022'),
                        name: "Qyear",
                        sort: true
                    },
                    {
                        label: () => vm.$t('UNICODE63246'),
                        name: "Unicode",
                        sort: true
                    },
                    {
                        label: () => vm.$t('USA_MSMQ18528'),
                        name: "UsesMsmq",
                        sort: true
                    },
                    {
                        label: () => vm.$t('JOURNAL20931'),
                        name: "Journal",
                        sort: true
                    }],
                    config: {
                        table_title: () => vm.$t('LISTA_DE_MENSAGENS31887'),
                    }
                }
            };
        },
        methods: {
            exportQueue: function(queue) {
                this.exportForm.data = queue;
                this.exportForm.show = true;
            },
            closeExportQueue: function () {
                this.exportForm.show = false;
                this.exportForm.data = { };
            }
        },
        created: function () {
            this.tQueues.rows = this.Model.MQueues.Queues || [];
            this.tQueues.total_rows = this.tQueues.rows.length;
        }
    };
</script>
