<template>
  <div id="system_setup_security_container">
    <br />
    <card>
      <template #header>
        {{ Resources.AUTENTICACAO37999 }}
      </template>
      <template #body>
        <row>
          <select-input v-model="Security.AuthenticationMode" v-if="SelectLists" :options="SelectLists.AuthenticationMode" :label="Resources.MODO_DE_AUTENTICACAO19339"></select-input>
        </row>
        <row>
          <select-input v-model="Security.AllowMultiSessionPerUser" v-if="SelectLists" :options="SelectLists.MultisessionMode" :label="Resources.POLITICA_DE_SESSOES_19368"></select-input>
        </row>
        <row>
          <checkbox-input v-model="Security.AllowAuthenticationRecovery" :label="Resources.PERMITE_RECUPERACAO_41959"></checkbox-input>
        </row>
        <row>
          <checkbox-input v-model="Security.Activate2FA" :label="Resources.ATIVAR_AUTENTICACAO_40943"></checkbox-input>
        </row>
        <row v-if="Security.Activate2FA">
          <checkbox-input v-model="Security.Mandatory2FA" :label="Resources.OBRIGATORIO_A_UTILIZ32451"></checkbox-input>
        </row>
        <row >
            <numeric-input v-model="Security.SessionTimeOut" :label="Resources.TIME_OUT_DA_SESSAO36825"></numeric-input>
        </row>
      </template>
    </card>
    <hr />
    <card>
      <template #header>
        {{ Resources.POLITICA_DE_PASSWORD17131 }}
      </template>
      <template #body>
        <row>
          <numeric-input v-model="Security.MinCharacters" :label="Resources.MINIMO_DE_CARACTERES10869"></numeric-input>
        </row>
        <row>
          <select-input v-model="Security.PasswordStrength" v-if="SelectLists" :options="SelectLists.PasswordStrength" :label="Resources.MODO_DE_AUTENTICACAO19339"></select-input>
        </row>
        <row>
          <numeric-input v-model="Security.MaxAttempts" :label="Resources.NUMERO_MAXIMO_TENTAT34521"></numeric-input>
        </row>
        <row>
          <checkbox-input v-model="Security.ExpirationDateBool" :label="Resources.EXPIRACAO_DA_PASSWOR46052"></checkbox-input>
        </row>
        <row>
          <text-input v-model="Security.ExpirationDate"></text-input>
        </row>
        <row>
          <select-input v-model="Security.PasswordAlgorithms" v-if="SelectLists" :options="SelectLists.PasswordAlgorithms" :label="Resources.ALGORITMO_DE_ENCRIPT09649"></select-input>
        </row>
        <row>
          <checkbox-input v-model="Security.UsePasswordBlacklist" :label="Resources.USE_PASSWORD_BLACKLI22314"></checkbox-input>
          <a class="nav-link" href="#" @click.prevent="navigateTo('manage_blacklist')">{{ Resources.MANAGE_PASSWORD_BLAC01612 }}</a>
        </row>
      </template>
    </card>
    <br />
    <row>
      <q-button
        b-style="primary"
        :label="Resources.GRAVAR_CONFIGURACAO36308"
        @click="SaveConfigSecurity" />
    </row>

    <hr />
    <row>
        <qtable :rows="tIdentityProviders.rows"
                :columns="tIdentityProviders.columns"
                :config="tIdentityProviders.config"
                :totalRows="tIdentityProviders.total_rows">

            <template #actions="props">
              <q-button-group borderless>
                <q-button
                  :title="Resources.EDITAR11616"
                  @click="changeIdentityProvider(props.row)">
                  <q-icon icon="pencil" />
                </q-button>
                <q-button
                  :title="Resources.ELIMINAR21155"
                  @click="deleteIdentityProvider(props.row)">
                  <q-icon icon="bin" />
                </q-button>
              </q-button-group>
            </template>
            <template #table-footer>
                <tr>
                    <td colspan="4">
                      <q-button
                        :label="Resources.INSERIR43365"
                        @click="createIdentityProvider">
                        <q-icon icon="plus-sign" />
                      </q-button>
                    </td>
                </tr>
            </template>
        </qtable>
    </row>

    <row>
      <qtable :rows="tRoleProviders.rows"
              :columns="tRoleProviders.columns"
              :config="tRoleProviders.config"
              :totalRows="tRoleProviders.total_rows">

          <template #actions="props">
            <q-button-group borderless>
              <q-button
                :title="Resources.EDITAR11616"
                @click="changeRoleProvider(props.row)">
                <q-icon icon="pencil" />
              </q-button>
              <q-button
                :title="Resources.ELIMINAR21155"
                @click="deleteRoleProvider(props.row)">
                <q-icon icon="bin" />
              </q-button>
            </q-button-group>
          </template>
          <template #table-footer>
              <tr>
                  <td colspan="5">
                    <q-button
                      :label="Resources.INSERIR43365"
                      @click="createRoleProvider">
                      <q-icon icon="plus-sign" />
                    </q-button>
                  </td>
              </tr>
          </template>
      </qtable>
    </row>

    <hr />
    <row>
      <qtable :rows="tUsers.rows"
              :columns="tUsers.columns"
              :config="tUsers.config"
              :totalRows="tUsers.total_rows">

          <template #actions="props">
            <q-button-group borderless>
              <q-button
                :title="Resources.EDITAR11616"
                @click="changeUser(props.row)">
                <q-icon icon="pencil" />
              </q-button>
              <q-button
                :title="Resources.ELIMINAR21155"
                @click="deleteUser(props.row)">
                <q-icon icon="bin" />
              </q-button>
            </q-button-group>
          </template>
          <template #Type="props">
              <!-- This is a horrible and temporary solution, needs refactor -->
              {{ SelectLists.DisplayUserType.filter((t) => t.Text == props.row.Type)[0] }}
          </template>
          <template #AutoLogin="props">
              <span v-if="props.row.AutoLogin" class='glyphicons glyphicons-check' />
              <span v-else class='glyphicons glyphicons-unchecked' />
          </template>
          <template #table-footer>
              <tr>
                  <td colspan="4">
                    <q-button
                      :label="Resources.INSERIR43365"
                      @click="createUser">
                      <q-icon icon="plus-sign" />
                    </q-button>
                  </td>
              </tr>
          </template>
      </qtable>
    </row>

    <modal_identity_provider
        v-if="modalForms.identityProvider.show"
        :Model="modalForms.identityProvider.data"
        :SelectLists="SelectLists"
        @callback="callbackIdentityProvider"
        @close="closeIdentityProviderModal" />

    <modal_role_provider
        v-if="modalForms.roleProvider.show"
        :Model="modalForms.roleProvider.data"
        :SelectLists="SelectLists"
        @callback="callbackRoleProvider"
        @close="closeRoleProviderModal" />

    <modal_user
        v-if="modalForms.user.show"
        :Model="modalForms.user.data"
        :SelectLists="SelectLists"
        @callback="callbackUser"
        @close="closeUserModal" />
  </div>
