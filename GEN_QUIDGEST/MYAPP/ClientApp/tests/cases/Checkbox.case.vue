<template>
	<div
		class="container-fluid"
		style="flex: 1 1 auto">
		<form class="form-horizontal">
			<fieldset>
				<div class="form-flow">
					<q-row-container>
						<q-control-wrapper class="control-join-group">
							<base-input-structure
								id="CTRL_1"
								label="Disabled filled checkbox"
								label-position="right"
								readonly
								:label-attrs="{ class: 'i-checkbox i-checkbox__label' }">
								<template #label>
									<q-checkbox-input
										id="CTRL_1"
										v-model="field1"
										readonly
										@update:model-value="updateValue('field1', $event)" />
								</template>
							</base-input-structure>
						</q-control-wrapper>
					</q-row-container>

					<q-row-container>
						<q-control-wrapper class="control-join-group">
							<base-input-structure
								id="CTRL_2"
								label="Disabled empty checkbox"
								label-position="right"
								readonly
								:label-attrs="{ class: 'i-checkbox i-checkbox__label' }">
								<template #label>
									<q-checkbox-input
										id="CTRL_2"
										v-model="field2"
										readonly
										@update:model-value="updateValue('field2', $event)" />
								</template>
							</base-input-structure>
						</q-control-wrapper>
					</q-row-container>

					<q-row-container>
						<q-control-wrapper class="control-join-group">
							<base-input-structure
								id="CTRL_3"
								label="Checkbox with label on the right"
								label-position="right"
								:label-attrs="{ class: 'i-checkbox i-checkbox__label' }">
								<template #label>
									<q-checkbox-input
										id="CTRL_3"
										v-model="field3"
										@update:model-value="updateValue('field3', $event)"
										@click="fieldClicked('field3')" />
								</template>
							</base-input-structure>
						</q-control-wrapper>
					</q-row-container>

					<q-row-container>
						<q-control-wrapper class="control-join-group">
							<base-input-structure
								id="CTRL_4"
								label="Checkbox with label on the left"
								label-position="left"
								:label-attrs="{ class: 'i-checkbox i-checkbox__label' }">
								<template #label>
									<q-checkbox-input
										id="CTRL_4"
										v-model="field4"
										@update:model-value="updateValue('field4', $event)"
										@click="fieldClicked('field4')" />
								</template>
							</base-input-structure>
						</q-control-wrapper>
					</q-row-container>
				</div>
			</fieldset>
		</form>
	</div>
</template>

<script>
	export default {
		name: 'QCheckboxContainer',

		docsfile: './docs/inputs/CheckBoxInput.md',

		inheritAttrs: false,

		expose: [],

		data()
		{
			return {
				field1: true,
				field2: false,
				field3: false,
				field4: 1
			}
		},

		methods: {
			updateValue(field, value)
			{
				this[field] = value
				// eslint-disable-next-line no-console
				console.log(value)
			},

			fieldClicked(field)
			{
				// eslint-disable-next-line no-console
				console.log(`Field ${field} was clicked!`)
			}
		}
	}
</script>
