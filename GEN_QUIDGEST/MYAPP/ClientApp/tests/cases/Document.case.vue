<template>
	<div
		class="container-fluid"
		style="flex: 1 1 auto">
		<!-- This control with id CTRL_1 is for showing file upload with all extensions and versioningIsOn allowed -->
		<base-input-structure
			id="CTRL_1"
			label="File upload component with versioningIsOn allowed">
			<q-document
				id="Document-CTRL_1"
				size="xlarge"
				v-model="ctrl1.fileName"
				versioning-is-on
				:editing="ctrl1.editing"
				:current-version="ctrl1.currentVersion"
				:versions="ctrl1.versions"
				:versions-info="ctrl1.versionsInfo"
				:file-properties="ctrl1.fileProperties"
				:popup-is-visible="popupIsVisible"
				@submit-file="submitFile('ctrl1', $event)"
				@edit-file="editFile('ctrl1')"
				@get-properties="getProperties('ctrl1')"
				@get-version-history="getVersionHistory('ctrl1')"
				@get-file="getFile()"
				@get-file-version="getFileVersion($event)"
				@delete-file="deleteFile('ctrl1')"
				@delete-last="deleteLast('ctrl1')"
				@delete-history="deleteHistory('ctrl1')"
				@show-popup="openPopup($event)"
				@hide-popup="closePopup($event)" />
		</base-input-structure>
	</div>

	<div
		class="container-fluid"
		style="flex: 1 1 auto">
		<!-- This control with id CTRL_2 is for showing file upload with specified extension to allow and versioningIsOn not allowed -->
		<base-input-structure
			id="CTRL_2"
			label="File upload component with versioningIsOn not allowed">
			<q-document
				id="Document-CTRL_2"
				size="xlarge"
				v-model="ctrl2.fileName"
				:versioning-is-on="false"
				:editing="ctrl2.editing"
				:current-version="ctrl2.currentVersion"
				:versions="ctrl2.versions"
				:versions-info="ctrl2.versionsInfo"
				:file-properties="ctrl2.fileProperties"
				:popup-is-visible="popupIsVisible"
				@submit-file="submitFile('ctrl2', $event)"
				@edit-file="editFile('ctrl2')"
				@get-properties="getProperties('ctrl2')"
				@get-version-history="getVersionHistory('ctrl2')"
				@get-file="getFile()"
				@get-file-version="getFileVersion($event)"
				@delete-file="deleteFile('ctrl2')"
				@delete-last="deleteLast('ctrl2')"
				@delete-history="deleteHistory('ctrl2')"
				@show-popup="openPopup($event)"
				@hide-popup="closePopup($event)" />
		</base-input-structure>
	</div>

	<div
		class="container-fluid"
		style="flex: 1 1 auto">
		<!-- This control with id CTRL_3 is for showing file upload with readonly property -->
		<base-input-structure
			id="CTRL_3"
			label="File upload component in readonly mode when file doesn't exist">
			<q-document
				id="Document-CTRL_3"
				size="xlarge"
				v-model="ctrl3.fileName"
				readonly
				versioning-is-on
				:editing="ctrl3.editing"
				:current-version="ctrl3.currentVersion"
				:versions="ctrl3.versions"
				:versions-info="ctrl3.versionsInfo"
				:file-properties="ctrl3.fileProperties"
				:popup-is-visible="popupIsVisible"
				@submit-file="submitFile('ctrl3', $event)"
				@edit-file="editFile('ctrl3')"
				@get-properties="getProperties('ctrl3')"
				@get-version-history="getVersionHistory('ctrl3')"
				@get-file="getFile()"
				@get-file-version="getFileVersion($event)"
				@delete-file="deleteFile('ctrl3')"
				@delete-last="deleteLast('ctrl3')"
				@delete-history="deleteHistory('ctrl3')"
				@show-popup="openPopup($event)"
				@hide-popup="closePopup($event)" />
		</base-input-structure>
	</div>

	<div
		class="container-fluid"
		style="flex: 1 1 auto">
		<!-- This control with id CTRL_4 is for showing file upload with readonly property -->
		<base-input-structure
			id="CTRL_4"
			label="File upload component in readonly mode when file exists">
			<q-document
				id="Document-CTRL_4"
				size="xlarge"
				v-model="ctrl4.fileName"
				readonly
				versioning-is-on
				:editing="ctrl4.editing"
				:current-version="ctrl4.currentVersion"
				:versions="ctrl4.versions"
				:versions-info="ctrl4.versionsInfo"
				:file-properties="ctrl4.fileProperties"
				:popup-is-visible="popupIsVisible"
				@submit-file="submitFile('ctrl4', $event)"
				@edit-file="editFile('ctrl4')"
				@get-properties="getProperties('ctrl4')"
				@get-version-history="getVersionHistory('ctrl4')"
				@get-file="getFile()"
				@get-file-version="getFileVersion($event)"
				@delete-file="deleteFile('ctrl4')"
				@delete-last="deleteLast('ctrl4')"
				@delete-history="deleteHistory('ctrl4')"
				@show-popup="openPopup($event)"
				@hide-popup="closePopup($event)" />
		</base-input-structure>
	</div>

	<div
		class="container-fluid"
		style="flex: 1 1 auto">
		<!-- This control with id CTRL_5 is for showing the state of the component when a file is being edited -->
		<base-input-structure
			id="CTRL_5"
			label="File upload component with file being edited">
			<q-document
				id="Document-CTRL_5"
				size="xlarge"
				v-model="ctrl5.fileName"
				versioning-is-on
				:editing="ctrl5.editing"
				:current-version="ctrl5.currentVersion"
				:versions="ctrl5.versions"
				:versions-info="ctrl5.versionsInfo"
				:file-properties="ctrl5.fileProperties"
				:popup-is-visible="popupIsVisible"
				@submit-file="submitFile('ctrl5', $event)"
				@edit-file="editFile('ctrl5')"
				@get-properties="getProperties('ctrl5')"
				@get-version-history="getVersionHistory('ctrl5')"
				@get-file="getFile()"
				@get-file-version="getFileVersion($event)"
				@delete-file="deleteFil('ctrl5')"
				@delete-last="deleteLast('ctrl5')"
				@delete-history="deleteHistory('ctrl5')"
				@show-popup="openPopup($event)"
				@hide-popup="closePopup($event)" />
		</base-input-structure>
	</div>

	<div
		class="container-fluid"
		style="flex: 1 1 auto">
		<!-- This control with id CTRL_6 is for showing file upload with specified extension to allow -->
		<base-input-structure
			id="CTRL_6"
			label="File upload component with specified extensions (only pdf is allowed)">
			<q-document
				id="Document-CTRL_6"
				size="xlarge"
				v-model="ctrl6.fileName"
				:extensions="fileExtensions"
				:max-file-size="50000"
				:versioning-is-on="false"
				:editing="ctrl6.editing"
				:current-version="ctrl6.currentVersion"
				:versions="ctrl6.versions"
				:versions-info="ctrl6.versionsInfo"
				:file-properties="ctrl6.fileProperties"
				:popup-is-visible="popupIsVisible"
				@submit-file="submitFile('ctrl6', $event)"
				@edit-file="editFile('ctrl6')"
				@get-properties="getProperties('ctrl6')"
				@get-version-history="getVersionHistory('ctrl6')"
				@get-file="getFile()"
				@get-file-version="getFileVersion($event)"
				@delete-file="deleteFile('ctrl6')"
				@delete-last="deleteLast('ctrl6')"
				@delete-history="deleteHistory('ctrl6')"
				@show-popup="openPopup($event)"
				@hide-popup="closePopup($event)" />
		</base-input-structure>
	</div>

	<div
		class="container-fluid"
		style="flex: 1 1 auto">
		<!-- This control with id CTRL_7 is for showing file upload with default maximum size -->
		<base-input-structure
			id="CTRL_7"
			label="File upload component with default maximum size of file (Default maximum file size: 1000kb)">
			<q-document
				id="Document-CTRL_7"
				size="xlarge"
				v-model="ctrl7.fileName"
				:versioning-is-on="false"
				:editing="ctrl7.editing"
				:current-version="ctrl7.currentVersion"
				:versions="ctrl7.versions"
				:versions-info="ctrl7.versionsInfo"
				:file-properties="ctrl7.fileProperties"
				:popup-is-visible="popupIsVisible"
				@submit-file="submitFile('ctrl7', $event)"
				@edit-file="editFile('ctrl7')"
				@get-properties="getProperties('ctrl7')"
				@get-version-history="getVersionHistory('ctrl7')"
				@get-file="getFile()"
				@get-file-version="getFileVersion($event)"
				@delete-file="deleteFile('ctrl7')"
				@delete-last="deleteLast('ctrl7')"
				@delete-history="deleteHistory('ctrl7')"
				@show-popup="openPopup($event)"
				@hide-popup="closePopup($event)" />
		</base-input-structure>
	</div>

	<div
		class="container-fluid"
		style="flex: 1 1 auto">
		<!-- This control with id CTRL_8 is for showing file upload with specified maximum size -->
		<base-input-structure
			id="CTRL_8"
			label="File upload component with specified maximum size of file (maximum file size: 50kb)">
			<q-document
				id="Document-CTRL_8"
				size="xlarge"
				v-model="ctrl8.fileName"
				:max-file-size="maximumFileSize"
				:versioning-is-on="false"
				:editing="ctrl8.editing"
				:current-version="ctrl8.currentVersion"
				:versions="ctrl8.versions"
				:versions-info="ctrl8.versionsInfo"
				:file-properties="ctrl8.fileProperties"
				:popup-is-visible="popupIsVisible"
				@submit-file="submitFile('ctrl8', $event)"
				@edit-file="editFile('ctrl8')"
				@get-properties="getProperties('ctrl8')"
				@get-version-history="getVersionHistory('ctrl8')"
				@get-file="getFile()"
				@get-file-version="getFileVersion($event)"
				@delete-file="deleteFile('ctrl8')"
				@delete-last="deleteLast('ctrl8')"
				@delete-history="deleteHistory('ctrl8')"
				@show-popup="openPopup($event)"
				@hide-popup="closePopup($event)" />
		</base-input-structure>
	</div>

	<div
		class="container-fluid"
		style="flex: 1 1 auto">
		<!-- This control with id CTRL_9 is for showing file upload with small size -->
		<base-input-structure
			id="CTRL_9"
			label="File upload component with small size">
			<q-document
				id="Document-CTRL_9"
				size="small"
				v-model="ctrl9.fileName"
				:versioning-is-on="false"
				:editing="ctrl9.editing"
				:current-version="ctrl9.currentVersion"
				:versions="ctrl9.versions"
				:versions-info="ctrl9.versionsInfo"
				:file-properties="ctrl9.fileProperties"
				:popup-is-visible="popupIsVisible"
				@submit-file="submitFile('ctrl9', $event)"
				@edit-file="editFile('ctrl9')"
				@get-properties="getProperties('ctrl9')"
				@get-version-history="getVersionHistory('ctrl9')"
				@get-file="getFile()"
				@get-file-version="getFileVersion($event)"
				@delete-file="deleteFile('ctrl9')"
				@delete-last="deleteLast('ctrl9')"
				@delete-history="deleteHistory('ctrl9')"
				@show-popup="openPopup($event)"
				@hide-popup="closePopup($event)" />
		</base-input-structure>
	</div>

	<div
		class="container-fluid"
		style="flex: 1 1 auto">
		<!-- This control with id CTRL_10 is for showing file upload with medium size -->
		<base-input-structure
			id="CTRL_10"
			label="File upload component with medium size">
			<q-document
				id="Document-CTRL_10"
				size="medium"
				v-model="ctrl10.fileName"
				:versioning-is-on="false"
				:editing="ctrl10.editing"
				:current-version="ctrl10.currentVersion"
				:versions="ctrl10.versions"
				:versions-info="ctrl10.versionsInfo"
				:file-properties="ctrl10.fileProperties"
				:popup-is-visible="popupIsVisible"
				@submit-file="submitFile('ctrl10', $event)"
				@edit-file="editFile('ctrl10')"
				@get-properties="getProperties('ctrl10')"
				@get-version-history="getVersionHistory('ctrl10')"
				@get-file="getFile()"
				@get-file-version="getFileVersion($event)"
				@delete-file="deleteFile('ctrl10')"
				@delete-last="deleteLast('ctrl10')"
				@delete-history="deleteHistory('ctrl10')"
				@show-popup="openPopup($event)"
				@hide-popup="closePopup($event)" />
		</base-input-structure>
	</div>

	<div
		class="container-fluid"
		style="flex: 1 1 auto">
		<!-- This control with id CTRL_11 is for showing file upload with large size -->
		<base-input-structure
			id="CTRL_11"
			label="File upload component with large size">
			<q-document
				id="Document-CTRL_11"
				size="large"
				v-model="ctrl11.fileName"
				:versioning-is-on="false"
				:editing="ctrl11.editing"
				:current-version="ctrl11.currentVersion"
				:versions="ctrl11.versions"
				:versions-info="ctrl11.versionsInfo"
				:file-properties="ctrl11.fileProperties"
				:popup-is-visible="popupIsVisible"
				@submit-file="submitFile('ctrl11', $event)"
				@edit-file="editFile('ctrl11')"
				@get-properties="getProperties('ctrl11')"
				@get-version-history="getVersionHistory('ctrl11')"
				@get-file="getFile()"
				@get-file-version="getFileVersion($event)"
				@delete-file="deleteFile('ctrl11')"
				@delete-last="deleteLast('ctrl1')"
				@delete-history="deleteHistory('ctrl11')"
				@show-popup="openPopup($event)"
				@hide-popup="closePopup($event)" />
		</base-input-structure>
	</div>

	<div
		class="container-fluid"
		style="flex: 1 1 auto">
		<!-- This control with id CTRL_12 is for showing file upload with x-large size -->
		<base-input-structure
			id="CTRL_12"
			label="File upload component with extra large size">
			<q-document
				id="Document-CTRL_12"
				size="xlarge"
				v-model="ctrl12.fileName"
				:versioning-is-on="false"
				:editing="ctrl12.editing"
				:current-version="ctrl12.currentVersion"
				:versions="ctrl12.versions"
				:versions-info="ctrl12.versionsInfo"
				:file-properties="ctrl12.fileProperties"
				:popup-is-visible="popupIsVisible"
				@submit-file="submitFile('ctrl12', $event)"
				@edit-file="editFile('ctrl12')"
				@get-properties="getProperties('ctrl12')"
				@get-version-history="getVersionHistory('ctrl12')"
				@get-file="getFile()"
				@get-file-version="getFileVersion($event)"
				@delete-file="deleteFile('ctrl12')"
				@delete-last="deleteLast('ctrl12')"
				@delete-history="deleteHistory('ctrl12')"
				@show-popup="openPopup($event)"
				@hide-popup="closePopup($event)" />
		</base-input-structure>
	</div>
