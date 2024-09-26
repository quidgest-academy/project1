<template>
  <div id="user_management_container">
    <h1 class="f-header__title">{{ Resources.UTILIZADOR52387 }}</h1>
    <row v-if="!isEmptyObject(Model.ResultMsg)">
      <div class="alert alert-danger">
        <span>
          <b class="status-message">{{ Model.ResultMsg }}</b>
        </span>
      </div>
      <br />
    </row>
    <row>
      <checkbox-input v-model="Model.StatusDisableLogin" :label="Resources.DESACTIVAR_CONTA37602" :isReadOnly="lockControls"></checkbox-input>
    </row>
    <row>
      <text-input ref="username" v-model="Model.Username" :label="Resources.NOME__48276" :isReadOnly="Model.ModForm != '1'"></text-input>
    </row>
    <row>
      <text-input v-model="Model.Email" :label="Resources.EMAIL25170" :isReadOnly="lockControls"></text-input>
    </row>
    <row>
      <text-input v-model="Model.Phone" :label="Resources.TELEFONE37757" :isReadOnly="lockControls"></text-input>
    </row>
    <row>
      <checkbox-input v-model="Model.StatusFirstLogin" :label="Resources.O_UTILIZADOR_TEM_QUE05121" :isReadOnly="lockControls"></checkbox-input>
    </row>
    <row v-if="Model.ShowInvalidate2FA">
      <checkbox-input v-model="Model.Invalidate2FA" :label="Resources.INVALIDAR_AUTENTICAC21095" :isReadOnly="lockControls || Model.BlockInvalidate2FA"></checkbox-input>
    </row>
    <row>
      <checkbox-input v-model="Model.PasswordChange" :label="Resources.ALTERAR_A_PALAVRA_CH54014" :isReadOnly="lockControls || !hasQuidgestIDProvider"></checkbox-input>
    </row>
    <row>
      <password-input v-model="Model.PasswordNew" :label="Resources.NOVA_15272" :isReadOnly="lockControls || !Model.PasswordChange"
               :style='{ "opacity": (!Model.PasswordChange) ? "0.5" : "1" }'
               :disabled="!Model.PasswordChange"></password-input>
    </row>
    <row>
      <password-input v-model="Model.PasswordConfirm" :label="Resources.CONFIRMAR_64824" :isReadOnly="lockControls  || !Model.PasswordChange"
               :style='{ "opacity": (!Model.PasswordChange) ? "0.5" : "1" }'
               :disabled="!Model.PasswordChange"></password-input>

      <div v-show="Model.PasswordChange" ref="PassMeter" id="passMeter">
        <meter ref="pswStrengthMeter" max="4" id="password-strength-meter" value="0"></meter>
        <p ref="pswStrengthText" id="password-strength-text"></p>
      </div>
    </row>
    <hr />
    <row>
        <qtable id="PrivilegeTable"
                    :rows="Model.Modules"
                    :columns="tPrivileges.columns"
                    :config="tPrivileges.config"
                    :totalRows="totalPrivileges">

            <template #permission="props">
                <multiselect v-model="Model.AssignedRoles[props.row.Cod]" :options="Model.AvaiableRoles[props.row.Cod]" :multiple="!Model.OnlyLevels" :taggable="true"
                             :max="props.row.OnlyLevels?1:999"  :custom-label="roleName" label="Designation" trackBy="Role">
                </multiselect>
            </template>

        </qtable>
    </row>
    <row>
      <div class="q-button-container">
        <q-button
            ref="submitBtn"
            b-style="primary"
            :label="Resources[Model.SubmitValue]"
            :disabled="submitBtnLock"
            @click="submit" />
        <q-button
            :label="Resources.CANCELAR49513"
            @click="cancelConfig" />
      </div>
    </row>
  </div>
</template>

