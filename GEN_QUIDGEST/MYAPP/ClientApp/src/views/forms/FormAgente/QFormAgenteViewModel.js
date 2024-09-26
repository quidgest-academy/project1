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
			name: 'AGENTE',
			area: 'AGENT',
			actions: {
				recalculateFormulas: 'RecalculateFormulas_AGENTE'
			}
		})

		/** The primary key. */
		this.ValCodagent = reactive(new modelFieldType.PrimaryKey({
			id: 'ValCodagent',
			originId: 'ValCodagent',
			area: 'AGENT',
			field: 'CODAGENT',
			description: '',
		}).cloneFrom(values?.ValCodagent))
		watch(() => this.ValCodagent.value, (newValue, oldValue) => this.onUpdate('agent.codagent', this.ValCodagent, newValue, oldValue))

		/** The used foreign keys. */
		this.ValCodpmora = reactive(new modelFieldType.ForeignKey({
			id: 'ValCodpmora',
			originId: 'ValCodpmora',
			area: 'AGENT',
			field: 'CODPMORA',
			relatedArea: 'PMORA',
			description: '',
		}).cloneFrom(values?.ValCodpmora))
		watch(() => this.ValCodpmora.value, (newValue, oldValue) => this.onUpdate('agent.codpmora', this.ValCodpmora, newValue, oldValue))

		this.ValCodpnasc = reactive(new modelFieldType.ForeignKey({
			id: 'ValCodpnasc',
			originId: 'ValCodpnasc',
			area: 'AGENT',
			field: 'CODPNASC',
			relatedArea: 'PNASC',
			description: '',
		}).cloneFrom(values?.ValCodpnasc))
		watch(() => this.ValCodpnasc.value, (newValue, oldValue) => this.onUpdate('agent.codpnasc', this.ValCodpnasc, newValue, oldValue))

		/** The remaining form fields. */
		this.ValFoto = reactive(new modelFieldType.Image({
			id: 'ValFoto',
			originId: 'ValFoto',
			area: 'AGENT',
			field: 'FOTO',
			description: computed(() => this.Resources.FOTOGRAFIA36807),
		}).cloneFrom(values?.ValFoto))
		watch(() => this.ValFoto.value, (newValue, oldValue) => this.onUpdate('agent.foto', this.ValFoto, newValue, oldValue))

		this.ValNome = reactive(new modelFieldType.String({
			id: 'ValNome',
			originId: 'ValNome',
			area: 'AGENT',
			field: 'NOME',
			maxLength: 80,
			description: computed(() => this.Resources.NOME47814),
		}).cloneFrom(values?.ValNome))
		watch(() => this.ValNome.value, (newValue, oldValue) => this.onUpdate('agent.nome', this.ValNome, newValue, oldValue))

		this.ValDnascime = reactive(new modelFieldType.Date({
			id: 'ValDnascime',
			originId: 'ValDnascime',
			area: 'AGENT',
			field: 'DNASCIME',
			description: computed(() => this.Resources.DATA_DE_NASCIMENTO13938),
		}).cloneFrom(values?.ValDnascime))
		watch(() => this.ValDnascime.value, (newValue, oldValue) => this.onUpdate('agent.dnascime', this.ValDnascime, newValue, oldValue))

		this.ValEmail = reactive(new modelFieldType.String({
			id: 'ValEmail',
			originId: 'ValEmail',
			area: 'AGENT',
			field: 'EMAIL',
			maxLength: 80,
			description: computed(() => this.Resources.E_MAIL42251),
		}).cloneFrom(values?.ValEmail))
		watch(() => this.ValEmail.value, (newValue, oldValue) => this.onUpdate('agent.email', this.ValEmail, newValue, oldValue))

		this.ValTelefone = reactive(new modelFieldType.String({
			id: 'ValTelefone',
			originId: 'ValTelefone',
			area: 'AGENT',
			field: 'TELEFONE',
			maxLength: 14,
			description: computed(() => this.Resources.TELEFONE37757),
			maskType: 'MP',
			maskFormat: '+000 000000000',
			maskRequired: '+000 000000000',
		}).cloneFrom(values?.ValTelefone))
		watch(() => this.ValTelefone.value, (newValue, oldValue) => this.onUpdate('agent.telefone', this.ValTelefone, newValue, oldValue))

		this.TablePmoraPais = reactive(new modelFieldType.String({
			type: 'Lookup',
			id: 'TablePmoraPais',
			originId: 'ValPais',
			area: 'PMORA',
			field: 'PAIS',
			maxLength: 50,
			description: computed(() => this.Resources.PAIS58483),
		}).cloneFrom(values?.TablePmoraPais))
		watch(() => this.TablePmoraPais.value, (newValue, oldValue) => this.onUpdate('pmora.pais', this.TablePmoraPais, newValue, oldValue))

		this.TablePnascPais = reactive(new modelFieldType.String({
			type: 'Lookup',
			id: 'TablePnascPais',
			originId: 'ValPais',
			area: 'PNASC',
			field: 'PAIS',
			maxLength: 50,
			description: computed(() => this.Resources.PAIS58483),
		}).cloneFrom(values?.TablePnascPais))
		watch(() => this.TablePnascPais.value, (newValue, oldValue) => this.onUpdate('pnasc.pais', this.TablePnascPais, newValue, oldValue))
	}

	/**
	 * Creates a clone of the current QFormAgenteViewModel instance.
	 * @returns {QFormAgenteViewModel} A new instance of QFormAgenteViewModel
	 */
	clone()
	{
		return new ViewModel(this.vueContext, { callbacks: this.externalCallbacks }, this)
	}

	static QPrimaryKeyName = 'ValCodagent'

	get QPrimaryKey() { return this.ValCodagent.value }
	set QPrimaryKey(value) { this.ValCodagent.updateValue(value) }
}
