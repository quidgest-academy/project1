import { computed, reactive, readonly } from 'vue'
import { validate as uuidValidate, v4 as uuidv4 } from 'uuid'
import _assignIn from 'lodash-es/assignIn'
import _cloneDeep from 'lodash-es/cloneDeep'
import _findIndex from 'lodash-es/findIndex'
import _flatMap from 'lodash-es/flatMap'
import _forEach from 'lodash-es/forEach'
import _get from 'lodash-es/get'
import _has from 'lodash-es/has'
import _isEmpty from 'lodash-es/isEmpty'
import _isEqual from 'lodash-es/isEqual'
import _some from 'lodash-es/some'
import _toNumber from 'lodash-es/toNumber'

import { useSystemDataStore } from '@/stores/systemData.js'
import { useTracingDataStore } from '@/stores/tracingData.js'
import { useUserDataStore } from '@/stores/userData.js'

import { postData } from '@/api/network'
import { validateCoordinate } from '@/utils/geography.js'
import genericFunctions from '@/mixins/genericFunctions.js'

export class Base
{
	static EMPTY_VALUE = null

	constructor(options)
	{
		this.type = null
		this.id = null
		this.originId = null
		this.area = null
		this.field = null
		this.relatedArea = null
		this.valueFormula = null
		this.showWhenConditions = []
		this.blockWhenConditions = []
		// Ignore the field when the model is submitted to the server.
		this.ignoreFldSubmit = false
		this.isRequired = false
		this.originalValue = this.constructor.EMPTY_VALUE
		this.arrayOptions = []
		this.serverErrorMessages = []
		this.serverWarningMessages = []
		// Indicates if the field is permanently readonly, regardless of form mode.
		this.isFixed = false

		// This should be a private field, but unfortunately they don't work with proxies:
		// https://github.com/tc39/proposal-class-fields/issues/106
		Object.defineProperty(this, '_value', {
			value: this.constructor.EMPTY_VALUE,
			configurable: true,
			writable: true,
			enumerable: false
		})

		_assignIn(this, options)
	}

	/**
	 * The current value of the field.
	 */
	get value()
	{
		return this._value
	}

	/**
	 * Setter for the field value.
	 */
	set value(newValue)
	{
		this.updateValue(newValue)
	}

	/**
	 * Checks if the field's value is different from its original value (dirty).
	 * @type {boolean} True if the field's value is dirty, false otherwise.
	 */
	get isDirty()
	{
		return !this.isFixed && !this.hasSameValue(this.originalValue)
	}

	/**
	 * Retrieves the display value of the field.
	 * @type {string} The display value of the field.
	 */
	get displayValue()
	{
		return this.parseValue(this.value)
	}

	/**
	 * The value in the format expected by the server-side.
	 */
	get serverValue()
	{
		return this.value
	}

	/**
	 * Parses the given value based on the specified rules and options.
	 *
	 * If this is an array field, the display value corresponds to the corresponding
	 * value from the arrayOptions based on the current value (array key).
	 *
	 * Otherwise, it returns the string representation of the current value,
	 * or an empty string if the value is undefined or null.
	 *
	 * @param {any} value - The value to be parsed.
	 * @returns {string} The parsed value as a string.
	 */
	parseValue(value)
	{
		// If this is an array field, the value will correspond to the array key, not the actual value.
		if (!_isEmpty(this.arrayOptions))
		{
			const option = this.arrayOptions.find((e) => e.key === value)

			if (!_isEmpty(option))
				return option.value?.toString() ?? ''
		}

		return value?.toString() ?? ''
	}

	/**
	 * Updates the value of the field.
	 *
	 * This method accepts a new value and updates the field's value accordingly.
	 * If the provided value is an object with a 'Value' property, it is treated as a special case
	 * for handling dropdown options where 'Value' represents the new value, and 'List' contains the options.
	 * If 'List' is an array, it sets the options list and tries to add the selected option to the list if not already present.
	 * Otherwise, it directly sets the provided value as the field's value.
	 *
	 * Note: To keep the context «this» and for it to work on «@update:model-value="model.ValField.updateValue"»,
	 * it needs to be declared this way and not as a function of the class.
	 *
	 * @param {any} newValue - The new value to set for the field
	 */
	updateValue(newValue)
	{
		let value

		// Prototype. So that it is possible to use the dropdowns that send the object with Text and Value of the option.
		if (!_isEmpty(newValue) && this.type === 'Lookup')
		{
			// The initial options list of the dropdown (lazy load - may have one option previously selected).
			if (Array.isArray(newValue.List))
			{
				let items = newValue.List

				items = items.map((item) => ({
					key: item.key,
					// FIXME: review need for computed once i18n is refactored.
					value: computed(() => this.parseValue(item.value))
				}))

				reactive(this).options = items

				// If for some reason the selected option is not in the list of options, add it.
				if (!_isEmpty(newValue.Selected) &&
					!_some(newValue.List, (option) => option.key === newValue.Selected))
				{
					const selectedItem = {
						key: newValue.Selected,
						// FIXME: review need for computed once i18n is refactored.
						value: computed(() => this.parseValue(newValue.Value))
					}

					reactive(this).options.unshift(selectedItem)
				}

				// Total rows is unknown if query returned results and response.TotalRows is "0"
				const isTotalRowsUnknown = newValue.List.length > 0 && newValue.TotalRows === 0

				reactive(this).totalRows = isTotalRowsUnknown
					? undefined
					: Math.max(newValue.TotalRows, items.length)
			}

			// If value is an object
			if (_has(newValue, 'Value'))
				value = newValue.Value
			else
				value = newValue
		}
		else
			value = newValue

		if (this.isValidType(value))
			reactive(this)._value = this.sanitizeValue(value)
		else
		{
			const tracing = useTracingDataStore()
			tracing.addError({
				origin: 'updateValue',
				message: `Tried to assign an unsupported value type to "${this.id}".`,
				contextData: value
			})
		}
	}

