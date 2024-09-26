namespace quidgest.uitests.pages;

[System.CodeDom.Compiler.GeneratedCode("Genio", "")]
public class ContactoForm: PageObject
{
	private By formLocator = By.CssSelector("#q-modal-form-CONTACTO");
	private IWebElement form => driver.FindElement(formLocator);

	/// <summary>
	/// Data do contacto
	/// </summary>
	public DateInputControl ContcDtcontat => new DateInputControl(driver, formLocator, "#CONTACTOCONTCDTCONTAT");
	/// <summary>
	/// Propriedade
	/// </summary>
	public LookupControl ProprTitulo => new LookupControl(driver, formLocator, "container-CONTACTOPROPRTITULO__");
	public SeeMorePage ProprTituloSeeMorePage => new SeeMorePage(driver, "CONTACTO", "CONTACTOPROPRTITULO__");
	/// <summary>
	/// Nome do cliente
	/// </summary>
	public BaseInputControl ContcCltname => new BaseInputControl(driver, formLocator, "#CONTACTOCONTCCLTNAME_");
	/// <summary>
	/// Email do cliente
	/// </summary>
	public BaseInputControl ContcCltemail => new BaseInputControl(driver, formLocator, "#CONTACTOCONTCCLTEMAIL");
	/// <summary>
	/// Telefone
	/// </summary>
	public BaseInputControl ContcTelefone => new BaseInputControl(driver, formLocator, "#CONTACTOCONTCTELEFONE");
	/// <summary>
	/// Descrição
	/// </summary>
	public BaseInputControl ContcDescriic => new BaseInputControl(driver, formLocator, "#CONTACTOCONTCDESCRIIC");

	private IWebElement saveBtn => form.FindElement(By.CssSelector("#bottom-save-btn"));
	private IWebElement cancelBtn => form.FindElement(By.CssSelector("#bottom-cancel-btn"));
	public FORM_MODE mode {get; private set;}

	public ContactoForm(IWebDriver driver, FORM_MODE mode, By subformLocator=null): base(driver)
	{
		this.mode = mode;
		formLocator = subformLocator ?? formLocator;

		wait.Until(c => form);
		WaitForLoading();
	}

	public void WaitForLoading()
	{
		wait.Until(c => form.FindElement(ByData.Key("CONTACTO")).GetAttribute("data-loading") != "true");
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
