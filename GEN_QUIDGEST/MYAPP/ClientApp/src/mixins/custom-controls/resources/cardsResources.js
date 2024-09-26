import { computed } from 'vue'

export default class CardsResources
{
	constructor(fnGetResource)
	{
		this._fnGetResource = typeof fnGetResource !== 'function' ? resId => resId : fnGetResource
		Object.defineProperty(this, '_fnGetResource', { enumerable: false })

		this.noRecordsText = computed(() => this._fnGetResource('SEM_REGISTOS62529'))
		this.emptyText = computed(() => this._fnGetResource('SEM_DADOS_PARA_MOSTR24928'))
		this.createText = computed(() => this._fnGetResource('CRIAR24836'))
		this.insertText = computed(() => this._fnGetResource('INSERIR43365'))
		this.cardImage = computed(() => this._fnGetResource('IMAGEM_DO_CARTAO15404'))
		this.loading = computed(() => this._fnGetResource('A_CARREGAR___34906'))
	}
}
