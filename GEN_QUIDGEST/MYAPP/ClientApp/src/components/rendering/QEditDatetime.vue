<template>
	<component
		:is="options?.component ? options.component : 'base-input-structure'"
		:id="`${tableName}_${rowIndex}_${columnName}`"
		:class="containerClasses"
		:d-flex-inline="false"
		:label-attrs="{ class: 'i-text__label' }"
		:model-field-ref="modelField"
		:error-display-type="options?.errorDisplayType">
		<q-date-time-picker
			:id="`${tableName}_${rowIndex}_${columnName}`"
			:size="size"
			:classes="classes"
			:format="options.dateTimeType"
			:disabled="options.disabled"
			:readonly="options.readonly"
			:model-value="datetimeValue"
			:teleport="false"
			@update:model-value="update($event)" />
	</component>
</template>

<script>
	import _isEmpty from 'lodash-es/isEmpty'

	import { timeToString } from '@/mixins/genericFunctions.js'
	import { inputSize } from '@/mixins/quidgest.mainEnums.js'
	import modelFieldType from '@/mixins/formModelFieldTypes.js'

	import BaseInputStructure from '@/components/inputs/BaseInputStructure.vue'
	import GridBaseInputStructure from '@/components/inputs/GridBaseInputStructure.vue'
	import QDateTimePicker from '@/components/inputs/QDateTimePicker.vue'

	export default {
		name: 'QEditDatetime',

		emits: ['update', 'loaded'],

		components: {
			BaseInputStructure,
			GridBaseInputStructure,
			QDateTimePicker
		},

		props: {
			/**
			 * The current date or time value for the input control.
			 */
			value: {
				type: [Number, String],
				default: ''
			},

			/**
			 * The name of the table in the database, used to construct the ID for the component.
			 */
			tableName: {
				type: String,
				required: true
			},

			/**
			 * The index of the current row, used to construct the ID for the component.
			 */
			rowIndex: {
				type: [Number, String],
				required: true
			},

			/**
			 * The name of the column in the database, used to construct the ID for the component.
			 */
			columnName: {
				type: String,
				required: true
			},

			/**
			 * A set of options to configure the datetime input, such as blocked state and display type.
			 */
			options: {
				type: Object,
				default: () => ({})
			},

			/**
			 * The size class for the input component.
			 */
			size: {
				type: String,
				validator: (value) => _isEmpty(value) || Reflect.has(inputSize, value)
			},

			/**
			 * An array of additional classes to apply to the input component.
			 */
			classes: {
				type: Array,
				default: () => []
			},

			/**
			 * An array of classes to be applied to the input's container.
			 */
			containerClasses: {
				type: Array,
				default: () => []
			},

			/**
			 * A list of error messages to display in relation to the input.
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
				modelField: new modelFieldType.Date()
			}
		},

		mounted()
		{
			this.$emit('loaded')
		},

		computed: {
			/**
			 * Formats the input value according to the type of date time being edited, either Time or Date.
			 */
			datetimeValue()
			{
				if (this.options.dateTimeType === 'time' && this.value.length === 5)
				{
					const units = this.value.split(':')
					return {
						hours: parseInt(units[0]),
						minutes: parseInt(units[1])
					}
				}
				return this.value
			}
		},

		methods: {
			/**
			 * Updates the value of the control, converting time to a string format if necessary.
			 * @param {Object|String} event - The new value for the datetime input.
			 */
			update(event)
			{
				// If updating time, convert the time object to a string format.
				let updatedTime = event
				if (this.options.dateTimeType === 'time')
					updatedTime = timeToString(event)
				// If date value is a date object
				else if(typeof event?.toISOString === 'function')
					updatedTime = event?.toISOString()
				// If date value is already a string
				else
					updatedTime = event

				this.$emit('update', updatedTime)
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
