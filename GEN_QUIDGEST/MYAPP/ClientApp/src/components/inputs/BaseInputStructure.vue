<template>
	<div
		v-if="isVisible"
		:id="controlId"
		ref="mainWrapper"
		v-bind="wrapperAttrs"
		:class="[{ draggable: reportingModeOn }, classObject.labelPosition]"
		:data-draggable="reportingModeOn"
		:data-loading="loading">
		<div
			style="align-items: center"
			:class="[classObject.labelContainerFlex, ...classes]">
			<slot
				v-if="labelPosition !== labelAlignment.left"
				name="label" />

			<label
				v-if="hasLabel && !isEmpty(label)"
				:id="labelId"
				v-bind="labelAttrs"
				:for="id"
				:data-val-required="isRequired && !(readonly || disabled)"
				:class="[{ disabled: disabled }, ...(classObject.labelClass || [])]">
				{{ label }}
			</label>

			<slot
				v-if="labelPosition === labelAlignment.left"
				name="label" />

			<q-popover-help
				v-if="popoverText && !isEmpty(label)"
				:help-control="helpControl"
				:id="id"
				:label="label" />

			<q-tooltip-help
				v-if="tooltipText && !isEmpty(label)"
				:help-control="helpControl"
				:anchor="anchorId" />

			<a
				v-if="reportingModeOn"
				href="javascript:void(0)"
				class="q-icon--reporting report-mode"
				@click.stop.prevent="addCavField">
				<q-icon icon="stats" />
			</a>

			<a
				v-if="hasSuggestions && suggestionModeOn && !isEmpty(label)"
				href="javascript:void(0)"
				class="suggest suggest-mode"
				@click.stop.prevent="openSuggestionMode">
				<q-icon icon="new-suggestion" />
			</a>
		</div>

		<component
			v-if="viewMode?.props"
			:is="`q-${viewMode.type}`"
			:texts="texts"
			v-bind="viewMode.props"
			v-on="viewMode.handlers" />
		<slot v-else />

		<q-subtext-help
			v-if="hasSubtext && !isEmpty(label)"
			:help-control="helpControl"
			:label="label" />
	</div>
</template>

