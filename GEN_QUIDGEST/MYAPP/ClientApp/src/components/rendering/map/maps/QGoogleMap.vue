<template>
	<!--
	<g-map-autocomplete v-if="!styleVariables.disableSearch && !styleVariables.disableSearch.value"
		placeholder="Search"
		@place_changed="setPlace" />
	-->

	<div class="map-area-container">
		<g-map
			v-if="hasValidApiKey"
			ref="gmap"
			class="gmaps-container"
			map-type-id="roadmap"
			:api-key="apiKey"
			:center="centerCoords"
			:zoom="zoom"
			@click="mapClick">
			<!--
			<g-map-cluster zoom-on-click>
				<g-map-marker
					v-for="marker in markers"
					:key="marker.row.Fields.PrimaryKey"
					:position="marker.position"
					clickable
					:draggable="!readonly"
					:icon="marker.icon"
					@click="markerClick($event, marker.row.Fields.PrimaryKey)"
					@dragend="onDragEnd(marker.row.Fields.PrimaryKey, $event.latLng, false, marker)">
					<g-map-info-window
						v-if="!isSinglePoint"
						closeclick
						@closeclick="openMarker(null)">
						<component
							v-if="false"
							:is="infoWindow"
							:row="marker.row"
							:address="marker.address"
							@row-action="emitAction" />
					</g-map-info-window>
				</g-map-marker>
			</g-map-cluster>
			-->
		</g-map>
	</div>
</template>

<script>
	import { GoogleMap as GMap/*, Marker as GMapMarker, InfoWindow as GMapInfoWindow, MarkerCluster as GMapCluster, Autocomplete as GMapAutocomplete*/ } from 'vue3-google-map'

	export default {
		name: 'QGoogleMap',

		emits: [
			'is-ready',
			'row-action',
			'set-marker',
			'remove-marker',
			'set-shapes',
			'changed-center',
			'changed-zoom',
			'open-info-window',
			'close-info-window'
		],

		components: {
			GMap,
			/*GMapMarker,
			GMapInfoWindow,
			GMapCluster,
			GMapAutocomplete*/
		},

		inheritAttrs: false,

		props: {
			/**
			 * The follow-up action when a marker is clicked.
			 */
			markerFollowUpAction: String,

			/**
			 * The CRS to use for the map.
			 */
			crs: String,

			/**
			 * Google API key.
			 */
			apiKey: {
				type: String,
				required: true
			},

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
			}
		},

		expose: ['isReady', 'isFullscreen'],

		data()
		{
			return {
				zoom: this.styleVariables.zoomLevel ? this.styleVariables.zoomLevel.value : 2,
				centerCoords: this.getCenterCoords(),
				isReady: false,
				isFullscreen: false
			}
		},

		computed: {
			hasValidApiKey()
			{
				return this.apiKey?.startsWith('AIzaSY')
			}
		},

		methods: {
			/**
			 * Converts the specified coordinates to the format expected by google maps.
			 * @param {object} coords The coordinates
			 * @returns An object with the coordinates.
			 */
			convertCoords(coords)
			{
				return coords ? { lat: coords.lat || 0, lng: coords.lng || 0 } : { lat: 0, lng: 0 }
			},

			/**
			 * Calculates the coordinates where to initially center the map.
			 */
			getCenterCoords()
			{
				if (this.isSinglePoint && this.markers.length > 0 && (!this.styleVariables.centerCoord || !this.styleVariables.centerCoord.value))
					return this.convertCoords(this.markers[0].coords)
				return this.convertCoords(this.styleVariables.centerCoord?.value)
			},

			setPlace()
			{

			},

			mapClick()
			{

			}
		}
	}
</script>
