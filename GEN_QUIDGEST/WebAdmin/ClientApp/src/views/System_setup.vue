<template>
<div id="system_setup_container">
	<div class="q-stack--column">
		<h1 class="f-header__title">
			{{ Resources.CONFIGURACAO_DO_SIST39343 }}
		</h1>
	</div>
	<hr />

	<template v-if="!isEmptyObject(Model.ResultMsg)">
		<div class="alert alert-info">
			<p>{{ Resources.ESTADO_DA_OPERACAO38065 }}</p>
			<p><b class="status-message">{{ Model.ResultMsg }}</b></p>
		</div>
		<br />
	</template>

	<select class="form-control" style="float:right; width: 200px;" v-model="currentApp">
		<option v-for="app in Model.Applications" v-bind:value="app.Id" :key="app.Id">{{ app.Name }}</option>
	</select>



	<div>
		<ul class="nav nav-tabs c-tab c-tab__divider" id="system_setup_tabs" role="tablist">
			<li class="nav-item c-tab__item">
				<a class="nav-link c-tab__item-header active" id="database-tab" data-toggle="tab" data-target="#database" role="tab" aria-controls="database" aria-selected="true">{{ Resources.BASE_DE_DADOS58234 }}</a>
			</li>
			<li class="nav-item c-tab__item">
				<a class="nav-link c-tab__item-header" id="security-tab" data-toggle="tab" data-target="#security" role="tab" aria-controls="security" aria-selected="false">{{ Resources.SEGURANCA53664 }}</a>
			</li>
			<li class="nav-item c-tab__item">
				<a class="nav-link c-tab__item-header" id="audit-tab" data-toggle="tab" data-target="#audit" role="tab" aria-controls="audit" aria-selected="false">Audit</a>
			</li>
			<li class="nav-item c-tab__item">
				<a class="nav-link c-tab__item-header" id="paths-tab" data-toggle="tab" data-target="#paths" role="tab" aria-controls="paths" aria-selected="false">{{ Resources.APLICACAO16322 }}</a>
			</li>
			<li class="nav-item c-tab__item">
				<a class="nav-link c-tab__item-header" id="messaging-tab" data-toggle="tab" data-target="#messaging" role="tab" aria-controls="messaging" aria-selected="false">Messaging</a>
			</li>
			<li class="nav-item c-tab__item">
				<a class="nav-link c-tab__item-header" id="scheduler-tab" data-toggle="tab" data-target="#scheduler" role="tab" aria-controls="scheduler" aria-selected="false">Scheduler</a>
			</li>
			<li class="nav-item c-tab__item">
				<a class="nav-link c-tab__item-header" id="others-tab" data-toggle="tab" data-target="#others" role="tab" aria-controls="others" aria-selected="false">{{ Resources.MAIS25935 }}</a>
			</li>
		</ul>
		<div class="tab-content c-tab__item-container">
			<!--BD-->
			<div class="tab-pane c-tab__item-content active" id="database" ref="database" role="tabpanel" aria-labelledby="database-tab">
				<database v-if="Model && isActiveTab('database')" :model="Model" @updateModal="setModal"></database>
			</div>
			<!--Security-->
			<div class="tab-pane c-tab__item-content" id="security" ref="security" role="tabpanel" aria-labelledby="security-tab">
				<security v-if="Model.Security && isActiveTab('security')" :Model="Model.Security" :SelectLists="Model.SelectLists" @updateModal="fetchData" @updateUsers="updateUsers"></security>
			</div>
			<!--Audit-->
			<div class="tab-pane c-tab__item-content" id="audit" ref="audit" role="tabpanel" aria-labelledby="audit-tab">
				<audit v-if="Model && isActiveTab('audit')" :Model="Model"></audit>
			</div>
			<!--Path-->
			<div class="tab-pane c-tab__item-content" id="paths" ref="paths" role="tabpanel" aria-labelledby="paths-tab">
				<paths v-if="Paths && isActiveTab('paths')" :Model="Paths"></paths>
			</div>
			<!--Messaging-->
			<div class="tab-pane c-tab__item-content" id="messaging" ref="messaging" role="tabpanel" aria-labelledby="messaging-tab">
				<messaging v-if="Model && isActiveTab('messaging')"  :Model="Model.Messaging" :Metadata="Model.MessagingMetadata" @updateModal="fetchData"></messaging>
			</div>
			<!--Scheduler-->
			<div class="tab-pane c-tab__item-content" id="scheduler" ref="scheduler" role="tabpanel" aria-labelledby="scheduler-tab">
				<scheduler v-if="Model && isActiveTab('scheduler')" :Model="Model.Scheduler" :TaskList="Model.SelectLists.SchedulerTaskList" @updateModal="fetchData"></scheduler>
			</div>
			<!--Others-->
			<div class="tab-pane c-tab__item-content" id="others" ref="others" role="tabpanel" aria-labelledby="others-tab">
				<others v-if="Model && isActiveTab('others')" :Model="Model" :Cores="Cores" :SelectLists="Model.SelectLists" @updateModal="fetchData"></others>
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
	import { reactive } from 'vue';
	import database from './System_setup/Database.vue';
	import security from './System_setup/Security.vue';
	import audit from './System_setup/Audit.vue';
	import paths from './System_setup/Paths.vue';
	import others from './System_setup/Others.vue';
		import messaging from './System_setup/Messaging.vue';
	import scheduler from './System_setup/Scheduler.vue';

	export default {
    name: 'system_setup',
    mixins: [reusableMixin],
    components: { database, security, audit, paths, others, messaging, scheduler },
	data() {
		return {
			Model: {},
			activeTab: 'database'
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
