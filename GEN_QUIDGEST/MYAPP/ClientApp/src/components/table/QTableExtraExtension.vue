<template>
	<q-table-config
		:table-ctrl="listCtrl"
		modal-id="config"
		v-bind="listCtrl.config"
		v-on="tableConfigHandlers"
		:signal="listCtrl.subSignals.config"
		:texts="listCtrl.texts" />

	<q-table-column-config
		v-if="isListVisible"
		modal-id="column-config"
		v-bind="listCtrl.config"
		v-on="tableColumnConfigHandlers"
		:signal="listCtrl.subSignals.columnConfig"
		:columns="listCtrl.columns"
		:default-search-column-name="listCtrl.config.defaultSearchColumnName"
		:texts="listCtrl.texts" />

	<q-table-advanced-filters
		v-if="listCtrl.config.allowAdvancedFilters"
		modal-id="advanced-filters"
		v-bind="listCtrl.config"
		v-on="tableAdvancedFilters"
		:signal="listCtrl.subSignals.advancedFilters"
		:table-name="listCtrl.config.name"
		:columns="listCtrl.columns"
		:filters="listCtrl.advancedFilters"
		mode="editAll"
		:texts="listCtrl.texts"
		:filter-operators="filterOperators" />

	<q-table-advanced-filters
		v-if="listCtrl.config.allowAdvancedFilters"
		modal-id="advanced-filters-new"
		v-bind="listCtrl.config"
		v-on="tableAdvancedFilters"
		:signal="listCtrl.subSignals.advancedFiltersNew"
		:table-name="listCtrl.config.name"
		:columns="listCtrl.columns"
		:filters="listCtrl.advancedFilters"
		mode="new"
		:texts="listCtrl.texts"
		:filter-operators="filterOperators" />

	<q-table-view-save
		modal-id="view-save"
		v-bind="listCtrl.config"
		v-on="tableViewSaveHandlers"
		:signal="listCtrl.subSignals.viewSave"
		:config-names="listCtrl.config.UserTableConfigNames"
		:texts="listCtrl.texts" />

	<q-table-views
		modal-id="views"
		v-bind="listCtrl.config"
		v-on="tableViewHandlers"
		:signal="listCtrl.subSignals.views"
		:config-names="listCtrl.config.UserTableConfigNames"
		:config-name-default="listCtrl.config.UserTableConfigNameDefault"
		:texts="listCtrl.texts" />
</template>

