<template>
	<li class="dropdown n-menu__aside-item">
		<div class="d-flex align-items-center">
			<q-button
				v-if="userIsLoggedIn && appAlerts.length > 0"
				id="sidebar-collapse"
				class="nav-link n-menu__aside-link"
				href="javascript:void(0)"
				role="button"
				aria-haspopup="true"
				aria-expanded="true"
				:aria-label="texts.options"
				:tabindex="$attrs.tabindex"
				@click.stop.prevent="openAlert">
				<span
					data-toggle="tooltip"
					data-placement="left"
					:title="texts.alerts">
					<q-icon icon="notifications" />
				</span>

				<span
					class="e-badge e-badge--highlight"
					aria-hidden="true">
					{{ notifications.length }}
				</span>

				<span class="hidden-elem">
					{{ texts.options }}
				</span>
			</q-button>

			<button
				type="button"
				name="user-avatar"
				class="dropdown-toggle n-menu__aside-link removecaret"
				style="border-width: 0; background-color: transparent"
				data-toggle="dropdown"
				aria-haspopup="true"
				aria-expanded="false"
				:title="texts.userAvatar"
				:tabindex="$attrs.tabindex">
				<img
					class="avatar"
					data-toggle="tooltip"
					data-placement="left"
					aria-hidden="true"
					:src="avatarSrc"
					:alt="texts.userAvatar"
					:title="userData.name" />
			</button>

			<div
				class="dropdown-menu dropdown-menu-right c-user__dropdown"
				aria-labelledby="navbarDropdown">
				<div class="q-card__content">
					<div class="q-card__title n-module__title">{{ fullName }}</div>
					<div
						class="q-card__subtitle n-module__subtitle"
						style="padding-bottom: 0">
						{{ userRole }}
					</div>
				</div>

				<ul class="c-sidebar__list">
					<li
						v-for="menu in model.EPHUserAvatarMenus"
						:key="menu.Title"
						class="c-sidebar__list-item">
						<q-router-link
							data-toggle="tooltip"
							data-placement="top"
							class="c-sidebar__list-link"
							:title="getMenuText(menu.Title)"
							:link="getMenuRoute(menu, true)"
							:tabindex="$attrs.tabindex">
							<i
								v-if="menu.Font"
								:class="[menu.Font, 'c-header__icon']"></i>
							{{ getMenuText(menu.Title) }}
						</q-router-link>
					</li>

					<li
						v-for="menu in model.UserAvatarMenus"
						:key="menu.Title"
						class="c-sidebar__list-item">
						<q-router-link
							data-toggle="tooltip"
							data-placement="top"
							class="c-sidebar__list-link"
							:title="getMenuText(menu.Title)"
							:link="getMenuRoute(menu)"
							:tabindex="$attrs.tabindex">
							<i
								v-if="menu.Font"
								:class="['glyphicons', `glyphicons-${menu.Font}`, 'c-header__icon']"></i>
							{{ getMenuText(menu.Title) }}
						</q-router-link>
					</li>

					<template
						v-for="module in system.availableModules"
						:key="module.id">
						<li
							v-if="system.currentModule === module.id"
							class="c-sidebar__list-item">
							<a
								:href="`Content/Manual/${module.id}Manual.pdf`"
								data-toggle="tooltip"
								target="_blank"
								rel="noopener noreferrer"
								class="c-sidebar__list-link"
								data-placement="top"
								:title="texts.userHelp"
								:tabindex="$attrs.tabindex">
								<span>
									<q-icon icon="user-help" />
									{{ texts.userHelp }}
								</span>
							</a>
						</li>
					</template>

					<template v-if="config.LoginType !== 'AD'">
						<li
							class="c-sidebar__list-item"
							v-if="hasUserSettings">
							<a
								href="javascript:void(0)"
								data-toggle="tooltip"
								class="c-sidebar__list-link"
								data-placement="top"
								:title="texts.userSettings"
								:tabindex="$attrs.tabindex"
								@click.prevent="userSettings">
								<span>
									<q-icon icon="reset-password" />
									{{ texts.userSettings }}
								</span>
							</a>
						</li>

						<li class="c-sidebar__list-item">
							<a
								href="javascript:void(0)"
								data-toggle="tooltip"
								data-placement="top"
								class="c-sidebar__list-link"
								:title="texts.leave"
								:tabindex="$attrs.tabindex"
								@click.prevent="logOff">
								<span>
									<q-icon icon="exit" />
									{{ texts.leave }}
								</span>
							</a>
						</li>
					</template>
				</ul>
			</div>
		</div>
	</li>
</template>

