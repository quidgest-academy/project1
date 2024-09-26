/**
 * @jest-environment jsdom
 */
import { within } from '@testing-library/dom'
import '@testing-library/jest-dom'
import { fireEvent } from '@testing-library/vue'
import { flushPromises } from '@vue/test-utils'
import cloneDeep from 'lodash-es/cloneDeep'
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'
import { nextTick } from 'vue'
import { render } from './utils'

import listFunctions from '@/mixins/listFunctions.js'
import fakeData from '../cases/Table.mock.js'

import QTableActiveFilters from '@/components/table/QTableActiveFilters.vue'
import QTableChecklistCheckbox from '@/components/table/QTableChecklistCheckbox.vue'
import QTableColumnFilters from '@/components/table/QTableColumnFilters.vue'
import QTableExport from '@/components/table/QTableExport.vue'
import QTableLimitInfo from '@/components/table/QTableLimitInfo.vue'
import QTablePagination from '@/components/table/QTablePagination.vue'
import QTablePaginationAlt from '@/components/table/QTablePaginationAlt.vue'
import QTableRecordActionsMenu from '@/components/table/QTableRecordActionsMenu.vue'
import QTableSearch from '@/components/table/QTableSearch.vue'
import QTableStaticFilters from '@/components/table/QTableStaticFilters.vue'

const global = {
	stubs: ['inline-svg']
}

/**
 * Create popup element for dropdown to be teleported to.
 */
function createPopup()
{
	const popup = document.createElement('div')
	popup.id = 'q-dropdown'
	popup.style.position = 'absolute'
	popup.style.zIndex = 10000
	document.body.appendChild(popup)
	return popup
}

let tableTest
beforeEach(() => (tableTest = fakeData.getTableTest()))

