<template>
	<!-- BEGIN: Button view -->
	<q-button-group v-if="display !== 'dropdown'">
		<!-- BEGIN: CRUD action links -->
		<q-table-actions
			v-if="numArrayVisibleActions(allowedCrudActions, readonly) >= 1 && (display === 'inline' || display === 'inlineAll')"
			v-bind="$attrs"
			b-style="tertiary"
			:actions="allowedCrudActions"
			:enable-actions="enableRowActions"
			:readonly="readonly"
			:show-action-text="false"
			:separator-class="separatorClass"
			@action-click="executeActionRow" />
		<!-- END: CRUD action links -->

		<!-- BEGIN: Custom action links -->
		<q-table-actions
			v-if="
				numArrayVisibleActions(visibleCustomActions, readonly) >= 1 &&
					(display === 'inline' || display === 'inlineAll' || display === 'mixed')
			"
			v-bind="$attrs"
			b-style="tertiary"
			:actions="visibleCustomActions"
			:enable-actions="enableRowActions"
			:readonly="readonly"
			:show-action-text="false"
			:separator-class="separatorClass"
			@action-click="executeActionRow" />
		<!-- END: Custom action links -->

		<!-- BEGIN: General action links -->
		<q-table-actions
			v-if="numArrayVisibleActions(allowedGeneralActions, readonly) >= 1 && (display === 'inline' || display === 'inlineAll')"
			v-bind="$attrs"
			b-style="secondary"
			:actions="allowedGeneralActions"
			:enable-actions="enableGeneralActions"
			:readonly="readonly"
			:show-action-text="showGeneralActionText"
			:separator-class="separatorClass"
			@action-click="executeActionRow" />
		<!-- END: General action links -->

		<!-- BEGIN: General custom action links -->
		<q-table-actions
			v-if="
				numArrayVisibleActions(generalCustomActions, readonly) >= 1 &&
					(display === 'inline' || display === 'inlineAll' || display === 'mixed')
			"
			v-bind="$attrs"
			b-style="secondary"
			:actions="generalCustomActions"
			:enable-actions="enableGeneralActions"
			:readonly="readonly"
			:show-action-text="showGeneralActionText"
			:separator-class="separatorClass"
			@action-click="executeActionRow" />
		<!-- END: General custom action links -->
	</q-button-group>
	<!-- END: Button view -->

	<!-- BEGIN: Dropdown view -->
	<div
		v-if="numDropdownVisibleActions > 1"
		:class="[classDropDown]">
		<q-toggle-dropdown
			ref="optionsButton"
			v-bind="$attrs"
			data-testid="options-btn"
			data-boundary="window"
			aria-expanded="false"
			:class="['row-action-btn']"
			:title="texts.actionMenuTitle"
			@toggle-dropdown="toggleDropdown">
			<q-icon icon="more-items" />
		</q-toggle-dropdown>

		<div
			:class="['dropdown-menu', { 'dropdown-menu-right': dropdownAlignment === 'right' }]"
			role="menu">
			<template v-if="isDropdownClicked">
				<!-- BEGIN: Custom action links -->
				<q-table-actions
					v-if="numArrayVisibleActions(visibleCustomActions, readonly) >= 1 && display === 'dropdown'"
					borderless
					show-action-text
					:actions="visibleCustomActions"
					:enable-actions="enableRowActions"
					:readonly="readonly"
					:separator-class="separatorClass"
					@action-click="executeActionRow" />
				<!-- END: Custom action links -->

				<hr
					v-if="
						numArrayVisibleActions(visibleCustomActions, readonly) >= 1 &&
							numArrayVisibleActions(allowedCrudActions, readonly) >= 1 &&
							display === 'dropdown'
					"
					class="dropdown-divider" />

				<!-- BEGIN: CRUD action links -->
				<q-table-actions
					v-if="numArrayVisibleActions(allowedCrudActions, readonly) >= 1 && (display === 'dropdown' || display === 'mixed')"
					borderless
					show-action-text
					:actions="allowedCrudActions"
					:enable-actions="enableRowActions"
					:readonly="readonly"
					:separator-class="separatorClass"
					@action-click="executeActionRow" />
				<!-- END: CRUD action links -->

				<!-- BEGIN: General custom action links -->
				<q-table-actions
					v-if="numArrayVisibleActions(visibleGeneralCustomActions, readonly) >= 1 && display === 'dropdown'"
					borderless
					show-action-text
					:actions="visibleGeneralCustomActions"
					:enable-actions="enableGeneralActions"
					:readonly="readonly"
					:separator-class="separatorClass"
					@action-click="executeActionRow" />
				<!-- END: General custom action links -->

				<!-- BEGIN: General action links -->
				<q-table-actions
					v-if="numArrayVisibleActions(allowedGeneralActions, readonly) >= 1 && (display === 'dropdown' || display === 'mixed')"
					borderless
					show-action-text
					:actions="allowedGeneralActions"
					:enable-actions="enableGeneralActions"
					:readonly="readonly"
					:separator-class="separatorClass"
					@action-click="executeActionRow" />
				<!-- END: General action links -->
			</template>
		</div>
	</div>
	<template v-else-if="display === 'dropdown'">
		<q-table-actions
			v-if="numVisibleActions === 1"
			v-bind="$attrs"
			borderless
			:actions="[singleVisibleAction]"
			:enable-actions="singleVisibleAction.isInReadOnly"
			:show-action-text="displaySingleVisibleActionText"
			:separator-class="separatorClass"
			@action-click="executeActionRow" />
	</template>
	<!-- END: Dropdown view -->
