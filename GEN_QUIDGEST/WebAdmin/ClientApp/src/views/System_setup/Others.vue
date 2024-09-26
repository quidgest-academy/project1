<template>
    <div id="system_setup_others_container">
        <br />
        <card>
            <template #header>
                Crystal Reports
            </template>
            <template #body>
                <row>
                    <text-input v-model="Model.pathReports" :label="Resources.CAMINHO_PARA_RELATOR05547"></text-input>
                </row>
            </template>
        </card>
        <hr />
        <card>
            <template #header>
                SQL Server Reporting Services
            </template>
            <template #body>
                <row>
                    <text-input v-model="Model.ssrsServer" :label="Resources.URL05719"></text-input>
                </row>
                <row>
                    <text-input v-model="Model.ssrsServerPath" :label="Resources.CAMINHO18436"></text-input>
                </row>
                <row>
                    <checkbox-input v-model="Model.isLocalReports" :label="Resources.SAO_OS_RELATORIOS_LO04230"></checkbox-input>
                </row>
                <row>
                    <text-input v-model="Model.ssrsServerDomain" :label="Resources.DOMINIO33043"></text-input>
                </row>
                <row>
                    <text-input v-model="Model.ssrsServerUsername" :label="Resources.NOME_DE_UTILIZADOR58858"></text-input>
                </row>
                <row>
                    <password-input v-model="Model.ssrsServerPassword" :label="Resources.PALAVRA_PASSE44126" :showFiller="Model.hasSsrsServerPassword"></password-input>
                </row>
            </template>
        </card>
        <hr />
        <card>
            <template #header>
                Date Format
            </template>
            <template #body>
                <row>
                    <text-input v-if="Model.DateFormat" v-model="Model.DateFormat.date" :label="Resources.DATA18071"></text-input>
                </row>
                <row>
                    <text-input v-if="Model.DateFormat" v-model="Model.DateFormat.dateTime" :label="Resources.DATA_E_HORA33196"></text-input>
                </row>
                <row>
                    <text-input v-if="Model.DateFormat" v-model="Model.DateFormat.dateTimeSeconds" :label="Resources.DATA__HORAS_E_SEGUND03637"></text-input>
                </row>
                <row>
                    <text-input v-if="Model.DateFormat" v-model="Model.DateFormat.time" :label="Resources.HORAS01448"></text-input>
                </row>
            </template>
        </card>
        <hr />
        <card>
            <template #header>
                Number Format
            </template>
            <template #body>
                <row>
                    <select-input v-model="Model.DecimalSeparator" v-if="SelectLists" :options="SelectLists.DecimalSeparator" :label="Resources.SEPARADOR_DECIMAL14173"></select-input>
                </row>
                <row>
                    <select-input v-model="Model.GroupSeparator" v-if="SelectLists" :options="SelectLists.GroupSeparator" :label="Resources.SEPARADOR_DE_GRUPO26735"></select-input>
                </row>
            </template>
        </card>
        <hr />
        <card>
            <template #header>
                Environment
            </template>
            <template #body>
                <row>
                    <checkbox-input v-model="Model.QAEnvironment" :label="Resources.AMBIENTE_DE_QA_09940"></checkbox-input>
                </row>
            </template>
        </card>
        <br />
        <row>
            <q-button
              b-style="primary"
              :label="Resources.GRAVAR45301"
              @click="SaveConfigOthers" />
        </row>
        <hr />
        <row>
            <qtable :rows="tMoreP.rows"
                    :columns="tMoreP.columns"
                    :config="tMoreP.config"
                    :totalRows="tMoreP.total_rows"
                    :table_title="Resources.MOTOR_DE_PESQUISA__E50766">
                <template #actions="props">
                  <q-button-group borderless>
                    <q-button
                      :title="Resources.EDITAR11616"
                      @click="changeMoreProperty(props.row)">
                      <q-icon icon="pencil" />
                    </q-button>
                    <q-button
                      :title="Resources.ELIMINAR21155"
                      @click="deleteMoreProperty(props.row)">
                      <q-icon icon="bin" />
                    </q-button>
                  </q-button-group>
                </template>
                <template #table-footer>
                    <tr>
                        <td colspan="3">
                          <q-button
                            :label="Resources.INSERIR43365"
                            @click="createMoreProperty">
                            <q-icon icon="plus-sign" />
                          </q-button>
                        </td>
                    </tr>
                </template>
            </qtable>
        </row>
        <hr />
        <row>
            <qtable :rows="cfgCores.rows"
                    :columns="cfgCores.columns"
                    :config="cfgCores.config"
                    :totalRows="cfgCores.total_rows">
                <template #actions="props">
                  <q-button-group borderless>
                    <q-button
                      :title="Resources.EDITAR11616"
                      @click="editCore(props.row)">
                      <q-icon icon="pencil" />
                    </q-button>
                    <q-button
                      :title="Resources.ELIMINAR21155"
                      @click="deleteCore(props.row)">
                      <q-icon icon="bin" />
                    </q-button>
                  </q-button-group>
                </template>
                <template #table-footer>
                    <tr>
                        <td colspan="2">
                          <q-button
                            :label="Resources.INSERIR43365"
                            @click="createCore">
                            <q-icon icon="plus-sign" />
                          </q-button>
                        </td>
                    </tr>
                </template>
            </qtable>
        </row>

        <modal_core
            v-if="modalForms.core.show"
            :Model="modalForms.core.data"
            :SelectLists="SelectLists"
            @updateModal="callbackCore"
            @close="closeCoreModal" />

        <modal_more_property
            v-if="modalForms.moreProperties.show"
            :Model="modalForms.moreProperties.data"
            :SelectLists="SelectLists"
            @callback="callbackMoreProperty"
            @close="closeMoreProperties" />

    </div>
