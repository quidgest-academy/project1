/*****************************************************************
 *                                                               *
 * This store holds generic data about the application,          *
 * also defining functions to access and mutate it.              *
 *                                                               *
 *****************************************************************/

import { defineStore } from 'pinia'

import _remove from 'lodash-es/remove'

//----------------------------------------------------------------
// State variables
//----------------------------------------------------------------

const state = () => {
	return {
		menus: {},

		menuPath: [],

		infoMessages: [],

		modals: [],

		dropdown: {
			display: 'none'
		},

		reportingModeCAV: false,

		suggestionModeOn: false,

		notifications: [],

		homepages: {},

		isPublicRoute: false,

		isFullScreenPage: false,

		asyncProcesses: [],

		busyPageStateStack: []
	}
}

//----------------------------------------------------------------
// Variable getters
//----------------------------------------------------------------

const getters = {
	/**
	 * True if there are any menus to show, false otherwise.
	 * @param {object} state The current global state
	 */
	hasMenus(state)
	{
		return state.menus && state.menus.MenuList && state.menus.MenuList.length > 0
	},

	/**
	 * The id of the latest modal/popup that has a route, or an empty string if there are no popups or none of them have routes.
	 * @param {object} state The current global state
	 */
	latestModalId(state)
	{
		let id = ''

		for (let modal of state.modals)
			if (modal.hasRoute && modal.isActive)
				id = modal.id

		return id
	},

	/**
	 * A list of the info messages that are fixed on the screen.
	 * @param {object} state The current global state
	 */
	fixedInfoMessages(state)
	{
		return state.infoMessages.filter((message) => message.pinned)
	},

	/**
	 * A list of the info messages that aren't fixed on the screen.
	 * @param {object} state The current global state
	 */
	relativeInfoMessages(state)
	{
		return state.infoMessages.filter((message) => !message.pinned)
	},

	/**
	 * Whether there are any asynchronous processes currently running.
	 * @param {object} state The current global state
	 */
	isLoading(state)
	{
		return state.asyncProcesses.length > 0 || state.busyPageStateStack.length > 0
	}
}

//----------------------------------------------------------------
// Actions
//----------------------------------------------------------------

