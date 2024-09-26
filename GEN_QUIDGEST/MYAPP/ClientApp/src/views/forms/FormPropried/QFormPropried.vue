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
			data-key="PROPRIED"
			:data-loading="!formInitialDataLoaded"
			:key="domVersionKey">
			<template v-if="formControl.initialized && showFormBody">
				<q-row-container v-show="controls.PROPRIEDPROPRIDPROPRE.isVisible || controls.PROPRIEDPROPRVENDIDA_.isVisible">
					<q-control-wrapper
						v-show="controls.PROPRIEDPROPRIDPROPRE.isVisible"
						class="control-join-group">
						<base-input-structure
							class="i-text"
							v-bind="controls.PROPRIEDPROPRIDPROPRE"
							v-on="controls.PROPRIEDPROPRIDPROPRE.handlers"
							:loading="controls.PROPRIEDPROPRIDPROPRE.props.loading"
							:reporting-mode-on="reportingModeCAV"
							:suggestion-mode-on="suggestionModeOn">
							<q-numeric-input
								v-if="controls.PROPRIEDPROPRIDPROPRE.isVisible"
								v-bind="controls.PROPRIEDPROPRIDPROPRE.props"
								@update:model-value="model.ValIdpropre.fnUpdateValue" />
						</base-input-structure>
					</q-control-wrapper>
					<q-control-wrapper
						v-show="controls.PROPRIEDPROPRVENDIDA_.isVisible"
						class="control-join-group">
						<base-input-structure
							class="i-checkbox"
							v-bind="controls.PROPRIEDPROPRVENDIDA_"
							v-on="controls.PROPRIEDPROPRVENDIDA_.handlers"
							:loading="controls.PROPRIEDPROPRVENDIDA_.props.loading"
							:reporting-mode-on="reportingModeCAV"
							:suggestion-mode-on="suggestionModeOn">
							<template #label>
								<q-checkbox-input
									v-if="controls.PROPRIEDPROPRVENDIDA_.isVisible"
									v-bind="controls.PROPRIEDPROPRVENDIDA_.props"
									@update:model-value="model.ValVendida.fnUpdateValue" />
							</template>
						</base-input-structure>
					</q-control-wrapper>
				</q-row-container>
				<q-row-container
					v-show="controls.PROPRIEDPSEUDNEWGRP01.isVisible"
					is-large>
					<q-control-wrapper
						v-show="controls.PROPRIEDPSEUDNEWGRP01.isVisible"
						class="row-line-group">
						<q-group-box-container
							id="PROPRIEDPSEUDNEWGRP01"
							v-bind="controls.PROPRIEDPSEUDNEWGRP01"
							:is-visible="controls.PROPRIEDPSEUDNEWGRP01.isVisible">
							<!-- Start PROPRIEDPSEUDNEWGRP01 -->
							<q-row-container v-show="controls.PROPRIEDPROPRFOTO____.isVisible">
								<q-control-wrapper
									v-show="controls.PROPRIEDPROPRFOTO____.isVisible"
									class="control-join-group">
									<base-input-structure
										class="q-image"
										v-bind="controls.PROPRIEDPROPRFOTO____"
										v-on="controls.PROPRIEDPROPRFOTO____.handlers"
										:loading="controls.PROPRIEDPROPRFOTO____.props.loading"
										:reporting-mode-on="reportingModeCAV"
										:suggestion-mode-on="suggestionModeOn">
										<q-image
											v-if="controls.PROPRIEDPROPRFOTO____.isVisible"
											v-bind="controls.PROPRIEDPROPRFOTO____.props"
											v-on="controls.PROPRIEDPROPRFOTO____.handlers" />
									</base-input-structure>
								</q-control-wrapper>
							</q-row-container>
							<q-row-container v-show="controls.PROPRIEDPROPRTITULO__.isVisible">
								<q-control-wrapper
									v-show="controls.PROPRIEDPROPRTITULO__.isVisible"
									class="control-join-group">
									<base-input-structure
										class="i-text"
										v-bind="controls.PROPRIEDPROPRTITULO__"
										v-on="controls.PROPRIEDPROPRTITULO__.handlers"
										:loading="controls.PROPRIEDPROPRTITULO__.props.loading"
										:reporting-mode-on="reportingModeCAV"
										:suggestion-mode-on="suggestionModeOn">
										<q-text-field
											v-bind="controls.PROPRIEDPROPRTITULO__.props"
											:model-value="model.ValTitulo.value"
											@blur="onBlur(controls.PROPRIEDPROPRTITULO__, model.ValTitulo.value)"
											@change="model.ValTitulo.fnUpdateValueOnChange" />
									</base-input-structure>
								</q-control-wrapper>
							</q-row-container>
							<q-row-container v-show="controls.PROPRIEDPROPRPRECO___.isVisible">
								<q-control-wrapper
									v-show="controls.PROPRIEDPROPRPRECO___.isVisible"
									class="control-join-group">
									<base-input-structure
										class="i-text"
										v-bind="controls.PROPRIEDPROPRPRECO___"
										v-on="controls.PROPRIEDPROPRPRECO___.handlers"
										:loading="controls.PROPRIEDPROPRPRECO___.props.loading"
										:reporting-mode-on="reportingModeCAV"
										:suggestion-mode-on="suggestionModeOn">
										<q-numeric-input
											v-if="controls.PROPRIEDPROPRPRECO___.isVisible"
											v-bind="controls.PROPRIEDPROPRPRECO___.props"
											@update:model-value="model.ValPreco.fnUpdateValue" />
									</base-input-structure>
								</q-control-wrapper>
							</q-row-container>
							<q-row-container v-show="controls.PROPRIEDPROPRDESCRICA.isVisible">
								<q-control-wrapper
									v-show="controls.PROPRIEDPROPRDESCRICA.isVisible"
									class="control-join-group">
									<base-input-structure
										class="i-textarea"
										v-bind="controls.PROPRIEDPROPRDESCRICA"
										v-on="controls.PROPRIEDPROPRDESCRICA.handlers"
										:loading="controls.PROPRIEDPROPRDESCRICA.props.loading"
										:reporting-mode-on="reportingModeCAV"
										:suggestion-mode-on="suggestionModeOn">
										<q-textarea-input
											v-if="controls.PROPRIEDPROPRDESCRICA.isVisible"
											id="PROPRIEDPROPRDESCRICA"
											:size="controls.PROPRIEDPROPRDESCRICA.size"
											:model-value="model.ValDescrica.value"
											:rows="10"
											:cols="80"
											:is-required="controls.PROPRIEDPROPRDESCRICA.isRequired"
											:readonly="controls.PROPRIEDPROPRDESCRICA.readonly"
											:placeholder="controls.PROPRIEDPROPRDESCRICA.placeholder"
											@update:model-value="model.ValDescrica.fnUpdateValue" />
									</base-input-structure>
								</q-control-wrapper>
							</q-row-container>
							<!-- End PROPRIEDPSEUDNEWGRP01 -->
						</q-group-box-container>
					</q-control-wrapper>
				</q-row-container>
				<q-row-container
					v-show="controls.PROPRIEDPSEUDNEWGRP05.isVisible"
					is-large>
					<q-control-wrapper
						v-show="controls.PROPRIEDPSEUDNEWGRP05.isVisible"
						class="row-line-group">
						<q-accordion-container
							id="PROPRIEDPSEUDNEWGRP05"
							v-bind="controls.PROPRIEDPSEUDNEWGRP05"
							v-on="controls.PROPRIEDPSEUDNEWGRP05.handlers"
							v-slot="{ onStateChanged }">
							<!-- Start PROPRIEDPSEUDNEWGRP05 -->
							<q-group-collapsible
								id="PROPRIEDPSEUDNEWGRP02"
								v-bind="controls.PROPRIEDPSEUDNEWGRP02"
								v-on="controls.PROPRIEDPSEUDNEWGRP02.handlers"
								@state-changed="(state, groupId) => onStateChanged(state, groupId)">
								<!-- Start PROPRIEDPSEUDNEWGRP02 -->
								<q-row-container v-show="controls.PROPRIEDCIDADCIDADE__.isVisible">
									<q-control-wrapper
										v-show="controls.PROPRIEDCIDADCIDADE__.isVisible"
										class="control-join-group">
										<base-input-structure
											class="i-text"
											v-bind="controls.PROPRIEDCIDADCIDADE__"
											v-on="controls.PROPRIEDCIDADCIDADE__.handlers"
											:loading="controls.PROPRIEDCIDADCIDADE__.props.loading"
											:reporting-mode-on="reportingModeCAV"
											:suggestion-mode-on="suggestionModeOn">
											<q-lookup
												v-if="controls.PROPRIEDCIDADCIDADE__.isVisible"
												v-bind="controls.PROPRIEDCIDADCIDADE__.props"
												v-on="controls.PROPRIEDCIDADCIDADE__.handlers" />
											<q-see-more-propriedcidadcidade
												v-if="controls.PROPRIEDCIDADCIDADE__.seeMoreIsVisible"
												v-bind="controls.PROPRIEDCIDADCIDADE__.seeMoreParams"
												v-on="controls.PROPRIEDCIDADCIDADE__.handlers" />
										</base-input-structure>
									</q-control-wrapper>
								</q-row-container>
								<q-row-container v-show="controls.PROPRIEDPAIS_PAIS____.isVisible">
									<q-control-wrapper
										v-show="controls.PROPRIEDPAIS_PAIS____.isVisible"
										class="control-join-group">
										<base-input-structure
											class="i-text"
											v-bind="controls.PROPRIEDPAIS_PAIS____"
											v-on="controls.PROPRIEDPAIS_PAIS____.handlers"
											:loading="controls.PROPRIEDPAIS_PAIS____.props.loading"
											:reporting-mode-on="reportingModeCAV"
											:suggestion-mode-on="suggestionModeOn">
											<q-text-field
												v-bind="controls.PROPRIEDPAIS_PAIS____.props"
												:model-value="model.CidadPaisValPais.value" />
										</base-input-structure>
									</q-control-wrapper>
								</q-row-container>
								<q-row-container
									v-show="controls.PROPRIEDPROPRLOCALIZA.isVisible"
									is-large>
									<q-control-wrapper
										v-show="controls.PROPRIEDPROPRLOCALIZA.isVisible"
										class="row-line-group">
										<base-input-structure
											class="i-text"
											v-bind="controls.PROPRIEDPROPRLOCALIZA"
											v-on="controls.PROPRIEDPROPRLOCALIZA.handlers"
											:loading="controls.PROPRIEDPROPRLOCALIZA.props.loading"
											:reporting-mode-on="reportingModeCAV"
											:suggestion-mode-on="suggestionModeOn">
											<q-text-field
												v-bind="controls.PROPRIEDPROPRLOCALIZA.props"
												:model-value="model.ValLocaliza.value"
												@blur="onBlur(controls.PROPRIEDPROPRLOCALIZA, model.ValLocaliza.value)"
												@change="model.ValLocaliza.fnUpdateValueOnChange" />
										</base-input-structure>
									</q-control-wrapper>
								</q-row-container>
								<!-- End PROPRIEDPSEUDNEWGRP02 -->
							</q-group-collapsible>
							<q-group-collapsible
								id="PROPRIEDPSEUDNEWGRP03"
								v-bind="controls.PROPRIEDPSEUDNEWGRP03"
								v-on="controls.PROPRIEDPSEUDNEWGRP03.handlers"
								@state-changed="(state, groupId) => onStateChanged(state, groupId)">
								<!-- Start PROPRIEDPSEUDNEWGRP03 -->
								<q-row-container v-show="controls.PROPRIEDPROPRTIPOPROP.isVisible || controls.PROPRIEDPROPRESPEXTER.isVisible || controls.PROPRIEDPROPRTIPOLOGI.isVisible">
									<q-control-wrapper
										v-show="controls.PROPRIEDPROPRTIPOPROP.isVisible"
										class="control-join-group">
										<base-input-structure
											class="i-text"
											v-bind="controls.PROPRIEDPROPRTIPOPROP"
											v-on="controls.PROPRIEDPROPRTIPOPROP.handlers"
											:loading="controls.PROPRIEDPROPRTIPOPROP.props.loading"
											:reporting-mode-on="reportingModeCAV"
											:suggestion-mode-on="suggestionModeOn">
											<q-select
												v-if="controls.PROPRIEDPROPRTIPOPROP.isVisible"
												v-bind="controls.PROPRIEDPROPRTIPOPROP.props"
												:model-value="model.ValTipoprop.value"
												@update:model-value="model.ValTipoprop.fnUpdateValue" />
										</base-input-structure>
									</q-control-wrapper>
									<q-control-wrapper
										v-show="controls.PROPRIEDPROPRESPEXTER.isVisible"
										class="control-join-group">
										<base-input-structure
											class="i-text"
											v-bind="controls.PROPRIEDPROPRESPEXTER"
											v-on="controls.PROPRIEDPROPRESPEXTER.handlers"
											:loading="controls.PROPRIEDPROPRESPEXTER.props.loading"
											:reporting-mode-on="reportingModeCAV"
											:suggestion-mode-on="suggestionModeOn">
											<q-numeric-input
												v-if="controls.PROPRIEDPROPRESPEXTER.isVisible"
												v-bind="controls.PROPRIEDPROPRESPEXTER.props"
												@update:model-value="model.ValEspexter.fnUpdateValue" />
										</base-input-structure>
									</q-control-wrapper>
									<q-control-wrapper
										v-show="controls.PROPRIEDPROPRTIPOLOGI.isVisible"
										class="control-join-group">
										<base-input-structure
											class="i-radio-container"
											v-bind="controls.PROPRIEDPROPRTIPOLOGI"
											v-on="controls.PROPRIEDPROPRTIPOLOGI.handlers"
											:label-position="labelAlignment.topleft"
											:loading="controls.PROPRIEDPROPRTIPOLOGI.props.loading"
											:reporting-mode-on="reportingModeCAV"
											:suggestion-mode-on="suggestionModeOn">
											<q-radio-group
												v-if="controls.PROPRIEDPROPRTIPOLOGI.isVisible"
												id="PROPRIEDPROPRTIPOLOGI"
												:model-value="model.ValTipologi.value"
												deselect-radio
												:label-left-side="controls.PROPRIEDPROPRTIPOLOGI.labelPosition === labelAlignment.left"
												:number-of-columns="controls.PROPRIEDPROPRTIPOLOGI.columnNumber"
												:is-required="controls.PROPRIEDPROPRTIPOLOGI.isRequired"
												:readonly="controls.PROPRIEDPROPRTIPOLOGI.readonly"
												:options-list="controls.PROPRIEDPROPRTIPOLOGI.items"
												@update:model-value="model.ValTipologi.fnUpdateValue" />
										</base-input-structure>
									</q-control-wrapper>
								</q-row-container>
								<q-row-container v-show="controls.PROPRIEDPROPRTAMANHO_.isVisible || controls.PROPRIEDPROPRNR_WCS__.isVisible">
									<q-control-wrapper
										v-show="controls.PROPRIEDPROPRTAMANHO_.isVisible"
										class="control-join-group">
										<base-input-structure
											class="i-text"
											v-bind="controls.PROPRIEDPROPRTAMANHO_"
											v-on="controls.PROPRIEDPROPRTAMANHO_.handlers"
											:loading="controls.PROPRIEDPROPRTAMANHO_.props.loading"
											:reporting-mode-on="reportingModeCAV"
											:suggestion-mode-on="suggestionModeOn">
											<q-numeric-input
												v-if="controls.PROPRIEDPROPRTAMANHO_.isVisible"
												v-bind="controls.PROPRIEDPROPRTAMANHO_.props"
												@update:model-value="model.ValTamanho.fnUpdateValue" />
										</base-input-structure>
									</q-control-wrapper>
									<q-control-wrapper
										v-show="controls.PROPRIEDPROPRNR_WCS__.isVisible"
										class="control-join-group">
										<base-input-structure
											class="i-text"
											v-bind="controls.PROPRIEDPROPRNR_WCS__"
											v-on="controls.PROPRIEDPROPRNR_WCS__.handlers"
											:loading="controls.PROPRIEDPROPRNR_WCS__.props.loading"
											:reporting-mode-on="reportingModeCAV"
											:suggestion-mode-on="suggestionModeOn">
											<q-numeric-input
												v-if="controls.PROPRIEDPROPRNR_WCS__.isVisible"
												v-bind="controls.PROPRIEDPROPRNR_WCS__.props"
												@update:model-value="model.ValNr_wcs.fnUpdateValue" />
										</base-input-structure>
									</q-control-wrapper>
								</q-row-container>
								<q-row-container v-show="controls.PROPRIEDPROPRDTCONST_.isVisible || controls.PROPRIEDPROPRIDADEPRO.isVisible">
									<q-control-wrapper
										v-show="controls.PROPRIEDPROPRDTCONST_.isVisible"
										class="control-join-group">
										<base-input-structure
											class="i-text"
											v-bind="controls.PROPRIEDPROPRDTCONST_"
											v-on="controls.PROPRIEDPROPRDTCONST_.handlers"
											:loading="controls.PROPRIEDPROPRDTCONST_.props.loading"
											:reporting-mode-on="reportingModeCAV"
											:suggestion-mode-on="suggestionModeOn">
											<q-date-time-picker
												v-if="controls.PROPRIEDPROPRDTCONST_.isVisible"
												v-bind="controls.PROPRIEDPROPRDTCONST_.props"
												:model-value="model.ValDtconst.value"
												@reset-icon-click="model.ValDtconst.fnUpdateValue(model.ValDtconst.originalValue ?? new Date())"
												@update:model-value="model.ValDtconst.fnUpdateValue($event ?? '')" />
										</base-input-structure>
									</q-control-wrapper>
									<q-control-wrapper
										v-show="controls.PROPRIEDPROPRIDADEPRO.isVisible"
										class="control-join-group">
										<base-input-structure
											class="i-text"
											v-bind="controls.PROPRIEDPROPRIDADEPRO"
											v-on="controls.PROPRIEDPROPRIDADEPRO.handlers"
											:loading="controls.PROPRIEDPROPRIDADEPRO.props.loading"
											:reporting-mode-on="reportingModeCAV"
											:suggestion-mode-on="suggestionModeOn">
											<q-numeric-input
												v-if="controls.PROPRIEDPROPRIDADEPRO.isVisible"
												v-bind="controls.PROPRIEDPROPRIDADEPRO.props"
												@update:model-value="model.ValIdadepro.fnUpdateValue" />
										</base-input-structure>
									</q-control-wrapper>
								</q-row-container>
								<!-- End PROPRIEDPSEUDNEWGRP03 -->
							</q-group-collapsible>
							<q-group-collapsible
								id="PROPRIEDPSEUDNEWGRP04"
								v-bind="controls.PROPRIEDPSEUDNEWGRP04"
								v-on="controls.PROPRIEDPSEUDNEWGRP04.handlers"
								@state-changed="(state, groupId) => onStateChanged(state, groupId)">
								<!-- Start PROPRIEDPSEUDNEWGRP04 -->
								<q-row-container v-show="controls.PROPRIEDAGENTNOME____.isVisible">
									<q-control-wrapper
										v-show="controls.PROPRIEDAGENTNOME____.isVisible"
										class="control-join-group">
										<base-input-structure
											class="i-text"
											v-bind="controls.PROPRIEDAGENTNOME____"
											v-on="controls.PROPRIEDAGENTNOME____.handlers"
											:loading="controls.PROPRIEDAGENTNOME____.props.loading"
											:reporting-mode-on="reportingModeCAV"
											:suggestion-mode-on="suggestionModeOn">
											<q-lookup
												v-if="controls.PROPRIEDAGENTNOME____.isVisible"
												v-bind="controls.PROPRIEDAGENTNOME____.props"
												v-on="controls.PROPRIEDAGENTNOME____.handlers" />
											<q-see-more-propriedagentnome
												v-if="controls.PROPRIEDAGENTNOME____.seeMoreIsVisible"
												v-bind="controls.PROPRIEDAGENTNOME____.seeMoreParams"
												v-on="controls.PROPRIEDAGENTNOME____.handlers" />
										</base-input-structure>
									</q-control-wrapper>
								</q-row-container>
								<q-row-container v-show="controls.PROPRIEDAGENTFOTO____.isVisible || controls.PROPRIEDAGENTEMAIL___.isVisible">
									<q-control-wrapper
										v-show="controls.PROPRIEDAGENTFOTO____.isVisible"
										class="control-join-group">
										<base-input-structure
											class="q-image"
											v-bind="controls.PROPRIEDAGENTFOTO____"
											v-on="controls.PROPRIEDAGENTFOTO____.handlers"
											:loading="controls.PROPRIEDAGENTFOTO____.props.loading"
											:reporting-mode-on="reportingModeCAV"
											:suggestion-mode-on="suggestionModeOn">
											<q-image
												v-if="controls.PROPRIEDAGENTFOTO____.isVisible"
												v-bind="controls.PROPRIEDAGENTFOTO____.props"
												v-on="controls.PROPRIEDAGENTFOTO____.handlers" />
										</base-input-structure>
									</q-control-wrapper>
									<q-control-wrapper
										v-show="controls.PROPRIEDAGENTEMAIL___.isVisible"
										class="control-join-group">
										<base-input-structure
											class="i-text"
											v-bind="controls.PROPRIEDAGENTEMAIL___"
											v-on="controls.PROPRIEDAGENTEMAIL___.handlers"
											:loading="controls.PROPRIEDAGENTEMAIL___.props.loading"
											:reporting-mode-on="reportingModeCAV"
											:suggestion-mode-on="suggestionModeOn">
											<q-text-field
												v-bind="controls.PROPRIEDAGENTEMAIL___.props"
												:model-value="model.AgentValEmail.value" />
										</base-input-structure>
									</q-control-wrapper>
								</q-row-container>
								<!-- End PROPRIEDPSEUDNEWGRP04 -->
							</q-group-collapsible>
							<!-- End PROPRIEDPSEUDNEWGRP05 -->
						</q-accordion-container>
					</q-control-wrapper>
				</q-row-container>
				<q-row-container
					v-show="controls.PROPRIEDPSEUDFIELD001.isVisible"
					is-large>
					<q-control-wrapper
						v-show="controls.PROPRIEDPSEUDFIELD001.isVisible"
						class="row-line-group">
						<q-table
							v-show="controls.PROPRIEDPSEUDFIELD001.isVisible"
							v-bind="controls.PROPRIEDPSEUDFIELD001"
							v-on="controls.PROPRIEDPSEUDFIELD001.handlers" />
						<q-table-extra-extension
							:list-ctrl="controls.PROPRIEDPSEUDFIELD001"
							v-on="controls.PROPRIEDPSEUDFIELD001.handlers" />
					</q-control-wrapper>
				</q-row-container>
				<q-row-container
					v-show="controls.PROPRIEDPSEUDFIELD002.isVisible"
					is-large>
					<q-control-wrapper
						v-show="controls.PROPRIEDPSEUDFIELD002.isVisible"
						class="row-line-group">
						<q-table
							v-show="controls.PROPRIEDPSEUDFIELD002.isVisible"
							v-bind="controls.PROPRIEDPSEUDFIELD002"
							v-on="controls.PROPRIEDPSEUDFIELD002.handlers" />
						<q-table-extra-extension
							:list-ctrl="controls.PROPRIEDPSEUDFIELD002"
							v-on="controls.PROPRIEDPSEUDFIELD002.handlers" />
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

	import FormViewModel from './QFormPropriedViewModel.js'

	const requiredTextResources = ['QFormPropried', 'hardcoded', 'messages']

