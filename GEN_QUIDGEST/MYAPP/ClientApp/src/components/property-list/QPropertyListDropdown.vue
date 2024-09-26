<template>
	<div :class="['propertyname-cell', 'd-flex', { focus: isEditMode }]">
		<q-text-field
			size="small"
			data-testid="property-input"
			v-model="curValue"
			:style="dropdownStyles"
			:readonly="!isEditMode"
			@keydown.stop="editPropertyLabel" />

		<div
			v-if="showDropdown"
			class="nested__dropdown"
			:aria-hidden="!showDropdown">
			<a
				href="javascript:void(0)"
				@click.stop.prevent="toggleDropdown">
				<span
					style="cursor: pointer"
					:aria-hidden="!showDropdown"
					:title="texts.dropdownTitle">
					<q-icon icon="expand" />
				</span>
			</a>

			<ul
				v-show="openDropdown"
				role="listbox"
				data-testid="nestedDropdownContainer"
				class="dropdown-list"
				:aria-expanded="openDropdown">
				<li
					v-for="(item, i) in menuItems"
					role="option"
					:key="i"
					:class="[{ 'dropdown-submenu': item.children }, 'dropdown_item']"
					@click.stop.prevent="openChild(item, e)">
					{{ item.name }}

					<span
						v-if="item.children"
						style="cursor: pointer"
						:title="texts.dropdownTitle">
						<q-icon icon="step-forward" />
					</span>

					<ul
						v-if="item.children && openSubmenuDropdown"
						class="dropdown-list submenu">
						<li
							v-for="(child, j) in item.children"
							class="dropdown_item"
							:key="j"
							@click.stop.prevent="selectedType(child)">
							{{ child.name }}
						</li>
					</ul>
				</li>
			</ul>
		</div>
	</div>
</template>

<script>
	export default {
		name: 'QPropertyListDropdown',

		emits: [
			'select-type',
			'select-action',
			'updated-property-label',
			'update:modelValue'
		],

		props: {
			/**
			 * The list of menu items.
			 */
			menuItems: {
				type: Array,
				default: () => {
					return []
				}
			},

			/**
			 * The string vaue to be edited by the input.
			 */
			modelValue: {
				type: String,
				required: true
			},

			/**
			 * Necessary strings to be used in labels and buttons.
			 */
			texts: {
				type: Object,
				required: true
			},

			/**
			 * Whether or not the dropdown should be visible.
			 */
			showDropdown: {
				type: Boolean,
				default: false
			}
		},

		expose: [],

		data()
		{
			return {
				openDropdown: false,
				openSubmenuDropdown: false,
				isEditMode: false
			}
		},

		mounted()
		{
			document.addEventListener('click', this.closeDropdown)
		},

		beforeUnmount()
		{
			document.removeEventListener('click', this.closeDropdown)
		},

		computed: {
			dropdownStyles()
			{
				if (!this.isEditMode)
				{
					return {
						border: 'none',
						background: 'transparent',
						color: '#404b69'
					}
				}

				return {
					border: '1px solid #c2c8cc'
				}
			},

			curValue: {
				get()
				{
					return this.modelValue
				},
				set(newValue)
				{
					this.$emit('update:modelValue', newValue)
				}
			}
		},

		methods: {
			editPropertyLabel(e)
			{
				switch (e.which)
				{
					// Enter
					case 13:
						this.$emit('updated-property-label', this.curValue)
						this.isEditMode = false
						break
					default:
						break
				}
			},

			selectedType(type)
			{
				this.$emit('select-type', type)
				this.openDropdown = false
			},

			openChild(item)
			{
				if (item.children)
					this.openSubmenuDropdown = !this.openSubmenuDropdown
				else
				{
					if (item.action === 'edit')
						this.isEditMode = !this.isEditMode

					this.$emit('select-action', item)
					this.openDropdown = false
				}
			},

			toggleDropdown()
			{
				this.openSubmenuDropdown = false
				this.openDropdown = !this.openDropdown
			},

			closeDropdown(e)
			{
				if (!this.$el.contains(e.target))
				{
					this.openSubmenuDropdown = false
					this.openDropdown = false
				}
			}
		}
	}
</script>
