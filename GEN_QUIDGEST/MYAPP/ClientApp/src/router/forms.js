import { propsConverter } from './routeUtils.js'

export default function getFormsRoutes()
{
	return [
		{
			path: '/:culture/:system/:module/form/AGENTE/:mode/:id?',
			name: 'form-AGENTE',
			props: route => propsConverter(route),
			component: () => import('@/views/forms/FormAgente/QFormAgente.vue'),
			meta: {
				routeType: 'form',
				baseArea: 'AGENT',
				humanKeyFields: ['ValEmail', 'ValNome']
			}
		},
		{
			path: '/:culture/:system/:module/form/CIDADE/:mode/:id?',
			name: 'form-CIDADE',
			props: route => propsConverter(route),
			component: () => import('@/views/forms/FormCidade/QFormCidade.vue'),
			meta: {
				routeType: 'form',
				baseArea: 'CIDAD',
				humanKeyFields: ['ValCidade']
			}
		},
		{
			path: '/:culture/:system/:module/form/CONTACTO/:mode/:id?',
			name: 'form-CONTACTO',
			props: route => propsConverter(route),
			component: () => import('@/views/forms/FormContacto/QFormContacto.vue'),
			meta: {
				routeType: 'form',
				baseArea: 'CONTC',
				humanKeyFields: ['ValCltemail', 'ValCltname']
			}
		},
		{
			path: '/:culture/:system/:module/form/FOTOS/:mode/:id?',
			name: 'form-FOTOS',
			props: route => propsConverter(route),
			component: () => import('@/views/forms/FormFotos/QFormFotos.vue'),
			meta: {
				routeType: 'form',
				baseArea: 'ALBUM',
				humanKeyFields: ['ValTitulo']
			}
		},
		{
			path: '/:culture/:system/:module/form/PAIS/:mode/:id?',
			name: 'form-PAIS',
			props: route => propsConverter(route),
			component: () => import('@/views/forms/FormPais/QFormPais.vue'),
			meta: {
				routeType: 'form',
				baseArea: 'PAIS',
				humanKeyFields: ['ValPais']
			}
		},
		{
			path: '/:culture/:system/:module/form/PROPRIED/:mode/:id?',
			name: 'form-PROPRIED',
			props: route => propsConverter(route),
			component: () => import('@/views/forms/FormPropried/QFormPropried.vue'),
			meta: {
				routeType: 'form',
				baseArea: 'PROPR',
				humanKeyFields: ['ValTitulo']
			}
		},
	]
}
