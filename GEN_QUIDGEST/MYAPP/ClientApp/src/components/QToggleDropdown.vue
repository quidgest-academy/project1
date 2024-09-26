<template>
	<q-button
		ref="dropdownBtn"
		data-toggle="dropdown"
		:disabled="disabled"
		@click="toggleDropdown">
		<slot></slot>
	</q-button>
</template>

<script>
	import { Dropdown } from 'bootstrap'

	export default {
		name: 'QToggleDropdown',

		emits: ['toggle-dropdown'],

		props: {
			/**
			 * Indicates if the button is disabled.
			 */
			disabled: {
				type: Boolean,
				default: false
			}
		},

		expose: [],

		mounted()
		{
			// Instead of propagating the action to the DOM,
			// initialize the dropdown content manually.
			let obj = { each: (callback) => callback.call(this.$refs.dropdownBtn.$el) }
			Dropdown._jQueryInterface.call(obj)
		},

		methods: {
			toggleDropdown(e)
			{
				this.$emit('toggle-dropdown', e)
			}
		}
	}
</script>
