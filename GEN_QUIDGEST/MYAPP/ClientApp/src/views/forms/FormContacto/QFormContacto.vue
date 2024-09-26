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
			data-key="CONTACTO"
			:data-loading="!formInitialDataLoaded"
			:key="domVersionKey">
			<template v-if="formControl.initialized && showFormBody">
				<q-row-container v-show="controls.CONTACTOCONTCDTCONTAT.isVisible">
					<q-control-wrapper
						v-show="controls.CONTACTOCONTCDTCONTAT.isVisible"
						class="control-join-group">
						<base-input-structure
							class="i-text"
							v-bind="controls.CONTACTOCONTCDTCONTAT"
							v-on="controls.CONTACTOCONTCDTCONTAT.handlers"
							:loading="controls.CONTACTOCONTCDTCONTAT.props.loading"
							:reporting-mode-on="reportingModeCAV"
							:suggestion-mode-on="suggestionModeOn">
							<q-date-time-picker
								v-if="controls.CONTACTOCONTCDTCONTAT.isVisible"
								v-bind="controls.CONTACTOCONTCDTCONTAT.props"
								:model-value="model.ValDtcontat.value"
								@reset-icon-click="model.ValDtcontat.fnUpdateValue(model.ValDtcontat.originalValue ?? new Date())"
								@update:model-value="model.ValDtcontat.fnUpdateValue($event ?? '')" />
						</base-input-structure>
					</q-control-wrapper>
				</q-row-container>
				<q-row-container v-show="controls.CONTACTOPROPRTITULO__.isVisible">
					<q-control-wrapper
						v-show="controls.CONTACTOPROPRTITULO__.isVisible"
						class="control-join-group">
						<base-input-structure
							class="i-text"
							v-bind="controls.CONTACTOPROPRTITULO__"
							v-on="controls.CONTACTOPROPRTITULO__.handlers"
							:loading="controls.CONTACTOPROPRTITULO__.props.loading"
							:reporting-mode-on="reportingModeCAV"
							:suggestion-mode-on="suggestionModeOn">
							<q-lookup
								v-if="controls.CONTACTOPROPRTITULO__.isVisible"
								v-bind="controls.CONTACTOPROPRTITULO__.props"
								v-on="controls.CONTACTOPROPRTITULO__.handlers" />
							<q-see-more-contactoproprtitulo
								v-if="controls.CONTACTOPROPRTITULO__.seeMoreIsVisible"
								v-bind="controls.CONTACTOPROPRTITULO__.seeMoreParams"
								v-on="controls.CONTACTOPROPRTITULO__.handlers" />
						</base-input-structure>
					</q-control-wrapper>
				</q-row-container>
				<q-row-container v-show="controls.CONTACTOCONTCCLTNAME_.isVisible || controls.CONTACTOCONTCCLTEMAIL.isVisible || controls.CONTACTOCONTCTELEFONE.isVisible || controls.CONTACTOCONTCDESCRIIC.isVisible">
					<q-control-wrapper
						v-show="controls.CONTACTOCONTCCLTNAME_.isVisible"
						class="control-join-group">
						<base-input-structure
							class="i-text"
							v-bind="controls.CONTACTOCONTCCLTNAME_"
							v-on="controls.CONTACTOCONTCCLTNAME_.handlers"
							:loading="controls.CONTACTOCONTCCLTNAME_.props.loading"
							:reporting-mode-on="reportingModeCAV"
							:suggestion-mode-on="suggestionModeOn">
							<q-text-field
								v-bind="controls.CONTACTOCONTCCLTNAME_.props"
								:model-value="model.ValCltname.value"
								@blur="onBlur(controls.CONTACTOCONTCCLTNAME_, model.ValCltname.value)"
								@change="model.ValCltname.fnUpdateValueOnChange" />
						</base-input-structure>
					</q-control-wrapper>
					<q-control-wrapper
						v-show="controls.CONTACTOCONTCCLTEMAIL.isVisible"
						class="control-join-group">
						<base-input-structure
							class="i-text"
							v-bind="controls.CONTACTOCONTCCLTEMAIL"
							v-on="controls.CONTACTOCONTCCLTEMAIL.handlers"
							:loading="controls.CONTACTOCONTCCLTEMAIL.props.loading"
							:reporting-mode-on="reportingModeCAV"
							:suggestion-mode-on="suggestionModeOn">
							<q-text-field
								v-bind="controls.CONTACTOCONTCCLTEMAIL.props"
								:model-value="model.ValCltemail.value"
								@blur="onBlur(controls.CONTACTOCONTCCLTEMAIL, model.ValCltemail.value)"
								@change="model.ValCltemail.fnUpdateValueOnChange" />
						</base-input-structure>
					</q-control-wrapper>
					<q-control-wrapper
						v-show="controls.CONTACTOCONTCTELEFONE.isVisible"
						class="control-join-group">
						<base-input-structure
							class="i-text"
							v-bind="controls.CONTACTOCONTCTELEFONE"
							v-on="controls.CONTACTOCONTCTELEFONE.handlers"
							:loading="controls.CONTACTOCONTCTELEFONE.props.loading"
							:reporting-mode-on="reportingModeCAV"
							:suggestion-mode-on="suggestionModeOn">
							<q-mask
								v-if="controls.CONTACTOCONTCTELEFONE.isVisible"
								v-bind="controls.CONTACTOCONTCTELEFONE"
								:model-value="model.ValTelefone.value"
								@update:model-value="model.ValTelefone.fnUpdateValue" />
						</base-input-structure>
					</q-control-wrapper>
					<q-control-wrapper
						v-show="controls.CONTACTOCONTCDESCRIIC.isVisible"
						class="control-join-group">
						<base-input-structure
							class="i-textarea"
							v-bind="controls.CONTACTOCONTCDESCRIIC"
							v-on="controls.CONTACTOCONTCDESCRIIC.handlers"
							:loading="controls.CONTACTOCONTCDESCRIIC.props.loading"
							:reporting-mode-on="reportingModeCAV"
							:suggestion-mode-on="suggestionModeOn">
							<q-textarea-input
								v-if="controls.CONTACTOCONTCDESCRIIC.isVisible"
								id="CONTACTOCONTCDESCRIIC"
								:size="controls.CONTACTOCONTCDESCRIIC.size"
								:model-value="model.ValDescriic.value"
								:rows="5"
								:cols="80"
								:is-required="controls.CONTACTOCONTCDESCRIIC.isRequired"
								:readonly="controls.CONTACTOCONTCDESCRIIC.readonly"
								:placeholder="controls.CONTACTOCONTCDESCRIIC.placeholder"
								@update:model-value="model.ValDescriic.fnUpdateValue" />
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

	import FormViewModel from './QFormContactoViewModel.js'

	const requiredTextResources = ['QFormContacto', 'hardcoded', 'messages']

