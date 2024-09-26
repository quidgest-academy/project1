<template>
	<div class="i-chip mb-2">
		{{ getCellNameValue(option, displayColumnName) }}
		<q-button
			v-if="!disabled"
			b-style="secondary"
			borderless
			@click="closeChip">
			<q-icon icon="remove" />
		</q-button>
	</div>
</template>

<script>
	import listFunctions from '@/mixins/listFunctions.js'

	export default {
		name: 'QCheckTag',

		emits: ['remove-label'],

		inheritAttrs: false,

		props: {
			/**
			 * The option object for the tag.
			 */
			option: Object,

			/**
			 * The column name to be displayed in the tag.
			 */
			displayColumnName: String,

			/**
			 * Object containing the name of primary key column name.
			 */
			primaryKeyColumnName: {
				type: String,
				required: true
			},

			/**
			 * Configure whether write mode is enabled or not.
			 */
			disabled: {
				type: Boolean,
				default: false
			}
		},

		expose: [],

		methods: {
			/**
			 * Emits an event to remove a label when the close ('x') button on the chip is clicked.
			 */
			closeChip()
			{
				this.$emit(
					'remove-label',
					listFunctions.getCellNameValue(
						this.option,
						this.primaryKeyColumnName
					)
				)
			},

			/**
			 * Retrieves the display value of a row's column.
			 * @param {Object} row - The row object containing the columnName.
			 * @param {String} columnName - The name of the column to retrieve the value from.
			 * @returns {String} The value of the cell in the specified column of the row.
			 */
			getCellNameValue(row, columnName)
			{
				return listFunctions.getCellNameValue(row, columnName)
			}
		}
	}
</script>
