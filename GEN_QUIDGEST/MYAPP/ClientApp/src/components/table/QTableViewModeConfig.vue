<template>
	<q-button-toggle
		v-if="options.length > 1"
		v-model="active"
		:options="options"
		required
		borderless>
		<template
			v-for="option in options"
			:key="option.key"
			#[option.key]>
			<q-icon :icon="getViewModeIcon(option)" />
		</template>
	</q-button-toggle>
</template>

<script>
	export default {
		name: 'QTableViewModeConfig',

		emits: ['update:model-value'],

		inheritAttrs: false,

		props: {
			/**
			 * The current value of the view mode selector, which corresponds to the selected view mode's identifier.
			 */
			modelValue: {
				type: String,
				required: true
			},

			/**
			 * An array of available view modes that the user can toggle between.
			 */
			viewModes: {
				type: Array,
				default: () => []
			},

			/**
			 * An object containing localized text strings for the titles of the view mode toggle buttons.
			 */
			texts: {
				type: Object,
				required: true
			}
		},

		expose: [],

		computed: {
			active: {
				get()
				{
					return this.modelValue
				},
				set(id)
				{
					this.$emit('update:model-value', id)
				}
			},

			options() {
				return this.viewModes.map((viewMode) => {
					return { key: viewMode.id, title: this.getViewModeButtonTitle(viewMode) }
				})
			}
		},

		methods: {
			getViewModeIcon(option)
			{
				// FIXME: generate view mode icon
				return option.key === 'LIST' ? 'list-view' : 'alternative-view'
			},

			getViewModeButtonTitle(viewMode)
			{
				// FIXME: generate view mode title
				return viewMode.id === 'LIST' ? this.texts.toListViewButtonTitle : this.texts.toAlternativeViewButtonTitle
			}
		}
	}
</script>
