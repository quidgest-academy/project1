/*****************************************************************
 *                                                               *
 * This store holds tracing events data about the application,   *
 * also defining functions to access and mutate it.              *
 *                                                               *
 *****************************************************************/

import { defineStore } from 'pinia'

import _some from 'lodash-es/some'

import { EventTracker, TraceEventType } from '@/mixins/tracingEvents.js'

//----------------------------------------------------------------
// State variables
//----------------------------------------------------------------

const state = () => {
	return {
		eventTracker: new EventTracker({
			active: false
		})
	}
}

//----------------------------------------------------------------
// Variable getters
//----------------------------------------------------------------

const getters = {
	/**
	 * True if there are any error, false otherwise.
	 * @param {object} state The current global state
	 */
	hasErrors(state)
	{
		return _some(state.eventTracker.events, (event) => event.type === TraceEventType.ERROR || event.type === TraceEventType.SERVER_ERROR)
	},

	/**
	 * True if the event tracing feature is active, false otherwise.
	 * @param {object} state The current global state
	 */
	isEventTracingActive(state)
	{
		return state.eventTracker.active
	}
}

//----------------------------------------------------------------
// Actions
//----------------------------------------------------------------

const actions = {
	/**
	 * Sets the event tracing feature active or inactive.
	 * @param {boolean} active - If the event tracing feature should be active.
	 */
	activateEventTracker(active)
	{
		if (typeof active !== 'boolean')
			return

		this.eventTracker.active = active
		if (!this.eventTracker.active)
			this.eventTracker.reset()
	},

	/**
	 * Adds a new trace event.
	 * @param {Object} eventData - The data for the tracing event.
	 */
	addTrace(eventData)
	{
		if (typeof eventData !== 'object')
			return
		return this.eventTracker.addTrace(eventData)
	},

	/**
	 * Adds a new warning event.
	 * @param {Object} eventData - The data for the warning event.
	 */
	addWarning(eventData)
	{
		if (typeof eventData !== 'object')
			return
		return this.eventTracker.addWarning(eventData)
	},

	/**
	 * Adds a new error event.
	 * @param {Object} eventData - The data for the error event.
	 */
	addError(eventData)
	{
		if (typeof eventData !== 'object')
			return
		return this.eventTracker.addError(eventData)
	},

	/**
	 * Adds a new request trace event.
	 * @param {Object} eventData - The data for the request trace event.
	 */
	addRequestTrace(eventData)
	{
		if (typeof eventData !== 'object')
			return
		return this.eventTracker.addRequestTrace(eventData)
	},

	/**
	 * Adds a new response trace event.
	 * @param {Object} eventData - The data for the response trace event.
	 */
	addResponseTrace(eventData)
	{
		if (typeof eventData !== 'object')
			return
		return this.eventTracker.addResponseTrace(eventData)
	},

	/**
	 * Adds a new server error event.
	 * @param {Object} eventData - The data for the server error event.
	 */
	addServerError(eventData)
	{
		if (typeof eventData !== 'object')
			return
		return this.eventTracker.addResponseTrace(eventData)
	},

	/**
	 * Adds a new server errors event.
	 * @param {Object} eventData - The data for the server errors event.
	 */
	addServerErrors(eventData)
	{
		if (typeof eventData !== 'object')
			return
		return this.eventTracker.addServerErrors(eventData)
	},

	/**
	 * Resets the store to its initial state.
	 */
	resetStore()
	{
		Object.assign(this, state())
	}
}

//----------------------------------------------------------------
// Store export
//----------------------------------------------------------------

export const useTracingDataStore = defineStore('tracingData', {
	state,
	getters,
	actions
})
