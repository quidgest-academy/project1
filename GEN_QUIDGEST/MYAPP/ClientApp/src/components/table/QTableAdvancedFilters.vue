<template>
	<!-- BEGIN: Advanced Filters Popup -->
	<teleport
		:to="`#q-modal-${modalId}-header`"
		:key="domKey"
		v-if="(showPopup || showInline) && showHeader">
		<div>
			<h2 class="c-modal__header-title">{{ texts.advancedFiltersText }}</h2>
		</div>
	</teleport>

	<teleport
		:to="`#q-modal-${modalId}-body`"
		:key="domKey"
		v-if="(showPopup || showInline) && showBody">
		<div
			v-for="(editFilter, filterIdx) in editFilters"
			:key="filterIdx"
			:id="'filter_' + filterIdx">
			<hr v-if="filterIdx > 0" />
			<div :class="['search-filter-form', { 'selected-filter': selectedFilterIdx === filterIdx }]">
				<!-- BEGIN: Edit filter -->
				<!-- BEGIN: Filter name, state, delete -->
				<div>
					<span>
						<base-input-structure
							:id="'filter_' + filterIdx + '_state'"
							class="d-inline-flex valign-top">
							<q-toggle-input
								:id="'filter_' + filterIdx + '_state'"
								v-model="editFilter.active"
								:true-label="texts.activeText"
								:false-label="texts.inactiveText"
								style="display: inline" />
						</base-input-structure>
					</span>
				</div>
				<!-- END: Filter name, state, delete -->
				<div class="search-filter-conds no-gutters">
					<!-- BEGIN: Conditions -->
					<div
						v-for="(condition, conditionIdx) in editFilter.conditions"
						:key="conditionIdx"
						class="row no-gutters mb-2">
						<div class="col-12">
							<label v-if="conditionIdx === 0">{{ texts.createFilterText }}</label>
							<label v-else>{{ texts.orText }}</label>
						</div>
						<div class="filter-input-container">
							<q-select
								v-model="editFilter.conditions[conditionIdx].field"
								size="large"
								:items="searchableColumnOptions"
								@update:model-value="setFilterDefaultOperator(editFilter, conditionIdx, searchableColumns)" />
						</div>

						<div class="filter-input-container">
							<q-select
								v-model="editFilter.conditions[conditionIdx].operator"
								size="large"
								:items="getFilterOperatorOptions(editFilter, conditionIdx, filterOperators, searchableColumns)"
								@update:model-value="setFilterDefaultValues(editFilter, conditionIdx, searchableColumns)" />
						</div>

						<div class="filter-input-container">
							<component
								:is="getFilterInputComponent(editFilter, conditionIdx, searchableColumns)"
								v-for="(value, valueIdx) in getFilterValueCount(editFilter, conditionIdx, searchableColumns)"
								:key="editFilter.conditions[conditionIdx].field + '_' + valueIdx"
								:table-name="tableName + '_filters'"
								:row-index="filterIdx"
								:column-name="conditionIdx + '_' + valueIdx"
								:options="{
									...getFilterColumnFromName(editFilter, conditionIdx, searchableColumns),
									...{ keyIsValue: true },
									component: 'grid-base-input-structure',
									errorDisplayType: 'text'
								}"
								:classes="[
									getFilterColumnFromName(editFilter, conditionIdx, searchableColumns).currency !== undefined
										? ''
										: 'filter-input-field'
								]"
								:container-classes="['filter-value-container']"
								size="large"
								:value="editFilter.conditions[conditionIdx].values[valueIdx]"
								:raw-value="editFilter.conditions[conditionIdx].values[valueIdx]"
								:placeholder="getFilterPlaceholder(editFilter, conditionIdx, searchableColumns)"
								:error-messages="getFilterValueErrorMessages(filterIdx, conditionIdx, valueIdx)"
								@update="setFilterConditionValue(editFilter, conditionIdx, valueIdx, $event)">
							</component>
						</div>

						<div class="filter-input-container">
							<q-button
								b-style="secondary"
								:title="texts.removeConditionText"
								:disabled="editFilter.conditions.length < 2"
								@click="removeCondition(editFilter, conditionIdx)">
								<q-icon icon="remove" />
							</q-button>
						</div>
					</div>
					<!-- END: Conditions -->
				</div>
				<div class="actions">
					<q-button
						b-style="secondary"
						:label="texts.createConditionText"
						:title="texts.createConditionText"
						@click="addCondition(editFilter, editFilter.conditions.length, searchableColumns)">
						<q-icon icon="add" />
					</q-button>
					<q-button
						v-if="mode !== 'new'"
						b-style="secondary"
						:title="texts.deleteFilterText"
						:label="texts.deleteFilterText"
						@click="
							removeFilter(filterIdx);
							fnHidePopup()
						">
						<q-icon icon="remove" />
					</q-button>
				</div>
				<!-- END: Edit filter -->
			</div>
		</div>
	</teleport>

	<teleport
		:to="`#q-modal-${modalId}-footer`"
		:key="domKey"
		v-if="(showPopup || showInline) && showFooter">
		<div class="actions float-right">
			<q-button
				b-style="primary"
				:title="mode === 'new' ? texts.applyFilterText : texts.applyFiltersText"
				:label="mode === 'new' ? texts.applyFilterText : texts.applyFiltersText"
				@click="
					saveFilters(editFilters);
					validationErrorFieldIndex.length === 0 ? fnHidePopup() : null
				">
				<q-icon icon="ok" />
			</q-button>

			<q-button
				v-if="mode === 'editAll'"
				b-style="secondary"
				:title="texts.deleteFiltersText"
				:label="texts.deleteFiltersText"
				@click="
					removeFilters(editFilters);
					fnHidePopup()
				">
				<q-icon icon="remove" />
			</q-button>

			<q-button
				b-style="secondary"
				@click="fnHidePopup()"
				:title="texts.cancelText"
				:label="texts.cancelText">
				<q-icon icon="cancel" />
			</q-button>
		</div>
	</teleport>
	<!-- END: Advanced Filters Popup -->