describe('QTableRecordActionsMenu.vue', () => {
	const setupWrapper = function (actionsMenu, displayType) {
		return render(QTableRecordActionsMenu, {
			global,
			props: {
				...actionsMenu,
				display: displayType || actionsMenu.display,
				texts: tableTest.texts
			}
		})
	}

	it('In normal mode with 0 actions, dropdown display, component has no menu or button', async () => {
		const actionsMenu = fakeData.actionsMenu0
		const wrapper = setupWrapper(actionsMenu, 'dropdown')

		await nextTick()

		// No button
		const button = await wrapper.queryByRole('button')
		expect(button).toBeNull()
	})

	it('In normal mode with 0 actions, inline display, component has no menu or button', async () => {
		const actionsMenu = fakeData.actionsMenu0
		const wrapper = setupWrapper(actionsMenu, 'inline')

		await nextTick()

		// No button
		const button = await wrapper.queryByRole('button')
		expect(button).toBeNull()
	})

	it('In normal mode with 0 actions, inlineAll display, component has no menu or button', async () => {
		const actionsMenu = fakeData.actionsMenu0
		const wrapper = setupWrapper(actionsMenu, 'inlineAll')

		await nextTick()

		// No button
		const button = await wrapper.queryByRole('button')
		expect(button).toBeNull()
	})

	it('In read-only mode with 1 action, 0 available in read-only, dropdown display, component has no button or menu', async () => {
		const actionsMenu = fakeData.actionsMenu1ReadOnly0
		const wrapper = setupWrapper(actionsMenu, 'dropdown')

		await nextTick()

		// No button
		const button = await wrapper.queryByRole('button')
		expect(button).toBeNull()
	})

	it('In read-only mode with 1 action, 0 available in read-only, inline display, component has no button or menu', async () => {
		const actionsMenu = fakeData.actionsMenu1ReadOnly0
		const wrapper = setupWrapper(actionsMenu, 'inline')

		await nextTick()

		// No button
		const button = await wrapper.queryByRole('button')
		expect(button).toBeNull()
	})

	it('In read-only mode with 1 action, 0 available in read-only, inlineAll display, component has no button or menu', async () => {
		const actionsMenu = fakeData.actionsMenu1ReadOnly0
		const wrapper = setupWrapper(actionsMenu, 'inlineAll')

		await nextTick()

		// No button
		const button = await wrapper.queryByRole('button')
		expect(button).toBeNull()
	})

	it('In read-only mode with 1 action, 1 available in read-only, dropdown display, component has button, clicking action emits action', async () => {
		const actionsMenu = fakeData.actionsMenu1ReadOnly1
		const idx = 0
		const wrapper = setupWrapper(actionsMenu, 'dropdown')

		await nextTick()

		// Get action reference (whether it is in custom actions or CRUD actions)
		const actionsMenuActions = actionsMenu.customActions.concat(actionsMenu.crudActions)
		const action = actionsMenuActions[idx]
		// Get and click button
		const button = await wrapper.findByRole('button')
		expect(button)
		await fireEvent.click(button)
		expect(wrapper.emitted()).toHaveProperty('row-action')
		expect(wrapper.emitted()['row-action'][0][0]['name']).toBe(action.name)
	})

	it('In read-only mode with 1 action, 1 available in read-only, inline display, component has button, clicking action emits action', async () => {
		const actionsMenu = fakeData.actionsMenu1ReadOnly1
		const idx = 0
		const wrapper = setupWrapper(actionsMenu, 'inline')

		await nextTick()

		// Get action reference (whether it is in custom actions or CRUD actions)
		const actionsMenuActions = actionsMenu.customActions.concat(actionsMenu.crudActions)
		const action = actionsMenuActions[idx]
		// Get and click button
		const button = await wrapper.findByRole('button')
		expect(button)
		await fireEvent.click(button)
		expect(wrapper.emitted()).toHaveProperty('row-action')
		expect(wrapper.emitted()['row-action'][0][0]['name']).toBe(action.name)
	})

	it('In read-only mode with 1 action, 1 available in read-only, inlineAll display, component has button, clicking action emits action', async () => {
		const actionsMenu = fakeData.actionsMenu1ReadOnly1
		const idx = 0
		const wrapper = setupWrapper(actionsMenu, 'inlineAll')

		await nextTick()

		// Get action reference (whether it is in custom actions or CRUD actions)
		const actionsMenuActions = actionsMenu.customActions.concat(actionsMenu.crudActions)
		const action = actionsMenuActions[idx]
		// Get and click button
		const button = await wrapper.findByRole('button')
		expect(button)
		await fireEvent.click(button)
		expect(wrapper.emitted()).toHaveProperty('row-action')
		expect(wrapper.emitted()['row-action'][0][0]['name']).toBe(action.name)
	})

	it('In read-only mode with N actions, 0 available in read-only, dropdown display, component has no button or menu', async () => {
		const actionsMenu = fakeData.actionsMenuNReadOnly0
		const wrapper = setupWrapper(actionsMenu, 'dropdown')

		await nextTick()

		// No button
		const button = await wrapper.queryByRole('button')
		expect(button).toBeNull()
	})

	it('In read-only mode with N actions, 0 available in read-only, inline display, component has no button or menu', async () => {
		const actionsMenu = fakeData.actionsMenuNReadOnly0
		const wrapper = setupWrapper(actionsMenu, 'inline')

		await nextTick()

		// No button
		const button = await wrapper.queryByRole('button')
		expect(button).toBeNull()
	})

	it('In read-only mode with N actions, 0 available in read-only, inlineAll display, component has no button or menu', async () => {
		const actionsMenu = fakeData.actionsMenuNReadOnly0
		const wrapper = setupWrapper(actionsMenu, 'inlineAll')

		await nextTick()

		// No button
		const button = await wrapper.queryByRole('button')
		expect(button).toBeNull()
	})

	it('In read-only mode with N actions, 1 available in read-only, dropdown display, component has button, clicking action emits action', async () => {
		const actionsMenu = fakeData.actionsMenuNReadOnly1
		const idx = 0
		const wrapper = setupWrapper(actionsMenu, 'dropdown')

		await nextTick()

		// Get action reference (whether it is in custom actions or CRUD actions)
		const actionsMenuActions = actionsMenu.customActions.concat(actionsMenu.crudActions)
		const action = actionsMenuActions[idx]
		// Get and click button
		const button = await wrapper.findByTestId('table-action')
		expect(button)
		await fireEvent.click(button)
		expect(wrapper.emitted()).toHaveProperty('row-action')
		expect(wrapper.emitted()['row-action'][0][0]['name']).toBe(action.name)
	})

	it('In read-only mode with N actions, 1 available in read-only, inline display, component has button, clicking action emits action', async () => {
		const actionsMenu = fakeData.actionsMenuNReadOnly1
		const idx = 0
		const wrapper = setupWrapper(actionsMenu, 'inline')

		await nextTick()

		// Get action reference (whether it is in custom actions or CRUD actions)
		const actionsMenuActions = actionsMenu.customActions.concat(actionsMenu.crudActions)
		const action = actionsMenuActions[idx]
		// Get and click button
		const button = await wrapper.findByTestId('table-action')
		expect(button)
		await fireEvent.click(button)
		expect(wrapper.emitted()).toHaveProperty('row-action')
		expect(wrapper.emitted()['row-action'][0][0]['name']).toBe(action.name)
	})

	it('In read-only mode with N actions, 1 available in read-only, inlineAll display, component has button, clicking action emits action', async () => {
		const actionsMenu = fakeData.actionsMenuNReadOnly1
		const idx = 0
		const wrapper = setupWrapper(actionsMenu, 'inlineAll')

		await nextTick()

		// Get action reference (whether it is in custom actions or CRUD actions)
		const actionsMenuActions = actionsMenu.customActions.concat(actionsMenu.crudActions)
		const action = actionsMenuActions[idx]
		// Get and click button
		const button = await wrapper.findByTestId('table-action')
		expect(button)
		await fireEvent.click(button)
		expect(wrapper.emitted()).toHaveProperty('row-action')
		expect(wrapper.emitted()['row-action'][0][0]['name']).toBe(action.name)
	})

	it('In read-only mode with N actions, N available in read-only, dropdown display, component has menu, clicking action emits action', async () => {
		const actionsMenu = fakeData.actionsMenuNReadOnlyN
		const idx = 0
		const wrapper = setupWrapper(actionsMenu, 'dropdown')

		await nextTick()

		// Get action reference (whether it is in custom actions or CRUD actions)
		const actionsMenuActions = actionsMenu.customActions.concat(actionsMenu.crudActions)
		const action = actionsMenuActions[idx]
		// Get and click toggle button
		const button = await wrapper.findByRole('button')
		expect(button)
		await fireEvent.click(button)
		// Get menu
		const menu = await wrapper.findByRole('menu')
		expect(menu)
		// Get and click action
		const actions = await wrapper.findAllByTestId('table-action')
		expect(actions)
		await fireEvent.click(actions[idx])
		expect(wrapper.emitted()).toHaveProperty('row-action')
		expect(wrapper.emitted()['row-action'][0][0]['name']).toBe(action.name)
	})

	it('In read-only mode with N actions, N available in read-only, inline display, component has multiple buttons, clicking action emits action', async () => {
		const actionsMenu = fakeData.actionsMenuNReadOnlyN
		const idx = 0
		const wrapper = setupWrapper(actionsMenu, 'inline')

		await nextTick()

		// Get action reference (whether it is in custom actions or CRUD actions)
		const actionsMenuActions = actionsMenu.customActions.concat(actionsMenu.crudActions)
		const numVisibleActions = listFunctions.numArrayVisibleActions(actionsMenuActions)
		const action = actionsMenuActions[idx]
		// Get and click button
		const buttons = await wrapper.findAllByTestId('table-action')
		expect(buttons).toHaveLength(numVisibleActions)
		await fireEvent.click(buttons[0])
		expect(wrapper.emitted()).toHaveProperty('row-action')
		expect(wrapper.emitted()['row-action'][0][0]['name']).toBe(action.name)
	})

	it('In read-only mode with N actions, N available in read-only, inlineAll display, component has multiple buttons, clicking action emits action', async () => {
		const actionsMenu = fakeData.actionsMenuNReadOnlyN
		const idx = 0
		const wrapper = setupWrapper(actionsMenu, 'inlineAll')

		await nextTick()

		// Get action reference (whether it is in custom actions or CRUD actions)
		const actionsMenuActions = actionsMenu.customActions.concat(actionsMenu.crudActions)
		const numVisibleActions = listFunctions.numArrayVisibleActions(actionsMenuActions)
		const action = actionsMenuActions[idx]
		// Get and click button
		const buttons = await wrapper.findAllByTestId('table-action')
		expect(buttons).toHaveLength(numVisibleActions)
		await fireEvent.click(buttons[0])
		expect(wrapper.emitted()).toHaveProperty('row-action')
		expect(wrapper.emitted()['row-action'][0][0]['name']).toBe(action.name)
	})
})

