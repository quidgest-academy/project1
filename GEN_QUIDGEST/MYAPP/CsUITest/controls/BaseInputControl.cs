namespace quidgest.uitests.controls;

public class BaseInputControl : ControlObject
{
    public BaseInputControl(IWebDriver driver, By containerLocator, string css) 
        : base(driver, containerLocator, By.CssSelector(css))
    {
    }

    /// <summary>
    /// Get the input's value
    /// </summary>
    public string GetValue()
    {
        return m_control.GetAttribute("value");
    }

    /// <summary>
    /// Set the input's value
    /// </summary>
    public void SetValue(string val)
    {
        ClearValue();
        m_control.SendKeys(val);
    }

    /// <summary>
    /// Clear the input's value. The built-in Clear() method does not always work but this does
    /// </summary>
    public void ClearValue()
    {
        m_control.SendKeys(Keys.Control + "a");
        m_control.SendKeys(Keys.Delete);
    }

    /// <summary>
    /// Confirm the input's value
    /// </summary>
    public void Confirm()
	{
		m_control.SendKeys(Keys.Enter);
	}
}