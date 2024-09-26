import { createFramework } from '@quidgest/ui'

const framework = createFramework({
	themes: {
		defaultTheme: 'Light',
		themes: [
			{
				name: 'Light',
				mode: 'light',
				colors: {
				}
			}
		]
	},
	defaults: {
		QIconSvg: {
			bundle: 'Content/svgbundle.svg'
		},
		QListItem: {
			icons: {
				check: {
					icon: 'ok'
				},
				description: {
					icon: 'help'
				}
			}
		},
		QSelect: {
			itemValue: 'key',
			itemLabel: 'value',
			icons: {
				clear: {
					icon: 'close'
				},
				chevron: {
					icon: 'expand'
				}
			}
		},
		QCombobox: {
			itemValue: 'key',
			itemLabel: 'value',
			icons: {
				clear: {
					icon: 'close'
				},
				chevron: {
					icon: 'expand'
				}
			}
		}
	}
})

export default framework
