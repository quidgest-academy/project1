<template>
	<div id="flat-menu-container">
		<div id="flat-menu-content">
			<nav
				class="main-header navbar navbar-expand c-header--sidebar"
				ref="header">
				<ul
					v-if="Object.keys(system.availableModules).length > 0"
					class="navbar-nav c-header__content--sidebar">
					<li class="nav-item c-header__item--sidebar">
						<q-button 
							id="main-menu-toggle"
							class="nav-link c-header__link--sidebar action-item" 
							:title="texts.mainMenu"
							:aria-expanded="!sidebarIsCollapsed"
							@click.stop.prevent="toggleSidebar">
							<q-icon icon="menu-hamburger" />
						</q-button>
					</li>

					<li
						v-if="config.QAEnvironment === 1"
						class="nav-item c-header__item--sidebar">
						<span class="nav-link c-header__link--sidebar betatesting">
							<q-icon icon="alert" /> TESTING
						</span>
					</li>
				</ul>

				<div class="ml-auto d-flex">
					<q-select
						v-if="userIsLoggedIn && system.availableSystems.length > 1"
						id="system-years"
						size="fit-content"
						:model-value="system.currentSystem"
						:items="availableSystems"
						:groups="availableSystemsGroups"
						@update:model-value="selectSystem">
						<template #prepend>
							<q-icon icon="system-choice" />
						</template>
					</q-select>

					<q-tooltip
						anchor="#system-years"
						placement="bottom"
						:text="texts.systemYears" />

					<language-items v-if="layoutConfig.LanguagePlacement === 'in_header'" />

					<embedded-menu />
				</div>
			</nav>

			<menu-flat
				:key="userData.name"
				:loading="loadingMenus" />
		</div>
	</div>
</template>

<script>
	import { defineAsyncComponent, computed } from 'vue'
	import { mapActions, mapState } from 'pinia'

	import { useSystemDataStore } from '@/stores/systemData.js'
	import mainConfigUtils from '@/api/global/mainConfigUtils.js'
	import LayoutHandlers from '@/mixins/layoutHandlers'
	import hardcodedTexts from '@/hardcodedTexts.js'

	import EmbeddedMenu from '@/views/shared/EmbeddedMenu.vue'
	import MenuFlat from './MenuFlat.vue'

	export default {
		name: 'QNavigationBar',

		components: {
			LanguageItems: defineAsyncComponent(() => import('@/views/shared/LanguageItems.vue')),
			EmbeddedMenu,
			MenuFlat
		},

		mixins: [LayoutHandlers],

		props: {
			/**
			 * Whether or not the menu structure is loading.
			 */
			loadingMenus: {
				type: Boolean,
				default: false
			}
		},

		expose: [],

		data()
		{
			return {
				texts: {
					systemYears: computed(() => this.Resources[hardcodedTexts.systemYears]),
					mainMenu: computed(() => this.Resources[hardcodedTexts.mainMenu])
				}
			}
		},

		mounted()
		{
			if (this.options.autoCollapseSize)
				this.autoCollapseSidebar(false)

			this.setListeners()
		},

		beforeUnmount()
		{
			this.removeListeners()
		},

		computed: {
			...mapState(useSystemDataStore, [
				'system'
			]),

			availableSystems()
			{
				return (this.system.availableSystems || []).map((availableSystem) => ({
					key: availableSystem,
					value: availableSystem.toString(),
					disabled: this.system.currentSystem === availableSystem,
					group: availableSystem
				}))
			},

			availableSystemsGroups()
			{
				return (this.system.availableSystems || []).map((availableSystem) => ({
					id: availableSystem
				}))
			}
		},

		methods: {
			...mapActions(useSystemDataStore, [
				'setCurrentSystem'
			]),

			/**
			 * Change the system (year) in the store and redirect to the home page.
			 * We should not preserve the current page because the user may not even have access to it on the other system.
			 * @param {String} selectedSystem Selected system
			 */
			selectSystem(selectedSystem)
			{
				this.setCurrentSystem(selectedSystem)
				// Before opening the home page, we must update the configuration to have the menu list for this system.
				mainConfigUtils.updateMainConfig(() => {
					this.$router.push({
						name: 'home',
						params: {
							culture: this.system.currentLang,
							system: selectedSystem,
							module: this.system.currentModule
						}
					})
				})
			}
		},

		watch: {
			headerHeight: {
				handler(val)
				{
					document.documentElement.style.setProperty('--visible-header-height', val)
				},
				immediate: true
			}
		}
	}
</script>
