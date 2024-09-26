<template>
  <div id="notifications_container">
    <div class="q-stack--column">
			<h1 class="f-header__title">
			{{ Resources.SERVIDORES_DE_EMAIL15136 }}
			</h1>
		</div>
    <hr>
    <template v-if="!isEmptyObject(Model.ResultMsg)">
            <div class="alert alert-info">
                <p><b class="status-message">{{ Model.ResultMsg }}</b></p>
            </div>
            <br />
        </template>
    <row>
      <ul class="nav nav-tabs c-tab c-tab__divider" id="notifications_tabs" role="tablist">
        <li class="nav-item c-tab__item ">
          <a class="nav-link c-tab__item-header active" id="pmail-tab" data-toggle="tab" data-target="#pmail" role="tab" aria-controls="pmail" aria-selected="false">{{ Resources.SERVIDORES_DE_EMAIL15136 }}</a>
        </li>
        <li class="nav-item c-tab__item">
          <a class="nav-link c-tab__item-header" id="signatures-tab" data-toggle="tab" data-target="#signatures" role="tab" aria-controls="signatures" aria-selected="false">{{ Resources.ASSINATURAS_DE_EMAIL13716 }}</a>
        </li>
        <li class="nav-item c-tab__item">
          <a class="nav-link c-tab__item-header" id="more-tab" data-toggle="tab" data-target="#more" role="tab" aria-controls="more" aria-selected="false">{{ Resources.MAIS25935 }}</a>
        </li>
      </ul>
      <div class="tab-content c-tab__item-container">
        <!--Pmail-->
        <div class="tab-pane c-tab__item-content active" ref="pmail" id="pmail" role="tabpanel" aria-labelledby="pmail-tab">
          <notifications_pmail v-if="isActiveTab('pmail')" :EmailProperties="Model.emailProperties" @updateModal="fetchData"></notifications_pmail>
        </div>
        <!--Signatures-->
        <div class="tab-pane c-tab__item-content" ref="signatures" id="signatures" role="tabpanel" aria-labelledby="signatures-tab">
          <notifications_signatures v-if="isActiveTab('signatures')" :EmailSignatures="Model.emailSignatures" @updateModal="fetchData"></notifications_signatures>
        </div>

        <!--More-->
        <div class="tab-pane c-tab__item-content" ref="more" id="more" role="tabpanel" aria-labelledby="more-tab">
          <row>
            <select-input  v-model="Model.passwordRecovery" :options="Model.emailPropertiesList" :label="Resources.PASSWORD_RECOVERY53114"></select-input>
          </row>
          <row>
              <q-button
                b-style="primary"
                :label="Resources.GRAVAR45301"
                @click="SaveEmail" />
          </row>
        </div>

      </div>
    </row>

  </div>
</template>

<script>
  // @ is an alias to /src
  import { reusableMixin } from '@/mixins/mainMixin.js';
  import { QUtils } from '@/utils/mainUtils';

  import notifications_pmail from './Notifications/Pmail.vue';
  import notifications_signatures from './Notifications/Signatures.vue';

  export default {
    name: 'email',
    mixins: [reusableMixin],
    components: {
      notifications_pmail,
      notifications_signatures
    },
    data: function () {
      return {
        Model: {},
        activeTab: 'pmail'
      };
    },
    methods: {
      fetchData: function () {
        var vm = this;
        QUtils.log("Fetch data - Email");
        QUtils.FetchData(QUtils.apiActionURL('Email', 'Index')).done(function (data) {
          QUtils.log("Fetch data - OK (Email)", data);
          $.each(data, function (propName, value) { vm.Model[propName] = value; });
          vm.Model.emailPropertiesList = vm.Model.emailProperties.map(x => { return { Value: x.ValId, Text: x.ValId }; });
        });
      },
      isActiveTab: function (tabName) {
        return this.activeTab === tabName;
        },
      SaveEmail: function () {
        var vm = this;
          QUtils.log("ManageProperties - Request", QUtils.apiActionURL('Email', 'SaveEmail'));
          var params = { userRegistration: vm.Model.userRegistration, passwordRecovery : vm.Model.passwordRecovery};
            QUtils.postData('Email', 'SaveEmail', null, params, function (data) {
                vm.Model.ResultMsg = data.ResultMsg;
            });
        },
    },
    mounted: function () {
      var vm = this;
      vm.observer = new MutationObserver(mutations => {
        for (const m of mutations) {
          const newValue = m.target.getAttribute(m.attributeName);
          vm.$nextTick(() => {
            if (~newValue.indexOf('active')) {
              vm.activeTab = m.target.id;
            }
          });
        }
      });

      $.each(vm.$refs, function (ref) {
        vm.observer.observe(vm.$refs[ref], {
          attributes: true,
          attributeFilter: ['class'],
        });
      });
    },
    beforeUnmount() {
      this.observer.disconnect();
    },
    created: function () {
      // Ler dados
      this.fetchData();
    },
    watch: {
      // call again the method if the route changes
      '$route': 'fetchData'
    }
  };
</script>
