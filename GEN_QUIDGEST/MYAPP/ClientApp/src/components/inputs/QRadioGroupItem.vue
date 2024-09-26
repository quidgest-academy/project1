<template>
	<div class="i-radio__data">
		<div class="i-radio__data-content">
			<slot name="prepend" />

			<label
				:class="[
					{
						'checkfocus': focused,
						'i-radio--disabled': disabled || readonly
					},
					'i-radio',
					'i-radio__label'
				]"
				:for="id">
				{{ label }}

				<input
					:id="id"
					:data-testid="`radio_label_${value}`"
					type="radio"
					:disabled="disabled || readonly"
					:name="name"
					:value="value"
					:checked="checked"
					:title="label"
					:aria-label="label"
					@click="onClick"
					@focus="onFocus"
					@focusout="onFocusOut"
					@keyup="onKeyUp" />

				<span class="i-radio__field" />
			</label>

			<slot name="append" />
		</div>
	</div>
</template>

<script>
	export default {
		name: 'QRadioGroupItem',

		emits: {
			click: (payload) => typeof payload === 'object',
			focus: (payload) => typeof payload === 'object',
			focusout: (payload) => typeof payload === 'object',
			keyup: (payload) => typeof payload === 'object'
		},

		inheritAttrs: false,

		props: {
			/**
			 * Unique identifier for the control.
			 */
			id: String,

			/**
			 * Holds the input value.
			 */
			value: [Number, String],

			/**
			 * The label of the input.
			 */
			label: {
				type: String,
				default: ''
			},

			/**
			 * The name of the input.
			 */
			name: {
				type: String,
				default: ''
			},

			/**
			 * Whether the input is currently checked.
			 */
			checked: {
				type: Boolean,
				default: false
			},

			/**
			 * Whether the field is focused.
			 */
			focused: {
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

		expose: [],

		methods: {
			/**
			 * Emits the "click" event.
			 * @param {object} event The event object
			 */
			onClick(event)
			{
				this.$emit('click', event)
			},

			/**
			 * Emits the "focus" event.
			 * @param {object} event The event object
			 */
			onFocus(event)
			{
				this.$emit('focus', event)
			},

			/**
			 * Emits the "focusout" event.
			 * @param {object} event The event object
			 */
			onFocusOut(event)
			{
				this.$emit('focusout', event)
			},

			/**
			 * Emits the "keyup" event.
			 * @param {object} event The event object
			 */
			onKeyUp(event)
			{
				this.$emit('keyup', event)
			}
		}
	}
</script>
