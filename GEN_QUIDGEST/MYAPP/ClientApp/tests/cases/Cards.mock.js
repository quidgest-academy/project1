import controlClass from '@/mixins/fieldControl.js'
import assets from './Cards.assets'

export default {
	simpleUsage()
	{
		const lorem =
			'Lorem ipsum dolor sit amet, consectetur adipiscing elit'

		const defaults = {
			viewModes: [
				{
					type: 'cards'
				}
			],
			mappedValues: [
				{
					rowKey: '1',
					title: { value: 'Card title' },
					subtitle: { value: 'Card subtitle' },
					text: [
						{
							source: { label: 'About' },
							value: lorem
						}
					],
					image: assets.thumbnail,
					dominantColor: '#70597e',
					btnPermission: {
						editBtnDisabled: false,
						viewBtnDisabled: false,
						deleteBtnDisabled: false,
						insertBtnDisabled: false,
					}
				}
			],
			config: {
				generalActions: [
					{
						id: 'insert',
						name: 'insert',
						title: 'Insert',
						iconSvg: 'add',
						isInReadOnly: true
					}
				],
				crudActions: [
					{
						id: 'show',
						name: 'SHOW',
						title: 'View',
						iconSvg: 'view',
						isInReadOnly: true
					},
					{
						id: 'edit',
						name: 'EDIT',
						title: 'Edit',
						iconSvg: 'pencil',
						isInReadOnly: true
					}
				],
				customActions: [],
				resourcesPath: 'Content/img/',
				perPage: 6
			}
		}

		return {
			options: {
				variants: [
					{
						num: 0,
						key: 'card',
						text: 'Default',
						get value()
						{
							return this.text
						}
					},
					{
						num: 1,
						key: 'card-img-top',
						text: 'Edge-to-Edge',
						get value()
						{
							return this.text
						}
					},
					{
						num: 2,
						key: 'card-img-thumbnail',
						text: 'Thumbnail',
						get value()
						{
							return this.text
						}
					},
					{
						num: 3,
						key: 'card-img-background',
						text: 'Background',
						get value()
						{
							return this.text
						}
					},
					{
						num: 4,
						key: 'card-horizontal',
						text: 'Horizontal',
						get value()
						{
							return this.text
						}
					},
				],
				displayModes: [
					{
						num: 0,
						key: 'grid',
						text: 'Grid',
						get value()
						{
							return this.text
						}
					},
					{
						num: 1,
						key: 'carousel',
						text: 'Carousel',
						get value()
						{
							return this.text
						}
					},
				],
				sizes: [
					{
						num: 0,
						key: 'regular',
						text: 'Regular',
						get value()
						{
							return this.text
						}
					},
					{
						num: 1,
						key: 'small',
						text: 'Small',
						get value()
						{
							return this.text
						}
					}
				],
				shape: [
					{
						num: 0,
						key: 'rectangular',
						text: 'Rectangular',
						get value()
						{
							return this.text
						}
					},
					{
						num: 1,
						key: 'square',
						text: 'Square',
						get value()
						{
							return this.text
						}
					},
					{
						num: 2,
						key: 'circular',
						text: 'Circular',
						get value()
						{
							return this.text
						}
					}
				],
				actionsAlignment: [
					{
						num: 0,
						key: 'left',
						text: 'Left',
						get value()
						{
							return this.text
						}
					},
					{
						num: 1,
						key: 'right',
						text: 'Right',
						get value()
						{
							return this.text
						}
					}
				],
				actionsStyle: [
					{
						num: 0,
						key: 'dropdown',
						text: 'Dropdown',
						get value()
						{
							return this.text
						}
					},
					{
						num: 1,
						key: 'inline',
						text: 'Inline',
						get value()
						{
							return this.text
						}
					},
					{
						num: 2,
						key: 'mixed',
						text: 'Mixed',
						get value()
						{
							return this.text
						}
					}
				],
				actionsPlacement: [
					{
						num: 0,
						key: 'header',
						text: 'Header',
						get value()
						{
							return this.text
						}
					},
					{
						num: 1,
						key: 'footer',
						text: 'Footer',
						get value()
						{
							return this.text
						}
					}
				],
				hoverScaleAmounts: [
					{
						num: 0,
						key: 1.00,
						text: '1.00',
						get value()
						{
							return this.text
						}
					},
					{
						num: 1,
						key: 1.01,
						text: '1.01',
						get value()
						{
							return this.text
						}
					},
					{
						num: 2,
						key: 1.02,
						text: '1.02',
						get value()
						{
							return this.text
						}
					},
					{
						num: 3,
						key: 1.03,
						text: '1.03',
						get value()
						{
							return this.text
						}
					},
					{
						num: 4,
						key: 1.04,
						text: '1.04',
						get value()
						{
							return this.text
						}
					},
					{
						num: 5,
						key: 1.05,
						text: '1.05',
						get value()
						{
							return this.text
						}
					}
				],
				contentAlignment: [
					{
						num: 0,
						key: 'left',
						text: 'Left',
						get value()
						{
							return this.text
						}
					},
					{
						num: 1,
						key: 'center',
						text: 'Center',
						get value()
						{
							return this.text
						}
					}
				],
				containerAlignment: [
					{
						num: 0,
						key: 'left',
						text: 'Left',
						get value()
						{
							return this.text
						}
					},
					{
						num: 1,
						key: 'center',
						text: 'Center',
						get value()
						{
							return this.text
						}
					}
				]
			},
			usage: {
				container: {
					loading: false,
					numberOfCards: 6,
				},
				cards: new controlClass.TableSpecialRenderingControl(
					{
						specialRendering: defaults.specialRendering,
						subtype: 'card',
						mappedValues: defaults.mappedValues,
						styleVariables: {
							actionsAlignment: {
								value: 'left',
								isMapped: false
							},
							actionsPlacement: {
								value: 'footer',
								isMapped: false
							},
							actionsStyle: {
								value: 'dropdown',
								isMapped: false
							},
							contentAlignment: {
								value: 'left',
								isMapped: false
							},
							containerAlignment: {
								value: 'left',
								isMapped: false
							},
							imageShape: {
								value: 'rectangular',
								isMapped: false
							},
							size: {
								value: 'regular',
								isMapped: false
							},
							displayMode: {
								value: 'grid',
								isMapped: false
							},
							showColumnTitles: {
								value: false,
								isMapped: false
							},
							hoverScaleAmount: {
								value: 1.00,
								isMapped: false
							},
							customInsertCard: {
								value: false,
								isMapped: false
							},
							customInsertCardStyle: {
								value: 'secondary',
								isMapped: false
							},
						},
						listConfig: defaults.config
					},
					{
						$getResource: (resId) => resId
					},
					{}
				)
			}
		}
	}
}
