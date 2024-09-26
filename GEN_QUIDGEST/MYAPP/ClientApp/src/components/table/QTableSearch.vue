<template>
	<div
		class="q-table-search col"
		v-click-outside="hideDropdown">
		<q-input-group size="block">
			<!-- TODO: remove and provide slot  -->
			<q-select
				v-if="searchableColumns.length > 0 && displayFiltersInList"
				v-model="dropdownValueAux"
				:items="getDdlOptions"
				:size="filterListSize"
				:disabled="disabled"
				@update:model-value="onDropdownItemSelect" />
			<!-- Main input -->
			<q-text-field
				v-model="searchValue"
				ref="globalSearch"
				role="searchbox"
				class="q-table-search__field"
				:placeholder="placeholder"
				:disabled="disabled || (!selectedFilter && displayFiltersInList && searchableColumns.length > 0 && !showSearchAllColumns)"
				@focusin="showDropdown"
				@keydown.stop="searchDropKeyPress">
				<template
					v-if="isClearBtnVisible"
					#append>
					<q-button
						borderless
						b-style="tertiary"
						size="small"
						:disabled="disabled"
						@click="resetQuery">
						<slot name="global-search-clear-icon">
							<q-icon icon="remove-sign" />
						</slot>
					</q-button>
				</template>
			</q-text-field>
			<!-- Columns  -->
			<div
				v-if="searchableColumns.length > 1 && !displayFiltersInList && dropdownShown"
				:id="searchFieldsId"
				class="search-field-menu dropdown-menu srch-fld-menu srch-fld-menu-vue"
				role="menu"
				tabindex="-1"
				x-placement="bottom-start"
				:style="[dropdownShown ? { display: 'block' } : { display: 'none' }]">
				<q-button
					v-for="(column, index) in searchableColumns"
					:key="index"
					ref="searchField"
					class="dropdown-item"
					role="menuitem"
					:data-search-field="column.field"
					@keydown.stop="searchDropKeyPress"
					@click.stop.prevent="searchByColumn(column, searchValue)">
					{{ texts.searchText }} <em>{{ column.label }}</em> {{ texts.forText }}:
					<strong>{{ searchValue }}</strong>
				</q-button>

				<template v-if="showSearchAllColumns">
					<q-button
						ref="searchAllFields"
						class="dropdown-item"
						role="menuitem"
						@keydown.stop="searchDropKeyPress"
						@click.stop.prevent="searchByAllColumns(searchValue)">
						{{ texts.searchText }} <span>{{ texts.allFieldsText }}</span> {{ texts.forText }}: <strong>{{ searchValue }}</strong>
					</q-button>
				</template>
			</div>
			<!-- BEGIN: Select search result -->
			<div
				v-if="searchResults.length > 0 && displayFiltersInList && dropdownShown"
				:id="searchFieldsId"
				class="search-field-menu dropdown-menu srch-fld-menu srch-fld-menu-vue"
				role="menu"
				tabindex="-1"
				x-placement="bottom-start"
				:style="[dropdownShown ? { display: 'block' } : { display: 'none' }]">
				<a
					v-for="(column, index) in searchResults"
					:key="index"
					class="dropdown-item"
					href="javascript:void(0)"
					role="menuitem"
					@click.stop.prevent="selectSearchResult(column.id)">
					<em>{{ column.label }}</em>
				</a>
			</div>

			<template #append>
				<template v-if="showRefreshButton">
					<q-button
						b-style="secondary"
						:title="texts.searchText"
						:disabled="disabled"
						@click="emitSearch(searchValue)">
						<slot name="refresh-button-text">
							<q-icon icon="search" />
						</slot>
					</q-button>
				</template>

				<!-- END: Search/refresh button -->
				<!-- BEGIN: Extra buttons -->
				<slot name="extra-buttons"></slot>
			</template>
		</q-input-group>
		<div
			v-if="searchError"
			class="btn-popover">
			<q-icon icon="exclamation-sign" />
			{{ emptyTextMessage }}
		</div>
	</div>
</template>

