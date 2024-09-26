import { computed } from 'vue'
import _assignIn from 'lodash-es/assignIn'
import _get from 'lodash-es/get'
import _keyBy from 'lodash-es/keyBy'
import _toString from 'lodash-es/toString'

import { useSystemDataStore } from '@/stores/systemData.js'
import listFunctions from '@/mixins/listFunctions.js'

export class BaseColumn
{
	#visibility = true

	constructor(options)
	{
		this.order = 0
		this.dataType = 'None'
		this.searchFieldType = null
		this.dataDisplay = null
		this.dataDisplayText = null
		this.component = null
		this.name = 'Undefined'
		this.area = 'Undefined'
		this.field = 'Undefined'
		this.label = ''
		this.supportForm = null
		this.supportFormIsPopup = false
		this.params = null
		this.cellAction = false
		this.sortable = true
		this.array = null
		this.useDistinctValues = false
		this.textColor = null
		this.bgColor = null
		this.isOrderingColumn = false
		this.initialSort = false
		this.initialSortOrder = ''
		this.isDefaultSearch = false
		this.pkColumn = null
		this.isHtmlField = false

		this.fnVisibility = () => true
		this.visibility = computed({
			get: () => this.#visibility && this.fnVisibility(),
			set: (val) => this.#visibility = val
		})

		// Add all properties to itself
		_assignIn(this, options)
	}
}

export class TextColumn extends BaseColumn
{
	constructor(options)
	{
		super({
			dataType: 'Text',
			searchFieldType: 'text',
			dataDisplay: listFunctions.textDisplayCell,
			dataDisplayText: listFunctions.textDisplayCell
		})

		// Add all properties to itself
		_assignIn(this, options)
	}
}

export class NumericColumn extends BaseColumn
{
	constructor(options)
	{
		const systemDataStore = useSystemDataStore()

		super({
			dataType: 'Numeric',
			searchFieldType: 'num',
			dataDisplay: listFunctions.numericDisplayCell,
			dataDisplayText: listFunctions.numericDisplayCell,
			maxDigits: 0,
			decimalPlaces: 0,
			numberFormat: {
				decimalSeparator: systemDataStore.system.numberFormat.decimalSeparator,
				groupSeparator: systemDataStore.system.numberFormat.thousandsSeparator,
			},
			showTotal: true,
			columnClasses: 'c-table__cell-numeric row-numeric',
			columnHeaderClasses: 'c-table__head-numeric'
		})

		// Add all properties to itself
		_assignIn(this, options)

		if (this.isOrderingColumn)
			this.columnHeaderClasses += ' thead-order'
	}
}

export class CurrencyColumn extends NumericColumn
{
	constructor(options)
	{
		const systemDataStore = useSystemDataStore()

		super({
			dataType: 'Currency',
			searchFieldType: 'num',
			dataDisplay: listFunctions.currencyDisplayCell,
			dataDisplayText: listFunctions.currencyDisplayCell,
			currency: systemDataStore.system.baseCurrency.code,
			currencySymbol: systemDataStore.system.baseCurrency.symbol
		})

		// Add all properties to itself
		_assignIn(this, options)

		// Currency fields get 2 more decimal places than what is defined
		this.decimalPlaces += 2
	}
}

export class DateColumn extends BaseColumn
{
	constructor(options)
	{
		const systemDataStore = useSystemDataStore()
		
		super({
			dataType: 'Date',
			searchFieldType: 'date',
			dateTimeType: 'dateTime',
			dateFormats: systemDataStore.system.dateFormat,
			dataDisplay: listFunctions.dateDisplayCell,
			dataDisplayText: listFunctions.dateDisplayCell
		})

		// Add all properties to itself
		_assignIn(this, options)
	}
}