<script>
  // @ is an alias to /src
  import { reusableMixin } from '@/mixins/mainMixin';
  import { QUtils } from '@/utils/mainUtils';
  import bootbox from 'bootbox';
  import Multiselect from '@/components/multiselect/multiselect_main';

  import _get from "lodash-es/get";

  export default {
    name: 'user_management',
    components: { 'multiselect': Multiselect },
    mixins: [reusableMixin],
        data: function () {
            var vm = this;
            return {
                Model: {},
                tPrivileges: {
                    columns: [
                        {
                            label: () => vm.$t('SIGLA14738'),
                            name: "Cod",
                            sort: true
                        },
                        {
                            label: () => vm.$t('MODULO43834'),
                            name: "Description",
                            sort: true
                        },
                        {
                            label: () => vm.$t('PERMISSOES38545'),
                            slot_name: "permission"
                        }],
                    config: {
                        table_title: () => vm.$t('PERMISSOES38545'),
                        pagination: false,
                        pagination_info: false,
                        server_mode: true,
                        preservePageOnDataChange: true,
                        highlight_row_hover: false
                    }
                },
                identityProviders: [],
            };
        },
    computed: {
      lockControls: function () { return this.$route.params.mod == 3; },
      totalPrivileges: function () { if (!$.isEmptyObject(this.Model.Privileges)) return this.Model.Privileges.length; return 0; },
      submitBtnLock: function () {
        var vm = this;
        return $.isEmptyObject(vm.Model.Username) ||
          (vm.Model.PasswordChange && ($.isEmptyObject(vm.Model.PasswordNew) || $.isEmptyObject(vm.Model.PasswordConfirm) ||
            vm.Model.PasswordNew !== vm.Model.PasswordConfirm));
      },
      passwordStrength: function () {
        var vm = this,
        //calcular strenght da password
          score = vm.scorePassword(vm.Model.PasswordNew),
          scoreStrenght = 0,
          pswStrengthMeter = $(vm.$refs.pswStrengthMeter),
          pswStrengthText = $(vm.$refs.pswStrengthText);

        if ($.isEmptyObject(vm.Model.PasswordNew)) {
          scoreStrenght = 0;
          pswStrengthMeter.text('');
        }
        else {
          if (score > 80) {
            scoreStrenght = 4;
            pswStrengthText.text(vm.Resources.FORTE13835);
          } else if (score > 60) {
            scoreStrenght = 3;
            pswStrengthText.text(vm.Resources.BOM29058);
          } else if (score >= 30) {
            scoreStrenght = 2;
            pswStrengthText.text(vm.Resources.FRACO22195);
          } else if (score < 30) {
            scoreStrenght = 1;
            pswStrengthText.text(vm.Resources.POBRE46544);
          }
        }
        pswStrengthMeter.val(scoreStrenght);

        return scoreStrenght;
      },
      hasQuidgestIDProvider() {
        for (var key in this.identityProviders) {
            if (this.identityProviders[key] === "GenioServer.security.QuidgestIdentityProvider") { return true; }
        }
        return false;
      }
    },
    methods: {
      fetchData: function () {
        var vm = this;
        const mod = vm.$route.params.mod,
              cod = _get(vm.$route.params, 'cod', null);
        QUtils.log("Fetch data - User management");
        QUtils.FetchData(QUtils.apiActionURL('ManageUsers', 'Index', { mod, cod })).done(function (data) {
          QUtils.log("Fetch data - OK (User management)", data);

          if (data.Success) {
            vm.Model = data.model;

            // Update IdentityProviders list
            vm.identityProviders = [];
            $.each(data.model.IdentityProviders, function (idx, identityProvider) {
             vm.identityProviders.push(identityProvider);
            });
          }
          else if (data.redirect) {
            vm.$router.replace({ name: data.redirect });
          }
        });
      },
      cancelConfig: function () {
        var vm = this;

        if (!vm.Model.Username) {
          this.$router.replace({ name: 'users', params: { culture: vm.currentLang, system: vm.currentYear } });
          return;
        }

         bootbox.confirm({
           message: vm.Resources.ATENCAO__AS_ALTERACO04365,
           buttons: {
             confirm: {
               label: vm.Resources.SIM28552,
               className: 'b-icon-text b-icon-text--primary'
             },
             cancel: {
               label: vm.Resources.NAO06521,
               className: 'b-icon-text b-icon-text--secondary'
             }
           },
           callback: function (result) {
             if (result) {
               vm.$router.replace({ name: 'users', params: { culture: vm.currentLang, system: vm.currentYear } });
             }
           }
         });
      },
      submit: function () {
        var vm = this;
        const mod = vm.$route.params.mod,
              cod = _get(vm.$route.params, 'cod', null);
        QUtils.postData('ManageUsers', 'SaveConfig', vm.Model, { mod, cod }, function (data) {
          if (data.Success) {
                if (data.ignoredRoles.length == 0) {
                    vm.$router.replace({ name: 'users', params: { culture: vm.currentLang, system: vm.currentYear } });
                }
                else {
                    var message = vm.Resources.ATENCAO__ALGUMAS_PER50140 + "<br>";
                    for (var i = 0; i < data.ignoredRoles.length; i++)
                    {
                        var pair = data.ignoredRoles[i];
                        var MESSAGE = vm.Resources.AT_CHILD_FOI_IGNORADO_31949;
                        var roleMessage = MESSAGE.replace("@child", "<b>"+ vm.Resources[pair.child] + "</b>");
                        roleMessage = roleMessage.replace("@parent","<b>"+ vm.Resources[pair.parent]+ "</b>");
                        message += roleMessage + "<br>";
                    }
                     bootbox.alert(message, function () {
                        vm.$router.replace({ name: 'users', params: { culture: vm.currentLang, system: vm.currentYear } });
                     });
                }
          }
          else {
              $.each(data.model, function (propName, value) { vm.Model[propName] = value; });
          }
        });
      },
      scorePassword: function (pass) {
        var score = 0;
        if (!pass)
          return score;

        // award every unique letter until 5 repetitions
        var letters = new Object();
        for (var i = 0; i < pass.length; i++) {
          letters[pass[i]] = (letters[pass[i]] || 0) + 1;
          score += 5.0 / letters[pass[i]];
        }

        // bonus points for mixing it up
        var variations = {
          digits: /\d/.test(pass),
          lower: /[a-z]/.test(pass),
          upper: /[A-Z]/.test(pass),
          nonWords: /\W/.test(pass),
        }

        var variationCount = 0;
        for (var check in variations) {
          variationCount += (variations[check] == true) ? 1 : 0;
        }
        score += (variationCount - 1) * 10;

        return parseInt(score);
      },
      roleName : function (role) {
        return this.Resources[role.Designation];
      }
    },
    mounted: function () {
      var vm = this;
      $(vm.$refs.username).focus();
    },
    created: function () {
      // Ler dados
      this.fetchData();
    },
    watch: {
      // call again the method if the route changes
      '$route': 'fetchData',
      'Model.PasswordChange': {
        handler() {
          this.Model.PasswordNew = "";
          this.Model.PasswordConfirm = "";
        },
        deep: true
      },
      'Model.PasswordNew': {
        handler() {
          var vm = this,
            //calcular strenght da password
            score = vm.scorePassword(vm.Model.PasswordNew),
            scoreStrenght = 0,
            pswStrengthMeter = $(vm.$refs.pswStrengthMeter),
            pswStrengthText = $(vm.$refs.pswStrengthText);

          if ($.isEmptyObject(vm.Model.PasswordNew)) {
            scoreStrenght = 0;
            pswStrengthMeter.text('');
          }
          else {
            if (score > 80) {
              scoreStrenght = 4;
              pswStrengthText.text(vm.Resources.FORTE13835);
            } else if (score > 60) {
              scoreStrenght = 3;
              pswStrengthText.text(vm.Resources.BOM29058);
            } else if (score >= 30) {
              scoreStrenght = 2;
              pswStrengthText.text(vm.Resources.FRACO22195);
            } else if (score < 30) {
              scoreStrenght = 1;
              pswStrengthText.text(vm.Resources.POBRE46544);
            }
          }
          pswStrengthMeter.val(scoreStrenght);
        },
        deep: true
      }
    }
  };
