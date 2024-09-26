<template>
	<div
		v-if="marker.coords || markerHasDescription || markerHasActions"
		:class="popupClass"
		:style="popupStyle"
		@click.stop.prevent>
		<q-button
			v-if="dismissible"
			borderless
			size="small"
			b-style="plain"
			class="q-map-info-window__close"
			:title="texts.close"
			@click="$emit('close-info-window')">
			<q-icon icon="close" />
		</q-button>

		<div class="q-map-marker-info">
			<div
				v-if="marker.coords"
				class="q-map-marker-coords">
				<q-row-container>
					<b>{{ isEuclideanCoord ? 'X' : texts.latitude }}:</b> {{ marker.coords.lat }}
				</q-row-container>

				<q-row-container>
					<b>{{ isEuclideanCoord ? 'Y' : texts.longitude }}:</b> {{ marker.coords.lng }}
				</q-row-container>
			</div>

			<div
				v-if="markerHasDescription"
				class="q-map-marker-description">
				<q-row-container v-if="!showSourcesInDescription">
					<b>{{ texts.description }}</b>
				</q-row-container>

				<template
					v-for="descText in marker.description"
					:key="descText.textValue">
					<q-row-container v-if="descText.textValue && descText.textValue.length > 0">
						<b v-if="showSourcesInDescription && descText.textLabel">
							{{ descText.textLabel }}:
						</b>

						<q-render-data
							:component="descText.source?.component"
							:value="descText.textValue"
							:options="descText.source?.componentOptions || descText.source"
							:resources-path="resourcesPath" />
					</q-row-container>
				</template>
			</div>
		</div>

		<q-button-group
			v-if="markerHasActions"
			class="q-map-marker-btns">
			<q-button
				v-for="btn in markerActions"
				:key="btn.name"
				:title="btn.title"
				:disabled="!hasPermission(btn.name)"
				@click="actionClick(btn.id)">
				<q-icon
					v-if="btn.icon"
					v-bind="btn.icon" />
			</q-button>
		</q-button-group>

		<span class="q-map-popup-tip" />
	</div>
</template>

<script>
	import { defineAsyncComponent } from 'vue'

	import { formModes } from '@/mixins/quidgest.mainEnums.js'

	export default {
		name: 'QMapDefaultPopup',

		emits: [
			'row-action',
			'close-info-window'
		],

		components: {
			QRowContainer: defineAsyncComponent(() => import('@/components/containers/RowContainer.vue')),
			QRenderData: defineAsyncComponent(() => import('@/components/rendering/QRenderData.vue'))
		},

		inheritAttrs: false,

		props: {
			/**
			 * The necessary strings to be used inside the component.
			 */
			texts: {
				type: Object,
				required: true
			},

			/**
			 * Whether or not the popup is for a shape, instead of a point.
			 */
			isShape: {
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
			 * The marker data.
			 */
			marker: {
				type: Object,
				required: true
			},

			/**
			 * The available actions.
			 */
			markerActions: {
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
			 * Path for the resources.
			 */
			resourcesPath: {
				type: String,
				required: true
			},

			/**
			 * Whether the popup is dismissible.
			 */
			dismissible: {
				type: Boolean,
				default: false
			},

			/**
			 * The mouse X position.
			 */
			mouseX: {
				type: Number,
				default: 0
			},

			/**
			 * The mouse Y position (inverted vertically).
			 */
			mouseY: {
				type: Number,
				default: 0
			}
		},

		expose: [],

		computed: {
			/**
			 * The popup class depending on the configuration.
			 */
			popupClass()
			{
				const classes = ['q-map-info-window']

				if (this.styleVariables.openPopupOnHover?.value)
					classes.push('q-map-info-window-dynamic')
				else
				{
					classes.push('q-map-info-window-static')

					if (this.isShape)
						classes.push('q-map-shape-info')
				}

				return classes
			},

			/**
			 * The popup position depending on the configuration.
			 */
			popupStyle()
			{
				return this.styleVariables.openPopupOnHover?.value
					? { left: `${this.mouseX}px`, bottom: `${this.mouseY}px`, transform: 'translate(-50%, -1rem)' }
					: {}
			},

			/**
			 * True if the marker has a description, false otherwise.
			 */
			markerHasDescription()
			{
				return Array.isArray(this.marker.description) && this.marker.description.length > 0 &&
					this.marker.description.some((el) => typeof el.textValue === 'string' && el.textValue.length > 0)
			},

			/**
			 * True if the marker has actions, false otherwise.
			 */
			markerHasActions()
			{
				return this.markerActions && this.markerActions.length > 0
			},

			/**
			 * True if the labels of the data sources should be shown in the description, false otherwise.
			 */
			showSourcesInDescription()
			{
				return this.styleVariables.showSourcesInDescription && this.styleVariables.showSourcesInDescription.value
			}
		},

		methods: {
			/**
			 * Handles the button click.
			 * @param {string} actionType The action type
			 */
			actionClick(actionType)
			{
				const actionData = {
					id: actionType,
					rowKey: this.marker.rowKey
				}

				this.$emit('row-action', actionData)

				// Ensures the popup closes when navigating to the support form, necessary if the form is a popup.
				this.$emit('close-info-window')
			},

			/**
			 * Checks if the user has permission to execute the specified action.
			 * @param {string} actionType The action type
			 * @returns True if the user has permission, false otherwise.
			 */
			hasPermission(actionType)
			{
				const permissions = this.marker.btnPermission

				if (!permissions)
					return false

				switch (actionType.toUpperCase())
				{
					case formModes.show:
						return !permissions.viewBtnDisabled
					case formModes.edit:
						return !permissions.editBtnDisabled
					case formModes.duplicate:
						return !permissions.insertBtnDisabled
					case formModes.delete:
						return !permissions.deleteBtnDisabled
				}

				return true
			}
		}
	}
</script>
