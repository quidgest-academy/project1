/*****************************************************************
 *                                                               *
 * This store holds authentication data,                         *
 * also defining functions to access and mutate it.              *
 *                                                               *
 *****************************************************************/

import { defineStore } from 'pinia'

//----------------------------------------------------------------
// State variables
//----------------------------------------------------------------

const state = () => {
	return {
		hasPasswordRecovery: false,

		// It's preferable that it stays hidden and then appears rather than the opposite.
		hasUsernameAuth: false,

		hasOpenIdAuth: false,

		has2FAOptions: false
	}
}

//----------------------------------------------------------------
// Variable getters
//----------------------------------------------------------------

const getters = {
	/**
	 * True if user has any settings, false otherwise.
	 * @param {object} state The current global state
	 */
	hasUserSettings(state)
	{
		return state.hasUsernameAuth || state.hasOpenIdAuth || state.has2FAOptions
	}
}

//----------------------------------------------------------------
// Actions
//----------------------------------------------------------------

const actions = {
	/**
	 * Sets whether the password recovery page is available.
	 * @param {boolean} hasPasswordRecovery Whether or not the password recovery page is available
	 */
	setPasswordRecovery(hasPasswordRecovery)
	{
		if (typeof hasPasswordRecovery !== 'boolean')
			return

		this.hasPasswordRecovery = hasPasswordRecovery
	},

	/**
	 * Sets whether the username and password authentification is available.
	 * @param {boolean} hasUsernameAuth Whether the username and password authentification is available
	 */
	setUsernameAuth(hasUsernameAuth)
	{
		if (typeof hasUsernameAuth !== 'boolean')
			return

		this.hasUsernameAuth = hasUsernameAuth
	},

	/**
	 * Sets whether OpenID authentication is available.
	 * @param {boolean} hasOpenIdAuth Whether OpenID authentication is available
	 */
	setOpenIdAuth(hasOpenIdAuth)
	{
		if (typeof hasOpenIdAuth !== 'boolean')
			return

		this.hasOpenIdAuth = hasOpenIdAuth
	},

	/**
	 * Sets the availability of 2FA options.
	 * @param {boolean} has2FAOptions Whether 2FA options are available
	 */
	set2FAOptions(has2FAOptions)
	{
		if (typeof has2FAOptions !== 'boolean')
			return

		this.has2FAOptions = has2FAOptions
	},

	/**
	 * Resets the auth data.
	 */
	resetStore()
	{
		Object.assign(this, state())
	}
}

//----------------------------------------------------------------
// Store export
//----------------------------------------------------------------

export const useAuthDataStore = defineStore('authData', {
	state,
	getters,
	actions
})
