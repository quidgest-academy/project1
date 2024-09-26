import { postData } from '@/api/network'
import eventBus from '@/api/global/eventBus.js'
import mainConfigUtils from '@/api/global/mainConfigUtils.js'
import { resetStoreState } from '@/mixins/genericFunctions.js'

/**
 * Called after a successful logout.
 */
function logOffSuccess()
{
	resetStoreState()

	mainConfigUtils.updateAFToken()
	mainConfigUtils.updateMainConfig()
}

/**
 * Handles the server call to log out of the application.
 */
export function logOff()
{
	eventBus.emit(
		'go-to-route',
		{ name: 'main' },
		() => postData('Account', 'LogOff', {}, logOffSuccess))
}

export default {
	logOff
}
