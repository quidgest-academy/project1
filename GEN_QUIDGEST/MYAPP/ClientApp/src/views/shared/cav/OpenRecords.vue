<template>
	<q-button
		b-style="secondary"
		borderless
		:title="texts.openReport"
		@click="loadQueryList">
		<q-icon icon="open-report" />
	</q-button>

	<teleport
		v-if="showQueryList"
		:to="`#q-modal-${modalId}-body`">
		<div class="content">
			<div class="bootbox-body">
				<table class="c-table c-table-hover table-resizable">
					<thead class="c-table__head">
						<tr>
							<th>{{ texts.queryName }}</th>
							<th>{{ texts.queryAccess }}</th>
						</tr>
					</thead>

					<tbody class="c-table__body">
						<tr
							v-for="row in reportList"
							:key="`cav-query-list-row-${row.ID}`"
							@click.stop.prevent="onSelectedQueryToLoad(row.ID)">
							<td>{{ row.Title }}</td>
							<td>{{ getAccessTypeTitle(row.Acess) }}</td>
						</tr>
					</tbody>
				</table>
			</div>
		</div>
	</teleport>
</template>

<script>
	import { computed } from 'vue'
	import { mapActions } from 'pinia'

	import { useGenericDataStore } from '@/stores/genericData.js'
	import { removeModal } from '@/mixins/genericFunctions.js'
	import { fetchData } from '@/api/network'
	import hardcodedTexts from '@/hardcodedTexts.js'

	export default {
		name: 'QCavOpenRecords',

		emits: ['load-query'],

		expose: [],

		data()
		{
			return {
				showQueryList: false,

				reportList: [],

				texts: {
					openReport: computed(() => this.Resources[hardcodedTexts.openReport]),
					selectQuery: computed(() => this.Resources[hardcodedTexts.selectQuery]),
					queryName: computed(() => this.Resources[hardcodedTexts.queryName]),
					queryAccess: computed(() => this.Resources[hardcodedTexts.queryAccess]),
					public: computed(() => this.Resources[hardcodedTexts.public]),
					personal: computed(() => this.Resources[hardcodedTexts.personal]),
					inactive: computed(() => this.Resources[hardcodedTexts.inactive])
				},

				modalId: 'cav-query-list'
			}
		},

		beforeUnmount()
		{
			this.fnHideQueryList()
		},

		methods: {
			...mapActions(useGenericDataStore, [
				'setModal'
			]),

			removeModal,

			fnShowQueryList()
			{
				const modalProps = {
					id: this.modalId,
					headerTitle: this.texts.selectQuery,
					closeButtonEnable: true,
					isActive: true,
					hideFooter: true,
					dismissWithEsc: true,
					dismissAction: this.fnHideQueryList
				}
				this.setModal(modalProps)

				this.$nextTick().then(() => this.showQueryList = true)
			},

			fnHideQueryList()
			{
				this.removeModal(this.modalId)
				this.showQueryList = false
			},

			/**
			 * Requests a list of saved queries
			 */
			loadQueryList()
			{
				fetchData('Cav', 'LoadQueryList', null, data => { // { ID, Title, Acess, Opercria }
					this.reportList = data
					this.fnShowQueryList()
				})
			},

			getAccessTypeTitle(access)
			{
				switch (access)
				{
					case 'PUB':
						return this.texts.public
					case 'PES':
						return this.texts.personal
					case 'INA':
						return this.texts.inactive
					default:
						return access || ''
				}
			},

			onSelectedQueryToLoad(queryId)
			{
				this.$emit('load-query', queryId)
				this.fnHideQueryList()
			}
		}
	}
</script>
