import axios from 'axios'
import mockAdapter from 'axios-mock-adapter'

import { handleDates } from './axiosResponseParsers'

const axiosInstance = axios.create()

// Add a response interceptor
axiosInstance.interceptors.response.use((response) => {
	// To avoid overwriting if another interceptor already defined the same object (meta)
	response.config.meta = response.config.meta || {}
	// Set the End timestamp
	response.config.meta.requestEndAt = new Date().getTime()

	// Any status code that lie within the range of 2xx cause this function to trigger
	handleDates(response.data)

	return response
})

// Add a request interceptor
axiosInstance.interceptors.request.use((config) => {
	// Do something before request is sent
	// TODO: Convert datetime to ISO

	// To avoid overwriting if another interceptor already defined the same object (meta)
	config.meta = config.meta || {}
	// Set the Start timestamp
	config.meta.requestStartAt = new Date().getTime()

	return config
})

export function getAxiosMockInstance(url, params, mockData)
{
	var axiosMockInstance = axios.create()

	// Add a response interceptor
	axiosMockInstance.interceptors.response.use((response) => {
		// Any status code that lie within the range of 2xx cause this function to trigger
		handleDates(response.data)
		return response
	})

	// All requests using this instance will have a 2 seconds delay:
	var mock = new mockAdapter(axiosMockInstance, { delayResponse: 2000 })

	mock.onGet(url, params).reply(200, mockData)

	return axiosMockInstance
}

export default {
	axiosInstance,
	getAxiosMockInstance
}
