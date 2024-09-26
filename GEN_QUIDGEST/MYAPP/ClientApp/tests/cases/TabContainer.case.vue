<template>
	<!-- Tabs with left aligned property and initially first tab selected -->
	<div
		class="container-fluid mt-3 mb-3"
		style="flex: 1 1 auto">
		<base-input-structure
			id="CTRL_1"
			label="Tabs with left aligned property and initially first tab selected">
			<q-tab-container
				is-visible
				:selected-tab="tabs1.selectedTabInitial"
				:tabs-list="tabs1.simpleTabList"
				:align-tabs="tabs1.alignValue"
				:icon-alignment="tabs1.iconAlignmentValue"
				@tab-changed="changeTab('tabs1', 'selectedTabInitial', $event)">
				<template #tab-panel>
					<section v-show="tabs1.selectedTabInitial === 1">
						<strong>Tab One</strong> and it's content goes here ....
					</section>
					<section v-show="tabs1.selectedTabInitial === 2">
						<strong>Tab Two</strong> and it's content goes here ....
					</section>
					<section v-show="tabs1.selectedTabInitial === 3">
						<strong>Tab Three</strong> and it's content goes here ....
					</section>
					<section v-show="tabs1.selectedTabInitial === 4">
						<strong>Tab Four</strong> and it's content goes here ....
					</section>
				</template>
			</q-tab-container>
		</base-input-structure>
	</div>

	<!-- Tabs with justify aligned property and initially second tab selected -->
	<div
		class="container-fluid mt-3 mb-3"
		style="flex: 1 1 auto">
		<base-input-structure
			id="CTRL_2"
			label="Tabs with justify aligned property and initially second tab selected">
			<q-tab-container
				is-visible
				:selected-tab="tabs2.selectedTab"
				:tabs-list="tabs2.simpleTabList"
				:align-tabs="tabs2.alignTabsJustify"
				:icon-alignment="tabs2.iconAlignmentValue"
				@tab-changed="changeTab('tabs2', 'selectedTab', $event)">
				<template #tab-panel>
					<section v-show="tabs2.selectedTab === 1">
						<strong>Tab One</strong> and it's content goes here ....
					</section>
					<section v-show="tabs2.selectedTab === 2">
						<strong>Tab Two</strong> and it's content goes here ....
					</section>
					<section v-show="tabs2.selectedTab === 3">
						<strong>Tab Three</strong> and it's content goes here ....
					</section>
					<section v-show="tabs2.selectedTab === 4">
						<strong>Tab Four</strong> and it's content goes here ....
					</section>
				</template>
			</q-tab-container>
		</base-input-structure>
	</div>

	<!-- Tabs with icon property and icon left aligned -->
	<div
		class="container-fluid"
		style="flex: 1 1 auto">
		<base-input-structure
			id="CTRL_3"
			label="Tabs with icon property and icon left aligned">
			<q-tab-container
				is-visible
				:selected-tab="tabs3.selectedTabInitial"
				:tabs-list="tabs3.simpleTabList"
				:align-tabs="tabs3.alignValue"
				:icon-alignment="tabs3.iconAlignmentValue"
				@tab-changed="changeTab('tabs3', 'selectedTabInitial', $event)">
				<template #tab-panel>
					<section v-show="tabs3.selectedTabInitial === 1">
						<strong>Tab One</strong> and it's content goes here ....
					</section>
					<section v-show="tabs3.selectedTabInitial === 2">
						<strong>Tab Two</strong> and it's content goes here ....
					</section>
					<section v-show="tabs3.selectedTabInitial === 3">
						<strong>Tab Three</strong> and it's content goes here ....
					</section>
					<section v-show="tabs3.selectedTabInitial === 4">
						<strong>Tab Four</strong> and it's content goes here ....
					</section>
				</template>
			</q-tab-container>
		</base-input-structure>
	</div>

	<!-- Tabs with icon property and icon right aligned -->
	<div
		class="container-fluid mt-3"
		style="flex: 1 1 auto">
		<base-input-structure
			id="CTRL_4"
			label="Tabs with icon property and icon right aligned">
			<q-tab-container
				is-visible
				:selected-tab="tabs4.selectedTab"
				:tabs-list="tabs4.simpleTabList"
				:align-tabs="tabs4.alignValue"
				:icon-alignment="tabs4.iconAlignmentRight"
				@tab-changed="changeTab('tabs4', 'selectedTab', $event)">
				<template #tab-panel>
					<section v-show="tabs4.selectedTab === 1">
						<strong>Tab One</strong> and it's content goes here ....
					</section>
					<section v-show="tabs4.selectedTab === 2">
						<strong>Tab Two</strong> and it's content goes here ....
					</section>
					<section v-show="tabs4.selectedTab === 3">
						<strong>Tab Three</strong> and it's content goes here ....
					</section>
					<section v-show="tabs4.selectedTab === 4">
						<strong>Tab Four</strong> and it's content goes here ....
					</section>
				</template>
			</q-tab-container>
		</base-input-structure>
	</div>

	<!-- Tabs without icon property -->
	<div
		class="container-fluid mt-3"
		style="flex: 1 1 auto">
		<base-input-structure
			id="CTRL_5"
			label="Tabs without icon property">
			<q-tab-container
				is-visible
				:selected-tab="tabs5.selectedTabInitial"
				:tabs-list="tabs5.tabsListWithoutIcons"
				@tab-changed="changeTab('tabs5', 'selectedTabInitial', $event)">
				<template #tab-panel>
					<section v-show="tabs5.selectedTabInitial === 1">
						<strong>Tab One</strong> and it's content goes here ....
					</section>
					<section v-show="tabs5.selectedTabInitial === 2">
						<strong>Tab Two</strong> and it's content goes here ....
					</section>
					<section v-show="tabs5.selectedTabInitial === 3">
						<strong>Tab Three</strong> and it's content goes here ....
					</section>
					<section v-show="tabs5.selectedTabInitial === 4">
						<strong>Tab Four</strong> and it's content goes here ....
					</section>
				</template>
			</q-tab-container>
		</base-input-structure>
	</div>

	<!-- Tabs with isVisible property(tab 3 is not visible) -->
	<div
		class="container-fluid mt-3"
		style="flex: 1 1 auto">
		<base-input-structure
			id="CTRL_6"
			label="Tabs with isVisible property(tab 3 is not visible)">
			<q-tab-container
				is-visible
				:selected-tab="tabs6.selectedTabInitial"
				:tabs-list="tabs6.tabsListWithVisible"
				@tab-changed="changeTab('tabs6', 'selectedTabInitial', $event)">
				<template #tab-panel>
					<section v-show="tabs6.selectedTabInitial === 1">
						<strong>Tab One</strong> and it's content goes here ....
					</section>
					<section v-show="tabs6.selectedTabInitial === 2">
						<strong>Tab Two</strong> and it's content goes here ....
					</section>
					<section v-show="tabs6.selectedTabInitial === 3">
						<strong>Tab Three</strong> and it's content goes here ....
					</section>
					<section v-show="tabs6.selectedTabInitial === 4">
						<strong>Tab Four</strong> and it's content goes here ....
					</section>
				</template>
			</q-tab-container>
		</base-input-structure>
	</div>

	<!-- Tabs with disabled property -->
	<div
		class="container-fluid mt-3"
		style="flex: 1 1 auto">
		<base-input-structure
			id="CTRL_7"
			label="Tabs with disabled property (Tab Two and Tab Four are disabled)">
			<q-tab-container
				is-visible
				:selected-tab="tabs7.selectedTabInitial"
				:tabs-list="tabs7.tabsListDisabled"
				@tab-changed="changeTab('tabs7', 'selectedTabInitial', $event)">
				<template #tab-panel>
					<section v-show="tabs7.selectedTabInitial === 1">
						<strong>Tab One</strong> and it's content goes here ....
					</section>
					<section v-show="tabs7.selectedTabInitial === 2">
						<strong>Tab Two</strong> and it's content goes here ....
					</section>
					<section v-show="tabs7.selectedTabInitial === 3">
						<strong>Tab Three</strong> and it's content goes here ....
					</section>
					<section v-show="tabs7.selectedTabInitial === 4">
						<strong>Tab Four</strong> and it's content goes here ....
					</section>
				</template>
			</q-tab-container>
		</base-input-structure>
	</div>

	<!-- Tabs with lengthy tab contents -->
	<div
		class="container-fluid mt-3"
		style="flex: 1 1 auto">
		<base-input-structure
			id="CTRL_8"
			label="Tabs with lengthy tab content">
			<q-tab-container
				is-visible
				:selected-tab="tabs8.selectedTabInitial"
				:tabs-list="tabs8.simpleTabList"
				@tab-changed="changeTab('tabs8', 'selectedTabInitial', $event)">
				<template #tab-panel>
					<section v-show="tabs8.selectedTabInitial === 1">
						<strong>Tab One</strong> and it's content goes here ....
						<strong>Vue (pronounced /vjuː/, like view) </strong> is a progressive
						framework for building user interfaces. Unlike other monolithic
						frameworks, Vue is designed from the ground up to be incrementally
						adoptable. The core library is focused on the view layer only, and is
						easy to pick up and integrate with other libraries or existing
						projects. On the other hand, Vue is also perfectly capable of powering
						sophisticated Single-Page Applications when used in combination with
						modern tooling and supporting libraries. We have already created our
						very first Vue app! This looks pretty similar to rendering a string
						template, but Vue has done a lot of work under the hood. The data and
						the DOM are now linked, and everything is now reactive. How do we
						know? Take a look at the example below where counter property
						increments every second and you will see how rendered DOM changes:
					</section>
					<section v-show="tabs8.selectedTabInitial === 2">
						<strong>Tab Two</strong> and it's content goes here ....
						<strong>Angular </strong> (commonly referred to as "Angular 2+" or
						"Angular v2 and above")[4][5] is a TypeScript-based free and
						open-source web application framework led by the Angular Team at
						Google and by a community of individuals and corporations. Angular is
						a complete rewrite from the same team that built AngularJS. Angular is
						used as the frontend of the MEAN stack, consisting of MongoDB
						database, Express.js web application server framework, Angular it'self
						(or AngularJS), and Node.js server runtime environment.Google designed
						Angular as a ground-up rewrite of AngularJS. Angular does not have a
						concept of "scope" or controllers; instead, it uses a hierarchy of
						components as it's primary architectural characteristic.[6] Angular has
						a different expression syntax, focusing on "[ ]" for property binding,
						and "( )" for event binding[7] Modularity – much core functionality
						has moved to modules Angular recommends the use of Microsoft's
						TypeScript language, which introduces the following features: Static
						typing, including Generics Annotations TypeScript is a superset of
						ECMAScript 6 (ES6), and is backwards compatible with ECMAScript 5
						(i.e.: JavaScript). Dynamic loading Asynchronous template
						compilations
					</section>
					<section v-show="tabs8.selectedTabInitial === 3">
						<strong>Tab Three</strong> and it's content goes here ....
					</section>
					<section v-show="tabs8.selectedTabInitial === 4">
						<strong>Tab Four</strong> and it's content goes here ....
					</section>
				</template>
			</q-tab-container>
		</base-input-structure>
	</div>

	<!-- Tabs with lengthy tab titles -->
	<div
		class="container-fluid mt-3"
		style="flex: 1 1 auto">
		<base-input-structure
			id="CTRL_9"
			label="Tabs with lengthy tab titles">
			<q-tab-container
				is-visible
				:selected-tab="tabs9.selectedTabInitial"
				:tabs-list="tabs9.tabsListWithLengthyValues"
				@tab-changed="changeTab('tabs9', 'selectedTabInitial', $event)">
				<template #tab-panel>
					<section v-show="tabs9.selectedTabInitial === 1">
						<strong>Tab One</strong> and it's content goes here ....
					</section>
					<section v-show="tabs9.selectedTabInitial === 2">
						<strong>Tab Two</strong> and it's content goes here ....
					</section>
					<section v-show="tabs9.selectedTabInitial === 3">
						<strong>Tab Three</strong> and it's content goes here ....
					</section>
					<section v-show="tabs9.selectedTabInitial === 4">
						<strong>Tab Four</strong> and it's content goes here ....
					</section>
				</template>
			</q-tab-container>
		</base-input-structure>
	</div>
	<!-- Tabs with multiple line tabs -->
	<div
		class="container-fluid mt-3 mb-5"
		style="flex: 1 1 auto">
		<base-input-structure
			id="CTRL_10"
			label="Tabs with multiple line tabs">
			<q-tab-container
				is-visible
				:selected-tab="tabs10.selectedTabInitial"
				:tabs-list="tabs10.multipleTabList"
				@tab-changed="changeTab('tabs10', 'selectedTabInitial', $event)">
				<template #tab-panel>
					<section v-show="tabs10.selectedTabInitial === 1">
						<strong>Tab One</strong> and it's content goes here ....
					</section>
					<section v-show="tabs10.selectedTabInitial === 2">
						<strong>Tab Two</strong> and it's content goes here ....
					</section>
					<section v-show="tabs10.selectedTabInitial === 3">
						<strong>Tab Three</strong> and it's content goes here ....
					</section>
					<section v-show="tabs10.selectedTabInitial === 4">
						<strong>Tab Four</strong> and it's content goes here ....
					</section>
					<section v-show="tabs10.selectedTabInitial === 5">
						<strong>Tab Five</strong> and it's content goes here ....
					</section>
					<section v-show="tabs10.selectedTabInitial === 6">
						<strong>Tab Six</strong> and it's content goes here ....
					</section>
					<section v-show="tabs10.selectedTabInitial === 7">
						<strong>Tab Seven</strong> and it's content goes here ....
					</section>
					<section v-show="tabs10.selectedTabInitial === 8">
						<strong>Tab Eight</strong> and it's content goes here ....
					</section>
					<section v-show="tabs10.selectedTabInitial === 9">
						<strong>Tab Nine</strong> and it's content goes here ....
					</section>
					<section v-show="tabs10.selectedTabInitial === 10">
						<strong>Tab Ten</strong> and it's content goes here ....
					</section>
					<section v-show="tabs10.selectedTabInitial === 11">
						<strong>Tab Eleven</strong> and it's content goes here ....
					</section>
					<section v-show="tabs10.selectedTabInitial === 12">
						<strong>Tab Twelve</strong> and it's content goes here ....
					</section>
					<section v-show="tabs10.selectedTabInitial === 13">
						<strong>Tab Thirteen</strong> and it's content goes here ....
					</section>
				</template>
			</q-tab-container>
		</base-input-structure>
	</div>
</template>

<script>
	import fakeData from './TabContainer.mock'

	export default {
		name: 'QTabsContainer',

		docsfile: './docs/containers/TabContainer.md',

		inheritAttrs: false,

		expose: [],

		data()
		{
			return {
				tabs1: fakeData.simpleUsage(),
				tabs2: fakeData.simpleUsage(),
				tabs3: fakeData.simpleUsage(),
				tabs4: fakeData.simpleUsage(),
				tabs5: fakeData.simpleUsage(),
				tabs6: fakeData.simpleUsage(),
				tabs7: fakeData.simpleUsage(),
				tabs8: fakeData.simpleUsage(),
				tabs9: fakeData.simpleUsage(),
				tabs10: fakeData.simpleUsage()
			}
		},

		methods: {
			changeTab(tab, tabProp, selectedTab)
			{
				this[tab][tabProp] = selectedTab
			}
		}
	}
</script>