const actions = {
	/**
	 * Sets the available menus.
	 * @param {string} menus The available menus
	 */
	setMenus(menus)
	{
		if (typeof menus !== 'object' || menus === null)
			return

		this.menus = menus
	},

	/**
	 * Adds a new alert to the list of currently displayed info messages.
	 * @param {object} alertProps The properties of the alert
	 */
	setInfoMessage(alertProps)
	{
		if (typeof alertProps !== 'object' || alertProps === null || typeof alertProps.message !== 'string')
			return
		if (typeof alertProps.isDismissible === 'undefined')
			alertProps.isDismissible = true
		if (typeof alertProps.isResource === 'undefined')
			alertProps.isResource = false

		const length = this.infoMessages.length

		if (length === 0)
			alertProps.id = 1
		else
			alertProps.id = this.infoMessages[length - 1].id + 1

		const duplicatedResult = this.infoMessages.find((item) => item.type === alertProps.type && item.message === alertProps.message)
		if (duplicatedResult !== undefined)
			return

		this.infoMessages.push(alertProps)
	},

	/**
	 * Removes the specified alert from the list of currently displayed info messages.
	 * @param {number} alertId The id of the alert
	 */
	removeInfoMessage(alertId)
	{
		if (typeof alertId !== 'number' || !Number.isInteger(alertId))
			return
		if (alertId < 1)
			return

		for (let i = 0; i < this.infoMessages.length; i++)
		{
			if (this.infoMessages[i].id !== alertId)
				continue

			this.infoMessages.splice(i, 1)
			return
		}
	},

	/**
	 * Clears the list of info messages.
	 */
	clearInfoMessages()
	{
		this.infoMessages = []
	},

	/**
	 * Sets the list of notifications.
	 */
	setNotifications(notifications)
	{
		this.notifications = notifications
	},

	/**
	 * Removes the specified alert from the list of currently displayed alerts.
	 * @param {number} alertId The id of the alert
	 */
	removeNotification(alertId)
	{
		for (let i = 0; i < this.notifications.length; i++)
		{
			if (this.notifications[i].id !== alertId)
				continue

			this.notifications.splice(i, 1)
			return
		}
	},

	/**
	 * Clears the list of notifications.
	 */
	clearNotifications()
	{
		this.notifications = []
	},

	/**
	 * Adds a new modal/popup to be displayed.
	 * @param {object} props The modal properties
	 */
	setModal(props)
	{
		if (typeof props !== 'object' || props === null)
			props = {}

		if (typeof props.id !== 'string')
			props.id = this.modals.length.toString()

		// If no return element is specified, use the element that opened the popup
		if (props.returnElement === undefined || props.returnElement === null)
			props.returnElement = document.activeElement

		let index = -1

		for (let i = 0; i < this.modals.length; i++)
		{
			if (this.modals[i].id !== props.id)
				continue

			index = i
			break
		}

		if (index < 0) // Creates a new modal.
		{
			if (typeof props.isActive !== 'boolean')
				props.isActive = true
			if (typeof props.hasRoute !== 'boolean')
				props.hasRoute = false

			this.modals.push(props)
		}
		else // Updates the props of an existing modal.
		{
			this.modals[index] = {
				...this.modals[index],
				...props
			}
		}
	},

	/**
	 * Removes the specified modal/popup, or the last one if no id is passed.
	 * @param {string} modalId The id of the modal
	 */
	removeModal(modalId)
	{
		const length = this.modals.length
		if (length === 0)
			return

		let id = modalId
		if (typeof modalId !== 'string')
			id = this.modals[length - 1]

		for (let i = 0; i < this.modals.length; i++)
		{
			if (this.modals[i].id !== id)
				continue

			let removedModalArr = this.modals.splice(i, 1)
			return removedModalArr[0]
		}
	},

	/**
	 * Clears the list of modals/popups.
	 */
	clearModals()
	{
		this.modals = []
	},

	/**
	 * Sets the state of the dropdown to visible/hidden, according to the specified flag.
	 * @param {object} props The necessary properties
	 */
	setDropdown(props)
	{
		if (typeof props.isVisible !== 'boolean')
			return

		// Clear dropdown properties.
		this.dropdown = {}

		// Set display property to actual CSS value.
		this.dropdown.display = props.isVisible !== false ? 'block' : 'none'
		delete props.isVisible

		// Copy all properties to dropdown.
		for (let key in props)
			this.dropdown[key] = props[key]
	},

	/**
	 * Sets the state of the CAV mode.
	 * @param {boolean} isVisible Whether or not the CAV mode is active
	 */
	setReportingModeCAV(isActive)
	{
		if (typeof isActive !== 'boolean')
			return

		this.reportingModeCAV = isActive

		if (this.reportingModeCAV)
			this.suggestionModeOn = false
	},

	/**
	 * Sets the state of the Suggestion mode.
	 * @param {boolean} isVisible Whether or not the Suggestion mode is active
	 */
	setSuggestionMode(isActive)
	{
		if (typeof isActive !== 'boolean')
			return

		this.suggestionModeOn = isActive

		if (this.suggestionModeOn)
			this.reportingModeCAV = false
	},

	/**
	 * Toggles the state of the Suggestion mode.
	 */
	toggleSuggestionMode()
	{
		this.setSuggestionMode(!this.suggestionModeOn)
	},

	/**
	 * Adds a new entry to the menus path.
	 * @param {string} menuId The id of the menu
	 */
	addToMenuPath(menuId)
	{
		if (typeof menuId !== 'string' || menuId.length < 1)
			return
		if (this.menuPath.includes(menuId))
			return

		this.menuPath.push(menuId)
	},

	/**
	 * Removes an entry from the menus path.
	 * @param {string} menuId The id of the menu
	 */
	removeFromMenuPath(menuId)
	{
		if (typeof menuId !== 'string' || menuId.length < 1)
			return

		for (let i = 0; i < this.menuPath.length; i++)
			if (this.menuPath[i].startsWith(menuId))
				this.menuPath.splice(i, 1)
	},

	/**
	 * Clears the menus path.
	 */
	clearMenuPath()
	{
		this.menuPath = []
	},

	/**
	 * Sets the available home pages.
	 * @param {object} homepages The available home pages
	 */
	setHomePages(homepages)
	{
		if (typeof homepages !== 'object' || homepages === null)
			return

		this.homepages = homepages
	},

	/**
	 * Sets whether the route is public.
	 * @param {boolean} isPublicRoute Whether or not the route is public
	 */
	setPublicRoute(isPublicRoute)
	{
		if (typeof isPublicRoute !== 'boolean')
			return

		this.isPublicRoute = isPublicRoute
	},

	/**
	 * Sets whether this is a full-screen page.
	 * @param {boolean} isFullScreenPage Whether or not it's a full-screen page
	 */
	setFullScreenPage(isFullScreenPage)
	{
		if (typeof isFullScreenPage !== 'boolean')
			return

		this.isFullScreenPage = isFullScreenPage
	},

	/**
	 * Adds the specified process to the stack of running processes.
	 * @param {string} processId The id of the process
	 */
	addAsyncProcess(processId)
	{
		if (typeof processId !== 'string' || processId.length === 0)
			return

		this.asyncProcesses.push(processId)
	},

	/**
	 * Removes the process with the specified id from the stack of running processes.
	 * @param {any} processId The id of the process
	 */
	removeAsyncProcess(processId)
	{
		_remove(this.asyncProcesses, (procId) => procId === processId)
	},

	/**
	 * Adds the specified process to the stack of page blocking processes.
	 * @param {object} process The process
	 */
	addProcessToBusyPageStack(process)
	{
		if (typeof process !== 'object' || !Reflect.has(process, 'id'))
			return

		this.busyPageStateStack.push(process)
	},

	/**
	 * Removes the process with the specified id from the stack of page blocking processes.
	 * @param {any} processId The id of the process
	 */
	removeProcessFromBusyPageStack(processId)
	{
		_remove(this.busyPageStateStack, (proc) => proc.id === processId)
	},

	/**
	 * Resets the generic data.
	 */
	resetStore()
	{
		Object.assign(this, state())
	}
}

//----------------------------------------------------------------
// Store export
//----------------------------------------------------------------

export const useGenericDataStore = defineStore('genericData', {
	state,
	getters,
	actions
})
