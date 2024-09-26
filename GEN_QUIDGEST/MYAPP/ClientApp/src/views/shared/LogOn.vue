<template>
	<div
		v-if="isVisible"
		ref="logonMenu"
		:class="logonClasses">
		<div class="f-login">
			<div class="f-login__container">
				<div class="f-login__background">
					<div class="f-login__brand">
						<img
							:src="`${system.resourcesPath}f-login__brand.png`"
							:alt="texts.enter" />
						<p>{{ texts.appName }}</p>
					</div>

					<p class="q-logon-text">{{ texts.authentication }}</p>

					<div
						id="login-container"
						class="form-flow">
						<div
							v-for="value in model.OpenIdConnAuthMethods"
							:key="value">
							<q-button
								b-style="primary"
								block
								class="q-btn--login"
								:title="value ?? 'OpenId Connect Auth'"
								:label="value ?? 'OpenId Connect Auth'"
								:loading="loading"
								@click="OpenIdConnAuthButtonClick(value)" />
						</div>

						<div
							v-for="value in model.CASAuthMethods"
							:key="value">
							<q-button
								b-style="primary"
								block
								class="q-btn--login"
								:title="value ?? 'CAS Protocol'"
								:label="value ?? 'CAS Protocol'"
								:loading="loading"
								@click="CASAuthButtonClick(value)" />
						</div>

						<div
							v-for="value in model.CMDAuthMethods"
							:key="value">
							<q-button
								b-style="primary"
								block
								class="q-btn--login"
								:title="value ?? 'CMD Protocol'"
								:label="value ?? 'CMD Protocol'"
								:loading="loading"
								@click="CMDAuthButtonClick(value)" />
						</div>

						<template v-if="hasUsernameAuth">
							<hr
								v-if="
									!isEmpty(model.OpenIdConnAuthMethods) || !isEmpty(model.CASAuthMethods) || !isEmpty(model.CMDAuthMethods)
								" />

							<q-input-group
								size="block"
								:prepend-icon="{ icon: 'user' }"
								:class="{ error: userError }">
								<q-text-field
									v-model="currentUser"
									name="username"
									:placeholder="texts.user"
									@keyup.enter="executeLogon"
									@input="hideUserError" />
							</q-input-group>

							<div
								v-if="userError"
								id="user-error"
								class="i-text__error">
								<q-icon icon="exclamation-sign" />
								{{ returnMessage.UserName[0] }}
							</div>

							<q-password-input
								size="block"
								:classes="[{ error: passError }]"
								:model-value="password"
								name="password"
								:placeholder="texts.password"
								@update:model-value="updatePasswordValue"
								@keyup-enter="executeLogon"
								:readonly="!layoutConfig.ShowPasswordToggle">
								<template #prepend>
									<span>
										<q-icon icon="lock" />
									</span>
								</template>
							</q-password-input>

							<div
								v-if="errorMessage"
								id="error-message"
								class="i-text__error">
								<q-icon icon="exclamation-sign" />
								{{ errorMessage }}
							</div>

							<div class="q-logon-btns-container">
								<q-button
									id="login-btn"
									b-style="secondary"
									block
									borderless
									class="q-btn--login"
									:title="texts.enter"
									:label="texts.enter"
									:loading="loading"
									:data-loading="loading"
									@click="executeLogon" />

								<q-register-button
									v-if="allowRegistration && layoutConfig.UserRegisterStyle === 'button'"
									:registration-types="userRegistration.registrationTypes"
									:display-style="layoutConfig.UserRegisterStyle"
									@navigate-to-register-route="navigateToRegisterRoute" />
							</div>
						</template>

						<div
							class="q-logon-links-container"
							v-if="hasPasswordRecovery || allowRegistration">
							<q-router-link
								v-if="hasPasswordRecovery"
								id="forgot-password"
								class="f-login__link"
								:link="{
									name: 'password-recovery',
									params: { culture: system.currentLang }
								}">
								{{ texts.forgotPassword }}
							</q-router-link>

							<q-register-button
								v-if="allowRegistration && layoutConfig.UserRegisterStyle === 'hyperlink'"
								:registration-types="userRegistration.registrationTypes"
								:display-style="layoutConfig.UserRegisterStyle" />
						</div>
					</div>
				</div>
			</div>
		</div>
	</div>
