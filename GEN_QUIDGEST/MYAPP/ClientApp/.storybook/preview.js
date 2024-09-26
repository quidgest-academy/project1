﻿/** @type { import('@storybook/vue3').Preview } */
import 'bootstrap'
import { setup } from '@storybook/vue3'
import framework from '@/plugins/quidgest-ui'

import Components from '@/components/index.js'

// Global CSS
import '@/assets/styles/quidgest.scss'

// Register global components
setup((app) => {
	app.use(framework)
	app.use(Components)
})

const preview = {
	argTypes: {},
	parameters: {
		controls: {
			matchers: {
				color: /(background|color)$/i,
				date: /Date$/,
				text: /(title|label)$/i
			}
		},
		options: {
			storySort: {
				order: ['Introduction'],
				method: 'alphabetical'
			}
		}
	}
}

export default preview
