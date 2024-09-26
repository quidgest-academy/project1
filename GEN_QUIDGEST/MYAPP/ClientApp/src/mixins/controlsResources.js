import { computed } from 'vue'

class BaseResources
{
	constructor(fnGetResource)
	{
		this._fnGetResource = typeof fnGetResource !== 'function' ? resId => resId : fnGetResource
		Object.defineProperty(this, '_fnGetResource', { enumerable: false })

		this.showHelps = computed(() => this._fnGetResource('CLIQUE_PARA_MOSTRAR_63030'))
	}
}

class TableListMainResources extends BaseResources
{
	constructor(fnGetResource)
	{
		super (fnGetResource)
		this._fnGetResource = typeof fnGetResource !== 'function' ? resId => resId : fnGetResource
		Object.defineProperty(this, '_fnGetResource', { enumerable: false })

		this.actionMenuTitle = computed(() => this._fnGetResource('ACOES22599'))
		this.emptyText = computed(() => this._fnGetResource('SEM_DADOS_PARA_MOSTR24928'))
		this.removeText = computed(() => this._fnGetResource('REMOVER50666'))
		this.importButtonTitle = computed(() => this._fnGetResource('IMPORTAR64751'))
		this.templateButtonTitle = computed(() => this._fnGetResource('SELECIONE_O_TEMPLATE17055'))
		this.submitText = computed(() => this._fnGetResource('SUBMETER21206'))
		this.applyText = computed(() => this._fnGetResource('APLICAR33981'))
		this.closeText = computed(() => this._fnGetResource('FECHAR32496'))
		this.okText = computed(() => this._fnGetResource('OK15819'))
		this.pendingRecords = computed(() => this._fnGetResource('ATENCAO__ESTA_FICHA_24725'))
		this.dropToUpload = computed(() => this._fnGetResource('ARRASTE_FICHEIROS_AT59049'))
		this.saveText = computed(() => this._fnGetResource('GRAVAR45301'))
		this.viewText = computed(() => this._fnGetResource('CONSULTAR57388'))
		this.deleteText = computed(() => this._fnGetResource('ELIMINAR21155'))
		this.duplicateText = computed(() => this._fnGetResource('DUPLICAR09748'))
		this.confirmText = computed(() => this._fnGetResource('CONFIRMAR09808'))
		this.cancelText = computed(() => this._fnGetResource('CANCELAR49513'))
		this.discard = computed(() => this._fnGetResource('DESCARTAR04620'))
		this.resetText = computed(() => this._fnGetResource('REPOR35852'))
		this.insertText = computed(() => this._fnGetResource('INSERIR43365'))
		this.tableConfig = computed(() => this._fnGetResource('DEFINICOES_DA_TABELA04919'))
		this.baseTable = computed(() => this._fnGetResource('TABELA_BASE39739'))
		this.baseTableAsDefault = computed(() => this._fnGetResource('TABELA_BASE_POR_OMIS45720'))
		this.configureColumns = computed(() => this._fnGetResource('CONFIGURAR_COLUNAS42252'))
		this.configureFilters = computed(() => this._fnGetResource('CONFIGURAR_FILTROS27188'))
		this.manageViews = computed(() => this._fnGetResource('GERIR_VISTAS48777'))
		this.createView = computed(() => this._fnGetResource('CRIAR_VISTA61829'))
		this.saveChanges = computed(() => this._fnGetResource('GRAVAR_ALTERACOES12886'))
		this.saveWithOtherName = computed(() => this._fnGetResource('GRAVAR_COM_OUTRO_NOM00327'))
		this.viewModeConfigButtonTitle = computed(() => this._fnGetResource('OPCOES_DE_VISUALIZAC22988'))
		this.toListViewButtonTitle = computed(() => this._fnGetResource('MUDAR_PARA_VISTA_EM_03626'))
		this.toAlternativeViewButtonTitle = computed(() => this._fnGetResource('MUDAR_PARA_VISTA_ALT46064'))
		this.orderText = computed(() => this._fnGetResource('ORDEM38897'))
		this.nameOfColumnText = computed(() => this._fnGetResource('NOME_DA_COLUNA14566'))
		this.visibleText = computed(() => this._fnGetResource('VISIVEL07768'))
		this.searchTextTitle = computed(() => this._fnGetResource('CAIXA_DE_PESQUISA53870'))
		this.searchText = computed(() => this._fnGetResource('PESQUISAR34506'))
		this.forText = computed(() => this._fnGetResource('POR12741'))
		this.ofText = computed(() => this._fnGetResource('DE37566'))
		this.allFieldsText = computed(() => this._fnGetResource('TODOS_OS_CAMPOS47279'))
		this.showText = computed(() => this._fnGetResource('MOSTRAR50268'))
		this.hideText = computed(() => this._fnGetResource('ESCONDER31385'))
		this.filtersText = computed(() => this._fnGetResource('FILTROS01340'))
		this.fieldIsRequired = computed(() => this._fnGetResource('O_CAMPO__0__E_OBRIGA36687'))
		this.isRequired = computed(() => this._fnGetResource('E_OBRIGATORIO35368'))
		this.limitsButtonTitle = computed(() => this._fnGetResource('LIMITE12596'))
		this.limitsListTitlePrepend = computed(() => this._fnGetResource('A_INFORMACAO_NA_LIST25711'))
		this.limitsListTitleAppend = computed(() => this._fnGetResource('ESTA_LIMITADA_POR50241'))
		this.textRowsSelected = computed(() => this._fnGetResource('REGISTO_S__SELECIONA64172'))
		this.hasTextWrapText = computed(() => this._fnGetResource('QUEBRA_DE_LINHA53477'))
		this.groupActionsText = computed(() => this._fnGetResource('ACOES_COLETIVAS25162'))
		this.advancedFiltersText = computed(() => this._fnGetResource('FILTROS_AVANCADOS32501'))
		this.applyFilterText = computed(() => this._fnGetResource('APLICAR_FILTRO50221'))
		this.applyFiltersText = computed(() => this._fnGetResource('APLICAR_FILTROS22808'))
		this.createFilterText = computed(() => this._fnGetResource('CRIAR_FILTRO61015'))
		this.filterNameText = computed(() => this._fnGetResource('NOME_DO_FILTRO02285'))
		this.orText = computed(() => this._fnGetResource('OU11765'))
		this.createConditionText = computed(() => this._fnGetResource('CRIAR_CONDICAO10949'))
		this.removeConditionText = computed(() => this._fnGetResource('REMOVER_CONDICAO64117'))
		this.savedFiltersText = computed(() => this._fnGetResource('FILTROS_GRAVADOS05983'))
		this.saveFilterText = computed(() => this._fnGetResource('GRAVAR_FILTRO16375'))
		this.deleteFilterText = computed(() => this._fnGetResource('REMOVER_FILTRO51662'))
		this.deleteFiltersText = computed(() => this._fnGetResource('REMOVER_FILTROS62153'))
		this.activateFilterText = computed(() => this._fnGetResource('ACTIVAR_FILTRO21924'))
		this.deactivateFilterText = computed(() => this._fnGetResource('DESACTIVAR_FILTRO34573'))
		this.columnActionsText = computed(() => this._fnGetResource('ACOES_DA_COLUNA45080'))
		this.sortText = computed(() => this._fnGetResource('ORDENAR00426'))
		this.ascendingText = computed(() => this._fnGetResource('ASCENDENTE30808'))
		this.descendingText = computed(() => this._fnGetResource('DESCENDENTE19792'))
		this.sortAscendingText = computed(() => this._fnGetResource('ORDENAR_ASCENDENTE32130'))
		this.sortDescendingText = computed(() => this._fnGetResource('ORDENAR_DESCENDENTE63669'))
		this.moveToAdvancedFiltersText = computed(() => this._fnGetResource('MOVER_PARA_FILTROS_A24438'))
		this.staticFiltersTitle = computed(() => this._fnGetResource('FILTROS_GLOBAIS30027'))
		this.activeFiltersTitle = computed(() => this._fnGetResource('FILTROS_ATIVOS07219'))
		this.removeAllText = computed(() => this._fnGetResource('REMOVER_TODOS43893'))
		this.rowDragAndDropTitle = computed(() => this._fnGetResource('REORDENAR52758'))
		this.exportButtonTitle = computed(() => this._fnGetResource('EXPORTAR35632'))
		this.defaultKeywordSearchText = computed(() => this._fnGetResource('PESQUISA_PREDEFINIDA03529'))
		this.lineBreak = computed(() => this._fnGetResource('QUEBRA_DE_LINHA53477'))
		this.yesLabel = computed(() => this._fnGetResource('SIM28552'))
		this.noLabel = computed(() => this._fnGetResource('NAO06521'))
		this.activeText = computed(() => this._fnGetResource('ACTIVE03270'))
		this.inactiveText = computed(() => this._fnGetResource('INACTIVE23138'))
		this.showRecordsWhereText = computed(() => this._fnGetResource('MOSTRAR_REGISTOS_QUA55160'))
		this.visibleColumnsText = computed(() => this._fnGetResource('COLUNAS_VISIVEIS27717'))
		this.selectView = computed(() => this._fnGetResource('SELECIONAR_VISTA16672'))
		this.saveViewText = computed(() => this._fnGetResource('GUARDAR_VISTA35229'))
		this.savedView = computed(() => this._fnGetResource('VISTA_GRAVADA55829'))
		this.viewManagerText = computed(() => this._fnGetResource('GESTOR_DE_VISTAS43375'))
		this.clearResizeText = computed(() => this._fnGetResource('LIMPAR_REDIMENSIONAM00007'))
		this.viewExistsText = computed(() => this._fnGetResource('ESSA_VISTA_JA_EXISTE52743'))
		this.wantToOverwriteText = computed(() => this._fnGetResource('DESEJA_SUBSTITUI_LA_25718'))
		this.wantToSaveChanges = computed(() => this._fnGetResource('QUER_GRAVAR_AS_ALTER22033'))
		this.wantToSaveChangesToView = computed(() => this._fnGetResource('SALVAR_AS_ALTERACOES51739'))
		this.tableViewSaveSuccess = computed(() => this._fnGetResource('VISUALIZACAO_DE_TABE40128'))
		this.viewNameText = computed(() => this._fnGetResource('NOME_DA_VISTA31135'))
		this.setDefaultViewText = computed(() => this._fnGetResource('DEFINIR_COMO_VISTA_P09954'))
		this.defaultViewText = computed(() => this._fnGetResource('VISTA_PREDEFINIDA61222'))
		this.downloadTemplateText = computed(() => this._fnGetResource('FACA_O_DOWNLOAD_DO_A43406'))
		this.fillTemplateFileText = computed(() => this._fnGetResource('PREENCHA_O_FICHEIRO_42287'))
		this.importTemplateFileText = computed(() => this._fnGetResource('APOS_PREENCHER_O_FIC53348'))
		this.allRecordsText = computed(() => this._fnGetResource('TODOS59977'))
		this.currentPageText = computed(() => this._fnGetResource('PAGINA_ATUAL46671'))
		this.rowsPerPage = computed(() => this._fnGetResource('LINHAS_POR_PAGINA55027'))
		this.gotToPage = computed(() => this._fnGetResource('IR_PARA_PAGINA12084'))
		this.noneText = computed(() => this._fnGetResource('NENHUM21531'))
		this.loading = computed(() => this._fnGetResource('A_CARREGAR___34906'))
		this.onDate = computed(() => this._fnGetResource('EM_32327'))
		this.state = computed(() => this._fnGetResource('ESTADO07788'))
		this.first = computed(() => this._fnGetResource('PRIMEIRA43991'))
		this.last = computed(() => this._fnGetResource('ULTIMA04868'))
		this.previous = computed(() => this._fnGetResource('ANTERIOR34904'))
		this.next = computed(() => this._fnGetResource('PROXIMO29858'))
	}
}

