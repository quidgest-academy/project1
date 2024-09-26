/* eslint-disable no-unused-vars */
import { computed, reactive } from 'vue'
import _merge from 'lodash-es/merge'

import netAPI from '@/api/network'
/* eslint-enable no-unused-vars */
/**
 * The s_modpro array.
 */
export const QArrayS_modpro = {
	type: 'C',
	pluralName: 'MODOS_DE_PROCESSAMEN07602',
	singularName: 'MODO_DE_PROCESSAMENT14469',
	fnResources: null,
	setResources(fnResources)
	{
		this.fnResources = fnResources
		return this
	},
	get elements()
	{
		// eslint-disable-next-line no-unused-vars
		const vm = this
		return [
			{
				num: 1,
				key: 'INDIV',
				resourceId: 'INDIVIDUAL42893',
				get value() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
			},
			{
				num: 2,
				key: 'global',
				resourceId: 'GLOBAL58588',
				get value() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
			},
			{
				num: 3,
				key: 'unidade',
				resourceId: 'UNIDADE_ORGANICA38383',
				get value() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
			},
			{
				num: 4,
				key: 'horario',
				resourceId: 'HORARIO56549',
				get value() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
			},
		]
	},
}

/**
 * The s_module array.
 */
export const QArrayS_module = {
	type: 'C',
	pluralName: 'MODULES33542',
	singularName: 'MODULE42049',
	fnResources: null,
	setLanguage(lang)
	{
		if (lang)
			this.lang = lang.replace('-', '').toUpperCase()
		return this
	},
	get elements()
	{
		if (!this.array)
		{
			this.array = reactive([])
			netAPI.fetchDynamicArray('S_module', this.lang, (res) => _merge(this.array, res))
		}

		return this.array
	},
}

/**
 * The s_prstat array.
 */
export const QArrayS_prstat = {
	type: 'C',
	pluralName: 'ESTADOS_DO_PROCESSO59118',
	singularName: 'ESTADO_DO_PROCESSO07540',
	fnResources: null,
	setResources(fnResources)
	{
		this.fnResources = fnResources
		return this
	},
	get elements()
	{
		// eslint-disable-next-line no-unused-vars
		const vm = this
		return [
			{
				num: 1,
				key: 'EE',
				resourceId: 'EM_EXECUCAO53706',
				get value() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
			},
			{
				num: 2,
				key: 'FE',
				resourceId: 'EM_FILA_DE_ESPERA21822',
				get value() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
			},
			{
				num: 3,
				key: 'AG',
				resourceId: 'AGENDADO_PARA_EXECUC11223',
				get value() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
			},
			{
				num: 4,
				key: 'T',
				resourceId: 'TERMINADO46276',
				get value() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
			},
			{
				num: 5,
				key: 'C',
				resourceId: 'CANCELADO05982',
				get value() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
			},
			{
				num: 6,
				key: 'NR',
				resourceId: 'NAO_RESPONDE33275',
				get value() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
			},
			{
				num: 7,
				key: 'AB',
				resourceId: 'ABORTADO52378',
				get value() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
			},
			{
				num: 8,
				key: 'AC',
				resourceId: 'A_CANCELAR43988',
				get value() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
			},
		]
	},
}

/**
 * The s_resul array.
 */
export const QArrayS_resul = {
	type: 'C',
	pluralName: 'RESULTADOS20000',
	singularName: 'RESULTADO50955',
	fnResources: null,
	setResources(fnResources)
	{
		this.fnResources = fnResources
		return this
	},
	get elements()
	{
		// eslint-disable-next-line no-unused-vars
		const vm = this
		return [
			{
				num: 1,
				key: 'ok',
				resourceId: 'SUCESSO65230',
				get value() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
			},
			{
				num: 2,
				key: 'er',
				resourceId: 'ERRO38355',
				get value() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
			},
			{
				num: 3,
				key: 'wa',
				resourceId: 'AVISO03237',
				get value() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
			},
			{
				num: 4,
				key: 'c',
				resourceId: 'CANCELADO05982',
				get value() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
			},
		]
	},
}

/**
 * The s_roles array.
 */
export const QArrayS_roles = {
	type: 'C',
	pluralName: 'ROLE60946',
	singularName: 'ROLES61449',
	fnResources: null,
	setLanguage(lang)
	{
		if (lang)
			this.lang = lang.replace('-', '').toUpperCase()
		return this
	},
	get elements()
	{
		if (!this.array)
		{
			this.array = reactive([])
			netAPI.fetchDynamicArray('S_roles', this.lang, (res) => _merge(this.array, res))
		}

		return this.array
	},
}

/**
 * The s_tpproc array.
 */
export const QArrayS_tpproc = {
	type: 'C',
	pluralName: 'PROCESS_TYPES19050',
	singularName: 'PROCESS_TYPE50593',
	fnResources: null,
	setLanguage(lang)
	{
		if (lang)
			this.lang = lang.replace('-', '').toUpperCase()
		return this
	},
	get elements()
	{
		if (!this.array)
		{
			this.array = reactive([])
			netAPI.fetchDynamicArray('S_tpproc', this.lang, (res) => _merge(this.array, res))
		}

		return this.array
	},
}

/**
 * The tipocons array.
 */
export const QArrayTipocons = {
	type: 'C',
	pluralName: 'TIPOS_DE_CONSTRUCOES55880',
	singularName: 'TIPO_DE_CONSTRUCAO35217',
	fnResources: null,
	setResources(fnResources)
	{
		this.fnResources = fnResources
		return this
	},
	get elements()
	{
		// eslint-disable-next-line no-unused-vars
		const vm = this
		return [
			{
				num: 1,
				key: 'a',
				resourceId: 'APARTAMENTO13855',
				get value() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
			},
			{
				num: 2,
				key: 'm',
				resourceId: 'MORADIA65264',
				get value() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
			},
			{
				num: 3,
				key: 'o',
				resourceId: 'OUTRA00632',
				get value() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
			},
		]
	},
}

/**
 * The tipologi array.
 */
export const QArrayTipologi = {
	type: 'N',
	pluralName: 'TIPOLOGIAS19201',
	singularName: 'TIPOLOGIA13928',
	fnResources: null,
	setResources(fnResources)
	{
		this.fnResources = fnResources
		return this
	},
	get elements()
	{
		// eslint-disable-next-line no-unused-vars
		const vm = this
		return [
			{
				num: 1,
				key: 1,
				resourceId: 'T036607',
				get value() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
			},
			{
				num: 2,
				key: 2,
				resourceId: 'T133664',
				get value() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
			},
			{
				num: 3,
				key: 3,
				resourceId: 'T233813',
				get value() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
			},
			{
				num: 4,
				key: 4,
				resourceId: 'T3_OU_MAIOR29810',
				get value() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
			},
		]
	},
}


export default {
	QArrayS_modpro,
	QArrayS_module,
	QArrayS_prstat,
	QArrayS_resul,
	QArrayS_roles,
	QArrayS_tpproc,
	QArrayTipocons,
	QArrayTipologi,
}
