<template>
	<div :class="{ 'checklist-col-base': rowKey !== undefined }">
		<q-checkbox-input
			:id="rowKey !== undefined ? `${tableName}_${rowKey}` : `${tableName}_all`"
			:model-value="value"
			:disabled="disabled"
			:readonly="readonly"
			data-table-action-selected="false"
			tabindex="-1"
			@mousedown="onMousedown"
			@click="onSelect" />
	</div>
</template>

<script>
	export default {
		name: 'QTableChecklistCheckbox',

		emits: ['toggle-row-selected', 'toggle-all-rows-selected'],

		props: {
			/**
			 * The current value or state of the checkbox, indicating whether it's checked (true) or unchecked (false).
			 */
			value: {
				type: Boolean,
				default: false
			},

			/**
			 * A unique identifier or name associated with the parent table of the checkbox.
			 */
			tableName: {
				type: String,
				required: true
			},

			/**
			 * The key or identifier for the specific row in the table.
			 * If it's not provided, the checkbox is meant to toggle all rows in the table.
			 */
			rowKey: [String, Number],

			/**
			 * Indicates whether the table is in a read-only state, which will affect the checkbox's interactivity.
			 */
			readonly: {
				type: Boolean,
				default: false
			},

			/**
			 * A flag indicating whether the checkbox should be manually disabled, independent of the table's read-only state.
			 */
			disabled: {
				type: Boolean,
				default: false
			}
		},

		expose: [],

		methods: {
			/**
			 * Handler for selecting the checkbox
			 */
			onSelect(event)
			{
				event.stopPropagation()

				const emitName = this.rowKey !== undefined ? 'toggle-row-selected' : 'toggle-all-rows-selected'
				this.$emit(emitName)
			},

			/**
			 * Mousedown handler
			 */
			onMousedown(event)
			{
				event.stopPropagation()
				event.preventDefault()
			}
		}
	}
</script>
