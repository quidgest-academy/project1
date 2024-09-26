import _filter from 'lodash-es/filter'
import _forEach from 'lodash-es/forEach'
import _get from 'lodash-es/get'
import _groupBy from 'lodash-es/groupBy'
import _isArray from 'lodash-es/isArray'
import _isEmpty from 'lodash-es/isEmpty'
import _isString from 'lodash-es/isString'
import _map from 'lodash-es/map'
import _orderBy from 'lodash-es/orderBy'
import _toSafeInteger from 'lodash-es/toSafeInteger'
import _uniq from 'lodash-es/uniq'

import { shallowReactive, toRaw } from 'vue'

import { v4 as uuidv4 } from 'uuid'

/**
 * Function to retrieve the call stack.
 * @returns {string} Call stack as a string.
 */
function getCallStack()
{
	return (new Error()).stack
}

/**
 * Enumeration of trace event types.
 * @enum {string}
 */
export const TraceEventType = {
	TRACE: 'trace',
	WARNING: 'warning',
	ERROR: 'error',
	REQUEST: 'request',
	RESPONSE: 'response',
	SERVER_ERROR: 'server error'
}

/**
 * Represents a trace event.
 */
export class TraceEvent
{
	/**
	 * Creates a TraceEvent instance.
	 * @param {Object} options - Event options.
	 * @param {string} [options.origin=''] - Origin of the event.
	 * @param {string} [options.message=''] - Event message.
	 * @param {string} [options.callStack=getCallStack()] - Call stack when the event occurred.
	 * @param {*} options.contextData - Context data associated with the event.
	 * @param {number} [options.timestamp=Date.now()] - Timestamp of the event.
	 */
	constructor(options)
	{
		this.uid = uuidv4()

		this.traceId = _get(options, 'traceId', this.uid)

		this.origin = _get(options, 'origin', '')
		this.message = _get(options, 'message', '')
		this.callStack = _get(options, 'callStack', getCallStack())
		this.contextData = toRaw(_get(options, 'contextData'))
		this.timestamp = _get(options, 'timestamp', Date.now())

		this.type = TraceEventType.TRACE
	}
}

/**
 * Represents a warning event.
 */
export class WarningEvent extends TraceEvent
{
	/**
	 * Creates a WarningEvent instance.
	 * @param {Object} options - Event options.
	 */
	constructor(options)
	{
		super(options)
		this.type = TraceEventType.WARNING
	}
}

/**
 * Represents an error event.
 */
export class ErrorEvent extends TraceEvent
{
	/**
	 * Creates an ErrorEvent instance.
	 * @param {Object} options - Event options.
	 */
	constructor(options)
	{
		super(options)
		this.type = TraceEventType.ERROR
	}
}

/**
 * Represents a request event.
 */
export class RequestEvent extends TraceEvent
{
	/**
	 * Creates a RequestEvent instance.
	 * @param {Object} options - Event options.
	 * @param {string} [options.requestType=''] - The type of the request.
	 * @param {string} [options.requestUrl=''] - The URL of the request.
	 * @param {*} options.requestParams - Parameters of the request.
	 * @param {*} options.requestData - Data associated with the request.
	 */
	constructor(options)
	{
		super(options)

		/**
		 * The type of the request.
		 * @type {string}
		 */
		this.requestType = _get(options, 'requestType', '')

		/**
		 * The URL of the request.
		 * @type {string}
		 */
		this.requestUrl = _get(options, 'requestUrl', '')

		/**
		 * Parameters of the request.
		 * @type {*}
		 */
		this.requestParams = _get(options, 'requestParams')

		/**
		 * Data associated with the request.
		 * @type {*}
		 */
		this.requestData = _get(options, 'requestData')

		this.type = TraceEventType.REQUEST
	}
}

/**
 * Represents a response event.
 */
export class ResponseEvent extends RequestEvent
{
	/**
	 * Creates a ResponseEvent instance.
	 * @param {Object} options - Event options.
	 * @param {string} [options.responseStatus=''] - The status of the response.
	 * @param {*} options.responseData - Data associated with the response.
	 */
	constructor(options)
	{
		super(options)

		/**
		 * The status of the response.
		 * @type {string}
		 */
		this.responseStatus = _get(options, 'responseStatus', '')

		/**
		 * Data associated with the response.
		 * @type {*}
		 */
		this.responseData = _get(options, 'responseData')

		const startAt = _toSafeInteger(_get(options, 'meta.requestStartAt', 0)),
			endAt = _toSafeInteger(_get(options, 'meta.requestEndAt', 0))

		/**
		 * The request duration.
		 * @type {Number}
		 */
		this.time = endAt - startAt

		this.type = TraceEventType.RESPONSE
	}
}

export class ServerErrorEvent extends TraceEvent
{
	/**
	 * Creates an ServerErrorEvent instance.
	 * @param {Object} options - Event options.
	 */
	constructor(options)
	{
		super(options)
		this.type = TraceEventType.SERVER_ERROR
	}
}

/**
 * Maximum number of events to be stored by default.
 * @type {number}
 */
