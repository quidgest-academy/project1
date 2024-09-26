import { markRaw } from 'vue'
import { v4 as uuidv4 } from 'uuid'
import _forEach from 'lodash-es/forEach'
import _isEmpty from 'lodash-es/isEmpty'
import _has from 'lodash-es/has'
import _get from 'lodash-es/get'
import _toPath from 'lodash-es/toPath'

import { QEventEmitter } from '@/api/global/eventBus.js'
import modelFieldType from '@/mixins/formModelFieldTypes.js'

import { useGlobalTablesDataStore } from '@/stores/globalTablesData.js'

export default class ViewModelBase
{
	constructor(vueContext, options)
	{
		// The Vue context properties.
		Object.defineProperty(this, 'vueContext', {
			value: (vueContext || {}),
			enumerable: false
		})

		Object.defineProperty(this, 'Resources', {
			get() { return this.vueContext.Resources },
			enumerable: false
		})

		Object.defineProperty(this, 'navigationId', {
			get() { return this.vueContext.navigationId },
			enumerable: false,
			configurable: true
		})

		// Unique identifier.
		Object.defineProperty(this, 'uniqueIdentifier', {
			value: uuidv4(),
			enumerable: false
		})

		// Internal events for the formulas.
		Object.defineProperty(this, 'internalEvents', {
			value: markRaw(new QEventEmitter()),
			enumerable: false
		})

		// External callback for invocation of external methods such as onUpdate of fields.
		Object.defineProperty(this, 'externalCallbacks', {
			value: markRaw({
				onUpdate: options?.callbacks?.onUpdate,
				setFormKey: options?.callbacks?.setFormKey
			}),
			enumerable: false
		})

		// The extra properties of the form or menu.
		Object.defineProperty(this, 'extraProperties', {
			value: {},
			enumerable: false,
			writable: true
		})

		// List of server errors associated to the model.
		Object.defineProperty(this, 'serverErrorMessages', {
			value: [],
			enumerable: false,
			writable: true
		})

		// List of server warnings associated to the model.
		Object.defineProperty(this, 'serverWarningMessages', {
			value: [],
			enumerable: false,
			writable: true
		})
	}

	/**
	 * Getter for the GLOB table (if it exists).
	 */
	get tGlob()
	{
		const globalTablesData = useGlobalTablesDataStore()
		return globalTablesData.getTableByName('Glob')
	}

	/**
	 * Checks if this model is equal to the specified one.
	 * @param {object} otherModel The other model
	 * @returns True if the models are equal, false otherwise.
	 */
	equals(otherModel)
	{
		if (!(otherModel instanceof ViewModelBase))
			return false

		for (let modelField in this)
		{
			const fieldObj = this[modelField]

			if (!(fieldObj instanceof modelFieldType.Base) ||
				!fieldObj.hasSameValue(otherModel[modelField]?.value))
				return false
		}

		return true
	}

	/**
	 * Creates a clone of the current instance.
	 */
	clone()
	{
		throw new Error('This method should be implemented in a sub-class.')
	}

	/**
	 * Hydrates the raw data coming from the server with the necessary metadata.
	 * @param {object} rawData The data to be hydrated
	 */
	hydrate(rawData)
	{
		for (let modelField in this)
			if (this[modelField] instanceof modelFieldType.Base)
				this.hydrateField(modelField, rawData)

		// Global tables
		const globalTablesData = useGlobalTablesDataStore()
		globalTablesData.loadFromViewModel(rawData)

		// The extra properties of the form or menu
		this.setExtraProperties(rawData?.ExtraProperties)
	}

	/**
	 * Hydrates the raw data for a given field coming from the server
	 * with the necessary metadata.
	 * @param {object} modelField The target field
	 * @param {object} rawData The data to be hydrated
	 */
	hydrateField(modelField, rawData)
	{
		const fieldObj = this[modelField]

		if (!(fieldObj instanceof modelFieldType.Base) || fieldObj.isReady || !_has(rawData, modelField))
			return

		let rawDataFieldValue = rawData[modelField]

		if (typeof fieldObj.hydrate === 'function')
			fieldObj.hydrate(rawDataFieldValue)
	}

	/**
	 * Adds the specified properties to the extra properties of the form or menu.
	 * @param {object} extraProps The extra properties
	 */
	setExtraProperties(extraProps)
	{
		if (_isEmpty(extraProps))
			return

		this.extraProperties = {
			...this.extraProperties,
			...extraProps
		}
	}