	/**
	 * To keep the context «this» and for it to work on «@update:model-value="model.ValField.updateValue"»,
	 * it needs to be bound in a function.
	 * @param {any} newValue - The new value to set for the field
	 */
	fnUpdateValue = (newValue) => this.updateValue(newValue)

	/**
	 * Updates the value of the field from the change event.
	 * @param {object} event - The change event
	 */
	fnUpdateValueOnChange = (event) => this.updateValue(event.target?.value)

	/**
	 * Sanitizes the specified value, can be useful so the field won't be marked as dirty
	 * when assigned a different value, but still equivalent.
	 * @param {any} value - The value to sanitize
	 * @returns The sanitized value
	 */
	sanitizeValue(value)
	{
		if (!this.isValidType(value))
			throw new Error('Unsupported value type.')
		return value
	}

	/**
	 * Resets the current value back to it's original one.
	 */
	resetValue()
	{
		if (!this.isDirty)
			return

		this.hydrate(this.originalValue)
	}

	/**
	 * Hydrates the raw data for this field coming from the server
	 * with the necessary metadata.
	 * @param {object} rawDataFieldValue - The data to be hydrated
	 */
	hydrate(rawDataFieldValue)
	{
		let rawDataFieldOriginalValue = undefined

		if (rawDataFieldValue instanceof Base)
		{
			rawDataFieldOriginalValue = rawDataFieldValue.originalValue
			rawDataFieldValue = rawDataFieldValue.value
		}

		this.updateValue(rawDataFieldValue)

		// Deep clone is used to ensure the object is not reactive
		this.originalValue = rawDataFieldOriginalValue === undefined
			? this.cloneValue()
			: _cloneDeep(rawDataFieldOriginalValue)

		this.isReady = true
	}

	/**
	 * Initializes this field with a clone of the value of the provided field.
	 * @param {object} other - The field to clone the value from
	 * @returns {this} The current instance with the cloned value
	 */
	cloneFrom(other)
	{
		if (other instanceof Base)
		{
			const value = other.cloneValue()
			/*
				The lookup fields, in addition to the value, also have a list of options.
				If we don't clone this list, when we change the form's mode,
					the GridTableList will lose the Lookups data during the recovery of the Grid's original value (resetValue).
				TODO: However, it is necessary to change the logic of changing the mode.
						It should make a request to the server to load the new form data
						OR
						Requires revision for the manwin «BEFORE_LOAD_...» and IF's based on the mode in the Load of the ViewModel.
			*/
			if (this.type === 'Lookup' && other.type === 'Lookup' && Array.isArray(other.options))
				this.hydrate({ Value: value, List: _cloneDeep(other.options) })
			else
				this.hydrate(value)
		}

		return this
	}

	/**
	 * Deep clones the field's value.
	 * @returns {any} A deep cloned value of the field.
	 */
	cloneValue()
	{
		return _cloneDeep(this.value)
	}

	/**
	 * Checks if the field's value is equal to the provided value.
	 * @param {any} otherValue - The value to compare with the field's value
	 * @returns {boolean} True if the field's value is equal to the provided value, false otherwise.
	 */
	hasSameValue(otherValue)
	{
		return _isEqual(this.value, otherValue)
	}

	/**
	 * Clears the field's value by setting it to the field's standard empty value.
	 * @param {any} value - A value to overwrite the standard empty value
	 */
	clearValue(value)
	{
		const val = typeof value === 'undefined' ? this.constructor.EMPTY_VALUE : value
		this.updateValue(val)
	}

	/**
	 * Validates the size of the field.
	 * @returns {boolean} True if the field's size is valid, false otherwise.
	 */
	validateSize()
	{
		return true
	}

