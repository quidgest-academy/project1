import { mapState, mapActions } from 'pinia'
import _isEmpty from 'lodash-es/isEmpty'

import { useGenericLayoutDataStore } from '@/stores/genericLayoutData.js'
import { useGenericDataStore } from '@/stores/genericData.js'
import { useSystemDataStore } from '@/stores/systemData.js'
import { useUserDataStore } from '@/stores/userData.js'

import { loadResources } from '@/plugins/i18n.js'

/***************************************************************************
 * Mixin with handlers to be reused by both layouts.                       *
 ***************************************************************************/
export default {
	created()
	{
		loadResources(this, ['QLayout'])
	},

	data()
	{
		return {
			config: {
				QAEnvironment: 0,
				LoginType: 'NORMAL'
			},

			options: {
				autoCollapseSize: 992
			}
		}
	},

	computed: {
		...mapState(useSystemDataStore, [
			'system'
		]),

		...mapState(useGenericLayoutDataStore, [
			'headerHeight',
			'layoutConfig',
			'pageScroll',
			'progressBar',
			'bookmarkMenuIsOpen',
			'moduleMenuIsOpen',
			'rightSidebarIsCollapsed',
			'rightSidebarIsVisible'
		]),

		...mapState(useGenericDataStore, [
			'hasMenus',
			'menus',
			'menuPath',
			'isPublicRoute',
			'isFullScreenPage'
		]),

		...mapState(useUserDataStore, [
			'userIsLoggedIn'
		]),

		/**
		 * The title of the selected module.
		 */
		currentModuleTitle()
		{
			const currentModId = this.system.currentModule

			if (this.system.availableModules[currentModId])
			{
				const moduleNameId = this.system.availableModules[currentModId].title
				return this.Resources[moduleNameId]
			}

			return currentModId
		},

		/**
		 * The data of the current user.
		 */
		userData()
		{
			const userDataStore = useUserDataStore()

			return {
				name: userDataStore.username
			}
		},

		/**
		 * The margin the main content gets from the sides of the page.
		 */
		containerClasses()
		{
			return this.layoutConfig.ContainerWidth === 'reduced' ? 'container' : 'container-fluid'
		},

		authenticationClasses()
		{
			if (this.layoutConfig.AuthenticationStyle === 'default')
				return []

			return [`layout-${this.layoutConfig.AuthenticationStyle}`]
		}
	},

	methods: {
		...mapActions(useGenericLayoutDataStore, [
			'setLayoutConfig',
			'setHeaderHeight',
			'setBookmarkMenuState',
			'setModuleMenuState',
			'setRightSidebarCollapseState',
			'setRightSidebarVisibility',
			'setPageScroll'
		]),

		...mapActions(useGenericDataStore, [
			'addToMenuPath',
			'removeFromMenuPath',
			'clearMenuPath'
		]),

		isEmpty: _isEmpty,

		/**
		 * Sets the current page scroll.
		 */
		updatePageScroll()
		{
			this.setPageScroll(window.scrollY)
		},

		/**
		 * Sets the current menu path, according to the specified menu id.
		 * @param {string} menuId The id of the menu
		 */
		setMenuPath(menuId)
		{
			if (typeof menuId !== 'string')
				return

			this.clearMenuPath()

			for (let i = 0; i < menuId.length; i++)
			{
				const id = menuId.substring(0, i + 1)
				this.addToMenuPath(id)
			}
		},

		/**
		 * Checks if the specified dropdown menu is open.
		 * @param {object} menu The menu to check
		 * @returns True if the dropdown menu is open, false otherwise.
		 */
		menuIsOpen(menu)
		{
			return this.menuPath.includes(menu.Order)
		},

		/**
		 * Gets the icon for the given menu.
		 * @param {object} menuEntry The menu to check the icon
		 * @returns An object with the icon and the type.
		 */
		getMenuIcon(menuEntry)
		{
			const data = {
				icon: '',
				type: 'svg'
			}

			if (menuEntry.Vector)
				data.icon = menuEntry.Vector
			else if (menuEntry.Font)
			{
				data.icon = menuEntry.Font
				data.type = 'font'
			}
			else if (menuEntry.Image)
			{
				data.icon = menuEntry.Image
				data.type = 'img'
			}
			else
				return undefined

			return data
		},

		/**
		 * Toggles the bookmarks menu.
		 */
		toggleBookmarksMenu()
		{
			this.setBookmarkMenuState(!this.bookmarkMenuIsOpen)
		},

		/**
		 * Toggles the modules menu.
		 */
		toggleModulesMenu()
		{
			this.setModuleMenuState(!this.moduleMenuIsOpen)
		}
	}
}
