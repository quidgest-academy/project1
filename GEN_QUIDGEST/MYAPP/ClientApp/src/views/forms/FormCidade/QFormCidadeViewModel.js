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
			name: 'CIDADE',
			area: 'CIDAD',
			actions: {
				recalculateFormulas: 'RecalculateFormulas_CIDADE'
			}
		})

		/** The primary key. */
		this.ValCodcidad = reactive(new modelFieldType.PrimaryKey({
			id: 'ValCodcidad',
			originId: 'ValCodcidad',
			area: 'CIDAD',
			field: 'CODCIDAD',
			description: '',
		}).cloneFrom(values?.ValCodcidad))
		watch(() => this.ValCodcidad.value, (newValue, oldValue) => this.onUpdate('cidad.codcidad', this.ValCodcidad, newValue, oldValue))

		/** The used foreign keys. */
		this.ValCodpais = reactive(new modelFieldType.ForeignKey({
			id: 'ValCodpais',
			originId: 'ValCodpais',
			area: 'CIDAD',
			field: 'CODPAIS',
			relatedArea: 'PAIS',
			description: '',
		}).cloneFrom(values?.ValCodpais))
		watch(() => this.ValCodpais.value, (newValue, oldValue) => this.onUpdate('cidad.codpais', this.ValCodpais, newValue, oldValue))

		/** The remaining form fields. */
		this.ValCidade = reactive(new modelFieldType.String({
			id: 'ValCidade',
			originId: 'ValCidade',
			area: 'CIDAD',
			field: 'CIDADE',
			maxLength: 50,
			description: computed(() => this.Resources.CIDADE42080),
		}).cloneFrom(values?.ValCidade))
		watch(() => this.ValCidade.value, (newValue, oldValue) => this.onUpdate('cidad.cidade', this.ValCidade, newValue, oldValue))

		this.TablePaisPais = reactive(new modelFieldType.String({
			type: 'Lookup',
			id: 'TablePaisPais',
			originId: 'ValPais',
			area: 'PAIS',
			field: 'PAIS',
			maxLength: 50,
			description: computed(() => this.Resources.PAIS58483),
		}).cloneFrom(values?.TablePaisPais))
		watch(() => this.TablePaisPais.value, (newValue, oldValue) => this.onUpdate('pais.pais', this.TablePaisPais, newValue, oldValue))
	}

	/**
	 * Creates a clone of the current QFormCidadeViewModel instance.
	 * @returns {QFormCidadeViewModel} A new instance of QFormCidadeViewModel
	 */
	clone()
	{
		return new ViewModel(this.vueContext, { callbacks: this.externalCallbacks }, this)
	}

	static QPrimaryKeyName = 'ValCodcidad'

	get QPrimaryKey() { return this.ValCodcidad.value }
	set QPrimaryKey(value) { this.ValCodcidad.updateValue(value) }
}
