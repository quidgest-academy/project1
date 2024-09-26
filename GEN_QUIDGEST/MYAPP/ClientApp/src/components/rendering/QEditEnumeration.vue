<template>
	<component
		:is="options?.component ? options.component : 'base-input-structure'"
		:class="containerClasses"
		:label-attrs="{ class: 'i-text__label' }"
		:model-field-ref="modelField"
		:error-display-type="options?.errorDisplayType">
		<q-select
			:model-value="value?.key"
			:size="size"
			:class="classes"
			:items="items"
			clearable
			item-value="key"
			item-label="value"
			@update:model-value="updateSelectedValue" />
	</component>
</template>

<script>
	import _isEmpty from 'lodash-es/isEmpty'

	import { inputSize } from '@/mixins/quidgest.mainEnums.js'
	import modelFieldType from '@/mixins/formModelFieldTypes.js'

	import BaseInputStructure from '@/components/inputs/BaseInputStructure.vue'
	import GridBaseInputStructure from '@/components/inputs/GridBaseInputStructure.vue'

	export default {
		name: 'QEditEnumeration',

		emits: ['update', 'loaded'],

		components: {
			BaseInputStructure,
			GridBaseInputStructure
		},

		props: {
			/**
			 * The current selected value of the enumeration, represented as an object with a key-value pair.
			 */
			value: {
				type: Object,
				default: () => ({})
			},

			/**
			 * The name of the table in the database, used to identify the field context.
			 */
			tableName: {
				type: String,
				required: true
			},

			/**
			 * The index of the current row in the table, used to identify the field context.
			 */
			rowIndex: {
				type: [Number, String],
				required: true
			},

			/**
			 * The name of the column in the database, used to identify the field context.
			 */
			columnName: {
				type: String,
				required: true
			},

			/**
			 * A set of options to configure the enumeration field, such as the component type and error display.
			 */
			options: {
				type: Object,
				default: () => ({})
			},

			/**
			 * Sizing class for the control, based on predefined options.
			 */
			size: {
				type: String,
				validator: (value) => _isEmpty(value) || Reflect.has(inputSize, value)
			},

			/**
			 * Additional classes to apply to the control.
			 */
			classes: {
				type: Array,
				default: () => []
			},

			/**
			 * Classes to be applied to the control's container.
			 */
			containerClasses: {
				type: Array,
				default: () => []
			},

			/**
			 * A list of error messages associated with the control.
			 */
			errorMessages: {
				type: Array,
				default: () => []
			}
		},

		expose: [],

		data()
		{
			return {
				modelField: new modelFieldType.String()
			}
		},

		mounted()
		{
			this.$emit('loaded')
		},

		computed: {
			/**
			 * Transforms the distinctValues object or array from options into the format expected by QSelect.
			 */
			items()
			{
				if (!this.options.distinctValues)
					return this.options.array

				const optionsArr = []
				const optionsObj = this.options.distinctValues

				const parseKey = this.options.arrayType === 'N' || this.options.arrayType === 'L'

				for (let key in optionsObj)
				{
					if (key.length < 1)
						continue

					const value = optionsObj[key]
					const arrKey = this.options.keyIsValue ? value : (parseKey ? parseInt(key) : key)

					optionsArr.push({ key: arrKey, value: value })
				}

				return optionsArr
			}
		},

		methods: {
			/**
			 * Emits the search query for the QSelect component when the user performs a search.
			 * @param {string} value - The search query entered by the user.
			 */
			updateSearchQuery(value)
			{
				this.searchValue = value
			},

			/**
			 * Updates the currently selected value in the QSelect component and emits it to the parent component.
			 * @param {string|number} key - The key representing the selected enumeration option.
			 */
			updateSelectedValue(key)
			{
				const option = this.items.find((option) => option.key === key)
				this.$emit('update', option)
			}
		},

		watch: {
			errorMessages: {
				handler(newValue)
				{
					this.modelField.serverErrorMessages = newValue
				},
				deep: true
			}
		}
	}
</script>