	/**
	 * Validates the value of the field.
	 * @returns {boolean} True if the field's value is valid, false otherwise.
	 */
	validateValue()
	{
		return this.isRequired
			? this.value !== this.constructor.EMPTY_VALUE
			: true
	}

	/**
	 * Checks if the specified value has a valid type.
	 * @param {any} value - The value to check
	 * @returns True if the specified value is of a valid type, false otherwise
	 */
	isValidType()
	{
		return true
	}

	/**
	 * Set server error messages associated with the field.
	 * @param {Array} errors The server errors
	 */
	setServerErrorMessages(errors)
	{
		this.serverErrorMessages = errors
	}

	/**
	 * Checks if there are any server error messages associated with the field.
	 * @returns {boolean} True if there are server error messages, false otherwise.
	 */
	hasServerErrorMessages()
	{
		return this.serverErrorMessages.length > 0
	}

	/**
	 * Clears the server error messages associated with the field.
	 */
	clearServerErrorMessages()
	{
		this.serverErrorMessages.length = 0
	}

	/**
	 * Set server warning messages associated with the field.
	 * @param {Array} warnings The server warnings
	 */
	setServerWarningMessages()
	{
		this.serverWarningMessages = []
	}

	/**
	 * Checks if there are any server warning messages associated with the field.
	 * @returns {boolean} True if there are server warning messages, false otherwise.
	 */
	hasServerWarningMessages()
	{
		return this.serverWarningMessages.length > 0
	}

	/**
	 * Clears the server warning messages associated with the field.
	 */
	clearServerWarningMessages()
	{
		this.serverWarningMessages.length = 0
	}
}

export class String extends Base
{
	static EMPTY_VALUE = ''

	constructor(options)
	{
		super({
			type: 'String',
			maxLength: -1
		})

		_assignIn(this, options)
	}

	/**
	 * @override
	 */
	sanitizeValue(value)
	{
		const sanitizedVal = super.sanitizeValue(value)

		if (genericFunctions.isEmpty(sanitizedVal))
			return this.constructor.EMPTY_VALUE

		return sanitizedVal
	}

	/**
	 * @override
	 */
	validateSize()
	{
		if (this.maxLength > 0)
		{
			const length = this.value?.length ?? 0
			return length <= this.maxLength
		}
		return true
	}

	/**
	 * @override
	 */
	isValidType(value)
	{
		return typeof value === 'string' || genericFunctions.isEmpty(value)
	}
}

export class MultiLineString extends String
{
	constructor(options)
	{
		super({
			maxLength: -1
		})

		_assignIn(this, options)

		// No limit (varchar max)
		this.maxLength = -1
	}

	/**
	 * @override
	 */
	get serverValue() {
		//The server expects \r\n, but text edited through web textarea only has \n
		//convert first from server format to web format, in case the text came from server and wasn't edited
		let value = this.value.replaceAll("\r\n", "\n");
		//Convert to server format
		return value.replaceAll("\n", "\r\n");
	}
}

export class Password extends String
{
	constructor(options)
	{
		super({
			type: 'Password',
			maxLength: -1
		})

		_assignIn(this, options)
	}
}

export class PrimaryKey extends String
{
	constructor(options)
	{
		super({
			maxLength: 16
		})

		_assignIn(this, options)
	}

	/**
	 * @override
	 */
	get serverValue()
	{
		return this.value === this.constructor.EMPTY_VALUE ? null : this.value
	}

	/**
	 * @override
	 */
	validateSize()
	{
		// GUIDs
		if (this.maxLength === 16)
			return _isEmpty(this.value) || uuidValidate(this.value)
		// Other key types
		return super.validateSize()
	}
}

export class ForeignKey extends PrimaryKey
{
	constructor(options)
	{
		super({
			relatedArea: null
		})

		_assignIn(this, options)
	}
}

export class Coordinate extends String
{
	constructor(options)
	{
		super({
			type: 'Coordinate'
		})

		_assignIn(this, options)
	}

	/**
	 * @override
	 */
	isValidType(value)
	{
		return super.isValidType(value) && validateCoordinate(value)
	}
}

export class Geographic extends Base
{
	constructor(options)
	{
		super({
			type: 'Geographic'
		})

		_assignIn(this, options)
	}

	/**
	 * @override
	 */
	isValidType(value)
	{
		return typeof value === 'object'
	}
}

export class Date extends Base
{
	static EMPTY_VALUE = ''

	constructor(options)
	{
		const systemDataStore = useSystemDataStore()

		super({
			type: 'Date',
			dateFormat: systemDataStore.system.dateFormat.date
		})

		_assignIn(this, options)
	}

	/**
	 * @override
	 */
	get displayValue()
	{
		return genericFunctions.dateDisplay(this.value, this.dateFormat)
	}

	/**
	 * @override
	 */
	get serverValue()
	{
		return genericFunctions.dateToISOString(this.value)
	}

