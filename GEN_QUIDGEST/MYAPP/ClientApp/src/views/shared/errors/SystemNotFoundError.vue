<template>
	<error
		image-name="not-found.png"
		:error-title="errorTitle"
		:back-button-visible="false" />
</template>

<script>
	import hardcodedTexts from '@/hardcodedTexts.js'

	import Error from './Error.vue'

	export default {
		name: 'QSystemNotFoundError',

		components: {
			Error
		},

		expose: [],

		computed: {
			errorTitle()
			{
				return this.Resources[hardcodedTexts.systemNotFound]
			}
		}
	}
</script>
