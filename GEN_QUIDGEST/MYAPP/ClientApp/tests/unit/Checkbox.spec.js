/**
 * @jest-environment jsdom
 */
import '@testing-library/jest-dom'
import userEvent from '@testing-library/user-event'

import { render } from './utils'
import CheckBoxInput from '@/components/inputs/CheckBoxInput'
import BaseInputStructure from '@/components/inputs/BaseInputStructure'

describe('CheckBoxInput.vue', () => {
	it('Changes the model value when clicking', async () => {
		const wrapper = render(CheckBoxInput, {
			props: {
				modelValue: true,
				dataTestid: 'checkbox'
			}
		})

		const checkbox = await wrapper.getByTestId('checkbox')
		await userEvent.click(checkbox)
		expect(wrapper).toEmitModelValue(false)
		expect(checkbox).not.toBeChecked()

		await wrapper.rerender({ modelValue: false })
		await userEvent.click(checkbox)
		expect(wrapper).toEmitModelValue(true)
		expect(checkbox).toBeChecked()
	})

	it('Ignores clicks in disabled mode', async () => {
		const wrapper = render(CheckBoxInput, {
			props: {
				modelValue: true,
				disabled: true,
				dataTestid: 'checkbox'
			}
		})

		const checkbox = await wrapper.getByTestId('checkbox')
		await userEvent.click(checkbox)
		expect(checkbox).toBeChecked()
	})

	it('Ignores clicks in readonly mode', async () => {
		const wrapper = render(CheckBoxInput, {
			props: {
				modelValue: true,
				readonly: true,
				dataTestid: 'checkbox'
			}
		})

		const checkbox = await wrapper.getByTestId('checkbox')
		await userEvent.click(checkbox)
		expect(checkbox).toBeChecked()
	})

	it('Click in checkbox with label', async () => {
		const wrapper = render({
			components: { CheckBoxInput, BaseInputStructure },
			template: `
				<div>
					<BaseInputStructure id="checkbox-test" label="Teste" :hasLabel="true" labelPosition="left">
						<CheckBoxInput :modelValue="false" dataTestid="checkbox" />
					</BaseInputStructure>
				</div>
			`
		})

		const label = wrapper.getByText('Teste')
		expect(label).toBeInTheDocument()
		const checkbox = await wrapper.getByTestId('checkbox')
		await userEvent.click(checkbox)
		expect(checkbox.checked).toBe(true)
	})
})
