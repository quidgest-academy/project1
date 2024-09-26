<template>
	<div
		v-if="userIsLoggedIn"
		v-show="!isSidebarEmpty"
		class="wrapper">
		<div class="c-right-sidebar__control">
			<!--This button must use the v-show because the button must always exist-->
			<q-button
				v-show="rightSidebarIsCollapsed"
				ref="sidebarOpenButton"
				:id="sidebarOpenButtonId"
				b-style="secondary"
				class="right-sidebar-control"
				:title="texts.open"
				@click="openSidebar">
				<q-icon icon="step-back" />
			</q-button>
		</div>

		<div
			ref="sidebar"
			id="right-sidenav"
			:class="classes"
			@transitionend="onTransitionEnd">
			<div class="c-right-sidebar__container">
				<nav class="c-right-sidebar__header">
					<div
						class="nav flex-column nav-pills"
						aria-orientation="vertical">
						<q-button
							ref="sidebarCloseButton"
							id="close-sidebar-btn"
							:title="texts.close"
							:disabled="disableButtons"
							@click="closeSidebar">
							<q-icon icon="close" />
						</q-button>

						<q-button
							v-if="isCavAvailable && !suggestionModeOn"
							id="advanced-report-mode-toggle"
							:active="reportingModeCAV"
							:title="texts.enterInReport"
							:disabled="disableButtons"
							@click="toggleReportingMode">
							<q-icon icon="stats" />
						</q-button>

						<q-button
							v-if="showFormAnchors && !suggestionModeOn && layoutConfig.FormAnchorsPosition === 'sidebar'"
							id="form-tree-toggle"
							:active="isActive('form-anchors-tab')"
							:title="texts.formAreas"
							:disabled="disableButtons"
							@click="toggleSidebarTab('form-anchors-tab')">
							<q-icon icon="list-bordered" />
						</q-button>

						<q-button
							v-show="showFormActions && !suggestionModeOn"
							id="form-actions-toggle"
							:active="isActive('form-actions-tab')"
							:title="texts.formActions"
							:disabled="disableButtons"
							@click="toggleSidebarTab('form-actions-tab')">
							<q-icon icon="more-items" />
						</q-button>

						<q-button
							v-if="appAlerts.length > 0 && !suggestionModeOn"
							id="alerts-toggle"
							:active="isActive('alerts-tab')"
							:title="texts.alerts"
							:disabled="disableButtons"
							@click="toggleSidebarTab('alerts-tab')">
							<q-icon icon="notifications" />

							<span
								v-if="notifications.length > 0"
								class="e-badge e-badge--highlight">
								<span aria-hidden="true" />
							</span>
						</q-button>

						<q-button
							v-if="isSuggestionsAvailable"
							id="suggestion-mode-toggle"
							:active="suggestionModeOn"
							:title="suggestionModeOn ? texts.closeSuggestions : texts.enterInSuggestion"
							:disabled="disableButtons"
							@click="toggleSuggestionModeOn">
							<q-icon :icon="suggestionModeOn ? 'suggestion-mode-close' : 'suggestion-mode'" />
						</q-button>

						<q-button
							v-if="suggestionModeOn"
							id="suggestion-view"
							:title="texts.suggestions"
							:disabled="disableButtons"
							@click="openSuggestionList">
							<q-icon icon="suggestion-mode-view" />
						</q-button>

						<q-button
							v-if="suggestionModeOn"
							id="suggestion-open"
							:title="texts.suggest"
							:disabled="disableButtons"
							@click="openSuggestionMode">
							<q-icon icon="new-suggestion" />
						</q-button>

						<q-button
							v-if="isChatBotAvailable"
							id="chatbot-toggle"
							:active="isActive('chatbot-tab')"
							b-style="secondary"
							title="ChatBot"
							class="nav-link"
							:disabled="disableButtons"
							@click="toggleSidebarTab('chatbot-tab')">
							<q-icon-img :icon="`${system.resourcesPath}chatbot.png`" />
						</q-button>
					</div>
				</nav>

				<div class="c-right-sidebar__content">
					<div
						v-show="extendedTab === 'form-actions-tab' && showFormActions"
						id="form-actions-tab"
						class="c-tab__item--sidebar">
						<form-action-buttons :buttons-list="formModeData" />
					</div>

					<div
						v-if="appAlerts.length > 0"
						v-show="extendedTab === 'alerts-tab'"
						id="alerts-tab">
						<alerts
							:alerts="notifications"
							@fetch-alerts="fetchAlerts"
							@clear-alerts="clearNotifications"
							@dismiss-alert="removeNotification"
							@navigate-to="onAlertClick" />
					</div>

					<div
						v-show="extendedTab === 'form-anchors-tab' && showFormAnchors"
						id="form-anchors-tab">
						<q-anchor-container-vertical
							:title="texts.formAreas"
							:tree="formAnchorsTree"
							:header-height="visibleHeaderHeight"
							@focus-control="(...args) => $emit('focus-control', ...args)" />
					</div>

					<div
						v-if="isChatBotAvailable"
						v-show="extendedTab === 'chatbot-tab'"
						id="chatbot-tab">
						<q-chat-bot
							:username="userData.name"
							:project-path="applicationName"
							:date-format="system.dateFormat.dateTimeSeconds"
							:api-endpoint="chatbotProxyUrl" />
					</div>

					<div v-show="extendedTab === 'widgets-panel'">
						<div id="widgets-panel"></div>
					</div>
				</div>
			</div>
		</div>
	</div>
