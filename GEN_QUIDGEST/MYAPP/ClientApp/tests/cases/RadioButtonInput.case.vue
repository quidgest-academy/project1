<template>
	<div
		class="container-fluid"
		style="flex: 1 1 auto">
		<form class="form-horizontal">
			<fieldset>
				<div class="form-flow">
					<!-- This control with id CTRL_1 is for showing radio select list with labels on right with deselect functionality-->
					<q-row-container>
						<q-control-wrapper class="control-join-group">
							<base-input-structure
								id="CTRL_1"
								:class="['i-text', { 'i-text--disabled': false }]"
								label="Radio Button for select list (Left) with deselect:"
								:label-attrs="{ class: 'i-text__label' }">
								<q-radio-group
									id="CTRL_1"
									v-model="radio1"
									:options-list="simpleListItems"
									:number-of-columns="2"
									:label-left-side="false"
									deselect-radio />
							</base-input-structure>
						</q-control-wrapper>
					</q-row-container>
					<div class="mt-1 mb-3">Selected: {{ radio1 }}</div>

					<!-- This control with id CTRL_2 is for showing radio select list with labels on left-->
					<q-row-container>
						<q-control-wrapper class="control-join-group">
							<base-input-structure
								id="CTRL_2"
								:class="['i-text', { 'i-text--disabled': false }]"
								label="Radio Button for select list (Right) without deselect:"
								:label-attrs="{ class: 'i-text__label' }">
								<q-radio-group
									id="CTRL_2"
									v-model="radio2"
									:options-list="simpleListItems"
									:number-of-columns="3"
									label-left-side />
							</base-input-structure>
						</q-control-wrapper>
					</q-row-container>
					<div class="mt-1 mb-3">Selected: {{ radio2 }}</div>

					<!-- This control with id CTRL_3 is for showing number options of radio select list-->
					<q-row-container>
						<q-control-wrapper class="control-join-group">
							<base-input-structure
								id="CTRL_3"
								:class="['i-text', { 'i-text--disabled': false }]"
								label="Radio Button for select list with number:"
								:label-attrs="{ class: 'i-text__label' }">
								<q-radio-group
									id="CTRL_3"
									v-model="radio3"
									:options-list="numberListItems"
									:number-of-columns="1"
									label-left-side />
							</base-input-structure>
						</q-control-wrapper>
					</q-row-container>
					<div class="mt-1 mb-3">Selected: {{ radio3 }}</div>

					<!-- This control with id CTRL_4 is for showing disabled radio select list-->
					<q-row-container>
						<q-control-wrapper class="control-join-group">
							<base-input-structure
								id="CTRL_4"
								:class="['i-text', { 'i-text--disabled': false }]"
								label="Radio Button for select list (Disabled):"
								:label-attrs="{ class: 'i-text__label' }">
								<q-radio-group
									id="CTRL_4"
									v-model="radio4"
									:options-list="simpleListItems"
									:number-of-columns="3"
									:label-left-side="false"
									disabled />
							</base-input-structure>
						</q-control-wrapper>
					</q-row-container>
				</div>
			</fieldset>
		</form>
	</div>
</template>

<script>
	import fakeData from './RadioButtonInput.mock'

	export default {
		name: 'QRadioContainer',

		docsfile: './docs/inputs/RadioButtonInput.md',

		inheritAttrs: false,

		expose: [],

		data()
		{
			return fakeData.simpleUsage()
		}
	}
</script>
