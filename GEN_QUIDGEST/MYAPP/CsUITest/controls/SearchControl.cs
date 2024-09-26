using System.Collections.Generic;

namespace quidgest.uitests.controls;

public class SearchControl : ControlObject
{
    private IWebElement input => m_control.FindElement(By.CssSelector("input[role='searchbox']"));
    private IWebElement clearBtn => m_control.FindElement(By.CssSelector(".q-table-search__field button"));
    private IList<IWebElement> usersSearchFlds => m_control.FindElements(By.CssSelector("#Users-srch-flds .dropdown-item"));

    public SearchControl(IWebDriver driver, By containerLocator, By controlLocator)
		: base(driver, containerLocator, controlLocator)
    {
    }

    public void Search(string text, bool allFields = false)
    {
        input.SendKeys(text);

        if (!allFields)
            input.SendKeys(Keys.Return);
        else
        {
            if (usersSearchFlds.Count > 0)
            {
                int size = usersSearchFlds.Count;
                usersSearchFlds[size-1].Click();
            }
        }
    }

    public void Search(string text, int fieldIndex)
    {
        input.SendKeys(text);

        if (fieldIndex >= usersSearchFlds.Count)
            throw new ArgumentException($"Invalid field index: {usersSearchFlds}");

        usersSearchFlds[fieldIndex].Click();
    }

    public void Search(string text, string fieldName)
    {
        input.SendKeys(text);
        int index = usersSearchFlds.FindIndex(o => o.GetAttribute("data-search-field") == fieldName);

        if (index >= 0)
            usersSearchFlds[index].Click();
        else
            throw new ArgumentException($"Invalid field: {fieldName}");
    }

    public void Clear()
    {
        clearBtn.Click();
    }

}
