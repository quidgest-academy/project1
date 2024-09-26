namespace quidgest.uitests.controls;

public class CheckboxInputControl : ControlObject
{
    private IWebElement checkbox => m_control.FindElement(By.CssSelector("[data-testid=checkbox-container]"));

    public CheckboxInputControl(IWebDriver driver, By containerLocator, string css)
        : base(driver, containerLocator, By.CssSelector(css))
    {
    }

    public bool GetValue()
    {
        return checkbox.FindElement(By.CssSelector("input")).Selected;
    }

    public void Toggle()
    {
        checkbox.Click();
    }

    public void SetValue(bool val)
    {
        if (GetValue() != val)
            Toggle();
    }
}
