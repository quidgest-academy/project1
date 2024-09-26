/**********************************************************************************
 *                                                                                *
 *                                   ATTENTION!                                   *
 *     Whenever a component is added/removed from this file, or has it's name     *
 *        changed, that must be reflected in the "vue/no-undef-components"        *
 *                          rule of the ".eslintrc" file.                         *
 *                                                                                *
 **********************************************************************************/

import { defineAsyncComponent } from 'vue'

// Wrapper controls
import QControlWrapper from './ControlWrapper.vue'

// Quidgest UI
import {
	QBadge,
	QButton,
	QButtonGroup,
	QButtonToggle,
	QCombobox,
	QField,
	QInputGroup,
	QIcon,
	QIconImg,
	QIconFont,
	QIconSvg,
	QOverlay,
	QPopover,
	QLineLoader,
	QSelect,
	QSpinnerLoader,
	QTextField,
	QTooltip
} from '@quidgest/ui/components'

// Static controls
import QStaticText from './QStaticText.vue'
import QPageBusyState from './QPageBusyState.vue'
import QInfoMessage from './QInfoMessage.vue'

// Inputs controls
import BaseInputStructure from './inputs/BaseInputStructure.vue'
import QPasswordInput from './inputs/PasswordInput.vue'
import QTextareaInput from './inputs/TextareaInput.vue'
import QRadioGroup from './inputs/RadioButtonInput.vue'
import QNumericInput from './inputs/NumericInput.vue'
import QCheckboxInput from './inputs/CheckBoxInput.vue'
import QCheckListInput from './inputs/CheckListInput.vue'
import QToggleInput from './inputs/ToggleInput.vue'
import QMask from './inputs/QMask.vue'
import QListBoxInput from './inputs/ListBoxInput.vue'
import QLookup from './inputs/QLookup.vue'
import VFragment from './VFragment.vue'

