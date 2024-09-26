import { readonly } from 'vue'
import { isNavigationFailure } from 'vue-router'
import { mapActions, mapState } from 'pinia'
import _forEach from 'lodash-es/forEach'
import _isEmpty from 'lodash-es/isEmpty'
import _some from 'lodash-es/some'

import { useGenericDataStore } from '@/stores/genericData.js'
import { useGlobalTablesDataStore } from '@/stores/globalTablesData.js'
import { useNavDataStore } from '@/stores/navData.js'

import netAPI from '@/api/network'
import { QEventEmitter } from '@/api/global/eventBus.js'
import hardcodedTexts from '@/hardcodedTexts.js'
import formFunctions from '@/mixins/formFunctions.js'
import genericFunctions from '@/mixins/genericFunctions.js'
import modelFieldType from '@/mixins/formModelFieldTypes.js'
import fieldControlClass from '@/mixins/fieldControl.js'
import formControlClass from '@/mixins/formControl.js'
import { formModes, labelAlignment, messageTypes } from '@/mixins/quidgest.mainEnums.js'
import { loadResources } from '@/plugins/i18n.js'

import LayoutHandlers from '@/mixins/layoutHandlers.js'
import ListHandlers from '@/mixins/listHandlers.js'
import NavHandlers from '@/mixins/navHandlers.js'
import VueNavigation from '@/mixins/vueNavigation.js'

/*****************************************************************
 * This mixin defines methods to be reused in form components.   *
 *****************************************************************/
