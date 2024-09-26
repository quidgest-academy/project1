import { computed, isRef } from 'vue'

import hardcodedTexts from '@/hardcodedTexts.js'

// Search filter condition operators
export const operators = {
	fnResources: null,
	setResources(fnResources)
	{
		this.fnResources = fnResources
		return this
	},
	get elements()
	{
		const vm = this
		return {
			'text': {
				'LIKE': {
					key: 'LIKE',
					resourceId: hardcodedTexts.isLike,
					get Title() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
					ValueCount: 1,
					placeholderResourceId: hardcodedTexts.keyword,
					get Placeholder() { return '%' + (vm.fnResources ? vm.fnResources(this.placeholderResourceId) : this.placeholderResourceId) + '%' }
				},
				'STRTWTH': {
					key: 'STRTWTH',
					resourceId: hardcodedTexts.startsWith,
					get Title() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
					ValueCount: 1
				},
				'CON': {
					key: 'CON',
					resourceId: hardcodedTexts.contains,
					get Title() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
					ValueCount: 1
				},
				'NOTCON': {
					key: 'NOTCON',
					resourceId: hardcodedTexts.notContains,
					get Title() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
					ValueCount: 1
				},
				'EQ': {
					key: 'EQ',
					resourceId: hardcodedTexts.isEqualTo,
					get Title() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
					ValueCount: 1
				},
				'NOTEQ': {
					key: 'NOTEQ',
					resourceId: hardcodedTexts.notEqual,
					get Title() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
					ValueCount: 1
				},
				'SET': {
					key: 'SET',
					resourceId: hardcodedTexts.isDefined,
					get Title() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
					ValueCount: 0
				},
				'NOTSET': {
					key: 'NOTSET',
					resourceId: hardcodedTexts.isNotDefined,
					get Title() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
					ValueCount: 0
				}
			},
			'num': {
				'EQ': {
					key: 'EQ',
					resourceId: hardcodedTexts.isEqualTo,
					get Title() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
					ValueCount: 1
				},
				'NOTEQ': {
					key: 'NOTEQ',
					resourceId: hardcodedTexts.notEqual,
					get Title() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
					ValueCount: 1
				},
				'GREAT': {
					key: 'GREAT',
					resourceId: hardcodedTexts.greaterThan,
					get Title() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
					ValueCount: 1
				},
				'LESS': {
					key: 'LESS',
					resourceId: hardcodedTexts.lessThan,
					get Title() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
					ValueCount: 1
				},
				'GREATEQ': {
					key: 'GREATEQ',
					resourceId: hardcodedTexts.greaterOrEqual,
					get Title() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
					ValueCount: 1
				},
				'LESSEQ': {
					key: 'LESSEQ',
					resourceId: hardcodedTexts.lessOrEqual,
					get Title() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
					ValueCount: 1
				},
				'BETW': {
					key: 'BETW',
					resourceId: hardcodedTexts.isBetween,
					get Title() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
					ValueCount: 2
				},
				'SET': {
					key: 'SET',
					resourceId: hardcodedTexts.isDefined,
					get Title() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
					ValueCount: 0
				},
				'NOTSET': {
					key: 'NOTSET',
					resourceId: hardcodedTexts.isNotDefined,
					get Title() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
					ValueCount: 0
				}
			},
			'bool': {
				'TRUE': {
					key: 'TRUE',
					resourceId: hardcodedTexts.isTrue,
					get Title() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
					ValueCount: 0
				},
				'FALSE': {
					key: 'FALSE',
					resourceId: hardcodedTexts.isFalse,
					get Title() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
					ValueCount: 0
				}
			},
			'date': {
				'EQ': {
					key: 'EQ',
					resourceId: hardcodedTexts.isEqualTo,
					get Title() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
					ValueCount: 1
				},
				'NOTEQ': {
					key: 'NOTEQ',
					resourceId: hardcodedTexts.notEqual,
					get Title() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
					ValueCount: 1
				},
				'AFT': {
					key: 'AFT',
					resourceId: hardcodedTexts.isAfter,
					get Title() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
					ValueCount: 1
				},
				'BEF': {
					key: 'BEF',
					resourceId: hardcodedTexts.isBefore,
					get Title() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
					ValueCount: 1
				},
				'AFTEQ': {
					key: 'AFTEQ',
					resourceId: hardcodedTexts.afterOrEqual,
					get Title() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
					ValueCount: 1
				},
				'BEFEQ': {
					key: 'BEFEQ',
					resourceId: hardcodedTexts.beforeOrEqual,
					get Title() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
					ValueCount: 1
				},
				'BETW': {
					key: 'BETW',
					resourceId: hardcodedTexts.isBetween,
					get Title() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
					ValueCount: 2
				},
				'SET': {
					key: 'SET',
					resourceId: hardcodedTexts.isDefined,
					get Title() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
					ValueCount: 0
				},
				'NOTSET': {
					key: 'NOTSET',
					resourceId: hardcodedTexts.isNotDefined,
					get Title() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
					ValueCount: 0
				}
			},
			'enum': {
				'IS': {
					key: 'IS',
					resourceId: hardcodedTexts.is,
					get Title() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
					ValueCount: 1
				},
				'ISNOT': {
					key: 'ISNOT',
					resourceId: hardcodedTexts.isNot,
					get Title() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
					ValueCount: 1
				},
				'IN': {
					key: 'IN',
					resourceId: hardcodedTexts.oneOf,
					get Title() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
					ValueCount: 1,
					inputComponent: 'q-edit-check-list',
					defaultValue: []
				},
				'SET': {
					key: 'SET',
					resourceId: hardcodedTexts.isDefined,
					get Title() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
					ValueCount: 0
				},
				'NOTSET': {
					key: 'NOTSET',
					resourceId: hardcodedTexts.isNotDefined,
					get Title() { return computed(() => vm.fnResources ? vm.fnResources(this.resourceId) : this.resourceId) },
					ValueCount: 0
				}
			}
		}
	}
}

export const searchBarOperator = function (dataType, searchValue)
{
	var condOperator = ''
	switch (dataType)
	{
		case 'text':
			condOperator = 'CON'
			break
		case 'num':
			condOperator = 'EQ'
			break
		case 'bool':
			if (searchValue.toUpperCase() === 'TRUE')
				condOperator = 'TRUE'
			else if (searchValue.toUpperCase() === 'FALSE')
				condOperator = 'FALSE'
			else
				condOperator = 'TRUE'
			break
		case 'date':
			condOperator = 'EQ'
			break
		case 'enum':
			condOperator = 'IS'
			break
	}
	return condOperator
}

export const defaultValue = function (column)
{
	var value = ''
	switch (column.searchFieldType)
	{
		case 'text':
			value = ''
			break
		case 'num':
			value = 0
			break
		case 'bool':
			value = false
			break
		case 'date':
			value = ''
			break
		case 'enum':
			for (let key in column.array)
			{
				value = column.array[key]
				if (isRef(value))
					value = value.value
				break
			}
			break
	}
	return value
}

// Components used by advanced filters, column filters and editable fields in normal tables
// (different than the ones in the editable table lists)
export const inputComponents = {
	text: 'q-edit-text',
	num: 'q-edit-numeric',
	bool: 'q-edit-boolean',
	date: 'q-edit-datetime',
	enum: 'q-edit-enumeration'
}

export default {
	operators,
	searchBarOperator,
	defaultValue,
	inputComponents,
	getWithTranslation(fnResources)
	{
		return {
			operators: operators.setResources(fnResources),
			searchBarOperator,
			defaultValue,
			inputComponents
		}
	}
}
