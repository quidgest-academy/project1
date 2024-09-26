<template>
	<div
		class="container-fluid"
		style="flex: 1 1 auto">
		<form class="form-horizontal">
			<fieldset>
				<div class="form-flow">
					<!-- This control with id CTRL_1 is for large text area with required property-->
					<q-row-container>
						<q-control-wrapper class="control-join-group">
							<base-input-structure
								id="CTRL_1"
								class="i-text"
								label="Medium text area (Required)"
								is-required
								:label-attrs="{ class: 'i-text__label' }">
								<q-textarea-input
									id="CTRL_1"
									:cols="25"
									size="medium" />
							</base-input-structure>
						</q-control-wrapper>
					</q-row-container>

					<!-- This control with id CTRL_2 is for read only text area -->
					<q-row-container>
						<q-control-wrapper class="control-join-group">
							<base-input-structure
								id="CTRL_2"
								class="i-text"
								label="Small text area (Read only)"
								:label-attrs="{ class: 'i-text__label' }">
								<q-textarea-input
									id="CTRL_2"
									size="small"
									v-model="readOnlyText"
									:rows="2"
									readonly />
							</base-input-structure>
						</q-control-wrapper>
					</q-row-container>

					<!-- This control with id CTRL_3 is for disabled text area -->
					<q-row-container>
						<q-control-wrapper class="control-join-group">
							<base-input-structure
								id="CTRL_3"
								class="i-text"
								label="Small text area (Disabled)"
								:label-attrs="{ class: 'i-text__label' }">
								<q-textarea-input
									id="CTRL_3"
									size="small"
									v-model="readOnlyText"
									:rows="2"
									disabled />
							</base-input-structure>
						</q-control-wrapper>
					</q-row-container>

					<!-- This control with id CTRL_4 is for text area with visible lines-->
					<q-row-container>
						<q-control-wrapper class="control-join-group">
							<base-input-structure
								id="CTRL_4"
								class="i-text"
								label="Large Text area with 3 visible lines"
								:label-attrs="{ class: 'i-text__label' }">
								<q-textarea-input
									id="CTRL_4"
									:cols="35"
									size="large"
									v-model="demoText"
									:rows="3" />
							</base-input-structure>
						</q-control-wrapper>
					</q-row-container>

					<!-- This control with id CTRL_5 is for text area with scroll due to long text-->
					<q-row-container>
						<q-control-wrapper class="control-join-group">
							<base-input-structure
								id="CTRL_5"
								class="i-text"
								label="XLarge text area with scroll"
								:label-attrs="{ class: 'i-text__label' }">
								<q-textarea-input
									id="CTRL_5"
									:cols="65"
									size="xlarge"
									v-model="demoTextScroll"
									:rows="3" />
							</base-input-structure>
						</q-control-wrapper>
					</q-row-container>
				</div>
			</fieldset>
		</form>
	</div>
</template>

<script>
	import fakeData from './TextareaInput.mock'

	export default {
		name: 'QTextareaContainer',

		docsfile: './docs/inputs/TextareaInput.md',

		inheritAttrs: false,

		expose: [],

		data()
		{
			return fakeData.simpleUsage()
		}
	}
</script>
