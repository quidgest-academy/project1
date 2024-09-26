<template>
	<div
		id="modules-title"
		:class="[
			{ 'modules-list-view': layoutConfig.ModulesStyle === 'list' },
			'n-sidebar__title'
		]">
		<q-icon icon="modules" />

		<p style="margin: 0">
			{{ texts.modules }}
		</p>
	</div>
</template>

<script>
	import { computed } from 'vue'

	import hardcodedTexts from '@/hardcodedTexts'
	import LayoutHandlers from '@/mixins/layoutHandlers'

	export default {
		name: 'ModuleHeader',

		mixins: [LayoutHandlers],

		expose: [],

		data()
		{
			return {
				texts: {
					modules: computed(() => this.Resources[hardcodedTexts.modules])
				}
			}
		}
	}
</script>
