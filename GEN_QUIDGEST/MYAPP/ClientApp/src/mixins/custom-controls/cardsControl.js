import { computed } from 'vue'

import CustomControl from './baseControl.js'
import CardsResources from './resources/cardsResources.js'

/**
 * Cards control
 */
export default class CardsControl extends CustomControl
{
	constructor(controlContext, controlOrder)
	{
		super(controlContext, controlOrder)

		this.texts = new CardsResources(controlContext.vueContext.$getResource)
		this.usesFullSizeImg = true

		// Cards-specific handlers
		this.handlers = {
			'update:visible': (id) => this.onUpdateVisible(id)
		}
	}

	/**
	 * Get the properties for configuring the cards component.
	 * @param {object} viewMode - The current view mode of the cards.
	 * @returns {object} - An object containing cards properties.
	 */
	getProps(viewMode)
	{
		// TODO: only pass cards-specific props
		return {
			id: viewMode.containerId,
			subtype: viewMode.subtype,
			mappedValues: viewMode.mappedValues,
			styleVariables: viewMode.styleVariables,
			listConfig: this.controlContext.config,
			readonly: computed(() => viewMode.readonly),
			loading: !this.controlContext.loaded
		}
	}

	/**
	 * Sets any additional properties that might be needed for the cards
	 * @param {object} viewMode The current view mode
	 */
	setCustomProperties(viewMode)
	{
		viewMode.implementsOwnInsert = viewMode.styleVariables.customInsertCard?.value ?? false
	}

	/**
	 * Handles the model value update event.
	 * @param {string} rowKey - The key of the current slide.
	 */
	onUpdateVisible(rowKey)
	{
		this.fetchImage(rowKey, 'image')
	}
}
