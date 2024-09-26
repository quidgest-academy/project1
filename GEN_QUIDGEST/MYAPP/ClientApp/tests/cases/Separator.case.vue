<template>
	<div
		class="container-fluid"
		style="flex: 1 1 auto">
		<fieldset>
			<div class="form-flow">
				<q-row-container>
					<q-control-wrapper class="control-join-group">
						<base-input-structure
							id="CTRL_1"
							label="separator Input">
							<div class="step-column">
								<div class="separator-box">
									<div class="separator-filter">
										<h1>Vertical</h1>
										<select v-model="vertical">
											<option :value="true">true</option>
											<option :value="false">false</option>
										</select>
										<h1>Size</h1>
										<select v-model="size">
											<option value="small">small</option>
											<option value="medium">medium</option>
											<option value="big">big</option>
											<option value="full">full</option>
										</select>
										<h1>Variants</h1>
										<select v-model="sepStyle">
											<option value="primary">primary</option>
											<option value="secondary">secondary</option>
											<option value="dark">dark</option>
										</select>
										<h1>Elevation</h1>
										<select v-model="elevated">
											<option :value="true">true</option>
											<option :value="false">false</option>
										</select>
										<h1>Margin</h1>
										<select v-model="space">
											<option value="none">none</option>
											<option value="thin">thin</option>
											<option value="base">base</option>
											<option value="large">large</option>
											<option value="extraLarge">extra-large</option>
										</select>
									</div>
									<div class="live-tab-sep" />
									<div class="separator-prop">
										<div class="back-style">
											<q-separator
												:size="size"
												:sep-style="sepStyle"
												:vertical="vertical"
												:elevated="elevated"
												:space="space" />
										</div>
									</div>
								</div>
								<div class="live-tab-sep-bottom" />
							</div>
						</base-input-structure>
					</q-control-wrapper>
				</q-row-container>
			</div>
		</fieldset>
	</div>
</template>

<script>
	import QSeparator from '@/components/rendering/QSeparator.vue'

	export default {
		name: 'QSeparatorContainer',

		components: {
			QSeparator
		},

		expose: [],

		data()
		{
			return {
				size: 'medium',
				sepStyle: 'primary',
				vertical: false,
				elevated: false,
				space: 'thin'
			}
		}
	}
</script>

<!-- eslint-disable-next-line vue/no-restricted-block -->
<style scoped>
	.back-style {
		background-color: var(--light);
		width: relative;
		height: relative;
	}

	.separator-box {
		display: flex;
		flex-direction: row;
		width: 100%;
		height: 100%;
	}

	.separator-filter {
		display: flex;
		flex-direction: column;
		width: 25vw;
		align-items: center;
		justify-content: flex-start;
	}

	.live-tab-sep {
		width: 1px;
		background-color: lightgray;
		margin-right: 3rem;
		z-index: 5;
		margin-bottom: 10px;
	}

	.separator-prop {
		display: flex;
		align-items: center;
		justify-content: center;
		width: relative;
		height: relative;
	}

	.step-column {
		display: flex;
		flex-direction: column;
		width: 100%;
	}

	.live-tab-sep-bottom {
		display: flex;
		flex-direction: row;
		width: 100%;
		height: 1px;
		background-color: lightgray;
		margin-top: 5px;
	}
</style>
