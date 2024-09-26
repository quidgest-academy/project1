namespace quidgest.uitests.pages;

[System.CodeDom.Compiler.GeneratedCode("Genio", "")]
public class FotosForm: PageObject
{
	private By formLocator = By.CssSelector("#form-container");
	private IWebElement form => driver.FindElement(formLocator);

	/// <summary>
	/// Foto
	/// </summary>
	public BaseInputControl AlbumFoto => new BaseInputControl(driver, formLocator, "#FOTOS___ALBUMFOTO____");
	/// <summary>
	/// Título
	/// </summary>
	public BaseInputControl AlbumTitulo => new BaseInputControl(driver, formLocator, "#FOTOS___ALBUMTITULO__");
	/// <summary>
	/// Propriedade
	/// </summary>
	public LookupControl ProprTitulo => new LookupControl(driver, formLocator, "container-FOTOS___PROPRTITULO__");
	public SeeMorePage ProprTituloSeeMorePage => new SeeMorePage(driver, "FOTOS", "FOTOS___PROPRTITULO__");

	private IWebElement saveBtn => form.FindElement(By.CssSelector("#bottom-save-btn"));
	private IWebElement cancelBtn => form.FindElement(By.CssSelector("#bottom-cancel-btn"));
	public FORM_MODE mode {get; private set;}

	public FotosForm(IWebDriver driver, FORM_MODE mode, By subformLocator=null): base(driver)
	{
		this.mode = mode;
		formLocator = subformLocator ?? formLocator;

		wait.Until(c => form);
		WaitForLoading();
	}

	public void WaitForLoading()
	{
		wait.Until(c => form.FindElement(ByData.Key("FOTOS")).GetAttribute("data-loading") != "true");
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
