import { isRef } from 'vue'
import { format } from 'date-fns'
import tinycolor from 'tinycolor2'
import cloneDeep from 'lodash-es/cloneDeep'
import _forEach from 'lodash-es/forEach'
import _get from 'lodash-es/get'
import _isArray from 'lodash-es/isArray'
import _isDate from 'lodash-es/isDate'
import _isEmpty from 'lodash-es/isEmpty'
import _set from 'lodash-es/set'
import Swal from 'sweetalert2/dist/sweetalert2.js'

import { useAuthDataStore } from '@/stores/authData.js'
import { useCustomDataStore } from '@/stores/customData.js'
import { useGenericDataStore } from '@/stores/genericData.js'
import { useGenericLayoutDataStore } from '@/stores/genericLayoutData.js'
import { useGlobalTablesDataStore } from '@/stores/globalTablesData.js'
import { useLayoutDataStore } from '@/stores/layoutData.js'
import { useNavDataStore } from '@/stores/navData.js'
import { useSystemDataStore } from '@/stores/systemData.js'
import { useTracingDataStore } from '@/stores/tracingData.js'
import { useUserDataStore } from '@/stores/userData.js'
import { formModes } from '@/mixins/quidgest.mainEnums.js'
import eventBus from '@/api/global/eventBus.js'
import openQSign from '@/api/genio/qSign.js'

/**
 * Determines the readable text color based on the given background color.
 * @param {string} backgroundColor The background color for which to determine the readable text color
 * @returns {string} The readable text color (either 'black' or 'white').
 */
export function getReadableTextColor(backgroundColor)
{
	// Create a tinycolor instance from the given background color
	const color = tinycolor(backgroundColor)
	// Calculate the luminance of the background color
	const luminance = color.getLuminance()
	// If the luminance is greater than 0.5, the background color is light, so use black as the text color
	// If the luminance is less than or equal to 0.5, the background color is dark, so use white as the text color
	const textColor = luminance > 0.5 ? 'black' : 'white'

	return textColor
}

/**
 * Changes the window's onbeforeunload event, so the user will be asked if he wants to leave, in case the form is dirty.
 * @param {boolean} isDirty True if the form is dirty, false otherwise
 */
export function setNavigationState(isDirty)
{
	if (isDirty)
		window.onbeforeunload = () => ''
	else
		window.onbeforeunload = () => { }
}

/**
 * Normalize data for use in navigation parameters.
 * For now, only transforms Date objects to ISO strings within the input object.
 *
 * @param {Object|Array} obj - The input object or array that need be normalized.
 * @returns {Object|Array} A new object or array with normalized data.
 */
export function normalizeDataInNavigationParams(obj)
{
	// Determine whether the input is an array or object, and initialize the output accordingly
	const newObj = Array.isArray(obj) ? [] : {}

	// Iterate through all keys in the input object or array
	for (const key in obj)
	{
		// If the current property is a Date object, convert it to an ISO string
		if (obj[key] instanceof Date)
			newObj[key] = dateToISOString(obj[key])
		// If the current property is a non-null object or array, call the function recursively
		else if (typeof obj[key] === 'object' && obj[key] !== null)
			newObj[key] = normalizeDataInNavigationParams(obj[key])
		// Otherwise, just copy the property to the output object or array
		else
			newObj[key] = obj[key]
	}

	// Return the output object or array
	return newObj
}

/**
 * Should be called whenever there's a route change, to update the navigation history.
 * @param {object} routeData The route where the user is navigating to
 */
export function normalizeRouteForSaveNavigation(routeData)
{
	const routeBranch = routeData.meta.order
	const args = {
		location: routeData.name,
		params: routeData.params,
		properties: {
			routeBranch: routeBranch || ''
		}
	}

	return args
}

/**
 * Should be called whenever there's a route change, to update the navigation history.
 * @param {object} routeData The route where the user is navigating to
 * @param {function} updateHistory A function that will update the navigation history
 */
export function saveNavigation(routeData, updateHistory)
{
	if (typeof updateHistory !== 'function')
		return

	const args = normalizeRouteForSaveNavigation(routeData)

	updateHistory({ options: args })
}

/**
 * Sets the configuration of the entire application.
 * @param {object} data The app data
 */
