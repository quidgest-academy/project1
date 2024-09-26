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

	import MenuViewModel from './QMenuPRO_11ViewModel.js'

	const requiredTextResources = ['QMenuPRO_11', 'hardcoded', 'messages']

/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL PRO FORM_INCLUDEJS PRO_MENU_11]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */

	export default {
		name: 'QMenuPro11',

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
				componentOnLoadProc: asyncProcM.getProcListMonitor('QMenuPRO_11', false),

				interfaceMetadata: {
					id: 'QMenuPRO_11', // Used for resources
					requiredTextResources
				},

				menuInfo: {
					id: '11',
					isMenuList: true,
					designation: computed(() => this.Resources.AGENTES_IMOBILIARIOS24797),
					acronym: 'PRO_11',
					name: 'AGENT',
					route: 'menu-PRO_11',
					order: '11',
					controller: 'AGENT',
					action: 'PRO_Menu_11',
					isPopup: false
				},

				model: new MenuViewModel(this),

				controls: {
					menu: new controlClass.TableSpecialRenderingControl({
						fnHydrateViewModel: (data) => vm.model.hydrate(data),
						id: 'PRO_Menu_11',
						controller: 'AGENT',
						action: 'PRO_Menu_11',
						hasDependencies: false,
						isInCollapsible: false,
						tableModeClasses: [
							'q-table--full-height',
							'page-full-height'
						],
						columnsOriginal: [
							new listColumnTypes.ImageColumn({
								order: 1,
								name: 'ValFoto',
								area: 'AGENT',
								field: 'FOTO',
								label: computed(() => this.Resources.FOTOGRAFIA36807),
								scrollData: 3,
								sortable: false,
							}),
							new listColumnTypes.TextColumn({
								order: 2,
								name: 'ValNome',
								area: 'AGENT',
								field: 'NOME',
								label: computed(() => this.Resources.NOME47814),
								dataLength: 80,
								scrollData: 30,
							}),
						],
						config: {
							name: 'PRO_Menu_11',
							serverMode: true,
							pkColumn: 'ValCodagent',
							tableAlias: 'AGENT',
							tableNamePlural: computed(() => this.Resources.AGENTES_IMOBILIARIO54381),
							viewManagement: '',
							showLimitsInfo: true,
							tableTitle: computed(() => this.Resources.AGENTES_IMOBILIARIOS24797),
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
										formName: 'AGENTE',
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
										formName: 'AGENTE',
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
										formName: 'AGENTE',
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
										formName: 'AGENTE',
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
										formName: 'AGENTE',
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
								id: 'RCA_PRO_111',
								name: 'form-AGENTE',
								params: {
									isRoute: true,
									limits: [
										{
											identifier: 'id',
											fnValueSelector: (row) => row.ValCodagent
										},
									],
									isControlled: true,
									action: vm.openFormAction, type: 'form', mode: 'EDIT', formName: 'AGENTE',
								}
							},
							formsDefinition: {
								'AGENTE': {
									fnKeySelector: (row) => row.Fields.ValCodagent,
									isPopup: false
								},
							},
							// The list support form: AGENTE
							crudConditions: {
							},
							defaultSearchColumnName: '',
							defaultSearchColumnNameOriginal: '',
							initialSortColumnName: '',
							initialSortColumnOrder: 'asc'
						},
						changeEvents: ['changed-AGENT', 'changed-PMORA', 'changed-PNASC'],
						uuid: '95b4f097-f0a8-42c5-81ec-de9c53e74e70',
						allSelectedRows: 'false',
						viewModes: [
							{
								id: 'CARDS',
								type: 'cards',
								subtype: 'card-horizontal',
								label: computed(() => this.Resources.CARTOES27587),
								order: 1,
								mappingVariables: readonly({
									title: {
										allowsMultiple: false,
										sources: [
											'AGENT.NOME',
										]
									},
									image: {
										allowsMultiple: false,
										sources: [
											'AGENT.FOTO',
										]
									},
								}),
								styleVariables: {
									actionsAlignment: {
										rawValue: 'left',
										isMapped: false
									},
									actionsStyle: {
										rawValue: 'dropdown',
										isMapped: false
									},
									backgroundColor: {
										rawValue: 'auto',
										isMapped: false
									},
									customFollowupDefaultTarget: {
										rawValue: 'blank',
										isMapped: false
									},
									customInsertCard: {
										rawValue: true,
										isMapped: false
									},
									customInsertCardStyle: {
										rawValue: 'image',
										isMapped: false
									},
									displayMode: {
										rawValue: 'grid',
										isMapped: false
									},
									containerAlignment: {
										rawValue: 'left',
										isMapped: false
									},
									hoverScaleAmount: {
										rawValue: '1.00',
										isMapped: false
									},
									showColumnTitles: {
										rawValue: false,
										isMapped: false
									},
									showEmptyColumnTitles: {
										rawValue: true,
										isMapped: false
									},
									size: {
										rawValue: 'regular',
										isMapped: false
									},
								},
								groups: {
								}
							},
							{
								id: 'LIST',
								type: 'list',
								subtype: '',
								label: computed(() => this.Resources.LISTA13474),
								order: 2,
								mappingVariables: readonly({
								}),
								styleVariables: {
								},
								groups: {
								}
							},
						],
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
// USE /[MANUAL PRO FORM_CODEJS PRO_MENU_11]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */
		},

		methods: {
/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL PRO FUNCTIONS_JS PRO_11]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */
/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL PRO LISTING_CODEJS PRO_MENU_11]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */
		}
	}
</script>
