namespace quidgest.uitests.controls;

public class CollapsibleZoneControl : ControlObject
{
    private IWebElement toggle => m_control.FindElement(By.CssSelector(".q-group-collapsible__header button"));

    public CollapsibleZoneControl(IWebDriver driver, By containerLocator, string css) 
        : base(driver, containerLocator, By.CssSelector(css))
    {
    }

    public bool IsExpanded => m_control.GetAttribute("class").Contains("q-group-collapsible--open");

    public void Toggle()
    {
        toggle.Click();
    }
}