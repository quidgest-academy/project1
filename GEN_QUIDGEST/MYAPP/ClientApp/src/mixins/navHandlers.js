import { mapState, mapActions } from 'pinia'
import { v4 as uuidv4 } from 'uuid'
import _isEmpty from 'lodash-es/isEmpty'
import _merge from 'lodash-es/merge'

import { useSystemDataStore } from '@/stores/systemData.js'
import { useGenericDataStore } from '@/stores/genericData.js'
import { useUserDataStore } from '@/stores/userData.js'
import { useNavDataStore } from '@/stores/navData.js'

import { NetworkAPI } from '@/api/network'
import eventBus from '@/api/global/eventBus.js'
import genericFunctions from '@/mixins/genericFunctions.js'

const MAIN_HISTORY_BRANCH_ID = 'main'

export const navigationProperties = {
	data()
	{
		return {
			netAPI: new NetworkAPI(this.navigationId)
		}
	},

	computed: {
		...mapState(useUserDataStore, [
			'valuesOfPHEs'
		]),

		/**
		 * The current navigation.
		 */
		navigation()
		{
			const navDataStore = useNavDataStore()
			return navDataStore.navigation.getHistory(this.navigationId)
		},

		/**
		 * The previous navigation.
		 */
		previousNav()
		{
			const navDataStore = useNavDataStore()
			const previousNav = navDataStore.previousNav

			return previousNav !== null ? previousNav.getHistory() : null
		},

		/**
		 * The containers state in the store, based on the current navigation ID.
		 */
		containersState()
		{
			return this.getContainersState(this.navigationId)
		},

		/**
		 * The current control in the store, based on the current navigation ID.
		 */
		currentControl()
		{
			return this.getCurrentControl(this.navigationId)
		}
	},

	methods: {
		...mapActions(useNavDataStore, [
			'getContainersState',
			'getCurrentControl',
			'storeContainerState',
			'setCurrentControl',
			'removeCurrentControl'
		])
	},

	watch: {
		'navigationId'(newValue)
		{
			this.netAPI = new NetworkAPI(newValue)
		}
	}
}

/*****************************************************************
 * This mixin should be used by all forms and menus, in order to *
 * ensure that a new navigation level exists before making any   *
 * server requests.                                              *
 *****************************************************************/
export default {
	mixins: [navigationProperties],

	beforeCreate()
	{
		const systemDataStore = useSystemDataStore()
		const genericDataStore = useGenericDataStore()
		const navDataStore = useNavDataStore()

		let historyBranchId = MAIN_HISTORY_BRANCH_ID

		const isWizard = this.$route.meta.isWizardStep

		if (this.isNested || isWizard)
		{
			if (!_isEmpty(this.historyBranchId))
				historyBranchId = this.historyBranchId

			let branchExist = navDataStore.navigation.history.has(historyBranchId),
				nestedData = _merge({
					isNested: true,
					params: {
						id: this.id,
						mode: this.mode,
						modes: this.modes,
						culture: systemDataStore.system.currentLang,
						system: systemDataStore.system.currentSystem,
						module: systemDataStore.system.currentModule
					}
				}, this.nestedRouteParams),
				wizardRoute = {
					location: this.$route.name,
					params: this.$route.params
				}

			if (branchExist)
			{
				let newId = isWizard && !this.isNested ? historyBranchId : uuidv4()
				navDataStore.beforeRequestContext(newId)

				let newBranch = navDataStore.navigation.getHistory(newId),
					parentNavigationContext = navDataStore.navigation.getHistory(historyBranchId),
					historyLevel = {
						navigationId: newBranch.navigationId,
						options: isWizard ? wizardRoute : nestedData,
						previousLevel: parentNavigationContext.currentLevel
					}

				navDataStore.addHistoryLevel(historyLevel)
				historyBranchId = newBranch.navigationId
			}
			else
			{
				historyBranchId = uuidv4()
				navDataStore.beforeRequestContext(historyBranchId)

				let newBranch = navDataStore.navigation.getHistory(historyBranchId),
					historyLevel = {
						navigationId: newBranch.navigationId,
						options: isWizard ? wizardRoute : nestedData
					}

				navDataStore.addHistoryLevel(historyLevel)
			}
		}
		else if (typeof this.$route.name === 'string')
		{
			// If it's a form and the "modes" aren't passed to it, it means this isn't a normal route change.
			// The route change was probably triggered by the user inserting a new url in the address bar,
			// therefore, we clear the navigation and current menu path.
			if (this.$route.meta.routeType === 'form' && typeof this.$route.params.modes !== 'string' && this.$route.params.isControlled !== 'true')
			{
				genericDataStore.clearMenuPath()
				navDataStore.clearHistory()
			}

			historyBranchId = this.$route.params.historyBranchId || MAIN_HISTORY_BRANCH_ID
			navDataStore.beforeRequestContext(historyBranchId)

			let menuOrder = ((this.$route || {}).meta || {}).order,
				navigation = navDataStore.navigation.getHistory(historyBranchId)

			if (!_isEmpty(menuOrder))
			{
				// If the current menu belongs to a different tree from the ones on the navigation history, we clear the history.
				let clearNav = false

				for (let historyLevel of navigation.convertToCollection())
				{
					if (_isEmpty(historyLevel.properties.routeBranch) || menuOrder.startsWith(historyLevel.properties.routeBranch))
						continue

					clearNav = true
					break
				}

				if (clearNav)
				{
					const historyData = {
						location: `home-${systemDataStore.system.currentModule}`,
						params: {
							...this.$route.params,
							isPopup: 'false'
						}
					}

					if (historyBranchId === MAIN_HISTORY_BRANCH_ID)
						navDataStore.clearHistory()
					navDataStore.addHistoryLevel({ navigationId: historyBranchId, options: historyData })
				}
			}

			let options = genericFunctions.normalizeRouteForSaveNavigation(this.$route)
			navDataStore.addHistoryLevel({ navigationId: historyBranchId, options })
		}

		if (!this.isNested)
		{
			eventBus.emit('navigation-id-change', {
				navigationId: historyBranchId
			})
		}

		this.navigationId = historyBranchId
	},

	methods: {
		...mapActions(useNavDataStore, [
			'addHistoryLevel',
			'removeHistoryLevel',
			'removeHistoryLevels',
			'removeNavigationLevelsUpTo',
			'clearHistory',
			'setParamValue',
			'removeParamValue',
			'setEntryValue',
			'removeEntryValue',
			'clearEntries',
			'setNavProperties',
			'removeNavProperty',
			'retrievePreviousNav'
		]),

		/**
		 * Goes back to the previous navigation level, if it exists.
		 */
		goBack()
		{
			genericFunctions.goBack(this.navigationId, this.$route.meta.hasInitialPHE)
		}
	}
}
