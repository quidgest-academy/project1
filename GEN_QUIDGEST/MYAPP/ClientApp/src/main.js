import { createApp } from 'vue'
import { createPinia } from 'pinia'

import { setAppConfig } from './mixins/genericFunctions.js'
import { setExternalAppsPlugin } from './integratedApps.js'
import { setupI18n, resourcesMixin } from './plugins/i18n.js'
import eventTracker from './plugins/eventTracker.js'
import { simpleFetch } from './api/network'
import { setupRouter } from './router'
import Components from './components/index.js'
import formComponents from './components/formComponents.js'
import gridTableListFormComponents from './components/gridTableListFormComponents.js'
import eventBus from './api/global/eventBus.js'
import QValidationSummary from './views/shared/QValidationSummary.vue'
import App from './App.vue'
import framework from './plugins/quidgest-ui'

// Global CSS
import './assets/styles/quidgest.scss'

const pinia = createPinia()
const app = createApp(App)

app.use(framework)
app.use(pinia)
app.use(eventTracker)

const i18n = setupI18n()
app.config.globalProperties.$__i18n = i18n

// Init router instance
const router = setupRouter(i18n)

// Parse URL to get the selected System (year)
let currentSystem = undefined
const hash = window.location.hash.slice(1)
if (hash.length > 0)
{
	const currentRoute = router.resolve(hash)
	currentSystem = currentRoute.params.system
}

async function delay(ms)
{
	return new Promise((resolve) => setTimeout(resolve, ms))
}

async function retryWithDelay(maxRetries, timeout, fn)
{
	try
	{
		return await fn()
	}
	catch (error)
	{
		if (maxRetries <= 0)
			throw error
		
		await delay(timeout)
		return retryWithDelay(maxRetries - 1, timeout, fn)
	}
}

// Get config from backend
retryWithDelay(5, 1000, () => simpleFetch('Config', 'GetConfig', currentSystem)).then(response => {
	if (!response.data.Success)
	{
		// eslint-disable-next-line no-console
		console.error('ERROR: Unable to start the application!')
		return
	}

	setAppConfig(response.data.Data)
	app.use(i18n).use(router)

	// Init global components
	app.component('QValidationSummary', QValidationSummary)
	app.use(Components)
	app.use(formComponents)
	app.use(gridTableListFormComponents)

	// Init external apps
	setExternalAppsPlugin(app, router)

	// Create the Global event bus
	// To communicate between components in different levels
	app.config.globalProperties.$eventHub = eventBus

	// Global mixin applied to every vue instance
	app.mixin(resourcesMixin)
	app.mount('#app')
})
	.catch(error => {
		// eslint-disable-next-line no-console
		console.error('GetConfig error:', error)
	})
