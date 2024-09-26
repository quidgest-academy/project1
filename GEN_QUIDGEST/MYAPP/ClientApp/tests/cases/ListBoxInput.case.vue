<template>
	<!-- This control with id CTRL_1 is for showing list items with by default unselected value-->
	<div
		class="container-fluid mt-3"
		style="flex: 1 1 auto">
		<base-input-structure
			id="CTRL_1"
			label="List box with model value default deselected">
			<q-list-box-input
				id="CTRL_1"
				size="xlarge"
				v-model="listUnselectedVar"
				:options="simpleListItemsForSize"
				:no-of-items="noOfItemsForSizeSmall">
			</q-list-box-input>
			<span
				v-if="noOfItemsForSizeSmall >= 0"
				class="ml-2 mb-2">
				Selected: {{ listUnselectedVar }}
			</span>
		</base-input-structure>
	</div>

	<!-- This control with id CTRL_2 is for showing list items with small size -->
	<div
		class="container-fluid mt-3"
		style="flex: 1 1 auto">
		<base-input-structure
			id="CTRL_2"
			label="List box with small size, no of items: 4 and total items: 4">
			<q-list-box-input
				id="CTRL_2"
				size="small"
				v-model="listSmallVar"
				:options="simpleListItemsForSize"
				:no-of-items="noOfItemsForSizeSmall">
			</q-list-box-input>
			<span
				v-if="noOfItemsForSizeSmall >= 0"
				class="ml-2 mb-2">
				Selected: {{ listSmallVar }}
			</span>
		</base-input-structure>
	</div>

	<!-- This control with id CTRL_3 is for showing list items with medium size -->
	<div
		class="container-fluid mt-3"
		style="flex: 1 1 auto">
		<base-input-structure
			id="CTRL_3"
			label="List box with medium size, no of items: 6 and total items: 4">
			<q-list-box-input
				id="CTRL_3"
				size="medium"
				v-model="listMediumVar"
				:options="simpleListItemsForSize"
				:no-of-items="noOfItemsForSizeMedium">
			</q-list-box-input>
			<span
				v-if="noOfItemsForSizeMedium >= 0"
				class="ml-2 mb-2">
				Selected: {{ listMediumVar }}
			</span>
		</base-input-structure>
	</div>

	<!-- This control with id CTRL_4 is for showing list items with large size -->
	<div
		class="container-fluid mt-3"
		style="flex: 1 1 auto">
		<base-input-structure
			id="CTRL_4"
			label="List box with large size, no of items: 3 and total items: 4">
			<q-list-box-input
				id="CTRL_4"
				size="large"
				v-model="listLargeVar"
				:options="simpleListItemsForSize"
				:no-of-items="noOfItemsForSizeLarge">
			</q-list-box-input>
			<span
				v-if="noOfItemsForSizeLarge >= 0"
				class="ml-2 mb-2">
				Selected: {{ listLargeVar }}
			</span>
		</base-input-structure>
	</div>

	<!-- This control with id CTRL_5 is for showing list items with no of items 4 -->
	<div
		class="container-fluid mt-3"
		style="flex: 1 1 auto">
		<base-input-structure
			id="CTRL_5"
			label="List box with X-Large size, no of items: 0 and total items: 7">
			<q-list-box-input
				id="CTRL_5"
				size="xlarge"
				v-model="listVar"
				:options="simpleListItems"
				:no-of-items="NumberOfItemsToShowOne">
			</q-list-box-input>
		</base-input-structure>
		<span
			v-if="NumberOfItemsToShowOne >= 0"
			class="ml-2 mb-2">
			Selected: {{ listVar }}
		</span>
	</div>

	<!-- This control with id CTRL_6 is for showing list items with default no of items -->
	<div
		class="container-fluid mt-3"
		style="flex: 1 1 auto">
		<base-input-structure
			id="CTRL_6"
			label="List box with default no of items. Default no of items is 1 and total items: 7">
			<q-list-box-input
				id="CTRL_6"
				size="xlarge"
				v-model="defaultNoOfItems"
				:options="simpleListItems">
			</q-list-box-input>
		</base-input-structure>
		<span
			v-if="NumberOfItemsToShowOne >= 0"
			class="ml-2 mb-2">
			Selected: {{ defaultNoOfItems }}
		</span>
	</div>

	<!-- This control with id CTRL_7 is for showing list items for lengthy values -->
	<div
		class="container-fluid mt-3"
		style="flex: 1 1 auto">
		<base-input-structure
			id="CTRL_7"
			label="List box with lengthy values">
			<q-list-box-input
				id="CTRL_7"
				size="xlarge"
				v-model="listVarData"
				:options="simpleList"
				:no-of-items="NumberOfItemsToShowTwo">
			</q-list-box-input>
		</base-input-structure>
		<span
			v-if="NumberOfItemsToShowTwo >= 0"
			class="ml-2 mb-2">
			Selected: {{ listVarData }}
		</span>
	</div>

	<!-- This control with id CTRL_8 is for showing list items with no of items 4 -->
	<div
		class="container-fluid mt-3"
		style="flex: 1 1 auto">
		<base-input-structure
			id="CTRL_8"
			label="List box with disable values">
			<q-list-box-input
				id="CTRL_8"
				size="xlarge"
				v-model="listVar"
				:options="simpleListItemsForSize"
				:no-of-items="noOfItemsForSizeXl"
				:readonly="readonly">
			</q-list-box-input>
		</base-input-structure>
	</div>

	<div
		class="container-fluid mt-3"
		style="flex: 1 1 auto">
		<base-input-structure
			id="CTRL_9"
			label="List box with description box without image">
			<q-list-box-input
				id="CTRL_9"
				size="xlarge"
				image-box
				v-model="listUnselectedVar"
				:options="simpleListItemsForSize"
				:no-of-items="noOfItemsForSizeSmall">
			</q-list-box-input>
			<span
				v-if="noOfItemsForSizeSmall >= 0"
				class="ml-2 mb-2">
				Selected: {{ listUnselectedVar }}
			</span>
		</base-input-structure>
	</div>

	<div
		class="container-fluid mt-3"
		style="flex: 1 1 auto">
		<base-input-structure
			id="CTRL_10"
			label="List box with description box with image">
			<q-list-box-input
				id="CTRL_10"
				size="xlarge"
				image-box
				has-image
				v-model="listUnselectedVar"
				:options="simpleListItemsForSize"
				:no-of-items="noOfItemsForSizeSmall">
			</q-list-box-input>
			<span
				v-if="noOfItemsForSizeSmall >= 0"
				class="ml-2 mb-2">
				Selected: {{ listUnselectedVar }}
			</span>
		</base-input-structure>
	</div>
</template>

<script>
	import fakeData from './ListBoxInput.mock'

	export default {
		name: 'QListBoxContainer',

		docsfile: './docs/inputs/ListBoxInput.md',

		inheritAttrs: false,

		expose: [],

		data()
		{
			return fakeData.simpleUsage()
		}
	}
</script>
