namespace quidgest.uitests.pages;

[System.CodeDom.Compiler.GeneratedCode("Genio", "")]
public class PropriedForm: PageObject
{
	private By formLocator = By.CssSelector("#form-container");
	private IWebElement form => driver.FindElement(formLocator);

	/// <summary>
	/// ID
	/// </summary>
	public BaseInputControl ProprIdpropre => new BaseInputControl(driver, formLocator, "#PROPRIEDPROPRIDPROPRE");
	/// <summary>
	/// Vendida
	/// </summary>
	public CheckboxInputControl ProprVendida => new CheckboxInputControl(driver, formLocator, "#container-PROPRIEDPROPRVENDIDA_");
	/// <summary>
	/// Informação principal
	/// </summary>
	public CollapsibleZoneControl PseudNewgrp01 => new CollapsibleZoneControl(driver, formLocator, "#PROPRIEDPSEUDNEWGRP01-container");
	/// <summary>
	/// Fotografia
	/// </summary>
	public BaseInputControl ProprFoto => new BaseInputControl(driver, formLocator, "#PROPRIEDPROPRFOTO____");
	/// <summary>
	/// Título
	/// </summary>
	public BaseInputControl ProprTitulo => new BaseInputControl(driver, formLocator, "#PROPRIEDPROPRTITULO__");
	/// <summary>
	/// Preço
	/// </summary>
	public BaseInputControl ProprPreco => new BaseInputControl(driver, formLocator, "#PROPRIEDPROPRPRECO___");
	/// <summary>
	/// Descrição
	/// </summary>
	public BaseInputControl ProprDescrica => new BaseInputControl(driver, formLocator, "#PROPRIEDPROPRDESCRICA");
	/// <summary>
	/// New Group
	/// </summary>
	public IWebElement PseudNewgrp05 => throw new NotImplementedException();
	/// <summary>
	/// Localização
	/// </summary>
	public CollapsibleZoneControl PseudNewgrp02 => new CollapsibleZoneControl(driver, formLocator, "#PROPRIEDPSEUDNEWGRP02-container");
	/// <summary>
	/// Cidade
	/// </summary>
	public LookupControl CidadCidade => new LookupControl(driver, formLocator, "container-PROPRIEDCIDADCIDADE__");
	public SeeMorePage CidadCidadeSeeMorePage => new SeeMorePage(driver, "PROPRIED", "PROPRIEDCIDADCIDADE__");
	/// <summary>
	/// País
	/// </summary>
	public IWebElement PaisPais => throw new NotImplementedException();
	/// <summary>
	/// Localização
	/// </summary>
	public BaseInputControl ProprLocaliza => new BaseInputControl(driver, formLocator, "#PROPRIEDPROPRLOCALIZA");
	/// <summary>
	/// Detalhes
	/// </summary>
	public CollapsibleZoneControl PseudNewgrp03 => new CollapsibleZoneControl(driver, formLocator, "#PROPRIEDPSEUDNEWGRP03-container");
	/// <summary>
	/// Tipo de construção
	/// </summary>
	public EnumControl ProprTipoprop => new EnumControl(driver, formLocator, "container-PROPRIEDPROPRTIPOPROP");
	/// <summary>
	/// Espaço exterior (m2)
	/// </summary>
	public BaseInputControl ProprEspexter => new BaseInputControl(driver, formLocator, "#PROPRIEDPROPRESPEXTER");
	/// <summary>
	/// Tipologia
	/// </summary>
	public RadiobuttonControl ProprTipologi => new RadiobuttonControl(driver, formLocator, "container-PROPRIEDPROPRTIPOLOGI");
	/// <summary>
	/// Tamanho (m2)
	/// </summary>
	public BaseInputControl ProprTamanho => new BaseInputControl(driver, formLocator, "#PROPRIEDPROPRTAMANHO_");
	/// <summary>
	/// Número de casas de banho
	/// </summary>
	public BaseInputControl ProprNr_wcs => new BaseInputControl(driver, formLocator, "#PROPRIEDPROPRNR_WCS__");
	/// <summary>
	/// Data de contrução
	/// </summary>
	public DateInputControl ProprDtconst => new DateInputControl(driver, formLocator, "#PROPRIEDPROPRDTCONST_");
	/// <summary>
	/// Idade da construção
	/// </summary>
	public BaseInputControl ProprIdadepro => new BaseInputControl(driver, formLocator, "#PROPRIEDPROPRIDADEPRO");
	/// <summary>
	/// Informação do agente
	/// </summary>
	public CollapsibleZoneControl PseudNewgrp04 => new CollapsibleZoneControl(driver, formLocator, "#PROPRIEDPSEUDNEWGRP04-container");
	/// <summary>
	/// Agente imobiliário responsável
	/// </summary>
	public LookupControl AgentNome => new LookupControl(driver, formLocator, "container-PROPRIEDAGENTNOME____");
	public SeeMorePage AgentNomeSeeMorePage => new SeeMorePage(driver, "PROPRIED", "PROPRIEDAGENTNOME____");
	/// <summary>
	/// Fotografia
	/// </summary>
	public IWebElement AgentFoto => throw new NotImplementedException();
	/// <summary>
	/// E-mail
	/// </summary>
	public IWebElement AgentEmail => throw new NotImplementedException();
	/// <summary>
	/// Albúm
	/// </summary>
	public ListControl PseudField001 => new ListControl(driver, formLocator, "#PROPRIEDPSEUDFIELD001");
	/// <summary>
	/// Contactos
	/// </summary>
	public ListControl PseudField002 => new ListControl(driver, formLocator, "#PROPRIEDPSEUDFIELD002");

	private IWebElement saveBtn => form.FindElement(By.CssSelector("#bottom-save-btn"));
	private IWebElement cancelBtn => form.FindElement(By.CssSelector("#bottom-cancel-btn"));
	public FORM_MODE mode {get; private set;}

	public PropriedForm(IWebDriver driver, FORM_MODE mode, By subformLocator=null): base(driver)
	{
		this.mode = mode;
		formLocator = subformLocator ?? formLocator;

		wait.Until(c => form);
		WaitForLoading();
	}

	public void WaitForLoading()
	{
		wait.Until(c => form.FindElement(ByData.Key("PROPRIED")).GetAttribute("data-loading") != "true");
	}

	public void Save()
	{
		WaitForLoading();
		saveBtn.Click();
	}

	public void Cancel(bool force = false)
	{
		WaitForLoading();
		cancelBtn.Click();

		// Force the cancel and lose all changes
		if (force)
		{
			ConfirmationPopup confirmPopup = new(driver);
			confirmPopup.Confirm();
		}
	}
}
