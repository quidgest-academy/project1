<template>
	<component
		v-if="component"
		:is="component"
		v-bind="customComponentAttrs" />
	<div
		v-else-if="isEmptyValue"
		:data-field-value="true">
		--
	</div>
	<component
		v-else
		:is="basicDisplayElement"
		:style="basicDisplayElementStyle"
		:class="basicDisplayElementClasses">
		<span :data-field-value="true">
			{{ value }}
		</span>
	</component>
</template>

<script>
	import _isObjectLike from 'lodash-es/isObjectLike'
	import _isArray from 'lodash-es/isArray'
	import _some from 'lodash-es/some'

	export default {
		name: 'QRenderData',

		inheritAttrs: false,

		props: {
			/**
			 * Optional component name for Vue's dynamic component. If provided, the specified component is rendered.
			 */
			component: {
				type: String,
				default: ''
			},

			/**
			 * The data value to be displayed. Can be any type.
			 */
			value: {
				type: null,
				default: () => ({})
			},

			/**
			 * A background color to apply to the basic display element (span). If specified, the span with a background color is used.
			 */
			backgroundColor: {
				type: String,
				default: ''
			}
		},

		expose: [],

		computed: {
			/**
			 * Returns a combined list of component attributes including props and attrs.
			 */
			customComponentAttrs()
			{
				return {
					...this.$props,
					...this.$attrs
				}
			},

			/**
			 * Determines the basic display element based on whether a background color is provided.
			 * If a background color is provided, a span is used; otherwise, a fragment is used for unwrapping the content.
			 */
			basicDisplayElement()
			{
				return this.backgroundColor ? 'span' : 'v-fragment'
			},

			/**
			 * Returns the class list for the basic display element, applying e-badge if a background color is set.
			 */
			basicDisplayElementClasses()
			{
				return this.backgroundColor ? ['e-badge'] : []
			},

			/**
			 * Returns the inline style for the basic display element, setting the background color if one is provided.
			 */
			basicDisplayElementStyle()
			{
				return this.backgroundColor
					? { 'background-color': this.backgroundColor }
					: {}
			},

			/**
			 * Checks if the value is empty. It analyses objects differently from primitive values.
			 * It returns true if the value is an object with all undefined, null, or empty values,
			 * or if the value is a primitive that is undefined, null, or empty.
			 */
			isEmptyValue()
			{
				if (_isObjectLike(this.value) && !_isArray(this.value))
					return !_some(this.value, (value) => !(value === undefined || value === null || value === ''))

				return this.value === undefined || this.value === null || this.value === ''
			}
		}
	}
</script>