describe('QTableSearch.vue', () => {
	const setupWrapper = function (dataSearchbar, searchableColumns) {
		return render(QTableSearch, {
			global,
			props: {
				...dataSearchbar,
				searchableColumns: searchableColumns,
				texts: tableTest.texts
			}
		})
	}

	it('Clicking search button emits emitSearch event and search value', async () => {
		const dataSearchbar = fakeData.searchbar01,
			searchableColumns = fakeData.searchableColumns01
		const wrapper = setupWrapper(dataSearchbar, searchableColumns)
		const searchValue = 'asd'

		await nextTick()

		// Get and click toggle button
		const searchbox = await wrapper.findByRole('searchbox')
		expect(searchbox)
		await fireEvent.click(searchbox)
		await fireEvent.update(searchbox, searchValue)
		// KeyUp event must be fired after updating textbox to call a related update function
		await fireEvent.keyUp(searchbox, { key: 'ArrowUp', keyCode: 38, charCode: 38 })
		// Get and click search button
		const button = await wrapper.findByTitle('PESQUISAR34506')
		expect(button)
		await fireEvent.click(button)
		expect(wrapper.emitted()).toHaveProperty('emit-search')
		expect(wrapper.emitted()['emit-search'][0][0]).toBe(searchValue)
	})

	it('Selecting search in column emits searchByColumn event, column and search value', async () => {
		const dataSearchbar = fakeData.searchbar01,
			searchableColumns = fakeData.searchableColumns01
		const wrapper = setupWrapper(dataSearchbar, searchableColumns)
		const searchValue = 'asd'
		const idx = 0

		await nextTick()

		// Get and click toggle button
		const searchbox = await wrapper.findByRole('searchbox')
		expect(searchbox)
		searchbox.focus()
		await fireEvent.update(searchbox, searchValue)
		// KeyUp event must be fired after updating textbox to call a related update function
		await fireEvent.keyUp(searchbox, { key: 'ArrowUp', keyCode: 38, charCode: 38 })
		// Get menu
		const menu = await wrapper.findByRole('menu')
		expect(menu)
		// Get and click action
		const actions = await wrapper.findAllByRole('menuitem')
		expect(actions)
		await fireEvent.click(actions[idx])
		expect(wrapper.emitted()).toHaveProperty('search-by-column')
		expect(wrapper.emitted()['search-by-column'][0][0]).toEqual(searchableColumns[idx])
		expect(wrapper.emitted()['search-by-column'][0][1]).toBe(searchValue)
	})

	it('Selecting search all columns emits searchByAllColumns event and search value', async () => {
		const dataSearchbar = fakeData.searchbar01,
			searchableColumns = fakeData.searchableColumns01
		const wrapper = setupWrapper(dataSearchbar, searchableColumns)
		const searchValue = 'asd'
		const idx = searchableColumns.length

		await nextTick()

		// Get and click toggle button
		const searchbox = await wrapper.findByRole('searchbox')
		expect(searchbox)
		searchbox.focus()
		await fireEvent.update(searchbox, searchValue)
		// KeyUp event must be fired after updating textbox to call a related update function
		await fireEvent.keyUp(searchbox, { key: 'ArrowUp', keyCode: 38, charCode: 38 })
		// Get menu
		const menu = await wrapper.findByRole('menu')
		expect(menu)
		// Get and click action
		const actions = await wrapper.findAllByRole('menuitem')
		expect(actions)
		await fireEvent.click(actions[idx])
		expect(wrapper.emitted()).toHaveProperty('search-by-all-columns')
		expect(wrapper.emitted()['search-by-all-columns'][0][0]).toBe(searchValue)
	})
})

describe('QTableExport.vue', () => {
	it('File export menu item emits selected format', async () => {
		const dataOptions = fakeData.exportOptions01
		const wrapper = render(QTableExport, {
			global,
			props: {
				options: dataOptions,
				texts: tableTest.texts
			}
		})
		const idx = 0

		await nextTick()

		const button = await wrapper.findByRole('button')
		expect(button)
		// Click export button
		await fireEvent.click(button)
		// Get menu items
		const menuitems = await wrapper.findAllByRole('menuitem')
		// Click export menu item and check emit
		await fireEvent.click(menuitems[idx])
		expect(wrapper.emitted()).toHaveProperty('export-data')
		expect(wrapper.emitted()['export-data'][0][0]).toBe(dataOptions[idx].id)
	})
})

describe('QTableStaticFilters.vue', () => {
	it('Selecting a filter radio button emits event to update filter values', async () => {
		const dataFilters = fakeData.groupFiltersSingle01
		const wrapper = render(QTableStaticFilters, {
			global,
			props: {
				groupFilters: dataFilters,
				texts: tableTest.texts
			}
		})
		const idx = 0

		await nextTick()

		// Get filter control group
		const controlgroup = await wrapper.findByRole('radiogroup')
		expect(controlgroup)
		// Get filter controls
		const controls = await within(controlgroup).findAllByRole('radio')
		// Click filter control and check emit
		await fireEvent.click(controls[idx])
		expect(wrapper.emitted()).toHaveProperty('on-update-filter')
	})

	it('Selecting a filter checkbox emits event to update filter values', async () => {
		const dataFilters = fakeData.groupFiltersMultiple01
		const wrapper = render(QTableStaticFilters, {
			global,
			props: {
				groupFilters: dataFilters,
				texts: tableTest.texts
			}
		})
		const idx = 0

		await nextTick()

		// Get filter controls
		const controls = await wrapper.findAllByRole('checkbox')
		// Click filter control and check emit
		await fireEvent.click(controls[idx])
		expect(wrapper.emitted()).toHaveProperty('on-update-filter')
	})

	it('Selecting an active filter checkbox emits event to update filter values', async () => {
		const dataFilters = fakeData.activeFilters01
		const wrapper = render(QTableStaticFilters, {
			global,
			props: {
				activeFilters: dataFilters,
				texts: tableTest.texts
			}
		})
		const idx = 0

		await nextTick()

		// Get filter controls
		const controls = await wrapper.findAllByRole('checkbox')
		// Click filter control and check emit
		await fireEvent.click(controls[idx])

		await flushPromises()
		await vi.dynamicImportSettled()

		expect(wrapper.emitted()).toHaveProperty('on-update-filter')
	})
})

