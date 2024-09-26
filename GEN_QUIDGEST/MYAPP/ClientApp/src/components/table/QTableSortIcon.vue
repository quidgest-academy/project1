<template>
	<div class="float-right column-sort-icon">
		<template v-if="order === 'asc'">
			<slot name="vbt-sort-asc-icon"></slot>
		</template>
		<template v-else-if="order === 'desc'">
			<slot name="vbt-sort-desc-icon"></slot>
		</template>
		<template v-else>
			<slot name="vbt-no-sort-icon"></slot>
		</template>
	</div>
</template>

<script>
	import findIndex from 'lodash-es/findIndex'

	export default {
		name: 'QTableSortIcon',

		props: {
			/**
			 * An array of objects representing the current sort state of the table, with each object corresponding to a column.
			 */
			sort: {
				type: Array,
				default: () => []
			},

			/**
			 * The configuration object for the column associated with this sort icon.
			 */
			column: {
				type: Object,
				default: () => ({})
			}
		},

		expose: [],

		computed: {
			order()
			{
				let index = findIndex(this.sort, {
					vbtColId: this.column.vbtColId
				})

				if (index === -1) {
					return null
				}
				return this.sort[index].order
			}
		}
	}
</script>
