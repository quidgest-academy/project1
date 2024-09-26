<template>
	<q-control-wrapper class="form-check-columns">
		<div
			v-for="(column, columnId) in columnList"
			:key="columnId"
			class="column">
			<template
				v-for="item in column"
				:key="item.itemId">
				<base-input-structure
					:id="item.itemId"
					class="i-checkbox"
					:disabled="disabled"
					:readonly="readonly"
					:label="item.option.value"
					:label-position="labelPosition"
					:label-attrs="{ class: 'i-checkbox i-checkbox__label' }">
					<template #label>
						<q-checkbox-input
							:id="item.itemId"
							v-model="item.isChecked"
							:disabled="disabled"
							:readonly="readonly" />
					</template>
				</base-input-structure>
			</template>
		</div>
	</q-control-wrapper>
</template>

<script>
	import _isEmpty from 'lodash-es/isEmpty'
	import _filter from 'lodash-es/filter'
	import _findIndex from 'lodash-es/findIndex'
	import _map from 'lodash-es/map'

	import { inputSize, labelAlignment } from '@/mixins/quidgest.mainEnums'

	import QControlWrapper from '@/components/ControlWrapper.vue'
	import BaseInputStructure from '@/components/inputs/BaseInputStructure.vue'
	import QCheckboxInput from '@/components/inputs/CheckBoxInput.vue'

	export default {
		name: 'QCheckboxGroup',

		emits: ['update:modelValue'],

		components: {
			QControlWrapper,
			BaseInputStructure,
			QCheckboxInput
		},

		inheritAttrs: false,

		props: {
			/**
			 * Unique identifier for the control.
			 */
			id: String,

			/**
			 * The string vaue to be edited by the input
			 */
			modelValue: {
				type: Array,
				required: true
			},

			/**
			 * The list of the options
			 */
			options: {
				type: Array,
				default: () => []
			},

			/**
			 * Number of columns
			 */
			numberOfColumns: {
				type: Number,
				default: 0
			},

			/**
			 * Sizing class for the control
			 */
			size: {
				type: String,
				validator: (value) => _isEmpty(value) || Reflect.has(inputSize, value)
			},

			/**
			 * The label position
			 */
			labelPosition: {
				type: String,
				default: '',
				validator: (value) => _isEmpty(value) || Reflect.has(labelAlignment, value)
			},

			/**
			 * Whether the field is disabled.
			 */
			disabled: {
				type: Boolean,
				default: false
			},

			/**
			 * Whether the field is readonly.
			 */
			readonly: {
				type: Boolean,
				default: false
			},

			/**
			 * Whether the field is required.
			 */
			isRequired: {
				type: Boolean,
				default: false
			}
		},

		expose: [],

		data()
		{
			return {
				/**
				 * The unique control identifier
				 */
				controlId: this.id || this._.uid,
				columnList: {}
			}
		},

		created()
		{
			this.setData()
		},

		methods: {
			setData()
			{
				if (!_isEmpty(this.options))
				{
					let vm = this,
						numColumns = this.numberOfColumns === 0 ? this.options.length : this.numberOfColumns,
						midCount = Math.ceil(this.options.length / numColumns)

					if (!Number.isSafeInteger(midCount))
						midCount = 1

					this.columnList =  {}

					for (let col = 0; col < numColumns; col++)
					{
						Reflect.set(this.columnList, `column-${this.controlId}-${col}`,
							_map(this.options.slice(col * midCount, col * midCount + midCount),
								(option) => {
									return {
										get isChecked()
										{
											return _findIndex(vm.modelValue || [], o => o === this.option.key) !== -1
										},
										set isChecked(value)
										{
											if (value)
												vm.$emit('update:modelValue', [...(vm.modelValue || []), this.option.key])
											else
												vm.$emit('update:modelValue', _filter(vm.modelValue || [], o => o !== this.option.key))
										},
										option,
										itemId: `mcb-item-${this.controlId}-${col}`
									}
								}))
					}
				}
			}
		}
	}
</script>
