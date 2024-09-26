<template>
	<div
		:id="controlId"
		class="q-image__field"
		ref="imgContainer"
		@dragenter.prevent="handleDragEnter"
		@dragleave.stop="handleDragLeave"
		@dragover.prevent="() => {}"
		@drop.prevent="handleDrop">
		<div class="q-image__container">
			<a
				:class="{ 'q-image__field-empty': isEmptyImage }"
				href="javascript:void(0)">
				<img
					ref="mainImg"
					data-testid="main-img"
					class="q-image__field-thumbnail"
					:style="imageStyle"
					:src="imageURL"
					@click.stop.prevent="openPreview" />
			</a>

			<input
				type="file"
				data-testid="file-input"
				ref="fileInput"
				class="q-image__field-input"
				:accept="extensions"
				@change="handleFileChange" />
		</div>

		<q-line-loader v-if="loading" />

		<div
			v-if="!readonly && !disabled && !fullSizeImage"
			:class="['q-image__field-drop-area', { 'q-image__field-drop-area-active': dragCount > 0 }]">
			<span>{{ texts.dropToUpload }}</span>
		</div>

		<div
			v-if="!readonly"
			class="q-image__field-actions">
			<q-button-group :disabled="disabled">
				<q-button
					b-style="primary"
					data-testid="submit-btn"
					:label="isEmptyImage || !image ? texts.submitLabel : ''"
					:title="texts.submitLabel"
					@click="handleImageUpload">
					<q-icon icon="upload-img" />
				</q-button>

				<q-button
					v-if="!isEmptyImage && image"
					b-style="secondary"
					data-testid="edit-btn"
					:title="texts.editLabel"
					@click="handleOpenEdit">
					<q-icon icon="edit-img" />
				</q-button>

				<q-button
					v-if="!isEmptyImage && image"
					b-style="secondary"
					data-testid="delete-btn"
					:title="texts.deleteLabel"
					@click="handleImageDelete">
					<q-icon icon="delete" />
				</q-button>
			</q-button-group>
		</div>

		<template v-if="fullSizeImage">
			<q-image-editor
				v-if="popupIsVisible && showEditModal"
				:control-id="controlId"
				:texts="texts"
				:image-to-edit="getImageURL(fullSizeImage)"
				@image-edited="imageEdited"
				@close-editor="handleCloseEdit" />
			<div
				v-else
				class="q-image__modal-main-container">
				<div class="q-image__modal-container">
					<img
						:src="getImageURL(fullSizeImage)"
						class="q-image__modal-image" />

					<div class="q-image__modal-buttons">
						<q-button
							b-style="plain"
							borderless
							class="q-image__modal-button"
							:title="texts.download"
							@click="downloadImage">
							<q-icon icon="download" />
						</q-button>

						<q-button
							b-style="plain"
							borderless
							class="q-image__modal-button"
							:title="texts.close"
							@click="closePreview">
							<q-icon icon="close" />
						</q-button>
					</div>
				</div>
			</div>
		</template>
	</div>
</template>