export function setAppConfig(data)
{
	if (typeof data !== 'object' || data === null)
		return

	const systemDataStore = useSystemDataStore()
	const genericDataStore = useGenericDataStore()
	const authDataStore = useAuthDataStore()
	const userDataStore = useUserDataStore()
	const tracingDataStore = useTracingDataStore()

	if (data.availableModules)
		systemDataStore.setAvailableModules(data.availableModules)
	if (data.defaultModule)
		systemDataStore.setDefaultModule(data.defaultModule)
	if (data.currentModule)
		systemDataStore.setCurrentModule(data.currentModule)
	if (data.years)
		systemDataStore.setAvailableSystems(data.years)
	if (data.defaultSystem)
		systemDataStore.setDefaultSystem(data.defaultSystem)

	// Internally the setCurrentSystem is protected from the assignment of an empty value or one that does not exist in the available systems.
	systemDataStore.setCurrentSystem(data.currentSystem || data.defaultSystem)

	if (data.defaultListRows)
		systemDataStore.setDefaultListRows(data.defaultListRows)
	if (data.numberFormat)
		systemDataStore.setNumberFormat(data.numberFormat)
	if (data.dateFormat)
		systemDataStore.setDateFormat(data.dateFormat)
	if (data.userName)
		userDataStore.setUserData({ Name: data.userName })
	if (data.homePages)
		genericDataStore.setHomePages(data.homePages)
	if (data.schedulerLicense)
		systemDataStore.setSchedulerLicenseKey(data.schedulerLicense)
	if (typeof data.eventTracking === 'boolean')
		tracingDataStore.activateEventTracker(data.eventTracking)

	authDataStore.setUsernameAuth(data.hasUsernameAuth)
	authDataStore.setPasswordRecovery(data.hasPasswordRecovery)
}

/**
 * Builds the human key of the current record.
 * @param {object} humanKeyFields An array with the keys of the fields in the model needed for the human key
 * @param {object} model The form/menu model
 * @returns A string with the human key.
 */
export function buildHumanKey(humanKeyFields, model)
{
	if (!Array.isArray(humanKeyFields) || _isEmpty(humanKeyFields) || typeof model !== 'object' || _isEmpty(model))
		return ''

	var humanKey = ''

	for (let fieldId of humanKeyFields)
	{
		let field = model[fieldId]
		if (_isEmpty(field))
			break

		let value = field.displayValue

		if (_isEmpty(value))
			continue

		if (humanKey.length > 0)
			humanKey += '; '

		humanKey += `${field.description}: ${value}`
	}

	return humanKey
}

/**
 * Converts the specified raw layout variables data to the format expected by the application.
 * @param {object} layoutConfig The raw layout variables data
 * @returns An object with variables in the expected format.
 */
export function getLayoutVariables(layoutConfig)
{
	const layoutVariables = {}

	for (let i in layoutConfig)
	{
		const chosenValue = layoutConfig[i].chosen
		const defaultValue = layoutConfig[i].default
		const possibleValues = layoutConfig[i].values

		if (typeof chosenValue !== 'undefined')
		{
			if (Array.isArray(possibleValues) && possibleValues.includes(chosenValue))
				layoutVariables[i] = chosenValue
			else
				layoutVariables[i] = defaultValue
		}
		else
			layoutVariables[i] = defaultValue
	}

	return layoutVariables
}

/**
 * Displays a popup window with information, it can also receive some user input.
 * Documentation: https://sweetalert2.github.io/#configuration
 *
 * Usage examples:
 *
 *   buttons: {
 *     confirm: {
 *       label: 'Confirm',
 *       action: () => {} // Callback function to be called when the user clicks on the "confirm" button.
 *     },
 *     cancel: {
 *       label: 'Cancel',
 *       action: () => {} // Callback function to be called when the user clicks on the "cancel" button.
 *     }
 *   }
 *
 *   options: {
 *     input: { // More info: https://sweetalert2.github.io/#input-types
 *       type: 'text', // The type can be: text, email, url, password, textarea, select, radio, checkbox, file, range.
 *       label: 'Input label',
 *       placeholder: 'Input placeholder',
 *       validator: () => {}, // Function to validate the user input.
 *       choices: { // Used only for the types: select, radio.
 *         '#ff0000': 'Red',
 *         '#00ff00': 'Green',
 *         '#0000ff': 'Blue'
 *       },
 *       attrs: { // Used only for the types: password, textarea, file, range.
 *         'aria-label': 'Type your message here'
 *       }
 *     },
 *     timeout: 3000,
 *     hideCloseBtn: false,
 *     hideFooterBtns: false,
 *     useHtml: true
 *   }
 *
 * @param {string} message The message to display
 * @param {string} icon The icon of the message (e.g.: info, error, warning, success, question) (optional)
 * @param {string} title The title of the message (optional)
 * @param {object} buttons The available buttons (optional)
 * @param {object} options Additional supported options (optional)
 */
