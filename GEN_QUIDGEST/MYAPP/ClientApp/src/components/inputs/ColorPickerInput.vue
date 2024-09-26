<template>
	<div
		class="q-colorpicker-input"
		ref="colorPicker">
		<q-input-group>
			<template #prepend>
				<span
					class="q-colorpicker-input-color-container"
					@click="togglePicker">
					<i
						class="q-colorpicker-input-color"
						:style="{ backgroundColor: chosenColor }"></i>
				</span>
			</template>
			<q-text-field
				v-model="chosenColor"
				placeholder="#000000"
				@click="togglePicker" />
			<template #append>
				<q-button
					b-style="tertiary"
					borderless
					@click="togglePicker">
					<q-icon :icon="toggleIcon" />
				</q-button>
			</template>
		</q-input-group>

		<color-picker
			v-if="displayPicker"
			:theme="theme"
			:color="chosenColor"
			:colors-default="colorsDefaults"
			@change-color="changeColor" />
	</div>
</template>

<script>
	/**
	 * Check docks here:
	 * @see https://github.com/anish2690/vue-color-kit
	 */
	import { ColorPicker } from 'vue-color-kit'

	export default {
		name: 'QColorPicker',

		emits: ['changed-color'],

		components: {
			ColorPicker
		},

		inheritAttrs: false,

		props: {
			/**
			 * Can be Hex or RGBA
			 */
			color: {
				type: String,
				default: '#000000'
			},

			/**
			 * Color Picker Color
			 */
			theme: {
				type: String,
				default: 'light',
				validator: (val) => ['light', 'dark'].includes(val)
			},

			/**
			 * Available colors on the Picker palette
			 */
			colorsDefaults: {
				type: Array,
				default: () => []
			}
		},

		expose: [],

		data()
		{
			return {
				standard: true,
				chosenColor: this.setColor(),
				displayPicker: false,
				hasClickListener: false
			}
		},

		beforeUnmount()
		{
			this.unbindOutsideClickListener()
		},

		computed: {
			toggleIcon()
			{
				return this.displayPicker ? 'collapse' : 'expand'
			}
		},

		methods: {
			setColor()
			{
				// Sets user or default color.
				return typeof this.color === 'undefined' ? '#000000' : this.color
			},

			showPicker()
			{
				this.bindOutsideClickListener()
				this.displayPicker = true
			},

			togglePicker()
			{
				// Toggles picker render state.
				this.standard = false
				this.displayPicker ? this.hidePicker() : this.showPicker()
			},

			hidePicker()
			{
				// Removes picker from screen.
				this.unbindOutsideClickListener()
				this.displayPicker = false
			},

			changeColor(color)
			{
				this.chosenColor = color.hex

				this.$emit('changed-color', this.color)
			},

			outsideClickListener(event)
			{
				if (this.displayPicker && this.$refs.colorPicker && !this.$refs.colorPicker.contains(event.target))
					this.hidePicker()
			},

			bindOutsideClickListener()
			{
				if (!this.hasClickListener)
				{
					window.addEventListener('mousedown', this.outsideClickListener)
					this.hasClickListener = true
				}
			},

			unbindOutsideClickListener()
			{
				window.removeEventListener('mousedown', this.outsideClickListener)
				this.hasClickListener = false
			}
		}
	}
</script>
