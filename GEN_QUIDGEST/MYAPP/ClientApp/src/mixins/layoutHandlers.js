import { mapState, mapActions } from 'pinia'

import { useLayoutDataStore } from '@/stores/layoutData.js'

import GenericLayoutHandlers from '@/mixins/genericLayoutHandlers.js'

/***************************************************************************
 * Mixin with handlers to be reused by the vertical layout components.     *
 ***************************************************************************/
export default {
	mixins: [
		GenericLayoutHandlers
	],

	computed: {
		...mapState(useLayoutDataStore, [
			'layoutType',
			'sidebarIsCollapsed',
			'sidebarIsVisible',
			'navBarIsVisible',
			'stickyThreshold',
			'isAccordionMenu'
		]),

		/**
		 * The visible vertical space (in pixels) occupied by this layout's header.
		 */
		visibleHeaderHeight()
		{
			return this.headerHeight
		},

		/**
		 * Gets the current module icon.
		 */
		currentModuleIcon()
		{
			const currentModId = this.system.currentModule
			const module = this.system.availableModules[currentModId]
			const data = {
				icon: 'menu-hamburger',
				type: 'svg'
			}

			if (module)
				return this.getModuleIconProps(module, data.icon)
			return data
		}
	},

	methods: {
		...mapActions(useLayoutDataStore, [
			'setSidebarCollapseState',
			'setSidebarVisibility',
			'setNavBarVisibility'
		]),

		/**
		 * Expands the sidebar.
		 */
		expandSidebar()
		{
			this.setSidebarVisibility(true)
			this.setSidebarCollapseState(false)
			this.$eventHub.emit('toggle-sidebar', 'expand')
		},

		/**
		 * Collapses the sidebar.
		 */
		collapseSidebar()
		{
			if (this.options.autoCollapseSize && window.innerWidth <= this.options.autoCollapseSize)
				this.setSidebarVisibility(false)

			this.setSidebarCollapseState(true)
			this.$eventHub.emit('toggle-sidebar', 'collapse')
		},

		/**
		 * Collapses the sidebar when a certain screen size is reached.
		 * @param {boolean} resize Whether or not the window is being resized
		 */
		autoCollapseSidebar(resize = true)
		{
			if (resize && !this.options.autoCollapseSize)
				return

			if (this.options.autoCollapseSize && window.innerWidth <= this.options.autoCollapseSize)
				this.collapseSidebar()
			else
				this.setSidebarVisibility(true)
		},

		/**
		 * Toggles the sidebar.
		 */
		toggleSidebar()
		{
			if (this.sidebarIsCollapsed)
				this.expandSidebar()
			else
				this.collapseSidebar()
		},

		/**
		 * Expands a dropdown menu.
		 * @param {object} menu The menu
		 */
		expandDropdownMenu(menu)
		{
			// If the menu is an accordion, removes all entries outside the current branch.
			if (this.isAccordionMenu)
			{
				let otherMenus = []
				for (let menuId of this.menuPath)
					if (!menu.Order.startsWith(menuId))
						otherMenus.push(menuId)

				for (let menuId of otherMenus)
					this.removeFromMenuPath(menuId)

				this.removeFromMenuPath(menu.Order)
			}

			this.addToMenuPath(menu.Order)
		},

		/**
		 * Collapses a dropdown menu.
		 * @param {object} menu The menu
		 */
		collapseDropdownMenu(menu)
		{
			this.removeFromMenuPath(menu.Order)
		},

		/**
		 * Toggles a dropdown menu.
		 * @param {object} menu The menu
		 */
		toggleDropdownMenu(menu)
		{
			if (this.menuIsOpen(menu))
				this.collapseDropdownMenu(menu)
			else
				this.expandDropdownMenu(menu)
		},

		/**
		 * Sets the necessary listeners to control the state of the layout.
		 */
		setListeners()
		{
			window.addEventListener('scroll', this.updatePageScroll)
			if (this.options.autoCollapseSize)
				window.addEventListener('resize', this.autoCollapseSidebar)
		},

		/**
		 * Removes all the listeners.
		 */
		removeListeners()
		{
			window.removeEventListener('scroll', this.updatePageScroll)
			if (this.options.autoCollapseSize)
				window.removeEventListener('resize', this.autoCollapseSidebar)
		},

		/**
		 * Gets the icon for the given module.
		 * @param {object} module The module
		 * @param {string} defaultIcon The default icon
		 * @returns The icon properties.
		 */
		getModuleIconProps(module, defaultIcon = '')
		{
			const data = {
				icon: defaultIcon,
				type: 'svg'
			}

			if (module.vector)
				data.icon = module.vector
			else if (module.font)
			{
				data.icon = module.font
				data.type = 'font'
			}
			else if (module.image)
			{
				data.icon = module.image
				data.type = 'img'
			}
			else if (this.isEmpty(defaultIcon))
				return undefined

			return data
		}
	}
}
