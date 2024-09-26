<template>
	<div :class="separatorClasses" />
</template>

<script>
	export default {
		name: 'QSeparator',

		props: {
			/**
			 * The style of the separator.
			 */
			sepStyle: {
				type: String,
				default: ''
			},

			/**
			 * An array of custom classes for the separator.
			 */
			customClasses: {
				type: Array,
				default: () => []
			},

			/**
			 * Indicates if the separator should have an elevated appearance.
			 */
			elevated: {
				type: Boolean,
				default: false
			},

			/**
			 * Indicates if the separator is vertical or horizontal.
			 */
			vertical: {
				type: Boolean,
				default: false
			},

			/**
			 * The size of the separator.
			 */
			size: {
				type: String,
				default: ''
			},

			/**
			 * The margin of the separator.
			 */
			space: {
				type: String,
				default: ''
			}
		},

		// TODO: Remove these properties from the "expose" (only necessary for unit tests).
		expose: [
			'separatorClasses',
			'size',
			'vertical'
		],

		computed: {
			separatorClasses()
			{
				const sepPrefix = 'q-sep'
				const classes = [sepPrefix]

				if (this.sepStyle)
					classes.push(`${sepPrefix}--${this.sepStyle}`)

				if (this.elevated)
					classes.push(`${sepPrefix}--elevated`)

				if (this.vertical)
					classes.push(`${sepPrefix}--vertical`)

				if (this.size)
					classes.push(`${sepPrefix}--size-${this.size}`)

				if (this.space)
					classes.push(`${sepPrefix}--space-${this.space}`)

				// Adds any provided custom classes to the default ones.
				for (let customClass of this.customClasses)
					classes.push(customClass)

				return classes
			}
		}
	}
</script>
