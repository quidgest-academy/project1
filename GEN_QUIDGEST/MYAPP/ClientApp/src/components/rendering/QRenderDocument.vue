<template>
	<a
		v-if="value?.fileName && value?.ticket"
		class="column-data-link"
		rel="tooltip"
		title="Descarregar"
		data-table-action-selected="false"
		tabindex="-1"
		@click.stop.prevent="onSelect"
		@keyup.enter="onSelect">
		{{ value.fileName }}
	</a>
</template>

<script>
	import { documentViewTypeMode } from '@/mixins/quidgest.mainEnums.js'

	export default {
		name: 'QRenderDocument',

		emits: ['execute-action'],

		props: {
			/**
			 * The object containing properties necessary to represent a document.
			 * It usually has a ticket for authentication, a fileName for display and download,
			 * title for tooltip, and viewType to determine how the document is to be processed.
			 */
			value: {
				type: Object,
				default: () => ({
					ticket: '',
					fileName: '',
					title: '',
					viewType: documentViewTypeMode.preview
				})
			}
		},

		expose: [],

		methods: {
			/**
			 * Method to execute when the anchor link is clicked.
			 * It emits the 'execute-action' event with details for the document download.
			 */
			onSelect()
			{
				const viewType = this.value?.viewType ?? documentViewTypeMode.preview

				this.$emit('execute-action', {
					action: 'download',
					ticket: this.value.ticket,
					fileName: this.value.fileName,
					viewType: viewType
				})
			}
		}
	}
</script>
