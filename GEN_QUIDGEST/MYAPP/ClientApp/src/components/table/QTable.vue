<template>
	<!-- TODO configurable header title position -->
	<div
		:id="controlId"
		:class="['q-table-list', ...classes, $attrs.class, isListVisible ? tableModeClasses : null]"
		:data-loading="!loaded">
		<div
			v-show="showHeader"
			class="page-header row no-gutters justify-content-between">
			<!-- BEGIN: Table controls -->
			<div
				v-if="tableTitle.length > 0 || hasActionBar || $slots.tableTitle"
				class="c-action-bar">
				<!-- BEGIN: Table title -->
				<div class="table-title c-table__title">
					<component
						v-if="hasLabel"
						:is="headerTag"
						:id="labelId">
						<slot
							name="tableTitle"
							:table-title="tableTitle">
							{{ tableTitle }}
						</slot>
					</component>
					<q-popover-help
						v-if="popoverText"
						:help-control="helpControl"
						:id="id"
						:label="tableTitle" />
				</div>
				<!-- END: Table title -->
				<!-- BEGIN: Extra menus -->
				<div
					v-if="hasActionBar"
					class="c-action-bar__menu">
					<!-- BEGIN: Saved views menu -->
					<q-select
						v-if="showSavedViews"
						v-model="selectedViewId"
						:items="savedViewsOptions"
						:groups="[
							{ id: 'user', title: '' },
							{ id: 'system', title: '' }
						]"
						size="small"
						item-value="id"
						item-label="text"
						@update:model-value="confirmAndSetSelectedViewById" />
					<!-- END: Saved views menu -->
					<!-- BEGIN: Configuration menu / button -->
					<q-dropdown-menu
						v-if="showConfigMenu"
						:id="configMenuId"
						icon="table-configuration"
						:texts="{ title: texts.tableConfig }"
						:options="configOptions"
						:button-options="{ borderless: true }"
						:button-classes="['dropdown-toggle']"
						@selected="emitConfigAction">
						<span
							v-if="confirmChanges"
							class="e-badge e-badge--highlight">
							<span aria-hidden="true"></span>
						</span>
					</q-dropdown-menu>
					<!-- END: Configuration menu / button -->
					<!-- BEGIN: Toggle show/hide filters -->
					<div
						v-if="hasFilters"
						class="flex-align-center">
						<div class="i-switch">
							<label class="action-input">
								<span class="i-switch__label hidden-h-elem"></span>
								<span class="i-switch__label-text">
									{{ texts.activeFiltersTitle }}
								</span>
							</label>
						</div>

						<q-toggle-input
							:model-value="filtersVisible"
							:title="showHideFiltersTitle"
							:true-label="texts.showText"
							:false-label="texts.hideText"
							display-type="label-toggle"
							@update:model-value="toggleShowFilters" />
					</div>
					<!-- END: Toggle show/hide filters -->
					<q-table-view-mode-config
						:model-value="$props.activeViewModeId"
						:view-modes="$props.viewModes"
						:texts="$props.texts"
						@update:model-value="(newVal) => $emit('update:active-view-mode', newVal)" />
					<!-- BEGIN: Row reorder toggle -->
					<q-button
						v-if="showRowDragAndDropOption && !readonly"
						b-style="secondary"
						borderless
						:title="texts.rowDragAndDropTitle"
						@click="toggleShowRowDragAndDrop">
						<q-icon icon="reorder" />
					</q-button>
					<!-- END: Row reorder toggle -->
					<q-button-group>
						<!-- BEGIN: Export menu -->
						<q-table-export
							v-if="allowFileExport"
							:options="exportOptions"
							:texts="texts"
							@export-data="exportData">
						</q-table-export>
						<!-- END: Export menu -->
						<!-- BEGIN: Import menu -->
						<q-table-import
							v-if="allowFileImport"
							modal-id="data-import"
							:options="importOptions"
							:template-options="importTemplateOptions"
							:data-import-response="dataImportResponse"
							:server-mode="serverMode"
							:texts="texts"
							@import-data="importData"
							@show-import-popup="$emit('show-popup', importModalProps)"
							@hide-import-popup="$emit('hide-popup', 'data-import')"
							@export-template="exportTemplate">
						</q-table-import>
						<!-- END: Import menu -->
					</q-button-group>
				</div>
				<!-- END: Extra menus -->
			</div>
			<!-- END: Table controls -->
			<template v-if="showSearchBar">
				<!-- BEGIN: Global search text -->
				<q-table-search
					v-if="globalSearch.visibility"
					:table-title="tableTitle"
					:searchable-columns="searchableColumns"
					:placeholder="`${texts.searchText} ${defaultSearchColumnLabel}`"
					:search-prop-value="query.globalSearch"
					:classes="globalSearch.classes"
					:case-sensitive="globalSearch.caseSensitive"
					:search-on-press-enter="globalSearch.searchOnPressEnter"
					:search-debounce-rate="globalSearch.searchDebounceRate"
					:show-refresh-button="globalSearch.showRefreshButton"
					:show-reset-button="globalSearch.showResetButton"
					:texts="texts"
					:disabled="!loaded"
					:signal="signalSearch"
					@clear-global-search="clearGlobalSearch"
					@search-by-column="searchByColumn"
					@search-by-all-columns="searchByAllColumns"
					@emit-search="searchByColumn(this.defaultSearchColumn, $event)"
					@reset-query="resetQuery">
					<template #extra-buttons>
						<!-- BEGIN: Advanced Filters -->
						<q-button
							v-if="config.allowAdvancedFilters"
							b-style="secondary"
							:title="texts.advancedFiltersText"
							:disabled="!loaded"
							@click="showAdvancedFiltersNew">
							<q-icon
								icon="advanced-filters"
								class="search-filters-icon" />
						</q-button>
						<!-- END: Advanced Filters -->
					</template>
				</q-table-search>
				<!-- END: Global search text -->
			</template>
			<q-tooltip-help
				v-if="tooltipText"
				:help-control="helpControl"
				:anchor="anchorId"
				:label="tableTitle" />

			<q-info-banner-help
				v-if="hasInfoBanner"
				:help-control="helpControl"
				:id="id" />
		</div>

		<q-subtitle-help
			v-if="subtitleText"
			:help-control="helpControl"
			:id="id" />

		<component
			:is="isListVisible && !(rowComponent === 'q-form-container' && formName !== '') ? 'div' : 'v-fragment'"
			class="table-and-filters-wrapper">
			<!-- BEGIN: Filters -->
			<div
				v-if="hasFilters"
				v-show="filtersVisible"
				class="c-table__filter-row">
				<q-table-static-filters
					v-if="hasStaticFilters"
					:texts="texts"
					:menu-name="config.name"
					:group-filters="groupFilters"
					:active-filters="activeFilters"
					@on-update-filter="updateFilters" />

				<!-- Condition "searchableColumns.length > 0" needed because of delay in loading columns -->
				<q-table-active-filters
					v-if="searchableColumns.length > 0"
					:searchable-columns="searchableColumns"
					:advanced-filters="query.advancedFilters"
					:column-filters="query.columnFilters"
					:search-bar-filters="query.searchBarFilters"
					:has-filters-active="hasCustomFilters"
					:texts="texts"
					:filter-operators="filterOperators"
					@set-property="(...args) => $emit('set-property', ...args)"
					@signal-component="(...args) => $emit('signal-component', ...args)"
					@show-advanced-filters="(...args) => $emit('show-advanced-filters', ...args)"
					@remove-column-filter="removeColumnFilter"
					@remove-search-bar-filter="removeSearchBarFilter"
					@remove-custom-filters="removeAllCustomFilters" />
			</div>
			<!-- END: Filters -->
			<div
				v-if="isListVisible"
				:class="['table-responsive-wrapper', tableWrapperClasses, { 'text-nowrap': !hasTextWrap }]"
				ref="tableWrapperElem">
				<div
					v-if="actionsPlacement === 'above' || generalActionsPlacement === 'above'"
					class="c-action-bar">
					<q-table-record-actions-menu
						:btn-permission="rowSelected?.btnPermission ?? { insertBtnDisabled: !canInsert }"
						:action-visibility="rowSelected?.actionVisibility ?? {}"
						:crud-actions="actionsPlacement === 'above' ? crudActions : null"
						:custom-actions="actionsPlacement === 'above' ? customActions : null"
						:general-actions="generalActionsPlacement === 'above' ? generalActions : null"
						:general-custom-actions="generalActionsPlacement === 'above' ? generalCustomActions : null"
						:actions-placement="actionsPlacement"
						:show-row-action-icon="showRowActionIcon"
						:show-general-action-icon="showGeneralActionIcon"
						:show-row-action-text="showRowActionText"
						:show-general-action-text="showGeneralActionText"
						:readonly="tableIsReadonly"
						display="inlineAll"
						:enable-row-actions="rowSelected !== undefined && rowSelected !== null"
						:texts="texts"
						@row-action="
							(emitAction) =>
								$emit('row-action', {
									...emitAction,
									row: rowSelected,
									rowKeyPath: rowSelected?.rowKeyPath ?? '',
									rowValue: rowSelected?.Value ?? ''
								})
						" />
				</div>
				<div v-if="rowComponent === 'q-form-container' && formName !== ''">
					<template v-if="permissions.canView !== undefined && permissions.canView !== null ? permissions.canView : true">
						<div
							v-for="(row, index) in vbtRows"
							:key="row.rowKey"
							class="multiform c-multiform__section">
							<component
								:is="rowComponent"
								:id="row.rowKey"
								:form-data="rowFormProps[index]"
								:row-component-props="{
									...rowComponentProps,
									permissions: permissions,
									actionsPlacement: 'actionsPlacement'
								}"
								:resources-path="config.resourcesPath"
								@edit="
									rowComponentProps.parentFormMode === 'EDIT' &&
										(permissions.canEdit !== undefined && permissions.canEdit !== null ? permissions.canEdit : true)
										? onMultiformSelect(row)
										: null
								"
								@deselect="$emit('set-array-sub-prop-where', 'rowFormProps', 'id', row.rowKey, 'mode', 'SHOW')">
							</component>
						</div>
						<div
							v-if="newRowID !== ''"
							class="multiform c-multiform__section">
							<component
								:is="rowComponent"
								:id="newRowID"
								:form-data="{ form: formName, id: newRowID, mode: 'NEW' }"
								:row-component-props="rowComponentPropsInsert"
								:resources-path="config.resourcesPath"
								@insert-form="(...args) => $emit('insert-form', ...args)"
								@cancel-insert="(...args) => $emit('cancel-insert', ...args)">
							</component>
						</div>
					</template>
				</div>
				<div
					v-else
					:class="['table-responsive', tableContainerClasses]"
					ref="tableContainerElem"
					tabindex="0"
					@keydown="tableOnKeyDown"
					@focusout="tableOnFocusout">
					<!-- FOR: COLUMN RESIZE, uses ref property -->
					<table
						:class="['c-table', tableClasses]"
						ref="tableElem">
						<!-- BEGIN: Header -->
						<thead class="c-table__head">
							<q-table-header
								ref="headerRowElem"
								:header-row="headerRow"
								:columns="topLevelColumns"
								row-index="h"
								:query="query"
								:table-name="name"
								:readonly="tableIsReadonly"
								:allow-column-filters="allowColumnFilters && !hasRowDragAndDrop"
								:allow-column-sort="allowColumnSort && !hasRowDragAndDrop"
								:searchable-columns="searchableColumns"
								:filters="query.columnFilters"
								:filter-operators="filterOperators"
								:table-container-elem="tableContainerElem"
								:row-count="rowCount"
								:texts="texts"
								:disabled="!loaded"
								@update-sort="updateSortQuery"
								@check-all-rows="checkAllRows"
								@check-current-page-rows="checkCurrentPageRows"
								@check-none-rows="checkNoneRows"
								@unselect-all-rows="$emit('unselect-all-rows')"
								@set-dropdown="(...args) => $emit('set-dropdown', ...args)"
								@set-property="(...args) => $emit('set-property', ...args)"
								@edit-column-filter="(...args) => editColumnFilter(...args)"
								@remove-column-filter="(...args) => removeColumnFilter(...args)"
								@add-advanced-filter="(...args) => $emit('add-advanced-filter', ...args)"
								@show-advanced-filters="
									(idx) => {
										$emit('signal-component', 'config', { show: true, selectedTab: 'advanced-filters' }, true)
										$emit('signal-component', 'advancedFilters', { selectedFilterIdx: idx }, true)
									}
								"
								@column-resize="onColumnResize()"
								@focusin="rowOnFocusin($event)"
								@focusout="rowOnFocusout($event)">
							</q-table-header>
						</thead>
						<!-- END: Header -->
						<tbody
							:class="['c-table__body', { 'c-table__body--loading': !loaded }]"
							ref="tbody"
							data-testid="table-body">
							<tr
								v-if="!loaded"
								class="c-table__row-loader">
								<td :colspan="headerColSpan">
									<div class="c-table__row-loader__wrapper">
										<q-spinner-loader :size="24" />

										<span class="c-table__row--loading-text">
											{{ texts.loading }}
										</span>
									</div>
								</td>
							</tr>
							<!-- BEGIN: data rows -->
							<template
								v-for="(row, index) in vbtRows"
								:key="row.rowKey + '_' + rowDomKey">
								<component
									:is="rowComponent"
									ref="rowElems"
									:table-name="name"
									:id="row.rowKey"
									:row="row"
									:columns="vbtColumns"
									:column-hierarchy="columnHierarchy"
									:row-key-path="[row.rowKey]"
									:row-index="index"
									:navigated-row-key-path="navigatedRowKeyPath"
									:prop-row-classes="getRowClasses(row)"
									:unique-id="uniqueId"
									:is-valid="rowIsValid(row)"
									:row-title="getRowTitle(row)"
									:crud-actions="crudActions"
									:custom-actions="customActions"
									:general-actions="generalActions"
									:general-custom-actions="generalCustomActions"
									:actions-placement="actionsPlacement"
									:general-actions-placement="generalActionsPlacement"
									:show-row-action-icon="showRowActionIcon"
									:show-general-action-icon="showGeneralActionIcon"
									:show-row-action-text="showRowActionText"
									:show-general-action-text="showGeneralActionText"
									:enable-action-button-base-classes="enableRowActionButtonBaseClasses"
									:readonly="tableIsReadonly"
									:text-color="rowTextColor"
									:bg-color="rowBgColor"
									:bg-color-selected="rowBgColorSelected"
									:row-selected-for-group="isRowSelected(row)"
									:cell-titles="getRowCellDataTitles(row, vbtColumns)"
									:has-row-drag-and-drop="hasRowDragAndDrop"
									:sort-order-column="sortOrderColumn"
									:expand-icon="expandIcon"
									:collapse-icon="collapseIcon"
									:row-action-display="rowActionDisplay"
									:disable-checkbox="blockTableCheck"
									:row-key-to-scroll="rowKeyToScroll"
									:texts="texts"
									:resources-path="config.resourcesPath"
									@row-click="(...args) => executeRowClickAction(...args)"
									@row-action="(emitAction) => $emit('row-action', emitAction)"
									@row-reorder="rowReorder"
									@execute-action="(...args) => $emit('execute-action', ...args)"
									@cell-action="(...args) => executeActionCell(...args)"
									@remove-row="removeRow(row.rowKey)"
									@toggle-row-selected="toggleRowSelectMultiple(row)"
									@update="(...args) => updateCell(...args)"
									@update-external="(...args) => $emit('update-external', ...args)"
									@toggle-show-children="setChildRowsVisibility"
									@go-to-row="(...args) => goToRow(...args)"
									@navigate-row="(...args) => navigateToRowByMultiIndex(...args)"
									@focusin="rowOnFocusin($event)"
									@focusout="rowOnFocusout($event)"
									@loaded="onRowLoaded"
									@sub-rows-loaded="onSubRowsLoaded" />
							</template>
							<!-- BEGIN: No results row -->
							<transition name="c-table-transition">
								<tr
									v-if="vbtRows.length === 0"
									:class="['c-table__row--empty', { 'c-table__row--loading': !loaded }]"
									data-testid="table-row">
									<td :colspan="headerColSpan">
										<slot name="empty-results">
											<template v-if="loaded">
												<img
													v-if="emptyRowImgPath"
													:src="emptyRowImgPath"
													:alt="texts.emptyText" />
												{{ texts.emptyText }}
											</template>
										</slot>
									</td>
								</tr>
							</transition>
							<!-- END: No results row -->
							<!-- END: data rows -->
						</tbody>
						<!-- BEGIN: Column totalers -->
						<tfoot v-if="hasColumnTotals">
							<tr
								id="columns-sum"
								class="columns-sum">
								<template
									v-for="(column, key) in columnTotals"
									:key="key">
									<td :class="getColumnTotalClass(column)">
										{{ column }}
									</td>
								</template>
							</tr>
						</tfoot>
						<tfoot v-else-if="hasNewRecordRow">
							<q-table-row
								:table-name="name"
								:key="newRow.Rownum"
								:row="newRow"
								:columns="vbtColumns"
								row-index="new"
								unique-id="new_row"
								:text-color="rowTextColor"
								:bg-color="rowBgColor"
								:bg-color-selected="rowBgColorSelected"
								:cell-titles="getRowCellDataTitles(newRow, vbtColumns)"
								@update="(...args) => setCellValue(...args)">
								<template #checklist>
									<q-button
										b-style="secondary"
										:label="texts.removeText"
										:title="texts.removeText"
										@click="emitRowsDelete(rowsSelected)">
										<q-icon icon="remove" />
									</q-button>
								</template>
								<template #actions>
									<q-button
										b-style="secondary"
										:label="texts.insertText"
										:title="texts.insertText"
										@click="emitRowAdd(newRow)">
										<q-icon icon="add" />
									</q-button>
								</template>
							</q-table-row>
						</tfoot>
						<!-- END: Column totalers -->
					</table>
				</div>
				<!-- BEGIN: Footer -->
				<!-- Adding class hiddenFooter since using the v-if to hide the footer is causing an issue (scrolling down causes the page to go up and down) -->
				<div
					v-show="showFooter && !isFooterEmpty"
					:class="['c-table__footer-out', { 'c-table__footer-out--loading': !loaded }]"
					ref="tableFooterElem">
					<q-table-footer
						:pagination-placement="paginationPlacement"
						:pagination="showPagination && !hasRowDragAndDrop"
						:show-per-page-menu="perPageMenuVisible"
						:current-page-rows-length="currentPageRowsLength"
						:filtered-rows-length="filteredRowsLength"
						:original-rows-length="originalRowsLength"
						:page="page"
						v-model:perPage="perPage"
						:per-page-options="unselectedPerPageOptions"
						:has-more="hasMore"
						:show-rows-selected-count="showRowsSelectedCount"
						:rows-selected-count="rowsSelectedCount"
						:has-row-select-actions="hasRowSelectActions"
						:group-actions="groupActions"
						:show-record-count="showRecordCount"
						:row-count="rowCount"
						:show-alternate-pagination="showAlternatePagination"
						:num-visibile-pagination-buttons="numVisibilePaginationButtons"
						:show-limits="showLimits"
						:table-limits="tableLimits"
						:table-id="controlId"
						:table-name-plural="tableNamePlural"
						:texts="texts"
						:disabled="!loaded"
						@update:page="(emitAction) => goToPage(emitAction)"
						@update:per-page="setPerPage($event)"
						@group-action="rowGroupAction($event)">
						<template #row-general-actions>
							<q-table-record-actions-menu
								v-if="generalActionsPlacement === 'below'"
								:btn-permission="{ insertBtnDisabled: !canInsert }"
								:general-actions="generalActions"
								:general-custom-actions="generalCustomActions"
								:actions-placement="actionsPlacement"
								:readonly="tableIsReadonly"
								:enable-general-actions="loaded"
								display="inline"
								:texts="texts"
								@row-action="(emitAction) => $emit('row-action', emitAction)" />
						</template>
					</q-table-footer>
				</div>
				<!-- END: Footer -->
			</div>
			<template v-else-if="activeViewMode">
				<component
					v-if="activeViewMode.props"
					:is="`q-${activeViewMode.type}`"
					:texts="texts"
					v-bind="activeViewMode.props"
					v-on="activeViewMode.handlers ?? {}">
					<template #empty-image>
						<img
							v-if="emptyRowImgPath"
							:src="emptyRowImgPath"
							:alt="texts.emptyText" />
					</template>
					<template #empty-text>
						{{ texts.emptyText }}
					</template>
				</component>
				<!-- BEGIN: Footer -->
				<!-- Adding class hiddenFooter since using the v-if to hide the footer is causing an issue (scrolling down causes the page to go up and down) -->
				<div
					v-show="showFooter && !isFooterEmpty"
					:class="['c-sr__footer-out', 'c-table__footer-out', { 'c-table__footer-out--loading': !loaded }]">
					<q-table-footer
						:pagination="showPagination"
						:show-per-page-menu="false"
						:current-page-rows-length="currentPageRowsLength"
						:filtered-rows-length="filteredRowsLength"
						:original-rows-length="originalRowsLength"
						:page="page"
						v-model:perPage="perPage"
						:per-page-options="unselectedPerPageOptions"
						:has-more="hasMore"
						:show-rows-selected-count="showRowsSelectedCount"
						:rows-selected-count="rowsSelectedCount"
						:has-row-select-actions="hasRowSelectActions"
						:group-actions="groupActions"
						:show-record-count="showRecordCount"
						:row-count="rowCount"
						:show-alternate-pagination="showAlternatePagination"
						:num-visibile-pagination-buttons="numVisibilePaginationButtons"
						:show-limits="showLimits"
						:table-limits="tableLimits"
						:table-id="controlId"
						:table-name-plural="tableNamePlural"
						:texts="texts"
						:disabled="!loaded"
						@update:page="(emitAction) => goToPage(emitAction)"
						@update:per-page="setPerPage($event)"
						@group-action="rowGroupAction($event)">
						<template #row-general-actions>
							<q-table-record-actions-menu
								v-if="generalActionsPlacement === 'below' && !activeViewMode.implementsOwnInsert"
								:btn-permission="{ insertBtnDisabled: !canInsert }"
								:general-actions="generalActions"
								:general-custom-actions="generalCustomActions"
								:actions-placement="actionsPlacement"
								:readonly="tableIsReadonly"
								:enable-general-actions="loaded"
								display="inline"
								:texts="texts"
								@row-action="$emit('row-action', $event)" />
						</template>
					</q-table-footer>
				</div>
				<!-- END: Footer -->
			</template>
		</component>
	</div>
