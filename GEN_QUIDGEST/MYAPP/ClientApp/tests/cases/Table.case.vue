<template>
	<div class="q-table-list">
		<div>
			<q-table
				:name="tableTest.config.name"
				:rows="tableTest.rows"
				:columns="tableTest.columns"
				:config="tableTest.config"
				:total-rows="tableTest.totalRows"
				:advanced-filters="tableTest.advancedFilters"
				:group-filters="tableTest.groupFilters"
				:active-filters="tableTest.activeFilters"
				:header-level="1"
				:readonly="tableTest.readonly"
				:rows-checked="tableTest.rowsChecked"
				@execute-action="executeAction"
				@row-action="rowAction"
				@cell-action="cellAction"
				@on-export-data="exportDataAction"
				@on-import-data="importDataAction"
				@on-export-template="exportTemplateAction"
				@show-popup="showPopupAction(tableTest, $event)"
				@hide-popup="hidePopupAction(tableTest, $event)"
				@show-column-config="tableTest.config.columnConfigIsVisible = true"
				@show-advanced-filters="setAdvancedFiltersPopup(tableTest, [true, -1])"
				@add-advanced-filter="(...args) => addAdvancedFilter(tableTest, ...args)"
				@deactivate-all-advanced-filters="deactivateAllAdvancedFilters(tableTest)"
				@on-save-column-config="saveColumnConfig"
				@on-reset-column-config="resetColumnConfig"
				@check-row="checkRow(tableTest.rowsChecked, $event)"
				@uncheck-row="uncheckRow(tableTest.rowsChecked, $event)"
				@check-rows="checkRows(tableTest.rowsChecked, $event)"
				@uncheck-all-rows="uncheckAllRows(tableTest.rowsChecked)"
				@set-dropdown="setDropdown" />
			<q-table-column-config
				v-bind="tableTest.config"
				modal-id="column-config"
				:columns="tableTest.columns"
				:server-mode="tableTest.config.serverMode"
				:is-visible="tableTest.config.columnConfigIsVisible"
				:has-text-wrap="tableTest.config.hasTextWrap"
				:texts="tableTest.texts"
				@show-popup="showPopupAction(tableTest, $event)"
				@hide-popup="hidePopupAction(tableTest, $event)"
				@save-column-config="saveColumnConfig"
				@reset-column-config="resetColumnConfig"
				@toggle-text-wrap="tableTest.config.hasTextWrap = !tableTest.config.hasTextWrap" />
			<q-table-advanced-filters
				v-if="tableTest.config.allowAdvancedFilters"
				modal-id="advanced-filters"
				:columns="tableTest.columns"
				:filters="tableTest.advancedFilters"
				:search-filter-data="tableTest.config.searchFilterData"
				:texts="tableTest.texts"
				:has-advanced-filters-active="tableTest.config.hasAdvancedFiltersActive"
				:server-mode="tableTest.config.serverMode"
				:signal-open="tableTest.config.signalOpenAdvancedFilters"
				@show-popup="showPopupAction(tableTest, $event)"
				@hide-popup="hidePopupAction(tableTest, $event)"
				@add-advanced-filter="(...args) => addAdvancedFilter(tableTest, ...args)"
				@edit-advanced-filter="(...args) => editAdvancedFilter(tableTest, ...args)"
				@set-advanced-filter-state="(...args) => setAdvancedFilterState(tableTest, ...args)"
				@set-advanced-filter-states="(...args) => setAdvancedFilterStates(tableTest, ...args)"
				@deactivate-all-advanced-filters="(...args) => deactivateAllAdvancedFilters(tableTest, ...args)"
				@remove-advanced-filter="(...args) => removeAdvancedFilter(tableTest, ...args)" />
		</div>

		<div>
			<q-table
				:name="tableTest.config.name"
				:rows="tableTest.rows"
				:columns="tableTest.columns"
				:config="{ ...tableTest.config, ...{ tableTitle: 'BASIC TYPES (READ-ONLY)' } }"
				:total-rows="tableTest.totalRows"
				:group-filters="tableTest.groupFilters"
				:active-filters="tableTest.activeFilters"
				:header-level="1"
				readonly
				@execute-action="executeAction"
				@row-action="rowAction"
				@cell-action="cellAction"
				@on-export-data="exportDataAction"
				@on-import-data="displayEmit"
				@on-export-template="exportTemplateAction"
				@show-popup="showPopupAction"
				@hide-popup="hidePopupAction"
				@check-row="checkRow(tableTest.rowsChecked, $event)"
				@uncheck-row="uncheckRow(tableTest.rowsChecked, $event)"
				@check-rows="checkRows(tableTest.rowsChecked, $event)"
				@uncheck-all-rows="uncheckAllRows(tableTest.rowsChecked)"
				@set-dropdown="setDropdown" />
		</div>

		<div>
			<q-table
				:name="tableTest.config.name"
				:rows="tableTest.rows"
				:columns="tableTest.columns"
				:config="{ ...tableTest.config, ...{ tableTitle: 'BASIC TYPES (LIMITS)', showLimitsInfo: true } }"
				:total-rows="tableTest.totalRows"
				:advanced-filters="tableTest.advancedFilters"
				:group-filters="tableTest.groupFilters"
				:active-filters="tableTest.activeFilters"
				:header-level="1"
				:readonly="tableTest.readonly"
				:rows-checked="tableTest.rowsChecked"
				:table-limits="tableLimits01"
				@execute-action="executeAction"
				@row-action="rowAction"
				@cell-action="cellAction"
				@on-export-data="exportDataAction"
				@on-import-data="importDataAction"
				@on-export-template="exportTemplateAction"
				@show-popup="showPopupAction(tableTest, $event)"
				@hide-popup="hidePopupAction(tableTest, $event)"
				@show-column-config="tableTest.config.columnConfigIsVisible = true"
				@show-advanced-filters="setAdvancedFiltersPopup(tableTest, [true, -1])"
				@add-advanced-filter="(...args) => addAdvancedFilter(tableTest, ...args)"
				@deactivate-all-advanced-filters="deactivateAllAdvancedFilters(tableTest)"
				@on-save-column-config="saveColumnConfig"
				@on-reset-column-config="resetColumnConfig"
				@check-row="checkRow(tableTest.rowsChecked, $event)"
				@uncheck-row="uncheckRow(tableTest.rowsChecked, $event)"
				@check-rows="checkRows(tableTest.rowsChecked, $event)"
				@uncheck-all-rows="uncheckAllRows(tableTest.rowsChecked)"
				@set-dropdown="setDropdown" />
		</div>

		<div>
			<q-table
				:name="tableTest.config.name"
				:rows="tableTest.rows"
				:columns="tableTest.columns.map(obj => ({ ...obj, scrollData: 5 }))"
				:config="{ ...tableTest.config, ...{ tableTitle: 'BASIC TYPES (SCROLL)' } }"
				:total-rows="tableTest.totalRows"
				:advanced-filters="tableTest.advancedFilters"
				:group-filters="tableTest.groupFilters"
				:active-filters="tableTest.activeFilters"
				:header-level="1"
				:readonly="tableTest.readonly"
				:rows-checked="tableTest.rowsChecked"
				:table-limits="tableLimits01"
				@execute-action="executeAction"
				@row-action="rowAction"
				@cell-action="cellAction"
				@on-export-data="exportDataAction"
				@on-import-data="importDataAction"
				@on-export-template="exportTemplateAction"
				@show-popup="showPopupAction(tableTest, $event)"
				@hide-popup="hidePopupAction(tableTest, $event)"
				@show-column-config="tableTest.config.columnConfigIsVisible = true"
				@show-advanced-filters="setAdvancedFiltersPopup(tableTest, [true, -1])"
				@add-advanced-filter="(...args) => addAdvancedFilter(tableTest, ...args)"
				@deactivate-all-advanced-filters="deactivateAllAdvancedFilters(tableTest)"
				@on-save-column-config="saveColumnConfig"
				@on-reset-column-config="resetColumnConfig"
				@check-row="checkRow(tableTest.rowsChecked, $event)"
				@uncheck-row="uncheckRow(tableTest.rowsChecked, $event)"
				@check-rows="checkRows(tableTest.rowsChecked, $event)"
				@uncheck-all-rows="uncheckAllRows(tableTest.rowsChecked)"
				@set-dropdown="setDropdown" />
		</div>

		<div>
			<q-table
				:name="tableTestEdit.config.name"
				:rows="tableTestEdit.rows"
				:columns="tableTestEdit.columns"
				:config="tableTestEdit.config"
				:total-rows="tableTestEdit.totalRows"
				:group-filters="tableTestEdit.groupFilters"
				:active-filters="tableTestEdit.activeFilters"
				:header-level="1"
				:readonly="tableTestEdit.readonly"
				@execute-action="executeAction"
				@row-action="rowAction"
				@cell-action="cellAction"
				@on-export-data="exportDataAction"
				@on-import-data="displayEmit"
				@on-export-template="exportTemplateAction"
				@show-popup="showPopupAction"
				@hide-popup="hidePopupAction"
				@set-dropdown="setDropdown" />
		</div>

		<div>
			<q-table
				:name="tableTestReorder.config.name"
				:rows="tableTestReorder.rows"
				:columns="tableTestReorder.columns"
				:config="tableTestReorder.config"
				:total-rows="tableTestReorder.totalRows"
				:group-filters="tableTestReorder.groupFilters"
				:active-filters="tableTestReorder.activeFilters"
				:header-level="1"
				:readonly="tableTestReorder.readonly"
				@execute-action="executeAction"
				@row-action="rowAction"
				@cell-action="cellAction"
				@on-export-data="exportDataAction"
				@on-import-data="displayEmit"
				@on-export-template="exportTemplateAction"
				@show-popup="showPopupAction"
				@hide-popup="hidePopupAction"
				@set-dropdown="setDropdown" />
		</div>

		<div>
			<q-table
				:name="tableTestDates.config.name"
				:rows="tableTestDates.rows"
				:columns="tableTestDates.columns"
				:config="tableTestDates.config"
				:total-rows="tableTestDates.totalRows"
				@execute-action="executeAction"
				@row-action="rowAction"
				@cell-action="cellAction"
				@set-dropdown="setDropdown" />
		</div>

		<div>
			<q-table
				:name="tableTestOther.config.name"
				:rows="tableTestOther.rows"
				:columns="tableTestOther.columns"
				:config="{ ...tableTestOther.config, ...{ tableTitle: 'OTHER TYPES' } }"
				:total-rows="tableTestOther.totalRows"
				:advanced-filters="tableTestOther.advancedFilters"
				:group-filters="tableTestOther.groupFilters"
				:active-filters="tableTestOther.activeFilters"
				:header-level="1"
				:readonly="tableTestOther.readonly"
				:rows-checked="tableTestOther.rowsChecked"
				:table-limits="tableLimits01"
				@execute-action="executeAction"
				@row-action="rowAction"
				@cell-action="cellAction"
				@on-export-data="exportDataAction"
				@on-import-data="importDataAction"
				@on-export-template="exportTemplateAction"
				@show-popup="showPopupAction(tableTestOther, $event)"
				@hide-popup="hidePopupAction(tableTestOther, $event)"
				@show-column-config="tableTestOther.config.columnConfigIsVisible = true"
				@show-advanced-filters="setAdvancedFiltersPopup(tableTestOther, [true, -1])"
				@add-advanced-filter="(...args) => addAdvancedFilter(tableTestOther, ...args)"
				@deactivate-all-advanced-filters="deactivateAllAdvancedFilters(tableTestOther)"
				@on-save-column-config="saveColumnConfig"
				@on-reset-column-config="resetColumnConfig"
				@check-row="checkRow(tableTestOther.rowsChecked, $event)"
				@uncheck-row="uncheckRow(tableTestOther.rowsChecked, $event)"
				@check-rows="checkRows(tableTestOther.rowsChecked, $event)"
				@uncheck-all-rows="uncheckAllRows(tableTestOther.rowsChecked)"
				@set-dropdown="setDropdown" />
		</div>

		<div>
			<q-table
				:name="tableTestOther.config.name"
				:rows="tableTestOther.rows"
				:columns="tableTestOther.columns.map(obj => ({ ...obj, scrollData: 5 }))"
				:config="{ ...tableTestOther.config, ...{ tableTitle: 'OTHER TYPES (SCROLL)' } }"
				:total-rows="tableTestOther.totalRows"
				:advanced-filters="tableTestOther.advancedFilters"
				:group-filters="tableTestOther.groupFilters"
				:active-filters="tableTestOther.activeFilters"
				:header-level="1"
				:readonly="tableTestOther.readonly"
				:rows-checked="tableTestOther.rowsChecked"
				:table-limits="tableLimits01"
				@execute-action="executeAction"
				@row-action="rowAction"
				@cell-action="cellAction"
				@on-export-data="exportDataAction"
				@on-import-data="importDataAction"
				@on-export-template="exportTemplateAction"
				@show-popup="showPopupAction(tableTestOther, $event)"
				@hide-popup="hidePopupAction(tableTestOther, $event)"
				@show-column-config="tableTestOther.config.columnConfigIsVisible = true"
				@show-advanced-filters="setAdvancedFiltersPopup(tableTestOther, [true, -1])"
				@add-advanced-filter="(...args) => addAdvancedFilter(tableTestOther, ...args)"
				@deactivate-all-advanced-filters="deactivateAllAdvancedFilters(tableTestOther)"
				@on-save-column-config="saveColumnConfig"
				@on-reset-column-config="resetColumnConfig"
				@check-row="checkRow(tableTestOther.rowsChecked, $event)"
				@uncheck-row="uncheckRow(tableTestOther.rowsChecked, $event)"
				@check-rows="checkRows(tableTestOther.rowsChecked, $event)"
				@uncheck-all-rows="uncheckAllRows(tableTestOther.rowsChecked)"
				@set-dropdown="setDropdown" />
		</div>

		<div>
			<q-table
				:name="tableTestTotaler.config.name"
				:rows="tableTestTotaler.rows"
				:columns="tableTestTotaler.columns"
				:config="tableTestTotaler.config"
				:total-rows="tableTestTotaler.totalRows"
				@execute-action="executeAction"
				@row-action="rowAction"
				@cell-action="cellAction"
				@select-row="selectRow(tableTestTotaler.rowsSelected, $event)"
				@unselect-row="unselectRow(tableTestTotaler.rowsSelected, $event)"
				@select-rows="selectRows(tableTestTotaler.rowsSelected, $event)"
				@unselect-all-rows="unselectAllRows(tableTestTotaler.rowsSelected)"
				@set-dropdown="setDropdown" />
		</div>

		<div>
			<q-table
				:name="tableTestTotalerSelected.config.name"
				:rows="tableTestTotalerSelected.rows"
				:columns="tableTestTotalerSelected.columns"
				:config="tableTestTotalerSelected.config"
				:total-rows="tableTestTotalerSelected.totalRows"
				:rows-selected="tableTestTotalerSelected.rowsSelected"
				@execute-action="executeAction"
				@row-action="rowAction"
				@cell-action="cellAction"
				@select-row="selectRow(tableTestTotalerSelected.rowsSelected, $event)"
				@unselect-row="unselectRow(tableTestTotalerSelected.rowsSelected, $event)"
				@select-rows="selectRows(tableTestTotalerSelected.rowsSelected, $event)"
				@unselect-all-rows="unselectAllRows(tableTestTotalerSelected.rowsSelected)"
				@set-dropdown="setDropdown" />
		</div>

		<div>
			<q-table
				:name="tableTestSelectMultiple.config.name"
				:rows="tableTestSelectMultiple.rows"
				:columns="tableTestSelectMultiple.columns"
				:config="tableTestSelectMultiple.config"
				:total-rows="tableTestSelectMultiple.totalRows"
				:rows-selected="tableTestSelectMultiple.rowsSelected"
				:rows-checked="tableTestSelectMultiple.rowsChecked"
				@execute-action="executeAction"
				@row-action="rowAction"
				@cell-action="cellAction"
				@select-row="selectRow(tableTestSelectMultiple.rowsSelected, $event)"
				@unselect-row="unselectRow(tableTestSelectMultiple.rowsSelected, $event)"
				@select-rows="selectRows(tableTestSelectMultiple.rowsSelected, $event)"
				@unselect-all-rows="unselectAllRows(tableTestSelectMultiple.rowsSelected)"
				@check-row="checkRow(tableTestSelectMultiple.rowsChecked, $event)"
				@uncheck-row="uncheckRow(tableTestSelectMultiple.rowsChecked, $event)"
				@check-rows="checkRows(tableTestSelectMultiple.rowsChecked, $event)"
				@uncheck-all-rows="uncheckAllRows(tableTestSelectMultiple.rowsChecked)"
				@check-selected-rows="checkSelectedRows(tableTestSelectMultiple)"
				@select-checked-rows="selectCheckedRows(tableTestSelectMultiple)"
				@row-group-action="rowGroupAction"
				@set-dropdown="setDropdown" />
		</div>

		<div>
			<q-table
				:name="tableTestSelectMultipleMultiAction.config.name"
				:rows="tableTestSelectMultipleMultiAction.rows"
				:columns="tableTestSelectMultipleMultiAction.columns"
				:config="tableTestSelectMultipleMultiAction.config"
				:total-rows="tableTestSelectMultipleMultiAction.totalRows"
				:rows-selected="tableTestSelectMultipleMultiAction.rowsSelected"
				:rows-checked="tableTestSelectMultipleMultiAction.rowsChecked"
				@execute-action="executeAction"
				@row-action="rowAction"
				@cell-action="cellAction"
				@select-row="selectRow(tableTestSelectMultipleMultiAction.rowsSelected, $event)"
				@unselect-row="unselectRow(tableTestSelectMultipleMultiAction.rowsSelected, $event)"
				@select-rows="selectRows(tableTestSelectMultipleMultiAction.rowsSelected, $event)"
				@unselect-all-rows="unselectAllRows(tableTestSelectMultipleMultiAction.rowsSelected)"
				@check-row="checkRow(tableTestSelectMultipleMultiAction.rowsChecked, $event)"
				@uncheck-row="uncheckRow(tableTestSelectMultipleMultiAction.rowsChecked, $event)"
				@check-rows="checkRows(tableTestSelectMultipleMultiAction.rowsChecked, $event)"
				@uncheck-all-rows="uncheckAllRows(tableTestSelectMultipleMultiAction.rowsChecked)"
				@check-selected-rows="checkSelectedRows(tableTestSelectMultipleMultiAction)"
				@select-checked-rows="selectCheckedRows(tableTestSelectMultipleMultiAction)"
				@row-group-action="rowGroupAction"
				@set-dropdown="setDropdown" />
		</div>

		<div>
			<q-table
				:name="tableTestSelectSingle.config.name"
				:rows="tableTestSelectSingle.rows"
				:columns="tableTestSelectSingle.columns"
				:config="tableTestSelectSingle.config"
				:total-rows="tableTestSelectSingle.totalRows"
				:rows-selected="tableTestSelectSingle.rowsSelected"
				:rows-checked="tableTestSelectSingle.rowsChecked"
				@execute-action="executeAction"
				@row-action="rowAction"
				@cell-action="cellAction"
				@select-row="selectRow(tableTestSelectSingle.rowsSelected, $event)"
				@unselect-row="unselectRow(tableTestSelectSingle.rowsSelected, $event)"
				@select-rows="selectRows(tableTestSelectSingle.rowsSelected, $event)"
				@unselect-all-rows="unselectAllRows(tableTestSelectSingle.rowsSelected)"
				@check-row="checkRow(tableTestSelectSingle.rowsChecked, $event)"
				@uncheck-row="uncheckRow(tableTestSelectSingle.rowsChecked, $event)"
				@check-rows="checkRows(tableTestSelectSingle.rowsChecked, $event)"
				@uncheck-all-rows="uncheckAllRows(tableTestSelectSingle.rowsChecked)"
				@check-selected-rows="checkSelectedRows(tableTestSelectSingle)"
				@select-checked-rows="selectCheckedRows(tableTestSelectSingle)"
				@set-dropdown="setDropdown" />
		</div>

		<div>
			<q-table
				:name="tableTestRemoveRows.config.name"
				:rows="tableTestRemoveRows.rows"
				:columns="tableTestRemoveRows.columns"
				:config="tableTestRemoveRows.config"
				:total-rows="tableTestRemoveRows.totalRows"
				:rows-selected="tableTestRemoveRows.rowsSelected"
				@execute-action="executeAction"
				@row-action="rowAction"
				@cell-action="cellAction"
				@select-row="selectRow(tableTestRemoveRows.rowsSelected, $event)"
				@unselect-row="unselectRow(tableTestRemoveRows.rowsSelected, $event)"
				@select-rows="selectRows(tableTestRemoveRows.rowsSelected, $event)"
				@unselect-all-rows="unselectAllRows(tableTestRemoveRows.rowsSelected)"
				@remove-row="removeRow(tableTestRemoveRows.rows, $event)"
				@set-dropdown="setDropdown" />
		</div>

		<div>
			<q-table
				:name="tableTestPaginationNormal.config.name"
				:rows="tableTestPaginationNormal.rows"
				:columns="tableTestPaginationNormal.columns"
				:config="tableTestPaginationNormal.config"
				:total-rows="tableTestPaginationNormal.totalRows"
				@execute-action="executeAction"
				@row-action="rowAction"
				@cell-action="cellAction"
				@set-dropdown="setDropdown" />
		</div>

		<div>
			<q-table
				:name="tableTestPaginationAlt.config.name"
				:rows="tableTestPaginationAlt.rows"
				:columns="tableTestPaginationAlt.columns"
				:config="tableTestPaginationAlt.config"
				:total-rows="tableTestPaginationAlt.totalRows"
				@execute-action="executeAction"
				@row-action="rowAction"
				@cell-action="cellAction"
				@set-dropdown="setDropdown" />
		</div>

		<div>
			<q-table
				:name="tableTest.config.name"
				:rows="[]"
				:columns="tableTest.columns"
				:config="tableTest.config"
				:total-rows="0"
				@execute-action="executeAction"
				@row-action="rowAction"
				@cell-action="cellAction"
				@set-dropdown="setDropdown" />
		</div>

		<!--Sub-components-->
		<div>
			<h1>Sub-components</h1>
		</div>

		<div>
			<h2>Actions Menu (Dropdown)</h2>
		</div>

		<div>
			<h3>Normal mode, 0 Actions</h3>
			<q-table-record-actions-menu
				:row-key="actionsMenu0.rowKey"
				:crud-actions="actionsMenu0.crudActions"
				:custom-actions="actionsMenu0.customActions"
				:texts="tableTest.texts"
				:readonly="actionsMenu0.readonly" />
		</div>

		<div>
			<h3>Normal mode, 1 Actions</h3>
			<q-table-record-actions-menu
				:row-key="actionsMenu1.rowKey"
				:crud-actions="actionsMenu1.crudActions"
				:custom-actions="actionsMenu1.customActions"
				:texts="tableTest.texts"
				:readonly="actionsMenu1.readonly" />
		</div>

		<div>
			<h3>Normal mode, N Actions</h3>
			<q-table-record-actions-menu
				:row-key="actionsMenuN.rowKey"
				:crud-actions="actionsMenuN.crudActions"
				:custom-actions="actionsMenuN.customActions"
				:texts="tableTest.texts"
				:readonly="actionsMenuN.readonly" />
		</div>

		<div>
			<h3>Read-Only mode, 1 Actions (0 available in Read-Only)</h3>
			<q-table-record-actions-menu
				:row-key="actionsMenu1ReadOnly0.rowKey"
				:crud-actions="actionsMenu1ReadOnly0.crudActions"
				:custom-actions="actionsMenu1ReadOnly0.customActions"
				:texts="tableTest.texts"
				:readonly="actionsMenu1ReadOnly0.readonly" />
		</div>

		<div>
			<h3>Read-Only mode, 1 Actions (1 available in Read-Only)</h3>
			<q-table-record-actions-menu
				:row-key="actionsMenu1ReadOnly1.rowKey"
				:crud-actions="actionsMenu1ReadOnly1.crudActions"
				:custom-actions="actionsMenu1ReadOnly1.customActions"
				:texts="tableTest.texts"
				:readonly="actionsMenu1ReadOnly1.readonly" />
		</div>

		<div>
			<h3>Read-Only mode, N Actions (0 available in Read-Only)</h3>
			<q-table-record-actions-menu
				:row-key="actionsMenuNReadOnly0.rowKey"
				:crud-actions="actionsMenuNReadOnly0.crudActions"
				:custom-actions="actionsMenuNReadOnly0.customActions"
				:texts="tableTest.texts"
				:readonly="actionsMenuNReadOnly0.readonly" />
		</div>

		<div>
			<h3>Read-Only mode, N Actions (1 available in Read-Only)</h3>
			<q-table-record-actions-menu
				:row-key="actionsMenuNReadOnly1.rowKey"
				:crud-actions="actionsMenuNReadOnly1.crudActions"
				:custom-actions="actionsMenuNReadOnly1.customActions"
				:texts="tableTest.texts"
				:readonly="actionsMenuNReadOnly1.readonly" />
		</div>

		<div>
			<h3>Read-Only mode, N Actions (N available in Read-Only)</h3>
			<q-table-record-actions-menu
				:row-key="actionsMenuNReadOnlyN.rowKey"
				:crud-actions="actionsMenuNReadOnlyN.crudActions"
				:custom-actions="actionsMenuNReadOnlyN.customActions"
				:texts="tableTest.texts"
				:readonly="actionsMenuNReadOnlyN.readonly" />
		</div>

		<div>
			<h2>Actions Menu (Icons)</h2>
		</div>

		<div>
			<h3>Normal mode, N Actions</h3>
			<q-table-record-actions-menu
				:row-key="actionsMenuN.rowKey"
				:crud-actions="actionsMenuN.crudActions"
				:custom-actions="actionsMenuN.customActions"
				:texts="tableTest.texts"
				display="icons"
				:readonly="actionsMenuN.readonly" />
		</div>

		<div>
			<h2>Search</h2>
		</div>

		<div>
			<h3>Search</h3>
			<q-table-search
				v-if="searchbar01.globalSearch.visibility"
				:table-title="searchbar01.tableTitle"
				:searchable-columns="searchableColumns01"
				:placeholder="searchbar01.globalSearch.placeholder"
				:classes="searchbar01.globalSearch.classes"
				:case-sensitive="searchbar01.globalSearch.caseSensitive"
				:search-on-press-enter="searchbar01.globalSearch.searchOnPressEnter"
				:search-debounce-rate="searchbar01.globalSearch.searchDebounceRate"
				:show-refresh-button="searchbar01.globalSearch.showRefreshButton"
				:show-reset-button="searchbar01.globalSearch.showResetButton"
				:texts="tableTest.texts" />
		</div>

		<div>
			<h2>Import/Export</h2>
		</div>

		<div>
			<h3>Export</h3>
			<q-table-export
				:options="exportOptions01"
				:texts="tableTest.texts" />
		</div>

		<div>
			<h3>Import</h3>
			<q-table-import
				modal-id="data-import"
				:texts="tableTest.texts"
				:options="importOptions01"
				:template-options="importTemplateOptions01"
				:data-import-response="tableTest.config.dataImportResponse"
				:server-mode="tableTest.config.serverMode" />
		</div>

		<div>
			<h2>Static Filters</h2>
		</div>

		<div>
			<h3>Single Select Filters</h3>
			<q-table-static-filters
				:menu-name="tableTest.config.name"
				:group-filters="groupFiltersSingle01" />
		</div>

		<div>
			<h3>Multiple Select Filters</h3>
			<q-table-static-filters
				:menu-name="tableTest.config.name"
				:group-filters="groupFiltersMultiple01" />
		</div>

		<div>
			<h3>Active Filter</h3>
			<q-table-static-filters
				:menu-name="tableTest.config.name"
				:active-filters="activeFilters01" />
		</div>

		<div>
			<h2>Pagination</h2>
		</div>

		<div>
			<h3>Normal Pagination (No Pages)</h3>
			<q-table-pagination
				:page="1"
				:per-page="paginationNormal01.perPage"
				:total="0"
				:num-visibile-pagination-buttons="paginationNormal01.numVisibilePaginationButtons" />
		</div>

		<div>
			<h3>Normal Pagination (First Page)</h3>
			<q-table-pagination
				:page="1"
				:per-page="paginationNormal01.perPage"
				:total="paginationNormal01.rowCount"
				:num-visibile-pagination-buttons="paginationNormal01.numVisibilePaginationButtons" />
		</div>

		<div>
			<h3>Normal Pagination (Second Page)</h3>
			<q-table-pagination
				:page="2"
				:per-page="paginationNormal01.perPage"
				:total="paginationNormal01.rowCount"
				:num-visibile-pagination-buttons="paginationNormal01.numVisibilePaginationButtons" />
		</div>

		<div>
			<h3>Normal Pagination (Middle Page)</h3>
			<q-table-pagination
				:page="paginationNormal01.page"
				:per-page="paginationNormal01.perPage"
				:total="paginationNormal01.rowCount"
				:num-visibile-pagination-buttons="paginationNormal01.numVisibilePaginationButtons" />
		</div>

		<div>
			<h3>Normal Pagination (Second Last Page)</h3>
			<q-table-pagination
				:page="(paginationNormal01.rowCount / paginationNormal01.perPage) - 1"
				:per-page="paginationNormal01.perPage"
				:total="paginationNormal01.rowCount"
				:num-visibile-pagination-buttons="paginationNormal01.numVisibilePaginationButtons" />
		</div>

		<div>
			<h3>Normal Pagination (Last Page)</h3>
			<q-table-pagination
				:page="paginationNormal01.rowCount / paginationNormal01.perPage"
				:per-page="paginationNormal01.perPage"
				:total="paginationNormal01.rowCount"
				:num-visibile-pagination-buttons="paginationNormal01.numVisibilePaginationButtons" />
		</div>

		<div>
			<h3>Alternate Pagination (No Pages)</h3>
			<q-table-pagination-alt
				:page="1"
				:per-page="paginationAlt01.perPage"
				:total="0"
				:has-more-pages="false" />
		</div>

		<div>
			<h3>Alternate Pagination (First Page)</h3>
			<q-table-pagination-alt
				:page="1"
				:per-page="paginationAlt01.perPage"
				:total="paginationAlt01.rowCount"
				:has-more-pages="paginationAlt01.hasMore" />
		</div>

		<div>
			<h3>Alternate Pagination (Second Page)</h3>
			<q-table-pagination-alt
				:page="2"
				:per-page="paginationAlt01.perPage"
				:total="paginationAlt01.rowCount"
				:has-more-pages="paginationAlt01.hasMore" />
		</div>

		<div>
			<h3>Alternate Pagination (Middle Page)</h3>
			<q-table-pagination-alt
				:page="paginationAlt01.page"
				:per-page="paginationAlt01.perPage"
				:total="paginationAlt01.rowCount"
				:has-more-pages="paginationAlt01.hasMore" />
		</div>

		<div>
			<h3>Alternate Pagination (Last Page)</h3>
			<q-table-pagination-alt
				:page="paginationAlt01.rowCount / paginationAlt01.perPage"
				:per-page="paginationAlt01.perPage"
				:total="paginationAlt01.rowCount"
				:has-more-pages="false" />
		</div>

		<div>
			<h2>Limits</h2>
		</div>

		<div>
			<h3>Limits Info</h3>
			<q-table-limit-info
				:limits="tableLimits01"
				:table-name-plural="tableLimitsText01.tableNamePlural"
				:texts="tableTest.texts" />
		</div>

		<div>
			<h2>Checklist</h2>
		</div>

		<div>
			<h3>Row checkbox</h3>
			<q-table-checklist-checkbox
				:value="checklistCheckboxRow01.value"
				:table-name="checklistCheckboxRow01.tableName"
				:readonly="checklistCheckboxRow01.readonly"
				:row-key="checklistCheckboxRow01.rowKey" />
		</div>

		<div>
			<h3>Header checkbox</h3>
			<q-table-checklist-checkbox
				:value="checklistCheckboxRow01.value"
				:table-name="checklistCheckboxRow01.tableName"
				:readonly="checklistCheckboxRow01.readonly" />
		</div>

		<div>
			<h2>Column dropdown</h2>
		</div>

		<div>
			<h3>Column dropdown</h3>
			<q-table-column-filters
				allow-column-filters
				:column="columns01[1]"
				:searchable-columns="searchableColumns01"
				:filter="columnFilter01"
				:texts="tableTest.texts"
				@set-dropdown="setDropdown" />
		</div>

		<div>
			<h2>Active Filters</h2>
		</div>

		<div>
			<h3>Active Filters</h3>
			<q-table-active-filters
				has-filters-active
				:searchable-columns="searchableColumns01"
				:advanced-filters="filterArray01"
				:column-filters="filterHash01"
				:search-bar-filters="filterHash02"
				:texts="tableTest.texts" />
		</div>
	</div>
</template>

<script>
	import fakeData from './Table.mock.js'

	import QTableColumnConfig from '@/components/table/QTableColumnConfig.vue'
	import QTableAdvancedFilters from '@/components/table/QTableAdvancedFilters.vue'

	export default {
		name: 'QTableContainer',

		docsfile: './docs/table/QTable.md',

		components: {
			QTableColumnConfig,
			QTableAdvancedFilters
		},

		emits: [
			'show-popup',
			'hide-popup',
			'set-dropdown'
		],

		inheritAttrs: false,

		expose: [],

		data()
		{
			return fakeData
		},

		methods: fakeData.simpleUsageMethods
	}
</script>
