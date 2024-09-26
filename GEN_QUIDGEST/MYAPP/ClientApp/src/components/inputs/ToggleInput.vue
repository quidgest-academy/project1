<template>
	<div class="i-switch">
		<label
			class="action-input"
			:for="controlId">
			<input
				type="checkbox"
				v-model="curValue"
				:name="controlId"
				:disabled="disabled || readonly" />

			<span
				data-testid="switch"
				:class="[{ 'i-switch--undefined': curValue === null }, 'i-switch__label']"
				@click.stop.prevent="onClick()" />

			<span
				v-if="showFalseLabel"
				data-option="false"
				:class="[{ 'float-none': displayType === 'label-toggle' }, 'i-switch__label-text']"
				@click.stop.prevent="onClick(false)">
				{{ falseLabel }}
			</span>

			<span
				v-if="showTrueLabel"
				data-option="true"
				:class="[{ 'float-none': displayType === 'label-toggle', 'i-switch--on-left': displayType === 'label-left' }, 'i-switch__label-text']"
				@click.stop.prevent="onClick(true)">
				{{ trueLabel }}
			</span>
		</label>
	</div>
</template>

<script>
	import _isEmpty from 'lodash-es/isEmpty'

	import { labelDisplay } from '@/mixins/quidgest.mainEnums'

	/**
	 * Boolean value toggler. Provides labels for the true and false values.
	 * There is no empty representation, so mandatory fields don't make sense to use with this control.
	 */
	export default {
		name: 'QToggle',

		emits: ['update:modelValue'],

		inheritAttrs: false,

		props: {
			/**
			 * Unique identifier for the control.
			 */
			id: String,

			/**
			 * The boolean (true/false) or number 2 for undefined
			 */
			modelValue: {
				type: [Boolean, Number],
				default: false,
				validator: (value) => [false, true, 0, 1, null].includes(value)
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
			 * Text label for a true value
			 */
			trueLabel: {
				type: String,
				default: ''
			},

			/**
			 * Text label for a false value
			 */
			falseLabel: {
				type: String,
				default: ''
			},

			/**
			 * Determines how to the labels are displayed: label-toggle, label-left
			 */
			displayType: {
				type: String,
				default: '',
				validator: (value) => _isEmpty(value) || Reflect.has(labelDisplay, value)
			},

			/**
			 * Determines if button has a middle state  when passed modelValue 2
			 */
			nullable: {
				type: Boolean,
				default: false
			}
		},

		expose: [],

		data()
		{
			return {
				controlId: this.id || this._.uid
			}
		},

		computed: {
			curValue: {
				get()
				{
					if (this.nullable && this.modelValue === null)
						return null

					return this.modelValue ? true : false
				},
				set(newValue)
				{
					let val = newValue

					if (typeof this.modelValue === 'number')
						val = newValue ? 1 : 0

					this.$emit('update:modelValue', val)
				}
			},

			showTrueLabel()
			{
				if (this.displayType === 'label-left')
					return true

				if (this.displayType === 'label-toggle')
				{
					if (this.nullable && this.curValue === null)
						return false

					return this.modelValue && this.trueLabel
				}

				return !(this.trueLabel === null || this.trueLabel === '')
			},

			showFalseLabel()
			{
				if (this.displayType === 'label-left')
					return false

				if (this.displayType === 'label-toggle')
				{
					if (this.nullable && this.curValue === null)
						return false

					return !this.modelValue && this.falseLabel
				}

				return !(this.falseLabel === null || this.falseLabel === '')
			}
		},

		methods: {
			/**
			 * Process the value change event.
			 * If it doesn't receive the value by parameter, then it inverts the current value
			 */
			onClick(val)
			{
				if (!this.disabled && !this.readonly)
				{
					// Toggle value request
					if (val === undefined)
					{
						if (this.nullable && this.curValue === null)
							this.curValue = true
						else
							this.curValue = !this.curValue
					}
					// Explicit value change
					else
						this.curValue = val
				}
			}
		}
	}
</script>
