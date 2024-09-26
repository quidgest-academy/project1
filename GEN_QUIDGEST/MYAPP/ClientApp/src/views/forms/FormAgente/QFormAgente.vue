<template>
	<teleport
		v-if="formModalIsReady && showFormHeader"
		:to="`#${uiContainersId.header}`"
		:disabled="!isPopup || isNested">
		<div
			ref="formHeader"
			:class="{ 'c-sticky-header': isStickyHeader, 'sticky-top': isStickyTop }">
			<div
				v-if="showFormHeader"
				class="c-action-bar">
				<h1
					v-if="formControl.uiComponents.header && formInfo.designation"
					class="form-header">
					{{ formInfo.designation }}
				</h1>

				<div class="c-action-bar__menu">
					<template
						v-for="(section, sectionId) in formButtonSections"
						:key="sectionId">
						<span
							v-if="showHeadingSep(sectionId)"
							class="main-title-sep" />

						<q-button-group
							v-if="formControl.uiComponents.headerButtons"
							borderless>
							<template
								v-for="btn in section"
								:key="btn.id">
								<q-button
									v-if="showFormHeaderButton(btn)"
									:id="`top-${btn.id}`"
									:title="btn.text"
									:label="btn.label"
									:disabled="btn.disabled"
									:active="btn.isSelected"
									@click="btn.action">
									<q-icon
										v-if="btn.icon"
										v-bind="btn.icon" />
								</q-button>
							</template>
						</q-button-group>
					</template>
				</div>
			</div>

			<q-anchor-container-horizontal
				v-if="layoutConfig.FormAnchorsPosition === 'form-header' && groupFields.length > 0"
				:is-visible="anchorContainerVisibility"
				:anchors="groupFields"
				:controls="controls"
				:header-height="visibleHeaderHeight"
				@focus-control="(...args) => focusControl(...args)" />
		</div>
	</teleport>

	<teleport
		v-if="formModalIsReady && showFormBody"
		:to="`#${uiContainersId.body}`"
		:disabled="!isPopup || isNested">
		<q-validation-summary
			:messages="validationErrors"
			@error-clicked="focusField" />

		<div class="heading-button-group-clear"></div>

		<div :class="[`float-${actionsPlacement}`, 'c-action-bar']">
			<q-button-group borderless>
				<template
					v-for="btn in formButtons"
					:key="btn.id">
					<q-button
						v-if="btn.isActive && btn.isVisible && btn.showInHeading"
						:id="`heading-${btn.id}`"
						:label="btn.text"
						:b-style="btn.style"
						:disabled="btn.disabled"
						:icon-on-right="btn.iconOnRight"
						:class="btn.classes"
						@click="btn.action(); btn.emitAction ? $emit(btn.emitAction.name, btn.emitAction.params) : null">
						<q-icon
							v-if="btn.icon"
							v-bind="btn.icon" />
					</q-button>
				</template>
			</q-button-group>
		</div>

		<div class="heading-button-group-clear"></div>

		<div
			class="form-flow"
			data-key="AGENTE"
			:data-loading="!formInitialDataLoaded"
			:key="domVersionKey">
			<template v-if="formControl.initialized && showFormBody">
				<q-row-container
					v-show="controls.AGENTE__PSEUDNEWGRP01.isVisible || controls.AGENTE__PMORAPAIS____.isVisible || controls.AGENTE__PNASCPAIS____.isVisible"
					is-large>
					<q-control-wrapper
						v-show="controls.AGENTE__PSEUDNEWGRP01.isVisible"
						class="row-line-group">
						<q-group-box-container
							id="AGENTE__PSEUDNEWGRP01"
							v-bind="controls.AGENTE__PSEUDNEWGRP01"
							:is-visible="controls.AGENTE__PSEUDNEWGRP01.isVisible">
							<!-- Start AGENTE__PSEUDNEWGRP01 -->
							<q-row-container v-show="controls.AGENTE__AGENTFOTO____.isVisible">
								<q-control-wrapper
									v-show="controls.AGENTE__AGENTFOTO____.isVisible"
									class="control-join-group">
									<base-input-structure
										class="q-image"
										v-bind="controls.AGENTE__AGENTFOTO____"
										v-on="controls.AGENTE__AGENTFOTO____.handlers"
										:loading="controls.AGENTE__AGENTFOTO____.props.loading"
										:reporting-mode-on="reportingModeCAV"
										:suggestion-mode-on="suggestionModeOn">
										<q-image
											v-if="controls.AGENTE__AGENTFOTO____.isVisible"
											v-bind="controls.AGENTE__AGENTFOTO____.props"
											v-on="controls.AGENTE__AGENTFOTO____.handlers" />
									</base-input-structure>
								</q-control-wrapper>
							</q-row-container>
							<q-row-container v-show="controls.AGENTE__AGENTNOME____.isVisible || controls.AGENTE__AGENTDNASCIME.isVisible">
								<q-control-wrapper
									v-show="controls.AGENTE__AGENTNOME____.isVisible"
									class="control-join-group">
									<base-input-structure
										class="i-text"
										v-bind="controls.AGENTE__AGENTNOME____"
										v-on="controls.AGENTE__AGENTNOME____.handlers"
										:loading="controls.AGENTE__AGENTNOME____.props.loading"
										:reporting-mode-on="reportingModeCAV"
										:suggestion-mode-on="suggestionModeOn">
										<q-text-field
											v-bind="controls.AGENTE__AGENTNOME____.props"
											:model-value="model.ValNome.value"
											@blur="onBlur(controls.AGENTE__AGENTNOME____, model.ValNome.value)"
											@change="model.ValNome.fnUpdateValueOnChange" />
									</base-input-structure>
								</q-control-wrapper>
								<q-control-wrapper
									v-show="controls.AGENTE__AGENTDNASCIME.isVisible"
									class="control-join-group">
									<base-input-structure
										class="i-text"
										v-bind="controls.AGENTE__AGENTDNASCIME"
										v-on="controls.AGENTE__AGENTDNASCIME.handlers"
										:loading="controls.AGENTE__AGENTDNASCIME.props.loading"
										:reporting-mode-on="reportingModeCAV"
										:suggestion-mode-on="suggestionModeOn">
										<q-date-time-picker
											v-if="controls.AGENTE__AGENTDNASCIME.isVisible"
											v-bind="controls.AGENTE__AGENTDNASCIME.props"
											:model-value="model.ValDnascime.value"
											@reset-icon-click="model.ValDnascime.fnUpdateValue(model.ValDnascime.originalValue ?? new Date())"
											@update:model-value="model.ValDnascime.fnUpdateValue($event ?? '')" />
									</base-input-structure>
								</q-control-wrapper>
							</q-row-container>
							<q-row-container v-show="controls.AGENTE__AGENTEMAIL___.isVisible || controls.AGENTE__AGENTTELEFONE.isVisible">
								<q-control-wrapper
									v-show="controls.AGENTE__AGENTEMAIL___.isVisible"
									class="control-join-group">
									<base-input-structure
										class="i-text"
										v-bind="controls.AGENTE__AGENTEMAIL___"
										v-on="controls.AGENTE__AGENTEMAIL___.handlers"
										:loading="controls.AGENTE__AGENTEMAIL___.props.loading"
										:reporting-mode-on="reportingModeCAV"
										:suggestion-mode-on="suggestionModeOn">
										<q-text-field
											v-bind="controls.AGENTE__AGENTEMAIL___.props"
											:model-value="model.ValEmail.value"
											@blur="onBlur(controls.AGENTE__AGENTEMAIL___, model.ValEmail.value)"
											@change="model.ValEmail.fnUpdateValueOnChange" />
									</base-input-structure>
								</q-control-wrapper>
								<q-control-wrapper
									v-show="controls.AGENTE__AGENTTELEFONE.isVisible"
									class="control-join-group">
									<base-input-structure
										class="i-text"
										v-bind="controls.AGENTE__AGENTTELEFONE"
										v-on="controls.AGENTE__AGENTTELEFONE.handlers"
										:loading="controls.AGENTE__AGENTTELEFONE.props.loading"
										:reporting-mode-on="reportingModeCAV"
										:suggestion-mode-on="suggestionModeOn">
										<q-mask
											v-if="controls.AGENTE__AGENTTELEFONE.isVisible"
											v-bind="controls.AGENTE__AGENTTELEFONE"
											:model-value="model.ValTelefone.value"
											@update:model-value="model.ValTelefone.fnUpdateValue" />
									</base-input-structure>
								</q-control-wrapper>
							</q-row-container>
							<!-- End AGENTE__PSEUDNEWGRP01 -->
						</q-group-box-container>
					</q-control-wrapper>
					<q-control-wrapper
						v-show="controls.AGENTE__PMORAPAIS____.isVisible"
						class="control-join-group">
						<base-input-structure
							class="i-text"
							v-bind="controls.AGENTE__PMORAPAIS____"
							v-on="controls.AGENTE__PMORAPAIS____.handlers"
							:loading="controls.AGENTE__PMORAPAIS____.props.loading"
							:reporting-mode-on="reportingModeCAV"
							:suggestion-mode-on="suggestionModeOn">
							<q-lookup
								v-if="controls.AGENTE__PMORAPAIS____.isVisible"
								v-bind="controls.AGENTE__PMORAPAIS____.props"
								v-on="controls.AGENTE__PMORAPAIS____.handlers" />
							<q-see-more-agente-pmorapais
								v-if="controls.AGENTE__PMORAPAIS____.seeMoreIsVisible"
								v-bind="controls.AGENTE__PMORAPAIS____.seeMoreParams"
								v-on="controls.AGENTE__PMORAPAIS____.handlers" />
						</base-input-structure>
					</q-control-wrapper>
					<q-control-wrapper
						v-show="controls.AGENTE__PNASCPAIS____.isVisible"
						class="control-join-group">
						<base-input-structure
							class="i-text"
							v-bind="controls.AGENTE__PNASCPAIS____"
							v-on="controls.AGENTE__PNASCPAIS____.handlers"
							:loading="controls.AGENTE__PNASCPAIS____.props.loading"
							:reporting-mode-on="reportingModeCAV"
							:suggestion-mode-on="suggestionModeOn">
							<q-lookup
								v-if="controls.AGENTE__PNASCPAIS____.isVisible"
								v-bind="controls.AGENTE__PNASCPAIS____.props"
								v-on="controls.AGENTE__PNASCPAIS____.handlers" />
							<q-see-more-agente-pnascpais
								v-if="controls.AGENTE__PNASCPAIS____.seeMoreIsVisible"
								v-bind="controls.AGENTE__PNASCPAIS____.seeMoreParams"
								v-on="controls.AGENTE__PNASCPAIS____.handlers" />
						</base-input-structure>
					</q-control-wrapper>
				</q-row-container>
			</template>
		</div>
	</teleport>

	<hr v-if="!isPopup && showFormFooter" />

	<teleport
		v-if="formModalIsReady && showFormFooter"
		:to="`#${uiContainersId.footer}`"
		:disabled="!isPopup || isNested">
		<q-row-container v-if="showFormFooter">
			<div id="footer-action-btns">
				<template
					v-for="btn in formButtons"
					:key="btn.id">
					<q-button
						v-if="btn.isActive && btn.isVisible && btn.showInFooter"
						:id="`bottom-${btn.id}`"
						:label="btn.text"
						:b-style="btn.style"
						:disabled="btn.disabled"
						:icon-on-right="btn.iconOnRight"
						:class="btn.classes"
						@click="btn.action(); btn.emitAction ? $emit(btn.emitAction.name, btn.emitAction.params) : null">
						<q-icon
							v-if="btn.icon"
							v-bind="btn.icon" />
					</q-button>
				</template>
			</div>
		</q-row-container>
	</teleport>