describe('QTablePagination.vue', () => {
	it('Clicking first pagination button emits event and page number 1', async () => {
		const dataPagination = fakeData.paginationNormal01
		const wrapper = render(QTablePagination, {
			global,
			props: {
				page: dataPagination.page,
				perPage: dataPagination.perPage,
				total: dataPagination.rowCount,
				numVisibilePaginationButtons: dataPagination.numVisibilePaginationButtons,
				texts: tableTest.texts
			}
		})

		await nextTick()

		// Get page buttons
		const buttons = await wrapper.findAllByRole('button')
		// Click page button and check emit
		await fireEvent.click(buttons[0])
		expect(wrapper.emitted()).toHaveProperty('update:page')
		expect(wrapper.emitted()['update:page'][0][0]).toBe(1)
	})

	it('Clicking previous pagination button emits event and previous page number', async () => {
		const dataPagination = fakeData.paginationNormal01
		const wrapper = render(QTablePagination, {
			global,
			props: {
				page: dataPagination.page,
				perPage: dataPagination.perPage,
				total: dataPagination.rowCount,
				numVisibilePaginationButtons: dataPagination.numVisibilePaginationButtons,
				texts: tableTest.texts
			}
		})

		await nextTick()

		// Get page buttons
		const buttons = await wrapper.findAllByRole('button')
		// Click page button and check emit
		await fireEvent.click(buttons[1])
		expect(wrapper.emitted()).toHaveProperty('update:page')
		expect(wrapper.emitted()['update:page'][0][0]).toBe(dataPagination.page - 1)
	})

	it('Clicking current pagination button emits event and current page number', async () => {
		const dataPagination = fakeData.paginationNormal01
		const wrapper = render(QTablePagination, {
			global,
			props: {
				page: dataPagination.page,
				perPage: dataPagination.perPage,
				total: dataPagination.rowCount,
				numVisibilePaginationButtons: dataPagination.numVisibilePaginationButtons,
				texts: tableTest.texts
			}
		})

		await nextTick()

		// Get page buttons
		const buttons = await wrapper.findAllByRole('button')
		// Click page button and check emit
		await fireEvent.click(buttons[4])
		expect(wrapper.emitted()).toHaveProperty('update:page')
		expect(wrapper.emitted()['update:page'][0][0]).toBe(dataPagination.page)
	})

	it('Clicking next pagination button emits event and next page number', async () => {
		const dataPagination = fakeData.paginationNormal01
		const wrapper = render(QTablePagination, {
			global,
			props: {
				page: dataPagination.page,
				perPage: dataPagination.perPage,
				total: dataPagination.rowCount,
				numVisibilePaginationButtons: dataPagination.numVisibilePaginationButtons,
				texts: tableTest.texts
			}
		})

		await nextTick()

		// Get page buttons
		const buttons = await wrapper.findAllByRole('button')
		// Click page button and check emit
		await fireEvent.click(buttons[7])
		expect(wrapper.emitted()).toHaveProperty('update:page')
		expect(wrapper.emitted()['update:page'][0][0]).toBe(dataPagination.page + 1)
	})

	it('Clicking last pagination button emits event and last page number', async () => {
		const dataPagination = fakeData.paginationNormal01
		const wrapper = render(QTablePagination, {
			global,
			props: {
				page: dataPagination.page,
				perPage: dataPagination.perPage,
				total: dataPagination.rowCount,
				numVisibilePaginationButtons: dataPagination.numVisibilePaginationButtons,
				texts: tableTest.texts
			}
		})

		await nextTick()

		// Get page buttons
		const buttons = await wrapper.findAllByRole('button')
		// Click page button and check emit
		await fireEvent.click(buttons[8])
		expect(wrapper.emitted()).toHaveProperty('update:page')
		expect(wrapper.emitted()['update:page'][0][0]).toBe(
			dataPagination.rowCount / dataPagination.perPage
		)
	})

	it('Pagination with no rows has no buttons', async () => {
		const dataPagination = cloneDeep(fakeData.paginationNormal01)
		const wrapper = render(QTablePagination, {
			global,
			props: {
				page: 0,
				perPage: dataPagination.perPage,
				total: 0,
				numVisibilePaginationButtons: dataPagination.numVisibilePaginationButtons,
				texts: tableTest.texts
			}
		})

		await nextTick()

		// Get page buttons
		const buttons = await wrapper.queryAllByRole('button')
		expect(buttons).toHaveLength(0)
	})

	it('Pagination on first page has first, previous, numbered pages, next, last buttons', async () => {
		const dataPagination = cloneDeep(fakeData.paginationNormal01)
		dataPagination.page = 1
		const wrapper = render(QTablePagination, {
			global,
			props: {
				page: dataPagination.page,
				perPage: dataPagination.perPage,
				total: dataPagination.rowCount,
				numVisibilePaginationButtons: dataPagination.numVisibilePaginationButtons,
				texts: tableTest.texts
			}
		})

		await nextTick()

		// Get page buttons
		const buttons = await wrapper.findAllByRole('button')
		expect(buttons[0].getAttribute('aria-label')).toBe(tableTest.texts.first.value)
		expect(buttons[1].getAttribute('aria-label')).toBe(tableTest.texts.previous.value)
		expect(buttons[2]).toHaveTextContent(dataPagination.page)
		expect(buttons[3]).toHaveTextContent(dataPagination.page + 1)
		expect(buttons[4]).toHaveTextContent(dataPagination.page + 2)
		expect(buttons[5]).toHaveTextContent(dataPagination.page + 3)
		expect(buttons[6]).toHaveTextContent(dataPagination.page + 4)
		expect(buttons[7].getAttribute('aria-label')).toBe(tableTest.texts.next.value)
		expect(buttons[8].getAttribute('aria-label')).toBe(tableTest.texts.last.value)
	})

	it('Pagination on second page has first, previous, numbered pages, next, last buttons', async () => {
		const dataPagination = cloneDeep(fakeData.paginationNormal01)
		dataPagination.page = 2
		const wrapper = render(QTablePagination, {
			global,
			props: {
				page: dataPagination.page,
				perPage: dataPagination.perPage,
				total: dataPagination.rowCount,
				numVisibilePaginationButtons: dataPagination.numVisibilePaginationButtons,
				texts: tableTest.texts
			}
		})

		await nextTick()

		// Get page buttons
		const buttons = await wrapper.findAllByRole('button')
		expect(buttons[0].getAttribute('aria-label')).toBe(tableTest.texts.first.value)
		expect(buttons[1].getAttribute('aria-label')).toBe(tableTest.texts.previous.value)
		expect(buttons[2]).toHaveTextContent(dataPagination.page - 1)
		expect(buttons[3]).toHaveTextContent(dataPagination.page)
		expect(buttons[4]).toHaveTextContent(dataPagination.page + 1)
		expect(buttons[5]).toHaveTextContent(dataPagination.page + 2)
		expect(buttons[6]).toHaveTextContent(dataPagination.page + 3)
		expect(buttons[7].getAttribute('aria-label')).toBe(tableTest.texts.next.value)
		expect(buttons[8].getAttribute('aria-label')).toBe(tableTest.texts.last.value)
	})

	it('Pagination on middle page has first, previous, numbered pages, next, last buttons', async () => {
		const dataPagination = cloneDeep(fakeData.paginationNormal01)
		const wrapper = render(QTablePagination, {
			global,
			props: {
				page: dataPagination.page,
				perPage: dataPagination.perPage,
				total: dataPagination.rowCount,
				numVisibilePaginationButtons: dataPagination.numVisibilePaginationButtons,
				texts: tableTest.texts
			}
		})

		await nextTick()

		// Get page buttons
		const buttons = await wrapper.findAllByRole('button')
		expect(buttons[0].getAttribute('aria-label')).toBe(tableTest.texts.first.value)
		expect(buttons[1].getAttribute('aria-label')).toBe(tableTest.texts.previous.value)
		expect(buttons[2]).toHaveTextContent(dataPagination.page - 2)
		expect(buttons[3]).toHaveTextContent(dataPagination.page - 1)
		expect(buttons[4]).toHaveTextContent(dataPagination.page)
		expect(buttons[5]).toHaveTextContent(dataPagination.page + 1)
		expect(buttons[6]).toHaveTextContent(dataPagination.page + 2)
		expect(buttons[7].getAttribute('aria-label')).toBe(tableTest.texts.next.value)
		expect(buttons[8].getAttribute('aria-label')).toBe(tableTest.texts.last.value)
	})

	it('Pagination on second last page has first, previous, numbered pages, next buttons', async () => {
		const dataPagination = cloneDeep(fakeData.paginationNormal01)
		dataPagination.page = 9
		const wrapper = render(QTablePagination, {
			global,
			props: {
				page: dataPagination.page,
				perPage: dataPagination.perPage,
				total: dataPagination.rowCount,
				numVisibilePaginationButtons: dataPagination.numVisibilePaginationButtons,
				texts: tableTest.texts
			}
		})

		await nextTick()

		// Get page buttons
		const buttons = await wrapper.findAllByRole('button')
		expect(buttons[0].getAttribute('aria-label')).toBe(tableTest.texts.first.value)
		expect(buttons[1].getAttribute('aria-label')).toBe(tableTest.texts.previous.value)
		expect(buttons[2]).toHaveTextContent(dataPagination.page - 3)
		expect(buttons[3]).toHaveTextContent(dataPagination.page - 2)
		expect(buttons[4]).toHaveTextContent(dataPagination.page - 1)
		expect(buttons[5]).toHaveTextContent(dataPagination.page)
		expect(buttons[6]).toHaveTextContent(dataPagination.page + 1)
		expect(buttons[7].getAttribute('aria-label')).toBe(tableTest.texts.next.value)
	})

	it('Pagination on last page has first, previous, numbered pages buttons', async () => {
		const dataPagination = cloneDeep(fakeData.paginationNormal01)
		dataPagination.page = 10
		const wrapper = render(QTablePagination, {
			global,
			props: {
				page: dataPagination.page,
				perPage: dataPagination.perPage,
				total: dataPagination.rowCount,
				numVisibilePaginationButtons: dataPagination.numVisibilePaginationButtons,
				texts: tableTest.texts
			}
		})

		await nextTick()

		// Get page buttons
		const buttons = await wrapper.findAllByRole('button')
		expect(buttons[0].getAttribute('aria-label')).toBe(tableTest.texts.first.value)
		expect(buttons[1].getAttribute('aria-label')).toBe(tableTest.texts.previous.value)
		expect(buttons[2]).toHaveTextContent(dataPagination.page - 4)
		expect(buttons[3]).toHaveTextContent(dataPagination.page - 3)
		expect(buttons[4]).toHaveTextContent(dataPagination.page - 2)
		expect(buttons[5]).toHaveTextContent(dataPagination.page - 1)
		expect(buttons[6]).toHaveTextContent(dataPagination.page)
	})
})

