<template>
	<base-input-structure
		v-for="(option, optionIdx) in selectOptions"
		:key="optionIdx"
		:id="option.value"
		:disabled="disabled"
		:readonly="readonly"
		:label="option.value"
		label-position="right"
		:label-attrs="{ class: 'i-checkbox i-checkbox__label' }">
		<template #label>
			<q-checkbox-input
				:id="option.value"
				v-model="selectOptions[optionIdx].selected"
				:disabled="disabled"
				:readonly="readonly" />
		</template>
	</base-input-structure>
</template>

<script>
	import cloneDeep from 'lodash-es/cloneDeep'
	import _isEmpty from 'lodash-es/isEmpty'

	import { inputSize } from '@/mixins/quidgest.mainEnums.js'

	export default {
		name: 'QCheckList',

		emits: ['update:modelValue'],

		inheritAttrs: false,

		props: {
			/**
			 * Unique identifier for the control.
			 */
			id: String,

			/**
			 * The string vaue to be edited by the input.
			 */
			modelValue: {
				type: Array,
				default: () => []
			},

			/**
			 * Sizing class for the control.
			 */
			size: {
				type: String,
				validator: (value) => _isEmpty(value) || Reflect.has(inputSize, value)
			},

			/**
			 * Whether the field is disabled.
			 */
			disabled: {
				type: Boolean,
				default: false
			},

			/**
			 * Whether the field is readonly.
			 */
			readonly: {
				type: Boolean,
				default: false
			},

			/**
			 * Whether the field is required.
			 */
			isRequired: {
				type: Boolean,
				default: false
			},

			/**
			 * Options available for selection.
			 */
			options: {
				type: Array,
				default: () => []
			},

			/**
			 * Additional classes to be applied to the control.
			 */
			classes: {
				type: Array,
				default: () => []
			}
		},

		expose: [],

		data()
		{
			return {
				selectOptions: [],

				controlId: this.id || this._.uid
			}
		},

		mounted()
		{
			// Set selected options
			// Must be called this way because it will be modified when checking or unchecking options
			this.selectOptions = this.setSelectedOptions()
		},

		computed: {
			/**
			 * Get array of selected option values.
			 */
			selectedOptionsValue()
			{
				const selectValues = []
				let selectOption = {}

				for (let idx in this.selectOptions)
				{
					selectOption = this.selectOptions[idx]
					if (selectOption.selected !== false)
						selectValues.push(selectOption.value)
				}

				return selectValues
			}
		},

		methods: {
			/**
			 * Get array of all possible options, each with a property set to whether the option is selected or not.
			 * @returns {Array} An array of options with their selected state.
			 */
			setSelectedOptions()
			{
				// Make copy of all options
				const selectOptions = cloneDeep(this.options)

				// Set property of each option to it's state, selected or not, based on whether it's value is in the modelValue array
				for (let idx in selectOptions)
				{
					const selectOption = selectOptions[idx]
					// Option's value is in the modelValue array
					if (this.modelValue.findIndex((elem) => elem === selectOption.value) > -1)
						selectOption.selected = true
					// Option's value is in not the modelValue array
					else
						selectOption.selected = false
				}

				return selectOptions
			}
		},

		watch: {
			selectOptions: {
				handler()
				{
					this.$emit('update:modelValue', this.selectedOptionsValue)
				},
				deep: true
			}
		}
	}
</script>