<script>
	export default {
		name: 'QTableSearch',

		emits: [
			'emit-search',
			'reset-query',
			'search-by-column',
			'search-by-all-columns',
			'value-changed',
			'dropdown-item-select',
			'result-selected'
		],

		props: {
			/**
			 * An object containing signals for managing the state and interactivity of the search component.
			 */
			signal: {
				type: Object,
				default: () => ({})
			},

			/**
			 * The title or name of the table which is being searched, used for accessibility and identifying the search context.
			 */
			tableTitle: {
				type: String,
				default: ''
			},

			/**
			 * An array of columns that can be individually searched.
			 */
			searchableColumns: {
				type: Array,
				required: true
			},

			/**
			 * Placeholder text for the global search input field.
			 */
			placeholder: {
				type: String,
				default: ''
			},

			/**
			 * Custom class names to apply to the search input field.
			 */
			classes: {
				type: String,
				default: ''
			},

			/**
			 * Flag indicating whether a refresh button should be shown next to the search input field.
			 */
			showRefreshButton: {
				type: Boolean,
				default: true
			},

			/**
			 * An object containing localized strings for displaying text within the component.
			 */
			texts: {
				type: Object,
				required: true
			},

			/**
			 * If the search query should be cleared upon performing a search operation.
			 */
			clearOnSearch: {
				type: Boolean,
				default: false
			},

			/**
			 * Determines whether the option to search across all columns should be available.
			 */
			showSearchAllColumns: {
				type: Boolean,
				default: true
			},

			/**
			 * The search value received as a property, which can be used to set the search input's value externally.
			 */
			searchPropValue: {
				type: String,
				default: ''
			},

			/**
			 * Determines whether the column filters should be displayed within the list dropdown.
			 */
			displayFiltersInList: {
				type: Boolean,
				default: false
			},

			/**
			 * The size of the filter list dropdown.
			 */
			filterListSize: {
				type: String,
				default: 'medium'
			},

			/**
			 * An array containing results from a search operation for displaying in a dropdown menu as selectable options.
			 */
			searchResults: {
				type: Array,
				default: () => []
			},

			/**
			 * Whether the search bar is disabled.
			 */
			disabled: {
				type: Boolean,
				default: false
			}
		},

		directives: {
			'click-outside': {
				mounted(el, binding) {
					el.clickOutsideEvent = function (event) {
						/*
						 * We need to use the .closest() to check if the user clicked in an option inside the dropdown
						 * Since it has position: absolute; we have to do it this way
						 */
						if (!this.searchFieldsId) {
							if (!(el === event.target || el.contains(event.target))) {
								binding.value(event, el)
							}
						} else if (!(el === event.target || el.contains(event.target) || event.target.closest(this.searchFieldsId))) {
							binding.value(event, el)
						}
					}
					document.addEventListener('click', el.clickOutsideEvent)
				},
				unmounted(el) {
					document.removeEventListener('click', el.clickOutsideEvent)
				}
			}
		},

		expose: [],

		data() {
			return {
				searchValue: '',
				dropdownShown: false,
				selectedFilter: '',
				dropdownValueAux: '',
				focusableDropdownElements: [],
				focusedDropdownElementIndex: 0,
				searchError: false
			}
		},

		mounted() {
			if (this.searchPropValue) {
				this.searchValue = this.searchPropValue
			}

			this.focusableDropdownElements = this.getFocusableDropdownElements()
		},

		computed: {
			isClearBtnVisible() {
				return this.searchValue.length > 0
			},

			/**
			 * DDL -> "DropDownList", refers to the DropDownInput (not the other dropdown)
			 */
			getDdlOptions() {
				// Return filters ready for DDL.
				return this.searchableColumns.map((e) => {
					return {
						key: e.field,
						label: e.label
					}
				})
			},

			searchFieldsId() {
				return this.tableTitle + '-srch-flds'
			},

			emptyTextMessage() {
				return this.texts.fieldIsRequired.replace('{0}', '')
			}
		},

		methods: {
			/**
			 * Search by a column for a value
			 * @param column {Object}
			 * @param value {String}
			 */
			searchByColumn(column, value) {
				this.hideDropdown()

				// Prevent creation of empty filters
				if (value === '') {
					this.searchError = true
					return
				}
				this.searchError = false

				// Clear search bar value since a filter is being added
				this.searchValue = ''

				this.$emit('search-by-column', column, value)
			},

			/**
			 * Search all columns for a value
			 * @param value {String}
			 */
			searchByAllColumns(value) {
				this.hideDropdown()

				// Prevent creation of empty filters
				if (value === '') {
					this.searchError = true
					return
				}
				this.searchError = false

				this.$emit('search-by-all-columns', value)
			},

			/**
			 * Search for a value
			 * @param value {String}
			 */
			emitSearch(value) {
				this.hideDropdown()

				// Prevent creation of empty filters
				if (value === '' || this.disabled) {
					this.searchError = true
					return
				}
				this.searchError = false

				if (!this.showSearchAllColumns) {
					// Only show dropdown
					this.showDropdown()
					return
				}

				if (this.clearOnSearch) this.searchValue = ''

				this.$emit('emit-search', value)
			},

			/**
			 * Emit event to reset search query
			 */
			resetQuery() {
				if (this.disabled) return

				this.searchValue = ''
				this.hideDropdown()

				this.$emit('reset-query')
			},

			/**
			 * Emit the event that correspond to the selected result in the dropdown
			 */
			selectSearchResult(id) {
				this.hideDropdown()
				this.$emit('result-selected', id)
			},

			/**
			 * Emit the dropdown item select event
			 */
			onDropdownItemSelect(data) {
				this.selectedFilter = data.value

				this.$emit('dropdown-item-select', data.value)
			},

			/**
			 * Hide dropdown with suggestion methods
			 */
			hideDropdown() {
				this.dropdownShown = false
				this.$nextTick().then(() => {
					this.focusableDropdownElements = this.getFocusableDropdownElements()
					this.focusedDropdownElementIndex = 0
				})
			},

			/**
			 * Show dropdown with suggestion methods
			 */
			showDropdown() {
				this.dropdownShown = true
				this.$nextTick().then(() => {
					this.focusableDropdownElements = this.getFocusableDropdownElements()
					this.focusedDropdownElementIndex = 0
				})
			},

			/**
			 * Toggle dropdown with suggestion methods
			 */
			toggleDropdown() {
				this.dropdownShown = !this.dropdownShown
			},

			/**
			 * Get focusable element in dropdown
			 */
			getFocusableDropdownElements() {
				let focusableElements = []

				// Search bar
				if (this.$refs.globalSearch) {
					focusableElements.push(this.$refs.globalSearch.inputRef)
				}

				// Search by field buttons
				if (this.$refs.searchField) {
					for(let idx in this.$refs.searchField)
						focusableElements.push(this.$refs.searchField[idx]?.$el)
				}

				// Search all fields button
				if (this.$refs.searchAllFields?.$el) {
					focusableElements.push(this.$refs.searchAllFields.$el)
				}

				return focusableElements
			},

			/**
			 * Focused on the searchbar element
			 */
			focusSearchbar() {
				this.$refs.globalSearch.$refs.inputRef.focus()
			},

			/**
			 * Get focused element index
			 * @param value {number}
			 */
			setFocusedDropdownElement(value) {
				this.focusedDropdownElementIndex = value
				if (!this.focusedDropdownElementIndex || isNaN(this.focusedDropdownElementIndex) || this.focusedDropdownElementIndex < 0) {
					this.focusedDropdownElementIndex = 0
				} else if (this.$refs.searchField && this.focusedDropdownElementIndex > this.$refs.searchField.length) {
					this.focusedDropdownElementIndex = this.$refs.searchField.length + 1
				}

				if (!this.focusableDropdownElements[this.focusedDropdownElementIndex]) return
				this.focusableDropdownElements[this.focusedDropdownElementIndex].focus()
			},

			/**
			 * Key handler for search dropdown
			 */
			searchDropKeyPress(event) {
				let key = event.key
				switch (key) {
					case 'Tab':
						this.hideDropdown()
						break
					case 'ArrowUp':
						event.preventDefault()
						this.focusedDropdownElementIndex--
						this.setFocusedDropdownElement(this.focusedDropdownElementIndex)
						break
					case 'ArrowDown':
						event.preventDefault()
						this.focusedDropdownElementIndex++
						this.setFocusedDropdownElement(this.focusedDropdownElementIndex)
						break
					case 'ArrowLeft':
					case 'ArrowRight':
						this.focusSearchbar()
						break
					case 'Enter':
						if (this.focusedDropdownElementIndex === 0) this.emitSearch(this.searchValue)
						break
				}
			}
		},

		watch: {
			searchPropValue(newVal) {
				this.searchValue = newVal
			},

			searchValue(newVal) {
				this.$emit('value-changed', newVal)
			},

			signal: {
				handler(newValue) {
					for (let key in newValue) {
						switch (key) {
							case 'resetQuery':
								if (newValue.resetQuery) this.resetQuery()
								break
							default:
								if (['searchError'].includes(key)) {
									this[key] = newValue[key]
								}
								break
						}
					}
				},
				deep: true
			}
		}
	}
</script>
