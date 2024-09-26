import { defineAsyncComponent } from 'vue'

export default {
	install: (app) => {
		app.component('QFormAccountInfo', defineAsyncComponent(() => import('@/views/shared/AccountInfo.vue')))
		app.component('QFormAgente', defineAsyncComponent(() => import('@/views/forms/FormAgente/QFormAgente.vue')))
		app.component('QFormCidade', defineAsyncComponent(() => import('@/views/forms/FormCidade/QFormCidade.vue')))
		app.component('QFormContacto', defineAsyncComponent(() => import('@/views/forms/FormContacto/QFormContacto.vue')))
		app.component('QFormFotos', defineAsyncComponent(() => import('@/views/forms/FormFotos/QFormFotos.vue')))
		app.component('QFormPais', defineAsyncComponent(() => import('@/views/forms/FormPais/QFormPais.vue')))
		app.component('QFormPropried', defineAsyncComponent(() => import('@/views/forms/FormPropried/QFormPropried.vue')))
	}
}