export class BooleanColumn extends BaseColumn
{
	constructor(options)
	{
		super({
			dataType: 'Boolean',
			searchFieldType: 'bool',
			component: 'q-render-boolean',
			dataDisplay: listFunctions.booleanDisplayCell,
			dataDisplayText: listFunctions.booleanDisplayCell
		})

		// Add all properties to itself
		_assignIn(this, options)
	}
}

export class ImageColumn extends BaseColumn
{
	constructor(options)
	{
		super({
			dataType: 'Image',
			component: 'q-render-image',
			cellAction: true,
			dataDisplay: listFunctions.imageDisplayCell,
			dataDisplayText: listFunctions.imageDisplayCell
		})

		// Add all properties to itself
		_assignIn(this, options)
	}
}

export class DocumentColumn extends BaseColumn
{
	constructor(options)
	{
		super({
			dataType: 'Document',
			component: 'q-render-document',
			dataDisplay: listFunctions.documentDisplayCell,
			dataDisplayText: listFunctions.documentDisplayCell,
			isSerialized: true
		})

		// Add all properties to itself
		_assignIn(this, options)
	}
}

export class ArrayColumn extends BaseColumn
{
	constructor(options)
	{
		super({
			dataType: 'Array',
			component: 'q-render-array',
			searchFieldType: 'enum',
			dataDisplay: listFunctions.enumerationDisplayCell,
			dataDisplayText: listFunctions.enumerationDisplayCell
		})

		// Add all properties to itself
		_assignIn(this, options)
	}

	get arrayAsObj()
	{
		return new Proxy(_keyBy(this.array, aElem => _toString(aElem.key)), {
			get(target, name)
			{
				return _get({
					__v_skip: true,
					__v_isReactive: true,
					__v_isRef: false,
					__v_isReadonly: true,
					__v_raw: true
				}, name, _get(target, name, {}))
			}
		})
	}
}

export class GeographicColumn extends BaseColumn
{
	constructor(options)
	{
		const systemDataStore = useSystemDataStore()

		super({
			dataType: 'Geographic',
			dataDisplay: listFunctions.geographicDisplayCell,
			dataDisplayText: listFunctions.geographicDisplayCell,
			numberFormat: {
				decimalSeparator: systemDataStore.system.numberFormat.decimalSeparator,
				groupSeparator: systemDataStore.system.numberFormat.thousandsSeparator,
			}
		})

		// Add all properties to itself
		_assignIn(this, options)
	}
}

export class GeographicShapeColumn extends BaseColumn
{
	constructor(options)
	{
		super({
			dataType: 'GeographicShape',
			dataDisplay: listFunctions.geographicShapeDisplayCell,
			dataDisplayText: listFunctions.geographicShapeDisplayCell
		})

		// Add all properties to itself
		_assignIn(this, options)
	}
}

export class GeometricShapeColumn extends GeographicShapeColumn
{
	constructor(options)
	{
		super({
			dataType: 'GeometricShape'
		})

		// Add all properties to itself
		_assignIn(this, options)
	}
}

export class HyperLinkColumn extends BaseColumn
{
	constructor(options)
	{
		super({
			dataType: 'Text',
			searchFieldType: 'text',
			dataDisplay: listFunctions.hyperLinkDisplayCell,
			dataDisplayText: listFunctions.hyperLinkDisplayCell,
			component: 'q-render-hyperlink'
		})

		// Add all properties to itself
		_assignIn(this, options)
	}
}

export class HtmlColumn extends TextColumn
{
	constructor(options)
	{
		super({
			component: 'q-render-html',
			isHtmlField: true
		})

		// Add all properties to itself
		_assignIn(this, options)
	}
}

export default {
	BaseColumn,
	TextColumn,
	NumericColumn,
	CurrencyColumn,
	DateColumn,
	BooleanColumn,
	ImageColumn,
	DocumentColumn,
	ArrayColumn,
	GeographicColumn,
	GeographicShapeColumn,
	GeometricShapeColumn,
	HyperLinkColumn,
	HtmlColumn
}