</template>

<script>
	import { computed, defineAsyncComponent } from 'vue'
	import { mapState, mapActions } from 'pinia'

	import { useSystemDataStore } from '@/stores/systemData.js'
	import { useGenericDataStore } from '@/stores/genericData.js'
	import hardcodedTexts from '@/hardcodedTexts.js'
	import LayoutHandlers from '@/mixins/layoutHandlers.js'
	import AlertHandlers from '@/mixins/alertHandlers.js'

	export default {
		name: 'QSidebar',

		emits: [
			'changed-sidebar-width',
			'focus-control'
		],

		components: {
			QAnchorContainerVertical: defineAsyncComponent(() => import('@/components/containers/QAnchorContainerVertical.vue')),
			FormActionButtons: defineAsyncComponent(() => import('./FormActionButtons.vue')),
			Alerts: defineAsyncComponent(() => import('./Alerts.vue')),
			QChatBot: defineAsyncComponent(() => import('@quidgest/chatbot'))
		},

		mixins: [
			AlertHandlers,
			LayoutHandlers
		],

		expose: [],

		data()
		{
			return {
				formModeData: {},

				formAnchorsTree: [],

				extendedTab: '',

				focusedSidebarButtonId: null,

				texts: {
					open: computed(() => this.Resources[hardcodedTexts.open]),
					alerts: computed(() => this.Resources[hardcodedTexts.alerts]),
					close: computed(() => this.Resources[hardcodedTexts.close]),
					formActions: computed(() => this.Resources[hardcodedTexts.formActions]),
					enterInReport: computed(() => this.Resources[hardcodedTexts.enterInReport]),
					enterInSuggestion: computed(() => this.Resources[hardcodedTexts.enterInSuggestion]),
					suggest: computed(() => this.Resources[hardcodedTexts.suggest]),
					suggestions: computed(() => this.Resources[hardcodedTexts.suggestions]),
					closeSuggestions: computed(() => this.Resources[hardcodedTexts.closeSuggestions]),
					formAreas: computed(() => this.Resources[hardcodedTexts.formAreas])
				}
			}
		},

		mounted()
		{
			if (this.options?.autoCollapseSize)
			{
				this.autoCloseSidebar(false)
				//Must be called here to finalize visibilty because the open/close transition does not happen when loading
				this.onTransitionEnd()
				window.addEventListener('resize', this.autoCloseSidebar)
			}

			this.$eventHub.on('changed-form-buttons', (sections) => {
				this.formModeData = sections
			})

			this.$eventHub.on('changed-form-anchors', (tree) => {
				this.formAnchorsTree = tree
			})

			this.$eventHub.on('open-sidebar-on-tab', (tabId) => {
				this.openSidebar()
				this.toggleSidebarTab(tabId)
			})

			// Sets the default state for the sidebar (closed or opened).
			if (this.layoutConfig.DefaultSidebarState !== 'opened' && !this.isChatBotAvailable)
				this.closeSidebar()

			// Emits the initial width of the sidebar.
			this.onSidebarWidthChange()
		},

		beforeUnmount()
		{
			if (this.options?.autoCollapseSize)
				window.removeEventListener('resize', this.autoCloseSidebar)

			this.$eventHub.off('changed-form-buttons')
			this.$eventHub.off('changed-form-tree')

			this.onSidebarWidthChange()
		},

		computed: {
			...mapState(useSystemDataStore, [
				'isCavAvailable',
				'isSuggestionsAvailable',
				'isChatBotAvailable',
				'applicationName'
			]),

			...mapState(useGenericDataStore, [
				'reportingModeCAV',
				'suggestionModeOn',
				'notifications'
			]),

			/**
			 * True if the button to toggle the form actions should be visible, false otherwise.
			 */
			showFormActions()
			{
				return Object.keys(this.formModeData).length > 0
			},

			/**
			 * The width of the right sidebar (in rem).
			 */
			sidebarWidth()
			{
				let width = 0

				if (this.userIsLoggedIn && !this.rightSidebarIsCollapsed)
				{
					if (this.extendedTab)
					{
						if (this.extendedTab === 'chatbot-tab')
							width = 35
						else
							width = 18.75
					}
					else if (!this.isSidebarEmpty)
						width = 3.125
				}

				return width
			},

			/**
			 * True if the form anchors should be visible, false otherwise.
			 */
			showFormAnchors()
			{
				return Array.isArray(this.formAnchorsTree) && this.formAnchorsTree.length > 0
			},

			/**
			 * True if the buttons of the sidebar should be disabled, false otherwise.
			 */
			disableButtons()
			{
				return this.extendedTab === 'widgets-panel'
			},

			/**
			 * True if the alerts tab should be visible, false otherwise.
			 */
			showAlerts()
			{
				return this.appAlerts.length > 0 && this.extendedTab === 'alerts-tab'
			},

			/**
			 * True if the sidebar is empty, false otherwise.
			 */
			isSidebarEmpty()
			{
				return !this.showFormActions &&
					!this.isCavAvailable &&
					!this.isSuggestionsAvailable &&
					!this.isChatBotAvailable &&
					!this.showFormAnchors &&
					!this.disableButtons &&
					!this.showAlerts
			},

			/**
			 * Backend proxy endpoint to re-route chabot's requests.
			 */
			chatbotProxyUrl()
			{
				return 'chatbotapi'
			},

			classes()
			{
				let classes = []

				classes.push('c-right-sidebar')
				if(!this.rightSidebarIsVisible)
					classes.push('invisible')

				return classes
			},

			/**
			 * ID of the DOM element to open the sidebar.
			 */
			sidebarOpenButtonId()
			{
				return "open-sidebar-btn"
			}
		},

		methods: {
			...mapActions(useGenericDataStore, [
				'setSuggestionMode',
				'toggleSuggestionMode',
				'clearNotifications',
				'removeNotification'
			]),

			onSidebarWidthChange()
			{
				if (this.userIsLoggedIn && !this.isSidebarEmpty)
					this.$emit('changed-sidebar-width', this.sidebarWidth)
				else
					this.$emit('changed-sidebar-width', 0)
			},

			openSidebar()
			{
				/**
				 * Check if the open button was focused and save this value which is checked after the CSS transition ends.
				 * This must be done here because the button element disappears and loses focus during this function, 
				 * after setting the collapse state.
				 */
				if(document?.activeElement?.id === this.sidebarOpenButtonId)
					this.focusedSidebarButtonId = this.sidebarOpenButtonId

				this.setRightSidebarCollapseState(false)
			},

			closeSidebar()
			{
				this.setRightSidebarCollapseState(true)
				this.extendedTab = ''
			},

			/**
			 * Called when a CSS transition for the right sidebar finishes
			 */
			onTransitionEnd()
			{
				/**
				 * If the right sidebar is being closed, set the actual value for visibility to false.
				 * Must be done here, after the transition ends so it doesn't disappear before the CSS transition.
				 */
				if(this.rightSidebarIsCollapsed)
					this.setRightSidebarVisibility(false)

				/**
				 * Check if any of the sidebar buttons were focused and, if so, decide which one the focus should move to. 
				 * Must be done here, after the CSS transition ends so the element that will be focused is visible and can be focused.
				 */
				let sidebarOpenButton = this.$refs?.sidebarOpenButton
				let sidebarCloseButton = this.$refs?.sidebarCloseButton
				let sidebar = this.$refs?.sidebar

				// If the open button was focused
				if(this.focusedSidebarButtonId === this.sidebarOpenButtonId)
				{
					// Focus on the close button
					sidebarCloseButton?.$el?.focus()
					this.focusedSidebarButtonId = null
				}
				// If any of the buttons within the sidebar were focused
				else if(sidebar?.contains(document?.activeElement))
				{
					// Focus on the open button
					sidebarOpenButton?.$el?.focus()
				}
			},

			toggleSidebarTab(tabId)
			{
				if (this.extendedTab === tabId)
					this.extendedTab = ''
				else
					this.extendedTab = tabId
			},

			toggleReportingMode()
			{
				this.$eventHub.emit('toggle-reporting-mode')
			},

			toggleSuggestionModeOn()
			{
				this.toggleSuggestionMode()

				if (this.suggestionModeOn)
					this.extendedTab = ''
			},

			openSuggestionMode()
			{
				const params = {
					id: '',
					label: '',
					help: '',
					arrayName: '',
				}

				this.$eventHub.emit('show-suggestion-popup', 'SuggestionIndex', params)
			},

			openSuggestionList()
			{
				this.$eventHub.emit('show-suggestion-popup', 'SuggestionList', {})
			},

			/**
			 * Collapses the right sidebar when a certain screen size is reached.
			 * @param {boolean} resize Whether or not the window is being resized
			 */
			autoCloseSidebar(resize = true)
			{
				if (resize && !this.options?.autoCollapseSize || this.extendedTab === 'widgets-panel')
					return

				if (this.options?.autoCollapseSize && window.innerWidth <= this.options.autoCollapseSize)
					this.closeSidebar()
				else
					this.openSidebar()
			},

			isActive(buttonId)
			{
				return this.extendedTab === buttonId
			}
		},

		watch: {
			isSidebarEmpty()
			{
				this.onSidebarWidthChange()
			},

			sidebarWidth()
			{
				this.onSidebarWidthChange()
			},

			$route(to)
			{
				if (typeof to.name !== 'string' || !to.name.startsWith('form'))
				{
					this.formModeData = {}
					this.formAnchorsTree = []

					if (this.extendedTab === 'form-actions-tab' || this.extendedTab === 'form-anchors-tab')
						this.extendedTab = ''
				}
			}
		}
	}
</script>
