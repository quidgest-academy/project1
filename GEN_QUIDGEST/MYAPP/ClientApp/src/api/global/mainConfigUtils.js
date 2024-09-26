import { simpleFetch } from '@/api/network'
import { setAppConfig } from '@/mixins/genericFunctions.js'
import { useSystemDataStore } from '@/stores/systemData.js'

/**
 * Update the main configuration
 * @param {CallableFunction} afterUpdateCallback Callback to be executed after update of the main configuration
 * @returns { Promise } Returns the fetch promise
 */
export function updateMainConfig(afterUpdateCallback)
{
	// The GetConfig action always requires the system for which it should return the configuration (especially when it requires permissions validation).
	const systemDataStore = useSystemDataStore()

	return simpleFetch('Config', 'GetConfig', systemDataStore.system.currentSystem).then((response) => {
		if (!response.data.Success)
			return

		setAppConfig(response.data.Data)

		if (typeof afterUpdateCallback === 'function')
			afterUpdateCallback(response.data.Data)
	})
}

/**
 * Update the Anti Forgery token
 * @returns { Promise } Returns the fetch promise
 */
export function updateAFToken()
{
	return simpleFetch('Config', 'GetAFToken').then((response) => {
		if (!response.data.Success)
			return

		document.getElementsByName('__RequestVerificationToken').forEach((elem) => { elem.value = response.data.Data.formToken })
	})
}

export default {
	updateMainConfig,
	updateAFToken
}
