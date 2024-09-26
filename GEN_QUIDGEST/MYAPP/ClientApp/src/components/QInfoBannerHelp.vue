<template>
	<div class="q-help__info-banner">
		<div class="q-help__info-banner-header">
			<q-icon icon="help" />
		</div>
		<div
			v-if="isHtml"
			v-html="infoBannerText"
			class="q-help__info-banner-text"></div>
		<div v-else>{{ infoBannerText }}</div>
	</div>
</template>

<script>
	export default {
		name: 'InfoBannerHelp',

		expose: [],

		props: {
			/**
			 * The help object.
			 */
			helpControl: {
				type: Object
			},

			/**
			 * Whether the help content is HTML or not. If not, it is plain text.
			 */
			isHtml: {
				type: Boolean,
				default: true
			}
		},

		computed: {
			infoBannerText() {
				if(this.helpControl.shortHelp.text && this.helpControl.shortHelp.type === 'Info-banner')
				{
					return this.helpControl.shortHelp.text
				}
				else if (this.helpControl.detailedHelp.text && this.helpControl.detailedHelp.type === 'Info-banner')
				{
					return this.helpControl.detailedHelp.text
				}
				return ""
			}
		}
	}
</script>
