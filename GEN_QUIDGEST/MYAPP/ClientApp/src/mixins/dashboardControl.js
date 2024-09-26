import _assignInWith from 'lodash-es/assignInWith'
import _isUndefined from 'lodash-es/isUndefined'
import _merge from 'lodash-es/merge'
import { computed } from 'vue'

import controlsResources from './controlsResources.js'

/**
 * Dashboard control
 */
export class DashboardControl
{
	constructor(options, vueContext)
	{
		this.vueContext = vueContext
		Object.defineProperty(this, 'vueContext', { enumerable: false })

		this.handlers = {}
		this.resourcesPath = computed(() => vueContext.system?.resourcesPath || '')
		this.texts = new controlsResources.DashboardResources(vueContext.$getResource)

		_merge(this, options || {})
	}

	/**
	 * Initializes the necessary properties.
	 */
	init()
	{
		this.initHandlers()
	}

	/**
	 * Initialize the default handlers
	 */
	initHandlers()
	{
		const handlers = {
			save: (eventData) => this.save(eventData),
			fetchData: (eventData) => this.fetchData(eventData),
			navigateTo: (eventData) => this.navigateTo(eventData)
		}

		_assignInWith(this.handlers, handlers, (objValue, srcValue) => _isUndefined(objValue) ? srcValue : objValue)
	}

	save()
	{
		this.vueContext.onDashboardSave(this)
	}

	fetchData(eventData)
	{
		this.vueContext.fetchWidgetData(this, eventData)
	}

	navigateTo(target)
	{
		if (target.Type === 'menu')
			this.vueContext.navigateToRouteName(`menu-${target.Name}`, {})
		else if (target.Type === 'form')
			this.vueContext.navigateToForm(target.Name, 'SHOW', target.Id, {})
	}
}
