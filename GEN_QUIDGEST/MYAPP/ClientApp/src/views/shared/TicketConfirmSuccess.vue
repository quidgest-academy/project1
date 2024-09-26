<template>
	<div class="f-login">
		<div class="f-login__container">
			<div class="f-login__background">
				<div class="f-login__brand">
					<img
						:src="`${system.resourcesPath}f-login__brand.png`"
						alt="" />
					<p>{{ texts.appName }}</p>
					
					<h5>{{ texts.emailConfirmed }}</h5>
				</div>

				<p class="q-logon-text">{{ texts.userCreateSuccess }}</p>

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
</template>

<script>
	import { computed } from 'vue'
	import { mapState } from 'pinia'

	import { useSystemDataStore } from '@/stores/systemData.js'
	import hardcodedTexts from '@/hardcodedTexts.js'

	import QRouterLink from '@/views/shared/QRouterLink.vue'

	export default {
		name: 'TicketConfirmSuccess',

		components: {
			QRouterLink
		},

		expose: [],

		data()
		{
			return {
				texts: {
					appName: computed(() => this.Resources[hardcodedTexts.appName]),
					emailConfirmed: computed(() => this.Resources[hardcodedTexts.emailConfirmed]),
					userCreateSuccess: computed(() => this.Resources[hardcodedTexts.userCreateSuccess]),
					backToLogin: computed(() => this.Resources[hardcodedTexts.backToLogin])
				}
			}
		},

		computed: {
			...mapState(useSystemDataStore, [
				'system'
			])
		}
	}
</script>
