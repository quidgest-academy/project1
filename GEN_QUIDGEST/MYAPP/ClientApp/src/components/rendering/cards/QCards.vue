<template>
	<div
		:id="id"
		class="container-fluid"
		data-testid="cards"
		role="rowgroup">
		<component
			v-if="hasContent"
			:is="`q-card-${displayMode}`"
			:card-config="cardConfig"
			:loading="$props.loading"
			:container-alignment="styleVariables.containerAlignment?.value">
			<div
				v-if="hasCustomInsertCard"
				:class="columnClasses">
				<q-insert-card
					:config="cardConfig"
					:subtype="insertCardStyle"
					:insert-action="insertAction"
					:texts="texts"
					@row-action="rowAction($event)" />
			</div>

			<div
				v-for="card in cards"
				:key="card.id"
				:class="columnClasses">
				<q-card
					v-bind="card"
					@click.stop="onCardClick(card)">
					<template #title>
						<q-render-data
							:component="card.mappedValue.title?.source?.component"
							:value="card.mappedValue.title?.value"
							:background-color="card.mappedValue.title?.bgColor"
							:options="card.mappedValue.title?.source?.componentOptions || card.mappedValue.title?.source"
							:resources-path="listConfig.resourcesPath" />
					</template>

					<template
						v-if="card.mappedValue.subtitle"
						#subtitle>
						<q-render-data
							:component="card.mappedValue.subtitle?.source?.component"
							:value="card.mappedValue.subtitle?.value"
							:background-color="card.mappedValue.subtitle?.bgColor"
							:options="card.mappedValue.subtitle?.source?.componentOptions || card.mappedValue.subtitle?.source"
							:resources-path="listConfig.resourcesPath" />
					</template>

					<template #[`content.append`]>
						<p
							v-for="text in card.mappedValue.text"
							:key="text"
							class="q-card__text"
							:data-field="`${text.source?.area}.${text.source?.field}`"
							role="cell">
							<span
								v-if="showColumnTitles"
								class="label">
								{{ text.source?.label }}:
							</span>
							<q-render-data
								:component="text.source?.component"
								:value="text.value"
								:background-color="text?.bgColor"
								:options="text.source?.componentOptions || text.source"
								:resources-path="listConfig.resourcesPath" />
						</p>
					</template>

					<template #image>
						<img
							v-if="card.mappedValue.image?.previewData"
							role="cell"
							loading="lazy"
							decoding="async"
							:alt="texts.cardImage"
							class="q-card__img"
							:key="card.domVersionKey"
							:src="card.mappedValue.image.previewData" />
					</template>

					<template
						v-if="hasRowActions"
						#[actionsPlacement]>
						<div
							role="cell"
							:class="[actionsAlignment, 'text-center', 'row-actions']">
							<q-table-record-actions-menu
								:texts="texts"
								:btn-permission="card.mappedValue.btnPermission"
								:action-visibility="card.mappedValue.actionVisibility"
								:crud-actions="listConfig.crudActions"
								:custom-actions="listConfig.customActions"
								:dropdown-direction="actionsMenuDirection"
								:dropdown-alignment="actionsMenuAlignment"
								:show-row-action-icon="listConfig.showRowActionIcon"
								:show-general-action-icon="listConfig.showGeneralActionIcon"
								:show-row-action-text="showRowActionText"
								:show-general-action-text="listConfig.showGeneralActionText"
								:readonly="readonly"
								:display="cardActionsStyle"
								@row-action="rowAction($event, card)" />
						</div>
					</template>
				</q-card>
			</div>
		</component>
		<div
			v-else
			:class="emptyContainerClasses">
			<div class="q-cards-empty-container">
				<img
					v-if="listConfig.resourcesPath"
					:src="`${listConfig.resourcesPath}empty_card_container.png`"
					:alt="texts.noRecordsText" />
				<h5>{{ texts.emptyText }}</h5>
			</div>
		</div>
	</div>
</template>

