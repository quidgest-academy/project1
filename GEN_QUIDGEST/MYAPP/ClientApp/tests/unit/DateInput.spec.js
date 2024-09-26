/**
 * @jest-environment jsdom
 */
import { mount } from './utils'

import QDateTimePicker from '@/components/inputs/QDateTimePicker.vue'

describe('QDateTimePicker.vue', () => {
	let wrapper

	beforeEach(() => {
		wrapper = mount(QDateTimePicker, {
			propsData: {
				id: 'CTRL_1',
				format: 'date'
			}
		})
	})

	afterEach(() => {
		wrapper.unmount()
	})

	it('Checks the componet is rendering', () => {
		expect(wrapper.exists()).toBe(true)
	})
})