export function displayMessage(message, icon, title, buttons, options)
{
	return new Promise((resolve) => {
		const prefs = {
			titleText: title,
			text: message,
			icon: icon ?? 'info',
			allowOutsideClick: false,
			allowEscapeKey: false,
			showCloseButton: true,
			buttonsStyling: false,
			customClass: {
				actions: 'swal2-my-actions',
				cancelButton: 'q-btn q-btn--secondary',
				confirmButton: 'q-btn q-btn--primary',
				denyButton: 'q-btn q-btn--danger'
			}
		}

		if (buttons)
		{
			if (buttons.confirm)
			{
				prefs.showConfirmButton = true
				prefs.confirmButtonText = buttons.confirm.label
			}

			if (buttons.cancel)
			{
				prefs.showCancelButton = true
				prefs.cancelButtonText = buttons.cancel.label
			}

			if (buttons.deny)
			{
				prefs.showDenyButton = true
				prefs.denyButtonText = buttons.deny.label
			}
		}

		if (options)
		{
			if (options.input)
			{
				prefs.input = options.input.type || 'text'
				prefs.inputLabel = options.input.label
				prefs.inputPlaceholder = options.input.placeholder
				prefs.inputValidator = options.input.validator
				prefs.inputOptions = options.input.choices
				prefs.inputAttributes = options.input.attrs
			}

			if (options.timeout)
				prefs.timer = options.timeout

			if (options.hideCloseBtn)
				prefs.showCloseButton = false

			if (options.hideFooterBtns)
			{
				prefs.showConfirmButton = false
				prefs.showCancelButton = false
				prefs.showDenyButton = false
			}

			if (options.useHtml)
				prefs.html = message

			if (options.imageUrl)
			{
				prefs.imageUrl = options.imageUrl
				prefs.imageAlt = options.imageAlt || 'Custom image'
			}
		}

		Swal.fire(prefs).then((result) => {
			/*
			 * This check must be done this way because callbackParams is not always an object
			 * and it shouldn't necessarily be. It can be a single value. Using isEmpty() can prevent this from working.
			 */
			if (options?.callbackParams !== undefined && options?.callbackParams !== null)
				result.value = options.callbackParams

			if (buttons)
			{
				if (result.isConfirmed && buttons.confirm && typeof buttons.confirm.action === 'function')
				{
					buttons.confirm.action(result.value)
					resolve(true)
				}

				if (result.isDismissed && buttons.cancel && typeof buttons.cancel.action === 'function')
				{
					buttons.cancel.action(result.value)
					resolve(false)
				}

				if (result.isDenied && buttons.deny && typeof buttons.deny.action === 'function')
				{
					buttons.deny.action(result.value)
					resolve(false)
				}
			}
		})
	})
}

/**
 * Sets whether the cookies are visible.
 * @param {boolean} val The value of the cookies visibility
 */
export function setShowCookies(val)
{
	const systemDataStore = useSystemDataStore()
	systemDataStore.setShowCookies(val)
}

/**
 * Scrolls to the top of the page.
 */
export function scrollToTop()
{
	document.body.scrollTop = document.documentElement.scrollTop = 0
}

/**
 * Scrolls to the bottom of the page.
 */
export function scrollToBottom()
{
	window.scrollTo(0, document.body.scrollHeight)
}

/**
 * Scrolls to the specified element.
 * @param {string} id The id of the element to scroll to
 * @param {string} position The scroll position - center or start
 */
export function scrollTo(id, position = 'center')
{
	const options = {
		behavior: 'smooth',
		block: position
	}

	const elem = document.getElementById(id)
	if (!elem)
		return

	const initialSPT = document.documentElement.style.scrollPaddingTop
	const vHeaderHeight = parseInt(document.documentElement.style.getPropertyValue('--visible-header-height'))
	const sticky = document.querySelector('#form-container > .c-sticky-header')
	const menu = document.querySelector('#main-header-navbar .n-menu__navbar--double-l2')

	if (sticky)
	{
		const extraHeight = menu ? menu.clientHeight : 5
		const totalHeight = vHeaderHeight ? vHeaderHeight + sticky.clientHeight - extraHeight : sticky.clientHeight + 5
		document.documentElement.style.scrollPaddingTop = `${totalHeight}px`
	}

	elem.scrollIntoView(options)
	elem.focus()

	document.documentElement.style.scrollPaddingTop = initialSPT
}

/**
 * Focus on a DOM element.
 * @param {object || string} element A reference to the DOM element or ID of the DOM element
 */
export function focusElement(element)
{
	// If element is a string, get DOM element with that ID
	if (typeof element === 'string' && element !== '')
		element = document.getElementById(element)

	if (element === undefined || element === null)
		return

	// If the element can be focused, focus on it
	if (typeof element?.focus === 'function')
		element.focus()
}

/**
 * Removes the specified modal/popup, or the last one if no id is passed.
 * @param {string} modalId The id of the modal
 */
export function removeModal(modalId)
{
	// Remove modal popup
	const genericDataStore = useGenericDataStore()
	const removedModal = genericDataStore.removeModal(modalId)

	// Focus on the element / control that opened the popup
	focusElement(removedModal?.returnElement)
}

/**
 * A function similar, yet simpler, to "string.Format()" in C#:
 * https://docs.microsoft.com/en-us/dotnet/api/system.string.format
 * Can receive any number of parameters, the first of which must be the string to format.
 * @returns A formatted string.
 */
export function formatString()
{
	if (arguments.length < 1)
		return ''

	let string = arguments[0]

	if (typeof string !== 'string' || string.length === 0)
		return ''
	if (arguments.length < 2)
		return string

	for (let i = 0; i < arguments.length - 1; i++)
	{
		const reg = new RegExp('\\{' + i + '\\}', 'gm')
		string = string.replace(reg, arguments[i + 1] ?? '')
	}

	return string
}

/**
 * Converts the specified date to a string.
 * @param {object} date The date that should be converted
 * @param {string} language The language to which the date should be converted
 * @param {string} defaultLang A fallback language, in case the first one is invalid (optional)
 * @returns The date in a string format.
 */