<script>
	import { defineAsyncComponent } from 'vue'

	import { validateTexts } from '@/mixins/genericFunctions.js'

	import QCard from '@/components/containers/QCard.vue'

	// The texts needed by the component.
	const DEFAULT_TEXTS = {
		noRecordsText: 'No records',
		emptyText: 'No data to show',
		createText: 'Create',
		insertText: 'Insert',
		cardImage: 'Card image'
	}

	export default {
		name: 'QCards',

		emits: ['update:visible', 'row-action'],

		components: {
			QCard,
			QTableRecordActionsMenu: defineAsyncComponent(() => import('@/components/table/QTableRecordActionsMenu.vue')),
			QInsertCard: defineAsyncComponent(() => import('./QInsertCard.vue')),
			QCardGrid: defineAsyncComponent(() => import('./QCardGrid.vue')),
			QCardCarousel: defineAsyncComponent(() => import('./QCardCarousel.vue'))
		},

		inheritAttrs: false,

		props: {
			/**
			 * The unique identifier for the container.
			 */
			id: String,

			/**
			 * The card type (must match it's vue component's name).
			 */
			subtype: String,

			/**
			 * The data from which we will display the cards.
			 */
			mappedValues: {
				type: Array,
				default: () => []
			},

			/**
			 * The defined style variables.
			 */
			styleVariables: {
				type: Object,
				default: () => ({})
			},

			/**
			 * The configuration of the list.
			 */
			listConfig: {
				type: Object,
				default: () => ({})
			},

			/**
			 * The necessary strings to be used inside the component.
			 */
			texts: {
				type: Object,
				validator: (value) => validateTexts(DEFAULT_TEXTS, value),
				default: () => DEFAULT_TEXTS
			},

			/**
			 * Whether or not the 'read-only' mode is active.
			 */
			readonly: {
				type: Boolean,
				default: false
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
			cardConfig()
			{
				const size = this.styleVariables.size?.value,
					contentAlignment = this.styleVariables.contentAlignment?.value,
					imageShape = this.styleVariables.imageShape?.value,
					hoverScaleAmount = this.styleVariables.hoverScaleAmount?.value
						?.toString()
						.slice(-1)

				return {
					subtype: this.subtype,
					size: size,
					contentAlignment: contentAlignment,
					imageShape: imageShape,
					hoverScaleAmount: hoverScaleAmount
				}
			},

			cards()
			{
				const _cards = []

				this.mappedValues.forEach((val) => {
					_cards.push({
						id: val.rowKey,
						image: val.image?.previewData ?? val.image?.value,
						colorPlaceholder: val.image?.dominantColor,
						mappedValue: val,
						...this.cardConfig
					})
				})

				return _cards
			},

			actionsAlignment()
			{
				// actionsAlignment: 'left' | 'right', defaults to 'left'
				const _float = this.styleVariables.actionsAlignment?.value ?? 'left'
				return `float-${_float}`
			},

			actionsPlacement()
			{
				// actionsAlignment: 'footer' | 'header', defaults to 'footer'
				return this.styleVariables.actionsPlacement?.value ?? 'footer'
			},

			actionsMenuDirection()
			{
				return this.actionsPlacement === 'header' ? 'dropdown' : 'dropup'
			},

			actionsMenuAlignment()
			{
				return this.actionsAlignment === 'float-right' ? 'right' : 'left'
			},

			cardActionsStyle()
			{
				const actionsStyle = this.styleVariables.actionsStyle?.value ?? 'dropdown'

				return actionsStyle === 'dropdown'
					? 'dropdown'
					: actionsStyle === 'mixed'
						? 'mixed'
						: 'inlineAll'
			},

			containerAlignment()
			{
				return this.styleVariables.containerAlignment?.value ?? 'left'
			},

			columnClasses()
			{
				return this.displayMode === 'carousel'
					? 'col'
					: ['col-auto', 'd-flex', 'align-items-stretch']
			},

			displayMode()
			{
				return this.styleVariables.displayMode?.value ?? 'grid'
			},

			emptyContainerClasses()
			{
				const classes = ['d-flex']

				if (this.containerAlignment === 'center')
					classes.push('flex-column')

				return classes
			},

			hasContent()
			{
				// There is content to display if
				// - it's loading: will show card skeleton loaders
				// - has custom insertion card: will always displayed even if empty
				// - has actual cards to display
				return this.loading || this.hasCustomInsertCard || this.cards.length > 0
			},

			hasCustomInsertCard()
			{
				if (!this.insertAction)
					return false

				return this.styleVariables.customInsertCard?.value ?? false
			},

			hasRowActions()
			{
				return (
					(this.listConfig.crudActions && this.listConfig.crudActions.length > 0) ||
					(this.listConfig.customActions && this.listConfig.customActions.length > 0)
				)
			},

			insertCardStyle()
			{
				// actionsAlignment: 'image' | 'primary' | 'secondary', defaults to 'secondary'
				return this.styleVariables.customInsertCardStyle?.value ?? 'secondary'
			},

			insertAction()
			{
				return this.listConfig.generalActions?.find((act) => act.id === 'insert')
			},

			showColumnTitles()
			{
				return this.styleVariables.showColumnTitles?.value ?? false
			},

			showRowActionText()
			{
				return this.cardActionsStyle === 'dropdown' || this.cardActionsStyle === 'mixed'
			}
		},

		methods: {
			onCardClick(card)
			{
				if (card.mappedValue.customFollowup !== undefined)
				{
					const url = card.mappedValue.customFollowup.value

					if (url)
					{
						const customFollowupTarget =
							card.mappedValue.customFollowupTarget?.value ||
							this.styleVariables.customFollowupDefaultTarget?.value

						window.open(url, `_${customFollowupTarget}`)
					}
				}
				else if (
					this.listConfig.rowClickAction &&
					Object.keys(this.listConfig.rowClickAction).length > 0
				)
				{
					// Execute the default row action.
					this.rowAction(this.listConfig.rowClickAction, card)
				}
			},

			/**
			 * Emit a row action
			 * @param {object} action
			 * @param {object} card
			 */
			rowAction(action, card)
			{
				this.$emit('row-action', { ...action, rowKey: card?.id })
			}
		},

		watch: {
			cards: {
				handler(newVal, oldVal)
				{
					// TODO: use lazy: request card image just before it scrolls into view
					newVal.forEach((card) => {
						const oldCard = oldVal?.find((s) => s.id === card.id)

						if (!oldCard || (oldCard.image !== card.image))
							this.$emit('update:visible', card.id)
					})
				},

				deep: true,
				immediate: true
			}
		}
	}
</script>
