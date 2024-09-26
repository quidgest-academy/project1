﻿import { readonly } from 'vue'
import { v4 as uuidv4 } from 'uuid'
import _assignIn from 'lodash-es/assignIn'
import _forEach from 'lodash-es/forEach'
import _isArray from 'lodash-es/isArray'
import _isDate from 'lodash-es/isDate'
import _isEmpty from 'lodash-es/isEmpty'
import _upperFirst from 'lodash-es/upperFirst'

import { dateToISOString } from '@/mixins/genericFunctions.js'

/**
 * The object that represents a new level
 */
export class HistoryLevel
{
	constructor(previousLevel, options)
	{
		_assignIn(this, {
			uniqueIdentifier: uuidv4(),
			previousLevel,
			isNested: false,
			upperLevels: new Map(),
			entries: {},
			params: {},
			location: '',
			properties: {
				routeBranch: '',
				breadcrumbName: ''
			},
			// Stores the values of open forms.
			formValues: {},
			// Stores the state of collapsible groups (is open) and tabs (is selected).
			containersState: {},
			// Stores the currently active control.
			currentControl: {}
		}, options || {})
	}

	setPreviousLevel(previousLevel)
	{
		this.previousLevel = previousLevel
	}

	get level()
	{
		return _isEmpty(this.previousLevel) ? 0 : this.previousLevel.level + 1
	}

	/**
	 * Adds/updates a value in the entries of the current level.
	 * @param {object} param1 The key and value to set
	 */
	setEntryValue({ key, value })
	{
		Reflect.set(this.entries, key, value)
	}

	/**
	 * Get a value of the entry.
	 * @param {string} key The key to get
	 */
	getEntryValue(key)
	{
		if (Reflect.has(this.entries, key))
			return Reflect.get(this.entries, key)
		else if (!_isEmpty(this.previousLevel))
			return this.previousLevel.getEntryValue(key)
		return null
	}

	/**
	 * Removes the value with the specified key from the entries of the current level.
	 * @param {string} key The key to remove
	 */
	removeEntryValue(key)
	{
		Reflect.deleteProperty(this.entries, key)
	}

	/**
	 * Clears the values of all the entries of the current level.
	 */
	clearEntries()
	{
		this.entries = {}
	}

	/**
	 * Adds/updates a value in the params of the current level.
	 * @param {object} param1 The key and value to set
	 */
	setParamValue({ key, value })
	{
		Reflect.set(this.params, key, value)
	}

	/**
	 * Removes the value with the specified key from the params of the current level.
	 * @param {string} key The key to remove
	 */
	removeParamValue(key)
	{
		Reflect.deleteProperty(this.params, key)
	}

	/**
	 * Sets the value of the form mode in the navigation
	 * @param {string} mode The new mode to set
	 */
	setMode(mode)
	{
		Reflect.set(this.params, 'mode', mode)
	}

	/**
	 * Adds/updates a value in the properties of the current level.
	 * @param {object} param0 The key and value to set
	 */
	setProperty({ key, value })
	{
		this.properties[key] = value
	}

	/**
	 * Removes the value with the specified key from the properties of the current level.
	 * @param {string} key The key to remove
	 */
	removeProperty(key)
	{
		delete this.properties[key]
	}

	// TODO: Validate params | the record Id ?
	checkLocation({ location })
	{
		return this.location === location
	}

	/**
	 * Check if there is any history level with specific location.
	 * @param {object} param0 The location that has to be found.
	 * @returns True if contains
	 */
	containsLocation({ location, params })
	{
		if (this.checkLocation({ location, params }))
			return true
		else if (!_isEmpty(this.previousLevel))
			return this.previousLevel.containsLocation({ location, params })
		return false
	}

	/**
	 * Get history level by location.
	 * @param {object} param0 The location that has to be found.
	 * @returns The HistoryLevel or null
	 */
	getLevelByLocation({ location, params })
	{
		if (this.checkLocation({ location, params }))
			return this
		else if (!_isEmpty(this.previousLevel))
			return this.previousLevel.getLevelByLocation({ location, params })
		return null
	}

	/**
	 * Get history level by unique identifier.
	 * @param {string} uniqueIdentifier The unique identifier that has to be found.
	 * @returns The HistoryLevel or null
	 */
	getLevelByUId(uniqueIdentifier)
	{
		if (this.uniqueIdentifier === uniqueIdentifier)
			return this
		else if (!_isEmpty(this.previousLevel))
			return this.previousLevel.getLevelByUId(uniqueIdentifier)
		return null
	}

