<template>
	<div
		:class="[...classes, $attrs.class]"
		data-testid="q-card"
		:data-loading="$props.loading"
		@click.stop="$emit('click', $event)">
		<div class="q-card__overlay">
			<div
				v-if="$slots.header || hasHeaderImage"
				class="q-card__header">
				<div
					v-if="$slots.image && hasHeaderImage && hasImageCropper"
					:class="imgCropperClasses"
					:style="imgCropperStyle">
					<q-skeleton-loader v-if="$props.loading" />
					<slot
						v-else
						name="image" />
				</div>
				<template v-else-if="$slots.image && hasHeaderImage && !hasImageCropper">
					<q-skeleton-loader v-if="$props.loading" />
					<slot
						v-else
						name="image" />
				</template>
				<slot
					v-else
					name="header" />
			</div>

			<div class="q-card__body">
				<div
					v-if="$slots.image && hasBodyImage"
					:class="imgCropperClasses"
					:style="imgCropperStyle">
					<q-skeleton-loader v-if="$props.loading" />
					<slot
						v-else
						name="image" />
				</div>
				<div class="q-card__content">
					<slot name="content.prepend" />

					<h5
						v-if="$slots.title || $props.title"
						class="q-card__title"
						role="cell">
						<q-skeleton-loader
							v-if="$props.loading"
							type="text" />
						<template v-else-if="$slots.title">
							<slot name="title" />
						</template>
						<template v-else>
							{{ $props.title }}
						</template>
					</h5>

					<p
						v-if="$slots.subtitle || $props.subtitle"
						class="q-card__subtitle"
						role="cell">
						<q-skeleton-loader
							v-if="$props.loading"
							type="text" />
						<template v-else-if="$slots.subtitle">
							<slot name="subtitle" />
						</template>
						<template v-else>
							{{ $props.subtitle }}
						</template>
					</p>

					<slot name="content.append" />
				</div>
			</div>

			<div
				v-if="$slots.footer"
				class="q-card__footer">
				<slot name="footer" />
			</div>
		</div>

		<div
			class="q-card__underlay"
			:style="underlayStyle">
			<slot name="underlay" />
			<template v-if="hasUnderlayImage">
				<q-skeleton-loader v-if="$props.loading" />
				<slot
					v-else
					name="image" />
			</template>
		</div>
	</div>
</template>

<script>
	import { defineAsyncComponent } from 'vue'

	export default {
		name: 'QCard',

		emits: ['click'],

		components: {
			QSkeletonLoader: defineAsyncComponent(() => import('@/components/QSkeletonLoader.vue'))
		},

		inheritAttrs: false,

		props: {
			/**
			 * The title of the card.
			 */
			title: {
				type: String,
				default: ''
			},

			/**
			 * The subtitle of the card.
			 */
			subtitle: {
				type: String,
				default: ''
			},

			/**
			 * The subtype of the card.
			 */
			subtype: {
				type: String,
				default: ''
			},

			/**
			 * The size of the card.
			 */
			size: {
				type: String,
				default: 'regular',
				validator: (value) => ['regular', 'small'].includes(value)
			},

			/**
			 * The justification of the card's content.
			 */
			contentAlignment: {
				type: String,
				default: 'left',
				validator: (value) => ['left', 'center'].includes(value)
			},

			/**
			 * The hover scale amount of the card.
			 */
			hoverScaleAmount: {
				type: [String, Number],
				default: 1
			},

			/**
			 * The aspect ratio of the image in the card.
			 */
			imageShape: {
				type: String,
				default: 'rectangular',
				validator: (value) => ['rectangular', 'square', 'circular'].includes(value)
			},

			/**
			 * The color placeholder of the card.
			 */
			colorPlaceholder: {
				type: String,
				default: ''
			},

			/**
			 * Whether or not the card's content is loading.
			 */
			loading: {
				type: Boolean,
				default: false
			}
		},

		expose: [],

		computed: {
			classes()
			{
				const baseClass = 'q-card'
				const classes = [baseClass]

				const subtype = this.subtype.replace('card-', '')
				if (subtype && subtype !== 'card')
					classes.push(`${baseClass}--${subtype}`)

				if (this.size && this.size !== 'regular')
					classes.push(`${baseClass}--size-${this.size}`)

				if (this.hoverScaleAmount > 1)
					classes.push(`${baseClass}--scale-${this.hoverScaleAmount}`)

				if (this.contentAlignment === 'center')
					classes.push(`${baseClass}--centered`)

				if (this.loading)
					classes.push(`${baseClass}--loading`)

				return classes
			},

			hasHeaderImage()
			{
				return this.subtype === 'card-img-top' || this.subtype === 'card-img-thumbnail'
			},

			hasImageCropper()
			{
				return this.subtype !== 'card-img-thumbnail'
			},

			hasBodyImage()
			{
				return !this.hasHeaderImage && this.subtype !== 'card-img-background'
			},

			hasUnderlayImage()
			{
				return this.subtype === 'card-img-background'
			},

			underlayStyle()
			{
				const style = {}

				if (this.subtype !== 'card-img-background')
					return style

				if (this.colorPlaceholder)
					style['background-color'] = this.colorPlaceholder

				return style
			},

			imgCropperClasses()
			{
				const baseClass = 'q-card__img-cropper'
				const classes = [baseClass]

				if (this.imageShape && this.imageShape !== 'rectangular')
					classes.push(`${baseClass}--${this.imageShape}`)

				return classes
			},

			imgCropperStyle()
			{
				const style = {}

				// Apply the color placeholder to display while the card image is loading
				if (this.colorPlaceholder)
					style['background-color'] = this.colorPlaceholder

				return style
			}
		}
	}
</script>
