namespace quidgest.uitests.pages;

[System.CodeDom.Compiler.GeneratedCode("Genio", "")]
public class AgenteForm: PageObject
{
	private By formLocator = By.CssSelector("#form-container");
	private IWebElement form => driver.FindElement(formLocator);

	/// <summary>
	/// Informação do agente
	/// </summary>
	public CollapsibleZoneControl PseudNewgrp01 => new CollapsibleZoneControl(driver, formLocator, "#AGENTE__PSEUDNEWGRP01-container");
	/// <summary>
	/// Fotografia
	/// </summary>
	public BaseInputControl AgentFoto => new BaseInputControl(driver, formLocator, "#AGENTE__AGENTFOTO____");
	/// <summary>
	/// Nome
	/// </summary>
	public BaseInputControl AgentNome => new BaseInputControl(driver, formLocator, "#AGENTE__AGENTNOME____");
	/// <summary>
	/// Data de nascimento
	/// </summary>
	public DateInputControl AgentDnascime => new DateInputControl(driver, formLocator, "#AGENTE__AGENTDNASCIME");
	/// <summary>
	/// E-mail
	/// </summary>
	public BaseInputControl AgentEmail => new BaseInputControl(driver, formLocator, "#AGENTE__AGENTEMAIL___");
	/// <summary>
	/// Telefone
	/// </summary>
	public BaseInputControl AgentTelefone => new BaseInputControl(driver, formLocator, "#AGENTE__AGENTTELEFONE");
	/// <summary>
	/// País de morada
	/// </summary>
	public LookupControl PmoraPais => new LookupControl(driver, formLocator, "container-AGENTE__PMORAPAIS____");
	public SeeMorePage PmoraPaisSeeMorePage => new SeeMorePage(driver, "AGENTE", "AGENTE__PMORAPAIS____");
	/// <summary>
	/// País de nascimento
	/// </summary>
	public LookupControl PnascPais => new LookupControl(driver, formLocator, "container-AGENTE__PNASCPAIS____");
	public SeeMorePage PnascPaisSeeMorePage => new SeeMorePage(driver, "AGENTE", "AGENTE__PNASCPAIS____");

	private IWebElement saveBtn => form.FindElement(By.CssSelector("#bottom-save-btn"));
	private IWebElement cancelBtn => form.FindElement(By.CssSelector("#bottom-cancel-btn"));
	public FORM_MODE mode {get; private set;}

	public AgenteForm(IWebDriver driver, FORM_MODE mode, By subformLocator=null): base(driver)
	{
		this.mode = mode;
		formLocator = subformLocator ?? formLocator;

		wait.Until(c => form);
		WaitForLoading();
	}

	public void WaitForLoading()
	{
		wait.Until(c => form.FindElement(ByData.Key("AGENTE")).GetAttribute("data-loading") != "true");
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
