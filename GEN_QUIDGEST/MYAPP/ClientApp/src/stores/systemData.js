/*****************************************************************
 *                                                               *
 * This store holds generated data. Most of that data should     *
 * only be accessed, never mutated.                              *
 *                                                               *
 *****************************************************************/

import { defineStore } from 'pinia'

//----------------------------------------------------------------
// State variables
//----------------------------------------------------------------

const state = () => {
	return {
		applicationName: 'My application',

		genio: {
			buildVersion: 24,
			dbIdxVersion: 41,
			dbVersion: '2541',
			genioVersion: '349,60',
			trackChangesVersion: '0',
			assemblyVersion: '349,60.2541.0.24',
			generationDate: {
				year: 2024,
				month: 9,
				day: 26
			}
		},

		system: {
			acronym: 'QUIDGEST',
			name: 'Quidgest',
			defaultSystem: '',
			currentSystem: '',
			availableSystems: [],
			defaultLang: 'pt-PT',
			currentLang: 'pt-PT',
			supportedLangs: [
				{
					language: 'pt-PT',
					acronym: 'PT',
					languageName: 'Português'
				},
			],
			defaultModule: 'Public',
			currentModule: 'Public',
			availableModules: {},
			defaultListRows: 0,
			numberFormat: {
				decimalSeparator: ',',
				thousandsSeparator: ' '
			},
			dateFormat: {
				date: 'dd/MM/yyyy',
				dateTime: 'dd/MM/yyyy HH:mm',
				dateTimeSeconds: 'dd/MM/yyyy HH:mm:ss',
				time: 'HH:mm'
			},
			baseCurrency: {
				symbol: '€',
				code: 'EUR',
				precision: 2
			},
			cookies: {
				cookieText: '',
				cookieActive: false,
				filePath: '',
				shouldShowCookies: true,
			},
			resourcesPath: 'Content/img/',
			schedulerLicense: undefined
		},

		authConfig: {
			useCertificate: false,
			maxUsrSize: 100,
			maxPswSize: 150
		},

		isCavAvailable: false,

		isChatBotAvailable: false,

		isSuggestionsAvailable: true,

		appAlerts: [
		],

		userRegistration: {
			allowRegistration: false,
			registrationTypes: [
			]
		}
	}
}

//----------------------------------------------------------------
// Actions
//----------------------------------------------------------------

const actions = {
	/**
	 * Sets the available systems.
	 * @param {string} availableSystems The available systems
	 */
	setAvailableSystems(availableSystems)
	{
		if (Array.isArray(availableSystems) === false)
			return
		if (this.system.availableSystems === availableSystems)
			return

		this.system.availableSystems = availableSystems
	},

	/**
	 * Sets the default system.
	 * @param {string} defaultSystem The default system
	 */
	setDefaultSystem(defaultSystem)
	{
		if (typeof defaultSystem !== 'string' || defaultSystem.length === 0)
			return
		if (this.system.defaultSystem === defaultSystem)
			return

		this.system.defaultSystem = defaultSystem
	},

	/**
	 * Sets the currently selected system.
	 * @param {string} currentSystem The current system
	 */
	setCurrentSystem(currentSystem)
	{
		if (typeof currentSystem !== 'string' || currentSystem.length === 0)
			return
		if (this.system.currentSystem === currentSystem)
			return
		if (!this.system.availableSystems.includes(currentSystem))
			return

		this.system.currentSystem = currentSystem
	},

	/**
	 * Sets the available modules.
	 * @param {object} availableModules The available modules
	 */
	setAvailableModules(availableModules)
	{
		if (typeof availableModules !== 'object' || availableModules === null)
			return

		this.system.availableModules = availableModules
	},

	/**
	 * Sets the default module.
	 * @param {string} module The default module
	 */
	setDefaultModule(module)
	{
		if (typeof module !== 'string' || module.length === 0)
			return
		if (this.system.defaultModule === module)
			return
		if (!this.system.availableModules[module] && module !== 'Public')
			return

		this.system.defaultModule = module
	},

	/**
	 * Sets the currently selected module.
	 * @param {string} module The current module
	 */
	setCurrentModule(module)
	{
		if (typeof module !== 'string' || module.length === 0)
			return
		if (this.system.currentModule === module)
			return
		if (this.system.availableModules[module] === undefined && module !== 'Public')
			return

		this.system.currentModule = module
	},

	/**
	 * Sets the currently selected language.
	 * @param {string} lang The current language
	 */
	setCurrentLang(lang)
	{
		if (typeof lang !== 'string' || lang.length === 0)
			return
		if (this.system.currentLang === lang)
			return
		if (!this.system.supportedLangs.find(obj => obj.language === lang))
			return

		this.system.currentLang = lang
	},

	/**
	 * Sets the default number of rows for lists.
	 * @param {number} rowsNum The number of rows
	 */
	setDefaultListRows(rowsNum)
	{
		if (typeof rowsNum !== 'number')
			return
		if (this.system.defaultListRows === rowsNum)
			return

		this.system.defaultListRows = rowsNum
	},
	
	/**
	 * Sets the format used by numeric inputs in the application.
	 * @param {object} numberFormat The formats of the numbers
	 */
	setNumberFormat(numberFormat)
	{
		if (typeof numberFormat !== 'object' || numberFormat === null)
			return

		this.system.numberFormat.decimalSeparator = numberFormat.DecimalSeparator ?? ','
		this.system.numberFormat.thousandsSeparator = numberFormat.GroupSeparator ?? ' '
	},

	/**
	 * Sets the format used by date inputs in the application.
	 * @param {object} dateFormat The formats of the dates
	 */
	setDateFormat(dateFormat)
	{
		if (typeof dateFormat !== 'object' || dateFormat === null)
			return
		if (!dateFormat.date && !dateFormat.dateTime && !dateFormat.dateTimeSeconds && !dateFormat.time)
			return

		for (let i in dateFormat)
		{
			// Get property name starting with lowercase letter
			let propName = i.substring(0, 1).toLowerCase() + i.substring(1)
			this.system.dateFormat[propName] = dateFormat[i]
		}
	},

	/**
	 * Sets the scheduler license key to use premium features of the Full Calendar.
	 * @param {string} schedulerLicenseKey The the license key
	 */
	setSchedulerLicenseKey(schedulerLicenseKey)
	{
		if (typeof schedulerLicenseKey !== 'string')
			return

		this.system.schedulerLicense = schedulerLicenseKey
	},

	setShowCookies(newVal){
		if (typeof newVal !== 'boolean')
			return
		this.system.cookies.shouldShowCookies = newVal
	},

	/**
	 * Resets the system data.
	 */
	resetStore()
	{
		Object.assign(this, state())
	}
}

//----------------------------------------------------------------
// Store export
//----------------------------------------------------------------

export const useSystemDataStore = defineStore('systemData', {
	state,
	actions
})
