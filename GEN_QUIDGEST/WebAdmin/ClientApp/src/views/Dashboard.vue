<template>
	<div id="dashboard_container">
		<div class="q-stack--column">
			<h1 class="f-header__title">
			{{ Resources.DASHBOARD51597 }}
				<!-- <q-button v-if="Model.HasConfig"
					b-style="primary"
					class="ml-auto"
					:label="maintenanceBtnText"
					@click="ScheduleMaintenance" /> -->
			</h1>
		</div>
		<hr />
		<div v-if="!loaded" class="card text-center">
			<div class="card-body">
				<span class="feature-icon glyphicons glyphicons-hourglass"></span>
				<h4>{{ Resources.A_CARREGAR___34906 }}</h4>
			</div>
		</div>
		<div v-else>
			<div v-if="!Model.HasConfig" class="alert alert-danger">
				<h4>{{ Resources.NO_CONFIGURATION_FIL40493 }}</h4>
				<p><span v-html="Model.ResultErrors"></span></p>
				<q-button
					:label="Resources.CRIAR_CONFIGURACAO_D17273"
					@click.stop="createNewConfig" />
			</div>
			<div v-if="Model.ResultErrors && Model.HasConfig" class="alert alert-danger">
				<h4>{{ Resources.ERRO_35877 }}</h4>
				<p><span v-html="Model.ResultErrors"></span></p>
				<q-button
					v-if="showDBButton()"
					:label="Resources.REINDEXAR_BASE_DE_DA52902"
					@click.stop="navigateTo($event, 'maintenance')" />
			</div>
			<br>

			<div v-if="Model.IsBetaTestig" class="alert alert-warning">
				<p></p>
				<p><b>{{ Resources.AMBIENTE_DE_QUALIDAD42119 }}</b></p>
			</div>

			<div v-if="needsBasicConfig()">
				<h4>{{ Resources.CONFIGURAR_O_SEU_PRO44930 }}</h4>
				<div class="row">
					<div class="col-auto" >
						<div id="card-system"
							:class="getCardClass('card-system')"
							@click.stop="Model.HasConfig ? navigateTo($event, 'system_setup') : null">
							<span class="mdi mdi-tools"></span>
							<div class="c-card__title">{{ Resources.CONFIGURACAO_DO_SIST39343 }}</div>
						</div>
						<q-tooltip
							v-if="!Model.HasConfig"
							anchor="#card-system"
							:text="Resources.ESTA_ACAO_ESTA_BLOQU25038" />
						<h4 class="q-type__subtitle">
							<span>1&#46;</span>
							{{ Resources.CONFIGURAR_O_SEU_PRO44930 }}
							<span class="mdi mdi-check-bold"></span>
						</h4>
					</div>
					<div class="col-auto">
						<div id="card-database"
							:class="getCardClass('card-database')"
							@click.stop="Model.HasConfig ? navigateTo($event, 'maintenance') : null">
							<span class="mdi mdi-database-cog"></span>
							<div class="c-card__title">{{ Resources.MANUTENCAO_DA_BASE_D10092 }}</div>
						</div>
						<q-tooltip
							v-if="!Model.HasConfig"
							anchor="#card-database"
							:text="Resources.ESTA_ACAO_ESTA_BLOQU25038" />
						<h4 class="q-type__subtitle">
							<span>2&#46;</span>
							{{ Resources.ATUALIZAR_A_BASE_DE_60308 }}
							<span class="mdi mdi-check-bold"></span>
						</h4>
					</div>
					<div class="col-auto">
						<div id="card-user"
							:class="getCardClass('card-user')"
							@click.stop="Model.HasConfig ? navigateTo($event, 'users') : null">
							<span class="mdi mdi-account-cog"></span>
							<div class="c-card__title">{{ Resources.GESTAO_DE_UTILIZADOR20428 }}</div>
						</div>
						<q-tooltip
							v-if="!Model.HasConfig"
							anchor="#card-user"
							:text="Resources.ESTA_ACAO_ESTA_BLOQU25038" />
						<h4 class="q-type__subtitle">
							<span>3&#46;</span>
							{{ Resources.CRIAR_PERFIL_DE_UTIL50057 }}
							<span class="mdi mdi-check-bold"></span>
						</h4>
					</div>
				</div>
			</div>

			<!-- INFORMATION -->
			<div v-else>
				<div class="row">
					<div class="col-12">
						<div class="control-row-group">
							<QGroupBoxContainer :label="Resources.SOBRE44896">
								<div class="container-fluid">
									<dl class="row">
										<dt>{{ Resources.SISTEMA05814 }}</dt>
										<dd>PRO</dd>
										<dt>{{ Resources.ACRONIMO12609 }}</dt>
										<dd>QUIDGEST</dd>
										<dt>{{ Resources.CLIENTE40500 }}</dt>
										<dd>Quidgest</dd>
									</dl>
									<dl class="row">
										<dt>{{ Resources.VERSAO_DE_SISTEMA07287 }}</dt>
										<dd>24</dd>
										<dt>{{ Resources.VERSAO_DE_BASE_DE_DA46937 }}</dt>
										<dd>{{ Model.VersionDbGen }}</dd>
										<dt>{{ Resources.APP_MIGRATION_VERSIO41495 }}</dt>
										<dd>0</dd>
										<dt>{{ Resources.VERSAO_DOS_INDICES49454 }}</dt>
										<dd>{{ Model.VersionIdxDbGen }}</dd>
										<dt>{{ Resources.VERSAO_DE_GENIO44840 }}</dt>
										<dd>349.60</dd>
										<dt>{{ Resources.GERADO_EM27272 }}</dt>
										<dd>09/26/2024</dd>
									</dl>
									<dl class="row">
										<span class="app-brand">
											<img src="@/assets/img/f-login__brand.png">
										</span>
									</dl> 
								</div>
							</QGroupBoxContainer>
						</div>
					</div>
				</div>
				<div class="row">
					<div class="col-12">
						<div class="control-row-group">
							<QGroupBoxContainer :label="Resources.AMBIENTE12083">
								<dl class="wa-environment">
									<dt>{{ Resources.SERVIDOR_DE_SGBD19838 }}</dt>
									<dd>{{ Model.SGBDServer }}</dd>
									<dt>{{ Resources.SGBD26061 }}</dt>
									<dd>{{ Model.TpSGBD }}</dd>
									<dt>{{ Resources.VERSAO_DO_SGBD43730 }}</dt>
									<dd>{{ Model.SGBDVersion }}
										<span
											v-if="Model.HasSGBDVersion"
											style="color:red;">&nbsp;
											<i class="glyphicons glyphicons-exclamation-sign" aria-hidden="true"></i>
										</span>
									</dd>
									<dt>{{ Resources.BASE_DE_DADOS58234 }}</dt>
									<dd>{{ Model.DBSchema }}</dd>
									<dt>{{ Resources.TAMANHO_DA_BD56664 }}</dt>
									<dd>{{ Model.DBSize }} MB</dd>
									<dt class="version">{{ Resources.VERSAO_DA_BD12683 }}</dt>
									<dd class="version">{{ Model.VersionDb }}
										<span class="icon" v-if="Model.HasDiffIdxVersion">
											&nbsp;
											<span class="mdi mdi-alert-circle"></span>
										</span>
									</dd>
									<dt>{{ Resources.COMPUTADOR39938 }}</dt>
									<dd>{{ Model.PCDesc }}</dd>
									<dt>{{ Resources.SISTEMA_OPERATIVO30480 }}</dt>
									<dd>{{ Model.SODesc }}</dd>
									<dt>{{ Resources.PROCESSADOR36325 }}</dt>
									<dd>{{ Model.HardwProcDesc }}</dd>
									<dt>{{ Resources.MEMORIA09056 }}</dt>
									<dd>{{ Model.HardwMemDesc }}</dd>
									<dt>{{ Resources.DRIVES34119 }}</dt>
									<dd>
										<span v-html="Model.HardwDrivDesc"></span>
									</dd>
								</dl>
							</QGroupBoxContainer>
						</div>
					</div>
				</div>
				<div class="row">
					<div class="col-12">
						<div class="control-row-group">
							<QGroupBoxContainer :label="Resources.CONTACTOS35567">
								<address>
									<strong>Quidgest, S.A.</strong>
									Rua Viriato, 7  1050-233 Lisboa &#124; Portugal  &#124;  (+351) 213 870 563
									<a href="mailto:quidgest@quidgest.com">quidgest@quidgest.com</a>
								</address>
							</QGroupBoxContainer>
						</div>
					</div>
				</div>
			</div>
		</div>
	</div>
