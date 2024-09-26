<template>
	<div
		:id="controlId"
		class="i-dbedit">
		<div
			:id="`list-${controlId}`"
			class="input-xlarge check-list-extended">
			<template
				v-for="option in getComputedRows"
				:key="option">
				<check-tag
					:option="option"
					:disabled="disabled"
					:primary-key-column-name="primaryKeyColumnName"
					:display-column-name="searchColumnName"
					@remove-label="removeLabel($event)" />
			</template>

			<q-button
				v-if="!disabled"
				b-style="primary"
				:class="['checklist--button', { 'disabled__checklist--button': getComputedDisabled }]"
				:title="texts.addButtonTitle"
				@click="onButtonClick">
				<q-icon icon="add" />
			</q-button>

			<span class="checklist__search--input">
				<input
					:id="`search-${controlId}`"
					v-show="searchVisible"
					v-model="search"
					ref="search"
					type="text"
					autocomplete="off"
					:disabled="disabled"
					:aria-hidden="!searchVisible"
					:title="texts.searchInputTitle"
					:placeholder="`${texts.placeholder} ${searchColumnLabel}`"
					@input="onChange($event)"
					@keydown="onkeydown($event)"
					@focus="onFocus" />
			</span>
		</div>
	</div>
</template>

<script>
	import _toLower from 'lodash-es/toLower'
	import _startsWith from 'lodash-es/startsWith'

	import { validateTexts } from '@/mixins/genericFunctions.js'
	import listFunctions from '@/mixins/listFunctions.js'

	import CheckTag from './CheckTag.vue'

	// The texts needed by the component.
	const DEFAULT_TEXTS = {
		placeholder: 'Search in',
		addButtonTitle: 'Insert',
		searchInputTitle: 'Search'
	}

	export default {
		name: 'QCheckListExtension',

		emits: [
			'remove-label',
			'on-enter'
		],

		components: {
			CheckTag
		},

		inheritAttrs: false,

		props: {
			/**
			 * Unique identifier for the control.
			 */
			id: String,

			/**
			 * Necessary strings to be used in labels and buttons.
			 */
			texts: {
				type: Object,
				validator: (value) => validateTexts(DEFAULT_TEXTS, value),
				default: () => DEFAULT_TEXTS
			},

			/**
			 * Whether the field is disabled.
			 */
			disabled: {
				type: Boolean,
				default: false
			},

			/**
			 * Object containing the name of primary key column name.
			 */
			primaryKeyColumnName: {
				type: String,
				required: true
			},

			/**
			 * Array containing the enumeration.
			 */
			options: {
				type: Array,
				required: true
			},

			/**
			 * The column name of the options which is to be used for searching.
			 */
			searchColumnName: {
				type: String,
				required: true
			},

			/**
			 * The label for the search column.
			 */
			searchColumnLabel: {
				type: String,
				default: ''
			},

			/**
			 * Contains the keys of the rows that have been selected.
			 */
			rowsSelected: {
				type: Object,
				default: () => ({})
			}
		},

		expose: [],

		data()
		{
			return {
				controlId: this.id || this._.uid,
				isBack: false,
				results: [],
				search: '',
				start: 0,
				searchVisible: false
			}
		},

		computed: {
			/**
			 * Computes rows that have been selected based on the keys.
			 */
			getComputedRows()
			{
				return listFunctions.getRowsFromKeyHash(this.options, this.rowsSelected)
			},

			/**
			 * Computes the disabled state for the list.
			 */
			getComputedDisabled()
			{
				return this.disabled ? true : this.options.length === this.getComputedRows.length
			}
		},

		methods: {
			/**
			 * Filters result based on the search query.
			 */
			filterResults()
			{
				this.start = this.search.length // set the starting point for selection
				if (!this.search)
					this.results = []
				else
				{
					this.results = this.options.filter((item) =>
						_startsWith(
							_toLower(
								listFunctions.getCellNameValue(item, this.searchColumnName)
							),
							_toLower(this.search)
						)
					)
				}
				// for next result
				if (
					this.results.length > 1 &&
					this.search === listFunctions.getCellNameValue(this.results[0], this.searchColumnName)
				) {
					this.results.shift()
				}
				// Suggest the result if search text match with option text
				if (this.results.length !== 0)
				{
					this.search = listFunctions.getCellNameValue(
						this.results[0],
						this.searchColumnName
					)
					this.setSelection()
				}
			},

			/**
			 * Sets the text selection in the input box.
			 */
			setSelection()
			{
				const stop = listFunctions.getCellNameValue(
					this.results[0],
					this.searchColumnName
				).length

				this.$nextTick().then(() => {
					this.$refs.search.setSelectionRange(this.start, stop)
				})
			},

			/**
			 * Handles various key down events for the input element.
			 * @param {KeyboardEvent} e - The KeyboardEvent object.
			 */
			onkeydown(e)
			{
				switch (e.key)
				{
					case 'Backspace':
						this.isBack = true
						break
					case 'Delete':
						this.search = this.$refs.search.value = ''
						break
					case 'ArrowRight':
						this.search = listFunctions.getCellNameValue(
							this.results[0],
							this.searchColumnName
						)
						break
					case 'Escape':
						this.search = ''
						this.searchVisible = false
						break
					case 'Enter':
						if (this.search && this.results.length !== 0)
						{
							this.searchVisible = false
							this.search = ''
							this.$emit(
								'on-enter',
								listFunctions.getCellNameValue(
									this.results[0],
									this.primaryKeyColumnName
								)
							)
						}
						break
					default:
						break
				}
			},

			/**
			 * Focuses input and selects all text inside it.
			 */
			onFocus()
			{
				this.$refs.search.setSelectionRange(0, this.search.length)
			},

			/**
			 * Filters results upon change in the search text unless the backspace key was used.
			 */
			onChange()
			{
				if (!this.isBack)
					this.filterResults()
				else
					this.isBack = false
			},

			/**
			 * Removes a label associated with a selected option.
			 * @param {string} primaryKey - The primary key of the option to remove.
			 */
			removeLabel(primaryKey)
			{
				this.$emit('remove-label', primaryKey)
			},

			/**
			 * Toggles the visibility of the search input and focuses it after visibility change.
			 */
			onButtonClick()
			{
				this.searchVisible = !this.searchVisible
				this.$nextTick().then(() => {
					this.$refs.search.focus()
				})
			}
		}
	}
</script>
