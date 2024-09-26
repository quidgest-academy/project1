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
			name: 'CONTACTO',
			area: 'CONTC',
			actions: {
				recalculateFormulas: 'RecalculateFormulas_CONTACTO'
			}
		})

		/** The primary key. */
		this.ValCodcontc = reactive(new modelFieldType.PrimaryKey({
			id: 'ValCodcontc',
			originId: 'ValCodcontc',
			area: 'CONTC',
			field: 'CODCONTC',
			description: '',
		}).cloneFrom(values?.ValCodcontc))
		watch(() => this.ValCodcontc.value, (newValue, oldValue) => this.onUpdate('contc.codcontc', this.ValCodcontc, newValue, oldValue))

		/** The used foreign keys. */
		this.ValCodpropr = reactive(new modelFieldType.ForeignKey({
			id: 'ValCodpropr',
			originId: 'ValCodpropr',
			area: 'CONTC',
			field: 'CODPROPR',
			relatedArea: 'PROPR',
			description: '',
		}).cloneFrom(values?.ValCodpropr))
		watch(() => this.ValCodpropr.value, (newValue, oldValue) => this.onUpdate('contc.codpropr', this.ValCodpropr, newValue, oldValue))

		/** The remaining form fields. */
		this.ValDtcontat = reactive(new modelFieldType.Date({
			id: 'ValDtcontat',
			originId: 'ValDtcontat',
			area: 'CONTC',
			field: 'DTCONTAT',
			description: computed(() => this.Resources.DATA_DO_CONTACTO52251),
		}).cloneFrom(values?.ValDtcontat))
		watch(() => this.ValDtcontat.value, (newValue, oldValue) => this.onUpdate('contc.dtcontat', this.ValDtcontat, newValue, oldValue))

		this.TableProprTitulo = reactive(new modelFieldType.String({
			type: 'Lookup',
			id: 'TableProprTitulo',
			originId: 'ValTitulo',
			area: 'PROPR',
			field: 'TITULO',
			maxLength: 80,
			description: computed(() => this.Resources.TITULO39021),
		}).cloneFrom(values?.TableProprTitulo))
		watch(() => this.TableProprTitulo.value, (newValue, oldValue) => this.onUpdate('propr.titulo', this.TableProprTitulo, newValue, oldValue))

		this.ValCltname = reactive(new modelFieldType.String({
			id: 'ValCltname',
			originId: 'ValCltname',
			area: 'CONTC',
			field: 'CLTNAME',
			maxLength: 50,
			description: computed(() => this.Resources.NOME_DO_CLIENTE38483),
		}).cloneFrom(values?.ValCltname))
		watch(() => this.ValCltname.value, (newValue, oldValue) => this.onUpdate('contc.cltname', this.ValCltname, newValue, oldValue))

		this.ValCltemail = reactive(new modelFieldType.String({
			id: 'ValCltemail',
			originId: 'ValCltemail',
			area: 'CONTC',
			field: 'CLTEMAIL',
			maxLength: 80,
			description: computed(() => this.Resources.EMAIL_DO_CLIENTE30111),
		}).cloneFrom(values?.ValCltemail))
		watch(() => this.ValCltemail.value, (newValue, oldValue) => this.onUpdate('contc.cltemail', this.ValCltemail, newValue, oldValue))

		this.ValTelefone = reactive(new modelFieldType.String({
			id: 'ValTelefone',
			originId: 'ValTelefone',
			area: 'CONTC',
			field: 'TELEFONE',
			maxLength: 14,
			description: computed(() => this.Resources.TELEFONE37757),
			maskType: 'MP',
			maskFormat: '+000 000000000',
			maskRequired: '+000 000000000',
		}).cloneFrom(values?.ValTelefone))
		watch(() => this.ValTelefone.value, (newValue, oldValue) => this.onUpdate('contc.telefone', this.ValTelefone, newValue, oldValue))

		this.ValDescriic = reactive(new modelFieldType.MultiLineString({
			id: 'ValDescriic',
			originId: 'ValDescriic',
			area: 'CONTC',
			field: 'DESCRIIC',
			description: computed(() => this.Resources.DESCRICAO07528),
		}).cloneFrom(values?.ValDescriic))
		watch(() => this.ValDescriic.value, (newValue, oldValue) => this.onUpdate('contc.descriic', this.ValDescriic, newValue, oldValue))
	}

	/**
	 * Creates a clone of the current QFormContactoViewModel instance.
	 * @returns {QFormContactoViewModel} A new instance of QFormContactoViewModel
	 */
	clone()
	{
		return new ViewModel(this.vueContext, { callbacks: this.externalCallbacks }, this)
	}

	static QPrimaryKeyName = 'ValCodcontc'

	get QPrimaryKey() { return this.ValCodcontc.value }
	set QPrimaryKey(value) { this.ValCodcontc.updateValue(value) }
}
