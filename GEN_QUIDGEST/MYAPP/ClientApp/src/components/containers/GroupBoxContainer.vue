<template>
	<div
		:id="controlId"
		v-show="isVisible"
		:class="['c-groupbox', { 'c-groupbox--no-border': noBorder }, $attrs.class]">
		<div
			v-if="label"
			class="c-groupbox__title"
			:id="labelId">
			{{ label }}

			<q-popover-help
				v-if="popoverText"
				:help-control="helpControl"
				:id="id"
				:label="label" />
		</div>

		<q-tooltip-help
			v-if="tooltipText"
			:help-control="helpControl"
			:anchor="anchorId"
			:label="label" />

		<q-subtitle-help
			v-if="subtitleText"
			:help-control="helpControl"
			:label="label" />

		<div
			:id="`${controlId}-content`"
			class="form-flow">
			<slot></slot>
		</div>
	</div>
</template>

<script>
	import HelpControl from '@/mixins/helpControls.js'
	import { defineAsyncComponent } from 'vue'

	export default {
		name: 'QGroupBoxContainer',

		inheritAttrs: false,

		components: {
			QPopoverHelp: defineAsyncComponent(() => import('@/components/QPopoverHelp.vue')),
			QTooltipHelp: defineAsyncComponent(() => import('@/components/QTooltipHelp.vue')),
			QSubtitleHelp: defineAsyncComponent(() => import('@/components/QSubtitleHelp.vue'))
		},

		mixins: [HelpControl],

		props: {
			/**
			 * Unique identifier for the control.
			 */
			id: {
				type: String,
				default: null
			},

			/**
			 * The group label.
			 */
			label: {
				type: String,
				default: null
			},

			/**
			 * Visible property to hide and show group.
			 */
			isVisible: {
				type: Boolean,
				default: true
			},

			/**
			 * Whether or not the group should have a border.
			 */
			noBorder: {
				type: Boolean,
				default: false
			}
		},

		expose: [],

		data()
		{
			return {
				controlId: this.id || `groupbox-${this._.uid}`
			}
		},

		computed: {
			labelId() {
				return `label_${this.controlId}`
			},

			anchorId() {
				return this.labelId? `#${this.labelId}` : ""
			}
		}
	}
</script>
