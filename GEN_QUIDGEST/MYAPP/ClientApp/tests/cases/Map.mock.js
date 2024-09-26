import controlClass from '@/mixins/fieldControl.js'

import mockConfig from '../config.json'

export default {
	singlePointMap: new controlClass.FieldSpecialRenderingControl({
		viewModes: [
			{
				type: 'map'
			}
		],
		viewMode: {
			id: 'MAP',
			type: 'map',
			subtype: 'leaflet-map',
			label: 'Mapa',
			visible: true,
			order: 1,
			implicitVariable: 'geographicData',
			implicitIsMultiple: true,
			mappingVariables: {
				markerDescription: {
					allowsMultiple: true,
					sources: [
						'INSTA.DESCRICA',
						'EQUIP.DESIGNAC',
						'INSTA.VALOR'
					]
				},
				markerIcon: {
					allowsMultiple: false,
					sources: [
						'EQUIP.FOTOGRAF'
					]
				}
			},
			styleVariables: {
				showSourcesInDescription: {
					value: true,
					isMapped: false
				},
				collapseLayerOptions: {
					value: true,
					isMapped: false
				},
				disableControls: {
					value: false,
					isMapped: false
				},
				disableSearch: {
					value: false,
					isMapped: false
				},
				fitZoom: {
					value: false,
					isMapped: false
				},
				zoomLevel: {
					value: 6,
					isMapped: false
				}
			},
			isSinglePoint: true,
			mapVersionKey: 0,
			mappedValues: [
				{
					geographicData: [
						{
							value: {
								lat: 39.52596814013205,
								lng: -8.254177130638674
							},
							rawData: 'POINT(-8.2541771306386735 39.525968140132051)',
							source: {
								type: 'String',
								id: 'ValCoordgeo',
								originId: 'ValCoordgeo',
								area: 'INSTA',
								field: 'COORDGEO',
								relatedArea: null,
								valueFormula: null,
								isRequired: false,
								value: 'POINT(-8.2541771306386735 39.525968140132051)',
								maxCharacters: 50,
								description: 'Geographic coordinate',
								isReady: true,
								originalValue: 'POINT(-8.2541771306386735 39.525968140132051)',
								isDirty: false
							},
							type: 'Geographic'
						},
						{
							value: {
								layerName: 'Shapes layer',
								shapes: [
									{
										type: 'polyline',
										latlngs: [
											{
												lat: 38.714845,
												lng: -9.140893
											},
											{
												lat: 38.715217,
												lng: -9.141183
											},
											{
												lat: 38.715502,
												lng: -9.141237
											},
											{
												lat: 38.716339,
												lng: -9.141955
											},
											{
												lat: 38.716413,
												lng: -9.142245
											},
											{
												lat: 38.724778,
												lng: -9.14953
											},
											{
												lat: 38.725869,
												lng: -9.149605
											},
											{
												lat: 38.733423,
												lng: -9.144959
											},
											{
												lat: 38.733582,
												lng: -9.144702
											},
											{
												lat: 38.733858,
												lng: -9.144648
											},
											{
												lat: 38.73415,
												lng: -9.144841
											},
											{
												lat: 38.739884,
												lng: -9.146225
											},
											{
												lat: 38.741029,
												lng: -9.146343
											},
											{
												lat: 38.746653,
												lng: -9.14776
											},
											{
												lat: 38.747883,
												lng: -9.148039
											},
											{
												lat: 38.74877,
												lng: -9.148006
											},
											{
												lat: 38.749431,
												lng: -9.148178
											},
											{
												lat: 38.749849,
												lng: -9.148339
											},
											{
												lat: 38.751364,
												lng: -9.149144
											},
											{
												lat: 38.752343,
												lng: -9.146183
											},
											{
												lat: 38.753272,
												lng: -9.146676
											}
										]
									},
									{
										type: 'circle',
										center: {
											lat: 38.706147,
											lng: -8.964844
										},
										radius: 11085.305
									},
									{
										type: 'rectangle',
										shapeParts: [
											[
												{
													lat: 38.283458,
													lng: -9.321213
												},
												{
													lat: 38.466321,
													lng: -9.321213
												},
												{
													lat: 38.466321,
													lng: -8.694305
												},
												{
													lat: 38.283458,
													lng: -8.694305
												},
												{
													lat: 38.283458,
													lng: -9.321213
												}
											]
										]
									},
									{
										type: 'polygon',
										shapeParts: [
											[
												{
													lat: 38.757095,
													lng: -9.16779
												},
												{
													lat: 38.757187,
													lng: -9.169722
												},
												{
													lat: 38.756677,
													lng: -9.170698
												},
												{
													lat: 38.756013,
													lng: -9.171352
												},
												{
													lat: 38.754635,
													lng: -9.17117
												},
												{
													lat: 38.753179,
													lng: -9.169947
												},
												{
													lat: 38.752493,
													lng: -9.168745
												},
												{
													lat: 38.751414,
													lng: -9.16882
												},
												{
													lat: 38.746133,
													lng: -9.164883
												},
												{
													lat: 38.744804,
													lng: -9.164422
												},
												{
													lat: 38.744687,
													lng: -9.16366
												},
												{
													lat: 38.744938,
													lng: -9.163102
												},
												{
													lat: 38.745515,
													lng: -9.16352
												},
												{
													lat: 38.746017,
													lng: -9.162748
												},
												{
													lat: 38.746001,
													lng: -9.162104
												},
												{
													lat: 38.745733,
													lng: -9.161621
												},
												{
													lat: 38.746419,
													lng: -9.159186
												},
												{
													lat: 38.747322,
													lng: -9.15719
												},
												{
													lat: 38.74744,
													lng: -9.156622
												},
												{
													lat: 38.747347,
													lng: -9.156021
												},
												{
													lat: 38.748427,
													lng: -9.14894
												},
												{
													lat: 38.751439,
													lng: -9.150903
												},
												{
													lat: 38.755816,
													lng: -9.154047
												},
												{
													lat: 38.756794,
													lng: -9.154701
												},
												{
													lat: 38.758844,
													lng: -9.156268
												},
												{
													lat: 38.757965,
													lng: -9.1594
												},
												{
													lat: 38.757888,
													lng: -9.161053
												},
												{
													lat: 38.757087,
													lng: -9.164808
												},
												{
													lat: 38.757095,
													lng: -9.16779
												}
											]
										]
									}
								]
							},
							rawData: '',
							source: {
								type: 'String',
								id: 'ValTerreno',
								originId: 'ValTerreno',
								area: 'PESSO',
								field: 'TERRENO',
								relatedArea: null,
								valueFormula: null,
								ignoreFldSubmit: false,
								isRequired: false,
								value: '',
								maxCharacters: 50,
								description: 'Terreno',
								isReady: true,
								originalValue: '',
								isDirty: false
							},
							type: 'GeographicShape'
						}
					],
					markerDescription: [
						{
							value: '',
							rawData: '',
							source: {
								type: 'String',
								id: 'ValDescrica',
								originId: 'ValDescrica',
								area: 'INSTA',
								field: 'DESCRICA',
								relatedArea: null,
								valueFormula: null,
								isRequired: false,
								value: '',
								maxCharacters: -1,
								description: 'Description',
								isReady: true,
								originalValue: '',
								isDirty: false
							}
						},
						{
							value: 'Berbequim eléctrico de 1500 w',
							rawData: 'Berbequim eléctrico de 1500 w',
							source: {
								type: 'Text',
								id: 'EquipValDesignac',
								originId: 'ValDesignac',
								area: 'EQUIP',
								field: 'DESIGNAC',
								relatedArea: null,
								valueFormula: null,
								isRequired: false,
								value: 'Berbequim eléctrico de 1500 w',
								maxCharacters: -1,
								description: 'Designation',
								isReady: true,
								originalValue: 'Berbequim eléctrico de 1500 w',
								isDirty: false
							}
						},
						{
							value: 1704,
							rawData: 1704,
							source: {
								type: 'Number',
								id: 'ValValor',
								originId: 'ValValor',
								area: 'INSTA',
								field: 'VALOR',
								relatedArea: null,
								valueFormula: {
									dependencyEvents: [
										'fieldChange:insta.horas',
										'fieldChange:insta.precohor'
									],
									isServerRecalc: false
								},
								isRequired: false,
								value: 1704,
								maxIntegers: -1,
								maxDecimals: -1,
								maxDigits: 9,
								decimalDigits: 2,
								description: 'Value',
								isReady: true,
								originalValue: 1704,
								isDirty: false
							}
						}
					],
					markerIcon: {
						value: {
							data: 'iVBORw0KGgoAAAANSUhEUgAAADIAAAAyCAYAAAAeP4ixAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAANySURBVGhD7ZlPSBRRHMfVJKND0LFLx4KIOhVBdOzYKa3M6FKHICIqQyKhrED7axEVoYJWUGoFGZRBGWhWeDDykJIJYWVRpuj6b90/L3/PefJm5jsz783M7kr4gQ+78/783vuyuzO7s1nsP2EhyHxjIQii9VM/y9permROQQX7NjhqzAxO4CDRWBxuVNeg+A4STyThhoLqF19BsvMr4CbCcuWBG8ZK6mgHQQs72ftzSHuOrA7KQcampuFibi4pvGjMZmwiGoNjvFRFKUhkMgoXUbXhbbdRibFt5Y1wjJsqKAVBxWX9gOq46YVnEFRU1/ae70Y1xioev4NjVHTDNQg6O+XtusD76tu7bX1Wt555wMcSI+NTcIyThLVtQ0ktb0c4BonFE7ZCpE6QW82dfCzqc1KwdPcl134rjkFQEVIniKqLZr6uCHIUrlEIGGTc5VQbdhAB6nMSAYOgycIwgxCo3cvFO2f3IJOxIMTyvZWwT0UrtiCP3vfAicIwghCn69tgn6rJZJLXEdiCoEmyQYMMj01yUZ+Oa49U830I0hrk1MyrQKA+pIxnv/E4B5ogqxPEidwd5+F4oaDoahM/XnO4ih87jSMyEoQ4WPXCcU5x3Stb+/POPtb8oc/UJhNKkMHRCd6my1BkkpU1tM1JXHnaYWqT+zafvGvai0woQfzS/2dEWfQOkMnIWwuN9aNM2oOsO1oDx/pRJqVBrLeKCPk4qDIpDUI/BegDKiRyC9xPvTrK2ILQFzI0SThfPiPFdS1GtVlsQaam3e8coiB+0L0JkZ1vPrZiC0LIE6yGFUQXeQ9oTRiEfrFZJwrDeGsRTj/eCNQuW3StiY+TgUEIVIAMI0hH7wAcTxKoXRaR0iD0ygpL77fyedZXe2NJremYkI+trj50m4+x4hiEQIV0glglCiuf2Nr6fg2zcw/b+fNNJ+pM/VadcA1y/E4LLBZE4sffCDvb+IY/p8d9N5+x6pcf4XhZtz+GXIMQqGBQPw8MsXgiwfI8rlmyy/ZcNnaE8QxCoMLp1gulIAQqni5VUA5CoEVSrSpaQQi3i2WYrnI4zTqhHYT4MnO6RIuHZSJhvmelgq8ggi2l9+BG/FrWMHtK9kOgIILa111wY6p2ff1tVPJPKEFk6GK3Yv91uGHh+mM1bDwaM2aEQ+hBMsVCkPkFY/8A9YGpHL9MQPsAAAAASUVORK5CYII=',
							encoding: 'base64',
							dataFormat: 'png',
							fileName: 'Image.png'
						},
						rawData: {
							data: 'iVBORw0KGgoAAAANSUhEUgAAADIAAAAyCAYAAAAeP4ixAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAANySURBVGhD7ZlPSBRRHMfVJKND0LFLx4KIOhVBdOzYKa3M6FKHICIqQyKhrED7axEVoYJWUGoFGZRBGWhWeDDykJIJYWVRpuj6b90/L3/PefJm5jsz783M7kr4gQ+78/783vuyuzO7s1nsP2EhyHxjIQii9VM/y9permROQQX7NjhqzAxO4CDRWBxuVNeg+A4STyThhoLqF19BsvMr4CbCcuWBG8ZK6mgHQQs72ftzSHuOrA7KQcampuFibi4pvGjMZmwiGoNjvFRFKUhkMgoXUbXhbbdRibFt5Y1wjJsqKAVBxWX9gOq46YVnEFRU1/ae70Y1xioev4NjVHTDNQg6O+XtusD76tu7bX1Wt555wMcSI+NTcIyThLVtQ0ktb0c4BonFE7ZCpE6QW82dfCzqc1KwdPcl134rjkFQEVIniKqLZr6uCHIUrlEIGGTc5VQbdhAB6nMSAYOgycIwgxCo3cvFO2f3IJOxIMTyvZWwT0UrtiCP3vfAicIwghCn69tgn6rJZJLXEdiCoEmyQYMMj01yUZ+Oa49U830I0hrk1MyrQKA+pIxnv/E4B5ogqxPEidwd5+F4oaDoahM/XnO4ih87jSMyEoQ4WPXCcU5x3Stb+/POPtb8oc/UJhNKkMHRCd6my1BkkpU1tM1JXHnaYWqT+zafvGvai0woQfzS/2dEWfQOkMnIWwuN9aNM2oOsO1oDx/pRJqVBrLeKCPk4qDIpDUI/BegDKiRyC9xPvTrK2ILQFzI0SThfPiPFdS1GtVlsQaam3e8coiB+0L0JkZ1vPrZiC0LIE6yGFUQXeQ9oTRiEfrFZJwrDeGsRTj/eCNQuW3StiY+TgUEIVIAMI0hH7wAcTxKoXRaR0iD0ygpL77fyedZXe2NJremYkI+trj50m4+x4hiEQIV0glglCiuf2Nr6fg2zcw/b+fNNJ+pM/VadcA1y/E4LLBZE4sffCDvb+IY/p8d9N5+x6pcf4XhZtz+GXIMQqGBQPw8MsXgiwfI8rlmyy/ZcNnaE8QxCoMLp1gulIAQqni5VUA5CoEVSrSpaQQi3i2WYrnI4zTqhHYT4MnO6RIuHZSJhvmelgq8ggi2l9+BG/FrWMHtK9kOgIILa111wY6p2ff1tVPJPKEFk6GK3Yv91uGHh+mM1bDwaM2aEQ+hBMsVCkPkFY/8A9YGpHL9MQPsAAAAASUVORK5CYII=',
							encoding: 'base64',
							dataFormat: 'png',
							fileName: 'Image.png'
						},
						source: {
							type: 'Image',
							id: 'EquipValFotograf',
							originId: 'ValFotograf',
							area: 'EQUIP',
							field: 'FOTOGRAF',
							relatedArea: null,
							valueFormula: null,
							isRequired: false,
							value: {
								data: 'iVBORw0KGgoAAAANSUhEUgAAADIAAAAyCAYAAAAeP4ixAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAANySURBVGhD7ZlPSBRRHMfVJKND0LFLx4KIOhVBdOzYKa3M6FKHICIqQyKhrED7axEVoYJWUGoFGZRBGWhWeDDykJIJYWVRpuj6b90/L3/PefJm5jsz783M7kr4gQ+78/783vuyuzO7s1nsP2EhyHxjIQii9VM/y9permROQQX7NjhqzAxO4CDRWBxuVNeg+A4STyThhoLqF19BsvMr4CbCcuWBG8ZK6mgHQQs72ftzSHuOrA7KQcampuFibi4pvGjMZmwiGoNjvFRFKUhkMgoXUbXhbbdRibFt5Y1wjJsqKAVBxWX9gOq46YVnEFRU1/ae70Y1xioev4NjVHTDNQg6O+XtusD76tu7bX1Wt555wMcSI+NTcIyThLVtQ0ktb0c4BonFE7ZCpE6QW82dfCzqc1KwdPcl134rjkFQEVIniKqLZr6uCHIUrlEIGGTc5VQbdhAB6nMSAYOgycIwgxCo3cvFO2f3IJOxIMTyvZWwT0UrtiCP3vfAicIwghCn69tgn6rJZJLXEdiCoEmyQYMMj01yUZ+Oa49U830I0hrk1MyrQKA+pIxnv/E4B5ogqxPEidwd5+F4oaDoahM/XnO4ih87jSMyEoQ4WPXCcU5x3Stb+/POPtb8oc/UJhNKkMHRCd6my1BkkpU1tM1JXHnaYWqT+zafvGvai0woQfzS/2dEWfQOkMnIWwuN9aNM2oOsO1oDx/pRJqVBrLeKCPk4qDIpDUI/BegDKiRyC9xPvTrK2ILQFzI0SThfPiPFdS1GtVlsQaam3e8coiB+0L0JkZ1vPrZiC0LIE6yGFUQXeQ9oTRiEfrFZJwrDeGsRTj/eCNQuW3StiY+TgUEIVIAMI0hH7wAcTxKoXRaR0iD0ygpL77fyedZXe2NJremYkI+trj50m4+x4hiEQIV0glglCiuf2Nr6fg2zcw/b+fNNJ+pM/VadcA1y/E4LLBZE4sffCDvb+IY/p8d9N5+x6pcf4XhZtz+GXIMQqGBQPw8MsXgiwfI8rlmyy/ZcNnaE8QxCoMLp1gulIAQqni5VUA5CoEVSrSpaQQi3i2WYrnI4zTqhHYT4MnO6RIuHZSJhvmelgq8ggi2l9+BG/FrWMHtK9kOgIILa111wY6p2ff1tVPJPKEFk6GK3Yv91uGHh+mM1bDwaM2aEQ+hBMsVCkPkFY/8A9YGpHL9MQPsAAAAASUVORK5CYII=',
								encoding: 'base64',
								dataFormat: 'png',
								fileName: 'Image.png'
							},
							description: 'Photograph',
							isReady: true,
							originalValue: {
								data: 'iVBORw0KGgoAAAANSUhEUgAAADIAAAAyCAYAAAAeP4ixAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAANySURBVGhD7ZlPSBRRHMfVJKND0LFLx4KIOhVBdOzYKa3M6FKHICIqQyKhrED7axEVoYJWUGoFGZRBGWhWeDDykJIJYWVRpuj6b90/L3/PefJm5jsz783M7kr4gQ+78/783vuyuzO7s1nsP2EhyHxjIQii9VM/y9permROQQX7NjhqzAxO4CDRWBxuVNeg+A4STyThhoLqF19BsvMr4CbCcuWBG8ZK6mgHQQs72ftzSHuOrA7KQcampuFibi4pvGjMZmwiGoNjvFRFKUhkMgoXUbXhbbdRibFt5Y1wjJsqKAVBxWX9gOq46YVnEFRU1/ae70Y1xioev4NjVHTDNQg6O+XtusD76tu7bX1Wt555wMcSI+NTcIyThLVtQ0ktb0c4BonFE7ZCpE6QW82dfCzqc1KwdPcl134rjkFQEVIniKqLZr6uCHIUrlEIGGTc5VQbdhAB6nMSAYOgycIwgxCo3cvFO2f3IJOxIMTyvZWwT0UrtiCP3vfAicIwghCn69tgn6rJZJLXEdiCoEmyQYMMj01yUZ+Oa49U830I0hrk1MyrQKA+pIxnv/E4B5ogqxPEidwd5+F4oaDoahM/XnO4ih87jSMyEoQ4WPXCcU5x3Stb+/POPtb8oc/UJhNKkMHRCd6my1BkkpU1tM1JXHnaYWqT+zafvGvai0woQfzS/2dEWfQOkMnIWwuN9aNM2oOsO1oDx/pRJqVBrLeKCPk4qDIpDUI/BegDKiRyC9xPvTrK2ILQFzI0SThfPiPFdS1GtVlsQaam3e8coiB+0L0JkZ1vPrZiC0LIE6yGFUQXeQ9oTRiEfrFZJwrDeGsRTj/eCNQuW3StiY+TgUEIVIAMI0hH7wAcTxKoXRaR0iD0ygpL77fyedZXe2NJremYkI+trj50m4+x4hiEQIV0glglCiuf2Nr6fg2zcw/b+fNNJ+pM/VadcA1y/E4LLBZE4sffCDvb+IY/p8d9N5+x6pcf4XhZtz+GXIMQqGBQPw8MsXgiwfI8rlmyy/ZcNnaE8QxCoMLp1gulIAQqni5VUA5CoEVSrSpaQQi3i2WYrnI4zTqhHYT4MnO6RIuHZSJhvmelgq8ggi2l9+BG/FrWMHtK9kOgIILa111wY6p2ff1tVPJPKEFk6GK3Yv91uGHh+mM1bDwaM2aEQ+hBMsVCkPkFY/8A9YGpHL9MQPsAAAAASUVORK5CYII=',
								encoding: 'base64',
								dataFormat: 'png',
								fileName: 'Image.png'
							},
							isDirty: false
						}
					}
				}
			],
			readonly: false,
			componentName: 'q-map',
			containerId: 'q-map-container-INSTA___INSTACOORDGEO'
		}
	}, {
		$getResource: resId => resId
	}),

	multiPointMap: new controlClass.TableSpecialRenderingControl({
		viewModes: [
			{
				type: 'map'
			}
		],
		viewMode: {
			id: 'MAP',
			type: 'map',
			subtype: 'leaflet-map',
			label: 'Mapa',
			visible: true,
			order: 2,
			mappingVariables: {
				geographicData: {
					allowsMultiple: false,
					sources: [
						'INSTA.COORDGEO'
					]
				},
				markerDescription: {
					allowsMultiple: true,
					sources: [
						'EQUIP.DESIGNAC',
						'INSTA.PRECOHOR',
						'INSTA.VALOR'
					]
				}
			},
			styleVariables: {
				zoomLevel: {
					value: 4,
					isMapped: false
				},
				fitZoom: {
					value: true,
					isMapped: false
				},
				disableControls: {
					value: true,
					isMapped: false
				},
				centerCoord: {
					value: {
						lat: 39,
						lng: -8.5
					},
					isMapped: false
				},
				disableSearch: {
					value: false,
					isMapped: false
				},
				showSourcesInDescription: {
					value: false,
					isMapped: false
				},
				collapseLayerOptions: {
					value: false,
					isMapped: false
				}
			},
			isSinglePoint: false,
			mapVersionKey: 0,
			mappedValues: [
				{
					rowKey: '368b589b-e1a7-46b8-87b9-51fd32b2e2fb',
					btnPermission: {
						editBtnDisabled: false,
						viewBtnDisabled: false,
						deleteBtnDisabled: false,
						insertBtnDisabled: false
					},
					markerDescription: [
						{
							value: 'Berbequim eléctrico de 1500 w',
							rawData: 'Berbequim eléctrico de 1500 w',
							source: {
								order: 2,
								dataType: 'Text',
								searchFieldType: 'text',
								component: null,
								name: 'Equip.ValDesignac',
								area: 'EQUIP',
								field: 'DESIGNAC',
								label: 'Equipment',
								supportForm: null,
								supportFormIsPopup: false,
								params: null,
								cellAction: false,
								visibility: true,
								sortable: true,
								array: null,
								useDistinctValues: false,
								textColor: null,
								bgColor: null,
								isOrderingColumn: false,
								initialSort: false,
								initialSortOrder: '',
								isDefaultSearch: false,
								pkColumn: 'ValCodequip',
								dataLength: 50
							}
						},
						{
							value: 24,
							rawData: 24,
							source: {
								order: 7,
								dataType: 'currency',
								searchFieldType: 'num',
								component: null,
								name: 'ValPrecohor',
								area: 'INSTA',
								field: 'PRECOHOR',
								label: 'Price per hour',
								supportForm: null,
								supportFormIsPopup: false,
								params: null,
								cellAction: false,
								visibility: true,
								sortable: true,
								array: null,
								useDistinctValues: false,
								textColor: null,
								bgColor: null,
								isOrderingColumn: false,
								initialSort: false,
								initialSortOrder: '',
								isDefaultSearch: false,
								pkColumn: null,
								maxDigits: 9,
								decimalPlaces: 0,
								showTotal: true,
								columnClasses: 'c-table__cell-numeric row-numeric',
								columnHeaderClasses: 'c-table__head-numeric',
								currency: 'EUR',
								currencySymbol: '€'
							}
						},
						{
							value: 1728,
							rawData: 1728,
							source: {
								order: 8,
								dataType: 'currency',
								searchFieldType: 'num',
								component: null,
								name: 'ValValor',
								area: 'INSTA',
								field: 'VALOR',
								label: 'Value',
								supportForm: null,
								supportFormIsPopup: false,
								params: null,
								cellAction: false,
								visibility: true,
								sortable: true,
								array: null,
								useDistinctValues: false,
								textColor: null,
								bgColor: null,
								isOrderingColumn: false,
								initialSort: false,
								initialSortOrder: '',
								isDefaultSearch: false,
								pkColumn: null,
								maxDigits: 9,
								decimalPlaces: 0,
								showTotal: true,
								columnClasses: 'c-table__cell-numeric row-numeric',
								columnHeaderClasses: 'c-table__head-numeric',
								currency: 'EUR',
								currencySymbol: '€'
							}
						}
					],
					geographicData: [
						{
							value: {
								lat: 39.52596814013205,
								lng: -8.254177130638674
							},
							rawData: 'POINT(-8.2541771306386735 39.525968140132051)',
							source: {
								order: 9,
								dataType: 'Geographic',
								searchFieldType: null,
								component: null,
								name: 'ValCoordgeo',
								area: 'INSTA',
								field: 'COORDGEO',
								label: 'Geographic coordinate',
								supportForm: null,
								supportFormIsPopup: false,
								params: null,
								cellAction: false,
								visibility: true,
								sortable: false,
								array: null,
								useDistinctValues: false,
								textColor: null,
								bgColor: null,
								isOrderingColumn: false,
								initialSort: false,
								initialSortOrder: '',
								isDefaultSearch: false,
								pkColumn: null
							},
							type: 'Geographic'
						},
						{
							value: {
								layerName: 'Layer 1',
								shapes: [
									{
										type: 'polyline',
										latlngs: [
											{
												lat: 38.714845,
												lng: -9.140893
											},
											{
												lat: 38.715217,
												lng: -9.141183
											},
											{
												lat: 38.715502,
												lng: -9.141237
											},
											{
												lat: 38.716339,
												lng: -9.141955
											},
											{
												lat: 38.716413,
												lng: -9.142245
											},
											{
												lat: 38.724778,
												lng: -9.14953
											},
											{
												lat: 38.725869,
												lng: -9.149605
											},
											{
												lat: 38.733423,
												lng: -9.144959
											},
											{
												lat: 38.733582,
												lng: -9.144702
											},
											{
												lat: 38.733858,
												lng: -9.144648
											},
											{
												lat: 38.73415,
												lng: -9.144841
											},
											{
												lat: 38.739884,
												lng: -9.146225
											},
											{
												lat: 38.741029,
												lng: -9.146343
											},
											{
												lat: 38.746653,
												lng: -9.14776
											},
											{
												lat: 38.747883,
												lng: -9.148039
											},
											{
												lat: 38.74877,
												lng: -9.148006
											},
											{
												lat: 38.749431,
												lng: -9.148178
											},
											{
												lat: 38.749849,
												lng: -9.148339
											},
											{
												lat: 38.751364,
												lng: -9.149144
											},
											{
												lat: 38.752343,
												lng: -9.146183
											},
											{
												lat: 38.753272,
												lng: -9.146676
											}
										]
									},
									{
										type: 'circle',
										center: {
											lat: 38.706147,
											lng: -8.964844
										},
										radius: 11085.305
									}
								]
							},
							rawData: '',
							source: {
								order: 10,
								dataType: 'GeographicShape',
								searchFieldType: null,
								component: null,
								name: 'ValTerrain',
								area: 'INSTA',
								field: 'TERRAIN',
								label: 'Terreno',
								supportForm: null,
								supportFormIsPopup: false,
								params: null,
								cellAction: false,
								visibility: true,
								sortable: false,
								array: null,
								useDistinctValues: false,
								textColor: null,
								bgColor: null,
								isOrderingColumn: false,
								initialSort: false,
								initialSortOrder: '',
								isDefaultSearch: false,
								pkColumn: null
							},
							type: 'GeographicShape'
						}
					]
				},
				{
					rowKey: '0f8347d2-cw22-34jd-9sfw-23dsmcf33ed3',
					btnPermission: {
						editBtnDisabled: false,
						viewBtnDisabled: false,
						deleteBtnDisabled: false,
						insertBtnDisabled: false
					},
					markerDescription: [
						{
							value: 'Berbequim eléctrico de 1500 w',
							rawData: 'Berbequim eléctrico de 1500 w',
							source: {
								order: 2,
								dataType: 'Text',
								searchFieldType: 'text',
								component: null,
								name: 'Equip.ValDesignac',
								area: 'EQUIP',
								field: 'DESIGNAC',
								label: 'Equipment',
								supportForm: null,
								supportFormIsPopup: false,
								params: null,
								cellAction: false,
								visibility: true,
								sortable: true,
								array: null,
								useDistinctValues: false,
								textColor: null,
								bgColor: null,
								isOrderingColumn: false,
								initialSort: false,
								initialSortOrder: '',
								isDefaultSearch: false,
								pkColumn: 'ValCodequip',
								dataLength: 50
							}
						},
						{
							value: 24,
							rawData: 24,
							source: {
								order: 7,
								dataType: 'currency',
								searchFieldType: 'num',
								component: null,
								name: 'ValPrecohor',
								area: 'INSTA',
								field: 'PRECOHOR',
								label: 'Price per hour',
								supportForm: null,
								supportFormIsPopup: false,
								params: null,
								cellAction: false,
								visibility: true,
								sortable: true,
								array: null,
								useDistinctValues: false,
								textColor: null,
								bgColor: null,
								isOrderingColumn: false,
								initialSort: false,
								initialSortOrder: '',
								isDefaultSearch: false,
								pkColumn: null,
								maxDigits: 9,
								decimalPlaces: 0,
								showTotal: true,
								columnClasses: 'c-table__cell-numeric row-numeric',
								columnHeaderClasses: 'c-table__head-numeric',
								currency: 'EUR',
								currencySymbol: '€'
							}
						},
						{
							value: 1728,
							rawData: 1728,
							source: {
								order: 8,
								dataType: 'currency',
								searchFieldType: 'num',
								component: null,
								name: 'ValValor',
								area: 'INSTA',
								field: 'VALOR',
								label: 'Value',
								supportForm: null,
								supportFormIsPopup: false,
								params: null,
								cellAction: false,
								visibility: true,
								sortable: true,
								array: null,
								useDistinctValues: false,
								textColor: null,
								bgColor: null,
								isOrderingColumn: false,
								initialSort: false,
								initialSortOrder: '',
								isDefaultSearch: false,
								pkColumn: null,
								maxDigits: 9,
								decimalPlaces: 0,
								showTotal: true,
								columnClasses: 'c-table__cell-numeric row-numeric',
								columnHeaderClasses: 'c-table__head-numeric',
								currency: 'EUR',
								currencySymbol: '€'
							}
						}
					],
					geographicData: [
						{
							value: {
								lat: 40.416775,
								lng: -3.703790
							},
							rawData: 'POINT(-3.703790 40.416775)',
							source: {
								order: 9,
								dataType: 'Geographic',
								searchFieldType: null,
								component: null,
								name: 'ValCoordgeo',
								area: 'INSTA',
								field: 'COORDGEO',
								label: 'Geographic coordinate',
								supportForm: null,
								supportFormIsPopup: false,
								params: null,
								cellAction: false,
								visibility: true,
								sortable: false,
								array: null,
								useDistinctValues: false,
								textColor: null,
								bgColor: null,
								isOrderingColumn: false,
								initialSort: false,
								initialSortOrder: '',
								isDefaultSearch: false,
								pkColumn: null
							},
							type: 'Geographic'
						},
						{
							value: {
								layerName: 'Layer 2',
								shapes: [
									{
										type: 'rectangle',
										shapeParts: [
											[
												{
													lat: 38.283458,
													lng: -9.321213
												},
												{
													lat: 38.466321,
													lng: -9.321213
												},
												{
													lat: 38.466321,
													lng: -8.694305
												},
												{
													lat: 38.283458,
													lng: -8.694305
												},
												{
													lat: 38.283458,
													lng: -9.321213
												}
											],
											[
												{
													lat: 38.34,
													lng: -9.1
												},
												{
													lat: 38.4,
													lng: -9.1
												},
												{
													lat: 38.4,
													lng: -8.9
												},
												{
													lat: 38.34,
													lng: -8.9
												},
												{
													lat: 38.34,
													lng: -9.1
												}
											]
										]
									},
									{
										type: 'polygon',
										shapeParts: [
											[
												{
													lat: 38.757095,
													lng: -9.16779
												},
												{
													lat: 38.757187,
													lng: -9.169722
												},
												{
													lat: 38.756677,
													lng: -9.170698
												},
												{
													lat: 38.756013,
													lng: -9.171352
												},
												{
													lat: 38.754635,
													lng: -9.17117
												},
												{
													lat: 38.753179,
													lng: -9.169947
												},
												{
													lat: 38.752493,
													lng: -9.168745
												},
												{
													lat: 38.751414,
													lng: -9.16882
												},
												{
													lat: 38.746133,
													lng: -9.164883
												},
												{
													lat: 38.744804,
													lng: -9.164422
												},
												{
													lat: 38.744687,
													lng: -9.16366
												},
												{
													lat: 38.744938,
													lng: -9.163102
												},
												{
													lat: 38.745515,
													lng: -9.16352
												},
												{
													lat: 38.746017,
													lng: -9.162748
												},
												{
													lat: 38.746001,
													lng: -9.162104
												},
												{
													lat: 38.745733,
													lng: -9.161621
												},
												{
													lat: 38.746419,
													lng: -9.159186
												},
												{
													lat: 38.747322,
													lng: -9.15719
												},
												{
													lat: 38.74744,
													lng: -9.156622
												},
												{
													lat: 38.747347,
													lng: -9.156021
												},
												{
													lat: 38.748427,
													lng: -9.14894
												},
												{
													lat: 38.751439,
													lng: -9.150903
												},
												{
													lat: 38.755816,
													lng: -9.154047
												},
												{
													lat: 38.756794,
													lng: -9.154701
												},
												{
													lat: 38.758844,
													lng: -9.156268
												},
												{
													lat: 38.757965,
													lng: -9.1594
												},
												{
													lat: 38.757888,
													lng: -9.161053
												},
												{
													lat: 38.757087,
													lng: -9.164808
												},
												{
													lat: 38.757095,
													lng: -9.16779
												}
											]
										]
									}
								]
							},
							rawData: '',
							source: {
								order: 10,
								dataType: 'GeographicShape',
								searchFieldType: null,
								component: null,
								name: 'ValTerrain',
								area: 'INSTA',
								field: 'TERRAIN',
								label: 'Terreno',
								supportForm: null,
								supportFormIsPopup: false,
								params: null,
								cellAction: false,
								visibility: true,
								sortable: false,
								array: null,
								useDistinctValues: false,
								textColor: null,
								bgColor: null,
								isOrderingColumn: false,
								initialSort: false,
								initialSortOrder: '',
								isDefaultSearch: false,
								pkColumn: null
							},
							type: 'GeographicShape'
						}
					]
				},
				{
					rowKey: '0c18fc0a-de48-473c-8e9b-a65ab895fdca',
					btnPermission: {
						editBtnDisabled: false,
						viewBtnDisabled: false,
						deleteBtnDisabled: false,
						insertBtnDisabled: false
					},
					markerDescription: [
						{
							value: 'Berbequim eléctrico de 1500 w',
							rawData: 'Berbequim eléctrico de 1500 w',
							source: {
								order: 2,
								dataType: 'Text',
								searchFieldType: 'text',
								component: null,
								name: 'Equip.ValDesignac',
								area: 'EQUIP',
								field: 'DESIGNAC',
								label: 'Equipment',
								supportForm: null,
								supportFormIsPopup: false,
								params: null,
								cellAction: false,
								visibility: true,
								sortable: true,
								array: null,
								useDistinctValues: false,
								textColor: null,
								bgColor: null,
								isOrderingColumn: false,
								initialSort: false,
								initialSortOrder: '',
								isDefaultSearch: false,
								pkColumn: 'ValCodequip',
								dataLength: 50
							}
						},
						{
							value: 15,
							rawData: 15,
							source: {
								order: 7,
								dataType: 'currency',
								searchFieldType: 'num',
								component: null,
								name: 'ValPrecohor',
								area: 'INSTA',
								field: 'PRECOHOR',
								label: 'Price per hour',
								supportForm: null,
								supportFormIsPopup: false,
								params: null,
								cellAction: false,
								visibility: true,
								sortable: true,
								array: null,
								useDistinctValues: false,
								textColor: null,
								bgColor: null,
								isOrderingColumn: false,
								initialSort: false,
								initialSortOrder: '',
								isDefaultSearch: false,
								pkColumn: null,
								maxDigits: 9,
								decimalPlaces: 0,
								showTotal: true,
								columnClasses: 'c-table__cell-numeric row-numeric',
								columnHeaderClasses: 'c-table__head-numeric',
								currency: 'EUR',
								currencySymbol: '€'
							}
						},
						{
							value: 360,
							rawData: 360,
							source: {
								order: 8,
								dataType: 'currency',
								searchFieldType: 'num',
								component: null,
								name: 'ValValor',
								area: 'INSTA',
								field: 'VALOR',
								label: 'Value',
								supportForm: null,
								supportFormIsPopup: false,
								params: null,
								cellAction: false,
								visibility: true,
								sortable: true,
								array: null,
								useDistinctValues: false,
								textColor: null,
								bgColor: null,
								isOrderingColumn: false,
								initialSort: false,
								initialSortOrder: '',
								isDefaultSearch: false,
								pkColumn: null,
								maxDigits: 9,
								decimalPlaces: 0,
								showTotal: true,
								columnClasses: 'c-table__cell-numeric row-numeric',
								columnHeaderClasses: 'c-table__head-numeric',
								currency: 'EUR',
								currencySymbol: '€'
							}
						}
					],
					geographicData: [
						{
							value: {
								lat: 39.012809,
								lng: -8.934376899999961
							},
							rawData: 'POINT(-8.9343768999999611 39.012809)',
							source: {
								order: 9,
								dataType: 'Geographic',
								searchFieldType: null,
								component: null,
								name: 'ValCoordgeo',
								area: 'INSTA',
								field: 'COORDGEO',
								label: 'Geographic coordinate',
								supportForm: null,
								supportFormIsPopup: false,
								params: null,
								cellAction: false,
								visibility: true,
								sortable: false,
								array: null,
								useDistinctValues: false,
								textColor: null,
								bgColor: null,
								isOrderingColumn: false,
								initialSort: false,
								initialSortOrder: '',
								isDefaultSearch: false,
								pkColumn: null
							},
							type: 'Geographic'
						},
						{
							value: {
								layerName: 'Layer 3',
								shapes: [
									{
										type: 'polyline',
										label: 'Route from company A to B',
										latlngs: [
											{
												lat: 47.334852,
												lng: -1.509485
											},
											{
												lat: 47.342596,
												lng: -1.328731
											},
											{
												lat: 47.241487,
												lng: -1.190568
											},
											{
												lat: 47.234787,
												lng: -1.358337
											}
										],
										color: 'green'
									},
									{
										type: 'polygon',
										label: 'Area of delivery',
										shapeParts: [
											[
												{
													lat: 46.334852,
													lng: -1.509485
												},
												{
													lat: 46.342596,
													lng: -1.328731
												},
												{
													lat: 46.241487,
													lng: -1.190568
												},
												{
													lat: 46.234787,
													lng: -1.358337
												}
											]
										],
										color: '#41b782',
										fillOpacity: 0.5,
										fillColor: '#41b782'
									},
									{
										type: 'rectangle',
										bounds: [
											{
												lat: 46.334852,
												lng: -1.190568
											},
											{
												lat: 46.241487,
												lng: -1.090357
											}
										]
									}
								]
							},
							rawData: '',
							source: {
								order: 10,
								dataType: 'GeographicShape',
								searchFieldType: null,
								component: null,
								name: 'ValTerrain',
								area: 'INSTA',
								field: 'TERRAIN',
								label: 'Terreno',
								supportForm: null,
								supportFormIsPopup: false,
								params: null,
								cellAction: false,
								visibility: true,
								sortable: false,
								array: null,
								useDistinctValues: false,
								textColor: null,
								bgColor: null,
								isOrderingColumn: false,
								initialSort: false,
								initialSortOrder: '',
								isDefaultSearch: false,
								pkColumn: null
							},
							type: 'GeographicShape'
						}
					]
				},
				{
					rowKey: '40ff9794-2710-438a-b88a-a6253244604e',
					btnPermission: {
						editBtnDisabled: false,
						viewBtnDisabled: false,
						deleteBtnDisabled: false,
						insertBtnDisabled: false
					},
					markerDescription: [
						{
							value: 'Berbequim eléctrico de 1500 w',
							rawData: 'Berbequim eléctrico de 1500 w',
							source: {
								order: 2,
								dataType: 'Text',
								searchFieldType: 'text',
								component: null,
								name: 'Equip.ValDesignac',
								area: 'EQUIP',
								field: 'DESIGNAC',
								label: 'Equipment',
								supportForm: null,
								supportFormIsPopup: false,
								params: null,
								cellAction: false,
								visibility: true,
								sortable: true,
								array: null,
								useDistinctValues: false,
								textColor: null,
								bgColor: null,
								isOrderingColumn: false,
								initialSort: false,
								initialSortOrder: '',
								isDefaultSearch: false,
								pkColumn: 'ValCodequip',
								dataLength: 50
							}
						},
						{
							value: 15,
							rawData: 15,
							source: {
								order: 7,
								dataType: 'currency',
								searchFieldType: 'num',
								component: null,
								name: 'ValPrecohor',
								area: 'INSTA',
								field: 'PRECOHOR',
								label: 'Price per hour',
								supportForm: null,
								supportFormIsPopup: false,
								params: null,
								cellAction: false,
								visibility: true,
								sortable: true,
								array: null,
								useDistinctValues: false,
								textColor: null,
								bgColor: null,
								isOrderingColumn: false,
								initialSort: false,
								initialSortOrder: '',
								isDefaultSearch: false,
								pkColumn: null,
								maxDigits: 9,
								decimalPlaces: 0,
								showTotal: true,
								columnClasses: 'c-table__cell-numeric row-numeric',
								columnHeaderClasses: 'c-table__head-numeric',
								currency: 'EUR',
								currencySymbol: '€'
							}
						},
						{
							value: 22680,
							rawData: 22680,
							source: {
								order: 8,
								dataType: 'currency',
								searchFieldType: 'num',
								component: null,
								name: 'ValValor',
								area: 'INSTA',
								field: 'VALOR',
								label: 'Value',
								supportForm: null,
								supportFormIsPopup: false,
								params: null,
								cellAction: false,
								visibility: true,
								sortable: true,
								array: null,
								useDistinctValues: false,
								textColor: null,
								bgColor: null,
								isOrderingColumn: false,
								initialSort: false,
								initialSortOrder: '',
								isDefaultSearch: false,
								pkColumn: null,
								maxDigits: 9,
								decimalPlaces: 0,
								showTotal: true,
								columnClasses: 'c-table__cell-numeric row-numeric',
								columnHeaderClasses: 'c-table__head-numeric',
								currency: 'EUR',
								currencySymbol: '€'
							}
						}
					],
					geographicData: [
						{
							value: {
								lat: 38.725161847874716,
								lng: -9.114189147949219
							},
							rawData: 'POINT(-9.1141891479492188 38.725161847874716)',
							source: {
								order: 9,
								dataType: 'Geographic',
								searchFieldType: null,
								component: null,
								name: 'ValCoordgeo',
								area: 'INSTA',
								field: 'COORDGEO',
								label: 'Geographic coordinate',
								supportForm: null,
								supportFormIsPopup: false,
								params: null,
								cellAction: false,
								visibility: true,
								sortable: false,
								array: null,
								useDistinctValues: false,
								textColor: null,
								bgColor: null,
								isOrderingColumn: false,
								initialSort: false,
								initialSortOrder: '',
								isDefaultSearch: false,
								pkColumn: null
							},
							type: 'Geographic'
						},
						{
							value: {
								layerName: 'Layer 4',
								shapes: [
									{
										type: 'rectangle',
										shapeParts: [
											[
												{
													lat: 46.334852,
													lng: -1.509485
												},
												{
													lat: 46.342596,
													lng: -1.328731
												},
												{
													lat: 46.241487,
													lng: -1.190568
												},
												{
													lat: 46.234787,
													lng: -1.358337
												}
											]
										],
										fill: true,
										color: '#35495d'
									},
									{
										type: 'circle',
										label: 'Radius of reachable clients',
										center: {
											lat: 50.5,
											lng: 30.5
										},
										radius: 250000
									},
									{
										type: 'marker',
										latlng: {
											lat: 46.552158,
											lng: 4.7241165
										}
									},
									{
										type: 'circlemarker',
										latlng: {
											lat: 43.9562644,
											lng: 22.32564151
										}
									}
								]
							},
							rawData: '',
							source: {
								order: 10,
								dataType: 'GeographicShape',
								searchFieldType: null,
								component: null,
								name: 'ValTerrain',
								area: 'INSTA',
								field: 'TERRAIN',
								label: 'Terreno',
								supportForm: null,
								supportFormIsPopup: false,
								params: null,
								cellAction: false,
								visibility: true,
								sortable: false,
								array: null,
								useDistinctValues: false,
								textColor: null,
								bgColor: null,
								isOrderingColumn: false,
								initialSort: false,
								initialSortOrder: '',
								isDefaultSearch: false,
								pkColumn: null
							},
							type: 'GeographicShape'
						}
					]
				},
				{
					rowKey: '9af7ccc4-52d0-498a-8a19-d1ba0fca7938',
					btnPermission: {
						editBtnDisabled: false,
						viewBtnDisabled: false,
						deleteBtnDisabled: false,
						insertBtnDisabled: false
					},
					markerDescription: [
						{
							value: 'Berbequim eléctrico de 1500 w',
							rawData: 'Berbequim eléctrico de 1500 w',
							source: {
								order: 2,
								dataType: 'Text',
								searchFieldType: 'text',
								component: null,
								name: 'Equip.ValDesignac',
								area: 'EQUIP',
								field: 'DESIGNAC',
								label: 'Equipment',
								supportForm: null,
								supportFormIsPopup: false,
								params: null,
								cellAction: false,
								visibility: true,
								sortable: true,
								array: null,
								useDistinctValues: false,
								textColor: null,
								bgColor: null,
								isOrderingColumn: false,
								initialSort: false,
								initialSortOrder: '',
								isDefaultSearch: false,
								pkColumn: 'ValCodequip',
								dataLength: 50
							}
						},
						{
							value: 15,
							rawData: 15,
							source: {
								order: 7,
								dataType: 'currency',
								searchFieldType: 'num',
								component: null,
								name: 'ValPrecohor',
								area: 'INSTA',
								field: 'PRECOHOR',
								label: 'Price per hour',
								supportForm: null,
								supportFormIsPopup: false,
								params: null,
								cellAction: false,
								visibility: true,
								sortable: true,
								array: null,
								useDistinctValues: false,
								textColor: null,
								bgColor: null,
								isOrderingColumn: false,
								initialSort: false,
								initialSortOrder: '',
								isDefaultSearch: false,
								pkColumn: null,
								maxDigits: 9,
								decimalPlaces: 0,
								showTotal: true,
								columnClasses: 'c-table__cell-numeric row-numeric',
								columnHeaderClasses: 'c-table__head-numeric',
								currency: 'EUR',
								currencySymbol: '€'
							}
						},
						{
							value: 3240,
							rawData: 3240,
							source: {
								order: 8,
								dataType: 'currency',
								searchFieldType: 'num',
								component: null,
								name: 'ValValor',
								area: 'INSTA',
								field: 'VALOR',
								label: 'Value',
								supportForm: null,
								supportFormIsPopup: false,
								params: null,
								cellAction: false,
								visibility: true,
								sortable: true,
								array: null,
								useDistinctValues: false,
								textColor: null,
								bgColor: null,
								isOrderingColumn: false,
								initialSort: false,
								initialSortOrder: '',
								isDefaultSearch: false,
								pkColumn: null,
								maxDigits: 9,
								decimalPlaces: 0,
								showTotal: true,
								columnClasses: 'c-table__cell-numeric row-numeric',
								columnHeaderClasses: 'c-table__head-numeric',
								currency: 'EUR',
								currencySymbol: '€'
							}
						}
					],
					geographicData: [
						{
							value: {
								lat: 38.7276142,
								lng: -9.143309000000045
							},
							rawData: 'POINT(-9.1433090000000448 38.7276142)',
							source: {
								order: 9,
								dataType: 'Geographic',
								searchFieldType: null,
								component: null,
								name: 'ValCoordgeo',
								area: 'INSTA',
								field: 'COORDGEO',
								label: 'Geographic coordinate',
								supportForm: null,
								supportFormIsPopup: false,
								params: null,
								cellAction: false,
								visibility: true,
								sortable: false,
								array: null,
								useDistinctValues: false,
								textColor: null,
								bgColor: null,
								isOrderingColumn: false,
								initialSort: false,
								initialSortOrder: '',
								isDefaultSearch: false,
								pkColumn: null
							},
							type: 'Geographic'
						},
						{
							value: null,
							rawData: '',
							source: {
								order: 10,
								dataType: 'GeographicShape',
								searchFieldType: null,
								component: null,
								name: 'ValTerrain',
								area: 'INSTA',
								field: 'TERRAIN',
								label: 'Terreno',
								supportForm: null,
								supportFormIsPopup: false,
								params: null,
								cellAction: false,
								visibility: true,
								sortable: false,
								array: null,
								useDistinctValues: false,
								textColor: null,
								bgColor: null,
								isOrderingColumn: false,
								initialSort: false,
								initialSortOrder: '',
								isDefaultSearch: false,
								pkColumn: null
							},
							type: 'GeographicShape'
						}
					]
				},
				{
					rowKey: 'd1f54506-c74b-4df4-a1f7-8066d152bdd0',
					btnPermission: {
						editBtnDisabled: false,
						viewBtnDisabled: false,
						deleteBtnDisabled: false,
						insertBtnDisabled: false
					},
					markerDescription: [
						{
							value: 'Berbequim eléctrico de 1500 w',
							rawData: 'Berbequim eléctrico de 1500 w',
							source: {
								order: 2,
								dataType: 'Text',
								searchFieldType: 'text',
								component: null,
								name: 'Equip.ValDesignac',
								area: 'EQUIP',
								field: 'DESIGNAC',
								label: 'Equipment',
								supportForm: null,
								supportFormIsPopup: false,
								params: null,
								cellAction: false,
								visibility: true,
								sortable: true,
								array: null,
								useDistinctValues: false,
								textColor: null,
								bgColor: null,
								isOrderingColumn: false,
								initialSort: false,
								initialSortOrder: '',
								isDefaultSearch: false,
								pkColumn: 'ValCodequip',
								dataLength: 50
							}
						},
						{
							value: 15,
							rawData: 15,
							source: {
								order: 7,
								dataType: 'currency',
								searchFieldType: 'num',
								component: null,
								name: 'ValPrecohor',
								area: 'INSTA',
								field: 'PRECOHOR',
								label: 'Price per hour',
								supportForm: null,
								supportFormIsPopup: false,
								params: null,
								cellAction: false,
								visibility: true,
								sortable: true,
								array: null,
								useDistinctValues: false,
								textColor: null,
								bgColor: null,
								isOrderingColumn: false,
								initialSort: false,
								initialSortOrder: '',
								isDefaultSearch: false,
								pkColumn: null,
								maxDigits: 9,
								decimalPlaces: 0,
								showTotal: true,
								columnClasses: 'c-table__cell-numeric row-numeric',
								columnHeaderClasses: 'c-table__head-numeric',
								currency: 'EUR',
								currencySymbol: '€'
							}
						},
						{
							value: 32025,
							rawData: 32025,
							source: {
								order: 8,
								dataType: 'currency',
								searchFieldType: 'num',
								component: null,
								name: 'ValValor',
								area: 'INSTA',
								field: 'VALOR',
								label: 'Value',
								supportForm: null,
								supportFormIsPopup: false,
								params: null,
								cellAction: false,
								visibility: true,
								sortable: true,
								array: null,
								useDistinctValues: false,
								textColor: null,
								bgColor: null,
								isOrderingColumn: false,
								initialSort: false,
								initialSortOrder: '',
								isDefaultSearch: false,
								pkColumn: null,
								maxDigits: 9,
								decimalPlaces: 0,
								showTotal: true,
								columnClasses: 'c-table__cell-numeric row-numeric',
								columnHeaderClasses: 'c-table__head-numeric',
								currency: 'EUR',
								currencySymbol: '€'
							}
						}
					],
					geographicData: [
						{
							value: null,
							rawData: '',
							source: {
								order: 9,
								dataType: 'Geographic',
								searchFieldType: null,
								component: null,
								name: 'ValCoordgeo',
								area: 'INSTA',
								field: 'COORDGEO',
								label: 'Geographic coordinate',
								supportForm: null,
								supportFormIsPopup: false,
								params: null,
								cellAction: false,
								visibility: true,
								sortable: false,
								array: null,
								useDistinctValues: false,
								textColor: null,
								bgColor: null,
								isOrderingColumn: false,
								initialSort: false,
								initialSortOrder: '',
								isDefaultSearch: false,
								pkColumn: null
							},
							type: 'Geographic'
						},
						{
							value: null,
							rawData: '',
							source: {
								order: 10,
								dataType: 'GeographicShape',
								searchFieldType: null,
								component: null,
								name: 'ValTerrain',
								area: 'INSTA',
								field: 'TERRAIN',
								label: 'Terreno',
								supportForm: null,
								supportFormIsPopup: false,
								params: null,
								cellAction: false,
								visibility: true,
								sortable: false,
								array: null,
								useDistinctValues: false,
								textColor: null,
								bgColor: null,
								isOrderingColumn: false,
								initialSort: false,
								initialSortOrder: '',
								isDefaultSearch: false,
								pkColumn: null
							},
							type: 'GeographicShape'
						}
					]
				}
			],
			readonly: true,
			componentName: 'q-map',
			containerId: 'q-map-container'
		},
		listConfig: {
			serverMode: false,
			defaultPerPage: 15,
			perPage: 15,
			actionsPlacement: 'left',
			paginationPlacement: 'left',
			hasTextWrap: false,
			allowFileExport: false,
			allowFileImport: false,
			exportOptions: [
				{
					key: 'pdf',
					value: 'Portable document format (PDF)'
				},
				{
					key: 'ods',
					value: 'Spreadsheet (ODS)'
				},
				{
					key: 'xlsx',
					value: 'Excel spreadsheet (XLSX)'
				},
				{
					key: 'csv',
					value: 'Comma-separated values (CSV)'
				},
				{
					key: 'xml',
					value: 'XML format (XML)'
				}
			],
			importOptions: [
				{
					key: 'xlsx',
					value: 'Excel spreadsheet (XLSX)'
				}
			],
			importTemplateOptions: [
				{
					key: 'xlsx',
					value: 'Excel Template Download'
				}
			],
			hasRowDragAndDrop: false,
			tableTitle: 'Instalations',
			tableNamePlural: 'Instalations',
			allowManageViews: true,
			hasCustomColumns: false,
			globalSearch: {
				visibility: true,
				searchOnPressEnter: true
			},
			filtersVisible: true,
			allowAdvancedFilters: true,
			allowColumnFilters: true,
			showRecordCount: false,
			showRowsSelectedCount: false,
			linkRowsSelectedAndChecked: false,
			menuForJump: '',
			sortByField: false,
			showRowDragAndDropOption: false,
			showLimitsInfo: true,
			showAfterFilter: false,
			columnResizeOptions: {},
			permissions: {
				canView: true,
				canEdit: true,
				canDuplicate: true,
				canDelete: true,
				canInsert: true
			},
			crudConditions: {},
			name: 'GQT_Menu_231',
			pkColumn: 'ValCodinsta',
			tableAlias: 'INSTA',
			showAlternatePagination: true,
			crudActions: [
				{
					id: 'show',
					name: 'show',
					title: 'View',
					iconSvg: 'view',
					isInReadOnly: true,
					params: {
						type: 'form',
						formName: 'INSTA',
						mode: 'SHOW',
						isControlled: true
					}
				},
				{
					id: 'edit',
					name: 'edit',
					title: 'Edit',
					iconSvg: 'pencil',
					isInReadOnly: false,
					params: {
						type: 'form',
						formName: 'INSTA',
						mode: 'EDIT',
						isControlled: true
					}
				},
				{
					id: 'duplicate',
					name: 'duplicate',
					title: 'Duplicate',
					iconSvg: 'duplicate',
					isInReadOnly: false,
					params: {
						type: 'form',
						formName: 'INSTA',
						mode: 'DUPLICATE',
						isControlled: true
					}
				},
				{
					id: 'delete',
					name: 'delete',
					title: 'Delete',
					iconSvg: 'delete',
					isInReadOnly: false,
					params: {
						type: 'form',
						formName: 'INSTA',
						mode: 'DELETE',
						isControlled: true
					}
				}
			],
			addAction: {
				name: 'insert',
				title: 'Insert',
				icon: 'plus-sign',
				isInReadOnly: false,
				params: {
					type: 'form',
					formName: 'INSTA',
					mode: 'NEW',
					repeatInsertion: false,
					isControlled: true
				}
			},
			customActions: [],
			MCActions: [],
			rowClickAction: {
				name: 'form-INSTA',
				type: 'follow-up',
				params: {
					limits: [
						{
							identifier: 'id'
						}
					],
					isControlled: true,
					type: 'form',
					mode: 'SHOW',
					formName: 'INSTA'
				}
			},
			formsDefinition: {
				INSTA: {
					isPopup: false
				}
			},
			defaultSearchColumnName: '',
			defaultSearchColumnNameOriginal: '',
			initialSortColumnName: '',
			initialSortColumnOrder: '',
			UserTableConfig: {},
			columnSizes: null,
			UserTableConfigNames: [],
			page: 1
		}
	}, {
		$getResource: resId => resId
	}),

	extraProperties: {
		subtypes: {
			google: 'google-map',
			leaflet: 'leaflet-map'
		},
		apiKeys: {
			google: mockConfig.apiKeys.googleMaps,
			geoapify: mockConfig.apiKeys.geoapify
		},
		globalIcon: {
			data: 'iVBORw0KGgoAAAANSUhEUgAAAQAAAAEACAYAAABccqhmAAAACXBIWXMAARlAAAEZQAGA43XUAAAP7UlEQVR42u2dzXEjORJGJ4IWjBF16XaCt7msA7Ri9khTaEg5QS/KgV4LEKFtSqAaLKKKRRKZADLf4YueljQSm6p8yH/89c9///MXQsineBMQAgAIIQCAEAIACCEAgBACAAghAIAQAgDIjv7378/dXLwvAAAZNO5a3wMBAKRo7Nu+9sffv/8cog6/tY8a4ud2pX4eAgBIyOBXPjdEg74Y9/HXvz9Pv/88//5zigpRHxldPj7Frx/j/3v8A4jVnwsQAADSMvjE2K+GPiYGvmTcHzMArMEg97WX7z/+gcKPv4EBAEB6Rv9p8MmpHjYYethg6EuGP///5t8jxNdxWgMCv1MAgF4w+vix/QaDf9bA31HIeAbp564ewgAMAADaYPi3f/9MxH0afXS5axv8y97CBVo5GBAmAABO+/vTfovRf3Sg3Gu9egaHNEwABADAteHH0/4Y3fteDf5ZGEwxpBkIDwCAU8P/OcTTPiwk8CwqLOQLDrP3BhAAALOGv4/lutBoPK8Jg++/x1wBIAAANpN7ieF7Oe1f9QqmFATkCABA74Y/zAw/YPjbQYA3AAC6dPcvyb1ZjI/hvwCCGBrsAQEA6OLUjxA4YvjFPYLTtXxIWAAAGj31Pwdmzhi+GAgICwBAi6f+t7tPck+najBeewiAAACo7fLvk849Tn09EISrN0BIAABqZfiPnPrVw4Kb3ADPKADQcvlHTv1mvIGJkAAA1HL5McQGQwKeWQAgYfyHhRl4C+50sBASXMIynlkAUNz4kyx/j2O577zmUOj7aGpM+zJ4jgHAW409s3i/95M8JLv91hReaNZpJiT46iAkOQgAyib7epi1T3vpz8lW38Pt6u/79d9/1n1/f36+XXjcsJasMQiQHAQALxr/rKuv9W071yUbh7X13S92OOaWmOxnm4hbgwEVAgBgyvhX9+3NjTX392cNYOvVYQ92GNaEARAAAM9CoDnjzy3OWNy4q9UVt/Sz0i3Gme0/AQgAgKaHehoy/tzqrP1S3N7mktPsvsMaIEggQGIQACyokYTf3PBPva3UzqxB28XcRE0Q3FUHEABYqvPXjvHvDL/XgZfc6vMMCLThOmL8ACCt9R8bMP67UVdLk24Zr+BQaYrye4iIUMAxAGbtvR8VE1Rpksr86qv52rTM9iTNcODoHQLejX9o4NS/uvuutuDOSpVDpeRruELXKwQo99U5/e9Ofa9LLTK7FLVyA3eVAQBA0k81BqUkld+rqDhufZMU9Ph7IO6vM7d+ZJVVMzMYrvMBHh+yYeH2XXWXH6NfLxsqe2nBY6cgzT60olKivZ0exAPA9Wc2vdHfG6EAAOjG9cf45daxifZl5AatAABZf4zfPgTcVQU8bfLl5CcceOb352LDsJca87lWcwnG321icMIDIPH3VgyJ8XcLATcJQfPtvto1/97q/FvXfjWa15Eu6ZpvE+aUcHxirL3OPkAgOs/hwgvg9G88c5ys517R86fUbCLyFE/TMW7uaf5SDcWJTtNeAKd/ow/JgpubvaXnmYx15nbjkAljbvbptwoCrd+1ZS+A07/MA1K8ZLQS54bMfXibfv6D243nNwuF9J691iEgnA8w6wVw+je6Wmrjg70JALNx28OGcdsUBGMPVQ3BTk/TuQDq/o2eDKUAMLvj8PTkwo10kOnQqjegVPKdCAGo+6t1i5UAgMDKrbHVBqfk3yoBf7PdgYz7Nrpa+h0ArCT6wi+ja8yE277NrhNnyWejDT+vAuBBoq/k+vJTa+VCjcEva12eJP8anRZ70wOQ3qvXujcwSDUGWbtPwNRSSa3GH41231cB8EKir8iOw1a8AWEvIFhLBjLy2+is+LMAqLTmPMzWaTXjHkt6AZaSgSz8aHTY58kGl1FxlfYjb6B6uVCwOchcGEDn35NLPgxOvElcc1a1XKjgFZrpDKT236jr9yQAQmMguGkeqpkglAqLrIQBuP+NUr9DD2ClXKjvDQhWhkyFAbj/jf7COwdAM+XC+HwwIGQNANoLP7U3/RgBQNXmIen30MItT7j/jQ6CGALAarlQo5dCIEdkJgywUAY8K3S8nTRPrYWOPguqsmtAIAxQrwoBAL1mj2rZ/9Q9TpJX1hSS7cnizUPCPQHd3yJE+W9T6698skdgdLcnb0CseUi6GtB7OZDhn8puXmZbT3Bg/GrNQ4LJYhObgkgAVoz/Z6O7p8Kju5QLZcvFJhKBvQNgVPAARBZ+Ko7u0jwkkzA2cZEoFYDHo79Fk1RC23rwBvQ9RhOVAACgWP+fbS4aHbv8qrsGhJLGAKB2GCBYIxf75W5cy40Wmoca7BqdaASytwGoeIJntq1n6TIOjH79opLDe8+MyFwAADA4BFSsxFPpqjKzYcE7SVmhQ6ProSAAoFABuJ4+Fxc0Xr55J2Mtvy/fwbf2Hn197j1jE8gbAQDDANiXDAHWvg95AdnpOsHSMQAwugeg+AN5hcBcAOD+/V57rxprHgMAhgGgtuXWSf9/Cx4AAAAAzzUBKV1pVnurb/WKBAAAAK30AXgAwCNjV4eBBgAEqjGUAWtJ0GjUZ72VQ4D0Z0wxMXb6msj7fB1TjY3DnQHgu2WZTkCj24CMegDJ+O1nyW2XC69i2U01L9EjAGgFrhsCjADg9QanXHUi87pOWhDoLAfANKDxcWBrVYC77sa1f9dsZFmli7FTALAPwOpGYM21zxrXecehml2LsAUAAKCl/vpgFABPtzbPRpgteABj4RwAK8EMLgWtcRdgs7sNLHlcBd9nloI2dCtQkI6XOy9pvuWqat3CJA0Aid6R3m8H6h0Ag5X7AKUBUOKkku6+lA4BJC4I4V4Am+3ANyUeC41ApYxLMhkoBQBBj7H7C0J7bwTaCRuOWpundBLw3ZNKIw+gAIADTUD2Lgc10QwEAFQAcKQEaAwAVkqBgp5MAACUAC0D4GChEoAHIJ0ELJovMlECtAIA6UrA2HkI4NoDkLwbULNVHADUWwwyKd0OfMYDEAUAewAsAqCnEhoeQD0AlI7/e58CNFMGFH4o1bK95AAkk4Ai8f8RAPhJBJ6pAvQHAOE7AfcAoB0QdD2phgcgCwCB1xx67wC0BoCd8AkqHgaQA5DKAYi4/2cLdmOkCqAzqiqd9aUKUP4mJin330IHoDkACOYBVBo/8ABkPIDCreKm4n8zIYBCHkC89EMOoJxhCW8xmqzE/6YAoLVbX6r7a/bai0sIACVVHACFYWWq/m8uCaiwvVY0/pP3AN6+VlvSA5C4jXkQeo1HAND+ijDR67RKjwjHVVWn2G9QXF+XgJQBwMUApF5nCe9K+DAw0f9vOQQQvy9Qygvo6YGS3NlX8PSfJBrCLNmLtSSgVjnQ5EnAM+DP/TcJAKUwwFwyyNDvX6waZBH65h4ChTDAZD3Y0Ok/Cp3+Z4vvGy5g40NCqHozmFn33ywAFMIA8w8Gi2Fs7f93AQDtpqDk4SAUsOn1qd8PAQD6aAoiFPC1D+JgFfCWH45B4fQ3OSHWYda/+52QAKCPjLDbk6Lhxq/xV8d7IABA/8lA8gGGqz3WG75MPyQKdwfmToxvlxEIdD/4Zb7ZiweFpCBJP8eNXtSHBU8PICAW1qlUdjy8r5wYBq4Vc2r8XW+BBgB2cwGLngAewdvGP2m4/p7ATdwIBDj5nZ7+LgBQqT14ITFIdeBFcAetrk5vY94eT5KPCkpKhCwS2boVSLmC43LRC51j+hC4aRYCBFlQ75Q2O7lv5/YaT35U9gS+x4jxBu57+yuFapPVkV8AUG9GYEvD0ODZG0j/zYrxPnsdPANAcVJwa0hw8OYN3Mb6n81a4+zCkQ/d09/uxB8AqDNIsvmegaRUOFgHwdzTiaf+VDMv43mrk+eYswUv4M4b+Log5M9pZOWhzBj+Pon1q4HY+9wGE2X1vYCcNzDFG3h2SwbUseEPibvfQh7G9Q4HFknqDgo9VSn4A4Jbj6CHh3X+GqPhn2beTvUkrPfqC2WntryAByC4LVO1BIPca4kf28cTPzRw6rtt+QUAfXkBHwunZIjJwsM8a50MPe1qGn1Syz9m3tfQEFi52YkQoOqg0DsewUc0rlMOBjkovHvj7qPvczX6mNgLKxBr4r3k9AcA8wf83AEElgzq6hkcv7Lr22var0Aifu0QwXmaGX1o2fDZ4AwAevYCtsIgRKM8RSgcvgz2x99b4fBl5Jev/zT0ffwexxjPTwvvU+jAiwoeW34BwAY10CJcCgZhxUCnmFQ8R0iMqZKPTw8MPf05oZP3h6vcAECzg0JSQFjyEp4FXejM4Gn5BQDdtghrwyEYMXBOfwDwdjJwMG783sSqdgCAFwAAuLYNANhoDkJs+QUAjluEES2/AEBpfyBeQP+nP00/AMBVcxDKbPlFAMBjc5D3xB9lPwBQ9FYaRNMPAHDoBVAWpOwHAJzvDyQhSNkPAFAWxAtg2g8AkBDE2Ej8AQAuFEFt9fvzvAIAQgHnNX9OfwAgDYMzEMD1BwD0BmCAZP0BgFMYEAo0lvXn9AcAbBLG9UcAgB2CuP4IALA9iKw/AgCyYQDbg3D9AQB7A/ACWPAJAFyHArQJc68fAHA+MYjx4/oDANqEAQGuPwCgNwDDZbsvAKA3ALHdFwDQG4DY7wcASAgi9vsBABKC6Kl2X4wfAJAQpN2XZwsAkBCk5o8AAB2C1gFA4g8AkBAk8cfpDwBICJL4QwCgZxAwMsyNvgCAkWG8ABJ/AMCrSAgy7AMAKAti/CT+AABzAoCAxB8A8FoWJCHIjb4AgLKgey+AxB8AoCzIqC8dfwCAsiCJP54PAOAMBl6nBbnZBwDgBXifFmTHHwCgLOivOYgdfwAAOZ8WpOwHAJDD5iDKfgAAOW8OouwHAJDD5iDKfgAALYPA9DXj39N+/K4BAHLqBVD2AwDIX4sw034AADlvEQ6c/gAAbQSBoRZhmn4AAHLsBTDrDwCQ00Ehmn4AAHI+KETTDwBAr6rjQSFOfwCAnHsBnP4AADn0Ajj9AQBy7AXEm30xfACAvHkBDPwAAOTUC+B6LwCAHHsBnP4AADn1Ajj9AQBy3B3I6Q8AkNMZAU5/AIAcewGc/gAAOfUCOP0BAHK8L4DTHwAgp14Apz8AQLVA0MDuQE5/AIAqegG1NwjT8w8AUD0QVL1HgIk/AICcewHM+wMA5NAL4PQHAMh5RYDTHwAgh30BnP4AADmfFOT0BwCoNQhcbt8R9gI4/QEA6iQhGOj6QwCAUKA0APac/gAA+eoN4IJPAIA6yweU2h8YSPwBANRhPqBQafB6uy+uPwBAnYUCw5tJQab9AAByCIFAyQ8AIEMQmIUD4ZHhR7efkx8AIEMLRE4PjD7p9CPmBwDIakgwLkwQXgz/mHwtxg8AkDUIJFWCS2iwv7j68b93ua9FAADZmhzc5aFw+zkEAJADGGD4AAAhBAAQQgAAIQQAEEIAACEEABBCAAAhBAAQQjb0f/6FrgiGN2vSAAAAAElFTkSuQmCC',
			dataFormat: 'jpg',
			encoding: 'base64',
			altText: '',
			titleText: '',
		},
		overlays: [
			{
				name: 'Mapbox Grayscale',
				url: 'https://api.mapbox.com/styles/v1/mapbox/light-v9/tiles/{z}/{x}/{y}?access_token=pk.eyJ1IjoibWFwYm94IiwiYSI6ImNpejY4NXVycTA2emYycXBndHRqcmZ3N3gifQ.rJcFIG214AriISLbB6B5aw',
				attribution: 'Map data &copy; OpenStreetMap contributors, Imagery &copy; Mapbox'
			},
			{
				name: 'Mapbox Streets',
				url: 'https://api.mapbox.com/styles/v1/mapbox/streets-v9/tiles/{z}/{x}/{y}?access_token=pk.eyJ1IjoibWFwYm94IiwiYSI6ImNpejY4NXVycTA2emYycXBndHRqcmZ3N3gifQ.rJcFIG214AriISLbB6B5aw',
				attribution: 'Map data &copy; OpenStreetMap contributors, Imagery &copy; Mapbox'
			},
			{
				name: 'Mapbox Satellite',
				url: 'https://api.mapbox.com/styles/v1/mapbox/satellite-streets-v9/tiles/{z}/{x}/{y}?access_token=pk.eyJ1IjoibWFwYm94IiwiYSI6ImNpejY4NXVycTA2emYycXBndHRqcmZ3N3gifQ.rJcFIG214AriISLbB6B5aw',
				attribution: 'Map data &copy; OpenStreetMap contributors, Imagery &copy; Mapbox'
			},
			{
				name: 'Google Streets',
				url: 'https://{s}.google.com/vt?lyrs=m&x={x}&y={y}&z={z}',
				attribution: '&copy; Google',
				subdomains: ['mt0', 'mt1', 'mt2', 'mt3']
			},
			{
				name: 'Google Terrain',
				url: 'https://{s}.google.com/vt?lyrs=p&x={x}&y={y}&z={z}',
				attribution: '&copy; Google',
				subdomains: ['mt0', 'mt1', 'mt2', 'mt3']
			},
			{
				name: 'Google Satellite',
				url: 'https://{s}.google.com/vt?lyrs=s&x={x}&y={y}&z={z}',
				attribution: '&copy; Google',
				subdomains: ['mt0', 'mt1', 'mt2', 'mt3']
			},
			{
				name: 'Google Hybrid',
				url: 'https://{s}.google.com/vt?lyrs=s,h&x={x}&y={y}&z={z}',
				attribution: '&copy; Google',
				subdomains: ['mt0', 'mt1', 'mt2', 'mt3']
			}
		]
	}
}