export default {
	install: (app) => {
		//---------------------------------------------------------
		// IMPORT COMPONENTS
		// Components here are used so often that there is no advantage in loading them as async
		//---------------------------------------------------------

		// Wrapper controls
		app.component('QControlWrapper', QControlWrapper)
		app.component('VFragment', VFragment)

		// Static controls
		app.component('QBadge', QBadge)
		app.component('QButton', QButton)
		app.component('QButtonGroup', QButtonGroup)
		app.component('QButtonToggle', QButtonToggle)
		app.component('QField', QField)
		app.component('QIcon', QIcon)
		app.component('QIconImg', QIconImg)
		app.component('QIconFont', QIconFont)
		app.component('QIconSvg', QIconSvg)
		app.component('QStaticText', QStaticText)
		app.component('QPopover', QPopover)
		app.component('QPageBusyState', QPageBusyState)
		app.component('QLineLoader', QLineLoader)
		app.component('QSelect', QSelect)
		app.component('QOverlay', QOverlay)
		app.component('QSpinnerLoader', QSpinnerLoader)
		app.component('QTooltip', QTooltip)
		app.component('QInfoMessage', QInfoMessage)

		// Inputs controls
		app.component('BaseInputStructure', BaseInputStructure)
		app.component('QCombobox', QCombobox)
		app.component('QTextField', QTextField)
		app.component('QPasswordInput', QPasswordInput)
		app.component('QTextareaInput', QTextareaInput)
		app.component('QRadioGroup', QRadioGroup)
		app.component('QNumericInput', QNumericInput)
		app.component('QCheckboxInput', QCheckboxInput)
		app.component('QCheckListInput', QCheckListInput)
		app.component('QToggleInput', QToggleInput)
		app.component('QMask', QMask)
		app.component('QListBoxInput', QListBoxInput)
		app.component('QLookup', QLookup)
		app.component('QInputGroup', QInputGroup)

		//---------------------------------------------------------
		// ASYNC COMPONENTS
		// Components here usually have large code or libraries associated with them
		//---------------------------------------------------------

		app.component('QSkeletonLoader', defineAsyncComponent(() => import('./QSkeletonLoader.vue')))
		app.component('GridBaseInputStructure', defineAsyncComponent(() => import('./inputs/GridBaseInputStructure.vue')))
		app.component('QDateTimePicker', defineAsyncComponent(() => import('./inputs/QDateTimePicker.vue')))
		app.component('QTextEditor', defineAsyncComponent(() => import('./inputs/QTextEditor.vue')))
		app.component('QMultiCheckBoxesInput', defineAsyncComponent(() => import('./inputs/MultiCheckBoxesInput.vue')))
		app.component('QColorPickerInput', defineAsyncComponent(() => import('./inputs/ColorPickerInput.vue')))
		app.component('QDocument', defineAsyncComponent(() => import('./inputs/document/QDocument.vue')))
		app.component('QImage', defineAsyncComponent(() => import('./inputs/image/QImage.vue')))
		app.component('QCodeEditor', defineAsyncComponent(() => import('./inputs/code/QCodeEditor.vue')))

		// Container controls
		app.component('QGroupBoxContainer', defineAsyncComponent(() => import('./containers/GroupBoxContainer.vue')))
		app.component('QAccordionContainer', defineAsyncComponent(() => import('./containers/QAccordionContainer.vue')))
		app.component('QGroupCollapsible', defineAsyncComponent(() => import('./containers/QGroupCollapsible.vue')))
		app.component('QRowContainer', defineAsyncComponent(() => import('./containers/RowContainer.vue')))
		app.component('QTabContainer', defineAsyncComponent(() => import('./containers/TabContainer.vue')))
		app.component('QModalContainer', defineAsyncComponent(() => import('./containers/QModalContainer.vue')))
		app.component('QFormContainer', defineAsyncComponent(() => import('./containers/QFormContainer.vue')))
		app.component('QWizard', defineAsyncComponent(() => import('./containers/wizard/QWizard.vue')))
		app.component('QAnchorContainerHorizontal', defineAsyncComponent(() => import('./containers/QAnchorContainerHorizontal.vue')))
		app.component('QAnchorContainerVertical', defineAsyncComponent(() => import('./containers/QAnchorContainerVertical.vue')))
		app.component('QAnchorElement', defineAsyncComponent(() => import('./containers/QAnchorElement.vue')))
		app.component('QCard', defineAsyncComponent(() => import('./containers/QCard.vue')))

		// Rendering controls
		// Render components are used by tables to display fields.
		// Edit components are used by advanced filters, column filters and editable fields in normal tables
		// (different than the ones in the editable table lists).
		app.component('QRenderArray', defineAsyncComponent(() => import('./rendering/QRenderArray.vue')))
		app.component('QRenderBoolean', defineAsyncComponent(() => import('./rendering/QRenderBoolean.vue')))
		app.component('QRenderData', defineAsyncComponent(() => import('./rendering/QRenderData.vue')))
		app.component('QRenderHyperlink', defineAsyncComponent(() => import('./rendering/QRenderHyperlink.vue')))
		app.component('QRenderHtml', defineAsyncComponent(() => import('./rendering/QRenderHtml.vue')))
		app.component('QRenderImage', defineAsyncComponent(() => import('./rendering/QRenderImage.vue')))
		app.component('QRenderDocument', defineAsyncComponent(() => import('./rendering/QRenderDocument.vue')))
		app.component('QEditText', defineAsyncComponent(() => import('./rendering/QEditText.vue')))
		app.component('QEditTextMultiline', defineAsyncComponent(() => import('./rendering/QEditTextMultiline.vue')))
		app.component('QEditNumeric', defineAsyncComponent(() => import('./rendering/QEditNumeric.vue')))
		app.component('QEditBoolean', defineAsyncComponent(() => import('./rendering/QEditBoolean.vue')))
		app.component('QEditDatetime', defineAsyncComponent(() => import('./rendering/QEditDatetime.vue')))
		app.component('QEditEnumeration', defineAsyncComponent(() => import('./rendering/QEditEnumeration.vue')))
		app.component('QEditCheckList', defineAsyncComponent(() => import('./rendering/QEditCheckList.vue')))
		app.component('QEditRadio', defineAsyncComponent(() => import('./rendering/QEditRadio.vue')))

		// Complex controls
		app.component('QPasswordMeter', defineAsyncComponent(() => import('./rendering/QPasswordMeter.vue')))
		app.component('QProgress', defineAsyncComponent(() => import('./rendering/QProgress.vue')))
		app.component('QPropertyList', defineAsyncComponent(() => import('./property-list/QPropertyList.vue')))
		app.component('QTimeline', defineAsyncComponent(() => import('./timeline/QTimeline.vue')))
		app.component('QDashboard', defineAsyncComponent(() => import('./dashboard/QDashboard.vue')))

		// Special renderings
		app.component('QCards', defineAsyncComponent(() => import('./rendering/cards/QCards.vue')))
		app.component('QCarousel', defineAsyncComponent(() => import('./rendering/QCarousel.vue')))
		app.component('QMap', defineAsyncComponent(() => import('./rendering/map/QMap.vue')))

		// Table components
		app.component('QTable', defineAsyncComponent(() => import('./table/QTable.vue')))
		app.component('QTableRecordActionsMenu', defineAsyncComponent(() => import('./table/QTableRecordActionsMenu.vue')))
		app.component('QTableSearch', defineAsyncComponent(() => import('./table/QTableSearch.vue')))
		app.component('QTableExport', defineAsyncComponent(() => import('./table/QTableExport.vue')))
		app.component('QTableImport', defineAsyncComponent(() => import('./table/QTableImport.vue')))
		app.component('QTableStaticFilters', defineAsyncComponent(() => import('./table/QTableStaticFilters.vue')))
		app.component('QTablePagination', defineAsyncComponent(() => import('./table/QTablePagination.vue')))
		app.component('QTablePaginationAlt', defineAsyncComponent(() => import('./table/QTablePaginationAlt.vue')))
		app.component('QTableLimitInfo', defineAsyncComponent(() => import('./table/QTableLimitInfo.vue')))
		app.component('QTableChecklistCheckbox', defineAsyncComponent(() => import('./table/QTableChecklistCheckbox.vue')))
		app.component('QTableSelector', defineAsyncComponent(() => import('./table/QTableSelector.vue')))
		app.component('QTableColumnFilters', defineAsyncComponent(() => import('./table/QTableColumnFilters.vue')))
		app.component('QTableActiveFilters', defineAsyncComponent(() => import('./table/QTableActiveFilters.vue')))
		app.component('QTableActions', defineAsyncComponent(() => import('./table/QTableActions.vue')))
		app.component('QTableConfig', defineAsyncComponent(() => import('./table/QTableConfig.vue')))
		app.component('QTableViews', defineAsyncComponent(() => import('./table/QTableViews.vue')))
		app.component('QTableViewSave',	defineAsyncComponent(() => import('./table/QTableViewSave.vue')))
		app.component('QTableExtraExtension', defineAsyncComponent(() => import('./table/QTableExtraExtension.vue')))
		app.component('QTableViewModeConfig', defineAsyncComponent(() => import('./table/QTableViewModeConfig.vue')))
		app.component('QGridTableList', defineAsyncComponent(() => import('./table/QGridTableList.vue')))
		app.component('QCheckListExtension', defineAsyncComponent(() => import('./extensions/CheckListExtension.vue')))
	}
}