</template>

<script>
	import _map from 'lodash-es/map'

	import { btnHasPermission } from '@/mixins/genericFunctions.js'
	import { numArrayVisibleActions } from '@/mixins/listFunctions.js'

	import QToggleDropdown from '@/components/QToggleDropdown.vue'
	import QTableActions from './QTableActions.vue'

	export default {
		name: 'QTableRecordActionsMenu',

		emits: ['row-action'],

		inheritAttrs: false,

		components: {
			QTableActions,
			QToggleDropdown
		},

		props: {
			/**
			 * An object containing permissions for various row action buttons to determine their active/disabled state.
			 */
			btnPermission: {
				type: Object,
				default: () => ({
					editBtnDisabled: false,
					viewBtnDisabled: false,
					deleteBtnDisabled: false,
					insertBtnDisabled: false
				})
			},

			/**
			 * An object containing the visibility state (shown/hidden) of all row specific custom actions.
			 */
			actionVisibility: {
				type: Object,
				default: () => {}
			},

			/**
			 * An array of default CRUD action configurations for the table.
			 */
			crudActions: {
				type: Array,
				default: () => []
			},

			/**
			 * An array of custom action configurations defined for the table records.
			 */
			customActions: {
				type: Array,
				default: () => []
			},

			/**
			 * An array of general action configurations for the table component.
			 */
			generalActions: {
				type: Array,
				default: () => []
			},

			/**
			 * An array of custom general actions defined at the table level, as opposed to specific rows.
			 */
			generalCustomActions: {
				type: Array,
				default: () => []
			},

			/**
			 * Property to determine the positioning of action buttons within a row (e.g., 'left', 'right').
			 */
			actionsPlacement: {
				type: String,
				default: 'left'
			},

			/**
			 * Determines if the table is in a read-only mode that affects all row actions.
			 */
			readonly: {
				type: Boolean,
				default: false
			},

			/**
			 * Controls the presentation style of action buttons, can be 'inline', 'dropdown', or 'mixed'.
			 */
			display: {
				type: String,
				default: 'dropdown'
			},

			/**
			 * Determines if row action icons should be shown.
			 */
			showRowActionIcon: {
				type: Boolean,
				default: true
			},

			/**
			 * Determines if general action icons are displayed.
			 */
			showGeneralActionIcon: {
				type: Boolean,
				default: true
			},

			/**
			 * Determines if the text labels for row actions should be visible when displayed 'inline' or 'inlineAll'.
			 */
			showRowActionText: {
				type: Boolean,
				default: true
			},

			/**
			 * Determines if the text labels for general actions should be visible.
			 */
			showGeneralActionText: {
				type: Boolean,
				default: true
			},

			/**
			 * An object containing localized strings used for action button titles and other text content.
			 */
			texts: {
				type: Object,
				required: true
			},

			/**
			 * Flag representing the enabled state of row actions. When true, all row actions are interactable.
			 */
			enableRowActions: {
				type: Boolean,
				default: true
			},

			/**
			 * Flag representing the enabled state of general actions. When true, all general actions are interactable.
			 */
			enableGeneralActions: {
				type: Boolean,
				default: true
			},

			/**
			 * The direction of the dropdown menu for actions when 'display' is set to 'dropdown' or 'mixed'.
			 */
			dropdownDirection: {
				type: String,
				default: ''
			},

			/**
			 * The alignment of the dropdown menu relative to the button or other triggering element, can be 'left' or 'right'.
			 */
			dropdownAlignment: {
				type: String,
				default: 'left'
			}
		},

		expose: [],

		data() {
			return {
				/**
				 * State of the dropdown to allow the menu items to be initialized
				 * only after the dropdown button is clicked
				 */
				isDropdownClicked: false
			}
		},

		computed: {
			/**
			 * The allowed CRUD actions on the row
			 */
			allowedCrudActions() {
				return _map(this.crudActions, (action) => {
					return {
						...action,
						disabled: !this.hasPermission(action.id)
					}
				})
			},

			/**
			 * The allowed General actions on the table
			 */
			allowedGeneralActions() {
				return _map(this.generalActions, (action) => {
					return {
						...action,
						disabled: !this.hasPermission(action.id)
					}
				})
			},

			visibleCustomActions() {
				return _map(this.customActions, (action) => {
					return {
						...action,
						isVisible: this.actionVisibility ? this.actionVisibility[action.id] : true
					}
				})
			},

			visibleGeneralCustomActions() {
				return _map(this.generalCustomActions, (action) => {
					return {
						...action,
						isVisible: action.visibleCondition ? action.visibleCondition() : true
					}
				})
			},

			/**
			 * Determine total number of actions that are visible
			 * (also accounting for read-only mode)
			 */
			numVisibleActions() {
				if (typeof this.numArrayVisibleActions !== 'function') return 0
				return (
					this.numArrayVisibleActions(this.allowedCrudActions, this.readonly) +
					this.numArrayVisibleActions(this.visibleCustomActions, this.readonly) +
					this.numArrayVisibleActions(this.allowedGeneralActions, this.readonly) +
					this.numArrayVisibleActions(this.visibleGeneralCustomActions, this.readonly)
				)
			},

			numDropdownVisibleActions() {
				if (this.display === 'inline' || this.display === 'inlineAll') return 0
				else if (this.display === 'mixed')
					return (
						this.numArrayVisibleActions(this.allowedCrudActions, this.readonly) +
						this.numArrayVisibleActions(this.allowedGeneralActions, this.readonly)
					)
				return this.numVisibleActions
			},

			/**
			 * Determine which row has single action that is visible (also accounting for read-only mode)
			 */
			arrayWithSingleVisibleAction() {
				if (this.numVisibleActions === 1) {
					if (this.numArrayVisibleActions(this.allowedCrudActions, this.readonly) === 1) return this.allowedCrudActions
					if (this.numArrayVisibleActions(this.visibleCustomActions, this.readonly) === 1) return this.visibleCustomActions
					if (this.numArrayVisibleActions(this.allowedGeneralActions, this.readonly) === 1) return this.allowedGeneralActions
					if (this.numArrayVisibleActions(this.visibleGeneralCustomActions, this.readonly) === 1) return this.generalCustomActions
				}
				return null
			},

			/**
			 * Determine the action that is visible when only one is (also accounting for read-only mode)
			 */
			singleVisibleAction() {
				if (this.arrayWithSingleVisibleAction === null) return null
				return this.arrayWithSingleVisibleAction[this.firstArrayVisibleActionIndex(this.arrayWithSingleVisibleAction)]
			},

			/**
			 * Determines whether the text of the single visible action should be displayed or not.
			 */
			displaySingleVisibleActionText() {
				if (this.singleVisibleAction === null) return false
				return this.singleVisibleAction.icon === null || this.singleVisibleAction.icon === undefined
			},

			/**
			 * Determines the direction of the dropdown menu.
			 */
			classDropDown() {
				if (this.dropdownDirection) return this.dropdownDirection

				if (this.actionsPlacement === 'right') return 'dropleft'
				return 'dropdown'
			},

			/**
			 * Determines the class for the separator
			 */
			separatorClass() {
				if (this.display === 'dropdown') return 'dropdown-divider'
				return 'action-sep'
			}
		},

		methods: {
			numArrayVisibleActions,

			/**
			 * Handler for the dropdown button click
			 */
			toggleDropdown() {
				this.isDropdownClicked = true
			},

			/**
			 * Checks if the user has permission to execute the specified action.
			 * @param {string} actionType The action type
			 * @returns True if the user has permission, false otherwise.
			 */
			hasPermission(actionType) {
				return btnHasPermission(this.btnPermission, actionType)
			},

			/**
			 * Get index of first visible action (also accounting for read-only mode)
			 * @param actionArray {Array}
			 */
			firstArrayVisibleActionIndex(actionArray) {
				//Has no actions
				if (this.numArrayVisibleActions(actionArray, this.readonly) === 0) return -1

				//Has actions

				//If not in read-only mode
				if (this.readonly === false) return 0

				//Iterate actions and check if available in read-only mode
				for (let idx = 0; idx < actionArray.length; idx++) {
					if (actionArray[idx].isInReadOnly !== false) return idx
				}
				//No actions visible in read-only mode
				return -1
			},

			/**
			 * Emit data for executing action
			 * @param actionName {Object}
			 * @param actionsEnabled {Boolean}
			 */
			executeActionRow(action, actionsEnabled) {
				if (this.isDisabled(action) || !actionsEnabled) return

				this.$emit('row-action', action)
			},

			/**
			 * Verify if the action is disabled (with row info)
			 * @param action {Object}
			 */
			isDisabled(action) {
				if (action.disabled) return true

				let permissions = {
					editBtnDisabled: false,
					viewBtnDisabled: false,
					deleteBtnDisabled: false,
					insertBtnDisabled: false,
					...this.btnPermission
				}

				if (action.params) {
					if (action.params.mode === 'EDIT') {
						return permissions.editBtnDisabled
					} else if (action.params.mode === 'SHOW') {
						return permissions.viewBtnDisabled
					} else if (action.params.mode === 'DELETE') {
						return permissions.deleteBtnDisabled
					} else if (action.params.mode === 'DUPLICATE') {
						if (permissions.viewBtnDisabled || permissions.insertBtnDisabled) return true
					} else if (action.params.mode === 'NEW') {
						return permissions.insertBtnDisabled
					}
				}

				return false
			}
		}
	}
</script>
