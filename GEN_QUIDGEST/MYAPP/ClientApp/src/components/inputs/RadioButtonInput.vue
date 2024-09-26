<template>
	<fieldset
		role="radiogroup"
		:aria-labelledby="labelId"
		:class="[{ labelleft: labelLeftSide }, 'i-radio__control']">
		<div class="form-check-columns">
			<div
				v-for="column in columnList"
				:key="column"
				class="column">
				<q-radio-group-item
					v-for="option in column"
					:key="option.key"
					:id="`input_${controlId}_${option.key}`"
					:label="String(option.value)"
					:name="`radio_btn_${controlId}`"
					:value="option.key"
					:checked="modelValue === option.key"
					:focused="activeEl === option.key"
					:disabled="disabled"
					:readonly="readonly"
					@click="selectElement(option.key, $event)"
					@keyup="selectElement(option.key, $event)"
					@focus="focusElement(option.key)"
					@focusout="removeFocus">
					<template #prepend>
						<slot :name="`${option.key}.prepend`" />
					</template>

					<template #append>
						<slot :name="`${option.key}.append`" />
					</template>
				</q-radio-group-item>
			</div>
		</div>
	</fieldset>
</template>

<script>
	import _isEmpty from 'lodash-es/isEmpty'

	import QRadioGroupItem from './QRadioGroupItem.vue'

	export default {
		name: 'QRadioGroup',

		emits: ['update:modelValue'],

		components: {
			QRadioGroupItem
		},

		inheritAttrs: false,

		props: {
			/**
			 * Unique identifier for the control.
			 */
			id: String,

			/**
			 * Holds the current value.
			 */
			modelValue: [Number, String],

			/**
			 * Options for radio input.
			 */
			optionsList: {
				type: Array,
				required: true,
				validator: (prop) => prop.every((e) => Reflect.has(e, 'key') && Reflect.has(e, 'value'))
			},

			/**
			 * Radio input value positions.
			 */
			labelLeftSide: {
				type: Boolean,
				default: false
			},

			/**
			 * Number of columns for options.
			 */
			numberOfColumns: {
				type: Number,
				default: 1
			},

			/**
			 * To deselect radio list options.
			 */
			deselectRadio: {
				type: Boolean,
				default: false
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
			}
		},

		// TODO: Remove these properties from the "expose" (only necessary for unit tests).
		expose: [
			'optionsList'
		],

		data()
		{
			return {
				controlId: this.id || `radio_select_${this._.uid}`,

				activeEl: ''
			}
		},

		computed: {
			labelId()
			{
				return `label_${this.controlId}`
			},

			columnList()
			{
				let columns = []
				let midCount = Math.ceil(this.optionsList.length / this.numberOfColumns)

				for (let col = 0; col < this.numberOfColumns; col++)
					columns.push(this.optionsList.slice(col * midCount, col * midCount + midCount))

				return columns
			}
		},

		methods: {
			/**
			 * Emits the new value of the input.
			 * @param {string|number} newValue The new value of the radio input
			 */
			updateValue(newValue)
			{
				if (newValue === this.modelValue)
					return

				this.$emit('update:modelValue', newValue)
				this.focusElement(newValue)
			},

			/**
			 * To select radio input option.
			 * @param {string} el The key of the selected element
			 * @param {object} event The event
			 */
			selectElement(el, event)
			{
				this.focusElement(el)

				if (this.deselectRadio && !_isEmpty(this.modelValue) && el === this.modelValue)
				{
					if (event.type === 'click' || event.key === 'Backspace' || event.key === 'Delete')
						this.updateValue(undefined)
				}
				else if (event.type === 'click' || event.key === 'Enter')
					this.updateValue(el)
			},

			/**
			 * To focus radio input option.
			 * @param {string} el The key of the element that should gain focus
			 */
			focusElement(el)
			{
				this.activeEl = el
			},

			/**
			 * To remove focus from radio input option.
			 */
			removeFocus()
			{
				this.activeEl = ''
			}
		}
	}
</script>
