<template>
	<div
		ref="optionsMenu"
		:class="['q-document__dropdown', 'dropdown-menu', { show: isReady }]">
		<!-- Download menu item -->
		<q-button
			class="dropdown-item"
			data-testid="document-download"
			:title="texts.downloadLabel"
			:label="texts.downloadLabel"
			:disabled="!modelValue"
			@click="getFile('')">
			<q-icon icon="download" />
		</q-button>

		<!-- Attach menu item -->
		<q-button
			v-if="!readonly && !editing && (!versioning || !modelValue || versionCount === 0)"
			class="dropdown-item"
			data-testid="document-attach"
			:title="texts.attachLabel"
			:label="texts.attachLabel"
			@click="attachFile">
			<q-icon icon="attachment" />
		</q-button>

		<!-- Submit menu item -->
		<q-button
			v-if="!readonly && versioning && editing"
			class="dropdown-item"
			data-testid="document-submit"
			:title="texts.submitLabel"
			:label="texts.submitLabel"
			@click="submitFile">
			<q-icon icon="upload" />
		</q-button>

		<!-- Edit menu item -->
		<q-button
			v-if="!readonly && versioning && !editing && modelValue && versionCount > 0"
			class="dropdown-item"
			data-testid="document-edit"
			:title="texts.editLabel"
			:label="texts.editLabel"
			@click="editFile">
			<q-icon icon="pencil" />
		</q-button>

		<!-- Delete menu item -->
		<q-button
			v-if="!readonly"
			class="dropdown-item"
			data-testid="document-delete"
			:title="texts.deleteLabel"
			:label="texts.deleteLabel"
			:disabled="editing || !modelValue"
			@click="deleteFile">
			<q-icon icon="delete" />
		</q-button>

		<!-- Versions sub menu -->
		<div
			v-if="versioning && !editing && versionCount > 1"
			class="dropdown-submenu">
			<!-- Versions sub menu item -->
			<div :class="['dropdown-menu', { show: showSubMenu }]">
				<!-- View versions history menu item -->
				<q-button
					class="dropdown-item"
					data-testid="document-history"
					:title="texts.viewAll"
					:label="texts.viewAll"
					@click="viewAllVersions">
					<q-icon icon="properties" />
				</q-button>

				<div class="dropdown-divider" />

				<!-- Available versions -->
				<q-button
					v-for="version in visibleVersions"
					:key="version.key"
					:class="['dropdown-item', { 'q-document__dropdown-dirty': version.dirty }]"
					:title="version.dirty ? texts.pendingDocumentVersion : `${texts.downloadLabel} ${version.key}`"
					:label="version.key"
					@click="getFile(version.key)">
					<q-icon icon="download" />
					<q-icon
						v-if="version.dirty"
						icon="alert"
						class="q-document__dropdown-dirty-indicator" />
				</q-button>

				<div
					v-if="!readonly"
					class="dropdown-divider" />

				<!-- Delete last item menu -->
				<q-button
					v-if="!readonly"
					class="dropdown-item"
					data-testid="document-delete-last"
					:title="texts.deleteLastLabel"
					:label="texts.deleteLastLabel"
					@click="deleteLastVersion">
					<q-icon icon="delete" />
				</q-button>

				<!-- Delete history menu item -->
				<q-button
					v-if="!readonly"
					class="dropdown-item"
					data-testid="document-delete-history"
					:title="texts.deleteHistoryLabel"
					:label="texts.deleteHistoryLabel"
					@click="deleteFileHistory">
					<q-icon icon="delete" />
				</q-button>
			</div>
		</div>

		<!-- Versions dropdown menu item -->
		<q-button
			v-if="versioning && !editing && versionCount > 1"
			class="dropdown-item"
			data-testid="document-versions"
			:title="texts.versionsLabel"
			:label="texts.versionsLabel"
			@click="toggleSubMenu">
			<q-icon icon="list" />
		</q-button>

		<!-- Create document menu item -->
		<q-button
			v-if="!readonly && usesTemplates"
			class="dropdown-item"
			data-testid="document-create"
			:title="texts.createDocument"
			:label="texts.createDocument"
			@click="createDocument">
			<q-icon icon="plus" />
		</q-button>

		<div class="dropdown-divider" />

		<!-- Properties menu item -->
		<q-button
			class="dropdown-item"
			data-testid="document-properties"
			:title="texts.propertyLabel"
			:label="texts.propertyLabel"
			:disabled="!modelValue"
			@click="getProperties">
			<q-icon icon="properties" />
		</q-button>
	</div>