	/**
	 * @override
	 */
	isValidType(value)
	{
		return genericFunctions.isDate(value) && !isNaN(value) || genericFunctions.isEmpty(value)
	}

	/**
	 * @override
	 */
	sanitizeValue(value)
	{
		const sanitizedVal = super.sanitizeValue(value)

		if (genericFunctions.isEmpty(sanitizedVal))
			return this.constructor.EMPTY_VALUE

		return new window.Date(window.Date.parse(sanitizedVal))
	}
}

export class DateTime extends Date
{
	constructor(options)
	{
		const systemDataStore = useSystemDataStore()

		super({
			type: 'DateTime',
			dateFormat: systemDataStore.system.dateFormat.dateTime
		})

		_assignIn(this, options)
	}
}

export class DateTimeSeconds extends DateTime
{
	constructor(options)
	{
		const systemDataStore = useSystemDataStore()

		super({
			type: 'DateTimeSeconds',
			dateFormat: systemDataStore.system.dateFormat.dateTimeSeconds
		})

		_assignIn(this, options)
	}
}

export class Time extends Base
{
	static EMPTY_VALUE = '__:__'

	constructor(options)
	{
		super({
			type: 'Time'
		})

		_assignIn(this, options)
	}

	/**
	 * @override
	 */
	get displayValue()
	{
		if (_isEmpty(super.displayValue) || super.displayValue === Time.EMPTY_VALUE)
			return ''

		return genericFunctions.timeToString(this.value)
	}

	/**
	 * @override
	 */
	get serverValue()
	{
		return this.value !== Time.EMPTY_VALUE ? this.value : null
	}

	/**
	 * @override
	 */
	hydrate(rawDataFieldValue)
	{
		// Ensure instance-specific empty value representation
		// (convert '' to '__:__')
		if (_isEmpty(rawDataFieldValue))
			rawDataFieldValue = Time.EMPTY_VALUE

		super.hydrate(rawDataFieldValue)
	}

	/**
	 * @override
	 */
	isValidType(value)
	{
		return typeof value === 'object' || typeof value === 'string' || value === null
	}

	/**
	 * @override
	 */
	sanitizeValue(value)
	{
		const sanitizedVal = super.sanitizeValue(value)

		if (typeof sanitizedVal === 'object')
			return sanitizedVal ? genericFunctions.timeToString(sanitizedVal) : ''

		return sanitizedVal
	}
}

export class Boolean extends Base
{
	constructor(options)
	{
		super({
			type: 'Boolean'
		})

		_assignIn(this, options)
	}

	/**
	 * @override
	 */
	get serverValue()
	{
		return this.value ?? false
	}

	/**
	 * @override
	 */
	sanitizeValue(value)
	{
		const sanitizedVal = super.sanitizeValue(value)

		if (typeof sanitizedVal === 'number')
			return sanitizedVal === 1

		return sanitizedVal
	}

	/**
	 * @override
	 */
	clearValue()
	{
		super.clearValue(false)
	}

	/**
	 * @override
	 */
	isValidType(value)
	{
		return typeof value === 'boolean' || value === this.constructor.EMPTY_VALUE || [0, 1].includes(value)
	}
}

export class Number extends Base
{
	static EMPTY_VALUE = 0

	constructor(options)
	{
		super({
			type: 'Number',
			maxDigits: -1,
			decimalDigits: 0,
			maxIntegers: -1,
			maxDecimals: -1
		})

		_assignIn(this, options)
	}

	/**
	 * @override
	 */
	get displayValue()
	{
		const value = _toNumber(this.value)
		if (isNaN(value))
			return ''
		return value.toFixed(this.decimalDigits)
	}

	/**
	 * @override
	 */
	sanitizeValue(value)
	{
		const sanitizedVal = super.sanitizeValue(value)
		return _toNumber(sanitizedVal)
	}

	/**
	 * @override
	 */
	validateValue()
	{
		return super.validateValue() && (this.isRequired ? !isNaN(_toNumber(this.value)) : true)
	}

	/**
	 * @override
	 */
	isValidType(value)
	{
		return !isNaN(value)
	}
}

export class Image extends Base
{
	constructor(options)
	{
		super({
			type: 'Image'
		})

		_assignIn(this, options)
	}

	/**
	 * @override
	 */
	isValidType(value)
	{
		return genericFunctions.validateImageFormat(value)
	}
}

class DocumentData
{
	constructor(options)
	{
		this.id = null
		this.versionSubmitAction = readonly({
			insert: 0, // The initial version of the file was submitted.
			submit: 1, // A new version of an already existing file was submitted.
			unlock: 2  // No new version was submitted, the editing state was simply changed.
		})
		this.deleteTypes = readonly({
			current: 0,  // Deletes the lastest version.
			versions: 1, // Deletes all versions except the last one.
			all: 2       // Deletes the document and all it's versions.
		})
		this.ticket = null
		this.fileData = null
		// Set default as no delete type
		this.deleteType = -1
		// Set default as no submit mode
		this.submitMode = -1
		this.version = '1'

		_assignIn(this, options)
	}

