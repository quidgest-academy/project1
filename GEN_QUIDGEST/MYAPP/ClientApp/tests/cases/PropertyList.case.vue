<template>
	<div class="row">
		<div class="col-12">
			<div class="control-row-group">
				<div style="width: fit-content">
					<div id="CTRL_1">
						Editable Property List

						<q-property-list
							grouping-field="type"
							value-column-name="Val"
							primary-key-column-name="PrimaryKey"
							format="dateTimeSeconds"
							:rows="rows"
							:type-select-options="typeSelectOptions"
							:max-characters="15"
							:edit-property-menu-items="editPropertyMenuItems"
							:date-format="{ DateTimeSeconds: 'DD-MM-YYYY HH:mm:ss' }"
							@update-value="updateValue"
							@add-property="addNew"
							@delete-property="deleteProperty"
							@edit-property="editProperty"
							@update-type="updateType"
							@update-property-label="updateValue" />
					</div>

					<br />

					<div id="CTRL_2">
						Value Edit mode

						<q-property-list
							grouping-field="type"
							primary-key-column-name="PrimaryKey"
							value-column-name="Val"
							format="dateTimeSeconds"
							value-edit-mode
							:rows="rows"
							:type-select-options="typeSelectOptions"
							:max-characters="15"
							:edit-property-menu-items="editPropertyMenuItems"
							:date-format="{ DateTimeSeconds: 'DD-MM-YYYY HH:mm:ss' }"
							@update-value="updateValue"
							@add-property="addNew"
							@delete-property="deleteProperty"
							@edit-property="editProperty"
							@update-type="updateType"
							@update-property-label="updateValue" />
					</div>

					<br />

					<div id="CTRL_3">
						Disable edit Mode Property List

						<q-property-list
							grouping-field="type"
							primary-key-column-name="PrimaryKey"
							value-column-name="Val"
							format="dateTimeSeconds"
							readonly
							:rows="rows"
							:type-select-options="typeSelectOptions"
							:max-characters="15"
							:edit-property-menu-items="editPropertyMenuItems"
							:date-format="{ DateTimeSeconds: 'DD-MM-YYYY HH:mm:ss' }"
							@update-value="updateValue"
							@add-property="addNew"
							@delete-property="deleteProperty"
							@edit-property="editProperty"
							@update-type="updateType"
							@update-property-label="updateValue" />
					</div>
				</div>
			</div>
		</div>
	</div>
</template>

<script>
	import mockData from './PropertyList.mock'

	export default {
		name: 'QPropertyListContainer',

		docsfile: './docs/property-list/QPropertyList.md',

		expose: [],

		data()
		{
			return {
				menuItems: [
					{
						name: 'Edit label'
					},
					{
						name: 'Switch Type',
						children: [
							{ name: 'Text' },
							{ name: 'Boolean' },
							{ name: 'Enumarition' },
							{ name: 'calendar' }
						]
					},
					{
						name: 'Delete'
					}
				],
				selectedDropdown: 'None',
				rows: mockData.rows,
				typeSelectOptions: mockData.typeSelectOptions,
				editPropertyMenuItems: mockData.menuItems
			}
		},

		methods: {
			setSelectedItem(item)
			{
				this.selectedDropdown = item
			},

			updateValue({ PrimaryKey, Value })
			{
				var str = 'Primary Key' + ':' + JSON.stringify(PrimaryKey) + '\n'
				str += 'Updated Value' + ':' + JSON.stringify(Value) + '\n'

				alert(str)
			},

			updateType({ PrimaryKey, newType })
			{
				var str = 'Primary Key' + ':' + JSON.stringify(PrimaryKey) + '\n'
				str += 'Updated type' + ':' + JSON.stringify(newType) + '\n'

				alert(str)
			},

			deleteProperty(PrimaryKey)
			{
				var str = 'Primary Key' + ':' + JSON.stringify(PrimaryKey) + '\n'

				alert(str)
			},

			editProperty(PrimaryKey)
			{
				var str = 'Primary Key' + ':' + JSON.stringify(PrimaryKey) + '\n'

				alert(str)
			},

			addNew({ Property, Value, type })
			{
				var str = 'Property name' + ':' + JSON.stringify(Property) + '\n'
				str += 'Value' + ':' + JSON.stringify(Value) + '\n'
				str += 'Datatype' + ':' + JSON.stringify(type) + '\n'

				alert(str)
			}
		}
	}
</script>
