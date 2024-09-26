import { mapState } from 'pinia'
import cloneDeep from 'lodash-es/cloneDeep'
import _assignIn from 'lodash-es/assignIn'
import _find from 'lodash-es/find'
import _foreach from 'lodash-es/forEach'
import _get from 'lodash-es/get'
import _isEmpty from 'lodash-es/isEmpty'

import { useSystemDataStore } from '@/stores/systemData.js'
import { useGenericLayoutDataStore } from '@/stores/genericLayoutData.js'

import netAPI from '@/api/network'
import genericFunctions from '@/mixins/genericFunctions.js'
import listFunctions from '@/mixins/listFunctions.js'
import qEnums from '@/mixins/quidgest.mainEnums.js'

/*****************************************************************
 * This mixin aggregates operations over lists, which can be     *
 * reused in menus and form components.                          *
 *****************************************************************/
export default {
	computed: {
		...mapState(useSystemDataStore, [
			'system'
		]),

		...mapState(useGenericLayoutDataStore, [
			'layoutConfig'
		])
	},

	methods: {
		/**
		 * Fetches the data from the server and loads the list.
		 * @param {object} listControl The list control object
		 * @param {object} params The necessary parameters
		 * @param {Function} fnHydrateViewModel The custom callback method for hydrate the page view model data
		 * @param {Function} fnUpdateData The custom callback method for update the data
		 * @returns A promise with the response from the server.
		 */
		fetchListData(listControl, params, fnHydrateViewModel, fnUpdateData)
		{
			// Table list limits
			const limits = listControl.getLimitsValues()

			// Object with required parameters
			let actionParams = {
				id: limits.id ?? this.$route.params.id,
				/**
				 * The limit values can come in the queryParams, but for now, we will opt to send them directly in the Navigation instead of through here,
				 * to prevent potential issues with limit value formatting (especially with dates)
				 * and eliminate the need for the server-side to insert the limits into the navigation itself.
				 */
				queryParams: {},
				// Table configuration state
				tableConfiguration: {},
				// Whether this is the first time loading the table after navigating to it
				isFirstLoad: false,
				noRedirect: false,
				...params
			}

			// Use current control if the ID matches this table ID
			const currentControl = this.currentControl.id === listControl.id ? this.currentControl : null
			if (!_isEmpty(currentControl))
			{
				this.removeCurrentControl({
					navigationId: this.navigationId,
					controlId: listControl.id
				})
			}

			// Put the limit values in Navigation (history) before making the request to the server.
			_foreach(limits, (value, key) => {
				const entry = {
					navigationId: this.navigationId,
					key,
					value
				}
				this.setEntryValue(entry)
			})

			// If it's the first load
			if (!listControl.dataAlreadyRequested)
			{
				// Used for «Jump if just one»
				Reflect.set(actionParams, 'isFirstLoad', true)
				Reflect.set(actionParams, 'noRedirect', true)

				// BEGIN: If returning from a form
				// Get current unsaved configuration data so it can be loaded in the hydrate function
				let currentTableConfig = currentControl?.data?.tableConfig
				// If current unsaved configuration exists
				if (currentTableConfig !== undefined && currentTableConfig !== null)
					Reflect.set(actionParams, 'tableConfiguration', currentTableConfig)
				// END: If returning from a form
				// If no current unsaved configuration exists and it's the first load
				else
					Reflect.set(actionParams, 'loadDefaultView', true)
			}
			listControl.dataAlreadyRequested = true

			return netAPI.postData(
				listControl.controller,
				listControl.action,
				actionParams,
				(data) => {
					// When loading additional data for the page ViewModel
					if (typeof fnHydrateViewModel === 'function')
						fnHydrateViewModel(data, listControl)

					// When loading additional data for the branch of the tree,
					// we use a customized callback to assign data to the branch's children.
					if (typeof fnUpdateData === 'function')
						fnUpdateData(data, listControl)
					else
					{
						let rowKeyToScroll = ''

						// FOR: table go to row on return
						// If returning to the table from a form, set key of row to go to
						if (!_isEmpty(currentControl) && currentControl.id === listControl.id)
						{
							rowKeyToScroll = currentControl?.data?.rowKey
							listControl.config.rowKeyToScroll = rowKeyToScroll
						}

						if (listControl.type === 'TreeList')
							listControl.hydrate(listControl, data, rowKeyToScroll)
						else
							listControl.hydrate(listControl, data)
						listControl.isLoaded = true

						listControl.afterLoaded()
					}
				},
				undefined,
				undefined,
				this.navigationId)
		},

		/**
		 * Fetches the data from the server and loads the list.
		 * @param {object} timelineControl The Timeline control object
		 * @param {object} params The necessary parameters
		 * @returns A promise with the response from the server.
		 */
		fetchTimelineData(timelineControl, params)
		{
			if (_isEmpty(params))
				params = {}

			_assignIn(params, this.$route.params)

			return netAPI.postData(
				timelineControl.controller,
				timelineControl.action,
				params,
				(data) => {
					timelineControl.hydrate(timelineControl, data)
					timelineControl.isLoaded = true
				},
				undefined,
				undefined,
				this.navigationId)
		},

		/**
		 * Open support form of timeline item.
		 * @param {object} emittedAction The TimelineItem data
		 */
		timelineOpenForm(emittedAction)
		{
			if (emittedAction?.SupportForm)
			{
				let options = { isPopup: emittedAction.IsPopupForm }
				this.navigateToForm(emittedAction.SupportForm, 'SHOW', emittedAction.Identifier, options)
			}
		},

		/**
		 *
		 * @param {object} listConf The list configuration
		 * @returns A promise with the response from the server.
		 */
		onTableListChangeQuery(listConf)
		{
			return this.fetchListData(listConf, { tableConfiguration: listFunctions.getTableConfiguration(listConf) })
		},

		/**
		 * Set whether search will be triggered next time search data changes
		 * @param {object} listConf The list configuration
		 * @param {boolean} value
		 */
		setSearchOnNextChange(listConf, value)
		{
			listConf.searchOnNextChange.value = value
		},

		/**
		 * Add advanced filter
		 * @param {object} listConf The list configuration
		 * @param filter {Object}
		 */
		addAdvancedFilter(listConf, filter)
		{
			this.setSearchOnNextChange(listConf, true)
			listConf.advancedFilters.push(filter)
		},

		/**
		 * Edit advanced filter
		 * @param {object} listConf The list configuration
		 * @param filter {Object}
		 * @param index {number}
		 */
		editAdvancedFilter(listConf, filter, index)
		{
			this.setSearchOnNextChange(listConf, true)
			listConf.advancedFilters[index] = filter
		},

		/**
		 * Set advanced filter state
		 * @param {object} listConf The list configuration
		 * @param {number} index : index
		 * @param {boolean} active : active state
		 */
		setAdvancedFilterState(listConf, index, active)
		{
			this.setSearchOnNextChange(listConf, true)
			listConf.advancedFilters[index].active = active
		},

		/**
		 * Set multiple advanced filter states
		 * @param {object} listConf The list configuration
		 * @param {Array} selectedFilterIdxs : index
		 * @param {boolean} active : active state
		 */
		setAdvancedFilterStates(listConf, selectedFilterIdxs, active)
		{
			this.setSearchOnNextChange(listConf, true)
			var selectedFilterIdx = -1
			for (let idx in selectedFilterIdxs)
			{
				selectedFilterIdx = selectedFilterIdxs[idx]
				listConf.advancedFilters[selectedFilterIdx].active = active
			}
		},

		/**
		 * Remove all advanced filters
		 * @param {object} listConf The list configuration
		 */
		removeAllAdvancedFilters(listConf)
		{
			this.setSearchOnNextChange(listConf, true)
			listConf.advancedFilters.splice(0)
		},

		/**
		 * Set all advanced filter states to inactive
		 * @param {object} listConf The list configuration
		 */
		deactivateAllAdvancedFilters(listConf)
		{
			this.setSearchOnNextChange(listConf, true)
			for (let idx in listConf.advancedFilters)
				listConf.advancedFilters[idx].active = false
		},

		/**
		 * Set advanced filter state
		 * @param {object} listConf The list configuration
		 * @param {number} index : index
		 */
		removeAdvancedFilter(listConf, index)
		{
			this.setSearchOnNextChange(listConf, true)
			listConf.advancedFilters.splice(index, 1)
		},

		/**
		 * Clear all advanced filters
		 * @param {object} listConf The list configuration
		 */
		clearAdvancedFilters(listConf)
		{
			this.setSearchOnNextChange(listConf, true)
			listConf.advancedFilters = []
		},

		/**
		 * Open advanced filters
		 * @param {object} listConf The list configuration
		 * @param {boolean} visible
		 * @param {number} selectedFilterIdx
		 */
		setAdvancedFiltersPopup(listConf, visible, selectedFilterIdx)
		{
			var useVisible = false
			if (visible !== undefined)
				useVisible = !!visible

			var useSelectedFilterIdx = null
			if (selectedFilterIdx !== undefined)
				useSelectedFilterIdx = selectedFilterIdx

			// Set advanced filters open config to show and select corresponding filter by index
			listConf.subSignals.advancedFilters = { 'show': useVisible, 'selectedFilterIdx': useSelectedFilterIdx }
		},

		/**
		 * Set property in table object
		 * @param {object} listConf The list configuration
		 * @param {string} name...
		 * @param {object} value
		 */
		setProperty()
		{
			switch (arguments.length)
			{
				case 3:
					arguments[0][arguments[1]] = arguments[2]
					break
				case 4:
					arguments[0][arguments[1]][arguments[2]] = arguments[3]
					break
				case 5:
					arguments[0][arguments[1]][arguments[2]][arguments[3]] = arguments[4]
					break
				default:
					return
			}
		},

		/**
		 * Set sub-property in array in table object where property has value
		 * @param {object} listConf The list configuration
		 * @param {string} arrayName
		 * @param {string} propertyName
		 * @param {string} propertyValue
		 * @param {string} key
		 * @param {object} value
		 * @param {object} otherValue
		 */
		setArraySubPropWhere(listConf, arrayName, propertyName, propertyValue, key, value, otherValue)
		{
			for (let idx in listConf[arrayName])
			{
				let elem = listConf[arrayName][idx]
				if (elem[propertyName] === propertyValue)
					listConf[arrayName][idx][key] = value
				else if (otherValue !== undefined && otherValue !== null)
					listConf[arrayName][idx][key] = otherValue
			}
		},

		/**
		 * Set property in table object that is used to send a signal to a component
		 * @param {object} listConf The list configuration
		 * @param {string} id
		 * @param {object} signal
		 */
		signalComponent(listConf, id, signal, mergeProps)
		{
			if (mergeProps)
			{
				listConf.subSignals[id] = {
					...listConf.subSignals[id],
					...signal
				}
			}
			else
				listConf.subSignals[id] = signal
		},

		/**
		 * Update table configuration object (based on changes it's properties)
		 * @param {object} listConf The list configuration
		 */
		updateConfig(listConf)
		{
			if (listConf.config.viewManagement === qEnums.tableViewManagementModes.persistOne)
				this.onTableListSaveView(listConf, { name: '_', isSelected: true })
			else if (listConf.config.viewManagement === qEnums.tableViewManagementModes.persistMany
				|| listConf.config.viewManagement === qEnums.tableViewManagementModes.nonPersistent)
				listConf.confirmChanges = true

			listFunctions.updateConfigOptions(listConf)
		},

		/**
		 * Save view (user table configuration)
		 * @param {object} listConf The list configuration
		 * @param {object} eObj
		 */
		onTableListSaveView(listConf, eObj)
		{
			if (_isEmpty(eObj.name))
				return

			if (typeof eObj.isSelected !== 'boolean')
				eObj.isSelected = false

			// Get table configuration
			const config = listFunctions.getTableConfiguration(listConf)
			// Convert to format used in database
			const configEncoded = listFunctions.convertTableConfigurationToDB(config)

			const params = {
				uuid: listConf.uuid,
				configName: eObj.name,
				isSelected: eObj.isSelected,
				data: configEncoded
			}

			// Send request to save configuration
			netAPI.postData(
				'Tblcfg',
				'SaveConfig',
				params,
				() => {
					// Clear view name array if there are no views
					if (_isEmpty(listConf.config.UserTableConfigNames))
						listConf.config.UserTableConfigNames = []

					// Add view name to list of available views
					if (!listConf.config.UserTableConfigNames.includes(eObj.name))
						listConf.config.UserTableConfigNames.push(eObj.name)

					// Set default view
					if (eObj.isSelected)
						listConf.config.UserTableConfigNameDefault = eObj.name

					// Set opened view name to this view
					listConf.config.UserTableConfigName = eObj.name

					// Reset property for whether there are changes
					listConf.confirmChanges = false
				},
				undefined,
				undefined,
				this.navigationId)
		},

		/**
		 * Select view (user table configuration)
		 * @param {object} listConf The list configuration
		 * @param {object} eObj
		 */
		onTableListSelectView(listConf, eObj)
		{
			const params = {
				uuid: listConf.uuid,
				configName: eObj.name
			}

			// BEGIN: Send request to save configuration
			netAPI.postData(
				'Tblcfg',
				'SelectConfig',
				params,
				() => {
					// Reload table
					if (eObj.name && eObj.name !== '')
						this.fetchListData(listConf, { queryParams: { uuid: listConf.uuid, loadDefaultView: true } })
					else
						this.onTableListCloseView(listConf, eObj)
				},
				undefined,
				undefined,
				this.navigationId)
		},

		/**
		 * Table view action (user table configuration)
		 * @param {object} listConf The list configuration
		 * @param {object} eObj
		 */
		onTableListViewAction(listConf, eObj)
		{
			const tableViewFun = {
				'SHOW': this.onTableListOpenView,
				'DUPLICATE': this.onTableListCopyView,
				'DELETE': this.onTableListDeleteView
			}

			if (tableViewFun[eObj.name] === undefined || tableViewFun[eObj.name] === null)
				return

			tableViewFun[eObj.name](listConf, eObj)
		},

		/**
		 * Open a table view (user table configuration)
		 * @param {object} listConf The list configuration
		 * @param {object} eObj
		 */
		onTableListOpenView(listConf, eObj)
		{
			// Reset property for whether there are changes
			listConf.confirmChanges = false
			// Reload table
			this.fetchListData(listConf, { userTableConfigName: eObj.rowValue })
		},

		/**
		 * Open a table view (user table configuration)
		 * @param {object} listConf The list configuration
		 * @param {object} eObj
		 */
		onTableListCopyView(listConf, eObj)
		{
			const params = {
				uuid: listConf.uuid,
				configName: eObj.name,
				isSelected: eObj.isSelected,
				copyFromName: eObj.copyFromName
			}

			// Send request to save configuration
			netAPI.postData(
				'Tblcfg',
				'CopyConfig',
				params,
				(data) => {
					// If user chose to set the copied view as the default
					const loadDefaultView = Boolean(data?.LoadDefaultView)
					// Reload table
					this.fetchListData(listConf, { queryParams: { uuid: listConf.uuid, loadDefaultView } })
				},
				undefined,
				undefined,
				this.navigationId)
		},

		/**
		 * Open a table view (user table configuration)
		 * @param {object} listConf The list configuration
		 * @param {object} eObj
		 */
		onTableListDeleteView(listConf, eObj)
		{
			const params = {
				uuid: listConf.uuid,
				configName: eObj.rowValue
			}

			// Send request to save configuration
			netAPI.postData(
				'Tblcfg',
				'DeleteConfig',
				params,
				(data) => {
					// If view was default view
					if (data.DeletedDefaultView)
					{
						// Clear view configuration
						listFunctions.applyTableConfiguration(listConf, {})
						listConf.config.UserTableConfigName = null
						// Reload table
						this.fetchListData(listConf)
					}
					// If view was opened but not the default view
					else if (eObj.rowValue === listConf.config.UserTableConfigName)
					{
						// Reload table
						this.fetchListData(listConf, { queryParams: { uuid: listConf.uuid, loadDefaultView: true } })
					}
					// If view was not opened
					else
					{
						const idx = listConf.config.UserTableConfigNames.findIndex((x) => x === eObj.rowValue)
						listConf.config.UserTableConfigNames.splice(idx, 1)
					}
				},
				undefined,
				undefined,
				this.navigationId)
		},

		/**
		 * Close a table view (user table configuration)
		 * @param {object} listConf The list configuration
		 */
		onTableListCloseView(listConf)
		{
			// Clear view configuration
			listFunctions.applyTableConfiguration(listConf, {})
			listConf.config.UserTableConfigName = null
			// Reset property for whether there are changes
			listConf.confirmChanges = false
			// Reload table
			this.fetchListData(listConf)
		},

		/**
		 * Export table data to file
		 * @param {object} listConf The list configuration
		 * @param {object} eObj
		 * @param {boolean} template (false: download data file, true: download template file)
		 * @returns A promise with the response from the server.
		 */
		onTableListExportData(listConf, eObj, template)
		{
			let params = {},
				paramNameList = 'ExportList',
				paramNameType = 'ExportType'

			// Change parameter names when downloading template file
			if (template !== false)
			{
				paramNameList = 'ImportList',
				paramNameType = 'ImportType'
			}

			Reflect.set(params, paramNameList, 'true')
			Reflect.set(params, paramNameType, eObj.format)

			// Table list limits
			const limits = listConf.getLimitsValues()
			// ID that records are limited by
			const id = limits.id ?? this.$route.params.id

			// Put the limit values in Navigation (history) before making the request to the server.
			_foreach(limits, (value, key) => {
				const entry = {
					navigationId: this.navigationId,
					key,
					value
				}
				this.setEntryValue(entry)
			})

			const tableConfiguration = listFunctions.getTableConfiguration(listConf)

			return netAPI.postData(
				listConf.controller,
				listConf.action,
				{ queryParams: params, id, tableConfiguration },
				(data) => {
					// Make call to download file using the response URL
					netAPI.postData(
						data.controller,
						data.action,
						{
							id: data.id,
							type: eObj.format
						},
						(_, request) => netAPI.forceDownload(request.data, data.id),
						undefined,
						{ responseType: 'arraybuffer' },
						this.navigationId)
				},
				undefined,
				{ params },
				this.navigationId)
		},

		/**
		 * Import table data from file
		 * @param {object} listConf The list configuration
		 * @param {object} eObj
		 * @returns A promise with the response from the server.
		 */
		onTableListImportData(listConf, eObj)
		{
			var params = {}

			Reflect.set(params, 'importType', eObj.format)
			Reflect.set(params, 'qqfile', eObj.fileName)

			let formData = new FormData()
			formData.append('file', eObj.file)

			return netAPI.postData(
				listConf.controller,
				`${listConf.action}_UploadFile`,
				formData,
				(data) => listConf.dataImportResponse = data,
				undefined,
				{
					params,
					headers: { 'Content-Type': 'multipart/form-data' }
				},
				this.navigationId)
		},

		/**
		 *
		 * @param {object} listConf The list configuration
		 * @param {object} eObj
		 */
		onTableListApplyColumnConfig(listConf, eObj)
		{
			var columnsOrdered = []
			var columnCfg = {}

			// Column order and visibility
			if (eObj.columnOrder !== undefined && eObj.columnOrder !== null)
			{
				// Iterate column configuration data
				for (let idxCfg in eObj.columnOrder)
				{
					columnCfg = eObj.columnOrder[idxCfg]

					// Find column, set properties and add to ordered columns array
					let idx = listConf.columns.findIndex((x) => x.name === columnCfg.Fields.formField)
					if (idx > -1)
					{
						let currentColumn = cloneDeep(listConf.columns[idx])
						currentColumn.formField = columnCfg.Fields.formField
						currentColumn.position = columnCfg.Fields.order
						currentColumn.visibility = columnCfg.Fields.visibility

						columnsOrdered.push(currentColumn)
					}
				}

				// Set columns to columns configured by user
				listConf.columnsCustom = columnsOrdered
				listConf.config.hasCustomColumns = true
			}

			// Default search column
			if (eObj.defaultSearchColumn !== undefined && eObj.defaultSearchColumn !== null)
				listConf.config.defaultSearchColumnName = eObj.defaultSearchColumn
		},

		/**
		 * Reset column configuration
		 * @param {object} listConf The list configuration
		 */
		onTableListResetColumnConfig(listConf)
		{
			listConf.columnsCustom = []
			listConf.config.defaultSearchColumnName = listConf.config.defaultSearchColumnNameOriginal
		},

		/**
		 * Reset column sizes
		 * @param {object} listConf The list configuration
		 */
		onTableListResetColumnSizes(listConf)
		{
			listConf.signal = { resetColumnSizes: true }
		},

		/**
		 * Update the value of the id of the active view mode.
		 * @param {object} listConf The list configuration
		 * @param {object} id The id of the active view mode
		 */
		updateActiveViewMode(listConf, id)
		{
			listConf.activeViewModeId = id

			// Re-initialize the table configuration options
			listConf.initUserConfig()
		},

		/**
		 * Add row to array of dirty rows
		 * @param {object} listConf The list configuration
		 * @param {object} eObj
		 * @param {boolean} isDirty
		 * @returns Boolean
		 */
		onRowDirty(listConf, eObj, isDirty)
		{
			if (isDirty)
				listConf.rowsDirty[eObj] = true
			else
				delete listConf.rowsDirty[eObj]
		},

		/**
		 * Remove row from array of rows
		 * @param rows {Object}
		 * @param rowKey {Object}
		 * @returns
		 */
		onRemoveRow(rows, rowKey)
		{
			var rowIdx = rows.findIndex((elem) => elem.rowKey === rowKey)
			rows.splice(rowIdx, 1)
		},

		/**
		 * Signal that something just happened to a row.
		 * Depends on table configuration.
		 * @param {object} listConf The list configuration
		 * @param {object} eObj
		 * @returns Boolean
		 */
		onGoToRow(listConf, eObj)
		{
			// If single row selection is enabled, select the row
			if (listConf.config.rowClickActionInternal === 'selectSingle')
				this.onSelectRow(listConf, { rowKeyPath: eObj })
			else
			{
				let row = listFunctions.getRowByKeyPath(listConf.rows, eObj)

				if (row)
				{
					row.isHighlighted = true
					setTimeout(() => {
						// Prevent re-rendering again and causing an infinite loop
						listConf.config.rerenderRowsOnNextChange = false
						delete row.isHighlighted
					}, 1500)
				}
			}
		},

		/**
		 * Selects a row in a list - including a confirmation check for dirty extended support forms.
		 * @param {object} listConf - The list configuration
		 * @param {object} eventData - Information for the selection - the row ID (rowKeyPath) and the selection type (single or multiple)
		 */
		onSelectRow(listConf, eventData)
		{
			let row = listFunctions.getRowByKeyPath(listConf.rows, eventData.rowKeyPath)

			if (!row)
				return

			let rowIdStr = row.rowKey

			const rowsSelected = Object.keys(listConf.rowsSelected)

			if (listConf.newRowID)
				rowsSelected.push(listConf.newRowID)

			if (listConf.linkedForm && rowsSelected.length !== 0 && !_isEmpty(listConf.rowsDirty))
				listConf.linkedForm.handleLeaveForm(() => this.afterRowSelectConfirmation(listConf, row, rowIdStr, eventData))
			else
				this.afterRowSelectConfirmation(listConf, row, rowIdStr, eventData)
		},

		/**
		 * Performs the row selection (auxiliar function to onSelectRow handler)
		 * @param {object} listConf The list configuration
		 * @param {object} rowID The ID of the row to select
		 */
		executeRowSelection(listConf, rowID)
		{
			// Set row ID in hashtable of selected rows
			listConf.rowsSelected[rowID] = true

			this.setEntryValue({
				navigationId: this.navigationId,
				key: `TableListControl_${listConf.id}`,
				value: rowID
			})

			// Remove properties for selecting the row that was previously selected because of doing an action on it
			listConf.config.rowKeyToScroll = ''
		},

		/**
		 * Function that runs after the confirmation to change/select a row
		 * @param {object} listConf The list configuration
		 * @param {object} row The item to select
		 * @param {object} rowIdStr The row id
		 * @param {object} eventData The eventDataused in the selection of the row
		 */
		afterRowSelectConfirmation(listConf, row, rowIdStr, eventData)
		{
			// If we select a different record, the changes made to the previous will be lost - clean dirty rows array
			Object.keys(listConf.rowsDirty).forEach((key) => { delete listConf.rowsDirty[key] })

			if (!eventData.multipleSelection && !_isEmpty(listConf.rowsSelected))
				this.onUnselectAllRows(listConf)

			// Perform row selection
			this.setListReturnControl(listConf, row, true)
			this.executeRowSelection(listConf, rowIdStr)

			// Update form
			listConf.vueContext.internalEvents?.emit('on-table-row-selected', { tableId: listConf.id, row })
		},

		/**
		 * Remove row from array of selected rows
		 * @param {object} listConf The list configuration
		 * @param {object} eObj
		 * @returns Boolean
		 */
		onUnselectRow(listConf, eObj)
		{
			delete listConf.rowsSelected[eObj]
		},

		/**
		 * Add row to array of selected rows
		 * @param {object} listConf The list configuration
		 * @param {object} eObj
		 * @returns Boolean
		 */
		onSelectRows(listConf, eObj)
		{
			for (let rowKey in eObj)
				listConf.rowsSelected[rowKey] = true
		},

		/**
		 * Remove rows from array of selected rows
		 * @param {object} listConf The list configuration
		 * @param {object} eObj
		 * @returns Boolean
		 */
		onUnselectRows(listConf, eObj)
		{
			for (let rowKey in eObj)
				delete listConf.rowsSelected[rowKey]
		},

		/**
		 * Remove all rows from array of selected rows
		 * @param {object} listConf The list configuration
		 * @returns Boolean
		 */
		onUnselectAllRows(listConf)
		{
			for (let rowKey in listConf.rowsSelected)
				delete listConf.rowsSelected[rowKey]
		},

		/**
		 * Convert hashtable of row IDs to array of row IDs
		 * @param {object} rowKeyHashTable
		 * @returns Boolean
		 */
		rowKeyHashTableToArray(rowKeyHashTable)
		{
			return Object.keys(rowKeyHashTable)
		},

		/**
		 * Convert hashtable of row IDs to array of row IDs
		 * @param {object} rowKeyArray
		 * @returns Boolean
		 */
		rowKeyArrayToHashTable(rowKeyArray)
		{
			const rowKeyHashTable = {}

			for (let idx = 0; idx < rowKeyArray.length; idx++)
				rowKeyHashTable[rowKeyArray[idx]] = true

			return rowKeyHashTable
		},

		/**
		 * Row add
		 * @param {object} listConf The list configuration
		 * @param {object} eObj Row object
		 */
		onTableListRowAdd(listConf, eObj)
		{
			var params = {}

			Reflect.set(params, 'partialView', '')
			Reflect.set(params, 'InsertMode', 'true')
			Reflect.set(params, 'Expose', listConf.config.name)

			for (let key in eObj.Fields)
				Reflect.set(params, key, eObj.Fields[key])

			return netAPI.postData(
				listConf.config.tableAlias,
				`${listConf.action}Form_New`,
				params,
				() => this.fetchListData(listConf),
				undefined,
				{ params },
				this.navigationId)
		},

		/**
		 * Row edit
		 * @param {object} listConf The list configuration
		 * @param {object} eObj Row object
		 */
		onTableListRowEdit(listConf, eObj)
		{
			var params = {}

			Reflect.set(params, 'partialView', '')
			Reflect.set(params, 'InsertMode', 'false')
			Reflect.set(params, 'Expose', listConf.config.name)

			for (let key in eObj.Fields)
				Reflect.set(params, key, eObj.Fields[key])

			return netAPI.postData(
				listConf.config.tableAlias,
				`${listConf.action}Form_Edit`,
				params,
				undefined,
				undefined,
				{ params },
				this.navigationId)
		},

		/**
		 * Rows delete
		 * @param {object} listConf The list configuration
		 * @param {object} eObj Hashtable of row primary keys
		 */
		onTableListRowsDelete(listConf, eObj)
		{
			var params = {}

			Reflect.set(params, 'partialView', '')
			Reflect.set(params, 'InsertMode', 'false')
			Reflect.set(params, 'Expose', listConf.config.name)

			var rowKeys = Object.keys(eObj)

			Reflect.set(params, 'rowKeys', rowKeys)

			return netAPI.postData(
				listConf.config.tableAlias,
				`${listConf.action}Form_Delete_Rows`,
				params,
				() => this.fetchListData(listConf),
				undefined,
				{ params },
				this.navigationId)
		},

		/**
		 * Get ordering column of table
		 * @param {object} listConf The list configuration
		 */
		getOrderingColumn(listConf)
		{
			for (let idx in listConf.columns)
			{
				let column = listConf.columns[idx]
				if (column.isOrderingColumn !== undefined && column.isOrderingColumn !== false)
					return column
			}

			return null
		},

		/**
		 * Toggle drag and drop mode
		 * @param {object} listConf The list configuration
		 */
		onToggleRowsDragDrop(listConf)
		{
			listConf.config.hasRowDragAndDrop = !listConf.config.hasRowDragAndDrop

			var sortOrderColumn = this.getOrderingColumn(listConf)
			if (listConf.config.hasRowDragAndDrop && sortOrderColumn)
			{
				sortOrderColumn.component = 'q-edit-numeric'
				sortOrderColumn.componentOptions = { size: 'mini' }
			}
			else
				sortOrderColumn.component = undefined
		},

		/**
		 * Row reorder
		 * @param {object} listConf The list configuration
		 * @param {object} eObj
		 */
		onTableListRowReorder(listConf, eObj)
		{
			const params = {
				id: eObj.rowKey,
				position: eObj.index
			}

			return netAPI.postData(
				listConf.controller,
				`Reorder${listConf.action}`,
				params,
				(data) => {
					listConf.hydrate(listConf, data)
					listConf.isLoaded = true
					// Set row key path to navigate to after reloading
					listConf.config.navigatedRowKeyPath = [eObj.rowKey]
					// Set property to trigger focusing on the right element after reloading
					listConf.config.setNavOnUpdate = true
				},
				undefined,
				{ params },
				this.navigationId)
		},

		/**
		 * Run group action on selected rows
		 * @param {object} listConf The list configuration
		 * @param {object} eObj
		 */
		onTableListRowGroupAction(listConf, eObj)
		{
			let params = {}

			// Set selected ids
			Reflect.set(params, 'ids', Object.keys(eObj.rowsSelected))

			// Set all selected param
			params.allSelected = eObj.allSelected === 'true' || eObj.allSelected === true

			// Add table configuration
			params.tableConfiguration = listFunctions.getTableConfiguration(listConf)

			switch (eObj.action.params.type)
			{
				case 'menu':
					return netAPI.postData(
						listConf.controller,
						`${listConf.action}_Selections`,
						params,
						() => {
							// Store temporary table configuration for when returning
							this.setListReturnControl(listConf, null, true)

							// Go to follow-up menu list
							// Use the method defined in the action object. The same method normally used to navigate to menu lists.
							eObj.action.params.action(listConf, eObj.action, listFunctions.getRowByKeyPath(listConf.rows, params.ids[0]))
						},
						undefined,
						{ params },
						this.navigationId)
				case 'form':
					return netAPI.postData(
						listConf.controller,
						`${listConf.action}_Selections`,
						params,
						() => {
							// Store temporary table configuration for when returning
							this.setListReturnControl(listConf, null, true)

							// Go to follow-up form
							if (params.ids.length > 0)
							{
								let routeOptions = {}
								if (Number.isInteger(eObj.action.params.goBack))
									Reflect.set(routeOptions, 'goBack', eObj.action.params.goBack)
								this.navigateToForm(eObj.action.params.formName, eObj.action.params.mode, params.ids[0], routeOptions)
							}
						},
						undefined,
						{ params },
						this.navigationId)
				case 'routine':
					// Call routine
					eObj.action.params.actionRoutine(params)
					break
				case 'qsign':
					eObj.action.params.actionRoutine(params)
					break
				case 'report':
					return netAPI.postData(
						listConf.controller,
						`${listConf.action}_Selections`,
						params,
						// Go to follow-up report
						() => this.navigateToReport(eObj.action.params.baseArea, eObj.action.name, { allSelected: params.allSelected }),
						undefined,
						{ params },
						this.navigationId)
				default:
					if (typeof eObj.action.params.action === 'function')
						eObj.action.params.action(params)
					break
			}
		},

		/**
		 * Get new record data
		 * @param {object} listConf The list configuration
		 * @param {object} eObj Row object
		 */
		onTableListInsertRow(listConf, eObj)
		{
			var controller = listConf.config.tableAlias
			var action = listConf.action + '_New'

			if (eObj.controller)
				controller = eObj.controller
			if (eObj.action)
				action = eObj.action

			return netAPI.postData(
				controller,
				action,
				null,
				(data) => {
					if (data.QPrimaryKey !== undefined && data.QPrimaryKey !== null)
						listConf.newRowID = data.QPrimaryKey
				},
				undefined,
				undefined,
				this.navigationId)
		},

		/**
		 * Called when saving a new record
		 * @param {object} listConf The list configuration
		 */
		onTableListInsertForm(listConf)
		{
			listConf.newRowID = ''
		},

		/**
		 * Get new record data
		 * @param {object} listConf The list configuration
		 * @param {object} eObj Row object
		 */
		onTableListCancelInsertRow(listConf, eObj)
		{
			var controller = listConf.config.tableAlias
			var action = null
			var addAction = _find(listConf.config.generalActions, (act) => act.id === 'insert')
			action = `MF${addAction.params.formName}_Cancel`

			if (eObj.controller)
				controller = eObj.controller
			if (eObj.action)
				action = eObj.action

			return netAPI.postData(
				controller,
				action,
				{ id: listConf.newRowID },
				(data) => {
					if (data.Success)
						listConf.newRowID = ''
				},
				undefined,
				undefined,
				this.navigationId)
		},

		/**
		 * Sets the row to highlight when the user returns to the list
		 * @param {object} listConf The list configuration
		 * @param {object} row The row
		 * @param {boolean} storeTableConfig Whether to store the table configuration to use when returning
		 */
		setListReturnControl(listConf, row, storeTableConfig = false)
		{
			// Get table configuration to use when returning
			const tableConfig = storeTableConfig ? listFunctions.getTableConfiguration(listConf) : undefined

			if (listConf.type === 'TreeList')
			{
				this.setCurrentControl({
					navigationId: this.navigationId,
					controlData: {
						id: listConf.id,
						data: {
							rowKey: listConf.config.rowKeyToScroll
						}
					}
				})
			}
			else
			{
				this.setCurrentControl({
					navigationId: this.navigationId,
					controlData: {
						id: listConf.id,
						data: {
							rowKey: row?.rowKey,
							tableConfig: tableConfig
						}
					}
				})
			}
		},

		/**
		 *
		 * @param {object} listConf The list configuration
		 * @param {object} eObj
		 */
		async onTableListExecuteAction(listConf, eObj)
		{
			// Download file
			if (!_isEmpty(eObj) && eObj.action === 'download' && eObj.ticket && eObj.fileName)
			{
				netAPI.getFile(
					listConf.config.tableAlias,
					eObj.ticket,
					eObj.viewType,
					this.navigationId)
				return
			}

			// Insert in multiforms
			if (listConf.type === 'Multiform' && eObj.name === 'insert')
			{
				const addAction = _find(listConf.config.generalActions, (act) => act.id === 'insert')
				eObj.controller = listConf.config.tableAlias
				eObj.action = `${addAction.params.formName}_NEW_GET`
				this.onTableListInsertRow(listConf, eObj)
				return
			}

			let actionCfg = null
			let actionId = null

			// If custom action is already given
			if (eObj.action)
			{
				actionCfg = eObj.action
				actionId = eObj.action.id
			}
			else
				actionId = eObj.id

			// If the action is not defined, do nothing
			if (!actionId)
				return

			// Find the action by it's id
			// CRUD
			if (!actionCfg)
				actionCfg = _find(listConf.config.crudActions, (act) => act.id === actionId)
			if (!actionCfg)
				actionCfg = _find(listConf.config.generalActions, (act) => act.id === actionId)
			// Custom action
			if (!actionCfg)
				actionCfg = _find(listConf.config.customActions, (act) => act.id === actionId)
			// Row click action
			if (!actionCfg && listConf.config.rowClickAction && listConf.config.rowClickAction.id === actionId)
				actionCfg = listConf.config.rowClickAction
			// General custom action
			if (!actionCfg)
				actionCfg = _find(listConf.config.generalCustomActions, (act) => act.id === actionId)

			if (!actionCfg || !actionCfg.params || typeof actionCfg.params.action !== 'function')
				return

			// Get row key and row key path
			let rowKey
			let rowKeyPath
			if (eObj.rowKeyPath && Array.isArray(eObj.rowKeyPath))
			{
				rowKey = eObj.rowKeyPath[eObj.rowKeyPath.length - 1]
				rowKeyPath = eObj.rowKeyPath
			}
			else if (eObj.rowKey)
			{
				rowKey = eObj.rowKey
				rowKeyPath = [eObj.rowKey]
			}

			// Find row by row key path
			let row = listFunctions.getRowByKeyPath(listConf.rows, rowKeyPath),
				historyEntries = []

			if (listConf.type !== 'TreeList')
			{
				historyEntries.push({
					key: (listConf.config.tableAlias || '').toLowerCase(),
					value: rowKey
				})
			}

			// Prevent the action if it is not allowed
			// Added to account for actions triggered by short-cut keys
			if (!listFunctions.actionIsAllowed(actionCfg, row?.btnPermission, listConf.config.permissions, listConf.readonly))
				return

			// Check if the action is a row-specific action
			let crudAction = _find(listConf.config.crudActions, (act) => act.id === actionId)
			let insertAction = _find(listConf.config.generalActions, (act) => act.id === actionId && act.id === 'insert')
			let rowSpecificAction = crudAction || insertAction

			if (listConf.type === 'TreeList')
			{
				// Set the right tableAlias in the navigation entry
				if (!_isEmpty(row?.Area))
				{
					_foreach(_get(listConf.config.treeListDefinitions, row.Area, []), (branchAreaKey, branchArea) => {
						historyEntries.push({
							key: branchArea,
							value: branchAreaKey(row)
						})
					})
				}

				// BEGIN: Get form name
				let rowFormName = null

				// It's needed to know if it's a CRUD action, because the action form name must be filled with the value from the "row"
				if (crudAction)
					rowFormName = row.Form

				// Shows correct form to open when inserting a record
				if (insertAction)
					rowFormName = listConf.getInsertFormName(row)

				// If the row has a form defined it overrides the action form
				if (rowFormName && rowFormName.length > 0)
					actionCfg.params.formName = rowFormName
				// BEGIN: Get form name

				// FOR: tree table select row on return
				// Tree tables: store path of row keys
				if (row === undefined || row === null)
					listConf.config.rowKeyToScroll = []
				else
					listConf.config.rowKeyToScroll = rowKeyPath
			}

			// For normal tables, if going to a form or sub menu list, store configuration to use when returning
			const storeTableConfig = (rowSpecificAction || actionCfg.params.isRoute) && listConf.activeViewModeId === 'LIST'

			this.setListReturnControl(listConf, row, storeTableConfig)
			this.setParamValue({
				navigationId: this.navigationId,
				key: 'anchor',
				value: listConf.id
			})

			// If the before execute function is defined, execute it and check if we can perform the action on the list.
			if (typeof actionCfg.params.canExecuteAction === 'function')
			{
				const canContinueExecution = await actionCfg.params.canExecuteAction()
				if (!canContinueExecution)
					return
			}

			actionCfg.params.action(listConf, actionCfg, row, historyEntries)
		},

		/**
		 *
		 * @param {object} listConf The list configuration
		 * @param {object} eObj
		 */
		onTableListCellAction(listConf, eObj)
		{
			if (!_isEmpty(eObj) && !_isEmpty(eObj.column) && !_isEmpty(eObj.column.params) && eObj.column.params.type === 'form')
				this.openFormAction(listConf, eObj.column, eObj.row)
		},

		/**
		 *
		 * @param {object} listConf The list configuration
		 * @param {object} eObj
		 */
		onTableListUpdateCell(listConf, eObj)
		{
			if (listConf.config.hasRowDragAndDrop)
			{
				const pObj = {
					rowKey: eObj.row.rowKey,
					index: parseInt(eObj.value || 0) - 1
				}
				this.onTableListRowReorder(listConf, pObj)
			}
		},

		/**
		 * Execute action from 'MC' menu
		 * @param {object} listConf The list configuration
		 * @param {object} actionName
		 * @param {string} id
		 */
		tableListMCAction(listConf, actionName, id)
		{
			const actionMC = _find(listConf.config.MCActions, (act) => act.name === actionName)
			const row = _find(listConf.rows, (rw) => rw.rowKey === id)

			if (actionMC && row)
				actionMC.params.action(listConf, actionMC, row)
		},

		/**
		 * Navigates to a form.
		 * @param {object} listConf The list configuration
		 * @param {object} actionCfg Action configuration
		 * @param {object} row The row data object
		 * @param {array} historyEntries The History entries to be applied at the next level
		 */
		openFormAction(listConf, actionCfg, row, historyEntries)
		{
			if (actionCfg.params.type !== 'form')
				return

			// Whether or not the current context is a form.
			let isForm = typeof this.formInfo === 'object' && typeof this.isEditable === 'boolean'

			let formModes = ''
			if (listConf.config.permissions.canView && genericFunctions.btnHasPermission(row?.btnPermission, qEnums.formModes.show))
				formModes += 'v'
			if (!isForm || this.isEditable)
			{
				if (listConf.config.permissions.canEdit && genericFunctions.btnHasPermission(row?.btnPermission, qEnums.formModes.edit))
					formModes += 'e'
				if (listConf.config.permissions.canDuplicate && genericFunctions.btnHasPermission(row?.btnPermission, qEnums.formModes.duplicate))
					formModes += 'd'
				if (listConf.config.permissions.canDelete && genericFunctions.btnHasPermission(row?.btnPermission, qEnums.formModes.delete))
					formModes += 'a'
				if (listConf.config.permissions.canInsert && genericFunctions.btnHasPermission(row?.btnPermission, qEnums.formModes.new))
					formModes += 'i'
			}

			let formName = actionCfg.params.formName,
				mode = actionCfg.params.mode,
				id = null,
				formDef = listConf.config.formsDefinition[formName],
				options = {
					isPopup: formDef.isPopup,
					repeatInsert: actionCfg.params.repeatInsertion,
					isDuplicate: false,
					modes: formModes
				},
				query = {},
				prefillValues = actionCfg.params.prefillValues || {}

			// Apply history limits that cannot be applied at the form level.
			// (See description in the formHandlers prop)
			if (Array.isArray(historyEntries))
				Reflect.set(options, 'historyEntries', JSON.stringify(historyEntries))

			// GoBack pattern (menus)
			if (Number.isInteger(actionCfg.params.goBack))
				Reflect.set(options, 'goBack', actionCfg.params.goBack)

			// Controlled change for other route. e.g: Support form
			if (actionCfg.params.isControlled)
				Reflect.set(options, 'isControlled', true)

			// Other options
			if (actionCfg.params.otherOptions)
			{
				for (let prop in actionCfg.params.otherOptions)
					if (Object.prototype.hasOwnProperty.call(actionCfg.params.otherOptions, prop))
						Reflect.set(options, prop, actionCfg.params.otherOptions[prop])
			}

			let tableName = listConf.controller[0] + listConf.controller.substring(1).toLowerCase()
			let tableViewModelName = listConf.action + '_ViewModel'
			this.setEntryValue({ navigationId: this.navigationId, key: 'TableName', value: tableName })
			this.setEntryValue({ navigationId: this.navigationId, key: 'TableViewModelName', value: tableViewModelName })

			if (mode === 'DUPLICATE')
				options.isDuplicate = true

			if (mode !== 'NEW')
			{
				id = formDef.fnKeySelector(row)
				if (!_isEmpty(actionCfg.params.limits))
				{
					_foreach(actionCfg.params.limits, (limit) => {
						if (limit.identifier === 'id')
							id = limit.fnValueSelector(row.Fields)
						else
							Reflect.set(options, limit.identifier, limit.fnValueSelector(row.Fields))
					})
				}
			}
			else
				options.isControlled = true

			this.navigateToForm(formName, mode, id, options, query, prefillValues)
		},

		/**
		 * Navigates to a menu.
		 * @param {object} _ The list configuration
		 * @param {object} actionCfg Action configuration
		 * @param {object} row The row data object
		 */
		openMenuAction(_, actionCfg, row)
		{
			if (actionCfg.params.type !== 'menu')
				return

			var params = {}

			if (!_isEmpty(actionCfg.params.limits))
			{
				_foreach(actionCfg.params.limits, (limit) => {
					let limitValue = limit.fnValueSelector(row.Fields)
					Reflect.set(params, limit.identifier, limitValue)
					this.setEntryValue({ navigationId: this.navigationId, key: limit.identifier, value: limitValue })
				})
			}

			this.navigateToRouteName(`menu-${actionCfg.params.menuName}`, params)
		},

		/**
		 *
		 * @param {*} _
		 * @param {*} actionCfg
		 * @param {*} row
		 */
		openReportAction(_, actionCfg, row)
		{
			if (actionCfg.params.type !== 'report' && actionCfg.params.type !== 'ssrsViewer')
				return

			if (!_isEmpty(actionCfg.params.limits))
			{
				_foreach(actionCfg.params.limits, (limit) => {
					let limitValue = limit.fnValueSelector(row.Fields)
					Reflect.set(actionCfg.params, limit.identifier, limitValue)
					this.setEntryValue({ navigationId: this.navigationId, key: limit.identifier, value: limitValue })
				})
			}

			if (actionCfg.params.type === 'report')
				this.navigateToReport(actionCfg.params.baseArea, actionCfg.name, actionCfg.params)
			else if (actionCfg.params.type === 'ssrsViewer')
				this.navigateToReportingServicesViewer(actionCfg.params.baseArea, actionCfg.name, actionCfg.params)
		},

		/**
		 *
		 * @param {*} _
		 * @param {*} actionCfg
		 * @param {*} row
		 */
		openRoutineAction(_, actionCfg, row)
		{
			if (actionCfg.params.type !== 'routine')
				return

			let params = {}

			if (!_isEmpty(actionCfg.params.limits))
			{
				_foreach(actionCfg.params.limits, (limit) => {
					params[limit.identifier] = limit.fnValueSelector(row.Fields)
				})
			}

			if (actionCfg.params.actionRoutine)
				actionCfg.params.actionRoutine(params)
		},

		/**
		 *
		 * @param {*} _
		 * @param {*} actionCfg
		 * @param {*} row
		 */
		openQSignAction(_, actionCfg, row)
		{
			if (actionCfg.params.type !== 'qsign')
				return

			let params = {
				id: _get(row, 'rowKey', null)
			}

			if (!_isEmpty(actionCfg.params.limits))
			{
				_foreach(actionCfg.params.limits, (limit) => {
					params[limit.identifier] = limit.fnValueSelector(row.Fields)
				})
			}

			if (actionCfg.params.actionRoutine)
				actionCfg.params.actionRoutine(params)
		},

		/**
		 * Adds a route that indicates if all table rows are selected or not
		 * @param {*} value The value to put in the parameter value
		 */
		onSetQtableAllSelected(listConf, value)
		{
			let allSelected = this.navigation.currentLevel.params.allSelected || []
			// "allSelectedRows" is a string due to an issue where Vue won't recognize changes to boolean props
			listConf.allSelectedRows = value.isSelected.toString().toLowerCase()

			if (value.isSelected)
			{
				if (!allSelected.includes(value.id))
					allSelected.push(value.id)
			}
			else
			{
				// Remove all selected
				const idx = allSelected.findIndex((e) => e === value.id)
				if (idx === -1)
					return // No need to continue!

				allSelected.splice(idx, 1)
			}

			this.navigation.currentLevel.params.allSelected = allSelected
		},

		/**
		 * Adds a route that indicates if all table rows are selected or not
		 * @param {*} listConfig
		 * @param {*} tableId
		 */
		onFetchQtableAllSelected(listConfig, tableId)
		{
			let allSelected = this.navigation.currentLevel.params.allSelected || []

			if (allSelected.findIndex((e) => e === tableId) !== -1)
				listConfig.allSelectedRows = 'true'
		}
	}
}
