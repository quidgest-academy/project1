export default {
	simpleUsage()
	{
		return {
			containerId: "CarouselTest",
			mappedValues: [
				{
					rowKey: "1",
					slideTitle: "Mountain",
					slideSubtitle: "",
					slideImage:
						"data:image/gif;base64,R0lGODlhAQABAIAAAMLCwgAAACH5BAAAAAAALAAAAAABAAEAAAICRAEAOw==",
				},
				{
					rowKey: "2",
					slideTitle: "Desert",
					slideSubtitle: "",
					slideImage:
						"data:image/gif;base64,R0lGODlhAQABAIAAAMLCwgAAACH5BAAAAAAALAAAAAABAAEAAAICRAEAOw==",
				},
				{
					rowKey: "3",
					slideTitle: "Fog",
					slideSubtitle: "",
					slideImage:
						"data:image/gif;base64,R0lGODlhAQABAIAAAMLCwgAAACH5BAAAAAAALAAAAAABAAEAAAICRAEAOw==",
				},
			],
			styleVariables: {
				showIndicators: {
					value: true,
					isMapped: false
				},
				showControls: {
					value: true,
					isMapped: false
				},
				keyboardControllable: {
					value: true,
					isMapped: false
				},
				autoCycleInterval: {
					value: "5000",
					isMapped: false
				},
				autoCyclePause: {
					value: "hover",
					isMapped: false
				},
				ride: {
					value: "carousel",
					isMapped: false
				},
				wrap: {
					value: true,
					isMapped: false
				}
			},
			listConfig: {
				name: "CarouselTest",
			}
		}
	},
	simpleUsageMethods: {
		runAction(eventName, emittedAction)
		{
			var str = eventName + ":\n" + JSON.stringify(emittedAction)
			alert(str)
		},
		rowAction(emittedAction)
		{
			this.runAction("row-action", emittedAction)
		}
	}
}