</template>

<script>
	import { markRaw, defineAsyncComponent } from 'vue'
	import cloneDeep from 'lodash-es/cloneDeep'
	import filter from 'lodash-es/filter'
	import find from 'lodash-es/find'
	import findIndex from 'lodash-es/findIndex'
	import forEach from 'lodash-es/forEach'
	import get from 'lodash-es/get'
	import has from 'lodash-es/has'
	import includes from 'lodash-es/includes'
	import isEmpty from 'lodash-es/isEmpty'
	import orderBy from 'lodash-es/orderBy'

	import Sortable from 'sortablejs'

	import { tableViewManagementModes } from '@/mixins/quidgest.mainEnums.js'
	import genericFunctions from '@/mixins/genericFunctions.js'
	import listFunctions from '@/mixins/listFunctions.js'
	import HelpControl from '@/mixins/helpControls.js'

	import ColumnResizeable from '@/api/genio/columnResizeable.js'
	import searchFilterData from '@/api/genio/searchFilterData.js'

	// The texts needed by the component.
	const DEFAULT_TEXTS = {
		actionMenuTitle: 'Actions',
		emptyText: 'No data to show',
		removeText: 'remove',
		importButtonTitle: 'Import',
		templateButtonTitle: 'Select a template to use.',
		submitText: 'Submit',
		applyText: 'Apply',
		closeText: 'Close',
		dropToUpload: 'Drop files here to upload',
		okText: 'OK',
		pendingRecords: 'Warning: This record is pending, you must edit or delete it.',
		saveText: 'Save',
		viewText: 'View',
		deleteText: 'Delete',
		duplicateText: 'Duplicate',
		confirmText: 'Confirm',
		cancelText: 'Cancel',
		discard: 'Discard',
		resetText: 'Reset',
		insertText: 'Insert',
		tableConfig: 'Table configuration',
		configureColumns: 'Configure columns',
		viewModeConfigButtonTitle: 'View options',
		toListViewButtonTitle: 'List view',
		toAlternativeViewButtonTitle: 'Alternative view',
		orderText: 'Order',
		nameOfColumnText: 'Column name',
		visibleText: 'Visible',
		searchTextTitle: 'Search box',
		searchText: 'Search',
		forText: 'for',
		ofText: 'of',
		orText: 'Or',
		allFieldsText: 'all fields',
		showText: 'Show',
		hideText: 'Hide',
		filtersText: 'Filters',
		limitsButtonTitle: 'Limit',
		limitsListTitlePrepend: 'The information in the list of',
		limitsListTitleAppend: 'is limited by',
		textRowsSelected: 'selected record(s)',
		hasTextWrapText: 'Line break',
		groupActionsText: 'Group actions',
		advancedFiltersText: 'Advanced filters',
		moveToAdvancedFiltersText: 'Move to advanced filters',
		applyFilterText: 'Apply filter',
		createFilterText: 'Create filter',
		filterNameText: 'Filter name',
		createConditionText: 'Create condition',
		removeConditionText: 'Remove condition',
		savedFiltersText: 'Saved filters',
		saveFilterText: 'Save filter',
		deleteFilterText: 'Delete filter',
		activateFilterText: 'Activate filter',
		deactivateFilterText: 'Deactivate filter',
		columnActionsText: 'Column actions',
		sortText: 'Sort',
		ascendingText: 'Ascending',
		descendingText: 'Descending',
		sortAscendingText: 'Sort ascending',
		sortDescendingText: 'Sort descending',
		staticFiltersTitle: 'Global filters',
		activeFiltersTitle: 'Active filters',
		removeAllText: 'Remove all',
		rowDragAndDropTitle: 'Reorder',
		exportButtonTitle: 'Export',
		defaultKeywordSearchText: 'Default search',
		lineBreak: 'Line break',
		yesLabel: 'Yes',
		noLabel: 'No',
		activeText: 'Active',
		inactiveText: 'Inactive',
		showRecordsWhereText: 'Show records when',
		visibleColumnsText: 'Visible columns',
		saveViewText: 'Save view',
		viewManagerText: 'View manager',
		clearResizeText: 'Clear resize',
		viewExistsText: 'This view already exists.',
		wantToOverwriteText: 'Do you want to overwrite it?',
		tableViewSaveSuccess: 'Table view saved successfully',
		viewNameText: 'View name',
		setDefaultViewText: 'Set as default view',
		defaultViewText: 'Default view',
		downloadTemplateText: 'Download the excel template file by clicking the button below',
		fillTemplateFileText: 'Fill the file with the necessary information',
		importTemplateFileText: 'After filling the file click on the submit button to import it',
		allRecordsText: 'All',
		currentPageText: 'Current page',
		noneText: 'None',
		loading: 'Loading data...',
		onDate: 'On:',
		state: 'State',
		first: 'First',
		last: 'Last',
		previous: 'Previous',
		next: 'Next'
	}

	export default {
		name: 'QTable',

		emits: [
			'add-advanced-filter',
			'cancel-insert',
			'cell-action',
			'close-view',
			'deactivate-all-advanced-filters',
			'execute-action',
			'fetch-qtable-all-selected',
			'hide-popup',
			'insert-form',
			'loaded',
			'on-change-query',
			'on-export-data',
			'on-export-template',
			'on-import-data',
			'remove-all-advanced-filters',
			'remove-row',
			'reset-query',
			'row-action',
			'row-add',
			'row-edit',
			'row-group-action',
			'row-reorder',
			'rows-delete',
			'rows-loaded',
			'save-view',
			'go-to-row',
			'select-row',
			'select-rows',
			'set-array-sub-prop-where',
			'set-dropdown',
			'set-info-message',
			'set-property',
			'set-qtable-all-selected',
			'set-row-index-property',
			'set-search-on-next-change',
			'show-advanced-filters',
			'show-popup',
			'signal-component',
			'toggle-rows-drag-drop',
			'tree-load-branch-data',
			'unselect-all-rows',
			'unselect-row',
			'update-cell',
			'update-config',
			'update-external',
			'update-list-visible',
			'update:active-view-mode',
			'view-action'
		],

		components: {
			QDropdownMenu: defineAsyncComponent(() => import('@/components/QDropdownMenu.vue')),
			QTableStaticFilters: defineAsyncComponent(() => import('./QTableStaticFilters.vue')),
			QTableActiveFilters: defineAsyncComponent(() => import('./QTableActiveFilters.vue')),
			QTableHeader: defineAsyncComponent(() => import('./QTableHeader.vue')),
			QTableFooter: defineAsyncComponent(() => import('./QTableFooter.vue')),
			QTableSearch: defineAsyncComponent(() => import('./QTableSearch.vue')),
			QTableRecordActionsMenu: defineAsyncComponent(() => import('./QTableRecordActionsMenu.vue')),
			QTableChecklistCheckbox: defineAsyncComponent(() => import('./QTableChecklistCheckbox.vue')),
			QTableExport: defineAsyncComponent(() => import('./QTableExport.vue')),
			QTableImport: defineAsyncComponent(() => import('./QTableImport.vue')),
			QTableRow: defineAsyncComponent(() => import('./QTableRow.vue')),
			QTreeTableRow: defineAsyncComponent(() => import('./QTreeTableRow.vue')),
			QPopoverHelp: defineAsyncComponent(() => import('@/components/QPopoverHelp.vue')),
			QTooltipHelp: defineAsyncComponent(() => import('@/components/QTooltipHelp.vue')),
			QSubtitleHelp: defineAsyncComponent(() => import('@/components/QSubtitleHelp.vue')),
			QInfoBannerHelp: defineAsyncComponent(() => import('@/components/QInfoBannerHelp.vue'))
		},

		inheritAttrs: false,

		mixins: [HelpControl],

		props: {
			/**
			 * The unique identifier for the table component instance.
			 */
			id: String,

			/**
			 * Flag indicating if the label is to be displayed.
			 */
			hasLabel: {
				type: Boolean,
				default: true
			},

			/**
			 * The array of data objects that represents each row of data in the table.
			 */
			rows: {
				type: Array,
				default: () => []
			},

			/**
			 * The configuration for the table columns, including header names, keys, and additional properties.
			 */
			columns: {
				type: Array,
				required: true
			},

			/**
			 * The component used to render individual table rows.
			 */
			rowComponent: {
				type: String,
				default: 'q-table-row'
			},

			/**
			 * The type of the table, which can affect rendering or features (e.g., 'List', 'TreeList').
			 */
			type: {
				type: String,
				default: 'List'
			},

			/**
			 * The total number of rows available, which may be utilized for server-side pagination.
			 */
			totalRows: {
				type: Number,
				default: 0
			},

			/**
			 * Flag indicating whether there are more pages of data available for server-side pagination.
			 */
			hasMorePages: {
				type: Boolean,
				default: false
			},

			/**
			 * Custom configuration object to setup various aspects of the table component's behavior and appearance.
			 */
			config: {
				type: Object,
				default: () => ({})
			},

			/**
			 * Localization and customization of textual content within the table component.
			 */
			texts: {
				type: Object,
				validator: (value) => genericFunctions.validateTexts(DEFAULT_TEXTS, value),
				default: () => DEFAULT_TEXTS
			},

			/**
			 * Indicates whether the table is in a read-only or interactive state.
			 */
			readonly: {
				type: Boolean,
				default: false
			},

			/**
			 * Custom classes added to the table's surrounding container element.
			 */
			classes: {
				type: [Array, String],
				default: () => []
			},

			/**
			 * Custom classes added to the table's surrounding container element, only when in normal table mode.
			 */
			tableModeClasses: {
				type: [Array, String],
				default: () => []
			},

			/**
			 * Custom classes added directly to the <table> element.
			 */
			tableClasses: {
				type: [Object, String],
				default: () => ({})
			},

			/**
			 * Custom classes added to the container that usually includes the table and possibly other elements like pagination.
			 */
			tableContainerClasses: {
				type: [Object, String],
				default: () => ({})
			},

			/**
			 * Custom classes added to the wrapper element surrounding the table.
			 */
			tableWrapperClasses: {
				type: [Object, String],
				default: () => ({})
			},

			/**
			 * An array of actions that can be performed on the entire table, such as adding or exporting rows.
			 */
			actions: {
				type: Array,
				default: () => []
			},

			/**
			 * Tracks the current state to determine if the next data change should trigger a re-fetch.
			 */
			searchOnNextChange: {
				type: Object,
				default: () => ({})
			},

			/**
			 * Definition of advanced filters that may apply more complex conditions to the table data.
			 */
			advancedFilters: {
				type: Array,
				default: () => []
			},

			/**
			 * Definition of filters applied to individual table columns, often representing a subset of the total filters being used.
			 */
			columnFilters: {
				type: Object,
				default: () => ({})
			},

			/**
			 * Definition of global filters that apply generalized filtering criteria outside individual columns.
			 */
			groupFilters: {
				type: Array,
				default: () => []
			},

			/**
			 * Object representing filters that are currently active and affecting the table data.
			 */
			activeFilters: {
				type: Object,
				default: () => ({})
			},

			/**
			 * An array representing the different limits that may apply to the table data or query results.
			 */
			tableLimits: {
				type: Array,
				default: () => []
			},

			/**
			 * The response payload from an import action, which could include status messages or data validation errors.
			 */
			dataImportResponse: {
				type: Object,
				default: () => ({})
			},

			/**
			 * The header level (e.g., h1, h2) used for the table's accessible title region.
			 */
			headerLevel: {
				type: Number,
				default: 1
			},

			/**
			 * A map of selected rows by their keys, often used for batch actions or manipulation of multiple rows.
			 */
			rowsSelected: {
				type: Object,
				default: () => ({})
			},

			/**
			 * The name of the icon to be used for expand functionality within collapsible rows or sections.
			 */
			expandIcon: {
				type: String,
				default: 'square-plus'
			},

			/**
			 * The name of the icon to be used for collapse functionality within collapsible rows or sections.
			 */
			collapseIcon: {
				type: String,
				default: 'square-minus'
			},

			/**
			 * Array of different view modes available for the table, such as a list view or card view.
			 */
			viewModes: {
				type: Array,
				default: () => []
			},

			/**
			 * The identifier for the table's current view mode out of the possible options defined in viewModes.
			 */
			activeViewModeId: {
				type: String,
				default: ''
			},

			/**
			 * The name of the form to be potentially used or referenced within row-level actions or edit states.
			 */
			formName: {
				type: String,
				default: ''
			},

			/**
			 * Object properties to be associated with each individual row component, such as form states or additional configuration.
			 */
			rowFormProps: {
				type: Array,
				default: () => []
			},

			/**
			 * Additional properties to be passed down to row components, which can include permissions or event handlers.
			 */
			rowComponentProps: {
				type: Object,
				default: () => ({})
			},

			/**
			 * The identifier for a row that may be added to the table data as a new entry.
			 */
			newRowID: {
				type: String,
				default: ''
			},

			/**
			 * Indicates if any pending changes need to be confirmed by the user before proceeding.
			 */
			confirmChanges: {
				type: Boolean,
				default: false
			},

			/**
			 * An object for passing signals to the component, often used for higher-level state management or control.
			 */
			signal: {
				type: Object,
				default: () => ({})
			},

			/**
			 * A predefined set of operators used for creating or managing filters within the component.
			 */
			filterOperators: {
				type: Object,
				default: () => searchFilterData.operators.elements
			},

			/**
			 * A flag to indicate the 'select all' state is active for the table.
			 */
			allSelectedRows: {
				type: String,
				default: 'false'
			},

			/**
			 * Object with properties for the header row.
			 */
			headerRow: {
				type: Object,
				default: () => ({
					isNavigated: false
				})
			},

			/**
			 * Indicates if the component has finished loading necessary data and can be interacted with or not.
			 */
			loaded: {
				type: Boolean,
				default: true
			}
		},

		expose: [],

		data()
		{
			return {
				controlId: this.id || this.config.name || `q-table-${this._.uid}`,
				vbtRows: [],
				vbtColumns: [],
				columnHierarchy: [],
				name: '',
				query: {
					sort: [],
					advancedFilters: [],
					columnFilters: {},
					searchBarFilters: {},
					groupFilters: [],
					activeFilters: {},
					globalSearch: ''
				},
				page: 1,
				perPage: 10,
				defaultPerPage: 10,
				showRecordCount: false,
				numVisibilePaginationButtons: 3,
				tempFilteredResults: [],
				pagination: true,
				multiColumnSort: false,
				cardTitle: '',
				tableTitle: '',
				tableNamePlural: '',
				globalSearch: {
					classes: '',
					visibility: false,
					caseSensitive: false,
					showRefreshButton: true,
					showResetButton: false,
					showClearButton: false,
					searchOnPressEnter: true,
					searchDebounceRate: 60,
					init: {
						value: ''
					}
				},
				allowColumnFilters: false,
				allowColumnResize: true,
				perPageOptions: [],
				serverMode: false,
				numRows: 0,
				cardMode: false,
				isFirstTime: true,
				isResponsive: false,
				preservePageOnDataChange: false,
				canEmitQueries: false,
				changeQueryOnLoad: false,
				canInsert: true,
				crudActions: [],
				customActions: [],
				addAction: {},
				rowClickAction: {},
				actionsPlacement: 'left',
				generalActionsPlacement: 'below',
				paginationPlacement: 'left',
				lcid: 'pt-PT',
				system: 0,
				pkColumn: '',
				menuForJump: '',
				groupActions: [],
				showColumnTotals: false,
				showColumnTotalsSelected: false,
				showRowsSelectedCount: false,
				showAlternatePagination: false,
				numberFormat: {
					decimalSeparator: ',',
					groupSeparator: '.'
				},
				dateFormats: {
					date: 'dd/MM/yyyy',
					dateTime: 'dd/MM/yyyy HH:mm',
					dateTimeSeconds: 'dd/MM/yyyy HH:mm:ss',
					hours: 'HH:mm',
					use12Hour: false
				},
				rowTextColor: '',
				rowBgColor: '',
				filtersVisible: false,
				staticFiltersVisible: true,
				allowFileExport: false,
				exportOptions: [],
				allowFileImport: false,
				importOptions: [],
				importTemplateOptions: [],
				showLimitsInfo: false,
				viewManagementMode: tableViewManagementModes.disabled,
				showImportResponsePopup: false,
				showFooter: true,
				resizableGrid: null,
				hasTextWrap: false,
				hasRowDragAndDrop: false,
				showRowDragAndDropOption: false,
				fieldsEditable: false,
				hasNewRecordRow: false,
				newRow: {
					Rownum: -1,
					Fields: {}
				},
				rowKeyToScroll: '',
				userTableConfigName: '',
				userTableConfigNames: [],
				selectedViewId: 0,
				initialSortColumnName: '',
				initialSortColumnOrder: '',
				defaultSearchColumnName: '',
				columnResizeOptions: {},
				tableContainerElem: {},
				importModalProps: {
					id: 'data-import',
					props: {
						modalWidth: 'md',
						hideHeader: true,
						closeButtonEnable: true
					}
				},
				signalSearch: {},

				// SortableJS plugin instance
				sortablePlugin: null,

				blockTableCheck: false,
				vbtAllSelected: this.allSelectedRows === 'true',
				/**
				 * Row key path of navigated row
				 */
				navigatedRowKeyPath: null,
				/**
				 * Index of navigated row in the array of all navigable row elements
				 */
				navRowIndex: null,
				/**
				 * Whether to navigate to the element that should be navigated to when the component is updated
				 */
				setNavOnUpdate: false,
				/**
				 * Array of all navigable row elements
				 */
				navRowElems: [],
				/**
				 * Index of the first data row in the array of all navigable row elements.
				 * Can be 0 or 1 depending on whether the header row has navigable elements.
				 */
				firstDataRowIndex: 0,
				/**
				 * Number of rows that need to be rendered when the row data changes
				 */
				rowsToLoad: 0,
				/**
				 * Used to change the key for each row component to cause them to re-render when changing row data.
				 * All rows are re-rendered so that when the number of rendered rows matches the number of row objects
				 * loaded, an emit can be done to signal that the rendering is totally done.
				 * Used by things that depend on certain HTML elements existing.
				 */
				rowDomKey: 0,
				/**
				 * Whether the rows should be re-rendered when the row data changes.
				 */
				rerenderRowsOnNextChange: true
			}
		},

		provide()
		{
			return {
				getValueFromRow: this.getValueFromRow,
				getCellSlotName: this.getCellSlotName,
				canShowColumn: this.canShowColumn,
				isSortableColumn: this.isSortableColumn,
				isSearchableColumn: this.isSearchableColumn,
				isActionsColumn: this.isActionsColumn,
				isExtendedActionsColumn: this.isExtendedActionsColumn,
				isChecklistColumn: this.isChecklistColumn,
				isDragAndDropColumn: this.isDragAndDropColumn,
				getRowClasses: this.getRowClasses,
				getRowTitle: this.getRowTitle,
				rowIsValid: this.rowIsValid,
				hasDataAction: this.hasDataAction,
				hasExtendedAction: this.hasExtendedAction,
				getCellDataDisplay: this.getCellDataDisplay,
				getRowCellDataTitles: this.getRowCellDataTitles,
				isRowSelected: this.isRowSelected,
				rowWithoutChildren: this.rowWithoutChildren,
				columnFullName: this.columnFullName
			}
		},

		created()
		{
			this.initConfig()
		},

		mounted()
		{
			/*
				This can't be moved to 'data'
				because of the order of assignment of the other copies of rows.

				The 'tempFilteredResults' is not filled...
			*/
			this.vbtRows = this.rows

			// FOR: GETTING NAVIGABLE ROW ELEMENTS
			// Track number of rows to render
			this.setRowsToLoad()

			if (this.showRowDragAndDropOption) this.perPage = -1

			// Add columns to "new record" row
			if (this.hasNewRecordRow) this.initRow(this.newRow)

			//FOR: ROW ACTIONS, EXTENDED ACTIONS, COLUMN ORDER AND VISIBILITY
			//Must be called when loading or changing columns
			this.updateColumns()

			if (this.globalSearch.visibility) this.$nextTick().then(() => this.initGlobalSearch())

			this.query.advancedFilters = this.advancedFilters
			this.query.columnFilters = this.columnFilters

			this.$nextTick().then(() => {
				if (!this.serverMode)
				{
					this.filter(false, true)
				} else
				{
					this.canEmitQueries = true

					if (this.changeQueryOnLoad !== false) this.changeQuery()

					this.$emit('loaded')
				}
			})

			this.handleShiftKey()

			this.$nextTick().then(() => {
				if (this.$refs.tableElem && this.$refs.tableContainerElem)
				{
					//FOR: COLUMN RESIZE
					this.applyColumnResizeable()

					//FOR: COLUMN SIZES
					this.setColumnSizes(this.columnSizes)
				}
			})

			if (!this.readonly && this.rowsSelectableMultiple)
			{
				//Check if everything is supposed to be selected
				this.initHeaderSelector()
			}

			this.initRowsDragAndDrop()

			this.selectedViewId = this.getViewIdByName(this.userTableConfigName)

			// Set static filters in query property since this is what's sent when searching or filtering
			this.query.groupFilters = this.groupFilters
		},

		updated()
		{
			//DOM reference must be copied to property to it can be updated when the DOM element finally exists
			//and so the property can be passed as a prop to other componenents
			//so they will react to the change (when the DOM element finally exists)
			this.tableContainerElem = Array.isArray(this.$refs.tableContainerElem) ? this.$refs.tableContainerElem[0] : this.$refs.tableContainerElem

			// Update table navigation properties
			if(this.setNavOnUpdate)
			{
				this.$emit('set-property', 'config', 'setNavOnUpdate', false)

				this.$nextTick().then(()=> {
					this.navigateToTableRowAction('first')
				})
			}
		},

		beforeUnmount()
		{
			window.removeEventListener('keyup', this._handleShiftKey)
			window.removeEventListener('keydown', this._handleShiftKey)
			document.removeEventListener('selectstart', this._documentOnselectstart)

			this.destroyRowsDragAndDrop()

			if (this.resizableGrid)
			{
				this.resizableGrid.destroy()
				this.resizableGrid = null
			}
		},

		computed: {
			/**
			 * Determine if the table is in a read-only state.
			 */
			tableIsReadonly()
			{
				return this.readonly || !this.loaded
			},

			/**
			 * Determine if the table header should be displayed.
			 */
			showHeader()
			{
				return this.tableTitle.length > 0 || this.hasActionBar || this.showSearchBar || this.$slots.tableTitle
			},

			/**
			 * Get the total number of rows to be displayed.
			 */
			rowCount()
			{
				if (!this.serverMode) return this.tempFilteredResults.length

				return this.totalRows
			},

			/**
			 * Determine if the columns have totals.
			 */
			hasColumnTotals()
			{
				if (!this.showColumnTotals && !this.columnTotalRowsSelected) return false

				return Object.values(this.columnTotals).find((e) => e !== '')
			},

			/**
			 * Calculate the column totals for display.
			 */
			columnTotals()
			{
				const totals = {}

				for (let column of this.vbtColumns)
				{
					if (this.canShowColumn(column))
					{
						let totalVal = ''

						if (this.showColumnTotals) totalVal = this.getColumnTotalValueDisplay(column)
						else if (this.columnTotalRowsSelected) totalVal = this.getColumnTotalValueDisplay(column, true)

						totals[column.name] = totalVal
					}
				}

				return totals
			},

			/**
			 * Get top-level columns based on the hierarchy or default to the full column list.
			 */
			topLevelColumns()
			{
				if (!Array.isArray(this.columnHierarchy)) return this.vbtColumns
				return this.columnHierarchy[0] ? this.columnHierarchy[0] : this.vbtColumns
			},

			/**
			 * Determine the icon used for toggling filters visibility.
			 */
			toggleFiltersIcon()
			{
				return this.filtersVisible ? 'collapse' : 'expand'
			},

			/**
			 * Determine if there are options for reordering columns.
			 */
			hasColumnReorder()
			{
				return this.config.hasCustomColumns
			},

			/**
			 * Calculate total number of pages based on the current per-page setting and the number of filtered results.
			 */
			totalPages()
			{
				return Math.ceil(this.rowCount / this.perPage)
			},

			/**
			 * Determine if there are more pages beyond the current one.
			 */
			hasMore()
			{
				if (!this.serverMode) return this.page < this.totalPages

				return this.hasMorePages
			},

			/**
			 * Get the total number of filtered results before pagination.
			 */
			filteredResultsCount()
			{
				return this.tempFilteredResults.length
			},

			/**
			 * Determine if there is a unique identifier column among the table columns.
			 */
			uniqueId()
			{
				let uniqueId = ''

				if (!this.hasUniqueId)
				{
					uniqueId = 'vbtId'
					return uniqueId
				}

				this.vbtColumns.some((column) => {
					if (has(column, 'uniqueId') && column.uniqueId === true)
					{
						uniqueId = column.name
						return true
					}
				})

				return uniqueId
			},

			/**
			 * Determine if the table columns include a unique identifier column.
			 */
			hasUniqueId()
			{
				return this.vbtColumns.some((column) => has(column, 'uniqueId') && column.uniqueId === true)
			},

			/**
			 * Get the unique identifier for the configuration menu.
			 */
			configMenuId()
			{
				return this.controlId + '-config-menu-btn'
			},

			/**
			 * Define the HTML level tag to be used for the table header.
			 */
			headerTag()
			{
				return 'h' + this.headerLevel
			},

			/**
			 * Get the length of the current page's row collection.
			 */
			currentPageRowsLength()
			{
				return this.vbtRows.length
			},

			/**
			 * Get the length of the filtered results set.
			 */
			filteredRowsLength()
			{
				return this.rowCount
			},

			/**
			 * Get the length of the original rows set without any filtering.
			 */
			originalRowsLength()
			{
				return this.serverMode ? this.rowCount : this.rows.length
			},

			/**
			 * Determine the total colspan value for the header based on visible columns.
			 */
			headerColSpan()
			{
				return this.vbtColumns.filter((column) => this.canShowColumn(column)).length
			},

			/**
			 * Determine if the search bar is visible.
			 */
			showSearchBar()
			{
				return this.globalSearch.visibility
			},

			/**
			 * Determine if the table has multiple pages.
			 */
			hasMultiplePages()
			{
				/*
				 * Normal paging just needs the total number of pages.
				 * Alternate paging needs the page number and hasMore to calculate if there are multiple pages.
				 */
				return this.totalPages > 1 || this.page > 1 || this.hasMore
			},

			/**
			 * Determine if pagination should be displayed.
			 */
			showPagination()
			{
				return this.pagination && this.hasMultiplePages
			},

			/**
			 * Determine if rows can be selected (multiple).
			 */
			rowsSelectableMultiple()
			{
				return this.rowClickActionInternal === 'selectMultiple'
			},

			/**
			 * Determine if rows can be selected (single).
			 */
			rowsSelectableSingle()
			{
				return this.rowClickActionInternal === 'selectSingle'
			},

			/**
			 * Determine if all rows are selected in the checklist column.
			 */
			isSelectedAllRows()
			{
				return Object.keys(this.rows).every((key) => this.rowsSelected[this.rows[key].rowKey])
			},

			/**
			 * Number of rows selected
			 */
			rowsSelectedCount()
			{
				return Object.keys(this.rowsSelected).length
			},

			/**
			 * Determine if the column totals row should be visible based on selected rows count.
			 */
			columnTotalRowsSelected()
			{
				return this.showColumnTotalsSelected && this.rowsSelectedCount > 0
			},

			/**
			 * Gets the list of columns that are searchable.
			 */
			searchableColumns()
			{
				return listFunctions.getSearchableColumns(this.columns)
			},

			/**
			 * Gets the list of columns that are sortable.
			 */
			sortableColumns()
			{
				return listFunctions.getSortableColumns(this.columns)
			},

			/**
			 * Determine if any filters are available on the table.
			 */
			hasFilters()
			{
				return this.hasStaticFilters || this.hasCustomFilters
			},

			/**
			 * Determine if there are any advanced, column-specific, or search bar filters available.
			 */
			hasCustomFilters()
			{
				return this.hasAdvancedFilters || this.hasColumnFilters || this.hasSearchBarFilters
			},

			/**
			 * Determine if any of the custom filters are currently active.
			 */
			hasCustomFiltersActive()
			{
				return this.hasAdvancedFiltersActive || this.hasColumnFilters || this.hasSearchBarFilters
			},

			/**
			 * Determine if any of the static filters are defined.
			 */
			hasStaticFilters()
			{
				return this.groupFilters.length > 0 || Object.keys(this.activeFilters).length > 0
			},

			/**
			 * Determine if any of the static filters are actively selected.
			 */
			hasStaticFiltersActive()
			{
				// Iterate group filters
				for (let key in this.groupFilters)
				{
					const curFilter = this.groupFilters[key]

					for (let idx in curFilter.filters)
					{
						const curSubFilter = curFilter.filters[idx]

						if (curSubFilter.selected !== false) return true
					}
				}

				// Iterate active filters
				for (let key in this.activeFilters.options)
				{
					const curFilter = this.activeFilters.options[key]

					if (curFilter.selected !== false) return true
				}

				return false
			},

			/**
			 * Generates a title based on whether the filters are currently displayed.
			 */
			showHideFiltersTitle()
			{
				return `${this.texts.showText}/${this.texts.hideText} ${this.texts.filtersText}`
			},

			/**
			 * Determine if any advanced filters are defined.
			 */
			hasAdvancedFilters()
			{
				return this.query.advancedFilters.length > 0
			},

			/**
			 * Determine if any advanced filters are currently active.
			 */
			hasAdvancedFiltersActive()
			{
				return this.advancedFiltersActive.length > 0
			},

			/**
			 * Provides a list of all currently active advanced filters.
			 */
			advancedFiltersActive()
			{
				const advancedFiltersActive = []

				for (let idx in this.query.advancedFilters)
				{
					const filter = this.query.advancedFilters[idx]

					if (filter.active !== false) advancedFiltersActive.push(filter)
				}

				return advancedFiltersActive
			},

			/**
			 * Determine if any column-specific filters have been defined.
			 */
			hasColumnFilters()
			{
				return Object.keys(this.query.columnFilters).length > 0
			},

			/**
			 * Determine if any filters applied through the search bar are active.
			 */
			hasSearchBarFilters()
			{
				return Object.keys(this.query.searchBarFilters).length > 0
			},

			/**
			 * Identify if rows are fully loaded and searchable.
			 */
			hasTableViewLoaded()
			{
				return (
					this.config.UserTableConfigName !== undefined &&
					this.config.UserTableConfigName !== null &&
					this.config.UserTableConfigName !== ''
				)
			},

			/**
			 * Determine if the table has a row click action.
			 */
			hasRowClickAction()
			{
				return !isEmpty(this.rowClickAction)
			},

			/**
			 * Determine if the table has row actions.
			 */
			hasRowActions()
			{
				return this.crudActions.length > 0 || this.customActions.length > 0 || this.hasNewRecordRow
			},

			/**
			 * Determine if the table has extended row actions.
			 */
			hasExtendedRowActions()
			{
				return typeof this.extendedActions !== 'undefined' && Array.isArray(this.extendedActions) && this.extendedActions.length > 0
			},

			/**
			 * Determine if the table has row select actions.
			 */
			hasRowSelectActions()
			{
				return this.rowsSelectableMultiple && (this.groupActions.length > 0 || this.menuForJump !== '')
			},

			/**
			 * Determine if the table limits information should be visible.
			 */
			hasLimits()
			{
				if (!this.tableLimits) return false
				return Object.keys(this.tableLimits).length > 0
			},

			/**
			 * Determine if the table limits information should be visible.
			 */
			showLimits()
			{
				if (!this.hasLimits) return false
				return this.showLimitsInfo
			},

			/**
			 * The currently configured view mode component or undefined if none.
			 */
			activeViewMode()
			{
				return this.viewModes.find((viewMode) => viewMode.id === this.activeViewModeId)
			},

			/**
			 * Determine if the user is allowed to customize the view modes.
			 */
			hasViewModeToggle()
			{
				return this.viewModes.length > 1
			},

			/**
			 * Determine if the table has action bar.
			 */
			hasActionBar()
			{
				return (
					this.showConfigMenu ||
					this.hasFilters ||
					this.showRowDragAndDropOption ||
					this.allowFileExport ||
					this.allowFileImport ||
					this.hasViewModeToggle
				)
			},

			/**
			 * Determine the column used for ordering the rows.
			 */
			sortOrderColumn()
			{
				for (let idx in this.columns)
				{
					const column = this.columns[idx]

					if (column.isOrderingColumn !== undefined && column.isOrderingColumn !== false) return column
				}
				return null
			},

			/**
			 * Get the default search column based on configuration.
			 */
			defaultSearchColumn()
			{
				return listFunctions.getDefaultSearchColumn(this.columns, this.defaultSearchColumnName)
			},

			/**
			 * Get the label of the default search column.
			 */
			defaultSearchColumnLabel()
			{
				if (isEmpty(this.defaultSearchColumn?.label)) return ''
				return this.defaultSearchColumn.label
			},

			/**
			 * Determine which column is used for initial sorting.
			 */
			initialSortColumn()
			{
				return find(this.vbtColumns, (col) => col.name === this.initialSortColumnName)
			},

			/**
			 * Determine if the list view is enabled.
			 */
			isListVisible()
			{
				// FIXME: remove by implementing a view mode manager
				// QTable SHOULD ONLY IMPLEMENT TABLE LOGIC!!! => DOES NOT CARE ABOUT VIEWMODES
				return !this.viewModes.length || this.activeViewModeId === 'LIST'
			},

			/**
			 * Get the row component properties with overrides for inserting rows.
			 */
			rowComponentPropsInsert()
			{
				const obj = cloneDeep(this.rowComponentProps)

				obj.formButtonsOverride.saveBtn.emitAction = {
					name: 'insert-form',
					params: []
				}
				obj.formButtonsOverride.resetCancelBtn.emitAction = {
					name: 'cancel-insert',
					params: []
				}

				return obj
			},

			/**
			 * Get the currently selected row.
			 */
			rowSelected()
			{
				let rowIdStr = Object.keys(this.rowsSelected)[0]
				if (rowIdStr === undefined || rowIdStr === null) return null

				let rowId = rowIdStr.split(',')

				let rowSelected = listFunctions.getRowByKeyPath(this.rows, rowId)

				if (rowSelected !== undefined && rowSelected !== null) rowSelected.rowKeyPath = rowId

				return rowSelected
			},

			/**
			 * Determine whether the table footer is empty or not.
			 */
			isFooterEmpty()
			{
				return (
					!this.showRowsSelectedCount &&
					!this.hasRowSelectActions &&
					!this.showRecordCount &&
					!this.showPagination &&
					!this.showLimits &&
					listFunctions.numArrayVisibleActions(this.generalActions, this.tableIsReadonly) === 0 &&
					listFunctions.numArrayVisibleActions(this.generalCustomActions, this.tableIsReadonly) === 0
				)
			},

			/**
			 * Get options array of saved views.
			 */
			savedViewsOptions()
			{
				if (!this.userTableConfigNames) return []

				let viewIdx = 1
				const savedViewsOptions = []

				for (const idx in this.userTableConfigNames)
				{
					const configName = this.userTableConfigNames[idx]
					savedViewsOptions.push({
						id: viewIdx,
						text: configName,
						key: viewIdx++,
						value: configName,
						group: 'user'
					})
				}

				savedViewsOptions.push({
					id: 0,
					text: this.texts.baseTable,
					separatorBefore: true,
					key: 0,
					value: this.texts.baseTable,
					group: 'system'
				})

				return savedViewsOptions
			},

			/**
			 * Determine whether to show the list of saved table views.
			 */
			showSavedViews()
			{
				return this.config.allowManageViews && this.savedViewsOptions.length > 1
			},

			/**
			 * Determine whether to show table view configuration dropdown menu.
			 */
			showConfigMenu()
			{
				return (
					this.config.allowManageViews || (this.config.allowColumnConfiguration && this.isListVisible) || (this.config.allowAdvancedFilters && this.hasAdvancedFilters)
				)
			},

			/**
			 * Get unselected options for the `perPage` dropdown.
			 */
			unselectedPerPageOptions()
			{
				const perPageOptions = this.perPageOptions.concat([this.defaultPerPage]).sort((a, b) => a - b)
				return perPageOptions.filter((item, pos) => item !== this.perPage && perPageOptions.indexOf(item) === pos)
			},

			/**
			 * Determine if the per page menu should be visible.
			 */
			perPageMenuVisible()
			{
				return listFunctions.getPerPageMenuVisible(
					this.perPageOptions,
					this.defaultPerPage,
					this.rowCount,
					this.page,
					this.showAlternatePagination,
					this.hasMorePages
				)
			},

			/**
			 * Get the path to the image to be used for an empty row.
			 */
			emptyRowImgPath()
			{
				if (!this.config.resourcesPath || !this.config.emptyRowImg) return ''

				let resourcesPath = this.config.resourcesPath
				if (resourcesPath[resourcesPath.length - 1] !== '/') resourcesPath += '/'

				return resourcesPath + this.config.emptyRowImg
			},

			labelId() {
				return `label_${this.controlId}`
			},

			hasInfoBanner() {
				if(!this.helpControl) return false
				return this.helpControl.shortHelp.type === 'Info-banner' || this.helpControl.detailedHelp?.type === 'Info-banner'
			},
		},

		methods: {
			getValueFromRow: listFunctions.getCellValue,
			getCellSlotName: listFunctions.getCellSlotName,
			isSortableColumn: listFunctions.isSortableColumn,
			isActionsColumn: listFunctions.isActionsColumn,
			isExtendedActionsColumn: listFunctions.isExtendedActionsColumn,
			isChecklistColumn: listFunctions.isChecklistColumn,
			isDragAndDropColumn: listFunctions.isDragAndDropColumn,
			hasExtendedAction: listFunctions.hasExtendedAction,
			hasDataAction: listFunctions.hasDataAction,
			rowWithoutChildren: listFunctions.rowWithoutChildren,

			/**
			 * Sets all data properties from props passed in
			 */
			initConfig()
			{
				if (isEmpty(this.config)) return

				this.pagination = has(this.config, 'pagination') ? this.config.pagination : true

				this.numVisibilePaginationButtons = has(this.config, 'numVisibilePaginationButtons') ? this.config.numVisibilePaginationButtons : 3

				this.perPageOptions = has(this.config, 'perPageOptions') ? this.config.perPageOptions : []

				this.defaultPerPage = has(this.config, 'perPageDefault') ? this.config.perPageDefault : 10

				this.perPage = has(this.config, 'perPage') ? this.config.perPage : 10

				this.page = has(this.config, 'page') ? this.config.page : 1

				this.changeQueryOnLoad = has(this.config, 'changeQueryOnLoad') ? this.config.changeQueryOnLoad : false

				this.showRecordCount = has(this.config, 'showRecordCount') ? this.config.showRecordCount : false

				this.multiColumnSort = has(this.config, 'multiColumnSort') ? this.config.multiColumnSort : false

				this.cardTitle = has(this.config, 'cardTitle') ? this.config.cardTitle : ''

				this.tableTitle = has(this.config, 'tableTitle') ? this.config.tableTitle : ''

				this.tableNamePlural = has(this.config, 'tableNamePlural') ? this.config.tableNamePlural : ''

				if (has(this.config, 'globalSearch'))
				{
					this.globalSearch.visibility = has(this.config.globalSearch, 'visibility') ? this.config.globalSearch.visibility : false
					this.globalSearch.caseSensitive = has(this.config.globalSearch, 'caseSensitive') ? this.config.globalSearch.caseSensitive : false
					this.globalSearch.showRefreshButton = has(this.config.globalSearch, 'showRefreshButton')
						? this.config.globalSearch.showRefreshButton
						: true
					this.globalSearch.showResetButton = has(this.config.globalSearch, 'showResetButton')
						? this.config.globalSearch.showResetButton
						: false
					this.globalSearch.showClearButton = has(this.config.globalSearch, 'showClearButton')
						? this.config.globalSearch.showClearButton
						: false
					this.globalSearch.searchOnPressEnter = has(this.config.globalSearch, 'searchOnPressEnter')
						? this.config.globalSearch.searchOnPressEnter
						: true
					this.globalSearch.searchDebounceRate = has(this.config.globalSearch, 'searchDebounceRate')
						? this.config.globalSearch.searchDebounceRate
						: 60
					this.globalSearch.classes = has(this.config.globalSearch, 'classes') ? this.config.globalSearch.classes : ''
					this.globalSearch.init.value = has(this.config.globalSearch, 'init.value') ? this.config.globalSearch.init.value : ''
				}

				this.query.globalSearch = has(this.config, 'query') ? this.config.query : ''

				this.query.searchBarFilters = has(this.config, 'searchBarFilters') ? this.config.searchBarFilters : {}

				this.filtersVisible = has(this.config, 'filtersVisible') ? this.config.filtersVisible : false

				this.allowColumnFilters = has(this.config, 'allowColumnFilters') ? this.config.allowColumnFilters : false

				this.allowColumnSort = has(this.config, 'allowColumnSort') ? this.config.allowColumnSort : false

				this.serverMode = has(this.config, 'serverMode') ? this.config.serverMode : false

				this.cardMode = has(this.config, 'cardMode') ? this.config.cardMode : false

				this.preservePageOnDataChange = has(this.config, 'preservePageOnDataChange') ? this.config.preservePageOnDataChange : false

				this.permissions = has(this.config, 'permissions') ? this.config.permissions : {}

				this.canInsert = get(this.config, 'canInsert', true)

				this.crudActions = has(this.config, 'crudActions') ? this.config.crudActions : []

				this.customActions = has(this.config, 'customActions') ? this.config.customActions : []

				this.generalActions = has(this.config, 'generalActions') ? this.config.generalActions : []

				this.generalCustomActions = has(this.config, 'generalCustomActions') ? this.config.generalCustomActions : []

				this.rowActionDisplay = has(this.config, 'rowActionDisplay') ? this.config.rowActionDisplay : 'dropdown'

				this.rowClickAction = has(this.config, 'rowClickAction') ? this.config.rowClickAction : {}

				this.rowClickActionInternal = has(this.config, 'rowClickActionInternal') ? this.config.rowClickActionInternal : ''

				this.menuForJump = has(this.config, 'menuForJump') ? this.config.menuForJump : ''

				this.groupActions = has(this.config, 'groupActions') ? this.config.groupActions : []

				this.actionsPlacement = has(this.config, 'actionsPlacement') ? this.config.actionsPlacement : 'left'

				this.generalActionsPlacement = has(this.config, 'generalActionsPlacement') ? this.config.generalActionsPlacement : 'below'

				this.rowActionClasses = has(this.config, 'rowActionClasses') ? this.config.rowActionClasses : {}

				this.enableRowActionButtonBaseClasses = has(this.config, 'enableRowActionButtonBaseClasses')
					? this.config.enableRowActionButtonBaseClasses
					: true

				this.showRowActionIcon = has(this.config, 'showRowActionIcon') ? this.config.showRowActionIcon : true

				this.showGeneralActionIcon = has(this.config, 'showGeneralActionIcon') ? this.config.showGeneralActionIcon : true

				this.showRowActionText = has(this.config, 'showRowActionText') ? this.config.showRowActionText : true

				this.showGeneralActionText = has(this.config, 'showGeneralActionText') ? this.config.showGeneralActionText : true

				this.paginationPlacement = has(this.config, 'paginationPlacement') ? this.config.paginationPlacement : 'left'

				this.extendedActions = has(this.config, 'extendedActions') ? this.config.extendedActions : []

				this.lcid = has(this.config, 'lcid') ? this.config.lcid : 'pt-PT'

				this.system = has(this.config, 'system') ? this.config.system : 0

				this.name = has(this.config, 'name') ? this.config.name : ''

				this.pkColumn = has(this.config, 'pkColumn') ? this.config.pkColumn : ''

				this.showColumnTotals = has(this.config, 'showColumnTotals') ? this.config.showColumnTotals : false

				this.showColumnTotalsSelected = has(this.config, 'showColumnTotalsSelected') ? this.config.showColumnTotalsSelected : false

				this.showRowsSelectedCount = has(this.config, 'showRowsSelectedCount') ? this.config.showRowsSelectedCount : false

				this.allowFileExport = has(this.config, 'allowFileExport') ? this.config.allowFileExport : false

				this.exportOptions = has(this.config, 'exportOptions') ? this.config.exportOptions : []

				this.allowFileImport = has(this.config, 'allowFileImport') ? this.config.allowFileImport : false

				this.importOptions = has(this.config, 'importOptions') ? this.config.importOptions : []

				this.importTemplateOptions = has(this.config, 'importTemplateOptions') ? this.config.importTemplateOptions : []

				this.showLimitsInfo = has(this.config, 'showLimitsInfo') ? this.config.showLimitsInfo : false

				this.configOptions = has(this.config, 'configOptions') ? this.config.configOptions : []

				this.viewManagementMode = has(this.config, 'viewManagement') ? this.config.viewManagement : 'N'

				this.showFooter = has(this.config, 'showFooter') ? this.config.showFooter : true

				this.showImportResponsePopup = has(this.config, 'showImportResponsePopup') ? this.config.showImportResponsePopup : ''

				this.showAlternatePagination = has(this.config, 'showAlternatePagination') ? this.config.showAlternatePagination : false

				this.hasRowDragAndDrop = has(this.config, 'hasRowDragAndDrop') ? this.config.hasRowDragAndDrop : false

				this.showRowDragAndDropOption = has(this.config, 'showRowDragAndDropOption') ? this.config.showRowDragAndDropOption : false

				if (has(this.config, 'numberFormat'))
				{
					this.numberFormat.decimalSeparator = has(this.config.numberFormat, 'decimalSeparator')
						? this.config.numberFormat.decimalSeparator
						: ','
					this.numberFormat.groupSeparator = has(this.config.numberFormat, 'groupSeparator') ? this.config.numberFormat.groupSeparator : '.'
				} else
				{
					this.numberFormat = {
						decimalSeparator: ',',
						groupSeparator: '.'
					}
				}

				if (has(this.config, 'dateFormats'))
				{
					this.dateFormats.date = has(this.config.dateFormats, 'date') ? this.config.dateFormats.date : 'dd/MM/yyyy'
					this.dateFormats.dateTime = has(this.config.dateFormats, 'dateTime') ? this.config.dateFormats.dateTime : 'dd/MM/yyyy HH:mm'
					this.dateFormats.dateTimeSeconds = has(this.config.dateFormats, 'dateTimeSeconds')
						? this.config.dateFormats.dateTimeSeconds
						: 'dd/MM/yyyy HH:mm:ss'
					this.dateFormats.hours = has(this.config.dateFormats, 'hours') ? this.config.dateFormats.hours : 'HH:mm'
					this.dateFormats.use12Hour = has(this.config.dateFormats, 'use12Hour') ? this.config.dateFormats.use12Hour : false
				} else
				{
					this.dateFormats = {
						date: 'dd/MM/yyyy',
						dateTime: 'dd/MM/yyyy HH:mm',
						dateTimeSeconds: 'dd/MM/yyyy HH:mm:ss',
						hours: 'HH:mm',
						use12Hour: false
					}
				}

				this.rerenderRowsOnNextChange = this.config?.rerenderRowsOnNextChange ?? true

				this.setNavOnUpdate = this.config?.setNavOnUpdate ?? false

				this.navigatedRowKeyPath = this.config?.navigatedRowKeyPath ?? null

				//FOR: TABLE LIST ROW COLOR
				this.rowTextColor = has(this.config, 'rowTextColor') ? this.config.rowTextColor : ''

				this.rowBgColor = has(this.config, 'rowBgColor') ? this.config.rowBgColor : ''

				this.rowBgColorSelected = has(this.config, 'rowBgColorSelected') ? this.config.rowBgColorSelected : '#E0E0E0'

				this.hasTextWrap = has(this.config, 'hasTextWrap') ? this.config.hasTextWrap : false

				this.fieldsEditable = has(this.config, 'fieldsEditable') ? this.config.fieldsEditable : false

				this.hasNewRecordRow = has(this.config, 'hasNewRecordRow') ? this.config.hasNewRecordRow : false

				this.columnResizeOptions = has(this.config, 'columnResizeOptions') ? this.config.columnResizeOptions : {}

				this.rowKeyToScroll = has(this.config, 'rowKeyToScroll') ? this.config.rowKeyToScroll : ''

				//FOR: USER_TABLE_CONFIG
				this.userTableConfigName = has(this.config, 'UserTableConfigName') ? this.config.UserTableConfigName : ''

				this.userTableConfigNames = has(this.config, 'UserTableConfigNames') ? this.config.UserTableConfigNames : []

				this.selectedViewId = this.getViewIdByName(this.userTableConfigName)

				this.defaultSearchColumnName = has(this.config, 'defaultSearchColumnName') ? this.config.defaultSearchColumnName : ''

				this.initialSortColumnName = has(this.config, 'initialSortColumnName') ? this.config.initialSortColumnName : ''

				this.initialSortColumnOrder = has(this.config, 'initialSortColumnOrder') ? this.config.initialSortColumnOrder : ''

				this.columnSizes = has(this.config, 'columnSizes') ? this.config.columnSizes : []

				this.allowColumnResize = has(this.config, 'allowColumnResize') ? this.config.allowColumnResize : true
			},

			/**
			 * Get navigation row index from the row's multi-index
			 * @param multiIndex {string} Row multi-index
			 */
			getNavRowIndexFromMultiIndex(multiIndex)
			{
				return this.navRowElems.findIndex((elem) => {
					return elem?.getAttribute('index')?.toString() === multiIndex.toString()
				})
			},

			/**
			 * Get table action sub-elements of a given element
			 * @param element {DOMElement} DOM element
			 */
			getTableRowActionElements(element)
			{
				const actionElements = []

				// Set main element as first if it has the attribute
				if(element?.hasAttribute('data-table-action-selected'))
					actionElements.push(element)

				// Get sub-elements
				const actionSubElementsNodeList = element?.querySelectorAll("[data-table-action-selected]")
				if(actionSubElementsNodeList !== undefined && actionSubElementsNodeList !== null)
					actionElements.push(...Array.from(actionSubElementsNodeList))

				return actionElements
			},

			/**
			 * Reset table action properties of sub-elements of a given element
			 * @param rowIndex {number} Row index
			 */
			resetTableRowActionProperties(rowIndex)
			{
				if(rowIndex === undefined || rowIndex === null)
					return

				const rowElems = this.navRowElems
				if(rowElems.length > 0)
				{
					// Clear the state for all action elements in this row
					let actionElements = this.getTableRowActionElements(rowElems[rowIndex])
					actionElements.forEach((actionElement) => {
						actionElement.setAttribute('data-table-action-selected', 'false')
					})
				}
			},

			/**
			 * Reset table action state
			 * @param rowIndex {number} Row index
			 */
			resetTableRowActionState(rowIndex)
			{
				// Reset action element properties
				this.resetTableRowActionProperties(rowIndex)

				// If focus is left on an element that is inconsistent with the navigation state
				// focus on the table container element
				if(document.activeElement?.getAttribute('data-table-action-selected') === 'false')
				{
					const tableContainerElem = this.getTableContainerElement()
					tableContainerElem?.focus()
				}
			},

			/**
			 * Focus on the next or previous action sub-element of a given element
			 * @param actionElements {DOMElement} DOM element
			 * @param direction {string} Direction to move focus ("first", "next" or "previous")
			 */
			focusTableRowActionElement(actionElements, direction = "first")
			{
				if(actionElements === undefined || actionElements === null || actionElements.length === 0)
					return

				let isFocused = false

				// Set value to add or subtract from index based on direction value
				const directionIdx = direction === "next" ? 1 : -1

				// Find focused element and focus on the next one if it exists
				actionElements.every((actionElement, index, array) => {
					// If element is focused
					if(actionElement.getAttribute('data-table-action-selected') === 'true')
					{
						// If focusing on the first element, set all others to false
						if(direction === 'first')
						{
							actionElement.setAttribute('data-table-action-selected', 'false')
							return true
						}

						isFocused = true

						// Prevent going out of bounds for adjacentActionElement
						if((directionIdx === 1 && index >= array.length - 1)
							|| (directionIdx === -1 && index === 0))
							return false

						const adjacentActionElement = actionElements[index + directionIdx]
						if(typeof adjacentActionElement.focus === 'function')
						{
							actionElement.setAttribute('data-table-action-selected', 'false')
							adjacentActionElement.setAttribute('data-table-action-selected', 'true')
							adjacentActionElement.focus()
							return false
						}
					}
					// If element is not focused go to next iteration
					return true
				})

				// None of the elements are focused. Focus on the first one.
				if(!isFocused && typeof actionElements[0].focus === 'function')
				{
					actionElements[0].setAttribute('data-table-action-selected', 'true')
					actionElements[0].focus()
				}
			},

			/**
			 * Navigate to next or previous row action
			 * @param direction {string} Direction to move focus ("first", "next" or "previous")
			 */
			navigateToTableRowAction(direction)
			{
				// Find action elements and focus on adjacent one
				const rowElems = this.navRowElems
				if(rowElems.length === 0)
					return
				let actionElements = this.getTableRowActionElements(rowElems[this.navRowIndex])
				this.focusTableRowActionElement(actionElements, direction)
			},

			/**
			 * Navigate to row
			 * @param index {number} Index of row to navigate to
			 */
			navigateToRow(index)
			{
				const rowElems = this.navRowElems

				// Check index bounds
				if(index < 0 || index >= rowElems?.length)
					return

				// Navigate to row
				if(this.navRowIndex === undefined || this.navRowIndex === null)
					this.navRowIndex = 0

				if(index !== this.navRowIndex)
				{
					// Reset state, including properties, of action elements of current row
					this.resetTableRowActionState(this.navRowIndex)
				}

				this.navRowIndex = index

				// Find action elements and focus on first one
				this.$nextTick().then(()=> {
					this.navigateToTableRowAction('first')
				})
			},

			/**
			 * Navigate to row by multi-index
			 * @param multiIndex {string} Row multi-index
			 */
			navigateToRowByMultiIndex(multiIndex)
			{
				this.navigateToRow(this.getNavRowIndexFromMultiIndex(multiIndex))
			},

			/**
			 * Reset table navigation state
			 */
			resetNavigationState()
			{
				// Reset state, including properties, of action elements of current row
				this.resetTableRowActionState(this.navRowIndex)
				// Reset row index
				this.navRowIndex = null
			},

			/**
			 * Focusout handler for table
			 * @param event {object} event object
			 */
			tableOnFocusout(event)
			{
				const tableContainerElem = this.getTableContainerElement()
				// If focus is on the table or sub-element of the table
				// Logically the focus is on the table
				if(event.relatedTarget === tableContainerElem
					|| tableContainerElem.contains(event.relatedTarget))
					return

				this.resetNavigationState()
			},

			/**
			 * Keydown handler for table
			 * FOR: TABLE KEYBOARD OPERATION
			 * @param event {object} Event object
			 */
			tableOnKeyDown(event)
			{
				const key = event?.key
				const tableContainerElem = this.getTableContainerElement()

				switch(key)
				{
					case "Tab":
					case "Escape":
						this.resetNavigationState()
						// If the focused element is the table
						if(event.target === tableContainerElem)
							return
						// If the focused element is a sub-element of the table
						tableContainerElem?.focus()
						event.preventDefault()
						break;
					case "ArrowUp":
					case "-":
						// Navigate to the previous row
						this.navigateToRow(this.navRowIndex - 1)
						event.preventDefault()
						break;
					case "ArrowDown":
					case "+":
						// Navigate to the first row if not navigated to any row, other wise navigate to the next row
						if(this.navRowIndex === undefined || this.navRowIndex === null || isNaN(this.navRowIndex))
							this.navigateToRow(this.firstDataRowIndex)
						else
							this.navigateToRow(this.navRowIndex + 1)
						event.preventDefault()
						break;
					case "ArrowLeft":
						// Navigate to the previous action element
						this.navigateToTableRowAction('previous')
						event.preventDefault()
						break;
					case "ArrowRight":
						// Navigate to the next action element
						this.navigateToTableRowAction('next')
						event.preventDefault()
						break;
					case "Home":
						// Navigate to first data row
						this.navigateToRow(this.firstDataRowIndex)
						event.preventDefault()
						break;
					case "End":
						// Navigate to last data row
						this.navigateToRow(this.navRowElems?.length - 1)
						event.preventDefault()
						break;
					case "Insert":
						// Insert new record
						this.$emit('row-action', { id: 'insert' })
						event.preventDefault()
						break;
					case "PageUp":
						// Go to previous page
						if(this.page > 1)
							this.goToPage(this.page - 1)
						event.preventDefault()
						break;
					case "PageDown":
						// Go to previous page
						if(this.hasMore)
							this.goToPage(this.page + 1)
						event.preventDefault()
						break;
				}
			},

			/**
			 * FOR: TABLE KEYBOARD OPERATION
			 * Focusin handler for table rows
			 * Used to set keyboard navigation state when focusing on a row or row sub-element
			 * so keyboard navigation can continue from there
			 * @param event {object} Event object
			 */
			rowOnFocusin(event)
			{
				// Actual element mousedown happens on
				let element = event?.target

				// Get action element that mousedown happened on or propagated to
				// since mousedown can originate on sub-elements
				let actionElement = element
				// If element that get's focused is a parent element of the element
				while(!actionElement?.hasAttribute('data-table-action-selected') && actionElement?.parentElement)
					actionElement = actionElement.parentElement

				// Get row element
				let rowElement = element
				while(rowElement?.tagName !== 'TR' && rowElement?.parentElement)
					rowElement = rowElement.parentElement

				// Get multi-index of row
				let multiIndex = rowElement.getAttribute('index')

				// Set row as the current navigated row
				this.navRowIndex = this.getNavRowIndexFromMultiIndex(multiIndex)

				// Reset properties of action elements of current row
				this.resetTableRowActionProperties(this.navRowIndex)

				// Set navigation state on this element
				if(actionElement?.hasAttribute('data-table-action-selected'))
					actionElement.setAttribute('data-table-action-selected', true)

				// Prevent re-rendering rows when changing the row navigated property
				this.rerenderRowsOnNextChange = false

				// Set the row property that signals if the row is navigated
				if(multiIndex === 'h')
					this.$emit('set-property', 'headerRow', 'isNavigated', true)
				else
					this.$emit('set-row-index-property', multiIndex, 'isNavigated', true)
			},

			/**
			 * FOR: TABLE KEYBOARD OPERATION
			 * Focusout handler for table rows
			 * Used to update keyboard navigation state when focusing away from a row or row sub-element
			 * so keyboard navigation can continue from there
			 * @param event {object} Event object
			 */
			rowOnFocusout(event)
			{
				// Actual element focusout happens on
				let element = event?.target

				// Get row element
				let rowElement = element
				while(rowElement?.tagName !== 'TR' && rowElement?.parentElement)
					rowElement = rowElement.parentElement

				// Element focus went to, if any
				let elementFocused = event?.relatedTarget

				// If element focus went to is in the row, navigation stays on the row
				if(rowElement.contains(elementFocused))
					return

				// Reset properties of action elements
				this.resetTableRowActionProperties(this.navRowIndex)

				//Get multi-index of row
				let multiIndex = rowElement.getAttribute('index')

				// Prevent re-rendering rows when changing the row navigated property
				this.rerenderRowsOnNextChange = false

				// Set the row property that signals if the row is navigated
				if(multiIndex === 'h')
					this.$emit('set-property', 'headerRow', 'isNavigated', false)
				else
					this.$emit('set-row-index-property', multiIndex, 'isNavigated', false)
			},

			/**
			 * Go to page
			 * @param pageNumber {number}
			 */
			goToPage(pageNumber)
			{
				this.page = pageNumber

				if (!this.serverMode) this.paginateFilter()
				else this.changeQuery(pageNumber)

				if (!this.readonly && this.rowsSelectableMultiple) this.initHeaderSelector()
			},

			/**
			 * Set records per page
			 * @param perPage {number}
			 */
			setPerPage(perPage)
			{
				this.perPage = perPage

				// Update value in table object
				this.$emit('set-property', 'config', 'perPageSelected', this.perPage)

				if (!this.serverMode)
				{
					let doPaginateFilter = this.page === 1

					if (!this.preservePageOnDataChange) this.page = 1

					if (doPaginateFilter) this.paginateFilter()
				} else
				{
					this.changeQuery()
					this.$emit('update-config')
				}
			},

			/**
			 * Sort when table is first loaded (UNUSED)
			 */
			initialSort()
			{
				// TODO optimze this with removing this filter
				let initial_sort_columns = filter(this.vbtColumns, (column) => has(column, 'initialSort') && column.initialSort === true)

				initial_sort_columns.some((initial_sort_column) => {
					const result = findIndex(this.query.sort, {
						vbtColId: initial_sort_column.vbtColId
					})

					if (result === -1)
					{
						//BEGIN: initial sort order validation
						let initialSortOrder = 'asc'

						if (has(initial_sort_column, 'initialSortOrder') && includes(['asc', 'desc'], initial_sort_column.initialSortOrder))
							initialSortOrder = initial_sort_column.initialSortOrder

						//END: initial sort order validation
						this.query.sort.push({
							vbtColId: initial_sort_column.vbtColId,
							name: initial_sort_column.name,
							order: initialSortOrder,
							caseSensitive: this.isSortCaseSensitive(initial_sort_column)
						})
					}

					// if multicolum sort sort is false, then consider only first initial sort column
					if (!this.multiColumnSort) return true
				})

				this.updateSort()
			},

			/**
			 * Initialize search
			 * @param {string} emitSearch
			 */
			initGlobalSearch(emitSearch = false)
			{
				this.query.globalSearch = this.globalSearch.init.value

				if (emitSearch) this.updateSearch()
			},

			/**
			 * Initialize search
			 */
			initHeaderSelector()
			{
				this.$emit('fetch-qtable-all-selected', this.id)

				//If less than three records, disable button
				if (this.vbtAllSelected)
				{
					this.checkCurrentPageRows(true)
					this.disableAllChecks()
				}
			},

			isSortCaseSensitive(column)
			{
				return column.sortCaseSensitive !== undefined ? column.sortCaseSensitive : true
			},

			/**
			 * Update sorting (built-in method)
			 * @param column {Object}
			 * @param direction {string}
			 * @param {string} emitSearch
			 */
			updateSortQuery(column, direction, emitSearch = true, emitUpdate = true)
			{
				let result = findIndex(this.query.sort, { vbtColId: column.vbtColId })

				if (result === -1)
				{
					if (!this.multiColumnSort) this.query.sort = []

					this.query.sort.push({
						vbtColId: column.vbtColId,
						name: column.name,
						order: 'asc',
						caseSensitive: this.isSortCaseSensitive(column)
					})

					result = this.query.sort.length - 1
				}

				if (direction !== undefined)
				{
					this.query.sort[result].order = direction
					this.updateInitialSortProperties()
					if (emitUpdate) this.$emit('update-config')
					if (emitSearch) this.updateSort()
				} else
				{
					this.query.sort = []
					this.updateInitialSortProperties()
					if (emitUpdate) this.$emit('update-config')
					this.updateSort()
					this.filter(!this.preservePageOnDataChange)
				}
			},

			/**
			 * Update initial sort display in column headers
			 */
			updateInitialSortDisplay()
			{
				// FOR: USER INITIAL SORT
				if (
					this.initialSortColumn !== undefined &&
					this.initialSortColumnOrder !== undefined &&
					this.initialSortColumn !== null &&
					this.initialSortColumnOrder !== null
				)
				{
					this.updateSortQuery(this.initialSortColumn, this.initialSortColumnOrder, false, false)
				} else this.query.sort = []
			},

			/**
			 * Update properties for initial sort to current sort
			 */
			updateInitialSortProperties()
			{
				let initialSortColumnName = ''
				let initialSortColumnOrder = ''

				if (this.query.sort.length > 0)
				{
					initialSortColumnName = this.query.sort[0].name
					initialSortColumnOrder = this.query.sort[0].order
				}

				this.$emit('set-property', 'config', 'initialSortColumnName', initialSortColumnName)
				this.$emit('set-property', 'config', 'initialSortColumnOrder', initialSortColumnOrder)
			},

			/**
			 * Inititalize row
			 * @param row {Object}
			 * @param rowRum {number}
			 */
			initRow(row, rowRum)
			{
				if (rowRum === undefined) row.Rownum = -1
				else row.Rownum = rowRum

				row.Fields = {}
			},

			/**
			 * Updates to do when columns are changed
			 * FOR: ROW ACTIONS, EXTENDED ACTIONS, COLUMN ORDER AND VISIBILITY
			 * Must be called when loading or changing columns
			 */
			updateColumns()
			{
				//Put references to columns in vbtColumns
				this.vbtColumns.splice(0)

				for (let idx in this.columns) this.vbtColumns[idx] = cloneDeep(this.columns[idx])

				//FOR: TABLE LIST ROW ACTIONS
				//BEGIN: Add row actions column
				if (this.hasRowActions)
				{
					const actionsColumn = {
						label: '',
						name: 'actions',
						slotName: 'actions',
						dataType: 'Action',
						isActions: true,
						columnClasses: 'row-actions',
						columnHeaderClasses: 'thead-actions',
						rowTextAlignment: 'text-center',
						columnTextAlignment: 'text-center'
					}

					if (this.actionsPlacement === 'right') this.vbtColumns.push(actionsColumn)
					else if (this.actionsPlacement === 'left') this.vbtColumns.unshift(actionsColumn)
				}
				//END: Add row actions column

				//BEGIN: Add checklist column
				const checklistColumn = {
					label: '',
					name: 'Checkbox',
					slotName: 'checklist',
					dataType: 'Checkbox',
					isChecklist: true,
					checkListName: 'CheckList Name',
					checkListTitle: 'CheckList Title'
				}

				if (this.rowsSelectableMultiple !== false) this.vbtColumns.unshift(checklistColumn)
				//END: Add checklist column

				//FOR: EXTENDED ROW ACTIONS
				//Add row actions column
				if (this.hasExtendedRowActions)
				{
					this.vbtColumns.unshift({
						label: '',
						name: 'ExtendedAction',
						dataType: 'ExtendedAction',
						isExtendedActions: true,
						columnClasses: 'row-extended-actions',
						columnHeaderClasses: 'thead-actions'
					})
				}

				//FOR: TABLE LIST ROW DRAG AND DROP
				//BEGIN: Add drag and drop column
				const dragColumn = {
					label: '',
					name: 'dragAndDrop',
					slotName: 'dragAndDrop',
					isDragAndDrop: true,
					columnClasses: 'row-orders',
					columnHeaderClasses: 'thead-actions',
					rowTextAlignment: 'text-center',
					columnTextAlignment: 'text-center'
				}

				if (this.showRowDragAndDropOption || this.hasRowDragAndDrop) this.vbtColumns.unshift(dragColumn)
				//END: Add drag and drop column

				//Add vbt column IDs
				forEach(this.vbtColumns, (_, index) => {
					Reflect.set(this.vbtColumns[index], 'vbtColId', index + 1)
				})

				// If tree table, create column hierarchy
				if (this.type === 'TreeList') this.columnHierarchy = listFunctions.getColumnHierarchy(this.vbtColumns)

				this.$nextTick().then(() => {
					if (this.$refs.tableElem && this.$refs.tableContainerElem)
					{
						//FOR: COLUMN RESIZE
						this.applyColumnResizeable()

						//FOR: COLUMN SIZES
						this.setColumnSizes(this.columnSizes)
					}
				})
			},

			//FOR: COLUMN RESIZE
			/**
			 * Apply the column resizable feature
			 */
			applyColumnResizeable()
			{
				if (this.isListVisible && this.allowColumnResize)
				{
					this.resizableGrid = markRaw(
						new ColumnResizeable(
							this.$refs.tableElem,
							this.columnResizeOptions,
							this.$refs.tableFooterElem,
							this.$refs.tableContainerElem,
							this.$refs.tableWrapperElem
						)
					)

					this.resizableGrid.init()
				}
			},

			/**
			 * Get CSS class for table action element
			 * @param action {Object}
			 * @returns CSS class
			 */
			getActionButtonClass(action)
			{
				return has(action, 'class') ? action.class : 'btn-secondary'
			},

			/**
			 * Call sort function
			 */
			updateSort()
			{
				//Remove search errors
				this.signalSearch = { searchError: false }

				if (this.serverMode)
				{
					this.changeQuery()
				} else
				{
					this.sort()
				}
			},

			/**
			 * Sort by column (built-in method)
			 */
			sort()
			{
				if (this.query.sort.length !== 0)
				{
					let orders = this.query.sort.map((sortConfig) => sortConfig.order)

					this.tempFilteredResults = orderBy(
						this.tempFilteredResults,
						this.query.sort.map((sortConfig) => {
							return (row) => {
								let value = get(row.Fields, sortConfig.name)
								if (sortConfig.caseSensitive) return value ?? ''
								return value?.toString().toLowerCase() ?? ''
							}
						}),
						orders
					)
				}

				this.paginateFilter()
			},

			/**
			 * Call search function
			 */
			updateSearch()
			{
				// Remove search errors
				this.signalSearch = { searchError: false }

				// Update value in table object
				this.$emit('set-property', 'config', 'query', this.query.globalSearch)

				if (this.serverMode) this.changeQuery()
				else this.filter(!this.preservePageOnDataChange)
			},

			/**
			 * Filter rows (built-in method)
			 * @param resetPage {Boolean}
			 * @param isInit {Boolean}
			 * @returns Array (success) or Boolean (?)
			 */
			filter(resetPage = true, isInit = false)
			{
				let res = filter(this.rows, () => true)

				this.tempFilteredResults = res

				// Do global search only if global search text is not empty and
				// filtered results is also not empty
				if ((this.query.globalSearch !== '' || this.hasSearchBarFilters) && this.rowCount !== 0)
				{
					this.tempFilteredResults = this.search(this.tempFilteredResults)
				}

				this.sort()
				if (resetPage || this.rowCount === 0)
				{
					this.page = 1
				} else if (!isInit)
				{
					let newTotalPage = Math.ceil(this.rowCount / this.perPage)
					this.page = this.page <= newTotalPage ? this.page : newTotalPage
				}
			},

			/**
			 * Search rows (built-in method)
			 * @param tempFilteredResults {Object}
			 * @returns Array (success) or Boolean (?)
			 */
			search(tempFilteredResults)
			{
				let globalSearchResults = filter(tempFilteredResults, (row) => {
					let flag = false

					this.vbtColumns.some((vbt_column) =>
					{
						let searchValue = this.query.globalSearch
						if (this.hasSearchBarFilters)
						{
							//Skip fields that do not have corresponding column filters
							if (this.query.searchBarFilters[vbt_column.area + '.' + vbt_column.field] === undefined)
							{
								return
							} else
							{
								searchValue = this.query.searchBarFilters[vbt_column.area + '.' + vbt_column.field].conditions[0].values[0]
							}
						}

						let value = this.getCellDataSearch(row, vbt_column, {})

						if (value === null || typeof value === 'undefined')
						{
							value = ''
						}

						if (typeof value !== 'string')
						{
							value = value.toString()
						}

						if (typeof searchValue !== 'string')
						{
							searchValue = searchValue.toString()
						}

						if (!this.globalSearch.caseSensitive)
						{
							value = value.toLowerCase()
							searchValue = searchValue.toLowerCase()
						}

						if (value.indexOf(searchValue) > -1)
						{
							flag = true
							return
						}
					})

					return flag
				})

				return globalSearchResults
			},

			/**
			 * Get array of rows for current page (built-in method)
			 * @returns Array of rows
			 */
			paginateFilter()
			{
				if (this.pagination)
				{
					let start = (this.page - 1) * this.perPage
					let end = start + this.perPage
					this.vbtRows = this.tempFilteredResults.slice(start, end)
				} else
				{
					this.vbtRows = cloneDeep(this.tempFilteredResults)
				}
			},

			//FOR: TABLE LIST ACTIONS
			/**
			 * Returns true if the action is to be called and false otherwise (if false then selects the row clicked)
			 * @param row {Object}
			 */
			doClickAction(row)
			{
				if (!this.loaded) return false

				switch (this.rowClickActionInternal)
				{
					case 'selectSingle':
						this.toggleRowSelectSingle(row)
						return false
					case 'selectMultiple':
						if (!this.readonly)
							this.toggleRowSelectMultiple(row)
						return false
					default:
						return true
				}
			},

			/**
			 * Emit event and object representing action
			 * @param row {Object}
			 * @param column {Object}
			 */
			executeActionCell(row, column)
			{
				const emitAction = { row: row }

				if (column !== undefined) emitAction['column'] = column

				this.$emit('cell-action', emitAction)
			},

			//FOR: EXTENDED ROW ACTIONS
			disableAllChecks()
			{
				this.blockTableCheck = true
			},

			enableAllChecks()
			{
				this.blockTableCheck = false
			},

			/**
			 * Check all rows in "every" checklist column page
			 * @returns Boolean
			 */
			checkAllRows()
			{
				if (!this.vbtAllSelected)
				{
					this.checkCurrentPageRows()

					this.$emit('set-qtable-all-selected', { isSelected: true, id: this.id })
					this.vbtAllSelected = true

					//Make sure no one can uncheck rows
					this.disableAllChecks()
				}
			},

			/**
			 * Check all rows in current checklist column page
			 * @returns Boolean
			 */
			checkCurrentPageRows(isInit = false)
			{
				if (this.vbtAllSelected && !isInit)
				{
					this.$emit('set-qtable-all-selected', { isSelected: false, id: this.id })
					this.vbtAllSelected = false

					//Re-enable rows
					this.enableAllChecks()
				}

				var key = '',
					row = {},
					rowKeysArray = {}

				//Check all visible rows if they aren't checked already
				if (!this.isSelectedAllRows)
				{
					for (key in this.rows)
					{
						row = this.rows[key]

						//If row is not already selected
						if (!this.isRowSelected(row))
						{
							//Add to selected rows
							rowKeysArray[row.rowKey] = true
						}
					}
					this.$emit('select-rows', rowKeysArray)
				}
			},

			/**
			 * Uncheck all rows in "every" checklist column page
			 * @returns Boolean
			 */
			checkNoneRows()
			{
				if (this.vbtAllSelected)
				{
					this.$emit('set-qtable-all-selected', { isSelected: false, id: this.id })
					this.vbtAllSelected = false

					//Re-enable rows
					this.enableAllChecks()
				}

				this.$emit('unselect-all-rows')
			},

			//FOR: TABLE LIST COLUMN SUPPORT FORMS
			/**
			 * Determine if column has a support form associated
			 * @param column {Object}
			 * @returns Boolean
			 */
			columnHasSupportForm(column)
			{
				return has(column, 'supportForm') && column.supportForm.length > 0
			},

			/**
			 * Get primary key of row
			 * @param row {Object}
			 * @returns String
			 */
			getRowPk(row)
			{
				if (row.Fields === undefined || this.pkColumn === undefined)
				{
					return null
				}
				return row.Fields[this.pkColumn]
			},

			/**
			 * Get primary key of row
			 * @param row {Object}
			 * @returns String
			 */
			getRowKey(row)
			{
				return this.getRowPk(row) || row.Rownum
			},

			//FOR: Formatting field data
			/**
			 * Get formatted string representing cell value. Calls formatting function based on column data type.
			 * @param row {Object}
			 * @param column {Object}
			 * @param options {Object} [optional]
			 * @returns String(plain text or HTML)
			 */
			getCellDataDisplay(row, column, options)
			{
				if (options !== undefined)
				{
					return listFunctions.getCellValueDisplay(this, row, column, options)
				}

				return listFunctions.getCellValueDisplay(this, row, column)
			},

			/**
			 * Get string for title attribute of each cell in a row
			 * @param row {Object}
			 * @param columns {Object}
			 * @param options {Object} [optional]
			 * @returns String
			 */
			getRowCellDataTitles(row, columns, options)
			{
				var cellTitles = {}

				if (options !== undefined)
				{
					options = {}
				}

				for (let col in columns)
				{
					let column = columns[col]

					if (column.isHtmlField) cellTitles[column.name] = ''
					else if (this.isDragAndDropColumn(column)) cellTitles[column.name] = this.texts.rowDragAndDropTitle
					else cellTitles[column.name] = listFunctions.getCellValueDisplay(this, row, column, options)
				}

				return cellTitles
			},

			/**
			 * Get string for search value of cell data
			 * @param row {Object}
			 * @param column {Object}
			 * @param options {Object} [optional]
			 * @returns String
			 */
			getCellDataSearch(row, column, options)
			{
				if (options !== undefined)
				{
					return listFunctions.getCellValueSearch(this, row, column, options)
				}

				return listFunctions.getCellValueSearch(this, row, column)
			},

			//BEGIN: Row methods
			/**
			 * Determine if row is in a valid state
			 * @param row {Object}
			 * @returns String
			 */
			rowIsValid(row)
			{
				if (this.config.rowValidation)
				{
					if (this.config.rowValidation.fnValidate(row) === false)
					{
						//Row is not valid
						return false
					}
				}
				return true
			},

			/**
			 * Get row CSS classes
			 * @param row {Object}
			 * @returns String
			 */
			getRowClasses(row, columnsLevel = 0)
			{
				let rowClasses = []

				if (this.hasRowClickAction) rowClasses.push('c-table__row--clickable')

				if (this.rowIsValid(row) === false) rowClasses.push(this.config.rowValidation.class ?? 'c-table__row--pending')
				else if (columnsLevel > 0) rowClasses.push('q-tree-table-row')

				if (!this.loaded) rowClasses.push('c-table__row--loading')

				return rowClasses.join(' ')
			},

			/**
			 * Get row title (for title attribute)
			 * @param row {Object}
			 * @returns String
			 */
			getRowTitle(row)
			{
				if (this.rowIsValid(row) === false)
				{
					return this.config.rowValidation.message.value
				}
				return ''
			},

			//FOR: ROW CLICK ACTION (default click action or select row)
			/**
			 * Do action when clicking on row: default click action (emit) or select row
			 * @param row {Object}
			 */
			executeRowClickAction(row)
			{
				if (this.blockTableCheck || this.hasRowDragAndDrop)
				{
					return
				}

				//Remove child rows
				row = this.rowWithoutChildren(row)

				//Row actions that do not emit data outside of the component
				if (!this.doClickAction(row)) return

				//Execute default row action
				let rowKeyPath = listFunctions.getRowKeyPath(this.rows, row)
				if (Object.keys(this.rowClickAction).length > 0) this.$emit('row-action', { id: this.rowClickAction.id, rowKeyPath })
				else this.$emit('row-action', { rowKeyPath })
			},

			/**
			 * Toggle selecting/deselecting single row
			 * @param row {Object}
			 */
			toggleRowSelectSingle(row)
			{
				let rowKeyPath = listFunctions.getRowKeyPath(this.rows, row)

				//If row is already selected, remove from selected rows
				if (this.isRowSelected(row))
				{
					this.$emit('unselect-row', rowKeyPath)
				} else
				{
					//Add to selected rows
					this.$emit('select-row', { rowKeyPath, multipleSelection: false })
				}
			},

			/**
			 * Toggle selecting/deselecting row (add to or remove from selected rows array)
			 * @param row {Object}
			 */
			toggleRowSelectMultiple(row)
			{
				let rowKeyPath = listFunctions.getRowKeyPath(this.rows, row)

				//If row is already selected, remove from selected rows
				if (this.isRowSelected(row))
				{
					this.$emit('unselect-row', rowKeyPath)
				} else
				{
					//If row is not already selected, add to selected rows
					this.$emit('select-row', { rowKeyPath, multipleSelection: true })
				}
			},

			/**
			 * Determine if row is selected
			 * @param row {Object}
			 * @returns Boolean
			 */
			isRowSelected(row)
			{
				let rowKeyPath = listFunctions.getRowKeyPath(this.rows, row)

				for (let x in this.rowsSelected)
				{
					if (x.toString() === rowKeyPath.toString())
					{
						return true
					}
				}
				return false
			},

			//FOR: EXTENDED ROW ACTIONS
			/**
			 * Emit event to remove selected row
			 * @param rowKey {String}
			 * @returns Boolean
			 */
			removeRow(rowKey)
			{
				this.$emit('remove-row', rowKey)
			},
			//END: Row methods

			/**
			 * Get sum of values of column in rows
			 * @param column {Object}
			 * @param selectedOnly {Boolean} total only selected rows
			 * @returns Number
			 */
			getColumnTotalValue(column, selectedOnly)
			{
				if (column.showTotal)
				{
					var total = 0
					var row = {}

					if (selectedOnly === undefined)
					{
						selectedOnly = false
					}

					//Sum column values of rows
					if (column.showTotal !== false)
					{
						for (let idx in this.rows)
						{
							row = this.rows[idx]

							if (selectedOnly === false || this.isRowSelected(row))
							{
								total += this.getValueFromRow(row, column)
							}
						}

						return total
					}
				}
				return ''
			},

			/**
			 * Get sum of values of column in rows and return formatted string of value
			 * @param column {Object}
			 * @param selectedOnly {Boolean} total only selected rows
			 * @returns String
			 */
			getColumnTotalValueDisplay(column, selectedOnly)
			{
				//Actions column shows text to show this is the totals row
				if (this.isActionsColumn(column))
				{
					return 'Total'
				}
				//Column to show total (numeric or currency type)
				else if (column.showTotal)
				{
					var value = 0

					//Determine whether to get all rows or only selected rows
					if (selectedOnly !== undefined)
					{
						value = this.getColumnTotalValue(column, selectedOnly)
					} else
					{
						value = this.getColumnTotalValue(column)
					}

					//Determine whether to display as currency or plain number
					if (column.currency !== undefined)
					{
						return genericFunctions.currencyDisplay(
							value,
							this.numberFormat.decimalSeparator,
							this.numberFormat.groupSeparator,
							column.decimalPlaces,
							column.currency,
							this.lcid,
							'narrowSymbol'
						)
					} else
					{
						return genericFunctions.numericDisplay(value, this.numberFormat.decimalSeparator, this.numberFormat.groupSeparator, {
							minimumFractionDigits: column.decimalPlaces,
							maximumFractionDigits: column.decimalPlaces
						})
					}
				}
				return ''
			},

			/**
			 * Get CSS class for column with sum of values
			 * @param column {Object}
			 * @returns String
			 */
			getColumnTotalClass(column)
			{
				if (this.isActionsColumn(column))
				{
					if (this.actionsPlacement === 'right')
					{
						return 'columns-sum-total-right'
					} else
					{
						return 'columns-sum-total-left'
					}
				} else
				{
					return 'c-table__cell-numeric'
				}
			},

			/**
			 * Submit selected rows
			 * @param event {String}
			 * @returns
			 */
			rowGroupAction(event)
			{
				this.$emit('row-group-action', {
					rowsSelected: this.rowsSelected,
					allSelected: this.vbtAllSelected,
					action: event.action
				})
			},

			/**
			 * Clear search (built-in method)
			 */
			clearGlobalSearch()
			{
				this.query.globalSearch = ''
				this.updateSearch()
			},

			/**
			 * Reset search query
			 */
			resetQuery()
			{
				this.clearSearchBarFilters()

				//Clear search bar
				if (this.query.globalSearch?.length > 0)
				{
					this.query.globalSearch = ''
				}
				this.$emit('reset-query')
				this.updateSearch()
			},

			/**
			 * Emit search event (built-in method)
			 * @param searchValue {String}
			 */
			emitSearch(searchValue)
			{
				this.query.globalSearch = searchValue
				this.updateSearch()
			},

			/**
			 * Emit search query event (update page if necessary)
			 * @param page {Number}
			 */
			changeQuery(page = null)
			{
				// Update page number
				if (page === undefined || page === null)
				{
					if (!this.preservePageOnDataChange)
						this.page = 1

					page = this.page
				}

				// Update page value in table object
				this.$emit('set-property', 'config', 'page', page)

				// Signal change in query
				if (this.serverMode && this.canEmitQueries)
					this.$emit('on-change-query')
			},

			/**
			 * Determine if column is searchable
			 * @param column {Object}
			 * @returns boolean
			 */
			isSearchableColumn(column)
			{
				return listFunctions.isSearchableColumn(column)
			},

			/**
			 * Full column name
			 * @param {object} column
			 */
			columnFullName(column)
			{
				return listFunctions.columnFullName(column)
			},

			//BEGIN: FOR: SEARCH FILTERS
			/**
			 * Edit column filter
			 * @param {string} fullColumnName
			 * @param {Object} filter
			 */
			editColumnFilter(fullColumnName, filter)
			{
				this.query.columnFilters[fullColumnName] = filter
				this.$emit('set-property', 'columnFilters', this.query.columnFilters)
				this.$emit('update-config')
				this.updateSearch()
			},

			/**
			 * Remove column filter
			 * @param {string} fullColumnName
			 */
			removeColumnFilter(fullColumnName)
			{
				delete this.query.columnFilters[fullColumnName]
				this.$emit('set-property', 'columnFilters', this.query.columnFilters)
				this.$emit('update-config')
				this.updateSearch()
			},

			/**
			 * Clear all column filters
			 * @param {string} emitSearch
			 */
			clearColumnFilters(emitSearch = false)
			{
				this.query.columnFilters = {}
				this.$emit('set-property', 'columnFilters', this.query.columnFilters)
				if (emitSearch)
				{
					this.updateSearch()
				}
			},

			/**
			 * Remove search bar filter
			 * @param {string} fullColumnName
			 */
			removeSearchBarFilter(fullColumnName)
			{
				delete this.query.searchBarFilters[fullColumnName]
				this.updateSearch()
				this.signalSearch = { resetQuery: true }
			},

			/**
			 * Clear all search bar filters
			 * @param {string} emitSearch
			 */
			clearSearchBarFilters(emitSearch = false)
			{
				this.query.searchBarFilters = {}
				this.$emit('set-property', 'config', 'searchBarFilters', this.query.searchBarFilters)
				if (emitSearch)
				{
					this.updateSearch()
				}
			},

			/**
			 * Search by a column for a value
			 * @param column {Object}
			 * @param value {String}
			 */
			searchByColumn(column, value)
			{
				var columnName = listFunctions.columnFullName(column)
				var operator = searchFilterData.searchBarOperator(column.searchFieldType, value)
				this.query.searchBarFilters = {}
				this.query.searchBarFilters[columnName] = listFunctions.searchFilter('', true, [
					listFunctions.searchFilterCondition('', true, listFunctions.columnFullName(column), operator, [value])
				])
				this.query.globalSearch = ''

				// Update properties in table object
				this.$emit('set-property', 'config', 'searchBarFilters', this.query.searchBarFilters)
				this.$emit('set-property', 'config', 'query', this.query.globalSearch)

				this.updateSearch()
			},

			/**
			 * Search all columns for a value
			 * @param value {String}
			 */
			searchByAllColumns(value)
			{
				this.clearSearchBarFilters()
				this.emitSearch(value)
			},

			/**
			 * Remove all advanced filters and remove other custom filters
			 */
			removeAllCustomFilters()
			{
				this.clearColumnFilters()
				this.clearSearchBarFilters()
				//Also reloads table
				this.$emit('remove-all-advanced-filters')
				this.$emit('update-config')
				//this.updateSearch();
			},

			/**
			 * Deactivate all advanced filters and remove other custom filters
			 */
			deactivateAllCustomFilters()
			{
				this.$emit('deactivate-all-advanced-filters')
				this.clearColumnFilters()
				this.clearSearchBarFilters()
				this.updateSearch()
			},

			/**
			 * Show advanced filters interface
			 */
			showAdvancedFilters()
			{
				this.$emit(
					'signal-component',
					'config',
					{
						show: true,
						selectedTab: 'advanced-filters',
						returnElement: this.configMenuId
					},
					true
				)
				this.$emit('signal-component', 'advancedFilters', { selectedFilterIdx: undefined }, true)
			},

			/**
			 * Show advanced filters interface for creating a new filter
			 */
			showAdvancedFiltersNew()
			{
				this.$emit('signal-component', 'advancedFiltersNew', { show: true, selectedFilterIdx: undefined }, true)
			},
			//END: FOR: SEARCH FILTERS

			/**
			 * Emit action to export table data
			 * @param format {String}
			 * @returns Boolean
			 */
			exportData(format)
			{
				var payload = {
					format: format
				}
				this.$emit('on-export-data', payload)
			},

			/**
			 * Emit action to import table data
			 * @param format {String}
			 * @returns Boolean
			 */
			importData(payload)
			{
				/*
				var payload = {
					format: format
				}
				/**/
				this.$emit('on-import-data', payload)
			},

			/**
			 * Emit action to download template file
			 * @param format {String}
			 * @returns Boolean
			 */
			exportTemplate(format)
			{
				var payload = {
					format: format
				}
				this.$emit('on-export-template', payload)
			},

			/**
			 * Determine if multiple filters can be selected in group of filters
			 * @param entry {Object}
			 * @returns Boolean
			 */
			groupFilterIsMultiple(entry)
			{
				if (entry.isMultiple === undefined)
				{
					return false
				}
				return entry.isMultiple
			},

			/**
			 * Update values for all filters
			 */
			updateFilters()
			{
				// Calculate filter values for "multiple" type filters
				this.calcGroupFilterValues(this.groupFilters)
				this.query.groupFilters = this.groupFilters

				this.query.activeFilters = this.activeFilters

				this.changeQuery()

				this.$emit('update-config')
			},

			/**
			 * Get value for all radio button groups based on filter model
			 * @param groupFilters {Object}
			 */
			calcGroupFilterValues(groupFilters)
			{
				for (let idx in groupFilters)
				{
					var entry = groupFilters[idx]

					if (this.groupFilterIsMultiple(entry))
					{
						entry.value = this.checkboxValue(entry)
					}
				}
			},

			/**
			 * Get value for radio button group based on filter model
			 * @param entry {Object}
			 * @returns String
			 */
			checkboxValue(entry)
			{
				if (!this.groupFilterIsMultiple(entry))
				{
					return ''
				}

				var multipleFilterValue = ''

				for (let idx in entry.filters)
				{
					var filter = entry.filters[idx]
					if (filter.selected !== false)
					{
						multipleFilterValue += filter.key
					}
				}

				return multipleFilterValue
			},

			/**
			 * Get value for radio button group based on filter model
			 * @param entry {Object}
			 * @returns String
			 */
			radioValue(entry)
			{
				if (this.groupFilterIsMultiple(entry))
				{
					return ''
				}

				return filter.value
			},

			/**
			 * listener for events when shift key is pressed
			 */
			_documentOnselectstart(event)
			{
				return !(event.key === 'Shift' && event.shiftKey === true)
			},

			/**
			 * listener for events when shift key is pressed
			 */
			_handleShiftKey()
			{
				document.addEventListener('selectstart', this._documentOnselectstart)
			},

			/**
			 * Add event listeners for events when shift key is pressed?
			 */
			handleShiftKey()
			{
				window.addEventListener('keyup', this._handleShiftKey)
				window.addEventListener('keydown', this._handleShiftKey)
			},

			/**
			 * Determine if column is visible
			 * @param column {Object}
			 * @returns Boolean
			 */
			canShowColumn(column)
			{
				//FOR: DRAG AND DROP COLUMNS
				if ((this.isDragAndDropColumn(column) && !this.hasRowDragAndDrop) || (this.isActionsColumn(column) && this.hasRowDragAndDrop))
				{
					return false
				}
				//For all columns
				return column.visibility === undefined || column.visibility ? true : false
			},

			/**
			 * Toggle showing/hiding filters
			 */
			toggleShowFilters()
			{
				this.filtersVisible = !this.filtersVisible
				this.$emit('set-property', 'config', 'filtersVisible', this.filtersVisible)
			},

			/**
			 * Toggle showing/hiding static filters
			 */
			toggleShowStaticFilters()
			{
				this.staticFiltersVisible = !this.staticFiltersVisible
			},

			/**
			 * Emit to add new row
			 * @param row {Object}
			 * @returns
			 */
			emitRowAdd(row)
			{
				this.$emit('row-add', row)

				//Clear object for new row
				this.initRow(this.newRow)
			},

			/**
			 * Emit to update row values
			 * @param row {Object}
			 * @returns
			 */
			emitRowEdit(row)
			{
				this.$emit('row-edit', row)
			},

			/**
			 * Emit to delete rows
			 * @param rowKeys {Object}
			 * @returns
			 */
			emitRowsDelete(rowKeys)
			{
				this.$emit('rows-delete', rowKeys)

				//Clear checked rows so it doesn't have keys of records that don't exist
				this.$emit('unselect-all-rows')
			},

			/**
			 * Set the value of a cell
			 * @param row {Object}
			 * @param column {Object}
			 * @param value {Object}
			 * @returns
			 */
			setCellValue(row, column, value)
			{
				listFunctions.setCellValue(row, column, value)
			},

			/**
			 * Set the value of a cell
			 * @param row {Object}
			 * @param column {Object}
			 * @param value {Object}
			 * @returns
			 */
			setTableCellValue(row, column, value)
			{
				listFunctions.setTableCellValue(this, row, column, value)
			},

			/**
			 * Set the value of a cell
			 * @param row {Object}
			 * @param column {Object}
			 * @param options {Object}
			 * @returns
			 */
			cellOnChange(row, column, options)
			{
				listFunctions.cellOnChange(this, row, column, options)
			},

			/**
			 * Called when updating the value of a cell
			 * @param row {Object}
			 * @param column {Object}
			 * @param value {Object}
			 * @returns
			 */
			updateCell(row, column, event)
			{
				this.$emit('update-cell', { row: row, column: column, value: event })

				// Optionally prevent rerendering rows when changing cell values
				if(column.rerenderRowsOnNextChange === false)
					this.rerenderRowsOnNextChange = false
				else
					this.rerenderRows()

				//Set cell value in component data
				this.setCellValue(row, column, event)

				//Set cell value in external table data
				this.setTableCellValue(row, column, event)

				//Call method set to run when changing data in this column
				this.cellOnChange(row, column, {})

				if (this.fieldsEditable)
				{
					this.emitRowEdit(row, column, event)
				}

				if(this.hasRowDragAndDrop && column.isOrderingColumn)
				{
					let index = listFunctions.getCellValue(row, column)
					this.navigateToRow(parseInt(index) - 1)
				}
			},

			/**
			 * Toggle text wrap in cells
			 */
			toggleTextWrap()
			{
				this.hasTextWrap = !this.hasTextWrap
			},

			/**
			 * Toggle row reorder by drag and drop
			 */
			toggleShowRowDragAndDrop()
			{
				if (!this.hasRowDragAndDrop)
				{
					//Sort by the ordering column
					if (this.sortOrderColumn !== undefined && this.sortOrderColumn !== null)
					{
						this.updateSortQuery(this.sortOrderColumn, 'asc', true, false)
					}
				}

				this.$emit('toggle-rows-drag-drop')

				if (!this.hasRowDragAndDrop)
				{
					this.setPerPage(-1)
				}

				// FOR: GETTING NAVIGABLE ROW ELEMENTS
				// Rerender rows and re'calculate navigable rows
				this.rerenderRows()
			},

			/**
			 * Get the table DOM element
			 * @returns
			 */
			getTableElement()
			{
				const tableElem = this.$refs.tableElem

				if (tableElem === undefined || tableElem === null) return null

				if (Array.isArray(tableElem))
				{
					if (tableElem.length < 1) return null
					return tableElem[0]
				}

				return tableElem
			},

			/**
			 * Get the column header DOM elements
			 * @returns
			 */
			getColumnHeaderElements()
			{
				var columnHeaderDOMElems = []
				//Get table element
				var tableElem = this.getTableElement()
				if (tableElem === undefined || tableElem === null)
				{
					return columnHeaderDOMElems
				}
				//Get row elements
				var rowElems = tableElem.getElementsByTagName('TR')
				if (rowElems === undefined || rowElems === null || rowElems.length < 1)
				{
					return columnHeaderDOMElems
				}
				//Get column header elements
				var columnHeaderElems = rowElems[0].getElementsByTagName('TH')
				if (columnHeaderElems === undefined || columnHeaderElems === null || columnHeaderElems.length < 1)
				{
					return columnHeaderDOMElems
				}
				//Add DOM elements to array
				for (let idx in columnHeaderElems)
				{
					columnHeaderDOMElems.push(columnHeaderElems.item(idx))
				}
				return columnHeaderDOMElems
			},

			/**
			 * Get the column size configuration
			 * @returns
			 */
			getColumnSizes()
			{
				//Get column header elements
				var columnHeaderElems = this.getColumnHeaderElements()

				//Get column sizes
				var columnSizes = {}
				var column = {}
				for (let idx in columnHeaderElems)
				{
					column = columnHeaderElems[idx]
					if (column.style.width.length > 0)
					{
						columnSizes[column.getAttribute('data-column-name')] = column.style.width
					}
				}

				//Get table size
				var tableSize = ''
				var tableElem = this.getTableElement()

				if (tableElem !== undefined && tableElem !== null)
				{
					tableSize = tableElem.style.width
				}

				return { columnSizes: columnSizes, tableSize: tableSize }
			},

			/**
			 * Set the column sizes in the DOM elements
			 * @param columnSizes {Array}
			 * @returns
			 */
			setColumnSizes(columnSizes)
			{
				//Set to default if null
				if (
					columnSizes === undefined ||
					columnSizes === null ||
					columnSizes.columnSizes === undefined ||
					columnSizes.columnSizes === null ||
					columnSizes.tableSize === undefined ||
					columnSizes.tableSize === null
				)
				{
					columnSizes = { columnSizes: {}, tableSize: '' }
				}

				//Get table element
				var tableElem = this.getTableElement()
				if (tableElem === undefined || tableElem === null)
				{
					return
				}

				//Set table size
				tableElem.style.width = columnSizes.tableSize

				//Get column header elements
				var columnHeaderElems = this.getColumnHeaderElements()

				//Set column sizes
				var column = {}
				var columnSize = {}
				for (let idx in columnHeaderElems)
				{
					column = columnHeaderElems[idx]

					columnSize = columnSizes.columnSizes[column.getAttribute('data-column-name')]
					if (columnSize !== undefined && columnSize !== null)
					{
						if (columnSize.length > 0)
						{
							column.style.width = columnSize
						}
					} else
					{
						column.style.width = null
					}
				}

				//Set table footer size
				this.setTableFooterSize()

				//Set table container element size
				const tableContainerElement = this.getTableContainerElement()
				if (tableContainerElement === undefined || tableContainerElement === null) return

				if (columnSizes.tableSize === '') tableContainerElement.style.width = null
			},

			/**
			 * Get the table wrapper DOM element
			 * @returns
			 */
			getTableWrapperElement()
			{
				const tableWrapperElem = this.$refs.tableWrapperElem
				if (tableWrapperElem === undefined || tableWrapperElem === null)
				{
					return null
				}
				if (Array.isArray(tableWrapperElem))
				{
					if (tableWrapperElem.length < 1)
					{
						return null
					}
					return tableWrapperElem[0]
				}

				return tableWrapperElem
			},

			/**
			 * Get the table container DOM element
			 * @returns
			 */
			getTableContainerElement()
			{
				const tableContainerElem = this.$refs.tableContainerElem
				if (tableContainerElem === undefined || tableContainerElem === null)
				{
					return null
				}
				if (Array.isArray(tableContainerElem))
				{
					if (tableContainerElem.length < 1)
					{
						return null
					}
					return tableContainerElem[0]
				}

				return tableContainerElem
			},

			/**
			 * Get the table footer DOM element
			 * @returns
			 */
			getTableFooterElement()
			{
				const tableFooterElem = this.$refs.tableFooterElem
				if (tableFooterElem === undefined || tableFooterElem === null)
				{
					return null
				}
				if (Array.isArray(tableFooterElem))
				{
					if (tableFooterElem.length < 1)
					{
						return null
					}
					return tableFooterElem[0]
				}

				return tableFooterElem
			},

			/**
			 * Set the size of the table footer DOM element
			 * @returns
			 */
			setTableFooterSize()
			{
				var tableElem = this.getTableElement()
				if (tableElem === undefined || tableElem === null)
				{
					return
				}
				var tableWrapperElem = this.getTableWrapperElement()
				if (tableWrapperElem === undefined || tableWrapperElem === null)
				{
					return
				}
				var tableFooterElem = this.getTableFooterElement()
				if (tableFooterElem === undefined || tableFooterElem === null)
				{
					return
				}

				if (tableElem.offsetWidth < tableWrapperElem.offsetWidth)
				{
					tableFooterElem.style.width = tableElem.offsetWidth + 'px'
				} else
				{
					tableFooterElem.style.width = null
				}
			},

			/**
			 * Get the header DOM element
			 * @returns array
			 */
			getHeaderRowElement()
			{
				const headerRowComponent = this.$refs.headerRowElem

				if (Array.isArray(headerRowComponent) && headerRowComponent.length > 0)
					return headerRowComponent[0].$el

				return headerRowComponent?.$el
			},

			/**
			 * Save current view
			 * @returns
			 */
			saveCurrentView()
			{
				this.$emit('save-view', {
					name: this.config.UserTableConfigName,
					isSelected: false
				})

				let alertProps = {
					type: 'success',
					message: this.texts.tableViewSaveSuccess,
					icon: 'ok'
				}
				this.$emit('set-info-message', alertProps)
				this.$emit('set-property', 'confirmChanges', false)
				this.$emit('update-config')
			},

			/**
			 * Close current view
			 * @returns
			 */
			closeCurrentView()
			{
				this.$emit('close-view', {})
			},

			/**
			 * Check before saving or copying a view
			 * @returns
			 */
			checkSaveCurrentView()
			{
				let buttons = {
					confirm: {
						label: this.texts.saveText,
						action: this.saveCurrentView
					},
					cancel: {
						label: this.texts.cancelText
					}
				}

				genericFunctions.displayMessage(this.texts.wantToSaveChanges, 'warning', null, buttons)
			},

			/**
			 * Fired on column resize
			 * @returns
			 */
			onColumnResize()
			{
				this.$emit('set-property', 'config', 'columnSizes', this.getColumnSizes())
				this.$emit('update-config')
			},

			/**
			 * Fired on multiform select
			 * @param row {Object}
			 * @returns
			 */
			onMultiformSelect(row)
			{
				for (let idx in this.rowFormProps)
				{
					if (this.rowFormProps[idx].mode === 'EDIT')
					{
						return
					}
				}
				this.$emit('set-array-sub-prop-where', 'rowFormProps', 'id', row.rowKey, 'mode', 'EDIT', 'SHOW')
			},

			/**
			 * Handler for reordering rows with keyboard.
			 * @param evt {Object} { row, sortOrderColumn, shiftValue }
			 * @returns
			 */
			rowReorder(eventData)
			{
				// Emit for row reorder by the server
				var rowKey = this.getRowKey(eventData.row),
					currentIndex = findIndex(this.vbtRows, (row) => this.getRowKey(row) === rowKey),
					index = currentIndex + eventData.shiftValue
				this.$emit('row-reorder', { rowKey, index })
				// Set table navigation to the row
				this.navigateToRow(parseInt(index))
				this.$emit('set-property', 'config', 'setNavOnUpdate', true)
				// FOR: GETTING NAVIGABLE ROW ELEMENTS
				// Make rows re-mount
				this.rerenderRows()
			},

			/**
			 * Handler for reordering rows with drag and drop.
			 * @param evt {Object}
			 * @returns
			 */
			onRowDragAndDrop(evt)
			{
				if (evt.newIndex !== evt.oldIndex)
				{
					// Emit for row reorder by the server
					var rowIndex = parseInt(evt.item.getAttribute('index')),
						row = this.vbtRows[rowIndex],
						rowKey = row ? row.rowKey : null,
						index = evt.newIndex
					if (!rowKey) return

					// Removes the class to prevent the selection of other rows text, 
					// added when starting the drag
					this.$refs.tableElem.classList.remove('.q-table-list__dragging');

					this.$emit('row-reorder', { rowKey, index })
					// Set table navigation to the row
					this.navigateToRow(parseInt(index))
					this.$emit('set-property', 'config', 'setNavOnUpdate', true)
					// FOR: GETTING NAVIGABLE ROW ELEMENTS
					// Make rows re-mount
					this.rerenderRows()
				}
			},

			destroyRowsDragAndDrop()
			{
				if (this.sortablePlugin && this.sortablePlugin.destroy)
				{
					this.sortablePlugin.destroy()
				}
				this.sortablePlugin = null
			},

			initRowsDragAndDrop()
			{
				this.destroyRowsDragAndDrop()
				try
				{
					if (this.hasRowDragAndDrop)
					{
						this.sortablePlugin = Sortable.create(Array.isArray(this.$refs.tbody) ? this.$refs.tbody[0] : this.$refs.tbody, {
							draggable: 'tr',
							dataIdAttr: 'id',
							direction: 'vertical',
							chosenClass: 'row-grabbed',
							ghostClass: 'row-ghost',
							dragClass: 'row-drag',
							handle: '.c-table__drag',
							animation: 300,
							forceFallback: true,
							fallbackClass: 'row-drag',
							// Adds a class to prevent the selection of other rows text
							onStart: () => this.$refs.tableElem.classList.add('.q-table-list__dragging'),
							onEnd: this.onRowDragAndDrop
						})
					}
				} catch {
					this.destroyRowsDragAndDrop()
				}
			},

			/**
			 * Scroll to row
			 * @param rowId {String}
			 * @param behavior {String}
			 * @returns
			 */
			scrollToRow(rowId, behavior = 'smooth')
			{
				var elem = document.getElementById(rowId)
				if (!elem) return
				var wrapper = this.tableContainerElem
				var dist = elem.offsetTop - 160
				wrapper?.scroll?.({ top: dist, left: 0, behavior })
			},

			/**
			 * Scroll to and select row
			 * @param rowKeyPath {Array/String}
			 * @param rowId {String}
			 * @returns
			 */
			goToRow(rowKeyPath, rowId)
			{
				this.$emit('go-to-row', rowKeyPath)

				this.scrollToRow(rowId)
			},

			/**
			 * Emits for table configuration menu
			 * @param id {string}
			 */
			emitConfigAction(id)
			{
				let eObj = find(this.configOptions, ['id', id])
				if (!eObj) return

				// If saving changes to current table view
				if (eObj.id === 'viewSaveChanges') this.checkSaveCurrentView()
				// Other options that open the table configuration pop-up and need an element ID
				// to signal which tab will be selected in the pop-up
				else if (eObj.elementId)
				{
					if (eObj.elementId === 'advanced-filters') this.showAdvancedFilters()
					else
						this.$emit('signal-component', 'config', {
							show: true,
							selectedTab: eObj.elementId,
							returnElement: this.configMenuId
						})
				}
			},

			/**
			 * Get view by ID
			 * @param id {number}
			 */
			getViewById(id)
			{
				return find(this.savedViewsOptions, ['id', id])
			},

			/**
			 * Get view ID by Name
			 * @param name {string}
			 */
			getViewIdByName(name)
			{
				let view = find(this.savedViewsOptions, ['value', name])
				if (!view) return 0
				return view.id
			},

			/**
			 * Set view as selected view by ID
			 * @param id {number}
			 */
			setSelectedViewById(id)
			{
				//Remove search errors
				this.signalSearch = { searchError: false }

				if (id === 0)
				{
					this.closeCurrentView()
				} else
				{
					let selectedViewInfo = find(this.savedViewsOptions, ['key', id])
					if (!selectedViewInfo) return
					//Emit data to script which calls apply function
					this.$emit('view-action', { name: 'SHOW', rowValue: selectedViewInfo.value })
				}
			},

			/**
			 * Save view and set view as selected view by ID
			 * @param id {number}
			 */
			saveViewOpenView(id)
			{
				this.saveCurrentView()
				this.setSelectedViewById(id)
			},

			/**
			 * Confirm whether to save if there are changes and set view as selected view by ID
			 * @param id {number}
			 */
			confirmAndSetSelectedViewById(id)
			{
				if (this.confirmChanges)
				{
					let buttons = {
						confirm: {
							label: this.texts.saveText,
							action: this.saveViewOpenView
						},
						cancel: {
							label: this.texts.discard,
							action: this.setSelectedViewById
						}
					}
					genericFunctions.displayMessage(`${this.texts.wantToSaveChangesToView}`, 'warning', null, buttons, { callbackParams: id })
				} else
				{
					this.setSelectedViewById(id)
				}
			},

			setChildRowsVisibility(eventData)
			{
				if (eventData.show === true && eventData.row?.alreadyLoaded === false)
				{
					// Prevent rows from re-rendering when changing the property to show sub-rows
					this.$emit('set-property', 'config', 'rerenderRowsOnNextChange', false)

					this.$emit('tree-load-branch-data', { row: eventData.row })
				}
			},

			/**
			 * Get all navigable row elements
			 * FOR: GETTING NAVIGABLE ROW ELEMENTS
			 */
			getNavRowElements()
			{
				// Get main row elements
				const tableElem = this.getTableElement()
				const navRowElemsNL = tableElem.querySelectorAll("tr[data-table-action-selected]")
				const navRowElems = Array.from(navRowElemsNL)

				// Add header row if it has focusable elements
				const headerRowElement = this.getHeaderRowElement()
				const headerRowActionElements = this.getTableRowActionElements(headerRowElement)
				if(headerRowActionElements.length > 0)
					navRowElems.unshift(headerRowElement)

				return navRowElems
			},

			/**
			 * Set all navigable row elements
			 * FOR: GETTING NAVIGABLE ROW ELEMENTS
			 */
			setNavRowElements()
			{
				/**
				 * Check if table element exists first. If not, cancel.
				 * This can happen if the table is already being unmounted because
				 * the rows are unmounted and changes to the rows trigger this function.
				 */
				const tableElem = this.getTableElement()
				if(tableElem === undefined || tableElem === null)
					return

				this.navRowElems = this.getNavRowElements()

				/**
				 * Set index of first data row.
				 * An index of 'h' is used for the header row.
				 * If element 0 of the array is the header row, the first data row is element 1
				 */
				if(this.navRowElems.length > 0 && this.navRowElems[0].getAttribute('index') === 'h')
					this.firstDataRowIndex = 1
				else
					this.firstDataRowIndex = 0
			},

			/**
			 * Set number of rows to render from row data loaded
			 * FOR: GETTING NAVIGABLE ROW ELEMENTS
			 */
			setRowsToLoad()
			{
				// Track number of rows to render
				this.rowsToLoad = this.vbtRows?.length
			},

			/**
			 * Re-render rows
			 * FOR: GETTING NAVIGABLE ROW ELEMENTS
			 */
			rerenderRows()
			{
				// Track number of rows to render
				this.setRowsToLoad()
				// Make rows re-mount
				this.rowDomKey++
			},

			/**
			 * Called when a row is rendered
			 * FOR: GETTING NAVIGABLE ROW ELEMENTS
			 */
			onRowLoaded()
			{
				this.rowsToLoad--
				if(this.rowsToLoad === 0)
					this.onRowsLoaded()
			},

			/**
			 * Called when rows are rendered
			 * FOR: GETTING NAVIGABLE ROW ELEMENTS
			 */
			onRowsLoaded()
			{
				this.$emit('rows-loaded')
				this.setNavRowElements()
			},

			/**
			 * Called when all sub-rows are loaded
			 * FOR: GETTING NAVIGABLE ROW ELEMENTS
			 */
			onSubRowsLoaded()
			{
				this.$emit('rows-loaded')
				this.setNavRowElements()
			}
		},
		//END: methods

		watch: {
			'rows': {
				handler(newVal)
				{
					this.vbtRows = newVal

					if (!this.serverMode)
					{
						this.filter(!this.preservePageOnDataChange, !this.isFirstTime)
					} else
					{
						if (this.preservePageOnDataChange)
						{
							let predictedTotalPage = Math.ceil(this.rowCount / this.perPage)
							if (predictedTotalPage !== 0)
							{
								this.page = this.page <= predictedTotalPage ? this.page : predictedTotalPage
							} else
							{
								this.page = 1
							}
						}
					}
					this.isFirstTime = false

					if (this.vbtAllSelected) this.checkCurrentPageRows(true)

					// Track number of rows to render
					this.setRowsToLoad()

					if(this.rerenderRowsOnNextChange)
					{
						// FOR: GETTING NAVIGABLE ROW ELEMENTS
						// Rerender rows and re-calculate navigable rows
						this.rerenderRows()
					}
					else
					{
						this.rerenderRowsOnNextChange = true
						this.$emit('set-property', 'config', 'rerenderRowsOnNextChange', true)
					}
				},
				deep: true
			},

			'columns': {
				handler()
				{
					//FOR: ROW ACTIONS, EXTENDED ACTIONS, COLUMN ORDER AND VISIBILITY
					//Must be called when loading or changing columns
					this.updateColumns()
				},
				deep: true
			},

			'advancedFilters': {
				handler()
				{
					this.query.advancedFilters = this.advancedFilters
				},
				deep: true
			},

			'query.advancedFilters': {
				handler()
				{
					if (this.serverMode && this.searchOnNextChange.value)
					{
						this.updateSearch()
					}
					this.$emit('set-search-on-next-change', false)
				},
				deep: true
			},

			'columnFilters': {
				handler()
				{
					this.query.columnFilters = this.columnFilters
				},
				deep: true
			},

			'query.columnFilters': {
				handler()
				{
					if (this.serverMode && this.searchOnNextChange.value)
					{
						this.updateSearch()
					}
					this.$emit('set-search-on-next-change', false)
				},
				deep: true
			},

			'config.columnSizes': {
				handler()
				{
					this.setColumnSizes(this.config.columnSizes)
				}
			},

			'dataImportResponse': {
				handler(newVal)
				{
					if (this.serverMode)
					{
						//Reload table if import is successful
						if (typeof newVal.success !== 'undefined')
						{
							if (newVal.success !== false)
							{
								this.changeQuery()
							}
						}
					}
				},
				deep: true
			},

			activeViewModeId()
			{
				if (this.isListVisible)
				{
					this.$nextTick().then(() => {
						if (this.$refs.tableElem && this.$refs.tableContainerElem)
						{
							//FOR: COLUMN RESIZE
							this.applyColumnResizeable()

							//FOR: COLUMN SIZES
							this.setColumnSizes(this.columnSizes)
						}
					})
				}
			},

			signal: {
				handler(newValue)
				{
					if (newValue.resetColumnSizes !== undefined)
					{
						this.setColumnSizes(null)
					}
					if (newValue.saveCurrentView !== undefined)
					{
						this.saveCurrentView()
					}
				},
				deep: true
			},

			'config': {
				handler()
				{
					this.initConfig()
				},
				deep: true
			},

			'initialSortColumn': {
				handler()
				{
					this.updateInitialSortDisplay()
				}
			},

			'initialSortColumnOrder': {
				handler()
				{
					this.updateInitialSortDisplay()
				}
			},

			'config.multiColumnSort': {
				handler()
				{
					this.resetSort()
				}
			},

			'config.hasRowDragAndDrop': {
				handler(newValue)
				{
					this.hasRowDragAndDrop = newValue
				}
			},

			'hasRowDragAndDrop'(newValue)
			{
				if (newValue) this.initRowsDragAndDrop()
				else this.destroyRowsDragAndDrop()
			},

			'allSelectedRows': {
				handler(newValue) {
					if (newValue === "true")
						this.checkAllRows()
					else
						this.checkNoneRows()
				},
				deep: true,
				immediate: true
			}
		}
	}
</script>