</script>
<style >
    /*Permissions table must be fixed, since the content will be changed */
    #PrivilegeTable .c-table {
        table-layout : fixed;
    }
    #PrivilegeTable th:nth-child(1) {
        width: 10%;
    }
    #PrivilegeTable th:nth-child(2) {
        width: 30%;
    }
</style>
<style scoped>
  meter {
    width: 100%;
    height: 10px;
  }

    /* WebKit */
    meter::-webkit-meter-bar {
      background: #EEE;
      box-shadow: 0 2px 3px rgba(0,0,0,0.2) inset;
      border-radius: 3px;
    }

    /* Webkit based browsers */
    meter::-webkit-meter-optimum-value {
      transition: width .4s linear;
    }

    meter[value="0"]::-webkit-meter-optimum-value {
      background: grey;
    }

    meter[value="1"]::-webkit-meter-optimum-value {
      background: red;
    }

    meter[value="2"]::-webkit-meter-optimum-value {
      background: orange;
    }

    meter[value="3"]::-webkit-meter-optimum-value {
      background: yellow;
    }

    meter[value="4"]::-webkit-meter-optimum-value {
      background: green;
    }

    /* Gecko based browsers */
    meter[value="0"]::-moz-meter-bar {
      background: grey;
    }

    meter[value="1"]::-moz-meter-bar {
      background: red;
    }

    meter[value="2"]::-moz-meter-bar {
      background: orange;
    }

    meter[value="3"]::-moz-meter-bar {
      background: yellow;
    }

    meter[value="4"]::-moz-meter-bar {
      background: green;
    }

</style>
