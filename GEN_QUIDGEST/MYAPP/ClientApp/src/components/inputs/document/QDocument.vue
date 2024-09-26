<template>
	<div :id="controlId">
		<div class="q-document__container">
			<q-input-group :size="size">
				<q-text-field
					:model-value="modelValue"
					readonly
					data-testid="document-input"
					:class="['q-document__field', { 'q-document__field-empty': !modelValue && (readonly || disabled) }]"
					:aria-labelledby="labelId"
					@click="handleFieldClick" />

				<template #append>
					<q-button
						ref="optionsButton"
						data-testid="options-button"
						b-style="secondary"
						aria-haspopup="true"
						:title="texts.actionLabel"
						:disabled="isOptionsButtonDisabled"
						@click="toggleDropdown">
						<q-icon icon="more-items" />
					</q-button>
				</template>
			</q-input-group>

			<input
				:id="`q-document-file-${controlId}`"
				ref="fileAttach"
				class="q-document__attach"
				type="file"
				data-testid="file-input"
				:accept="extensions"
				@change="attachFile" />

			<div
				v-if="!disabled && !readonly && versioning && editing"
				class="q-document__editing">
				<q-icon icon="information" /> {{ texts.editingDocument }}
			</div>
		</div>

		<q-document-dropdown
			v-if="!disabled && showOptions"
			:model-value="modelValue"
			:anchor="$refs.optionsButton.$el"
			:texts="texts"
			:readonly="readonly"
			:editing="editing"
			:versioning="versioning"
			:versions="orderedVersions"
			:uses-templates="usesTemplates"
			@attach-file="triggerFileAttach"
			@close="toggleDropdown"
			@delete-file="confirmFileDelete"
			@delete-history="confirmDeleteHistory"
			@delete-last="confirmDeleteLast"
			@edit-file="editFile"
			@get-file="getFile"
			@get-properties="getProperties"
			@get-version-history="viewAllVersions"
			@show-templates-popup="createDocument"
			@submit-file="setFileSubmitModalState(true)" />

		<q-document-properties
			v-if="popupIsVisible && showProperties"
			:control-id="controlId"
			:texts="texts"
			:file-properties="fileProperties"
			@hide-popup="setPropertiesModalState(false)" />

		<q-document-submit
			v-if="popupIsVisible && showFileSubmit"
			:control-id="controlId"
			:texts="texts"
			:label-id="labelId"
			:extensions="extensions"
			:max-file-size="maxFileSize"
			:minor-version-value="minorVersionValue"
			:major-version-value="majorVersionValue"
			@submit-file="submitFileVersion"
			@hide-popup="setFileSubmitModalState(false)" />

		<q-document-versions
			v-if="popupIsVisible && showVersions"
			:control-id="controlId"
			:texts="texts"
			:readonly="readonly"
			:versions="versions"
			:versions-info="versionsInfo"
			:resources-path="resourcesPath"
			@get-file="getFile"
			@delete-last="confirmDeleteLast"
			@delete-history="confirmDeleteHistory" />
	</div>
</template>

