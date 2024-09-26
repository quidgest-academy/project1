<template>
	<template
		v-for="(action, index) in actions"
		:key="action.id">
		<q-button
			v-if="actionIsAvailable(action)"
			v-bind="$attrs"
			data-testid="table-action"
			:data-action-key="action.id"
			:borderless="borderless"
			:b-style="bStyle"
			:class="customActionClasses"
			:title="action.title"
			:label="showActionText ? action.title : ''"
			:disabled="action.disabled || !enableActions"
			@click="$emit('action-click', action, enableActions)">
			<q-icon
				v-if="action.icon && showIcon"
				v-bind="action.icon" />
		</q-button>

		<div
			v-if="action.separator && actionIsAvailable(action) && index < actions.length - 1"
			:class="separatorClass"></div>
	</template>
</template>

<script>
	export default {
		name: 'QTableActions',

		emits: ['action-click'],

		inheritAttrs: false,

		props: {
			/**
			 * An array of actions that the user can perform, typically represented by buttons or other interactive elements.
			 */
			actions: {
				type: Array,
				default: () => []
			},

			/**
			 * An array of custom CSS classes to be applied to each action component.
			 */
			customActionClasses: {
				type: Array,
				default: () => []
			},

			/**
			 * The base style type to be applied to the action buttons (e.g., 'secondary', 'primary').
			 */
			bStyle: {
				type: String,
				default: 'secondary'
			},

			/**
			 * Flag indicating whether the action buttons should be rendered without a border.
			 */
			borderless: {
				type: Boolean,
				default: false
			},

			/**
			 * Indicates whether the table is in a read-only state which might limit available actions.
			 */
			readonly: {
				type: Boolean,
				default: false
			},

			/**
			 * Determines whether the action text should be displayed alongside its icon.
			 */
			showActionText: {
				type: Boolean,
				default: true
			},

			/**
			 * Determines whether the icon for each action should be displayed.
			 */
			showIcon: {
				type: Boolean,
				default: true
			},

			/**
			 * Flag indicating whether action buttons should be enabled or disabled.
			 */
			enableActions: {
				type: Boolean,
				default: true
			},

			/**
			 * The CSS class applied to dividers or separators between action buttons.
			 */
			separatorClass: {
				type: String,
				default: 'dropdown-divider'
			}
		},

		expose: [],

		methods: {
			actionIsAvailable(action) {
				if (action.isVisible === false)
					return false
				if (this.readonly)
					return action.isInReadOnly
				return true
			}
		}
	}
</script>
