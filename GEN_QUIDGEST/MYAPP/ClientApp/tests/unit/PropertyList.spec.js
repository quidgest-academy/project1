/**
 * @jest-environment jsdom
 */
import '@testing-library/jest-dom'
import { nextTick } from 'vue'
import { fireEvent } from '@testing-library/vue'
import { render } from './utils'

import QPropertyList from '@/components/property-list/QPropertyList.vue'
import fakeData from '../cases/PropertyList.mock.js'

const global = {
	stubs: ['inline-svg']
}

//--------------
// FIXME: skipping these tests until QPropertyList can be reworked
//--------------

describe.skip('QPropertyList.vue', () => {
	let wrapper = null

	beforeEach(() => {
		wrapper = render(QPropertyList, {
			global,
			props: {
				rows: fakeData.rows,
				maxCharacters: 15,
				editPropertymenuItems: fakeData.menuItems,
				typeSelectOptions: fakeData.typeSelectOptions,
				groupingField: 'type',
				primaryKeyColumnName: 'PrimaryKey',
				valueColumnName: 'Val'
			}
		})
	})

	it('By default list is in edit mode', async () => {
		const editButton = wrapper.getByTitle('Edit')
		expect(editButton.getAttribute('aria-hidden') === 'false').toBeTruthy()
	})

	it('On click add, dropdown should appear', async () => {
		const addButton = wrapper.getByTitle('Add')
		await fireEvent.click(addButton)
		await nextTick()
		expect(wrapper.getAllByRole('listbox')[0].getAttribute('aria-expanded') === 'true').toBeTruthy()
	})

	it('On click edit control should go into edit mode', async () => {
		const editButton = wrapper.getAllByTitle('Edit')
		await fireEvent.click(editButton[0])
		await nextTick()
		const dropDown = wrapper.getAllByTitle('Menu')
		expect(dropDown[0].getAttribute('aria-hidden') === 'false').toBeTruthy()
	})

	it('On click select type bottom row should appear', async () => {
		const addButton = wrapper.getAllByTitle('Add')
		await fireEvent.click(addButton[0])
		await nextTick()
		await fireEvent.click(wrapper.getAllByRole('listbox')[0].children[0])
		await nextTick()
		const rows = wrapper.getAllByRole('row')
		const addRow = rows[9]
		expect(addRow.getAttribute('aria-hidden') === 'false').toBeTruthy()
	})

	it('On click add, event should emit', async () => {
		const addDropdown = wrapper.getAllByTitle('Add')
		await fireEvent.click(addDropdown[0])
		await nextTick()
		await fireEvent.click(wrapper.getAllByRole('listbox')[0].children[0])
		await nextTick()

		const addButton = wrapper.getAllByTitle('Add')[1]
		await fireEvent.click(addButton)
		expect(wrapper.emitted()['add-property']).toBeTruthy()
	})

	it('Check grouping on specific field', async () => {
		const groupbutton = wrapper.getByTitle('Group')
		await fireEvent.click(groupbutton)
		await nextTick()
		const rows = wrapper.getAllByRole('row')
		expect(rows.length === 6).toBeTruthy()
	})
})

describe.skip('Property List with different props object', () => {
	it('List should not be in edit mode when readonly is true', async () => {
		const wrapper = render(QPropertyList, {
			global,
			props: {
				rows: fakeData.rows,
				editPropertymenuItems: fakeData.menuItems,
				typeSelectOptions: fakeData.typeSelectOptions,
				maxCharacters: 15,
				groupingField: 'type',
				primaryKeyColumnName: 'PrimaryKey',
				readonly: true,
				valueColumnName: 'Val'
			}
		})
		await nextTick()
		const editButton = wrapper.getByTitle('Edit')
		expect(editButton.getAttribute('aria-hidden') === 'true').toBeTruthy()
	})
})
