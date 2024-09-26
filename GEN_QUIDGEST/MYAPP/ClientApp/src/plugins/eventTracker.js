import { mapActions } from 'pinia'

import { useTracingDataStore } from '@/stores/tracingData.js'

export default {
	install(app)
	{
		app.config.globalProperties.$eventTracker = {
			...mapActions(useTracingDataStore, [
				'addTrace',
				'addWarning',
				'addError'
			])
		}

		if (process.env.NODE_ENV === 'production')
		{
			// Global handler for uncaught errors propagating from within the application
			app.config.errorHandler = (err, instance, info) => {
				const tracing = useTracingDataStore()
				tracing.addError({
					origin: 'Global errorHandler',
					message: 'Unhandled error',
					contextData: {
						err,
						instance,
						info
					}
				})
			}
		}

		// Custom handler for runtime warnings from Vue
		// TODO: Do we want it or not?
		/*
		app.config.warnHandler = (msg, instance, trace) => {
			const tracing = useTracingDataStore()
			tracing.addWarning({
				origin: 'Global warnHandler',
				message: msg,
				contextData: {
					instance,
					trace
				}
			})
		}
		*/
	}
}
