<template>
	<div
		v-if="isVisible"
		ref="mainWrapper"
		:class="wrapperClasses">
		<slot name="label" />

		<slot />

		<template v-if="hasMessages">
			<template
				v-for="(type, index) in messageTypes"
				:key="index">
				<div
					v-if="messageDescription[type]"
					:class="['btn-popover', type]">
					<q-icon icon="exclamation-sign" />
					{{ messageDescription[type] }}
				</div>
			</template>
		</template>
	</div>
</template>

<script>
	export default {
		name: 'QGridBaseInputStructure',

		inheritAttrs: false,

		props: {
			/**
			 * Reference to the model field object which may contain error messages and other context.
			 */
			modelFieldRef: Object,

			/**
			 * Whether or not the control is currently visible.
			 */
			isVisible: {
				type: Boolean,
				default: true
			}
		},

		expose: [],

		computed: {
			/**
			 * Dynamic classes for the main wrapper element based on current state.
			 */
			wrapperClasses()
			{
				const classes = ['grid-base-input-structure', this.$attrs.class]

				if (this.hasErrorMessages)
					classes.push('error')

				else if (this.hasWarningMessages)
					classes.push('warning')

				else if (this.hasInfoMessages)
					classes.push('info')

				return classes
			},

			/**
			 * Indicates if there are any server error messages.
			 */
			hasErrorMessages()
			{
				return this.modelFieldRef?.hasServerErrorMessages()
			},

			/**
			 * Indicates if there are any server warning messages.
			 */
			hasWarningMessages()
			{
				return this.modelFieldRef?.hasServerWarningMessages()
			},

			/**
			 * Indicates if there are any server info messages.
			 */
			hasInfoMessages()
			{
				return this.modelFieldRef?.serverInfoMessages?.length > 0
			},

			/**
			 * Indicates if there are any messages.
			 */
			hasMessages()
			{
				return this.hasErrorMessages || this.hasWarningMessages || this.hasInfoMessages
			},

			/**
			 * Gets the types of messages to be displayed in this input
			 */
			messageTypes()
			{
				const types = []

				if (this.hasErrorMessages)
					types.push('error')
				if (this.hasWarningMessages)
					types.push('warning')
				if (this.hasInfoMessages)
					types.push('info')

				return types
			},

			/**
			 * Concatenated object of error messages.
			 */
			messageDescription()
			{
				return {
					error: this.modelFieldRef?.serverErrorMessages?.join('\n'),
					warning: this.modelFieldRef?.serverWarningMessages?.join('\n'),
					info: this.modelFieldRef?.serverInfoMessages?.join('\n')
				}
			}
		}
	}
</script>
