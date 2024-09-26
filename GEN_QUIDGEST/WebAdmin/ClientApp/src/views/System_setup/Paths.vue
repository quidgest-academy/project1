<template>
    <div id="system_setup_paths_container">
        <br />
        <card>
            <template #header>
                {{Resources.CAMINHOS41141}} ({{Model.app}})
            </template>
            <template #body>
                <row>
                    <text-input v-model="Model.pathApp" :label="Resources.CAMINHO_PARA_A_APLIC44450"></text-input>
                </row>
                <row>
                    <text-input v-model="Model.pathDocuments" :label="Resources.CAMINHO_PARA_DOCUMEN18456"></text-input>
                </row>
            </template>
        </card>
        <br />
        <row>
            <div class="q-button-container">
                <q-button
                b-style="primary"
                :label="Resources.GRAVAR45301"
                @click="SavePathCfg" />
                <a
                    class="btn btn-secondary glyphicons glyphicons-download-alt"
                    :href="DownloadRedirect">
                    Configuracoes.redirect.xml
                </a>
            </div>
        </row>
    </div>
</template>

<script>
  // @ is an alias to /src
  import { reusableMixin } from '@/mixins/mainMixin';
  import { QUtils } from '@/utils/mainUtils';
  import bootbox from 'bootbox';

  export default {
    name: 'paths',
    props: {
      Model: {
        required: true
      }
    },
    mixins: [reusableMixin],
    data: function () {
      return {
      };
    },
    methods: {
      SavePathCfg: function () {
        var vm = this;
        QUtils.log("SavePathCfg - Request", QUtils.apiActionURL('Config', 'SavePathCfg'));
        QUtils.postData('Config', 'SavePathCfg', vm.Model, null, function (data) {
          QUtils.log("SavePathCfg - Response", data);
          if (data.Success) {
            bootbox.alert(vm.Resources.ALTERACOES_EFECTUADA64514);
          }
          else {
            bootbox.alert(data.Message);
          }
        });
       }
    },
    computed: {
      DownloadRedirect: function () {
        return QUtils.apiActionURL('Config', 'DownloadRedirect');
      }
    }
  };
</script>