const DEFAULT_MAX_EVENTS_STACK = 75

/**
 * Class for tracking and managing events.
 */
export class EventTracker
{
	/**
	 * Creates an EventTracker instance.
	 * @param {Object} options - Tracker options.
	 * @param {boolean} [options.active=false] - Whether event tracking is active.
	 */
	constructor(options)
	{
		/**
		 * Array to store events.
		 * @type {Array}
		 */
		this.events = shallowReactive([])

		this.active = _get(options, 'active', false)

		/**
		 * Maximum number of events to be stored.
		 * @type {number}
		 */
		this.maxEventsStack = _get(options, 'maxEventsStack', DEFAULT_MAX_EVENTS_STACK)
	}

	/**
	 * Resets the event tracker by clearing stored events.
	 */
	reset()
	{
		this.events.splice()
	}

	/**
	 * Adds an event to the tracker.
	 * @param {TraceEvent|WarningEvent|ErrorEvent|RequestEvent|ResponseEvent} event - The event to be added.
	 */
	addEvent(event)
	{
		if (this.active && event instanceof TraceEvent)
		{
			this.events.push(event)

			if (this.events.length > this.maxEventsStack)
				this.events.splice(0, this.events.length - this.maxEventsStack)
		}
		return event?.traceId
	}

	/**
	 * Adds a trace event to the tracker.
	 * @param {Object} options - Event options.
	 */
	addTrace(options)
	{
		return this.addEvent(new TraceEvent(options))
	}

	/**
	 * Adds a warning event to the tracker.
	 * @param {Object} options - Event options.
	 */
	addWarning(options)
	{
		// To facilitate debugging during development, errors will be added to the console
		// as the debug window will only be accessible if the feature is activated.
		if (process.env.NODE_ENV === 'development')
			// eslint-disable-next-line no-console
			console.warn('Tracing Warning', options)
		return this.addEvent(new WarningEvent(options))
	}

	/**
	 * Adds an error event to the tracker.
	 * @param {Object} options - Event options.
	 */
	addError(options)
	{
		// To facilitate debugging during development, warnings will be added to the console
		// as the debug window will only be accessible if the feature is activated.
		if (process.env.NODE_ENV === 'development')
			// eslint-disable-next-line no-console
			console.error('Tracing Error', options)
		return this.addEvent(new ErrorEvent(options))
	}

	/**
	 * Adds a request trace event to the tracker.
	 * @param {Object} options - Event options.
	 */
	addRequestTrace(options)
	{
		return this.addEvent(new RequestEvent(options))
	}

	/**
	 * Adds a response trace event to the tracker.
	 * @param {Object} options - Event options.
	 */
	addResponseTrace(options)
	{
		return this.addEvent(new ResponseEvent(options))
	}

	/**
	 * Adds a Server error event to the tracker.
	 * @param {Object} options - Event options.
	 */
	addServerError(options)
	{
		return this.addEvent(new ServerErrorEvent(options))
	}

	/**
	 * Adds a Server error events to the tracker.
	 * It must contain an «errors» property with the array of error strings.
	 * @param {Object} options - Event options.
	 */
	addServerErrors(options)
	{
		const errors = _get(options, 'errors')
		const contextData = _get(options, 'contextData')
		const traceId = _get(options, 'traceId')

		if (_isArray(errors))
		{
			_forEach(errors, srvError => {
				if (_isString(srvError))
				{
					this.addServerError({
						origin: 'server',
						callStack: '-',
						message: srvError,
						contextData,
						traceId
					})
				}
			})
		}
	}

	/**
	 * Retrieves events of a specific type.
	 * @param {string} traceEventType - The type of events to retrieve.
	 * @returns {Array} An array of events of the specified type.
	 */
	getEventsOfType(traceEventType)
	{
		if (!_isEmpty(traceEventType))
			return _filter(this.events, event => event.type === traceEventType)
		return this.events
	}

	/**
	 * Retrieves events of specific types.
	 * @param {Array} traceEventTypes - The types of events to retrieve.
	 * @returns {Array} An array of events of the specified types.
	 */
	getEventsOfTypes(traceEventTypes)
	{
		if (_isArray(traceEventTypes) && traceEventTypes.length > 0)
			return _filter(this.events, event => traceEventTypes.includes(event.type))
		return this.events
	}

	/**
	 * Groups events by «traceId».
	 * @returns {Array} An array of events grouped by «traceId».
	 */
	getEventsByGroup()
	{
		const errors = this.getEventsOfTypes([ TraceEventType.ERROR, TraceEventType.SERVER_ERROR ])
		const traceIds = _uniq(_map(errors, error => error.traceId))
		const relatedEvents = _filter(this.events, event => traceIds.includes(event.traceId))
		const groups = _groupBy(_orderBy(relatedEvents, 'timestamp'), 'traceId')
		return groups
	}
}

/**
 * Module exporting TraceEventType, event classes, and EventTracker.
 * @module
 */
export default {
	TraceEventType,
	TraceEvent,
	WarningEvent,
	ErrorEvent,
	EventTracker
}
