<template>
	<tr
		:class="classes"
		v-bind="$attrs"
		:index="rowIndex">
		<slot
			name="columns"
			:columns="columns">
			<template
				v-for="(column, key, index) in columns"
				:key="column.name">
				<th
					v-if="canShowColumn(column)"
					:key="index"
					:class="columnClasses(column)"
					:data-column-name="column.name"
					@mousedown="onColumnMouseDown()"
					@mousemove="onColumnMouseMove()"
					@mouseup="onColumnMouseUp()">
					<!-- BEGIN: FOR: TABLE LIST ROW ACTIONS -->
					<div
						v-if="isActionsColumn(column) || isDragAndDropColumn(column)"
						class="column-header-content">
						<q-icon icon="actions" />
					</div>
					<!-- END: FOR: TABLE LIST ROW ACTIONS -->
					<!-- BEGIN: Checklist header cell content -->
					<div
						v-else-if="isChecklistColumn(column)"
						class="column-header-content">
						<slot
							:name="'column_' + getCellSlotName(column)"
							:column="column">
							<q-table-selector
								:texts="texts"
								:readonly="readonly"
								:disable-selector="rowCount < 1"
								@check-all-rows="$emit('check-all-rows')"
								@check-current-page-rows="$emit('check-current-page-rows')"
								@check-none-rows="$emit('check-none-rows')" />
						</slot>
					</div>
					<!-- END: Checklist header cell content -->
					<!-- BEGIN: Extended row action column -->
					<div
						v-else-if="isExtendedActionsColumn(column)"
						class="extended-row-header">
						<slot
							:name="getCellSlotName(column)"
							:column="column">
							<span
								v-if="hasExtendedAction('remove-reset')"
								:key="column.name">
								<q-button
									b-style="secondary"
									:title="texts.resetText"
									data-table-action-selected="false"
									tabindex="-1"
									@click="$emit('unselect-all-rows')">
									<q-icon icon="reset" />
								</q-button>
							</span>
						</slot>
					</div>
					<!-- END: Extended row action column -->
					<!-- BEGIN: Header cell content -->
					<div
						v-else
						class="column-header-content">
						<!-- BEGIN: Header cell title -->
						<div class="column-header-text">
							<slot
								:name="'column_' + getCellSlotName(column)"
								:column="column">
								{{ column.label }}
							</slot>
							<div v-if="(allowColumnFilters && isSearchableColumn(column)) || (allowColumnSort && isSortableColumn(column))">
								<q-table-column-filters
									:column="column"
									:query="query"
									:texts="texts"
									:allow-column-filters="allowColumnFilters"
									:allow-column-sort="allowColumnSort"
									:searchable-columns="searchableColumns"
									:filter="filters[columnFullName(column)]"
									:table-container-elem="tableContainerElem"
									:filter-operators="filterOperators"
									:disabled="disabled"
									:table-name="tableName"
									@set-dropdown="(...args) => $emit('set-dropdown', ...args)"
									@set-property="(...args) => $emit('set-property', ...args)"
									@update-sort="(...args) => $emit('update-sort', ...args)"
									@edit-column-filter="(...args) => $emit('edit-column-filter', ...args)"
									@remove-column-filter="(...args) => $emit('remove-column-filter', ...args)"
									@add-advanced-filter="(...args) => $emit('add-advanced-filter', ...args)"
									@show-advanced-filters="(...args) => $emit('show-advanced-filters', ...args)" />
							</div>
						</div>
						<!-- END: Header cell title -->
					</div>
					<!-- END: Header cell content -->
				</th>
			</template>
		</slot>
	</tr>
</template>