/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL PRO FORM_INCLUDEJS PROPRIED]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */

	export default {
		name: 'QFormPropried',

		components: {
			QSeeMorePropriedcidadcidade: defineAsyncComponent(() => import('@/views/forms/FormPropried/dbedits/PropriedcidadcidadeSeeMore.vue')),
			QSeeMorePropriedagentnome: defineAsyncComponent(() => import('@/views/forms/FormPropried/dbedits/PropriedagentnomeSeeMore.vue')),
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
						name: 'PROPRIED',
						location: 'form-PROPRIED',
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
				componentOnLoadProc: asyncProcM.getProcListMonitor('QFormPropried', false),

				interfaceMetadata: {
					id: 'QFormPropried', // Used for resources
					requiredTextResources
				},

				formInfo: {
					type: 'normal',
					name: 'PROPRIED',
					route: 'form-PROPRIED',
					area: 'PROPR',
					primaryKey: 'ValCodpropr',
					designation: computed(() => this.Resources.PROPRIEDADE00464),
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
					PROPRIEDPROPRIDPROPRE: new fieldControlClass.NumberControl({
						modelField: 'ValIdpropre',
						valueChangeEvent: 'fieldChange:propr.idpropre',
						id: 'PROPRIEDPROPRIDPROPRE',
						name: 'IDPROPRE',
						size: 'small',
						label: computed(() => this.Resources.ID48520),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						maxIntegers: 10,
						maxDecimals: 0,
						isSequencial: true,
						controlLimits: [
						],
					}, this),
					PROPRIEDPROPRVENDIDA_: new fieldControlClass.BooleanControl({
						modelField: 'ValVendida',
						valueChangeEvent: 'fieldChange:propr.vendida',
						id: 'PROPRIEDPROPRVENDIDA_',
						name: 'VENDIDA',
						size: 'mini',
						label: computed(() => this.Resources.VENDIDA08366),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						controlLimits: [
						],
					}, this),
					PROPRIEDPSEUDNEWGRP01: new fieldControlClass.GroupControl({
						id: 'PROPRIEDPSEUDNEWGRP01',
						name: 'NEWGRP01',
						size: 'block',
						label: computed(() => this.Resources.INFORMACAO_PRINCIPAL62886),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						isCollapsible: false,
						anchored: false,
						controlLimits: [
						],
					}, this),
					PROPRIEDPROPRFOTO____: new fieldControlClass.ImageControl({
						modelField: 'ValFoto',
						valueChangeEvent: 'fieldChange:propr.foto',
						id: 'PROPRIEDPROPRFOTO____',
						name: 'FOTO',
						size: 'mini',
						label: computed(() => this.Resources.FOTOGRAFIA36807),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						container: 'PROPRIEDPSEUDNEWGRP01',
						height: 50,
						width: 30,
						maxFileSize: 10485760, // In bytes.
						maxFileSizeLabel: '10 MB',
						controlLimits: [
						],
					}, this),
					PROPRIEDPROPRTITULO__: new fieldControlClass.StringControl({
						modelField: 'ValTitulo',
						valueChangeEvent: 'fieldChange:propr.titulo',
						id: 'PROPRIEDPROPRTITULO__',
						name: 'TITULO',
						size: 'xxlarge',
						label: computed(() => this.Resources.TITULO39021),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						container: 'PROPRIEDPSEUDNEWGRP01',
						maxLength: 80,
						labelId: 'label_PROPRIEDPROPRTITULO__',
						controlLimits: [
						],
					}, this),
					PROPRIEDPROPRPRECO___: new fieldControlClass.CurrencyControl({
						modelField: 'ValPreco',
						valueChangeEvent: 'fieldChange:propr.preco',
						id: 'PROPRIEDPROPRPRECO___',
						name: 'PRECO',
						size: 'small',
						label: computed(() => this.Resources.PRECO50007),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						container: 'PROPRIEDPSEUDNEWGRP01',
						maxIntegers: 5,
						maxDecimals: 4,
						controlLimits: [
						],
					}, this),
					PROPRIEDPROPRDESCRICA: new fieldControlClass.StringControl({
						modelField: 'ValDescrica',
						valueChangeEvent: 'fieldChange:propr.descrica',
						id: 'PROPRIEDPROPRDESCRICA',
						name: 'DESCRICA',
						size: 'xxlarge',
						label: computed(() => this.Resources.DESCRICAO07528),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						container: 'PROPRIEDPSEUDNEWGRP01',
						controlLimits: [
						],
					}, this),
					PROPRIEDPSEUDNEWGRP05: new fieldControlClass.AccordionControl({
						id: 'PROPRIEDPSEUDNEWGRP05',
						name: 'NEWGRP05',
						size: 'block',
						label: computed(() => this.Resources.NEW_GROUP63448),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						isCollapsible: false,
						anchored: false,
						controlLimits: [
						],
					}, this),
					PROPRIEDPSEUDNEWGRP02: new fieldControlClass.GroupControl({
						id: 'PROPRIEDPSEUDNEWGRP02',
						name: 'NEWGRP02',
						size: 'block',
						label: computed(() => this.Resources.LOCALIZACAO54665),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						container: 'PROPRIEDPSEUDNEWGRP05',
						isCollapsible: true,
						anchored: false,
						openingEvent: 'opened-PROPRIEDPSEUDNEWGRP02',
						isInAccordion: true,
						controlLimits: [
						],
					}, this),
					PROPRIEDCIDADCIDADE__: new fieldControlClass.LookupControl({
						modelField: 'TableCidadCidade',
						valueChangeEvent: 'fieldChange:cidad.cidade',
						id: 'PROPRIEDCIDADCIDADE__',
						name: 'CIDADE',
						size: 'xxlarge',
						label: computed(() => this.Resources.CIDADE42080),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						parentOpeningEvent: 'opened-PROPRIEDPSEUDNEWGRP02',
						container: 'PROPRIEDPSEUDNEWGRP02',
						externalCallbacks: {
							getModelField: vm.getModelField,
							getModelFieldValue: vm.getModelFieldValue,
							setModelFieldValue: vm.setModelFieldValue
						},
						externalProperties: {
							modelKeys: computed(() => vm.modelKeys)
						},
						lookupKeyModelField: {
							name: 'ValCodcidad',
							dependencyEvent: 'fieldChange:propr.codcidad'
						},
						dependentFields: () => ({
							set 'cidad.codcidad'(value) { vm.model.ValCodcidad.updateValue(value) },
							set 'cidad.cidade'(value) { vm.model.TableCidadCidade.updateValue(value) },
							set 'pais.pais'(value) { vm.model.CidadPaisValPais.updateValue(value) },
						}),
						controlLimits: [
						],
					}, this),
					PROPRIEDPAIS_PAIS____: new fieldControlClass.StringControl({
						modelField: 'CidadPaisValPais',
						valueChangeEvent: 'fieldChange:pais.pais',
						dependentModelField: 'ValCodpais',
						dependentChangeEvent: 'fieldChange:cidad.codpais',
						id: 'PROPRIEDPAIS_PAIS____',
						name: 'PAIS',
						size: 'xxlarge',
						label: computed(() => this.Resources.PAIS58483),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						parentOpeningEvent: 'opened-PROPRIEDPSEUDNEWGRP02',
						container: 'PROPRIEDPSEUDNEWGRP02',
						maxLength: 50,
						labelId: 'label_PROPRIEDPAIS_PAIS____',
						controlLimits: [
						],
					}, this),
					PROPRIEDPROPRLOCALIZA: new fieldControlClass.FieldSpecialRenderingControl({
						modelField: 'ValLocaliza',
						valueChangeEvent: 'fieldChange:propr.localiza',
						isGeographicShape: false,
						isEuclideanCoord: false,
						id: 'PROPRIEDPROPRLOCALIZA',
						name: 'LOCALIZA',
						size: 'block',
						label: computed(() => this.Resources.LOCALIZACAO54665),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						parentOpeningEvent: 'opened-PROPRIEDPSEUDNEWGRP02',
						container: 'PROPRIEDPSEUDNEWGRP02',
						viewModes: [
							{
								id: 'MAP',
								type: 'map',
								subtype: 'leaflet-map',
								label: computed(() => this.Resources.MAPA24527),
								order: 1,
								implicitVariable: 'geographicData',
								implicitIsMultiple: true,
								mappingVariables: readonly({
									markerIcon: {
										allowsMultiple: false,
										sources: [
											'PROPR.FOTO',
										]
									},
								}),
								styleVariables: {
									zoomLevel: {
										rawValue: -1,
										isMapped: false
									},
									minZoom: {
										rawValue: 0,
										isMapped: false
									},
									maxZoom: {
										rawValue: 18,
										isMapped: false
									},
									zoomWithCtrl: {
										rawValue: true,
										isMapped: false
									},
									fitZoom: {
										rawValue: true,
										isMapped: false
									},
									boundSouthWest: {
										rawValue: undefined,
										isMapped: false
									},
									boundNorthEast: {
										rawValue: undefined,
										isMapped: false
									},
									disableSearch: {
										rawValue: false,
										isMapped: false
									},
									disableControls: {
										rawValue: false,
										isMapped: false
									},
									centerCoord: {
										rawValue: undefined,
										isMapped: false
									},
									showSourcesInDescription: {
										rawValue: true,
										isMapped: false
									},
									collapseLayerOptions: {
										rawValue: false,
										isMapped: false
									},
									crs: {
										rawValue: 'EPSG:4326',
										isMapped: false
									},
									mapHeight: {
										rawValue: '75vh',
										isMapped: false
									},
									allowMarkers: {
										rawValue: true,
										isMapped: false
									},
									allowPolylines: {
										rawValue: true,
										isMapped: false
									},
									allowPolygons: {
										rawValue: true,
										isMapped: false
									},
									allowEdit: {
										rawValue: true,
										isMapped: false
									},
									allowDrag: {
										rawValue: true,
										isMapped: false
									},
									allowCutting: {
										rawValue: true,
										isMapped: false
									},
									allowRemoval: {
										rawValue: true,
										isMapped: false
									},
									allowRotate: {
										rawValue: true,
										isMapped: false
									},
									shapeOutlineWeight: {
										rawValue: 7,
										isMapped: false
									},
									polylineColor: {
										rawValue: '#079ede',
										isMapped: false
									},
									polygonColor: {
										rawValue: '#118f13',
										isMapped: false
									},
									circleColor: {
										rawValue: '#f53505',
										isMapped: false
									},
									groupMarkersInCluster: {
										rawValue: true,
										isMapped: false
									},
									allowExporting: {
										rawValue: true,
										isMapped: false
									},
									backgroundOverlay: {
										rawValue: 'OpenStreetMap',
										isMapped: false
									},
									openPopupOnHover: {
										rawValue: false,
										isMapped: false
									},
								},
								groups: {
									externalLayer: [
									],
								}
							},
						],
						controlLimits: [
						],
					}, this),
					PROPRIEDPSEUDNEWGRP03: new fieldControlClass.GroupControl({
						id: 'PROPRIEDPSEUDNEWGRP03',
						name: 'NEWGRP03',
						size: 'block',
						label: computed(() => this.Resources.DETALHES04088),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						container: 'PROPRIEDPSEUDNEWGRP05',
						isCollapsible: true,
						anchored: false,
						openingEvent: 'opened-PROPRIEDPSEUDNEWGRP03',
						isInAccordion: true,
						controlLimits: [
						],
					}, this),
					PROPRIEDPROPRTIPOPROP: new fieldControlClass.ArrayStringControl({
						modelField: 'ValTipoprop',
						valueChangeEvent: 'fieldChange:propr.tipoprop',
						id: 'PROPRIEDPROPRTIPOPROP',
						name: 'TIPOPROP',
						size: 'mini',
						label: computed(() => this.Resources.TIPO_DE_CONSTRUCAO35217),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						parentOpeningEvent: 'opened-PROPRIEDPSEUDNEWGRP03',
						container: 'PROPRIEDPSEUDNEWGRP03',
						maxLength: 1,
						labelId: 'label_PROPRIEDPROPRTIPOPROP',
						arrayName: 'tipocons',
						controlLimits: [
						],
					}, this),
					PROPRIEDPROPRESPEXTER: new fieldControlClass.NumberControl({
						modelField: 'ValEspexter',
						valueChangeEvent: 'fieldChange:propr.espexter',
						id: 'PROPRIEDPROPRESPEXTER',
						name: 'ESPEXTER',
						size: 'large',
						label: computed(() => this.Resources.ESPACO_EXTERIOR__M2_04786),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						parentOpeningEvent: 'opened-PROPRIEDPSEUDNEWGRP03',
						container: 'PROPRIEDPSEUDNEWGRP03',
						maxIntegers: 4,
						maxDecimals: 2,
						controlLimits: [
						],
					}, this),
					PROPRIEDPROPRTIPOLOGI: new fieldControlClass.ArrayNumberControl({
						modelField: 'ValTipologi',
						valueChangeEvent: 'fieldChange:propr.tipologi',
						id: 'PROPRIEDPROPRTIPOLOGI',
						name: 'TIPOLOGI',
						size: 'small',
						label: computed(() => this.Resources.TIPOLOGIA13928),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						parentOpeningEvent: 'opened-PROPRIEDPSEUDNEWGRP03',
						container: 'PROPRIEDPSEUDNEWGRP03',
						maxIntegers: 1,
						maxDecimals: 0,
						arrayName: 'tipologi',
						columnNumber: 4,
						controlLimits: [
						],
					}, this),
					PROPRIEDPROPRTAMANHO_: new fieldControlClass.NumberControl({
						modelField: 'ValTamanho',
						valueChangeEvent: 'fieldChange:propr.tamanho',
						id: 'PROPRIEDPROPRTAMANHO_',
						name: 'TAMANHO',
						size: 'medium',
						label: computed(() => this.Resources.TAMANHO__M2_40951),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						parentOpeningEvent: 'opened-PROPRIEDPSEUDNEWGRP03',
						container: 'PROPRIEDPSEUDNEWGRP03',
						maxIntegers: 3,
						maxDecimals: 2,
						controlLimits: [
						],
					}, this),
					PROPRIEDPROPRNR_WCS__: new fieldControlClass.NumberControl({
						modelField: 'ValNr_wcs',
						valueChangeEvent: 'fieldChange:propr.nr_wcs',
						id: 'PROPRIEDPROPRNR_WCS__',
						name: 'NR_WCS',
						size: 'large',
						label: computed(() => this.Resources.NUMERO_DE_CASAS_DE_B39783),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						parentOpeningEvent: 'opened-PROPRIEDPSEUDNEWGRP03',
						container: 'PROPRIEDPSEUDNEWGRP03',
						maxIntegers: 3,
						maxDecimals: 0,
						controlLimits: [
						],
					}, this),
					PROPRIEDPROPRDTCONST_: new fieldControlClass.DateControl({
						modelField: 'ValDtconst',
						valueChangeEvent: 'fieldChange:propr.dtconst',
						id: 'PROPRIEDPROPRDTCONST_',
						name: 'DTCONST',
						size: 'medium',
						label: computed(() => this.Resources.DATA_DE_CONTRUCAO03489),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						parentOpeningEvent: 'opened-PROPRIEDPSEUDNEWGRP03',
						container: 'PROPRIEDPSEUDNEWGRP03',
						format: 'date',
						controlLimits: [
						],
					}, this),
					PROPRIEDPROPRIDADEPRO: new fieldControlClass.NumberControl({
						modelField: 'ValIdadepro',
						valueChangeEvent: 'fieldChange:propr.idadepro',
						id: 'PROPRIEDPROPRIDADEPRO',
						name: 'IDADEPRO',
						size: 'large',
						label: computed(() => this.Resources.IDADE_DA_CONSTRUCAO37805),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						parentOpeningEvent: 'opened-PROPRIEDPSEUDNEWGRP03',
						container: 'PROPRIEDPSEUDNEWGRP03',
						isFormulaBlocked: true,
						maxIntegers: 4,
						maxDecimals: 0,
						controlLimits: [
						],
					}, this),
					PROPRIEDPSEUDNEWGRP04: new fieldControlClass.GroupControl({
						id: 'PROPRIEDPSEUDNEWGRP04',
						name: 'NEWGRP04',
						size: 'block',
						label: computed(() => this.Resources.INFORMACAO_DO_AGENTE51492),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						container: 'PROPRIEDPSEUDNEWGRP05',
						isCollapsible: true,
						anchored: false,
						openingEvent: 'opened-PROPRIEDPSEUDNEWGRP04',
						isInAccordion: true,
						controlLimits: [
						],
					}, this),
					PROPRIEDAGENTNOME____: new fieldControlClass.LookupControl({
						modelField: 'TableAgentNome',
						valueChangeEvent: 'fieldChange:agent.nome',
						id: 'PROPRIEDAGENTNOME____',
						name: 'NOME',
						size: 'xxlarge',
						label: computed(() => this.Resources.AGENTE_IMOBILIARIO_R18314),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						parentOpeningEvent: 'opened-PROPRIEDPSEUDNEWGRP04',
						container: 'PROPRIEDPSEUDNEWGRP04',
						externalCallbacks: {
							getModelField: vm.getModelField,
							getModelFieldValue: vm.getModelFieldValue,
							setModelFieldValue: vm.setModelFieldValue
						},
						externalProperties: {
							modelKeys: computed(() => vm.modelKeys)
						},
						lookupKeyModelField: {
							name: 'ValCodagent',
							dependencyEvent: 'fieldChange:propr.codagent'
						},
						dependentFields: () => ({
							set 'agent.codagent'(value) { vm.model.ValCodagent.updateValue(value) },
							set 'agent.nome'(value) { vm.model.TableAgentNome.updateValue(value) },
							set 'agent.foto'(value) { vm.model.AgentValFoto.updateValue(value) },
							set 'agent.email'(value) { vm.model.AgentValEmail.updateValue(value) },
						}),
						controlLimits: [
						],
					}, this),
					PROPRIEDAGENTFOTO____: new fieldControlClass.ImageControl({
						modelField: 'AgentValFoto',
						valueChangeEvent: 'fieldChange:agent.foto',
						dependentModelField: 'ValCodagent',
						dependentChangeEvent: 'fieldChange:propr.codagent',
						id: 'PROPRIEDAGENTFOTO____',
						name: 'FOTO',
						size: 'mini',
						label: computed(() => this.Resources.FOTOGRAFIA36807),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						parentOpeningEvent: 'opened-PROPRIEDPSEUDNEWGRP04',
						container: 'PROPRIEDPSEUDNEWGRP04',
						height: 50,
						width: 30,
						maxFileSize: 10485760, // In bytes.
						maxFileSizeLabel: '10 MB',
						controlLimits: [
						],
					}, this),
					PROPRIEDAGENTEMAIL___: new fieldControlClass.StringControl({
						modelField: 'AgentValEmail',
						valueChangeEvent: 'fieldChange:agent.email',
						dependentModelField: 'ValCodagent',
						dependentChangeEvent: 'fieldChange:propr.codagent',
						id: 'PROPRIEDAGENTEMAIL___',
						name: 'EMAIL',
						size: 'xxlarge',
						label: computed(() => this.Resources.E_MAIL42251),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						parentOpeningEvent: 'opened-PROPRIEDPSEUDNEWGRP04',
						container: 'PROPRIEDPSEUDNEWGRP04',
						maxLength: 80,
						labelId: 'label_PROPRIEDAGENTEMAIL___',
						controlLimits: [
						],
					}, this),
					PROPRIEDPSEUDFIELD001: new fieldControlClass.TableSpecialRenderingControl({
						id: 'PROPRIEDPSEUDFIELD001',
						name: 'FIELD001',
						size: 'block',
						label: computed(() => this.Resources.ALBUM30644),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						controller: 'PROPR',
						action: 'Propried_ValField001',
						hasDependencies: false,
						isInCollapsible: false,
						columnsOriginal: [
							new listColumnTypes.ImageColumn({
								order: 1,
								name: 'ValFoto',
								area: 'ALBUM',
								field: 'FOTO',
								label: computed(() => this.Resources.FOTO19492),
								scrollData: 3,
								sortable: false,
							}),
							new listColumnTypes.TextColumn({
								order: 2,
								name: 'ValTitulo',
								area: 'ALBUM',
								field: 'TITULO',
								label: computed(() => this.Resources.TITULO39021),
								dataLength: 50,
								scrollData: 30,
							}),
						],
						config: {
							name: 'ValField001',
							serverMode: true,
							pkColumn: 'ValCodalbum',
							tableAlias: 'ALBUM',
							tableNamePlural: computed(() => this.Resources.FOTOGRAFIAS36023),
							viewManagement: '',
							showLimitsInfo: true,
							tableTitle: computed(() => this.Resources.ALBUM30644),
							perPage: 10,
							showAlternatePagination: true,
							permissions: {
							},
							globalSearch: {
								visibility: false,
								searchOnPressEnter: true
							},
							filtersVisible: false,
							allowColumnFilters: false,
							allowColumnSort: true,
							crudActions: [
								{
									id: 'show',
									name: 'show',
									title: computed(() => this.Resources.CONSULTAR57388),
									icon: {
										icon: 'view'
									},
									isInReadOnly: true,
									params: {
										canExecuteAction: vm.applyChanges,
										action: vm.openFormAction,
										type: 'form',
										formName: 'FOTOS',
										mode: 'SHOW',
										isControlled: true
									}
								},
								{
									id: 'edit',
									name: 'edit',
									title: computed(() => this.Resources.EDITAR11616),
									icon: {
										icon: 'pencil'
									},
									isInReadOnly: false,
									params: {
										canExecuteAction: vm.applyChanges,
										action: vm.openFormAction,
										type: 'form',
										formName: 'FOTOS',
										mode: 'EDIT',
										isControlled: true
									}
								},
								{
									id: 'duplicate',
									name: 'duplicate',
									title: computed(() => this.Resources.DUPLICAR09748),
									icon: {
										icon: 'duplicate'
									},
									isInReadOnly: false,
									params: {
										canExecuteAction: vm.applyChanges,
										action: vm.openFormAction,
										type: 'form',
										formName: 'FOTOS',
										mode: 'DUPLICATE',
										isControlled: true
									}
								},
								{
									id: 'delete',
									name: 'delete',
									title: computed(() => this.Resources.ELIMINAR21155),
									icon: {
										icon: 'delete'
									},
									isInReadOnly: false,
									params: {
										canExecuteAction: vm.applyChanges,
										action: vm.openFormAction,
										type: 'form',
										formName: 'FOTOS',
										mode: 'DELETE',
										isControlled: true
									}
								}
							],
							generalActions: [
								{
									id: 'insert',
									name: 'insert',
									title: computed(() => this.Resources.INSERIR43365),
									icon: {
										icon: 'add'
									},
									isInReadOnly: false,
									params: {
										canExecuteAction: vm.applyChanges,
										action: vm.openFormAction,
										type: 'form',
										formName: 'FOTOS',
										mode: 'NEW',
										repeatInsertion: false,
										isControlled: true
									}
								},
							],
							generalCustomActions: [
							],
							groupActions: [
							],
							customActions: [
							],
							MCActions: [
							],
							rowClickAction: {
								id: 'RCA__FOTOS',
								name: '_FOTOS',
								title: '',
								isInReadOnly: true,
								params: {
									isRoute: true,
									canExecuteAction: vm.applyChanges,
									action: vm.openFormAction,
									type: 'form',
									formName: 'FOTOS',
									mode: 'SHOW',
									isControlled: true
								}
							},
							formsDefinition: {
								'FOTOS': {
									fnKeySelector: (row) => row.Fields.ValCodalbum,
									isPopup: false
								},
							},
							// The list support form: FOTOS
							crudConditions: {
							},
							defaultSearchColumnName: 'ValTitulo',
							defaultSearchColumnNameOriginal: 'ValTitulo',
							initialSortColumnName: '',
							initialSortColumnOrder: 'asc'
						},
						changeEvents: ['changed-ALBUM', 'changed-PROPR'],
						uuid: 'Propried_ValField001',
						allSelectedRows: 'false',
						viewModes: [
							{
								id: 'CAROUSEL',
								type: 'carousel',
								subtype: '',
								label: computed(() => this.Resources.CARROSSEL41899),
								order: 1,
								mappingVariables: readonly({
									slideTitle: {
										allowsMultiple: false,
										sources: [
											'ALBUM.TITULO',
										]
									},
									slideImage: {
										allowsMultiple: false,
										sources: [
											'ALBUM.FOTO',
										]
									},
								}),
								styleVariables: {
									showIndicators: {
										rawValue: true,
										isMapped: false
									},
									showControls: {
										rawValue: true,
										isMapped: false
									},
									keyboardControllable: {
										rawValue: true,
										isMapped: false
									},
									autoCycleInterval: {
										rawValue: 5000,
										isMapped: false
									},
									autoCyclePause: {
										rawValue: 'hover',
										isMapped: false
									},
									ride: {
										rawValue: 'carousel',
										isMapped: false
									},
									wrap: {
										rawValue: true,
										isMapped: false
									},
								},
								groups: {
								}
							},
						],
						controlLimits: [
							{
								identifier: ['id', 'propr'],
								dependencyEvents: ['fieldChange:propr.codpropr'],
								dependencyField: 'PROPR.CODPROPR',
								fnValueSelector: (model) => model.ValCodpropr.value
							},
						],
					}, this),
					PROPRIEDPSEUDFIELD002: new fieldControlClass.TableListControl({
						id: 'PROPRIEDPSEUDFIELD002',
						name: 'FIELD002',
						size: 'block',
						label: computed(() => this.Resources.CONTACTOS35567),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						controller: 'PROPR',
						action: 'Propried_ValField002',
						hasDependencies: false,
						isInCollapsible: false,
						columnsOriginal: [
							new listColumnTypes.DateColumn({
								order: 1,
								name: 'ValDtcontat',
								area: 'CONTC',
								field: 'DTCONTAT',
								label: computed(() => this.Resources.DATA_DO_CONTACTO52251),
								scrollData: 8,
								dateTimeType: 'date',
							}),
							new listColumnTypes.TextColumn({
								order: 2,
								name: 'ValCltname',
								area: 'CONTC',
								field: 'CLTNAME',
								label: computed(() => this.Resources.NOME_DO_CLIENTE38483),
								dataLength: 50,
								scrollData: 30,
							}),
							new listColumnTypes.TextColumn({
								order: 3,
								name: 'ValDescriic',
								area: 'CONTC',
								field: 'DESCRIIC',
								label: computed(() => this.Resources.DESCRICAO07528),
								scrollData: 30,
							}),
						],
						config: {
							name: 'ValField002',
							serverMode: true,
							pkColumn: 'ValCodcontc',
							tableAlias: 'CONTC',
							tableNamePlural: computed(() => this.Resources.CONTACTOS_DE_CLIENTE38641),
							viewManagement: '',
							showLimitsInfo: true,
							tableTitle: computed(() => this.Resources.CONTACTOS35567),
							perPage: 3,
							showAlternatePagination: true,
							permissions: {
							},
							globalSearch: {
								visibility: false,
								searchOnPressEnter: true
							},
							filtersVisible: false,
							allowColumnFilters: false,
							allowColumnSort: true,
							crudActions: [
								{
									id: 'show',
									name: 'show',
									title: computed(() => this.Resources.CONSULTAR57388),
									icon: {
										icon: 'view'
									},
									isInReadOnly: true,
									params: {
										canExecuteAction: vm.applyChanges,
										action: vm.openFormAction,
										type: 'form',
										formName: 'CONTACTO',
										mode: 'SHOW',
										isControlled: true
									}
								},
								{
									id: 'edit',
									name: 'edit',
									title: computed(() => this.Resources.EDITAR11616),
									icon: {
										icon: 'pencil'
									},
									isInReadOnly: false,
									params: {
										canExecuteAction: vm.applyChanges,
										action: vm.openFormAction,
										type: 'form',
										formName: 'CONTACTO',
										mode: 'EDIT',
										isControlled: true
									}
								},
								{
									id: 'duplicate',
									name: 'duplicate',
									title: computed(() => this.Resources.DUPLICAR09748),
									icon: {
										icon: 'duplicate'
									},
									isInReadOnly: false,
									params: {
										canExecuteAction: vm.applyChanges,
										action: vm.openFormAction,
										type: 'form',
										formName: 'CONTACTO',
										mode: 'DUPLICATE',
										isControlled: true
									}
								},
								{
									id: 'delete',
									name: 'delete',
									title: computed(() => this.Resources.ELIMINAR21155),
									icon: {
										icon: 'delete'
									},
									isInReadOnly: false,
									params: {
										canExecuteAction: vm.applyChanges,
										action: vm.openFormAction,
										type: 'form',
										formName: 'CONTACTO',
										mode: 'DELETE',
										isControlled: true
									}
								}
							],
							generalActions: [
								{
									id: 'insert',
									name: 'insert',
									title: computed(() => this.Resources.INSERIR43365),
									icon: {
										icon: 'add'
									},
									isInReadOnly: false,
									params: {
										canExecuteAction: vm.applyChanges,
										action: vm.openFormAction,
										type: 'form',
										formName: 'CONTACTO',
										mode: 'NEW',
										repeatInsertion: false,
										isControlled: true
									}
								},
							],
							generalCustomActions: [
							],
							groupActions: [
							],
							customActions: [
							],
							MCActions: [
							],
							rowClickAction: {
								id: 'RCA__CONTACTO',
								name: '_CONTACTO',
								title: '',
								isInReadOnly: true,
								params: {
									isRoute: true,
									canExecuteAction: vm.applyChanges,
									action: vm.openFormAction,
									type: 'form',
									formName: 'CONTACTO',
									mode: 'SHOW',
									isControlled: true
								}
							},
							formsDefinition: {
								'CONTACTO': {
									fnKeySelector: (row) => row.Fields.ValCodcontc,
									isPopup: true
								},
							},
							// The list support form: CONTACTO
							crudConditions: {
							},
							defaultSearchColumnName: '',
							defaultSearchColumnNameOriginal: '',
							initialSortColumnName: '',
							initialSortColumnOrder: 'asc'
						},
						changeEvents: ['changed-PROPR', 'changed-CONTC'],
						uuid: 'Propried_ValField002',
						allSelectedRows: 'false',
						controlLimits: [
							{
								identifier: ['id', 'propr'],
								dependencyEvents: ['fieldChange:propr.codpropr'],
								dependencyField: 'PROPR.CODPROPR',
								fnValueSelector: (model) => model.ValCodpropr.value
							},
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
					'PROPRIEDPSEUDNEWGRP01',
					'PROPRIEDPSEUDNEWGRP05',
					'PROPRIEDPSEUDNEWGRP02',
					'PROPRIEDPSEUDNEWGRP03',
					'PROPRIEDPSEUDNEWGRP04',
				]),

				tableFields: readonly([
					'PROPRIEDPSEUDFIELD001',
					'PROPRIEDPSEUDFIELD002',
				]),

				timelineFields: readonly([
				]),

				/**
				 * The Data API for easy access to model variables.
				 */
				dataApi: {
					Agent: {
						get ValEmail() { return vm.model.AgentValEmail.value },
						set ValEmail(value) { vm.model.AgentValEmail.updateValue(value) },
						get ValFoto() { return vm.model.AgentValFoto.value },
						set ValFoto(value) { vm.model.AgentValFoto.updateValue(value) },
						get ValNome() { return vm.model.TableAgentNome.value },
						set ValNome(value) { vm.model.TableAgentNome.updateValue(value) },
					},
					Cidad: {
						get ValCidade() { return vm.model.TableCidadCidade.value },
						set ValCidade(value) { vm.model.TableCidadCidade.updateValue(value) },
					},
					Pais: {
						get ValPais() { return vm.model.CidadPaisValPais.value },
						set ValPais(value) { vm.model.CidadPaisValPais.updateValue(value) },
					},
					Propr: {
						get ValCodagent() { return vm.model.ValCodagent.value },
						set ValCodagent(value) { vm.model.ValCodagent.updateValue(value) },
						get ValCodcidad() { return vm.model.ValCodcidad.value },
						set ValCodcidad(value) { vm.model.ValCodcidad.updateValue(value) },
						get ValDescrica() { return vm.model.ValDescrica.value },
						set ValDescrica(value) { vm.model.ValDescrica.updateValue(value) },
						get ValDtconst() { return vm.model.ValDtconst.value },
						set ValDtconst(value) { vm.model.ValDtconst.updateValue(value) },
						get ValEspexter() { return vm.model.ValEspexter.value },
						set ValEspexter(value) { vm.model.ValEspexter.updateValue(value) },
						get ValFoto() { return vm.model.ValFoto.value },
						set ValFoto(value) { vm.model.ValFoto.updateValue(value) },
						get ValIdadepro() { return vm.model.ValIdadepro.value },
						set ValIdadepro(value) { vm.model.ValIdadepro.updateValue(value) },
						get ValIdpropre() { return vm.model.ValIdpropre.value },
						set ValIdpropre(value) { vm.model.ValIdpropre.updateValue(value) },
						get ValLocaliza() { return vm.model.ValLocaliza.value },
						set ValLocaliza(value) { vm.model.ValLocaliza.updateValue(value) },
						get ValNr_wcs() { return vm.model.ValNr_wcs.value },
						set ValNr_wcs(value) { vm.model.ValNr_wcs.updateValue(value) },
						get ValPreco() { return vm.model.ValPreco.value },
						set ValPreco(value) { vm.model.ValPreco.updateValue(value) },
						get ValTamanho() { return vm.model.ValTamanho.value },
						set ValTamanho(value) { vm.model.ValTamanho.updateValue(value) },
						get ValTipologi() { return vm.model.ValTipologi.value },
						set ValTipologi(value) { vm.model.ValTipologi.updateValue(value) },
						get ValTipoprop() { return vm.model.ValTipoprop.value },
						set ValTipoprop(value) { vm.model.ValTipoprop.updateValue(value) },
						get ValTitulo() { return vm.model.ValTitulo.value },
						set ValTitulo(value) { vm.model.ValTitulo.updateValue(value) },
						get ValVendida() { return vm.model.ValVendida.value },
						set ValVendida(value) { vm.model.ValVendida.updateValue(value) },
					},
					keys: {
						/** The primary key of the PROPR table */
						get propr() { return vm.model.ValCodpropr },
						/** The foreign key to the AGENT table */
						get agent() { return vm.model.ValCodagent },
						/** The foreign key to the CIDAD table */
						get cidad() { return vm.model.ValCodcidad },
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
// USE /[MANUAL PRO FORM_CODEJS PROPRIED]/
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
// USE /[MANUAL PRO BEFORE_LOAD_JS PROPRIED]/
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
// USE /[MANUAL PRO FORM_LOADED_JS PROPRIED]/
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
// USE /[MANUAL PRO BEFORE_APPLY_JS PROPRIED]/
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
// USE /[MANUAL PRO AFTER_APPLY_JS PROPRIED]/
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
// USE /[MANUAL PRO BEFORE_SAVE_JS PROPRIED]/
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
// USE /[MANUAL PRO AFTER_SAVE_JS PROPRIED]/
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
// USE /[MANUAL PRO BEFORE_DEL_JS PROPRIED]/
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
// USE /[MANUAL PRO AFTER_DEL_JS PROPRIED]/
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
// USE /[MANUAL PRO BEFORE_EXIT_JS PROPRIED]/
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
// USE /[MANUAL PRO AFTER_EXIT_JS PROPRIED]/
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
// USE /[MANUAL PRO DLGUPDT PROPRIED]/
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
// USE /[MANUAL PRO CTRLBLR PROPRIED]/
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
// USE /[MANUAL PRO CTRLUPD PROPRIED]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */

				this.afterControlUpdate(controlField, fieldValue)
			},
/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL PRO FUNCTIONS_JS PROPRIED]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */
		},

		watch: {
			// Watchers for changes in the state of tabs and collapsible groups.
			'controls.PROPRIEDPSEUDNEWGRP02.isOpen'(newVal)
			{
				const data = {
					navigationId: this.navigationId,
					key: this.storeKey,
					formInfo: this.formInfo,
					fieldId: 'PROPRIEDPSEUDNEWGRP02',
					containerState: newVal
				}
				this.storeContainerState(data)
			},
			'controls.PROPRIEDPSEUDNEWGRP03.isOpen'(newVal)
			{
				const data = {
					navigationId: this.navigationId,
					key: this.storeKey,
					formInfo: this.formInfo,
					fieldId: 'PROPRIEDPSEUDNEWGRP03',
					containerState: newVal
				}
				this.storeContainerState(data)
			},
			'controls.PROPRIEDPSEUDNEWGRP04.isOpen'(newVal)
			{
				const data = {
					navigationId: this.navigationId,
					key: this.storeKey,
					formInfo: this.formInfo,
					fieldId: 'PROPRIEDPSEUDNEWGRP04',
					containerState: newVal
				}
				this.storeContainerState(data)
			},
		}
	}
</script>
