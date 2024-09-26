import { createFramework } from '@quidgest/ui'

const framework = createFramework({
	defaults: {
		QIcon: {
			type: 'font'
		},
		QIconFont: {
			library: 'glyphicons'
		}
	}
})

export default framework
