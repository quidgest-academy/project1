<template>
	<tr
		ref="rowElem"
		v-bind="$attrs"
		:id="rowId"
		:index="rowIndex"
		:class="rowClasses"
		:style="rowStyles"
		:title="getRowTitle(row)"
		data-testid="table-row"
		data-table-action-selected="false"
		tabindex="-1"
		@click="rowClickAction"
		@keydown="rowOnKeydown"
		@keyup="rowOnKeyup">
		<!-- BEGIN: cell -->
		<template v-for="(column, key, index) in columns">
			<td
				v-if="canShowColumn(column)"
				:key="index"
				:class="cellClasses(column)"
				:style="getCellStyles(column)"
				:title="getCellTitle(column)">
				<slot :name="'vbt-' + getCellSlotName(column)"> </slot>
				<!-- BEGIN: Row drag and drop column -->
				<template v-if="isDragAndDropColumn(column)">
					<slot
						:name="getCellSlotName(column)"
						:row-key="row.rowKey"
						:column="column">
						<div class="c-table__dnd">
							<span
								class="c-table__drag"
								@keydown="allowRowOrderKeys = true"
								@keyup="reorderRowUpDown">
								<q-icon
									icon="row-draggable"
									:key="row.Rownum" />
							</span>
							<q-button-group>
								<q-button
									:id="rowId + '-reorder-up'"
									b-style="tertiary"
									data-table-action-selected="false"
									tabindex="-1"
									@click="reorderRow(-1)">
									<q-icon
										icon="circle-arrow-top"
										:key="row.Rownum" />
								</q-button>
								<q-button
									:id="rowId + '-reorder-down'"
									b-style="tertiary"
									data-table-action-selected="false"
									tabindex="-1"
									@click="reorderRow(1)">
									<q-icon
										icon="circle-arrow-down"
										:key="row.Rownum" />
								</q-button>
								<q-button
									v-if="addAction"
									b-style="tertiary"
									data-table-action-selected="false"
									tabindex="-1"
									@click="addRowAfter">
									<q-icon
										icon="add"
										:key="row.Rownum" />
								</q-button>
							</q-button-group>
						</div>
					</slot>
				</template>
				<!-- END: Row drag and drop column -->
				<!-- BEGIN: Row action column -->
				<template v-else-if="isActionsColumn(column)">
					<slot
						:name="getCellSlotName(column)"
						:row-key="row.rowKey"
						:column="column">
						<q-table-record-actions-menu
							:btn-permission="row.btnPermission"
							:action-visibility="row.actionVisibility"
							:crud-actions="crudActions"
							:custom-actions="customActions"
							:general-actions="generalActionsPlacement === 'left' || generalActionsPlacement === 'right' ? generalActions : null"
							:general-custom-actions="
								generalActionsPlacement === 'left' || generalActionsPlacement === 'right' ? generalCustomActions : null
							"
							:actions-placement="actionsPlacement"
							:readonly="readonly"
							:display="rowActionDisplay"
							:show-row-action-icon="showRowActionIcon"
							:show-general-action-icon="showGeneralActionIcon"
							:show-row-action-text="showRowActionText"
							:show-general-action-text="showGeneralActionText"
							:texts="texts"
							data-table-action-selected="false"
							tabindex="-1"
							@row-action="(emitAction) => emitRowAction(emitAction)" />
					</slot>
				</template>
				<!-- END: Row action column -->
				<!-- BEGIN: Row checklist column -->
				<template v-else-if="isChecklistColumn(column)">
					<slot
						:name="getCellSlotName(column)"
						:row-key="row.rowKey"
						:column="column">
						<q-table-checklist-checkbox
							:value="isRowSelected(row)"
							:table-name="tableName"
							:readonly="readonly"
							:row-key="row.rowKey"
							:disabled="disableCheckbox"
							@click="onSelect"
							@toggle-row-selected="$emit('toggle-row-selected', row.rowKey)" />
					</slot>
				</template>
				<!-- END: Row checklist column -->
				<!-- BEGIN: Extended row action column -->
				<template v-else-if="isExtendedActionsColumn(column)">
					<slot
						:name="getCellSlotName(column)"
						:row="row"
						:column="column">
						<span
							v-if="hasExtendedAction('remove')"
							:key="column.name">
							<a
								:title="texts.removeText"
								data-table-action-selected="false"
								tabindex="-1"
								@click.stop="$emit('remove-row')">
								<q-icon icon="delete" />
							</a>
						</span>
					</slot>
				</template>
				<!-- END: Extended row action column -->
				<!-- BEGIN: Normal data columns -->
				<template v-else>
					<slot
						:name="getCellSlotName(column)"
						:row="row"
						:column="column"
						:cell-value="getValueFromRow(row, column)">
						<!-- If column has tree expand / collapse action, add wrapper element, if not, use v-fragment which adds content but does not add wrapper element -->
						<span
							v-if="hasTreeAction(column)"
							:style="{ 'margin-left': level * 24 + 'px' }">
						</span>

						<q-button
							v-if="hasTreeAction(column)"
							class="tree-expand-btn"
							data-table-action-selected="false"
							tabindex="-1"
							@click="toggleShowChildRows"
							data-testid="tree-action">
							<q-icon
								:icon="showChildren ? collapseIcon : expandIcon"
								:class="['action-item', 'tree-action-item']" />
						</q-button>
						<span
							v-if="hasTreeAction(column) && !hasDataAction(column)"
							style="margin-left: 0.3rem" />

						<!-- If column has action, add wrapper element for adding emit, if not, use v-fragment which adds content but does not add wrapper element -->
						<component
							:is="hasDataAction(column) && getCellDataDisplay(row, column) !== '' ? 'a' : 'v-fragment'"
							href="javascript:void(0)"
							class="column-data-link"
							data-table-action-selected="false"
							tabindex="-1"
							@click.stop.prevent="$emit('cell-action', row, column)">
							<q-render-data
								:component="column.component"
								:value="
									getCellDataDisplay(row, column, {
										useScroll: true,
										outputObject: true
									})
								"
								:background-color="getBackgroundColor(row, column)"
								:raw-value="getValueFromRow(row, column)"
								:table-name="tableName"
								:row-index="rowIndex"
								:column-name="column.name"
								:options="column.componentOptions || column"
								:row="row"
								:key="row.rowKey"
								:resources-path="resourcesPath"
								@update="$emit('update', row, column, $event)"
								@update-external="$emit('update-external', row, column, $event)"
								@execute-action="(...args) => $emit('execute-action', ...args)" />
						</component>
					</slot>
				</template>
				<!-- END: Normal data columns -->
			</td>
		</template>
		<!-- END: cell -->
	</tr>
