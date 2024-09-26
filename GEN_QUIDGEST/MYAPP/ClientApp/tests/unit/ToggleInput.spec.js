/**
 * @jest-environment jsdom
 */
import '@testing-library/jest-dom'
import { render } from '@testing-library/vue'
import userEvent from '@testing-library/user-event'

import ToggleInput from '@/components/inputs/ToggleInput.vue'

describe('ToggleInput.vue', () => {
	it('renders the correct model value', async () => {
		const wrapper = render(ToggleInput, {
			props: {
				modelValue: true,
				trueLabel: 'yes',
				falseLabel: 'no'
			}
		})

		const toggleInput = await wrapper.findByRole("checkbox")
		expect(toggleInput).toBeChecked()
		wrapper.getByText('yes')
		wrapper.getByText('no')
	})

	it('toggles model value when clicking switch', async () => {
		const wrapper = render(ToggleInput, {
			props: {
				modelValue: true
			}
		})

		const toggler = await wrapper.getByTestId('switch')
		await userEvent.click(toggler)
		expect(wrapper).toEmitModelValue(false)

		await wrapper.rerender({modelValue: false})
		await userEvent.click(toggler)
		expect(wrapper).toEmitModelValue(true)
	})

	it('changes model value when clicking labels', async () => {
		const wrapper = render(ToggleInput, {
			props: {
				modelValue: true,
				trueLabel: 'yes',
				falseLabel: 'no'
			}
		})

		const falseLabel = await wrapper.getByText('no')
		await userEvent.click(falseLabel)
		expect(wrapper).toEmitModelValue(false)

		const trueLabel = await wrapper.getByText('yes')
		await userEvent.click(trueLabel)
		expect(wrapper).toEmitModelValue(true)
	})

	it('ignores clicks in disabled mode', async () => {
		const wrapper = render(ToggleInput, {
			props: {
				modelValue: true,
				disabled: true,
				trueLabel: 'yes',
				falseLabel: 'no'
			}
		})

		const toggleInput = await wrapper.findByRole("checkbox")
		const falseLabel = await wrapper.getByText('no')
		await userEvent.click(falseLabel)
		expect(toggleInput).toBeChecked()
		expect(wrapper).not.toEmitModelValue(false)
	})

	it('ignores clicks in readonly mode', async () => {
		const wrapper = render(ToggleInput, {
			props: {
				modelValue: true,
				readonly: true,
				trueLabel: 'yes',
				falseLabel: 'no'
			}
		})

		const toggleInput = await wrapper.findByRole("checkbox")
		const falseLabel = await wrapper.getByText('no')
		await userEvent.click(falseLabel)
		expect(toggleInput).toBeChecked()
		expect(wrapper).not.toEmitModelValue(false)
	})
})
