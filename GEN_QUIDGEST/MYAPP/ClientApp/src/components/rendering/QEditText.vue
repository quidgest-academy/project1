<template>
	<component
		:is="options?.component ? options.component : 'base-input-structure'"
		:id="`${tableName}_${rowIndex}_${columnName}`"
		d-flex-inline
		:class="containerClasses"
		:label-attrs="{ class: 'i-text__label' }"
		:model-field-ref="modelField"
		:error-display-type="options?.errorDisplayType">
		<q-text-field
			:id="`${tableName}_${rowIndex}_${columnName}`"
			:model-value="value"
			:max-length="options.dataLength"
			:size="size"
			:classes="classes"
			:disabled="options.disabled"
			:readonly="options.readonly"
			:placeholder="placeholder"
			@update:model-value="$emit('update', $event)" />
	</component>
</template>

<script>
	import _isEmpty from 'lodash-es/isEmpty'

	import { inputSize } from '@/mixins/quidgest.mainEnums.js'
	import modelFieldType from '@/mixins/formModelFieldTypes.js'

	import BaseInputStructure from '@/components/inputs/BaseInputStructure.vue'
	import GridBaseInputStructure from '@/components/inputs/GridBaseInputStructure.vue'

	export default {
		name: 'QEditText',

		emits: ['update', 'loaded'],

		components: {
			BaseInputStructure,
			GridBaseInputStructure
		},

		props: {
			/**
			 * The actual value of the text input.
			 */
			value: {
				type: String,
				default: ''
			},

			/**
			 * The table name from the database, used for generating a unique ID for the input.
			 */
			tableName: {
				type: String,
				required: true
			},

			/**
			 * The index of the row, which along with tableName and columnName, helps generate a unique ID.
			 */
			rowIndex: {
				type: [Number, String],
				required: true
			},

			/**
			 * The column name from the database, used in generating a unique ID for the input.
			 */
			columnName: {
				type: String,
				required: true
			},

			/**
			 * Object containing various options to control the behavior of the input.
			 */
			options: {
				type: Object,
				default: () => ({})
			},

			/**
			 * Class to apply to the input for sizing.
			 */
			size: {
				type: String,
				validator: (value) => _isEmpty(value) || Reflect.has(inputSize, value)
			},

			/**
			 * Additional CSS classes to apply to the input.
			 */
			classes: {
				type: Array,
				default: () => []
			},

			/**
			 * Additional CSS classes to apply to the input container.
			 */
			containerClasses: {
				type: Array,
				default: () => []
			},

			/**
			 * Placeholder text for the input when empty.
			 */
			placeholder: {
				type: String,
				default: ''
			},

			/**
			 * Array of error messages related to the input's value.
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
