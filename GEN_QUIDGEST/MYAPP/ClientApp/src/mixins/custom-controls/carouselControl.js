import { computed } from 'vue'

import CustomControl from './baseControl.js'
import CarouselResources from './resources/carouselResources.js'

/**
 * Carousel control
 * @extends CustomControl
 */
export default class CarouselControl extends CustomControl
{
	/**
	 * Creates an instance of CarouselControl.
	 * @param {object} controlContext - The context of the control.
	 * @param {number} controlOrder - The order of the control.
	 */
	constructor(controlContext, controlOrder)
	{
		super(controlContext, controlOrder)

		this.texts = new CarouselResources(controlContext.vueContext.$getResource)
		this.usesFullSizeImg = true

		// Carousel-specific handlers
		this.handlers = {
			'update:visible': (id) => this.onUpdateVisible(id),
			'click:slide': (id) => this.onSlideClick(id)
		}
	}

	/**
	 * Get the properties for configuring the carousel component.
	 * @param {object} viewMode - The current view mode of the carousel.
	 * @returns {object} - An object containing carousel properties.
	 */
	getProps(viewMode)
	{
		const slides = computed(() => {
			return (viewMode.mappedValues ?? []).map((mappedValue) => ({
				id: mappedValue.rowKey,
				title: mappedValue.slideTitle?.value,
				subtitle: mappedValue.slideSubtitle?.value,
				image: mappedValue.slideImage?.previewData ?? mappedValue.slideImage?.value,
				colorPlaceholder: mappedValue.slideImage?.dominantColor
			}))
		})

		return {
			id: viewMode.containerId,
			slides: slides,
			showIndicators: viewMode.styleVariables.showIndicators.value,
			showControls: viewMode.styleVariables.showControls.value,
			keyboardControllable: viewMode.styleVariables.keyboardControllable.value,
			autoCycleInterval: viewMode.styleVariables.autoCycleInterval.value,
			autoCyclePause: viewMode.styleVariables.autoCyclePause.value,
			ride: viewMode.styleVariables.ride.value,
			wrap: viewMode.styleVariables.wrap.value,
			loading: !this.controlContext.loaded
		}
	}

	/**
	 * Handles the model value update event.
	 * @param {string} rowKey - The key of the current slide.
	 */
	onUpdateVisible(rowKey)
	{
		this.fetchImage(rowKey, 'slideImage')
	}

	/**
	 * Handles the slide click event.
	 * @param {string} rowKey - The key of the clicked slide.
	 */
	onSlideClick(rowKey)
	{
		const action = this.controlContext.config.rowClickAction

		if (!action)
			return

		const viewMode = this.controlContext.viewModes[this.controlOrder - 1],
			handler = viewMode.handlers.rowAction

		if (typeof handler === 'function')
			handler({ id: action.id, rowKey })
	}
}