class ImportExportResources extends BaseResources
{
	constructor(fnGetResource)
	{
		super (fnGetResource)
		this._fnGetResource = typeof fnGetResource !== 'function' ? resId => resId : fnGetResource
		Object.defineProperty(this, '_fnGetResource', { enumerable: false })
	}

	get exportOptions()
	{
		return [
			{ id: 'pdf', text: computed(() => this._fnGetResource('FORMATO_DE_DOCUMENTO48724')) },
			{ id: 'ods', text: computed(() => this._fnGetResource('FOLHA_DE_CALCULO__OD46941')) },
			{ id: 'xlsx', text: computed(() => this._fnGetResource('FOLHA_DE_CALCULO_EXC59518')) },
			{ id: 'csv', text: computed(() => this._fnGetResource('VALORES_SEPARADOS_PO10397')) },
			{ id: 'xml', text: computed(() => this._fnGetResource('FORMATO_XML__XML_44251')) },
		]
	}

	get importOptions()
	{
		return [
			{ key: 'xlsx', value: computed(() => this._fnGetResource('FOLHA_DE_CALCULO_EXC59518')) }
		]
	}

	get importTemplateOptions()
	{
		return [
			{ key: 'xlsx', value: computed(() => this._fnGetResource('DOWNLOAD_DE_TEMPLATE48385')) }
		]
	}
}