</template>

<script>
// @ is an alias to /src
import { reusableMixin } from '@/mixins/mainMixin';
import { QUtils } from '@/utils/mainUtils';
import bootbox from 'bootbox';
import moment from 'moment';
import QGroupBoxContainer from '@/components/GroupBoxContainer.vue';

export default {
	name: 'dashboard',
	mixins: [reusableMixin],
	components: {
		QGroupBoxContainer
	},
	data() {
		var vm = this;
		return {
			loaded: false,
			Model: {},
			modules: [],
			CurentMaintenance: {},
			style: {
				dtClass: 'col-sm-2 textRight',
				ddClass: 'col-sm-10'
			},
			showScheduleDT: true,
			scheduleDT: moment(),
			UsersCount: 0,
			queryParams: {
				sort: [],
				filters: [],
				global_search: "",
				per_page: 10,
				page: 1,
				component: "user",
			},
			tModules: {
				rows: [],
				total_rows: 0,
				columns: [
				{
					label: () => vm.$t('SIGLA14738'),
					name: "Codiprog",
					sort: true,
					initial_sort: true,
					initial_sort_order: "asc"
				},
				{
					label: () => vm.$t('NOME47814'),
					name: "Prog",
					sort: true
				},
				{
					label: () => vm.$t('PLATAFORMA28085'),
					name: "Platafor",
					sort: true
				},
				{
					label: () => vm.$t('VALIDADE07300'),
					name: "Vate",
					sort: true
				}],
				config: {
					table_title: () => vm.$t('MODULOS17298'),
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
		maintenanceBtnText() {
			var vm = this;
			return vm.CurentMaintenance.IsActive ? vm.Resources.DESACTIVAR_MANUTENCA45568 :
				(vm.CurentMaintenance.IsScheduled ? vm.Resources.MUDAR_AGENDAMENTO_DE08195 : vm.Resources.AGENDAR_MANUTENCAO19112);
		}
	},
	methods: {
		fetchData() {
			var vm = this;
			QUtils.log("Fetch data - Dashboard");
			QUtils.FetchData(QUtils.apiActionURL('Dashboard', 'Index')).done(function (data) {
				QUtils.log("Fetch data - OK (Dashboard)", data);
				$.each(data.model, function (propName, value) { vm.Model[propName] = value; });
				$.each(data.CurentMaintenance, function (propName, value) { vm.CurentMaintenance[propName] = value; });
				QUtils.FetchData(QUtils.apiActionURL('Users', 'GetUserList', vm.queryParams)).done(function (data) {
					vm.UsersCount = data.recordsTotal;
					vm.loaded = true;
				});
			});
		},
		needsBasicConfig() {
			return !this.Model.HasConfig || (this.Model.DBSize === 0 && this.Model.VersionDb === 0) || this.UsersCount === 0
		},
		showDBButton() {
			return this.Model.HasDiffVersion || this.Model.VersionDb != -1
		},
		getCardClass(cardId) {
			switch (cardId) {
				case 'card-system':
					return this.Model.HasConfig ? 'c-card-dashboard done' : 'c-card-dashboard disabled';
				
				case 'card-database':
					return this.Model.DBSize > 0 ? 'c-card-dashboard done' : 'c-card-dashboard disabled';

				case 'card-user':
					return this.UsersCount > 0 ? 'c-card-dashboard done' : 'c-card-dashboard disabled';

				default:
					return this.Model.HasConfig ? 'c-card-dashboard' : 'c-card-dashboard disabled';
			}
		},
		createNewConfig() {
			var vm = this;
			//Call method that creates a configuration file
			QUtils.postData('Dashboard', 'CreateConfiguration', null, null, function (data) {
					if (data.Success) {
						bootbox.alert(vm.Resources.NEW_CONFIGURATION_CR61652);
						vm.navigateTo(null, 'system_setup');
					}
					else {
						bootbox.alert(vm.Resources.THERE_WAS_AN_ERROR_C44163 + "<br>" + data.Message);
					}
					vm.fetchData();
			});               
		},
		ScheduleMaintenance() {
			var vm = this;
			if (vm.CurentMaintenance.IsActive) {
					QUtils.postData('Dashboard', 'DisableMaintenance', null, null, function (data) {
						QUtils.log("DisableMaintenance - Response", data);
						$.each(data.CurentMaintenance, function (propName, value) { vm.CurentMaintenance[propName] = value; });
					});
			}
			else {
				var dialog = bootbox.dialog({
					size: "small",
					title: vm.maintenanceBtnText,
					message: '<div id="xpto"></div><div><small>*' + vm.Resources.DEIXAR_VAZIO_PARA_LI30681 + '<small></div>',
					buttons: {
						confirm: {
							label: vm.Resources.CONFIRMAR09808,
							className: 'btn-success',
							callback: function () {
							QUtils.postData('Dashboard', 'ScheduleMaintenance', { date: vm.scheduleDT }, null, function (data) {
									QUtils.log("ScheduleMaintenance - Response", data);
									$.each(data.CurentMaintenance, function (propName, value) { vm.CurentMaintenance[propName] = value; });
								});
							}
						},
						cancel: {
							label: vm.Resources.CANCELAR49513,
							className: 'btn-danger'
						}
					},
				});
				dialog.init(function () {
					vm.scheduleDT = moment().add(5, 'minutes');
					$('#xpto').append(vm.$refs.scheduleDT.$el);
				});
				dialog.on('hide.bs.modal', function () {
					vm.showScheduleDT = false;
					setTimeout(function () { vm.showScheduleDT = true; }, 200);
				});
			}
		}
	},
	created() {
		this.modules = [];
		this.modules.push({
			Codiprog: 'PRO',
			Prog: 'My application',
			Platafor: 'VUE',
			Vate: '01/01/0001'
		});
		this.tModules.rows = this.modules;
		this.tModules.total_rows = this.modules.length;

		// Ler dados
		this.fetchData();
	},
	watch: {
		// call again the method if the route changes
		'$route': 'fetchData'
	}
};
</script>
