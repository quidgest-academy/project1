<template>
	<!-- BEGIN: Column Config Popup -->
	<teleport
		:to="`#q-modal-${modalId}-header`"
		:key="domKey"
		v-if="(showPopup || showInline) && showHeader">
		<div>
			<h4 class="c-modal__header-title">{{ texts.configureColumns }}</h4>
		</div>
	</teleport>

	<teleport
		:to="`#q-modal-${modalId}-body`"
		:key="domKey"
		v-if="(showPopup || showInline) && showBody">
		<div class="d-flex">
			<span class="mr-3">{{ texts.lineBreak }}</span>

			<q-toggle-input
				:model-value="hasTextWrap"
				:true-label="hasTextWrap ? texts.yesLabel : texts.noLabel"
				@update:model-value="toggleTextWrap" />
		</div>

		<q-table
			:rows="tableConf.rows"
			:columns="tableConf.columnsCfgCols"
			:config="tableConf.config"
			:total-rows="tableConf.totalRows"
			:header-level="1"
			:texts="texts"
			@update-external="(...args) => updateExternal(...args)"
			@set-row-index-property="(...args) => setRowIndexProperty(tableConf, ...args)"
			@row-reorder="onTableListRowReorder">
		</q-table>

		<div class="visible-columns-counter">
			{{ texts.visibleColumnsText }}: {{ visibleColumns }} {{ texts.ofText.toLowerCase() }} {{ columns.length }}
		</div>
	</teleport>

	<teleport
		:to="`#q-modal-${modalId}-footer`"
		:key="domKey"
		v-if="(showPopup || showInline) && showFooter">
		<div class="actions float-right">
			<q-button
				id="reset-column-config-btn"
				b-style="secondary"
				:label="texts.resetText"
				:title="texts.resetText"
				@click="resetColumnConfig">
				<q-icon icon="reset-definitions" />
			</q-button>

			<q-button
				id="apply-column-config-btn"
				b-style="primary"
				data-modal-close="true"
				data-modal-form="true"
				:label="texts.applyText"
				:title="texts.applyText"
				@click="applyColumnConfig()">
				<q-icon icon="apply" />
			</q-button>

			<q-button
				id="cancel-column-config-btn"
				b-style="secondary"
				data-dismiss="modal"
				:label="texts.cancelText"
				:title="texts.cancelText"
				@click="fnHidePopup()">
				<q-icon icon="cancel" />
			</q-button>
		</div>
	</teleport>
	<!-- END: Column Config Popup -->
</template>