</template>

<script>
	/* eslint-disable no-unused-vars */
	import { computed, readonly, defineAsyncComponent } from 'vue'
	import { useRoute } from 'vue-router'

	import FormHandlers from '@/mixins/formHandlers.js'
	import formFunctions from '@/mixins/formFunctions.js'
	import genericFunctions from '@/mixins/genericFunctions.js'
	import listFunctions from '@/mixins/listFunctions.js'
	import listColumnTypes from '@/mixins/listColumnTypes.js'
	import modelFieldType from '@/mixins/formModelFieldTypes.js'
	import fieldControlClass from '@/mixins/fieldControl.js'
	import qEnums from '@/mixins/quidgest.mainEnums.js'

	import hardcodedTexts from '@/hardcodedTexts.js'
	import netAPI from '@/api/network'
	import asyncProcM from '@/api/global/asyncProcMonitoring.js'
	import qApi from '@/api/genio/quidgestFunctions.js'
	import qFunctions from '@/api/genio/projectFunctions.js'
	import qProjArrays from '@/api/genio/projectArrays.js'
	/* eslint-enable no-unused-vars */

	import FormViewModel from './QFormAgenteViewModel.js'

	const requiredTextResources = ['QFormAgente', 'hardcoded', 'messages']

/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL PRO FORM_INCLUDEJS AGENTE]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */

	export default {
		name: 'QFormAgente',

		components: {
			QSeeMoreAgentePmorapais: defineAsyncComponent(() => import('@/views/forms/FormAgente/dbedits/AgentePmorapaisSeeMore.vue')),
			QSeeMoreAgentePnascpais: defineAsyncComponent(() => import('@/views/forms/FormAgente/dbedits/AgentePnascpaisSeeMore.vue')),
		},

		mixins: [
			FormHandlers
		],

		props: {
			/**
			 * Parameters passed in case the form is nested.
			 */
			nestedRouteParams: {
				type: Object,
				default: () => {
					return {
						name: 'AGENTE',
						location: 'form-AGENTE',
						params: {
							isNested: true
						}
					}
				}
			}
		},

		expose: [
			'cancel',
			'initFormProperties',
			'navigationId'
		],

		setup(props)
		{
			const route = useRoute()

			return {
				/*
				 * As properties are reactive, when using $route.params, then when we exit it updates cached components.
				 * Properties have no value and this creates an error in new versions of vue-router.
				 * That's why the value has to be copied to a local property to be used in the router-link tag.
				 */
				currentRouteParams: props.isNested ? {} : route.params
			}
		},

		data()
		{
			// eslint-disable-next-line
			const vm = this
			return {
				componentOnLoadProc: asyncProcM.getProcListMonitor('QFormAgente', false),

				interfaceMetadata: {
					id: 'QFormAgente', // Used for resources
					requiredTextResources
				},

				formInfo: {
					type: 'normal',
					name: 'AGENTE',
					route: 'form-AGENTE',
					area: 'AGENT',
					primaryKey: 'ValCodagent',
					designation: computed(() => this.Resources.AGENTE_IMOBILIARIO28727),
					identifier: '', // Unique identifier received by route (when it's nested).
					mode: ''
				},

				formButtons: {
					changeToShow: {
						id: 'change-to-show-btn',
						icon: {
							icon: 'view',
							type: 'svg'
						},
						type: 'form-mode',
						text: computed(() => vm.Resources[hardcodedTexts.view]),
						style: 'secondary',
						showInHeader: true,
						showInFooter: false,
						isActive: false,
						isSelected: computed(() => vm.formModes.show === vm.formInfo.mode),
						isVisible: computed(() => vm.authData.isAllowed && [vm.formModes.show, vm.formModes.edit, vm.formModes.delete].includes(vm.formInfo.mode)),
						action: vm.changeToShowMode
					},
					changeToEdit: {
						id: 'change-to-edit-btn',
						icon: {
							icon: 'pencil',
							type: 'svg'
						},
						type: 'form-mode',
						text: computed(() => vm.Resources[hardcodedTexts.edit]),
						style: 'secondary',
						showInHeader: true,
						showInFooter: false,
						isActive: false,
						isSelected: computed(() => vm.formModes.edit === vm.formInfo.mode),
						isVisible: computed(() => vm.authData.isAllowed && [vm.formModes.show, vm.formModes.edit, vm.formModes.delete].includes(vm.formInfo.mode)),
						action: vm.changeToEditMode
					},
					changeToDuplicate: {
						id: 'change-to-dup-btn',
						icon: {
							icon: 'duplicate',
							type: 'svg'
						},
						type: 'form-mode',
						text: computed(() => vm.Resources[hardcodedTexts.duplicate]),
						style: 'secondary',
						showInHeader: true,
						showInFooter: false,
						isActive: false,
						isSelected: computed(() => vm.formModes.duplicate === vm.formInfo.mode),
						isVisible: computed(() => vm.authData.isAllowed && vm.formModes.new !== vm.formInfo.mode),
						action: vm.changeToDupMode
					},
					changeToDelete: {
						id: 'change-to-delete-btn',
						icon: {
							icon: 'delete',
							type: 'svg'
						},
						type: 'form-mode',
						text: computed(() => vm.Resources[hardcodedTexts.delete]),
						style: 'secondary',
						showInHeader: true,
						showInFooter: false,
						isActive: false,
						isSelected: computed(() => vm.formModes.delete === vm.formInfo.mode),
						isVisible: computed(() => vm.authData.isAllowed && [vm.formModes.show, vm.formModes.edit, vm.formModes.delete].includes(vm.formInfo.mode)),
						action: vm.changeToDeleteMode
					},
					changeToInsert: {
						id: 'change-to-insert-btn',
						icon: {
							icon: 'add',
							type: 'svg'
						},
						type: 'form-insert',
						text: computed(() => vm.Resources[hardcodedTexts.insert]),
						label: computed(() => vm.Resources[hardcodedTexts.insert]),
						style: 'secondary',
						showInHeader: true,
						showInFooter: false,
						isActive: false,
						isSelected: computed(() => vm.formModes.new === vm.formInfo.mode),
						isVisible: computed(() => vm.authData.isAllowed && vm.formModes.duplicate !== vm.formInfo.mode),
						action: vm.changeToInsertMode
					},
					repeatInsertBtn: {
						id: 'repeat-insert-btn',
						icon: {
							icon: 'save-new',
							type: 'svg'
						},
						type: 'form-action',
						text: computed(() => vm.Resources[hardcodedTexts.repeatInsert]),
						style: 'primary',
						showInHeader: true,
						showInFooter: true,
						isActive: false,
						isVisible: computed(() => vm.authData.isAllowed && vm.formInfo.mode === vm.formModes.new),
						action: () => vm.saveForm(true)
					},
					saveBtn: {
						id: 'save-btn',
						icon: {
							icon: 'save',
							type: 'svg'
						},
						type: 'form-action',
						text: computed(() => vm.Resources.GRAVAR45301),
						style: 'primary',
						showInHeader: true,
						showInFooter: true,
						isActive: true,
						isVisible: computed(() => vm.authData.isAllowed && vm.isEditable),
						action: vm.saveForm
					},
					confirmBtn: {
						id: 'confirm-btn',
						icon: {
							icon: 'check',
							type: 'svg'
						},
						type: 'form-action',
						text: computed(() => vm.Resources[vm.isNested ? hardcodedTexts.delete : hardcodedTexts.confirm]),
						style: 'primary',
						showInHeader: true,
						showInFooter: true,
						isActive: true,
						isVisible: computed(() => vm.authData.isAllowed && (vm.formInfo.mode === vm.formModes.delete || vm.isNested)),
						action: vm.deleteRecord
					},
					cancelBtn: {
						id: 'cancel-btn',
						icon: {
							icon: 'cancel',
							type: 'svg'
						},
						type: 'form-action',
						text: computed(() => vm.Resources.CANCELAR49513),
						style: 'secondary',
						showInHeader: true,
						showInFooter: true,
						isActive: true,
						isVisible: computed(() => vm.authData.isAllowed && vm.isEditable),
						action: vm.leaveForm
					},
					resetCancelBtn: {
						id: 'reset-cancel-btn',
						icon: {
							icon: 'cancel',
							type: 'svg'
						},
						type: 'form-action',
						text: computed(() => vm.Resources[hardcodedTexts.cancel]),
						style: 'secondary',
						showInHeader: true,
						showInFooter: true,
						isActive: false,
						isVisible: computed(() => vm.authData.isAllowed && vm.isEditable),
						action: () => vm.model.resetValues(),
						emitAction: {
							name: 'deselect',
							params: {}
						}
					},
					editBtn: {
						id: 'edit-btn',
						icon: {
							icon: 'pencil',
							type: 'svg'
						},
						type: 'form-action',
						text: computed(() => vm.Resources[hardcodedTexts.edit]),
						style: 'primary',
						showInHeader: true,
						showInFooter: false,
						isActive: false,
						isVisible: computed(() => vm.authData.isAllowed && vm.parentFormMode !== vm.formModes.show && vm.parentFormMode !== vm.formModes.delete),
						action: () => {},
						emitAction: {
							name: 'edit',
							params: {}
						}
					},
					deleteQuickBtn: {
						id: 'delete-btn',
						icon: {
							icon: 'bin',
							type: 'svg'
						},
						type: 'form-action',
						text: computed(() => vm.Resources[hardcodedTexts.delete]),
						style: 'primary',
						showInHeader: true,
						showInFooter: false,
						isActive: false,
						isVisible: computed(() => vm.authData.isAllowed && vm.parentFormMode !== vm.formModes.show && (typeof vm.permissions.canDelete === 'boolean' ? vm.permissions.canDelete : true)),
						action: vm.deleteRecord
					},
					backBtn: {
						id: 'back-btn',
						icon: {
							icon: 'back',
							type: 'svg'
						},
						type: 'form-action',
						text: computed(() => vm.isPopup ? vm.Resources[hardcodedTexts.close] : vm.Resources[hardcodedTexts.goBack]),
						style: 'secondary',
						showInHeader: true,
						showInFooter: true,
						isActive: true,
						isVisible: computed(() => !vm.authData.isAllowed || !vm.isEditable),
						action: vm.leaveForm
					}
				},

				controls: {
					AGENTE__PSEUDNEWGRP01: new fieldControlClass.GroupControl({
						id: 'AGENTE__PSEUDNEWGRP01',
						name: 'NEWGRP01',
						size: 'block',
						label: computed(() => this.Resources.INFORMACAO_DO_AGENTE51492),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						isCollapsible: false,
						anchored: false,
						mustBeFilled: true,
						controlLimits: [
						],
					}, this),
					AGENTE__AGENTFOTO____: new fieldControlClass.ImageControl({
						modelField: 'ValFoto',
						valueChangeEvent: 'fieldChange:agent.foto',
						id: 'AGENTE__AGENTFOTO____',
						name: 'FOTO',
						size: 'mini',
						label: computed(() => this.Resources.FOTOGRAFIA36807),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						container: 'AGENTE__PSEUDNEWGRP01',
						height: 50,
						width: 30,
						maxFileSize: 10485760, // In bytes.
						maxFileSizeLabel: '10 MB',
						controlLimits: [
						],
					}, this),
					AGENTE__AGENTNOME____: new fieldControlClass.StringControl({
						modelField: 'ValNome',
						valueChangeEvent: 'fieldChange:agent.nome',
						id: 'AGENTE__AGENTNOME____',
						name: 'NOME',
						size: 'xxlarge',
						label: computed(() => this.Resources.NOME47814),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						container: 'AGENTE__PSEUDNEWGRP01',
						maxLength: 80,
						labelId: 'label_AGENTE__AGENTNOME____',
						mustBeFilled: true,
						controlLimits: [
						],
					}, this),
					AGENTE__AGENTDNASCIME: new fieldControlClass.DateControl({
						modelField: 'ValDnascime',
						valueChangeEvent: 'fieldChange:agent.dnascime',
						id: 'AGENTE__AGENTDNASCIME',
						name: 'DNASCIME',
						size: 'medium',
						label: computed(() => this.Resources.DATA_DE_NASCIMENTO13938),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						container: 'AGENTE__PSEUDNEWGRP01',
						format: 'date',
						controlLimits: [
						],
					}, this),
					AGENTE__AGENTEMAIL___: new fieldControlClass.StringControl({
						modelField: 'ValEmail',
						valueChangeEvent: 'fieldChange:agent.email',
						id: 'AGENTE__AGENTEMAIL___',
						name: 'EMAIL',
						size: 'xxlarge',
						label: computed(() => this.Resources.E_MAIL42251),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						container: 'AGENTE__PSEUDNEWGRP01',
						maxLength: 80,
						labelId: 'label_AGENTE__AGENTEMAIL___',
						mustBeFilled: true,
						controlLimits: [
						],
					}, this),
					AGENTE__AGENTTELEFONE: new fieldControlClass.MaskControl({
						modelField: 'ValTelefone',
						valueChangeEvent: 'fieldChange:agent.telefone',
						id: 'AGENTE__AGENTTELEFONE',
						name: 'TELEFONE',
						size: 'medium',
						label: computed(() => this.Resources.TELEFONE37757),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						container: 'AGENTE__PSEUDNEWGRP01',
						maxLength: 14,
						labelId: 'label_AGENTE__AGENTTELEFONE',
						controlLimits: [
						],
					}, this),
					AGENTE__PMORAPAIS____: new fieldControlClass.LookupControl({
						modelField: 'TablePmoraPais',
						valueChangeEvent: 'fieldChange:pmora.pais',
						id: 'AGENTE__PMORAPAIS____',
						name: 'PAIS',
						size: 'xxlarge',
						label: computed(() => this.Resources.PAIS_DE_MORADA63860),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						externalCallbacks: {
							getModelField: vm.getModelField,
							getModelFieldValue: vm.getModelFieldValue,
							setModelFieldValue: vm.setModelFieldValue
						},
						externalProperties: {
							modelKeys: computed(() => vm.modelKeys)
						},
						lookupKeyModelField: {
							name: 'ValCodpmora',
							dependencyEvent: 'fieldChange:agent.codpmora'
						},
						dependentFields: () => ({
							set 'pmora.codpais'(value) { vm.model.ValCodpmora.updateValue(value) },
							set 'pmora.pais'(value) { vm.model.TablePmoraPais.updateValue(value) },
						}),
						controlLimits: [
						],
					}, this),
					AGENTE__PNASCPAIS____: new fieldControlClass.LookupControl({
						modelField: 'TablePnascPais',
						valueChangeEvent: 'fieldChange:pnasc.pais',
						id: 'AGENTE__PNASCPAIS____',
						name: 'PAIS',
						size: 'xxlarge',
						label: computed(() => this.Resources.PAIS_DE_NASCIMENTO51886),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						externalCallbacks: {
							getModelField: vm.getModelField,
							getModelFieldValue: vm.getModelFieldValue,
							setModelFieldValue: vm.setModelFieldValue
						},
						externalProperties: {
							modelKeys: computed(() => vm.modelKeys)
						},
						lookupKeyModelField: {
							name: 'ValCodpnasc',
							dependencyEvent: 'fieldChange:agent.codpnasc'
						},
						dependentFields: () => ({
							set 'pnasc.codpais'(value) { vm.model.ValCodpnasc.updateValue(value) },
							set 'pnasc.pais'(value) { vm.model.TablePnascPais.updateValue(value) },
						}),
						controlLimits: [
						],
					}, this),
				},

				model: new FormViewModel(this, {
					callbacks: {
						onUpdate: this.onUpdate,
						setFormKey: this.setFormKey
					}
				}),

				groupFields: readonly([
					'AGENTE__PSEUDNEWGRP01',
				]),

				tableFields: readonly([
				]),

				timelineFields: readonly([
				]),

				/**
				 * The Data API for easy access to model variables.
				 */
				dataApi: {
					Agent: {
						get ValCodpmora() { return vm.model.ValCodpmora.value },
						set ValCodpmora(value) { vm.model.ValCodpmora.updateValue(value) },
						get ValCodpnasc() { return vm.model.ValCodpnasc.value },
						set ValCodpnasc(value) { vm.model.ValCodpnasc.updateValue(value) },
						get ValDnascime() { return vm.model.ValDnascime.value },
						set ValDnascime(value) { vm.model.ValDnascime.updateValue(value) },
						get ValEmail() { return vm.model.ValEmail.value },
						set ValEmail(value) { vm.model.ValEmail.updateValue(value) },
						get ValFoto() { return vm.model.ValFoto.value },
						set ValFoto(value) { vm.model.ValFoto.updateValue(value) },
						get ValNome() { return vm.model.ValNome.value },
						set ValNome(value) { vm.model.ValNome.updateValue(value) },
						get ValTelefone() { return vm.model.ValTelefone.value },
						set ValTelefone(value) { vm.model.ValTelefone.updateValue(value) },
					},
					Pmora: {
						get ValPais() { return vm.model.TablePmoraPais.value },
						set ValPais(value) { vm.model.TablePmoraPais.updateValue(value) },
					},
					Pnasc: {
						get ValPais() { return vm.model.TablePnascPais.value },
						set ValPais(value) { vm.model.TablePnascPais.updateValue(value) },
					},
					keys: {
						/** The primary key of the AGENT table */
						get agent() { return vm.model.ValCodagent },
						/** The foreign key to the PMORA table */
						get pmora() { return vm.model.ValCodpmora },
						/** The foreign key to the PNASC table */
						get pnasc() { return vm.model.ValCodpnasc },
					},
					get extraProperties() { return vm.model.extraProperties },
				},
			}
		},

		beforeRouteEnter(to, _, next)
		{
			// Called before the route that renders this component is confirmed.
			// Does NOT have access to `this` component instance, because
			// it has not been created yet when this guard is called!

			next((vm) => {
				vm.initFormProperties(to)
			})
		},

		beforeRouteLeave(to, _, next)
		{
			if (to.params.isControlled === 'true')
			{
				genericFunctions.setNavigationState(false)
				next()
			}
			else
				this.cancel(next)
		},

		beforeRouteUpdate(to, _, next)
		{
			if (to.params.isControlled === 'true')
				next()
			else
				this.cancel(next)
		},

		mounted()
		{
/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL PRO FORM_CODEJS AGENTE]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */
		},

		methods: {
			/**
			 * Called before form init.
			 */
			async beforeLoad()
			{
				let loadForm = true

				// Execute the "Before init" triggers.
				const triggers = this.getTriggers(qEnums.triggerEvents.beforeInit)
				for (let trigger of triggers)
					await formFunctions.executeTriggerAction(trigger)

				this.emitEvent('before-load-form')

/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL PRO BEFORE_LOAD_JS AGENTE]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */

				return loadForm
			},

			/**
			 * Called after form init.
			 */
			async afterLoad()
			{
				// Execute the "After init" triggers.
				const triggers = this.getTriggers(qEnums.triggerEvents.afterInit)
				for (let trigger of triggers)
					await formFunctions.executeTriggerAction(trigger)

				this.emitEvent('after-load-form')

/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL PRO FORM_LOADED_JS AGENTE]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */
			},

			/**
			 * Called before an apply action is performed.
			 */
			async beforeApply()
			{
				let applyForm = true // Set to 'false' to cancel form apply.

				// Execute the "Before apply" triggers.
				const triggers = this.getTriggers(qEnums.triggerEvents.beforeApply)
				for (let trigger of triggers)
					await formFunctions.executeTriggerAction(trigger)

				applyForm = await this.model.setDocumentChanges()

				if (applyForm)
				{
					const results = await this.model.saveDocuments()
					applyForm = results.every((e) => e === true)
				}

				this.emitEvent('before-apply-form')

/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL PRO BEFORE_APPLY_JS AGENTE]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */

				return applyForm
			},

			/**
			 * Called after an apply action is performed.
			 */
			async afterApply()
			{
				// Execute the "After apply" triggers.
				const triggers = this.getTriggers(qEnums.triggerEvents.afterApply)
				for (let trigger of triggers)
					await formFunctions.executeTriggerAction(trigger)

				this.emitEvent('after-apply-form')

/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL PRO AFTER_APPLY_JS AGENTE]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */
			},

			/**
			 * Called before the record is saved.
			 */
			async beforeSave()
			{
				let saveForm = true // Set to 'false' to cancel form saving.

				// Execute the "Before save" triggers.
				const triggers = this.getTriggers(qEnums.triggerEvents.beforeSave)
				for (let trigger of triggers)
					await formFunctions.executeTriggerAction(trigger)

				saveForm = await this.model.setDocumentChanges()

				if (saveForm)
				{
					const results = await this.model.saveDocuments()
					saveForm = results.every((e) => e === true)
				}

				this.emitEvent('before-save-form')

/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL PRO BEFORE_SAVE_JS AGENTE]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */

				return saveForm
			},

			/**
			 * Called after the record is saved.
			 */
			async afterSave()
			{
				let redirectPage = true // Set to 'false' to cancel page redirect.

				// Execute the "After save" triggers.
				const triggers = this.getTriggers(qEnums.triggerEvents.afterSave)
				for (let trigger of triggers)
					await formFunctions.executeTriggerAction(trigger)

				this.emitEvent('after-save-form')

/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL PRO AFTER_SAVE_JS AGENTE]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */

				return redirectPage
			},

			/**
			 * Called before the record is deleted.
			 */
			async beforeDel()
			{
				let deleteForm = true // Set to 'false' to cancel form delete.

				this.emitEvent('before-delete-form')

/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL PRO BEFORE_DEL_JS AGENTE]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */

				return deleteForm
			},

			/**
			 * Called after the record is deleted.
			 */
			async afterDel()
			{
				let redirectPage = true // Set to 'false' to cancel page redirect.

				this.emitEvent('after-delete-form')

/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL PRO AFTER_DEL_JS AGENTE]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */

				return redirectPage
			},

			/**
			 * Called before leaving the form.
			 */
			async beforeExit()
			{
				let leaveForm = true // Set to 'false' to cancel page redirect.

				// Execute the "Before exit" triggers.
				const triggers = this.getTriggers(qEnums.triggerEvents.beforeExit)
				for (let trigger of triggers)
					await formFunctions.executeTriggerAction(trigger)

				this.emitEvent('before-exit-form')

/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL PRO BEFORE_EXIT_JS AGENTE]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */

				return leaveForm
			},

			/**
			 * Called after leaving the form.
			 */
			async afterExit()
			{
				// Execute the "After exit" triggers.
				const triggers = this.getTriggers(qEnums.triggerEvents.afterExit)
				for (let trigger of triggers)
					await formFunctions.executeTriggerAction(trigger)

				this.emitEvent('after-exit-form')

/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL PRO AFTER_EXIT_JS AGENTE]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */
			},

			/**
			 * Called whenever a field's value is updated.
			 * @param {string} fieldName The name of the field in the format [table].[field] (ex: 'person.name')
			 * @param {object} fieldObject The object representing the field in the model
			 * @param {any} fieldValue The value of the field
			 * @param {any} oldFieldValue The previous value of the field
			 */
			// eslint-disable-next-line
			onUpdate(fieldName, fieldObject, fieldValue, oldFieldValue)
			{
/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL PRO DLGUPDT AGENTE]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */

				this.afterFieldUpdate(fieldName, fieldObject)
			},

			/**
			 * Called whenever a field is unfocused.
			 * @param {*} fieldObject The object representing the field in the model
			 * @param {*} fieldValue The value of the field
			 */
			// eslint-disable-next-line
			onBlur(fieldObject, fieldValue)
			{
/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL PRO CTRLBLR AGENTE]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */

				this.afterFieldUnfocus(fieldObject, fieldValue)
			},

			/**
			 * Called whenever a control's value is updated.
			 * @param {string} controlField The name of the field in the controls that will be updated
			 * @param {object} control The object representing the field in the controls
			 * @param {any} fieldValue The value of the field
			 */
			// eslint-disable-next-line
			onControlUpdate(controlField, control, fieldValue)
			{
/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL PRO CTRLUPD AGENTE]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */

				this.afterControlUpdate(controlField, fieldValue)
			},
/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL PRO FUNCTIONS_JS AGENTE]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */
		},

		watch: {
		}
	}
</script>