export function dateToString(date, currentLang, defaultLang)
{
	if (!(date instanceof Date))
		return ''

	const langs = [
		currentLang,
		defaultLang || 'en-GB'
	]

	const options = {
		dateStyle: 'short',
		timeStyle: 'medium'
	}

	return new Intl.DateTimeFormat(langs, options).format(date)
}

/**
 * Alternative method of toISOString.
 * Returns a string in simplified extended ISO-8601 format but without specified time zone.
 * YYYY-MM-DDTHH:mm:ss.sss
 * @param {string} date The date time
 * @returns The date in an ISO string format.
 */
export function dateToISOString(date)
{
	if (!(date instanceof Date))
		return ''

	const pad = (number) => `${number < 10 ? '0' : ''}${number}`

	return date.getFullYear()
		+ '-' + pad(date.getMonth() + 1)
		+ '-' + pad(date.getDate())
		+ 'T' + pad(date.getHours())
		+ ':' + pad(date.getMinutes())
		+ ':' + pad(date.getSeconds())
		+ '.' + String(date.getMilliseconds()).padStart(3, '0')
}

/**
 * Converts a simplified extended ISO-8601 date-time string without a time zone
 * into a Date object. The input format should match YYYY-MM-DDTHH:mm:ss.sss.
 * @param {string} isoString The date-time string in ISO-8601 format.
 * @returns {Date} A Date object representing the given date and time.
 */
export function isoStringToDate(isoString)
{
	if (typeof isoString !== 'string')
		throw new Error('Input must be a string.')

	const isoDateTimeFormat = /^(\d{4})-(\d{2})-(\d{2})[T](\d{2}):(\d{2}):(\d{2})(\.\d+)?([+-]\d{2}:?\d{2}|Z)?$/
	const parts = isoString.match(isoDateTimeFormat)

	if (!parts)
		throw new Error('Invalid ISO-8601 date-time format.')

	// If the string ends with 'Z', treat it as UTC.
	if (parts[8] === 'Z')
		return new Date(isoString);
	else
	{
		// Extract components from the match.
		const year = parseInt(parts[1], 10),
			month = parseInt(parts[2], 10) - 1, // Adjust for 0-based index
			day = parseInt(parts[3], 10),
			hours = parseInt(parts[4], 10),
			minutes = parseInt(parts[5], 10),
			seconds = parseInt(parts[6], 10),
			// Parse milliseconds, if present; otherwise default to 0.
			milliseconds = parts[7] ? parseFloat(parts[7]) * 1000 : 0

		return new Date(year, month, day, hours, minutes, seconds, milliseconds)
	}
}

/**
 * Converts the specified date time object to a string.
 * @param {object} time The date time object to be converted
 * @returns The time in a string format like: HH:mm.
 */
export function timeToString(time)
{
	const hours = time?.hours?.toString().padStart(2, '0') || '00'
	const minutes = time?.minutes?.toString().padStart(2, '0') || '00'

	return `${hours}:${minutes}`
}

/**
 * Checks if the provided object has the keys 'hours', 'minutes', and 'seconds'.
 * @param {Object} obj - The object to check for time properties.
 * @returns {boolean} Returns true if the object has all three keys, otherwise false.
 */
export function hasTimeProperties(obj)
{
	if (typeof obj !== 'object' || _isEmpty(obj))
		return false

	return 'hours' in obj && 'minutes' in obj && 'seconds' in obj
}

/**
 * Checks if the specified value is a date.
 * @param {any} value The value
 * @returns True if it's a date, false otherwise.
 */
export function isDate(value)
{
	return _isDate(value)
}

/**
 * Checks if the specified value is an image.
 * @param {object|string} value - The image representation, either as an object or string (url)
 * @returns True if it's a valid image, false otherwise.
 */
export function validateImageFormat(value)
{
	return typeof value === 'object'
		? value === null || 'data' in value && 'dataFormat' in value && 'encoding' in value
		// A string value means it can be either a path for the image or a base64 representation of one.
		: typeof value === 'string' && value.includes('/')
}

/**
 * Converts a given image's source to string format.
 * @param {object|string} data The raw image data or URL
 * @returns {string} The image's data in string format.
 */
export function imageObjToSrc(data)
{
	// URL
	if (typeof data === 'string')
		return data
	else if (typeof data !== 'object' || data === null)
		return null
	// Base64 / inline
	return `data:image/${data.dataFormat};${data.encoding},${data.data}`
}

/**
 * Computes a placeholder color according to the colors of the specified image source.
 * @param {object} src The image source
 */
