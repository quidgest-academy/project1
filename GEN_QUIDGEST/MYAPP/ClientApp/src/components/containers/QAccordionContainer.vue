<template>
	<div
		v-show="isVisible"
		:id="controlId"
		:class="['q-accordion-container', $attrs.class]">
		<slot @state-changed="onGroupStateChange"></slot>
	</div>
</template>

<script>
	export default {
		name: 'QAccordionContainer',

		emits: ['set-open-group'],

		inheritAttrs: false,

		props: {
			/**
			 * Unique identifier for the control.
			 */
			id: {
				type: String,
				required: true
			},

			/**
			 * Visible property to hide and show the accordion group.
			 */
			isVisible: {
				type: Boolean,
				default: true
			}
		},

		expose: [],

		data() {
			return {
				controlId: this.id || `q-accordion-container-${this._.uid}`
			}
		},

		methods: {
			onGroupStateChange(state, groupId)
			{
				this.$emit('set-open-group', state, groupId)
			}
		}
	}
</script>
