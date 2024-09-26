<template>
	<div
		class="q-submenus-widget">
		<h5 class="q-submenus-widget__title">{{ widget.Title }}</h5>		
		<div 
			class="q-submenus-widget__links" 
			v-if="!isEmpty(widget.MenuEntry.Children)">
			<div 
				class="q-submenus-widget__link"
				v-for="(child, index) in widget.MenuEntry.Children" 
				:key="index"
				@click="navigateToMenu(child)">
				<q-icon 
					class="q-submenus-widget__link-icon"
					v-if="getMenuIcon(child)"
					v-bind="getMenuIcon(child)"
				></q-icon>
				{{ Resources[child.Title] }}
			</div>
		</div>

		<div class="q-submenus-widget__footer">
			<q-icon
				v-if="getMenuIcon(widget.MenuEntry)"
				v-bind="getMenuIcon(widget.MenuEntry)" />
		</div>
	</div>
</template>

<script>
	import VueNavigation from '@/mixins/vueNavigation.js'
	import menuAction from '@/mixins/menuAction.js'
	import LayoutHandlers from '@/mixins/layoutHandlers'

	export default {
		name: 'QSubMenusWidget',

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
			navigateToMenu(menuEntry)
			{
				this.executeMenuAction(menuEntry)
			},
		}
	}
</script>
