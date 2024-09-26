import { markRaw, readonly } from 'vue'
import _forEach from 'lodash-es/forEach'
import _isEmpty from 'lodash-es/isEmpty'
import _isEqual from 'lodash-es/isEqual'
import _some from 'lodash-es/some'
import _set from 'lodash-es/set'

import { useTracingDataStore } from '@/stores/tracingData.js'

import { postData } from '@/api/network'
import modelFieldType from '@/mixins/formModelFieldTypes.js'
import ViewModelBase from '@/mixins/viewModelBase.js'

export default class FormViewModelBase extends ViewModelBase
{
	constructor(vueContext, options)
	{
		super(vueContext, options)

		// The view model metadata
		Object.defineProperty(this, 'modelInfo', {
			value: {
				name: undefined,
				area: undefined,
				actions: {
					recalculateFormulas: undefined,
					loadLookupContent: 'ReloadDBEdit',
					getLookupDependents: 'GetDependants'
				}
			},
			enumerable: false,
			configurable: true
		})

		// The web api request counters - to accept only the last one's response and discard the others
		Object.defineProperty(this, 'recalculateFormulasRequestNumber', {
			value: 0,
			enumerable: false,
			writable: true
		})

		// An object with the IDs and values that triggered the last calls to recalculateFormulas()
		Object.defineProperty(this, 'lastRecalculatedValues', {
			value: markRaw({}),
			enumerable: false,
			writable: true
		})
	}

	/**
	 * A list of the fields with unsaved changes.
	 */
	get dirtyFields()
	{
		const dirtyFields = []

		for (let modelField in this)
		{
			const fieldObj = this[modelField]

			if (fieldObj instanceof modelFieldType.Base &&
				fieldObj.isDirty)
				dirtyFields.push(fieldObj)
		}

		return dirtyFields
	}

	/**
	 * A list with the area and name of unsaved changes ex: AREA.FIELD.
	 */
	get dirtyFieldNames()
	{
		return this.dirtyFields.map((obj) => `${obj.area}.${obj.field}`)
	}

	/**
	 * True if the View Model has fields with unsaved changes, false otherwise.
	 */
	get isDirty()
	{
		return _some(this, (modelField) => modelField instanceof modelFieldType.Base && modelField.isDirty)
	}

	/**
	 * The values of the vue model in the format expected by the view model of the server.
	 */
	get serverObjModel()
	{
		const viewModel = {}

		for (let modelField in this)
		{
			const fieldObj = this[modelField]

			if (fieldObj instanceof modelFieldType.Base &&
				fieldObj.type !== 'Lookup' &&
				fieldObj.ignoreFldSubmit !== true)
			{
				const value = fieldObj.serverValue
				viewModel[modelField] = value
			}
		}

		return readonly(viewModel)
	}

	/**
	 * True if there are no validations errors, false otherwise.
	 */
	get isValid()
	{
		return !_some(this.validateModel(), (fldValidation) => !fldValidation.value || !fldValidation.size)
	}

	/**
	 * Resets the values of all model fields back to their original ones.
	 */
	resetValues()
	{
		for (let modelField in this)
		{
			const fieldObj = this[modelField]
			fieldObj.resetValue()
		}
	}