	/**
	 * Update the property values for the current level.
	 * @param {object} options Object with the new property values.
	 */
	updateData(options)
	{
		_assignIn(this, options || {})
	}

	/**
	 * Destroy the next levels.
	 */
	destroy()
	{
		this.upperLevels.forEach(upperLevel => upperLevel.destroy())
		this.upperLevels.clear()
	}

	/**
	 * Convert the structure in graph to a collection of levels.
	 * @returns An array of history levels.
	 */
	convertToCollection()
	{
		return [...(!_isEmpty(this.previousLevel) ? this.previousLevel.convertToCollection() : []), this]
	}

	/**
	 * The first HistoryLevel that is not nested or the last one.
	 * @returns The HistoryLevel that is not nested or the last one.
	 */
	getFirstNotNested()
	{
		if (this.isNested && !_isEmpty(this.previousLevel))
			return this.previousLevel.getFirstNotNested()
		return this
	}

	/**
	 * Checks if the specified key exists in history.
	 * @param {string} key The key
	 * @param {boolean} includeNulls Whether or not to check in keys with null values
	 * @param {boolean} startAtPrevious Whether or not to also check the previous history level
	 * @returns True if the entry exists, false otherwise.
	 */
	hasEntry(key, includeNulls = true, startAtPrevious = false)
	{
		if (!startAtPrevious && Reflect.has(this.entries, key))
		{
			let entryValue = Reflect.get(this.entries, key)
			return entryValue !== undefined && (entryValue !== null || includeNulls)
		}
		else if (!_isEmpty(this.previousLevel))
			return this.previousLevel.hasEntry(key, includeNulls)
		return false
	}

	/**
	 * Saves the value of the specified field in the store.
	 * @param {object} param0 The field data
	 */
	setStoreValue({ key, formInfo, field })
	{
		if (!validateParams(this.formValues, key, formInfo, false))
			return false

		const area = formInfo.area
		const formName = formInfo.name

		const dataId = {
			area,
			key,
			formName,
			fieldId: field.id
		}
		const dataValue = readonly({
			value: field.cloneValue(),
			oldValue: field.originalValue
		})

		setValues(this.formValues, dataId, dataValue)
		return true
	}

	/**
	 * Saves the state of the specified container in the store.
	 * @param {object} param0 The container and it's current state
	 */
	storeContainerState({ key, formInfo, fieldId, containerState })
	{
		if (!validateParams(this.containersState, key, formInfo, false))
			return false
		if (typeof fieldId !== 'string' || typeof containerState === 'undefined')
			return false

		const area = formInfo.area
		const formName = formInfo.name

		setValues(this.containersState, { area, key, formName, fieldId }, containerState)
		return true
	}

	/**
	 * Saves the data of the currently active control.
	 * @param {object} controlData The data of the current control
	 * @param {boolean} isNested Whether or not the control is for a nested form
	 */
	setCurrentControl(controlData, isNested)
	{
		if (_isEmpty(controlData) || typeof controlData.id !== 'string')
			return false

		if (isNested)
		{
			if (!Array.isArray(this.currentControl.nestedControls))
				this.currentControl.nestedControls = []

			this.currentControl.nestedControls.push(controlData)
		}
		else
		{
			this.currentControl = {
				...this.currentControl,
				...controlData
			}
		}

		return true
	}

	/**
	 * Removes the currently active control with the specified id.
	 * @param {string} controlId The id of the control to remove
	 */
	removeCurrentControl(controlId)
	{
		if (_isEmpty(this.currentControl))
			return false

		// Check if the main control is the one that should be removed.
		if (this.currentControl.id === controlId)
		{
			delete this.currentControl.id
			delete this.currentControl.data
			return true
		}

		// Check if the control to remove is one of the nested ones.
		const nestedControls = this.currentControl.nestedControls
		if (Array.isArray(nestedControls))
		{
			for (let i = 0; i < nestedControls.length; i++)
			{
				if (nestedControls[i].id === controlId)
				{
					nestedControls.splice(i, 1)
					return true
				}
			}
		}

		return false
	}

	/**
	 * Clears the data of the currently active control.
	 */
	clearCurrentControl()
	{
		this.currentControl = {}
	}
}

