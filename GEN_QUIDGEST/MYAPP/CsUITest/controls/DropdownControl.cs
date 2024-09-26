using System.Collections.Generic;
using System.Linq;

namespace quidgest.uitests.controls;

public abstract class DropdownControl : ControlObject
{
    protected IWebElement _display => m_control.FindElement(By.CssSelector("[role=combobox]"));    
    protected IWebElement _debounceElement => m_control.FindElement(ByData.Testid("debounce-container"));
    //dropdown is opened in a completely different global html location
    protected IWebElement _dropdown => driver.FindElement(ByData.Testid("combobox-dropdown"));
    protected IEnumerable<IWebElement> _rows => _dropdown.FindElements(By.CssSelector("[role=listbox] li"));

    public DropdownControl(IWebDriver driver, By containerLocator, string controlId) 
        : base(driver, containerLocator, By.Id(controlId))
    {
        WaitForLoad();
    }

    protected void WaitForLoad()
    {
        wait.Until(c => m_control.GetAttribute("data-loading") == "false");
    }

    protected void WaitForDebounce()
    {
        wait.Until(c => _debounceElement.GetAttribute("data-debouncing") == "false");
    }

    public abstract void SetValue(string val);

    public string GetValue()
    {
        //TODO: how to I obtain the pk value from the component. I only have the text.
        WaitForLoad();
        return _display.GetAttribute("value");
    }
        
    public string GetText()
    {
        WaitForLoad();
        return _display.GetAttribute("value");
    }

    public int GetRowByPk(string pk)
    {
        //TODO: needs a data-key attribute
        WaitForLoad();
        return _rows.FindIndex(o => o.GetAttribute("data-key") == pk);
    }

    public int GetRowByText(string text)
    {
        //The text inside the element has an extra space and will not match
        WaitForLoad();
        return _rows.FindIndex(o => o.GetAttribute("aria-label") == text);
    }

}