describe('QTablePaginationAlt.vue', () => {
	it('Clicking first pagination button emits event and page number 1', async () => {
		const dataPagination = fakeData.paginationAlt01
		const wrapper = render(QTablePaginationAlt, {
			global,
			props: {
				page: dataPagination.page,
				perPage: dataPagination.perPage,
				total: dataPagination.rowCount,
				hasMorePages: dataPagination.hasMore,
				texts: tableTest.texts
			}
		})

		await nextTick()

		// Get page buttons
		const buttons = await wrapper.findAllByRole('button')
		// Click page button and check emit
		await fireEvent.click(buttons[0])
		expect(wrapper.emitted()).toHaveProperty('update:page')
		expect(wrapper.emitted()['update:page'][0][0]).toBe(1)
	})

	it('Clicking previous pagination button emits event and previous page number', async () => {
		const dataPagination = fakeData.paginationAlt01
		const wrapper = render(QTablePaginationAlt, {
			global,
			props: {
				page: dataPagination.page,
				perPage: dataPagination.perPage,
				total: dataPagination.rowCount,
				hasMorePages: dataPagination.hasMore,
				texts: tableTest.texts
			}
		})

		await nextTick()

		// Get page buttons
		const buttons = await wrapper.findAllByRole('button')
		// Click page button and check emit
		await fireEvent.click(buttons[1])
		expect(wrapper.emitted()).toHaveProperty('update:page')
		expect(wrapper.emitted()['update:page'][0][0]).toBe(dataPagination.page - 1)
	})

	it('Clicking next pagination button emits event and next page number', async () => {
		const dataPagination = fakeData.paginationAlt01
		const wrapper = render(QTablePaginationAlt, {
			global,
			props: {
				page: dataPagination.page,
				perPage: dataPagination.perPage,
				total: dataPagination.rowCount,
				hasMorePages: dataPagination.hasMore,
				texts: tableTest.texts
			}
		})

		await nextTick()

		// Get page buttons
		const buttons = await wrapper.findAllByRole('button')
		// Click page button and check emit
		await fireEvent.click(buttons[2])
		expect(wrapper.emitted()).toHaveProperty('update:page')
		expect(wrapper.emitted()['update:page'][0][0]).toBe(dataPagination.page + 1)
	})

	it('Pagination with no rows shows has no buttons', async () => {
		const dataPagination = cloneDeep(fakeData.paginationAlt01)
		dataPagination.rowCount = 0
		dataPagination.hasMore = false
		const wrapper = render(QTablePaginationAlt, {
			global,
			props: {
				page: 1,
				perPage: dataPagination.perPage,
				total: 0,
				hasMorePages: false,
				texts: tableTest.texts
			}
		})

		await nextTick()

		// Get page buttons
		const buttons = await wrapper.queryAllByRole('button')
		expect(buttons).toHaveLength(0)
	})

	it('Pagination on first page has next button', async () => {
		const dataPagination = cloneDeep(fakeData.paginationAlt01)
		dataPagination.page = 1
		const wrapper = render(QTablePaginationAlt, {
			global,
			props: {
				page: dataPagination.page,
				perPage: dataPagination.perPage,
				total: dataPagination.rowCount,
				hasMorePages: dataPagination.hasMore,
				texts: tableTest.texts
			}
		})

		await nextTick()

		// Get page buttons
		const buttons = await wrapper.findAllByRole('button')
		expect(buttons[0].getAttribute('aria-label')).toBe(tableTest.texts.first.value)
		expect(buttons[1].getAttribute('aria-label')).toBe(tableTest.texts.previous.value)
		expect(buttons[2].getAttribute('aria-label')).toBe(tableTest.texts.next.value)
	})

	it('Pagination on second page has first, previous, next buttons', async () => {
		const dataPagination = cloneDeep(fakeData.paginationAlt01)
		dataPagination.page = 2
		const wrapper = render(QTablePaginationAlt, {
			global,
			props: {
				page: dataPagination.page,
				perPage: dataPagination.perPage,
				total: dataPagination.rowCount,
				hasMorePages: dataPagination.hasMore,
				texts: tableTest.texts
			}
		})

		await nextTick()

		// Get page buttons
		const buttons = await wrapper.findAllByRole('button')
		expect(buttons[0].getAttribute('aria-label')).toBe(tableTest.texts.first.value)
		expect(buttons[1].getAttribute('aria-label')).toBe(tableTest.texts.previous.value)
		expect(buttons[buttons.length - 1].getAttribute('aria-label')).toBe(tableTest.texts.next.value)
	})

	it('Pagination on middle page has first, previous, next buttons', async () => {
		const dataPagination = cloneDeep(fakeData.paginationAlt01)
		const wrapper = render(QTablePaginationAlt, {
			global,
			props: {
				page: dataPagination.page,
				perPage: dataPagination.perPage,
				total: dataPagination.rowCount,
				hasMorePages: dataPagination.hasMore,
				texts: tableTest.texts
			}
		})

		await nextTick()

		// Get page buttons
		const buttons = await wrapper.findAllByRole('button')
		expect(buttons[0].getAttribute('aria-label')).toBe(tableTest.texts.first.value)
		expect(buttons[1].getAttribute('aria-label')).toBe(tableTest.texts.previous.value)
		expect(buttons[2].getAttribute('aria-label')).toBe(tableTest.texts.next.value)
	})

	it('Pagination on last page has first, previous buttons', async () => {
		const dataPagination = cloneDeep(fakeData.paginationAlt01)
		dataPagination.page = 10
		const wrapper = render(QTablePaginationAlt, {
			global,
			props: {
				page: dataPagination.page,
				perPage: dataPagination.perPage,
				total: dataPagination.rowCount,
				hasMorePages: dataPagination.hasMore,
				texts: tableTest.texts
			}
		})

		await nextTick()

		// Get page buttons
		const buttons = await wrapper.findAllByRole('button')
		expect(buttons[0].getAttribute('aria-label')).toBe(tableTest.texts.first.value)
		expect(buttons[1].getAttribute('aria-label')).toBe(tableTest.texts.previous.value)
	})
})

