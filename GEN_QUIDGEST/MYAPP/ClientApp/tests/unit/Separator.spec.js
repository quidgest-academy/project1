// QSeparator.spec.js
import { shallowMount } from '@vue/test-utils'
import QSeparator from '@/components/rendering/QSeparator.vue'

describe('QSeparator.vue', () => {
	let wrapper;

	beforeEach(() => {
		wrapper = shallowMount(QSeparator);
	});

	it('renders without crashing', () => {
		expect(wrapper.exists()).toBe(true);
	});

	it('computes separatorClasses correctly', async () => {
		// Set props
		await wrapper.setProps({
			sepStyle: 'primary',
			elevated: true,
			vertical: true,
			size: 'big'
		});

		// Check computed classes
		expect(wrapper.vm.separatorClasses).toContain('q-sep--primary');
		expect(wrapper.vm.separatorClasses).toContain('q-sep--elevated');
		expect(wrapper.vm.separatorClasses).toContain('q-sep--vertical');
		expect(wrapper.vm.separatorClasses).toContain('q-sep--size-big');
	});

	// Test for defaults and other cases based on the internal logic

	it('defaults vertical to false', () => {
		expect(wrapper.vm.vertical).toBe(false);
	});

	it('defaults size to empty string', () => {
		expect(wrapper.vm.size).toBe('');
	});

	it('renders vertical separator correctly', async () => {
		await wrapper.setProps({ vertical: true });
		const separatorDiv = wrapper.find('div');
		expect(separatorDiv.classes()).toContain('q-sep--vertical');
	});

});