/*########################################
# Utils
##########################################*/

/**
 * Recursively converts values in an object, including dates to ISO strings
 * and deeply nested structures.
 * @param {any} srcValue The source value to be converted
 * @returns The converted value.
 */
function _entryConvert(srcValue)
{
	// Convert Date objects to ISO string format.
	if (_isDate(srcValue))
		return dateToISOString(srcValue)
	// Recursively process arrays, converting each element.
	if (_isArray(srcValue))
		return srcValue.map((item) => _entryConvert(item))
	// Recursively process objects, converting each value.
	if (typeof srcValue === 'object' && srcValue !== null)
	{
		const convertedObject = {}
		Object.entries(srcValue).forEach(([key, value]) => {
			// Use the conversion function for each value.
			convertedObject[key] = _entryConvert(value)
		})
		return convertedObject
	}

	// Return all other values unchanged.
	return srcValue
}

/**
 * Processes and transforms the history levels, including converting
 * dates within 'entries' to a specific string format.
 * @param {Object} hLevel The current history level to process
 * @returns {Array} An array of processed history levels.
 */
function _transformHistoryLevels(hLevel)
{
	let mode = 'None',
		result = []

	if (!_isEmpty(hLevel))
	{
		if ((hLevel.location || '').startsWith('menu-'))
			mode = 'List'
		else if ((hLevel.location || '').startsWith('form-'))
			mode = _upperFirst((hLevel.params || {}).mode || 'Show')

		result.push({
			Entries: _entryConvert(hLevel.entries, _entryConvert),
			Level: hLevel.level,
			FormMode: mode,
			Location: {
				vueRouteName: hLevel.location,
				mode: (hLevel.params || {}).mode || null,
				RoutedValues: hLevel.params
			},
			uniqueIdentifier: hLevel.uniqueIdentifier
		})

		if (!_isEmpty(hLevel.previousLevel))
			result.push(..._transformHistoryLevels(hLevel.previousLevel))
	}

	return result
}

/**
 * Adds/updates the value of the specified object in the store.
 * @param {object} valObj The stored object
 * @param {object} param1 The object keys
 * @param {any} data The new value
 */
function setValues(valObj, { area, key, formName, fieldId }, data)
{
	if (typeof valObj[area] === 'undefined')
		valObj[area] = {}
	if (typeof valObj[area][key] === 'undefined')
		valObj[area][key] = {}
	if (typeof valObj[area][key][formName] === 'undefined')
		valObj[area][key][formName] = {}

	valObj[area][key][formName][fieldId] = data
}

/**
 * Checks whether or not the specified keys are valid, if "checkPresence" is true also checks if they are present in the object.
 * @param {object} valObj The stored object
 * @param {string} key A key unique to the desired values
 * @param {object} formInfo The information of the form
 * @param {boolean} checkPresence Whether or not to check if the object already has values for the keys
 * @returns True if the object has stored values that match the specified keys, false otherwise.
 */
function validateParams(valObj, key, formInfo, checkPresence)
{
	if (typeof formInfo !== 'object' || formInfo === null)
		return false

	const area = formInfo.area
	const formName = formInfo.name

	if (typeof area !== 'string' || checkPresence && typeof valObj[area] === 'undefined')
		return false
	if (typeof key === 'undefined' || checkPresence && typeof valObj[area][key] === 'undefined')
		return false
	if (typeof formName !== 'string' || checkPresence && typeof valObj[area][key][formName] === 'undefined')
		return false

	return true
}

export class NavigationContext
{
	constructor(uniqueIdentifier)
	{
		this.navigationId = uniqueIdentifier || uuidv4()

		/**
		 * The current history level (object)
		 */
		this.currentLevel = null
	}

	/**
	 * The previous history level
	 */
	get previousLevel()
	{
		if (!_isEmpty(this.currentLevel))
			return this.currentLevel.previousLevel
		return null
	}

