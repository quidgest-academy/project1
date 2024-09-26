<template>
  <div>
    <div class="q-stack--column">
			<h1 class="f-header__title">
			{{ Resources.GESTAO_DE_UTILIZADOR20428 }}
			</h1>
		</div>
    <hr>
    <ul class="nav nav-tabs c-tab c-tab__divider" id="users_tabs" role="tablist">
        <li class="nav-item c-tab__item">
            <a class="nav-link c-tab__item-header active" id="all-users-tab" data-toggle="tab" data-target="#all-users" role="tab" aria-controls="all-users" aria-selected="true">{{ Resources.UTILIZADORES39761 }}</a>
        </li>
        <li class="nav-item c-tab__item">
            <a class="nav-link c-tab__item-header" id="role-tab" data-toggle="tab" data-target="#roles" role="tab" aria-controls="roles" aria-selected="true">{{ Resources.ROLES61449 }}</a>
        </li>
        <li class="nav-item c-tab__item">
            <a class="nav-link c-tab__item-header" id="Nroles-tab" data-toggle="tab" data-target="#Nroles" role="tab" aria-controls="Nroles" aria-selected="true">{{ Resources.GESTAO_DE_ACESSOS25265 }}</a>
        </li>
    </ul>
    <div class="tab-content c-tab__item-container">
      <div class="tab-pane c-tab__item-content active" id="all-users" ref="all-users" role="tabpanel" aria-labelledby="all-users">
        <row>
          <div class="module-container panel panel-default">
            <div class="panel-heading">{{ Resources.MODULOS17298 }}</div>
            <div class="form-check-inline">
                    <input type="checkbox" class="i-checkbox" v-model="selectModules" />
                    <label>{{ Resources.TODOS59977 }}</label>
                </div>
            <div class="form-check-inline" v-for="module in Modules" :key="module.Cod">
                <input type="checkbox" class="i-checkbox" v-model="module.active" />
                <label>{{Resources[module.Description]}}</label>
            </div>
          </div>
          <qtable :rows="Users.rows"
                  :columns="Users.columns"
                  :config="Users.config"
                  @on-change-query="onChangeQuery"
                  :totalRows="Users.total_rows"
                  :exportLabel="Resources.EXPORT_TO_EXCEL22478"
                  :enableExport="true">
            <!--Action column-->
            <template #actions="props">
              <q-button-group borderless>
                <q-button
                  :title="Resources.EDITAR11616"
                  @click="editUser(props.row)">
                  <q-icon icon="pencil" />
                </q-button>
                <q-button
                  :title="Resources.ELIMINAR21155"
                  @click="deleteUser(props.row)">
                  <q-icon icon="bin" />
                </q-button>
              </q-button-group>
            </template>
            <!--All roles slot -->
            <template #user-roles="props">
                <template v-for="userRole in props.row.UserRoles" :key="userRole.Role">
                    <span v-if="Modules[userRole.Module].active" class="role-tag">{{Resources[userRole.Designation]}}</span>
                </template>
            </template>
            <template #PRO="props">
              <template v-for="userRole in props.row.UserRoles" :key="userRole.Role">
                    <span v-if="userRole.Module === 'PRO'" class="role-tag">{{Resources[userRole.Designation]}}</span>
              </template>
            </template>
            <template #table-footer>
              <tr>
                <td colspan="2">
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

        <row v-if="hasAdIdentityProviders">
            <select-input v-model="domainprovider" :options="identityProviders" :label="'Select the domain'"></select-input>
            <q-button
              b-style="primary"
              :label="Resources.IMPORTAR_UTILIZADORE27134"
              @click="ImportarUsersAD" />
        </row>

      </div>
      <!--Roles tab info-->
      <div class="tab-pane c-tab__item-content" id="roles" ref="roles" role="tabpanel" aria-labelledby="all-users">
        <roles></roles>
      </div>
      <!--Many Roles tab info-->
      <div class="tab-pane c-tab__item-content" id="Nroles" ref="Nroles" role="tabpanel" aria-labelledby="all-users">
        <Nroles></Nroles>
      </div>
    </div>
  </div>
</template>