</template>

<script>
	import { defineAsyncComponent } from 'vue'
	import cloneDeep from 'lodash-es/cloneDeep'
	import has from 'lodash-es/has'
	import includes from 'lodash-es/includes'
	import _find from 'lodash-es/find'

	import QRenderBoolean from '@/components/rendering/QRenderBoolean.vue'
	import QRenderData from '@/components/rendering/QRenderData.vue'
	import QRenderDocument from '@/components/rendering/QRenderDocument.vue'
	import QRenderHyperlink from '@/components/rendering/QRenderHyperlink.vue'
	import QRenderImage from '@/components/rendering/QRenderImage.vue'

	import listFunctions from '@/mixins/listFunctions.js'

	export default {
		name: 'QTableRow',

		emits: [
			'loaded',
			'go-to-row',
			'navigate-row',
			'remove-row',
			'toggle-row-selected',
			'execute-action',
			'cell-action',
			'row-action',
			'row-click',
			'row-reorder',
			'change',
			'update',
			'update-external',
			'toggle-show-children'
		],

		components: {
			QTableRecordActionsMenu: defineAsyncComponent(() => import('@/components/table/QTableRecordActionsMenu.vue')),
			QTableChecklistCheckbox: defineAsyncComponent(() => import('@/components/table/QTableChecklistCheckbox.vue')),
			QRenderBoolean,
			QRenderData,
			QRenderDocument,
			QRenderHyperlink,
			QRenderImage
		},

		inheritAttrs: false,

		props: {
			/**
			 * Data object for the current table row.
			 */
			row: {
				type: Object,
				required: true
			},

			/**
			 * An array of column configuration objects for the table.
			 */
			columns: {
				type: Array,
				default: () => []
			},

			/**
			 * An array representing the key path to the current row, used for hierarchically structured data.
			 */
			rowKeyPath: {
				type: Array,
				required: true
			},

			/**
			 * Application-defined CSS classes for rows, or a method to generate such classes dynamically.
			 */
			propRowClasses: [Object, String],

			/**
			 * Application-defined CSS classes for cells, or a method to generate such classes dynamically.
			 */
			propCellClasses: [Object, String],

			/**
			 * A unique identifier for the row, often matching the primary key from the data source.
			 */
			uniqueId: {
				type: [Number, String],
				required: true
			},

			/**
			 * The index of the row within the current set of table data.
			 */
			rowIndex: {
				type: [Number, String],
				required: true
			},

			/**
			 * Flag indicating whether the row is in a valid state; this might be determined by data validation logic.
			 */
			isValid: {
				type: Boolean,
				default: true
			},

			/**
			 * A dynamic title for the row or a static string; used for tooltips or accessibility.
			 */
			rowTitle: {
				type: [Function, String],
				default: ''
			},

			/**
			 * A dynamic text color for the row or a static string; can be used to style rows conditionally.
			 */
			textColor: {
				type: [Function, String],
				default: ''
			},

			/**
			 * A dynamic background color for the row or a static string; used for conditional styling.
			 */
			bgColor: {
				type: [Function, String],
				default: ''
			},

			/**
			 * A specified background color to use for selected rows.
			 */
			bgColorSelected: {
				type: String,
				default: ''
			},

			/**
			 * A boolean indicating whether the row is selected, which can be used to style or perform actions on the row.
			 */
			rowSelectedForGroup: {
				type: Boolean,
				default: false
			},

			/**
			 * Tooltip text for cells within the row, typically based on the content or state of the cell.
			 */
			cellTitles: {
				type: Object,
				default: () => ({})
			},

			/**
			 * An array of default CRUD actions that can be performed on the row.
			 */
			crudActions: {
				type: Array,
				default: () => []
			},

			/**
			 * An array of custom-defined actions that can be performed on the row.
			 */
			customActions: {
				type: Array,
				default: () => []
			},

			/**
			 * An array of general actions that can be performed at the table level rather than on specific rows.
			 */
			generalActions: {
				type: Array,
				default: () => []
			},

			/**
			 * An array of custom-defined general actions available at the table level.
			 */
			generalCustomActions: {
				type: Array,
				default: () => []
			},

			/**
			 * Determines the display style for row actions ('dropdown', 'inline', etc.).
			 */
			rowActionDisplay: {
				type: String,
				default: 'dropdown'
			},

			/**
			 * Determines the placement of action buttons within the row ('left', 'right', etc.).
			 */
			actionsPlacement: {
				type: String,
				default: 'left'
			},

			/**
			 * Determines the placement of general action buttons in relation to the table ('below', 'above', etc.).
			 */
			generalActionsPlacement: {
				type: String,
				default: 'below'
			},

			/**
			 * Flag indicating if icons for row actions should be displayed.
			 */
			showRowActionIcon: {
				type: Boolean,
				default: true
			},

			/**
			 * Flag indicating if icons for general actions should be shown.
			 */
			showGeneralActionIcon: {
				type: Boolean,
				default: true
			},

			/**
			 * Flag indicating if text labels for row actions should be visible.
			 */
			showRowActionText: {
				type: Boolean,
				default: true
			},

			/**
			 * Flag indicating if text labels for general actions should be displayed.
			 */
			showGeneralActionText: {
				type: Boolean,
				default: true
			},

			/**
			 * Custom CSS classes to be applied to action buttons.
			 */
			actionClasses: {
				type: Object,
				default: () => ({})
			},

			/**
			 * Flag indicating if additional base classes should be applied to action buttons.
			 */
			enableActionButtonBaseClasses: {
				type: Boolean,
				default: true
			},

			/**
			 * The name of the table; used in various operations like reactivity and slot naming.
			 */
			tableName: {
				type: String,
				default: ''
			},

			/**
			 * Localized text strings to be used within the component (for labels, headers, etc.).
			 */
			texts: {
				type: Object,
				required: true
			},

			/**
			 * Flag indicating if the overall table is in a read-only state.
			 */
			readonly: {
				type: Boolean,
				default: false
			},

			/**
			 * Flag indicating the availability of drag-and-drop row reordering.
			 */
			hasRowDragAndDrop: {
				type: Boolean,
				default: false
			},

			/**
			 * Configuration for the sorting order column, if applicable.
			 */
			sortOrderColumn: {
				type: [Object, String],
				default: () => ({})
			},

			/**
			 * Hierarchical level of the row, used in tree structures to represent nesting.
			 */
			level: {
				type: Number,
				default: 0
			},

			/**
			 * Icon to use for indicating the ability to expand a row's nested content.
			 */
			expandIcon: {
				type: String,
				default: 'square-plus'
			},

			/**
			 * Icon to use for indicating the ability to collapse a row's nested content.
			 */
			collapseIcon: {
				type: String,
				default: 'square-minus'
			},

			/**
			 * Icon to represent an unassigned state in a tree structure; displayed when expand/collapse is not applicable.
			 */
			emptySquareIcon: {
				type: String,
				default: 'square'
			},

			/**
			 * Flag indicating whether the row's children should be displayed by default.
			 */
			showChildRows: {
				type: Boolean,
				default: false
			},

			/**
			 * Flag indicating whether the checkbox in the row should be disabled.
			 */
			disableCheckbox: {
				type: Boolean,
				default: false
			},

			/**
			 * Specifies a row key to scroll into view; can be used to bring a specific row into focus programmatically.
			 */
			rowKeyToScroll: {
				type: [String, Array],
				default: ''
			},

			/**
			 * Path for the resources.
			 */
			resourcesPath: {
				type: String,
				required: true
			},

			/**
			 * Specifies the index of the row that is navigated to.
			 * Can be a mulit-index which is the indexes for each level (in tree tables) separated by underscores.
			 */
			navigatedRowKeyPath: {
				type: Array,
				default: () => []
			}
		},

		expose: ['toggleShowChildRows'],

		data() {
			return {
				rowSelected: false,
				showChildren: false,
				allowRowOrderKeys: true
			}
		},

		inject: [
			'getValueFromRow',
			'getCellSlotName',
			'canShowColumn',
			'isSortableColumn',
			'isActionsColumn',
			'isExtendedActionsColumn',
			'isChecklistColumn',
			'isDragAndDropColumn',
			'getRowClasses',
			'getRowTitle',
			'rowIsValid',
			'hasExtendedAction',
			'hasDataAction',
			'getCellDataDisplay',
			'getRowCellDataTitles',
			'isRowSelected',
			'rowWithoutChildren'
		],

		mounted() {
			//Signal that the component is loaded
			this.$emit('loaded')

			this.showChildren = this.showChildRows

			//FOR: select row on return
			this.expandFromRowKeyPath(this.rowKeyToScroll)
			if(this.isLastInRowKeyPath(this.rowKeyToScroll))
				this.$emit('go-to-row', this.rowKeyToScroll, this.rowId)

			//FOR: navigate to row on return
			this.expandFromRowKeyPath(this.navigatedRowKeyPath)
			if(this.isLastInRowKeyPath(this.navigatedRowKeyPath))
				this.$emit('navigate-row', this.rowIndex)
		},

		computed: {
			rowId() {
				return this.tableName + '_row-' + this.rowIndex
			},

			rowClasses() {
				let classes = ['c-table__row']

				//Row selected
				if (this.rowSelected) classes.push('vbt-row-selected')

				classes.push(this.userRowClasses)

				if (this.row.isHighlighted) classes.push('c-table__row--highlight')

				if (this.row.isNavigated === true) classes.push('c-table__row--navigated')

				return classes
			},

			//Row classes passed in by propRowClasses prop
			userRowClasses() {
				let classes = ''

				if (typeof this.propRowClasses === 'string') {
					return this.propRowClasses
				} else if (typeof this.propRowClasses === 'object') {
					Object.entries(this.propRowClasses).forEach(([key, value]) => {
						if (typeof value === 'boolean' && value) {
							classes += key
						} else if (typeof value === 'function') {
							let truth = value(this.row)
							if (typeof truth === 'boolean' && truth) {
								classes += ' '
								classes += key
							}
						}
					})
				}

				return classes
			},

			/**
			 * Get styles for row
			 * @returns String
			 */
			rowStyles() {
				var rowStyles = {}

				//Don't apply styles for rows with invalid state
				if (this.isValid === false) {
					return rowStyles
				}

				//Row text color
				if (this.textColor) {
					if (typeof this.textColor === 'string' && this.textColor !== '') {
						rowStyles['color'] = this.textColor
					} else if (typeof this.textColor === 'function') {
						rowStyles['color'] = this.textColor(this.row)
					}
				}

				//Row background color
				if (this.bgColor) {
					if (typeof this.bgColor === 'string' && this.bgColor !== '') {
						rowStyles['background-color'] = this.bgColor
					} else if (typeof this.bgColor === 'function') {
						rowStyles['background-color'] = this.bgColor(this.row)
					}
				}

				//Row selected background color
				if (this.rowSelectedForGroup !== false) {
					rowStyles['background-color'] = this.bgColorSelected
				}

				return rowStyles
			},

			/**
			 * Determine if row has child rows
			 * @returns String
			 */
			isRowHasChild() {
				return this.row.hasChildren === true
			},

			addAction() {
				return _find(this.generalActions, (act) => act.id === 'insert')
			}
		},

		methods: {
			//CSS classes for cell
			/**
			 * Get CSS classes for column
			 * @param column {Object}
			 * @returns String
			 */
			cellClasses(column) {
				let classes = []

				//BEGIN: Text alignment class
				let alignments = ['text-justify', 'text-right', 'text-left', 'text-center']

				//Undefined data type, use rowTextAlignment
				if (has(column, 'rowTextAlignment') && includes(alignments, column.rowTextAlignment)) {
					classes.push(column.rowTextAlignment)
				}
				//END: Text alignment class

				//Adding user defined classes from column config to cells
				if (has(column, 'columnClasses')) {
					classes.push(column.columnClasses)
				}

				//Cell classes passed in by propCellClasses prop
				if (typeof this.propCellClasses === 'string') {
					return this.propCellClasses
				} else if (typeof this.propCellClasses === 'object') {
					Object.entries(this.propCellClasses).forEach(([key, value]) => {
						if (typeof value === 'boolean' && value) {
							classes.push(key)
						} else if (typeof value === 'function') {
							let truth = value(this.row, column, this.getValueFromRow(this.row, column.name))
							if (typeof truth === 'boolean' && truth) {
								classes.push(key)
							}
						}
					})
				}

				return classes
			},

			/**
			 * Get text for title attribute of cell content
			 * @param column {Object}
			 * @returns String
			 */
			getCellTitle(column) {
				const cellTitle = this.cellTitles[column.name]

				if (!cellTitle) return null
				else if (this.isDragAndDropColumn(column) || (column.scrollData !== undefined && cellTitle.length > column.scrollData))
					return cellTitle
				return null
			},

			/**
			 * Get styles for cell
			 * @param column {Object}
			 * @returns String
			 */
			getCellStyles(column) {
				var cellStyles = {}

				//Don't apply styles for rows with invalid state
				if (this.isValid === false) {
					return cellStyles
				}

				//Cell text color
				if (column.textColor) {
					if (typeof column.textColor === 'string' && column.textColor !== '') {
						cellStyles['color'] = column.textColor
					} else if (typeof column.textColor === 'function') {
						cellStyles['color'] = column.textColor(this.row, column)
					}
				}

				return cellStyles
			},

			/**
			 * Emit for row click action
			 * @returns
			 */
			rowClickAction() {
				this.$emit('row-click', this.row)
			},

			/**
			 * Row keydown handler
			 * @param event {object} Event object
			 */
			rowOnKeydown(event) {
				const key = event?.key

				switch(key)
				{
					case "Insert":
						// Insert new record
						this.$emit('row-action', { id: 'insert', rowKeyPath: this.rowKeyPath ?? [this.rowKey] })
						event.preventDefault()
						break
					case "Delete":
						// Prevent if not focused on the row element
						if(event.target.tagName !== 'TR')
							break
						// Delete record
						this.$emit('row-action', { id: 'delete', rowKeyPath: this.rowKeyPath ?? [this.rowKey] })
						event.preventDefault()
						break
				}
			},

			/**
			 * Row keyup handler
			 * @param event {object} Event object
			 */
			rowOnKeyup(event) {
				const key = event?.key

				switch(key)
				{
					case 'Enter':
						// Trigger only from the row and cell elements.
						if(event?.target.tagName === 'TR' || event?.target.tagName === 'TD')
							this.$emit('row-click', this.row)
						break
				}
			},

			/**
			 * Determine if column has expand and collapse control
			 * @param column {Object}
			 * @returns String
			 */
			hasTreeAction(column) {
				if (column.hasTreeShowHide !== undefined) {
					return column.hasTreeShowHide
				}
				return false
			},

			/**
			 * Toggle showing child rows
			 * @returns String
			 */
			toggleShowChildRows() {
				this.showChildren = !this.showChildren
				this.$emit('toggle-show-children', { show: this.showChildren, row: this.row })
			},

			/**
			 * FOR: select row on return
			 * Show sub-rows if this row is in a row key path to expand
			 * @param rowKeyPath {Array}
			 * @returns
			 */
			isLastInRowKeyPath(rowKeyPath) {
				// If this row is the last row in the row key path
				if (Array.isArray(rowKeyPath)) {
					if (this.row.rowKey === rowKeyPath[this.level] && this.level === rowKeyPath.length - 1)
						return true
				}
				// If the row key path is one row key as a string
				else if (this.row.rowKey === rowKeyPath)
					return true
			},

			/**
			 * FOR: navigate to row on return
			 * Show sub-rows if this row is in a row key path to expand
			 * @param rowKeyPath {Array}
			 * @returns
			 */
			expandFromRowKeyPath(rowKeyPath) {
				// If this row is not the last row in the row key path, expand it
				if (Array.isArray(rowKeyPath)) {
					if (this.row.rowKey === rowKeyPath[this.level] && this.level < rowKeyPath.length - 1) {
						this.showChildren = true
						this.$emit('toggle-show-children', { row: this.row, show: this.showChildren })
					}
				}
			},

			/**
			 * Cell background color
			 * @param row {Object}
			 * @param column {Object}
			 * @returns String
			 */
			getBackgroundColor(row, column) {
				if (column.bgColor) {
					if (typeof column.bgColor === 'string' && column.bgColor !== '') {
						return column.bgColor
					} else if (typeof column.bgColor === 'function') {
						return column.bgColor(row, column)
					}
				}
			},

			/**
			 * Emit row action
			 * @param emitAction {Object}
			 * @returns Boolean
			 */
			emitRowAction(emitAction) {
				if (this.row.Value !== undefined && this.row.Value !== null)
					emitAction.rowValue = this.row.Value

				if (this.rowKeyPath !== undefined && this.rowKeyPath !== null)
					emitAction.rowKeyPath = this.rowKeyPath

				if (this.row.rowKey !== undefined && this.row.rowKey !== null)
					emitAction.rowKey = this.row.rowKey

				this.$emit('row-action', emitAction)
			},

			/**
			 * Reorder row one up or down
			 * @param shift {Number}
			 * @returns
			 */
			reorderRow(shift) {
				var shiftValue = parseInt(shift)
				//Update column value
				this.$emit('row-reorder', { row: this.row, sortOrderColumn: this.sortOrderColumn, shiftValue })
			},

			/**
			 * Reorder row one up or down
			 * @param e {Object}
			 * @returns
			 */
			reorderRowUpDown(e) {
				var shiftValue = 0
				//Key pressed: tab
				//Must disable ordering keys right after keyup if it's the tab key
				//in order to avoid reordering rows when using shift+tab to tab backwards through elements
				if (e.keyCode === 9) {
					this.allowRowOrderKeys = false
					return
				}
				//Key pressed: left, up, delete
				else if (this.allowRowOrderKeys && (e.keyCode === 37 || e.keyCode === 38 || e.keyCode === 46)) {
					shiftValue = -1
				}
				//Key pressed: right, down, shift
				else if (this.allowRowOrderKeys && (e.keyCode === 39 || e.keyCode === 40 || e.keyCode === 16)) {
					shiftValue = 1
				}

				//Update column value
				this.reorderRow(shiftValue)
			},

			/**
			 * Add new row after this row
			 * @returns
			 */
			addRowAfter() {
				var addNewAction = cloneDeep(this.addAction)
				if (!addNewAction) return

				const sortColumnName = `${this.sortOrderColumn.area.toLowerCase()}.${this.sortOrderColumn.field.toLowerCase()}`
				addNewAction.params.prefillValues = {}
				addNewAction.params.prefillValues[sortColumnName] =
					parseInt(listFunctions.getCellValue(this.row, this.sortOrderColumn)) + 1
				this.emitRowAction({ action: addNewAction })
			},

			onSelect(event)
			{
				event.stopPropagation()
			}
		},

		watch: {
			row() {
				this.showChildren = this.row.showChildRows
				this.$emit('toggle-show-children', { row: this.row, show: this.showChildren })
			},

			showChildRows(newValue) {
				this.showChildren = newValue
				this.$emit('toggle-show-children', { row: this.row, show: this.showChildren })
			}
		}
	}
</script>
