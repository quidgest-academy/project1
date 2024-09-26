<template>
	<div
		:id="id"
		:class="['carousel-container', $attrs.class]">
		<div
			:id="`${id}-content`"
			class="carousel slide"
			:data-interval="autoCycleInterval"
			:data-keyboard="keyboardControllable"
			:data-pause="autoCyclePause"
			:data-ride="ride"
			:data-wrap="wrap">
			<ol
				v-if="showIndicators && slides.length > 1"
				class="carousel-indicators">
				<li
					v-for="(slide, index) in slides"
					:key="slide.id"
					:class="{ active: index === 0 }"
					:data-target="target"
					:data-slide-to="index" />
			</ol>

			<q-skeleton-loader v-if="loading" />

			<div
				v-else
				class="carousel-inner">
				<div
					v-for="(slide, index) in slides"
					:key="slide.id"
					:class="itemClasses(index)"
					:style="itemStyle(slide)"
					@click="onSlideClick(slide)">
					<div class="carousel-content">
						<div class="carousel-caption d-none d-md-block">
							<h2 v-if="slide.title">
								{{ slide.title }}
							</h2>

							<p v-if="slide.subtitle">
								{{ slide.subtitle }}
							</p>
						</div>
					</div>
				</div>
			</div>

			<template v-if="showControls && slides.length > 1">
				<a
					class="carousel-control-prev"
					role="button"
					data-slide="prev"
					:data-target="target">
					<span
						class="carousel-control-prev-icon"
						aria-hidden="true" />
					<span class="sr-only">{{ texts.previousText }}</span>
				</a>

				<a
					class="carousel-control-next"
					:data-target="target"
					role="button"
					data-slide="next">
					<span
						class="carousel-control-next-icon"
						aria-hidden="true" />
					<span class="sr-only">{{ texts.nextText }}</span>
				</a>
			</template>
		</div>
	</div>
</template>

<script>
	import { validateTexts } from '@/mixins/genericFunctions.js'

	// The texts needed by the component.
	const DEFAULT_TEXTS = {
		previousText: 'Previous',
		nextText: 'Next'
	}

	export default {
		name: 'QCarousel',

		emits: ['update:visible', 'click:slide'],

		inheritAttrs: false,

		props: {
			/**
			 * The unique identifier for the carousel.
			 */
			id: String,

			/**
			 * The slides of the carousel.
			 */
			slides: {
				type: Array,
				default: () => []
			},

			/**
			 * Whether to show carousel indicators.
			 */
			showIndicators: {
				type: Boolean,
				default: true
			},

			/**
			 * Whether to show carousel controls.
			 */
			showControls: {
				type: Boolean,
				default: true
			},

			/**
			 * Whether carousel can be controlled via keyboard.
			 */
			keyboardControllable: {
				type: Boolean,
				default: true
			},

			/**
			 * The interval for auto-cycling the carousel.
			 */
			autoCycleInterval: {
				type: Number,
				default: 5000
			},

			/**
			 * The condition for pausing auto-cycling ('hover' or 'false').
			 */
			autoCyclePause: {
				type: String,
				default: 'hover'
			},

			/**
			 * The type of ride behavior for carousel ('carousel' or 'false').
			 */
			ride: {
				type: String,
				default: 'carousel'
			},

			/**
			 * Whether to wrap carousel slides.
			 */
			wrap: {
				type: Boolean,
				default: true
			},

			/**
			 * The necessary strings to be used inside the component.
			 */
			texts: {
				type: Object,
				validator: (value) => validateTexts(DEFAULT_TEXTS, value),
				default: () => DEFAULT_TEXTS
			},

			/**
			 * Whether or not content is loading.
			 */
			loading: {
				type: Boolean,
				default: false
			}
		},

		expose: [],

		computed: {
			target()
			{
				return `#${this.id}-content`
			}
		},

		methods: {
			onSlideClick(slide)
			{
				const selection = window.getSelection()

				// To allow text selection without triggering the click action
				if (selection.toString().length !== 0)
					return

				this.$emit('click:slide', slide.id)
			},

			itemClasses(idx)
			{
				const classes = ['carousel-item']

				if (idx === 0)
					classes.push('active')

				return classes
			},

			itemStyle(slide)
			{
				const style = {}

				if (slide?.colorPlaceholder)
					style['background-color'] = slide.colorPlaceholder

				if (slide?.image)
					style['background-image'] = `url('${slide.image}')`

				return style
			}
		},

		watch: {
			slides: {
				handler(newVal, oldSlides)
				{
					// TODO: use lazy: request slide image just before it scrolls into view
					newVal.forEach((newSlide) => {
						const oldSlide = oldSlides?.find((s) => s.id === newSlide.id)
						if (!oldSlide || (oldSlide.image !== newSlide.image))
							this.$emit('update:visible', newSlide.id, newSlide.image)
					})
				},
				immediate: true,
				deep: true
			}
		}
	}
</script>
