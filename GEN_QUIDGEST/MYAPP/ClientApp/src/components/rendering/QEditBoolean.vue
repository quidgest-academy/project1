<template>
	<div class="checklist-col-base">
		<q-checkbox-input
			:id="`${tableName}_${rowIndex}_${columnName}`"
			:classes="classes"
			:readonly="options.readonly"
			:model-value="value"
			data-table-action-selected="false"
			tabindex="-1"
			@update:model-value="updateValue" />
	</div>
</template>

<script>
	import _isEmpty from 'lodash-es/isEmpty'

	import { inputSize } from '@/mixins/quidgest.mainEnums.js'

	import QCheckboxInput from '@/components/inputs/CheckBoxInput.vue'

	export default {
		name: 'QEditBoolean',

		emits: ['update', 'loaded'],

		components: {
			QCheckboxInput
		},

		props: {
			/**
			 * The checked value of the checkbox, can be a boolean or a number corresponding to true or false.
			 */
			value: {
				type: [Boolean, Number],
				default: false
			},

			/**
			 * The name of the table in the database, used to construct the checkbox ID.
			 */
			tableName: {
				type: String,
				required: true
			},

			/**
			 * The index of the current row, used together with tableName and columnName to construct the checkbox ID.
			 */
			rowIndex: {
				type: [Number, String],
				required: true
			},

			/**
			 * The name of the column in the database, used to construct the checkbox ID.
			 */
			columnName: {
				type: String,
				required: true
			},

			/**
			 * Options for the checkbox such as readOnly status.
			 */
			options: {
				type: Object,
				default: () => ({})
			},

			/**
			 * Sizing class for the checkbox, based on predefined options.
			 */
			size: {
				type: String,
				validator: (value) => _isEmpty(value) || Reflect.has(inputSize, value)
			},

			/**
			 * An array of additional classes to apply to the checkbox.
			 */
			classes: {
				type: Array,
				default: () => []
			},

			/**
			 * An array of classes to be applied to the checkbox's container.
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
			 * Emits an event to communicate that the checkbox value is updated.
			 * Translates various input event values to 0 or 1 to store boolean as a number.
			 * @param {Boolean|Number} event - The value of the checkbox after update.
			 */
			updateValue(event)
			{
				var value = 0

				if (typeof event === 'number')
					value = event === 0 ? 0 : 1
				else if (typeof event === 'boolean')
					value = event === true ? 1 : 0
				else
					value = 0

				this.$emit('update', value)
			}
		}
	}
</script>
