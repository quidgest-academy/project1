<template>
	<div
		class="container-fluid"
		style="flex: 1 1 auto">
		<form class="form-horizontal">
			<fieldset>
				<div class="form-flow">
					<q-row-container>
						<q-control-wrapper class="control-join-group">
							<div class="versions">
								<base-input-structure
									id="CTRL_1"
									label="enabled version">
									<q-toggle-input
										id="CTRL_1"
										v-model="isActive1"
										true-label="yes"
										false-label="no"
										nullable />
								</base-input-structure>

								<base-input-structure
									id="CTRL_2"
									label="disabled version">
									<q-toggle-input
										id="CTRL_2"
										v-model="isActive1"
										disabled
										true-label="yes"
										false-label="no"
										nullable />
								</base-input-structure>
								<q-button
									b-style="tertiary"
									@click="changeStuff(1)">
									indeterminate
								</q-button>
							</div>

							<div class="versions">
								<base-input-structure
									id="CTRL_3"
									label="no display type with no false label">
									<q-toggle-input
										id="CTRL_3"
										v-model="isActive2"
										true-label="yes"
										nullable />
									<q-button
										b-style="tertiary"
										@click="changeStuff(2)">
										indeterminate
									</q-button>
								</base-input-structure>
							</div>

							<div class="versions">
								<base-input-structure
									id="CTRL_4"
									label="displayType: label-toggle">
									<q-toggle-input
										id="CTRL_4"
										v-model="isActive3"
										true-label="yes"
										false-label="no"
										display-type="label-toggle"
										nullable />
									<q-button
										b-style="tertiary"
										@click="changeStuff(3)">
										indeterminate
									</q-button>
								</base-input-structure>
							</div>

							<div class="versions">
								<base-input-structure
									id="CTRL_5"
									label="displayType: label-left">
									<q-toggle-input
										id="CTRL_5"
										v-model="isActive4"
										true-label="yes"
										false-label="no"
										display-type="label-left"
										nullable />
									<q-button
										b-style="tertiary"
										@click="changeStuff(4)">
										indeterminate
									</q-button>
								</base-input-structure>
							</div>
						</q-control-wrapper>
					</q-row-container>
				</div>
			</fieldset>
		</form>
	</div>
</template>

<script>
	export default {
		name: 'QToggleContainer',

		docsfile: './docs/inputs/ToggleInput.md',

		inheritAttrs: false,

		expose: [],

		data()
		{
			return {
				isActive1: false,
				isActive2: false,
				isActive3: false,
				isActive4: false
			}
		},
		methods: {
			changeStuff(number)
			{
				this['isActive' + number] = null
			}
		}
	}
</script>
