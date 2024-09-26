<template>
	<base-input-structure
		:id="`${tableName}_${rowIndex}_${columnName}`"
		d-flex-inline
		:class="containerClasses"
		:label-attrs="{ class: 'i-text__label' }">
		<q-check-list-input
			:id="`${tableName}_${rowIndex}_${columnName}`"
			:size="size"
			:classes="classes"
			:options="selectOptions"
			:readonly="options.readonly"
			:model-value="value"
			@update:model-value="$emit('update', $event)" />
	</base-input-structure>
</template>

<script>
	import _isEmpty from 'lodash-es/isEmpty'

	import { inputSize } from '@/mixins/quidgest.mainEnums.js'
	import BaseInputStructure from '@/components/inputs/BaseInputStructure.vue'
	import QCheckListInput from '@/components/inputs/CheckListInput.vue'

	export default {
		name: 'QEditCheckList',

		emits: ['update', 'loaded'],

		components: {
			BaseInputStructure,
			QCheckListInput
		},

		props: {
			/**
			 * The current value of the checklist, which is an array of selected values.
			 */
			value: {
				type: Array,
				default: () => []
			},

			/**
			 * The name of the table in the database, used to construct the control ID.
			 */
			tableName: {
				type: String,
				required: true
			},

			/**
			 * The index of the current row, used together with tableName and columnName to construct the control ID.
			 */
			rowIndex: {
				type: Number,
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
			 * Options for the checklist such as readOnly status and the array of selectable options.
			 */
			options: {
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
			 * An array of additional classes to apply to the checklist.
			 */
			classes: {
				type: Array,
				default: () => []
			},

			/**
			 * An array of classes to be applied to the checklist's container.
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

		computed: {
			/**
			 * Parses and formats the options for the checklist based on the distinctValues option.
			 * Converts the distinct values object to an array format expected by the checklist input.
			 */
			selectOptions()
			{
				if (!this.options.distinctValues)
					return this.options.array

				const optionsArr = [],
					optionsObj = this.options.distinctValues

				const parseKey =
					this.options.arrayType === 'N' ||
					this.options.arrayType === 'L'

				for (let key in optionsObj)
				{
					if (key.length < 1)
						continue

					const value = optionsObj[key]
					const arrKey = this.options.keyIsValue
						? value
						: parseKey
							? parseInt(key)
							: key

					optionsArr.push({ key: arrKey, value: value })
				}

				return optionsArr
			}
		}
	}
</script>