export async function computeColorPlaceholder(src)
{
	if (_isEmpty(src))
		return null

	return new Promise((resolve, reject) => {
		var img = new Image()

		img.onload = () => {
			const canvas = document.createElement('canvas')

			if (canvas)
			{
				canvas.width = img.width
				canvas.height = img.height

				const context = canvas.getContext('2d', { willReadFrequently: true })
				context.drawImage(img, 0, 0, img.width, img.height)

				const x1 = parseInt(img.width * 0.2)
				const x2 = parseInt(img.width * 0.8)
				const y1 = parseInt(img.height * 0.2)
				const y2 = parseInt(img.height * 0.8)

				const sample1 = context.getImageData(x1, y1, 1, 1).data
				const sample2 = context.getImageData(x1, y2, 1, 1).data
				const sample3 = context.getImageData(x2, y1, 1, 1).data
				const sample4 = context.getImageData(x2, y2, 1, 1).data

				const avgR = (sample1[0] + sample2[0] + sample3[0] + sample4[0]) / 4
				const avgG = (sample1[1] + sample2[1] + sample3[1] + sample4[1]) / 4
				const avgB = (sample1[2] + sample2[2] + sample3[2] + sample4[2]) / 4

				img = null

				const rgb = `rgb(${parseInt(avgR)} ${parseInt(avgG)} ${parseInt(avgB)})`

				resolve(rgb)
			}
		}

		img.onerror = (error) => {
			reject(error)
		}

		// This will trigger the `onload`
		img.src = src
	})
}

/* BEGIN: Dropdowns */

/**
 * Calculate position of dropdown element based on toggle element and windows size and scroll
 * @param dropdownElem {DOM Object}
 * @param toggleElem {DOM Object}
 * @param hAlign {string} "left" or "right"
 * @returns
 */
export function getDropdownPosition(dropdownElem, toggleElem, hAlign)
{
	// Horizontal alignment
	const hAlignDropdown = hAlign?.toLowerCase() === 'right' ? 'right' : 'left'

	// Change position of dropdown so it aligns to the right of the toggle button
	// Must be done after showing the dropdown so it will have the dimensions used to calculate it's position

	// Get coordinates of toggle button
	const btnCoords = getCoords(toggleElem)

	// Get window dimensions
	const wWidth = window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth
	const wHeight = window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight
	const wScrollBarWidth = window.innerWidth - document.documentElement.clientWidth
	const wScrollBarHeight = window.innerHeight - document.documentElement.clientHeight

	const props = { isVisible: true }

	// Toggle button is near the left edge of the screen
	if (btnCoords.left < dropdownElem.offsetWidth)
	{
		// Align left
		props.left = btnCoords.left + 'px'
	}
	// Toggle button is near the right edge of the screen
	else if (wWidth - wScrollBarWidth - btnCoords.right < dropdownElem.offsetWidth)
	{
		// Align right
		props.right = parseInt(wWidth - btnCoords.right - wScrollBarWidth) + 'px'
	}
	// Defined default alignment
	else
	{
		if (hAlignDropdown === 'right')
		{
			// Align right
			props.right = parseInt(wWidth - btnCoords.right - wScrollBarWidth) + 'px'
		}
		else
		{
			// Align left
			props.left = btnCoords.left + 'px'
		}
	}

	// Toggle button is near the bottom edge of the screen
	if (wHeight + window.scrollY - btnCoords.bottom <= dropdownElem.offsetHeight)
	{
		// Show above toggle button
		props.bottom = parseInt(wHeight - btnCoords.top - wScrollBarHeight) + 'px'
	}
	else
	{
		// Show below toggle button
		props.top = btnCoords.bottom + 'px'
	}

	return props
}

/**
 * Get off-screen position of dropdown element based on toggle element and windows size and scroll
 * @returns
 */
export function getDropdownPositionOffScreen()
{
	return {
		isVisible: true,
		left: '-10000px',
		top: `${window.scrollY}px`
	}
}

/**
 * Determine if dropdown is focused (even indirectly)
 * @param dropdownElem {DOM Object}
 * @param toggleElem {DOM Object}
 * @param event {Event Object}
 */
export function dropdownIsFocused(dropdownElem, toggleElem, event)
{
	// Element that contains everything in the application
	const appElem = document.getElementById('app')

	/* Focus went to an element in the dropdown element */
	return dropdownElem.contains(event.relatedTarget)
		/* Focus went to the element that toggles showing the dropdown */
		|| event.relatedTarget === toggleElem
		/* Focus went to an element outside the "app" element (an element that is used like a popup) */
		|| (!appElem.contains(event.relatedTarget) && event.relatedTarget !== null)
		/* Focus came from the error popover */
		|| (event.target.classList.contains('btn-popover') && event.explicitOriginalTarget?.nodeName === '#text')
}

/**
 * Get coordinates of element
 * @param elem {DOM Object}
 */
export function getCoords(elem)
{
	var box = elem.getBoundingClientRect()

	var body = document.body
	var docEl = document.documentElement

	var scrollTop = window.scrollY || docEl.scrollTop || body.scrollTop
	var scrollLeft = window.scrollX || docEl.scrollLeft || body.scrollLeft

	var clientTop = docEl.clientTop || body.clientTop || 0
	var clientLeft = docEl.clientLeft || body.clientLeft || 0

	var top = box.top + scrollTop - clientTop
	var bottom = box.bottom + scrollTop - clientTop
	var left = box.left + scrollLeft - clientLeft
	var right = box.right + scrollLeft - clientLeft

	return {
		top: Math.round(top),
		bottom: Math.round(bottom),
		left: Math.round(left),
		right: Math.round(right)
	}
}