	/**
	 * Whether there are changes in this document that need to be saved.
	 */
	get hasUnsavedChanges()
	{
		return this.fileData !== null || this.deleteType !== -1
	}

	/**
	 * The document properties.
	 */
	get properties()
	{
		let createdDate = null,
			currentUser = '',
			fileName = '',
			extension = '',
			fileSize = ''

		if (this.fileData !== null)
		{
			const userDataStore = useUserDataStore()
			currentUser = userDataStore.username

			createdDate = new Date()
			createdDate.updateValue(this.fileData.lastModifiedDate)

			fileName = this.fileData.name
			extension = fileName.split('.').pop().toLowerCase()
			fileSize = `${this.fileData.size} bytes`
		}

		return {
			author: currentUser,
			createdDate: createdDate?.displayValue ?? '',
			editor: currentUser,
			fileType: extension,
			name: fileName,
			size: fileSize,
			version: this.version
		}
	}

	/**
	 * The document data to be submitted to the server.
	 */
	get dataToSubmit()
	{
		if (this.fileData === null)
			return null

		const submitData = new FormData()

		submitData.append(`${this.id}_file`, this.fileData)
		submitData.append('ticket', this.ticket)
		submitData.append('mode', this.submitMode)
		submitData.append('version', this.version)

		return submitData
	}

	/**
	 * Sets up the necessary document properties.
	 * @param {string} id The field id
	 * @param {string} ticket The file ticket
	 */
	setup(id, ticket)
	{
		this.id = id
		this.ticket = ticket
	}

	/**
	 * Resets the document properties.
	 */
	reset()
	{
		this.fileData = null
		this.deleteType = -1
	}

	/**
	 * Sets a new unsaved file.
	 * @param {object} file The file
	 * @param {number} submitMode The type of submit
	 * @param {string} version The document version
	 */
	setNewFile(file, submitMode, version = '1')
	{
		if (!(file instanceof File) ||
			!Object.values(this.versionSubmitAction).includes(submitMode) ||
			typeof version !== 'string')
			return

		this.fileData = file
		this.submitMode = submitMode
		this.version = version
	}

	/**
	 * Deletes the file and possibly it's versions, depending on the specified delete type.
	 * @param {number} deleteType The type of delete action to perform
	 */
	delete(deleteType)
	{
		if (!Object.values(this.deleteTypes).includes(deleteType))
			return

		this.deleteType = -1

		if (deleteType === this.deleteTypes.current)
		{
			if (this.fileData !== null)
				this.fileData = null
			else
				this.deleteType = this.deleteTypes.current
		}
		else if (deleteType === this.deleteTypes.versions)
			this.deleteType = this.deleteTypes.versions
		else
		{
			this.fileData = null
			this.deleteType = this.deleteTypes.all
		}
	}
}

export class Document extends Base
{
	constructor(options)
	{
		super({
			type: 'Document',
			currentDocument: new DocumentData(),
			tickets: {},
			properties: null,
			documentFK: null
		})

		_assignIn(this, options)
	}

	/**
	 * @override
	 */
	get isDirty()
	{
		return super.isDirty || this.properties.isDirty || this.currentDocument.hasUnsavedChanges
	}

	/**
	 * @override
	 */
	isValidType(value)
	{
		return typeof value === 'string' || value === this.constructor.EMPTY_VALUE
	}

	/**
	 * Sets the tickets to retrieve every document version from the server.
	 * @param {string} primaryKey The primary key of the current record
	 * @param {string} navigationId The current navigation id
	 * @returns A promise with the response from the server.
	 */
	setTickets(primaryKey, navigationId)
	{
		const params = {
			tableName: this.area,
			fieldName: this.originId,
			keyValue: primaryKey
		}

		return new Promise((resolve) => {
			postData(
				this.area,
				'GetDocumsTickets',
				params,
				(data, request) => {
					if (request.data?.Success)
					{
						this.tickets = {}
						for (let i in data.tickets)
						{
							const t = data.tickets[i]
							this.tickets[t.id] = t.ticket
						}

						this.properties.updateValue(data.properties)
						this.documentFK.updateValue(data.properties?.documentId ?? '')

						// Sets up the current document properties.
						this.currentDocument.setup(this.id, this.tickets.main)

						resolve(true)
					}
					else
					{
						const tracingDataStore = useTracingDataStore()
						tracingDataStore.addError({
							origin: 'setTickets',
							message: `Error found while trying to retrieve the document tickets for field "${this.id}".`
						})

						resolve(false)
					}
				},
				undefined,
				undefined,
				navigationId)
		})
	}
}