</template>

<script>
  // @ is an alias to /src
  import { reusableMixin } from '@/mixins/mainMixin';
  import { QUtils } from '@/utils/mainUtils';
  import bootbox from 'bootbox';
  import modal_core from './Core.vue';
  import modal_more_property from './MoreProperty.vue';

  export default {
    name: 'others',
    components: { modal_core, modal_more_property },
    props: {
      Model: {
        required: true
      },
      Cores: {
        required: true
      },
      SelectLists: {
        required: true
      }
    },
    mixins: [reusableMixin],
    emits: ['updateModal'],
    data: function () {
      var vm = this;
      return {
        modalForms: {
            core: {
                show: true,
                data: { }
            },
        moreProperties: {
                show: false,
                data: { }
            }
        },
        cfgCores: {
          rows: [],
          total_rows: 0,
          columns: [{
            label: vm.$t('ACOES22599'),
            name: "actions",
            slot_name: "actions",
            sort: false,
            column_classes: "thead-actions",
            row_text_alignment: 'text-center',
            column_text_alignment: 'text-center'
          },
          {
            label: vm.$t('INDEX00140'),
            name: "Index",
            sort: true,
            initial_sort: true,
            initial_sort_order: "asc"
          },
          {
            label: vm.$t('ID36840'),
            name: "Id",
            sort: true,
            initial_sort: true,
            initial_sort_order: "asc"
          },
          {
            label: vm.$t('AREA19058'),
            name: "Area",
            sort: true
          },
          {
            label: vm.$t('FSCRAWLER01982'),
            name: "Urlfscrawler",
            sort: true
          },
          {
            label: vm.$t('URL05719'),
            name: "Url",
            sort: true
          },
          {
            label: vm.$t('UTILIZADOR52387'),
            name: "ElasticUser",
            sort: true
          }],
          config: {
            table_title: vm.$t("MOTOR_DE_PESQUISA__E50766"),
            global_search: {
              classes: "qtable-global-search",
              // searchOnPressEnter: true,
              showRefreshButton: true,
              searchDebounceRate: 1000
            },
            server_mode: true,
            preservePageOnDataChange: true
          },
          queryParams: {
            sort: [],
            filters: [],
            global_search: "",
            per_page: 10,
            page: 1,
          }
        },
        tMoreP: {
            rows: [],
            total_rows: 0,
            columns: [{
              label: vm.$t('ACOES22599'),
              name: "actions",
              slot_name: "actions",
              sort: false,
              column_classes: "thead-actions",
              row_text_alignment: 'text-center',
              column_text_alignment: 'text-center'
            },
            {
              label: vm.$t('KEY01046'),
              name: "Key",
              sort: true,
              initial_sort: true,
              initial_sort_order: "asc"
            },
            {
              label: vm.$t('VALUE10285'),
              name: "Val",
              sort: true
              }],
            config: {
              table_title: vm.$t('MORE_PROPERTIES36834')
            }
        },
      };
    },
    methods: {
    showMorePropertyModal: function (mode, moreProperty) {
        var vm = this;
        vm.modalForms.moreProperties.data = $.extend(true, {}, moreProperty);
        vm.modalForms.moreProperties.data.FormMode = mode;
        //
        vm.modalForms.moreProperties.show = true;

        //$('#system_setup_more_property').modal('show');
        //
      },
      changeMoreProperty: function (moreProperty) {
        var vm = this;
        vm.showMorePropertyModal('edit', moreProperty);
      },
      deleteMoreProperty: function (moreProperty) {
        var vm = this;
        vm.showMorePropertyModal('delete', moreProperty);
      },
      createMoreProperty: function () {
        var vm = this,
          url = QUtils.apiActionURL('Config', 'GetNewMorePropertyCfg');
        QUtils.FetchData(url).done(function (data) {
          vm.showMorePropertyModal('new', data);
        });
      },
      callbackMoreProperty: function (eventData) {
          var vm = this;
          vm.closeMoreProperties();
        switch (eventData.mode) {
          case 'new':
            vm.Model.MoreProperties.push(eventData.moreProperty);
            break;
          case 'edit':
            vm.$emit('updateModal');
            break;
          case 'delete':
            vm.$emit('updateModal');
            break;
          }
      },
      closeMoreProperties: function() {
          this.modalForms.moreProperties.show = false;
      },
      closeCoreModal() {
          $('#system_setup_core').modal('hide')
      },
      SaveConfigOthers: function () {
          var vm = this;
          QUtils.log("SaveConfigOthers - Request", QUtils.apiActionURL('Config', 'SaveConfigOthers'));
          QUtils.postData('Config', 'SaveConfigOthers', vm.Model, null, function (data) {
              QUtils.log("SaveConfigOthers - Response", data);
              if (data.Success) {
                  bootbox.alert(vm.Resources.ALTERACOES_EFECTUADA64514);
              }
              else {
                  bootbox.alert(data.Message);
              }
          });
      },
      showCoreModal: function (mode, core) {
          var vm = this;
          vm.modalForms.core.data = $.extend(true, {}, core);
          vm.modalForms.core.data.FormMode = mode;
          //vm.modalForms.core.show = true;

          $('#system_setup_core').modal('show');
      },
      createCore: function () {
          var vm = this,
              url = QUtils.apiActionURL('Config', 'GetNewCoreCfg');
          QUtils.FetchData(url).done(function (data) {
              vm.showCoreModal('new', data);
          });
      },
      editCore: function (core) {
          var vm = this;
          vm.showCoreModal('edit', core);
      },
      deleteCore: function (core) {
          var vm = this;
          vm.showCoreModal('delete', core);
      },
      callbackCore: function () {
          this.$emit('updateModal');
      }
    },
    mounted: function () {
        this.cfgCores.rows = this.Model.Cores || [];
        this.cfgCores.total_rows = (this.Model.Cores || []).length;
    this.tMoreP.rows = this.Model.MoreProperties || [];
        this.tMoreP.total_rows = (this.tMoreP.rows || []).length;
    },
  updated: function () {
        this.tMoreP.rows = this.Model.MoreProperties || [];
        this.tMoreP.total_rows = (this.tMoreP.rows || []).length;
    },
  created: function () {
      // Ler dados + Resources

    },
    watch: {
      'Model.Cores': {
            handler() {
                this.cfgCores.rows = this.Model.Cores || [];
                this.cfgCores.total_rows = (this.Model.Cores || []).length;
            },
            deep: true
      }
    }
  };
</script>
