﻿/**
 * Checks if the stored object has information related to the specified form.
 * @param {string} key The key that identifies the form
 * @param {object} storeObj The object in the store
 * @param {object} formInfo The information of the form
 * @returns True if the stored object has valid data for the specified form, false otherwise.
 */
export function validateStoredValues(key, storeObj, formInfo)
{
	if (typeof key !== 'string' || key.length === 0)
		return false

	const areaName = formInfo.area
	const formName = formInfo.name

	const area = storeObj[areaName]
	if (typeof area === 'undefined')
		return false

	const record = area[key]
	if (typeof record === 'undefined')
		return false

	const form = record[formName]
	if (typeof form === 'undefined')
		return false

	return true
}

/**
 * Sets the value of the specified field with the value in the store.
 * @param {object} field The field
 * @param {object} context The current context
 */
export function setValuesFromStore(field = {}, context = {})
{
	const key = context.storeKey

	if (!validateStoredValues(key, context.formValues, context.formInfo))
		return

	const areaName = context.formArea
	const formName = context.formInfo.name
	const fieldValue = context.formValues[areaName][key][formName][field.id]

	if (typeof fieldValue === 'undefined')
		return

	// We don't want to rewrite new data coming from the server with old data already stored client-side.
	if (field.hasSameValue(fieldValue.oldValue))
		field.updateValue(fieldValue.value)
}

/**
 * Checks whether or not the specified field is visible, according to his and his parent's "show when" conditions.
 * @param {object} controls The form controls
 * @param {string} fieldId The id of the field
 * @param {boolean} checkCollapsed If true, will also check if it's not visible because one of the parents is a collapsed group or tab (optional)
 * @returns True if it's visible, false otherwise.
 */
export function fieldIsVisible(controls, fieldId, checkCollapsed)
{
	if (typeof checkCollapsed !== 'boolean')
		checkCollapsed = false

	let field = controls[fieldId]
	if (!field || !field.isVisible ||
		checkCollapsed && field.type === 'Group' && field.isCollapsible && !field.isOpen ||
		checkCollapsed && field.type === 'Tab' && controls.formTabs.selectedTab !== fieldId)
		return false

	let containerId = field.container
	let tabId = field.tab

	if (containerId)
		return fieldIsVisible(controls, containerId, checkCollapsed)
	else if (tabId)
		return fieldIsVisible(controls, tabId, checkCollapsed)
	return true
}

/**
 * Makes the specified field visible, if possible, by opening any tabs or collapsible groups that may be hiding it.
 * @param {object} controls The form controls
 * @param {string} fieldId The id of the field
 * @param {boolean} skipValidation If true, it won't check if the field can be made visible (optional)
 */
export function makeFieldVisible(controls, fieldId, skipValidation)
{
	if (typeof skipValidation !== 'boolean')
		skipValidation = false
	if (!skipValidation && !fieldIsVisible(controls, fieldId))
		return

	let field = controls[fieldId]

	if (field.type === 'Group' && field.isCollapsible)
		field.setState(true)
	else if (field.type === 'Tab')
		controls.formTabs.selectTab(fieldId)

	let containerId = field.container
	let tabId = field.tab

	if (containerId)
		makeFieldVisible(controls, containerId, true)
	else if (tabId)
		makeFieldVisible(controls, tabId, true)
}

/**
 * Executes the action of the specified form trigger.
 * @param {object} trigger The trigger
 * @returns The result of the trigger execution.
 */
export async function executeTriggerAction(trigger)
{
	if (typeof trigger.execute !== 'function')
		return null

	if (typeof trigger.condition !== 'function')
		return trigger.execute()

	const validation = await trigger.condition()
	if (typeof validation === 'boolean' && validation ||
		typeof validation === 'object' && validation.Success && validation.Result)
		return trigger.execute()
	return null
}

export default {
	validateStoredValues,
	setValuesFromStore,
	fieldIsVisible,
	makeFieldVisible,
	executeTriggerAction
}
