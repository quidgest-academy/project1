/*****************************************************************
 *                                                               *
 * This store holds data generic for all layout types,           *
 * also defining functions to access and mutate it.              *
 *                                                               *
 *****************************************************************/

import { defineStore } from 'pinia'

import { useGenericDataStore } from '@/stores/genericData.js'

import { getLayoutVariables } from '@/mixins/genericFunctions.js'
import layoutConfig from '@/assets/config/Layoutconfig.json'
import eventBus from '@/api/global/eventBus.js'

/**
 * Returns an object with the default configuration of the progress bar.
 */
function getProgressBarDefaultConfig()
{
	return {
		isVisible: false,
		modalProps: {
			buttons: []
		},
		props: {},
		handlers: {}
	}
}

//----------------------------------------------------------------
// State variables
//----------------------------------------------------------------

const state = () => {
	return {
		layoutConfig: getLayoutVariables(layoutConfig),

		bookmarkMenuIsOpen: false,

		moduleMenuIsOpen: false,

		rightSidebarIsCollapsed: false,

		rightSidebarIsVisible: true,

		pageScroll: 0,

		headerHeight: 0,

		progressBar: getProgressBarDefaultConfig()
	}
}

//----------------------------------------------------------------
// Actions
//----------------------------------------------------------------

const actions = {
	/**
	 * Sets the layout config.
	 * @param {object} layoutData The configuration of the layout
	 */
	setLayoutConfig(layoutData)
	{
		if (typeof layoutData !== 'object')
			return

		this.layoutConfig = {
			...this.layoutConfig,
			layoutData
		}
	},

	/**
	 * Sets the state of the bookmarks menu.
	 * @param {boolean} isOpen Whether or not the bookmarks menu is open
	 */
	setBookmarkMenuState(isOpen)
	{
		if (typeof isOpen !== 'boolean')
			return

		this.bookmarkMenuIsOpen = isOpen
	},

	/**
	 * Sets the state of the modules menu.
	 * @param {boolean} isOpen Whether or not the modules menu is open
	 */
	setModuleMenuState(isOpen)
	{
		if (typeof isOpen !== 'boolean')
			return

		this.moduleMenuIsOpen = isOpen
	},

	/**
	 * Sets the collapse state of the right sidebar.
	 * This value is updated right away when expanding and collapsing, 
	 * so it's more like the state that the sidebar should be in / is going to.
	 * When collapsing, it will be false before the sidebar is actually invisible.
	 * @param {boolean} isCollapsed Whether or not the right sidebar is collapsed
	 */
	setRightSidebarCollapseState(isCollapsed)
	{
		if (typeof isCollapsed !== 'boolean')
			return

		this.rightSidebarIsCollapsed = isCollapsed

		//If false, the value for rightSidebarIsVisible must also change to true right away
		if(!this.rightSidebarIsCollapsed)
			this.setRightSidebarVisibility(true)
	},

	/**
	 * Sets the visibility of the right sidebar. 
	 * This is used to indicate the actual visibility in real-time.
	 * This is needed because, with transitions, the visibility should
	 * not be changed to hidden until the transition finishes.
	 * @param {boolean} isVisible Whether or not the sidebar is visible
	 */
	setRightSidebarVisibility(isVisible)
	{
		if (typeof isVisible !== 'boolean')
			return

		this.rightSidebarIsVisible = isVisible
	},

	/**
	 * Sets the current page scroll.
	 * @param {number} pageScroll The current page scroll (in pixels)
	 */
	setPageScroll(pageScroll)
	{
		if (typeof pageScroll !== 'number')
			return

		this.pageScroll = pageScroll
	},

	/**
	 * Sets the current height of the header.
	 * @param {number} height The current height of the header (in pixels)
	 */
	setHeaderHeight(height)
	{
		if (typeof height !== 'number')
			return

		this.headerHeight = height
	},

	/**
	 * Sets the progress bar with the provided options.
	 * @param {object} modalProps Configuration of the progress bar container
	 * @param {object} props Progress bar configuration
	 * @param {object} handlers Progress bar event handlers
	 * @returns A promise to be resolved when the progress bar is set.
	 */
	setProgressBar(modalProps, props, handlers)
	{
		return new Promise((resolve) => {
			const genericDataStore = useGenericDataStore()

			this.progressBar.modalProps = {
				...this.progressBar.modalProps,
				...modalProps
			}
			this.progressBar.props = {
				...this.progressBar.props,
				...props
			}
			this.progressBar.handlers = {
				...this.progressBar.handlers,
				...handlers
			}

			// If the progress bar is still hidden, we need to set up a modal for it.
			if (!this.progressBar.isVisible)
			{
				const modalProps = {
					id: 'progress-bar',
					headerTitle: this.progressBar.modalProps.title,
					hideHeader: !this.progressBar.modalProps.title,
					hideFooter: !(this.progressBar.modalProps.buttons?.length > 0),
					modalWidth: this.progressBar.modalProps.width ?? 'sm',
					isActive: true
				}
				genericDataStore.setModal(modalProps)

				const modalIsReady = (modalId) => {
					if (modalId !== modalProps.id)
						return

					this.progressBar.containerId = `q-modal-${modalProps.id}`
					this.progressBar.isVisible = true
					eventBus.off('modal-is-ready', modalIsReady)
					resolve()
				}

				eventBus.on('modal-is-ready', modalIsReady)
			}
			else
				resolve()
		})
	},

	/**
	 * Resets the progress bar options.
	 */
	resetProgressBar()
	{
		const defaultConfig = getProgressBarDefaultConfig()

		for (let i in this.progressBar)
			this.progressBar[i] = defaultConfig[i]

		const genericDataStore = useGenericDataStore()
		genericDataStore.removeModal('progress-bar')
	},

	/**
	 * Resets the layout info.
	 */
	resetStore()
	{
		Object.assign(this, state())
	}
}

//----------------------------------------------------------------
// Store export
//----------------------------------------------------------------

export const useGenericLayoutDataStore = defineStore('genericLayoutData', {
	state,
	actions
})
