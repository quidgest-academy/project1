<template>
	<q-card
		v-if="variant === 'image'"
		v-bind="config"
		@click="$emit('row-action', insertAction)">
		<template #image>
			<img
				role="cell"
				loading="lazy"
				decoding="async"
				:alt="texts.cardImage"
				:class="imgClass"
				:src="`${listConfig.resourcesPath}insert_card.png`" />
		</template>
		<template #title>
			{{ texts.createText }}
			{{ listConfig.tableTitle }}
		</template>
		<template #subtitle>
			<q-table-record-actions-menu
				display="inline"
				show-general-action-text
				show-general-action-icon
				:texts="texts"
				:general-actions="[insertAction]"
				@row-action="$emit('row-action', $event)" />
		</template>
	</q-card>
	<q-card
		v-else
		v-bind="config"
		:class="['q-card--insert', `q-card--insert-${variant}`]"
		@click="$emit('row-action', insertAction)">
		<template #title></template>
		<template #subtitle></template>
		<template #text></template>
		<template #image></template>
		<template #underlay>
			<span>
				<q-icon icon="add" />
				{{ texts.insertText }}
			</span>
		</template>
	</q-card>
</template>

<script>
	import { defineAsyncComponent } from 'vue'

	import { validateTexts } from '@/mixins/genericFunctions.js'

	import QCard from '@/components/containers/QCard.vue'

	// The texts needed by the component.
	const DEFAULT_TEXTS = {
		createText: 'Create',
		insertText: 'Insert',
		cardImage: 'Card image'
	}

	export default {
		name: 'QInsertCard',

		emits: ['row-action'],

		components: {
			QCard,
			QTableRecordActionsMenu: defineAsyncComponent(() => import('@/components/table/QTableRecordActionsMenu.vue'))
		},

		inheritAttrs: false,

		props: {
			/**
			 * The configuration of the card.
			 */
			config: {
				type: Object,
				default: () => {
					return {}
				}
			},

			/**
			 * The variant of the insert action.
			 */
			variant: {
				type: String,
				require: true,
				validator: (value) => {
					return ['image', 'primary', 'secondary'].includes(value)
				}
			},

			/**
			 * The insert action.
			 */
			insertAction: {
				type: Object,
				required: true
			},

			/**
			 * The necessary strings to be used inside the component.
			 */
			texts: {
				type: Object,
				validator: (value) => validateTexts(DEFAULT_TEXTS, value),
				default: () => DEFAULT_TEXTS
			}
		},

		expose: []
	}
</script>
