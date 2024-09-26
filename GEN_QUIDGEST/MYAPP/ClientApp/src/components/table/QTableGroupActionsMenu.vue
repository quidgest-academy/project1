<template>
	<!-- BEGIN: Multiple actions -->
	<div
		v-if="numVisibleActions > 1"
		:class="[classDropDown, 'position-static']">
		<!-- BEGIN: Menu button -->
		<q-toggle-dropdown
			ref="optionsButton"
			data-testid="options-btn"
			data-boundary="window"
			aria-expanded="false"
			b-style="primary"
			:disabled="rowsSelectedCount === 0"
			:title="texts.groupActionsText">
			{{ texts.groupActionsText }}
		</q-toggle-dropdown>
		<!-- END: Menu button -->
		<!-- BEGIN: Dropdown menu -->
		<div
			:class="[classDropPull, 'dropdown-menu']"
			:x-placement="xPlacement"
			:style="{
				'position': 'absolute',
				'transform': classDropTransform,
				'top': '0',
				'left': '0',
				'will-change': 'transform'
			}"
			role="menu">
			<!-- BEGIN: CRUD action links -->
			<q-table-actions
				:actions="visibleGroupActions"
				@action-click="groupAction" />
			<!-- END: CRUD action links -->
		</div>
		<!-- END: Dropdown menu -->
	</div>
	<!-- END: Multiple actions -->
	<!-- BEGIN: Single action -->
	<q-button
		v-else-if="followUpAction"
		b-style="primary"
		:label="followUpAction.title"
		:disabled="rowsSelectedCount === 0"
		@click="groupAction(followUpAction)">
		<q-icon
			v-if="followUpAction.icon"
			v-bind="followUpAction.icon" />
	</q-button>
	<!-- END: Single action -->
</template>

<script>
	import _map from 'lodash-es/map'

	import QToggleDropdown from '@/components/QToggleDropdown.vue'
	import { numArrayVisibleActions } from '@/mixins/listFunctions.js'

	export default {
		name: 'QTableGroupActionsMenu',

		emits: ['group-action'],

		components: {
			QToggleDropdown
		},

		props: {
			/**
			 * The number of rows currently selected in the table, used to determine whether to enable or disable group actions.
			 */
			rowsSelectedCount: {
				type: Number,
				default: 0
			},

			/**
			 * An array of actions that can be applied to the group of selected rows.
			 */
			groupActions: {
				type: Array,
				default: () => []
			},

			/**
			 * Specifies the placement of the group action button relative to the table rows, which can be 'left' or 'right'.
			 */
			actionsPlacement: {
				type: String,
				default: 'left'
			},

			/**
			 * Localized text strings to be used in the group actions menu for labels, titles, and accessibility.
			 */
			texts: {
				type: Object,
				required: true
			}
		},

		expose: [],

		computed: {
			/**
			 * The array of group actions that are visible.
			 */
			visibleGroupActions() {
				return _map(this.groupActions, (action) => {
					return {
						...action,
						isVisible: action.visibleCondition ? action.visibleCondition() : true
					}
				})
			},

			/**
			 * Determine total number of actions that are visible
			 */
			numVisibleActions()
			{
				return numArrayVisibleActions(this.visibleGroupActions, false)
			},

			/**
			 * Create group action
			 */
			followUpAction()
			{
				return this.numVisibleActions === 1 ? this.visibleGroupActions[0] : null
			},

			//BEGIN: Props for styles
			classDropDown()
			{
				return this.actionsPlacement === 'right' ? 'dropleft' : 'dropdown'
			},

			classDropPull()
			{
				return this.actionsPlacement === 'right' ? 'pull-right' : 'pull-left'
			},

			classDropTransform()
			{
				return this.actionsPlacement === 'right' ? 'translate3d(1299px, 113px, 0)' : 'translate3d(2px, 97px, 0)'
			},

			xPlacement()
			{
				return this.actionsPlacement === 'right' ? 'left-start' : 'bottom-start'
			}
			//END: Props for styles
		},

		methods: {
			/**
			 * Emit data for executing action
			 * @param action {String}
			 */
			groupAction(action)
			{
				this.$emit('group-action', { action: action })
			}
		}
	}
</script>
