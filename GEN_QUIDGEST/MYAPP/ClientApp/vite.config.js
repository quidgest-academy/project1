import vue from '@vitejs/plugin-vue'
import path from 'path'
import { visualizer } from 'rollup-plugin-visualizer'
import mkcert from 'vite-plugin-mkcert'

import projectPack from './svgstore.config.js'

function svgbundlePlugin()
{
	return {
		name: 'svgbundle-plugin',
		buildStart: projectPack
	}
}

export default ({ mode }) => {
	let commonOptions = {
		base: './',
		root: './',
		resolve: {
			alias: {
				'@': path.resolve(__dirname, './src'),
				'vue-i18n': 'vue-i18n/dist/vue-i18n.cjs.js'
			},
			extensions: ['.mjs', '.js', '.json', '.vue'],
			dedupe: [
				'vue'
			]
		},
		build: {
			target: 'esnext',
			minify: true,
			manifest: false,
			sourcemap: false,
			outDir: 'dist',
			emptyOutDir: true,
			rollupOptions: {
				cache: true,
				// TODO: Why is treeshake disabled?
				treeshake: false,
				output: {
					// We need a balance between chunk size and number of requests
					experimentalMinChunkSize: 100000,
					compact: true,
					manualChunks: {
						ui: [
							'@quidgest/ui'
						],

						vendors: [
							'axios',
							'bootstrap',
							'date-fns',
							'eventemitter3',
							'jquery',
							'jquery-ui',
							'lodash-es',
							'pinia',
							'vue',
							'vue-cookies',
							'vue-i18n',
							'vue-router',
							'uuid'
						],

						mainPageScript: [
							'src/hardcodedTexts.js',
							'src/api/network/index.js',
							'src/api/genio/quidgestFunctions.js',
							'src/api/genio/projectArrays.js',
							'src/api/genio/menuRoutines.js',
							'src/mixins/quidgest.mainEnums.js',
							'src/mixins/layoutHandlers.js',
							'src/mixins/alertHandlers.js'
						],

						mixins: [
							'src/mixins/formHandlers.js',
							'src/mixins/listHandlers.js',
							'src/mixins/listColumnTypes.js',
							'src/mixins/menuAction.js',
							
							'src/mixins/formViewModelBase.js',
							'src/mixins/viewModelBase.js',
						],

						asyncMainPageFiles:
						[
							'src/views/shared/cav/CavContainer.vue',
							'src/views/shared/Footer.vue',
							'src/views/shared/LogOn.vue',
							'src/views/shared/Suggestions.vue',
							'src/views/shared/DebugWindow.vue',
						],

						/**
						 * The main page
						 */
						mainPage: [
							'src/App.vue',
							'src/views/layout/Layout.vue',
							'src/views/shared/QInfoMessageContainer.vue',
							'src/views/shared/Breadcrumbs.vue',
							'src/views/shared/RightSidebar.vue',
							'src/components/QPageBusyState.vue',
							'src/components/inputs/QCookies.vue',
							'src/views/layout/NavigationalBar.vue',
							'src/views/shared/BreadcrumbsContent.vue',
							'src/views/shared/QRouterLink.vue',
							'src/views/shared/Home.vue',
							'src/views/shared/EmbeddedMenu.vue',
						],

						tableControls: [
							'src/components/table/QTable.vue',
							'src/components/table/QTableRecordActionsMenu.vue',
							'src/components/table/QTableSearch.vue',
							'src/components/table/QTableExport.vue',
							'src/components/table/QTableImport.vue',
							'src/components/table/QTableStaticFilters.vue',
							'src/components/table/QTablePagination.vue',
							'src/components/table/QTablePaginationAlt.vue',
							'src/components/table/QTableLimitInfo.vue',
							'src/components/table/QTableChecklistCheckbox.vue',
							'src/components/table/QTableSelector.vue',
							'src/components/table/QTableColumnFilters.vue',
							'src/components/table/QTableActiveFilters.vue',
							'src/components/table/QTableActions.vue',
							'src/components/table/QTableConfig.vue',
							'src/components/table/QTableViews.vue',
							'src/components/table/QTableViewSave.vue',
							'src/components/table/QTableExtraExtension.vue',
							'src/components/table/QTableViewModeConfig.vue',
							'src/components/table/QGridTableList.vue',
							'src/components/table/QTableHeader.vue',
							'src/components/table/QTableFooter.vue',
							'src/components/table/QTableRow.vue',
							'src/components/table/QTreeTableRow.vue'
						],

						containersControls: [
							'src/components/containers/GroupBoxContainer.vue',
							'src/components/containers/QAccordionContainer.vue',
							'src/components/containers/QGroupCollapsible.vue',
							'src/components/containers/RowContainer.vue',
							'src/components/containers/TabContainer.vue',
							'src/components/containers/QModalContainer.vue',
							'src/components/containers/QFormContainer.vue',
							'src/components/containers/wizard/QWizard.vue',
							'src/components/containers/QAnchorContainerHorizontal.vue',
							'src/components/containers/QAnchorContainerVertical.vue',
							'src/components/containers/QAnchorElement.vue'
						],

						/**
						 * Render components are used by tables to display fields.
						 * Edit components are used by tables for advanced filters, column filters 
						 * and editable fields in normal tables 
						 * (different than in the editable table lists).
						 */
						renderingControls: [
							'src/components/rendering/QRenderArray.vue',
							'src/components/rendering/QRenderBoolean.vue',
							'src/components/rendering/QRenderData.vue',
							'src/components/rendering/QRenderHyperlink.vue',
							'src/components/rendering/QRenderHtml.vue',
							'src/components/rendering/QRenderImage.vue',
							'src/components/rendering/QRenderDocument.vue',
							'src/components/rendering/QEditText.vue',
							'src/components/rendering/QEditTextMultiline.vue',
							'src/components/rendering/QEditNumeric.vue',
							'src/components/rendering/QEditBoolean.vue',
							'src/components/rendering/QEditDatetime.vue',
							'src/components/rendering/QEditEnumeration.vue',
							'src/components/rendering/QEditCheckList.vue',
							'src/components/rendering/QEditRadio.vue'
						]
					}
				}
			},
			chunkSizeWarningLimit: 650,
			cssCodeSplit: false,
			reportCompressedSize: false,
			modulePreload: false
		},
		plugins: [
			vue(),
			svgbundlePlugin(),
			visualizer(/*{ open: true, filename: 'bundle-analysis.html' }*/)
		],
		css: {
			preprocessorOptions: {
				scss: {
					quietDeps: true
				}
			}
		}
	}

	if (mode === 'proto')
	{
		const entryHtml = 'index.html'

		commonOptions.server = {
			open: entryHtml
		}
		commonOptions.preview = {
			open: entryHtml
		}
		commonOptions.build.outDir = 'dist_proto'
		commonOptions.build.cacheDir = 'node_modules/.vite-proto'
		commonOptions.build.rollupOptions.input = entryHtml
	}
	else
	{
		commonOptions.server = {
			open: false,
			proxy: {
				'/api': {
					target: 'https://localhost:7015/',
					secure: false
				},
				'/chatbotapi': {
					target: 'https://localhost:7015/',
					secure: false
				},
				'/OpenIdRegister': {
					target: 'https://localhost:7015/',
					secure: false
				},
				'/OpenIdLogin': {
					target: 'https://localhost:7015/',
					secure: false
				}
			},
			port: 5173,
			https: true
		}

		commonOptions.preview = commonOptions.server

		commonOptions.test = {
			globals: true,
			setupFiles: [
				'./tests/matchers/index.js'
			],
			environment: 'jsdom'
		}
	}

	if (mode === 'development')
		commonOptions.plugins.push(mkcert())

	return commonOptions
}
