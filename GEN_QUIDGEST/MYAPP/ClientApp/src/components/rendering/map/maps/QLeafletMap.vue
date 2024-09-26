<template>
	<div
		class="map-area-container"
		:style="mapStyle">
		<l-map
			ref="map"
			class="leaflet-container"
			marker-zoom-animation
			use-global-leaflet
			:crs="crs"
			:zoom="zoomLevel"
			:min-zoom="mapMinZoom"
			:max-zoom="mapMaxZoom"
			:center="centerCoords"
			:max-bounds="mapMaxBounds"
			:options="mapOptions"
			@ready="initMap"
			@click="onMapClick"
			@zoomend="onZoomEnd"
			@update:zoom="updateZoom"
			@update:center="updateCenter"
			@move="closeInfoWindow">
			<slot />
		</l-map>
	</div>
</template>

<script>
	import { computed, nextTick, watch } from 'vue'
	import cloneDeep from 'lodash-es/cloneDeep'
	import _debounce from 'lodash-es/debounce'
	import proj4 from 'proj4'

	import { LMap } from '@vue-leaflet/vue-leaflet'
	import { OpenStreetMapProvider, GeoSearchControl } from 'leaflet-geosearch'
	import { GestureHandling } from 'leaflet-gesture-handling'
	import L from 'leaflet'

	import '@geoman-io/leaflet-geoman-free'
	import 'leaflet.featuregroup.subgroup'
	import 'leaflet.markercluster'

	import { imageObjToSrc } from '@/mixins/genericFunctions.js'
	import { addFullScreenMode } from './libraries/leaflet-fullscreen/leaflet-fullscreen.js'

	// The available shape types.
	const SHAPE_TYPES = {
		polyline: 'polyline',
		polygon: 'polygon',
		rectangle: 'rectangle',
		circle: 'circle',
		circlemarker: 'circlemarker',
		marker: 'marker'
	}

	const COORD_TYPES = {
		object: 'object',
		array: 'array'
	}

	export default {
		name: 'QLeafletMap',

		emits: {
			'changed-center': (payload) => typeof payload === 'object',
			'changed-zoom': (payload) => typeof payload === 'number',
			'close-info-window': () => true,
			'export-map': (payload) => typeof payload === 'boolean',
			'is-ready': (payload) => typeof payload === 'object',
			'open-info-window': (payload) => typeof payload === 'object',
			'remove-marker': (payload) => typeof payload === 'object',
			'row-action': (payload) => typeof payload === 'object',
			'set-marker': (payload) => typeof payload === 'object',
			'set-shapes': (payload) => typeof payload === 'object'
		},

		components: {
			LMap
		},

		inheritAttrs: false,

		props: {
			/**
			 * The follow-up action when a marker is clicked.
			 */
			followUpAction: String,

			/**
			 * The CRS to use for the map (documentation: https://leafletjs.com/reference.html#crs).
			 */
			crs: String,

			/**
			 * The necessary strings to be used inside the component.
			 */
			texts: {
				type: Object,
				required: true
			},

			/**
			 * Whether or not the "read-only" mode is active.
			 */
			readonly: {
				type: Boolean,
				default: false
			},

			/**
			 * To determine if we are dealing with one point or several.
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
			 * Whether or not to display the map controls (if true overrides all the style variables).
			 */
			hideMapControls: {
				type: Boolean,
				default: false
			},

			/**
			 * The list of markers to be displayed on the map.
			 */
			markers: {
				type: Array,
				default: () => []
			},

			/**
			 * A list with the layers of shapes/polygons already on the map.
			 */
			shapes: {
				type: Array,
				default: () => []
			},

			/**
			 * The defined style variables.
			 */
			styleVariables: {
				type: Object,
				default: () => ({})
			},

			/**
			 * Layers that come from external services.
			 */
			externalLayers: {
				type: Array,
				default: () => []
			},

			/**
			 * Additional overlays to add to the base ones.
			 */
			overlays: {
				type: Array,
				default: () => []
			}
		},

		expose: ['isReady', 'isFullscreen'],

		data()
		{
			return {
				zoomLevel: this.styleVariables.zoomLevel?.value ?? 2,
				mapMinZoom: this.styleVariables.minZoom?.value ?? 0,
				mapMaxZoom: this.styleVariables.maxZoom?.value ?? 18,
				mapOptions: {
					zoomControl: !this.styleVariables.disableControls?.value,
					gestureHandling: this.styleVariables.zoomWithCtrl?.rawValue ?? true,
					gestureHandlingOptions: {
						duration: 2000
					}
				},
				defaultOverlay: this.styleVariables.backgroundOverlay?.value || 'OpenStreetMap',
				baseOverlays: [
					{
						name: 'OpenStreetMap',
						url: 'https://tile.openstreetmap.org/{z}/{x}/{y}.png',
						attribution: '&copy; <a href="http://openstreetmap.org">OpenStreetMap</a>'
					},
					{
						name: 'Grayscale',
						url: 'https://server.arcgisonline.com/ArcGIS/rest/services/Canvas/World_Light_Gray_Base/MapServer/tile/{z}/{y}/{x}',
						attribution: 'Tiles &copy; Esri &mdash; Esri, DeLorme, NAVTEQ'
					},
					{
						name: 'Satellite',
						url: 'https://server.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer/tile/{z}/{y}/{x}',
						attribution: 'Tiles &copy; Esri &mdash; Source: Esri, i-cubed, USDA, USGS, AEX, GeoEye, Getmapping, Aerogrid, IGN, IGP, UPR-EGP, and the GIS User Community'
					}
				],
				// The colors used for each shape type.
				shapeColors: {
					polyline: computed(() => this.styleVariables.polylineColor?.value ?? '#079ede'),
					multipolyline: computed(() => this.styleVariables.polylineColor?.value ?? '#079ede'),
					polygon: computed(() => this.styleVariables.polygonColor?.value ?? '#118f13'),
					multipolygon: computed(() => this.styleVariables.polygonColor?.value ?? '#118f13'),
					circle: computed(() => this.styleVariables.circleColor?.value ?? '#f53505')
				},
				groupMarkersInCluster: this.styleVariables.groupMarkersInCluster?.value,
				isReady: false,
				isFullscreen: false,
				isResetting: false,
				isMoving: false,
				drawModeIsOn: false,
				editModeIsOn: false,
				centerCoords: [0, 0],
				activeLayers: [],
				layersList: [],
				map: null,
				baseMaps: {},
				baseLayers: {},
				controlLayer: null,
				shapesLayer: null,
				defaultMarkers: null,
				markersCluster: null,
				shapeMarkersCluster: null,
				fullscreenControl: null,
				scaleControl: null,
				zoomControl: null,
				searchControl: null,
				printControl: null
			}
		},

		computed: {
			/**
			 * A list with the mutually exclusive layer names.
			 */
			exclusiveLayers()
			{
				return [
					this.texts.defaultLayer,
					this.texts.clusterGroupLayer
				]
			},

			/**
			 * The CRS used in the provided coordinates.
			 */
			coordsCrs()
			{
				return this.styleVariables.crs?.value || this.crs
			},

			/**
			 * The CRS converter.
			 */
			projection()
			{
				return proj4(this.crs, this.coordsCrs)
			},

			/**
			 * The styles to be applied to the map.
			 */
			mapStyle()
			{
				const style = {}

				if (this.styleVariables.mapHeight?.value)
					style.height = this.styleVariables.mapHeight.value

				return style
			},

			/**
			 * True if the user can currently draw on the map, false otherwise.
			 */
			canDraw()
			{
				return !this.readonly && this.isDrawableMap && this.shapesLayer !== null
			},

			/**
			 * The maximum bounds of the map.
			 */
			mapMaxBounds()
			{
				let bounds = null,
					southWest = this.styleVariables.boundSouthWest?.value,
					northEast = this.styleVariables.boundNorthEast?.value

				if (typeof southWest === 'object' && typeof northEast === 'object')
				{
					bounds = [
						this.projectFromCoords(southWest, COORD_TYPES.array),
						this.projectFromCoords(northEast, COORD_TYPES.array)
					]
				}

				return bounds
			}
		},

		methods: {
			/**
			 * Checks if the provided coordinates have the same latitude and longitude.
			 * @param {object} coord1 The first coordinate
			 * @param {object} coord2 The second coordinate
			 * @retuns True if the two coordinates are equal, false otherwise.
			 */
			coordIsEqual(coord1, coord2)
			{
				return coord1?.lat === coord2?.lat && coord1?.lng === coord2?.lng
			},

			/**
			 * Checks whether or not the provided layer is visible on the map.
			 * @param {object} layer The layer to check
			 * @returns True if the layer is visible, false otherwise.
			 */
			isLayerVisible(layer)
			{
				return this.map.hasLayer(layer) && layer.options.visible !== false
			},

			/**
			 * Called when the user stops changing the zoom to set it's final value.
			 */
			onZoomEnd()
			{
				this.updateZoom(this.map.getZoom())
				this.isMoving = false
			},

			/**
			 * Updates the current value of the zoom.
			 * @param {number} zoom The new value for the zoom
			 */
			updateZoom(zoom)
			{
				if (this.zoomLevel === zoom)
					return

				this.zoomLevel = zoom
				this.$emit('changed-zoom', zoom)
				this.closeInfoWindow()
			},

			/**
			 * Updates the current value of the center.
			 * @param {object} center The new value for the center
			 */
			updateCenter(center)
			{
				if (this.isMoving || this.coordIsEqual(this.centerCoords, center))
					return

				this.centerCoords = this.projectFromCoords(center, COORD_TYPES.array, false)
				this.$emit('changed-center', this.projectToCoords(center))
				this.closeInfoWindow()
			},

			/**
			 * Emits the event to close the info window in case it's not a popup that opens on hover.
			 */
			closeInfoWindow()
			{
				if (!this.styleVariables.openPopupOnHover?.value)
					this.$emit('close-info-window')
			},

			/**
			 * Converts the specified coordinates to the correct CRS.
			 * @param {object} coords The coordinates (must be either an array [lat, lng] or an object { lat, lng })
			 * @param {function} converter The converter function to be used
			 * @param {object} outputType The type of the output, can be either object or array
			 * @param {boolean} inverted Whether the input coordinates are inverted ([lng, lat] instead of [lat, lng])
			 * @returns The converted coordinates, or null if the conversion failed.
			 */
			convertCoords(coords, converter, outputType = COORD_TYPES.object, inverted = false)
			{
				let convertedCoords = Array.isArray(coords) ? coords : [coords?.lat ?? 0, coords?.lng ?? 0]

				if (inverted)
					convertedCoords.reverse()

				// Only needs to convert the coordinates if the CRS's are different.
				if (this.crs !== this.coordsCrs && typeof converter === 'function')
					convertedCoords = (converter(convertedCoords.reverse()) ?? []).reverse()

				if (convertedCoords.length !== 2)
					return null

				if (outputType === COORD_TYPES.array)
					return convertedCoords
				else if (outputType === COORD_TYPES.object)
					return { lat: convertedCoords[0], lng: convertedCoords[1] }
				return null
			},

			/**
			 * Converts the specified coordinates from the CRS used inside the map to the one used outside.
			 * @param {object} coords The coordinates (must be either an array [lat, lng] or an object { lat, lng })
			 * @param {object} outputType The type of the output, can be either object or array
			 * @param {boolean} convert Whether to convert the coordinates to the expected CRS
			 * @param {boolean} inverted Whether the input coordinates are inverted ([lng, lat] instead of [lat, lng])
			 * @returns The converted coordinates, or null if the conversion failed.
			 */
			projectToCoords(coords, outputType = COORD_TYPES.object, convert = true, inverted = false)
			{
				return this.convertCoords(coords, convert ? this.projection.forward : null, outputType, inverted)
			},

			/**
			 * Converts the specified coordinates from the CRS used outside the map to the one used inside.
			 * @param {object} coords The coordinates (must be either an array [lat, lng] or an object { lat, lng })
			 * @param {object} outputType The type of the output, can be either object or array
			 * @param {boolean} convert Whether to convert the coordinates to the expected CRS
			 * @param {boolean} inverted Whether the input coordinates are inverted ([lng, lat] instead of [lat, lng])
			 * @returns The converted coordinates, or null if the conversion failed.
			 */
			projectFromCoords(coords, outputType = COORD_TYPES.object, convert = true, inverted = false)
			{
				return this.convertCoords(coords, convert ? this.projection.inverse : null, outputType, inverted)
			},

			/**
			 * Converts the coordinates in the specified list to their list format.
			 * @param {object} coordsList The list of coordinates
			 * @param {function} converter The converter function to be used
			 * @param {object} outputType The type of the output, can be either object or array
			 * @param {boolean} inverted Whether the input coordinates are inverted ([lng, lat] instead of [lat, lng])
			 * @returns A list with the converted coordinates.
			 */
			convertCoordsInList(coordsList, converter, outputType = COORD_TYPES.object, inverted = false)
			{
				const coordinates = []

				if (typeof converter !== 'function')
					return coordinates

				for (let coords of coordsList ?? [])
				{
					let convertedCoord
					if (Array.isArray(coords) && typeof coords[0] !== 'number')
						convertedCoord = this.convertCoordsInList(coords, converter, outputType, inverted)
					else
						convertedCoord = converter(coords, outputType, true, inverted)
					coordinates.push(convertedCoord)
				}

				return coordinates
			},

			/**
			 * Calculates the coordinates where to initially center the map.
			 */
			getCenterCoords()
			{
				if (this.isSinglePoint && this.markers.length > 0 && !this.styleVariables.centerCoord?.value)
					return this.projectFromCoords(this.markers[0].coords, COORD_TYPES.array)
				return this.projectFromCoords(this.styleVariables.centerCoord?.value, COORD_TYPES.array)
			},

			/**
			 * Initializes the necessary map properties.
			 */
			async initMap()
			{
				if (this.map !== null)
					return

				this.map = this.$refs.map?.leafletObject

				if (!this.map)
					return

				L.Map.addInitHook('addHandler', 'gestureHandling', GestureHandling)

				// Triggers when a user selects a checkbox to make a layer visible.
				this.map.off('overlayadd').on('overlayadd', (overlay) => {
					this.activateLayer(overlay.name)

					if (this.shapes.length > 0 && this.shapes[0].layerName === overlay.name)
						this.setDrawingTool()

					if (this.exclusiveLayers.find((l) => l === overlay.name))
					{
						for (let el of this.layersList)
							if (el.name !== overlay.name && this.exclusiveLayers.find((l) => l === el.name))
								nextTick().then(() => this.map.removeLayer(el.layer))
					}
				})

				// Triggers when a user deselects a checkbox to hide a layer.
				this.map.off('overlayremove').on('overlayremove', (overlay) => {
					this.deactivateLayer(overlay.name)

					this.$emit('close-info-window')

					if (!this.isDrawableMap)
						return

					if (this.shapes.length > 0 && this.shapes[0].layerName === overlay.name)
						this.map.pm?.toggleControls()
				})

				// Add fullscreen and scale controls.
				if (!this.styleVariables.disableControls?.value)
				{
					addFullScreenMode(L)
					this.fullscreenControl = new L.control.fullscreen({ forceSeparateButton: true })
					this.scaleControl = new L.control.scale()
					this.zoomControl = this.map.zoomControl

					this.map.addControl(this.fullscreenControl)
					this.map.addControl(this.scaleControl)

					this.map.off('enterFullscreen').on('enterFullscreen', () => this.isFullscreen = true)
					this.map.off('exitFullscreen').on('exitFullscreen', () => this.isFullscreen = false)
				}

				// Add the search bar.
				this.setSearchBar()

				// Add printing option.
				this.setPrintingOption()

				// Activate all the layers with shapes by default.
				this.activateShapeLayers()

				if (this.isDrawableMap && this.isSinglePoint)
					this.activateLayer(this.texts.shapesLayer)

				this.addOverlays()
				this.setDrawTexts()

				for (let i in this.baseLayers)
					this.baseLayers[i].addTo(this.map)

				await this.resetMapProperties()

				// Fit the zoom, so all the markers/shapes become visible.
				const mapCentered = await this.fitMapZoom()
				if (!mapCentered)
					this.centerCoords = this.getCenterCoords()

				this.$emit('is-ready', this.map)
				this.isReady = true
			},

			/**
			 * Resets the properties of the map, markers and clusters.
			 */
			async resetMapProperties()
			{
				if (this.map === null || this.isResetting)
					return

				this.isResetting = true

				if (this.controlLayer !== null)
					this.map.removeControl(this.controlLayer)

				// Remove all the layers already on the map.
				for (let el of this.layersList)
					this.map.removeLayer(el.layer)

				this.layersList = []

				const collapsed = this.styleVariables.collapseLayerOptions?.value || false
				this.controlLayer = L.control.layers(this.baseMaps, this.baseLayers, { collapsed, hideSingleBase: true, sortLayers: false })
				this.map.addControl(this.controlLayer)

				// Add the base layers (just for markers).
				this.setBaseMarkerLayers()

				// Add layers from external services.
				await this.addExternalLayers()

				this.setShapeLayers()

				const layerCount = this.layersList.reduce((accum, curr) => curr.name ? accum + 1 : accum, 0) + Object.keys(this.baseLayers).length

				// Set the layers visible on the map.
				for (let el of this.layersList)
				{
					if (!el.name)
						continue

					if (this.activeLayers.find((l) => l === el.name))
						el.layer.addTo(this.map)
					if (layerCount > 1)
						this.controlLayer.addOverlay(el.layer, el.name)
				}

				this.isResetting = false
			},

			/**
			 * Builds a leaflet icon to be used in the map.
			 * @param {string} iconData The icon data/url
			 * @returns A leaflet icon with the specified icon data.
			 */
			getMarkerIcon(iconData)
			{
				return L.icon({
					iconUrl: iconData,
					iconSize: [25, 25],
					iconAnchor: [12.5, 12.5]
				})
			},

			/**
			 * Adds a marker to the map.
			 * @param {object} event The click event
			 */
			onMapClick(event)
			{
				this.closeInfoWindow()

				// Check if it was a Ctrl + Click and the map can be edited.
				if (!this.readonly && !this.isDrawableMap && event.latlng && event.originalEvent?.ctrlKey)
				{
					const eventData = {
						coords: {
							lat: event.latlng.lat,
							lng: event.latlng.lng
						}
					}

					this.$emit('set-marker', eventData)
				}
			},

			/**
			 * Changes the position of a marker.
			 * @param {object} event The drag event
			 * @param {object} marker The marker that was dragged
			 */
			onMarkerDrag(event, marker)
			{
				if (this.readonly || this.isDrawableMap)
					return

				// The new position for the marker.
				const coords = event.target.getLatLng()

				if (typeof coords.lat !== 'number' || typeof coords.lng !== 'number')
					return

				const eventData = {
					...marker,
					coords: this.projectToCoords(coords)
				}

				this.centerMap(coords, undefined, { animate: false })
				this.$emit('set-marker', eventData)
			},

			/**
			 * Opens the info window of the specified marker, or removes it from the map if the shift key was pressed.
			 * @param {object} event The click event
			 * @param {object} marker The marker that was clicked
			 */
			onMarkerClick(event, marker)
			{
				if (event.originalEvent.shiftKey)
				{
					if (!this.readonly)
						this.$emit('remove-marker', marker)
				}
				else if (event.originalEvent.ctrlKey)
				{
					if (marker.rowKey && this.followUpAction)
					{
						const actionData = {
							id: this.followUpAction,
							rowKey: marker.rowKey
						}

						this.$emit('row-action', actionData)

						// Ensures the popup closes when navigating to the follow-up form, necessary if the form is a popup.
						this.$emit('close-info-window')
					}
				}
				else
				{
					this.centerMap(marker.coords, undefined, { animate: false })
					this.executeWhenReady(() => this.$emit('open-info-window', { marker, isShape: false, shapeClicked: true }))
				}
			},

			/**
			 * Centers the map on the specified shape and opens the info window with it's information.
			 * @param {object} event The click event
			 * @param {object} shape The shape that was clicked
			 * @param {object} layerProps The properties of the containing layer (optional)
			 */
			onShapeClick(event, shape, layerProps = {})
			{
				if (this.drawModeIsOn || this.editModeIsOn)
					return

				let centerCoords

				if (typeof shape.getCenter === 'function')
					centerCoords = shape.getCenter()
				else if (typeof shape._latlng === 'object')
					centerCoords = shape._latlng

				if (typeof event.latlng !== 'undefined')
					this.centerMap(event.latlng, undefined, { animate: false })

				if (typeof layerProps === 'object')
				{
					if (event.originalEvent.ctrlKey && this.followUpAction && layerProps.rowKey)
					{
						const actionData = {
							id: this.followUpAction,
							...layerProps
						}

						this.$emit('row-action', actionData)
					}
					else
					{
						const marker = {
							coords: this.projectToCoords(centerCoords),
							...layerProps
						}
						const isShape = !(shape instanceof L.Marker)

						this.executeWhenReady(() => this.$emit('open-info-window', { marker, isShape, shapeClicked: true }))
					}
				}
			},

			/**
			 * Defines the behavior when a user hovers a shape.
			 * @param {object} shape The shape that was clicked
			 * @param {object} shapeOptions The current options of the shape
			 * @param {object} layerprops The properties of the containing layer (optional)
			 */
			onShapeMouseOver(shape, shapeOptions = {}, layerProps = {})
			{
				const isShape = !(shape instanceof L.Marker)

				if (this.styleVariables.openPopupOnHover?.value)
					this.executeWhenReady(() => this.$emit('open-info-window', { marker: layerProps, isShape, shapeClicked: false }))

				if (this.drawModeIsOn || !isShape)
					return

				if (shape.hovered)
					return

				shape.hovered = true

				const options = {
					fillOpacity: 0.5,
					opacity: 0.9,
					weight: 8
				}

				if (shapeOptions.fillOpacity)
					options.fillOpacity = shapeOptions.fillOpacity + 0.2
				if (shapeOptions.opacity)
					options.opacity = shapeOptions.opacity + 0.2
				if (shapeOptions.weight)
					options.weight = shapeOptions.weight + 2

				shape.setStyle(options)
			},

			/**
			 * Defines the behavior when a user stops hovering a shape.
			 * @param {object} shape The shape that was clicked
			 * @param {object} shapeOptions The current options of the shape
			 */
			onShapeMouseOut(shape, shapeOptions = {})
			{
				if (this.styleVariables.openPopupOnHover?.value)
					this.executeWhenReady(() => this.$emit('close-info-window'))

				shape.hovered = false

				if (this.drawModeIsOn || shape instanceof L.Marker)
					return

				shape.setStyle(shapeOptions)
			},

			/**
			 * Creates a leaflet marker, with the specified data.
			 * @param {object} markerData The marker data
			 * @returns The created leaflet marker.
			 */
			createMarker(markerData)
			{
				if (typeof markerData !== 'object')
					return undefined

				const markerIcon = imageObjToSrc(markerData.icon)
				const marker = L.marker(L.latLng(markerData.coords.lat, markerData.coords.lng), {
					draggable: !this.readonly && !this.isDrawableMap,
					...(markerIcon && {
						icon: this.getMarkerIcon(markerIcon)
					})
				})

				marker.off('click').on('click', (event) => this.onMarkerClick(event, markerData))
				marker.off('dragend').on('dragend', (event) => this.onMarkerDrag(event, markerData))

				return marker
			},

			/**
			 * Centers the map on the specified coordinates.
			 * @param {object} coords The coordinates
			 * @param {number} zoomLevel The zoom level (optional)
			 * @param {object} options Additional options (optional)
			 */
			centerMap(coords, zoomLevel, options)
			{
				if (typeof coords !== 'object' || !coords.lat || !coords.lng)
					coords = { lat: 0, lng: 0 }
				if (typeof zoomLevel !== 'number' || zoomLevel < 0)
					zoomLevel = undefined
				if (typeof options !== 'object')
					options = {}

				this.isMoving = true

				const projectedCoords = this.projectFromCoords(coords, COORD_TYPES.array, false)
				nextTick().then(() => this.map?.flyTo(projectedCoords, zoomLevel, options))
			},

			/**
			 * Centers the map on the location specified in the event.
			 * @param {object} event The event
			 */
			setPlace(event)
			{
				const coords = {
					lat: event.location.y,
					lng: event.location.x
				}

				this.centerMap(coords, 12)
			},

			/**
			 * Gets all the sub-layers inside the specified layer.
			 * @param {object} layer The parent layer
			 * @returns A list with the sub-layers.
			 */
			getSubLayers(layer)
			{
				const subLayers = []

				if (typeof layer !== 'object' || typeof layer.getLayers !== 'function')
					return subLayers

				for (let subLayer of layer.getLayers())
				{
					// Having layers inside means it's not a shape, but a group of shapes.
					if (typeof subLayer._layers !== 'undefined')
						subLayers.push(...this.getSubLayers(subLayer))
					else
						subLayers.push(subLayer)
				}

				return subLayers
			},

			/**
			 * Adjusts the center and zoom of the map to fit the available markers/shapes.
			 * @param {object} layers The layers over which to fit the map
			 */
			async fitMapZoom(layers)
			{
				if (!Array.isArray(layers))
				{
					if (!this.styleVariables.fitZoom?.value || this.markers.length === 0 && this.shapes.length === 0)
						return false

					layers = []

					for (let layer of this.layersList)
						if (this.activeLayers.find((l) => l === layer.name))
							layers.push(...this.getSubLayers(layer.layer))
				}

				if (layers.length === 0)
					return false

				const group = L.featureGroup(layers)

				await nextTick()
				this.map?.fitBounds(group.getBounds().pad(0.1))

				if (typeof this.map?.getCenter === 'function')
					this.centerCoords = this.projectFromCoords(this.map.getCenter(), COORD_TYPES.array, false)

				return true
			},

			/**
			 * Activates all the layers with shapes.
			 */
			activateShapeLayers()
			{
				for (let layer of this.shapes)
					this.activateLayer(layer.layerName)
			},

			/**
			 * Adds the specified layer to the list of active layers.
			 * @param {string} layerName The name of the layer
			 */
			activateLayer(layerName)
			{
				if (!this.activeLayers.find((l) => l === layerName))
					this.activeLayers.push(layerName)
			},

			/**
			 * Removes the specified layer from the list of active layers.
			 * @param {number} layerName The name of the layer
			 */
			deactivateLayer(layerName)
			{
				const layerIndex = this.activeLayers.indexOf(layerName)
				if (layerIndex !== -1)
					this.activeLayers.splice(layerIndex, 1)
			},

			/**
			 * Adds the overlays to the map.
			 */
			addOverlays()
			{
				this.baseOverlays.forEach((overlay) => {
					const overlayObj = L.tileLayer(overlay.url, {
						id: overlay.name,
						attribution: overlay.attribution,
						...(overlay.subdomains ? { subdomains: overlay.subdomains } : {})
					})

					if (overlay.name === this.defaultOverlay)
						overlayObj.addTo(this.map)
					this.baseMaps[overlay.name] = overlayObj
				})

				if (Array.isArray(this.overlays))
				{
					this.overlays.forEach((overlay) => {
						if (!overlay.name)
							return

						const properties = {
							id: overlay.name,
							attribution: overlay.attribution,
							...(overlay.subdomains ? { subdomains: overlay.subdomains } : {}),
							...(overlay.properties ?? {})
						}

						const layer = overlay.type === 'WMS' ? L.tileLayer.wms(overlay.url, properties) : L.tileLayer(overlay.url, properties)
						if (overlay.transparent === true)
							this.baseLayers[overlay.name] = layer
						else
							this.baseMaps[overlay.name] = layer
					})
				}
			},

			/**
			 * Sets the base layers (with just markers).
			 */
			setBaseMarkerLayers()
			{
				if (this.defaultMarkers !== null)
				{
					const isActive = this.activeLayers.find((l) => l === this.texts.defaultLayer)
					this.map.removeControl(this.defaultMarkers)
					if (isActive)
						this.activateLayer(this.texts.defaultLayer)
				}

				if (this.markersCluster !== null)
				{
					const isActive = this.activeLayers.find((l) => l === this.texts.clusterGroupLayer)
					this.map.removeControl(this.markersCluster)
					if (isActive)
						this.activateLayer(this.texts.clusterGroupLayer)
				}

				const markerCount = this.markers.length

				if (markerCount > 0)
				{
					this.defaultMarkers = L.layerGroup()
					if (markerCount > 1)
						this.markersCluster = L.markerClusterGroup()

					this.markers.forEach((m) => {
						const defaultMarker = this.createMarker(m)
						this.defaultMarkers.addLayer(defaultMarker)

						if (markerCount > 1)
						{
							const clusterMarker = this.createMarker(m)
							this.markersCluster.addLayer(clusterMarker)
						}
					})

					// Add default layer.
					const markerLayer = {
						name: this.texts.defaultLayer,
						layer: this.defaultMarkers
					}
					this.layersList.push(markerLayer)

					if (markerCount > 1)
					{
						// Add cluster layer.
						const clusterLayer = {
							name: this.texts.clusterGroupLayer,
							layer: this.markersCluster
						}
						this.layersList.push(clusterLayer)
					}

					if (this.groupMarkersInCluster && markerCount > 1)
						this.activateLayer(this.texts.clusterGroupLayer)
					else
						this.activateLayer(this.texts.defaultLayer)
				}
			},

			/**
			 * Adds the external layers to the map.
			 */
			addExternalLayers()
			{
				if (!Array.isArray(this.externalLayers) || this.externalLayers.length === 0)
					return

				let layerCount = 1

				return Promise.resolve(import('esri-leaflet').then((esri) => {
					const setCustomColor = (customColor, feature) => customColor || this.shapeColors[feature.geometry?.type?.toLowerCase()] || '#3388ff'

					this.externalLayers.forEach((layerData) => {
						const layer = esri.featureLayer({
							url: layerData.url,
							minZoom: layerData.minZoom ?? 0,
							requestParams: layerData.requestData ?? {},
							fetchAllFeatures: true,
							style: (feature) => {
								let customColor

								if (typeof layerData.getColor === 'function')
									customColor = layerData.getColor(feature)

								return {
									weight: this.styleVariables.shapeOutlineWeight?.value ?? 7,
									color: setCustomColor(customColor, feature)
								}
							}
						})

						this.layersList.push({
							name: layerData.layerName || this.texts.externalLayer + (this.externalLayers.length > 1 ? ` ${layerCount}` : ''),
							layer
						})

						layer.addTo(this.map)
						layerCount++

						layer.off('click').on('click', (event) => {
							const shape = event.layer
							const feature = shape.feature
							const shapeProps = {
								properties: feature?.properties ?? {},
								layerOptions: layer.service?.options ?? {},
								event: event.originalEvent
							}

							this.onShapeClick(event, shape, shapeProps)

							this.executeWhenReady(() => {
								if (typeof layerData.getColor !== 'function')
									return

								layer.setStyle((feature) => ({ color: setCustomColor(layerData.getColor(feature), feature) }))
							})
						})

						let shapeOptions

						layer.off('mouseover').on('mouseover', (event) => {
							const shape = event.layer
							const feature = shape.feature
							const shapeProps = {
								properties: feature?.properties ?? {},
								layerOptions: layer.service?.options ?? {},
								event: event.originalEvent
							}

							shapeOptions = cloneDeep(shape.options)
							this.onShapeMouseOver(shape, shapeOptions, shapeProps)
						})

						layer.off('mouseout').on('mouseout', (event) => {
							const shape = event.layer

							this.onShapeMouseOut(shape, shapeOptions)
						})

						layer.off('removefeature').on('removefeature', _debounce(() => this.$emit('close-info-window'), 400))
					})
				}))
			},

			/**
			 * Adds the search bar to the map.
			 */
			setSearchBar()
			{
				if (this.styleVariables.disableSearch?.value || this.map === null)
					return

				if (this.searchControl !== null)
					this.map.removeControl(this.searchControl)

				this.searchControl = new GeoSearchControl({
					provider: new OpenStreetMapProvider(),
					searchLabel: this.texts.search,
					position: 'topleft',
					retainZoomLevel: true,
					showMarker: false,
					keepResult: false
				})

				this.map.addControl(this.searchControl)
				this.map.off('geosearch/showlocation').on('geosearch/showlocation', this.setPlace)
			},

			/**
			 * Adds the printing option to the map.
			 */
			setPrintingOption()
			{
				if (!this.styleVariables.allowExporting?.value || this.map === null || this.printControl !== null)
					return

				this.printControl = {
					name: 'printMap',
					block: 'custom',
					title: computed(() => this.texts.printMap),
					className: 'leaflet-map-print',
					actions: [
						{
							text: computed(() => this.texts.printLandscape),
							onClick: () => this.$emit('export-map', false)
						},
						{
							text: computed(() => this.texts.printPortrait),
							onClick: () => this.$emit('export-map', true)
						},
						{
							text: computed(() => this.texts.cancel),
							onClick: () => this.map.pm.Toolbar.buttons.printMap.toggle()
						}
					]
				}

				this.map.pm?.Toolbar.createCustomControl(this.printControl)
			},

			/**
			 * Makes all the controls in the map hidden.
			 * @param {array} mainControls A list with the main controls of the map
			 * @param {boolean} hideEditControls Whether to hide the edit controls
			 */
			hideControls(mainControls = [this.controlLayer, this.zoomControl, this.fullscreenControl, this.searchControl], hideEditControls = true)
			{
				for (let control of mainControls)
					if (control !== null)
						this.map.removeControl(control)

				if (hideEditControls && this.map.pm?.controlsVisible())
					this.map.pm.toggleControls()
			},

			/**
			 * Makes all the controls in the map visible.
			 * @param {array} mainControls A list with the main controls of the map
			 * @param {boolean} showEditControls Whether to show the edit controls
			 */
			showControls(mainControls = [this.controlLayer, this.zoomControl, this.fullscreenControl, this.searchControl], showEditControls = true)
			{
				for (let control of mainControls)
					if (control !== null)
						this.map.addControl(control)

				if (showEditControls && this.map.pm && !this.map.pm.controlsVisible())
					this.map.pm.toggleControls()
			},

			/**
			 * Sets the necessary options for the specified layer.
			 * @param {object} layer The layer
			 * @param {object} data The layer data
			 */
			setLayerOptions(layer, data)
			{
				layer.options = { ...data }
			},

			/**
			 * Adds the specified layer to the map.
			 * @param {object} layerData The data of the layer
			 * @param {boolean} canEditShapes Whether or not it's an editable layer
			 */
			addLayerToMap(layerData, canEditShapes = false)
			{
				if (!Array.isArray(layerData.shapes))
					return

				let layerObj, exists = false

				if (canEditShapes)
					layerObj = this.shapesLayer
				else
				{
					for (let layer of this.layersList)
					{
						// If there's already a layer with the same name, we add the shapes to that layer instead of creating a new one.
						if (layer.name === layerData.layerName)
						{
							let layerGroup = L.layerGroup(layerData.shapes)
							this.setLayerOptions(layerGroup, layerData)

							layer.layer.addLayer(layerGroup)
							exists = true
							break
						}
					}
				}

				if (!exists)
				{
					if (typeof layerObj === 'undefined')
					{
						let layerGroup = L.layerGroup(layerData.shapes)
						this.setLayerOptions(layerGroup, layerData)

						layerObj = L.featureGroup()
						layerObj.addLayer(layerGroup)
					}

					if (!layerObj.name)
						layerObj.name = layerData.layerName || this.texts.shapesLayer

					const shapesLayer = {
						name: layerData.layerName,
						layer: layerObj
					}

					this.layersList.push(shapesLayer)
				}
			},

			/**
			 * Sets the layers with shapes.
			 */
			setShapeLayers()
			{
				const canEditShapes = !this.readonly && this.isDrawableMap
				const shapeLayersList = this.getDrawnShapes()

				if (canEditShapes)
				{
					const layerExists = this.shapesLayer !== null

					this.shapesLayer = L.featureGroup()
					if (!layerExists)
						this.map.addLayer(this.shapesLayer)

					const shapesList = shapeLayersList.length > 0 ? shapeLayersList[0].shapes : []
					const layerOptions = {
						rowKey: shapeLayersList.length > 0 ? shapeLayersList[0].rowKey : undefined,
						description: shapeLayersList.length > 0 ? shapeLayersList[0].description : [],
						btnPermission: shapeLayersList.length > 0 ? shapeLayersList[0].btnPermission : {},
						followup: shapeLayersList.length > 0 ? shapeLayersList[0].followup : {}
					}

					// Add the color options for the layer.
					if (shapeLayersList.length > 0 && shapeLayersList[0].polygonColor)
						layerOptions.polygonColor = shapeLayersList[0].polygonColor
					if (shapeLayersList.length > 0 && shapeLayersList[0].polylineColor)
						layerOptions.polylineColor = shapeLayersList[0].polylineColor
					if (shapeLayersList.length > 0 && shapeLayersList[0].circleColor)
						layerOptions.circleColor = shapeLayersList[0].circleColor

					this.setLayerOptions(this.shapesLayer, layerOptions)

					// Add the shapes already on the map to the shapes layer.
					for (let shape of shapesList)
						this.shapesLayer.addLayer(shape)
				}

				// Add drawing and other custom features.
				this.setDrawingTool()

				// Draw the shapes/polygons.
				for (let i = 0; i < shapeLayersList.length; i++)
				{
					const layerData = shapeLayersList[i]
					this.addLayerToMap(layerData, canEditShapes && i === 0)
				}

				// Ensure the editable shapes layer has a name.
				if (this.shapesLayer !== null && !this.shapesLayer.name)
					this.shapesLayer.name = this.texts.shapesLayer

				this.setDrawingEvents()
			},

			/**
			 * Sets the texts to be used by the drawing tool.
			 */
			setDrawTexts()
			{
				if (!this.isDrawableMap)
					return

				const translations = {
					tooltips: {
						placeMarker: this.texts.placeMarker,
						firstVertex: this.texts.startShapeDraw,
						continueLine: this.texts.continueShapeDraw,
						finishLine: this.texts.endLineDraw,
						finishPoly: this.texts.endShapeDraw,
						finishRect: this.texts.endDrawing,
						startCircle: this.texts.startCircleDraw,
						finishCircle: this.texts.endCircleDraw,
						placeCircleMarker: this.texts.placeCircleMarker,
						placeText: this.texts.placeText
					},
					actions: {
						finish: this.texts.finish,
						cancel: this.texts.cancel,
						removeLastVertex: this.texts.deleteLastPoint
					},
					buttonTitles: {
						drawMarkerButton: this.texts.drawMarker,
						drawPolyButton: this.texts.drawPolygon,
						drawLineButton: this.texts.drawPolyline,
						drawCircleButton: this.texts.drawCircle,
						drawCircleMarkerButton: this.texts.drawCircleMarker,
						drawRectButton: this.texts.drawRectangle,
						editButton: this.texts.editLayers,
						dragButton: this.texts.dragLayers,
						cutButton: this.texts.cutLayers,
						deleteButton: this.texts.deleteLayers,
						snappingButton: this.texts.snapVertices,
						pinningButton: this.texts.pinVertices,
						rotateButton: this.texts.rotateLayers,
						drawTextButton: this.texts.drawText,
						scaleButton: this.texts.scaleLayers,
						autoTracingButton: this.texts.autoTrace
					},
					measurements: {
						totalLength: this.texts.length,
						segmentLength: this.texts.segmentLength,
						area: this.texts.area,
						radius: this.texts.radius,
						perimeter: this.texts.perimeter,
						height: this.texts.height,
						width: this.texts.width,
						coordinates: this.texts.position,
						coordinatesMarker: this.texts.positionMarker
					}
				}

				this.map.pm.setLang('currentLang', translations)
			},

			/**
			 * Sets the options of the specified shape.
			 * @param {object} shape The shape
			 * @param {object} options Extra options that can overwrite the default ones
			 */
			setShapeOptions(shape, options = {})
			{
				for (let i in options)
					shape.options[i] = options[i]

				if (!options.opacity && !(shape instanceof L.Marker))
					shape.options.opacity = 0.7
				if (!options.weight)
					shape.options.weight = this.styleVariables.shapeOutlineWeight?.value ?? 7

				if (!options.color)
				{
					let customColor

					if (typeof options.getColor === 'function')
						customColor = options.getColor(shape)

					if (shape instanceof L.Rectangle || shape instanceof L.Polygon)
						shape.options.color = customColor || options.polygonColor || this.shapeColors.polygon
					else if (shape instanceof L.Polyline)
						shape.options.color = customColor || options.polylineColor || this.shapeColors.polyline
					else if (shape instanceof L.Circle)
						shape.options.color = customColor || options.circleColor || this.shapeColors.circle
				}
			},

			/**
			 * Gets the styles to apply to a shape.
			 * @param {string} shapeColor The color of the shape
			 * @returns An object with the style that should be applied to the shape.
			 */
			getShapeStyle(shapeColor)
			{
				return {
					allowSelfIntersection: false,
					hintlineStyle: {
						color: shapeColor,
						opacity: 0.7,
						dashArray: [5, 5]
					},
					templineStyle: {
						color: shapeColor,
						opacity: 0.7,
						weight: this.styleVariables.shapeOutlineWeight?.value ?? 7
					}
				}
			},

			/**
			 * Sets up the drawing tool.
			 */
			setDrawingTool()
			{
				const pm = this.map?.pm

				// Removes any controls already active.
				if (pm?.controlsVisible())
					pm.toggleControls()

				if (!pm)
					return

				if (this.canDraw)
				{
					const polylineColor = this.shapesLayer.options.polylineColor || this.shapeColors.polyline
					const polygonColor = this.shapesLayer.options.polygonColor || this.shapeColors.polygon
					const circleColor = this.shapesLayer.options.circleColor || this.shapeColors.circle

					const polylineStyle = this.getShapeStyle(polylineColor)
					const polygonStyle = this.getShapeStyle(polygonColor)
					const circleStyle = this.getShapeStyle(circleColor)

					pm.Draw.Line.setOptions(polylineStyle)
					pm.Draw.Polygon.setOptions(polygonStyle)
					pm.Draw.Rectangle.setOptions(polygonStyle)
					pm.Draw.Circle.setOptions(circleStyle)

					pm.setGlobalOptions({
						layerGroup: this.shapesLayer
					})
				}

				pm.addControls({
					position: 'topleft',
					drawMarker: this.canDraw && (this.styleVariables.allowMarkers ? this.styleVariables.allowMarkers.value : true),
					drawPolyline: this.canDraw && (this.styleVariables.allowPolylines ? this.styleVariables.allowPolylines.value : true),
					drawPolygon: this.canDraw && (this.styleVariables.allowPolygons ? this.styleVariables.allowPolygons.value : true),
					drawRectangle: this.canDraw && (this.styleVariables.allowPolygons ? this.styleVariables.allowPolygons.value : true),
					drawCircle: false,
					drawCircleMarker: false,
					drawText: false,
					editMode: this.canDraw && (this.styleVariables.allowEdit ? this.styleVariables.allowEdit.value : true),
					dragMode: this.canDraw && (this.styleVariables.allowDrag ? this.styleVariables.allowDrag.value : true),
					cutPolygon: this.canDraw && (this.styleVariables.allowCutting ? this.styleVariables.allowCutting.value : true),
					removalMode: this.canDraw && (this.styleVariables.allowRemoval ? this.styleVariables.allowRemoval.value : true),
					rotateMode: this.canDraw && (this.styleVariables.allowRotate ? this.styleVariables.allowRotate.value : true),
					// Custom features.
					printMap: this.styleVariables.allowExporting ? this.styleVariables.allowExporting.value : true
				})
			},

			/**
			 * Sets the callback for the specified shape edit event.
			 * @param {string} eventName The name of the event
			 * @param {function} callback A callback function to be executed when the event triggers
			 */
			setShapeEditEvent(eventName, callback)
			{
				if (this.readonly || !this.isDrawableMap)
					return

				this.map.off(eventName).on(eventName, (event) => {
					this.$emit('close-info-window')

					if (typeof callback !== 'function')
						return

					const isActive = callback(event)

					if (isActive)
						return

					const shapesList = {
						rowKey: this.shapesLayer.options?.rowKey,
						description: this.shapesLayer.options?.description,
						btnPermission: this.shapesLayer.options?.btnPermission,
						layerName: this.shapesLayer.name,
						shapes: this.convertShapesToObjects()
					}

					this.$emit('set-shapes', shapesList)
				})
			},

			/**
			 * Sets the events related to the shapes/polygons.
			 */
			setDrawingEvents()
			{
				if (!this.readonly && this.isDrawableMap)
				{
					this.setShapeEditEvent('pm:globaldrawmodetoggled', (event) => {
						const isEnabled = event.enabled

						if (this.styleVariables.groupMarkersInCluster?.value)
						{
							if (isEnabled)
								this.groupMarkersInCluster = false
							else
								this.groupMarkersInCluster = this.styleVariables.groupMarkersInCluster?.value

							this.resetMapProperties()
						}

						return this.drawModeIsOn = isEnabled
					})

					this.setShapeEditEvent('pm:globaleditmodetoggled', (event) => {
						const isEnabled = event.enabled

						if (this.styleVariables.groupMarkersInCluster?.value)
						{
							if (isEnabled)
								this.groupMarkersInCluster = false
							else
								this.groupMarkersInCluster = this.styleVariables.groupMarkersInCluster?.value

							this.resetMapProperties()
						}

						return this.drawModeIsOn = isEnabled
					})

					this.setShapeEditEvent('pm:globaldragmodetoggled', (event) => {
						const isEnabled = event.enabled

						if (this.styleVariables.groupMarkersInCluster?.value)
						{
							if (isEnabled)
								this.groupMarkersInCluster = false
							else
								this.groupMarkersInCluster = this.styleVariables.groupMarkersInCluster?.value

							this.resetMapProperties()
						}

						return this.editModeIsOn = isEnabled
					})

					this.setShapeEditEvent('pm:globalcutmodetoggled', (event) => {
						return this.drawModeIsOn = event.enabled
					})

					this.setShapeEditEvent('pm:globalremovalmodetoggled', (event) => {
						return this.editModeIsOn = event.enabled
					})

					this.setShapeEditEvent('pm:globalrotatemodetoggled', (event) => {
						return this.editModeIsOn = event.enabled
					})

					this.setShapeEditEvent('pm:remove', (event) => {
						// This code is only necessary because there's a bug in the Geoman library.
						// If markers are inside a cluster group, they won't be removed without this.
						if (event.layer instanceof L.Marker && this.shapeMarkersCluster !== null)
						{
							this.shapesLayer.removeLayer(this.shapeMarkersCluster)
							this.shapeMarkersCluster.removeLayer(event.layer)
							this.shapesLayer.addLayer(this.shapeMarkersCluster)
						}

						return true
					})
				}

				for (let layer of this.layersList)
				{
					if (typeof layer.name !== 'string' || typeof layer.layer !== 'object')
						continue

					// Ignore the default layers (they won't have shapes).
					if (this.exclusiveLayers.find((l) => l === layer.name))
						continue

					this.setLayerEvents(layer.name, layer.layer)
				}
			},

			/**
			 * Sets the events of the specified layer.
			 * @param {string} layerName The layer name
			 * @param {object} layer The layer
			 * @param {object} otherProps Other properties of the layer
			 */
			setLayerEvents(layerName, layer, otherProps = {})
			{
				if (typeof layerName !== 'string' || typeof layer !== 'object')
					return

				const shapes = layer._layers

				for (let i in shapes)
				{
					const shape = shapes[i]

					// Having layers inside means it's not a shape, but a group of shapes.
					if (typeof shape._layers !== 'undefined')
						this.setLayerEvents(layerName, shape, shape.options)
					else
					{
						let layers = [shape]
						if (shape instanceof L.MarkerClusterGroup)
							layers = shape.getLayers()

						for (let l of layers)
						{
							const layerProps = {
								...layer.options,
								...(otherProps ?? {}),
								layerName
							}

							this.setShapeEvents(l, layerProps)
						}
					}
				}
			},

			/**
			 * Sets the events of the specified shape.
			 * @param {object} shape The shape
			 * @param {object} layerProps The properties of the layer where the shape is
			 */
			setShapeEvents(shape, layerProps)
			{
				if (typeof shape !== 'object')
					return

				const shapeOptions = cloneDeep(shape.options)

				shape.off('click').on('click', (event) => this.onShapeClick(event, shape, layerProps))
				shape.off('mouseover').on('mouseover', () => this.onShapeMouseOver(shape, shapeOptions, layerProps))
				shape.off('mouseout').on('mouseout', () => this.onShapeMouseOut(shape, shapeOptions, layerProps))
			},

			/**
			 * Transforms the specified shape into a Leaflet object type.
			 * @param {object} shape The shape
			 * @param {object} options The options of the shape
			 * @returns An object with the shape in a format that can be used by a Leaflet map.
			 */
			getLeafletShape(shape, options = {})
			{
				if (typeof shape !== 'object')
					return

				let shapeToDraw = null

				switch (shape.type)
				{
					case SHAPE_TYPES.polyline:
						if ('latlngs' in shape)
						{
							delete options.latlngs
							shapeToDraw = L.polyline(this.convertCoordsInList(shape.latlngs, this.projectFromCoords, COORD_TYPES.array), options)
						}
						break
					case SHAPE_TYPES.polygon:
						if ('shapeParts' in shape)
						{
							delete options.latlngs
							delete options.shapeParts
							shapeToDraw = L.polygon(this.convertCoordsInList(shape.shapeParts, this.projectFromCoords, COORD_TYPES.array), options)
						}
						break
					case SHAPE_TYPES.rectangle:
						if ('shapeParts' in shape)
						{
							delete options.latlngs
							delete options.shapeParts
							shapeToDraw = L.rectangle(this.convertCoordsInList(shape.shapeParts, this.projectFromCoords, COORD_TYPES.array), options)
						}
						else if ('bounds' in shape)
						{
							delete options.bounds
							shapeToDraw = L.rectangle(this.convertCoordsInList(shape.bounds, this.projectFromCoords, COORD_TYPES.array), options)
						}
						break
					case SHAPE_TYPES.circle:
						if ('center' in shape && 'radius' in shape)
						{
							delete options.center
							shapeToDraw = L.circle(this.projectFromCoords(shape.center, COORD_TYPES.array), options)
						}
						break
					case SHAPE_TYPES.circlemarker:
					case SHAPE_TYPES.marker:
						if ('latlng' in shape)
						{
							delete options.latlng
							delete options.opacity
							delete options.weight
							let shapeCoord = this.projectFromCoords(shape.latlng, COORD_TYPES.array)
							shapeToDraw = shape.type === 'circlemarker' ? L.circleMarker(shapeCoord, options) : L.marker(shapeCoord, options)
						}
						break
				}

				return shapeToDraw
			},

			/**
			 * Creates a list with the shape objects already on the map.
			 * @returns The list of shape objects.
			 */
			getDrawnShapes()
			{
				const shapesLayers = []

				for (let layer of this.shapes)
				{
					if (!Array.isArray(layer.shapes))
						break

					let shapesList = [],
						markers = [],
						colorOptions = {}

					if (layer.polygonColor)
						colorOptions.polygonColor = layer.polygonColor
					if (layer.polylineColor)
						colorOptions.polylineColor = layer.polylineColor
					if (layer.circleColor)
						colorOptions.circleColor = layer.circleColor

					for (let shape of layer.shapes)
					{
						let options = Object.assign({}, shape)
						delete options.type
						const shapeToDraw = this.getLeafletShape(shape, options)
						const shapeIcon = imageObjToSrc(layer.icon)

						if (shapeIcon)
							options.icon = this.getMarkerIcon(shapeIcon)

						if (typeof layer.getColor === 'function')
							options.getColor = layer.getColor

						// Add the color options.
						options = {
							...options,
							...colorOptions
						}

						this.setShapeOptions(shapeToDraw, options)

						if (this.groupMarkersInCluster && (shapeToDraw instanceof L.Marker || shapeToDraw instanceof L.CircleMarker))
							markers.push(shapeToDraw)

						// If markers should be clustered, doesn't add them since they will be added as a cluster group.
						if (shapeToDraw !== null && (!this.isDrawableMap || !(shapeToDraw instanceof L.Marker) || !this.groupMarkersInCluster))
							shapesList.push(shapeToDraw)
					}

					// Groups all the markers in a cluster.
					if (markers.length > 0)
					{
						const shapeMarkersCluster = L.markerClusterGroup()
						shapeMarkersCluster.addLayers(markers)
						shapesList.push(shapeMarkersCluster)

						if (this.isDrawableMap)
							this.shapeMarkersCluster = shapeMarkersCluster
					}

					const newLayer = {
						...colorOptions,
						rowKey: layer.rowKey,
						layerName: layer.layerName,
						description: layer.description,
						btnPermission: layer.btnPermission,
						followup: layer.followup,
						shapes: shapesList
					}
					shapesLayers.push(newLayer)
				}

				return shapesLayers
			},

			/**
			 * Converts the leaflet shapes in the editable layer to their expected object format.
			 * @returns A list with the converted shapes.
			 */
			convertShapesToObjects()
			{
				let shapesList = []

				for (let i in this.shapesLayer._layers)
				{
					const shapes = this.shapesLayer._layers[i]

					if (typeof shapes !== 'object' || typeof shapes.toGeoJSON !== 'function')
						return

					let parts = {
						[i]: shapes
					}

					// When using the "Cut" feature, a cut polyline will be separated into several parts.
					if (typeof shapes._layers === 'object')
						parts = shapes._layers
					// Markers can be grouped in a cluster, if so, they need to be taken out of it.
					else if (shapes instanceof L.MarkerClusterGroup)
						parts = shapes.getLayers()

					for (let j in parts)
					{
						const shape = parts[j]

						if (typeof shape !== 'object')
							return

						let shapeObj

						// Because of inheritance, the order must be: Rectangle > Polygon > Polyline.
						if (shape instanceof L.Rectangle)
						{
							const latLngs = shape.toGeoJSON().geometry.coordinates

							shapeObj = {
								type: SHAPE_TYPES.rectangle,
								shapeParts: this.convertCoordsInList(latLngs, this.projectToCoords, COORD_TYPES.object, true)
							}
						}
						else if (shape instanceof L.Polygon)
						{
							const latLngs = shape.toGeoJSON().geometry.coordinates

							shapeObj = {
								type: SHAPE_TYPES.polygon,
								shapeParts: this.convertCoordsInList(latLngs, this.projectToCoords, COORD_TYPES.object, true)
							}
						}
						else if (shape instanceof L.Polyline)
						{
							shapeObj = {
								type: SHAPE_TYPES.polyline,
								latlngs: this.convertCoordsInList(shape.getLatLngs(), this.projectToCoords)
							}
						}
						// Because of inheritance, the order must be: Circle > CircleMarker.
						else if (shape instanceof L.Circle)
						{
							shapeObj = {
								type: SHAPE_TYPES.circle,
								center: this.projectToCoords(shape.getCenter()),
								radius: shape.getRadius()
							}
						}
						else if (shape instanceof L.CircleMarker)
						{
							shapeObj = {
								type: SHAPE_TYPES.circlemarker,
								latlng: this.projectToCoords(shape.getLatLng())
							}
						}
						// The marker type is independent from the others.
						else if (shape instanceof L.Marker)
						{
							shapeObj = {
								type: SHAPE_TYPES.marker,
								latlng: this.projectToCoords(shape.getLatLng())
							}
						}

						if (typeof shapeObj !== 'undefined')
							shapesList.push(shapeObj)
					}
				}

				return shapesList
			},

			/**
			 * Executes the specified function, if the map is currently resetting waits for it to become ready.
			 * @param {function} execFunction The function to be executed
			 */
			executeWhenReady(execFunction)
			{
				setTimeout(() => {
					if (this.isResetting)
					{
						const stopWatch = watch(() => this.isResetting, () => {
							stopWatch()
							setTimeout(execFunction, 100)
						})
					}
					else
						execFunction()
				}, 100)
			}
		},

		watch: {
			readonly()
			{
				if (this.isDrawableMap)
				{
					if (this.shapesLayer === null)
						this.setShapeLayers()
					else
						this.setDrawingTool()
				}
			},

			hideMapControls(val)
			{
				if (val)
					this.hideControls()
				else
					this.showControls()
			},

			async shapes(newVal, oldVal)
			{
				if (newVal.length > 0 || oldVal.length > 0)
					await this.resetMapProperties()

				// Sometimes, the map may load before the shapes, if that happens we ensure the shapes become visible when they load.
				if (newVal.length !== 0 && oldVal.length === 0)
				{
					this.activateShapeLayers()

					for (let el of this.layersList)
						if (this.activeLayers.find((l) => l === el.name))
							el.layer.addTo(this.map)
				}

				if (newVal.length > 0 || oldVal.length > 0)
					this.fitMapZoom()
			},

			externalLayers(newVal, oldVal)
			{
				if (newVal.length > 0 || oldVal.length > 0)
					this.resetMapProperties()
			},

			markers: {
				handler(newVal, oldVal)
				{
					if (newVal.length > 0 || oldVal.length > 0)
						this.resetMapProperties()
				},
				deep: true
			},

			texts: {
				handler()
				{
					this.setSearchBar()
					this.setDrawTexts()
					this.setDrawingTool()
				},
				deep: true
			},

			'styleVariables.allowMarkers.value'()
			{
				this.setDrawingTool()
			},

			'styleVariables.allowPolylines.value'()
			{
				this.setDrawingTool()
			},

			'styleVariables.allowPolygons.value'()
			{
				this.setDrawingTool()
			},

			'styleVariables.allowEdit.value'()
			{
				this.setDrawingTool()
			},

			'styleVariables.allowDrag.value'()
			{
				this.setDrawingTool()
			},

			'styleVariables.allowCutting.value'()
			{
				this.setDrawingTool()
			},

			'styleVariables.allowRemoval.value'()
			{
				this.setDrawingTool()
			},

			'styleVariables.allowRotate.value'()
			{
				this.setDrawingTool()
			},

			'styleVariables.allowExporting.value'()
			{
				this.setDrawingTool()
			},

			'styleVariables.shapeOutlineWeight.value'()
			{
				this.resetMapProperties()
			},

			'styleVariables.polylineColor.value'()
			{
				this.resetMapProperties()
			},

			'styleVariables.polygonColor.value'()
			{
				this.resetMapProperties()
			},

			'styleVariables.circleColor.value'()
			{
				this.resetMapProperties()
			},

			'styleVariables.crs.value'()
			{
				this.resetMapProperties()
			},

			'styleVariables.groupMarkersInCluster.value'(val)
			{
				this.groupMarkersInCluster = val
				this.resetMapProperties()
			},

			'styleVariables.zoomLevel.value'(zoomLevel)
			{
				if (!isNaN(zoomLevel) && (this.isReady || !this.styleVariables.fitZoom?.value))
					this.executeWhenReady(() => this.updateZoom(zoomLevel))
			},

			'styleVariables.centerCoord': {
				handler(centerCoord)
				{
					if (typeof centerCoord?.value !== 'object' || (!this.isReady && this.styleVariables.fitZoom?.value))
						return

					const features = []

					switch (centerCoord.dataType)
					{
						case 'Geographic':
							features.push(L.marker(L.latLng(centerCoord.value.lat, centerCoord.value.lng)))
							break
						case 'GeographicShape':
						case 'GeometricShape':
							for (let shape of centerCoord.value?.shapes ?? [])
								features.push(this.getLeafletShape(shape))
							break
					}

					if (features.length > 0)
						this.executeWhenReady(() => this.fitMapZoom(features))
				},
				deep: true,
				immediate: true
			}
		}
	}
</script>