/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL PRO FORM_INCLUDEJS CONTACTO]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */

	export default {
		name: 'QFormContacto',

		components: {
			QSeeMoreContactoproprtitulo: defineAsyncComponent(() => import('@/views/forms/FormContacto/dbedits/ContactoproprtituloSeeMore.vue')),
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
						name: 'CONTACTO',
						location: 'form-CONTACTO',
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
				componentOnLoadProc: asyncProcM.getProcListMonitor('QFormContacto', false),

				interfaceMetadata: {
					id: 'QFormContacto', // Used for resources
					requiredTextResources
				},

				formInfo: {
					type: 'popup',
					name: 'CONTACTO',
					route: 'form-CONTACTO',
					area: 'CONTC',
					primaryKey: 'ValCodcontc',
					designation: computed(() => this.Resources.CONTACTO_DE_CLIENTE62085),
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
					CONTACTOCONTCDTCONTAT: new fieldControlClass.DateControl({
						modelField: 'ValDtcontat',
						valueChangeEvent: 'fieldChange:contc.dtcontat',
						id: 'CONTACTOCONTCDTCONTAT',
						name: 'DTCONTAT',
						size: 'medium',
						label: computed(() => this.Resources.DATA_DO_CONTACTO52251),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						format: 'date',
						controlLimits: [
						],
					}, this),
					CONTACTOPROPRTITULO__: new fieldControlClass.LookupControl({
						modelField: 'TableProprTitulo',
						valueChangeEvent: 'fieldChange:propr.titulo',
						id: 'CONTACTOPROPRTITULO__',
						name: 'TITULO',
						size: 'xxlarge',
						label: computed(() => this.Resources.PROPRIEDADE00464),
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
							name: 'ValCodpropr',
							dependencyEvent: 'fieldChange:contc.codpropr'
						},
						dependentFields: () => ({
							set 'propr.codpropr'(value) { vm.model.ValCodpropr.updateValue(value) },
							set 'propr.titulo'(value) { vm.model.TableProprTitulo.updateValue(value) },
						}),
						controlLimits: [
						],
					}, this),
					CONTACTOCONTCCLTNAME_: new fieldControlClass.StringControl({
						modelField: 'ValCltname',
						valueChangeEvent: 'fieldChange:contc.cltname',
						id: 'CONTACTOCONTCCLTNAME_',
						name: 'CLTNAME',
						size: 'xxlarge',
						label: computed(() => this.Resources.NOME_DO_CLIENTE38483),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						maxLength: 50,
						labelId: 'label_CONTACTOCONTCCLTNAME_',
						mustBeFilled: true,
						controlLimits: [
						],
					}, this),
					CONTACTOCONTCCLTEMAIL: new fieldControlClass.StringControl({
						modelField: 'ValCltemail',
						valueChangeEvent: 'fieldChange:contc.cltemail',
						id: 'CONTACTOCONTCCLTEMAIL',
						name: 'CLTEMAIL',
						size: 'xxlarge',
						label: computed(() => this.Resources.EMAIL_DO_CLIENTE30111),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						maxLength: 80,
						labelId: 'label_CONTACTOCONTCCLTEMAIL',
						mustBeFilled: true,
						controlLimits: [
						],
					}, this),
					CONTACTOCONTCTELEFONE: new fieldControlClass.MaskControl({
						modelField: 'ValTelefone',
						valueChangeEvent: 'fieldChange:contc.telefone',
						id: 'CONTACTOCONTCTELEFONE',
						name: 'TELEFONE',
						size: 'medium',
						label: computed(() => this.Resources.TELEFONE37757),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						maxLength: 14,
						labelId: 'label_CONTACTOCONTCTELEFONE',
						controlLimits: [
						],
					}, this),
					CONTACTOCONTCDESCRIIC: new fieldControlClass.StringControl({
						modelField: 'ValDescriic',
						valueChangeEvent: 'fieldChange:contc.descriic',
						id: 'CONTACTOCONTCDESCRIIC',
						name: 'DESCRIIC',
						size: 'xxlarge',
						label: computed(() => this.Resources.DESCRICAO07528),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
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
				]),

				tableFields: readonly([
				]),

				timelineFields: readonly([
				]),

				/**
				 * The Data API for easy access to model variables.
				 */
				dataApi: {
					Contc: {
						get ValCltemail() { return vm.model.ValCltemail.value },
						set ValCltemail(value) { vm.model.ValCltemail.updateValue(value) },
						get ValCltname() { return vm.model.ValCltname.value },
						set ValCltname(value) { vm.model.ValCltname.updateValue(value) },
						get ValCodpropr() { return vm.model.ValCodpropr.value },
						set ValCodpropr(value) { vm.model.ValCodpropr.updateValue(value) },
						get ValDescriic() { return vm.model.ValDescriic.value },
						set ValDescriic(value) { vm.model.ValDescriic.updateValue(value) },
						get ValDtcontat() { return vm.model.ValDtcontat.value },
						set ValDtcontat(value) { vm.model.ValDtcontat.updateValue(value) },
						get ValTelefone() { return vm.model.ValTelefone.value },
						set ValTelefone(value) { vm.model.ValTelefone.updateValue(value) },
					},
					Propr: {
						get ValTitulo() { return vm.model.TableProprTitulo.value },
						set ValTitulo(value) { vm.model.TableProprTitulo.updateValue(value) },
					},
					keys: {
						/** The primary key of the CONTC table */
						get contc() { return vm.model.ValCodcontc },
						/** The foreign key to the PROPR table */
						get propr() { return vm.model.ValCodpropr },
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

			to.params.isPopup = 'true'

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
// USE /[MANUAL PRO FORM_CODEJS CONTACTO]/
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
// USE /[MANUAL PRO BEFORE_LOAD_JS CONTACTO]/
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
// USE /[MANUAL PRO FORM_LOADED_JS CONTACTO]/
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
// USE /[MANUAL PRO BEFORE_APPLY_JS CONTACTO]/
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
// USE /[MANUAL PRO AFTER_APPLY_JS CONTACTO]/
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
// USE /[MANUAL PRO BEFORE_SAVE_JS CONTACTO]/
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
// USE /[MANUAL PRO AFTER_SAVE_JS CONTACTO]/
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
// USE /[MANUAL PRO BEFORE_DEL_JS CONTACTO]/
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
// USE /[MANUAL PRO AFTER_DEL_JS CONTACTO]/
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
// USE /[MANUAL PRO BEFORE_EXIT_JS CONTACTO]/
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
// USE /[MANUAL PRO AFTER_EXIT_JS CONTACTO]/
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
// USE /[MANUAL PRO DLGUPDT CONTACTO]/
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
// USE /[MANUAL PRO CTRLBLR CONTACTO]/
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
// USE /[MANUAL PRO CTRLUPD CONTACTO]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */

				this.afterControlUpdate(controlField, fieldValue)
			},
/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL PRO FUNCTIONS_JS CONTACTO]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */
		},

		watch: {
		}
	}
</script>
