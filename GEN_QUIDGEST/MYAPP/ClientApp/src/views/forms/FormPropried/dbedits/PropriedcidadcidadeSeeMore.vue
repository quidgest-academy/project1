<template>
	<teleport
		v-if="isReady"
		to="#q-modal-see-more-propriedcidadcidade-body">
		<q-row-container>
			<q-table
				v-bind="listCtrl"
				v-on="listCtrl.handlers" />
		</q-row-container>
	</teleport>
</template>

<script>
	/* eslint-disable no-unused-vars */
	import { computed } from 'vue'
	import { mapActions } from 'pinia'
	import _assignIn from 'lodash-es/assignIn'
	import _merge from 'lodash-es/merge'

	import { useGenericDataStore } from '@/stores/genericData.js'
	import { useNavDataStore } from '@/stores/navData.js'
	import VueNavigation from '@/mixins/vueNavigation.js'
	import ListHandlers from '@/mixins/listHandlers.js'
	import { navigationProperties } from '@/mixins/navHandlers.js'
	import { TableListControl } from '@/mixins/fieldControl.js'
	import listFunctions from '@/mixins/listFunctions.js'
	import listColumnTypes from '@/mixins/listColumnTypes.js'

	import { loadResources } from '@/plugins/i18n.js'
	import asyncProcM from '@/api/global/asyncProcMonitoring.js'

	import netAPI from '@/api/network'
	import qApi from '@/api/genio/quidgestFunctions.js'
	import qFunctions from '@/api/genio/projectFunctions.js'
	import qProjArrays from '@/api/genio/projectArrays.js'
	import genericFunctions from '@/mixins/genericFunctions.js'
	/* eslint-enable no-unused-vars */

	import ViewModelBase from '@/mixins/viewModelBase.js'

	const requiredTextResources = ['PROPRIEDCIDADCIDADE___SeeMore', 'hardcoded', 'messages']

	export default {
		name: 'PropriedcidadcidadeSeeMore',

		inheritAttrs: false,

		emits: [
			'close',
			'see-more-choice'
		],

		mixins: [
			navigationProperties,
			VueNavigation,
			ListHandlers
		],

		props: {
			/**
			 * Unique identifier for the control.
			 */
			id: String,

			/**
			 * The limits to which this "See more" control is subjected.
			 */
			limits: {
				type: Object,
				default: () => ({})
			},

			/**
			 * The id of the current navigation.
			 */
			navigationId: {
				type: String,
				default: ''
			}
		},

		expose: [],

		data()
		{
			return {
				isReady: false,

				componentOnLoadProc: asyncProcM.getProcListMonitor('PROPRIEDCIDADCIDADE___SeeMore', false),

				interfaceMetadata: {
					id: 'PROPRIEDCIDADCIDADE___SeeMore', // Used for resources
					requiredTextResources
				},

				menuInfo: {
					acronym: 'PROPRIEDCIDADCIDADE___SeeMore',
					name: 'PROPRIEDCIDADCIDADE___SeeMore',
					controller: 'PROPR',
					action: 'PROPRIED_CidadValCidade'
				},

				listCtrl: new TableListControl(this.getListConfig(), this),

				// Basic view model to handle access to GLOB, if necessary.
				model: new ViewModelBase(this),
			}
		},

		created()
		{
			let params = {
				id: this.id || null,
				Limits: this.limits || []
			}

			_merge(params, this.limits)

			this.componentOnLoadProc.addImmediateBusy(loadResources(this, requiredTextResources))
			this.listCtrl.init()
			this.componentOnLoadProc.addImmediateBusy(this.fetchListData(this.listCtrl, params))
			this.componentOnLoadProc.once(() => {
				this.isReady = true
				this.listCtrl.initData()
			}, this)
		},

		mounted()
		{
			// Listens for changes to the DB and updates the list accordingly.
			this.$eventHub.onMany(this.listCtrl.changeEvents, this.onTableDBDataChanged)

			const modalProps = {
				id: 'see-more-propriedcidadcidade',
				headerTitle: computed(() => this.Resources.CIDADES12303),
				closeButtonEnable: true,
				hideFooter: true,
				dismissWithEsc: true,
				dismissAction: this.close,
				isActive: true,
				returnElement: 'PROPRIEDCIDADCIDADE__'
			}
			this.setModal(modalProps)
		},

		beforeUnmount()
		{
			// Removes the listeners.
			this.$eventHub.offMany(this.listCtrl.changeEvents, this.onTableDBDataChanged)
			this.listCtrl.destroy()
			this.componentOnLoadProc.destroy()

			genericFunctions.removeModal('see-more-propriedcidadcidade')
		},

		methods: {
			...mapActions(useGenericDataStore, [
				'setModal',
				'setDropdown'
			]),

			...mapActions(useNavDataStore, [
				'setParamValue',
				'setEntryValue'
			]),

			close()
			{
				this.$emit('close')
			},

			onTableDBDataChanged()
			{
				let params = {
					id: this.id || null,
					Limits: this.limits
				}

				_merge(params, this.limits)

				this.componentOnLoadProc.addBusy(this.fetchListData(this.listCtrl, params))
			},

			handleRowAction(eventData)
			{
				if (eventData.id === 'see-more-choice')
				{
					let rowKey = eventData?.rowKeyPath
					if (Array.isArray(eventData?.rowKeyPath) && eventData?.rowKeyPath.length > 0)
						rowKey = eventData?.rowKeyPath[eventData?.rowKeyPath.length - 1]

					this.$emit('see-more-choice', rowKey)
				}
				else
					this.onTableListExecuteAction(this.listCtrl, eventData)
			},

			getListConfig()
			{
				const vm = this
				const listProps = {
					configuration: {
						controller: 'PROPR',
						action: 'Propried_CidadValCidade',
						hasDependencies: false,
						isInCollapsible: true,
						columnsOriginal: [
							new listColumnTypes.TextColumn({
								order: 1,
								name: 'ValCidade',
								area: 'CIDAD',
								field: 'CIDADE',
								label: computed(() => this.Resources.CIDADE27871),
								dataLength: 50,
								scrollData: 50,
							}),
						],
						config: {
							name: 'Propried_CidadValCidade',
							serverMode: true,
							pkColumn: 'ValCodcidad',
							tableAlias: 'CIDAD',
							tableNamePlural: computed(() => this.Resources.CIDADES12303),
							viewManagement: '',
							showLimitsInfo: true,
							tableTitle: '',
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
							generalCustomActions: [
							],
							groupActions: [
							],
							customActions: [
							],
							MCActions: [
							],
							rowClickAction: {
								id: 'see-more-choice',
								name: 'see-more-choice',
							},
							formsDefinition: {
							},
							crudConditions: {
							},
							defaultSearchColumnName: 'ValCidade',
							defaultSearchColumnNameOriginal: 'ValCidade',
							initialSortColumnName: '',
							initialSortColumnOrder: 'asc'
						},
						changeEvents: ['changed-PAIS', 'changed-CIDAD'],
						uuid: 'Propried_Propried_CidadValCidade',
						allSelectedRows: 'false',
						handlers: {
							rowAction: vm.handleRowAction
						},
						fixedControlLimits: vm.limits
					}
				}

				return listProps.configuration
			}
		}
	}
</script>
