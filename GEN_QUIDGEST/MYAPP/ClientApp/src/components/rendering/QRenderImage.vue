<template>
	<img
		class="q-render-image__thumbnail"
		v-bind="imageAttrs" />
</template>

<script>
	import { imageObjToSrc } from '@/mixins/genericFunctions.js'

	export default {
		name: 'QRenderImage',

		inheritAttrs: false,

		props: {
			/**
			 * The image object containing the data and metadata necessary to render the image.
			 * data: The actual base64 image data or link to the image.
			 * dataFormat: The format of the image, e.g., 'image/png'.
			 * fileName: The name of the image file.
			 * encoding: The type of encoding used for the image data e.g., 'base64'.
			 */
			value: {
				type: Object,
				default: () => ({
					data: '',
					dataFormat: 'image/png',
					fileName: '',
					encoding: 'base64',
					isThumbnail: true
				})
			},

			/**
			 * Path for the resources.
			 */
			resourcesPath: {
				type: String,
				required: true
			}
		},

		expose: [],

		computed: {
			/**
			 * Attributes for the image element.
			 */
			imageAttrs()
			{
				let src = `${this.resourcesPath}no_img.png`

				if (this.value !== null)
					src = imageObjToSrc(this.value)

				return { src }
			}
		}
	}
</script>
