<template>
	<div class="q-progress__container">
		<div
			:id="controlId"
			:class="containerClasses"
			tabindex="0">
			<div
				:class="progressClasses"
				:style="progressBarStyle">
				<span
					class="q-progress-bar-text"
					:style="textColor">{{ displayText }}</span>
			</div>
		</div>
		<div
			v-if="showLimits"
			class="q-progress__values">
			<span class="q-progress__values__min">{{ min }}</span>
			<span class="q-progress__values__max">{{ max }}</span>
		</div>
	</div>
</template>

<script>
	import genericFunctions from "@/mixins/genericFunctions.js";

	export default {
		name: 'QProgressBar',
		inheritAttrs: false,
		props: {

			/**
			 * Unique identifier for the progress bar.
			 */
			id: String,

			/**
			 * Text displayed in the progress bar.
			 */
			text: {
				type: String,
				default: ''
			},

			/**
			 * Progress bar is striped
			 */
			striped: {
				type: Boolean,
				default: false
			},

			/**
			 * Progress bar has an animation
			 */
			animated: {
				type: Boolean,
				default: true
			},

			/**
			 * Progress bar is mini
			 */
			mini: {
				type: Boolean,
				default: false
			},

			/**
			 * Gives color to the progress bar
			 */
			barColor: {
				type: String,
			},

			/**
			 * Minimum value for the range of the progress bar
			 */
			min: {
				type: Number,
				default: 0,
			},

			/**
			 * Maximum value for the range of the progress bar
			 */
			max: {
				type: Number,
				default: 100,
			},

			/**
			 * Current number of the progress bar
			 */
			modelValue: {
				type: Number,
				required: true,
			}, 

			/**
			 * Progress bar shows min and max
			 */
			showLimits: {
				type: Boolean, 
				default: false,
			},
		},

		data() {
			return {
				controlId: this.id || `q-progress-bar-${this._.uid}`,
			};
		},

		computed: {
			containerClasses() {
				return ['q-progress', { 'q-progress--mini': this.mini }];
			},

			progressClasses() {
				return [
					'q-progress-bar',
					{
						'q-progress-bar--striped': this.striped,
						'q-progress-bar--animated': this.animated && this.percentage < 100
					}
				];
			},

			/**
			 * Calculates the percentage
			 */
			percentage() {
				return (((this.modelValue - this.min) / (this.max - this.min)) * 100).toFixed(2);
			},

			/**
			 * Adds style to the progress bar
			 */
			progressBarStyle() {
				
				const style = {
					width: `${this.percentage}%`
				};

				if (this.barColor) 
					style.backgroundColor = this.barColor;
                
				return style;
            
			},

			/**
			 * Choses what text is gonna be displayed
			 */
			displayText() {
				return this.text || this.modelValue;
			},

			/**
			 * Defines a color to apply to the text of the progress bar 
			 * given the color of the progress bar
			 */
			textColor() {
				return {
					color: genericFunctions.getReadableTextColor(this.barColor)
				};
			}
		},
		expose: []
	};
</script>