<script>
	import has from 'lodash-es/has'
	import includes from 'lodash-es/includes'

	import searchFilterDataModule from '@/api/genio/searchFilterData'

	import QTableColumnFilters from './QTableColumnFilters.vue'
	import QTableSelector from './QTableSelector.vue'

	export default {
		name: 'QTableHeader',

		emits: [
			'column-resize',
			'update-sort',
			'unselect-all-rows',
			'set-property',
			'set-dropdown',
			'edit-column-filter',
			'remove-column-filter',
			'add-advanced-filter',
			'show-advanced-filters',
			'check-all-rows',
			'check-current-page-rows',
			'check-none-rows'
		],

		components: {
			QTableColumnFilters,
			QTableSelector
		},

		inheritAttrs: false,

		props: {
			/**
			 * Localized text strings to be used within the table header component.
			 */
			texts: {
				type: Object,
				required: true
			},

			/**
			 * An array containing column configurations, each object defines a column's properties in the table.
			 */
			columns: {
				type: Array,
				default: () => []
			},

			/**
			 * The object representing the current state of sorting and filtering applied to the table.
			 */
			query: {
				type: Object,
				default: () => ({})
			},

			/**
			 * The unique name associated with the table instance.
			 */
			tableName: {
				type: String,
				default: ''
			},

			/**
			 * Flag indicating whether the table is currently in read-only mode.
			 */
			readonly: {
				type: Boolean,
				default: false
			},

			/**
			 * Flag indicating whether checkboxes should be presented in each row.
			 */
			checkboxRows: {
				type: Boolean,
				default: false
			},

			/**
			 * Flag indicating whether filters are allowed on table columns.
			 */
			allowColumnFilters: {
				type: Boolean,
				default: false
			},

			/**
			 * Flag indicating whether sorting is allowed on table columns.
			 */
			allowColumnSort: {
				type: Boolean,
				default: false
			},

			/**
			 * An array of columns that can be used for search filtering.
			 */
			searchableColumns: {
				type: Array,
				default: () => []
			},

			/**
			 * The details of existing filters currently applied on the table columns.
			 */
			filters: {
				type: Object,
				default: () => ({})
			},

			/**
			 * A predefined set of operator definitions used in filter conditions.
			 */
			filterOperators: {
				type: Object,
				default: () => searchFilterDataModule.operators.elements
			},

			/**
			 * Flag indicating whether server-side processing is used for table operations like sorting and filtering.
			 */
			serverMode: {
				type: Boolean,
				default: false
			},

			/**
			 * The DOM element that wraps the table, used for managing the positioning of the table header.
			 */
			tableContainerElem: {
				type: Object,
				default: null
			},

			/**
			 * The total count of rows in the table.
			 */
			rowCount: {
				type: Number,
				default: 0
			},

			/**
			 * Whether the header content is disabled.
			 */
			disabled: {
				type: Boolean,
				default: false
			},

			/**
			 * The row index. Can be a multi-index which has the index for each level (in tree tables) separated by underscores.
			 */
			rowIndex: {
				type: String,
				default: 'h'
			},

			/**
			 * Object with properties for the state:
			 * isNavigated : Indicate whether the header is navigated to (for keyboard and mouse operations).
			 */
			headerRow: {
				type: Object,
				default: () => ({
					isNavigated: false
				})
			},
		},

		expose: [],

		data() {
			return {
				selectAllRows: false,
				mouseDown: false,
				mouseMove: false
			}
		},

		inject: [
			'getCellSlotName',
			'canShowColumn',
			'isSortableColumn',
			'isSearchableColumn',
			'isActionsColumn',
			'isChecklistColumn',
			'isDragAndDropColumn',
			'isExtendedActionsColumn',
			'hasExtendedAction',
			'columnFullName'
		],

		computed: {
			classes()
			{
				const classes = []

				if (this.headerRow?.isNavigated)
					classes.push('c-table__row--navigated')

				return classes
			}
		},

		methods: {
			/**
			 * Determine if rows are sorted by this column? (built-in method)
			 * @param column {Object}
			 * @returns Boolean
			 */
			isSort(column) {
				if (this.query.sort.name === undefined || this.query.sort.name === null) {
					return false
				}

				return this.query.sort.name === column.name
			},

			/**
			 * Get CSS classes for this column
			 * @param column {Object}
			 * @returns String
			 */
			columnClasses(column) {
				let classes = []

				//Decide text alignment class
				let alignments = ['text-justify', 'text-right', 'text-left', 'text-center']
				if (has(column, 'columnTextAlignment') && includes(alignments, column.columnTextAlignment)) {
					classes.push(column.columnTextAlignment)
				}

				//Adding user defined classes to rows
				if (has(column, 'columnHeaderClasses')) {
					classes.push(column.columnHeaderClasses)
				}

				return classes.join(' ')
			},

			/**
			 * Fired on mouse down on header element
			 */
			onColumnMouseDown() {
				this.mouseDown = true
				this.mouseMove = false
			},

			/**
			 * Fired on mouse move on header element
			 */
			onColumnMouseMove() {
				this.mouseMove = true
			},

			/**
			 * Fired on mouse up on header element
			 */
			onColumnMouseUp() {
				if (this.mouseDown && this.mouseMove) {
					this.$emit('column-resize')
				}
				this.mouseDown = false
				this.mouseMove = false
			}
		}
	}
</script>
