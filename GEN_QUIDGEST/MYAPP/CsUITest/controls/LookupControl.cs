using System.Linq;

namespace quidgest.uitests.controls;

public class LookupControl : DropdownControl
{
    protected IWebElement _seeMore => m_control.FindElement(By.CssSelector("[title=\"See more\"]"));

    protected IWebElement _clear => m_control.FindElement(By.CssSelector(".q-combobox__clear"));

    protected IWebElement _input => m_control.FindElement(By.CssSelector("[role=combobox]"));

    public LookupControl(IWebDriver driver, By containerLocator, string controlId) 
        : base(driver, containerLocator, controlId)
    {
    }

    public void Clear()
    {
       WaitForLoad();
        _clear.Click();        
    }

    public void TypeText(string text)
    {
        WaitForLoad();
        _input.SendKeys(text);
    }

    public override void SetValue(string val)
    {        
        WaitForLoad();

        if (!string.IsNullOrEmpty(GetValue()))
            Clear();

        TypeText(val);    
        WaitForDebounce();    

        int ix = GetRowByText(val);
        if (ix != -1)
            _rows.ElementAt(ix).Click();
    }

    public void SeeMore()
    {
        //This should be testid = SeeMore, or testid = actions + testkey = SeeMore
        _seeMore.Click();
    }

}