	/**
	 * Recalculates the server side formulas.
	 * @param {object} triggerFields An object with the fields that triggered the call
	 * @returns A promise with the response from the server.
	 */
	recalculateFormulas(triggerFields = {})
	{
		if (_isEmpty(this.modelInfo.area) || _isEmpty(this.modelInfo.actions.recalculateFormulas))
			return

		// Check if there was any change since the last call to recalculateFormulas().
		// If 'triggerFields' is empty, runs the recalculation anyway.
		if (Object.keys(triggerFields).length > 0)
		{
			let hasChanged = false

			for (let i in triggerFields)
			{
				const fieldValue = triggerFields[i]

				if (!_isEqual(this.lastRecalculatedValues[i], fieldValue))
				{
					this.lastRecalculatedValues[i] = fieldValue
					hasChanged = true
				}
			}

			// If no changes were detected, doesn't do anything.
			if (!hasChanged)
				return
		}

		const model = this.serverObjModel

		return postData(
			this.modelInfo.area,
			this.modelInfo.actions.recalculateFormulas,
			model,
			(data, request) => {
				const requestNumber = request.headers.recalculateformulasrequestnumber
				if (Number(requestNumber) !== this.recalculateFormulasRequestNumber)
					return

				if (request.data?.Success)
				{
					if (typeof data !== 'object')
						return

					for (let modelField in this)
					{
						const fieldObj = this[modelField]

						if (_isEmpty(fieldObj.area) || _isEmpty(fieldObj.field))
							continue

						if (fieldObj instanceof modelFieldType.Base)
						{
							const fieldArea = fieldObj.area.toLowerCase()
							const fieldName = fieldObj.field.toLowerCase()
							const fieldFullName = `${fieldArea}.${fieldName}`
							const fieldValue = data[fieldFullName]

							if (typeof fieldValue !== 'undefined')
								fieldObj.updateValue(fieldValue)
						}
					}
				}
				else
				{
					const tracingDataStore = useTracingDataStore()
					tracingDataStore.addError({
						origin: 'recalculateFormulas',
						message: `Error found while trying to recalculate the formulas for form "${this.modelInfo.name}".`,
						contextData: data
					})
				}
			},
			undefined,
			{
				headers: {
					RecalculateFormulasRequestNumber: ++this.recalculateFormulasRequestNumber
				}
			},
			this.navigationId)
	}

	/**
	 * Initialization of field value formula events
	 */
	initFieldsValueFormula()
	{
		_forEach(this, (modelField) => {
			// Field value formulas
			if (modelField.valueFormula)
			{
				if (typeof modelField.valueFormula.runFormula !== 'function')
				{
					modelField.valueFormula.runFormula = (originFieldData) => {
						if (modelField.valueFormula.stopRecalcCondition())
							return

						const execCondition = modelField.valueFormula.execCondition
						if (typeof execCondition === 'function' && !execCondition.call(this))
							return

						const params = {
							originField: originFieldData?.modelField,
							currentField: modelField
						}

						// If it's a server-side recalculation, it's value will be set when the recalculateFormulas() function is called.
						if (!modelField.valueFormula.isServerRecalc)
						{
							const evalResult = modelField.valueFormula.fnFormula.call(this, params)
							Promise.resolve(evalResult).then((value) => modelField.updateValue(value))
						}
					}
				}

				this.internalEvents.offMany([...modelField.valueFormula.dependencyEvents, 'CALC_FIELDS_FORMULAS'], modelField.valueFormula.runFormula)
				this.internalEvents.onMany([...modelField.valueFormula.dependencyEvents, 'CALC_FIELDS_FORMULAS'], modelField.valueFormula.runFormula)
			}

			// Fill when formula
			if (modelField.fillWhen)
			{
				if (typeof modelField.fillWhen.runFormula !== 'function')
				{
					modelField.fillWhen.runFormula = () => {
						Promise.resolve(modelField.fillWhen.fnFormula.call(this)).then((value) => {
							if (!value)
								modelField.clearValue()
						})
					}
				}

				this.internalEvents.offMany([...modelField.fillWhen.dependencyEvents, 'CALC_FILL_WHEN_FORMULAS'], modelField.fillWhen.runFormula)
				this.internalEvents.onMany([...modelField.fillWhen.dependencyEvents, 'CALC_FILL_WHEN_FORMULAS'], modelField.fillWhen.runFormula)
			}
		})
	}

	/**
	 * Forces the recalculation of the DB fields formulas.
	 */
	calcFieldsFormulas()
	{
		this.emitInternalEvent('CALC_FIELDS_FORMULAS')
	}

	/**
	 * Forces the recalculation of the "Show when" formulas.
	 */
	calcShowWhenFormulas()
	{
		this.emitInternalEvent('CALC_SHOW_WHEN_FORMULAS')
	}

	/**
	 * Forces the recalculation of the "Block when" formulas.
	 */
	calcBlockWhenFormulas()
	{
		this.emitInternalEvent('CALC_BLOCK_WHEN_FORMULAS')
	}

	/**
	 * Forces the recalculation of the "Fill when" formulas.
	 */
	calcFillWhenFormulas()
	{
		this.emitInternalEvent('CALC_FILL_WHEN_FORMULAS')
	}

