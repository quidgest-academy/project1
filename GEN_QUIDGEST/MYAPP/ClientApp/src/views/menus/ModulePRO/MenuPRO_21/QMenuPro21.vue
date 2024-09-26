<template>
	<teleport
		v-if="menuModalIsReady"
		:to="`#${uiContainersId.body}`"
		:disabled="!menuInfo.isPopup">
		<form
			class="form-horizontal"
			@submit.prevent>
			<q-row-container>
				<q-table
					v-if="componentOnLoadProc.loaded"
					v-bind="controls.menu"
					v-on="controls.menu.handlers">
				</q-table>

				<q-table-extra-extension
					:list-ctrl="controls.menu"
					v-on="controls.menu.handlers" />
			</q-row-container>
		</form>
	</teleport>

	<teleport
		v-if="menuModalIsReady && hasButtons"
		:to="`#${uiContainersId.footer}`"
		:disabled="!menuInfo.isPopup">
		<q-row-container>
			<div id="footer-action-btns">
				<template
					v-for="btn in menuButtons"
					:key="btn.id">
					<q-button
						v-if="btn.isVisible"
						:id="btn.id"
						:label="btn.text"
						:b-style="btn.style"
						:disabled="btn.disabled"
						:icon-on-right="btn.iconOnRight"
						:class="btn.classes"
						@click="btn.action">
						<q-icon
							v-if="btn.icon"
							v-bind="btn.icon" />
					</q-button>
				</template>
			</div>
		</q-row-container>
	</teleport>
</template>

<script>
	/* eslint-disable no-unused-vars */
	import { computed, readonly } from 'vue'

	import MenuHandlers from '@/mixins/menuHandlers.js'
	import controlClass from '@/mixins/fieldControl.js'
	import listFunctions from '@/mixins/listFunctions.js'
	import genericFunctions from '@/mixins/genericFunctions.js'
	import listColumnTypes from '@/mixins/listColumnTypes.js'

	import { loadResources } from '@/plugins/i18n.js'
	import asyncProcM from '@/api/global/asyncProcMonitoring.js'

	import hardcodedTexts from '@/hardcodedTexts'
	import netAPI from '@/api/network'
	import qApi from '@/api/genio/quidgestFunctions.js'
	import qFunctions from '@/api/genio/projectFunctions.js'
	import qProjArrays from '@/api/genio/projectArrays.js'
	import qEnums from '@/mixins/quidgest.mainEnums.js'
	/* eslint-enable no-unused-vars */

	import MenuViewModel from './QMenuPRO_21ViewModel.js'

	const requiredTextResources = ['QMenuPRO_21', 'hardcoded', 'messages']

