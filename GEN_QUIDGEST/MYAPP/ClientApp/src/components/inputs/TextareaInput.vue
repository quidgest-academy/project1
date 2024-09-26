<template>
	<textarea
		:id="controlId"
		v-model="curValue"
		:class="['i-textarea__field', 'i-textarea', size ? `input-${size}` : '']"
		:readonly="readonly"
		:disabled="disabled"
		:rows="rows"
		:cols="cols"
		:required="isRequired"
		:aria-labelledby="labelId"
		:placeholder="placeholder">
	</textarea>
</template>

<script>
	import _isEmpty from 'lodash-es/isEmpty'

	import { inputSize } from '@/mixins/quidgest.mainEnums.js'

	export default {
		name: 'QTextareaInput',

		emits: [
			'update:modelValue'
		],

		inheritAttrs: false,

		props: {
			/**
			 * Unique identifier for the control.
			 */
			id: String,

			/**
			 * Holds value of text area
			 */
			modelValue: String,

			/**
			 * Size of the text area
			 */
			size: {
				type: String,
				validator: (value) => _isEmpty(value) || Reflect.has(inputSize, value)
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
			 * For mandatory text area
			 */
			isRequired: {
				type: Boolean,
				default: false
			},

			/**
			 * Number of lines of text area
			 */
			rows: {
				type: Number,
				default: 2
			},

			/**
			 * Specifies the visible width of a text area
			 */
			cols: {
				type: Number,
				default: 20
			},

			/**
			 * The placeholder of the control
			 */
			placeholder: {
				type: String,
				default: ''
			}
		},

		// TODO: Remove these properties from the "expose" (only necessary for unit tests).
		expose: [
			'rows',
			'size'
		],

		data()
		{
			return {
				controlId: this.id || `input_ta_${this._.uid}`
			}
		},

		computed: {
			curValue: {
				get()
				{
					return this.modelValue
				},
				set(newValue)
				{
					this.$emit('update:modelValue', newValue)
				}
			},

			labelId()
			{
				return `label_${this.controlId}`
			}
		}
	}
</script>