</template>

<script>
	import cloneDeep from 'lodash-es/cloneDeep'

	import searchFilterDataModule from '@/api/genio/searchFilterData.js'
	import listFunctions from '@/mixins/listFunctions.js'

	export default {
		name: 'QTableAdvancedFilters',

		emits: [
			'show-popup',
			'hide-popup',
			'set-property',
			'update-config',
			'add-advanced-filter',
			'edit-advanced-filter',
			'set-advanced-filter-state',
			'set-advanced-filter-states',
			'deactivate-all-advanced-filters',
			'remove-advanced-filter'
		],

		inheritAttrs: false,

		props: {
			/**
			 * An object containing signals that can trigger different actions within the component.
			 */
			signal: {
				type: Object,
				default: () => ({})
			},

			/**
			 * The identifier for the modal element that wraps the advanced filters popup.
			 */
			modalId: {
				type: String,
				required: true
			},

			/**
			 * Localized text strings to display within the popup, such as buttons and titles.
			 */
			texts: {
				type: Object,
				required: true
			},

			/**
			 * The name of the table or data set these filters are targeting.
			 */
			tableName: {
				type: String,
				required: true
			},

			/**
			 * An array of column definitions which the advanced filters can be applied to.
			 */
			columns: {
				type: Array,
				default: () => []
			},

			/**
			 * An array of filter objects that represents the advanced filters' current configuration.
			 */
			filters: {
				type: Array,
				default: () => []
			},

			/**
			 * A set of predefined filter operators used in building the filter conditions (e.g., 'contains', 'equals').
			 */
			filterOperators: {
				type: Object,
				default: () => searchFilterDataModule.operators.elements
			},

			/**
			 * The mode of the popup, which can be 'new' for adding new filters or 'edit' for modifying existing filters.
			 */
			mode: {
				type: String,
				default: 'new'
			}
		},

		expose: [],

		data() {
			return {
				showPopup: false,
				showInline: false,
				showHeader: true,
				showBody: true,
				showFooter: true,
				domKey: 0,
				editFilters: [],
				selectedFilterIdx: null,
				rowsChecked: {},
				validationErrorFieldIndex: []
			}
		},

		mounted() {
			this.updateFilters()
		},

		computed: {
			searchableColumns() {
				return listFunctions.getSearchableColumns(this.columns)
			},

			searchableColumnOptions() {
				return listFunctions.getSearchableColumnOptions(this.searchableColumns)
			},

			validationErrorMessages() {
				return this.validationErrorFieldIndex.map((x) => x.message)
			}
		},

		methods: {
			getFilterOperatorOptions: listFunctions.getFilterOperatorOptions,

			//Show popup
			fnShowPopup() {
				this.$emit('show-popup', this.modalId)
				this.$nextTick().then(() => {
					this.showPopup = true
					this.domKey++
					this.initValidation()
					if (this.mode === 'new') {
						this.selectNewFilter()
					}
				})
			},

			//Hide popup
			fnHidePopup() {
				this.$emit('hide-popup', this.modalId)
			},

			/**
			 * Full column name
			 * @param {object} column
			 */
			columnFullName(column) {
				return listFunctions.columnFullName(column)
			},

			/**
			 * Get column of condition by index
			 * @param {Object} filter
			 * @param {number} conditionIdx : index
			 * @param {Array} searchableColumns
			 * @returns {Object}
			 */
			getFilterColumnFromName(filter, conditionIdx, searchableColumns) {
				return listFunctions.getFilterColumnFromName(filter, conditionIdx, searchableColumns)
			},

			/**
			 * Get operators for condition by index
			 * @param {Object} filter
			 * @param {number} conditionIdx : index
			 * @returns {Object}
			 */
			getFilterOperators(filter, conditionIdx, searchableColumns) {
				return listFunctions.getFilterOperators(this.filterOperators, filter, conditionIdx, searchableColumns)
			},

			/**
			 * Get number of values for condition by index
			 * @param {Object} filter
			 * @param {number} conditionIdx : index
			 * @returns {Object}
			 */
			getFilterValueCount(filter, conditionIdx, searchableColumns) {
				return listFunctions.getFilterValueCount(this.filterOperators, filter, conditionIdx, searchableColumns)
			},

			/**
			 * Get input component for condition by index
			 * @param {Object} filter
			 * @param {number} conditionIdx : index
			 * @returns {string}
			 */
			getFilterInputComponent(filter, conditionIdx, searchableColumns) {
				return listFunctions.getFilterInputComponent(this.filterOperators, filter, conditionIdx, searchableColumns)
			},

			/**
			 * Get placeholder for condition input by index
			 * @param {Object} filter
			 * @param {number} conditionIdx : index
			 * @returns {string}
			 */
			getFilterPlaceholder(filter, conditionIdx, searchableColumns) {
				return listFunctions.getFilterPlaceholder(this.filterOperators, filter, conditionIdx, searchableColumns)
			},

			/**
			 * Select default operator for condition by index
			 * @param {Object} filter
			 * @param {number} conditionIdx : index
			 */
			setFilterDefaultOperator(filter, conditionIdx, searchableColumns) {
				return listFunctions.setFilterDefaultOperator(this.filterOperators, filter, conditionIdx, searchableColumns)
			},

			/**
			 * Set default values for condition by index
			 * @param {Object} filter
			 * @param {number} conditionIdx : index
			 */
			setFilterDefaultValues(filter, conditionIdx, searchableColumns) {
				return listFunctions.setFilterDefaultValues(this.filterOperators, filter, conditionIdx, searchableColumns)
			},

			/**
			 * Set value of condition by index
			 * @param {Object} filter
			 * @param {number} conditionIdx : index
			 * @param {number} valueIdx : index
			 * @param {object} value : value
			 */
			setFilterConditionValue(filter, conditionIdx, valueIdx, value) {
				return listFunctions.setFilterConditionValue(filter, conditionIdx, valueIdx, value)
			},

			/**
			 * Set filter data in internal property for editing
			 */
			updateFilters() {
				if (this.mode === 'new') {
					this.selectNewFilter()
				} else {
					this.editFilters = cloneDeep(this.filters)
				}
			},

			/**
			 * Initialize new filter
			 */
			initNewFilter() {
				this.editFilters = []
				//Create new filter
				this.editFilters.push(listFunctions.searchFilter('', true, []))
				//Add first condition (each filter must have at least 1 condition)
				this.addCondition(this.editFilters[0], 0, this.searchableColumns)
			},

			/**
			 * Save filter
			 * @param {Object} filter
			 * @param {number} selectedFilterIdx
			 * @param {boolean} active : active state
			 */
			saveFilter(filter, selectedFilterIdx, active) {
				//If adding a new filter
				if (selectedFilterIdx === undefined || selectedFilterIdx === null) {
					selectedFilterIdx = this.filters.length
				}

				//Set filter state to active or inactive
				if (active !== undefined) {
					filter.active = active
				}
				//If active, add filter table key to checklist
				if (filter.active !== false) {
					this.rowsChecked[selectedFilterIdx.toString()] = true
				}

				//If adding new filter
				if (this.mode === 'new') {
					this.$emit('add-advanced-filter', filter)
					this.selectNewFilter()
				}
				//If saving existing filter
				else {
					this.$emit('edit-advanced-filter', [filter, selectedFilterIdx])
				}

				this.$emit('update-config')
			},

			/**
			 * Save filters
			 * @param {Array} filters
			 */
			saveFilters(filters) {
				//Validate filters
				this.validationErrorFieldIndex = this.getValidationErrorFieldIndex(filters, this.columns)
				//Not all filters are valid, don't save
				if (this.validationErrorFieldIndex.length > 0) {
					return
				}
				//Filters are valid, save
				for (let idx = 0; idx < filters.length; idx++) {
					let filter = filters[idx]
					this.saveFilter(filter, idx)
				}
			},

			/**
			 * Set advanced filter state
			 * @param {number} selectedFilterIdx : index
			 * @param {boolean} active : active state
			 */
			setFilterState(selectedFilterIdx, active) {
				//Add filter table key to checklist
				if (active !== false) {
					this.rowsChecked[selectedFilterIdx.toString()] = true
				} else {
					if (this.rowsChecked[selectedFilterIdx.toString()] !== undefined) {
						delete this.rowsChecked[selectedFilterIdx.toString()]
					}
				}

				//Set filter state
				this.$emit('set-advanced-filter-state', [selectedFilterIdx, active])

				this.selectNewFilter()
			},

			/**
			 * Set multiple advanced filter states
			 * @param {Array} selectedFilterIdxObjs : index
			 * @param {boolean} active : active state
			 */
			setMultipleFilterStates(selectedFilterIdxObjs, active) {
				var selectedFilterIdxs = []
				//Add all filter table keys
				for (let key in selectedFilterIdxObjs) {
					selectedFilterIdxs.push(parseInt(key))
					if (active !== false) {
						this.rowsChecked[key] = true
					} else {
						if (this.rowsChecked[key] !== undefined) {
							delete this.rowsChecked[key]
						}
					}
				}

				//Set filter states
				this.$emit('set-advanced-filter-states', [selectedFilterIdxs, active])

				this.selectNewFilter()
			},

			/**
			 * Get checked filters
			 * @param {Object} filter array
			 */
			getCheckedFilters(filters) {
				var checkedFilters = {}

				//Add all active filter table keys
				var filter = {}
				for (let idx in filters) {
					filter = filters[idx]
					if (filter.active !== false) {
						checkedFilters[idx.toString()] = true
					}
				}

				return checkedFilters
			},

			/**
			 * Update filter checklist
			 */
			updateFilterChecklist() {
				//Clear checklist
				this.rowsChecked = {}

				//Add all active filter table keys to checklist
				this.rowsChecked = this.getCheckedFilters(this.filters)
			},

			/**
			 * Set all advanced filter states to inactive
			 */
			deactivateAllFilters() {
				//Remove all filters table keys from checklist
				for (let key in this.rowsChecked) {
					if (this.rowsChecked[key] !== undefined) {
						delete this.rowsChecked[key]
					}
				}

				//Remove all filters
				this.$emit('deactivate-all-advanced-filters')

				this.selectNewFilter()
			},

			/**
			 * Remove advanced filter
			 * @param {number} selectedFilterIdx : index
			 */
			removeFilter(selectedFilterIdx) {
				if (selectedFilterIdx === undefined || selectedFilterIdx === null) {
					return
				}

				//Remove filter table key from checklist
				if (this.rowsChecked[selectedFilterIdx.toString()] !== undefined) {
					delete this.rowsChecked[selectedFilterIdx.toString()]
				}

				//Remove filter
				this.$emit('remove-advanced-filter', selectedFilterIdx)

				//Remake checklist so no checkboxes are mismatched
				this.updateFilterChecklist()

				this.$emit('update-config')
			},

			/**
			 * Remove filters
			 * @param {Array} filters
			 */
			removeFilters(filters) {
				for (let idx = filters.length - 1; idx >= 0; idx--) {
					this.removeFilter(idx)
				}
			},

			/**
			 * Add condition at index
			 * @param {Object} filter
			 * @param {number} conditionIdx : index
			 * @param {Array} searchableColumns
			 */
			addCondition(filter, conditionIdx, searchableColumns) {
				listFunctions.searchFilterAddCondition(
					filter,
					conditionIdx,
					listFunctions.searchFilterCondition('', true, listFunctions.columnFullName(this.searchableColumns[0]), '', [])
				)
				this.setFilterDefaultOperator(filter, conditionIdx, searchableColumns)
			},

			/**
			 * Remove condition at index
			 * @param {Object} filter
			 * @param {number} conditionIdx : index
			 */
			removeCondition(filter, conditionIdx) {
				listFunctions.searchFilterRemoveCondition(filter, conditionIdx)
			},

			/**
			 * Get filters as rows for table
			 * @param {Array} filters : filters
			 * @returns {Array}
			 */
			getFilterRows(filters) {
				var filterRows = []
				var filterRow = {}
				var filtersLength = filters.length
				for (let x = 0; x < filtersLength; x++) {
					filterRow = filters[x]
					filterRows.push({ Rownum: x, Fields: filterRow, rowKey: x })
				}
				return filterRows
			},

			/**
			 * Get formatted string representing filter name for a table cell.
			 * @param table {Object}
			 * @param row {Object}
			 * @param column {Object}
			 * @param options {Object}
			 * @returns String
			 */
			filterNameDisplayCell(table, row, column, options) {
				var value = listFunctions.textDisplayCell(table, row, column, options)
				if (value.length > 0) {
					return value
				}
				return listFunctions.getFilterName(this.filterOperators, row.Fields, this.searchableColumns, this.texts.orText)
			},

			/**
			 * Select filter to edit
			 * @param {string} rowKey : rowKey
			 * @returns {string}
			 */
			selectFilter(rowKey) {
				this.rowsSelected = {}
				this.rowsSelected[rowKey] = true
				this.editFilter = cloneDeep(this.filters[rowKey])
				this.selectedFilterIdx = rowKey

				this.$nextTick().then(() => {
					//Scroll to filter controls
					let filterForm = document.getElementById('filter_' + this.selectedFilterIdx)
					if (filterForm) {
						filterForm.scrollIntoView({ block: 'end', inline: 'nearest' })
					}
				})
			},

			/**
			 * Unselect filters and set new filter as filter to edit
			 * @returns {string}
			 */
			selectNewFilter() {
				this.selectedFilterIdx = null
				this.rowsSelected = {}
				this.initNewFilter()
			},

			/**
			 * Custom actions
			 * @param {Object} emitAction
			 * @returns {string}
			 */
			executeAction(emitAction) {
				if (emitAction.id === 'insert') {
					this.selectNewFilter()
				}
			},

			/**
			 * Initialize validation
			 */
			initValidation() {
				this.validationErrorFieldIndex = []
			},

			/**
			 * Get validation error information
			 * @param {Array} filters
			 * @param {Array} columns
			 * @returns {Array}
			 */
			getValidationErrorFieldIndex(filters, columns) {
				let validationErrorFieldIndex = []
				let conditionStates = []
				//Iterate filters
				for (let filterIdx = 0; filterIdx < filters.length; filterIdx++) {
					let filter = filters[filterIdx]
					conditionStates = listFunctions.filterValidate(filter, columns)
					//Iterate filter conditions
					for (let conditionIdx in conditionStates) {
						let conditionState = conditionStates[conditionIdx]
						if (conditionState.State !== 'VALID') {
							//Iterate values
							for (let valueIdx in conditionState.ValueStates) {
								let valueState = conditionState.ValueStates[valueIdx]
								if (valueState !== 'VALID') {
									validationErrorFieldIndex.push({
										elemId: this.tableName + '_filters_' + filterIdx + '_' + conditionIdx + '_' + valueIdx,
										fieldType: conditionState.Type,
										message: conditionState.Label + ' ' + this.texts.isRequired
									})
								}
							}
						}
					}
				}
				return validationErrorFieldIndex
			},

			/**
			 * Get value field error messages
			 * @param {number} filterIdx
			 * @param {number} conditionIdx
			 * @param {number} valueIdx
			 * @returns {Array}
			 */
			getFilterValueErrorMessages(filterIdx, conditionIdx, valueIdx) {
				let errors = this.validationErrorFieldIndex.filter(
					(error) => error.elemId === this.tableName + '_filters_' + filterIdx + '_' + conditionIdx + '_' + valueIdx
				)
				return errors.map((error) => error.message)
			},

			/**
			 * Focus on a field by error index
			 * @param {string} id
			 */
			focusErrorField(id) {
				let fieldInfo = this.validationErrorFieldIndex[id]
				if (!fieldInfo) return

				let elemId = fieldInfo.elemId
				if (fieldInfo.fieldType === 'Date') {
					elemId = 'dp-input-' + elemId
				}
				let elem = document.getElementById(elemId)
				if (!elem) return

				elem.focus()
			}
		},

		watch: {
			signal: {
				handler(newValue) {
					for (let key in newValue) {
						switch (key) {
							case 'show':
								if (newValue.show) {
									this.fnShowPopup()
								}
								break
							case 'selectedFilterIdx':
								if (newValue.selectedFilterIdx !== null) {
									if (newValue.selectedFilterIdx < 0) {
										this.selectFilter(this.filters.length + newValue.selectedFilterIdx)
									} else {
										this.selectFilter(newValue.selectedFilterIdx)
									}
								}
								break
							default:
								if (['showInline', 'showHeader', 'showBody', 'showFooter'].includes(key)) {
									this[key] = newValue[key]
								}
								break
						}
					}
					if (!this.editFilters) {
						this.selectNewFilter()
					}
				},
				deep: true
			},

			searchableColumns: {
				handler() {
					if (this.mode === 'new') {
						this.selectNewFilter()
					}
				},
				deep: true
			},

			filters: {
				handler(newValue) {
					this.updateFilters()

					this.filterRows = this.getFilterRows(newValue)
					this.totalRows = this.filterRows.length

					this.updateFilterChecklist()
				},
				deep: true
			},

			showInline: {
				handler(newValue) {
					if (newValue === true) {
						this.initValidation()
					}
				},
				deep: true
			}
		}
	}
</script>