/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL PRO FORM_INCLUDEJS PRO_MENU_21]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */

	export default {
		name: 'QMenuPro21',

		mixins: [
			MenuHandlers
		],

		inheritAttrs: false,

		props: {
			/**
			 * Whether or not the menu is used as a homepage.
			 */
			isHomePage: {
				type: Boolean,
				default: false
			}
		},

		expose: [
			'navigationId',
			'onBeforeRouteLeave',
			'updateMenuNavigation'
		],

		data()
		{
			// eslint-disable-next-line
			const vm = this
			return {
				componentOnLoadProc: asyncProcM.getProcListMonitor('QMenuPRO_21', false),

				interfaceMetadata: {
					id: 'QMenuPRO_21', // Used for resources
					requiredTextResources
				},

				menuInfo: {
					id: '21',
					isMenuList: true,
					designation: computed(() => this.Resources.PROPRIEDADES45924),
					acronym: 'PRO_21',
					name: 'PROPR',
					route: 'menu-PRO_21',
					order: '21',
					controller: 'PROPR',
					action: 'PRO_Menu_21',
					isPopup: false
				},

				model: new MenuViewModel(this),

				controls: {
					menu: new controlClass.TableListControl({
						fnHydrateViewModel: (data) => vm.model.hydrate(data),
						id: 'PRO_Menu_21',
						controller: 'PROPR',
						action: 'PRO_Menu_21',
						hasDependencies: false,
						isInCollapsible: false,
						tableModeClasses: [
							'q-table--full-height',
							'page-full-height'
						],
						columnsOriginal: [
							new listColumnTypes.TextColumn({
								order: 1,
								name: 'Agent.ValEmail',
								area: 'AGENT',
								field: 'EMAIL',
								label: computed(() => this.Resources.E_MAIL42251),
								dataLength: 80,
								scrollData: 30,
								pkColumn: 'ValCodagent',
							}),
							new listColumnTypes.TextColumn({
								order: 2,
								name: 'ValTitulo',
								area: 'PROPR',
								field: 'TITULO',
								label: computed(() => this.Resources.TITULO39021),
								dataLength: 80,
								scrollData: 30,
							}),
							new listColumnTypes.ImageColumn({
								order: 3,
								name: 'ValFoto',
								area: 'PROPR',
								field: 'FOTO',
								label: computed(() => this.Resources.FOTOGRAFIA36807),
								scrollData: 3,
								sortable: false,
							}),
							new listColumnTypes.CurrencyColumn({
								order: 4,
								name: 'ValPreco',
								area: 'PROPR',
								field: 'PRECO',
								label: computed(() => this.Resources.PRECO50007),
								scrollData: 10,
								maxDigits: 5,
								decimalPlaces: 2,
							}),
						],
						config: {
							name: 'PRO_Menu_21',
							serverMode: true,
							pkColumn: 'ValCodpropr',
							tableAlias: 'PROPR',
							tableNamePlural: computed(() => this.Resources.PROPRIEDADES45924),
							viewManagement: '',
							showLimitsInfo: true,
							tableTitle: computed(() => this.Resources.PROPRIEDADES45924),
							showAlternatePagination: true,
							permissions: {
							},
							globalSearch: {
								visibility: true,
								searchOnPressEnter: true
							},
							filtersVisible: true,
							allowColumnFilters: true,
							allowColumnSort: true,
							crudActions: [
								{
									id: 'show',
									name: 'show',
									title: computed(() => this.Resources.CONSULTAR57388),
									icon: {
										icon: 'view'
									},
									isInReadOnly: true,
									params: {
										action: vm.openFormAction,
										type: 'form',
										formName: 'PROPRIED',
										mode: 'SHOW',
										isControlled: true
									}
								},
								{
									id: 'edit',
									name: 'edit',
									title: computed(() => this.Resources.EDITAR11616),
									icon: {
										icon: 'pencil'
									},
									isInReadOnly: true,
									params: {
										action: vm.openFormAction,
										type: 'form',
										formName: 'PROPRIED',
										mode: 'EDIT',
										isControlled: true
									}
								},
								{
									id: 'duplicate',
									name: 'duplicate',
									title: computed(() => this.Resources.DUPLICAR09748),
									icon: {
										icon: 'duplicate'
									},
									isInReadOnly: true,
									params: {
										action: vm.openFormAction,
										type: 'form',
										formName: 'PROPRIED',
										mode: 'DUPLICATE',
										isControlled: true
									}
								},
								{
									id: 'delete',
									name: 'delete',
									title: computed(() => this.Resources.ELIMINAR21155),
									icon: {
										icon: 'delete'
									},
									isInReadOnly: true,
									params: {
										action: vm.openFormAction,
										type: 'form',
										formName: 'PROPRIED',
										mode: 'DELETE',
										isControlled: true
									}
								}
							],
							generalActions: [
								{
									id: 'insert',
									name: 'insert',
									title: computed(() => this.Resources.INSERIR43365),
									icon: {
										icon: 'add'
									},
									isInReadOnly: true,
									params: {
										action: vm.openFormAction,
										type: 'form',
										formName: 'PROPRIED',
										mode: 'NEW',
										repeatInsertion: false,
										isControlled: true
									}
								},
							],
							generalCustomActions: [
							],
							groupActions: [
							],
							customActions: [
							],
							MCActions: [
							],
							rowClickAction: {
								id: 'RCA_PRO_211',
								name: 'form-PROPRIED',
								params: {
									isRoute: true,
									limits: [
										{
											identifier: 'id',
											fnValueSelector: (row) => row.ValCodpropr
										},
									],
									isControlled: true,
									action: vm.openFormAction, type: 'form', mode: 'SHOW', formName: 'PROPRIED',
								}
							},
							formsDefinition: {
								'PROPRIED': {
									fnKeySelector: (row) => row.Fields.ValCodpropr,
									isPopup: false
								},
							},
							// The list support form: PROPRIED
							crudConditions: {
							},
							defaultSearchColumnName: 'ValTitulo',
							defaultSearchColumnNameOriginal: 'ValTitulo',
							initialSortColumnName: '',
							initialSortColumnOrder: 'asc'
						},
						groupFilters: [
							{
								id: 'filter_PRO_Menu_21_TIPOCONST',
								isMultiple: false,
								filters: [
									{
										id: 'filter_PRO_Menu_21_TIPOCONST_1',
										key: '1',
										value: computed(() => this.Resources.TODOS59977),
										selected: true
									},
									{
										id: 'filter_PRO_Menu_21_TIPOCONST_2',
										key: '2',
										value: computed(() => this.Resources.APARTAMENTO13855),
										selected: false
									},
									{
										id: 'filter_PRO_Menu_21_TIPOCONST_3',
										key: '3',
										value: computed(() => this.Resources.MORADIA65264),
										selected: false
									},
									{
										id: 'filter_PRO_Menu_21_TIPOCONST_4',
										key: '4',
										value: computed(() => this.Resources.OUTROS19682),
										selected: false
									},
								],
								value: '1',
								defaultValue: '1'
							},
						],
						changeEvents: ['changed-AGENT', 'changed-CIDAD', 'changed-PROPR'],
						uuid: '6a98d3ce-a8e0-4a43-837c-98a413bb8578',
						allSelectedRows: 'false',
						headerLevel: 1,
					}, this)
				}
			}
		},

		beforeRouteEnter(to, _, next)
		{
			// called before the route that renders this component is confirmed.
			// does NOT have access to `this` component instance,
			// because it has not been created yet when this guard is called!

			next((vm) => vm.updateMenuNavigation(to))
		},

		beforeRouteLeave(to, _, next)
		{
			this.onBeforeRouteLeave(to, next)
		},

		mounted()
		{
/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL PRO FORM_CODEJS PRO_MENU_21]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */
		},

		methods: {
/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL PRO FUNCTIONS_JS PRO_21]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */
/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL PRO LISTING_CODEJS PRO_MENU_21]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */
		}
	}
</script>
