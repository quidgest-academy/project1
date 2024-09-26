<template>
	<div
		:id="controlId"
		class="q-table-list property-list-container">
		<div class="table-responsive-wrapper text-nowrap">
			<div
				class="table-responsive"
				style="overflow: visible !important">
				<table class="c-table">
					<thead class="c-table__head">
						<tr>
							<th>
								<div
									class="column-header-content"
									style="outline: none"
									tabindex="0">
									<span
										style="cursor: pointer"
										:title="texts.groupingButtonTitle"
										@click.stop.prevent="checkGrouping">
										<q-icon icon="list" />
									</span>

									<span
										v-if="showGrouping && (!readonly || valueEditMode)"
										:aria-hidden="readonly || !valueEditMode"
										:title="texts.editButtonTitle"
										style="cursor: pointer"
										@click.stop.prevent="editClick">
										<q-icon icon="pencil" />
									</span>

									<span
										v-if="showGrouping"
										style="cursor: pointer"
										class="float-right column-sort-icon">
										<div
											:title="texts.sortingButtonTitle"
											@click.stop.prevent="sort">
											{{ order === 'asc' ? '&#x1F825;' : order === 'desc' ? '&#x1F827;' : '&#x1F825;&#x1F827;' }}
										</div>
									</span>

									<div
										v-if="showGrouping"
										:id="`property-list__dropdown-${controlId}`"
										class="nested__dropdown"
										:aria-hidden="!showGrouping">
										<q-button
											b-style="secondary"
											:class="['dropdown-trigger', 'nested__dropdown']"
											:name="texts.addButtonTitle"
											:title="texts.addButtonTitle"
											@click="toggleDropdown">
											<div class="d-flex dropdown-icon-group">
												<q-icon icon="plus" />
												<q-icon icon="expand" />
											</div>
										</q-button>

										<ul
											v-if="openDropdown"
											class="dropdown-list property-list__header-dropdown"
											role="listbox"
											:aria-expanded="openDropdown">
											<li
												v-for="(item, i) in typeSelectOptions"
												role="option"
												:key="i"
												:class="[{ 'dropdown-submenu': item.children }, 'dropdown_item']"
												@click.stop.prevent="selectType(item)">
												{{ item.key }}
											</li>
										</ul>
									</div>
								</div>
							</th>

							<th v-if="!showGrouping">
								<div
									class="column-header-content"
									style="outline: none"
									tabindex="0">
									<span
										v-show="!readonly || valueEditMode"
										style="cursor: pointer"
										:aria-hidden="readonly"
										:title="texts.editButtonTitle"
										@click.stop.prevent="editClick">
										<q-icon icon="pencil" />
									</span>

									<span
										style="cursor: pointer"
										class="float-right column-sort-icon">
										<div
											:title="texts.sortingButtonTitle"
											@click.stop.prevent="sort">
											{{ order === 'asc' ? '&#x1F825;' : order === 'desc' ? '&#x1F827;' : '&#x1F825;&#x1F827;' }}
										</div>
									</span>

									<div
										:id="`property-list__dropdown-${controlId}`"
										class="nested__dropdown">
										<q-button
											b-style="secondary"
											:class="['dropdown-trigger', 'nested__dropdown']"
											:name="texts.addButtonTitle"
											:title="texts.addButtonTitle"
											@click="toggleDropdown">
											<div class="d-flex dropdown-icon-group">
												<q-icon icon="plus" />
												<q-icon icon="expand" />
											</div>
										</q-button>

										<ul
											v-if="openDropdown"
											role="listbox"
											class="dropdown-list property-list__header-dropdown"
											:aria-expanded="openDropdown">
											<li
												v-for="(item, i) in typeSelectOptions"
												role="option"
												:key="i"
												:class="[{ 'dropdown-submenu': item.children }, 'dropdown_item']"
												@click.stop.prevent="selectType(item)">
												{{ item.key }}
											</li>
										</ul>
									</div>
								</div>
							</th>
						</tr>
					</thead>

					<tbody class="c-table__body">
						<template v-if="groupingField && showGrouping">
							<template
								v-for="(group, i) in groupedRows"
								:key="`${i}-${controlId}`">
								<tr @click.stop.prevent="expandGroup(group)">
									<td>
										<a
											href="javascript:void(0)"
											class="column-data-link"
											style="margin-left: 8px">
											{{ i }}

											<span>
												<q-icon
													v-if="group.length > 0 && group.visible"
													icon="step-forward" />
												<q-icon
													v-else
													icon="expand" />
											</span>
										</a>
									</td>
								</tr>

								<q-timeline-collapsible :show="group.length > 0 && group.visible">
									<tr
										v-for="(row, j) in group"
										:key="`${j}-${controlId}`">
										<td>
											<q-property-list-dropdown
												v-model="row.Fields.Property"
												:texts="texts"
												:show-dropdown="isEditable && !valueEditMode"
												:menu-items="editPropertyMenuItems"
												@select-type="dropdownSelectedType(row, $event)"
												@select-action="dropdownSelectedAction(row, $event)"
												@updated-property-label="updateProperty(row, $event)" />
										</td>

										<td
											class="property-list_value_column"
											:style="isEditable ? 'padding: 0' : ''">
											<span v-if="!isEditable">
												{{ getCellValue(row, valueColumnName) }}
											</span>
											<component
												v-else
												:is="
													getCellValue(row, 'type') === 'Text'
														? 'QTextInput'
														: getCellValue(row, 'type') === 'Boolean'
															? 'QCheckBoxInput'
															: getCellValue(row, 'type') === 'Number'
																? 'QNumericInput'
																: getCellValue(row, 'type') === 'Date'
																	? 'QDateTimePicker'
																	: getCellValue(row, 'type') === 'Enumeration'
																		? 'QSelect'
																		: ''
												"
												v-model="row.Fields.Val"
												class="property-list__value--inputs"
												:filter="false"
												:table-name="`property-list-${controlId}`"
												:readonly="!isEditable"
												:options="getCellValue(row, optionColumnName)"
												:value="getCellValue(row, valueColumnName)"
												:row-key="getCellValue(row, primaryKeyColumnName)"
												:key="getCellValue(row, primaryKeyColumnName)"
												:placeholder="getCellValue(row, 'placeholder')"
												:option-value="getCellValue(row, optionValueColumnName)"
												:option-label="getCellValue(row, optionLabelColumnName)"
												:max-length="maxCharacters"
												:format="format"
												:size="size"
												:date-format="dateFormat"
												:style="removeBorder ? 'border: none' : ''"
												@input="updateValue(row, $event)"
												@update:model-value="updateValue(row, $event)"
												@on-select="onSelect(row, $event)" />
										</td>
									</tr>
								</q-timeline-collapsible>
							</template>
						</template>
						<template v-else>
							<tr
								v-for="(row, i) in sortRows"
								:key="`${i}-${controlId}`">
								<td>
									<q-property-list-dropdown
										v-model="row.Fields.Property"
										:texts="texts"
										:show-dropdown="isEditable && !valueEditMode"
										:menu-items="editPropertyMenuItems"
										@select-type="dropdownSelectedType(row, $event)"
										@select-action="dropdownSelectedAction(row, $event)"
										@updated-property-label="updateProperty(row, $event)" />
								</td>

								<td
									class="property-list_value_column"
									:style="isEditable ? 'padding: 0' : ''">
									<span v-if="!isEditable">
										{{ getCellValue(row, valueColumnName) }}
									</span>
									<component
										v-else
										:is="
											getCellValue(row, 'type') === 'Text'
												? 'QTextInput'
												: getCellValue(row, 'type') === 'Boolean'
													? 'QCheckBoxInput'
													: getCellValue(row, 'type') === 'Number'
														? 'QNumericInput'
														: getCellValue(row, 'type') === 'Date'
															? 'QDateTimePicker'
															: getCellValue(row, 'type') === 'Enumeration'
																? 'QSelect'
																: ''
										"
										v-model="row.Fields.Val"
										class="property-list__value--inputs"
										:table-name="`property-list-${controlId}`"
										:readonly="!isEditable"
										:options="getCellValue(row, optionColumnName)"
										:filter="false"
										:value="getCellValue(row, valueColumnName)"
										:row-key="getCellValue(row, primaryKeyColumnName)"
										:key="getCellValue(row, primaryKeyColumnName)"
										:placeholder="getCellValue(row, 'placeholder')"
										:option-value="getCellValue(row, optionValueColumnName)"
										:option-label="getCellValue(row, optionLabelColumnName)"
										:max-length="maxCharacters"
										:size="size"
										:format="format"
										:date-format="dateFormat"
										:style="removeBorder ? 'border: none' : ''"
										@input="updateValue(row, $event)"
										@on-select="onSelect(row, $event)"
										@update:model-value="updateValue(row, $event)" />
								</td>
							</tr>
						</template>

						<tr
							v-if="selectedDropdownOption"
							class="property-list_addnew"
							:aria-hidden="!selectedDropdownOption">
							<td class="property-list--relative grouping">
								<div class="add__new-input-group">
									<q-control-wrapper class="control-join-group">
										<q-text-field
											v-model="addProperty"
											:size="size"
											:label="texts.addPropertyPlaceholder"
											:title="texts.addPropertyPlaceholder"
											@keydown.stop="submitAddNew" />
									</q-control-wrapper>

									<q-control-wrapper
										v-if="showGrouping"
										class="control-join-group">
										<base-input-structure
											v-if="selectedDropdownOption.value !== 'CheckBoxInput'"
											:id="`property-list-addvalue-${controlId}`"
											:class="['i-text']"
											:label="texts.addValuePlaceholder"
											:label-attrs="{ class: 'i-text__label' }">
											<component
												v-model="userInputModel"
												:is="selectedDropdownOption.value"
												:size="size"
												:class="selectedDropdownOption.value === 'CheckBoxInput' ? 'property-list-i-checkbox' : ''"
												:format="format"
												:date-format="dateFormat"
												:table-name="`property-list-${controlId}`"
												@keydown.stop="submitAddNew"
												@update:model-value="dateChange" />
										</base-input-structure>
										<base-input-structure
											v-else
											:id="`property-list-addvalue-${controlId}`"
											:label="texts.addValuePlaceholder"
											:label-attrs="{ class: 'i-checkbox i-checkbox__label' }">
											<template #label>
												<q-checkbox-input
													:id="`property-list-addvalue-${controlId}`"
													v-model="userInputModel"
													@keydown.stop="submitAddNew" />
											</template>
										</base-input-structure>
									</q-control-wrapper>

									<q-button
										v-if="showGrouping"
										b-style="secondary"
										:class="['float-right']"
										:name="texts.addButtonTitle"
										:title="texts.addButtonTitle"
										@click="onButtonSubmitAddNew">
										<q-icon icon="add" />
									</q-button>
								</div>

								<div
									v-if="showGrouping"
									class="close_addnew-circle close_add-new"
									@click.stop.prevent="closeAddNew">
									<q-icon icon="remove" />
								</div>
							</td>

							<td
								v-if="!showGrouping"
								class="property-list--relative">
								<div class="property-list-addnew-cell">
									<q-control-wrapper class="control-join-group">
										<base-input-structure
											v-if="selectedDropdownOption.value !== 'CheckBoxInput'"
											:id="`property-list-addvalue-${controlId}`"
											:class="['i-text']"
											:label="texts.addValuePlaceholder"
											:label-attrs="{ class: 'i-text__label' }">
											<component
												:is="selectedDropdownOption.value"
												:size="size"
												:class="selectedDropdownOption.value === 'CheckBoxInput' ? 'property-list-i-checkbox' : ''"
												v-model="userInputModel"
												:format="format"
												:date-format="dateFormat"
												:table-name="`property-list-${controlId}`"
												@keydown.stop="submitAddNew"
												@update:model-value="dateChange" />
										</base-input-structure>
										<base-input-structure
											v-else
											:id="`property-list-addvalue_checkbox-grouping-${controlId}`"
											:label="texts.addValuePlaceholder"
											:label-attrs="{ class: 'i-checkbox i-checkbox__label' }">
											<template #label>
												<q-checkbox-input
													:id="`property-list-addvalue_checkbox-grouping-${controlId}`"
													v-model="userInputModel"
													@keydown.stop="submitAddNew" />
											</template>
										</base-input-structure>
									</q-control-wrapper>

									<q-button
										b-style="secondary"
										:class="['float-right']"
										:name="texts.addButtonTitle"
										:title="texts.addButtonTitle"
										@click="onButtonSubmitAddNew">
										<q-icon icon="add" />
									</q-button>
								</div>

								<div
									class="close_addnew-circle close_add-new"
									@click.stop.prevent="closeAddNew">
									<q-icon icon="remove" />
								</div>
							</td>
						</tr>
					</tbody>
				</table>
			</div>
		</div>
	</div>
