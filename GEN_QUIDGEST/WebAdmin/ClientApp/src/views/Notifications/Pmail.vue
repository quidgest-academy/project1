<template>
  <div id="notifications_pmail_container">
      <row v-if="!isEmptyObject(Model.ResultMsg)">
          <div class="alert alert-danger">
              <span>
                  <b class="status-message">{{ Model.ResultMsg }}</b>
              </span>
          </div>
          <br />
      </row>
      <row>
          <br />
          <qtable :rows="tPMail.rows"
                  :columns="tPMail.columns"
                  :config="tPMail.config"
                  :totalRows="tPMail.total_rows">
              <template #actions="props">
                <q-button-group borderless>
                  <q-button
                    :title="Resources.EDITAR11616"
                    @click="ManageProperties(2, props.row)">
                    <q-icon icon="pencil" />
                  </q-button>
                  <q-button
                    :title="Resources.APAGAR04097"
                    @click="ManageProperties(3, props.row)">
                    <q-icon icon="bin" />
                  </q-button>
                </q-button-group>
              </template>
              <template #ValSsl="props">
                  <span v-if="props.row.ValSsl" class='glyphicons glyphicons-check' />
                  <span v-else class='glyphicons glyphicons-unchecked' />
              </template>
              <template #ValAuth="props">
                  <span v-if="props.row.ValAuth" class='glyphicons glyphicons-check' />
                  <span v-else class='glyphicons glyphicons-unchecked' />
              </template>
              <template #table-footer>
                <tr>
                  <td colspan="2">
                    <q-button
                      :label="Resources.INSERIR43365"
                      @click="ManageProperties(1)">
                        <q-icon icon="plus-sign" />
                      </q-button>
                    </td>
                </tr>
            </template>
          </qtable>
      </row>
  </div>
</template>

<script>
  // @ is an alias to /src
  import { reusableMixin } from '@/mixins/mainMixin';
  //import { QUtils } from '@/utils/mainUtils';

  export default {
    name: 'notifications_pmail',
    mixins: [reusableMixin],
    props: {
      EmailProperties: {
        required: true
      }
    },
    data: function () {
      var vm = this;
      return {
        Model: {},
        tPMail: {
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
                    label: () => vm.$t('ID36840'),
                    name: "ValId",
                    sort: true,
                    initial_sort: true,
                    initial_sort_order: "asc"
                },
                {
                    label: () => vm.$t('NOME_DO_REMETENTE60175'),
                    name: "ValDispname",
                    sort: true,
                },
                {
                    label: () => vm.$t('REMETENTE47685'),
                    name: "ValFrom",
                    sort: true
                },
                {
                    label: () => vm.$t('SERVIDOR_DE_SMTP00309'),
                    name: "ValSmtpserver",
                    sort: true
                },
                {
                    label: () => vm.$t('PORTA55707'),
                    name: "ValPort",
                    sort: true
                },
                {
                    label: 'SSL',
                    name: "ValSsl",
                    sort: true,
                    slot_name: 'ValSsl'
                },
                {
                    label: () => vm.$t('REQUER_AUTENTICACAO_31938'),
                    name: "ValAuth",
                    sort: true,
                    slot_name: 'ValAuth'
                },
                {
                    label: () => vm.$t('UTILIZADOR52387'),
                    name: "ValUsername",
                    sort: true
                }],
                config: {
                  table_title: () => vm.$t('SERVIDORES_DE_EMAIL15136')
                }
            }
      };
    },
    methods: {
      updateTPMailData: function() {
          var vm = this;
          vm.tPMail.rows = vm.EmailProperties || [];
          vm.tPMail.total_rows = (vm.EmailProperties || []).length;
      },
      ManageProperties: function (mod, row) {
        var vm = this, codpmail = '';
        if(!$.isEmptyObject(row) && (mod === 2 || mod === 3)) codpmail = row.ValCodpmail;
        vm.$router.push({ name: 'manage_properties', params: { mod, codpmail, culture: vm.currentLang, system: vm.currentYear } });
      }
    },
    created: function () {
      // Ler dados
      this.updateTPMailData();
    },
    watch: {
      // call again the method if the route changes
        EmailProperties: function () {
            this.updateTPMailData();
        }
    }
  };
</script>
