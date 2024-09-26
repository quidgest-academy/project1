/* eslint-disable no-unused-vars */
import { computed, reactive, watch } from 'vue'
import _merge from 'lodash-es/merge'

import ViewModelBase from '@/mixins/formViewModelBase.js'
import genericFunctions from '@/mixins/genericFunctions.js'
import modelFieldType from '@/mixins/formModelFieldTypes.js'

import hardcodedTexts from '@/hardcodedTexts.js'
import netAPI from '@/api/network'
import qApi from '@/api/genio/quidgestFunctions.js'
import qFunctions from '@/api/genio/projectFunctions.js'
import qProjArrays from '@/api/genio/projectArrays.js'
/* eslint-enable no-unused-vars */

/**
 * Represents a ViewModel class.
 * @extends ViewModelBase
 */
export default class ViewModel extends ViewModelBase
{
	/**
	 * Creates a new instance of the ViewModel.
	 * @param {object} vueContext - The Vue context
	 * @param {object} options - The options for the ViewModel
	 * @param {object} values - A ViewModel instance to copy values from
	 */
	// eslint-disable-next-line no-unused-vars
	constructor(vueContext, options, values)
	{
		super(vueContext, options)
		// eslint-disable-next-line no-unused-vars
		const vm = this.vueContext

		/** The view model metadata */
		_merge(this.modelInfo, {
			name: 'PROPRIED',
			area: 'PROPR',
			actions: {
				recalculateFormulas: 'RecalculateFormulas_PROPRIED'
			}
		})

		/** The primary key. */
		this.ValCodpropr = reactive(new modelFieldType.PrimaryKey({
			id: 'ValCodpropr',
			originId: 'ValCodpropr',
			area: 'PROPR',
			field: 'CODPROPR',
			description: '',
		}).cloneFrom(values?.ValCodpropr))
		watch(() => this.ValCodpropr.value, (newValue, oldValue) => this.onUpdate('propr.codpropr', this.ValCodpropr, newValue, oldValue))

		/** The used foreign keys. */
		this.ValCodcidad = reactive(new modelFieldType.ForeignKey({
			id: 'ValCodcidad',
			originId: 'ValCodcidad',
			area: 'PROPR',
			field: 'CODCIDAD',
			relatedArea: 'CIDAD',
			description: '',
		}).cloneFrom(values?.ValCodcidad))
		watch(() => this.ValCodcidad.value, (newValue, oldValue) => this.onUpdate('propr.codcidad', this.ValCodcidad, newValue, oldValue))

		this.ValCodagent = reactive(new modelFieldType.ForeignKey({
			id: 'ValCodagent',
			originId: 'ValCodagent',
			area: 'PROPR',
			field: 'CODAGENT',
			relatedArea: 'AGENT',
			description: '',
		}).cloneFrom(values?.ValCodagent))
		watch(() => this.ValCodagent.value, (newValue, oldValue) => this.onUpdate('propr.codagent', this.ValCodagent, newValue, oldValue))

		/** The remaining form fields. */
		this.ValIdpropre = reactive(new modelFieldType.Number({
			id: 'ValIdpropre',
			originId: 'ValIdpropre',
			area: 'PROPR',
			field: 'IDPROPRE',
			maxDigits: 10,
			decimalDigits: 0,
			description: computed(() => this.Resources.ID48520),
			isFixed: true,
		}).cloneFrom(values?.ValIdpropre))
		watch(() => this.ValIdpropre.value, (newValue, oldValue) => this.onUpdate('propr.idpropre', this.ValIdpropre, newValue, oldValue))

		this.ValVendida = reactive(new modelFieldType.Boolean({
			id: 'ValVendida',
			originId: 'ValVendida',
			area: 'PROPR',
			field: 'VENDIDA',
			description: computed(() => this.Resources.VENDIDA08366),
		}).cloneFrom(values?.ValVendida))
		watch(() => this.ValVendida.value, (newValue, oldValue) => this.onUpdate('propr.vendida', this.ValVendida, newValue, oldValue))

		this.ValFoto = reactive(new modelFieldType.Image({
			id: 'ValFoto',
			originId: 'ValFoto',
			area: 'PROPR',
			field: 'FOTO',
			description: computed(() => this.Resources.FOTOGRAFIA36807),
		}).cloneFrom(values?.ValFoto))
		watch(() => this.ValFoto.value, (newValue, oldValue) => this.onUpdate('propr.foto', this.ValFoto, newValue, oldValue))

		this.ValTitulo = reactive(new modelFieldType.String({
			id: 'ValTitulo',
			originId: 'ValTitulo',
			area: 'PROPR',
			field: 'TITULO',
			maxLength: 80,
			description: computed(() => this.Resources.TITULO39021),
		}).cloneFrom(values?.ValTitulo))
		watch(() => this.ValTitulo.value, (newValue, oldValue) => this.onUpdate('propr.titulo', this.ValTitulo, newValue, oldValue))

		this.ValPreco = reactive(new modelFieldType.Number({
			id: 'ValPreco',
			originId: 'ValPreco',
			area: 'PROPR',
			field: 'PRECO',
			maxDigits: 5,
			decimalDigits: 4,
			description: computed(() => this.Resources.PRECO50007),
		}).cloneFrom(values?.ValPreco))
		watch(() => this.ValPreco.value, (newValue, oldValue) => this.onUpdate('propr.preco', this.ValPreco, newValue, oldValue))

		this.ValDescrica = reactive(new modelFieldType.MultiLineString({
			id: 'ValDescrica',
			originId: 'ValDescrica',
			area: 'PROPR',
			field: 'DESCRICA',
			description: computed(() => this.Resources.DESCRICAO07528),
		}).cloneFrom(values?.ValDescrica))
		watch(() => this.ValDescrica.value, (newValue, oldValue) => this.onUpdate('propr.descrica', this.ValDescrica, newValue, oldValue))

		this.TableCidadCidade = reactive(new modelFieldType.String({
			type: 'Lookup',
			id: 'TableCidadCidade',
			originId: 'ValCidade',
			area: 'CIDAD',
			field: 'CIDADE',
			maxLength: 50,
			description: computed(() => this.Resources.CIDADE42080),
		}).cloneFrom(values?.TableCidadCidade))
		watch(() => this.TableCidadCidade.value, (newValue, oldValue) => this.onUpdate('cidad.cidade', this.TableCidadCidade, newValue, oldValue))

		this.CidadPaisValPais = reactive(new modelFieldType.String({
			id: 'CidadPaisValPais',
			originId: 'ValPais',
			area: 'PAIS',
			field: 'PAIS',
			maxLength: 50,
			description: computed(() => this.Resources.PAIS58483),
			isFixed: true,
		}).cloneFrom(values?.CidadPaisValPais))
		watch(() => this.CidadPaisValPais.value, (newValue, oldValue) => this.onUpdate('pais.pais', this.CidadPaisValPais, newValue, oldValue))

		this.ValLocaliza = reactive(new modelFieldType.Coordinate({
			id: 'ValLocaliza',
			originId: 'ValLocaliza',
			area: 'PROPR',
			field: 'LOCALIZA',
			description: computed(() => this.Resources.LOCALIZACAO54665),
		}).cloneFrom(values?.ValLocaliza))
		watch(() => this.ValLocaliza.value, (newValue, oldValue) => this.onUpdate('propr.localiza', this.ValLocaliza, newValue, oldValue))

		this.ValTipoprop = reactive(new modelFieldType.String({
			id: 'ValTipoprop',
			originId: 'ValTipoprop',
			area: 'PROPR',
			field: 'TIPOPROP',
			arrayOptions: qProjArrays.QArrayTipocons.setResources(vm.$getResource).elements,
			maxLength: 1,
			description: computed(() => this.Resources.TIPO_DE_CONSTRUCAO35217),
		}).cloneFrom(values?.ValTipoprop))
		watch(() => this.ValTipoprop.value, (newValue, oldValue) => this.onUpdate('propr.tipoprop', this.ValTipoprop, newValue, oldValue))

		this.ValEspexter = reactive(new modelFieldType.Number({
			id: 'ValEspexter',
			originId: 'ValEspexter',
			area: 'PROPR',
			field: 'ESPEXTER',
			maxDigits: 4,
			decimalDigits: 2,
			description: computed(() => this.Resources.ESPACO_EXTERIOR__M2_04786),
			showWhen: {
				// eslint-disable-next-line no-unused-vars
				fnFormula(params)
				{
					// Formula: [PROPR->TIPOPROP]=="m"
					// eslint-disable-next-line eqeqeq
					return this.ValTipoprop.value=="m"
				},
				dependencyEvents: ['fieldChange:propr.tipoprop'],
				isServerRecalc: false,
				isEmpty: qApi.emptyN,
			},
		}).cloneFrom(values?.ValEspexter))
		watch(() => this.ValEspexter.value, (newValue, oldValue) => this.onUpdate('propr.espexter', this.ValEspexter, newValue, oldValue))

		this.ValTipologi = reactive(new modelFieldType.Number({
			id: 'ValTipologi',
			originId: 'ValTipologi',
			area: 'PROPR',
			field: 'TIPOLOGI',
			arrayOptions: qProjArrays.QArrayTipologi.setResources(vm.$getResource).elements,
			maxDigits: 1,
			decimalDigits: 0,
			description: computed(() => this.Resources.TIPOLOGIA13928),
		}).cloneFrom(values?.ValTipologi))
		watch(() => this.ValTipologi.value, (newValue, oldValue) => this.onUpdate('propr.tipologi', this.ValTipologi, newValue, oldValue))

		this.ValTamanho = reactive(new modelFieldType.Number({
			id: 'ValTamanho',
			originId: 'ValTamanho',
			area: 'PROPR',
			field: 'TAMANHO',
			maxDigits: 3,
			decimalDigits: 2,
			description: computed(() => this.Resources.TAMANHO__M2_40951),
		}).cloneFrom(values?.ValTamanho))
		watch(() => this.ValTamanho.value, (newValue, oldValue) => this.onUpdate('propr.tamanho', this.ValTamanho, newValue, oldValue))

		this.ValNr_wcs = reactive(new modelFieldType.Number({
			id: 'ValNr_wcs',
			originId: 'ValNr_wcs',
			area: 'PROPR',
			field: 'NR_WCS',
			maxDigits: 3,
			decimalDigits: 0,
			description: computed(() => this.Resources.NUMERO_DE_CASAS_DE_B39783),
		}).cloneFrom(values?.ValNr_wcs))
		watch(() => this.ValNr_wcs.value, (newValue, oldValue) => this.onUpdate('propr.nr_wcs', this.ValNr_wcs, newValue, oldValue))

		this.ValDtconst = reactive(new modelFieldType.Date({
			id: 'ValDtconst',
			originId: 'ValDtconst',
			area: 'PROPR',
			field: 'DTCONST',
			description: computed(() => this.Resources.DATA_DE_CONTRUCAO03489),
		}).cloneFrom(values?.ValDtconst))
		watch(() => this.ValDtconst.value, (newValue, oldValue) => this.onUpdate('propr.dtconst', this.ValDtconst, newValue, oldValue))

		this.ValIdadepro = reactive(new modelFieldType.Number({
			id: 'ValIdadepro',
			originId: 'ValIdadepro',
			area: 'PROPR',
			field: 'IDADEPRO',
			maxDigits: 4,
			decimalDigits: 0,
			description: computed(() => this.Resources.IDADE_DA_CONSTRUCAO37805),
			isFixed: true,
			valueFormula: {
				stopRecalcCondition() { return false },
				// eslint-disable-next-line no-unused-vars
				fnFormula(params)
				{
					// Formula: Year([Today]) - Year([PROPR->DTCONST])
					// eslint-disable-next-line eqeqeq
					return qApi.Year(qApi.Hoje())-qApi.Year(this.ValDtconst.value)
				},
				dependencyEvents: ['fieldChange:propr.dtconst'],
				isServerRecalc: false,
				isEmpty: qApi.emptyN,
			},
		}).cloneFrom(values?.ValIdadepro))
		watch(() => this.ValIdadepro.value, (newValue, oldValue) => this.onUpdate('propr.idadepro', this.ValIdadepro, newValue, oldValue))

		this.TableAgentNome = reactive(new modelFieldType.String({
			type: 'Lookup',
			id: 'TableAgentNome',
			originId: 'ValNome',
			area: 'AGENT',
			field: 'NOME',
			maxLength: 80,
			description: computed(() => this.Resources.NOME47814),
		}).cloneFrom(values?.TableAgentNome))
		watch(() => this.TableAgentNome.value, (newValue, oldValue) => this.onUpdate('agent.nome', this.TableAgentNome, newValue, oldValue))

		this.AgentValFoto = reactive(new modelFieldType.Image({
			id: 'AgentValFoto',
			originId: 'ValFoto',
			area: 'AGENT',
			field: 'FOTO',
			description: computed(() => this.Resources.FOTOGRAFIA36807),
			isFixed: true,
		}).cloneFrom(values?.AgentValFoto))
		watch(() => this.AgentValFoto.value, (newValue, oldValue) => this.onUpdate('agent.foto', this.AgentValFoto, newValue, oldValue))

		this.AgentValEmail = reactive(new modelFieldType.String({
			id: 'AgentValEmail',
			originId: 'ValEmail',
			area: 'AGENT',
			field: 'EMAIL',
			maxLength: 80,
			description: computed(() => this.Resources.E_MAIL42251),
			isFixed: true,
		}).cloneFrom(values?.AgentValEmail))
		watch(() => this.AgentValEmail.value, (newValue, oldValue) => this.onUpdate('agent.email', this.AgentValEmail, newValue, oldValue))
	}

	/**
	 * Creates a clone of the current QFormPropriedViewModel instance.
	 * @returns {QFormPropriedViewModel} A new instance of QFormPropriedViewModel
	 */
	clone()
	{
		return new ViewModel(this.vueContext, { callbacks: this.externalCallbacks }, this)
	}

	static QPrimaryKeyName = 'ValCodpropr'

	get QPrimaryKey() { return this.ValCodpropr.value }
	set QPrimaryKey(value) { this.ValCodpropr.updateValue(value) }
}
