import { computed } from 'vue'

export default class CarouselResources
{
	constructor(fnGetResource)
	{
		this._fnGetResource = typeof fnGetResource !== 'function' ? resId => resId : fnGetResource
		Object.defineProperty(this, '_fnGetResource', { enumerable: false })

		this.previousText = computed(() => this._fnGetResource('ANTERIOR34904'))
		this.nextText = computed(() => this._fnGetResource('PROXIMO29858'))
	}
}
