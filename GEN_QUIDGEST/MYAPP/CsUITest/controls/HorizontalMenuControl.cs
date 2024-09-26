using OpenQA.Selenium.Support.Extensions;

namespace quidgest.uitests.controls;

public class HorizontalMenuControl: PageObject, IMenuControl {

    private IWebElement navbar => driver.FindElement(By.Id("main-header-navbar"));

    private IWebElement modules => navbar.FindElement(By.ClassName("modules__container"));
    private IWebElement currentModule => modules.FindElement(By.CssSelector(".modules__header"));

    private IWebElement bookmarks => navbar.FindElement(By.ClassName("bookmarks__container"));

    protected IWebElement menus => navbar.FindElement(By.Id("menu-navbar"));

    protected MenuTree _menuTree;

    public HorizontalMenuControl(IWebDriver driver, MenuTree menuTree): base(driver) 
    {
        _menuTree = menuTree;

        wait.Until(c => navbar);
        wait.Until(c => menus);
    }

    protected virtual void WaitForLoading()
    {
        wait.Until(c => menus);
        wait.Until(c => modules);
    }

    public void ActivateMenu(string moduleId, string itemId)
    {
        WaitForLoading();

        //Horizontal menus do not visualize the subelements until the parent element is clicked
        //To solve this we need to generate the declaration of the menu tree to a class
        // then request the forward path to the destination element to it.
        //Another solution would be to make the test programmer to explicitly activate the
        // menus in the correct sequence, but that would be needlessly verbose for tests.

        var menuNode = _menuTree.FindMenu(moduleId, itemId);
        //recursively click on the parent menus until we can click on the one we want
        ClickParentRecursive(moduleId, menuNode);
    }

    protected virtual void ClickParentRecursive(string moduleId, MenuTreeNode node)
    {
        var parent = node.Parent;
        if (parent != null)
            ClickParentRecursive(moduleId, parent);

        // Get the clickable element within the list item element that has the menu ID
        var liTarget = menus.FindElement(By.CssSelector("#" + moduleId + node.Id + " a"));
        liTarget.Click();
    }


    public void ActivateModule(string moduleId)
    {
        WaitForLoading();

        var cm = currentModule.GetAttribute("data-key");
        if (cm == moduleId) return;
        modules.Click();
        var item = modules.FindElement(ByData.Key(moduleId));
        wait.Until(c => item.Displayed);
        item.Click();
    }

    public void ActivateFavorite(string itemId)
    {
        bookmarks.Click();
        var item = bookmarks.FindElement(ByData.Key(itemId));
        wait.Until(c => item.Displayed);
        item.Click();
    }
}