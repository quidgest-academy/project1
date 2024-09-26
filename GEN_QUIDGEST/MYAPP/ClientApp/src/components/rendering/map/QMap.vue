<template>
	<div
		:id="controlId"
		:class="['q-maps', $attrs.class]"
		ref="mapContainer"
		@mousemove="updateMousePosition">
		<component
			:is="`q-${subtype}`"
			ref="map"
			:key="internalMapKey"
			:markers="markers"
			:shapes="shapes"
			:external-layers="externalLayers"
			:follow-up-action="followUpAction"
			v-bind="$props"
			@open-info-window="openInfoWindow"
			@close-info-window="closeInfoWindow"
			@is-ready="onMapIsReady"
			@export-map="$emit('export-map', $event)"
			@row-action="$emit('row-action', $event)"
			@set-marker="$emit('set-marker', $event)"
			@remove-marker="$emit('remove-marker', $event)"
			@set-shapes="$emit('set-shapes', $event)"
			@changed-center="$emit('changed-center', $event)"
			@changed-zoom="$emit('changed-zoom', $event)">
			<component
				v-if="currentMarker"
				:is="`q-${popupComponent}`"
				:texts="texts"
				:is-shape="isShape"
				:is-euclidean-coord="isEuclideanCoord"
				:marker="currentMarker"
				:marker-actions="markerActions"
				:style-variables="styleVariables"
				:resources-path="resourcesPath"
				:dismissible="!styleVariables.openPopupOnHover?.value"
				:mouse-x="mouseX"
				:mouse-y="mouseY"
				@close-info-window="closeInfoWindow"
				@row-action="$emit('row-action', $event)" />
		</component>
	</div>
</template>

