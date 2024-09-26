<template>
	<div id="q-footer">
<!-- eslint-disable indent, vue/html-indent, vue/script-indent -->
<!-- USE /[MANUAL PRO LAYOUT_FOOTER]/ -->
<!-- eslint-disable-next-line -->
<!-- eslint-enable indent, vue/html-indent, vue/script-indent -->
		<div class="c-footer">
			<div class="row">
				<language-items v-if="layoutConfig.LanguagePlacement === 'in_footer'" />

				<div class="version-container">
					<span class="c-footer__genio-version">
						&copy; {{ genio.generationDate.year }}
						<a
							href="http://www.quidgest.com/"
							target="_blank"
							rel="noopener noreferrer">
							Quidgest
						</a>
						| {{ version }} {{ genio.assemblyVersion }}
					</span>
				</div>
			</div>
		</div>
	</div>
</template>

<script>
	import { defineAsyncComponent } from 'vue'
	import { mapState } from 'pinia'

	import { useSystemDataStore } from '@/stores/systemData.js'
	import hardcodedTexts from '@/hardcodedTexts.js'
	import LayoutHandlers from '@/mixins/layoutHandlers.js'

	export default {
		name: 'QFooter',

		components: {
			LanguageItems: defineAsyncComponent(() => import('@/views/shared/LanguageItems.vue'))
		},

		mixins: [
			LayoutHandlers
		],

		expose: [],

		computed: {
			...mapState(useSystemDataStore, [
				'genio'
			]),

			version()
			{
				return this.Resources[hardcodedTexts.version]
			}
		}
	}
</script>
