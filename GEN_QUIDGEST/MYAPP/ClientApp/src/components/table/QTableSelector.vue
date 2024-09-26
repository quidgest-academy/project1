<template>
	<div class="dropdown">
		<q-toggle-dropdown
			aria-expanded="false"
			aria-haspopup="true"
			:class="['dropdown-toggle']"
			:disabled="readonly || disableSelector"
			data-table-action-selected="false"
			tabindex="-1">
			<q-icon icon="unchecked" />
		</q-toggle-dropdown>

		<div
			class="dropdown-menu"
			x-placement="top-end"
			role="menu">
			<q-button
				role="menuitem"
				b-style="secondary"
				:class="['dropdown-item']"
				:label="texts.allRecordsText"
				:title="texts.allRecordsText"
				@click="$emit('check-all-rows')">
				<q-icon icon="apply" />
			</q-button>

			<q-button
				role="menuitem"
				b-style="secondary"
				:class="['dropdown-item']"
				:label="texts.currentPageText"
				:title="texts.currentPageText"
				@click="$emit('check-current-page-rows')">
				<q-icon icon="check" />
			</q-button>

			<q-button
				role="menuitem"
				b-style="secondary"
				:class="['dropdown-item']"
				:label="texts.noneText"
				:title="texts.noneText"
				@click="$emit('check-none-rows')">
				<q-icon icon="remove" />
			</q-button>
		</div>
	</div>
</template>

<script>
	import QToggleDropdown from '@/components/QToggleDropdown.vue'

	export default {
		name: 'QTableSelector',

		emits: [
			'check-all-rows',
			'check-current-page-rows',
			'check-none-rows'
		],

		components: {
			QToggleDropdown
		},

		props: {
			/**
			 * An object containing localized text strings for actions related to selection of table rows.
			 */
			texts: {
				type: Object,
				required: true
			},

			/**
			 * A boolean indicating whether the table is in a read-only state, which can disable the selector functionality.
			 */
			readonly: {
				type: Boolean,
				default: false
			},

			/**
			 * A boolean that indicates if the row selection options should be disabled, irrespective of table read-only state.
			 */
			disableSelector: {
				type: Boolean,
				default: false
			}
		},

		expose: []
	}
</script>
