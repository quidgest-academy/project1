// eslint-disable-next-line no-unused-vars
import { updateQueryParams } from './routeUtils.js'

export default function getMenusRoutes()
{
	return [
		{
			path: '/:culture/:system/PRO/menu/PRO_311',
			name: 'menu-PRO_311',
			component: () => import('@/views/menus/ModulePRO/MenuPRO_311/QMenuPro311.vue'),
			meta: {
				routeType: 'menu',
				module: 'PRO',
				order: '311',
				baseArea: 'PAIS',
				hasInitialPHE: false,
				humanKeyFields: ['ValPais'],
			}
		},
		{
			path: '/:culture/:system/PRO/menu/PRO_321',
			name: 'menu-PRO_321',
			component: () => import('@/views/menus/ModulePRO/MenuPRO_321/QMenuPro321.vue'),
			meta: {
				routeType: 'menu',
				module: 'PRO',
				order: '321',
				baseArea: 'CIDAD',
				hasInitialPHE: false,
				humanKeyFields: ['ValCidade'],
			}
		},
		{
			path: '/:culture/:system/PRO/menu/PRO_21',
			name: 'menu-PRO_21',
			component: () => import('@/views/menus/ModulePRO/MenuPRO_21/QMenuPro21.vue'),
			meta: {
				routeType: 'menu',
				module: 'PRO',
				order: '21',
				baseArea: 'PROPR',
				hasInitialPHE: false,
				humanKeyFields: ['ValTitulo'],
			}
		},
		{
			path: '/:culture/:system/PRO/menu/PRO_411',
			name: 'menu-PRO_411',
			component: () => import('@/views/menus/ModulePRO/MenuPRO_411/QMenuPro411.vue'),
			beforeEnter: [updateQueryParams],
			meta: {
				routeType: 'menu',
				module: 'PRO',
				order: '411',
				baseArea: 'PROPR',
				hasInitialPHE: false,
				humanKeyFields: ['ValTitulo'],
				limitations: ['agent' /* DB */]
			}
		},
		{
			path: '/:culture/:system/PRO/menu/PRO_511',
			name: 'menu-PRO_511',
			component: () => import('@/views/menus/ModulePRO/MenuPRO_511/QMenuPro511.vue'),
			meta: {
				routeType: 'menu',
				module: 'PRO',
				order: '511',
				baseArea: 'PROPR',
				hasInitialPHE: false,
				humanKeyFields: ['ValTitulo'],
			}
		},
		{
			path: '/:culture/:system/PRO/menu/PRO_11',
			name: 'menu-PRO_11',
			component: () => import('@/views/menus/ModulePRO/MenuPRO_11/QMenuPro11.vue'),
			meta: {
				routeType: 'menu',
				module: 'PRO',
				order: '11',
				baseArea: 'AGENT',
				hasInitialPHE: false,
				humanKeyFields: ['ValEmail', 'ValNome'],
			}
		},
		{
			path: '/:culture/:system/PRO/menu/PRO_41',
			name: 'menu-PRO_41',
			component: () => import('@/views/menus/ModulePRO/MenuPRO_41/QMenuPro41.vue'),
			meta: {
				routeType: 'menu',
				module: 'PRO',
				order: '41',
				baseArea: 'AGENT',
				hasInitialPHE: false,
				humanKeyFields: ['ValEmail', 'ValNome'],
			}
		},
	]
}
