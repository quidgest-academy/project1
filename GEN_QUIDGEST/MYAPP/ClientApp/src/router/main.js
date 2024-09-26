﻿import { useSystemDataStore } from '@/stores/systemData.js'

export default function getMainRoutes()
{
	const systemDataStore = useSystemDataStore()

	return [
		{
			path: '/:pathMatch(.*)*',
			name: 'main',
			redirect: () => {
				return {
					name: `home-${systemDataStore.system.currentModule}`,
					params: {
						culture: systemDataStore.system.currentLang,
						system: systemDataStore.system.currentSystem,
						module: systemDataStore.system.currentModule
					}
				}
			}
		},
		{
			path: '/OpenIdLogin',
			name: 'OpenId',
			redirect: to => {
				return {
					name: `home-${systemDataStore.system.currentModule}`,
					params: {
						code: to.query.code ? to.query.code : '',
						id_token: to.query.id_token ? to.query.id_token : '',
						culture: to.params.culture ? to.params.culture : systemDataStore.system.currentLang,
						system: to.params.system ? to.params.system : systemDataStore.system.currentSystem,
						module: to.params.module ? to.params.module : systemDataStore.system.currentModule
					}
				}
			}
		},
		{
			path: '/CMDLogin',
			name: 'CMDLog',
			redirect: to => {
				const module = to.params.module ? to.params.module : systemDataStore.system.currentModule
				return {
					name: `home-${module}`,
					params: {
						culture: to.params.culture ? to.params.culture : systemDataStore.system.currentLang,
						system: to.params.system ? to.params.system : systemDataStore.system.currentSystem,
						module: to.params.module ? to.params.module : systemDataStore.system.currentModule
					}
				}
			}
		},
		{
			path: '/CASLogin',
			name: 'CASLog',
			redirect: to => {
				return {
					name: `home-${systemDataStore.system.currentModule}`,
					params: {
						SAMLart: to.query.SAMLart ? to.query.SAMLart : '',
						culture: to.params.culture ? to.params.culture : systemDataStore.system.currentLang,
						system: to.params.system ? to.params.system : systemDataStore.system.currentSystem,
						module: to.params.module ? to.params.module : systemDataStore.system.currentModule
					}
				}
			}
		},
		{
			path: '/:culture/:system/:module/Home',
			name: 'home',
			redirect: to => {
				return {
					name: `home-${systemDataStore.system.currentModule}`,
					params: {
						culture: to.params.culture ? to.params.culture : systemDataStore.system.currentLang,
						system: to.params.system ? to.params.system : systemDataStore.system.currentSystem,
						module: to.params.module ? to.params.module : systemDataStore.system.currentModule
					}
				}
			}
		},
		{
			path: '/:culture/:system/Home',
			name: 'home-Public',
			meta: {
				routeType: 'home',
				module: 'Public',
				hasInitialPHE: false,
				isHomePage: true
			},
			component: () => import('@/views/shared/Home.vue'),
			props: {
				isHomePage: true
			}
		},
		{
			path: '/:culture/:system/PRO/Home',
			name: 'home-PRO',
			meta: {
				routeType: 'home',
				module: 'PRO',
				hasInitialPHE: false,
				isHomePage: true
			},
			component: () => import('@/views/shared/Home.vue'),
			props: {
				isHomePage: true
			}
		},
		{
			path: '/Error',
			name: 'genericError',
			component: () => import('@/views/shared/errors/GenericError.vue'),
			meta: {
				isPublicPage: true,
				noBreadcrumbs: true,
				keepNavigation: true
			}
		},
		{
			path: '/ServerError',
			name: 'serverError',
			component: () => import('@/views/shared/errors/ServerError.vue'),
			meta: {
				isPublicPage: true,
				noBreadcrumbs: true,
				keepNavigation: true
			}
		},
		{
			path: '/NotFound',
			name: 'notFound',
			component: () => import('@/views/shared/errors/NotFoundError.vue'),
			meta: {
				isPublicPage: true,
				noBreadcrumbs: true,
				keepNavigation: true
			}
		},
		{
			path: '/SystemNotFound',
			name: 'systemNotFound',
			component: () => import('@/views/shared/errors/SystemNotFoundError.vue'),
			meta: {
				isPublicPage: true,
				noBreadcrumbs: true,
				keepNavigation: true,
				isFullScreenPage: true
			}
		},
		{
			path: '/PermissionError',
			name: 'permissionError',
			component: () => import('@/views/shared/errors/PermissionError.vue'),
			meta: {
				isPublicPage: true,
				noBreadcrumbs: true,
				keepNavigation: true
			}
		},
		{
			path: '/:culture/:system/:module/SSRS/:id',
			name: 'reporting-services-viewer',
			props: true,
			component: () => import('@/views/shared/ReportingServicesViewer.vue'),
			meta: {
				routeType: 'report'
			}
		}
	]
}
