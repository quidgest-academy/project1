<template>
	<div
		v-if="isVisible"
		:class="authenticationClasses">
		<div class="f-login">
			<div class="f-login__container">
				<div class="f-login__background">
					<div class="f-login__brand">
						<img
							:src="`${system.resourcesPath}f-login__brand.png`"
							alt="" />
						<p>{{ texts.appName }}</p>
					</div>

					<div
						id="recover-password-container"
						class="form-flow">
						<template v-if="!model.IsEmailSent">
							<p class="q-logon-text">{{ texts.enterEmail }}</p>
							<q-input-group
								size="block"
								:prepend-icon="{ icon: 'envelope' }"
								:class="{ error: emailError && showError }">
								<q-text-field
									v-model="model.Email.value"
									:placeholder="texts.email"
									v-bind="controls.Email"
									@update:model-value="model.Email.fnUpdateValue"
									@input="hideError"
									@keyup.enter="resetPassword" />
							</q-input-group>

							<div 
								v-if="emailError && showError"
								id="error-message"
								class="i-text__error">
								<q-icon icon="exclamation-sign" />
								{{ validationErrors["Email"][0] }}
							</div>
							
							<div class="q-logon-btns-container">
								<q-button
									b-style="secondary"
									block
									id="reset-button"
									class="q-btn--login"
									:label="texts.reset"
									:loading="loading"
									:data-loading="loading"
									:title="texts.reset"
									@click="resetPassword" />
							</div>
						</template>
						<template v-else>
							<p class="q-logon-text">{{ successMessage }}</p>
						</template>

						<div>
							<q-router-link
								class="f-login__link"
								:link="{ 
									name: 'main',
									params: { culture: system.currentLang } 
								}">
								{{ texts.backToLogin }}
							</q-router-link>
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
	import { useAuthDataStore } from '@/stores/authData.js'
	import NavHandlers from '@/mixins/navHandlers.js'
	import VueNavigation from '@/mixins/vueNavigation.js'
	import modelFieldType from '@/mixins/formModelFieldTypes.js'
	import fieldControlClass from '@/mixins/fieldControl.js'
	import genericFunctions from '@/mixins/genericFunctions.js'
	import hardcodedTexts from '@/hardcodedTexts.js'
	import LayoutHandlers from '@/mixins/layoutHandlers.js'

	import QRouterLink from '@/views/shared/QRouterLink.vue'

	export default {
		name: 'RecoverPassword',

		components: {
			QRouterLink
		},

		mixins: [
			NavHandlers,
			VueNavigation,
			LayoutHandlers
		],

		expose: [
			'navigationId'
		],

		data()
		{
			return {
				validationErrors: {},

				showError: false,

				isVisible: false,

				loading: false,

				model: {
					IsEmailSent: false,

					Email: new modelFieldType.String({
						id: 'Email',
						originId: 'Email',
						area: '',
						field: 'EMAIL',
						required: true
					})
				},

				controls: {
					Email: new fieldControlClass.StringControl({
						id: 'Email',
						modelField: 'Email',
						valueChangeEvent: 'fieldChange:email',
						name: 'Email',
						maxLength: 254,
						hasLable: false,
						isRequired: true
					}, this)
				},

				texts: {
					appName: computed(() => this.Resources[hardcodedTexts.appName]),
					enterEmail: computed(() => this.Resources[hardcodedTexts.enterEmail]),
					email: computed(() => this.Resources[hardcodedTexts.email]),
					reset: computed(() => this.Resources[hardcodedTexts.reset]),
					backToLogin: computed(() => this.Resources[hardcodedTexts.backToLogin])
				}
			}
		},

		created()
		{
			if (this.hasPasswordRecovery === false)
				this.navigateToRouteName('main')
			else
			{
				this.isVisible = true
				this.fetchData()
				this.controls.Email.init(true)
			}
		},

		beforeUnmount()
		{
			this.controls.Email.destroy()
		},

		computed: {
			...mapState(useSystemDataStore, [
				'system'
			]),

			...mapState(useAuthDataStore, [
				'hasPasswordRecovery'
			]),

			successMessage()
			{
				return genericFunctions.formatString(this.Resources[hardcodedTexts.passwordRecoverEmail], this.model.Email.value)
			},

			emailError()
			{
				return !this.isEmpty(this.validationErrors) && this.validationErrors["Email"];
			}
		},

		methods: {
			setData(modelValue)
			{
				this.model.IsEmailSent = modelValue.IsEmailSent
				this.model.Email.updateValue(modelValue.Email)
			},

			fetchData()
			{
				return this.netAPI.fetchData('Account', 'RecoverPassword', null, (_, response) => this.setData(response.data.Data))
			},

			hideError()
			{
				this.showError = false
			},

			async resetPassword()
			{	
				this.loading = true
				const params = { 
					Email: this.model.Email.value
				}

				await this.netAPI.postData('Account', 'RecoverPassword', params, this.handleResetPassword)
				this.loading = false
			},

			handleResetPassword(_, response)
			{	
				const data = response.data
				this.showError = !this.isEmpty(data.Errors)
				if(this.showError)
				{
					this.validationErrors = data.Errors
				}
				else {
					this.validationErrors = {}
					this.model.IsEmailSent = data.Data.IsEmailSent
				}
			}
		}
	}
</script>