	/**
	 * Adds a new history level to the navigation.
	 * @param {object} options Additional options about the level (optional)
	 * @param {object} previousLevel The previous history level
	 */
	addHistoryLevel(options, previousLevel)
	{
		this.cleanUpTo(options)

		// If no history level exists yet, we try to create one for the home page.
		if (_isEmpty(previousLevel) && _isEmpty(this.currentLevel) && (_isEmpty(options.location) || !options.location.startsWith('home')))
		{
			if (!options.params || typeof options.params.module !== 'string')
				return

			const homeOptions = {
				location: `home-${options.params.module}`,
				params: {
					culture: options.params.culture,
					system: options.params.system,
					module: options.params.module
				}
			}

			this.currentLevel = new HistoryLevel(null, homeOptions)
		}

		if (this.checkLocation(options))
			this.updateHistoryLevelData(options)
		else
		{
			const historyLevel = new HistoryLevel(previousLevel || this.currentLevel, options)
			if (!_isEmpty(historyLevel.previousLevel))
				historyLevel.previousLevel.upperLevels.set(historyLevel.uniqueIdentifier, historyLevel)

			this.currentLevel = historyLevel
		}
	}

	/**
	 * Removes the most recent history level from the navigation.
	 */
	removeNavigationLevel()
	{
		if (!_isEmpty(this.currentLevel))
		{
			let curUId = this.currentLevel.uniqueIdentifier
			this.currentLevel.destroy()
			this.currentLevel = this.currentLevel.previousLevel
			if (this.currentLevel?.upperLevels?.has(curUId))
				this.currentLevel?.upperLevels?.delete(curUId)
		}
	}

	/**
	 * Removes several history levels from the navigation.
	 * @param {number} levels The number of levels to remove
	 */
	removeHistoryLevels(levels)
	{
		if (typeof levels !== 'number')
			return

		while (levels-- > 0)
			this.removeNavigationLevel()
	}

	/**
	 * Remove history levels up to the indicated level. But it doesn't let remove the level 0 which corresponds to the «base» level / home page.
	 * @param {number} upToLevel The history level from which every highest level goes to be removed ( >= 0 )
	 */
	removeNavigationLevelsUpTo(upToLevel)
	{
		if (typeof upToLevel !== 'number')
			return
		if (_isEmpty(this.currentLevel))
			return

		while (this.currentLevel.level > 0 && this.currentLevel.level > upToLevel)
			this.removeNavigationLevel()
	}

	checkLocation({ location, params })
	{
		if (!_isEmpty(this.currentLevel))
			return this.currentLevel.checkLocation({ location, params })
		return false
	}

	/**
	 * Check if there is any history level with specific location.
	 * @param {object} param0 The location that has to be found.
	 * @returns True if contains
	 */
	containsLocation({ location, params })
	{
		if (!_isEmpty(this.currentLevel))
			return this.currentLevel.containsLocation({ location, params })
		return false
	}

	/**
	 * Get history level by location.
	 * @param {object} param0 The location that has to be found.
	 * @returns The HistoryLevel or null
	 */
	getLevelByLocation({ location, params })
	{
		if (!_isEmpty(this.currentLevel))
			return this.currentLevel.getLevelByLocation({ location, params })
		return null
	}

	/**
	 * Get history level by unique identifier.
	 * @param {string} uniqueIdentifier The unique identifier that has to be found.
	 * @returns The HistoryLevel or null
	 */
	getLevelByUId(uniqueIdentifier)
	{
		if (!_isEmpty(this.currentLevel))
			return this.currentLevel.getLevelByUId(uniqueIdentifier)
		return null
	}

	/**
	 * Checks if the given location already exists in history,
	 * if so, removes it and all levels after it.
	 * @param {object} param0
	 */
	cleanUpTo({ location, params })
	{
		while (this.containsLocation({ location, params }))
		{
			if (this.checkLocation({ location, params }))
				break
			this.removeNavigationLevel()
		}
	}

	/**
	 * Update the property values for the current level.
	 * @param {object} options Object with the new property values.
	 */
	updateHistoryLevelData(options)
	{
		if (this.containsLocation(options))
		{
			// Default function, to be called in case an error occurs.
			const updateData = () => {
				// eslint-disable-next-line no-console
				console.warn('Problems updating the history level data.', options)
			}
			(this.getLevelByLocation(options) || { updateData }).updateData(options)
		}
	}

	/**
	 * Adds/updates a value in the entries of the current level.
	 * @param {object} param1 The key and value to set
	 */
	setEntryValue({ key, value })
	{
		if (!_isEmpty(this.currentLevel))
			this.currentLevel.setEntryValue({ key, value })
	}

