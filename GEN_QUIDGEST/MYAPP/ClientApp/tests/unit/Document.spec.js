/**
 * @jest-environment jsdom
 */
import { fireEvent } from '@testing-library/vue'
import userEvent from '@testing-library/user-event'
import { flushPromises } from '@vue/test-utils'
import { vi } from 'vitest'

import { render } from './utils'

import QDocument from '@/components/inputs/document/QDocument.vue'
import fakeData from '../cases/Document.mock.js'

/**
 * Object that maps the events to the id of the option that triggers them.
 */
const documentEventMap = {
	'get-file': 'document-download',
	'get-properties': 'document-properties'
}

/**
 * Renders the "QDocument" component.
 * @param {object} props Extra props to add to the component
 * @returns The rendered component.
 */
function renderDocument(props = {})
{
	return render(QDocument, {
		props: {
			fileProperties: fakeData.simpleUsage().fileProperties,
			versions: fakeData.simpleUsage().versionsObj,
			versionsInfo: fakeData.simpleUsage().versionsInfo,
			resourcesPath: fakeData.simpleUsage().resourcesPath,
			...props
		}
	})
}

/**
 * Opens the options menu of the specified component.
 * @param {object} component The "QDocument" component
 */
async function openDocumentOptions(component)
{
	const optionsButton = component.getByTestId('options-button')
	await fireEvent.click(optionsButton)
	await flushPromises()
	await vi.dynamicImportSettled()
}

/**
 * Uploads a new file to the specified component.
 * @param {object} component The "QDocument" component
 * @param {object} file The file to upload
 */
async function uploadFile(component, file)
{
	const fileInput = component.getByTestId('file-input')
	await userEvent.upload(fileInput, file)
}

describe('QDocument.vue', () => {
	const file = new File(['This is a test file!'], 'Test.txt', { type: 'text/plain' })

	it('Options are disabled if the value is empty', async () => {
		const wrapper = renderDocument()
		await openDocumentOptions(wrapper)

		const downloadOption = wrapper.getByTestId('document-download')
		const attachOption = wrapper.getByTestId('document-attach')
		const deleteOption = wrapper.getByTestId('document-delete')
		const editOption = wrapper.queryByTestId('document-edit')

		expect(downloadOption.disabled).toBeTruthy()
		expect(deleteOption.disabled).toBeTruthy()
		expect(attachOption).not.toBeNull()
		expect(editOption).toBeNull()
	})

	it('New file is successfully uploaded', async () => {
		const wrapper = renderDocument()
		await uploadFile(wrapper, file)

		const docFile = await wrapper.asyncEmitted('submit-file')
		expect(docFile).toBeTruthy()
	})

	it('File with invalid size doesn\'t get uploaded', async () => {
		const wrapper = renderDocument({ maxFileSize: 15 })
		await uploadFile(wrapper, file)

		const docFile = await wrapper.asyncEmitted('submit-file')
		expect(docFile).not.toBeTruthy()
	})

	it.each(Object.keys(documentEventMap))('The "%s" event is emitted when the option is clicked', async (event) => {
		const wrapper = renderDocument({ modelValue: file.name })
		await openDocumentOptions(wrapper)

		const button = wrapper.getByTestId(documentEventMap[event])
		await fireEvent.click(button)

		expect(wrapper.emitted()).toHaveProperty(event)
	})
})
