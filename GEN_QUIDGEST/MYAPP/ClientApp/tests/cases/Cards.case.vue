<template>
	<h3>Usage</h3>
	<div class="demo-container">
		<div class="demo-main-container">
			<q-cards
				v-if="usage.cards.subtype"
				:subtype="usage.cards.subtype"
				:mapped-values="mappedValues"
				:style-variables="usage.cards.styleVariables"
				:list-config="usage.cards.listConfig"
				:loading="usage.container.loading" />
		</div>
		<div class="demo-container-sep" />
		<div class="demo-options">
			<h4>Options</h4>
			<div>
				<label class="i-text__label">Container options</label>
				<div class="demo-subtype-options">
					<div class="demo-option">
						<label class="i-text__label">Display mode</label>
						<q-select
							size="small"
							v-model="usage.cards.styleVariables.displayMode.value"
							:items="options.displayModes" />
					</div>
					<div class="demo-option">
						<label class="i-text__label">Container alignment</label>
						<q-select
							size="small"
							v-model="usage.cards.styleVariables.containerAlignment.value"
							readonly="usage.cards.styleVariables.displayMode.value !== 'grid'"
							:items="options.containerAlignment" />
					</div>
					<label class="i-text__label">Number of cards</label>
					<q-numeric-input
						v-model="usage.container.numberOfCards"
						size="small"
						thousands-separator="."
						decimal-point=","
						:max-integers="4"
						:max-decimals="0"
						:is-currency="false" />
					<div
						class="demo-option"
						style="margin-top: 0.25rem">
						<base-input-structure
							id="demo-option-loading"
							label="Loading"
							label-position="right"
							:label-attrs="{
								class: 'i-checkbox i-checkbox__label'
							}">
							<template #label>
								<q-checkbox-input
									id="demo-option-loading"
									v-model="usage.container.loading" />
							</template>
						</base-input-structure>
					</div>
				</div>
			</div>
			<div>
				<label class="i-text__label">Card options</label>
				<div class="demo-subtype-options">
					<div class="demo-option">
						<label class="i-text__label">Subtype</label>
						<q-select
							size="medium"
							v-model="usage.cards.subtype"
							:items="options.variants" />
					</div>
					<div class="demo-option">
						<label class="i-text__label">Size</label>
						<q-select
							size="medium"
							v-model="usage.cards.styleVariables.size.value"
							:items="options.sizes" />
					</div>
					<div class="demo-option">
						<label class="i-text__label">Content alignment</label>
						<q-select
							size="medium"
							v-model="usage.cards.styleVariables.contentAlignment.value"
							:items="options.contentAlignment" />
					</div>
					<div class="demo-option">
						<label class="i-text__label">Actions alignment</label>
						<q-select
							size="medium"
							v-model="usage.cards.styleVariables.actionsAlignment.value"
							:items="options.actionsAlignment" />
					</div>
					<div class="demo-option">
						<label class="i-text__label">Actions placement</label>
						<q-select
							size="medium"
							v-model="usage.cards.styleVariables.actionsPlacement.value"
							:items="options.actionsPlacement" />
					</div>
					<div class="demo-option">
						<label class="i-text__label">Actions style</label>
						<q-select
							size="medium"
							v-model="usage.cards.styleVariables.actionsStyle.value"
							:items="options.actionsStyle" />
					</div>
					<div class="demo-option">
						<label class="i-text__label">Image shape</label>
						<q-select
							size="medium"
							v-model="usage.cards.styleVariables.imageShape.value"
							:items="options.shape" />
					</div>
					<div class="demo-option">
						<label class="i-text__label">Hover scale amount</label>
						<q-select
							size="medium"
							v-model="usage.cards.styleVariables.hoverScaleAmount.value"
							:items="options.hoverScaleAmounts" />
					</div>

					<div
						class="demo-option"
						style="margin-top: 0.25rem">
						<base-input-structure
							id="demo-show-insert-card"
							label="Show insert card"
							label-position="right"
							:label-attrs="{
								class: 'i-checkbox i-checkbox__label'
							}">
							<template #label>
								<q-checkbox-input
									id="demo-show-insert-card"
									v-model="usage.cards.styleVariables.customInsertCard.value" />
							</template>
						</base-input-structure>
					</div>

					<div
						class="demo-option"
						style="margin-top: 0.25rem">
						<base-input-structure
							id="demo-show-col-titles-option"
							label="Show column titles"
							label-position="right"
							:label-attrs="{
								class: 'i-checkbox i-checkbox__label'
							}">
							<template #label>
								<q-checkbox-input
									id="demo-show-col-titles-option"
									v-model="usage.cards.styleVariables.showColumnTitles.value" />
							</template>
						</base-input-structure>
					</div>
				</div>
			</div>
		</div>
	</div>
</template>

<script>
	import fakeData from './Cards.mock'

	export default {
		name: 'QCardsContainer',

		docsfile: './docs/containers/QCard.md',

		inheritAttrs: false,

		expose: [],

		data()
		{
			return fakeData.simpleUsage()
		},

		computed: {
			mappedValues()
			{
				return Array(this.usage.container.numberOfCards).fill(
					this.usage.cards.mappedValues[0]
				)
			}
		}
	}
</script>