export class MultipleValues extends Base
{
	constructor(options)
	{
		super({
			type: 'MultipleValues',
			value: []
		})

		_assignIn(this, options)
	}

	/**
	 * @override
	 */
	clearValue()
	{
		super.clearValue([])
	}

	/**
	 * @override
	 */
	isValidType(value)
	{
		return typeof value === 'object'
	}
}

class GridTableListValue
{
	constructor(fieldValue)
	{
		this.elements = []
		this.newElements = _get(fieldValue, 'newElements', [])
		this.newRecordTemplate = _get(fieldValue, 'newRecordTemplate')
		this.editedElements = _get(fieldValue, 'editedElements', [])
		this.removedElements = _get(fieldValue, 'removedElements', [])
	}

	/**
	 * A list of the rows that aren't dirty.
	 */
	get emptyRows()
	{
		return this.newElements.filter((row) => !row.isDirty)
	}

	/**
	 * Whether the row is dirty.
	 */
	get isDirty()
	{
		return _some([
			_some(this.elements, (el) => el.isDirty),
			_some(this.newElements, (el) => el.isDirty),
		])
	}

	/**
	 * The value in the format expected by the server-side.
	 */
	get serverValue()
	{
		// For existing rows, we only send those that are edited (dirty)
		// and are not marked to be deleted.
		this.editedElements = this.elements.filter((row) => row.isDirty && !this.removedElements.includes(row.QPrimaryKey))
		const svrEditedElements = _flatMap(this.editedElements, (row) => row.serverObjModel)

		// For new rows, we must clear the client-side key.
		// Only those that are not empty (dirty) are sent.
		const svrNewElements = _flatMap(this.newElements.filter((row) => row.isDirty), (row) => {
			row.QPrimaryKey = null
			return row.serverObjModel
		})

		return {
			editedElements: svrEditedElements,
			newElements: svrNewElements,
			removedElements: this.removedElements
		}
	}

	/**
	 * Hydrates and returns a new view model.
	 * @param {object} viewModelData The view model data
	 * @param {object} viewModelClass The class for the grid view model
	 * @param {object} vueContext The Vue context in which this value will be used
	 * @returns A new view model of type viewModelClass.
	 */
	getViewModel(viewModelData, viewModelClass, vueContext)
	{
		if (viewModelData === undefined || viewModelClass === undefined || vueContext === undefined)
			return undefined

		const viewModel = new viewModelClass(vueContext)
		viewModel.hydrate(viewModelData)
		return viewModel
	}

	/**
	 * Adds a view model object to the list of new elements.
	 * @param {object} viewModelData The view model data
	 * @param {object} viewModelClass The class for the grid view model
	 * @param {object} vueContext The Vue context in which this value will be used
	 */
	addNewModel(viewModelData, viewModelClass, vueContext)
	{
		const viewModel = this.getViewModel(viewModelData, viewModelClass, vueContext)
		if (viewModel !== undefined)
			this.newElements.push(viewModel)
	}

	/**
	 * Removes empty rows from the list of new elements. Optionally retains one last empty row.
	 * @param {boolean} full A flag indicating whether to remove all empty rows or leave one remaining
	 */
	trimEmptyRows(full)
	{
		let pop = this.emptyRows.length
		if (!full)
			pop--

		while (pop--)
			this.newElements.pop()

		// Ensure the row left by the trim operation has no
		// server error messages from previous attempts to save the form
		_forEach(this.emptyRows, (row) => row.clearServerErrorMessages())
	}

	/**
	 * Marks the given view model as deleted or removes it from the list of new elements.
	 * @param {object} viewModelData The view model to be marked for deletion
	 */
	markForDeletion(viewModelData)
	{
		// Check if this is a new row
		// New rows are removed immediately
		// instead of being marked to be deleted
		const index = this.newElements.indexOf(viewModelData)

		if (index > -1)
			this.newElements.splice(index, 1)
		else
			this.removedElements.push(viewModelData.QPrimaryKey)
	}

	/**
	 * Reverts the deletion mark from the given view model, if it was previously marked.
	 * @param {object} viewModelData The view model to undo deletion
	 */
	undoDeletion(viewModelData)
	{
		const index = this.removedElements.indexOf(viewModelData.QPrimaryKey)

		if (index > -1)
			this.removedElements.splice(index, 1)
	}