class MultipleValuesExtensionResources extends BaseResources
{
	constructor(fnGetResource)
	{
		super (fnGetResource)
		this._fnGetResource = typeof fnGetResource !== 'function' ? resId => resId : fnGetResource
		Object.defineProperty(this, '_fnGetResource', { enumerable: false })

		this.placeholder = computed(() => this._fnGetResource('PROCURAR_EM64879'))
		this.addButtonTitle = computed(() => this._fnGetResource('ADICIONA17889'))
		this.searchInputTitle = computed(() => this._fnGetResource('PROCURAR15982'))
	}
}

class LookupResources extends BaseResources
{
	constructor(fnGetResource)
	{
		super (fnGetResource)
		this._fnGetResource = typeof fnGetResource !== 'function' ? resId => resId : fnGetResource
		Object.defineProperty(this, '_fnGetResource', { enumerable: false })

		this.placeholder = computed(() => this._fnGetResource('ESCOLHA___40245'))
		this.noData = computed(() => this._fnGetResource('SEM_DADOS_PARA_MOSTR24928'))
		this.viewDetails = computed(() => this._fnGetResource('VIEW_DETAILS09924'))
		this.viewMoreOptions = computed(() => this._fnGetResource('VER_MAIS32592'))
	}
}

class DocumentResources extends BaseResources
{
	constructor(fnGetResource)
	{
		super (fnGetResource)
		this._fnGetResource = typeof fnGetResource !== 'function' ? resId => resId : fnGetResource
		Object.defineProperty(this, '_fnGetResource', { enumerable: false })

		this.downloadLabel = computed(() => this._fnGetResource('DESCARREGAR58418'))
		this.attachLabel = computed(() => this._fnGetResource('ANEXAR20848'))
		this.submitLabel = computed(() => this._fnGetResource('SUBMETER21206'))
		this.editLabel = computed(() => this._fnGetResource('EDITAR11616'))
		this.chooseFileLabel = computed(() => this._fnGetResource('ESCOLHER_FICHEIRO44506'))
		this.deleteLabel = computed(() => this._fnGetResource('APAGAR04097'))
		this.propertyLabel = computed(() => this._fnGetResource('PROPRIEDADES45924'))
		this.versionsLabel = computed(() => this._fnGetResource('VERSOES25682'))
		this.viewAllLabel = computed(() => this._fnGetResource('VER_TODAS___44710'))
		this.deleteLastLabel = computed(() => this._fnGetResource('APAGAR_ULTIMA25492'))
		this.deleteHistoryLabel = computed(() => this._fnGetResource('APAGAR_HISTORICO26221'))
		this.nameLabel = computed(() => this._fnGetResource('NOME__48276'))
		this.sizeLabel = computed(() => this._fnGetResource('TAMANHO__48454'))
		this.extensionLabel = computed(() => this._fnGetResource('EXTENSAO__24742'))
		this.authorLabel = computed(() => this._fnGetResource('AUTOR__36547'))
		this.createdDateLabel = computed(() => this._fnGetResource('DATA_DE_CRIACAO__05001'))
		this.createdOnLabel = computed(() => this._fnGetResource('DATA_DE_CRIACAO16914'))
		this.currentVersionLabel = computed(() => this._fnGetResource('VERSAO_ATUAL__01161'))
		this.editedByLabel = computed(() => this._fnGetResource('EM_EDICAO_POR__14850'))
		this.okLabel = computed(() => this._fnGetResource('OK15819'))
		this.yesLabel = computed(() => this._fnGetResource('SIM28552'))
		this.noLabel = computed(() => this._fnGetResource('NAO06521'))
		this.filesSubmission = computed(() => this._fnGetResource('SUBMISSAO_DE_FICHEIR50281'))
		this.noFileSelected = computed(() => this._fnGetResource('NENHUM_FICHEIRO_SELE48024'))
		this.fileSizeError = computed(() => this._fnGetResource('O_FICHEIRO_SELECIONA49645'))
		this.extensionError = computed(() => this._fnGetResource('EXTENSAO_INVALIDA__E46375'))
		this.submitHeaderLabel = computed(() => this._fnGetResource('SELECCIONE_O_FICHEIR54804'))
		this.unlockHeaderLabel = computed(() => this._fnGetResource('DESBLOQUEAR__IGNORA_48447'))
		this.submitFilesHeaderLabel = computed(() => this._fnGetResource('SUBMETER__DESBLOQUEI57783'))
		this.majorVersionLabel = computed(() => this._fnGetResource('VERSAO_PRINCIPAL03233'))
		this.minorVersionLabel = computed(() => this._fnGetResource('VERSAO_SECUNDARIA37682'))
		this.cancelLabelValue = computed(() => this._fnGetResource('CANCELAR49513'))
		this.version = computed(() => this._fnGetResource('VERSAO61228'))
		this.documentLabel = computed(() => this._fnGetResource('DOCUMENTO60418'))
		this.bytesLabel = computed(() => this._fnGetResource('BYTES25864'))
		this.author = computed(() => this._fnGetResource('AUTOR45670'))
		this.deleteHeaderLabel = computed(() => this._fnGetResource('TEM_A_CERTEZA_QUE_QU37043'))
		this.actionLabel = computed(() => this._fnGetResource('ACOES22599'))
		this.viewAll = computed(() => this._fnGetResource('VER_TODAS10532'))
		this.closeLabel = computed(() => this._fnGetResource('FECHAR32496'))
		this.theLastVersionWillEliminate = computed(() => this._fnGetResource('A_ULTIMA_VERSAO_VAI_40630'))
		this.allTheVersionsExceptLastWillEliminate = computed(() => this._fnGetResource('TODAS_AS_VERSOES_EXC52356'))
		this.uploadDocVersionHeader = computed(() => this._fnGetResource('VERSOES_DO_DOCUMENTO34166'))
		this.createDocument = computed(() => this._fnGetResource('CRIAR_DOCUMENTO55731'))
		this.editingDocument = computed(() => this._fnGetResource('ESTE_DOCUMENTO_ENCON39456'))
		this.pendingDocumentVersion = computed(() => this._fnGetResource('ESTA_VERSAO_DO_DOCUM23227'))
		this.errorProcessingRequest = computed(() => this._fnGetResource('OCORREU_UM_ERRO_AO_P53091'))
	}
}

