<template>
	<div
		id="cookies"
		class="q-cookies">
		<div>
			<div>
				<a
					:href="filePath"
					target="blank"
					class="q-cookies-field">
					{{ text }}
				</a>
				<q-button
					b-style="secondary"
					class="q-cookies-button"
					@click="setCookieStorage">
					{{ buttonText }}
				</q-button>
			</div>
		</div>
	</div>
</template>

<script>

	export default{
		name: "QCookies",

		expose: [
			'setCookie',
		],

		emits: ['set-cookie'],

		props:{
			/**
			 * The text to be used in the body of the component
			 */
			text:{
				type: String,
				required: true
			},

			/**
			 * the text in the cookies button
			 */
			buttonText:{
				type: String,
				required: true
			},
            
			/**
			 * Path of the file to be used
			 */
			filePath:{
				type: String,
				required: true
			},
		},

		data() {
			return {
				isVisible: true,
			}
		},

		methods: {
			setCookieStorage() 
			{
				this.$emit('set-cookie', true);
			},
		},
	}
</script>