<template>
	<!-- BEGIN: View Popup -->
	<teleport
		v-if="(showPopup || showInline) && showHeader"
		:to="`#q-modal-${modalId}-header`"
		:key="domKey">
		<div>
			<h2 class="c-modal__header-title">{{ texts.viewManagerText }}</h2>
		</div>
	</teleport>

	<teleport
		v-if="(showPopup || showInline) && showBody"
		:to="`#q-modal-${modalId}-body`"
		:key="domKey">
		<div>
			<div class="d-flex">
				<q-toggle-input
					:model-value="selectedSavedView"
					:false-label="texts.baseTableAsDefault"
					:true-label="texts.savedView"
					@update:model-value="toggleBaseTable" />
			</div>
			<!-- BEGIN: View list -->
			<div ref="viewsTableContainer">
				<q-table
					:rows="viewRows"
					:columns="viewColumns"
					:config="config"
					:total-rows="totalRows"
					:header-level="1"
					@row-action="(emitAction) => viewRowAction(emitAction)"
					@update-external="(...args) => updateExternal(...args)" />
			</div>
			<!-- END: View list -->
		</div>
	</teleport>

	<teleport
		v-if="(showPopup || showInline) && showFooter"
		:to="`#q-modal-${modalId}-footer`"
		:key="domKey">
		<div class="actions float-right">
			<q-button
				b-style="primary"
				:label="texts.applyText"
				:title="texts.applyText"
				@click="setSelectedView">
				<q-icon icon="apply" />
			</q-button>

			<q-button
				b-style="secondary"
				:label="texts.cancelText"
				:title="texts.cancelText"
				@click="fnHidePopup()">
				<q-icon icon="cancel" />
			</q-button>
		</div>
	</teleport>
	<!-- END: View Popup -->
</template>

<script>
	import listFunctions from '@/mixins/listFunctions.js'

	import QTable from './QTable.vue'

	export default {
		name: 'QTableViews',

		emits: ['show-popup', 'hide-popup', 'select-view', 'view-action'],

		components: {
			QTable
		},

		inheritAttrs: false,

		props: {
			/**
			 * Object containing signal data that can be emitted to manipulate the component's behavior.
			 */
			signal: {
				type: Object,
				default: () => ({})
			},

			/**
			 * An object that provides localized text strings to be used within the views modal.
			 */
			texts: {
				type: Object,
				required: true
			},

			/**
			 * The identifier of the modal element the views should be rendered into.
			 */
			modalId: {
				type: String,
				required: true
			},

			/**
			 * A flag indicating whether the table data should be fetched from the server.
			 */
			serverMode: {
				type: Boolean,
				default: false
			},

			/**
			 * An array containing the names of the saved configurations/views.
			 */
			configNames: {
				type: Array,
				default: () => []
			},

			/**
			 * The name of the default saved view configuration.
			 */
			configNameDefault: {
				type: String,
				default: ''
			},

			/**
			 * The path to the directory containing related resource files for the component.
			 */
			resourcesPath: {
				type: String,
				default: ''
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
				viewRows: [],
				viewColumns: [
					{
						label: this.texts.viewNameText,
						name: 'name',
						dataDisplay: this.textDisplayCell
					},
					{
						label: this.texts.defaultViewText,
						name: 'selectedView',
						optionGroupName: 'selectedView',
						dataDisplay: listFunctions.radioDisplayCell,
						component: 'q-edit-radio',
						checkedValue: ''
					}
				],
				totalRows: 0,
				config: {
					name: 'Views',
					tableTitle: '',
					globalSearch: {
						visibility: false
					},
					showFooter: false,
					perPage: 10,
					rowBgColorSelected: '#e0e0e0',
					crudActions: [
						{
							id: 'SHOW',
							name: 'SHOW',
							title: this.texts.viewText,
							icon: {
								icon: 'go-to'
							},
							params: {
								type: 'form',
								formName: 'VIEW',
								mode: 'SHOW'
							}
						},
						{
							id: 'DUPLICATE',
							name: 'DUPLICATE',
							title: this.texts.duplicateText,
							icon: {
								icon: 'duplicate'
							},
							params: {
								type: 'form',
								formName: 'VIEW',
								mode: 'DUPLICATE'
							}
						},
						{
							id: 'DELETE',
							name: 'DELETE',
							title: this.texts.deleteText,
							icon: {
								icon: 'delete'
							},
							params: {
								type: 'form',
								formName: 'VIEW',
								mode: 'DELETE'
							}
						}
					],
					rowActionDisplay: 'inline',
					showRowActionText: false,
					allowFileExport: false,
					allowFileImport: false,
					resourcesPath: this.resourcesPath
				},
				selectedViewNameCfg: ''
			}
		},

		mounted() {
			this.viewRows = this.getRows()
			this.config.perPage = this.viewRows.length
			this.selectView(this.configNameDefault)
		},

		computed: {
			selectedSavedView() {
				return !(this.selectedViewNameCfg === undefined || this.selectedViewNameCfg === null || this.selectedViewNameCfg === '')
			}
		},

		methods: {
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
			 * Called when updating the value of a cell
			 * @param row {Object}
			 * @param column {Object}
			 * @param value {Object}
			 * @returns
			 */
			updateExternal(row, column, event) {
				if (column.name === 'selectedView') {
					this.selectedViewNameCfg = event
				}
			},

			/**
			 * Toggle using base table as default
			 * @returns
			 */
			toggleBaseTable(value) {
				if (value && this.configNames && this.configNames.length > 0) {
					this.selectView(this.configNames[0])
				} else {
					this.selectView('')
				}
			},

			/**
			 * Set view as selected view
			 * @returns
			 */
			setSelectedView() {
				//Emit data to script which calls apply function
				this.$emit('select-view', { name: this.selectedViewNameCfg })

				this.fnHidePopup()
			},

			/**
			 * Create column config array
			 * @returns Array
			 */
			getRows() {
				var rows = []

				//Iterate configNames
				var thisIdx = 1
				var configName = {}
				for (let idx in this.configNames) {
					configName = this.configNames[idx]
					rows.push({ Rownum: thisIdx, rowKey: thisIdx, Value: configName, Fields: { name: configName, selectedView: true } })
					thisIdx++
				}

				return rows
			},

			/**
			 * Select default view in column
			 * @param value {Object}
			 * @returns Array
			 */
			selectView(value) {
				var viewColumn = this.viewColumns.find((x) => x.name === 'selectedView')
				viewColumn.checkedValue = value
				this.selectedViewNameCfg = value

				//No view set as default
				if (value === '') {
					//Set all radio buttons to unchecked (needs to be done this way)
					if (!this.$refs.viewsTableContainer) return
					let selectedViewButtons = this.$refs.viewsTableContainer.querySelectorAll("[name='selectedView']")
					for (let idx in selectedViewButtons) {
						let selectedViewButton = selectedViewButtons[idx]
						if (selectedViewButton.type && selectedViewButton.type === 'radio') {
							selectedViewButton.checked = false
						}
					}
				}
			},

			/**
			 * Called when selecting a row action
			 * @param emitAction {Object}
			 * @returns
			 */
			viewRowAction(emitAction) {
				this.$emit('view-action', emitAction)
				if (emitAction.name && emitAction.name === 'SHOW') {
					this.fnHidePopup()
				}
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

			configNames: {
				handler() {
					this.viewRows = this.getRows()
					this.config.perPage = this.viewRows.length
				},
				deep: true
			},

			configNameDefault: {
				handler() {
					this.selectView(this.configNameDefault)
				},
				deep: true
			}
		}
	}
</script>