</template>

<script>
	import _isEmpty from 'lodash-es/isEmpty'

	import { validateTexts } from '@/mixins/genericFunctions.js'
	import { getCellNameValue } from '@/mixins/listFunctions.js'
	import { inputSize } from '@/mixins/quidgest.mainEnums.js'

	import QTimelineCollapsible from '@/components/timeline/QTimelineCollapsible.vue'
	import QPropertyListDropdown from './QPropertyListDropdown.vue'

	// The texts needed by the component.
	const DEFAULT_TEXTS = {
		addValuePlaceholder: 'Insert value',
		headerDropdownPlaceholder: 'Select type',
		addPropertyPlaceholder: 'Property name',
		sortingButtonTitle: 'Sort',
		editButtonTitle: 'Edit',
		groupingButtonTitle: 'Group',
		propertyHeaderTitle: 'Property',
		valueHeaderTitle: 'Value',
		addButtonTitle: 'Add',
		placeholder: 'Choose...',
		insertLabel: 'Insert',
		dropdownTitle: 'Menu',
		moreOptionLabel: 'See more',
		hiddenElem: 'Delete',
		filterPlaceholder: 'Search'
	}

	export default {
		name: 'QPropertyList',

		emits: [
			'update-value',
			'add-property',
			'delete-property',
			'edit-property',
			'update-type',
			'update-property-label',
		],

		components: {
			QTimelineCollapsible,
			QPropertyListDropdown
		},

		inheritAttrs: false,

		props: {
			/**
			 * Unique identifier for the control.
			 */
			id: String,

			/**
			 * Name of field for grouping
			 */
			groupingField: String,

			/**
			 * Necessary strings to be used in labels and buttons.
			 */
			texts: {
				type: Object,
				validator: (value) => validateTexts(DEFAULT_TEXTS, value),
				default: () => DEFAULT_TEXTS
			},

			/**
			 * Whether or not the list should have a border.
			 */
			removeBorder: {
				type: Boolean,
				default: true
			},

			/**
			 * The list rows.
			 */
			rows: {
				type: Array,
				default: () => {
					return []
				}
			},

			/**
			 * Name of primary key column name
			 */
			primaryKeyColumnName: {
				type: String,
				required: true
			},

			/**
			 * Name of value column name
			 */
			valueColumnName: {
				type: String,
				required: true
			},

			/**
			 * Name of option field name for Enumeration type
			 */
			optionColumnName: {
				type: String,
				default: 'options'
			},

			/**
			 * Name of optionValue  field name for Enumeration type
			 */
			optionValueColumnName: {
				type: String,
				default: 'optionValue'
			},

			/**
			 * Name of option label  field name for Enumeration type
			 */
			optionLabelColumnName: {
				type: String,
				default: 'optionLabel'
			},

			/**
			 * Maximum number of character for the string
			 */
			maxCharacters: {
				type: Number,
				default: -1
			},

			/**
			 * Configures whether the control is editable or not
			 */
			readonly: {
				type: Boolean,
				default: false
			},

			/**
			 * Configures whether the value edit mode
			 */
			valueEditMode: {
				type: Boolean,
				default: false
			},

			/**
			 * Sizing class for the control
			 */
			size: {
				type: String,
				default: 'small',
				validator: (value) => {
					return _isEmpty(value) || Reflect.has(inputSize, value)
				}
			},

			/**
			 * format of the control, {D : Date}, {T : Time}, {DT : DateTime}, {DS : DateTimeSeconds}
			 */
			format: {
				type: String,
				default: 'dateTime',
				validator: (propValue) => {
					return ['date', 'dateTime', 'dateTimeSeconds', 'time'].includes(propValue)
				}
			},

			/**
			 * The format to be used for date/time portion
			 */
			dateFormat: {
				type: Object,
				default: () => {
					return {
						date: 'dd/MM/yyyy',
						dateTime: 'dd/MM/yyyy HH:mm',
						dateTimeSeconds: 'dd/MM/yyyy HH:mm:ss',
						time: 'HH:mm'
					}
				}
			},

			/**
			 * Options for type select dropdown
			 */
			typeSelectOptions: {
				type: Array,
				default: () => {
					return []
				}
			},

			/**
			 * Options for type edit property dropdown
			 */
			editPropertyMenuItems: {
				type: Array,
				default: () => {
					return []
				}
			}
		},

		expose: [],

		data()
		{
			return {
				controlId: this.id || `i_text_${this._.uid}`,
				isEditable: false,
				order: '',
				addCloseDropdown: false,
				showGrouping: false,
				groupedRows: [],
				sortRows: [],
				openDropdown: false,
				editPropertyLable: false,
				userInputModel: '',
				selectedDropdownOption: '',
				addProperty: ''
			}
		},

		mounted()
		{
			document.addEventListener('click', this.closeDropdown)
		},

		beforeUnmount()
		{
			document.removeEventListener('click', this.closeDropdown)
		},

		methods: {
			sort()
			{
				if (this.order === 'asc' || this.order === '')
					this.sortAsc()
				else
					this.sortDesc()
			},

			sortAsc()
			{
				if (this.groupingField && this.showGrouping)
				{
					this.groupedRows = Object.keys(this.groupedRows)
						.sort((a, b) => {
							return this.compareOrder(a, b)
						})
						.reduce((obj, key) => {
							obj[key] = this.groupedRows[key]
							return obj
						}, {})
				}
				else
				{
					this.sortRows.sort((a, b) => {
						return this.compareOrder(a, b)
					})
				}
				this.order = 'desc'
			},

			sortDesc()
			{
				if (this.groupingField && this.showGrouping)
				{
					this.groupedRows = Object.keys(this.groupedRows)
						.sort((a, b) => {
							return this.compareOrder(a, b)
						})
						.reduce((obj, key) => {
							obj[key] = this.groupedRows[key]
							return obj
						}, {})
				}
				else
				{
					this.sortRows.sort((a, b) => {
						return this.compareOrder(a, b)
					})
				}
				this.order = 'asc'
			},

			compareOrder(a, b)
			{
				if (this.showGrouping
					? a.toLowerCase() < b.toLowerCase()
					: getCellNameValue(a, 'Property').toLowerCase() < getCellNameValue(b, 'Property').toLowerCase())
					return this.order === 'desc' ? 1 : -1
				if (this.showGrouping
					? a.toLowerCase() > b.toLowerCase()
					: getCellNameValue(a, 'Property').toLowerCase() > getCellNameValue(b, 'Property').toLowerCase())
					return this.order === 'desc' ? -1 : 1
				return 0
			},

			checkGrouping()
			{
				this.showGrouping = !this.showGrouping
				this.setGroupingRows()
			},

			groupByKey(array, key)
			{
				return array.reduce((result, currentValue) => {
					if (!currentValue['Fields'][key])
						return result

					return Object.assign(result, {
						[currentValue['Fields'][key]]: (result[currentValue['Fields'][key]] || []).concat(currentValue)
					})
				}, {})
			},

			onSelect(row, e)
			{
				const valueData = {
					PrimaryKey: getCellNameValue(row, this.primaryKeyColumnName),
					Value: e.value
				}

				this.$emit('update-value', valueData)
			},

			updateProperty(row)
			{
				const propertyLabelData = {
					PrimaryKey: getCellNameValue(row, this.primaryKeyColumnName),
					Value: getCellNameValue(row, 'Property')
				}

				this.$emit('update-property-label', propertyLabelData)
			},

			updateValue(row)
			{
				if (row.Fields.type === 'Boolean')
					row.Fields.Val = !row.Fields.Val

				const valueData = {
					PrimaryKey: getCellNameValue(row, this.primaryKeyColumnName),
					Value: getCellNameValue(row, 'Val')
				}

				this.$emit('update-value', valueData)
			},

			editClick()
			{
				if (this.valueEditMode || !this.readonly)
					this.isEditable = !this.isEditable
			},

			dropdownSelectedType(row, type)
			{
				const typeData = {
					PrimaryKey: getCellNameValue(row, this.primaryKeyColumnName),
					newType: type.name
				}

				this.$emit('update-type', typeData)
			},

			dropdownSelectedAction(row, type)
			{
				if (type.action === 'edit')
					this.editPropertyLable = !this.editPropertyLable

				if (type.action === 'delete')
					this.$emit('delete-property', getCellNameValue(row, this.primaryKeyColumnName))
			},

			expandGroup(group)
			{
				group.visible = !group.visible
			},

			getCellValue(row, columnName)
			{
				return getCellNameValue(row, columnName)
			},

			dateChange(e)
			{
				this.userInputModel = e.target.value
			},

			submitAddNew(e)
			{
				switch (e.which)
				{
					// Enter
					case 13:
						this.$emit('add-property', {
							Property: this.addProperty,
							Value: this.userInputModel,
							type:
								this.selectedDropdownOption.value === 'QTextInput'
									? 'Text'
									: this.selectedDropdownOption.value === 'QNumericInput'
										? 'Number'
										: this.selectedDropdownOption.value === 'QDateTimePicker'
											? 'Date'
											: this.selectedDropdownOption.value === 'CheckBoxInput'
												? 'Boolean'
												: 'Enumeration'
						})

						this.userInputModel = this.selectedDropdownOption.value === 'CheckBoxInput' ? false : ''
						this.addProperty = ''
						break
					default:
						break
				}
			},

			closeDropdown(e)
			{
				if (!this.$el.contains(e.target))
					this.openDropdown = false
			},

			onButtonSubmitAddNew()
			{
				this.$emit('add-property', {
					Property: this.addProperty,
					Value: this.userInputModel,
					type:
						this.selectedDropdownOption.value === 'QTextInput'
							? 'Text'
							: this.selectedDropdownOption.value === 'QNumericInput'
								? 'Number'
								: this.selectedDropdownOption.value === 'QDateTimePicker'
									? 'Date'
									: this.selectedDropdownOption.value === 'CheckBoxInput'
										? 'Boolean'
										: 'Enumeration'
				})

				this.userInputModel = this.selectedDropdownOption.value === 'CheckBoxInput' ? false : ''
				this.addProperty = ''
				this.selectedDropdownOption = null
			},

			toggleDropdown()
			{
				this.openDropdown = !this.openDropdown
			},

			selectType(option)
			{
				this.userInputModel =
					option.value === 'QNumericInput'
						? 0
						: option.value === 'CheckBoxInput'
							? false
							: ''
				this.selectedDropdownOption = option
				this.openDropdown = false
			},

			closeAddNew()
			{
				this.userInputModel = this.selectedDropdownOption.value === 'CheckBoxInput' ? false : ''
				this.addProperty = ''
				this.selectedDropdownOption = null
			},

			setGroupingRows()
			{
				if (this.groupingField && this.showGrouping)
				{
					this.groupedRows = this.groupByKey(this.sortRows, this.groupingField)
					Object.entries(this.groupedRows).forEach(([key]) => {
						this.groupedRows[key].visible = false
					})
				}
			}
		},

		watch: {
			rows: {
				handler(val)
				{
					this.sortRows = [...val]
					this.setGroupingRows()
				},
				immediate: true
			}
		}
	}
</script>
