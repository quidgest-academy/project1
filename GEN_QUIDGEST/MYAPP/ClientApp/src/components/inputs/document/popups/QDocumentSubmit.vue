<template>
	<teleport :to="`#q-modal-submit-file-${controlId}-body`">
		<q-row-container>
			<q-input-group :label="texts.submitHeaderLabel">
				<q-text-field
					:model-value="submittedFileName"
					readonly
					size="large"
					:class="fileInputClass"
					@click="handleFileUpload" />

				<template #append>
					<q-button
						b-style="secondary"
						:label="texts.chooseFileLabel"
						:title="texts.chooseFileLabel"
						:disabled="versionSubmitMode === versionSubmitModes.unlock"
						@click="handleFileUpload">
						<q-icon icon="choose-file" />
					</q-button>
				</template>
			</q-input-group>

			<input
				v-if="versionSubmitMode !== versionSubmitModes.unlock"
				:id="`q-document-version-${controlId}`"
				class="d-none"
				ref="fileInput"
				name="doc-file"
				type="file"
				:aria-labelledby="labelId"
				:accept="extensions"
				@change="attachFileVersion" />
		</q-row-container>

		<hr />

		<!-- Unlock radio option -->
		<q-radio-group
			id="`q-document-submit-options-${controlId}`"
			v-model="versionSubmitMode"
			:options-list="submitModeOptions" />

		<div
			v-if="majorVersionValue !== minorVersionValue"
			class="q-document__version-options">
			<q-radio-group
				:id="`q-document-version-options-${controlId}`"
				v-model="versionType"
				:options-list="versionOptions"
				:disabled="versionSubmitMode === versionSubmitModes.unlock">
				<template
					v-for="version in versionNumbers"
					:key="version.value"
					#[`${version.key}.append`]>
					<q-text-field
						size="small"
						readonly
						:model-value="version.value" />
				</template>
			</q-radio-group>
		</div>
	</teleport>

	<teleport :to="`#q-modal-submit-file-${controlId}-footer`">
		<div class="actions">
			<q-button
				b-style="primary"
				:label="texts.submitLabel"
				:title="texts.submitLabel"
				@click="submitFileVersion">
				<q-icon icon="submit" />
			</q-button>

			<q-button
				b-style="secondary"
				:label="texts.cancelLabelValue"
				:title="texts.cancelLabelValue"
				@click="$emit('hide-popup')">
				<q-icon icon="cancel" />
			</q-button>
		</div>
	</teleport>
</template>

<script>
	import { displayMessage } from '@/mixins/genericFunctions.js'

	export default {
		name: 'QDocumentSubmit',

		emits: {
			'hide-popup': () => true,
			'submit-file': (payload) => typeof payload === 'object'
		},

		inheritAttrs: false,

		props: {
			/**
			 * Unique ID for the control.
			 */
			controlId: String,

			/**
			 * The ID of the component's label.
			 */
			labelId: String,

			/**
			 * Necessary strings to be used in labels and buttons.
			 */
			texts: {
				type: Object,
				required: true
			},

			/**
			 * Extensions allowed for file select, some extension examples: .png, .jpg, .jpeg, .csv, .xls, .xlsx, .pdf.
			 */
			extensions: {
				type: Array,
				default: () => []
			},

			/**
			 * The value of the minor version.
			 */
			minorVersionValue: {
				type: String,
				default: ''
			},

			/**
			 * The value of the major version.
			 */
			majorVersionValue: {
				type: String,
				default: ''
			}
		},

		inject: [
			'validateFile'
		],

		expose: [],

		data()
		{
			return {
				// The data of a newly submitted document version (not yet stored server-side).
				submittedFileData: null,

				// The possible types of versions.
				versionTypes: {
					minor: 'Minor',
					major: 'Major'
				},

				// The selected type of version.
				versionType: 'Major',

				// The possible modes of version submission.
				versionSubmitModes: {
					unlock: 'UnlockFile',
					submit: 'Submit'
				},

				// The selected mode of version submission.
				versionSubmitMode: 'Submit'
			}
		},

		computed: {
			/**
			 * The class to apply to the file input.
			 */
			fileInputClass()
			{
				const isEditable = this.versionSubmitMode !== this.versionSubmitModes.unlock
				return `q-document__field${isEditable ? '' : '-empty'}`
			},

			/**
			 * The name of the newly submitted version of the document.
			 */
			submittedFileName()
			{
				return this.submittedFileData ? this.submittedFileData.name : ''
			},

			/**
			 * A list with the possible submit mode options.
			 */
			submitModeOptions()
			{
				return [
					{
						key: this.versionSubmitModes.unlock,
						value: this.texts.unlockHeaderLabel
					},
					{
						key: this.versionSubmitModes.submit,
						value: this.texts.submitFilesHeaderLabel
					}
				]
			},

			/**
			 * A list with the possible version options.
			 */
			versionOptions()
			{
				return [
					{
						key: this.versionTypes.major,
						value: this.texts.majorVersionLabel
					},
					{
						key: this.versionTypes.minor,
						value: this.texts.minorVersionLabel
					}
				]
			},

			/**
			 * A list with the possible version numbers.
			 */
			versionNumbers()
			{
				return [
					{
						key: this.versionTypes.major,
						value: this.majorVersionValue
					},
					{
						key: this.versionTypes.minor,
						value: this.minorVersionValue
					}
				]
			}
		},

		methods: {
			/**
			 * Triggers the click event on the file input.
			 */
			handleFileUpload()
			{
				this.$refs.fileInput?.click()
			},

			/**
			 * Retrieves the attached file object and keeps it's data.
			 * @param {object} event The file attach event
			 */
			attachFileVersion(event)
			{
				const callback = (fileData) => {
					this.submittedFileData = fileData
				}

				this.validateFile(event, callback)
			},

			/**
			 * Emits the event with the newly submitted version of the document.
			 */
			submitFileVersion()
			{
				let version = this.minorVersionValue
				if (this.versionType === this.versionTypes.major)
					version = this.majorVersionValue

				if (this.versionSubmitMode === this.versionSubmitModes.submit)
				{
					if (this.submittedFileName === '')
					{
						displayMessage(this.texts.noFileSelected)
						return
					}

					this.$emit('submit-file', { file: this.submittedFileData, isNewVersion: true, version })
				}
				else if (this.versionSubmitMode === this.versionSubmitModes.unlock)
					this.$emit('submit-file', { isNewVersion: false, version })

				this.submittedFileData = null
			}
		},

		watch: {
			versionSubmitMode(val)
			{
				if (val === this.versionSubmitModes.unlock)
					this.submittedFileData = null
			}
		}
	}
</script>
