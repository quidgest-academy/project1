<template>
	<div
		class="q-menu-widget"
		@click.stop="onClick">
		<h5 class="q-menu-widget__title">{{ widget.Title }}</h5>
		<q-icon 
			v-if="getMenuIcon(widget.MenuEntry)"
			v-bind="getMenuIcon(widget.MenuEntry)" />
		<q-icon
			v-else
			icon="bookmark" />
	</div>
</template>

<script>
	import VueNavigation from '@/mixins/vueNavigation.js'
	import menuAction from '@/mixins/menuAction.js'
	import LayoutHandlers from '@/mixins/layoutHandlers'

	export default {
		name: 'QMenuWidget',

		mixins: [VueNavigation, menuAction, LayoutHandlers],

		inheritAttrs: false,

		props: {
			/**
			 * The widget.
			 */
			widget: {
				type: Object,
				default: () => ({})
			},

			/**
			 * The path to the resources.
			 */
			resourcesPath: String
		},

		expose: [],

		methods: {
			onClick()
			{
				if (this.widget.MenuEntry)
					this.executeMenuAction(this.widget.MenuEntry)
			}
		}
	}
</script>
