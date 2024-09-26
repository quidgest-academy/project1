<template>
	<q-icon
		:icon="icon"
		:class="iconClasses" />
</template>

<script>
	export default {
		name: 'QRenderBoolean',

		props: {
			/**
			 * The boolean value to determine the icon state.
			 */
			value: {
				type: Boolean,
				default: false
			}
		},

		expose: [],

		computed: {
			/**
			 * The icon to be rendered depending on the boolean value.
			 * 'ok' for true values, 'close' for false.
			 */
			icon()
			{
				return this.value ? 'ok' : 'close'
			},

			iconClasses()
			{
				const classes = ['render-boolean']

				this.value ? classes.push('true-icon') : classes.push('false-icon')

				return classes
			}
		}
	}
</script>