/* END: Dropdowns */

/* BEGIN: Formatting field data to display */

/**
 * Get formatted string representing text
 * @param value {string}
 * @param options {object} [optional]
 * @returns String
 */
export function textDisplay(value, options)
{
	value = _isEmpty(value) ? '' : value

	// Optional options
	if (Number.isInteger(options?.scrollData) && value.length > options.scrollData && !options?.isHtml)
		value = value.substring(0, options.scrollData) + ' (...)'

	return value
}

/**
 * Get formatted string representing a number
 * @param value {string}
 * @param decimalSep {string}
 * @param groupSep {string}
 * @param numberFormatOptions {object}
 * @returns String
 */
export function numericDisplay(value, decimalSep, groupSep, numberFormatOptions)
{
	var strValue = ''

	if (numberFormatOptions !== undefined)
		strValue = new Intl.NumberFormat('en-US', numberFormatOptions).format(value)
	else
		strValue = new Intl.NumberFormat('en-US').format(value)

	strValue = strValue.replace(/,/g, ';').replace('.', ':')
	strValue = strValue.replace(':', decimalSep ?? '.').replace(/;/g, groupSep ?? ',')

	return strValue
}

/**
 * Get formatted string representing a currency
 * @param value {string}
 * @param decimalSep {string}
 * @param groupSep {string}
 * @param decimalPlaces {Number}
 * @param currencyCode {string}
 * @param lcidCode {string}
 * @param symbolType {string}
 * @returns String
 */
export function currencyDisplay(value, decimalSep, groupSep, decimalPlaces, currencyCode, lcidCode, symbolType)
{
	// Optional symbol type: "symbol", "narrowSymbol", "code", "name"
	var symbolTypeUse = 'symbol'
	if (symbolType !== undefined)
		symbolTypeUse = symbolType

	// Get number formatted according to location code without currency symbol
	var strNumber = new Intl.NumberFormat(lcidCode, { minimumFractionDigits: decimalPlaces, maximumFractionDigits: decimalPlaces }).format(value)
	// Get number formatted according to location code with currency symbol
	var strCurrency = new Intl.NumberFormat(lcidCode, { minimumFractionDigits: decimalPlaces, maximumFractionDigits: decimalPlaces, style: 'currency', currency: currencyCode.toUpperCase(), currencyDisplay: symbolTypeUse }).format(value)
	// Get number formatted according to application configuration
	var strNumberDisplay = numericDisplay(value, decimalSep, groupSep, { minimumFractionDigits: decimalPlaces, maximumFractionDigits: decimalPlaces })
	// In number formatted according to location code with currency symbol, replace number with number formatted according to application configuration
	var strValue = strCurrency.replace(strNumber, strNumberDisplay)

	return strValue
}

/**
 * Get formatted string representing a date/time
 * @param dateTimeStr {string}
 * @param dateTimeFormat {string}
 * @returns String
 */
export function dateDisplay(dateTimeStr, dateTimeFormat)
{
	// NULL dates
	if (isEmpty(dateTimeStr))
		return ''

	const date = new Date(dateTimeStr)
	return format(date, dateTimeFormat ?? 'dd/MM/yyyy HH:mm:ss')
}

/**
 * Get formatted string representing a boolean
 * @param value {string}
 * @returns Boolean
 */
export function booleanDisplay(value)
{
	if (typeof value === 'boolean')
		return value
	else if (typeof value === 'number')
		return value !== 0
	return false
}

/**
 * Get formatted string representing an image.
 * @param {object|string} value - The image representation, either as an object or string (url)
 * @returns {string|object} - Formatted string or object representing the image.
 */
export function imageDisplay(value)
{
	if (validateImageFormat(value))
		return value
	return ''
}

/**
 * Get formatted string representing a document
 * @param value {object}
 * @param options {object} [optional]
 * @returns String || Object
 */
export function documentDisplay(value, options)
{
	let rtnValue = cloneDeep(value)

	if (_isEmpty(rtnValue))
		return null

	// Optional options
	if (!_isEmpty(options))
	{
		// Column scroll
		if (options.scrollData !== undefined && rtnValue.fileName.length > options.scrollData)
			rtnValue.fileName = rtnValue.fileName.substring(0, options.scrollData) + ' (...)'

		// Output object instead of string
		if (options.outputObject === true)
			return rtnValue
	}

	return _get(rtnValue, 'fileName', null)
}

/**
 * Get formatted string representing a radio button
 * @param value {object}
 * @returns String || Object
 */
export function radioDisplay(value)
{
	return cloneDeep(value)
}

/**
 * Get value from an enumeration
 * @param enumeration {object}
 * @param value {string}
 * @returns String
 */
export function getValueFromEnumeration(enumeration, value)
{
	return _get(enumeration, value, '')
}

/**
 * Get formatted string representing a value from an enumeration
 * @param enumeration {object}
 * @param value {string}
 * @returns String
 */
