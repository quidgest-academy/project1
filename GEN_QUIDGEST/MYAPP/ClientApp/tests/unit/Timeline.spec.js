/**
 * @jest-environment jsdom
 */
import '@testing-library/jest-dom'
import { nextTick } from 'vue'
import { mount, render } from './utils'
import { fireEvent } from '@testing-library/vue'

import fakeData from '../cases/Timeline.mock'
import QTimeline from '@/components/timeline/QTimeline'
import QTimelineSummary from '@/components/timeline/QTimelineSummary'

describe('QTimeline.vue', () => {
	let wrapper

	beforeEach(() => {
		wrapper = mount(QTimeline, {
			props: fakeData.simpleUsage().yearlyTimeline
		})
	})

	afterEach(() => {
		wrapper.unmount()
	})

	it('On click bubble select group should be emitted', async () => {
		let horizontalSummary = await wrapper.findComponent(QTimelineSummary)
		let bubbleGroup = horizontalSummary.findAll('div')[3]
		await bubbleGroup.trigger('click')
		expect(horizontalSummary.emitted()['selected-group']).toBeTruthy()
	})

	it('Deselect the group ot click on selected bubble', async () => {
		let horizontalSummary = await wrapper.findComponent(QTimelineSummary)
		let bubbleGroup = horizontalSummary.findAll('div')[3]
		await bubbleGroup.trigger('click')
		await bubbleGroup.trigger('click')
		expect(horizontalSummary.emitted()['selected-group'][1][0]).toBeFalsy()
	})
})

describe('QTimeline.vue', () => {
	let wrapper

	beforeEach(() => {
		wrapper = render(QTimeline, {
			props: fakeData.simpleUsage().yearlyTimeline
		})
	})

	it('Expand vertical timeline on first reset button click', async () => {
		let resetButton = await wrapper.getByTestId('refresh-btn')
		await fireEvent.click(resetButton)
		await nextTick()
		let verticalTimeline = await wrapper.findAllByTestId(
			'vertical-timeline'
		)

		expect(verticalTimeline[0]).toBeInTheDocument()
	})

	it('Yearly timeline should have 4 groups', async () => {
		let bubbleGroups = await wrapper.findAllByTestId('bubble-group')
		expect(bubbleGroups.length).toEqual(4)
	})

	it('Emit form-popup event on click btn in each row', async () => {
		let circleBtns = await wrapper.findAllByTestId('bubble-group')
		// open timeline
		await fireEvent.click(circleBtns[0])
		let linkbtns = await wrapper.findAllByTestId('popup-btn')
		// click popup btn
		await fireEvent.click(linkbtns[0])
		expect(wrapper.emitted()['show-popup']).toBeTruthy()
	})

	it('Correct number of cards on vertical after selecting bubble', async () => {
		// get all horizontal bubbles
		let bubbleGroups = await wrapper.findAllByTestId('bubble-group')
		await fireEvent.click(bubbleGroups[0])
		let cards = wrapper.getAllByTestId('item-card')

		// only 1 item with 2018 year
		expect(cards.length).toEqual(1)
		// click 2nd bubble
		await fireEvent.click(bubbleGroups[1])
		// get 2nd bubble cards
		cards = wrapper.getAllByTestId('item-card')
		// 5 items with 2019 as year
		expect(cards.length).toEqual(5)
	})
})

describe('QTimeline.vue', () => {
	let wrapper

	beforeEach(() => {
		wrapper = render(QTimeline, {
			props: fakeData.simpleUsage().monthlyTimeline
		})
	})

	it('Monthly timeline should have 6 groups', async () => {
		let bubbleGroups = await wrapper.findAllByTestId('bubble-group')
		expect(bubbleGroups.length).toEqual(6)
	})
})
