<template>
	<button
		ref="button"
		class="q-btn q-btn--link dropdown b-column-actions"
		type="button"
		:title="texts.columnActionsText"
		:disabled="disabled"
		data-table-action-selected="false"
		tabindex="-1"
		@click="togglePopup"
		@mouseup="onButtonMouseup"
		@mousedown="onButtonMousedown"
		@mouseleave="onButtonMouseleave">
		<q-icon
			v-if="sortIcon"
			:icon="sortIcon" />

		<q-icon
			v-if="hasFilter"
			icon="filter" />
		<q-icon
			v-else-if="!sortDirection"
			icon="expand"
			class="column-dropdown-toggle" />
	</button>

	<!-- BEGIN: Column Filters Popup -->
	<teleport
		v-if="showPopup"
		to="#q-dropdown">
		<div
			ref="popup"
			tabindex="0"
			@keydown="popupOnKeydown">
			<div class="column-filter-form">
				<div
					v-if="allowColumnSort && isSortableColumn(column)"
					class="column-filter-form__btns">
					<!-- BEGIN: Column sorting -->
					<q-button
						b-style="secondary"
						block
						:label="texts.ascendingText"
						:title="texts.sortAscendingText"
						@click="sort('asc')">
						<q-icon icon="sort-ascending" />
					</q-button>

					<q-button
						b-style="secondary"
						block
						:label="texts.descendingText"
						:title="texts.sortDescendingText"
						@click="sort('desc')">
						<q-icon icon="sort-descending" />
					</q-button>
					<!-- END: Column sorting -->
				</div>

				<template v-if="allowColumnFilters && isSearchableColumn(column)">
					<!-- BEGIN: Column filter conditions -->
					<div class="search-filter-conds">
						<label style="display: block"> {{ texts.showRecordsWhereText }}: </label>
						<!-- BEGIN: Conditions -->
						<div
							v-for="(condition, conditionIdx) in editFilter.conditions"
							:key="conditionIdx"
							class="mb-2">
							<div v-if="conditionIdx > 0">
								{{ texts.orText }}
							</div>

							<q-select
								size="block"
								v-model="editFilter.conditions[conditionIdx].operator"
								:items="getFilterOperatorOptions(editFilter, conditionIdx, filterOperators, searchableColumns)"
								item-value="key"
								item-label="value"
								@update:model-value="setFilterDefaultValues(editFilter, conditionIdx, searchableColumns)" />

							<div>
								<component
									:is="getFilterInputComponent(editFilter, conditionIdx, searchableColumns)"
									v-for="(value, valueIdx) in getFilterValueCount(editFilter, conditionIdx, searchableColumns)"
									:key="editFilter.conditions[conditionIdx].field + '_' + valueIdx"
									size="large"
									table-name="Filters"
									:row-index="0"
									:column-name="getFilterColumnFromName(editFilter, conditionIdx, searchableColumns).name"
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
									:value="editFilter.conditions[conditionIdx].values[valueIdx]"
									:raw-value="editFilter.conditions[conditionIdx].values[valueIdx]"
									:placeholder="getFilterPlaceholder(editFilter, conditionIdx, searchableColumns)"
									:error-messages="getValueErrorMessages(valueIdx)"
									@update="setFilterConditionValue(editFilter, conditionIdx, valueIdx, $event)"
									@loaded="checkIfSetPopupFocus(getFilterValueCount(editFilter, conditionIdx, searchableColumns), valueIdx)" />
							</div>
						</div>
						<!-- END: Conditions -->
					</div>
					<!-- END: Column filter conditions -->

					<div class="column-filter-actions">
						<q-button
							b-style="primary"
							size="small"
							:title="texts.activateFilterText"
							@click="saveFilter">
							<q-icon icon="ok" />
						</q-button>

						<q-button
							b-style="secondary"
							size="small"
							:title="texts.deactivateFilterText"
							@click="removeFilter">
							<q-icon icon="remove" />
						</q-button>

						<q-button
							b-style="secondary"
							size="small"
							:title="texts.moveToAdvancedFiltersText"
							@click="moveFilterToAdvancedFilters">
							<q-icon
								icon="advanced-filters"
								class="search-filters-icon" />
						</q-button>
					</div>
				</template>
			</div>
		</div>
	</teleport>
	<!-- END: Column Filters Popup -->
