<template>
	<q-dropdown-menu
		id="export-drop"
		icon="file-export"
		:texts="dropdownTexts"
		:options="options"
		:button-options="dropdownButtonOptions"
		:button-classes="['dropdown-toggle']"
		@selected="$emit('export-data', $event)">
	</q-dropdown-menu>
</template>

<script>
	import { defineAsyncComponent } from 'vue'

	import { validateTexts } from '@/mixins/genericFunctions.js'

	// The texts needed by the component.
	const DEFAULT_TEXTS = {
		exportButtonTitle: 'Export'
	}

	export default {
		name: 'QTableExport',

		emits: ['export-data'],

		components: {
			QDropdownMenu: defineAsyncComponent(() => import('@/components/QDropdownMenu.vue'))
		},

		props: {
			/**
			 * An object containing localized texts for the component, allowing customization of displayed strings.
			 */
			texts: {
				type: Object,
				validator: (value) => validateTexts(DEFAULT_TEXTS, value),
				default: () => DEFAULT_TEXTS
			},

			/**
			 * An array of options available for export. Each option represents a format or export type.
			 */
			options: {
				type: Array,
				default: () => []
			}
		},

		expose: [],

		computed: {
			dropdownTexts()
			{
				return { title: this.texts.exportButtonTitle, label: this.texts.exportButtonTitle }
			},

			dropdownButtonOptions()
			{
				return {
					borderless: true
				}
			}
		}
	}
</script>
