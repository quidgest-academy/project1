namespace quidgest.uitests.pages;

public class LoginPage: PageObject {

	private IWebElement loginForm => driver.FindElement(By.Id("login-container"));
	private IWebElement username => loginForm.FindElement(By.Name("username"));
	private IWebElement password => loginForm.FindElement(By.Name("password"));
	private IWebElement submitButton => loginForm.FindElement(By.Id("login-btn"));

	public LoginPage(IWebDriver driver) : base(driver) {
		wait.Until(c => loginForm != null);
	}

	private void WaitForLoad()
	{
		wait.Until(c => submitButton.GetAttribute("data-loading") == "false");
	}
	public void Login(string username, string password) {

		this.username.Clear();
		this.username.SendKeys(username);

		this.password.Clear();
		this.password.SendKeys(password);

		this.submitButton.Click();
	}

	public void Register() {
        var btn = loginForm.FindElement(By.Id("link-register"));
		btn.Click();
    }

	public void ForgotPassword()
	{
		var btn = loginForm.FindElement(By.Id("forgot-password"));
		btn.Click();
    }

	public bool HasErrorMessage(string id)
	{
		WaitForLoad();
		IWebElement errorMessage = loginForm.FindElement(By.Id(id));

		return errorMessage.Text.Length > 0;
	}
}