<script>
	import { computed, defineAsyncComponent } from 'vue'
	import _debounce from 'lodash-es/debounce'

	import { validateTexts } from '@/mixins/genericFunctions.js'

	const DEFAULT_MAP = 'leaflet-map'
	const DEFAULT_POPUP = 'map-default-popup'

	const MAP_SUBTYPES = ['leaflet-map', 'google-map']
	const MAP_POPUPS = ['map-default-popup']

	// The texts needed by the component.
	const DEFAULT_TEXTS = {
		search: 'Search',
		defaultLayer: 'Default layer',
		clusterGroupLayer: 'Cluster group layer',
		shapesLayer: 'Shapes layer',
		latitude: 'Latitude',
		longitude: 'Longitude',
		description: 'Description:',
		cancel: 'Cancel',
		finish: 'Finish',
		deleteLastPoint: 'Delete last point',
		drawPolyline: 'Draw a polyline',
		drawPolygon: 'Draw a polygon',
		drawRectangle: 'Draw a rectangle',
		drawCircle: 'Draw a circle',
		drawMarker: 'Draw a marker',
		drawCircleMarker: 'Draw a circle marker',
		drawText: 'Draw text',
		radius: 'Radius',
		area: 'Area',
		perimeter: 'Perimeter',
		startCircleDraw: 'Click map to place circle center.',
		endCircleDraw: 'Click map to finish circle.',
		placeCircleMarker: 'Click map to place circle marker.',
		placeMarker: 'Click map to place marker.',
		placeText: 'Click map to place text.',
		startShapeDraw: 'Click to start drawing shape.',
		continueShapeDraw: 'Click to continue drawing shape.',
		endShapeDraw: 'Click first point to close this shape.',
		endLineDraw: 'Click last point to finish line.',
		endDrawing: 'Click to finish drawing.',
		editLayers: 'Edit layers',
		deleteLayers: 'Delete layers',
		dragLayers: 'Drag layers',
		cutLayers: 'Cut layers',
		rotateLayers: 'Rotate layers',
		scaleLayers: 'Scale layers',
		snapVertices: 'Snap dragged marker to other layers and vertices',
		pinVertices: 'Pin shared vertices together',
		autoTrace: 'Auto trace line',
		length: 'Length',
		segmentLength: 'Segment length',
		height: 'Height',
		width: 'Width',
		position: 'Position',
		positionMarker: 'Position marker',
		externalLayer: 'External layer',
		printMap: 'Print map',
		printLandscape: 'Landscape',
		printPortrait: 'Portrait'
	}

	export default {
		name: 'QMap',

		emits: [
			'changed-center',
			'changed-zoom',
			'export-map',
			'is-ready',
			'remove-marker',
			'row-action',
			'set-marker',
			'set-shapes',
			'shape-clicked'
		],

		components: {
			QMapDefaultPopup: defineAsyncComponent(() => import('./popups/QMapDefaultPopup.vue')),
			QLeafletMap: defineAsyncComponent(() => import('./maps/QLeafletMap.vue')),
			QGoogleMap: defineAsyncComponent(() => import('./maps/QGoogleMap.vue'))
		},

		inheritAttrs: false,

		props: {
			/**
			 * The unique identifier for the container.
			 */
			containerId: String,

			/**
			 * The map's API key.
			 */
			apiKey: String,

			/**
			 * The map version (used to force the re-render of the component).
			 */
			mapVersionKey: {
				type: Number,
				default: 0
			},

			/**
			 * The CRS to use for the map.
			 */
			crs: {
				type: String,
				default: 'EPSG:4326'
			},

			/**
			 * The name of the popup component that should be rendered.
			 */
			popupComponent: {
				type: String,
				validator: (value) => MAP_POPUPS.includes(value),
				default: DEFAULT_POPUP
			},

			/**
			 * The map type (must match it's vue component's name).
			 */
			subtype: {
				type: String,
				validator: (value) => MAP_SUBTYPES.includes(value),
				default: DEFAULT_MAP
			},

			/**
			 * Whether we are dealing with one point or several.
			 */
			isSinglePoint: {
				type: Boolean,
				default: false
			},

			/**
			 * Whether or not shapes can be drawn on the map.
			 */
			isDrawableMap: {
				type: Boolean,
				default: false
			},

			/**
			 * Whether we are dealing with euclidean or ellipsoidal coordinates.
			 */
			isEuclideanCoord: {
				type: Boolean,
				default: false
			},

			/**
			 * Whether or not to show an info window with details of the feature that was clicked.
			 */
			activateInfoWindow: {
				type: Boolean,
				default: true
			},

			/**
			 * Whether or not to display the map controls (if true overrides all the style variables).
			 */
			hideMapControls: {
				type: Boolean,
				default: false
			},

			/**
			 * The data from which we will display the markers.
			 */
			mappedValues: {
				type: Array,
				validator: (values) => values.every((el) => Array.isArray(el.geographicData)),
				default: () => []
			},

			/**
			 * The defined style variables.
			 */
			styleVariables: {
				type: Object,
				validator: (value) => {
					if (typeof value.zoomLevel === 'object' &&
						typeof value.zoomLevel.value === 'number' &&
						value.zoomLevel.value < -1)
						return false

					if (typeof value.centerCoord === 'object' &&
						typeof value.centerCoord.value === 'object' &&
						value.centerCoord.dataType === 'Geographic' &&
						(typeof value.centerCoord.value.lat !== 'number' || typeof value.centerCoord.value.lng !== 'number'))
						return false

					return true
				},
				default: () => ({})
			},

			/**
			 * The defined variable groups.
			 */
			groups: {
				type: Object,
				default: () => ({})
			},

			/**
			 * Whether or not the "read-only" mode is active.
			 */
			readonly: {
				type: Boolean,
				default: false
			},

			/**
			 * The necessary strings to be used inside the component.
			 */
			texts: {
				type: Object,
				validator: (value) => validateTexts(DEFAULT_TEXTS, value),
				default: () => DEFAULT_TEXTS
			},

			/**
			 * The configuration of the list, only available if the map is being used over a list.
			 */
			listConfig: {
				type: Object,
				default: () => ({})
			},

			/**
			 * Additional overlays to add to the base ones.
			 */
			overlays: {
				type: Array,
				default: () => []
			},

			/**
			 * Necessary tokens to access some of the external services.
			 */
			tokens: {
				type: Object,
				default: () => ({})
			},

			/**
			 * Functions defined manually to perform certain specific tasks.
			 */
			customFunctions: {
				type: Object,
				default: () => ({})
			},

			/**
			 * Path for the resources.
			 */
			resourcesPath: {
				type: String,
				default: ''
			}
		},

		expose: [],

		data()
		{
			return {
				controlId: this.containerId || `q-map-${this._.uid}`,
				map: null,
				isContainerReady: false,
				internalMapKey: 0,
				markers: [],
				shapes: [],
				externalLayers: [],
				currentMarker: null,
				resizeObserver: null,
				isShape: false,
				mouseX: 0,
				mouseY: 0
			}
		},

		mounted()
		{
			if (typeof ResizeObserver !== 'undefined')
			{
				this.resizeObserver = new ResizeObserver(_debounce(() => {
					if (this.map?.isReady && !this.map?.isFullscreen)
					{
						// We don't want this to be triggered the first time the map is loaded,
						// only when there's a resize of it's container.
						if (this.isContainerReady)
							this.internalMapKey++
						else
							this.isContainerReady = true
					}
				}, 400))
				this.resizeObserver.observe(this.$refs.mapContainer)
			}
		},

		beforeUnmount()
		{
			this.resizeObserver?.disconnect()
		},

		computed: {
			/**
			 * The available actions for the markers.
			 */
			markerActions()
			{
				if (this.isSinglePoint || !this.listConfig)
					return []

				var actions = []

				if (this.listConfig.customActions && this.listConfig.customActions.length > 0)
					actions = actions.concat(this.listConfig.customActions)
				if (this.listConfig.crudActions && this.listConfig.crudActions.length > 0)
					actions = actions.concat(this.listConfig.crudActions)

				return actions
			},

			/**
			 * The id of the follow up action for the markers and shapes.
			 */
			followUpAction()
			{
				if (!this.isSinglePoint && this.listConfig && this.listConfig.rowClickAction)
					return this.listConfig.rowClickAction.id
				return undefined
			}
		},

		methods: {
			/**
			 * Called to set the mouse coordinates.
			 * @param {object} event Information about the cursor
			 */
			updateMousePosition(event)
			{
				this.mouseX = event.clientX
				// MouseY position is inverted to be used as bottom of the element
				this.mouseY = window.innerHeight - event.clientY
			},

			/**
			 * Called to set the map state as either ready or not ready.
			 * @param {object} map The map node object
			 */
			onMapIsReady(map)
			{
				this.$emit('is-ready', map)
				this.map = this.$refs.map
			},

			/**
			 * Opens an info window with the specified geographic data.
			 * @param {object} marker The geographic data of the marker
			 */
			openInfoWindow(geoData)
			{
				const hoverPopup = this.styleVariables.openPopupOnHover?.value

				if (geoData.shapeClicked)
					this.$emit('shape-clicked', geoData.marker)

				if (geoData.shapeClicked !== hoverPopup && this.activateInfoWindow)
				{
					this.currentMarker = geoData.marker
					this.isShape = geoData.isShape

					this.setCurrentMarkerDescription()
				}
			},

			/**
			 * Closes the info window that's currently open.
			 */
			closeInfoWindow()
			{
				this.currentMarker = null
			},

			/**
			 * If current marker has dependencies from the provided description, updates it.
			 * @param {object} descriptions The marker descriptions
			 */
			setCurrentMarkerDescription(descriptions)
			{
				if (this.currentMarker === null)
					return

				const currentDesc = this.currentMarker.description

				for (let description of descriptions ?? [])
					for (let i = 0; i < currentDesc.length; i++)
						if (currentDesc[i].source?.id === description?.source?.id)
							currentDesc[i] = description

				// Additional data can be set through the "getFeatureData" custom function.
				const getFeatureData = this.customFunctions.getFeatureData

				if (typeof getFeatureData === 'function')
				{
					const featureData = getFeatureData(this.currentMarker)

					if (Array.isArray(featureData))
					{
						this.currentMarker.description = [
							...(currentDesc ?? []),
							...featureData
						]
					}
				}
			},

			/**
			 * Populates the lists of markers and shapes to display on the map.
			 */
			setMarkersAndShapes()
			{
				this.markers = []
				this.shapes = []

				for (let mappedData of this.mappedValues)
				{
					for (let geographicVal of mappedData.geographicData ?? [])
					{
						// If the value is empty, we ignore it.
						if (!geographicVal?.value)
							continue

						let feature = {}
						let descriptionTexts = []

						if (geographicVal.type === 'Geographic')
							feature = { coords: { ...geographicVal.value } }
						else if (geographicVal.type === 'GeographicShape')
							feature = { ...geographicVal.value }

						if (Array.isArray(mappedData.markerDescription))
						{
							for (let description of mappedData.markerDescription)
							{
								if (description)
								{
									const text = {
										source: description.source,
										textLabel: computed(() => description.source.label || description.source.description),
										textValue: String(description.value)
									}

									descriptionTexts.push(text)
								}
							}

							this.setCurrentMarkerDescription(descriptionTexts)
						}

						feature.description = descriptionTexts
						if (mappedData.rowKey)
							feature.rowKey = mappedData.rowKey
						if (mappedData.btnPermission)
							feature.btnPermission = { ...mappedData.btnPermission }
						if (mappedData.markerFollowup)
							feature.followup = mappedData.markerFollowup.value
						if (mappedData.markerIcon)
							feature.icon = mappedData.markerIcon.value
						if (mappedData.polylineDataColor)
							feature.polylineColor = mappedData.polylineDataColor.value
						if (mappedData.polygonDataColor)
							feature.polygonColor = mappedData.polygonDataColor.value
						if (mappedData.circleDataColor)
							feature.circleColor = mappedData.circleDataColor.value
						if (typeof this.customFunctions.getColor === 'function')
							feature.getColor = this.customFunctions.getColor

						if (geographicVal.type === 'Geographic')
							this.markers.push(feature)
						else if (geographicVal.type === 'GeographicShape')
							this.shapes.push(feature)
					}
				}
			},

			/**
			 * Populates the lists of external layers to display on the map.
			 */
			setExternalLayers()
			{
				this.externalLayers = []

				if (!this.groups?.externalLayer)
					return

				for (const externalLayer of this.groups.externalLayer)
				{
					const layerUrl = externalLayer.externalLayerUrl.value

					if (!layerUrl)
						continue

					const layerData = {
						url: layerUrl,
						layerName: externalLayer.externalLayerName?.value ?? '',
						minZoom: externalLayer.externalLayerMinZoomToLoad?.value ?? 0,
						requestData: {
							token: this.tokens[externalLayer.externalLayerConfig?.value] ?? '',
							where: externalLayer.externalLayerQuery?.value || '1=1'
						},
						getColor: this.customFunctions.getColor
					}

					this.externalLayers.push(layerData)
				}
			}
		},

		watch: {
			mapVersionKey()
			{
				this.internalMapKey++
			},

			activateInfoWindow(val)
			{
				if (!val)
					this.closeInfoWindow()
			},

			tokens(newVal, oldVal)
			{
				let refreshLayers = false

				if (Object.keys(newVal).length !== Object.keys(oldVal).length)
					refreshLayers = true

				if (!refreshLayers)
				{
					for (let i in newVal)
					{
						if (newVal[i] !== oldVal[i])
						{
							refreshLayers = true
							break
						}
					}
				}

				if (refreshLayers)
					this.setExternalLayers()
			},

			mappedValues: {
				handler()
				{
					this.setMarkersAndShapes()
				},
				deep: true,
				immediate: true
			},

			'groups.externalLayer': {
				handler()
				{
					this.setExternalLayers()
				},
				deep: true,
				immediate: true
			}
		}
	}
</script>