	/**
	 * Sets the whole value of the grid table list including its elements, new elements, and removed elements.
	 * @param {object} newValue The new value object representing the grid state
	 * @param {object} viewModelClass The class for the grid view model
	 * @param {object} vueContext The Vue context in which this value will be used
	 */
	setValue(newValue, viewModelClass, vueContext)
	{
		if (viewModelClass === undefined || vueContext === undefined)
			return

		let elements = [],
			newElements = []

		_forEach(_get(newValue, 'elements', []), (viewModelData) => {
			let viewModel = this.getViewModel(viewModelData, viewModelClass, vueContext)
			if (viewModel !== undefined)
				elements.push(viewModel)
		})

		_forEach(_get(newValue, 'newElements', []), (viewModelData) => {
			let viewModel = this.getViewModel(viewModelData, viewModelClass, vueContext)
			if (viewModel !== undefined)
				newElements.push(viewModel)
		})

		// For cases when more then one processe update value, we need update all at some time and do not use push to the central property.
		// bug case: Initial load of form and restore os the last tab (SelectTab)
		this.elements.splice(0, Infinity, ...elements)
		this.newElements.splice(0, Infinity, ...newElements)
		this.removedElements.splice(0, Infinity, ..._get(newValue, 'removedElements', []))
		this.newRecordTemplate = _get(newValue, 'newRecordTemplate', this.newRecordTemplate)
	}

	/**
	 * Returns an object representing the current state of the grid, with elements, new elements, and removed elements.
	 * @returns {object} An object containing the current state of the grid.
	 */
	getCurrentState()
	{
		return {
			elements: this.elements.filter((row) => !this.removedElements.includes(row.QPrimaryKey)),
			removedElements: this.elements.filter((row) => this.removedElements.includes(row.QPrimaryKey)),
			newElements: this.newElements.filter((row) => row.isDirty)
		}
	}

	/**
	 * Returns an object representing the current state of the grid suitable for server communication.
	 * @param {boolean} removedElementsOnlyKey True to return only the key of removed elements (defaults to false)
	 * @param {boolean} elementsOnlyDirty True to return only the elements that are dirty (have been modified)
	 * @returns {object} An object containing the current state of the grid in a server-compatible format.
	 */
	getCurrentStateSrvObject(removedElementsOnlyKey = false, elementsOnlyDirty = false)
	{
		let currentState = this.getCurrentState()

		return {
			elements: _flatMap(elementsOnlyDirty ? currentState.elements.filter((row) => row.isDirty) : currentState.elements, (row) => row.serverObjModel),
			removedElements: _flatMap(currentState.removedElements, (row) => removedElementsOnlyKey ? row.QPrimaryKey : row.serverObjModel),
			newElements: _flatMap(currentState.newElements, (row) => row.serverObjModel)
		}
	}

	/**
	 * Set server error messages associated with the field.
	 * @param {object} errors The server errors
	 * @param {array} key The model field path
	 */
	setServerErrorMessages(errors, key)
	{
		const rowsListName = key.shift()
		const rowId = key.shift()

		if (rowsListName === 'editedElements' || rowsListName === 'removedElements')
		{
			const modelIndex = _findIndex(this.editedElements, (row) => row.QPrimaryKey === rowId)
			const rowModel = _get(this.editedElements, modelIndex)
			rowModel?.setServerErrorMessages(key, errors)
		}
		else if (rowsListName === 'newElements')
		{
			const rowModel = _get(this.newElements, rowId)
			rowModel?.setServerErrorMessages(key, errors)
		}
	}

	/**
	 * Clears server error messages for all elements and new elements.
	 */
	clearServerErrorMessages()
	{
		_forEach(this.elements, (el) => el.clearServerErrorMessages())
		_forEach(this.newElements, (el) => el.clearServerErrorMessages())
	}

	/**
	 * Set server warning messages associated with the field.
	 * @param {object} errors The server errors
	 * @param {Array} key The model field path
	 */
	setServerWarningMessages(errors, key)
	{
		const rowsListName = key.shift()
		const rowId = key.shift()

		if (rowsListName === 'editedElements' || rowsListName === 'removedElements')
		{
			const modelIndex = _findIndex(this.editedElements, (row) => row.QPrimaryKey === rowId)
			const rowModel = _get(this.editedElements, modelIndex)
			rowModel?.setServerWarningMessages(key, errors)
		}
		else if (rowsListName === 'newElements')
		{
			const rowModel = _get(this.newElements, rowId)
			rowModel?.setServerWarningMessages(key, errors)
		}
	}

	/**
	 * Clears server warning messages for all elements and new elements.
	 */
	clearServerWarningMessages()
	{
		_forEach(this.elements, (el) => el.clearServerWarningMessages())
		_forEach(this.newElements, (el) => el.clearServerWarningMessages())
	}

	/**
	 * Deep clones the field's value.
	 */
	clone()
	{
		const _clone = new GridTableListValue()

		this.elements.forEach((model) => _clone.elements.push(model.clone()))
		this.newElements.forEach((model) => _clone.newElements.push(model.clone()))

		_clone.removedElements = _cloneDeep(this.removedElements)
		_clone.newRecordTemplate = _cloneDeep(this.newRecordTemplate)

		return _clone
	}
}

