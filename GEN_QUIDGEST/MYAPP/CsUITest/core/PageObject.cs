using OpenQA.Selenium.Support.UI;

namespace quidgest.uitests.core;

/// <summary>
/// Base class for every Page Object Model (POM).
/// </summary>
/// <remarks>
/// https://www.selenium.dev/documentation/en/guidelines_and_recommendations/page_object_models/
/// </remarks>
public class PageObject {
	//private final static Logger LOGGER = LoggerFactory.getLogger(PageObject.class.getName());

	protected IWebDriver driver;
	protected WebDriverWait wait;

	/// <summary>
	/// Initialize a Page Object Model (POM)
	/// </summary>
	/// <param name="driver">WebDriver</param>
	public PageObject(IWebDriver driver) {
		this.driver = driver;
		this.wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(Configuration.Instance.ExplicitWait.Value));
		this.wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException), typeof(NoSuchElementException));
	}
	

}