describe('QTableLimitInfo.vue', () => {
	it('Limits info dropdown displays list of limit', async () => {
		const dataLimits = fakeData.tableLimits01
		const dataText = fakeData.tableLimitsText01
		const wrapper = render(QTableLimitInfo, {
			global,
			props: {
				limits: dataLimits,
				tableNamePlural: dataText.tableNamePlural,
				texts: tableTest.texts
			}
		})

		await nextTick()

		const button = await wrapper.findByRole('button')
		expect(button)
		// Click button
		await fireEvent.click(button)
		// Get list items
		const listitems = await wrapper.findAllByRole('listitem')
		expect(listitems).toHaveLength(dataLimits.length + 1)
	})
})

describe('QTableChecklistCheckbox.vue', () => {
	it('Clicking row checklist checkbox emits event to toggle selecting row', async () => {
		const dataCheckbox = fakeData.checklistCheckboxRow01
		const wrapper = render(QTableChecklistCheckbox, {
			global,
			props: {
				value: dataCheckbox.value,
				tableName: dataCheckbox.tableName,
				readonly: dataCheckbox.readonly,
				rowKey: dataCheckbox.rowKey,
				texts: tableTest.texts
			}
		})

		await nextTick()

		const checkbox = await wrapper.findByRole('checkbox')
		expect(checkbox)
		// Click checkbox
		await fireEvent.click(checkbox)
		// Check emit
		expect(wrapper.emitted()).toHaveProperty('toggle-row-selected')
	})

	it('Clicking header checklist checkbox emits event to toggle selecting all rows', async () => {
		const dataCheckbox = fakeData.checklistCheckboxRow01
		const wrapper = render(QTableChecklistCheckbox, {
			global,
			props: {
				value: dataCheckbox.value,
				tableName: dataCheckbox.tableName,
				readonly: dataCheckbox.readonly,
				texts: tableTest.texts
			}
		})

		await nextTick()

		const checkbox = await wrapper.findByRole('checkbox')
		expect(checkbox)
		// Click checkbox
		await fireEvent.click(checkbox)
		// Check emit
		expect(wrapper.emitted()).toHaveProperty('toggle-all-rows-selected')
	})
})