class ImageResources extends BaseResources
{
	constructor(fnGetResource)
	{
		super (fnGetResource)
		this._fnGetResource = typeof fnGetResource !== 'function' ? resId => resId : fnGetResource
		Object.defineProperty(this, '_fnGetResource', { enumerable: false })

		this.submitLabel = computed(() => this._fnGetResource('SUBMETER21206'))
		this.editLabel = computed(() => this._fnGetResource('EDITAR11616'))
		this.deleteLabel = computed(() => this._fnGetResource('APAGAR04097'))
		this.fileSizeError = computed(() => this._fnGetResource('O_FICHEIRO_SELECIONA49645'))
		this.extensionError = computed(() => this._fnGetResource('EXTENSAO_INVALIDA__E46375'))
		this.editImage = computed(() => this._fnGetResource('EDITAR_IMAGEM49158'))
		this.cropWarning = computed(() => this._fnGetResource('ATENCAO__AO_GRAVAR_E10841'))
		this.dropToUpload = computed(() => this._fnGetResource('ARRASTE_FICHEIROS_AT59049'))
		this.save = computed(() => this._fnGetResource('GRAVAR45301'))
		this.cancel = computed(() => this._fnGetResource('CANCELAR49513'))
		this.zoomIn = computed(() => this._fnGetResource('ZOOM_IN39873'))
		this.zoomOut = computed(() => this._fnGetResource('ZOOM_OUT31562'))
		this.moveImageLeft = computed(() => this._fnGetResource('MOVER_IMAGEM_PARA_A_24246'))
		this.moveImageRight = computed(() => this._fnGetResource('MOVER_IMAGEM_PARA_A_35927'))
		this.moveImageUp = computed(() => this._fnGetResource('MOVER_IMAGEM_PARA_CI59923'))
		this.moveImageDown = computed(() => this._fnGetResource('MOVER_IMAGEM_PARA_BA49541'))
		this.rotateLeft = computed(() => this._fnGetResource('VIRAR_A_ESQUERDA33355'))
		this.rotateRight = computed(() => this._fnGetResource('VIRAR_A_DIREITA03453'))
		this.flipHorizontal = computed(() => this._fnGetResource('VIRAR_NA_HORIZONTAL60231'))
		this.flipVertical = computed(() => this._fnGetResource('VIRAR_NA_VERTICAL20218'))
		this.deleteHeaderLabel = computed(() => this._fnGetResource('TEM_A_CERTEZA_QUE_QU37043'))
		this.yesLabel = computed(() => this._fnGetResource('SIM28552'))
		this.noLabel = computed(() => this._fnGetResource('NAO06521'))
		this.close = computed(() => this._fnGetResource('FECHAR32496'))
		this.download = computed(() => this._fnGetResource('DESCARREGAR58418'))
	}
}

