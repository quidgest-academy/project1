<template>
	<div :class="listBoxClasses">
		<div
			v-show="noOfItems >= 0"
			:id="controlId"
			:class="[styleClass, 'card', 'i-list-box']"
			:style="{ 'max-height': height !== '0rem' ? height : '', 'overflow-y': 'auto' }">
			<ul
				class="list-group i-list-group"
				tabindex="1"
				data-testid="ulElement"
				role="listbox">
				<template
					v-for="option in options"
					:key="`${controlId}_${option.Value}`">
					<li
						:id="`list-label-${elementUid}-${option.Value}`"
						role="option"
						:title="option.Value"
						:tabindex="readonly ? '-1' : '1'"
						:data-testid="`list-label-${option.Value}`"
						:class="[
							{
								active: curValue === option.Value,
								disabled: readonly
							},
							'list-group-item',
							'i-cursor'
						]"
						@click.stop.prevent="changeInfo(option.Value, $event)"
						@keydown="changeInfo(option.Value, $event)">
						{{ option.Text }}
					</li>
				</template>
			</ul>
		</div>

		<div
			v-if="imageBox"
			class="q-list-box__details-box list-group-item"
			data-testid="test-description-box">
			<template v-if="selectedItemDetails">
				<img
					v-if="selectedItemDetails.Image"
					class="q-list-box__image-box"
					:src="selectedItemDetails.Image"
					:alt="selectedItemDetails.alt" />
				<h3>{{ selectedItemDetails.Title }}</h3>
				<p>{{ selectedItemDetails.Description }}</p>
			</template>
		</div>
	</div>
</template>

<script>
	export default {
		name: 'QListBox',

		emits: ['update:modelValue'],

		inheritAttrs: false,

		props: {
			/**
			 * Unique identifier for the control.
			 */
			id: String,

			/**
			 * Size property to define size of component
			 */
			size: String,

			/**
			 * Holds the selection result
			 */
			modelValue: [String, Number],

			/**
			 * Array containing the enumeration
			 */
			options: {
				type: Array,
				required: true,
				validator: (prop) => prop.every((e) => Object.keys(e).includes('Text') && Object.keys(e).includes('Value'))
			},

			/**
			 * This property defines the no of items has to display in the list, default value is 1
			 */
			noOfItems: {
				type: Number,
				default: 1
			},

			/**
			 * Readonly property to to show list only in readonly mode, no selection method can trigger
			 */
			readonly: {
				type: Boolean,
				default: false
			},

			/**
			 * Defines if details box should appear or not
			 */
			imageBox: {
				type: Boolean,
				default: false
			},

			/**
			 * Defines if details box has image or not
			 */
			hasImage: {
				type: Boolean,
				default: false
			}
		},

		// TODO: Remove these properties from the "expose" (only necessary for unit tests).
		expose: [
			'noOfItems',
			'options',
			'size'
		],

		data()
		{
			return {
				controlId: this.id || `list-box-${this._.uid}`,
				styleClass: this.size ? `input-${this.size}` : '',
				elementUid: this._.uid,
				height: `${this.noOfItems * 3}rem`
			}
		},

		computed: {
			listBoxClasses()
			{
				const lbPrefix = 'q-list-box'
				const classes = [lbPrefix]

				if (this.hasImage)
					classes.push(`${lbPrefix}--has-image`)

				return classes
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
			},

			selectedItemDetails()
			{
				if (this.imageBox && Array.isArray(this.options))
				{
					const selectedItem = this.options.find((opt) => opt.Value === this.curValue)

					if (typeof selectedItem === 'object')
					{
						return {
							Image: selectedItem.Image,
							Title: selectedItem.Title,
							Description: selectedItem.Description,
							alt: selectedItem.alt
						}
					}
				}

				return null
			}
		},

		methods: {
			/**
			 * Change the selected list item
			 * @param {any} option The value of the selected option
			 * @param {object} e The event
			 */
			changeInfo(option, e)
			{
				const key = e.key

				// Select / deselect
				if (e.type === 'click' || key === 'Enter')
					this.curValue = this.curValue === option ? '' : option
				else if (key === 'Backspace' || key === 'Delete')
					this.curValue = ''
			}
		}
	}
</script>