export function enumerationDisplay(enumeration, value)
{
	return getValueFromEnumeration(enumeration, value)
}

/* END: Formatting field data to display */

/**
 * Checks if value is empty. A value is considered empty unless it's an arguments object, array, string, or
 * jQuery-like collection with a length greater than 0 or an object with own enumerable properties.
 * @param value The value to inspect.
 * @return Returns true if value is empty, else false.
 */
export function isEmpty(value)
{
	return isDate(value) ? false : _isEmpty(value)
}

/**
 * Merge arrays, accounting for whether they are reactive or not.
 * @param {object/array} objValue Destination array
 * @param {object/array} srcValue Source array
 * @returns An array with all the elements.
 */
export function mergeOptions(objValue, srcValue)
{
	if (isRef(objValue))
	{
		if (_isArray(objValue.value))
		{
			objValue.value.push(...(srcValue || []))
			return objValue
		}
	}
	else if (_isArray(objValue) && _isArray(srcValue))
	{
		objValue.push(...(srcValue || []))
		return objValue
	}
}

/**
 * Validates whether `requiredTexts` is a subset of `texts`.
 * @param {object} requiredTexts The required texts
 * @param {object} texts The provided texts
 * @returns True if `requiredTexts` is a subset of `texts`, false otherwise.
 */
export function validateTexts(requiredTexts, texts)
{
	const textKeys = [...Object.keys(requiredTexts)]
	return textKeys.every((element) => Object.keys(texts).includes(element))
}

/**
 * Checks if the specified file has a valid extension and size, according to the provided parameters.
 * @param {object} file The file to check
 * @param {array} extensions The allowed extensions (an empty list means all extensions are valid)
 * @param {number} maxSize The max allowed size, in bytes (0 means there's no max size)
 * @returns 0 if the specified file is valid, 1 if it's extension is invalid and 2 if it's size is invalid.
 */
export function validateFileExtAndSize(file, extensions = [], maxSize = 0)
{
	if (typeof file !== 'object')
		return -1

	const fileExtension = '.' + file?.name.split('.')?.pop()?.toLowerCase()
	const exts = extensions.map((ext) => ext.toLowerCase())

	if (extensions.length > 0 && !exts.includes(fileExtension))
		return 1

	if (maxSize > 0 && file?.size > maxSize)
		return 2

	return 0
}

/**
 * Displays a message to the user, according to the provided error code when uploading a file.
 * @param {number} error The error code
 * @param {object} errorTexts An object with the error messages
 * @param {object} extraInfo Extra info to display in the error message
 */
export function handleFileError(error, errorTexts = {}, extraInfo = {})
{
	let errorMsg

	switch (error)
	{
		// Invalid extension.
		case 1:
			if (typeof errorTexts.extensionError === 'string' &&
				Array.isArray(extraInfo.extensions) &&
				extraInfo.extensions.length > 0)
				errorMsg = `${errorTexts.extensionError} ${extraInfo.extensions.join(', ')}`
			break
		// Invalid size.
		case 2:
			if (typeof errorTexts.fileSizeError === 'string' &&
				typeof extraInfo.maxSize === 'string')
				errorMsg = formatString(errorTexts.fileSizeError, extraInfo.maxSize)
			break
	}

	if (errorMsg)
		displayMessage(errorMsg, 'error')
}

/**
 * Goes back to the previous navigation level, if it exists.
 * @param {string} navigationId The id of the current navigation
 * @param {boolean} hasInitialPHE Whether or not the function is being called from a place with an initial PHE
 */
export function goBack(navigationId = 'main', hasInitialPHE = false)
{
	const systemDataStore = useSystemDataStore()
	const navDataStore = useNavDataStore()
	const navigation = navDataStore.navigation.getHistory(navigationId)
	var currentLevelWasEmpty = false

	if (navigation.currentLevel === null)
	{
		currentLevelWasEmpty = true

		if (navDataStore.previousNav !== null && navDataStore.previousNav.currentLevel !== null)
			navDataStore.retrievePreviousNav()
		else
		{
			// In case there's nothing to go back to, it should go to main.
			eventBus.emit('go-to-route', { name: 'main' })
			return
		}
	}

	// If the last route was skipped because of a "skip if just one" condition,
	// then we need to go back to the route before that last one.
	while (navigation.currentLevel?.params?.skipLastMenu === 'true')
		navigation.removeNavigationLevel()

	if (navigation.previousLevel === null || hasInitialPHE)
	{
		if (navDataStore.previousNav !== null && navDataStore.previousNav.currentLevel !== null)
		{
			// Replace the current navigation with the previous one.
			navDataStore.retrievePreviousNav()

			const params = {
				...navigation.currentLevel.params,
				culture: systemDataStore.system.currentLang, // We don't want to change the language when navigating back.
				keepNavigation: true
			}

			eventBus.emit('go-to-route', { name: navigation.currentLevel.location, params })
		}
		else
		{
			const module = systemDataStore.system.defaultModule
			const params = {
				culture: systemDataStore.system.currentLang,
				system: systemDataStore.system.currentSystem,
				module
			}

			eventBus.emit('go-to-route', { name: `home-${module}`, params })
		}
	}
	else
	{
		// If it's not an empty level, we can remove the current level
		if (!currentLevelWasEmpty)
			navigation.removeNavigationLevel()

		const level = navigation.currentLevel
		const params = {
			...level.params,
			culture: systemDataStore.system.currentLang // We don't want to change the language when navigating back.
		}

		eventBus.emit('go-to-route', { name: level.location, params })
	}
}

