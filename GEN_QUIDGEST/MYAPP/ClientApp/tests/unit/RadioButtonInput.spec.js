/**
 * @jest-environment jsdom
 */
import '@testing-library/jest-dom'
import { mount } from '@vue/test-utils'
import { fireEvent } from '@testing-library/vue'
import userEvent from '@testing-library/user-event'

import { render } from './utils'
import RadioSelectList from '@/components/inputs/RadioButtonInput'
import BaseInputStructure from '@/components/inputs/BaseInputStructure'

describe('RadioButtonInput.vue', () => {
	it('render the radio select list', async () => {
		const wrapper = mount(RadioSelectList, {
			props: {
				optionsList: [
					{
						value: 'Rails',
						key: '1'
					},
					{
						value: 'Django',
						key: '2'
					},
					{
						value: 'Angular',
						key: '3'
					},
					{
						value: 'Vue.js',
						key: '4'
					},
					{
						value: 'React',
						key: '5'
					}
				]
			}
		})
		expect(wrapper.findAll('.i-radio__control').length).toBe(1)
	})

	it('check the radio select list N items 1 column', async () => {
		const wrapper = mount(RadioSelectList, {
			props: {
				id: 'radiogroup',
				numberOfColumns: 1,
				optionsList: [
					{
						value: 'Rails',
						key: '1'
					},
					{
						value: 'Django',
						key: '2'
					},
					{
						value: 'Angular',
						key: '3'
					},
					{
						value: 'Vue.js',
						key: '4'
					},
					{
						value: 'React',
						key: '5'
					},
					{
						value: 'Python',
						key: '6'
					},
					{
						value: 'C++',
						key: '7'
					},
					{
						value: 'C',
						key: '8'
					},
					{
						value: 'Pearl',
						key: '9'
					},
					{
						value: 'Java',
						key: '10'
					}
				]
			}
		})
		expect(wrapper.findAll('.column').length).toBe(1)
	})

	it('check the radio select list N items N column', async () => {
		const wrapper = mount(RadioSelectList, {
			props: {
				id: 'radiogroup',
				numberOfColumns: 5,
				optionsList: [
					{
						value: 'Rails',
						key: '1'
					},
					{
						value: 'Django',
						key: '2'
					},
					{
						value: 'Angular',
						key: '3'
					},
					{
						value: 'Vue.js',
						key: '4'
					},
					{
						value: 'React',
						key: '5'
					},
					{
						value: 'Python',
						key: '6'
					},
					{
						value: 'C++',
						key: '7'
					},
					{
						value: 'C',
						key: '8'
					},
					{
						value: 'Pearl',
						key: '9'
					},
					{
						value: 'Java',
						key: '10'
					}
				]
			}
		})
		expect(wrapper.findAll('.column').length).toBe(5)
	})

	it('check the radio select list 10 items 5 column', async () => {
		const wrapper = mount(RadioSelectList, {
			props: {
				id: 'radiogroup',
				numberOfColumns: 5,
				optionsList: [
					{
						value: 'Rails',
						key: '1'
					},
					{
						value: 'Django',
						key: '2'
					},
					{
						value: 'Angular',
						key: '3'
					},
					{
						value: 'Vue.js',
						key: '4'
					},
					{
						value: 'React',
						key: '5'
					},
					{
						value: 'Python',
						key: '6'
					},
					{
						value: 'C++',
						key: '7'
					},
					{
						value: 'C',
						key: '8'
					},
					{
						value: 'Pearl',
						key: '9'
					},
					{
						value: 'Java',
						key: '10'
					}
				]
			}
		})
		expect(wrapper.findAll('.column').length).toBe(5)
		expect(wrapper.findAll('.column label').length).toBe(10)
	})

	it('check the radio select list 10 items 9 column', async () => {
		const wrapper = mount(RadioSelectList, {
			props: {
				id: 'radiogroup',
				numberOfColumns: 9,
				optionsList: [
					{
						value: 'Rails',
						key: '1'
					},
					{
						value: 'Django',
						key: '2'
					},
					{
						value: 'Angular',
						key: '3'
					},
					{
						value: 'Vue.js',
						key: '4'
					},
					{
						value: 'React',
						key: '5'
					},
					{
						value: 'Python',
						key: '6'
					},
					{
						value: 'C++',
						key: '7'
					},
					{
						value: 'C',
						key: '8'
					},
					{
						value: 'Pearl',
						key: '9'
					},
					{
						value: 'Java',
						key: '10'
					}
				]
			}
		})
		expect(wrapper.findAll('.column').length).toBe(9)
		expect(wrapper.findAll('.column label').length).toBe(10)
	})

	it('check the radio select list 9 items 3 column', async () => {
		const wrapper = mount(RadioSelectList, {
			props: {
				id: 'radiogroup',
				numberOfColumns: 3,
				optionsList: [
					{
						value: 'Rails',
						key: '1'
					},
					{
						value: 'Django',
						key: '2'
					},
					{
						value: 'Angular',
						key: '3'
					},
					{
						value: 'Vue.js',
						key: '4'
					},
					{
						value: 'React',
						key: '5'
					},
					{
						value: 'Python',
						key: '6'
					},
					{
						value: 'C++',
						key: '7'
					},
					{
						value: 'C',
						key: '8'
					},
					{
						value: 'Pearl',
						key: '9'
					}
				]
			}
		})
		expect(wrapper.findAll('.column').length).toBe(3)
		expect(wrapper.findAll('.column label').length).toBe(9)
	})

	it('check the radio select list 9 items 4 column', async () => {
		const wrapper = mount(RadioSelectList, {
			props: {
				id: 'radiogroup',
				numberOfColumns: 3,
				optionsList: [
					{
						value: 'Rails',
						key: '1'
					},
					{
						value: 'Django',
						key: '2'
					},
					{
						value: 'Angular',
						key: '3'
					},
					{
						value: 'Vue.js',
						key: '4'
					},
					{
						value: 'React',
						key: '5'
					},
					{
						value: 'Python',
						key: '6'
					},
					{
						value: 'C++',
						key: '7'
					},
					{
						value: 'C',
						key: '8'
					},
					{
						value: 'Pearl',
						key: '9'
					}
				]
			}
		})
		expect(wrapper.findAll('.column').length).toBe(3)
		expect(wrapper.findAll('.column label').length).toBe(9)
	})

	it('check the radio select list 5 items 10 column', async () => {
		const wrapper = mount(RadioSelectList, {
			props: {
				id: 'radiogroup',
				numberOfColumns: 10,
				optionsList: [
					{
						value: 'Rails',
						key: '1'
					},
					{
						value: 'Django',
						key: '2'
					},
					{
						value: 'Angular',
						key: '3'
					},
					{
						value: 'Vue.js',
						key: '4'
					},
					{
						value: 'React',
						key: '5'
					}
				]
			}
		})
		expect(wrapper.findAll('.column').length).toBe(10)
		expect(wrapper.findAll('.column label').length).toBe(5)
	})

	it('check and verify selected value of radio select', async () => {
		const wrapper = render(RadioSelectList, {
			props: {
				optionsList: [
					{
						value: 'Rails',
						key: '1'
					},
					{
						value: 'Django',
						key: '2'
					},
					{
						value: 'Angular',
						key: '3'
					},
					{
						value: 'Vue.js',
						key: '4'
					},
					{
						value: 'React',
						key: '5'
					}
				]
			}
		})
		const radioButton = await wrapper.findByTestId('radio_label_5')
		await radioButton.click()
		expect(wrapper).toEmitModelValue('5')
		expect(radioButton.checked).toBeTruthy()
	})

	it('check on initialisation none of the value is selected', async () => {
		const wrapper = render(RadioSelectList, {
			props: {
				numberOfColumns: 1,
				optionsList: [
					{
						value: 10,
						key: '1'
					},
					{
						value: 20,
						key: '2'
					}
				]
			}
		})
		const radioButton = await wrapper.findByTestId('radio_label_1')
		expect(radioButton.checked).not.toBeTruthy()
		const radioButton2 = await wrapper.findByTestId('radio_label_2')
		expect(radioButton2.checked).not.toBeTruthy()
	})

	it('check number of columns of radio select', async () => {
		const wrapper = mount(RadioSelectList, {
			props: {
				numberOfColumns: 2,
				optionsList: [
					{
						value: 'Rails',
						key: '1'
					},
					{
						value: 'Django',
						key: '2'
					},
					{
						value: 'Angular',
						key: '3'
					},
					{
						value: 'Vue.js',
						key: '4'
					}
				]
			}
		})
		expect(wrapper.findAll('.column').length).toBe(2)
	})

	it('check deselect functionality to radio select', async () => {
		const wrapper = render(RadioSelectList, {
			props: {
				numberOfColumns: 2,
				deselectRadio: true,
				modelValue: '5',
				optionsList: [
					{
						value: 'Rails',
						key: '1'
					},
					{
						value: 'Django',
						key: '2'
					},
					{
						value: 'Angular',
						key: '3'
					},
					{
						value: 'Vue.js',
						key: '4'
					},
					{
						value: 'React',
						key: '5'
					}
				]
			}
		})
		const valueInput = await wrapper.findByTestId('radio_label_5')
		expect(valueInput.checked).toBeTruthy()
		await valueInput.click()
		expect(wrapper).toEmitModelValue(undefined)
	})

	it('check deselect functionality to radio select when deselect is false', async () => {
		const wrapper = render(RadioSelectList, {
			props: {
				numberOfColumns: 2,
				deselectRadio: false,
				optionsList: [
					{
						value: 'Rails',
						key: '1'
					},
					{
						value: 'Django',
						key: '2'
					},
					{
						value: 'Angular',
						key: '3'
					},
					{
						value: 'Vue.js',
						key: '4'
					},
					{
						value: 'React',
						key: '5'
					}
				]
			}
		})
		const radioButton = await wrapper.findByTestId('radio_label_5')
		await fireEvent.update(radioButton, { target: { value: 'React' } })
		expect(radioButton.checked).toBeTruthy()
		await fireEvent.click(radioButton, { target: { value: 'React' } })
		expect(radioButton.checked).toBeTruthy()
	})

	it('check deselect functionality to radio select with backspace key', async () => {
		const wrapper = render(RadioSelectList, {
			props: {
				numberOfColumns: 2,
				deselectRadio: true,
				modelValue: '5',
				optionsList: [
					{
						value: 'Rails',
						key: '1'
					},
					{
						value: 'Django',
						key: '2'
					},
					{
						value: 'Angular',
						key: '3'
					},
					{
						value: 'Vue.js',
						key: '4'
					},
					{
						value: 'React',
						key: '5'
					}
				]
			}
		})
		const valueInput = await wrapper.findByTestId('radio_label_5')
		expect(valueInput.checked).toBeTruthy()
		await fireEvent.keyUp(valueInput, { key: 'Backspace' })
		expect(wrapper).toEmitModelValue(undefined)
	})

	it('check deselect functionality to radio select with delete key', async () => {
		const wrapper = render(RadioSelectList, {
			props: {
				numberOfColumns: 2,
				deselectRadio: true,
				modelValue: '5',
				optionsList: [
					{
						value: 'Rails',
						key: '1'
					},
					{
						value: 'Django',
						key: '2'
					},
					{
						value: 'Angular',
						key: '3'
					},
					{
						value: 'Vue.js',
						key: '4'
					},
					{
						value: 'React',
						key: '5'
					}
				]
			}
		})
		const valueInput = await wrapper.findByTestId('radio_label_5')
		expect(valueInput.checked).toBeTruthy()
		await fireEvent.keyUp(valueInput, { key: 'Delete' })
		expect(wrapper).toEmitModelValue(undefined)
	})

	it('check document tab order for radio select list and selection using arrow', async () => {
		const wrapper = render(RadioSelectList, {
			props: {
				numberOfColumns: 1,
				optionsList: [
					{
						value: 'Rails',
						key: '1'
					},
					{
						value: 'Django',
						key: '2'
					},
					{
						value: 'Angular',
						key: '3'
					},
					{
						value: 'Vue.js',
						key: '4'
					},
					{
						value: 'React',
						key: '5'
					}
				]
			}
		})
		const radioButton = await wrapper.findByTestId('radio_label_1')
		expect(document.body).toHaveFocus()
		await userEvent.tab()
		expect(radioButton).toHaveFocus()
		const radioButton2 = await wrapper.findByTestId('radio_label_2')
		await userEvent.type(radioButton2, '{down}')
		expect(radioButton2.checked).toBeTruthy()
	})

	it('check number of columns is 0', async () => {
		const wrapper = mount(RadioSelectList, {
			props: {
				numberOfColumns: 0,
				optionsList: []
			}
		})
		expect(wrapper.findAll('.column').length).toBe(0)
	})

	it('check number of columns is negative', async () => {
		const wrapper = mount(RadioSelectList, {
			props: {
				numberOfColumns: -2,
				optionsList: []
			}
		})
		expect(wrapper.findAll('.column').length).toBe(0)
	})

	it('verify default selected option value for radio select list', () => {
		let initial = '2'
		const wrapper = mount(RadioSelectList, {
			props: {
				optionsList: [
					{
						value: 'Rails',
						key: '1'
					},
					{
						value: 'Django',
						key: '2'
					}
				],
				modelValue: '2'
			}
		})
		const divArray = wrapper.componentVM.optionsList
		const element = divArray.find((item) => item.key === '2')
		expect(element.key).toMatch(initial)
	})

	it('verify on change selected value for radio select list', async () => {
		const wrapper = render(RadioSelectList, {
			props: {
				optionsList: [
					{
						value: 'Rails',
						key: '1'
					},
					{
						value: 'Django',
						key: '2'
					}
				],
				modelValue: '2'
			}
		})
		const radioButton = await wrapper.findByTestId('radio_label_2')
		expect(radioButton.checked).toBeTruthy()
		const radioButton2 = await wrapper.findByTestId('radio_label_1')
		await radioButton2.click()
		expect(radioButton2.checked).toBeTruthy()
		expect(wrapper).toEmitModelValue('1')
	})

	it('validation of options which does not include value object and gives warning', () => {
		const inValidOptionsArray = [
			{
				value: 'Rails',
				key: '1'
			},
			{
				value: 'Django',
				key: '2'
			},
			{
				value: 'Angular',
				key: '3'
			},
			{
				key: '4',
			}
		]
		const validOptionsArray = [
			{
				value: 'Rails',
				key: '1'
			},
			{
				value: 'Django',
				key: '2'
			},
			{
				value: 'Angular',
				key: '3'
			},
			{
				value: 'Vue.js',
				key: '4'
			}
		]
		const validator = RadioSelectList.props.optionsList.validator
		expect(validator(inValidOptionsArray)).not.toBeTruthy()
		expect(validator(validOptionsArray)).toBeTruthy()
	})

	it('verify radio button has label and clickable', async () => {
		const { getByText, findByTestId } = render({
			components: { RadioSelectList, BaseInputStructure },
			template: `
				<div>
					<BaseInputStructure label="Teste" labelPosition="" id="test">
					<RadioSelectList :optionsList="[
						{ value: 'Rails', key: '1' },
						{ value: 'Django', key: '2' }
					]" modelValue="2" />
					</BaseInputStructure>
				</div>
			`
		})
		const radioButton = await findByTestId('radio_label_2')
		await userEvent.click(radioButton)

		expect(radioButton.checked).toBeTruthy()
		expect(getByText('Teste')).toBeInTheDocument()
	})
})