</template>

<script>
	import fakeData from './Document.mock.js'

	function getNewObj()
	{
		return {
			fileName: '',
			editing: false,
			currentVersion: '',
			versions: {},
			versionsInfo: [],
			fileProperties: {}
		}
	}

	function downloadTestFile(version)
	{
		const data = 'This is ' + (version ? `the version "${version}" of ` : '') + 'a test file!'
		const fileName = version ? `Test.v${version}.txt` : 'Test.txt'

		const url = window.URL.createObjectURL(new Blob([data]))
		const link = document.createElement('a')
		link.href = url
		link.setAttribute('download', fileName)
		link.click()
	}

	export default {
		name: 'QDocumentContainer',

		docsfile: './docs/inputs/document/QDocument.md',

		emits: [
			'show-popup',
			'hide-popup'
		],

		inheritAttrs: false,

		props: {
			/**
			 * Whether the popup is currently visible.
			 */
			popupIsVisible: {
				type: Boolean,
				default: false
			}
		},

		expose: [],

		data()
		{
			return {
				...fakeData.simpleUsage(),
				ctrl1: getNewObj(),
				ctrl2: getNewObj(),
				ctrl3: getNewObj(),
				ctrl4: getNewObj(),
				ctrl5: getNewObj(),
				ctrl6: getNewObj(),
				ctrl7: getNewObj(),
				ctrl8: getNewObj(),
				ctrl9: getNewObj(),
				ctrl10: getNewObj(),
				ctrl11: getNewObj(),
				ctrl12: getNewObj()
			}
		},

		mounted()
		{
			this.ctrl1.fileName = this.currentFileName
			this.ctrl1.versions = this.versionsObj
			this.ctrl1.currentVersion = this.lastVersion

			this.ctrl4.fileName = this.currentFileName
			this.ctrl4.versions = this.versionsObj
			this.ctrl4.currentVersion = this.lastVersion

			this.ctrl5.fileName = this.currentFileName
			this.ctrl5.editing = true
			this.ctrl5.versions = this.versionsObj
			this.ctrl5.currentVersion = this.lastVersion
		},

		methods: {
			submitFile(ctrl, data)
			{
				// eslint-disable-next-line no-console
				console.log('Submit file emit was triggered.', data)

				if (data.file)
					this[ctrl].fileName = data.file.name
				this[ctrl].editing = false
				this[ctrl].currentVersion = data.version
				this[ctrl].versions[data.version] = '00000000-0000-0000-0000-000000000000'
			},

			editFile(ctrl)
			{
				// eslint-disable-next-line no-console
				console.log('Edit file emit was triggered.')

				this[ctrl].editing = true
			},

			getProperties(ctrl)
			{
				// eslint-disable-next-line no-console
				console.log('Get properties emit was triggered.')

				this[ctrl].fileProperties = {}
				setTimeout(() => {
					this[ctrl].fileProperties = this.fileProperties
				}, 100)
			},

			getVersionHistory(ctrl)
			{
				// eslint-disable-next-line no-console
				console.log('Get version history emit was triggered.')

				this[ctrl].versionsInfo = []
				setTimeout(() => {
					this[ctrl].versionsInfo = this.versionsInfo
				}, 100)
			},

			getFile()
			{
				// eslint-disable-next-line no-console
				console.log('Get file emit was triggered.')

				downloadTestFile()
			},

			getFileVersion(version)
			{
				// eslint-disable-next-line no-console
				console.log('Get file version emit was triggered.', version)

				downloadTestFile(version)
			},

			deleteFile(ctrl)
			{
				// eslint-disable-next-line no-console
				console.log('Delete file emit was triggered.')

				this[ctrl].fileName = ''
				this[ctrl].editing = false
				this[ctrl].currentVersion = ''
				this[ctrl].versions = {}
			},

			deleteLast(ctrl)
			{
				// eslint-disable-next-line no-console
				console.log('Delete last version emit was triggered.')

				this[ctrl].currentVersion = this.firstVersion
				this[ctrl].versions = this.firstVersionObj
			},

			deleteHistory(ctrl)
			{
				// eslint-disable-next-line no-console
				console.log('Delete history emit was triggered.')

				this[ctrl].currentVersion = this.lastVersion
				this[ctrl].versions = this.lastVersionObj
			},

			openPopup(modalData)
			{
				// eslint-disable-next-line no-console
				console.log(`Opened modal "${modalData.id}".`)

				this.$emit('show-popup', modalData)
			},

			closePopup(modalId)
			{
				// eslint-disable-next-line no-console
				console.log(`Closed modal "${modalId}".`)

				this.$emit('hide-popup', modalId)
			}
		}
	}
</script>