class DashboardResources extends BaseResources
{
	constructor(fnGetResource)
	{
		super (fnGetResource)
		this._fnGetResource = typeof fnGetResource !== 'function' ? resId => resId : fnGetResource
		Object.defineProperty(this, '_fnGetResource', { enumerable: false })

		this.editButtonTitle = computed(() => this._fnGetResource('EDITAR11616'))
		this.compactButtonTitle = computed(() => this._fnGetResource('COMPACTAR38838'))
		this.saveButtonTitle = computed(() => this._fnGetResource('GRAVAR45301'))
		this.cancelButtonTitle = computed(() => this._fnGetResource('CANCELAR49513'))
		this.helpText = computed(() => this._fnGetResource('PARA_ADICIONAR_UM_WI63588'))
		this.addButtonTitle = computed(() => this._fnGetResource('ADICIONAR14072'))
		this.addWidgetText = computed(() => this._fnGetResource('ADICIONAR_WIDGET21299'))
		this.noRecordsText = computed(() => this._fnGetResource('SEM_REGISTOS62529'))
		this.noDataText = computed(() => this._fnGetResource('SEM_DADOS_PARA_MOSTR24928'))
		this.previousPageText = computed(() => this._fnGetResource('PAGINA_ANTERIOR17471'))
		this.nextPageText = computed(() => this._fnGetResource('PAGINA_SEGUINTE34153'))
		this.removeButtonText = computed(() => this._fnGetResource('REMOVER14367'))
		this.refreshButtonText = computed(() => this._fnGetResource('ATUALIZAR22496'))
	}
}