<script>
  // @ is an alias to /src
  import { reusableMixin } from '@/mixins/mainMixin';
  import { QUtils } from '@/utils/mainUtils';
  import bootbox from 'bootbox';
  import roles from './RoleList.vue';
  import Nroles from './UserRoles.vue';

  export default {
    name: 'users',
    mixins: [reusableMixin],
    components: { roles,Nroles},
    data: function () {
      var vm = this;
      return {
        Model: {},
        Modules: {
          PRO : { active: true, Cod: "PRO", Description: 'MY_APPLICATION56216'},
        },
        Users: {
          rows: [],
          total_rows: 0,
          columns: [],
          config: {
            global_search: {
              classes: "qtable-global-search",
              placeholder : vm.$t('PESQUISAR_NOME_DE_UT55805'),
              searchOnPressEnter: true,
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
            component: "user",
          }
        },
        Roles: {
            rows: [],
            total_rows: 1,
            columns: [
                {
                    label: () => vm.$t('MODULO43834'),
                    name: "module",
                    sort: true,
                    initial_sort: true,
                    initial_sort_order: "asc"
                },
                {
                    label: () => vm.$t('ROLE60946'),
                    name: "role",
                    sort: true,
                    initial_sort: true,
                    initial_sort_order: "asc"
                }]
        },
        domainprovider: '',
        identityProviders: [],
        columnModules : true,
        hasAdIdentityProviders: false
      };
    },
    computed: {
            selectModules: {

                get: function () {
                  //If we explicitly declare the variables, vue will bind them
                  return                     this.Modules.PRO.active ;
                },
                set: function (value) {
                    //If we explicitly declare the variables, vue will bind them
                    this.Modules.PRO.active = value;
                    return value;
                }
            }


        },
    methods: {
      createUser: function () {
        this.$router.push({ name: 'manage_users', params: { mod: 1, culture: this.currentLang, system: this.currentYear } });
      },
      editUser: function (row) {
        this.$router.push({ name: 'manage_users', params: { mod: 2, cod: row.Codpsw, culture: this.currentLang, system: this.currentYear } });
      },
      deleteUser: function (row) {
        this.$router.push({ name: 'manage_users', params: { mod: 3, cod: row.Codpsw, culture: this.currentLang, system: this.currentYear } });
      },
      GetUserList: function () {
          var vm = this;
          QUtils.log("GetUserList - Users");
          QUtils.FetchData(QUtils.apiActionURL('Users', 'GetUserList', vm.Users.queryParams)).done(function (data) {
              QUtils.log("GetUserList - OK (Users)", data);
              vm.Users.total_rows = data.recordsTotal;
              var newRows = [];
              //iterate between users
              for (let j = 0; j < data.data.length; j++) {
                  var row = {};
                  let user = data.data[j];
                  row.Codpsw = user.Codpsw;
                  row.Nome = user.Nome;
                  row.UserRoles = user.privileges;
                  newRows.push(row);
              }
              vm.Users.rows = newRows;
          });
      },
      fetchData: function () {
          var vm = this;
          QUtils.log("Fetch data - Users");
          QUtils.FetchData(QUtils.apiActionURL('Users', 'Index')).done(function (data) {
              if (data.Success) {
                  // Update IdentityProviders list
                  vm.identityProviders = [];
                  $.each(data.model.IdentityProviders, function (idx, identityProvider) {
                      vm.identityProviders.push({ Value: identityProvider, Text: identityProvider });
                  });
                  vm.hasAdIdentityProviders = data.model.HasAdIdentityProviders;
              }
          });
          this.GetUserList();
      },
      onChangeQuery: function (queryParams) {
        this.Users.queryParams = queryParams;
        this.fetchData();
      },
      userColumns: function () {
        var vm = this;
        return [{
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
              name: "Nome",
              sort: true,
              initial_sort: true,
              initial_sort_order: "asc"
          },
          {
              label: () => vm.$t('ROLES61449'),
              name: "user-roles",
              slot_name: "user-roles",
              sort: false,
              visibility: !vm.columnModules
          },
          {
              label: () => vm.$t(vm.Modules.PRO.Description),
              name: "PRO",
              slot_name: "PRO",
              sort: false,
              visibility: vm.Modules.PRO.active && vm.columnModules
          },
          ];
      },

      ImportarUsersAD: function () {
        if (this.hasAdIdentityProviders === false) return

        var vm = this,
            domain = this.domainprovider;

        if ($.isEmptyObject(domain)) {
            bootbox.alert(vm.Resources.E_NECESSARIO_ESCOLHE33714);
            return;
        }

        var domainSplited = new Array();
        domainSplited = domain.split('=');

        if (domainSplited.length < 2 || $.isEmptyObject(domainSplited[1])) {
            bootbox.alert(vm.Resources.DOMINIO_IVALIDO_36204);
            return;
        }
        else {
            domain = domainSplited[1];
        }

        bootbox.confirm({
            message: vm.Resources.DESEJA_MESMO_MESMO_I42044,
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
                    QUtils.postData('Users', 'ImportUsersFromAD', null, { dominio: domain }, function (data) {
                        vm.fetchData();
                    });
                }
            }
        });
      }

    },
    created: function () {
      // [APM] Commented this line because the data was being fetched twice when the page was loaded.
      // this.fetchData();
    },
    watch: {
      // call again the method if the route changes
      '$route': 'fetchData',
      //Watcher to change the columns when the filters are checked
      Modules: {
          deep: true,
          immediate: true,
          handler() {
              this.Users.columns = this.userColumns();
          }
      }
    }
  };
</script>
