<template>
	<component
		:is="options?.component ? options.component : 'base-input-structure'"
		:id="`${tableName}_${rowIndex}_${columnName}`"
		:class="containerClasses"
		:d-flex-inline="false"
		:label-attrs="{ class: 'i-text__label' }"
		:model-field-ref="modelField"
		:error-display-type="options?.errorDisplayType">
		<q-numeric-input
			:id="`${tableName}_${rowIndex}_${columnName}`"
			:size="size ?? options.size"
			:classes="classes"
			:thousands-separator="options.thousandsSep"
			:decimal-point="options.decimalSep"
			:max-integers="options.maxDigits"
			:is-decimal="options.decimalPlaces !== undefined && options.decimalPlaces > 0"
			:max-decimals="options.decimalPlaces"
			:currency-symbol="options.currencySymbol"
			:readonly="options.readonly"
			:model-value="rawValue"
			data-table-action-selected="false"
			tabindex="-1"
			@update:model-value="onUpdateModelValue" />
	</component>
</template>

<script>
	import _isEmpty from 'lodash-es/isEmpty'

	import { inputSize } from '@/mixins/quidgest.mainEnums.js'
	import modelFieldType from '@/mixins/formModelFieldTypes.js'

	import QNumericInput from '@/components/inputs/NumericInput.vue'

	export default {
		name: 'QEditNumeric',

		emits: ['update', 'loaded'],

		components: {
			QNumericInput
		},

		props: {
			/**
			 * The current value of the numeric control as a number or string.
			 */
			value: {
				type: [Number, String],
				default: 0
			},

			/**
			 * The raw numeric value without any formatting.
			 */
			rawValue: {
				type: Number,
				default: 0
			},

			/**
			 * The name of the database table associated with this control, used for unique ID construction.
			 */
			tableName: {
				type: String,
				required: true
			},

			/**
			 * The index of the database row associated with this control, used for unique ID construction.
			 */
			rowIndex: {
				type: [Number, String],
				required: true
			},

			/**
			 * The name of the database column associated with this control, used for unique ID construction.
			 */
			columnName: {
				type: String,
				required: true
			},

			/**
			 * The options to configure the numeric input behavior, like readOnly state, currency, separators, etc.
			 */
			options: {
				type: Object,
				default: () => ({})
			},

			/**
			 * Sizing class for the control, based on predefined options.
			 */
			size: {
				type: String,
				validator: (value) => _isEmpty(value) || Reflect.has(inputSize, value)
			},

			/**
			 * Additional classes to apply to the control.
			 */
			classes: {
				type: Array,
				default: () => []
			},

			/**
			 * Additional classes to apply to the control's container.
			 */
			containerClasses: {
				type: Array,
				default: () => []
			},

			/**
			 * Error messages related to the current field to be displayed.
			 */
			errorMessages: {
				type: Array,
				default: () => []
			}
		},

		expose: [],

		data()
		{
			return {
				modelField: new modelFieldType.Number()
			}
		},

		mounted()
		{
			this.$emit('loaded')
		},

		methods: {
			/**
			 * Called when the model value is updated.
			 * @param {Object} event
			 */
			onUpdateModelValue(event)
			{
				this.$emit('update', event)
			}
		},

		watch: {
			errorMessages: {
				handler(newValue)
				{
					this.modelField.serverErrorMessages = newValue
				},
				deep: true
			}
		}
	}
</script>
