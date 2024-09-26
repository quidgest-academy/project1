<template>
	<div class="row">
		<div class="col-md-6">
			<div class="g-reporting__flow-search">
				<q-input-group>
					<q-text-field
						v-model="searchInput"
						:placeholder="texts.fieldSearch" />
					<template #append>
						<q-button
							id="search-button"
							:title="texts.search">
							<q-icon icon="search" />
						</q-button>
					</template>
				</q-input-group>
			</div>

			<div class="g-reporting__flow-content-selectable">
				<q-row-container is-large>
					<q-control-wrapper class="row-line-group">
						<q-accordion-container id="cav-groupings-select">
							<template
								v-for="table in filteredTables"
								:key="table.Id">
								<q-group-collapsible
									v-if="!isEmpty(table.Fields)"
									accordion-id="cav-groupings-select"
									:label="getCavText(table.Description)"
									:is-open="openGroup === `table-${table.Id}`"
									@state-changed="setGroupState(`table-${table.Id}`, $event)">
									<ul>
										<template
											v-for="field in table.Fields"
											:key="field.Id">
											<li
												v-if="!isEmpty(field.Description)"
												class="g-reporting__flow-content-selectable-item"
												@click.stop.prevent="addNewGroupField(field)">
												{{ getCavText(field.Description) }}
												<q-icon
													icon="circle-arrow-right"
													class="q-icon--reporting" />
											</li>
										</template>
									</ul>
								</q-group-collapsible>
							</template>
						</q-accordion-container>
					</q-control-wrapper>
				</q-row-container>
			</div>
		</div>

		<div class="col-md-6">
			<div>
				<h4 class="g-reporting__flow-subtitle">{{ texts.groups }}</h4>
			</div>

			<div class="g-reporting__flow-content">
				<div class="g-reporting__flow-content-droppable">
					<ul v-if="!isEmpty(groupings)">
						<li
							v-for="(group, idxGroup) in groupings"
							:key="group.internalKey">
							<div
								class="row"
								style="padding: 0.5rem 0 0.05rem 0.5rem">
								<div class="col-12">
									<q-control-wrapper class="control-join-group">
										<base-input-structure
											:id="group.internalKey"
											class="i-checkbox"
											label-position="left"
											:label="`${texts.group} ${idxGroup + 1} - ${texts.pageBreak}`"
											:label-attrs="{ class: 'i-checkbox i-checkbox__label' }">
											<template #label>
												<q-checkbox-input
													:id="group.internalKey"
													v-model="group.PageBreak" />
											</template>
										</base-input-structure>
									</q-control-wrapper>
								</div>
							</div>

							<div class="row">
								<div class="col-12">
									<ul v-if="!isEmpty(group.Fields)">
										<li
											v-for="field in group.Fields"
											:key="field.FieldId"
											class="g-reporting__flow-content-droppable-item">
											<div class="row">
												<div class="col-12">
													<strong>[{{ getCavText(field.tableTitle) }}]</strong>

													<q-button
														b-style="secondary"
														@click="removeField(group.internalKey, field.FieldId)">
														<q-icon
															icon="bin"
															class="g-reporting__i-remove" />
													</q-button>
												</div>
											</div>

											<div class="row">
												<div class="col-12">
													{{ getCavText(field.Title) }}
												</div>
											</div>
										</li>
									</ul>
								</div>
							</div>
						</li>
					</ul>
				</div>
			</div>
		</div>
	</div>
</template>

