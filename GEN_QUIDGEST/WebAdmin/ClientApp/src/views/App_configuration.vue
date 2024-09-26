<template>
	<div id="app_config">
		<div class="q-stack--column">
			<h1 class="f-header__title">
			{{ Resources.CONFIGURACAO_DA_APLI59110 }}
			</h1>
		</div>
		<hr />
		<div>
			<ul class="nav nav-tabs c-tab c-tab__divider" id="system_setup_tabs" role="tablist">
				<li class="nav-item c-tab__item">
					<a class="nav-link c-tab__item-header active" id="security-tab" data-toggle="tab" data-target="#security" role="tab" aria-controls="security" aria-selected="true">{{ Resources.SEGURANCA53664 }}</a>
				</li>
				<li class="nav-item c-tab__item">
					<a class="nav-link c-tab__item-header" id="paths-tab" data-toggle="tab" data-target="#paths" role="tab" aria-controls="paths" aria-selected="false">{{ Resources.CAMINHOS41141 }}</a>
				</li>
			</ul>
			<div class="tab-content c-tab__item-container">
				<!--Security-->
				<div class="tab-pane c-tab__item-content active" id="security" ref="security" role="tabpanel" aria-labelledby="security-tab">
					<security v-if="Model.Security && isActiveTab('security')" :Model="Model.Security" :SelectLists="Model.SelectLists" @updateModal="fetchData" @updateUsers="updateUsers"></security>
				</div>
				<!--Path-->
				<div class="tab-pane c-tab__item-content" id="paths" ref="paths" role="tabpanel" aria-labelledby="paths-tab">
					<paths v-if="Paths && isActiveTab('paths')" :Model="Paths"></paths>
				</div>
			</div>
		</div>
	</div>
</template>

<script>
// @ is an alias to /src
import { reusableMixin } from '@/mixins/mainMixin';
import { QUtils } from '@/utils/mainUtils';
import { reactive } from 'vue';
import security from './System_setup/Security.vue';
import paths from './System_setup/Paths.vue';

export default {
	name: 'system_setup',
	mixins: [reusableMixin],
	components: { security, paths },
	data() {
		return {
			Model: {},
			activeTab: 'security'
		};
	},
	computed: {
		Paths() {
			var vm = this;
			if ($.isEmptyObject(vm.currentApp) || $.isEmptyObject(vm.Model.Paths))
			return null;
			vm.Model.Paths[vm.currentApp].app = vm.currentApp;
			return vm.Model.Paths[vm.currentApp] || null;
		},
		Cores() {
			var vm = this;
			return !$.isEmptyObject(vm.currentApp) && !$.isEmptyObject(vm.Model.Cores) ? (vm.Model.Cores[vm.currentApp] || null) : null;
		}
	},
	methods: {
		isActiveTab(tabName) {
			return this.activeTab === tabName;
		},
		fetchData() {
			var vm = this;
			QUtils.log("Fetch data - Config", QUtils.apiActionURL('Config', 'Index'));
			QUtils.FetchData(QUtils.apiActionURL('Config', 'Index')).done(function (data) {
			QUtils.log("Fetch data - OK (Config)", data);
			if(data.redirect) {
				vm.$router.replace({ name: data.redirect, params: { culture: vm.currentLang, system: vm.currentYear } });
			}
			else if (data.reload) {
				vm.currentYear = data.system;
				vm.fetchData();
			}
			else {
				vm.setModal(data);
			}
			});
		},
		setModal(data) {
			var vm = this;
			$.extend(vm.Model, data);
			// Select the first exists application
			if ($.isEmptyObject(vm.currentApp) && !$.isEmptyObject(vm.Model.Applications)) {
			vm.currentApp = vm.Model.Applications[0].Id;
			}
			// Focus on errors div
			if (!$.isEmptyObject(vm.Model.ResultMsg)) {
			window.scrollTo(0,0);
			}
		},
		reloadMQueues() {
			var vm = this;
			QUtils.FetchData(QUtils.apiActionURL('Config', 'ReloadMQueues')).done(function (data) {
				if (data.Success) {
					$.each(data.MQueues, function (propName, value) {
						if ($.isArray(vm.Model.MQueues[propName])) { vm.Model.MQueues[propName].splice(0); }
						$.extend(vm.Model.MQueues[propName], value);
					});
				}
			});
		},
		updateUsers(eventData) {
			if ($.isEmptyObject(this.Model.Security[eventData.currentApp].Users))
				$.extend(this.Model.Security[eventData.currentApp], reactive({ Users: [] }));
			else
				this.Model.Security[eventData.currentApp].Users.splice(0);

			$.extend(this.Model.Security[eventData.currentApp].Users, eventData.users);
		}
	},
	mounted() {
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
	created() {
		// Ler dados
		this.fetchData();
	},
	watch: {
		// call again the method if the route changes
		'$route': 'fetchData',
		'currentApp': 'fetchData'
	}
};
</script>
