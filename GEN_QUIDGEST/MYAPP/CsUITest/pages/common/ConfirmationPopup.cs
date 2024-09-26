namespace quidgest.uitests.pages;

public class ConfirmationPopup: PageObject
{
    // TODO: When the "SweetAlert" dependency is removed, this should be improved so it doesn't use class names.
    IWebElement dialog => driver.FindElement(By.CssSelector(".swal2-popup[role='dialog']"));
    IWebElement buttonOk => dialog.FindElement(By.CssSelector("button.swal2-confirm"));
    IWebElement buttonCancel => dialog.FindElement(By.CssSelector("button.swal2-cancel"));
    IWebElement buttonDeny => dialog.FindElement(By.CssSelector("button.swal2-deny"));
    IWebElement dialogText => dialog.FindElement(By.Id("swal2-html-container"));

    public ConfirmationPopup(IWebDriver driver): base(driver)
    {
		wait.Until(c => dialog);
        wait.Until(c => dialog.Displayed);
	}

    public void Confirm()
    {
        buttonOk.AnimatedClick();
    }

    public void Cancel()
    {
        buttonCancel.AnimatedClick();
    }

    public void Deny()
    {
        buttonDeny.AnimatedClick();
    }

    public string GetDialogText()
    {
        return dialogText.Text;
    }
}
