<template>
	<div :id="id" :style="style" :class="['c-groupbox', { 'zone-no-border': noBorder }]" v-show="isVisible">
		<div class="c-groupbox__title" :help-id="id" :label-id="id" v-if="label">
			<div v-if="labelAlign === 'center'" style="text-align: center;">
                {{ label }}
            </div>
            <div v-else-if="labelAlign === 'right'" style="text-align: right;">
                {{ label }}
            </div>
            <template v-else>
                {{ label }}
            </template>
		</div>
		<div class="form-flow">
			<slot></slot>
		</div>
	</div>
</template>

<script>
	export default {
		name: 'QGroupBoxContainer',

		inheritAttrs: false,

		props: {
			id: {
				type: String,
				default: null
			},

			label: {
				type: String,
				default: null
			},

            /* Either "left", "center" or "right"*/
            labelAlign: {
                type: String,
                default: "left"
            },

			style: {
				type: String,
				default: null
			},

			/**
			 * Visible property to hide and show group
			 */
			isVisible: {
				type: Boolean,
				default: true
			},

			noBorder: {
				type: Boolean,
				default: false
			}
		}
	}
</script>