describe('QTableColumnFilters.vue', () => {
	let popup
	beforeEach(() => (popup = createPopup()))
	afterEach(() => document.body.removeChild(popup))

	it('Clicking sort ascending emits event, column and sort direction', async () => {
		const dataFilter = fakeData.columnFilter01
		const columns = fakeData.columns01
		const searchableColumns = fakeData.searchableColumns01
		const searchFilterData = fakeData.searchFilterData
		const wrapper = render(QTableColumnFilters, {
			global,
			props: {
				column: columns[1],
				query: {},
				allowColumnSort: true,
				allowColumnFilters: true,
				searchableColumns: searchableColumns,
				filter: dataFilter,
				searchFilterData: searchFilterData,
				texts: tableTest.texts
			}
		})

		await nextTick()

		const toggleButton = await wrapper.findByRole('button')
		expect(toggleButton)
		// Click button
		await fireEvent.click(toggleButton)
		// Check emit
		expect(wrapper.emitted()).toHaveProperty('set-dropdown')

		// Get buttons in dropdown
		const buttons = await within(popup).findAllByRole('button')
		// Click button
		await fireEvent.click(buttons[0])
		// Check emit
		expect(wrapper.emitted()).toHaveProperty('update-sort')
		expect(wrapper.emitted()['update-sort'][0][0]).toEqual(columns[1])
		expect(wrapper.emitted()['update-sort'][0][1]).toBe('asc')
	})

	it('Clicking sort descending emits event, column and sort direction', async () => {
		const dataFilter = fakeData.columnFilter01
		const columns = fakeData.columns01
		const searchableColumns = fakeData.searchableColumns01
		const searchFilterData = fakeData.searchFilterData
		const wrapper = render(QTableColumnFilters, {
			global,
			props: {
				column: columns[1],
				query: {},
				allowColumnSort: true,
				allowColumnFilters: true,
				searchableColumns: searchableColumns,
				filter: dataFilter,
				searchFilterData: searchFilterData,
				texts: tableTest.texts
			}
		})

		await nextTick()

		const toggleButton = await wrapper.findByRole('button')
		expect(toggleButton)
		// Click button
		await fireEvent.click(toggleButton)
		// Check emit
		expect(wrapper.emitted()).toHaveProperty('set-dropdown')

		// Get buttons in dropdown
		const buttons = await within(popup).findAllByRole('button')
		// Click button
		await fireEvent.click(buttons[1])
		// Check emit
		expect(wrapper.emitted()).toHaveProperty('update-sort')
		expect(wrapper.emitted()['update-sort'][0][0]).toEqual(columns[1])
		expect(wrapper.emitted()['update-sort'][0][1]).toBe('desc')
	})

	it('Clicking save emits event to save', async () => {
		const dataFilter = fakeData.columnFilter01
		const columns = fakeData.columns01
		const searchableColumns = fakeData.searchableColumns01
		const searchFilterData = fakeData.searchFilterData
		const wrapper = render(QTableColumnFilters, {
			global,
			props: {
				column: columns[1],
				query: {},
				allowColumnFilters: true,
				searchableColumns: searchableColumns,
				filter: dataFilter,
				searchFilterData: searchFilterData,
				texts: tableTest.texts
			}
		})

		await nextTick()

		const toggleButton = await wrapper.findByRole('button')
		expect(toggleButton)
		// Click button
		await fireEvent.click(toggleButton)
		// Check emit
		expect(wrapper.emitted()).toHaveProperty('set-dropdown')

		// Get buttons in dropdown
		const buttons = await within(popup).findAllByRole('button')
		// Click button
		await fireEvent.click(buttons[0])
		// Check emit
		expect(wrapper.emitted()).toHaveProperty('edit-column-filter')
		expect(wrapper.emitted()['edit-column-filter'][0][0]).toBe(
			listFunctions.columnFullName(columns[1])
		)
	})

	it('Clicking remove emits event to remove', async () => {
		const dataFilter = fakeData.columnFilter01
		const columns = fakeData.columns01
		const searchableColumns = fakeData.searchableColumns01
		const searchFilterData = fakeData.searchFilterData
		const wrapper = render(QTableColumnFilters, {
			global,
			props: {
				column: columns[1],
				query: {},
				allowColumnSort: true,
				allowColumnFilters: true,
				searchableColumns: searchableColumns,
				filter: dataFilter,
				searchFilterData: searchFilterData,
				texts: tableTest.texts
			}
		})

		await nextTick()

		const toggleButton = await wrapper.findByRole('button')
		expect(toggleButton)
		// Click button
		await fireEvent.click(toggleButton)
		// Check emit
		expect(wrapper.emitted()).toHaveProperty('set-dropdown')

		// Get buttons in dropdown
		const buttons = await within(popup).findAllByRole('button')
		// Click button
		await fireEvent.click(buttons[3])
		// Check emit
		expect(wrapper.emitted()).toHaveProperty('remove-column-filter')
		expect(wrapper.emitted()['remove-column-filter'][0][0]).toBe(
			listFunctions.columnFullName(columns[1])
		)
	})
})