</template>

<script>
	import Popper from 'popper.js'

	export default {
		name: 'QDocumentDropdown',

		emits: {
			'attach-file': () => true,
			'close': () => true,
			'delete-file': () => true,
			'delete-history': () => true,
			'delete-last': () => true,
			'edit-file': () => true,
			'get-file': (payload) => typeof payload === 'string',
			'get-properties': () => true,
			'get-version-history': () => true,
			'show-templates-popup': () => true,
			'submit-file': () => true
		},

		inheritAttrs: false,

		props: {
			/**
			 * The name of the current version of the file.
			 */
			modelValue: String,

			/**
			 * The html element to which the dropdown should be anchored.
			 */
			anchor: Object,

			/**
			 * Necessary strings to be used in labels and buttons.
			 */
			texts: {
				type: Object,
				required: true
			},

			/**
			 * Whether or not versioning is active for the document.
			 */
			versioning: {
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
			 * Whether or not the document is currently being edited by someone.
			 */
			editing: {
				type: Boolean,
				default: false
			},

			/**
			 * The current version numbers of the document.
			 */
			versions: {
				type: Array,
				default: () => []
			},

			/**
			 * Indicates whether the control uses document templates.
			 */
			usesTemplates: {
				type: Boolean,
				default: false
			}
		},

		expose: [],

		data()
		{
			return {
				// The max number of visible versions in the submenu dropdown.
				maxVisibleVersions: 5,

				// Whether or not the versions dropdown sub-menu is visible.
				showSubMenu: false,

				// Whether the dropdown is ready to be displayed.
				isReady: false,

				// The options menu popper object.
				popper: null
			}
		},

		mounted()
		{
			this.setup()
			this.bindOutsideClickListener()
		},

		beforeUnmount()
		{
			this.unbindOutsideClickListener()
		},

		computed: {
			/**
			 * The number of document versions.
			 */
			versionCount()
			{
				return this.versions?.length ?? 0
			},

			/**
			 * An array with only the N most recent versions (N = maxVisibleVersions).
			 */
			visibleVersions()
			{
				return this.versions.slice(0, this.maxVisibleVersions)
			}
		},

		methods: {
			/**
			 * Sets up the dropdown.
			 */
			setup()
			{
				this.popper = new Popper(this.anchor, this.$refs.optionsMenu, {
					placement: 'bottom-start',
					onCreate: () => (this.isReady = true),
					modifiers: {
						preventOverflow: {
							enabled: true,
							boundariesElement: 'window'
						}
					}
				})
			},

			/**
			 * Emits the event to close the dropdown.
			 */
			close()
			{
				this.$emit('close')
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
			 */
			getFile(version)
			{
				this.$emit('get-file', version)
			},

			/**
			 * Emits the event to fetch the details of all versions of the document from the server.
			 */
			viewAllVersions()
			{
				this.$emit('get-version-history')
			},

			/**
			 * Emits the event to attach a new file.
			 */
			attachFile()
			{
				this.$emit('attach-file')
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
			},

			/**
			 * Emits the event to delete the document history.
			 */
			deleteFileHistory()
			{
				this.$emit('delete-history')
			},

			/**
			 * Emits the event to submit a new file version.
			 */
			submitFile()
			{
				this.$emit('submit-file')
			},

			/**
			 * Toggles the versions sub-menu state to either open or closed.
			 */
			toggleSubMenu()
			{
				this.showSubMenu = !this.showSubMenu
			},

			/**
			 * Triggered when the user clicks on the page.
			 * @param event The click event
			 */
			outsideClickListener(event)
			{
				if (!(this.anchor?.contains(event.target) ||
					this.$refs.optionsMenu?.contains(event.target)))
					this.close()
			},

			/**
			 * Binds a listener to check for a click event.
			 */
			bindOutsideClickListener()
			{
				// Remove any previous binding before adding a new one.
				this.unbindOutsideClickListener()
				window.addEventListener('mousedown', this.outsideClickListener)
			},

			/**
			 * Unbinds the click listener.
			 */
			unbindOutsideClickListener()
			{
				window.removeEventListener('mousedown', this.outsideClickListener)
			},

			/**
			 * Emit event to open popup with document templates.
			 */
			createDocument()
			{
				this.$emit('show-templates-popup')
			}
		}
	}
</script>
