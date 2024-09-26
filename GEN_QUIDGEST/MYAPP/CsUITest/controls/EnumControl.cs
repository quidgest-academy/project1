using System.Collections.Generic;
using System.Linq;

namespace quidgest.uitests.controls;

public class EnumControl:DropdownControl
{
    protected IWebElement _button => _display.FindElement(By.CssSelector(".q-select__chevron"));

    public EnumControl(IWebDriver driver, By containerLocator, string controlId)
        : base(driver, containerLocator, controlId)
    {
    }

    public override void SetValue(string val)
    {
        WaitForLoad();        
        _button.Click();
        int ix = GetRowByText(val);
        
        if (ix != -1)
            _rows.ElementAt(ix).Click();
    }

}

