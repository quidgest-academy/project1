namespace quidgest.uitests.pages;

public class UserHomePage: PageObject {
	
	IWebElement userAvatar => driver.FindElement(By.CssSelector("button.UserAvatar"));

	public UserHomePage(IWebDriver driver) : base(driver) {
		wait.Until(c => userAvatar != null);
	}

}
