<template>
  <div id="system_setup_audit_container">
    <br />
    <row>
      <checkbox-input v-model="Model.RegistLoginOut" :label="Resources.AUDITORIA_DE_LOGIN_D00905"></checkbox-input>
    </row>
    <row>
      <checkbox-input v-model="Model.RegistActions" :label="Resources.AUDITORIA_DE_ACOES_D42106"></checkbox-input>
    </row>
    <row>
      <checkbox-input v-model="Model.AuditInterface" :label="Resources.AUDITORIA_DO_SISTEMA08460"></checkbox-input>
    </row>
    <br />
    <row>
      <checkbox-input v-model="Model.EventTracking" :label="Resources.REGISTO_DE_EVENTOS65341"></checkbox-input>
    </row>
    <br />
    <row>
      <q-button
        b-style="primary"
        :label="Resources.GRAVAR45301"
        @click="SaveConfigAudit" />
    </row>
  </div>
</template>

<script>
  // @ is an alias to /src
  import { reusableMixin } from '@/mixins/mainMixin';
  import { QUtils } from '@/utils/mainUtils';
  import bootbox from 'bootbox';

  export default {
    name: 'audit',
    props: {
      Model: {
        required: true
      }
    },
    mixins: [reusableMixin],
    data: function () {
      return {
        temp: {}
      };
    },
    methods: {
      SaveConfigAudit: function () {
        var vm = this;
        QUtils.log("SaveConfigAudit - Request", QUtils.apiActionURL('Config', 'SaveConfigAudit'));
        QUtils.postData('Config', 'SaveConfigAudit', vm.Model, null, function (data) {
          QUtils.log("SaveConfigAudit - Response", data);
          if (data.Success) {
            bootbox.alert(vm.Resources.ALTERACOES_EFECTUADA64514);
          }
          else {
            bootbox.alert(data.Message);
          }
        });
      }
    }
  };
</script>
