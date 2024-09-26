<template>
	<div
		v-if="value"
		:class="[...containerClasses, 'i-radio__control']">
		<label
			:class="[{ 'i-radio--disabled': options.readonly }, 'i-radio i-radio__label', 'i-radio--inline']"
			:for="`${tableName}_${rowIndex}_${columnName}`">
			{{ options.optionLabel }}
			<span
				v-if="!options.optionLabel"
				class="invisible">
				{{ rowIndex }}
			</span>
			<input
				type="radio"
				:id="`${tableName}_${rowIndex}_${columnName}`"
				:name="options.optionGroupName"
				:value="row.Value"
				:classes="classes"
				:readonly="options.readonly"
				:checked="row.Value === options.checkedValue"
				data-table-action-selected="false"
				tabindex="-1"
				@change="updateExternal($event)" />
			<span class="i-radio__field"></span>
		</label>
	</div>
</template>

<script>
	import _isEmpty from 'lodash-es/isEmpty'

	import { inputSize } from '@/mixins/quidgest.mainEnums.js'

	export default {
		name: 'QEditRadio',

		emits: ['update', 'update-external', 'loaded'],

		props: {
			/**
			 * The value to be used for the radio input (typically a boolean or number).
			 */
			value: {
				type: [Boolean, Number],
				default: false
			},

			/**
			 * The name of the table in the database, used to construct the unique ID for the radio input.
			 */
			tableName: {
				type: String,
				required: true
			},

			/**
			 * The index of the current row, used in conjunction with tableName and columnName to construct the unique ID.
			 */
			rowIndex: {
				type: [Number, String],
				required: true
			},

			/**
			 * The name of the column in the database, part of the unique ID for the radio input.
			 */
			columnName: {
				type: String,
				required: true
			},

			/**
			 * Configuration options for the radio input, such as read-only status and label text.
			 */
			options: {
				type: Object,
				default: () => ({})
			},

			/**
			 * The current row data object containing details necessary for the radio input.
			 */
			row: {
				type: Object,
				default: () => ({})
			},

			/**
			 * Sizing class for the control based on predefined options.
			 */
			size: {
				type: String,
				validator: (value) => _isEmpty(value) || Reflect.has(inputSize, value)
			},

			/**
			 * Classes to be applied to the radio input element.
			 */
			classes: {
				type: Array,
				default: () => []
			},

			/**
			 * Container classes to be applied to the radio input wrapper.
			 */
			containerClasses: {
				type: Array,
				default: () => []
			}
		},

		expose: [],

		mounted()
		{
			this.$emit('loaded')
		},

		methods: {
			/**
			 * Emits an 'update' event when the radio input's selected value has been changed.
			 * @param {Event} event - The native event object from the radio input's change event.
			 */
			update(event)
			{
				this.$emit('update', event.target.value)
			},

			/**
			 * Emits an 'update-external' event for any external updates of the radio input's selected value.
			 * @param {Event} event - The native event object from the radio input's change event.
			 */
			updateExternal(event)
			{
				this.$emit('update-external', event.target.value)
			}
		}
	}
</script>