</template>

<script>
  // @ is an alias to /src
  import { reusableMixin } from '@/mixins/mainMixin';
  import { QUtils } from '@/utils/mainUtils';
  import modal_user from './User.vue';
  import modal_identity_provider from './IdentityProvider.vue';
  import modal_role_provider from './RoleProvider.vue';
  import bootbox from 'bootbox';
  import { reactive } from 'vue';

  export default {
    name: 'security',
    components: { modal_role_provider, modal_identity_provider, modal_user },
    emits: ['updateUsers', 'updateModal'],
    props: {
      Model: {
        required: true
      },
      SelectLists: {
        required: true
      }
    },
    mixins: [reusableMixin],
    data: function () {
      var vm = this;
      return {
        modalForms: {
          user: {
            show: true,
            data: {}
          },
          identityProvider: {
            show: true,
            data: {}
          },
          roleProvider: {
            show: true,
            data: {}
          }
        },
        tIdentityProviders: {
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
                label: () => vm.$t('NOME47814'),
                name: "Name",
                sort: true,
                initial_sort: true,
                initial_sort_order: "asc"
            },
            {
                label: () => vm.$t('TIPO55111'),
                name: "Type",
                sort: true
            },
            {
                label: () => vm.$t('CONFIGURACAO10928'),
                name: "Config",
                sort: true
            }],
            config: {
                table_title: () => vm.$t('FORNECEDORES_DE_IDEN35608'),
                pagination: false,
                pagination_info: false,
                global_search: {
                    visibility: false
                }
            }
        },
        tRoleProviders: {
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
                label: () => vm.$t('NOME47814'),
                name: "Name",
                sort: true,
                initial_sort: true,
                initial_sort_order: "asc"
            },
            {
                label: () => vm.$t('TIPO55111'),
                name: "Type",
                sort: true
            },
            {
                label: () => vm.$t('CONFIGURACAO10928'),
                name: "Config",
                sort: true
            },
            {
                label: () => vm.$t('PRECONDICAO44917'),
                name: "Precond",
                sort: true
            }],
            config: {
                table_title: () => vm.$t('FORNECEDORES_DE_AUTO29899'),
                pagination: false,
                pagination_info: false,
                global_search: {
                    visibility: false
                }
            }
        },
        tUsers: {
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
                      label: () => vm.$t('NOME47814'),
                      name: "Name",
                      sort: true,
                      initial_sort: true,
                      initial_sort_order: "asc"
                  },
                  {
                      label: () => vm.$t('TIPO55111'),
                      name: "Type",
                      slot_name: 'Text',
                      sort: true
                  },
                  {
                      label: () => vm.$t('LOGIN_AUTOMATICO22707'),
                      name: "AutoLogin",
                      slot_name: 'AutoLogin',
                      sort: true
                  }],
              config: {
                  table_title: () => vm.$t('UTILIZADORES_FIXOS00716'),
                  pagination: false,
                  pagination_info: false,
                  global_search: {
                      visibility: false
                  }
              }
          }
      };
    },
    computed: {
        Security: function () {
            var vm = this;
            return reactive(!$.isEmptyObject(vm.currentApp) && !$.isEmptyObject(vm.Model) ? (vm.Model[vm.currentApp] || {}) : {});
        }
    },
    methods: {
      SaveConfigSecurity: function () {
        var vm = this;
        QUtils.log("SaveConfigSecurity - Request", QUtils.apiActionURL('Config', 'SaveConfigSecurity'));
        QUtils.postData('Config', 'SaveConfigSecurity', vm.Security, null, function (data) {
          QUtils.log("SaveConfigSecurity - Response", data);
          if (data.Success) {
            bootbox.alert(vm.Resources.ALTERACOES_EFETUADAS10166);
          } else {
            bootbox.alert(data.Message);
          }
        });
      },
      showUserModal: function (mode, user) {
        var vm = this;
        vm.modalForms.user.data = $.extend(true, {}, user);
        vm.modalForms.user.data.FormMode = mode;
        //vm.modalForms.user.show = true;

        $('#system_setup_user').modal('show');
      },
      closeUserModal() {
        $('#system_setup_user').modal('hide');
      },
      showIdentityProviderModal: function (mode, identityProvider) {
        var vm = this;
        vm.modalForms.identityProvider.data = $.extend(true, {}, identityProvider);
        vm.modalForms.identityProvider.data.FormMode = mode;
        //vm.modalForms.identityProvider.show = true;

        $('#system_setup_identity_provider').modal('show');
      },
      closeIdentityProviderModal() {
        $('#system_setup_identity_provider').modal('hide')
      },
      showRoleProviderModal: function (mode, roleProvider) {
        var vm = this;
        vm.modalForms.roleProvider.data = $.extend(true, {}, roleProvider);
        vm.modalForms.roleProvider.data.FormMode = mode;
        //vm.modalForms.roleProvider.show = true;

        $('#system_setup_role_provider').modal('show');
      },
      closeRoleProviderModal() {
        $('#system_setup_role_provider').modal('hide')
      },
      changeUser: function (user) {
        var vm = this;
        vm.showUserModal('edit', user);
      },
      deleteUser: function (user) {
        var vm = this;
        vm.showUserModal('delete', user);
      },
      createUser: function () {
        var vm = this,
          url = QUtils.apiActionURL('Config', 'GetNewUserCfg');
        QUtils.FetchData(url).done(function (data) {
          vm.showUserModal('new', data);
        });
      },
      callbackUser: function (eventData) {
        this.$emit('updateUsers', { users: eventData.users, currentApp: this.currentApp});
      },
      changeIdentityProvider: function (identityProvider) {
        var vm = this;
        vm.showIdentityProviderModal('edit', identityProvider);
      },
      deleteIdentityProvider: function (identityProvider) {
        var vm = this;
        vm.showIdentityProviderModal('delete', identityProvider);
      },
      createIdentityProvider: function () {
        var vm = this,
          url = QUtils.apiActionURL('Config', 'GetNewIdentityProviderCfg');
        QUtils.FetchData(url).done(function (data) {
          vm.showIdentityProviderModal('new', data);
        });
      },
      callbackIdentityProvider: function (eventData) {
        var vm = this;
        switch (eventData.mode) {
          case 'new':
            vm.Security.IdentityProviders.push(eventData.identityProvider);
            break;
          case 'edit':
            var editIdx = vm.Security.IdentityProviders.findIndex((ip) => { return ip.Rownum === eventData.identityProvider.Rownum });
            vm.Security.IdentityProviders[editIdx] = eventData.identityProvider;
            break;
          case 'delete':
            vm.$emit('updateModal');
            break;
        }
        vm.setIdentityProvidersTableData();
      },
      changeRoleProvider: function (roleProvider) {
        var vm = this;
        vm.showRoleProviderModal('edit', roleProvider);
      },
      deleteRoleProvider: function (roleProvider) {
        var vm = this;
        vm.showRoleProviderModal('delete', roleProvider);
      },
      createRoleProvider: function () {
        var vm = this,
          url = QUtils.apiActionURL('Config', 'GetNewRoleProviderCfg');
        QUtils.FetchData(url).done(function (data) {
          vm.showRoleProviderModal('new', data);
        });
      },
      callbackRoleProvider: function (eventData) {
        var vm = this;
        switch (eventData.mode) {
          case 'new':
            vm.Security.RoleProviders.push(eventData.roleProvider);
            break;
          case 'edit':
            var editIdx = vm.Security.RoleProviders.findIndex((ip) => { return ip.Rownum === eventData.roleProvider.Rownum });
            vm.Security.RoleProviders[editIdx] = eventData.roleProvider;
            break;
          case 'delete':
            vm.$emit('updateModal');
            break;
        }
        vm.setRoleProvidersTableData();
      },
      setIdentityProvidersTableData: function () {
          this.tIdentityProviders.rows = this.Security.IdentityProviders || [];
          this.tIdentityProviders.total_rows = this.tIdentityProviders.rows.length;
      },
      setRoleProvidersTableData: function () {
          this.tRoleProviders.rows = this.Security.RoleProviders || [];
          this.tRoleProviders.total_rows = this.tRoleProviders.rows.length;
      },
      setUsersTableData: function () {
          this.tUsers.rows = this.Security.Users || [];
          this.tUsers.total_rows = this.tUsers.rows.length;
      }
    },
    created: function() {
      this.setIdentityProvidersTableData();
      this.setRoleProvidersTableData();
      this.setUsersTableData();
    },
    updated: function() {
      this.setIdentityProvidersTableData();
      this.setRoleProvidersTableData();
      this.setUsersTableData();
    },
    watch: {
        'Security.Activate2FA': function (val) {
            if (!val) {
                this.Security.Mandatory2FA = false;
            }
        }
    }
  };
</script>
