namespace quidgest.uitests.pages;

public class TableConfigurationPage : PageObject {
    /// <summary>
    /// Table configuration container element
    /// </summary>
    private IWebElement tableConfigurationContainer => driver.FindElement(By.Id("q-modal-config"));

    /// <summary>
    /// Table configuration tab elements
    /// </summary>
    private IWebElement columnConfigurationTab => driver.FindElement(By.Id("tab-container-column-config"));
    private IWebElement advancedFiltersTab => driver.FindElement(By.Id("tab-container-advanced-filters"));
    private IWebElement saveViewTab => driver.FindElement(By.Id("tab-container-view-save"));
    private IWebElement viewManagerTab => driver.FindElement(By.Id("tab-container-views"));

    /// <summary>
    /// Table configuration tab content elements that contain the controls
    /// </summary>
    private IWebElement columnConfigurationForm => driver.FindElement(By.Id("q-modal-column-config-body"));
    private IWebElement advancedFiltersForm => driver.FindElement(By.Id("q-modal-advanced-filters-body"));
    private IWebElement saveViewForm => driver.FindElement(By.Id("q-modal-view-save-body"));
    private IWebElement viewManagerForm => driver.FindElement(By.Id("q-modal-views-body"));

    /// <summary>
    /// Column configuration buttons
    /// </summary>
    private IWebElement resetColumnConfigBtn => driver.FindElement(By.Id("reset-column-config-btn"));
    private IWebElement applyColumnConfigBtn => driver.FindElement(By.Id("apply-column-config-btn"));
    private IWebElement cancelColumnConfigBtn => driver.FindElement(By.Id("cancel-column-config-btn"));

    /// <summary>
    /// Column configuration table
    /// </summary>
    public ListControl columnConfigList => new ListControl(driver, By.Id("q-modal-column-config-body"), ".q-table-list");

    public TableConfigurationPage(IWebDriver driver) : base(driver)
    {
        wait.Until(c => tableConfigurationContainer != null);
    }

    /// <summary>
    /// Reset the column configuration
    /// </summary>
    public void ResetColumnConfig()
    {
        if (resetColumnConfigBtn == null)
            return;

        resetColumnConfigBtn.Click();
    }

    /// <summary>
    /// Apply the column configuration
    /// </summary>
    public void ApplyColumnConfig()
    {
        if (applyColumnConfigBtn == null)
            return;

        applyColumnConfigBtn.Click();
    }

    /// <summary>
    /// Cancel changes to the column configuration
    /// </summary>
    public void CancelColumnConfig()
    {
        if (cancelColumnConfigBtn == null)
            return;

        cancelColumnConfigBtn.Click();
    }
}