</template>

<script>
	import cloneDeep from 'lodash-es/cloneDeep'
	import findIndex from 'lodash-es/findIndex'
	import isEmpty from 'lodash-es/isEmpty'

	import searchFilterDataModule from '@/api/genio/searchFilterData.js'
	import genericFunctions from '@/mixins/genericFunctions.js'
	import listFunctions from '@/mixins/listFunctions.js'

	export default {
		name: 'QTableColumnFilters',

		emits: [
			'set-property',
			'set-dropdown',
			'update-sort',
			'edit-column-filter',
			'remove-column-filter',
			'add-advanced-filter',
			'show-advanced-filters',
			'update-config'
		],

		inheritAttrs: false,

		props: {
			/**
			 * Object containing localized text strings for sorting and filtering related actions.
			 */
			texts: {
				type: Object,
				required: true
			},

			/**
			 * The title for the menu that appears when interacting with a table's column filters and sorting options.
			 */
			menuTitle: {
				type: String,
				default: ''
			},

			/**
			 * Configuration object representing the column to which the sorting and filtering applies.
			 */
			column: {
				type: Object,
				required: true
			},

			/**
			 * The object representing the current state of sorting and filters applied to the table.
			 */
			query: {
				type: Object,
				default: () => ({})
			},

			/**
			 * Flag indicating whether column filters are allowed in the table.
			 */
			allowColumnFilters: {
				type: Boolean,
				default: false
			},

			/**
			 * Flag indicating whether column sorting is allowed in the table.
			 */
			allowColumnSort: {
				type: Boolean,
				default: false
			},

			/**
			 * An array of columns that are searchable, used for setting up column-specific filters.
			 */
			searchableColumns: {
				type: Array,
				default: () => []
			},

			/**
			 * An object representing the current filter settings for a particular column, if any.
			 */
			filter: {
				type: Object,
				default: () => ({})
			},

			/**
			 * A predefined set of filter operators used to construct the filter conditions.
			 */
			filterOperators: {
				type: Object,
				default: () => searchFilterDataModule.operators.elements
			},

			/**
			 * Flag indicating whether server-side functionality is used for filtering and sorting operations.
			 */
			serverMode: {
				type: Boolean,
				default: false
			},

			/**
			 * The DOM element that wraps the table, used for managing popup positioning and scrolling behaviors.
			 */
			tableContainerElem: {
				type: Object,
				default: null
			},

			/**
			 * A flag indicating whether the table is in a read-only state, which may affect the ability to filter or sort.
			 */
			disabled: {
				type: Boolean,
				default: false
			},

			/**
			 * The name associated with the table, used for fully qualified column naming during filtering and sorting.
			 */
			tableName: {
				type: String,
				default: ''
			}
		},

		expose: [],

		data()
		{
			return {
				showPopup: false,
				buttonMousedown: false,
				editFilter: {},
				tableContainerElement: null,
				validationErrorFieldIndex: []
			}
		},

		mounted()
		{
			// DOM element to add listener to
			this.tableContainerElement = this.tableContainerElem
			this.initFilter()

			// Clear validation errors
			this.validationErrorFieldIndex = []
		},

		beforeUnmount()
		{
			this.fnHidePopup()
		},

		computed: {
			sortDirection()
			{
				const index = findIndex(this.query.sort, {
					vbtColId: this.column.vbtColId
				})

				if (index === -1)
					return null
				return this.query.sort[index].order
			},

			sortIcon()
			{
				return this.sortDirection === null
					? ''
					: this.sortDirection === 'asc'
						? 'sort-ascending'
						: 'sort-descending'
			},

			hasFilter()
			{
				return Object.keys(this.filter).length > 0
			},

			conditionsAreValid()
			{
				return !this.validationErrorFieldIndex.some((value) => !isEmpty(value.message))
			}
		},

		methods: {
			getFilterOperatorOptions: listFunctions.getFilterOperatorOptions,

			/**
			 * Determine if column is sortable
			 * @param column {Object}
			 * @returns Boolean
			 */
			isSortableColumn(column)
			{
				return listFunctions.isSortableColumn(column)
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

			/**
			 * Get column of condition by index
			 * @param {Object} filter
			 * @param {number} conditionIdx : index
			 * @param {Array} searchableColumns
			 * @returns {Object}
			 */
			getFilterColumnFromName(filter, conditionIdx, searchableColumns)
			{
				return listFunctions.getFilterColumnFromName(
					filter,
					conditionIdx,
					searchableColumns
				)
			},

			/**
			 * Get operators for condition by index
			 * @param {Object} filter
			 * @param {number} conditionIdx : index
			 * @returns {Object}
			 */
			getFilterOperators(filter, conditionIdx, searchableColumns)
			{
				return listFunctions.getFilterOperators(
					this.filterOperators,
					filter,
					conditionIdx,
					searchableColumns
				)
			},

			/**
			 * Get number of values for condition by index
			 * @param {Object} filter
			 * @param {number} conditionIdx : index
			 * @returns {Object}
			 */
			getFilterValueCount(filter, conditionIdx, searchableColumns)
			{
				return listFunctions.getFilterValueCount(
					this.filterOperators,
					filter,
					conditionIdx,
					searchableColumns
				)
			},

			/**
			 * Get input component for condition by index
			 * @param {Object} filter
			 * @param {number} conditionIdx : index
			 * @returns {string}
			 */
			getFilterInputComponent(filter, conditionIdx, searchableColumns)
			{
				return listFunctions.getFilterInputComponent(
					this.filterOperators,
					filter,
					conditionIdx,
					searchableColumns
				)
			},

			/**
			 * Get placeholder for condition input by index
			 * @param {Object} filter
			 * @param {number} conditionIdx : index
			 * @returns {string}
			 */
			getFilterPlaceholder(filter, conditionIdx, searchableColumns)
			{
				return listFunctions.getFilterPlaceholder(
					this.filterOperators,
					filter,
					conditionIdx,
					searchableColumns
				)
			},

			/**
			 * Select default operator for condition by index
			 * @param {Object} filter
			 * @param {number} conditionIdx : index
			 */
			setFilterDefaultOperator(filter, conditionIdx, searchableColumns)
			{
				return listFunctions.setFilterDefaultOperator(
					this.filterOperators,
					filter,
					conditionIdx,
					searchableColumns
				)
			},

			/**
			 * Set default values for condition by index
			 * @param {Object} filter
			 * @param {number} conditionIdx : index
			 */
			setFilterDefaultValues(filter, conditionIdx, searchableColumns)
			{
				return listFunctions.setFilterDefaultValues(
					this.filterOperators,
					filter,
					conditionIdx,
					searchableColumns
				)
			},

			/**
			 * Set value of condition by index
			 * @param {Object} filter
			 * @param {number} conditionIdx : index
			 * @param {number} valueIdx : index
			 * @param {object} value : value
			 */
			setFilterConditionValue(filter, conditionIdx, valueIdx, value)
			{
				return listFunctions.setFilterConditionValue(filter, conditionIdx, valueIdx, value)
			},

			/**
			 * Initialize new filter
			 */
			initNewFilter()
			{
				// Create new filter and add condition
				this.editFilter = listFunctions.searchFilter('', true, [])

				listFunctions.searchFilterAddCondition(
					this.editFilter,
					0,
					listFunctions.searchFilterCondition(
						'',
						true,
						listFunctions.columnFullName(this.column),
						'',
						[]
					)
				)
				this.setFilterDefaultOperator(this.editFilter, 0, this.searchableColumns)
			},

			/**
			 * Initialize filter
			 */
			initFilter()
			{
				if (this.allowColumnFilters)
				{
					// Column filter already defined
					if (this.hasFilter)
					{
						// Copy column filter that is already defined
						this.editFilter = cloneDeep(this.filter)
					}
					// Column filter not already defined
					else
					{
						// New column filter
						this.initNewFilter()
					}
				}
			},

			/**
			 * Sort by column
			 * @param {string} order
			 */
			sort(order)
			{
				if (this.sortDirection === order)
					this.$emit('update-sort', this.column, undefined)
				else
					this.$emit('update-sort', this.column, order)

				this.closeFilters()

				this.$emit('update-config')
			},

			/**
			 * Save filter
			 */
			saveFilter()
			{
				// Validate condition
				this.validationErrorFieldIndex = this.getValidationErrorFieldIndex(
					this.editFilter,
					[this.column]
				)

				if (!this.conditionsAreValid)
					return

				// Save filter
				this.$emit(
					'edit-column-filter',
					listFunctions.columnFullName(this.column),
					this.editFilter
				)

				this.closeFilters()

				this.$emit('update-config')
			},

			/**
			 * Remove filter
			 */
			removeFilter()
			{
				// Clear validation errors
				this.validationErrorFieldIndex = []

				// Remove filter
				this.$emit('remove-column-filter', listFunctions.columnFullName(this.column))

				this.closeFilters()

				this.$emit('update-config')
			},

			/**
			 * Move filter to advanced filters
			 */
			moveFilterToAdvancedFilters()
			{
				// Add filter to advanced filters
				this.$emit('add-advanced-filter', this.editFilter)

				// Remove filter
				this.$emit('remove-column-filter', listFunctions.columnFullName(this.column))

				this.closeFilters()

				// Open advanced filters
				this.$emit('show-advanced-filters', -1)

				this.$emit('update-config')
			},

			/**
			 * Get validation error information
			 * @param {Array} filters
			 * @param {Array} columns
			 * @returns {Array}
			 */
			getValidationErrorFieldIndex(filters, columns)
			{
				let validationErrorFieldIndex = []
				let conditionStates = listFunctions.filterValidate(filters, columns)

				if (conditionStates.length > 0)
				{
					let conditionState = conditionStates[0]

					// Iterate values
					for (let valueIdx in conditionState.ValueStates)
					{
						let valueState = conditionState.ValueStates[valueIdx]
						let message = ''

						if (valueState !== 'VALID')
							message = conditionState.Label + ' ' + this.texts.isRequired

						validationErrorFieldIndex.push({
							message: message
						})
					}
				}

				return validationErrorFieldIndex
			},

			/**
			 * Get value field error messages
			 * @param {number} idx
			 * @returns {Array}
			 */
			getValueErrorMessages(idx)
			{
				const messages = this.validationErrorFieldIndex[idx]?.message

				if (isEmpty(messages))
					return []
				return [messages]
			},

			/**
			 * Cancel
			 */
			closeFilters()
			{
				this.fnHidePopup()
			},

			/**
			 * Show popup
			 */
			fnShowPopup()
			{
				// Initialize filter
				this.initFilter()

				// Show dropdown off-screen (in order to get it's dimensions)
				this.showPopup = true
				this.$emit('set-dropdown', genericFunctions.getDropdownPositionOffScreen())

				// Add event listener to window to hide dropdown on window resize
				window.addEventListener('resize', this.resizeListener)

				// These operations must be done after the dropdown has already been displayed (otherwise it doesn't exist yet)
				this.$nextTick().then(() => {
					if (this.getFilterValueCount(this.editFilter, 0, this.searchableColumns) < 1)
						this.focusPopup()
				})
			},

			/**
			 * Set popup position
			 */
			setPopupPosition()
			{
				// These operations must be done after the dropdown has already been displayed (otherwise it doesn't exist yet)
				this.$emit(
					'set-dropdown',
					genericFunctions.getDropdownPosition(
						this.$refs.popup,
						this.$refs.button,
						'left'
					)
				)
			},

			/**
			 * Add popup event listeners
			 */
			addPopupEvents()
			{
				// Add event listener to dropdown for focusout and set focus so that any change of focus to another element will trigger focusout
				// Must be done after showing the dropdown so the dropdown will exist
				this.$refs.popup.addEventListener('focusout', this.dropdownListener)
				this.$refs.popup.focus()

				// Add event listener to table container element to close the dropdown when scrolling
				// Must be done after showing the dropdown so the dropdown will exist
				if (
					this.tableContainerElement !== undefined &&
					this.tableContainerElement !== null
				)
				{
					this.tableContainerElement.addEventListener('scroll', this.scrollListener)
				}
			},

			/**
			 * Set popup position and add event listeners
			 */
			focusPopup()
			{
				this.setPopupPosition()
				this.addPopupEvents()
			},

			/**
			 * Check if last input element and set popup position and add event listeners
			 * @param {number} valueCount
			 * @param {number} index
			 */
			checkIfSetPopupFocus(valueCount, index)
			{
				if (index === valueCount - 1)
					this.focusPopup()
			},

			/**
			 * Hide popup
			 */
			fnHidePopup()
			{
				// Clear validation errors
				this.validationErrorFieldIndex = []

				// Remove event listener from dropdown for focusout
				if (this.$refs.popup !== undefined && this.$refs.popup !== null)
					this.$refs.popup.removeEventListener('focusout', this.dropdownListener)

				// Remove event listener from table container for scroll
				if (
					this.tableContainerElement !== undefined &&
					this.tableContainerElement !== null
				)
				{
					this.tableContainerElement.removeEventListener('scroll', this.scrollListener)
				}

				// Remove event listener from window to hide dropdown on window resize
				window.removeEventListener('resize', this.resizeListener)

				// Hide dropdown
				this.showPopup = false
				this.buttonMousedown = false
				this.$emit('set-dropdown', { isVisible: false })
			},

			/**
			 * Dropdown listener
			 */
			dropdownListener(event)
			{
				if (genericFunctions.dropdownIsFocused(this.$refs.popup, this.$refs.button, event))
				{
					// If focus came from the error popover
					if (
						event.target.classList.contains('btn-popover') &&
						event.explicitOriginalTarget?.nodeName === '#text'
					)
						this.$refs.popup.focus()

					return
				}

				this.fnHidePopup()
			},

			/**
			 * Scroll listener
			 */
			scrollListener()
			{
				this.fnHidePopup()
			},

			/**
			 * Resize listener
			 */
			resizeListener()
			{
				this.fnHidePopup()
			},

			/**
			 * Toggle popup
			 */
			togglePopup()
			{
				if (!this.showPopup)
					this.fnShowPopup()
				else
					this.fnHidePopup()
			},

			/**
			 * Mousemove on toggle button
			 */
			onButtonMouseleave()
			{
				if (this.showPopup && this.buttonMousedown)
					this.fnHidePopup()
			},

			/**
			 * Mousedown on toggle button
			 */
			onButtonMousedown()
			{
				this.buttonMousedown = true
			},

			/**
			 * Mouseup on toggle button
			 */
			onButtonMouseup()
			{
				this.buttonMousedown = false
			},

			/**
			 * Keydown on toggle button
			 * @param event {object} Event object
			 */
			popupOnKeydown(event)
			{
				if(event?.key === 'Escape')
				{
					this.fnHidePopup()
					this.$refs?.button?.focus()
				}
			}
		},

		watch: {
			filters: {
				handler() {
					this.initFilter()
				},
				deep: true
			},

			tableContainerElem: {
				handler() {
					// DOM element to add listener to
					this.tableContainerElement = this.tableContainerElem
				},
				deep: true
			}
		}
	}
</script>
