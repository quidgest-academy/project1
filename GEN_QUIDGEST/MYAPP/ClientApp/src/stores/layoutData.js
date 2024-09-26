/*****************************************************************
 *                                                               *
 * This store holds data specific for the vertical layout,       *
 * also defining functions to access and mutate it.              *
 *                                                               *
 *****************************************************************/

import { defineStore } from 'pinia'

//----------------------------------------------------------------
// State variables
//----------------------------------------------------------------

const state = () => {
	return {
		layoutType: 'vertical',

		sidebarIsCollapsed: false,

		sidebarIsVisible: true,

		navBarIsVisible: true,

		isAccordionMenu: true
	}
}

//----------------------------------------------------------------
// Actions
//----------------------------------------------------------------

const actions = {
	/**
	 * Sets the collapse state of the sidebar.
	 * @param {boolean} isCollapsed Whether or not the sidebar is collapsed
	 */
	setSidebarCollapseState(isCollapsed)
	{
		if (typeof isCollapsed !== 'boolean')
			return

		this.sidebarIsCollapsed = isCollapsed
	},

	/**
	 * Sets the visibility of the sidebar. 
	 * This value is updated right away when expanding and collapsing, 
	 * so it's more like the state that the sidebar should be in / is going to.
	 * When collapsing, it will be false before the sidebar is actually invisible.
	 * @param {boolean} isVisible Whether or not the sidebar is visible
	 */
	setSidebarVisibility(isVisible)
	{
		if (typeof isVisible !== 'boolean')
			return

		this.sidebarIsVisible = isVisible

		//If true, the value for navBarIsVisible must also change to true right away
		if(this.sidebarIsVisible)
			this.setNavBarVisibility(isVisible)
	},

	/**
	 * Sets the visibility of the navigation bar. 
	 * This is used to indicate the actual visibility in real-time.
	 * This is needed because, with transitions, the visibility should
	 * not be changed to hidden until the transition finishes.
	 * @param {boolean} isVisible Whether or not the sidebar is visible
	 */
	setNavBarVisibility(isVisible)
	{
		if (typeof isVisible !== 'boolean')
			return

		this.navBarIsVisible = isVisible
	},

	/**
	 * Sets the type of the dropdown menus.
	 * @param {boolean} isAccordion Whether or not the dropdown menu is an accordion
	 */
	setMenuTypeAccordion(isAccordion)
	{
		if (typeof isAccordion !== 'boolean')
			return

		this.isAccordionMenu = isAccordion
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

export const useLayoutDataStore = defineStore('layoutData', {
	state,
	actions
})

//----------------------------------------------------------------
// Normal exports (so properties and functions can be used in other stores)
//----------------------------------------------------------------

export {
	state as useMobileLayoutState,
	actions as useMobileLayoutActions
}
