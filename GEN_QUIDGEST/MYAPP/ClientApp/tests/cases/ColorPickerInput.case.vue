<template>
	<h1>Color picker</h1>

	<span>
		The color picker input allows users to select colors either through text input or by using a color palette. It features a toggle button to
		show/hide the color picker, supports various color formats (Hex or RGBA), and emits events to notify the parent component of color changes.
		The component also offers customization options for theme, default colors, and a sucker tool, making it a versatile tool for color selection
		in web applications.
	</span>

	<hr />

	<h3>Usage</h3>

	<div class="demo-container">
		<div class="demo-main-container">
			<div>
				<q-card>
					<template #[`content.append`]>
						<base-input-structure
							id="CTRL_1"
							label="Hexadecimal Color">
							<q-color-picker-input :color="colorHexa" />
						</base-input-structure>
						<base-input-structure
							id="CTRL_2"
							label="RGBA Color">
							<q-color-picker-input :color="colorRGBA" />
						</base-input-structure>
						<base-input-structure
							id="CTRL_3"
							label="Theme">
							<q-color-picker-input
								:color="colorRGBA"
								:theme="theme" />
						</base-input-structure>
						<base-input-structure
							id="CTRL_4"
							label="Available Colors">
							<q-color-picker-input
								:color="colorRGBA"
								:colors-defaults="setColors" />
						</base-input-structure>
					</template>
				</q-card>
			</div>
		</div>
	</div>
</template>

<script>
	export default {
		name: 'QColorPickerContainer',

		docsfile: './docs/inputs/ColorPickerInput.md',

		inheritAttrs: false,

		expose: [],

		data()
		{
			return {
				colorHexa: '#F123F0',
				colorRGBA: 'rgba(50, 0, 255, 0.5)',
				theme: 'dark',
				setColors: [
					'#FF0000',
					'#FF5733',
					'#B6FF33',
					'#33FFF1',
					'#FF33C7',
					'#FF3333',
					'#838470',
					'#367058',
					'#B9B5EC',
					'#ECB5E7',
					'#5D61FF',
					'#FF89CF',
					'#FC3CAD',
					'#BF3DCE',
					'#8E00A7',
					'rgba(133, 133, 133, 0.7)'
				]
			}
		}
	}
</script>
