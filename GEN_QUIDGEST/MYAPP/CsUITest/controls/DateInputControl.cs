using System.Globalization;

namespace quidgest.uitests.controls;

public class DateInputControl : ControlObject
{
    private IWebElement input => m_control.FindElement(By.CssSelector("input.dp__input"));

    private readonly string format;

    public DateInputControl(IWebDriver driver, By containerLocator, string css, string format = "dd/MM/yyyy") 
        : base(driver, containerLocator, By.CssSelector(css))
    {
        this.format = format;
    }

    public DateTime? GetValue()
    {
        var v = input.GetAttribute("value");
        if (string.IsNullOrEmpty(v))
            return null;
        return DateTime.ParseExact(v, format, CultureInfo.InvariantCulture);
    }

    public void SetValue(DateTime? val)
    {
        input.Clear();
        if (val.HasValue)
        {
            var v = val.Value.ToString(format, CultureInfo.InvariantCulture);
            input.SendKeys(v);
            input.SendKeys(Keys.Return);
        }
    }
}