	/**
	 * Performs validations over the model fields.
	 * @returns The validation results.
	 */
	validateModel()
	{
		let modelValidations = {}

		_forEach(this, (modelField, modelFieldName) => {
			if (modelField instanceof modelFieldType.Base)
			{
				_set(modelValidations, modelFieldName, {
					fieldName: modelFieldName,
					// If the field is required, ensures it's filled.
					value: modelField.validateValue(),
					// If the field has a maximum number of characters, ensures it hasn't been exceeded.
					size: modelField.validateSize()
				})
			}
		})

		return modelValidations
	}

	/**
	 * Saves the newly uploaded files in document fields, if the form has any.
	 * @returns A list with the results of the requests sent to the server.
	 */
	async saveDocuments()
	{
		const promises = [],
			documentFields = Object.values(this).filter((e) => e instanceof modelFieldType.Document && e.isDirty)

		for (let field of documentFields)
		{
			const currentDocument = field.currentDocument

			// Check for a newly uploaded file.
			if (currentDocument.fileData === null)
				continue

			// Submit a different request for each file.
			const promise = new Promise((resolve) => {
				postData(
					field.area,
					'SetFile',
					currentDocument.dataToSubmit,
					(data, request) => {
						if (request.data?.Success)
						{
							field.properties.updateValue(data.properties)
							field.documentFK.updateValue(data.properties.documentId)

							const areaKeyField = this.vueContext.dataApi.keys[field.area.toLowerCase()]
							field.setTickets(areaKeyField.value, this.navigationId)
							currentDocument.reset()

							resolve(true)
						}
						else
						{
							const tracingDataStore = useTracingDataStore()
							tracingDataStore.addError({
								origin: 'saveDocuments',
								message: `Error found while trying to save document "${field.id}".`,
								contextData: field
							})

							resolve(false)
						}
					},
					undefined,
					{ contentType: 'application/octet-stream' },
					this.navigationId)
			})

			promises.push(promise)
		}

		return await Promise.all(promises)
	}

	/**
	 * Saves the editing and deletion state of all the document fields in the form, if any.
	 * @returns A boolean with the result of the server request.
	 */
	async setDocumentChanges()
	{
		const unsavedChanges = [],
			documentFields = Object.values(this).filter((e) => e instanceof modelFieldType.Document && e.isDirty)

		for (let field of documentFields)
		{
			const currentDocument = field.currentDocument,
				properties = field.properties,
				changes = {}

			// Check the editing state.
			if (currentDocument.fileData === null && properties.value.editing !== (properties.originalValue?.editing ?? false))
				changes.editing = properties.value.editing

			// Check for versions that should be deleted.
			if (currentDocument.deleteType !== -1)
			{
				changes.currentVersion = properties.value.version
				changes.deleteType = currentDocument.deleteType
				changes.delete = true
			}

			if (!_isEmpty(changes))
			{
				changes.ticket = currentDocument.ticket
				unsavedChanges.push(changes)
			}
		}

		// Submit a single request with all the state changes and delete operations.
		if (unsavedChanges.length > 0)
		{
			const promise = new Promise((resolve) => {
				postData(
					this.modelInfo.area,
					'SetFilesState',
					{ documents: unsavedChanges },
					(_, request) => {
						if (request.data?.Success)
						{
							for (let field of documentFields)
							{
								const areaKeyField = this.vueContext.dataApi.keys[field.area.toLowerCase()]
								field.setTickets(areaKeyField.value, this.navigationId)

								// Only reset if not also submitting a new file to replace the one that was deleted
								if (field.currentDocument.submitMode === -1)
									field.currentDocument.reset()
							}

							resolve(true)
						}
						else
						{
							const tracingDataStore = useTracingDataStore()
							tracingDataStore.addError({
								origin: 'saveDocuments',
								message: `Error found while trying to set document properties in form "${this.modelInfo.name}".`,
								contextData: request.data
							})

							resolve(false)
						}
					},
					undefined,
					undefined,
					this.navigationId)
			})

			return await Promise.resolve(promise)
		}

		return true
	}
}