<script>
	import { defineAsyncComponent } from 'vue'
	import _isEmpty from 'lodash-es/isEmpty'

	// Core SortableJS (without default plugins)
	import Sortable from 'sortablejs/modular/sortable.core.esm.js'

	import { labelAlignment } from '@/mixins/quidgest.mainEnums.js'
	import HelpControl from '@/mixins/helpControls.js'

	export default {
		name: 'QBaseInputStructure',

		emits: ['show-suggestion-popup'],

		inheritAttrs: false,

		mixins: [HelpControl],

		components: {
			QPopoverHelp: defineAsyncComponent(() => import('@/components/QPopoverHelp.vue')),
			QTooltipHelp: defineAsyncComponent(() => import('@/components/QTooltipHelp.vue')),
			QSubtextHelp: defineAsyncComponent(() => import('@/components/QSubtextHelp.vue'))
		},

		props: {
			/**
			 * Unique identifier for the control.
			 */
			id: {
				type: String,
				required: true // Needs to be required because it's necessary for the suggestions.
			},

			/**
			 * Text strings which might be used to override default texts within the component.
			 */
			texts: Object,

			/**
			 * The label text for the input field.
			 */
			label: {
				type: String,
				default: ''
			},

			/**
			 * The position of the label relative to the input field.
			 */
			labelPosition: {
				type: String,
				default: labelAlignment.topleft,
				validator: (value) => _isEmpty(value) || Reflect.has(labelAlignment, value)
			},

			/**
			 * Flag indicating if the label is to be displayed.
			 */
			hasLabel: {
				type: Boolean,
				default: true
			},

			/**
			 * Controls the readonly state of the input field.
			 */
			readonly: {
				type: Boolean,
				default: false
			},

			/**
			 * Disables the input field, preventing user interaction.
			 */
			disabled: {
				type: Boolean,
				default: false
			},

			/**
			 * Determines if the input field is marked as required.
			 */
			isRequired: {
				type: Boolean,
				default: false
			},

			/**
			 * Whether or not the control is currently visible.
			 */
			isVisible: {
				type: Boolean,
				default: true
			},

			/**
			 * The name of the array if this control is part of an array structure.
			 */
			arrayName: {
				type: String,
				default: ''
			},

			/**
			 * Set flexbox to display inline if true.
			 */
			dFlexInline: {
				type: Boolean,
				default: false
			},

			/**
			 * An array of additional CSS classes to apply to the component.
			 */
			classes: {
				type: Array,
				default: () => []
			},

			/**
			 * Information about the model that the input is bound to, such as the table and field IDs.
			 */
			modelInfo: {
				// tableId | fieldId
				type: Object,
				default: null
			},

			/**
			 * Indicates whether the control is inside reporting mode.
			 */
			reportingModeOn: {
				type: Boolean,
				default: false
			},

			/**
			 * Specifies if the control has suggestions functionality enabled.
			 */
			hasSuggestions: {
				type: Boolean,
				default: false
			},

			/**
			 * Determines if suggestion mode is currently active.
			 */
			suggestionModeOn: {
				type: Boolean,
				default: false
			},

			/**
			 * Possible view modes for the control that can alter its presentation.
			 */
			viewModes: {
				type: Array,
				default: () => []
			},

			/**
			 * Whether the control is currently loading.
			 */
			loading: {
				type: Boolean,
				default: false
			}
		},

		expose: [],

		data()
		{
			return {
				labelAlignment,

				controlId: `container-${this.id || this._.uid}`,

				classObject: {
					labelPosition:
						_isEmpty(this.label) || _isEmpty(this.labelPosition)
							? ''
							: `label${this.labelPosition}`,
					labelContainerFlex: this.dFlexInline ? 'label-container--inline' : 'label-container'
				},

				wrapperAttrs: {
					class: this.$attrs.class ?? ''
				},

				labelAttrs: this.$attrs.labelAttrs ?? {},

				sortablePlugin: null
			}
		},

		mounted()
		{
			this.initialize()
		},

		beforeUnmount()
		{
			this.draggableDestroy()
		},

		computed: {
			/**
			 * The identifier for the label element associated with the control.
			 */
			labelId()
			{
				return `label_${this.id}`
			},

			/**
			 * Determines the first view mode object if any are present.
			 */
			viewMode()
			{
				// For now, we only allow one view mode over the same field
				// and have no option to toggle between view modes.
				if (Array.isArray(this.viewModes) && !_isEmpty(this.viewModes))
					return this.viewModes[0]
				return null
			},

			/**
			 * Verified if the help has a subtext.
			 */
			hasSubtext() {
				return this.helpControl?.shortHelp.type === 'Subtext'
			}
		},

		methods: {
			isEmpty: _isEmpty,

			/**
			 * Initializes the component by setting up draggability if reporting mode is on.
			 */
			initialize()
			{
				this.draggable(true)
			},

			/**
			 * Emits an event to show the suggestion popup with component details.
			 */
			openSuggestionMode()
			{
				const params = {
					id: this.id,
					label: this.label,
					help: this.shortHelp,
					detailHelp: this.detailHelp,
					arrayName: this.arrayName
				}

				this.$emit('show-suggestion-popup', 'SuggestionIndex', params)
			},

			/**
			 * Provides information to the HTML5 drag event dataTransfer object.
			 * @param {DataTransfer} dataTransfer - The dataTransfer object of the HTML5 DragEvent
			 */
			draggableSetData(dataTransfer)
			{
				// `dataTransfer` object of HTML5 DragEvent
				dataTransfer.setData('cav-field-info', JSON.stringify(this.modelInfo))
			},

			/**
			 * Sets up or destroys sortable dragging functionality based on the given flag and if reporting mode is active.
			 * @param {boolean} isMount - Indicates whether the method is called during mount.
			 */
			draggable(isMount)
			{
				this.draggableDestroy()

				try
				{
					if (this.reportingModeOn)
					{
						this.sortablePlugin = Sortable.create(
							(Array.isArray(this.$refs.mainWrapper)
								? this.$refs.mainWrapper[0]
								: this.$refs.mainWrapper
							).parentElement,
							{
								group: {
									name: 'cav-fields',
									pull: 'clone',
									put: false
								},
								draggable: `#${this.controlId}[data-draggable]`,
								sort: false,
								dataIdAttr: 'id',
								setData: this.draggableSetData
							}
						)
					}
					else if (!isMount)
						this.draggableDestroy()
				}
				catch
				{
					this.draggableDestroy()
				}
			},

			/**
			 * Destroys the Sortable instance if it exists.
			 */
			draggableDestroy()
			{
				this.sortablePlugin?.destroy()
				this.sortablePlugin = null
			},

			/**
			 * Triggers the addition of a custom view field in the event hub.
			 */
			addCavField()
			{
				this.$eventHub?.emit('add-cav-field', this.modelInfo)
			}
		},

		watch: {
			reportingModeOn()
			{
				this.draggable(false)
			}
		}
	}
</script>