</template>

<script>
	import { computed } from 'vue'
	import { mapState } from 'pinia'

	import { useSystemDataStore } from '@/stores/systemData.js'
	import { postData, fetchData } from '@/api/network'
	import mainConfigUtils from '@/api/global/mainConfigUtils.js'
	import { displayMessage, resetStoreState } from '@/mixins/genericFunctions.js'
	import LayoutHandlers from '@/mixins/layoutHandlers.js'
	import AuthHandlers from '@/mixins/authHandlers.js'
	import VueNavigation from '@/mixins/vueNavigation.js'
	import hardcodedTexts from '@/hardcodedTexts.js'

	import QRouterLink from '@/views/shared/QRouterLink.vue'
	import QRegisterButton from '@/views/shared/RegisterButton.vue'

	export default {
		name: 'LogOn',

		emits: ['set-visibility'],

		components: {
			QRouterLink,
			QRegisterButton
		},

		mixins: [
			VueNavigation,
			LayoutHandlers,
			AuthHandlers
		],

		props: {
			/**
			 * Whether or not the control is currently visible.
			 */
			isVisible: {
				type: Boolean,
				default: true
			}
		},

		expose: [],

		data()
		{
			return {
				currentUser: '',

				password: '',

				returnMessage: '',

				isPasswordVisible: false,

				showReturnMessage: false,

				loading: false,

				model: {
					CASAuthMethods: [],
					CMDAuthMethods: [],
					OpenIdConnAuthMethods: []
				},

				texts: {
					appName: computed(() => this.Resources[hardcodedTexts.appName]),
					user: computed(() => this.Resources[hardcodedTexts.user]),
					enter: computed(() => this.Resources[hardcodedTexts.enter]),
					authentication: computed(() => this.Resources[hardcodedTexts.authentication]),
					register: computed(() => this.Resources[hardcodedTexts.register]),
					password: computed(() => this.Resources[hardcodedTexts.password]),
					forgotPassword: computed(() => this.Resources[hardcodedTexts.forgotPassword])
				}
			}
		},

		created()
		{
			fetchData('Account', 'LogOn', {}, this.loadedContent)
		},

		mounted()
		{
			if (
				this.layoutConfig.LoginStyle === 'embeded_page' ||
				(this.isPublicRoute && !this.isFullScreenPage)
			)
				window.addEventListener('mousedown', this.hideLogon)
		},

		beforeUnmount()
		{
			if (
				this.layoutConfig.LoginStyle === 'embeded_page' ||
				(this.isPublicRoute && !this.isFullScreenPage)
			)
				window.removeEventListener('mousedown', this.hideLogon)
		},

		computed: {
			...mapState(useSystemDataStore, ['userRegistration']),

			passwordFieldType()
			{
				return this.isPasswordVisible ? 'text' : 'password'
			},

			eyeIcon()
			{
				return this.isPasswordVisible ? 'password-hidden' : 'view'
			},

			userError()
			{
				return this.returnMessage && this.returnMessage.UserName
			},

			passError()
			{
				return this.returnMessage && this.returnMessage.Password
			},

			hasError()
			{
				return this.returnMessage && this.returnMessage.Error
			},

			errorMessage()
			{
				if (this.passError)
					return this.returnMessage.Password[0]
				else if (this.hasError)
					return this.returnMessage.Error[0]
				return undefined
			},

			allowRegistration()
			{
				return this.userRegistration.allowRegistration && this.userRegistration.registrationTypes.length > 0
			},

			logonClasses()
			{
				const classes = ['d-block']

				if (
					this.layoutConfig.LoginStyle === 'embeded_page' ||
					(this.isPublicRoute && !this.isFullScreenPage)
				)
				{
					classes.push('log-on-container')
					classes.push('dropdown-menu')
					classes.push('dropdown-menu-right')
					classes.push('c-user__dropdown')
				}
				classes.push(...this.authenticationClasses)

				return classes
			}
		},

		methods: {
			async executeLogon()
			{
				if (this.loading)
					return

				this.loading = true

				const params = {
					returnUrl: '',
					userName: this.currentUser,
					password: this.password
				}
				await postData('Account', 'LogOn', params, this.loginSuccess)

				this.loading = false
			},

			loginSuccess(_, response)
			{
				const responseData = response.data
				this.showReturnMessage = true
				this.returnMessage = responseData.Errors

				if (responseData.Success)
				{
					if (responseData.Auth2FA && !responseData.Val2FA)
					{
						if (responseData.User.Auth2FATp === 'TOTP')
							this.confirmBox2FA()
						else if (responseData.User.Auth2FATp === 'WebAuth')
							this.handleSignInWebAuth(responseData)
					}
					else
						this.finalizeLogin()
				}
				else if (this.password.length > 0)
					this.password = ''
			},

			finalizeLogin()
			{
				resetStoreState()

				Promise.all([
					mainConfigUtils.updateAFToken(),
					mainConfigUtils.updateMainConfig()
				]).then(() => {
					const userData = {
						Name: this.currentUser
					}
					this.setUserData(userData)

					const routeParams = {
						name: 'home',
						params: {
							culture: this.system.defaultLang,
							system: this.system.defaultSystem,
							module: this.system.defaultModule
						}
					}
					this.$router.push(routeParams)
				})
			},

			clearReturnMessage()
			{
				this.showReturnMessage = false
			},

			confirmBox2FA()
			{
				const buttons = {
					confirm: {
						label: this.Resources[hardcodedTexts.confirm],
						action: (value) => {
							const params = {
								returnUrl: '',
								userName: this.currentUser,
								password: value
							}

							postData('Account', 'Authentication2FA', params, this.finalizeLogin)
						}
					},
					cancel: {
						label: this.Resources[hardcodedTexts.cancel]
					}
				}

				displayMessage(
					this.Resources[hardcodedTexts.enter6DigitCode],
					'question',
					null,
					buttons, {
						input: {
							type: 'text',
							placeholder: '000000',
							validator: (value) => value?.length !== 6 ? this.Resources[hardcodedTexts.invalidCode] : ''
						}
					})
			},

			handleSignInWebAuth()
			{
				// TODO
			},

			showPassword()
			{
				this.isPasswordVisible = true
			},

			hidePassword()
			{
				this.isPasswordVisible = false
			},

			setLogonVisibility(isVisible)
			{
				this.$emit('set-visibility', isVisible)
			},

			hideUserError()
			{
				delete this.returnMessage.UserName
			},

			hideLogon(event)
			{
				if (!this.isVisible)
					return

				let el = this.$refs.logonMenu
				let target = event.target

				if (el && el !== target && !el.contains(target))
					this.setLogonVisibility(false)
			},

			loadedContent(data)
			{
				if (this.isEmpty(data))
					return

				// Update the store data
				this.setPasswordRecovery(data.HasPasswordRecovery)
				this.setUsernameAuth(data.HasUsernameAuth)
				this.setOpenIdAuth(data.OpenIdConnAuthMethods?.length > 0)

				this.model.OpenIdConnAuthMethods = data.OpenIdConnAuthMethods
				this.model.CMDAuthMethods = data.CMDAuthMethods
				this.model.CASAuthMethods = data.CASAuthMethods
			},

			navigateToRegisterRoute()
			{
				const params = {
					id: this.userRegistration.registrationTypes[0].id
				}

				this.navigateToRouteName('user-register', params)
			},

			updatePasswordValue(newVal)
			{
				// When the user starts typing hide the error message
				delete this.returnMessage.Password
				this.password = newVal
			}
		}
	}
</script>
