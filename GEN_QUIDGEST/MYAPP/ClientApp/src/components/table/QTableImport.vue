<template>
	<q-button
		b-style="secondary"
		borderless
		class="removecaret dropdown-toggle"
		:label="texts.importButtonTitle"
		:title="texts.importButtonTitle"
		@click="fnShowImportPopup">
		<q-icon icon="file-import" />
	</q-button>

	<!-- BEGIN: Import Popup -->
	<teleport
		v-if="showImportPopup"
		:to="`#q-modal-${modalId}-body`"
		:key="domKey">
		<div
			v-if="showResponseData"
			class="bootbox-body c-message"
			:class="messageClasses">
			<div class="message-icon">
				<q-icon :icon="messageIcon" />
			</div>

			<h5>{{ dataImportResponse.msg }}</h5>

			<div
				v-for="(line, index) in dataImportResponse.lines"
				:key="index">
				{{ line }}
			</div>

			<div v-if="!successStatus">
				{{ dataImportResponse.errors[0] }}
			</div>
		</div>
		<div
			v-else
			id="import-list-input"
			@dragenter="handleDragEnter"
			@dragleave.stop="handleDragLeave"
			@dragover.prevent="() => {}"
			@drop.prevent="handleDrop">
			<div
				class="qq-uploader"
				ref="dragArea">
				<div
					v-if="dropAreaActive"
					class="qq-upload-drop-area"
					:style="uploadStyles">
					<span class="upload-text">{{ texts.dropToUpload }}</span>
				</div>
				<div v-else>
					<li>{{ texts.downloadTemplateText }}</li>
					<li>{{ texts.fillTemplateFileText }}</li>
					<li>{{ texts.importTemplateFileText }}</li>

					<q-button
						aria-expanded="false"
						aria-haspopup="true"
						b-style="secondary"
						:class="['removecaret', 'mt-2']"
						:label="templateOptions[0].value"
						:title="texts.templateButtonTitle"
						@click="$emit('export-template', templateOptions[0].key)">
						<q-icon icon="file-import" />
					</q-button>
				</div>

				<input
					type="file"
					name="file"
					ref="file"
					style="font-size: 15px; cursor: pointer; display: none"
					@change="submitFile" />
			</div>
		</div>
	</teleport>

	<teleport
		v-if="showImportPopup"
		:to="`#q-modal-${modalId}-footer`"
		:key="domKey">
		<q-button
			v-if="showResponseData"
			b-style="secondary"
			:class="['float-right']"
			:label="texts.cancelText"
			:title="texts.cancelText"
			@click="closeImportPopup">
			<q-icon icon="remove" />
		</q-button>
		<div
			v-else
			class="actions float-right">
			<q-button
				b-style="primary"
				:label="texts.submitText"
				:title="texts.submitText"
				@click="importFile">
				<q-icon icon="submit" />
			</q-button>

			<q-button
				b-style="secondary"
				:label="texts.cancelText"
				:title="texts.cancelText"
				@click="closeImportPopup">
				<q-icon icon="cancel" />
			</q-button>
		</div>
	</teleport>
</template>

<script>
	export default {
		name: 'QTableImport',

		emits: [
			'import-data',
			'show-import-popup',
			'hide-import-popup',
			'export-template'
		],

		props: {
			/**
			 * Array of options specifying the file types and sources that data can be imported from.
			 */
			options: {
				type: Array,
				default: () => []
			},

			/**
			 * An array of template options that users can choose from for importing predefined data structures.
			 */
			templateOptions: {
				type: Array,
				default: () => []
			},

			/**
			 * An object containing the response from a data import operation, which might include status and error messages.
			 */
			dataImportResponse: {
				type: Object,
				default: () => ({})
			},

			/**
			 * Indicates if the import operation should be handled via server-side functionality.
			 */
			serverMode: {
				type: Boolean,
				default: false
			},

			/**
			 * Unique identifier for the modal element to display import options and responses.
			 */
			modalId: {
				type: String,
				required: true
			},

			/**
			 * Localized text strings for the import component, which can include button labels and instructions.
			 */
			texts: {
				type: Object,
				required: true
			}
		},

		expose: [],

		data()
		{
			return {
				showImportPopup: false,
				showResponseData: false,
				dropAreaActive: false,
				domKey: 0
			}
		},

		computed: {
			successStatus()
			{
				return this.dataImportResponse.success
			},

			messageIcon()
			{
				return this.successStatus ? 'success' : 'error'
			},

			messageClasses()
			{
				return this.successStatus ? 'c-message-success' : 'c-message-error'
			},

			uploadStyles()
			{
				if (this.dropAreaActive)
					return {
						backgroundColor: '#ddd'
					}

				return {}
			}
		},

		methods: {
			importFile()
			{
				this.$refs.file.click()
			},

			handleDragEnter()
			{
				this.dropAreaActive = true
			},

			handleDragLeave()
			{
				this.dropAreaActive = false
			},

			handleDrop(event)
			{
				this.dropAreaActive = false
				this.submitFile(event)
			},

			/**
			 * Upload file and import data into table
			 * @param e {DOM Element} input element
			 */
			submitFile(e)
			{
				//Get file
				var files = e.target.files || e.dataTransfer.files
				if (!files.length) return

				var fileName = files[0].name.split('.')[0]

				//Emit data
				var payload = {
					format: 'xlsx',
					fileName: fileName,
					file: files[0]
				}

				this.$emit('import-data', payload)
			},

			fnShowImportPopup()
			{
				this.$emit('show-import-popup', 'xlsx')
				this.$nextTick().then(() => {
					this.showImportPopup = true
					this.domKey++
				})
			},

			closeImportPopup()
			{
				this.showImportPopup = false
				this.$emit('hide-import-popup')
				this.showResponseData = false
			}
		},

		watch: {
			dataImportResponse: {
				handler(after)
				{
					if (typeof after.success !== 'undefined')
					{
						this.showResponseData = true
						this.domKey++
					}
					else
						this.showResponseData = false
				},
				deep: true
			}
		}
	}
</script>