/**
 * Checks if the user has permission to execute the specified action.
 * @param {object} permissions The button permissions
 * @param {string} actionType The action type
 * @returns True if the user has permission, false otherwise.
 */
export function btnHasPermission(permissions, actionType)
{
	if (!permissions || typeof permissions !== 'object' || typeof actionType !== 'string')
		return false

	switch (actionType.toUpperCase())
	{
		case formModes.show:
			return permissions.viewBtnDisabled !== true
		case formModes.edit:
			return permissions.editBtnDisabled !== true
		case formModes.duplicate:
			return permissions.insertBtnDisabled !== true && permissions.viewBtnDisabled !== true
		case formModes.delete:
			return permissions.deleteBtnDisabled !== true
		case formModes.new:
		case 'INSERT': /* There should never be an INSERT option, but the ID of this button is already scattered around the templates. */
			return permissions.insertBtnDisabled !== true
	}

	return true
}

/**
 * Creates a JavaScript Model structure.
 * @param {Object} row - Data in the format { 'area.field': value }
 * @param {Object} objectStructure - Object structure in the format: 'Area.ValField': () => (rowFields) => rowFields['area.field']
 * @returns {Object} - The created Model structure object.
 */
export function getModelStructureObj(row, objectStructure)
{
	let obj = {}
	_forEach(objectStructure, (fnValueSelector, modelPath) => _set(obj, modelPath, fnValueSelector(row)))
	return obj
}

/**
 * Sets specific progress bar properties.
 *
 * Configuration structure:
 *   {number} props.progress - The percentage of progress.
 *   {string} props.text - The displayed text.
 *   {boolean} props.striped - Whether the progress bar should be striped.
 *   {boolean} props.animated - Whether the progress bar should be animated.
 *   {boolean} props.mini - Whether the progress bar should be minimal.
 *   {string} modalProps.title - The displayed title.
 *   {array} modalProps.buttons - A list of buttons to be made available.
 *
 * @param {object} modalProps Configuration of the progress bar container
 * @param {object} props Progress bar configuration
 * @param {object} handlers Progress bar event handlers
 */
export function setProgressBar(modalProps, props, handlers)
{
	const layoutDataStore = useGenericLayoutDataStore()
	layoutDataStore.setProgressBar(modalProps, props, handlers)
}

/**
 * Resets the progress bar to default values.
 * Mostly it will be used to close the progress bar.
 */
export function resetProgressBar()
{
	const layoutDataStore = useGenericLayoutDataStore()
	layoutDataStore.resetProgressBar()
}

/**
 * Resets the state of all the global stores.
 */
export function resetStoreState()
{
	const authDataStore = useAuthDataStore()
	const customDataStore = useCustomDataStore()
	const genericDataStore = useGenericDataStore()
	const userDataStore = useUserDataStore()
	const navDataStore = useNavDataStore()
	const genericLayoutDataStore = useGenericLayoutDataStore()
	const layoutDataStore = useLayoutDataStore()
	const globalTablesDataStore = useGlobalTablesDataStore()

	// Tracing data store is deliberately being left out, since it might be useful to keep it's data.
	authDataStore.resetStore()
	customDataStore.resetStore()
	genericDataStore.resetStore()
	userDataStore.resetStore()
	navDataStore.resetStore()
	genericLayoutDataStore.resetStore()
	layoutDataStore.resetStore()
	globalTablesDataStore.resetStore()
}

export default {
	openQSign,
	getReadableTextColor,
	setNavigationState,
	normalizeDataInNavigationParams,
	normalizeRouteForSaveNavigation,
	saveNavigation,
	setAppConfig,
	buildHumanKey,
	getLayoutVariables,
	displayMessage,
	setShowCookies,
	scrollToTop,
	scrollToBottom,
	scrollTo,
	formatString,
	dateToString,
	dateToISOString,
	isoStringToDate,
	timeToString,
	hasTimeProperties,
	isDate,
	validateImageFormat,
	imageObjToSrc,
	computeColorPlaceholder,
	getDropdownPosition,
	getDropdownPositionOffScreen,
	dropdownIsFocused,
	getCoords,
	textDisplay,
	numericDisplay,
	currencyDisplay,
	dateDisplay,
	booleanDisplay,
	imageDisplay,
	documentDisplay,
	enumerationDisplay,
	radioDisplay,
	isEmpty,
	mergeOptions,
	validateTexts,
	validateFileExtAndSize,
	handleFileError,
	goBack,
	btnHasPermission,
	getModelStructureObj,
	setProgressBar,
	resetProgressBar,
	resetStoreState,
	focusElement,
	removeModal
}
