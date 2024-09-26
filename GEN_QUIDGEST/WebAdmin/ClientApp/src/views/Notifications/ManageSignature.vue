<template>
    <div id="notifications_signature_container">
        <h1 class="f-header__title">{{ Resources.ASSINATURA_DE_EMAIL58979 }}</h1>
        <template v-if="!isEmptyObject(Model.ResultMsg)">
            <div class="alert alert-info">
                <p><b class="status-message">{{ Model.ResultMsg }}</b></p>
            </div>
            <br />
        </template>
        <template v-if="Model.FormMode==3">
            <div class="alert alert-block">
                <strong>Warning!</strong>
                {{ Resources.DESEJA_ELIMINAR_ESTA24564 }}
            </div>
        </template>

        <row>
            <card>
                <template #header>
                    {{ Resources.DADOS_DO_REGISTO10198 }}
                </template>
                <template #body>
                    <row>
                        <text-input v-model="Model.ValName" :label="Resources.NOME47814" :isReadOnly="blockForm"></text-input>
                    </row>
                    <row>
                        <image-input :getUrl="getEmailSignatureImage" :setUrl="setEmailSignatureImage" :label="Resources.IMAGEM19513"></image-input>
                    </row>
                    <row>
                        <text-input v-model="Model.ValTextass" :label="Resources.TEXTO_APOS_A_ASSINAT02808" :isReadOnly="blockForm"></text-input>
                    </row>
                </template>
            </card>
        </row>

        <row>
            <q-button
                v-if="Model.FormMode !== 3"
                b-style="primary"
                :label="Resources.GRAVAR_CONFIGURACAO36308"
                @click="SaveSignature" />
            <q-button
                v-else
                b-style="danger"
                :label="Resources.APAGAR04097"
                @click="SaveSignature" />
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

  import _get from "lodash-es/get";

    export default {
        name: 'notifications_signature',
        mixins: [reusableMixin],
        data: function () {
            //var vm = this;
            return {
                Model: {}
            };
        },
        computed: {
            blockForm: function () { return this.Model.FormMode == 3; },
            getEmailSignatureImage: function () {
                var codsigna = _get(this.$route.params, 'codsigna', null);
                if ($.isEmptyObject(codsigna)) { codsigna = this.Model.ValCodsigna; }
                return QUtils.apiActionURL('Email', 'getEmailSignatureImage', { key: codsigna });
            },
            setEmailSignatureImage: function () {
                return QUtils.apiActionURL('Email', 'setEmailSignatureImage', { key: this.Model.ValCodsigna });
            }
        },
        methods: {
            fetchData: function () {
                var vm = this,
                    mod = vm.$route.params.mod,
                    codsigna = _get(vm.$route.params, 'codsigna', null);
                QUtils.log("Fetch data - ManageSignature");
                QUtils.FetchData(QUtils.apiActionURL('Email', 'ManageSignature', { mod, codsigna })).done(function (data) {
                    QUtils.log("Fetch data - OK (ManageSignature)", data);
                    if (data.Success) {
                        $.each(data.model, function (propName, value) { vm.Model[propName] = value; });
                    }
                    else {
                        bootbox.alert(data.Message, function () {
                            vm.$router.replace({ name: 'notifications', params: { culture: vm.currentLang, system: vm.currentYear } });
                        })
                    }
                });
            },
            cancel: function () {
                var vm = this;
                vm.$router.replace({ name: 'email', params: { culture: vm.currentLang, system: vm.currentYear } });
            },
            SaveSignature: function () {
                var vm = this;
                QUtils.log("ManageSignature - Request", QUtils.apiActionURL('Notifications', 'SaveSignature'));
                  QUtils.postData('Email', 'SaveSignature', vm.Model, null, function (data) {
                      QUtils.log("ManageSignature - Response", data);
                      if (data.Success) {
                          vm.$router.replace({ name: 'email', params: { culture: vm.currentLang, system: vm.currentYear } });
                      }
                      else {
                          $.each(data.model, function (propName, value) { vm.Model[propName] = value; });
                      }
                  });
            }
        },
        created: function () {
            // Ler dados
            this.fetchData();
        }
    };
</script>
