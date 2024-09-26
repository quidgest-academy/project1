<template>
	<q-button
		v-if="visibleOptions?.length === 1 && singleOptionButton"
		:id="id"
		b-style="secondary"
		borderless
		:disabled="disabled"
		:class="[{ disabled: options[0].active === false }]"
		:title="options[0].text"
		:label="texts.label"
		@click="$emit('selected', options[0].id)">
		<q-icon
			v-if="icon"
			:icon="icon" />
	</q-button>
	<div
		v-else
		:id="`${id}-container`">
		<q-toggle-dropdown
			:id="id"
			aria-expanded="false"
			aria-haspopup="true"
			b-style="secondary"
			v-bind="buttonOptions"
			:disabled="disabled"
			:class="buttonClasses"
			:title="texts.title"
			:label="texts.label">
			<q-icon
				v-if="icon"
				:icon="icon" />
			<slot></slot>
		</q-toggle-dropdown>

		<div
			ref="dropdown"
			x-placement="top-end"
			role="menu"
			:class="[
				'dropdown-menu',
				{
					'dropdown-menu-right': dropdownAlignment === 'right',
					'dropdown-menu-left': dropdownAlignment === 'left'
				}
			]">
			<template
				v-for="(option, index) in visibleOptions"
				:key="index">
				<hr v-if="option.separatorBefore && index" />

				<q-button
					:id="option.elementId ? id + '-' + option.elementId : null"
					role="menuitem"
					borderless
					:disabled="disabled || option.active === false"
					:title="option.text"
					:label="option.text"
					@click="clickDropdown(option)">
					<q-icon
						v-if="option.icon"
						v-bind="option.icon" />
				</q-button>

				<hr v-if="option.separatorAfter" />
			</template>
		</div>
	</div>
</template>

<script>
	import filter from 'lodash-es/filter'

	import QToggleDropdown from '@/components/QToggleDropdown.vue'

	export default {
		name: 'QDropdownMenu',

		emits: ['selected'],

		components: {
			QToggleDropdown
		},

		props: {
			/**
			 * Unique identifier for the control.
			 */
			id: {
				type: String,
				required: true
			},

			/**
			 * The menu icon.
			 */
			icon: {
				type: String,
				default: ''
			},

			/**
			 *
			 */
			texts: {
				type: Object,
				required: true
			},

			/**
			 * Whether or not the menu is in blocked mode.
			 */
			disabled: {
				type: Boolean,
				default: false
			},

			/**
			 * The menu options.
			 */
			options: {
				type: Array,
				default: () => []
			},

			/**
			 * A list of classes for the buttons.
			 */
			buttonClasses: {
				type: Array,
				default: () => []
			},

			/**
			 * Options for the buttons.
			 */
			buttonOptions: {
				type: Object,
				default: () => ({})
			},

			/**
			 * Whether or not to have a single button to display the options.
			 */
			singleOptionButton: {
				type: Boolean,
				default: true
			},

			/**
			 * Context menu alignment (for now, it only supports left and right).
			 */
			dropdownAlignment: {
				type: String,
				default: ''
			}
		},

		expose: [],

		computed: {
			visibleOptions()
			{
				return filter(this.options, (option) => option.visible || option.visible === undefined)
			}
		},

		methods: {
			clickDropdown(option)
			{
				this.$emit('selected', option.id)

				if (this.$refs.dropdown)
					this.$refs.dropdown.click()
			}
		}
	}
</script>
