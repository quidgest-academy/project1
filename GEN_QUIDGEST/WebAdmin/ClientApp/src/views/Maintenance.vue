<template>
  <div id="maintenance_container">
    <div class="q-stack--column">
			<h1 class="f-header__title">
			{{ Resources.MANUTENCAO_DA_BASE_D10092 }}
			</h1>
		</div>
    <hr>
    <div>
      <ul class="nav nav-tabs c-tab c-tab__divider" id="maintenance_tabs" role="tablist">
        <li class="nav-item c-tab__item">
          <a class="nav-link c-tab__item-header active" id="index-tab" data-toggle="tab" data-target="#index" role="tab" aria-controls="index" aria-selected="true">{{ Resources.MANUTENCAO49776 }}</a>
        </li>
        <li class="nav-item c-tab__item">
          <a class="nav-link c-tab__item-header" id="backup-tab" data-toggle="tab" data-target="#backup" role="tab" aria-controls="backup" aria-selected="false">{{ Resources.BACKUP51008 }}</a>
        </li>
        <li class="nav-item c-tab__item">
          <a class="nav-link c-tab__item-header" id="security-tab" data-toggle="tab" data-target="#security" role="tab" aria-controls="security" aria-selected="false">{{ Resources.SEGURANCA53664 }}</a>
        </li>
        <li class="nav-item c-tab__item">
          <a class="nav-link c-tab__item-header" id="indexes-tab" data-toggle="tab" data-target="#indexes" role="tab" aria-controls="indexes" aria-selected="false">{{ Resources.INDICES58021 }}</a>
        </li>
        <li class="nav-item c-tab__item">
          <a class="nav-link c-tab__item-header" id="data_quality-tab" data-toggle="tab" data-target="#data_quality" role="tab" aria-controls="data_quality" aria-selected="false">{{ Resources.QUALIDADE_DE_DADOS10588 }}</a>
        </li>
        <li class="nav-item c-tab__item">
          <a class="nav-link c-tab__item-header" id="change_year-tab" data-toggle="tab" data-target="#change_year" role="tab" aria-controls="change_year" aria-selected="false">{{ Resources.MUDANCA_DE_ANO09709 }}</a>
        </li>
      </ul>
      <div class="tab-content c-tab__item-container">
        <!--Index-->
        <div class="tab-pane c-tab__item-content active" ref="index" id="index" role="tabpanel" aria-labelledby="index-tab">
          <maintenance_index v-if="isActiveTab('index')"></maintenance_index>
        </div>
        <!--Backup-->
        <div class="tab-pane c-tab__item-content" ref="backup" id="backup" role="tabpanel" aria-labelledby="backup-tab">
          <maintenance_backup v-if="isActiveTab('backup')"></maintenance_backup>
        </div>
        <!--Security-->
        <div class="tab-pane c-tab__item-content" ref="security" id="security" role="tabpanel" aria-labelledby="security-tab">
          <maintenance_security v-if="isActiveTab('security')"></maintenance_security>
        </div>
        <!--Indexes-->
        <div class="tab-pane c-tab__item-content" ref="indexes" id="indexes" role="tabpanel" aria-labelledby="indexes-tab">
          <maintenance_indexes v-if="isActiveTab('indexes')"></maintenance_indexes>
        </div>
        <!--Data Quality-->
        <div class="tab-pane c-tab__item-content" ref="data_quality" id="data_quality" role="tabpanel" aria-labelledby="data_quality-tab">
          <maintenance_data_quality v-if="isActiveTab('data_quality')"></maintenance_data_quality>
        </div>
        <!--Change Year-->
        <div class="tab-pane c-tab__item-content" ref="change_year" id="change_year" role="tabpanel" aria-labelledby="change_year-tab">
          <maintenance_change_year v-if="isActiveTab('change_year')"></maintenance_change_year>
        </div>
      </div>
    </div>
    <br />

  </div>
</template>

<script>
  // @ is an alias to /src
  import { reusableMixin } from '@/mixins/mainMixin';
  import { QUtils } from '@/utils/mainUtils';
  import maintenance_index from './Maintenance/Index.vue';
  import maintenance_backup from './Maintenance/Backup.vue';
  import maintenance_security from './Maintenance/Security.vue';
  import maintenance_indexes from './Maintenance/Indexes.vue';
  import maintenance_data_quality from './Maintenance/DataQuality.vue';
  import maintenance_change_year from './Maintenance/ChangeYear.vue';

  export default {
    name: 'maintenance',
    components: {
      maintenance_index,
      maintenance_backup,
      maintenance_security,
      maintenance_indexes,
      maintenance_data_quality,
      maintenance_change_year,
    },
    mixins: [reusableMixin],
    data: function () {
      return {
        Model: {},
        activeTab: 'index'
      };
    },
    methods: {
      isActiveTab: function (tabName) {
        return this.activeTab === tabName;
      },
      fetchData: function () {
        var vm = this;
        QUtils.log("Fetch data - DbAdmin", QUtils.apiActionURL('DbAdmin', 'Index'));
        QUtils.FetchData(QUtils.apiActionURL('DbAdmin', 'Index')).done(function (data) {
          QUtils.log("Fetch data - OK (DbAdmin)", data);
          if(data.redirect) {
            vm.$router.replace({ name: data.redirect, params: { culture: vm.currentLang, system: vm.currentYear } });
          }
        });
      },
    },
    created: function () {
      // Ler dados
      this.fetchData();
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
    }
  };
</script>
