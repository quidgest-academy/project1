<template>
	<q-field
		:readonly="readonly"
		:disabled="disabled"
		:size="size">
		<template #prepend>
			<a
				class="q-date-time-picker__icon-wrapper"
				@click="$emit('reset-icon-click')">
				<q-icon :icon="inputIcon" />
			</a>
		</template>

		<datepicker
			v-model="model"
			:id="id"
			:disabled="disabled"
			:readonly="readonly"
			:time-picker="isTimePicker"
			:format="dateTimeFormat"
			:placeholder="placeholder"
			:text-input="textInputOptions"
			:is24="!time12h"
			:locale="locale"
			:enable-time-picker="hasTimePicker"
			:enable-seconds="enableSeconds"
			:config="dateTimeConfig"
			:teleport="teleport"
			hide-input-icon
			clearable
			auto-apply>
			<template #clear-icon="{ clear }">
				<q-button
					class="q-date-time-picker__clear"
					b-style="plain"
					borderless
					@click="clear">
					<q-icon icon="close" />
				</q-button>
			</template>
		</datepicker>
	</q-field>
</template>

<script>
	import Datepicker from '@vuepic/vue-datepicker'

	export default {
		name: 'QDateTimePicker',

		components: {
			Datepicker
		},

		props: {
			/**
			 * Unique identifier for the control.
			 */
			id: {
				type: String,
				default: ''
			},

			/**
			 * Value of the control, could be a date, time or date with time
			 */
			modelValue: {
				validator(value) {
					return typeof value === 'string' || typeof value === 'object'
				},
				default: () => null
			},

			/**
			 * If control is Read only
			 */
			readonly: {
				type: Boolean,
				default: false
			},

			/**
			 * If control is a Fixed value, not to be changed with input.
			 */
			disabled: {
				type: Boolean,
				default: false
			},

			/**
			 * format of the control, {D : date}, {T : time}, {DT : dateTime}, {DS : dateTimeSeconds}
			 */
			format: {
				type: String,
				default: 'date' // Default format
			},

			/**
			 * Set datepicker locale.
			 * Datepicker will use built in javascript locale formatter to extract month and weekday names.
			 * https://vue3datepicker.com/api/props/#locale
			 */
			locale: {
				type: String,
				default: 'en-US'
			},

			/**
			 * Sizing class for the control
			 */
			size: {
				type: String,
				default: 'medium'
			},

			/**
			 * Custom classes
			 */
			classes: {
				type: Array,
				default: () => []
			},

			/**
			 * Date time input placeholder
			 */
			placeholder: {
				type: String,
				default: ''
			},

			/**
			 * If the time is in 24h format
			 */
			time12h: {
				type: Boolean,
				default: false
			},

			/**
			 * If the calendar and time picker are to be teleported into the input location
			 * this is used mainly to fix the positioning of the component on storybook
			 */
			teleport: {
				type: Boolean,
				default: true
			}
		},

		emits: [
			'update:modelValue',
			'reset-icon-click'
		],

		expose: [],

		computed: {
			model: {
				get() {
					if(typeof this.modelValue === 'string' && this.format === 'time') {
						if(this.modelValue === '') return null

						const timeObj = this.modelValue.split(':')
						return {
							hours: timeObj[0],
							minutes: timeObj[1]
						}
					}
					return this.modelValue
				},
				set(value) {
					this.$emit('update:modelValue', value)
				}
			},

			enableSeconds() {
				return this.format === 'dateTimeSeconds'
			},

			hasTimePicker() {
				return this.format !== 'date'
			},
			isTimePicker() {
				return this.format === 'time'
			},

			dateTimeFormat() {
				switch (this.format) {
					case 'date':
						return 'dd/MM/yyyy'
					case 'dateTime':
						return 'dd/MM/yyyy HH:mm'
					case 'dateTimeSeconds':
						return 'dd/MM/yyyy HH:mm:ss'
					case 'time':
						return 'HH:mm'
					default:
						return ''
				}
			},

			textFormat() {
				switch (this.format) {
					case 'date':
						return ['dd/MM/yyyy', 'dd-MM-yyyy', 'ddMMyyyy']
					case 'dateTime':
						return ['dd/MM/yyyy HH:mm', 'dd-MM-yyyy HH:mm', 'ddMMyyyy HH:mm', 'dd/MM/yyyy HHmm', 'dd-MM-yyyy HHmm', 'ddMMyyyy HHmm']
					case 'dateTimeSeconds':
						return ['dd/MM/yyyy HH:mm:ss', 'dd-MM-yyyy HH:mm:ss', 'ddMMyyyy HH:mm:ss', 'dd/MM/yyyy HHmmss', 'dd-MM-yyyy HHmmss', 'ddMMyyyy HHmmss']
					case 'time':
						return ['HH:mm', 'HHmm']
					default:
						return ''
				}
			},

			textInputOptions() {
				return {
					format: this.textFormat
				}
			},

			dateTimeConfig() {
				return {
					closeOnAutoApply: false
				}
			},

			inputIcon() {
				return this.format === 'time' ? 'time' : 'date'
			}
		}
	}
</script>
