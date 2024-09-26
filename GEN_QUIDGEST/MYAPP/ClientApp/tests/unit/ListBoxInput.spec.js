/**
 * @jest-environment jsdom
 */
import "@testing-library/jest-dom";
import ListBoxInput from "@/components/inputs/ListBoxInput.vue";
import { fireEvent, render, screen } from "@testing-library/vue";
import { mount } from "@vue/test-utils";
import userEvent from "@testing-library/user-event";

const options = [
	{
		Value: 1,
		Text: "Rails",
	},
	{
		Value: 2,
		Text: "Django",
	},
	{
		Value: 3,
		Text: "Angular",
	},
	{
		Value: 4,
		Text: "Vue.js",
	},
	{
		Value: 5,
		Text: "React",
	},
]

describe("ListBoxInput.vue", () => {
	it("check passed array has expected Value", () => {
		let initial = "Vue.js";
		var selected = initial;
		const wrapper = mount(ListBoxInput, {
			props: {
				options: options,
				modelValue: selected,
				imageBox: true,
			},
		});
		const divArray = wrapper.componentVM.options;
		const element = divArray.find((item) => item.Value === 4);
		//check if text matches with passed array value
		expect(element.Text).toMatch(initial);
	});

	it("renders details box when imageBox is true", async () =>  {
		const wrapper = render(ListBoxInput, {
			props: {
				options: options,
				modelValue: "Angular",
				imageBox: true,
				dataTestid: "test-description-box",
			},
		});

		const descriptionBox = await wrapper.getByTestId('test-description-box');
		await expect(descriptionBox).toBeInTheDocument();
	});

	it("check passed array value not equal to selected model Value", () => {
		let initial = "Vue.js";
		var selected = initial;
		const wrapper = mount(ListBoxInput, {
			props: {
				options: options,
				modelValue: selected,
			},
		});
		const divArray = wrapper.componentVM.options;
		const element = divArray.find((item) => item.Value === 2);
		//check if text not matches with passed array value
		expect(element.Text).not.toEqual(initial);
	});

	it("check the emit update model value", async () => {
		let initial = "Angular";
		var selected = initial;
		const wrapper = mount(ListBoxInput, {
			props: {
				options: options,
				modelValue: selected,
				noOfItems: 5,
			},
		});
		wrapper.vm.$emit("update:modelValue", "Rails");
		expect(wrapper.emitted()["update:modelValue"]).toBeTruthy();
		expect(wrapper.emitted()["update:modelValue"][0]).toEqual(["Rails"]);
	});

	it("check no of items to display in list", () => {
		let initial = "Angular";
		var selected = initial;
		var numberofItems = 5;
		const wrapper = mount(ListBoxInput, {
			props: {
				options: options,
				modelValue: selected,
				noOfItems: numberofItems,
			},
		});
		//Checking no of items display props is equal
		expect(wrapper.componentVM.noOfItems).toEqual(numberofItems);
	});

	it("check the default no of items of the component is eqaul to 1", () => {
		let initial = "Angular";
		var selected = initial;
		var defaultSize = 1;
		var noOfItemsTodisplay = 5;
		const wrapper = mount(ListBoxInput, {
			props: {
				options: options,
				modelValue: selected,
				noOfItems: defaultSize,
				size: "small",
			},
		});
		//Checking no of items display props is equal
		expect(wrapper.componentVM.noOfItems).not.toBe(noOfItemsTodisplay);
		//Checking default no of items are equal to 1
		expect(wrapper.componentVM.noOfItems).toEqual(defaultSize);
	});

	it("Render and verify selected value of listbox", async () => {
		let initial = "Angular";
		var selected = initial;
		var numberofItems = 3;
		const wrapper = render(ListBoxInput, {
			props: {
				options: options,
				modelValue: selected,
				noOfItems: numberofItems,
			},
		});
		//get item by test id
		const listBoxItems = await wrapper.findByTestId("list-label-1");
		//fire event to target the value
		await fireEvent.update(listBoxItems, { target: { Text: "Rails" } });
		expect(wrapper.emitted()["update:modelValue"]);
	});

	it("check the size of the component", () => {
		let initial = "Angular";
		var selected = initial;
		var numberofItems = 0;
		var sizeOfComponent = "small";
		const wrapper = mount(ListBoxInput, {
			props: {
				options: options,
				modelValue: selected,
				noOfItems: 3,
				size: "small",
			},
		});
		//Checking no of items display props is equal
		expect(wrapper.componentVM.noOfItems).not.toBe(numberofItems);
		//Checking no of items display props is equal using class
		expect(wrapper.findAll(".i-card")).not.toBe(numberofItems);
		expect(wrapper.componentVM.size).toEqual(sizeOfComponent);
	});

	it("check tab, select and deselect(using backspace, delete key)", async () => {
		let initial = "Angular";
		var selected = initial;
		var expectedValue = "";
		var numberofItems = 3;
		const wrapper = render(ListBoxInput, {
			props: {
				options: [
					{
						Value: 1,
						Text: "Rails",
					},
					{
						Value: 2,
						Text: "Django",
					},
					{
						Value: 3,
						Text: "Angular",
					},
					{
						Value: 4,
						Text: "Vue.js",
					},
				],
				modelValue: selected,
				noOfItems: numberofItems,
			},
		});

		//get item by test id
		const listBoxItems = await wrapper.findByTestId("list-label-1");

		//Select the first item by keyboard
		await userEvent.tab() //focus ul
		await userEvent.tab() //focus first li
		expect(listBoxItems).toHaveFocus();
		await userEvent.keyboard('{Enter}')
		expect(wrapper).toEmitModelValue(1)
		await wrapper.rerender({ modelValue: 1 })

		//Use backspace to undo the current value
		await userEvent.keyboard('{Backspace}')
		expect(wrapper).toEmitModelValue(expectedValue)
		await wrapper.rerender({ modelValue: "" })

		//Fire keydown event for delete
		await userEvent.keyboard('{Delete}')
		expect(wrapper).toEmitModelValue(expectedValue)
		await wrapper.rerender({ modelValue: "" })

		//Iterate the elements with tab
		await userEvent.tab();
		const liSecondElement = screen.getByTestId("list-label-2");
		expect(liSecondElement).toHaveFocus();
		await userEvent.tab();
		const liThirdElement = screen.getByTestId("list-label-3");
		expect(liThirdElement).toHaveFocus();
		await userEvent.tab();
		const liLastElement = screen.getByTestId("list-label-4");
		expect(liLastElement).toHaveFocus();

		// If page contains more elements, then moves to next element, otherwise cycle goes back to the body element.
		await userEvent.tab();
		expect(document.body).toHaveFocus();
	});

	it('check validation of options list', function () {
		const inValidOptionsArray = [
			{
				Value: 1,
				Text: "Rails",
			},
			{
				Value: 2,
				Text: "Django",
			},
			{
				Value: 3,
				Text: "Angular",
			},
			{
				Value: 4,
			},
		];
		const validOptionsArray = [
			{
				Value: 1,
				Text: "Rails",
				Image: "test.jpg",
				Title: "Ruby on Rails Long",
				Description: "Long description for Ruby on Rails"
			},
			{
				Value: 2,
				Text: "Django",
				Image: "test.png",
				Title: "Django Long",
				Description: "Long description for Django"
			},
			{
				Value: 3,
				Text: "Angular",
				Image: "test.svg",
				Title: "Angular Long",
				Description: "Long description for Angular"
			},
			{
				Value: 4,
				Text: "Vue.js",
				Image: "test.png",
				Title: "Vue.js Long",
				Description: "Long description for Vue.js"
			},
		]
		const validator = ListBoxInput.props.options.validator
		//Expect validator to return false value as there is no value for fourth object of array
		expect(validator(inValidOptionsArray)).toBe(false);
		//Expect validator to return true value as there is proper object of array
		expect(validator(validOptionsArray)).toBe(true);
	});
});
