/**
 * @jest-environment jsdom
 */
import { nextTick } from 'vue'
import { expect, vi } from 'vitest'
import { within } from '@testing-library/dom'
import { fireEvent } from '@testing-library/vue'
import { flushPromises } from '@vue/test-utils'
import cloneDeep from 'lodash-es/cloneDeep'
import '@testing-library/jest-dom'

import { render } from './utils'
import QTable from '@/components/table/QTable.vue'
import fakeData from '../cases/Table.mock.js'

const global = {
	stubs: ['inline-svg']
}

describe('QTable.vue', () => {
	let tableTest
	beforeEach(() => tableTest = fakeData.getTableTest())

	it('Table with row data displays rows', async () => {
		const wrapper = render(QTable, {
			global,
			props: {
				rows: tableTest.rows,
				columns: tableTest.columns.value,
				config: tableTest.config,
				totalRows: tableTest.totalRows,
				groupFilters: tableTest.groupFilters,
				activeFilters: tableTest.activeFilters,
				headerLevel: 1,
				readonly: tableTest.readonly
			}
		})

		const rowCount = tableTest.rows.length

		await flushPromises()
		await vi.dynamicImportSettled()

		// Get rows
		const rows = await wrapper.getAllByTestId('table-row')

		expect(rows).toHaveLength(rowCount)
	})

	it('Table with no row data displays <empty> row', async () => {
		const wrapper = render(QTable, {
			global,
			props: {
				rows: [],
				columns: tableTest.columns.value,
				config: tableTest.config,
				totalRows: tableTest.totalRows,
				groupFilters: tableTest.groupFilters,
				activeFilters: tableTest.activeFilters,
				headerLevel: 1,
				readonly: tableTest.readonly
			}
		})

		// When it's empty, there will be a row with a placeholder.
		const rowCount = 1

		await flushPromises()
		await vi.dynamicImportSettled()

		// Get rows
		const rows = await wrapper.getAllByTestId('table-row')

		expect(rows).toHaveLength(rowCount)
		const emptyRow = await wrapper.findByText('No data to show')
		expect(emptyRow)
	})

	it('Table in normal mode displays insert button, clicking button emits insert event', async () => {
		const wrapper = render(QTable, {
			global,
			props: {
				rows: tableTest.rows,
				columns: tableTest.columns.value,
				config: tableTest.config,
				totalRows: tableTest.totalRows,
				groupFilters: tableTest.groupFilters,
				activeFilters: tableTest.activeFilters,
				headerLevel: 1,
				readonly: false
			}
		})

		const generalAction = tableTest.config.generalActions[0]

		await flushPromises()
		await vi.dynamicImportSettled()

		// Insert button
		const button = await wrapper.findByTitle(generalAction.title)
		expect(button)

		// Click insert button and check emit
		await fireEvent.click(button)

		const rowAction = await wrapper.emitted('row-action')

		expect(rowAction).toBeTruthy()
		expect(rowAction[0][0].name).toBe(generalAction.name)
		expect(rowAction[0][0].params.formName).toBe(generalAction.params.formName)
		expect(rowAction[0][0].params.mode).toBe(generalAction.params.mode)
		expect(rowAction[0][0].params.type).toBe(generalAction.params.type)
	})

	it('Table in read-only mode does not display insert button', async () => {
		const wrapper = render(QTable, {
			global,
			props: {
				rows: tableTest.rows,
				columns: tableTest.columns.value,
				config: tableTest.config,
				totalRows: tableTest.totalRows,
				groupFilters: tableTest.groupFilters,
				activeFilters: tableTest.activeFilters,
				headerLevel: 1,
				readonly: true
			}
		})

		await nextTick()

		// No insert button
		const button = await wrapper.queryByTitle(tableTest.config.generalActions[0].title)
		expect(button).toBeNull()
	})

	it('Invalid row is highlighted', async () => {
		const cssClasses = fakeData.cssClasses
		const dataRows = fakeData.rowsInvalid, dataColumns = fakeData.columns01
		const wrapper = render(QTable, {
			global,
			props: {
				rows: dataRows,
				columns: dataColumns,
				config: tableTest.config,
				totalRows: tableTest.totalRows,
				groupFilters: tableTest.groupFilters,
				activeFilters: tableTest.activeFilters,
				headerLevel: 1,
				readonly: tableTest.readonly
			}
		})
		const rowNum = 0

		const rowCount = dataRows.length

		await nextTick()

		await flushPromises()
		await vi.dynamicImportSettled()

		// Get rows
		const rows = await wrapper.getAllByTestId('table-row')

		expect(rows).toHaveLength(rowCount)

		expect(rows[rowNum]).toHaveClass(cssClasses.invalidRow)
	})

	it('Rows where "Currency" column > 100 have style "color: #00A000"', async () => {
		const wrapper = render(QTable, {
			global,
			props: {
				rows: tableTest.rows,
				columns: tableTest.columns.value,
				config: tableTest.config,
				totalRows: tableTest.totalRows,
				groupFilters: tableTest.groupFilters,
				activeFilters: tableTest.activeFilters,
				headerLevel: 1,
				readonly: tableTest.readonly
			}
		})

		await nextTick()

		await flushPromises()
		await vi.dynamicImportSettled()

		// Get rows
		const rows = await wrapper.getAllByTestId('table-row')

		expect(rows[4]).toHaveStyle('color: #00A000;')
		expect(rows[6]).toHaveStyle('color: #00A000;')
	})

	it('Rows where "Array" column = 5 have style "background-color: #E0E0E0"', async () => {
		const wrapper = render(QTable, {
			global,
			props: {
				rows: tableTest.rows,
				columns: tableTest.columns.value,
				config: tableTest.config,
				totalRows: tableTest.totalRows,
				groupFilters: tableTest.groupFilters,
				activeFilters: tableTest.activeFilters,
				headerLevel: 1,
				readonly: tableTest.readonly
			}
		})

		await nextTick()

		await flushPromises()
		await vi.dynamicImportSettled()

		// Get rows
		const rows = await wrapper.getAllByTestId('table-row')

		expect(rows[3]).toHaveStyle('background-color: #E0E0E0')
		expect(rows[5]).toHaveStyle('background-color: #E0E0E0')
	})

	it('Cells where "Val" column length > 3 have style "color: #C08000"', async () => {
		const wrapper = render(QTable, {
			global,
			props: {
				rows: tableTest.rows,
				columns: tableTest.columns.value,
				config: tableTest.config,
				totalRows: tableTest.totalRows,
				groupFilters: tableTest.groupFilters,
				activeFilters: tableTest.activeFilters,
				headerLevel: 1,
				readonly: tableTest.readonly
			}
		})

		await nextTick()

		// Get index of column with textColor property
		const columnIdx = tableTest.columnsOriginal.value.findIndex(obj => obj.textColor !== undefined)
		var domColumnIdx = columnIdx
		// Account for extra column if table has checklist
		if (tableTest.config.rowsCheckable !== undefined && tableTest.config.rowsCheckable !== false)
			domColumnIdx++

		await flushPromises()
		await vi.dynamicImportSettled()

		// Get rows
		const rows = await wrapper.getAllByTestId('table-row')

		var cells = []
		cells = await within(rows[0]).queryAllByRole('cell')
		expect(cells[domColumnIdx + 1]).toHaveStyle('color: #C08000;')
		cells = await within(rows[1]).queryAllByRole('cell')
		expect(cells[domColumnIdx + 1]).toHaveStyle('color: #C08000;')
		cells = await within(rows[2]).queryAllByRole('cell')
		expect(cells[domColumnIdx + 1]).toHaveStyle('color: #C08000;')
		cells = await within(rows[3]).queryAllByRole('cell')
		expect(cells[domColumnIdx + 1]).toHaveStyle('color: #C08000;')
		cells = await within(rows[4]).queryAllByRole('cell')
		expect(cells[domColumnIdx + 1]).toHaveStyle('color: #C08000;')
		cells = await within(rows[5]).queryAllByRole('cell')
		expect(cells[domColumnIdx + 1]).toHaveStyle('color: #C08000;')
	})

	it('Cell with column scroll has truncated text followed by (...)', async () => {
		// Copy columns and add scrollData property
		const columnsScroll = cloneDeep(tableTest.columns.value)
		columnsScroll[2].scrollData = 5

		const wrapper = render(QTable, {
			global,
			props: {
				rows: tableTest.rows,
				columns: columnsScroll,
				config: tableTest.config,
				totalRows: tableTest.totalRows,
				groupFilters: tableTest.groupFilters,
				activeFilters: tableTest.activeFilters,
				headerLevel: 1,
				readonly: tableTest.readonly
			}
		})
		const rowIdx = 2

		await nextTick()

		// Get index of column with scroll
		const columnIdx = columnsScroll.findIndex(obj => obj.scrollData !== undefined)
		var domColumnIdx = columnIdx
		// Account for extra column if table has checklist
		if (tableTest.config.rowsCheckable !== undefined && tableTest.config.rowsCheckable !== false)
			domColumnIdx++

		await flushPromises()
		await vi.dynamicImportSettled()

		// Get rows
		const rows = await wrapper.getAllByTestId('table-row')

		var cells = []
		cells = await within(rows[rowIdx]).queryAllByRole('cell')
		expect(cells[domColumnIdx + 1]).toHaveTextContent('thing (...)')
	})

	it('Table container can be focused', async () => {
		render(QTable, {
			global,
			props: {
				rows: tableTest.rows,
				columns: tableTest.columns.value,
				config: tableTest.config,
				totalRows: tableTest.totalRows,
				groupFilters: tableTest.groupFilters,
				activeFilters: tableTest.activeFilters,
				headerLevel: 1,
				isBlocked: tableTest.isBlocked
			}
		})

		await flushPromises()
		await vi.dynamicImportSettled()

		// Get table container
		const tableContainerArr = document.getElementsByClassName('table-responsive')
		const tableContainer = tableContainerArr[0]

		// Focus on table container
		tableContainer.focus()
		expect(document.activeElement === tableContainer).toBe(true)
	})

	it('Focusing on table container and pressing down focuses on the first data row', async () => {
		const wrapper = render(QTable, {
			global,
			props: {
				rows: tableTest.rows,
				columns: tableTest.columns.value,
				config: tableTest.config,
				totalRows: tableTest.totalRows,
				groupFilters: tableTest.groupFilters,
				activeFilters: tableTest.activeFilters,
				headerLevel: 1,
				isBlocked: tableTest.isBlocked
			}
		})

		await flushPromises()
		await vi.dynamicImportSettled()

		// Get table container
		const tableContainerArr = document.getElementsByClassName('table-responsive')
		const tableContainer = tableContainerArr[0]

		// Get rows
		const rows = await wrapper.getAllByRole('row')

		// Focus on table container
		tableContainer.focus()
		expect(document.activeElement === tableContainer).toBe(true)

		// Navigate to first data row
		await fireEvent.keyDown(tableContainer, { key: 'ArrowDown', keyCode: 40 })
		expect(document.activeElement === rows[1]).toBe(true)
	})

	it('Focusing on the first data row and pressing up navigates to the header row', async () => {
		const wrapper = render(QTable, {
			global,
			props: {
				rows: tableTest.rows,
				columns: tableTest.columns.value,
				config: tableTest.config,
				totalRows: tableTest.totalRows,
				groupFilters: tableTest.groupFilters,
				activeFilters: tableTest.activeFilters,
				headerLevel: 1,
				isBlocked: tableTest.isBlocked
			}
		})

		await flushPromises()
		await vi.dynamicImportSettled()

		// Get table container
		const tableContainerArr = document.getElementsByClassName('table-responsive')
		const tableContainer = tableContainerArr[0]

		// Get rows
		const rows = await wrapper.getAllByRole('row')

		// Get header row buttons
		const headerRowButtons = await within(rows[0]).getAllByRole('button')

		// Focus on first data row
		rows[1].focus()

		// Navigate to header row
		await fireEvent.keyDown(tableContainer, { key: 'ArrowUp', keyCode: 38 })
		expect(document.activeElement === headerRowButtons[0]).toBe(true)
	})

	it('Focusing on a row and pressing down navigates to the next row', async () => {
		const wrapper = render(QTable, {
			global,
			props: {
				rows: tableTest.rows,
				columns: tableTest.columns.value,
				config: tableTest.config,
				totalRows: tableTest.totalRows,
				groupFilters: tableTest.groupFilters,
				activeFilters: tableTest.activeFilters,
				headerLevel: 1,
				isBlocked: tableTest.isBlocked
			}
		})

		await flushPromises()
		await vi.dynamicImportSettled()

		// Get table container
		const tableContainerArr = document.getElementsByClassName('table-responsive')
		const tableContainer = tableContainerArr[0]

		// Get rows
		const rows = await wrapper.getAllByRole('row')

		// Focus on a row
		rows[1].focus()

		// Navigate to the next row
		await fireEvent.keyDown(tableContainer, { key: 'ArrowDown', keyCode: 40 })
		expect(document.activeElement === rows[2]).toBe(true)
	})

	it('Focusing on a row and pressing up navigates to the previous row', async () => {
		const wrapper = render(QTable, {
			global,
			props: {
				rows: tableTest.rows,
				columns: tableTest.columns.value,
				config: tableTest.config,
				totalRows: tableTest.totalRows,
				groupFilters: tableTest.groupFilters,
				activeFilters: tableTest.activeFilters,
				headerLevel: 1,
				isBlocked: tableTest.isBlocked
			}
		})

		await flushPromises()
		await vi.dynamicImportSettled()

		// Get table container
		const tableContainerArr = document.getElementsByClassName('table-responsive')
		const tableContainer = tableContainerArr[0]

		// Get rows
		const rows = await wrapper.getAllByRole('row')

		// Focus on a row
		rows[2].focus()

		// Navigate to the previous row
		await fireEvent.keyDown(tableContainer, { key: 'ArrowUp', keyCode: 38 })
		expect(document.activeElement === rows[1]).toBe(true)
	})

	it('Focusing on a row and pressing right navigates to the next focusable element in the row', async () => {
		render(QTable, {
			global,
			props: {
				rows: tableTest.rows,
				columns: tableTest.columns.value,
				config: tableTest.config,
				totalRows: tableTest.totalRows,
				groupFilters: tableTest.groupFilters,
				activeFilters: tableTest.activeFilters,
				headerLevel: 1,
				isBlocked: tableTest.isBlocked
			}
		})

		await flushPromises()
		await vi.dynamicImportSettled()

		// Get table container
		const tableContainerArr = document.getElementsByClassName('table-responsive')
		const tableContainer = tableContainerArr[0]

		// Get rows
		//const rows = await wrapper.getAllByRole('row')
		const rows = document.getElementsByTagName('TR')

		// Get row action elements
		const rowActionElems = rows[1].querySelectorAll('[data-table-action-selected]')

		// Focus on a row
		rows[1].focus()

		// Navigate to next action element
		await fireEvent.keyDown(tableContainer, { key: 'ArrowRight', keyCode: 39 })
		expect(document.activeElement === rowActionElems[0]).toBe(true)
	})

	it('Focusing on an action element in a row and pressing right navigates to the next focusable element in the row', async () => {
		render(QTable, {
			global,
			props: {
				rows: tableTest.rows,
				columns: tableTest.columns.value,
				config: tableTest.config,
				totalRows: tableTest.totalRows,
				groupFilters: tableTest.groupFilters,
				activeFilters: tableTest.activeFilters,
				headerLevel: 1,
				isBlocked: tableTest.isBlocked
			}
		})

		await flushPromises()
		await vi.dynamicImportSettled()

		// Get table container
		const tableContainerArr = document.getElementsByClassName('table-responsive')
		const tableContainer = tableContainerArr[0]

		// Get rows
		//const rows = await wrapper.getAllByRole('row')
		const rows = document.getElementsByTagName('TR')

		// Get row action elements
		const rowActionElems = rows[1].querySelectorAll('[data-table-action-selected]')

		// Focus on an action element in the row
		rowActionElems[0].focus()

		// Navigate to next action element
		await fireEvent.keyDown(tableContainer, { key: 'ArrowRight', keyCode: 39 })
		expect(document.activeElement === rowActionElems[1]).toBe(true)
	})

	it('Focusing on an action element in a row and pressing left navigates to the previous focusable element in the row', async () => {
		render(QTable, {
			global,
			props: {
				rows: tableTest.rows,
				columns: tableTest.columns.value,
				config: tableTest.config,
				totalRows: tableTest.totalRows,
				groupFilters: tableTest.groupFilters,
				activeFilters: tableTest.activeFilters,
				headerLevel: 1,
				isBlocked: tableTest.isBlocked
			}
		})

		await flushPromises()
		await vi.dynamicImportSettled()

		// Get table container
		const tableContainerArr = document.getElementsByClassName('table-responsive')
		const tableContainer = tableContainerArr[0]

		// Get rows
		//const rows = await wrapper.getAllByRole('row')
		const rows = document.getElementsByTagName('TR')

		// Get row action elements
		const rowActionElems = rows[1].querySelectorAll('[data-table-action-selected]')

		// Focus on an action element in the row
		rowActionElems[1].focus()

		// Navigate to previous action element
		await fireEvent.keyDown(tableContainer, { key: 'ArrowLeft', keyCode: 37 })
		expect(document.activeElement === rowActionElems[0]).toBe(true)
	})

	it('Focusing on an element in a row and pressing escape focuses on the table container', async () => {
		render(QTable, {
			global,
			props: {
				rows: tableTest.rows,
				columns: tableTest.columns.value,
				config: tableTest.config,
				totalRows: tableTest.totalRows,
				groupFilters: tableTest.groupFilters,
				activeFilters: tableTest.activeFilters,
				headerLevel: 1,
				isBlocked: tableTest.isBlocked
			}
		})

		await flushPromises()
		await vi.dynamicImportSettled()

		// Get table container
		const tableContainerArr = document.getElementsByClassName('table-responsive')
		const tableContainer = tableContainerArr[0]

		// Get rows
		//const rows = await wrapper.getAllByRole('row')
		const rows = document.getElementsByTagName('TR')

		// Get row action elements
		const rowActionElems = rows[1].querySelectorAll('[data-table-action-selected]')

		// Focus on an action element in the row
		rowActionElems[1].focus()

		// Navigate to tableContainer
		await fireEvent.keyDown(tableContainer, { key: 'Escape', keyCode: 27 })
		expect(document.activeElement === tableContainer).toBe(true)
	})

	it('Focusing on an element in a row and pressing tab focuses on the table container', async () => {
		render(QTable, {
			global,
			props: {
				rows: tableTest.rows,
				columns: tableTest.columns.value,
				config: tableTest.config,
				totalRows: tableTest.totalRows,
				groupFilters: tableTest.groupFilters,
				activeFilters: tableTest.activeFilters,
				headerLevel: 1,
				isBlocked: tableTest.isBlocked
			}
		})

		await flushPromises()
		await vi.dynamicImportSettled()

		// Get table container
		const tableContainerArr = document.getElementsByClassName('table-responsive')
		const tableContainer = tableContainerArr[0]

		// Get rows
		//const rows = await wrapper.getAllByRole('row')
		const rows = document.getElementsByTagName('TR')

		// Get row action elements
		const rowActionElems = rows[1].querySelectorAll('[data-table-action-selected]')

		// Focus on an action element in the row
		rowActionElems[1].focus()

		// Navigate to tableContainer
		await fireEvent.keyDown(tableContainer, { key: 'Tab', keyCode: 9 })
		expect(document.activeElement === tableContainer).toBe(true)
	})

	it('Focusing on the a row and pressing home navigates to the first row', async () => {
		const wrapper = render(QTable, {
			global,
			props: {
				rows: tableTest.rows,
				columns: tableTest.columns.value,
				config: tableTest.config,
				totalRows: tableTest.totalRows,
				groupFilters: tableTest.groupFilters,
				activeFilters: tableTest.activeFilters,
				headerLevel: 1,
				isBlocked: tableTest.isBlocked
			}
		})

		await flushPromises()
		await vi.dynamicImportSettled()

		// Get table container
		const tableContainerArr = document.getElementsByClassName('table-responsive')
		const tableContainer = tableContainerArr[0]

		// Get rows
		const rows = await wrapper.getAllByRole('row')

		// Focus on a row
		rows[4].focus()

		// Navigate to the first row
		await fireEvent.keyDown(tableContainer, { key: 'Home', keyCode: 36 })
		expect(document.activeElement === rows[1]).toBe(true)
	})

	it('Focusing on the a row and pressing end navigates to the last row', async () => {
		const wrapper = render(QTable, {
			global,
			props: {
				rows: tableTest.rows,
				columns: tableTest.columns.value,
				config: tableTest.config,
				totalRows: tableTest.totalRows,
				groupFilters: tableTest.groupFilters,
				activeFilters: tableTest.activeFilters,
				headerLevel: 1,
				isBlocked: tableTest.isBlocked
			}
		})

		const rowCount = tableTest.rows.length

		await flushPromises()
		await vi.dynamicImportSettled()

		// Get table container
		const tableContainerArr = document.getElementsByClassName('table-responsive')
		const tableContainer = tableContainerArr[0]

		// Get rows
		const rows = await wrapper.getAllByRole('row')

		// Focus on a row
		rows[4].focus()

		// Navigate to the last row
		await fireEvent.keyDown(tableContainer, { key: 'End', keyCode: 35 })
		expect(document.activeElement === rows[rowCount]).toBe(true)
	})

	it('Focusing on a row and pressing delete emits row action to delete', async () => {
		const wrapper = render(QTable, {
			global,
			props: {
				rows: tableTest.rows,
				columns: tableTest.columns.value,
				config: tableTest.config,
				totalRows: tableTest.totalRows,
				groupFilters: tableTest.groupFilters,
				activeFilters: tableTest.activeFilters,
				headerLevel: 1,
				isBlocked: tableTest.isBlocked
			}
		})

		await flushPromises()
		await vi.dynamicImportSettled()

		const rowNum = 1

		// Get rows
		const rows = await wrapper.getAllByRole('row')

		// Focus on a row
		rows[rowNum].focus()
		expect(document.activeElement === rows[1]).toBe(true)

		// Press delete key
		await fireEvent.keyDown(rows[rowNum], { key: 'Delete', keyCode: 46 })

		const rowAction = await wrapper.emitted('row-action')

		expect(rowAction).toBeTruthy()
		expect(rowAction[0][0].id).toBe('delete')
		expect(rowAction[0][0].rowKeyPath).toBeTruthy()
	})

	it('Focusing on a sub element in a row and pressing delete does not emit row action to delete', async () => {
		const wrapper = render(QTable, {
			global,
			props: {
				rows: tableTest.rows,
				columns: tableTest.columns.value,
				config: tableTest.config,
				totalRows: tableTest.totalRows,
				groupFilters: tableTest.groupFilters,
				activeFilters: tableTest.activeFilters,
				headerLevel: 1,
				isBlocked: tableTest.isBlocked
			}
		})

		await flushPromises()
		await vi.dynamicImportSettled()

		const rowNum = 1

		// Get rows
		const rows = await wrapper.getAllByRole('row')

		// Get row action elements
		const rowActionElems = rows[rowNum].querySelectorAll('[data-table-action-selected]')

		// Focus on an action element in the row
		rowActionElems[0].focus()

		// Press delete key
		await fireEvent.keyDown(rowActionElems[0], { key: 'Delete', keyCode: 46 })

		const rowAction = await wrapper.emitted('row-action')

		expect(rowAction).toBeFalsy()
	})
})