	/**
	 * Emits an internal event.
	 * @param {string} eventName The name of the event
	 * @param {any} eventData The data to be emitted
	 */
	emitInternalEvent(eventName, eventData)
	{
		this.internalEvents.emit(eventName, eventData)
	}

	/**
	 * Handler to be called whenever the value of a field changes.
	 * @param {string} modelFieldName The id of the field
	 * @param {object} modelField The field
	 * @param {any} newValue The field's new value
	 * @param {any} oldValue The field's old value
	 */
	onUpdate(modelFieldName, modelField, newValue, oldValue)
	{
		// Foreign keys will also enter here, since it's a sub-class of primary key.
		if (modelField instanceof modelFieldType.PrimaryKey)
		{
			if (typeof this.externalCallbacks.setFormKey === 'function')
				this.externalCallbacks.setFormKey(modelField)

			// Don't emit event when key value is changed between empty string and null.
			if (!_isEmpty(newValue) || !_isEmpty(oldValue))
				this.emitInternalEvent(`fieldChange:${modelFieldName}`, { modelFieldName, modelField, newValue, oldValue })
		}
		else
			this.emitInternalEvent(`fieldChange:${modelFieldName}`, { modelFieldName, modelField, newValue, oldValue })

		if (typeof this.externalCallbacks.onUpdate === 'function')
			this.externalCallbacks.onUpdate(modelFieldName, modelField, newValue, oldValue)
	}

	/**
	 * Sets the external callbacks.
	 * @param {array} callbacks The callbacks list
	 * @returns {object} The current state of the model.
	 */
	setExternalCallback(callbacks)
	{
		_forEach(callbacks, (fn, cbName) => Reflect.set(this.externalCallbacks, cbName, fn))
		return this
	}

	/**
	 * Sets the current navigation id.
	 * @param {string} propertyRef The navigation id
	 * @returns {object} The current state of the model.
	 */
	setNavigationId(propertyRef)
	{
		Object.defineProperty(this, 'navigationId', {
			get() { return propertyRef },
			enumerable: false,
			configurable: true
		})

		return this
	}

	/**
	 * Set server error messages associated with the model field.
	 * @param {string|array} key The model field path
	 * @param {object} errors The server errors
	 */
	setServerErrorMessages(key, errors)
	{
		const path = typeof key === 'string' ? _toPath(key) : key
		const field = _get(this, path.shift())

		if (field instanceof modelFieldType.Base)
			field.setServerErrorMessages(errors, path)
		else
			this.serverErrorMessages.push(...errors)
	}

	/**
	 * Checks if there are any server error messages associated with the field.
	 * @returns {boolean} True if there are server error messages, false otherwise.
	 */
	hasServerErrorMessages()
	{
		if (this.serverErrorMessages.length > 0)
			return true

		for (let modelField in this)
			if (typeof this[modelField].hasServerErrorMessages === 'function'
				&& this[modelField].hasServerErrorMessages())
				return true

		return false
	}

	/**
	 * Clears the server error messages associated with the field.
	 */
	clearServerErrorMessages()
	{
		this.serverErrorMessages.splice(0)
		for (let modelField in this)
			this[modelField].clearServerErrorMessages()
	}

	/**
	 * Set server warning messages associated with the model field.
	 * @param {*} key The model field
	 * @param {*} warnings The server warnings
	 */
	setServerWarningMessages(key, warnings)
	{
		const path = typeof key === 'string' ? _toPath(key) : key
		const field = _get(this, path.shift())

		if (field instanceof modelFieldType.Base)
			field.setServerWarningMessages(warnings, path)
		else
			this.serverWarningMessages.push(...warnings)
	}

	/**
	 * Checks if there are any server warning messages associated with the field.
	 * @returns {boolean} True if there are server warning messages, false otherwise.
	 */
	hasServerWarningMessages()
	{
		if (!this.serverWarningMessages)
			return false

		if(this.serverWarningMessages.length > 0)
			return true

		for (let modelField in this)
			if (typeof this[modelField].hasServerWarningMessages === 'function' &&
				this[modelField].hasServerWarningMessages())
				return true
		return false
	}

	/**
	 * Clears the server warning messages associated with the field.
	 */
	clearServerWarningMessages()
	{
		this.serverWarningMessages.splice(0)
		for (let modelField in this)
			this[modelField].clearServerErrorMessages()
	}

	/**
	 * Unbinds all related events.
	 */
	unbindEvents()
	{
		this.internalEvents.removeAllListeners()
	}

	/**
	 * Destroy current model object.
	 */
	destroy()
	{
		this.unbindEvents()
	}
}