export class GridTableList extends Base
{
	constructor(options, vueContext)
	{
		super({
			type: 'GridTableList',
			_value: new GridTableListValue(),
			viewModelClass: undefined
		})

		// Just to initialize the View Model of Row's (Resources + NavigationId for requests)
		this.vueContext = vueContext

		_assignIn(this, options)
	}

	/**
	 * @override
	 */
	get isDirty()
	{
		return this.value.isDirty
	}

	/**
	 * @override
	 */
	get serverValue()
	{
		return this.value.serverValue
	}

	/**
	 * The current elements in the grid.
	 */
	get elements()
	{
		return this.value.elements
	}

	/**
	 * The elements that have been added to the grid.
	 */
	get newElements()
	{
		return this.value.newElements
	}

	/**
	 * The elements that have been edited in the grid.
	 */
	get editedElements()
	{
		return this.value.editedElements
	}

	/**
	 * The elements that have been marked for removal.
	 */
	get removedElements()
	{
		return this.value.removedElements
	}

	/**
	 * The rows in the grid that are not dirty.
	 */
	get emptyRows()
	{
		return this.value.emptyRows
	}

	/**
	 * @override
	 */
	updateValue(newValue)
	{
		this.value.setValue(newValue, this.viewModelClass, this.vueContext)
	}

	/**
	 * @override
	 */
	clearServerErrorMessages()
	{
		this.value.clearServerErrorMessages()
	}

	/**
	 * @override
	 */
	clearServerWarningMessages()
	{
		this.value.clearServerWarningMessages()
	}

	/**
	 * @override
	 */
	hasSameValue(otherValue)
	{
		if (!(otherValue instanceof GridTableListValue))
			return false

		return _isEqual(this.value.getCurrentStateSrvObject(), otherValue.getCurrentStateSrvObject())
	}

	/**
	 * @override
	 */
	cloneValue()
	{
		return this.value.clone()
	}

	/**
	 * @override
	 */
	isValidType(value)
	{
		return value instanceof GridTableListValue || value === null
	}

	/**
	 * Set server error messages associated with the model field.
	 * @param {object} errors The server errors
	 * @param {Array} key The model field
	 */
	setServerErrorMessages(errors, key)
	{
		this.value.setServerErrorMessages(errors, key)
	}

	/**
	 * Set server warning messages associated with the model field.
	 * @param {object} warnings The server warnings
	 * @param {Array} key The model field
	 */
	setServerWarningMessages(warnings, key)
	{
		this.value.setServerWarningMessages(warnings, key)
	}

	/**
	 * Update a single field's value for a specific model in the grid.
	 * @param {object} eventData Data describing the event that initiated the update
	 */
	setModelFieldValue(eventData)
	{
		let modelUId = _get(eventData, 'key'),
			fieldData = _get(eventData, 'value'),
			fieldName = _get(fieldData, 'modelField'),
			fieldValue = _get(fieldData, 'value')

		if (_isEmpty(modelUId) || _isEmpty(fieldName) || !_has(this, 'value.elements'))
			return

		let modelIndex = _findIndex(this.value.elements, (row) => row.uniqueIdentifier === modelUId)

		if (modelIndex !== -1)
			this.value.elements[modelIndex][fieldName].updateValue(fieldValue)
		else
		{
			modelIndex = _findIndex(this.value.newElements, (row) => row.uniqueIdentifier === modelUId)
			if (modelIndex !== -1)
				this.value.newElements[modelIndex][fieldName].updateValue(fieldValue)
		}
	}

	/**
	 * Adds a new model to the grid using the new record template.
	 */
	addNewModel()
	{
		let newModelData = _cloneDeep(this.value.newRecordTemplate)

		if (newModelData)
		{
			newModelData[this.viewModelClass.QPrimaryKeyName] = uuidv4()
			this.value.addNewModel(newModelData, this.viewModelClass, this.vueContext)
		}
	}

	/**
	 * Removes empty rows from the grid, optionally leaving one empty row if not full.
	 * @param {boolean} full A flag indicating whether to remove all empty rows or leave one remaining
	 */
	trimEmptyRows(full)
	{
		this.value.trimEmptyRows(full)
	}

	/**
	 * Marks the specified row for deletion in the grid.
	 * @param {object} row The row to be marked for deletion
	 */
	markForDeletion(row)
	{
		this.value.markForDeletion(row)
	}

	/**
	 * Undoes the deletion mark on the specified row, if it was previously marked.
	 * @param {object} row The row to remove from deletion
	 */
	undoDeletion(row)
	{
		this.value.undoDeletion(row)
	}
}

export default {
	Base,
	String,
	MultiLineString,
	Password,
	PrimaryKey,
	ForeignKey,
	Coordinate,
	Geographic,
	Date,
	DateTime,
	DateTimeSeconds,
	Time,
	Boolean,
	Number,
	Image,
	Document,
	MultipleValues,
	GridTableList
}
