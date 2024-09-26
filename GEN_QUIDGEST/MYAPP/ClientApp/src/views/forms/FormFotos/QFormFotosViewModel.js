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
			name: 'FOTOS',
			area: 'ALBUM',
			actions: {
				recalculateFormulas: 'RecalculateFormulas_FOTOS'
			}
		})

		/** The primary key. */
		this.ValCodalbum = reactive(new modelFieldType.PrimaryKey({
			id: 'ValCodalbum',
			originId: 'ValCodalbum',
			area: 'ALBUM',
			field: 'CODALBUM',
			description: '',
		}).cloneFrom(values?.ValCodalbum))
		watch(() => this.ValCodalbum.value, (newValue, oldValue) => this.onUpdate('album.codalbum', this.ValCodalbum, newValue, oldValue))

		/** The used foreign keys. */
		this.ValCodpropr = reactive(new modelFieldType.ForeignKey({
			id: 'ValCodpropr',
			originId: 'ValCodpropr',
			area: 'ALBUM',
			field: 'CODPROPR',
			relatedArea: 'PROPR',
			description: '',
		}).cloneFrom(values?.ValCodpropr))
		watch(() => this.ValCodpropr.value, (newValue, oldValue) => this.onUpdate('album.codpropr', this.ValCodpropr, newValue, oldValue))

		/** The remaining form fields. */
		this.ValFoto = reactive(new modelFieldType.Image({
			id: 'ValFoto',
			originId: 'ValFoto',
			area: 'ALBUM',
			field: 'FOTO',
			description: computed(() => this.Resources.FOTO19492),
		}).cloneFrom(values?.ValFoto))
		watch(() => this.ValFoto.value, (newValue, oldValue) => this.onUpdate('album.foto', this.ValFoto, newValue, oldValue))

		this.ValTitulo = reactive(new modelFieldType.String({
			id: 'ValTitulo',
			originId: 'ValTitulo',
			area: 'ALBUM',
			field: 'TITULO',
			maxLength: 50,
			description: computed(() => this.Resources.TITULO39021),
		}).cloneFrom(values?.ValTitulo))
		watch(() => this.ValTitulo.value, (newValue, oldValue) => this.onUpdate('album.titulo', this.ValTitulo, newValue, oldValue))

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
	}

	/**
	 * Creates a clone of the current QFormFotosViewModel instance.
	 * @returns {QFormFotosViewModel} A new instance of QFormFotosViewModel
	 */
	clone()
	{
		return new ViewModel(this.vueContext, { callbacks: this.externalCallbacks }, this)
	}

	static QPrimaryKeyName = 'ValCodalbum'

	get QPrimaryKey() { return this.ValCodalbum.value }
	set QPrimaryKey(value) { this.ValCodalbum.updateValue(value) }
}