<script>
	import { defineAsyncComponent } from 'vue'

	import { displayMessage, isEmpty, validateFileExtAndSize, validateTexts } from '@/mixins/genericFunctions.js'
	import { inputSize } from '@/mixins/quidgest.mainEnums.js'

	// The texts needed by the component.
	const DEFAULT_TEXTS = {
		downloadLabel: 'Download',
		attachLabel: 'Attach',
		submitLabel: 'Submit',
		chooseFileLabel: 'Choose file',
		editLabel: 'Edit',
		deleteLabel: 'Delete',
		propertyLabel: 'Properties',
		versionsLabel: 'Versions',
		viewAllLabel: 'View all...',
		deleteLastLabel: 'Delete last',
		deleteHistoryLabel: 'Delete history',
		nameLabel: 'Name: ',
		sizeLabel: 'Size: ',
		extensionLabel: 'Extension: ',
		authorLabel: 'Author: ',
		createdDateLabel: 'Create date: ',
		createdOnLabel: 'Created on',
		currentVersionLabel: 'Current version: ',
		editedByLabel: 'Edition by: ',
		okLabel: 'OK',
		yesLabel: 'Yes',
		noLabel: 'No',
		filesSubmission: 'Submission of files',
		noFileSelected: 'No file selected for submission.',
		fileSizeError: 'The selected file exceeds the allowed size of {0}.',
		extensionError: 'Invalid extension! Allowed extensions:',
		submitHeaderLabel: 'Select the file to be submitted: ',
		unlockHeaderLabel: 'Unlock: discards the current changes and the document will be free for editing.',
		submitFilesHeaderLabel: 'Submit: the document will be free for editing and a new version will be created.',
		majorVersionLabel: 'Major version',
		minorVersionLabel: 'Minor version',
		cancelLabelValue: 'Cancel',
		version: 'Version',
		documentLabel: 'Document',
		bytesLabel: 'Bytes',
		author: 'Author',
		deleteHeaderLabel: 'Are you sure you want to delete?',
		actionLabel: 'Actions',
		viewAll: 'View all',
		closeLabel: 'Close',
		theLastVersionWillEliminate: 'The last version will be eliminated.\\r\\nAre you sure you want to delete?',
		allTheVersionsExceptLastWillEliminate: 'All the versions except the last will be deleted.\\r\\nAre you sure you want to delete?',
		uploadDocVersionHeader: 'Document versions',
		createDocument: 'Create document',
		editingDocument: 'This document is currently being edited.',
		pendingDocumentVersion: 'This document version is not yet saved.',
		errorProcessingRequest: 'An error has occurred while processing the request.'
	}

	export default {
		name: 'QDocument',

		emits: {
			'delete-file': () => true,
			'delete-history': () => true,
			'delete-last': () => true,
			'edit-file': () => true,
			'file-error': (payload) => typeof payload === 'number',
			'get-file': (payload) => typeof payload === 'object',
			'get-properties': () => true,
			'get-version-history': () => true,
			'hide-popup': (payload) => typeof payload === 'string',
			'show-popup': (payload) => typeof payload === 'object',
			'show-templates-popup': () => true,
			'submit-file': (payload) => typeof payload === 'object'
		},

		components: {
			QDocumentDropdown: defineAsyncComponent(() => import('./popups/QDocumentDropdown.vue')),
			QDocumentSubmit: defineAsyncComponent(() => import('./popups/QDocumentSubmit.vue')),
			QDocumentVersions: defineAsyncComponent(() => import('./popups/QDocumentVersions.vue')),
			QDocumentProperties: defineAsyncComponent(() => import('./popups/QDocumentProperties.vue'))
		},

		inheritAttrs: false,

		props: {
			/**
			 * Unique identifier for the control.
			 */
			id: String,

			/**
			 * The name of the current version of the file.
			 */
			modelValue: String,

			/**
			 * Necessary strings to be used in labels and buttons.
			 */
			texts: {
				type: Object,
				validator: (value) => validateTexts(DEFAULT_TEXTS, value),
				default: () => DEFAULT_TEXTS
			},

			/**
			 * Extensions allowed for file select, some extension examples: .png, .jpg, .jpeg, .csv, .xls, .xlsx, .pdf.
			 */
			extensions: {
				type: Array,
				default: () => []
			},

			/**
			 * Maximum file size allowed, in bytes (must be a positive number).
			 */
			maxFileSize: {
				type: Number,
				validator: (value) => value >= 0,
				default: 0
			},

			/**
			 * Whether or not versioning is active for the document.
			 */
			versioning: {
				type: Boolean,
				default: false
			},

			/**
			 * Property to define the size of the component.
			 */
			size: {
				type: String,
				validator: (value) => isEmpty(value) || Reflect.has(inputSize, value)
			},

			/**
			 * Whether the field is disabled.
			 */
			disabled: {
				type: Boolean,
				default: false
			},

			/**
			 * Whether the field is readonly.
			 */
			readonly: {
				type: Boolean,
				default: false
			},

			/**
			 * The properties of the document.
			 */
			fileProperties: {
				type: Object,
				default: () => ({})
			},

			/**
			 * Whether or not the document is currently being edited by someone.
			 */
			editing: {
				type: Boolean,
				default: false
			},

			/**
			 * The current version of the document.
			 */
			currentVersion: {
				type: String,
				default: '1'
			},

			/**
			 * The current version numbers of the document.
			 */
			versions: {
				type: Object,
				default: () => ({})
			},

			/**
			 * The details about the version history of the document.
			 */
			versionsInfo: {
				type: Array,
				default: () => []
			},

			/**
			 * Whether or not one of the popups is currently open.
			 */
			popupIsVisible: {
				type: Boolean,
				default: false
			},

			/**
			 * The resources path.
			 */
			resourcesPath: {
				type: String,
				required: true
			},

			/**
			 * Indicates whether the control uses document templates.
			 */
			usesTemplates: {
				type: Boolean,
				default: false
			}
		},

		provide()
		{
			return {
				validateFile: this.validateFile
			}
		},

		expose: [],

		data()
		{
			return {
				// The id of the component.
				controlId: this.id || `q-document-${this._.uid}`,

				// Whether or not the properties popup is visible.
				showProperties: false,

				// Whether or not the file submit popup is visible.
				showFileSubmit: false,

				// Whether or not the version history popup is visible.
				showVersions: false,

				// Whether or not the versions dropdown menu is visible.
				showOptions: false,

				// The default minor version value.
				minorVersionValue: '',

				// The default major version value.
				majorVersionValue: ''
			}
		},

		computed: {
			/**
			 * The identifier of the component's label.
			 */
			labelId()
			{
				return `label_${this.controlId}`
			},

			/**
			 * An array with all the versions, ordered in ascendent order.
			 */
			orderedVersions()
			{
				const versions = []

				for (let i in this.versions)
				{
					const value = this.versions[i]
					const dirty = value?.length === 0

					versions.push({
						key: i,
						value,
						dirty
					})
				}

				return versions.sort((a, b) => Number(a.key) > Number(b.key) ? -1 : 1)
			},

			/**
			 * The last version that isn't dirty.
			 */
			lastNonDirtyVersion()
			{
				const lastVersion = this.orderedVersions.find((v) => v.value?.length > 0)
				return lastVersion?.key ?? this.currentVersion
			},

			/**
			 * Whether the options button is disabled.
			 */
			isOptionsButtonDisabled()
			{
				return this.disabled || (this.readonly && !this.modelValue)
			}
		},

		methods: {
			/**
			 * Handles the click event on the document field.
			 */
			handleFieldClick()
			{
				if (this.modelValue)
					this.getFile('', false)
				else
					this.triggerFileAttach()
			},

			/**
			 * Validates the attached file, if everything's ok calls the callback function, if one is provided.
			 * @param {object} event The file attach event
			 * @param {function} callback The callback function
			 */
			validateFile(event, callback)
			{
				const file = event.target.files[0]
				const validationResult = validateFileExtAndSize(file, this.extensions, this.maxFileSize)

				if (validationResult === 0 && typeof callback === 'function')
					callback(file)
				else
					this.$emit('file-error', validationResult)

				// Clears the value, so that the next "change" event will trigger even if the file name is the same.
				event.target.value = ''
			},

			/**
			 * Programatically triggers the file attach window.
			 */
			triggerFileAttach()
			{
				if (!this.readonly && !this.disabled)
					this.$refs.fileAttach.click()
			},

			/**
			 * Retrieves the attached file object and emits an event with it.
			 * @param {object} event The file attach event
			 */
			attachFile(event)
			{
				const callback = (file) => this.$emit('submit-file', { file, version: this.currentVersion })
				this.validateFile(event, callback)
			},

			/**
			 * Emits the event with the newly submitted version of the document.
			 * @param {object} file The attached file
			 */
			submitFileVersion(file)
			{
				this.$emit('submit-file', file)
				this.setFileSubmitModalState(false)
			},

			/**
			 * Emits the event to fetch the properties of the document from the server.
			 */
			getProperties()
			{
				if (this.modelValue)
					this.$emit('get-properties')
			},

			/**
			 * Emits the event to get the specified version of the document.
			 * @param {string} version The id of the version
			 * @param {boolean} download Whether to force the file download
			 */
			getFile(version = '', download = true)
			{
				let v = this.currentVersion
				if (version?.length > 0)
					v = version
				else if (!this.modelValue)
					return

				this.$emit('get-file', { version: v, download })
			},

			/**
			 * Emits the event to fetch the details of all versions of the document from the server.
			 */
			viewAllVersions()
			{
				this.$emit('get-version-history')
			},

			/**
			 * Emits the event to put the document in "Edit" mode.
			 */
			editFile()
			{
				this.$emit('edit-file')
			},

			/**
			 * Emits the event to delete the document and all it's versions.
			 */
			deleteFile()
			{
				this.$emit('delete-file')
			},

			/**
			 * Emits the event to delete the last version of the document.
			 */
			deleteLastVersion()
			{
				this.$emit('delete-last')
				this.setVersionsModalState(false)
			},

			/**
			 * Emits the event to delete the document history.
			 */
			deleteFileHistory()
			{
				this.$emit('delete-history')
				this.setVersionsModalState(false)
			},

			/**
			 * Confirmation window for the deletion of a document.
			 * @param {string} question The question to present to the user
			 * @param {function} action The action to be executed in case the user wants to proceed
			 */
			confirmDelete(question, action)
			{
				const buttons = {
					confirm: {
						label: this.texts.yesLabel,
						action
					},
					cancel: {
						label: this.texts.noLabel
					}
				}

				displayMessage(question, 'question', null, buttons)
			},

			/**
			 * Confirmation window for the deletion of the document.
			 */
			confirmFileDelete()
			{
				this.confirmDelete(this.texts.deleteHeaderLabel, this.deleteFile)
			},

			/**
			 * Confirmation window for the deletion of the last version of the document.
			 */
			confirmDeleteLast()
			{
				this.confirmDelete(this.texts.theLastVersionWillEliminate, this.deleteLastVersion)
			},

			/**
			 * Confirmation window for the deletion of the document history.
			 */
			confirmDeleteHistory()
			{
				this.confirmDelete(this.texts.allTheVersionsExceptLastWillEliminate, this.deleteFileHistory)
			},

			/**
			 * Sets the visibility of the properties popup.
			 * @param {boolean} isVisible The state of the popup
			 */
			setPropertiesModalState(isVisible)
			{
				const modalId = `file-properties-${this.controlId}`

				if (isVisible)
				{
					const modalProps = {
						id: modalId,
						props: {
							modalWidth: 'md',
							headerTitle: this.texts.propertyLabel,
							dismissAction: () => this.setPropertiesModalState(false)
						}
					}
					this.$emit('show-popup', modalProps)
				}
				else
					this.$emit('hide-popup', modalId)

				this.showProperties = isVisible
			},

			/**
			 * Sets the visibility of the file submit popup.
			 * @param {boolean} isVisible The state of the popup
			 */
			setFileSubmitModalState(isVisible)
			{
				const modalId = `submit-file-${this.controlId}`

				if (isVisible)
				{
					const version = Number(this.lastNonDirtyVersion)

					// If the popup is being opened, the default values of the versions need to be updated.
					if (!isNaN(version))
					{
						this.minorVersionValue = (version + 0.1).toFixed(1)
						this.majorVersionValue = (Math.floor(version) + 1).toString()

						if (Number(this.minorVersionValue) === Number(this.majorVersionValue))
							this.minorVersionValue = this.majorVersionValue
					}
					else
					{
						this.minorVersionValue = ''
						this.majorVersionValue = ''
					}

					const modalProps = {
						id: modalId,
						props: {
							headerTitle: this.texts.filesSubmission,
							dismissAction: () => this.setFileSubmitModalState(false)
						}
					}
					this.$emit('show-popup', modalProps)
				}
				else
					this.$emit('hide-popup', modalId)

				this.showFileSubmit = isVisible
			},

			/**
			 * Sets the visibility of the versions popup.
			 * @param {boolean} isVisible The state of the popup
			 */
			setVersionsModalState(isVisible)
			{
				const modalId = `file-versions-${this.controlId}`

				if (isVisible)
				{
					const modalProps = {
						id: modalId,
						props: {
							headerTitle: this.texts.uploadDocVersionHeader,
							hideFooter: this.disabled || this.readonly,
							dismissAction: () => this.setVersionsModalState(false)
						}
					}
					this.$emit('show-popup', modalProps)
				}
				else
					this.$emit('hide-popup', modalId)

				this.showVersions = isVisible
			},

			/**
			 * Toggles the options dropdown state to either open or closed.
			 */
			toggleDropdown()
			{
				this.showOptions = !this.showOptions
			},

			/**
			 * Emit event to open popup with document templates.
			 */
			createDocument()
			{
				this.$emit('show-templates-popup')
			}
		},

		watch: {
			fileProperties(val)
			{
				if (val && Object.keys(val).length > 0)
					this.setPropertiesModalState(true)
			},

			versionsInfo(val)
			{
				if (val?.length > 0)
					this.setVersionsModalState(true)
			}
		}
	}
</script>
