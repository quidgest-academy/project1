<template>
	<!-- BEGIN: Config Popup -->
	<teleport
		v-if="showPopup"
		:to="`#q-modal-${modalId}-header`"
		:key="domKey">
		<div>
			<h4 class="c-modal__header-title">{{ texts.tableConfig }}</h4>
		</div>
	</teleport>

	<teleport
		v-if="showPopup"
		:to="`#q-modal-${modalId}-body`"
		:key="domKey">
		<q-tab-container
			v-bind="tabGroup"
			@mounted="setAllTabsShowContent('tabGroup', true, true)"
			@before-unmount="setAllTabsShowContent('tabGroup', false, true)"
			@tab-changed="changeTab('tabGroup', 'selectedTab', $event)">
			<template #tab-panel>
				<template
					v-for="tab in tabGroup.tabsList"
					:key="tab.id">
					<section v-show="tabGroup.selectedTab === tab.id">
						<div :id="'q-modal-' + tab.id + '-header'"></div>
						<div :id="'q-modal-' + tab.id + '-body'"></div>
					</section>
				</template>
			</template>
		</q-tab-container>
	</teleport>

	<teleport
		v-if="showPopup"
		:to="`#q-modal-${modalId}-footer`"
		:key="domKey">
		<template
			v-for="tab in tabGroup.tabsList"
			:key="tab.id">
			<section v-show="tabGroup.selectedTab === tab.id">
				<div :id="'q-modal-' + tab.id + '-footer'"></div>
			</section>
		</template>
		<div class="actions float-right"></div>
	</teleport>
	<!-- END: Config Popup -->
</template>

<script>
	import { computed } from 'vue'
	import _find from 'lodash-es/find'

	import QTabContainer from '@/components/containers/TabContainer.vue'

	export default {
		name: 'QTableConfig',

		emits: [
			'apply-config',
			'hide-popup',
			'reset-config',
			'set-property',
			'show-popup',
			'signal-component'
		],

		components: {
			QTabContainer
		},

		inheritAttrs: false,

		props: {
			/**
			 * The control object containing configuration details and state for the table.
			 * Used for managing properties such as column configuration and filters.
			 */
			tableCtrl: {
				type: Object,
				required: true
			},

			/**
			 * An object containing signals that can trigger different actions within the configuration modal.
			 * These could include showing or hiding the modal, or navigating between different sections of the configuration.
			 */
			signal: {
				type: Object,
				default: () => ({})
			},

			/**
			 * The identifier for the modal container where the configuration component is rendered.
			 */
			modalId: {
				type: String,
				required: true
			},

			/**
			 * Object containing localized strings for various UI components and labels within the configuration modal.
			 */
			texts: {
				type: Object,
				required: true
			}
		},

		expose: [],

		data() {
			return {
				showPopup: false,
				domKey: 0,

				tabGroup: {
					selectedTab: 'column-config',
					alignTabs: 'left',
					iconAlignment: 'left',
					isVisible: true,
					tabsList: [
						{
							id: 'column-config',
							componentId: 'columnConfig',
							name: 'columns',
							label: this.texts.configureColumns,
							disabled: false,
							isVisible: computed(() => {
								return (
									this.tableCtrl.config.allowColumnConfiguration &&
									_find(this.tableCtrl.config.configOptions, ['id', 'columnConfig'])?.visible
								)
							})
						},
						{
							id: 'advanced-filters',
							componentId: 'advancedFilters',
							name: 'filters',
							label: this.texts.advancedFiltersText,
							disabled: false,
							isVisible: computed(() => {
								return (
									this.tableCtrl.config.allowAdvancedFilters &&
									_find(this.tableCtrl.config.configOptions, ['id', 'advancedFilters'])?.visible
								)
							})
						},
						{
							id: 'view-save',
							componentId: 'viewSave',
							name: 'newView',
							label: this.texts.saveViewText,
							disabled: false,
							isVisible: computed(() => {
								return (
									this.tableCtrl.config.allowManageViews && _find(this.tableCtrl.config.configOptions, ['id', 'viewSave'])?.visible
								)
							})
						},
						{
							id: 'views',
							componentId: 'views',
							name: 'views',
							label: this.texts.viewManagerText,
							disabled: false,
							isVisible: computed(() => {
								return this.tableCtrl.config.allowManageViews && _find(this.tableCtrl.config.configOptions, ['id', 'views'])?.visible
							})
						}
					]
				}
			}
		},

		methods: {
			//Show popup
			fnShowPopup() {
				this.$emit('show-popup', { id: this.modalId, props: { returnElement: this.signal.returnElement } })
				this.$nextTick().then(() => {
					this.showPopup = true
					this.domKey++
				})
			},

			//Hide popup
			fnHidePopup() {
				this.$emit('hide-popup', this.modalId)
			},

			getTab(tab, selectedTab) {
				return _find(this[tab]['tabsList'], (x) => x.id === selectedTab)
			},

			setAllTabsShowContent(tabGroupId, show, mergeProps) {
				for (let tabId in this[tabGroupId]['tabsList']) {
					let tabObj = this[tabGroupId]['tabsList'][tabId]
					this.$emit('signal-component', tabObj.componentId, { showInline: show, showHeader: false }, mergeProps)
				}
			},

			changeTab(tab, tabProp, selectedTab) {
				this[tab][tabProp] = selectedTab
			}
		},

		watch: {
			signal: {
				handler(newValue) {
					if (newValue.show) {
						this.fnShowPopup()
					} else if (newValue.show === false) {
						this.fnHidePopup()
					}
					if (newValue.selectedTab) {
						this.changeTab('tabGroup', 'selectedTab', newValue.selectedTab)
					}
				},
				deep: true
			}
		}
	}
</script>