<script>
	import { computed } from 'vue'
	import { v4 as uuidv4 } from 'uuid'
	import _isEmpty from 'lodash-es/isEmpty'
	import _map from 'lodash-es/map'
	import _filter from 'lodash-es/filter'
	import _startsWith from 'lodash-es/startsWith'
	import _toLower from 'lodash-es/toLower'
	import _assignIn from 'lodash-es/assignIn'
	import _find from 'lodash-es/find'
	import _findIndex from 'lodash-es/findIndex'
	import _foreach from 'lodash-es/forEach'
	import _some from 'lodash-es/some'

	import hardcodedTexts from '@/hardcodedTexts.js'

	/**
	 * @displayName GroupBySelected
	 */
	export default {
		name: 'QCavGroupBySelected',

		emits: ['update-groupings'],

		props: {
			/**
			 * Tables
			 */
			tables: {
				type: Array,
				default: () => []
			},

			/**
			 * Previously selected fields
			 */
			fieldsSelectedList: {
				type: Array,
				default: () => []
			},

			/**
			 * Previously selected groupings
			 */
			selectedGroupings: {
				type: Array,
				default: () => []
			}
		},

		inject: ['getCavText'],

		expose: [],

		data()
		{
			return {
				searchInput: '',

				groupings: [],

				texts: {
					fieldSearch: computed(() => this.Resources[hardcodedTexts.fieldSearch]),
					search: computed(() => this.Resources[hardcodedTexts.search]),
					groups: computed(() => this.Resources[hardcodedTexts.groups]),
					group: computed(() => this.Resources[hardcodedTexts.group]),
					pageBreak: computed(() => this.Resources[hardcodedTexts.pageBreak])
				},

				openGroup: ''
			}
		},

		created()
		{
			if (!_isEmpty(this.selectedGroupings))
				_foreach(this.selectedGroupings, (group) => this.groupings.push(this.hydrateGroup(group)))
			else
				this.groupings.push(this.hydrateGroup({}))
		},

		beforeMount()
		{
			this.$eventHub.emit('main-container-is-visible', false)
		},

		beforeUnmount()
		{
			this.$emit('update-groupings', this.groupings)
		},

		computed: {
			filteredTables()
			{
				return _map(this.tables, (table) => {
					return {
						Id: table.Id,
						TableUpId: table.TableUpId,
						Description: table.Description,
						Fields: _filter(table.Fields, (field) => {
							return (
								_findIndex(this.fieldsSelectedList, (f) => f.FieldId === field.Id) !== -1 &&
								!_some(this.groupings, { Fields: [{ FieldId: field.Id }] }) &&
								(_isEmpty(this.searchInput)
									? true
									: _startsWith(_toLower(this.getCavText(field.Description)), _toLower(this.searchInput)))
							)
						})
					}
				})
			}
		},

		methods: {
			isEmpty: _isEmpty,

			hydrateGroupField(fld)
			{
				let table = _find(this.tables, (t) => t.Id === fld.TableId),
					field = _find(table.Fields, (f) => f.Id === fld.FieldId)

				let extendedField = _assignIn(fld, {
					tableTitle: table.Description,
					Title: field.Description
				})

				return extendedField
			},

			hydrateGroup(group)
			{
				let extendedGroup = {
					internalKey: uuidv4(),
					PageBreak: group.PageBreak || false,
					Fields: []
				}

				_foreach(group.Fields, (field) => extendedGroup.Fields.push(this.hydrateGroupField(field)))

				return extendedGroup
			},

			addNewGroupField(field)
			{
				this.groupings[this.groupings.length - 1].Fields.push(
					this.hydrateGroupField({
						FieldId: field.Id,
						TableId: field.TableId
					})
				)
			},

			/**
			 * Remove group field by group internal key and field id
			 */
			removeField(groupInternalKey, fieldId)
			{
				let idxOfGroup = _findIndex(this.groupings, (g) => g.internalKey === groupInternalKey)

				if (idxOfGroup !== -1)
				{
					let idxToRemove = _findIndex(this.groupings[idxOfGroup].Fields, (f) => f.FieldId === fieldId)
					if (idxToRemove !== -1)
						this.groupings[idxOfGroup].Fields.splice(idxToRemove, 1)
				}
			},

			/**
			 * Sets the state of the specified group, either open or closed.
			 * @param {string} groupId The id of the group
			 * @param {boolean} isOpen Whether or not the group is open
			 */
			setGroupState(groupId, isOpen)
			{
				if (isOpen)
					this.openGroup = groupId
				else if (this.openGroup === groupId)
					this.openGroup = ''
			}
		}
	}
</script>
