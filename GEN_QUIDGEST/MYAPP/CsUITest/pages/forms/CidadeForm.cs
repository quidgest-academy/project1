namespace quidgest.uitests.pages;

[System.CodeDom.Compiler.GeneratedCode("Genio", "")]
public class CidadeForm: PageObject
{
	private By formLocator = By.CssSelector("#form-container");
	private IWebElement form => driver.FindElement(formLocator);

	/// <summary>
	/// Cidade
	/// </summary>
	public BaseInputControl CidadCidade => new BaseInputControl(driver, formLocator, "#CIDADE__CIDADCIDADE__");
	/// <summary>
	/// País
	/// </summary>
	public LookupControl PaisPais => new LookupControl(driver, formLocator, "container-CIDADE__PAIS_PAIS____");
	public SeeMorePage PaisPaisSeeMorePage => new SeeMorePage(driver, "CIDADE", "CIDADE__PAIS_PAIS____");
	/// <summary>
	/// Propriedades
	/// </summary>
	public ListControl PseudField001 => new ListControl(driver, formLocator, "#CIDADE__PSEUDFIELD001");

	private IWebElement saveBtn => form.FindElement(By.CssSelector("#bottom-save-btn"));
	private IWebElement cancelBtn => form.FindElement(By.CssSelector("#bottom-cancel-btn"));
	public FORM_MODE mode {get; private set;}

	public CidadeForm(IWebDriver driver, FORM_MODE mode, By subformLocator=null): base(driver)
	{
		this.mode = mode;
		formLocator = subformLocator ?? formLocator;

		wait.Until(c => form);
		WaitForLoading();
	}

	public void WaitForLoading()
	{
		wait.Until(c => form.FindElement(ByData.Key("CIDADE")).GetAttribute("data-loading") != "true");
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
