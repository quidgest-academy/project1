<template>
	<div :class="containerClasses">
		<q-spinner-loader
			v-if="$props.loading"
			class="q-cards__spinner-loader"
			:size="96" />
		<slot v-else />
	</div>
</template>

<script>
	export default {
		name: 'QCardGrid',

		inheritAttrs: false,

		props: {
			/**
			 * The alignment of the cards.
			 */
			containerAlignment: {
				type: String,
				default: 'left'
			},

			/**
			 * Whether or not content is loading.
			 */
			loading: {
				type: Boolean,
				default: false
			}
		},

		expose: [],

		computed: {
			containerClasses()
			{
				const baseClass = 'q-cards-grid'
				const classes = ['row']

				if (this.containerAlignment === 'center')
					classes.push(`${baseClass}__centered`)

				return classes
			}
		}
	}
</script>
