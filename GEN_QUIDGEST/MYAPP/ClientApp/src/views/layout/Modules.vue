<template>
	<ul
		v-if="layoutConfig.ModulesStyle === 'collapsible'"
		id="modules-tree-view"
		class="nav nav-pills nav-sidebar n-sidebar__nav d-block collpased-modules">
		<li :class="[{ 'menu-open': moduleMenuIsOpen }, 'nav-item', 'n-sidebar__nav-item', 'has-treeview']">
			<a 
				ref="menuButton"
				id="modules__toggle"
				href="javascript:void(0)"
				:class="['nav-link n-sidebar__nav-link', 'has-icon']"
				:data-key="system.currentModule"
				@click.stop.prevent="toggleModulesMenu"
				@keyup="menuItemKeyup">
				<q-icon-svg
					icon="modules"
					:custom-classes="['nav-icon', 'n-sidebar__icon', 'e-icon', 'section-header-icon']" />

				<p>
					{{ texts.modules }}
					<q-icon
						icon="expand"
						class="right" />
				</p>
			</a>

			<transition name="sidebar-dropdown">
				<ul
					v-if="moduleMenuIsOpen"
					id="collapsible-modules"
					class="nav nav-treeview">
					<all-modules 
						@navigate-to-module="toggleModulesMenu"
						@keyup="menuItemKeyup" />
				</ul>
			</transition>
		</li>
	</ul>
	<ul
		v-else-if="layoutConfig.ModulesStyle === 'list'"
		id="modules-tree-view"
		class="nav nav-pills nav-sidebar n-sidebar__nav d-block modules-list-view">
		<all-modules />
	</ul>
	<div
		v-else-if="layoutConfig.ModulesStyle === 'dropdown'"
		id="modules-tree-view"
		class="n-sidebar__nav-item--dropdown">
		<ul class="nav">
			<li class="dropdown">
				<a
					ref="menuButton"
					id="modules__toggle_dropdown"
					href="javascript:void(0)"
					class="nav-link n-sidebar__nav-link has-icon brand"
					@click="toggleModulesDropdown"
					@focusout="onDropdownFocusout($event)"
					@keyup.escape="closeModulesDropdownAndFocusButton">
					<module-header />
				</a>

				<ul
					ref="modulesDropdown"
					:class="['dropdown-menu', { 'show': showDropdownMenu }]"
					@focusout="onDropdownFocusout($event)"
					@keyup.escape="closeModulesDropdownAndFocusButton">
					<template
						v-for="mod in system.availableModules"
						:key="mod.id">
						<li v-if="mod.id !== system.currentModule">
							<a
								class="dropdown-item"
								href="javascript:void(0)"
								:data-key="mod.id"
								@click.prevent="navigateToModule(mod.id)">
								<q-icon 
									v-if="getModuleIconProps(mod)"
									v-bind="getModuleIconProps(mod)" />
								{{ Resources[mod.title] }}
							</a>
						</li>
					</template>
				</ul>
			</li>
		</ul>
	</div>
</template>

<script>
	import { computed } from 'vue'

	import hardcodedTexts from '@/hardcodedTexts.js'
	import LayoutHandlers from '@/mixins/layoutHandlers.js'
	import VueNavigation from '@/mixins/vueNavigation.js'

	import ModuleHeader from './ModuleHeader.vue'
	import AllModules from './AllModules.vue'

	export default {
		name: 'QModules',

		components: {
			ModuleHeader,
			AllModules
		},

		mixins: [
			LayoutHandlers,
			VueNavigation
		],

		expose: [],

		data()
		{
			return {
				texts: {
					modules: computed(() => this.Resources[hardcodedTexts.modules])
				},
				showDropdownMenu: false
			}
		},

		methods: {
			/**
			 * Focus on the menu toggle button.
			 */
			focusItem()
			{
				//Focus on the menu toggle button
				this.$refs?.menuButton?.focus()
			},

			/**
			 * Close the menu and focus on the menu toggle button.
			 */
			closeMenuAndFocusItem()
			{
				//Focus on the menu toggle button
				this.focusItem()
				
				//Close dropdown
				this.setModuleMenuState(false)
			},

			/*
			 * Called when pressing a key on any menu item
			 */
			menuItemKeyup(event)
			{
				const key = event?.key
				
				if(key === 'Escape')
					this.closeMenuAndFocusItem()
			},

			/**
			 * Close the modules menu and focus on the toggle button.
			 */
			closeModulesDropdownAndFocusButton()
			{
				this.showDropdownMenu = false
				this.$refs.menuButton.$el.focus()
			},

			/**
			 * Toggle showing or hiding the modules dropdown menu.
			 */
			toggleModulesDropdown()
			{
				this.showDropdownMenu = !this.showDropdownMenu
			},

			/**
			 * Handler for focusout on the dropdown menu.
			 * @param {object} event Event object
			 */
			onDropdownFocusout(event)
			{
				if(this.$refs.modulesDropdown.contains(event.relatedTarget)
					|| event.relatedTarget === this.$refs.modulesDropdown
					|| event.relatedTarget === this.$refs.menuButton.$el)
					return
				this.showDropdownMenu = false
			}
		}
	}
</script>