export default {
	emits: {
		'after-apply-form': () => true,
		'after-delete-form': () => true,
		'after-exit-form': () => true,
		'after-load-form': () => true,
		'after-save-form': () => true,
		'before-apply-form': () => true,
		'before-delete-form': () => true,
		'before-exit-form': () => true,
		'before-load-form': () => true,
		'before-save-form': () => true,
		'cancel-insert': (payload) => typeof payload === 'object',
		'close': (payload) => typeof payload === 'object',
		'custom-event': () => true,
		'deselect': (payload) => typeof payload === 'object',
		'edit': (payload) => typeof payload === 'object',
		'insert-form': (payload) => typeof payload === 'object',
		'is-form-dirty': (payload) => typeof payload === 'object',
		'update-form-mode': (payload) => typeof payload === 'string',
		'update-model-id': (payload) => typeof payload === 'string',
		'update:nested-model': (payload) => typeof payload === 'object'
	},

	mixins: [
		LayoutHandlers,
		ListHandlers,
		NavHandlers,
		VueNavigation
	],

	inheritAttrs: false,

	props: {
		/**
		 * The id of the form.
		 */
		id: {
			type: String,
			default: ''
		},

		/**
		 * The current mode of the form.
		 */
		mode: {
			type: String,
			default: 'EDIT'
		},

		/**
		 * A string with all the allowed modes of the form (if all modes are allowed it will be "vedai").
		 */
		modes: {
			type: String,
			default: ''
		},

		/**
		 * Whether or not this form is nested.
		 */
		isNested: {
			type: Boolean,
			default: false
		},

		/**
		 * The model of the form (if it's nested).
		 */
		nestedModel: {
			type: Object,
			default: () => ({})
		},

		/**
		 * The configuration of the form (if it's nested).
		 */
		nestedFormConfig: {
			type: fieldControlClass.NestedFormConfig,
			default: () => new fieldControlClass.NestedFormConfig()
		},

		/**
		 * The id of the navigation history for the form.
		 */
		historyBranchId: {
			type: String,
			default: ''
		},

		/**
		 * Whether or not the form is used as a homepage.
		 */
		isHomePage: {
			type: Boolean,
			default: false
		},

		/**
		 * If passed, the properties in this object will override the properties of the form buttons.
		 */
		buttonsOverride: {
			type: Object,
			default: () => ({})
		},

		/**
		 * The mode of the parent form.
		 */
		parentFormMode: {
			type: String,
			default: ''
		},

		/**
		 * The permissions of the form.
		 */
		permissions: {
			type: Object,
			default: () => ({})
		},

		/**
		 * The place where the actions should be (only for multiforms).
		 */
		actionsPlacement: {
			type: String,
			default: 'left'
		},

		/**
		 * We can't immediately apply the choices of the Table list or Tree list on the previous level to the history,
		 * because they will limit other branches of the history. History entries should only be added to the current form level.
		 *
		 * For example, if we choose Table list row at the level of the form to which the list belongs,
		 * forms such as «Multiform» or «Extended support form» will always have this limit applied.
		 */
		historyEntries: {
			type: Array,
			default: () => []
		},

		/**
		 * Values used to set default values for specified fields.
		 */
		prefillValues: {
			type: Object,
			default: () => ({})
		}
	},

	data()
	{
		return {
			internalEvents: new QEventEmitter(),

			formControl: new formControlClass.FormControl(this),

			labelAlignment: readonly(labelAlignment),

			formModes: readonly(formModes),

			authData: {
				isAllowed: false, // True if the user has permission to access the form data, false otherwise.
				errorMessage: ''
			},

			formModalIsReady: false,

			showExternalApp: true,

			// Indicates whether the form has already loaded initial data.
			formInitialDataLoaded: false,

			formHeader: '',

			validationErrors: {},

			// Controls the visibility of the horizontal anchor container.
			anchorContainerVisibility: true,

			// Allows anchor containers to be opened on first load.
			anchorsTabOpened: false,

			// Changing the value of this key will force a reload of the form DOM.
			domVersionKey: 1
		}
	},

	async created()
	{
		this.setNestedRouteData()
		// Load resources (translations).
		this.addBusy(loadResources(this, this.interfaceMetadata.requiredTextResources))

		// Apply history limits.
		/*
			We can't immediately apply the choices of the Table list or Tree list on the previous level to the history,
				because they will limit other branches of the history. History entries should only be added to the current form level.
			For example, if we choose Table list row at the level of the form to which the list belongs,
				forms such as «Multiform» or «Extended support form» will have this limit applied forever.
		*/
		_forEach(this.historyEntries, (historyEntry) => this.setEntryValue({ navigationId: this.navigationId, key: historyEntry.key, value: historyEntry.value }))

		const canLoad = await this.beforeLoad()
		// Load form data.
		if (canLoad)
			this.loadFormData(true)
	},

	mounted()
	{
		this.$eventTracker.addTrace({
			origin: 'mounted (formHandler)',
			message: 'Form is mounted',
			contextData: { formInfo: this.formInfo }
		})

		// Override form buttons with buttons received from prop.
		for (let btnKey in this.buttonsOverride)
		{
			const button = this.buttonsOverride[btnKey]

			for (let propKey in button)
			{
				delete this.formButtons[btnKey][propKey]
				this.formButtons[btnKey][propKey] = button[propKey]
			}
		}

		// Setup of the various listeners for changes to the DB, which will update the respective list whenever changes occur.
		this.formControl.initListOnDBChangeEvent()

		this.$eventHub.on('modal-is-ready', this.setFormModalReady)
		this.$eventHub.on('form-apply', this.formApplyCallback)
		this.$eventHub.on('focus-control', this.focusControl)

		// If there's alredy a modal for this form, marks the form as ready to be shown.
		const modalEl = document.getElementById(this.uiContainersId.main)
		if (modalEl !== null || this.isNested || this.formInfo.route !== this.$route.name)
			this.formModalIsReady = true
	},

	beforeUnmount()
	{
		this.$eventTracker.addTrace({
			origin: 'beforeUnmount (formHandler)',
			message: 'Form will be unmounted',
			contextData: { formInfo: this.formInfo }
		})

		this.showExternalApp = false
		this.internalEvents.removeAllListeners()
		this.formControl.destroy()

		this.$eventHub.off('modal-is-ready', this.setFormModalReady)
		this.$eventHub.off('form-apply', this.formApplyCallback)
		this.$eventHub.off('focus-control', this.focusControl)

		this.componentOnLoadProc?.destroy()
	},

	computed: {
		...mapState(useGenericDataStore, [
			'suggestionModeOn',
			'reportingModeCAV'
		]),

		/**
		 * Tree of anchors for the current form.
		 */
		formAnchors()
		{
			return this.buildFormAnchors()
		},

		/**
		 * True if the show anchor container button should be visible, false otherwise.
		 */
		isAnchorsButtonVisible()
		{
			if (this.layoutConfig.FormAnchorsPosition !== 'form-header')
				return false
			return Object.values(this.controls).some((x) => x.anchored === true)
		},

		/**
		 * True if the header of the form should be visible, false otherwise.
		 */
		showFormHeader()
		{
			return (this.formControl.uiComponents.header && !_isEmpty(this.formInfo.designation) || this.formControl.uiComponents.headerButtons) && this.authData.isAllowed
		},

		/**
		 * True if the body of the form should be visible, false otherwise.
		 */
		showFormBody()
		{
			return this.authData.isAllowed
		},

		/**
		 * True if the footer of the form should be visible, false otherwise.
		 */
		showFormFooter()
		{
			const hasButtons = Object.values(this.formButtons).some((btn) => btn.isActive && btn.isVisible && btn.showInFooter)
			return hasButtons && this.formControl.uiComponents.footer && this.authData.isAllowed
		},

		/**
		 * The threshold used to set the header fixed.
		 */
		stickyThreshold()
		{
			return this.formHeader.offsetTop - (this.layoutType === 'horizontal' ? this.navbarHeight : this.headerHeight)
		},

		/**
		 * True if on scroll the header of the form should stick to the top, false otherwise.
		 */
		isStickyTop()
		{
			return !this.isNested && !this.isPopup && this.pageScroll >= this.stickyThreshold && (_isEmpty(this.layoutConfig.ContainerWidth) || this.layoutConfig.ContainerWidth === 'whole_page')
		},

		/**
		 * True if the header of the form should be sticky, false otherwise.
		 */
		isStickyHeader()
		{
			return !this.isNested && !this.isPopup
		},

		/**
		 * True if the form has fields or table configurations with unsaved changes, false otherwise.
		 */
		isDirty()
		{
			if (!this.isEditable)
				return false

			if (Array.isArray(this.tableFields))
			{
				for (let tableName of this.tableFields)
				{
					const control = this.controls[tableName]
					if (control.rowsDirty && Object.keys(control.rowsDirty).length > 0)
						return true
				}
			}

			return this.model.isDirty
		},

		/**
		 * True if the form is a popup, false otherwise.
		 */
		isPopup()
		{
			return this.formInfo.type === 'popup'
		},

		/**
		 * True if the form is a wizard step, false otherwise.
		 */
		isWizard()
		{
			return this.formInfo.type === 'wizard'
		},

		/**
		 * True if the form is a widget, false otherwise.
		 */
		isWidget()
		{
			return this.formInfo.type === 'widget'
		},

		/**
		 * True if the form is in an editable mode, false otherwise.
		 */
		isEditable()
		{
			return ![this.formModes.show, this.formModes.delete].includes(this.formInfo.mode)
		},

		/**
		 * True if the form has a route, false otherwise.
		 */
		hasRoute()
		{
			return typeof this.$route === 'object' && !_isEmpty(this.$route) && !this.isNested
		},

		/**
		 * A key unique to this record in the global store.
		 */
		storeKey()
		{
			return !_isEmpty(this.formInfo.primaryKey) ? this.primaryKeyValue : `EMPTY_FORM_${this.formInfo.name}`
		},

		/**
		 * The value of the record's primary key.
		 */
		primaryKeyValue()
		{
			const pKey = this.formInfo.primaryKey ?? ''
			return !_isEmpty(pKey) ? this.model[pKey].value ?? '' : ''
		},

		/**
		 * The area of the form.
		 */
		formArea()
		{
			let controller = this.formInfo.area

			if (_isEmpty(controller))
			{
				// Empty forms use the Home controller (if they don't have an area).
				if (this.formInfo.type === 'empty' || this.isWidget)
					controller = 'Home'
				else
				{
					this.$eventTracker.addError({
						origin: 'formArea getter (formHandler)',
						message: 'The form\'s controller is unknown.',
						contextData: { formInfo: this.formInfo }
					})
				}
			}

			return controller
		},

		/**
		 * Form container identifiers.
		 */
		uiContainersId()
		{
			const formIdentifier = this.isNested ? '' : `q-modal-${this.isHomePage ? `home-${this.system.currentModule}` : this.formInfo.route}`

			return {
				main: formIdentifier,
				header: formIdentifier ? `${formIdentifier}-header` : 'app',
				body: formIdentifier ? `${formIdentifier}-body` : 'app',
				footer: formIdentifier ? `${formIdentifier}-footer` : 'app'
			}
		},

		/**
		 * The human key of the current record.
		 */
		humanKey()
		{
			return this.isNested ? '' : this.buildHumanKey()
		},

		/**
		 * The keys of the model.
		 */
		modelKeys()
		{
			return this.dataApi.keys
		},

		/**
		 * The sections of the form buttons.
		 */
		formButtonSections()
		{
			let formModeBtns = {},
				formActionBtns = {},
				formInsertBtns = {}

			for (let i in this.formButtons)
			{
				if (this.formButtons[i].type === 'form-mode')
					formModeBtns[i] = { ...this.formButtons[i] }
				else if (this.formButtons[i].type === 'form-action' || this.formButtons[i].type === 'custom')
					formActionBtns[i] = { ...this.formButtons[i] }
				else if (this.formButtons[i].type === 'form-insert')
					formInsertBtns[i] = { ...this.formButtons[i] }
			}

			return {
				modesButtons: formModeBtns,
				actionButtons: formActionBtns,
				insertButtons: formInsertBtns
			}
		},

		/**
		 * The identifier of the first form button section
		 * with at least one visible button.
		 */
		firstVisibleButtonSection()
		{
			const sectionIds = Object.keys(this.formButtonSections)
			return sectionIds.find((sectionId) => this.showFormHeaderSection(sectionId))
		},

		/**
		 * The form values in the store, based on the current navigation ID.
		 */
		formValues()
		{
			return this.getFormValues(this.navigationId)
		}
	},

	methods: {
		...mapActions(useGenericDataStore, [
			'setInfoMessage',
			'clearInfoMessages',
			'setModal',
			'setDropdown'
		]),

		...mapActions(useNavDataStore, [
			'getFormValues',
			'storeValue',
			'storeValues'
		]),

		removeModal: genericFunctions.removeModal,

		isEmpty: genericFunctions.isEmpty,

		/**
		 * A function called whenever a new modal is ready. If that modal is for the current form,
		 * sets the form as ready to be shown in it.
		 * @param {string} modalId The id of the modal
		 */
		setFormModalReady(modalId)
		{
			if (this.formInfo.route === modalId)
				this.formModalIsReady = true
		},

		/**
		 * Reloads the data of all the visible specified controls.
		 * @param {string[]} controlIds A list with the IDs of the controls to refresh
		 * @param {function} refreshCheck A function to set additional conditions of whether or not the control should refresh (optional)
		 * @returns {Promise<void>} A promise that resolves when all control refresh operations are complete
		 */
		async refreshControls(controlIds, refreshCheck)
		{
			if (!Array.isArray(controlIds))
				return

			if (typeof refreshCheck !== 'function')
				refreshCheck = () => true

			const promises = controlIds
				.filter((ctrlId) => {
					const control = this.controls[ctrlId]
					const isVisible = formFunctions.fieldIsVisible(this.controls, ctrlId, true)

					return !_isEmpty(control) && isVisible && refreshCheck(control)
				})
				.map((ctrlId) => {
					const control = this.controls[ctrlId]
					return control.reload()
				})

			await Promise.all(promises)
		},

		/**
		 * Reloads the data of all the visible table lists and timelines in the form.
		 * @param {boolean} isFirstLoad Whether or not the method is being called for the first time
		 * @returns {Promise<void>} A promise that resolves when all data refresh operations are complete
		 */
		async refreshAllListsData(isFirstLoad)
		{
			await Promise.all([
				// Load the data of the lists.
				this.refreshControls(this.tableFields, (table) => !isFirstLoad || !table.isInCollapsible && !table.config.showAfterFilter),
				// Load the data of the timelines.
				this.refreshControls(this.timelineFields)
			]).then(() => {
				this.initTablesData(this.tableFields)
				this.initTablesData(this.timelineFields)
			})
		},

		/**
		 * Initializes the data for the specified control IDs.
		 * @param {Array<string>} controlIds An array of control IDs whose data needs to be initialized.
		 */
		initTablesData(controlIds)
		{
			if (!Array.isArray(controlIds))
				return

			for (const tableFieldId of controlIds)
			{
				const tableField = this.controls[tableFieldId]
				tableField.initData()
			}
		},

		/**
		 * Reloads the data of the specified list.
		 * @param {string} listName The name of the list
		 * @param {array} dirtyFields A list of dirty fields
		 */
		async reloadList(listName, dirtyFields)
		{
			const list = this.controls[listName]
			const isVisible = formFunctions.fieldIsVisible(this.controls, listName, true)

			// Update the control if no dirty fields were specified,
			// or if there is some overlap between the dirty fields and the list dependencies.
			const updateControl = !Array.isArray(dirtyFields) ||
				_some(list.controlLimits, (limit) => dirtyFields.includes(limit.dependencyField)) ||
				_some(list.columns, (column) => dirtyFields.includes(`${column.area}.${column.field}`))

			const promises = []

			if (isVisible && updateControl)
				promises.push(list.reload())

			if (list.hasDependencies && updateControl)
				promises.push(this.model.recalculateFormulas())

			await Promise.all(promises)
		},

		/**
		 * Sets the values of the form fields with the values in the store.
		 */
		setValuesFromStore()
		{
			for (let i in this.model)
			{
				const field = this.model[i]
				formFunctions.setValuesFromStore(field, this)
			}
		},

		/**
		 * Sets the states of the form containers with the values in the store.
		 * @param {string} key A key unique to this form
		 */
		setContainersStateFromStore(key)
		{
			if (!formFunctions.validateStoredValues(key, this.containersState, this.formInfo))
				return

			const areaName = this.formArea
			const formName = this.formInfo.name

			for (let i in this.controls)
			{
				const container = this.controls[i]

				if (!(container instanceof fieldControlClass.GroupControl))
					continue

				let containerState = this.containersState[areaName][key][formName][container.id]
				if (typeof containerState === 'undefined')
					containerState = false

				if (!container.isInAccordion)
					container.setState(containerState)
				else if (containerState)
				{
					const accordion = this.controls[container.container]
					accordion.setOpenGroup(containerState, container.id)
				}
			}

			// In case the form has tabs, selects the right one.
			if (this.controls.formTabs)
			{
				const selectedTab = this.containersState[areaName][key][formName].formTabs
				if (selectedTab && typeof selectedTab === 'string')
					this.controls.formTabs.selectTab(selectedTab)
			}
		},

		/**
		 * Resets the state of all containers (closes collapsible groups and selects first available tab).
		 */
		resetContainersState()
		{
			for (let i in this.controls)
			{
				const container = this.controls[i]

				if (!(container instanceof fieldControlClass.GroupControl))
					continue

				container.setState(false)
			}

			this.controls.formTabs?.selectFirstTab()
		},

		/**
		 * Hydrates the raw data coming from the server with the necessary metadata.
		 * @param {object} rawData The data to be hydrated
		 */
		hydrate(rawData)
		{
			this.model.hydrate(rawData)

			// Global tables
			const globalTablesData = useGlobalTablesDataStore()
			globalTablesData.loadFromViewModel(rawData)
		},

		/**
		 * Hydrates the raw data for a given field coming from the server
		 * with the necessary metadata.
		 * @param {object} modelField The target field
		 * @param {object} rawData The data to be hydrated
		 */
		hydrateField(modelField, rawData)
		{
			this.model.hydrateField(modelField, rawData)
		},

		/**
		 * Removes the specified number of navigation levels, or 1 if unspecified.
		 * @param {number} goBack The number of levels we want to go back
		 * @returns A promise with the result.
		 */
		formRemoveHistoryLevels(goBack)
		{
			let levelsToRemove = 1
			if (Number.isInteger(Number(goBack)))
				levelsToRemove = Number.parseInt(goBack)

			this.removeHistoryLevels({
				navigationId: this.navigationId,
				levels: levelsToRemove
			})

			// If the last route was skipped because of a "skip if just one" condition,
			// then we need to go back to the route before that last one.
			while (this.navigation.currentLevel?.entries?.SkipIfJustOne)
				this.navigation.removeNavigationLevel()
		},

		/**
		 * Performs a client-side validation of the current values of the form fields.
		 * @returns An object with the errors found during the validation.
		 */
		validateFormValues()
		{
			const validationErrors = {}

			for (let i in this.controls)
			{
				const ctrl = this.controls[i]

				if (!ctrl.modelField)
					continue

				const field = this.model[ctrl.modelField]
				const errorMsgs = []

				if (typeof field === 'undefined')
					continue

				// If the field is required, ensures it's filled.
				if (!field.validateValue())
				{
					const errorMsg = genericFunctions.formatString(this.Resources[hardcodedTexts.fieldIsRequired], ctrl.label)
					errorMsgs.push(errorMsg)
				}

				// If the field has a maximum number of characters, ensures it hasn't been exceeded.
				if (!field.validateSize())
				{
					const errorMsg = genericFunctions.formatString(this.Resources[hardcodedTexts.maxCharsExceeded], ctrl.label, ctrl.maxLength)
					errorMsgs.push(errorMsg)
				}

				if (!_isEmpty(errorMsgs))
					validationErrors[field.id] = errorMsgs
			}

			// Adds a default error message at the end, to match the server-side behavior.
			if (!_isEmpty(validationErrors))
				validationErrors.Erro = [this.Resources[hardcodedTexts.saveError]]

			return validationErrors
		},

		/**
		 * If there are any dirty fields, makes them valid.
		 */
		setFormFieldsValid()
		{
			if (!this.isDirty)
				return

			for (let i in this.model)
			{
				const field = this.model[i]
				// The editable table list data is in memory.
				if (!(field instanceof modelFieldType.GridTableList) && field.isDirty)
					field.hydrate(field.value)
			}

			// Extended support forms - nested forms need to be validated as well.
			this.$emit('is-form-dirty', { isDirty: false, afterFormSave: true })
		},

		/**
		 * Changes the route to the specified navigation level.
		 * @param {object} navLevel The navigation level
		 * @param {object} options Additional options to include in route params (optional)
		 * @param {boolean} replace Whether to create a new entry in the browser history or to replace the current one (optional)
		 * @returns A falsy value if the navigation succeeds, or a navigation failure or true value if it fails.
		 */
		async navigateTo(navLevel, options, replace = false)
		{
			if (!this.hasRoute || typeof navLevel !== 'object')
				return true
			if (typeof options !== 'object')
				options = {}

			const nonNestedLevel = navLevel?.getFirstNotNested()
			const location = nonNestedLevel?.location || 'home'
			// The clearHistory flag in params should be true when we navigate to another menu. However, in forms without an area (menu structures M->F+),
			// the flag persisted from the previous menu navigation. This made extended support forms lose the positioning of the record after selecting a record action
			// (read, edit, ...) and then canceling and leaving the form.
			const params = {
				isControlled: true,
				...nonNestedLevel?.params,
				isDuplicate: false,
				...options,
				historyBranchId: this.navigationId,
				clearHistory: false // Reset clearHistory from previous navigation.
			}

			try
			{
				let result
				if (replace)
					result = await this.$router.replace({ name: location, params })
				else
					result = await this.$router.push({ name: location, params })

				genericFunctions.saveNavigation(this.$route, (data) => {
					const navParams = {
						navigationId: this.navigationId,
						options: data.options,
						previousLevel: this.navigation.previousLevel
					}

					this.addHistoryLevel(navParams)
				})

				return result
			}
			catch (error)
			{
				this.$eventTracker.addError({
					origin: 'navigateTo (formHandler)',
					message: 'An error occurred while navigating:',
					contextData: { error }
				})
				throw error
			}
		},

		/**
		 * Parses the error messages from the server response.
		 * @param {object} data The server response with the model state
		 * @returns The list of errors by field of the form.
		 */
		parseResponseErrors(data)
		{
			let errors = {}

			// Recursively clear any previous server error messages
			this.model.clearServerErrorMessages()

			for (const key in data)
			{
				if (!key.includes('.'))
					errors[key] = data[key]

				this.model.setServerErrorMessages(key, data[key])
			}

			return errors
		},

		/**
		 * Emits a component event with the specified name and properties.
		 * @param {string} eventName The name of the event
		 * @param {any} eventProps Properties to send in the emit
		 */
		emitEvent(eventName, eventProps)
		{
			if (typeof eventName === 'string')
				this.$emit(eventName, eventProps)
		},

		/**
		 * Builds an object with extra nav params that can be reused.
		 * @param {string} previousRowKey The key of the previous record, if any
		 * @returns An object with extra properties to be added to the navigation params.
		 */
		getExtraNavParams(previousRowKey)
		{
			return {
				culture: this.system.currentLang,
				previouslyRemovedRoute: this.formInfo.route,
				previouslyRemovedTable: this.formInfo.area,
				previouslyRemovedRowKey: previousRowKey,
				returnControl: this.navigation.currentLevel.previousLevel.params.anchor,
				keepAlerts: true
			}
		},

		/**
		 * Displays an error message that came from the server, in case there are no validation errors.
		 * @param {object} data The server response data
		 */
		displayErrorMessage(data)
		{
			if (typeof data.Message === 'string' && _isEmpty(this.validationErrors))
			{
				const errorProps = {
					type: messageTypes.E,
					message: data.Message,
					pinned: true
				}
				this.setInfoMessage(errorProps)
			}
		},

		/**
		 * Loads the raw data of the form from the server.
		 * @param {boolean} forceFetch Specifies if the form fields should be forcefully fetched (defaults to false)
		 * @returns True if the operation was successful, false otherwise.
		 */
		async fetchFormFields(forceFetch)
		{
			if (typeof forceFetch !== 'boolean')
				forceFetch = false

			if (this.formInitialDataLoaded && !forceFetch)
				return false

			const currentLevel = this.navigation.currentLevel
			if (_isEmpty(currentLevel))
				return false

			const mode = this.hasRoute ? this.$route.params.mode : this.mode,
				id = this.hasRoute ? (this.$route.params.id || null) : (this.isNested && this.mode === this.formModes.new ? null : this.id),
				dupParam = this.isNested && this.mode === 'DUPLICATE' ? 'true' : this.$route.params.isDuplicate,
				isDuplicate = typeof dupParam !== 'undefined' ? dupParam === 'true' : false,
				isNewLocation = id === null || isDuplicate

			// FOR: FORM_PREFILL_VALUES, ROW_REORDERING
			let prefillValues = this.hasRoute ? this.$route.params.prefillValues : this.prefillValues
			if (!prefillValues || typeof prefillValues !== 'object')
				prefillValues = undefined

			if (!Object.values(this.formModes).includes(mode))
				return false

			for (let i in this.model)
				this.model[i].isReady = false

			let params = { id, isNewLocation, prefillValues }
			if (this.$route.query)
			{
				params = {
					...params,
					...this.$route.query
				}
			}

			let opResult = false

			// When the form is used as a component with the already loaded model.
			if (this.isNested && !_isEmpty(this.nestedModel))
			{
				this.formInfo.mode = mode
				// virtual - Grid table list form.
				if (this.formInfo.type !== 'virtual')
					this.hydrate(this.nestedModel)
				this.authData.isAllowed = true
				this.formInitialDataLoaded = true

				// Handles the storing of field values.
				if (this.storeKey !== null)
				{
					this.setValuesFromStore()
					this.storeValues({
						navigationId: this.navigationId,
						key: this.storeKey,
						formInfo: this.formInfo,
						fields: this.model
					})
				}

				opResult = true
			}
			else
			{
				await netAPI.fetchFormData(
					this.formArea,
					this.formInfo.name,
					mode,
					params,
					async (data, response) => {
						// Show the warnings and errors.
						this.validationErrors = !_isEmpty(response.data.Errors)
							? this.parseResponseErrors(response.data.Errors)
							: {}

						if (typeof response.data.Success !== 'boolean' || !response.data.Success)
						{
							this.authData.isAllowed = false
							this.authData.errorMessage = response.data.Message ?? ''
							return
						}

						this.formInfo.mode = mode
						this.hydrate(data)
						this.authData.isAllowed = true
						this.formInitialDataLoaded = true

						// When creating a new record, or duplicating an existing one, the id in the navigation will be null because
						// the new history level is created before knowing the id of the new record. This ensures that doesn't happen.
						if (this.formInfo.primaryKey && (!currentLevel.params.id || currentLevel.params.id !== this.primaryKeyValue))
						{
							this.$emit('update-model-id', this.primaryKeyValue)

							if (!this.isNested)
							{
								const options = {
									id: this.primaryKeyValue,
									culture: this.system.currentLang,
									isControlled: true
								}

								await this.navigateTo(currentLevel, options, true)
								this.setFormReturnControl()
							}
						}

						// Handles the storing of field values.
						if (this.storeKey !== null)
						{
							this.setValuesFromStore()
							this.storeValues({
								navigationId: this.navigationId,
								key: this.storeKey,
								formInfo: this.formInfo,
								fields: this.model
							})
						}

						opResult = true
					},
					this.navigationId)
			}

			return opResult
		},

		/**
		 * Loads the raw data of the form field from the server.
		 * @returns A promise with the response from the server.
		 */
		async fetchFormField(modelField)
		{
			const id = this.hasRoute ? (this.$route.params.id || null) : this.id

			this.model[modelField].isReady = false

			await netAPI.fetchFormFieldData(
				this.formArea,
				this.formInfo.name,
				modelField,
				{ id },
				(data) => this.hydrateField(modelField, data),
				this.navigationId)
		},

		/**
		 * A function called whenever the form apply event is triggered, it calls the "applyChanges" function
		 * and executes the callback provided in the options, if any.
		 *
		 * It can receive an object with some options:
		 *     options = {
		 *         string form (optional) - The name of the form to be saved, if not specified the apply will always execute.
		 *         boolean showSuccess (optional) - Whether or not a success message should be displayed, defaults to false.
		 *         function callback (optional) - A function to be executed when the operation concludes.
		 *     }
		 * @param {object} options The provided options
		 */
		formApplyCallback(options)
		{
			if (typeof options !== 'object')
				options = {}

			if (typeof options.form === 'string' && options.form !== this.formInfo.name)
				return

			// NOTE: The absence of .catch() and .finally() blocks is intentional for now.
			// If the Promise is rejected, the code within the .then()
			// block is skipped, and no further error handling or cleanup occurs.
			// This approach might be revisited in the future to include better error handling.
			this.applyChanges(options.showSuccess).then((success) => {
				if (success && typeof options.callback === 'function')
					options.callback()
			})
		},

		/**
		 * Applies the current changes, made by the user, without leaving the form.
		 * @param {boolean} showSuccess Specifies if the user should be notified that the operation was successful (if true, a success message will be displayed)
		 * @returns True if the operation was successful, false otherwise.
		 */
		async applyChanges(showSuccess)
		{
			/*
			 * The Apply should only be invoked when the form is in a mode where the user can modify data.
			 * Thus, modes like Show and Delete, where invoking Apply doesn't make sense, are ignored.
			 * Considering that the query mode displays in the form the data that is already saved in the database,
			 *  and the user does not have the ability to edit the data, not executing the Apply in query mode is equivalent to its execution in edit mode.
			 * The reason for returning 'True' in this case has more to do with the scenarios when we have a
			 *  Button for Routine, Button for Form, or Trigger, with the Apply set to be executed.
			 */
			if (!this.isEditable)
				return Promise.resolve(true)

			if (typeof showSuccess !== 'boolean')
				showSuccess = false

			const shouldApply = await this.beforeApply()

			if (!shouldApply)
				return Promise.resolve(false)

			const applyProc = new Promise((resolve) => {
				netAPI.postFormData(
					this.formArea,
					this.formInfo.name,
					'SaveEdit',
					this.model.serverObjModel,
					async (data, response) => {
						// Show the warnings and errors.
						this.validationErrors = !_isEmpty(response.data.Errors)
							? this.parseResponseErrors(response.data.Errors)
							: {}

						if (!response.data.Success)
						{
							this.displayErrorMessage(response.data)
							resolve(false)
							return
						}

						// Emits an event saying the DB table was altered, so lists that show records from it know they need to refresh.
						this.$eventHub.emit(`changed-${this.formArea}`, this.model.dirtyFieldNames)

						// If the form fields were successfully saved, then they are no longer dirty.
						this.setFormFieldsValid()

						if (showSuccess)
						{
							const successProps = {
								type: messageTypes.OK,
								message: data.Message,
								pinned: true
							}

							this.clearInfoMessages()
							this.setInfoMessage(successProps)
						}

						await this.afterApply()
						resolve(true)
					},
					undefined,
					undefined,
					this.navigationId)
			})

			this.addBusy(applyProc, this.Resources[hardcodedTexts.processing])

			return applyProc
		},

		/**
		 * Saves the current content of the form.
		 * @param {boolean} repeatInsert Should be true if a new record is to be created after the current one, false otherwise
		 * @returns True if the operation was successful, false otherwise.
		 */
		async saveForm(repeatInsert)
		{
			if (typeof repeatInsert !== 'boolean')
				repeatInsert = false

			// We try to validate as many things as possible, before making the call to the server.
			const validationErrors = this.validateFormValues()
			if (!_isEmpty(validationErrors))
			{
				this.validationErrors = validationErrors
				return Promise.resolve(false)
			}

			const shouldSave = await this.beforeSave()

			if (!shouldSave)
				return Promise.resolve(false)

			const saveProc = new Promise((resolve) => {
				netAPI.postFormData(
					this.formArea,
					this.formInfo.name,
					this.formInfo.mode,
					this.model.serverObjModel,
					async (data, response) => {
						// Show the warnings and errors.
						this.validationErrors = !_isEmpty(response.data.Errors)
							? this.parseResponseErrors(response.data.Errors)
							: {}

						if (!response.data.Success)
						{
							this.displayErrorMessage(response.data)
							resolve(false)
							return
						}

						if (this.formInfo.mode === this.formModes.new)
						{
							// FOR: tree table select row on return.
							// Add new row key to row key path to select.
							const currentControl = this.navigation.currentLevel.previousLevel.currentControl
							const rowKeyToScroll = currentControl?.data?.rowKey ?? null

							if (Array.isArray(rowKeyToScroll))
								rowKeyToScroll.push(this.primaryKeyValue)
						}

						// Emits an event saying the DB table was altered, so lists that show records from it know they need to refresh.
						this.$eventHub.emit(`changed-${this.formArea}`, this.model.dirtyFieldNames)

						// If the form fields were successfully saved, then they are no longer dirty.
						this.setFormFieldsValid()

						this.clearInfoMessages()

						// If there are any warning messages, they will be displayed.
						if (typeof data.Warnings === 'object' && Array.isArray(data.Warnings) && !this.isEmpty(data.Warnings))
						{	
							this.showWarningsDialog(data)

							resolve(false)
							return
						}

						const shouldContinue = await this.afterSave()
						if (!shouldContinue)
						{
							resolve(false)
							return
						}

						this.continueSaveForm(repeatInsert, data)
						resolve(true)
					},
					undefined,
					undefined,
					this.navigationId)
			})

			this.addBusy(saveProc, this.Resources[hardcodedTexts.processing])

			return saveProc
		},

		/**
		 * Saves the current content of the form.
		 * @param {boolean} repeatInsert Should be true if a new record is to be created after the current one, false otherwise
		 * @param {object} data The server response data
		 */
		async continueSaveForm(repeatInsert, data)
		{
			// Sets up the success message that the user will see after leaving the form.
			const successProps = {
				type: messageTypes.OK,
				message: data.Message,
				pinned: true
			}

			this.setInfoMessage(successProps)
			genericFunctions.scrollToTop()

			if (repeatInsert)
			{
				const params = {
					...this.getExtraNavParams(this.primaryKeyValue),
					id: null,
					mode: this.formModes.new,
					modes: this.navigation.currentLevel.params.modes,
					isControlled: true
				}

				// Adds a new empty history level, with the same route as the current one.
				await this.navigateTo(this.navigation.currentLevel, params)

				this.navigation.currentLevel.clearEntries()
				this.resetContainersState()
				this.reloadFormData()
			}
			else
			{
				const options = {
					...this.getExtraNavParams(this.primaryKeyValue),
					/*
						* The back navigation doesn't need to show the data loss confirmation popup if we are going back to the previous level after a successful save.
						* This occurred when a button for a form without Apply opened a form with 'isControlled: false' in the parameters.
						* Thus, navigating back with the same parameters caused the confirmation message to appear.
						*/
					isControlled: true
				}

				this.formRemoveHistoryLevels((this.currentRouteParams || {}).goBack)

				if (this.isNested)
					this.$emit('close', { type: 'save' })
				else
					await this.navigateTo(this.navigation.currentLevel, options)
			}
		},

		/**
		 * Shows the warning messages dialog.
		 * @param {object} data The server response data
		*/
		showWarningsDialog(data)
		{	
			const buttons = {
				confirm: {
					action: () => this.continueSaveForm(false, data),
					label: this.formButtons.saveBtn.text,
				},
				cancel:{
					label: this.formButtons.cancelBtn.text,
				}
			}

			const warnings = data.Warnings.map(warning => `<div>${warning}</div>`).join('')
			genericFunctions.displayMessage(warnings, 'warning', null, buttons, { useHtml: true })
		},

		/**
		 * Deletes the current record.
		 * @returns True if the operation was successful, false otherwise.
		 */
		async deleteRecordAfterConfirmation()
		{
			const shouldDelete = await this.beforeDel()

			if (!shouldDelete)
				return Promise.resolve(false)

			return new Promise((resolve) => {
				netAPI.postFormData(
					this.formArea,
					this.formInfo.name,
					this.formModes.delete,
					{ id: this.primaryKeyValue },
					async (data, response) => {
						// Show the warnings and errors.
						this.validationErrors = !_isEmpty(response.data.Errors)
							? this.parseResponseErrors(response.data.Errors)
							: {}

						if (!response.data.Success)
						{
							this.displayErrorMessage(response.data)
							resolve(false)
							return
						}

						this.formControl.removeListOnDBChangeEvent()
						// Emits an event saying the DB table was altered, so lists that show records from it know they need to refresh.
						this.$eventHub.emit(`changed-${this.formArea}`)

						const shouldContinue = await this.afterDel()

						if (!shouldContinue)
						{
							resolve(false)
							return
						}

						// FOR: tree table select row on return.
						// Remove the row key of the deleted row from row key path to select.
						const currentControl = this.navigation.currentLevel.previousLevel.currentControl
						const rowKeyToScroll = currentControl?.data?.rowKey ?? null

						if (Array.isArray(rowKeyToScroll))
							rowKeyToScroll.pop()

						// Sets up the success message that the user will see after leaving the form.
						const successProps = {
							type: messageTypes.OK,
							message: data.Message,
							pinned: true
						}
						this.clearInfoMessages()
						this.setInfoMessage(successProps)

						const options = {
							culture: this.system.currentLang,
							previouslyRemovedRoute: this.formInfo.route,
							keepAlerts: true
						}

						this.formRemoveHistoryLevels((this.currentRouteParams || {}).goBack)

						if (this.isNested)
							this.$emit('close', { type: 'delete' })
						else
							await this.navigateTo(this.navigation.currentLevel, options)

						resolve(true)
					},
					undefined,
					undefined,
					this.navigationId)
			})
		},

		/**
		 * Deletes the current record.
		 */
		async deleteRecord()
		{
			// If is nested asks for confirmation.
			if (this.isNested)
			{
				const message = this.Resources[hardcodedTexts.confirmDelete]
				const buttons = {
					confirm: {
						label: this.Resources[hardcodedTexts.confirm],
						action: () => this.deleteRecordAfterConfirmation()
					},
					cancel: {
						label: this.Resources[hardcodedTexts.cancel]
					}
				}

				genericFunctions.displayMessage(message, 'warning', null, buttons)
			}
			else
				await this.deleteRecordAfterConfirmation()
		},

		/**
		 * Gets the form triggers that match the specified event type.
		 * @param {string} type The event type
		 * @returns A list of all the form triggers with the specified event type.
		 */
		getTriggers(type)
		{
			if (_isEmpty(this.triggers))
				return []

			return this.triggers.filter((t) => t.event === type && typeof t.execute === 'function')
		},

		/**
		 * Sets the value of the specified key in the navigation history entries.
		 * @param {object} modelField The key field
		 */
		setFormKey(modelField)
		{
			// Only adds the values of primary and foreign keys.
			if (!(modelField instanceof modelFieldType.PrimaryKey))
				return

			let areaId = modelField.area?.toLowerCase()
			if (modelField instanceof modelFieldType.ForeignKey)
				areaId = modelField.relatedArea?.toLowerCase()

			if (_isEmpty(areaId))
			{
				this.$eventTracker.addError({
					origin: 'setFormKey (formHandler)',
					message: 'Found an empty area identifier.'
				})
				return
			}

			// The FK must be null in the history because MVC has some validations in the application of limits that require null and not the empty string.
			// TODO: review MVC code
			const entry = {
				navigationId: this.navigationId,
				key: areaId,
				value: modelField.value === '' ? null : modelField.value
			}

			this.setEntryValue(entry)
		},

		/**
		 * Adds the form's keys to the entries of the navigation history.
		 */
		setFormKeys()
		{
			for (let key in this.dataApi.keys)
			{
				const modelField = this.dataApi.keys[key]
				this.setFormKey(modelField)
			}
		},

		/**
		 * Checks if the form has dirty fields, if so asks the user if he wants to proceed.
		 * @param {function} next A callback function
		 */
		handleDirtiness(next)
		{
			if (typeof next !== 'function')
				return

			if (this.isDirty || [this.formModes.new, this.formModes.duplicate].includes(this.formInfo.mode))
			{
				const buttons = {
					confirm: {
						label: this.Resources[hardcodedTexts.confirm],
						action: () => {
							genericFunctions.setNavigationState(false)
							next()
						}
					},
					cancel: {
						label: this.Resources[hardcodedTexts.cancel],
						action: () => next(false)
					}
				}

				genericFunctions.displayMessage(this.Resources[hardcodedTexts.isDirtyMessage], 'warning', null, buttons)
			}
			else
			{
				genericFunctions.setNavigationState(false)
				next()
			}
		},

		/**
		 * Changes the form mode to the specified mode.
		 * @param {string} mode The form mode
		 * @param {object} otherParams Other params to be included in the route (optional)
		 * @returns A promise to be resolved when the route changes.
		 */
		async setFormMode(mode, otherParams)
		{
			if (!this.hasRoute)
			{
				this.formInfo.mode = mode
				this.navigation.currentLevel.setMode(mode)
				this.$emit('update-form-mode', mode)

				// If the form is nested, instead of showing the usual warning, opens a popup to confirm the delete action.
				if (this.isNested && mode === this.formModes.delete)
				{
					this.changeToShowMode()
					this.deleteRecord()
				}

				return false
			}

			if (!otherParams)
				otherParams = {}

			const params = {
				mode,
				isControlled: false,
				...otherParams
			}

			try
			{
				const result = await this.navigateTo(this.navigation.currentLevel, params)

				if (!isNavigationFailure(result))
					this.model.resetValues()

				return result
			}
			catch (error)
			{
				this.$eventTracker.addError({
					origin: 'setFormKey (formHandler)',
					message: 'An error occurred while setting form mode:',
					contextData: { error }
				})
				throw error
			}
		},

		/**
		 * Changes the form mode to "Show" mode.
		 */
		changeToShowMode()
		{
			if ([this.formModes.show, this.formModes.new, this.formModes.duplicate].includes(this.formInfo.mode))
				return

			this.setFormMode(this.formModes.show)
		},

		/**
		 * Changes the form mode to "Edit" mode.
		 */
		changeToEditMode()
		{
			if ([this.formModes.edit, this.formModes.new, this.formModes.duplicate].includes(this.formInfo.mode))
				return

			this.setFormMode(this.formModes.edit)
		},

		/**
		 * Changes the form mode to "Duplicate" mode.
		 */
		changeToDupMode()
		{
			if ([this.formModes.duplicate, this.formModes.new].includes(this.formInfo.mode))
				return

			this.cancel(async () => {
				await this.setFormMode(this.formModes.duplicate, { isDuplicate: true, isControlled: true })

				this.resetContainersState()
				this.reloadFormData()
			})
		},

		/**
		 * Changes the form mode to "Delete" mode.
		 */
		changeToDeleteMode()
		{
			if ([this.formModes.delete, this.formModes.duplicate, this.formModes.new].includes(this.formInfo.mode))
				return

			this.setFormMode(this.formModes.delete)
		},

		/**
		 * Changes the form mode to "Insert" mode.
		 */
		changeToInsertMode()
		{
			if ([this.formModes.new, this.formModes.duplicate].includes(this.formInfo.mode))
				return

			this.cancel(async () => {
				await this.setFormMode(this.formModes.new, { id: null, isControlled: true })

				this.navigation.currentLevel.clearEntries()
				this.resetContainersState()
				this.reloadFormData()
			})
		},

		/**
		 * Sets the current control in the previous level (needed when the current record is changed from inside the form).
		 */
		setFormReturnControl()
		{
			let navLevel = this.navigation.previousLevel
			if (this.isNested)
				navLevel = this.navigation.currentLevel?.getFirstNotNested()

			if (_isEmpty(navLevel))
				return

			const currentControl = {
				id: navLevel.currentControl.id,
				data: {
					...navLevel.currentControl.data,
					rowKey: this.primaryKeyValue
				}
			}

			navLevel.setCurrentControl(currentControl, this.isNested)
		},

		/**
		 * Cancels the form submission and executes a follow-up action.
		 * @param {function} next A function to be executed after the cancel action
		 */
		cancel(next)
		{
			this.handleDirtiness(async (success) => {
				if (typeof success === 'boolean' && !success)
					return Promise.resolve(false) // If dirty handling was unsuccessful, abort leaving.

				const shouldLeave = await this.beforeExit()

				if (!shouldLeave)
					return Promise.resolve(false)

				// Local function to execute common form exit logic.
				const executeNextAndExit = async () => {
					this.clearInfoMessages()

					if (typeof next === 'function')
					{
						await next()
						this.afterExit()
					}
				}

				// Only New and Duplicate modes need to go to the server to remove the record from the database.
				if (![this.formModes.duplicate, this.formModes.new].includes(this.formInfo.mode))
				{
					await executeNextAndExit()
					return Promise.resolve(true)
				}

				/*
				 * TODO: In a situation where a form is open and the user logs out, Cancel will enter a loop.
				 * The server will return the redirect for PermissionError, but the 'beforeRouteLeave' will return to invoke Cancel.
				 */
				return new Promise((resolve) => {
					netAPI.postData(
						this.formInfo.area,
						`${this.formInfo.name}_Cancel`,
						null,
						async (_, response) => {
							// Show the warnings and errors.
							this.validationErrors = !_isEmpty(response.data.Errors)
								? this.parseResponseErrors(response.data.Errors)
								: {}

							if (!response.data.Success)
							{
								this.displayErrorMessage(response.data)
								resolve(false)
								return
							}

							await executeNextAndExit()
							resolve(true)
						},
						() => {
							this.validationErrors = {
								onCancelError: hardcodedTexts.errorProcessingRequest
							}
						},
						undefined,
						this.navigationId)
				})
			})
		},

		/**
		 * Leaves the form and goes to the previous page.
		 */
		leaveForm()
		{
			this.cancel(async () => {
				let previouslyRemovedRowKey = this.primaryKeyValue
				if (this.formInfo.mode === this.formModes.new && !_isEmpty(this.navigation.currentLevel?.params?.previouslyRemovedRowKey))
					previouslyRemovedRowKey = this.navigation.currentLevel.params.previouslyRemovedRowKey

				const options = {
					...this.getExtraNavParams(previouslyRemovedRowKey)
				}

				this.formRemoveHistoryLevels((this.currentRouteParams || {}).goBack)

				if (this.isNested)
					this.$emit('close', { type: 'cancel' })
				else
					await this.navigateTo(this.navigation.currentLevel, options)
			})
		},

		/**
		 * Executes the necessary procedures after the value of the specified field is updated.
		 * @param {string} fieldName The name of the field in the format [table].[field] (ex: 'person.name')
		 * @param {object} fieldObject The object representing the field in the model
		 */
		afterFieldUpdate(fieldName, fieldObject)
		{
			this.storeValue({
				navigationId: this.navigationId,
				key: this.storeKey,
				formInfo: this.formInfo,
				field: fieldObject
			})

			this.internalEvents.emit(`fieldChange:${fieldName}`, fieldObject)

			if (this.isNested)
			{
				this.$emit('is-form-dirty', { isDirty: this.isDirty, afterFormSave: false })
				this.$emit('update:nested-model', this.model)
			}
		},

		/**
		 * Executes the necessary procedures after the specified field is unfocused.
		 * @param {object} fieldObject The object representing the field in the model
		 * @param {any} fieldValue The value of the field
		 */
		afterFieldUnfocus(fieldObject, fieldValue)
		{
			// Only does the next lines of code if the form is nested.
			if (!this.isNested || this.nestedFormConfig.recordSelected || this.nestedFormConfig.prefillField !== fieldObject.modelField)
				return

			const area = this.nestedFormConfig.mainForm.area
			const action = `${area}_${this.nestedFormConfig.action}`
			const params = { queryParams: { searchFilter: fieldValue } }

			netAPI.postData(
				area,
				action,
				params,
				(data) => {
					if (data)
					{
						// There's a record already created.
						const eventData = {
							supportFormId: this.nestedFormConfig.supportFormId,
							formMode: this.formModes.edit,
							rowKey: data
						}

						this.$emit('update:modelValue', data)
						this.$emit('update:nested-model', eventData)
					}
					else
					{
						// There isn't a record already created.
						const buttons = {
							confirm: {
								label: this.Resources[hardcodedTexts.confirm],
								action: () => {
									const fieldRef = fieldObject.modelFieldRef
									const eventData = {
										supportFormId: this.nestedFormConfig.supportFormId,
										formMode: this.formModes.new,
										prefillValues: {
											[`${fieldRef.area}.${fieldRef.field}`.toLowerCase()]: fieldValue
										},
										rowKey: undefined
									}

									this.$emit('update:nested-model', eventData)
								}
							},
							cancel: {
								label: this.Resources[hardcodedTexts.cancel]
							}
						}
						genericFunctions.displayMessage(this.Resources[hardcodedTexts.notFoundCreateNew], 'question', null, buttons)
					}
				})
		},

		/**
		 * Called whenever a control's value is updated.
		 * @param {string} controlField The name of the field in the control that will be updated
		 * @param {any} fieldValue The value of the field
		 */
		afterControlUpdate(controlField, fieldValue)
		{
			this.internalEvents.emit(`controlChange:${controlField}`, fieldValue)
		},

		/**
		 * Highlights the control with the specified field id.
		 * @param {string} fieldId The id of the field
		 */
		focusField(fieldId)
		{
			if (!this.model[fieldId])
				return

			for (let i in this.controls)
			{
				// Note: The real field that has the field errors in the case of Lookup is the foreign key.
				if (this.controls[i].modelField !== fieldId && this.controls[i].lookupKeyModelField?.name !== fieldId)
					continue

				let controlId = this.controls[i].id

				// Will focus on the first visible control it finds that uses the specified model field.
				if (!formFunctions.fieldIsVisible(this.controls, controlId))
					continue

				this.focusControl(controlId, true)
				break
			}
		},

		/**
		 * Highlights the control with the specified control id.
		 * @param {string} controlId The id of the control
		 * @param {boolean} skipValidation If true, it won't check if the control can be made visible (optional)
		 * @param {string} position Determines if scroll keeps target element at the top of page or at the center (possible values: 'start', 'center')
		 */
		focusControl(controlId, skipValidation, position = 'center')
		{
			if (typeof skipValidation !== 'boolean')
				skipValidation = false

			if (!skipValidation && !formFunctions.fieldIsVisible(this.controls, controlId))
				return

			formFunctions.makeFieldVisible(this.controls, controlId, true)
			// Timeout to give time for the DOM to update (open the tabs and collapsible groups).
			setTimeout(() => genericFunctions.scrollTo(controlId, position), 500)
		},

		/**
		 * Sets the warning shown when deleting a record.
		 */
		setDeleteWarning()
		{
			const alertProps = {
				type: messageTypes.W,
				message: hardcodedTexts.confirmDelete,
				isDismissible: false,
				isResource: true
			}

			this.clearInfoMessages()
			this.setInfoMessage(alertProps)
		},

		/**
		 * Initializes the necessary properties as soon as the user enters the form.
		 * @param {object} routeData The data of the form route
		 * @returns True if the form was correctly initialized and the user has enough permissions, false otherwise.
		 */
		initFormProperties(routeData)
		{
			this.formInfo.route = routeData.name
			this.formInfo.identifier = `${this.formInfo.name}-${routeData.params.id}`

			if (routeData.params.mode === this.formModes.delete)
				this.setDeleteWarning()

			if (routeData.params.repeatInsert === 'true' && typeof this.formButtons.repeatInsertBtn !== 'undefined')
				this.formButtons.repeatInsertBtn.isActive = true

			if (typeof routeData.params.modes === 'string')
			{
				if (routeData.params.modes.includes('v'))
					this.formButtons.changeToShow.isActive = true
				if (routeData.params.modes.includes('e'))
					this.formButtons.changeToEdit.isActive = true
				if (routeData.params.modes.includes('d'))
					this.formButtons.changeToDuplicate.isActive = true
				if (routeData.params.modes.includes('a'))
					this.formButtons.changeToDelete.isActive = true
				if (routeData.params.modes.includes('i'))
					this.formButtons.changeToInsert.isActive = true
			}

			if (this.isNested && this.nestedFormConfig)
				this.formControl.setUIComponents(this.nestedFormConfig.uiComponents)

			if (this.isHomePage)
			{
				// Hide the interface elements that don't make sense on the home page.
				this.formControl.setUIComponents({
					header: false,
					headerButtons: false,
					footer: false
				})
			}

			if (this.isWidget)
			{
				this.formControl.setUIComponents({
					header: true,
					headerButtons: false,
					footer: false
				})
			}

			if (this.isPopup)
				this.setModalProperties({ isActive: true })

			if (!this.authData.isAllowed)
			{
				this.setModalProperties({ hideHeader: true })
				return false
			}

			return true
		},

		/**
		 * Inits the form and all it's controls.
		 * @param {boolean} refreshDom Whether or not to force a DOM update
		 */
		initFormControls(refreshDom)
		{
			this.formControl.init(false, this.isEditable)

			if (refreshDom)
				this.domVersionKey++

			// In some cases, this operation won't work as expected unless we force a DOM update before calling it.
			this.setContainersStateFromStore(this.storeKey)
			// Tabs need to be initialized only after setting the selected tab.
			this.formControl.initTabs()
		},

		/**
		 * If the form is a popup, sets it's properties.
		 * @param {object} props The modal properties
		 */
		setModalProperties(props)
		{
			if (!this.isPopup)
				return
			if (typeof props !== 'object')
				return

			const modalProps = {
				id: this.formInfo.route,
				...props
			}

			this.setModal(modalProps)
		},

		/**
		 * Builds the human key of the current record.
		 * @returns A string with the human key.
		 */
		buildHumanKey()
		{
			// Being a different route from the current one means the human key is probably already calculated, if not, it's because we have no way of calculating it.
			if (this.formInfo.route !== this.$route.name || this.isNested || this.isHomePage)
				return this.humanKey ?? ''

			const humanKeyFields = this.$route.meta.humanKeyFields
			return genericFunctions.buildHumanKey(humanKeyFields, this.model)
		},

		/**
		 * Sets the breadcrumbs properties in the global store.
		 */
		setBreadcrumbProperties()
		{
			if (this.isNested)
				return

			const navProps = {
				navigationId: this.navigationId,
				properties: {
					breadcrumbName: _isEmpty(this.formInfo.designation) ? '' : this.formInfo.designation,
					humanKey: this.humanKey
				}
			}
			this.setNavProperties(navProps)
		},

		/**
		 * As in nested forms, $route does not correspond to this form, we have to get data from props.
		 * @returns An object with the data in the params.
		 */
		getNestedRouteData()
		{
			if (!this.isNested)
				return {}

			return {
				name: this.formInfo.name,
				params: {
					id: this.id,
					mode: this.mode,
					modes: this.modes,
					isNested: true
				}
			}
		},

		/**
		 * Sets up the necessary properties for a nested form.
		 */
		setNestedRouteData()
		{
			if (!this.isNested)
				return

			const nestedRoute = this.getNestedRouteData()
			this.initFormProperties(nestedRoute)
		},

		/**
		 * Loads form data and initializes form controls.
		 * @param {boolean} isFirstLoad - Indicates whether it is the first load
		 * @returns {Promise<string>} A promise that resolves with a success message or rejects with an error message.
		 */
		loadFormData(isFirstLoad)
		{
			return new Promise((resolve, reject) => {
				// FOR: tree table select row on return.
				const currentLevel = this.navigation.currentLevel
				if (!_isEmpty(currentLevel))
				{
					const id = this.hasRoute ? (this.$route.params.id || null) : (this.isNested && this.mode === this.formModes.new ? null : this.id),
						removedKeyEntryName = `PreviouslyRemovedRowKey_${this.formArea.toLowerCase()}`

					// If ID is the ID of the record that was deleted, don't try to load
					if (currentLevel.hasEntry(removedKeyEntryName) && id === currentLevel?.previousLevel?.entries[removedKeyEntryName])
					{
						if (this.mode === this.formModes.new)
							currentLevel.previousLevel.removeEntryValue(removedKeyEntryName)

						resolve('Record was deleted, load not necessary.')
						return
					}
				}

				// Load form data.
				this.addBusy(this.fetchFormFields(!isFirstLoad), this.Resources[hardcodedTexts.formLoad])

				// Only after the data is loaded from the server, init all controls.
				this.componentOnLoadProc.once(() => {
					const route = this.isNested ? this.getNestedRouteData() : this.$route
					const formInited = isFirstLoad ? this.initFormProperties(route) : true

					if (!formInited)
					{
						reject('Form initialization failed.')
						return
					}

					this.formHeader = this.$refs.formHeader

					// Destroy any running periodic events.
					if (!isFirstLoad)
						this.formControl.destroyTriggers()

					// Init form, forcing a DOM update if it's not the first load.
					this.initFormControls(!isFirstLoad)

					// Load Table List controls data.
					this.refreshAllListsData(isFirstLoad)
						.then(() => {
							if (!this.isNested)
							{
								this.internalEvents.emit('form-buttons-change', this.formButtons)
								this.setBreadcrumbProperties()

								this.$nextTick().then(() => {
									// If the user is navigating from a record below, scrolls the page to the respective table.
									const anchor = this.$route.params.anchor
									if (!_isEmpty(anchor))
										this.focusControl(anchor)
								})
							}

							resolve('Form data loaded successfully.')
						})
						.catch((error) => reject(`Error refreshing lists data: ${error}`))
				}, this)
			})
		},

		/**
		 * Reloads all form data, forcing a DOM update.
		 */
		async reloadFormData()
		{
			await this.loadFormData(false)
		},

		/**
		 * Builds a tree of the anchors of the current form.
		 * @returns A tree of anchors.
		 */
		buildFormAnchors()
		{
			const toExpand = []
			const anchorsTree = []

			if (!this.groupFields)
				return []

			toExpand.push(...this.groupFields.map((ctrlId) => this.controls[ctrlId]))

			toExpand.forEach((el) => {
				if (el.anchored && !el.isCollapsible && !_isEmpty(el.label) && formFunctions.fieldIsVisible(this.controls, el.id, false))
				{
					const parent = this.getLastAnchoredParent(el)
					if (parent === null)
						anchorsTree.push(el)
					else
						parent.anchoredChildren.push(el)
				}
			})

			return anchorsTree
		},

		/**
		 * Gets the last anchored parent of the specified control.
		 * @param {object} ctrl The control
		 * @returns The last anchored parent, or null if none exists.
		 */
		getLastAnchoredParent(ctrl)
		{
			const parent = ctrl.parent
			if (_isEmpty(parent))
				return null
			if (this.controls[parent].anchored)
				return this.controls[ctrl.parent]
			return this.getLastAnchoredParent(this.controls[ctrl.parent])
		},

		/**
		 * Toggle visibility for anchor container.
		 */
		toggleAnchorVisibility()
		{
			this.anchorContainerVisibility = !this.anchorContainerVisibility
		},

		/**
		 * Gets a model field by it's name.
		 * @param {string} fieldName The name of the field
		 * @returns The model field.
		 */
		getModelField(fieldName)
		{
			return this.model[fieldName]
		},

		/**
		 * Gets the value of a model field by it's name.
		 * @param {string} fieldName The name of the field
		 * @returns The model field's value.
		 */
		getModelFieldValue(fieldName)
		{
			return this.getModelField(fieldName)?.value
		},

		/**
		 * Sets the value of a model field by it's name.
		 * @param {string} fieldName The name of the field
		 * @param {any} value The value to set
		 */
		setModelFieldValue(fieldName, value)
		{
			this.getModelField(fieldName)?.updateValue(value)
		},

		/**
		 * Checks if the section with the provided section id should be visible.
		 * @param {string} sectionId The identifier of the form header button section
		 * @returns True if the section with the provided section id should be visible, false otherwise.
		 */
		showFormHeaderSection(sectionId)
		{
			const section = this.formButtonSections[sectionId]
			return Object.values(section).some((btn) => this.showFormHeaderButton(btn))
		},

		/**
		 * Checks if the provided form header section should be prepended by a separator.
		 * @param {string} sectionId The identifier of the form header button section
		 * @returns True if the provided form header section should be prepended by a separator, false otherwise.
		 */
		showHeadingSep(sectionId)
		{
			// Should not show the separator if it's the first visible section.
			if (!this.formControl.uiComponents.headerButtons ||
				sectionId === this.firstVisibleButtonSection)
				return false

			// Should be prepended with a separator
			// if the section has at least one visible button.
			return this.showFormHeaderSection(sectionId)
		},

		/**
		 * Checks if the provided form header button should be visible.
		 * @param {object} btn The button to check
		 * @returns True if the provided form header button should be visible, false otherwise.
		 */
		showFormHeaderButton(btn)
		{
			return btn && btn.isActive && btn.isVisible && btn.showInHeader
		},

		/**
		 * Wraps the function that sets the form as busy.
		 * Blocks the entire screen if the form is not a widget.
		 * @param {promise} cbPromise The «Promise» object of the process
		 * @param {number} busyStateMessage The busy state message to display
		 */
		addBusy(cbPromise, busyStateMessage)
		{
			this.isWidget
				? this.componentOnLoadProc.addWL(cbPromise)
				: this.componentOnLoadProc.addBusy(cbPromise, busyStateMessage, 300)
		}
	},

	watch: {
		$route(to, from)
		{
			if (this.isNested)
				return

			this.validationErrors = {}

			// Needs to wait for the next tick to ensure it's only executed after the callback functions.
			if (from.name !== this.formInfo.route)
				this.$nextTick().then(() => this.setBreadcrumbProperties())

			// For the form that was closed, don't write their keys on the previous level.
			if (to.params.previouslyRemovedRoute !== from.name)
				this.$nextTick().then(() => this.setFormKeys())

			if (to.params.mode && this.formInfo.mode !== to.params.mode)
			{
				if (to.params.keepAlerts !== 'true')
					this.clearInfoMessages()

				if (to.name === this.formInfo.route)
				{
					// We need to ensure the navigation has the correct form mode,
					// it might not if the user directly changed the url.
					this.setParamValue({ navigationId: this.navigationId, key: 'mode', value: to.params.mode })

					if (to.name === from.name)
						this.formInfo.mode = to.params.mode

					if (this.formInfo.mode === this.formModes.delete)
						this.setDeleteWarning()
				}
			}

			// In case the route params change without leaving the form.
			if (this.componentOnLoadProc.loaded && to.name === this.formInfo.route &&
				(to.params.id && this.primaryKeyValue !== to.params.id || !this.authData.isAllowed || to.name !== from.name))
				this.reloadFormData()
		},

		formButtons: {
			handler(newVal)
			{
				if (!this.isNested)
					this.internalEvents.emit('form-buttons-change', newVal)
			},
			deep: true
		},

		formAnchors: {
			handler(newVal)
			{
				// Anchors should open on first form load.
				const open = !this.anchorsTabOpened && !this.isAnchorsButtonVisible && !_isEmpty(newVal) && !this.isPopup
				this.formControl.setFormAnchors(newVal, open)

				// anchorsTabOpened should be true after the first time it's opened.
				this.anchorsTabOpened = !this.anchorsTabOpened && open
			},
			deep: true,
			immediate: true
		},

		mode(val)
		{
			this.formInfo.mode = val
		},

		isEditable(val)
		{
			if (typeof this.formControl?.setFormModeBlockAndVisibility === 'function')
				this.formControl.setFormModeBlockAndVisibility(val)
		},

		isDirty(val)
		{
			const isInsertion = [this.formModes.new, this.formModes.duplicate].includes(this.formInfo.mode)
			genericFunctions.setNavigationState(val || isInsertion)
		},

		showFormHeader(val)
		{
			this.setModalProperties({ hideHeader: !val })
		},

		humanKey()
		{
			this.$nextTick().then(() => this.setBreadcrumbProperties())
		},

		'formInfo.designation'()
		{
			this.$nextTick().then(() => this.setBreadcrumbProperties())
		},

		'formInfo.mode'(val)
		{
			this.$nextTick().then(() => this.setBreadcrumbProperties())
			this.internalEvents.emit('form-mode-change', val)
		},

		'system.currentLang'(val)
		{
			this.internalEvents.emit('language-change', val)
		}
	}
}