<script>
	import searchFilterDataModule from '@/api/genio/searchFilterData'

	import genericFunctions from '@/mixins/genericFunctions.js'

	import QTableConfig from './QTableConfig.vue'
	import QTableColumnConfig from './QTableColumnConfig.vue'
	import QTableAdvancedFilters from './QTableAdvancedFilters.vue'
	import QTableViewSave from './QTableViewSave.vue'
	import QTableViews from './QTableViews.vue'

	export default {
		name: 'QTableExtraExtension',

		emits: [
			'signal-component',
			'show-popup',
			'hide-popup',
			'set-property',
			'update-config',
			'set-info-message',
			'save-column-config',
			'apply-column-config',
			'reset-column-config',
			'reset-column-sizes',
			'toggle-text-wrap',
			'add-advanced-filter',
			'edit-advanced-filter',
			'set-advanced-filter-state',
			'set-advanced-filter-states',
			'deactivate-all-advanced-filters',
			'remove-advanced-filter',
			'save-view',
			'copy-view',
			'select-view',
			'view-action'
		],

		components: {
			QTableConfig,
			QTableColumnConfig,
			QTableAdvancedFilters,
			QTableViewSave,
			QTableViews
		},

		inheritAttrs: false,

		props: {
			/**
			 * Control object containing necessary state and configuration properties for a list-style view of the table component.
			 */
			listCtrl: {
				type: Object,
				required: true
			},

			/**
			 * A set of operator definitions used for creating and managing advanced filters in the table.
			 */
			filterOperators: {
				type: Object,
				default: () => searchFilterDataModule.operators.elements
			}
		},

		expose: [],

		data()
		{
			return {
				tableConfigHandlers: {
					showPopup: (eventData) => this.emitEvent('show-popup', eventData),
					hidePopup: (eventData) => this.emitEvent('hide-popup', eventData),
					setProperty: (...args) => this.emitEventArgs('set-property', ...args),
					signalComponent: (...args) => this.emitEventArgs('signal-component', ...args)
				},

				tableColumnConfigHandlers: {
					showPopup: (eventData) => this.emitEvent('show-popup', eventData),
					hidePopup: (eventData) =>
					{
						this.emitEvent('hide-popup', eventData)
						this.closeConfigPopup()
					},
					setProperty: (...args) => this.emitEventArgs('set-property', ...args),
					updateConfig: (...args) => this.$emit('update-config', ...args),
					applyColumnConfig: (eventData) =>
						this.emitEvent('apply-column-config', eventData),
					resetColumnConfig: (eventData) =>
						this.emitEvent('reset-column-config', eventData),
					resetColumnSizes: (eventData) =>
						this.emitEvent('reset-column-sizes', eventData),
					toggleTextWrap: (eventData) => this.emitEvent('toggle-text-wrap', eventData)
				},

				tableAdvancedFilters: {
					showPopup: (eventData) => this.emitEvent('show-popup', eventData),
					hidePopup: (eventData) =>
					{
						this.emitEvent('hide-popup', eventData)
						this.closeConfigPopup()
					},
					setProperty: (...args) => this.emitEventArgs('set-property', ...args),
					updateConfig: (...args) => this.$emit('update-config', ...args),
					addAdvancedFilter: (eventData) =>
						this.emitEvent('add-advanced-filter', eventData),
					editAdvancedFilter: (eventData) =>
						this.emitEvent('edit-advanced-filter', eventData),
					setAdvancedFilterState: (eventData) =>
						this.emitEvent('set-advanced-filter-state', eventData),
					setAdvancedFilterStates: (eventData) =>
						this.emitEvent('set-advanced-filter-states', eventData),
					deactivateAllAFilters: (eventData) =>
						this.emitEvent('deactivate-all-advanced-filters', eventData),
					removeAdvancedFilter: (eventData) =>
						this.emitEvent('remove-advanced-filter', eventData)
				},

				tableViewSaveHandlers: {
					showPopup: (eventData) => this.emitEvent('show-popup', eventData),
					hidePopup: (eventData) =>
					{
						this.emitEvent('hide-popup', eventData)
						this.closeConfigPopup()
					},
					setProperty: (...args) => this.emitEventArgs('set-property', ...args),
					saveView: (eventData) => this.emitSaveViewEvent('save-view', eventData),
					copyView: (eventData) => this.emitSaveViewEvent('copy-view', eventData)
				},

				tableViewHandlers: {
					showPopup: (eventData) => this.emitEvent('show-popup', eventData),
					hidePopup: (eventData) =>
					{
						this.emitEvent('hide-popup', eventData)
						this.closeConfigPopup()
					},
					selectView: (eventData) => this.emitEvent('select-view', eventData),
					viewAction: (eventData) => this.emitViewEvent('view-action', eventData)
				}
			}
		},

		computed: {
			isListVisible()
			{
				// FIXME: remove by implementing a view mode manager
				// QTable SHOULD ONLY IMPLEMENT TABLE LOGIC!!! => DOES NOT CARE ABOUT VIEWMODES
				return !this.listCtrl.viewModes?.length || this.listCtrl.activeViewModeId === 'LIST'
			}
		},

		methods: {
			emitEvent(eventName, eventData)
			{
				this.$emit(eventName, eventData)
			},

			emitEventArgs()
			{
				this.$emit(...arguments)
			},

			emitEventCallbackParams(callbackParams)
			{
				this.emitEvent(callbackParams.eventName, callbackParams.eventData)
			},

			saveViewOpenView(callbackParams)
			{
				this.$emit('save-view', {
					name: this.listCtrl.config.UserTableConfigName,
					isSelected: false
				})
				this.emitEventCallbackParams(callbackParams)
			},

			emitViewEvent(eventName, eventData)
			{
				if (
					eventData.name !== undefined &&
					eventData.name !== null &&
					eventData.name === 'DUPLICATE'
				)
				{
					this.$emit(
						'signal-component',
						'viewSave',
						{ copyFromName: eventData.rowValue },
						true
					)
					this.$emit('signal-component', 'config', { selectedTab: 'view-save' }, false)
				}
				//Opening a view, confirm whether to save changes to current view
				else if (
					eventData.name !== undefined &&
					eventData.name !== null &&
					eventData.name === 'SHOW' &&
					this.listCtrl.confirmChanges
				)
				{
					let buttons = {
						confirm: {
							label: this.listCtrl.texts.saveText,
							action: this.saveViewOpenView
						},
						cancel: {
							label: this.listCtrl.texts.discard,
							action: this.emitEventCallbackParams
						}
					}
					genericFunctions.displayMessage(
						`${this.listCtrl.texts.wantToSaveChangesToView}`,
						'warning',
						null,
						buttons,
						{ callbackParams: { eventName: eventName, eventData: eventData } }
					)
				}
				else
					this.$emit(eventName, eventData)
			},

			emitSaveViewEvent(eventName, eventData)
			{
				this.$emit(eventName, eventData)

				let alertProps = {
					type: 'success',
					message: this.listCtrl.texts.tableViewSaveSuccess,
					icon: 'ok'
				}
				this.$emit('set-info-message', alertProps)
			},

			closeConfigPopup()
			{
				this.$emit('signal-component', 'config', { show: false })
			}
		}
	}
</script>
