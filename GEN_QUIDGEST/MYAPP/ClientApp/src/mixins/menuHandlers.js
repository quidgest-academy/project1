import hardcodedTexts from '@/hardcodedTexts.js'
import { loadResources } from '@/plugins/i18n.js'

import { messageTypes } from './quidgest.mainEnums.js'
import genericFunctions from './genericFunctions.js'
import GenericMenuHandlers from './genericMenuHandlers.js'

/***********************************************************************
 * This mixin defines methods to be reused in regular menu components. *
 ***********************************************************************/
export default {
	mixins: [
		GenericMenuHandlers
	],

	created()
	{
		this.componentOnLoadProc.addBusy(loadResources(this, this.interfaceMetadata.requiredTextResources), this.Resources[hardcodedTexts.genericLoad], 300)
		this.controls.menu.init()
		this.componentOnLoadProc.addWL(this.loadList())
		this.componentOnLoadProc.once(() => {
			this.setMenuNavProperties()
			this.controls.menu.initData()
		}, this)
	},

	mounted()
	{
		this.$eventTracker.addTrace({ origin: 'mounted (menuHandler)', message: 'Menu is mounted', contextData: { menuInfo: this.menuInfo } })

		// Listens for changes to the DB and updates the list accordingly.
		this.$eventHub.onMany(this.controls.menu.changeEvents, this.loadList)
	},

	beforeUnmount()
	{
		this.$eventTracker.addTrace({ origin: 'beforeUnmount (menuHandler)', message: 'Menu will be unmounted', contextData: { menuInfo: this.menuInfo } })
		// Removes the listeners.
		this.$eventHub.offMany(this.controls.menu.changeEvents, this.loadList)
	},

	computed: {
		/**
		 * True if there are invalid rows, false otherwise.
		 */
		hasInvalidRows()
		{
			if (!Array.isArray(this.controls?.menu?.rows))
				return false
			return this.controls.menu.rows.filter((row) => !this.controls.menu.config.rowValidation.fnValidate(row)).length !== 0
		}
	},

	methods: {
		onBeforeRouteLeave(to, next)
		{
			const buttons = {
				confirm: {
					label: this.Resources[hardcodedTexts.save],
					action: () => {
						if (this.isEmpty(this.controls.menu.config.UserTableConfigName))
						{
							this.controls.menu.subSignals.viewSave = { show: true, routeTo: to }
							this.controls.menu.confirmChanges = false
							next(false)
						}
						else
						{
							this.controls.menu.signal = { saveCurrentView: true }
							this.controls.menu.confirmChanges = false
							genericFunctions.setNavigationState(false)
							next()
						}
					}
				},
				cancel: {
					label: this.Resources[hardcodedTexts.discard],
					action: () => {
						this.controls.menu.confirmChanges = false
						genericFunctions.setNavigationState(false)
						next()
					}
				}
			}

			if (this.controls.menu.config.allowManageViews && this.controls.menu.confirmChanges)
				genericFunctions.displayMessage(this.Resources[hardcodedTexts.tableViewConfirmSaveChanges], 'warning', null, buttons)
			else
			{
				genericFunctions.setNavigationState(false)
				next()
			}
		},

		/**
		 * Fetches the data of the menu list from the server.
		 * @returns A promise to be resolved after the request completes.
		 */
		async loadList()
		{
			return this.controls.menu.reload()
		},

		/**
		 * Sets the menu's table name in the nav properties.
		 */
		setMenuNavProperties()
		{
			const tableName = this.menuInfo.designation
			const navProps = {
				navigationId: this.navigationId,
				properties: {
					tableName: tableName
				}
			}
			this.setNavProperties(navProps)
		}
	},

	watch: {
		'menuInfo.designation'()
		{
			this.setMenuNavProperties()
		},

		hasInvalidRows(val)
		{
			// If there are invalid rows, shows a warning message.
			if (val)
			{
				const warningProps = {
					type: messageTypes.W,
					message: hardcodedTexts.invalidRowsMsg,
					icon: 'error',
					dismissTime: 0,
					isResource: true
				}
				this.setInfoMessage(warningProps)
			}
		}
	}
}
