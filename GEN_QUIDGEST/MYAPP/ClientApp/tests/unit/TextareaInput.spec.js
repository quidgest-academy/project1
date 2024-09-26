/**
 * @jest-environment jsdom
 */
import '@testing-library/jest-dom'
import { fireEvent, render } from '@testing-library/vue'
import { mount } from '@vue/test-utils'
import userEvent from '@testing-library/user-event'

import TextareaInput from '@/components/inputs/TextareaInput.vue'

describe('TextareaInput.vue', () => {
	it('render the correct model value', async () => {
		let m = 'Text area sample'
		const wrapper = render(TextareaInput, {
			props: {
				modelValue: m,
				isRequired: true
			}
		})
		const textareaInput = await wrapper.findByRole('textbox')
		expect(textareaInput).toHaveValue(m)
	})

	it('update existing model value', async () => {
		let m = 'Text area sample'
		const wrapper = render(TextareaInput, {
			props: {
				modelValue: m
			}
		})
		const textareaInput = await wrapper.findByRole('textbox')
		await fireEvent.update(textareaInput, '')
		await userEvent.type(
			textareaInput,
			'changed Text'
		)
		expect(textareaInput).toHaveValue('changed Text')
	})

	it('verify long text for text area', async () => {
		
		const user = userEvent.setup({
			delay: null
		})
		
		let m = 'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry\'s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.'
		const wrapper = render(TextareaInput, {
			props: {
				modelValue: m,
				size: 'large'
			}
		})
		const textareaInput = await wrapper.findByRole('textbox')
		
		await fireEvent.update(textareaInput, '')
		await user.type(
			textareaInput,
			'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry\'s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.'
		)
		expect(textareaInput).toHaveValue(m)
	})

	it('verify number of visible lines of text area', async () => {
		let m = 3
		let sample = 'Lorem Ipsum is simply dummy text of the printing and typesetting industry.'
		const wrapper = mount(TextareaInput, {
			props: {
				modelValue: sample,
				rows: 3
			}
		})
		expect(wrapper.componentVM.rows).toEqual(m)
	})

	it('verify disabled text area', async () => {
		let m = 'Text area sample'
		const wrapper = render(TextareaInput, {
			props: {
				modelValue: m,
				disabled: true
			}
		})
		const textareaInput = await wrapper.findByRole('textbox')
		await fireEvent.update(textareaInput, '')
		await userEvent.type(
			textareaInput,
			'changed Text'
		)
		expect(textareaInput).toHaveValue('')
	})

	it('verify readonly text area', async () => {
		let m = 'Text area sample'
		const wrapper = render(TextareaInput, {
			props: {
				modelValue: m,
				readonly: true
			}
		})
		const textareaInput = await wrapper.findByRole('textbox')
		await fireEvent.update(textareaInput, '')
		await userEvent.type(
			textareaInput,
			'changed Text'
		)
		expect(textareaInput).toHaveValue('')
	})

	it('check text area size property', async () => {
		let m = 'Text area sample'
		let sizeOfComponent = 'small'
		const wrapper = mount(TextareaInput, {
			props: {
				modelValue: m,
				size: 'small'
			}
		})
		expect(wrapper.componentVM.size).toEqual(sizeOfComponent)
	})

	it('verify accessibility for text area', async () => {
		let m = 'Text area sample'
		const wrapper = render(TextareaInput, {
			props: {
				modelValue: m,
				size: 'small'
			}
		})
		const textareaInput = await wrapper.findByRole('textbox')
		expect(document.body).toHaveFocus()
		await userEvent.tab()
		expect(textareaInput).toHaveFocus()
	})
})
