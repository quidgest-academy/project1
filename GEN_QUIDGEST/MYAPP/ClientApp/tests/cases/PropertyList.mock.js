export default {
	typeSelectOptions: [
		{ key: 'Text', value: 'QTextInput' },
		{ key: 'Numeric', value: 'QNumericInput' },
		{ key: 'Calendar', value: 'QDateTimePicker' },
		{ key: 'Boolean', value: 'CheckBoxInput' },
		{ key: 'Enumeration', value: 'QTextInput' },
	],
	menuItems: [
		{
			name: 'Edit label',
			action: 'edit',
		},
		{
			name: 'Switch type',
			action: 'switch',
			children: [
				{ name: 'Text' },
				{ name: 'Boolean' },
				{ name: 'Enumeration' },
				{ name: 'Calendar' },
			],
		},
		{
			name: 'Delete',
			action: 'delete',
		}
	],
	rows: [
		{
			Rownum: 0,
			FormMode: '',
			Fields: {
				PrimaryKey: '81cc095a-03f7-43a6-a820-087c8d41a83d',
				decimalPlaces: 2,
				Val: true,
				Property: 'Allow new',
				type: 'Boolean',
				Text: 'Lorem ipsum dolor',
				Date: '2021-02-16 23:24:12',
				Image: '',
				Document: '',
				Action: '',
				Geographic: '',
				Unknown: '',
				ValZzstate: 0,
			},
		},
		{
			Rownum: 1,
			FormMode: '',
			Fields: {
				PrimaryKey: 'e669f856-2ee3-49ee-bf0f-13eaa21c7b18',
				Val: 'file path',
				Property: 'Data Source',
				decimalPlaces: 2,
				type: 'Text',
				Text: 'sit amet',
			},
		},
		{
			Rownum: 2,
			FormMode: '',
			Fields: {
				PrimaryKey: '54420e72-68b4-41c2-a41e-1d7afbcb6924',
				Val: false,
				type: 'Boolean',
				Property: 'Data Source',
				Text: 'consectetur adipiscing elit',
			},
		},
		{
			Rownum: 3,
			FormMode: '',
			Fields: {
				PrimaryKey: '47556088-f88e-47fd-b618-323112f96176',
				Val: 3000,
				type: 'Number',
				Property: 'size',
				Text: 'sed do eiusmod tempor',
				decimalPlaces: 2,
			},
		},
		{
			Rownum: 4,
			FormMode: '',
			Fields: {
				PrimaryKey: '97c468d4-8b0f-4d8a-b40b-37841684e23466',
				Val: 5000,
				type: 'Number',
				decimalPlaces: 2,
				Property: 'volume',
				Text: 'incididunt ut labore',
			},
		},
		{
			Rownum: 5,
			FormMode: '',
			Fields: {
				PrimaryKey: '97c468d4-8b0f-4d8a-b40b-37841684e23488',
				Val: '2021-02-23 23:46:50',
				type: 'Date',
				Property: 'Final Date',
				Text: 'incididunt ut labore',
			},
		},
		{
			Rownum: 6,
			FormMode: '',
			Fields: {
				PrimaryKey: '97c468d4-8b0f-4d8a-b40b-37841684e23422',
				Val: 'ava',
				type: 'Text',
				Property: 'Fruit',
				Text: 'incididunt ut labore',
			},
		},
		{
			Rownum: 7,
			FormMode: '',
			Fields: {
				PrimaryKey: '97c468d4-8b0f-4d8a-b40b-37841684e2355',
				Val: 'Dj',
				type: 'Enumeration',
				options: [
					{
						key: 'Rails',
						value: 'RA',
					},
					{ key: 'Django', value: 'Dj' },
					{ key: 'Angular', value: 'AN' },
					{ key: 'Vue.js', value: 'VU' },
					{ key: 'React', value: 'RE' },
				],
				placeholder: 'Select your option',
				Property: 'Enumeration',
				optionValue: 'value',
				optionLabel: 'key',
				Text: 'incididunt ut labore',
			}
		}
	],
	totalRows: 10
}