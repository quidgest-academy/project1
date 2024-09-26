<template>
	<h3>Usage</h3>

	<div class="demo-container">
		<div class="demo-main-container">
			<component
				:is="generalOptions.containerComponent"
				id="demo-accordion"
				v-slot="scope"
				:open-group="openGroup"
				@set-open-group="(state, groupId) => setOpenGroup(state, groupId)">
				<template
					v-for="index in generalOptions.groupNumber"
					:key="index">
					<q-group-collapsible
						:id="`group-${index}`"
						:is-open="isGroupOpen(`group-${index}`, openGroup)"
						v-bind="groupOptions"
						@state-changed="(state, groupId) => onStateChanged(scope, state, groupId)">
						<q-row-container>
							<q-control-wrapper class="control-join-group">
								<base-input-structure
									:id="`input-${index}-1`"
									class="i-text"
									label="Created by"
									readonly>
									<q-text-field
										:id="`input-${index}-1`"
										model-value="QUIDGEST"
										readonly />
								</base-input-structure>
							</q-control-wrapper>

							<q-control-wrapper class="control-join-group">
								<base-input-structure
									:id="`input-${index}-2`"
									class="i-text"
									label="Created on"
									readonly>
									<q-text-field
										:id="`input-${index}-2`"
										model-value="05/03/2023"
										readonly />
								</base-input-structure>
							</q-control-wrapper>

							<q-control-wrapper class="control-join-group">
								<base-input-structure
									:id="`input-${index}-3`"
									class="i-text"
									label="Changed by"
									readonly>
									<q-text-field
										:id="`input-${index}-3`"
										model-value="QUIDGEST"
										readonly />
								</base-input-structure>
							</q-control-wrapper>

							<q-control-wrapper class="control-join-group">
								<base-input-structure
									:id="`input-${index}-4`"
									class="i-text"
									label="Changed on"
									readonly>
									<q-text-field
										:id="`input-${index}-4`"
										model-value="15/03/2023"
										readonly />
								</base-input-structure>
							</q-control-wrapper>
						</q-row-container>
					</q-group-collapsible>
				</template>
			</component>
		</div>

		<div class="demo-container-sep" />

		<div class="demo-options">
			<h4>Options</h4>

			<div>
				<label class="i-text__label">Visualization options</label>

				<div class="demo-subtype-options">
					<div class="demo-option">
						<base-input-structure
							id="general-option-1"
							class="i-text"
							label="Number of groups">
							<q-numeric-input
								id="general-option-1"
								v-model="generalOptions.groupNumber"
								size="large"
								thousands-separator="."
								decimal-point=","
								:max-integers="4"
								:max-decimals="0" />
						</base-input-structure>
					</div>

					<div class="demo-option">
						<base-input-structure
							id="general-option-2"
							label="Is accordion"
							:label-attrs="{ class: 'i-checkbox i-checkbox__label' }"
							help-style="popover"
							user-help="All groups inside an accordion will close the others when they are opened, you'll need to have at least 2 visible groups to see this effect.">
							<template #label>
								<q-checkbox-input
									id="general-option-2"
									v-model="generalOptions.isAccordion" />
							</template>
						</base-input-structure>
					</div>
				</div>
			</div>

			<div>
				<label class="i-text__label">Collapsible Group options</label>

				<div class="demo-subtype-options">
					<div class="demo-option">
						<base-input-structure
							id="group-option-1"
							class="i-text"
							label="Group title"
							help-style="popover"
							user-help="Choose a name to give to your groups.">
							<q-text-field
								id="group-option-1"
								size="large"
								v-model="groupOptions.label" />
						</base-input-structure>
					</div>

					<div class="demo-option">
						<base-input-structure
							id="group-option-2"
							label="Is required"
							:label-attrs="{ class: 'i-checkbox i-checkbox__label' }"
							help-style="popover"
							user-help="In the application, a group will be required if one of it's fields is required.">
							<template #label>
								<q-checkbox-input
									id="group-option-2"
									v-model="groupOptions.isRequired" />
							</template>
						</base-input-structure>
					</div>
				</div>
			</div>
		</div>
	</div>
</template>

<script>
	import { computed } from 'vue'

	export default {
		name: 'QGroupCollapsibleContainer',

		docsfile: './docs/containers/QGroupCollapsible.md',

		inheritAttrs: false,

		expose: [],

		data()
		{
			return {
				generalOptions: {
					containerComponent: computed(() => (this.generalOptions?.isAccordion ? 'q-accordion-container' : 'div')),
					groupNumber: 3,
					isAccordion: false
				},

				groupOptions: {
					accordionId: computed(() => (this.generalOptions?.isAccordion ? 'demo-accordion' : '')),
					label: 'Your title',
					isRequired: false
				},

				// For accordion mode
				openGroup: null,

				// For independent mode
				openGroups: {}
			}
		},

		methods: {
			onStateChanged(parentScope, state, groupId)
			{
				this.generalOptions?.isAccordion ? parentScope.onStateChanged(state, groupId) : (this.openGroups[groupId] = state)
			},

			isGroupOpen(index, openGroup)
			{
				return this.generalOptions?.isAccordion ? openGroup === index : this.openGroups[index]
			},

			setOpenGroup(state, groupId)
			{
				this.openGroup = state ? groupId : null
			}
		},

		watch: {
			'generalOptions.isAccordion'()
			{
				this.openGroups = {}
			}
		}
	}
</script>
