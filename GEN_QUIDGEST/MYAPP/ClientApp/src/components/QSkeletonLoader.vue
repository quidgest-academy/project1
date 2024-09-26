<template>
	<div
		:class="classes"
		:style="$attrs.style">
		<div
			v-if="visible"
			class="q-skeleton-loader__bone"></div>
	</div>
</template>

<script>
	export default {
		name: 'QSkeletonLoader',

		inheritAttrs: false,

		props: {
			/**
			 * The type of content that is loading.
			 */
			type: {
				type: String,
				default: ''
			},

			/**
			 * The amount of time (in ms) to delay start of the loading animation for.
			 */
			delay: {
				type: Number,
				default: 0
			}
		},

		expose: [],

		data()
		{
			return {
				visible: false
			}
		},

		mounted()
		{
			setTimeout(() => (this.visible = true), this.delay)
		},

		computed: {
			classes()
			{
				const baseClass = 'q-skeleton-loader'
				const classes = [baseClass]

				if (this.type)
					classes.push(`${baseClass}--${this.type}`)

				return classes
			}
		}
	}
</script>