	/**
	 * Get a value of the entry.
	 * @param {string} key The key to get
	 */
	getEntryValue(key)
	{
		if (_isEmpty(this.currentLevel))
			return null
		return this.currentLevel.getEntryValue(key)
	}

	/**
	 * Removes the value with the specified key from the entries of the current level.
	 * @param {string} key The key to remove
	 */
	removeEntryValue(key)
	{
		if (!_isEmpty(this.currentLevel))
			this.currentLevel.removeEntryValue(key)
	}

	/**
	 * Clears the values of all the entries of the current level.
	 */
	clearEntries()
	{
		if (!_isEmpty(this.currentLevel))
			this.currentLevel.clearEntries()
	}

	/**
	 * Adds/updates a value in the params of the current level.
	 * @param {object} param1 The key and value to set
	 */
	setParamValue({ key, value })
	{
		if (!_isEmpty(this.currentLevel))
			this.currentLevel.setParamValue({ key, value })
	}

	/**
	 * Removes the value with the specified key from the params of the current level.
	 * @param {string} key The key to remove
	 */
	removeParamValue(key)
	{
		if (!_isEmpty(this.currentLevel))
			this.currentLevel.removeParamValue(key)
	}

	/**
	 * Adds/updates a value in the properties of the current level.
	 * @param {object} param0 The key and value to set
	 */
	setProperty({ key, value })
	{
		if (!_isEmpty(this.currentLevel))
			this.currentLevel.setProperty({ key, value })
	}

	/**
	 * Removes the value with the specified key from the properties of the current level.
	 * @param {string} key The key to remove
	 */
	removeProperty(key)
	{
		if (!_isEmpty(this.currentLevel))
			this.currentLevel.removeProperty(key)
	}

	/**
	 * Returns a History structure in the format expected by the server
	 */
	historyToSend()
	{
		let history = _transformHistoryLevels(this.currentLevel),
			historyToSend = []

		_forEach(history, h => historyToSend.push(h))

		return {
			History: historyToSend.reverse(),
			NavigationId: this.navigationId
		}
	}

	/**
	 * Convert the structure in graph to a collection of levels.
	 * @returns Array of history levels.
	 */
	convertToCollection()
	{
		if (!_isEmpty(this.currentLevel))
			return this.currentLevel.convertToCollection()
		return []
	}

	applyServerChanges(srvHistory)
	{
		_forEach(srvHistory, hLevel => {
			let level = this.getLevelByUId(hLevel.uId)
			if (level !== null)
			{
				_forEach(hLevel.remove, entryKey => level.removeEntryValue(entryKey))
				_forEach(hLevel.set, (value, key) => level.setEntryValue({ key, value }))
			}
		})
	}

	/**
	 * Saves the values of the specified fields in the store.
	 * @param {object} param0 The fields data
	 */
	storeValues({ key, formInfo, fields })
	{
		for (let i in fields)
			this.storeValue({ key, formInfo, field: fields[i] })
	}

	/**
	 * Saves the value of the specified field in the store.
	 * @param {object} param0 The field data
	 */
	storeValue({ key, formInfo, field, levelNumber })
	{
		var navLevel = this.currentLevel

		if (levelNumber !== '')
		{
			while (navLevel.level.toString() !== levelNumber)
			{
				if (!_isEmpty(navLevel.previousLevel))
					navLevel = navLevel.previousLevel
				else
					return false
			}
		}

		return navLevel.setStoreValue({ key, formInfo, field })
	}

	/**
	 * Saves the state of the specified container in the store.
	 * @param {object} containerData The container and it's current state
	 */
	storeContainerState(containerData)
	{
		return this.currentLevel.storeContainerState(containerData)
	}

	/**
	 * Saves the data of the currently active control.
	 * @param {object} controlData The data of the current control
	 * @param {boolean} isNested Whether or not the control is for a nested form
	 */
	setCurrentControl(controlData, isNested)
	{
		return this.currentLevel.setCurrentControl(controlData, isNested)
	}

	/**
	 * Removes the currently active control with the specified id.
	 * @param {string} controlId The id of the control to remove
	 */
	removeCurrentControl(controlId)
	{
		return this.currentLevel.removeCurrentControl(controlId)
	}

	/**
	 * Clears the data of the currently active control.
	 */
	clearCurrentControl()
	{
		this.currentLevel.clearCurrentControl()
	}
}

export default {
	HistoryLevel,
	NavigationContext
}
