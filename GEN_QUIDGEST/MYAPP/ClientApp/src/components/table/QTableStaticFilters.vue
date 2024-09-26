<template>
	<!-- BEGIN: Static Filters -->
	<div
		:id="menuName + '_filters'"
		class="filters-container">
		<!-- BEGIN: Active Filters -->
		<div
			v-if="hasActiveFilters"
			class="active-filter-box"
			:data-testid="menuName + '_active_filters'">
			<!-- BEGIN: Checkbox options -->
			<label class="i-text__label">{{ texts.state }}:</label>
			<base-input-structure
				v-for="(filter, filterIndex) in activeFilters.options"
				:key="filterIndex"
				:id="filter.id"
				:label="filter.value"
				label-position="right"
				:label-attrs="{ class: 'i-checkbox i-checkbox__label' }">
				<template #label>
					<q-checkbox-input
						v-model="filter.selected"
						:id="filter.id"
						@update:model-value="updateFilterValues" />
				</template>
			</base-input-structure>
			<!-- END: Checkbox options -->
			<!-- BEGIN: Date -->
			<base-input-structure
				:id="activeFilters.dateValue.id"
				:class="['i-text', 'i-flex', { 'i-text--disabled': false }]"
				:label="texts.onDate"
				label-position="left"
				:label-attrs="{ class: 'i-text__label' }">
				<q-date-time-picker
					v-model="filterDateValue.value"
					:id="activeFilters.dateValue.id"
					size="small"
					format="date"
					@update:model-value="updateFilterValues" />
			</base-input-structure>
			<!-- END: Date -->
		</div>
		<!-- END: Active Filters -->
		<!-- BEGIN: Group Filters -->
		<div
			v-for="(entry, groupIndex) in groupFilters"
			:key="groupIndex"
			class="static-filter-box"
			:data-testid="menuName + '_group_filters'">
			<!-- BEGIN: Checkbox options -->
			<template v-if="groupFilterIsMultiple(entry)">
				<div
					class="form-check-inline"
					v-for="(filter, filterIndex) in entry.filters"
					:key="filterIndex">
					<base-input-structure
						:id="filter.id"
						:label="filter.value"
						label-position="right"
						:label-attrs="{ class: 'i-checkbox i-checkbox__label' }"
						:user-help="filter.userHelp">
						<template #label>
							<q-checkbox-input
								v-model="filter.selected"
								:id="filter.id"
								@update:model-value="updateFilterValues" />
						</template>
					</base-input-structure>
				</div>
			</template>
			<!-- END: Checkbox options -->
			<!-- BEGIN: Radio button options -->
			<q-radio-group
				v-else
				v-model="entry.value"
				:options-list="entry.filters"
				:number-of-columns="entry.filters.length"
				@update:model-value="updateFilterValues" />
			<!-- END: Radio button options -->
		</div>
		<!-- END: Group Filters -->
	</div>
	<!-- END: Static Filters -->
</template>

<script>
	export default {
		name: 'QTableStaticFilters',

		emits: ['on-update-filter'],

		inheritAttrs: false,

		props: {
			/**
			 * The unique name identifying this specific set of filters, typically associated with a table or view.
			 */
			menuName: {
				type: String,
				default: ''
			},

			/**
			 * An array representing groups of filters that apply globally to the data set, affecting all columns.
			 */
			groupFilters: {
				type: Array,
				default: () => []
			},

			/**
			 * An object representing active filters, which can be a mixture of various filter types including boolean or date.
			 */
			activeFilters: {
				type: Object,
				default: () => ({})
			},

			/**
			 * Localization or custom text strings that are used within the static filters interface, aiding in text consistency and localization.
			 */
			texts: {
				type: Object,
				default: () => ({})
			}
		},

		expose: [],

		data()
		{
			return {
				filterDateValue: { value: '' }
			}
		},

		mounted()
		{
			if (this.hasActiveFilters){
				this.filterDateValue = this.activeFilters.dateValue
				this.filterDateValue.value = new Date()
			}
		},

		computed: {
			/**
			 * Determine if group filters exist
			 * @returns Boolean
			 */
			hasGroupFilters()
			{
				return this.groupFilters.length > 0
			},

			/**
			 * Determine if active filter exists
			 * @returns Boolean
			 */
			hasActiveFilters()
			{
				return Object.keys(this.activeFilters).length > 0
			}
		},

		methods: {
			/**
			 * Determine if multiple filters can be selected in group of filters
			 * @param entry {Object}
			 * @returns Boolean
			 */
			groupFilterIsMultiple(entry)
			{
				if (entry.isMultiple === undefined)
					return false

				return entry.isMultiple
			},

			/**
			 * Emit value of radio button group
			 */
			updateFilterValues()
			{
				this.$emit('on-update-filter')
			}
		}
	}
</script>
