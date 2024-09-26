<template>
	<div
		class="container-fluid"
		style="flex: 1 1 auto">
		<form class="form-horizontal">
			<fieldset>
				<div class="form-flow">
					<h3>Multi point Google map</h3>

					<q-row-container is-large>
						<q-control-wrapper id="CTRL_1">
							<q-map
								:api-key="extraProperties.apiKeys.google"
								:subtype="extraProperties.subtypes.google"
								:container-id="`${map1.viewMode.containerId}_1`"
								:mapped-values="map1.viewMode.mappedValues"
								:style-variables="map1.viewMode.styleVariables"
								:is-single-point="map1.viewMode.isSinglePoint"
								:list-config="map1.listConfig"
								@row-action="handleRowAction"
								@set-marker="handleSetMarker('map1', $event)"
								@remove-marker="handleRemoveMarker('map1', $event)" />
						</q-control-wrapper>
					</q-row-container>

					<hr />

					<h3>Multi point Leaflet map</h3>

					<q-row-container is-large>
						<q-control-wrapper id="CTRL_2">
							<q-map
								:subtype="extraProperties.subtypes.leaflet"
								:container-id="`${map2.viewMode.containerId}_2`"
								:mapped-values="map2.viewMode.mappedValues"
								:style-variables="map2.viewMode.styleVariables"
								:is-single-point="map2.viewMode.isSinglePoint"
								:list-config="map2.listConfig"
								:overlays="extraProperties.overlays"
								@row-action="handleRowAction"
								@set-marker="handleSetMarker('map2', $event)"
								@remove-marker="handleRemoveMarker('map2', $event)" />
						</q-control-wrapper>
					</q-row-container>

					<hr />

					<h3>Multi point Google map (Not editable)</h3>

					<q-row-container is-large>
						<q-control-wrapper id="CTRL_3">
							<q-map
								:api-key="extraProperties.apiKeys.google"
								:subtype="extraProperties.subtypes.google"
								:container-id="`${map3.viewMode.containerId}_3`"
								:mapped-values="map3.viewMode.mappedValues"
								:style-variables="map3.viewMode.styleVariables"
								:is-single-point="map3.viewMode.isSinglePoint"
								:list-config="map3.listConfig"
								readonly
								@row-action="handleRowAction"
								@set-marker="handleSetMarker('map3', $event)"
								@remove-marker="handleRemoveMarker('map3', $event)" />
						</q-control-wrapper>
					</q-row-container>

					<hr />

					<h3>Multi point Leaflet map (Not editable)</h3>

					<q-row-container is-large>
						<q-control-wrapper id="CTRL_4">
							<q-map
								:subtype="extraProperties.subtypes.leaflet"
								:container-id="`${map4.viewMode.containerId}_4`"
								:mapped-values="map4.viewMode.mappedValues"
								:style-variables="map4.viewMode.styleVariables"
								:is-single-point="map4.viewMode.isSinglePoint"
								:list-config="map4.listConfig"
								readonly
								:overlays="extraProperties.overlays"
								@row-action="handleRowAction"
								@set-marker="handleSetMarker('map4', $event)"
								@remove-marker="handleRemoveMarker('map4', $event)" />
						</q-control-wrapper>
					</q-row-container>

					<hr />

					<h3>Single point Google map</h3>

					<q-row-container is-large>
						<q-control-wrapper id="CTRL_5">
							<q-map
								:api-key="extraProperties.apiKeys.google"
								:subtype="extraProperties.subtypes.google"
								:container-id="`${map5.viewMode.containerId}_5`"
								:mapped-values="map5.viewMode.mappedValues"
								:style-variables="map5.viewMode.styleVariables"
								:is-single-point="map5.viewMode.isSinglePoint"
								@row-action="handleRowAction"
								@set-marker="handleSetMarker('map5', $event)"
								@remove-marker="handleRemoveMarker('map5', $event)" />
						</q-control-wrapper>
					</q-row-container>

					<hr />

					<h3>Single point Leaflet map</h3>

					<q-row-container is-large>
						<q-control-wrapper id="CTRL_6">
							<q-map
								:subtype="extraProperties.subtypes.leaflet"
								:container-id="`${map6.viewMode.containerId}_6`"
								:mapped-values="map6.viewMode.mappedValues"
								:style-variables="map6.viewMode.styleVariables"
								:is-single-point="map6.viewMode.isSinglePoint"
								@row-action="handleRowAction"
								@set-marker="handleSetMarker('map6', $event)"
								@remove-marker="handleRemoveMarker('map6', $event)" />
						</q-control-wrapper>
					</q-row-container>

					<hr />

					<h3>Single point Google map (Not editable)</h3>

					<q-row-container is-large>
						<q-control-wrapper id="CTRL_7">
							<q-map
								:api-key="extraProperties.apiKeys.google"
								:subtype="extraProperties.subtypes.google"
								:container-id="`${map7.viewMode.containerId}_7`"
								:mapped-values="map7.viewMode.mappedValues"
								:style-variables="map7.viewMode.styleVariables"
								:is-single-point="map7.viewMode.isSinglePoint"
								readonly
								@row-action="handleRowAction"
								@set-marker="handleSetMarker('map7', $event)"
								@remove-marker="handleRemoveMarker('map7', $event)" />
						</q-control-wrapper>
					</q-row-container>

					<hr />

					<h3>Single point Leaflet map (Not editable)</h3>

					<q-row-container is-large>
						<q-control-wrapper id="CTRL_8">
							<q-map
								:subtype="extraProperties.subtypes.leaflet"
								:container-id="`${map8.viewMode.containerId}_8`"
								:mapped-values="map8.viewMode.mappedValues"
								:style-variables="map8.viewMode.styleVariables"
								:is-single-point="map8.viewMode.isSinglePoint"
								readonly
								@row-action="handleRowAction"
								@set-marker="handleSetMarker('map8', $event)"
								@remove-marker="handleRemoveMarker('map8', $event)" />
						</q-control-wrapper>
					</q-row-container>

					<hr />

					<h3>Multi point drawable Leaflet map</h3>

					<q-row-container is-large>
						<q-control-wrapper id="CTRL_9">
							<q-map
								:subtype="extraProperties.subtypes.leaflet"
								:container-id="`${map9.viewMode.containerId}_9`"
								:mapped-values="map9.viewMode.mappedValues"
								:style-variables="map9.viewMode.styleVariables"
								:is-single-point="map9.viewMode.isSinglePoint"
								:list-config="map9.listConfig"
								is-drawable-map
								@row-action="handleRowAction"
								@set-marker="handleSetMarker('map9', $event)"
								@remove-marker="handleRemoveMarker('map9', $event)"
								@shape-clicked="handleShapeClick"
								@set-shapes="handleShapeSet('map9', $event)" />
						</q-control-wrapper>
					</q-row-container>

					<hr />

					<h3>Single point drawable Leaflet map</h3>

					<q-row-container is-large>
						<q-control-wrapper id="CTRL_10">
							<q-map
								:subtype="extraProperties.subtypes.leaflet"
								:container-id="`${map10.viewMode.containerId}_10`"
								:mapped-values="map10.viewMode.mappedValues"
								:style-variables="map10.viewMode.styleVariables"
								:is-single-point="map10.viewMode.isSinglePoint"
								:list-config="map10.listConfig"
								is-drawable-map
								@row-action="handleRowAction"
								@set-marker="handleSetMarker('map10', $event)"
								@remove-marker="handleRemoveMarker('map10', $event)"
								@shape-clicked="handleShapeClick"
								@set-shapes="handleShapeSet('map10', $event)" />
						</q-control-wrapper>
					</q-row-container>
				</div>
			</fieldset>
		</form>
	</div>