<script>
	import { computed } from 'vue'
	import { mapState } from 'pinia'
	import _merge from 'lodash-es/merge'

	import { useSystemDataStore } from '@/stores/systemData.js'
	import { useGenericDataStore } from '@/stores/genericData.js'
	import { fetchData } from '@/api/network'
	import { logOff } from '@/utils/user.js'
	import LayoutHandlers from '@/mixins/layoutHandlers.js'
	import AuthHandlers from '@/mixins/authHandlers.js'
	import hardcodedTexts from '@/hardcodedTexts.js'

	import QRouterLink from '@/views/shared/QRouterLink.vue'

	/**
	 * User avatar, responsible for render the avatar image and the associated dropdown menu
	 */
	export default {
		name: 'UserAvatar',

		inheritAttrs: false,

		components: {
			QRouterLink
		},

		mixins: [
			LayoutHandlers,
			AuthHandlers
		],

		expose: [],

		data()
		{
			return {
				model: {
					Avatar: {},
					UserAvatarMenus: [],
					EPHUserAvatarMenus: []
				},

				texts: {
					options: computed(() => this.Resources[hardcodedTexts.options]),
					alerts: computed(() => this.Resources[hardcodedTexts.alerts]),
					user: computed(() => this.Resources[hardcodedTexts.user]),
					userAvatar: computed(() => this.Resources[hardcodedTexts.userAvatar]),
					userHelp: computed(() => this.Resources[hardcodedTexts.userHelp]),
					userSettings: computed(() => this.Resources[hardcodedTexts.userSettings]),
					leave: computed(() => this.Resources[hardcodedTexts.leave]),
					year: computed(() => this.Resources[hardcodedTexts.year])
				}
			}
		},

		mounted()
		{
			this.fetchMenuEntries()
		},

		computed: {
			...mapState(useSystemDataStore, [
				'system',
				'appAlerts'
			]),

			...mapState(useGenericDataStore, [
				'notifications'
			]),

			fullName()
			{
				if (this.model.Avatar && this.model.Avatar.fullname)
					return this.model.Avatar.fullname
				return this.userData.name
			},

			userRole()
			{
				if (this.model.Avatar && this.model.Avatar.position)
					return this.model.Avatar.position
				return this.texts.user
			},

			avatarSrc()
			{
				if (this.model.Avatar && this.model.Avatar.image)
					return this.model.Avatar.image
				return `${this.system.resourcesPath}user_avatar.png`
			}
		},

		methods: {
			logOff,

			/**
			 * Emits an event to open the alerts tab.
			 */
			openAlert()
			{
				this.$eventHub.emit('open-sidebar-on-tab', 'alerts-tab')
			},

			/**
			 * Clears all the current data.
			 */
			clearModel()
			{
				this.model = {
					Avatar: {},
					UserAvatarMenus: [],
					EPHUserAvatarMenus: []
				}
			},

			/**
			 * Get the necessary data to render the component (Avatar image, Fullname, Position, custom menu list and EPH menu list).
			 */
			fetchMenuEntries()
			{
				if (this.userIsLoggedIn)
				{
					fetchData(
						'Account',
						'UserAvatar',
						{},
						(data) => {
							_merge(this.model, data)

							this.setOpenIdAuth(data.HasOpenIdAuth)
							this.set2FAOptions(data.Has2FAOptions)
						})
				}
				else
					this.clearModel()
			},

			/**
			 * Build the route for avatar custom menu list.
			 */
			getMenuRoute(menu, isPHE)
			{
				if (typeof menu !== 'object')
					menu = {}
				if (typeof isPHE !== 'boolean')
					isPHE = false

				var routeName = 'home'
				if (!this.isEmpty(menu.Action))
					routeName = menu.Action

				const routeParams = {
					name: routeName,
					params: {
						culture: this.system.currentLang,
						system: this.system.currentSystem,
						module: this.system.currentModule
					}
				}

				if (!this.isEmpty(menu.RecordID))
				{
					routeParams.params.id = menu.RecordID
					routeParams.params.mode = 'SHOW'
				}

				if (isPHE)
					routeParams.params.isPopup = true

				return routeParams
			},

			/**
			 * Gets the translation for the given menu title id.
			 */
			getMenuText(menuTitleId)
			{
				return !this.isEmpty(menuTitleId) ? this.Resources[menuTitleId] : ''
			},

			/**
			 * Navigates to the user's profile.
			 */
			userSettings()
			{
				if (this.hasUserSettings)
					this.$router.push({ name: 'profile' })
			}
		},

		watch: {
			'system.currentModule'()
			{
				this.fetchMenuEntries()
			}
		}
	}
</script>
