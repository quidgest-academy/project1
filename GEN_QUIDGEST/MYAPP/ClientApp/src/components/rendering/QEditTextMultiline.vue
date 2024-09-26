<template>
	<component
		:is="options?.component ? options.component : 'base-input-structure'"
		:id="`${tableName}_${rowIndex}_${columnName}`"
		d-flex-inline
		:class="containerClasses"
		:label-attrs="{ class: 'i-text__label' }"
		:model-field-ref="modelField"
		:error-display-type="options?.errorDisplayType">
		<q-textarea-input
			:id="`${tableName}_${rowIndex}_${columnName}`"
			:rows="1"
			:cols="10"
			:size="size"
			:classes="classes"
			:disabled="options.disabled"
			:readonly="options.readonly"
			:model-value="value"
			@update:model-value="$emit('update', $event)" />
	</component>
</template>

<script>
	import _isEmpty from 'lodash-es/isEmpty'

	import { inputSize } from '@/mixins/quidgest.mainEnums.js'
	import modelFieldType from '@/mixins/formModelFieldTypes.js'

	import BaseInputStructure from '@/components/inputs/BaseInputStructure.vue'
	import GridBaseInputStructure from '@/components/inputs/GridBaseInputStructure.vue'
	import QTextareaInput from '@/components/inputs/TextareaInput.vue'

	export default {
		name: 'QEditTextMultiline',

		emits: ['update', 'loaded'],

		components: {
			BaseInputStructure,
			GridBaseInputStructure,
			QTextareaInput
		},

		props: {
			/**
			 * The current value of the textarea input.
			 */
			value: {
				type: String,
				default: ''
			},

			/**
			 * The name of the table in the database, used to construct the control ID.
			 */
			tableName: {
				type: String,
				required: true
			},

			/**
			 * The index of the current row, used to construct the control ID.
			 */
			rowIndex: {
				type: [Number, String],
				required: true
			},

			/**
			 * The name of the column in the database, used to construct the control ID.
			 */
			columnName: {
				type: String,
				required: true
			},

			/**
			 * Configuration options for the textarea such as whether it is read-only and the error display type.
			 */
			options: {
				type: Object,
				default: () => ({})
			},

			/**
			 * Sizing class for the textarea, often indicating a relative size such as 'small', 'medium', or 'large'.
			 */
			size: {
				type: String,
				validator: (value) => _isEmpty(value) || Reflect.has(inputSize, value)
			},

			/**
			 * An array of additional classes to apply to the textarea.
			 */
			classes: {
				type: Array,
				default: () => []
			},

			/**
			 * An array of classes to be applied to the textarea container.
			 */
			containerClasses: {
				type: Array,
				default: () => []
			},

			/**
			 * Array of error messages related to the textarea's value.
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
				modelField: new modelFieldType.String()
			}
		},

		mounted()
		{
			this.$emit('loaded')
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