<script>
	import _find from 'lodash-es/find'

	import { inputSize } from '@/mixins/quidgest.mainEnums.js'
	import listFunctions from '@/mixins/listFunctions.js'

	import QTable from './QTable.vue'

	export default {
		name: 'QTableColumnConfig',

		emits: [
			'show-popup',
			'hide-popup',
			'set-property',
			'update-config',
			'apply-column-config',
			'reset-column-config',
			'reset-column-sizes',
			'toggle-text-wrap'
		],

		components: {
			QTable
		},

		inheritAttrs: false,

		props: {
			/**
			 * An object that contains signals to manage the state and behavior of the column config popup.
			 */
			signal: {
				type: Object,
				default: () => ({})
			},

			/**
			 * The unique identifier for the modal that will host the column configuration content.
			 */
			modalId: {
				type: String,
				required: true
			},

			/**
			 * An object with the texts that are used in the popup, allowing for localization of UI elements like titles and buttons.
			 */
			texts: {
				type: Object,
				required: true
			},

			/**
			 * An array of column configuration objects used to toggle visibility and order within the table.
			 */
			columns: {
				type: Array,
				default: () => []
			},

			/**
			 * Determines if the processing and changes to column configurations should depend on server-side functionality.
			 */
			serverMode: {
				type: Boolean,
				default: false
			},

			/**
			 * Indicates whether the table rows should be presented with text wrapping.
			 */
			hasTextWrap: {
				type: Boolean,
				default: false
			},

			/**
			 * The name of the column that has been designated as the default search column.
			 */
			defaultSearchColumnName: {
				type: String,
				required: true
			},

			/**
			 * The base path to the directory containing resources related to the column configuration component.
			 */
			resourcesPath: {
				type: String,
				required: true
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
				tableConf: {
					rows: [],
					columnsCfgCols: [
						{
							label: this.texts.orderText,
							name: 'order',
							dataDisplay: listFunctions.numericDisplayCell,
							dataOnChange: listFunctions.reCalcCellOrder,
							isOrderingColumn: true,
							columnClasses: 'c-table__cell-numeric row-numeric thead-order',
							columnHeaderClasses: 'c-table__head-numeric thead-order',
							component: 'q-edit-numeric',
							componentOptions: {
								maxDigits: 3,
								isDecimal: false,
								readonly: false,
								size: inputSize.mini
							}
						},
						{
							label: this.texts.nameOfColumnText,
							name: 'name',
							dataDisplay: listFunctions.textDisplayCell
						},
						{
							label: this.texts.defaultKeywordSearchText,
							name: 'defaultSearch',
							optionGroupName: 'defaultSearch',
							dataDisplay: listFunctions.radioDisplayCell,
							component: 'q-edit-radio',
							checkedValue: ''
						},
						{
							label: this.texts.visibleText,
							name: 'visibility',
							dataDisplay: listFunctions.booleanDisplayCell,
							component: 'q-edit-boolean',
							rerenderRowsOnNextChange: false
						}
					],
					totalRows: 0,
					config: {
						name: 'column_config',
						tableTitle: '',
						globalSearch: {
							visibility: false
						},
						allowColumnConfig: false,
						showFooter: false,
						perPage: 0,
						hasRowDragAndDrop: true,
						showRowDragAndDropOption: false,
						allowFileExport: false,
						allowFileImport: false,
						resourcesPath: this.resourcesPath
					}
				},
				defaultSearchColumnNameCfg: ''
			}
		},

		mounted() {
			//Set column config rows. Must be done when loading and when table columns change.
			//Can't be a computed property because, the way it is calculated, it will not update when the table columns change.
			this.tableConf.rows = this.getColumnsCfgRows()
			this.tableConf.config.perPage = this.tableConf.rows.length

			this.setDefaultSearchColumnName()
		},

		computed: {
			visibleColumns() {
				return this.tableConf.rows.filter((column) => {
					return column.Fields.visibility
				}).length
			}
		},

		methods: {
			setRowIndexProperty: listFunctions.setRowIndexProperty,

			//Show popup
			fnShowPopup() {
				this.$emit('show-popup', this.modalId)
				this.$nextTick().then(() => {
					this.showPopup = true
					this.domKey++
				})
			},

			//Hide popup
			fnHidePopup() {
				this.$emit('hide-popup', this.modalId)
			},

			/**
			 * Create column config array
			 * @returns Array
			 */
			getColumnsCfgRows() {
				var rows = []

				//Iterate columns
				var thisIdx = 1
				var column = {}
				var colOrder = 1
				for (let idx in this.columns) {
					column = this.columns[idx]
					let columnCfg = {
						Rownum: 0,
						Fields: {}
					}

					//Row value
					columnCfg.Value = column.formField || column.name

					//Column name
					columnCfg.Fields.name = column.label

					//Column order
					columnCfg.Fields.order = colOrder++

					//Column as default search option
					columnCfg.Fields.defaultSearch = false
					if (listFunctions.isSearchableColumn(column)) {
						columnCfg.Fields.defaultSearch = true
					}

					//Column visibility
					columnCfg.Fields.visibility = column.visibility === undefined || column.visibility ? 1 : 0

					//Other fields not displayed
					columnCfg.Fields.primaryKey = column.primaryKey
					columnCfg.Fields.userPrimaryKey = column.userPrimaryKey
					columnCfg.Fields.table = column.table || column.area.toLowerCase()
					columnCfg.Fields.alias = column.alias || columnCfg.Fields.name
					columnCfg.Fields.formField = column.formField || column.name

					//RowKey and Rownum
					columnCfg.rowKey = columnCfg.Rownum = thisIdx++

					rows.push(columnCfg)
				}

				return rows
			},

			/**
			 * Apply the column configuration
			 */
			applyColumnConfig() {
				//Emit data to script which calls apply function
				this.$emit('apply-column-config', { columnOrder: this.tableConf.rows, defaultSearchColumn: this.defaultSearchColumnNameCfg })

				//Hide popup
				this.fnHidePopup()

				this.$emit('update-config')
			},

			/**
			 * Reset the column configuration
			 */
			resetColumnConfig() {
				this.resetColumnSizes()

				//Update internal rows
				//Needed for resetting if changes have not been applied
				this.tableConf.rows = this.getColumnsCfgRows()
				this.tableConf.config.perPage = this.tableConf.rows.length

				//Emit data to script which calls reset function
				this.$emit('reset-column-config')

				//Emit data to script which calls reset function
				this.$emit('reset-column-sizes')

				//Hide popup
				this.fnHidePopup()

				//Clear column size object in table configuration object
				this.$emit('set-property', 'config', 'columnSizes', {})

				this.$emit('update-config')
			},

			/**
			 * Reset the column sizes
			 */
			resetColumnSizes() {
				//Emit data to script which calls reset function
				this.$emit('reset-column-sizes')

				//Hide popup
				this.fnHidePopup()

				//Clear column size object in table configuration object
				this.$emit('set-property', 'config', 'columnSizes', {})

				this.$emit('update-config')
			},

			/**
			 * Set the default search column
			 */
			setDefaultSearchColumnName() {
				var defaultSearchColumn = this.tableConf.columnsCfgCols.find((x) => x.name === 'defaultSearch')
				defaultSearchColumn.checkedValue = this.defaultSearchColumnName
				this.defaultSearchColumnNameCfg = this.defaultSearchColumnName
			},

			/**
			 * Toggle text wrap in cells
			 */
			toggleTextWrap() {
				//Emit toggle text wrap
				this.$emit('toggle-text-wrap')
			},

			/**
			 * Called when updating the value of a cell
			 * @param row {Object}
			 * @param column {Object}
			 * @param value {Object}
			 * @returns
			 */
			updateExternal(row, column, event) {
				if (column.name === 'defaultSearch') {
					this.defaultSearchColumnNameCfg = event
				}
			},

			onTableListRowReorder(eObj) {
				var row = listFunctions.getRowByKeyPath(this.tableConf.rows, eObj?.rowKey),
					orderingColumn = _find(this.tableConf.columnsCfgCols, (col) => col.isOrderingColumn === true)

				listFunctions.setTableCellValue(this.tableConf, row, orderingColumn, eObj.index + 1)
				listFunctions.reCalcCellOrder(this.tableConf, row, orderingColumn)
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
							default:
								if (['showInline', 'showHeader', 'showBody', 'showFooter'].includes(key)) {
									this[key] = newValue[key]
								}
								break
						}
					}
				},
				deep: true
			},

			columns: {
				handler() {
					this.tableConf.rows = this.getColumnsCfgRows()
					this.tableConf.config.perPage = this.tableConf.rows.length
				},
				deep: true
			},

			defaultSearchColumnName: {
				handler() {
					this.setDefaultSearchColumnName()
				},
				deep: true
			}
		}
	}
</script>