class TimelineResources extends BaseResources
{
	constructor(fnGetResource)
	{
		super (fnGetResource)
		this._fnGetResource = typeof fnGetResource !== 'function' ? resId => resId : fnGetResource
		Object.defineProperty(this, '_fnGetResource', { enumerable: false })

		this.reset = computed(() => this._fnGetResource('REPOR35852'))
		this.daily = computed(() => this._fnGetResource('DIARIA21794'))
		this.weekly = computed(() => this._fnGetResource('SEMANAL19148'))
		this.monthly = computed(() => this._fnGetResource('MENSAL53343'))
		this.yearly = computed(() => this._fnGetResource('ANUAL55239'))
	}
}

class WizardResources extends BaseResources
{
	constructor(fnGetResource)
	{
		super (fnGetResource)
		this._fnGetResource = typeof fnGetResource !== 'function' ? resId => resId : fnGetResource
		Object.defineProperty(this, '_fnGetResource', { enumerable: false })

		this.showNextSteps = computed(() => this._fnGetResource('CLIQUE_PARA_MOSTRAR_48694'))
		this.showPrevSteps = computed(() => this._fnGetResource('CLIQUE_PARA_MOSTRAR_07566'))
	}
}

class FormContainerResources extends BaseResources
{
	constructor(fnGetResource)
	{
		super (fnGetResource)
		this._fnGetResource = typeof fnGetResource !== 'function' ? resId => resId : fnGetResource
		Object.defineProperty(this, '_fnGetResource', { enumerable: false })

		this.chooseElement = computed(() => this._fnGetResource('ESCOLHA_UM_ELEMENTO_24060'))
		this.or = computed(() => this._fnGetResource('OU11765'))
		this.insert = computed(() => this._fnGetResource('INSERIR43365'))
	}
}

class CodeEditorResources extends BaseResources
{
	constructor(fnGetResource)
	{
		super (fnGetResource)
		this._fnGetResource = typeof fnGetResource !== 'function' ? resId => resId : fnGetResource
		Object.defineProperty(this, '_fnGetResource', { enumerable: false })

		this.showChanges = computed(() => this._fnGetResource('MOSTRAR_ALTERACOES47173'))
		this.dark = computed(() => this._fnGetResource('ESCURO16457'))
		this.light = computed(() => this._fnGetResource('CLARO60841'))
		this.theme = computed(() => this._fnGetResource('TEMA56931'))
		this.defaultPlaceholder = computed(() => this._fnGetResource('ESCREVA_O_SEU_CODIGO16246'))
	}
}

export default {
	TableListMainResources,
	ImportExportResources,
	MultipleValuesExtensionResources,
	LookupResources,
	DocumentResources,
	ImageResources,
	DashboardResources,
	TimelineResources,
	WizardResources,
	FormContainerResources,
	CodeEditorResources
}
