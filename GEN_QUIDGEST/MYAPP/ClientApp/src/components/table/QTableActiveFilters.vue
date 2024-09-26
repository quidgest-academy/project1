<template>
	<!-- BEGIN: Active Filters -->
	<div
		class="c-table__active-filters">
		<q-button
			v-for="(advancedFilter, advancedFilterIdx) in advancedFilters"
			:key="advancedFilterIdx"
			:style="{ 'background-color': advancedFilter.active ? null : 'white' }"
			:class="['e-badge', 'e-badge--filter', 'mb-1', 'advanced']"
			:title="getFilterName(advancedFilter, searchableColumns, texts.orText)"
			@click="editAdvancedFilters(advancedFilterIdx)">
			<span :style="{ opacity: advancedFilter.active ? '1' : '0.4' }">
				<q-icon
					icon="advanced-filters"
					class="search-filters-icon" />
				{{ getFilterName(advancedFilter, searchableColumns, texts.orText) }}
				<q-icon icon="pencil" />
			</span>
		</q-button>

		<q-button
			v-for="(columnFilter, columnFilterKey) in columnFilters"
			:key="columnFilterKey"
			:class="['e-badge', 'e-badge--filter', 'mb-1', 'column-filter']"
			:title="getFilterName(columnFilter, searchableColumns, texts.orText)"
			icon-on-right
			@click="removeColumnFilter(columnFilterKey)">
			{{ getFilterName(columnFilter, searchableColumns, texts.orText) }}
			<q-icon icon="remove" />
		</q-button>

		<q-button
			v-for="(searchBarFilter, searchBarFilterKey) in searchBarFilters"
			:key="searchBarFilterKey"
			:class="['e-badge', 'e-badge--filter', 'mb-1', 'search-bar-filter']"
			:title="getFilterName(searchBarFilter, searchableColumns, texts.orText)"
			icon-on-right
			@click="$emit('remove-search-bar-filter', searchBarFilterKey)">
			{{ getFilterName(searchBarFilter, searchableColumns, texts.orText) }}
			<q-icon icon="remove" />
		</q-button>

		<q-button
			v-if="hasFiltersActive"
			borderless
			:class="['e-badge--filter-remove', 'mb-1']"
			:label="texts.removeAllText"
			:title="texts.removeAllText"
			@click="removeCustomFilters" />
	</div>
	<!-- END: Active Filters -->
</template>

<script>
	import listFunctions from '@/mixins/listFunctions.js'

	import searchFilterDataModule from '@/api/genio/searchFilterData'

	export default {
		name: 'QTableActiveFilters',

		emits: [
			'set-property',
			'signal-component',
			'show-advanced-filters',
			'remove-column-filter',
			'remove-search-bar-filter',
			'remove-custom-filters',
			'update-config'
		],

		props: {
			/**
			 * Localized text strings to be used within the component for labels, titles, and accessibility.
			 */
			texts: {
				type: Object,
				required: true
			},

			/**
			 * Array of columns that can be searched, which determines which filters are applicable and how they are displayed.
			 */
			searchableColumns: {
				type: Array,
				default: () => []
			},

			/**
			 * Array of advanced filter objects, each containing specific filtering criteria intended for more complex queries.
			 */
			advancedFilters: {
				type: Array,
				default: () => []
			},

			/**
			 * Object where each key corresponds to a column's API field name, and its value is the filter applied to that column.
			 */
			columnFilters: {
				type: Object,
				default: () => ({})
			},

			/**
			 * Object mapping each field with a filter applied from the global search bar to its respective search criteria.
			 */
			searchBarFilters: {
				type: Object,
				default: () => ({})
			},

			/**
			 * Flag indicating whether any non-static filters are currently active, affecting the displayed set of data.
			 */
			hasFiltersActive: {
				type: Boolean,
				default: false
			},

			/**
			 * Predefined set of filter operators which describe how to apply filters to the data (e.g., 'equals', 'contains').
			 */
			filterOperators: {
				type: Object,
				default: () => searchFilterDataModule.operators.elements
			}
		},

		expose: [],

		methods: {
			/**
			 * Get name of filter, calculating if empty
			 * @param {Object} filter
			 * @param {Array} searchableColumns
			 * @param {string resource ID} orText
			 * @returns {String}
			 */
			getFilterName(filter, searchableColumns, orText)
			{
				return listFunctions.getFilterName(this.filterOperators, filter, searchableColumns, orText)
			},

			/**
			 * Edit advanced filters, highlighting selected filter
			 * @param {Number} advancedFilterIdx
			 */
			editAdvancedFilters(advancedFilterIdx)
			{
				this.$emit('signal-component', 'config', { show: true, selectedTab: 'advanced-filters' }, true)
				this.$emit('signal-component', 'advancedFilters', { selectedFilterIdx: advancedFilterIdx }, true)
			},

			/**
			 * Remove column filter
			 * @param {String} columnFilterKey
			 */
			removeColumnFilter(columnFilterKey)
			{
				this.$emit('remove-column-filter', columnFilterKey)
				this.$emit('update-config')
			},

			/**
			 * Remove custom filters
			 */
			removeCustomFilters()
			{
				this.$emit('remove-custom-filters')
				this.$emit('update-config')
			}
		}
	}
</script>
