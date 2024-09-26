<template>
  <div id="notifications_notif_container">
    <h1 class="f-header__title">{{ Resources.CONFIGURACAO_DA_MENS64174 }}</h1>

    <row v-if="!isEmptyObject(Model)">
      <ul class="nav nav-tabs c-tab c-tab__divider" id="notifications_tabs" role="tablist">
        <li class="nav-item c-tab__item">
          <a class="nav-link c-tab__item-header active" id="bd-tab" data-toggle="tab" data-target="#bd" role="tab" aria-controls="bd" aria-selected="true">{{ Resources.MENSAGENS_NA_BD02187 }}</a>
        </li>
        <li class="nav-item c-tab__item">
          <a class="nav-link c-tab__item-header" id="msgs-tab" data-toggle="tab" data-target="#msgs" role="tab" aria-controls="msgs" aria-selected="false">{{ Resources.CONFIGURACAO_DE_MENS13340 }}</a>
        </li>
      </ul>
      <div class="tab-content c-tab__item-container">
        <!--BD-->
        <div class="tab-pane c-tab__item-content active" ref="bd" id="bd" role="tabpanel" aria-labelledby="bd-tab">
          <br />
          <row>
              <qtable :rows="tNotifsOnBD.rows"
                      :columns="tNotifsOnBD.columns"
                      :config="tNotifsOnBD.config"
                      :totalRows="tNotifsOnBD.total_rows"
                      style="overflow-x: auto;">
              </qtable>
          </row>
        </div>
        <!--Msgs-->
        <div class="tab-pane c-tab__item-content" ref="msgs" id="msgs" role="tabpanel" aria-labelledby="msgs-tab">
          <row>
              <br />
              <qtable :rows="tMessagesConfig.rows"
                      :columns="tMessagesConfig.columns"
                      :config="tMessagesConfig.config"
                      :totalRows="tMessagesConfig.total_rows">
                  <template #actions="props">
                    <q-button-group borderless>
                      <q-button
                        :title="Resources.EDITAR11616"
                        @click="ManageMessage(2, props.row)">
                        <q-icon icon="pencil" />
                      </q-button>
                      <q-button
                        :title="Resources.APAGAR04097"
                        @click="ManageMessage(3, props.row)">
                        <q-icon icon="bin" />
                      </q-button>
                    </q-button-group>
                  </template>
                  <template #ValTo="props">
                    {{ props.row.ValTo }}{{ props.row.ValTomanual }}
                  </template>
                  <template #ValAtivo="props">
                    <span v-if="props.row.ValAtivo" class='glyphicons glyphicons-check' />
                    <span v-else class='glyphicons glyphicons-unchecked' />
                  </template>
                  <template #ValEmail="props">
                    <span v-if="props.row.ValEmail" class='glyphicons glyphicons-check' />
                    <span v-else class='glyphicons glyphicons-unchecked' />
                  </template>
                  <template #ValGravabd="props">
                    <span v-if="props.row.ValGravabd" class='glyphicons glyphicons-check' />
                    <span v-else class='glyphicons glyphicons-unchecked' />
                  </template>
                  <template #table-footer>
                    <tr>
                      <td colspan="2">
                        <q-button
                          :label="Resources.INSERIR43365"
                          @click="ManageMessage(1)">
                          <q-icon icon="plus-sign" />
                        </q-button>
                      </td>
                    </tr>
                  </template>
              </qtable>
          </row>
        </div>
      </div>
    </row>

    <row>
      <q-button
        :label="Resources.CANCELAR49513"
        @click="cancel" />
    </row>
  </div>
</template>

<script>
  // @ is an alias to /src
  import { reusableMixin } from '@/mixins/mainMixin';
  import { QUtils } from '@/utils/mainUtils';
  import bootbox from 'bootbox';

  export default {
    name: 'notifications_notif',
    mixins: [reusableMixin],
    data: function () {
      var vm = this;
      return {
        Model: {},
        tNotifsOnBD: {
          rows: [],
          total_rows: 0,
          columns: [],
          config: {
              table_title: () => vm.$t('MENSAGENS_NA_BD02187')
            }
        },
        tMessagesConfig: {
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
                label: () => vm.$t('NOME47814'),
                name: "ValDesignac",
                sort: true,
                initial_sort: true,
                initial_sort_order: "asc"
            },
            {
                label: () => vm.$t('REMETENTE47685'),
                name: "ValFrom",
                sort: true
            },
            {
                label: () => vm.$t('DESTINATARIO22298'),
                name: "ValTo",
                sort: true,
                slot_name: 'ValTo'
            },
            {
                label: () => vm.$t('ASSUNTO16830'),
                name: "ValAssunto",
                sort: true
            },
            {
                label: () => vm.$t('MENSAGEM32641'),
                name: "ValMensagem",
                sort: true
            },
            {
                label: () => vm.$t('ATIVO_00196'),
                name: "ValAtivo",
                sort: true,
                slot_name: 'ValAtivo',
                row_text_alignment: 'text-center'
            },
            {
                label: () => vm.$t('ENVIA_EMAIL_46551'),
                name: "ValEmail",
                sort: true,
                slot_name: 'ValEmail',
                row_text_alignment: 'text-center'
            },
            {
                label: () => vm.$t('GRAVA_NA_BD_43814'),
                name: "ValGravabd",
                sort: true,
                slot_name: 'ValGravabd',
                row_text_alignment: 'text-center'
            }],
            config: {
              table_title: () => vm.$t('CONFIGURACAO_DE_MENS13340')
            }
        }
      };
    },
    methods: {
      fetchData: function () {
        var vm = this,
            mod = vm.$route.params.mod,
            idnotif = vm.$route.params.idnotif;
        QUtils.log("Fetch data - ManageNotif");
        QUtils.FetchData(QUtils.apiActionURL('Notifications', 'ManageNotif', { mod, idnotif })).done(function (data) {
          QUtils.log("Fetch data - OK (ManageNotif)", data);

          if (data.Success) {
            $.each(data.model, function (propName, value) { vm.Model[propName] = value; });

            vm.tMessagesConfig.rows = vm.Model.MessagesConfig || [];
            vm.tMessagesConfig.total_rows = vm.tMessagesConfig.rows.length

            vm.tNotifsOnBD.columns = [];
            $.each(vm.Model.NotifsOnBD_headers, function(i, column) {
              vm.tNotifsOnBD.columns.push({
                label: column,
                name: column.toLowerCase(),
                sort: true
              });
            });
            vm.tNotifsOnBD.rows = vm.Model.NotifsOnBD_body || [];
            vm.tNotifsOnBD.total_rows = vm.tNotifsOnBD.rows.length
          }
          else if (data.redirect) {
            vm.$router.replace({ name: data.redirect });
          }
        });
      },
      cancel: function () {
        this.$router.replace({ name: 'notifications', params: { culture: this.currentLang, system: this.currentYear } });
      },
      ManageMessage: function(mod, row) {
        var codmesgs = $.isEmptyObject(row) ? null : row.ValCodmesgs,
            idnotif = this.$route.params.idnotif;
        this.$router.push({ name: 'manage_message', params: { mod, codmesgs, idnotif, culture: this.currentLang, system: this.currentYear } });
      }
    },
    created: function () {
      // Ler dados
      this.fetchData();
    }
  };
</script>
