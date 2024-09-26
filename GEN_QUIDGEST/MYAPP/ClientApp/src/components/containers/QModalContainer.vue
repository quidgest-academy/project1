<template>
	<div
		:id="controlId"
		class="c-modal container-fluid"
		:style="{ display: isActive ? 'block' : 'none' }"
		:data-backdrop="backdrop">
		<div
			ref="dialog"
			:class="['c-modal__dialog', widthClass, classes]"
			tabindex="0"
			role="dialog"
			aria-modal="true">
			<div class="c-modal__content">
				<div
					v-show="!hideHeader"
					class="c-modal__header">
					<h5
						v-if="headerTitle"
						class="c-modal__header-title">
						{{ headerTitle }}
					</h5>

					<q-button
						v-if="closeButtonEnable"
						borderless
						size="small"
						b-style="plain"
						class="q-modal-container__close"
						:title="texts.close"
						@click="dismissModal">
						<q-icon icon="close" />
					</q-button>

					<slot name="header"></slot>
				</div>

				<div
					v-show="!hideBody"
					class="c-modal__body">
					<slot name="body"></slot>
				</div>

				<div
					v-show="!hideFooter"
					class="c-modal__footer">
					<slot name="footer"></slot>
				</div>
			</div>
		</div>
	</div>
</template>

<script>
	import { computed } from 'vue'

	import hardcodedTexts from '@/hardcodedTexts.js'

	const modalWidths = ['sm', 'md', 'lg', 'xl']

	export default {
		name: 'QModal',

		emits: ['dismiss', 'is-ready'],

		inheritAttrs: false,

		props: {
			/**
			 * Unique identifier for the control.
			 */
			id: String,

			/**
			 * The title for the modal header.
			 */
			headerTitle: {
				type: String,
				default: ''
			},

			/**
			 * Whether the modal is active or not.
			 */
			isActive: {
				type: Boolean,
				default: false
			},

			/**
			 * Enable or disable the close button.
			 */
			closeButtonEnable: {
				type: Boolean,
				default: false
			},

			/**
			 * The backdrop option for the modal.
			 */
			backdrop: {
				type: String,
				default: 'static'
			},

			/**
			 * Whether to dismiss the modal with the 'Escape' key.
			 */
			dismissWithEsc: {
				type: Boolean,
				default: false
			},

			/**
			 * The width of the modal.
			 */
			modalWidth: {
				type: String,
				default: 'lg',
				validator: (value) => modalWidths.includes(value)
			},

			/**
			 * Whether to hide the modal header.
			 */
			hideHeader: {
				type: Boolean,
				default: false
			},

			/**
			 * Whether to hide the modal body.
			 */
			hideBody: {
				type: Boolean,
				default: false
			},

			/**
			 * Whether to hide the modal footer.
			 */
			hideFooter: {
				type: Boolean,
				default: false
			},

			/**
			 * Additional classes to apply to the modal.
			 */
			classes: {
				type: Array,
				default: () => []
			}
		},

		expose: [],

		data()
		{
			return {
				controlId: this.id || `q-modal-${this._.uid}`,

				texts: {
					close: computed(() => this.Resources[hardcodedTexts.close])
				},

				initFocused: false
			}
		},

		mounted()
		{
			if (this.dismissWithEsc)
				window.addEventListener('keydown', this.handleKeyPress)

			this.setOverflow(this.isActive)
			this.$emit('is-ready', this.controlId)

			this.initFocus()
		},

		updated()
		{
			// For popup forms called by a route, this must be called here
			// Focus on the dialog element
			// Needed for the user to be able to focus on the elements within by keyboard
			// without tabbing through all other elements on the page
			// Also lets screen readers read the dialog contents
			this.initFocus()
		},

		beforeUnmount()
		{
			if (this.dismissWithEsc)
				window.removeEventListener('keydown', this.handleKeyPress)

			this.setOverflow(false)
		},

		computed: {
			/**
			 * The width class based on the modal width prop.
			 */
			widthClass()
			{
				return `c-modal--${this.modalWidth}`
			}
		},

		methods: {
			/**
			 * Dismiss the modal.
			 */
			dismissModal()
			{
				this.$emit('dismiss', this.id)
			},

			/**
			 * Handle key press events (e.g., 'Escape' key).
			 * @param {KeyboardEvent} event - The keyboard event object.
			 */
			handleKeyPress(event)
			{
				if (!this.dismissWithEsc)
					return

				if (event.key === 'Escape')
					this.dismissModal()
			},

			/**
			 * Set overflow style on the body based on modal visibility.
			 * @param {boolean} isHidden - Whether the modal is hidden or not.
			 */
			setOverflow(isHidden)
			{
				if (isHidden)
					document.body.style.setProperty('overflow', 'hidden')
				else
					document.body.style.removeProperty('overflow')
			},

			/**
			 * Focus on the popup if it wasn't already focused.
			 */
			initFocus()
			{
				// If the popup wasn't already focused
				if (!this.initFocused)
				{
					// Try to focus on the main popup element
					this.$refs?.dialog?.focus()
					// If the popup element was focused, set the property to prevent focusing multiple times
					if (document.activeElement === this.$refs?.dialog)
						this.initFocused = true
				}
			}
		},

		watch: {
			dismissWithEsc(val)
			{
				if (val)
					window.addEventListener('keydown', this.handleKeyPress)
				else
					window.removeEventListener('keydown', this.handleKeyPress)
			},

			isActive(val)
			{
				this.setOverflow(val)
			}
		}
	}
</script>