<script>
	import { defineAsyncComponent } from 'vue'

	import { forceDownload } from '@/api/network'
	import { displayMessage, validateFileExtAndSize, validateImageFormat, validateTexts } from '@/mixins/genericFunctions.js'

	// The texts needed by the component.
	const DEFAULT_TEXTS = {
		submitLabel: 'Submit',
		deleteLabel: 'Delete',
		editLabel: 'Edit',
		fileSizeError: 'The selected file exceeds the allowed size of {0}.',
		extensionError: 'Invalid extension! Allowed extensions:',
		editImage: 'Edit image',
		cropWarning: 'Attention: Saving this form will replace the original image',
		dropToUpload: 'Drop files here to upload',
		save: 'Save',
		cancel: 'Cancel',
		zoomIn: 'Zoom in',
		zoomOut: 'Zoom out',
		moveImageLeft: 'Move image left',
		moveImageRight: 'Move image right',
		moveImageUp: 'Move image up',
		moveImageDown: 'Move image down',
		rotateLeft: 'Rotate left',
		rotateRight: 'Rotate right',
		flipHorizontal: 'Horizontal flip',
		flipVertical: 'Vertical flip',
		deleteHeaderLabel: 'Are you sure you want to delete?',
		yesLabel: 'Yes',
		noLabel: 'No',
		close: 'Close',
		download: 'Download'
	}

	export default {
		name: 'QImage',

		emits: {
			'close-image-preview': () => true,
			'delete-image': () => true,
			'file-error': (payload) => typeof payload === 'number',
			'hide-popup': (payload) => typeof payload === 'string',
			'open-image-preview': () => true,
			'show-popup': (payload) => typeof payload === 'object',
			'submit-image': (payload) => validateImageFormat(payload)
		},

		components: {
			QImageEditor: defineAsyncComponent(() => import('./popups/QImageEditor.vue'))
		},

		inheritAttrs: false,

		props: {
			/**
			 * Unique identifier for the control.
			 */
			id: String,

			/**
			 * The image to be displayed (can be minimized to improve performance).
			 */
			image: {
				type: [String, Object],
				validator: (value) => validateImageFormat(value)
			},

			/**
			 * The full sized image to be displayed in a modal.
			 */
			fullSizeImage: {
				type: [String, Object],
				validator: (value) => validateImageFormat(value)
			},

			/**
			 * Whether or not the current image is a default empty image.
			 */
			isEmptyImage: {
				type: Boolean,
				default: false
			},

			/**
			 * Necessary strings to be used in labels and buttons.
			 */
			texts: {
				type: Object,
				validator: (value) => validateTexts(DEFAULT_TEXTS, value),
				default: () => DEFAULT_TEXTS
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
			 * Extensions allowed for file select, some extension examples: .png, .jpg, .svg.
			 */
			extensions: {
				type: Array,
				default: () => ['.jpg', '.jpeg', '.png', '.gif', '.svg', '.webp', '.bmp']
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
			 * The maximum height of the image (must be a positive number).
			 */
			height: {
				type: Number,
				validator: (value) => value > 0
			},

			/**
			 * The maximum width of the image (must be a positive number).
			 */
			width: {
				type: Number,
				validator: (value) => value > 0
			},

			/**
			 * Whether or not the popup is currently open.
			 */
			popupIsVisible: {
				type: Boolean,
				default: false
			},

			/**
			 * Whether or not the image data is loading.
			 */
			loading: {
				type: Boolean,
				default: false
			}
		},

		expose: [],

		data()
		{
			return {
				controlId: this.id || `q-image-input-${this._.uid}`,

				dragCount: 0,

				showEditModal: false
			}
		},

		mounted()
		{
			document.addEventListener('keydown', this.onKeyPress)
		},

		beforeUnmount()
		{
			document.removeEventListener('keydown', this.onKeyPress)
		},

		computed: {
			/**
			 * The CSS properties of the image.
			 */
			imageStyle()
			{
				return {
					'max-height': this.height ? `${this.height}px` : 'auto',
					'max-width': this.width ? `${this.width}px` : 'auto'
				}
			},

			/**
			 * The base64 representation of the image to be displayed.
			 */
			imageURL()
			{
				return this.getImageURL(this.image)
			}
		},

		methods: {
			/**
			 * Returns the URL of the image.
			 * @param {String|Object} image - The ID of the image.
			 * @returns A base64 representation of the provided image.
			 */
			getImageURL(image)
			{
				// Here we are dealing with various cases:
				// - If image is not provided returns an empty path
				// - If image is string use it directly
				// - If image is dataURL object, create image url using data, encoding and data format
				return image
					? typeof image === 'object'
						? `data:image/${image.dataFormat};${image.encoding},${image.data}`
						: image
					: ''
			},

			/**
			 * Returns the MIME type of the image.
			 * @param {String|Object} image - The image.
			 * @returns The MIME type of the provided image.
			 */
			getImageType(image)
			{
				const base64Img = this.getImageURL(image)

				if (base64Img?.length > 0)
					return base64Img.split(';')[0]?.split('/').pop()
				return undefined
			},

			/**
			 * Reads the file and transforms it into an object.
			 * @param {File} file - The image file.
			 * @returns A promise to be resolved when the process is concluded.
			 */
			getFileAsObj(file)
			{
				return new Promise((resolve, reject) => {
					const reader = new FileReader()

					reader.onloadend = () => {
						const base64Img = reader.result
						const mimeType = file.type.split('/').pop()
						const fileData = base64Img.split(',').pop()

						const imgData = {
							data: fileData,
							dataFormat: mimeType,
							fileName: file.name,
							encoding: 'base64',
							isThumbnail: false
						}

						resolve(imgData)
					}
					reader.onerror = reject

					reader.readAsDataURL(file)
				})
			},

			/**
			 * Opens the image preview.
			 */
			openPreview()
			{
				if (!this.isEmptyImage)
					this.$emit('open-image-preview')
			},

			/**
			 * Closes the image preview.
			 */
			closePreview()
			{
				this.$emit('close-image-preview')
			},

			/**
			 * Handles the change of the selected file.
			 * @param {Event} event - The event that is being handled.
			 */
			async handleFileChange(event)
			{
				// This method will be called when the file gets uploaded.
				const files = event.target.files ?? event.dataTransfer.files

				if (files && files[0])
				{
					const file = files[0]
					const validationResult = validateFileExtAndSize(file, this.extensions, this.maxFileSize)

					if (validationResult === 0)
					{
						const imgData = await this.getFileAsObj(file)
						this.$emit('submit-image', imgData)
					}
					else
						this.$emit('file-error', validationResult)

					// Clears the value, so that the next "change" event will trigger even if the file name is the same.
					event.target.value = ''
				}
			},

			/**
			 * Handles the deletion of the image.
			 */
			handleImageDelete()
			{
				const buttons = {
					confirm: {
						label: this.texts.yesLabel,
						action: () => this.$emit('delete-image')
					},
					cancel: {
						label: this.texts.noLabel
					}
				}
				displayMessage(this.texts.deleteHeaderLabel, 'question', null, buttons)
			},

			/**
			 * Handles the uploading of the image.
			 */
			handleImageUpload()
			{
				// This function will trigger the click event on the file input.
				this.$refs.fileInput.click()
			},

			/**
			 * Handles a drag enter event.
			 */
			handleDragEnter()
			{
				if (this.disabled || this.readonly || this.fullSizeImage)
					return

				this.dragCount++
			},

			/**
			 * Handles a drag leave event.
			 */
			handleDragLeave()
			{
				if (this.readonly || this.disabled || this.fullSizeImage)
					return

				this.dragCount--
			},

			/**
			 * Handles a drop event.
			 * @param {Event} event - The event that is being handled.
			 */
			handleDrop(event)
			{
				if (this.readonly || this.disabled || this.fullSizeImage)
					return

				this.dragCount = 0
				this.handleFileChange(event)
			},

			/**
			 * Handles the opening of the image edit popup.
			 */
			handleOpenEdit()
			{
				const modalId = `image-edit-${this.controlId}`
				const modalProps = {
					id: modalId,
					props: {
						headerTitle: this.texts.editImage,
						dismissAction: this.closeEdit
					}
				}
				this.$emit('show-popup', modalProps)

				this.openPreview()
				this.showEditModal = true
			},

			/**
			 * Closes the image edit popup.
			 */
			closeEdit()
			{
				this.showEditModal = false
				this.closePreview()
			},

			/**
			 * Handles the closing of the image edit popup.
			 */
			handleCloseEdit()
			{
				const modalId = `image-edit-${this.controlId}`
				this.$emit('hide-popup', modalId)

				this.closeEdit()
			},

			/**
			 * Handles the editing of the image.
			 * @param {Object} imgData - The data of the edited image.
			 */
			imageEdited(imgData)
			{
				this.$emit('submit-image', imgData)
				this.handleCloseEdit()
			},

			/**
			 * Handles the downloading of the image.
			 */
			downloadImage()
			{
				const imgData = this.getImageURL(this.fullSizeImage)
				const imgType = this.getImageType(this.fullSizeImage)
				const fileName = 'Image' // TODO: Use the original name of the file

				forceDownload(imgData, fileName, imgType, false, false)
			},

			/**
			 * Handles key press events.
			 * @param {Object} event - The key press event.
			 */
			onKeyPress(event)
			{
				if (this.fullSizeImage &&
					!(this.popupIsVisible && this.showEditModal) &&
					event.key === 'Escape')
					this.closePreview()
			}
		},

		watch: {
			fullSizeImage(val)
			{
				if (this.showEditModal)
					return

				// We are manipulating the style of the body to prevent overflow when the preview mode is on.
				if (val)
					document.body.style.setProperty('overflow', 'hidden')
				else
					document.body.style.removeProperty('overflow')
			}
		}
	}
</script>
