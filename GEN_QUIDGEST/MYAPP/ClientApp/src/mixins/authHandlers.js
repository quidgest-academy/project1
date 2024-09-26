import { mapState, mapActions } from 'pinia'

import { useAuthDataStore } from '@/stores/authData.js'
import { useUserDataStore } from '@/stores/userData.js'
import { fetchData } from '@/api/network'

/***************************************************************************
 * This mixin defines methods to be reused in authentication components.   *
 ***************************************************************************/
export default {
	computed: {
		...mapState(useAuthDataStore, [
			'hasPasswordRecovery',
			'hasUsernameAuth',
			'hasUserSettings'
		]),

		userData()
		{
			const userDataStore = useUserDataStore()

			return {
				name: userDataStore.username
			}
		}
	},

	methods: {
		...mapActions(useUserDataStore, [
			'setUserData'
		]),

		...mapActions(useAuthDataStore, [
			'setPasswordRecovery',
			'setUsernameAuth',
			'setOpenIdAuth',
			'set2FAOptions'
		]),

		OpenIdConnAuthButtonClick(id)
		{
			fetchData('Account', 'OpenIdLoginRedirect', { id }, this.OpenIdConnAuthRedirect)
		},

		OpenIdConnAuthRedirect(data)
		{
			window.location.href = data.redirectUrl
		},

		CASAuthButtonClick(id)
		{
			fetchData('Account', 'CASLoginRedirect', { id }, this.CASAuthRedirect)
		},

		CASAuthRedirect(data)
		{
			window.location.href = data.redirectUrl
		},

		CMDAuthButtonClick(id)
		{
			fetchData('Account', 'CMDLoginRedirect', { id }, this.CMDAuthRedirect)
		},

		CMDAuthRedirect(data)
		{
			window.location.href = data.redirectUrl
		}
	}
}