describe('QTableActiveFilters.vue', () => {
	it('Configuration with advanced filters, column filters and searchbar filter show tag for each filter and tag to remove all filters', async () => {
		const dataAdvancedFilters = fakeData.filterArray01
		const dataColumnFilters = fakeData.filterHash01
		const dataSearchBarFilters = fakeData.filterHash02
		const hasFiltersActive = true
		const searchableColumns = fakeData.searchableColumns01
		const wrapper = render(QTableActiveFilters, {
			global,
			props: {
				searchableColumns: searchableColumns,
				advancedFilters: dataAdvancedFilters,
				columnFilters: dataColumnFilters,
				searchBarFilters: dataSearchBarFilters,
				hasFiltersActive: hasFiltersActive,
				texts: tableTest.texts
			}
		})
		const filterCount =
			dataAdvancedFilters.length +
			Object.keys(dataColumnFilters).length +
			Object.keys(dataSearchBarFilters).length +
			1

		await nextTick()

		// Get buttons in dropdown
		const buttons = await wrapper.findAllByRole('button')
		expect(buttons).toHaveLength(filterCount)
	})

	it('Clicking the remove all filters tag emits event to remove all filters', async () => {
		const dataAdvancedFilters = fakeData.filterArray01
		const dataColumnFilters = fakeData.filterHash01
		const dataSearchBarFilters = fakeData.filterHash02
		const hasFiltersActive = true
		const searchableColumns = fakeData.searchableColumns01
		const wrapper = render(QTableActiveFilters, {
			global,
			props: {
				searchableColumns: searchableColumns,
				advancedFilters: dataAdvancedFilters,
				columnFilters: dataColumnFilters,
				searchBarFilters: dataSearchBarFilters,
				hasFiltersActive: hasFiltersActive,
				texts: tableTest.texts
			}
		})
		const filterCount =
			dataAdvancedFilters.length +
			Object.keys(dataColumnFilters).length +
			Object.keys(dataSearchBarFilters).length +
			1

		await nextTick()

		// Get buttons in dropdown
		const buttons = await wrapper.findAllByRole('button')
		// Click button
		await fireEvent.click(buttons[filterCount - 1])
		// Check emit
		expect(wrapper.emitted()).toHaveProperty('remove-custom-filters')
	})

	it('Clicking advanced filter tag emits event to deactivate filter and filter index', async () => {
		const dataAdvancedFilters = fakeData.filterArray01
		const dataColumnFilters = null
		const dataSearchBarFilters = null
		const hasFiltersActive = true
		const searchableColumns = fakeData.searchableColumns01
		const wrapper = render(QTableActiveFilters, {
			global,
			props: {
				searchableColumns: searchableColumns,
				advancedFilters: dataAdvancedFilters,
				columnFilters: dataColumnFilters,
				searchBarFilters: dataSearchBarFilters,
				hasFiltersActive: hasFiltersActive,
				texts: tableTest.texts
			}
		})
		const idx = 0

		await nextTick()

		// Get buttons in dropdown
		const buttons = await wrapper.findAllByRole('button')
		// Click button
		await fireEvent.click(buttons[idx])
		// Check emit
		expect(wrapper.emitted()).toHaveProperty('signal-component')
		expect(wrapper.emitted()['signal-component'][0][0]).toStrictEqual('config')
		expect(wrapper.emitted()['signal-component'][0][1]).toStrictEqual({
			show: true,
			selectedTab: 'advanced-filters'
		})
	})

	it('Clicking column filter tag emits event to remove filter and filter key', async () => {
		const dataAdvancedFilters = null
		const dataColumnFilters = fakeData.filterHash01
		const dataSearchBarFilters = null
		const hasFiltersActive = true
		const searchableColumns = fakeData.searchableColumns01
		const wrapper = render(QTableActiveFilters, {
			global,
			props: {
				searchableColumns: searchableColumns,
				advancedFilters: dataAdvancedFilters,
				columnFilters: dataColumnFilters,
				searchBarFilters: dataSearchBarFilters,
				hasFiltersActive: hasFiltersActive,
				texts: tableTest.texts
			}
		})
		const idx = 0

		await nextTick()

		// Get buttons in dropdown
		const buttons = await wrapper.findAllByRole('button')
		// Click button
		await fireEvent.click(buttons[idx])
		// Check emit
		expect(wrapper.emitted()).toHaveProperty('remove-column-filter')
		expect(wrapper.emitted()['remove-column-filter'][0][0]).toBe(
			Object.keys(dataColumnFilters)[idx]
		)
	})

	it('Clicking searchbar filter tag emits event to remove filter and filter key', async () => {
		const dataAdvancedFilters = null
		const dataColumnFilters = null
		const dataSearchBarFilters = fakeData.filterHash02
		const hasFiltersActive = true
		const searchableColumns = fakeData.searchableColumns01
		const wrapper = render(QTableActiveFilters, {
			global,
			props: {
				searchableColumns: searchableColumns,
				advancedFilters: dataAdvancedFilters,
				columnFilters: dataColumnFilters,
				searchBarFilters: dataSearchBarFilters,
				hasFiltersActive: hasFiltersActive,
				texts: tableTest.texts
			}
		})
		const idx = 0

		await nextTick()

		// Get buttons in dropdown
		const buttons = await wrapper.findAllByRole('button')
		// Click button
		await fireEvent.click(buttons[idx])
		// Check emit
		expect(wrapper.emitted()).toHaveProperty('remove-search-bar-filter')
		expect(wrapper.emitted()['remove-search-bar-filter'][0][0]).toBe(
			Object.keys(dataSearchBarFilters)[idx]
		)
	})
})
