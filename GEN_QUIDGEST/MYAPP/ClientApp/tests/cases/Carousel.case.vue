<template>
	<div style="margin: 1rem">
		<div class="table-title c-table__title">
			<h1>Carousel</h1>
		</div>

		<q-carousel
			:container-id="containerId"
			:mapped-values="mappedValues"
			:style-variables="styleVariables"
			:list-config="listConfig"
			@row-action="rowAction" />
	</div>
</template>

<script>
	import QCarousel from '@/components/rendering/QCarousel.vue'

	import fakeData from './Carousel.mock.js'

	export default {
		name: 'QCarouselContainer',

		docsfile: './docs/rendering/QCarousel.md',

		components: {
			QCarousel
		},

		inheritAttrs: false,

		expose: [],

		data()
		{
			return fakeData.simpleUsage()
		},

		methods: fakeData.simpleUsageMethods
	}
</script>
