<template>
	<teleport :to="`#q-modal-file-versions-${controlId}-body`">
		<q-table
			:rows="tableRows"
			:columns="tableColumns"
			:config="tableConfig"
			@row-action="findVersionToDownload($event)" />
	</teleport>

	<teleport
		v-if="!readonly"
		:to="`#q-modal-file-versions-${controlId}-footer`">
		<div class="actions">
			<q-button
				b-style="primary"
				:label="texts.deleteLastLabel"
				@click="$emit('delete-last')">
				<q-icon icon="delete" />
			</q-button>

			<q-button
				b-style="secondary"
				:label="texts.deleteHistoryLabel"
				@click="$emit('delete-history')">
				<q-icon icon="delete" />
			</q-button>
		</div>
	</teleport>
</template>

<script>
	import QTable from '@/components/table/QTable.vue'

	export default {
		name: 'QDocumentVersions',

		emits: {
			'delete-history': () => true,
			'delete-last': () => true,
			'get-file': (payload) => typeof payload === 'string'
		},

		components: {
			QTable
		},

		inheritAttrs: false,

		props: {
			/**
			 * Unique ID for the control.
			 */
			controlId: String,

			/**
			 * Necessary strings to be used in labels and buttons.
			 */
			texts: {
				type: Object,
				required: true
			},

			/**
			 * Whether the field is readonly.
			 */
			readonly: {
				type: Boolean,
				default: false
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
			 * The resources path.
			 */
			resourcesPath: {
				type: String,
				required: true
			}
		},

		expose: [],

		computed: {
			/**
			 * The detailed history of versions, in the format the QTable component expects.
			 */
			tableRows()
			{
				const rows = []

				if (this.versionsInfo && this.versionsInfo.length > 0)
				{
					for (let i = 0; i < this.versionsInfo.length; i++)
					{
						const row = {
							Rownum: i,
							Fields: this.versionsInfo[i],
							rowKey: this.versionsInfo[i].id
						}

						rows.push(row)
					}
				}

				return rows
			},

			/**
			 * The configuration of the versions history table.
			 */
			tableConfig()
			{
				return {
					customActions: [
						{
							id: 'download',
							name: 'download',
							title: this.texts.downloadLabel,
							icon: {
								icon: 'download',
								type: 'svg'
							},
							isInReadOnly: true
						}
					],
					globalSearch: {
						visibility: false
					},
					rowValidation: {
						fnValidate: (row) => row.rowKey?.length > 0,
						message: this.texts.pendingDocumentVersion
					},
					resourcesPath: this.resourcesPath
				}
			},

			/**
			 * The columns of the versions history table.
			 */
			tableColumns()
			{
				return [
					{
						order: 1,
						dataType: 'Text',
						label: this.texts.version,
						name: 'version',
						sortable: true
					},
					{
						order: 2,
						dataType: 'Text',
						label: this.texts.documentLabel,
						name: 'fileName',
						sortable: true
					},
					{
						order: 3,
						dataType: 'Text',
						label: this.texts.bytesLabel,
						name: 'bytes',
						sortable: true
					},
					{
						order: 4,
						dataType: 'Text',
						label: this.texts.author,
						name: 'author',
						sortable: true
					},
					{
						order: 5,
						dataType: 'Text',
						label: this.texts.createdOnLabel,
						name: 'createdOn',
						sortable: true
					}
				]
			}
		},

		methods: {
			/**
			 * Finds the number of the version to be downloaded, according to the key of the clicked row.
			 * @param {object} rowData The data of the clicked row
			 */
			findVersionToDownload(rowData)
			{
				if (typeof rowData?.rowKey !== 'string')
					return

				for (let i in this.versions)
				{
					if (this.versions[i] === rowData.rowKey)
					{
						this.$emit('get-file', i)
						return
					}
				}
			}
		}
	}
</script>