</template>

<script>
	import cloneDeep from 'lodash-es/cloneDeep'

	import QMap from '@/components/rendering/map/QMap.vue'

	import fakeData from './Map.mock.js'

	export default {
		name: 'QMapContainer',

		docsfile: './docs/rendering/map/QMap.md',

		components: {
			QMap
		},

		inheritAttrs: false,

		expose: [],

		data()
		{
			return {
				extraProperties: fakeData.extraProperties,
				map1: cloneDeep(fakeData.multiPointMap),
				map2: cloneDeep(fakeData.multiPointMap),
				map3: cloneDeep(fakeData.multiPointMap),
				map4: cloneDeep(fakeData.multiPointMap),
				map5: cloneDeep(fakeData.singlePointMap),
				map6: cloneDeep(fakeData.singlePointMap),
				map7: cloneDeep(fakeData.singlePointMap),
				map8: cloneDeep(fakeData.singlePointMap),
				map9: cloneDeep(fakeData.multiPointMap),
				map10: cloneDeep(fakeData.singlePointMap)
			}
		},

		methods: {
			handleRowAction(actionData)
			{
				// eslint-disable-next-line no-console
				console.log('Row action emit was triggered.')

				alert(JSON.stringify(actionData))
			},

			handleSetMarker(map, marker)
			{
				// eslint-disable-next-line no-console
				console.log('Set marker emit was triggered.')

				const isSinglePoint = this[map].viewMode.isSinglePoint
				const mappedValues = this[map].viewMode.mappedValues

				const newMarker = {
					rowKey: '00000000-0000-0000-0000-' + Math.floor((Math.random() * 1000000000000) + 1),
					geographicData: [
						{
							type: 'Geographic',
							value: marker.coords
						}
					]
				}

				if (isSinglePoint)
				{
					if (mappedValues.length === 0)
						mappedValues.push(newMarker)
					else
					{
						for (let val of mappedValues)
						{
							for (let geoData of val.geographicData)
							{
								if (geoData.type !== 'Geographic')
									continue

								geoData.value = marker.coords
							}
						}
					}
				}
				else if (typeof marker.rowKey === 'undefined')
					mappedValues.push(newMarker)
				else
				{
					for (let val of mappedValues)
					{
						if (val.rowKey === marker.rowKey)
						{
							for (let geoData of val.geographicData)
							{
								if (geoData.type !== 'Geographic')
									continue

								geoData.value = marker.coords
							}
						}
					}
				}
			},

			handleRemoveMarker(map, marker)
			{
				// eslint-disable-next-line no-console
				console.log('Remove marker emit was triggered.')

				const mappedValues = this[map].viewMode.mappedValues

				for (let val of mappedValues)
				{
					if (val.rowKey === marker.rowKey)
					{
						for (let geoData of val.geographicData)
						{
							if (geoData.type !== 'Geographic')
								continue

							geoData.value = null
							geoData.rawData = ''
						}
					}
				}
			},

			handleShapeClick(layerProps)
			{
				// eslint-disable-next-line no-console
				console.log('Shape click emit was triggered.')

				// Timeout to give time for the map to recenter on the clicked shape.
				setTimeout(() => alert(`Clicked on a shape belonging to layer "${layerProps.layerName}".`), 100)
			},

			handleShapeSet(map, shapes)
			{
				// eslint-disable-next-line no-console
				console.log('Shape set emit was triggered.')

				const mappedValues = this[map].viewMode.mappedValues

				for (let val of mappedValues)
				{
					for (let geoData of val.geographicData)
					{
						if (geoData.type === 'GeographicShape')
						{
							geoData.value = shapes
							return
						}
					}
				}
			}
		}
	}
</script>
