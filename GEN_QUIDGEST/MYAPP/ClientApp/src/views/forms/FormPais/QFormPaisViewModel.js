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
			name: 'PAIS',
			area: 'PAIS',
			actions: {
				recalculateFormulas: 'RecalculateFormulas_PAIS'
			}
		})

		/** The primary key. */
		this.ValCodpais = reactive(new modelFieldType.PrimaryKey({
			id: 'ValCodpais',
			originId: 'ValCodpais',
			area: 'PAIS',
			field: 'CODPAIS',
			description: '',
		}).cloneFrom(values?.ValCodpais))
		watch(() => this.ValCodpais.value, (newValue, oldValue) => this.onUpdate('pais.codpais', this.ValCodpais, newValue, oldValue))

		/** The remaining form fields. */
		this.ValPais = reactive(new modelFieldType.String({
			id: 'ValPais',
			originId: 'ValPais',
			area: 'PAIS',
			field: 'PAIS',
			maxLength: 50,
			description: computed(() => this.Resources.PAIS58483),
		}).cloneFrom(values?.ValPais))
		watch(() => this.ValPais.value, (newValue, oldValue) => this.onUpdate('pais.pais', this.ValPais, newValue, oldValue))
	}

	/**
	 * Creates a clone of the current QFormPaisViewModel instance.
	 * @returns {QFormPaisViewModel} A new instance of QFormPaisViewModel
	 */
	clone()
	{
		return new ViewModel(this.vueContext, { callbacks: this.externalCallbacks }, this)
	}

	static QPrimaryKeyName = 'ValCodpais'

	get QPrimaryKey() { return this.ValCodpais.value }
	set QPrimaryKey(value) { this.ValCodpais.updateValue(value) }
}
