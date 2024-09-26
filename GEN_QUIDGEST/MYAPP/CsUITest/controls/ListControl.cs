using System.Collections.Generic;
using System.Linq;

namespace quidgest.uitests.controls;

public class ListControl : ControlObject
{
    /// <summary>
    /// Table ID
    /// </summary>
    private string id => m_control.GetAttribute("id");

    /// <summary>
    /// Row elements
    /// </summary>
    private IList<IWebElement> rows => m_control.FindElements(By.CssSelector("tbody tr"));

    /// <summary>
    /// Column elements
    /// </summary>
	private IList<IWebElement> columns => m_control.FindElements(By.CssSelector("thead th"));

    /// <summary>
    /// Ordering column name
    /// </summary>
    private string orderingColumnName => m_control.FindElement(By.CssSelector(".thead-order"))?.GetAttribute("data-column-name");

    //TODO - Instead of using title to identify the insert button we should create a specific attribute (Ex: data-key) for testing purpose
    /// <summary>
    /// Insert button
    /// </summary>
    private IWebElement insertBtn => m_control.FindElement(By.CssSelector("[data-testid=table-action][data-action-key='insert']"));

    /// <summary>
    /// Loading state
    /// </summary>
    private bool loading => m_control.FindElements(By.CssSelector("tbody.c-table__body--loading")).Any();

    /// <summary>
    /// Search bar
    /// </summary>
    public SearchControl Search => new SearchControl(driver, m_containerLocator, m_controlLocator);

    /// <summary>
    /// Table configuration menu button and items
    /// </summary>
    private IWebElement configBtn => m_control.FindElement(By.CssSelector("button[id$='config-menu-btn']"));
	private IWebElement columnConfigBtn => m_control.FindElement(By.CssSelector("button[id$='config-menu-btn-column-config']"));

    /// <summary>
    /// Gets a value indicating whether the user can insert in this table.
    /// </summary>
    public bool CanInsert => insertBtn.Enabled;

    public ListControl(IWebDriver driver, By containerLocator, string css) :
        base(driver, containerLocator, By.CssSelector(css))
    {
        WaitForLoading();
    }

    /// <summary>
    /// Wait for page to load
    /// </summary>
    private void WaitForLoading()
    {
        wait.Until(c => !loading);
    }

    /// <summary>
    /// Get a row element from it's primary key
    /// </summary>
    /// <param name="pk">Row's primary key</param>
    /// <returns>Row element</returns>
    public int GetRowByPk(string pk)
    {
        WaitForLoading();
        return rows.FindIndex(r => r.GetAttribute("data-key") == pk);
    }

    /// <summary>
    /// Get a column's index from it's name
    /// </summary>
    /// <param name="fieldRef">Column's name</param>
    /// <returns>Column index</returns>
    private int GetRawColumnIndex(string fieldRef)
	{
		WaitForLoading();

		string column_locator;

        var parts = fieldRef.Split('.', 2);

		// If the field is given as an exact name, but not as TABLE.COLUMN
		if(parts.Length == 1)
			column_locator = fieldRef;
		// If the field is given as TABLE.COLUMN
		else
            column_locator = CapFirst(parts[0]) + ".Val" + CapFirst(parts[1]);

        return columns.FindIndex(h => h.GetAttribute("data-column-name") == column_locator);
	}

    /// <summary>
    /// Get a column's name from it's index
    /// </summary>
    /// <param name="index">Column's index</param>
    /// <returns>Column name</returns>
    private string GetRawColumnNameByIndex(int index)
    {
        WaitForLoading();

        // Bounds checking
        if(index < 0 || index >= columns.Count)
            return null;

        return columns[index].GetAttribute("data-column-name");
    }

    /// <summary>
    /// Get the number of non-data columns at the beginning of the column array
    /// </summary>
    /// <returns>Number of non-data columns</returns>
    private int NonDataColumnAtBeginningCount()
    {
        WaitForLoading();

        // Find the first column that is not one of the special types of columns
        int firstNonDataColumnIndex = columns.FindIndex(col => {
            string columnName = col.GetAttribute("data-column-name");
            return !(columnName.Equals("actions") || columnName.Equals("Checkbox") || columnName.Equals("ExtendedAction") || columnName.Equals("dragAndDrop"));
        });

        // If all columns are special columns
        if (firstNonDataColumnIndex == -1)
            return columns.Count;

        // Index of first data column is the number of non-data columns
        return firstNonDataColumnIndex;
    }

    /// <summary>
    /// Get a column's index from it's name, relative to the starting index of data columns
    /// </summary>
    /// <param name="fieldRef">Column's name</param>
    /// <returns>Column index</returns>
    public int GetColumnIndex(string fieldRef)
    {
        WaitForLoading();

        // Get actual column index
        int columnIndex = GetRawColumnIndex(fieldRef);

        // Column not found
        if(columnIndex == -1)
            return -1;

        // Return index starting from where the data columns start
        return columnIndex - NonDataColumnAtBeginningCount();
    }

    /// <summary>
    /// Get a column's name from it's index, relative to the starting index of data columns
    /// </summary>
    /// <param name="index">Column's index</param>
    /// <returns>Column name</returns>
    public string GetColumnNameByIndex(int index)
    {
        WaitForLoading();

        // Add number of non data columns to index to use the actual index when finding the column
        return GetRawColumnNameByIndex(index + NonDataColumnAtBeginningCount());
    }

    /// <summary>
    /// Capitalize the first letter of a string
    /// </summary>
    /// <param name="s">string</param>
    /// <returns>string with the first letter capitalized</returns>
    private string CapFirst(string s)
    {
        if (s.Length == 0) return s;
        if (s.Length == 1) return s.ToUpperInvariant();
        return s.Substring(0, 1).ToUpperInvariant() + s.Substring(1).ToLowerInvariant();
    }

    /// <summary>
    /// Get a cell's value from it's row index and column name
    /// </summary>
    /// <param name="row">Row index</param>
    /// <param name="fieldRef">Column name</param>
    /// <returns>Cell value</returns>
    public string GetValue(int row, string fieldRef)
    {
        WaitForLoading();
        int cix = GetRawColumnIndex(fieldRef);
        // Console.WriteLine("row:" + row);
        // Console.WriteLine("col:" + cix);
        var cell = rows[row].FindElements(By.TagName("td"))[cix];
        return cell.Text;
    }

    /// <summary>
    /// Click on a row
    /// </summary>
    public void ClickRow(int index)
    {
        WaitForLoading();
        if (index >= rows.Count)
            throw new ArgumentException($"Invalid row index: {index}");

        rows[index].Click();
    }

    /// <summary>
    /// Click on the insert button
    /// </summary>
    public void Insert()
    {
        insertBtn.Click();
    }

    /// <summary>
    /// Run a row's action from it's row index and action name
    /// </summary>
    /// <param name="index">Row index</param>
    /// <param name="action">Action name</param>
    /// <param name="orderIndex">Index to move row to (only used with row reordering)</param>
    public void ExecuteAction(int index, String action, int orderIndex = 0)
    {
        WaitForLoading();
        if (index >= rows.Count || index < 0)
            throw new ArgumentException($"Invalid row index: {index}");

        var row = rows[index];

        // For row reorder functions, call specific functions
        if (action.Equals(ReorderAction.Reorder))
            ReorderRow(index, orderIndex);
        else if (action.Equals(ReorderAction.ReorderUp))
            ReorderRowUpOrDown(index, false);
        else if (action.Equals(ReorderAction.ReorderDown))
            ReorderRowUpOrDown(index, true);
        // Normal actions
        else
        {
            var cell = row.FindElement(By.CssSelector("td.row-actions"));
            var button = cell.FindElement(By.CssSelector("[data-testid=options-btn]"));
            button.Click();

            //TODO: instead of title it should be data-key=action
            var link = cell.FindElement(By.CssSelector("[data-testid=table-action][data-action-key='" + action + "']"));

            link.Click();
        }
    }

    /// <summary>
    /// Move the row at the given index to a new index
    /// </summary>
    /// <param name="currentIndex">Index of the row</param>
    /// <param name="newIndex">Index to move the row to</param>
    private void ReorderRow(int currentIndex, int newIndex)
    {
        // The element index starts at 0, it's always 1 less than the column order value
        int newOrderValue = newIndex + 1;

        // Get the input for the column order field and change it's value
        BaseInputControl rowOrderInput = new BaseInputControl(driver, By.Id("container-" + id + "_" + currentIndex + "_" + orderingColumnName), "#" + id + "_" + currentIndex + "_" + orderingColumnName);
        rowOrderInput.SetValue(newOrderValue.ToString());

        // Confirm the value
        rowOrderInput.Confirm();
    }

    /// <summary>
    /// Move the row at the given index up or down one
    /// </summary>
    /// <param name="currentIndex">Index of the row</param>
    /// <param name="incrememnt">Whether to move the row up or down one</param>
    private void ReorderRowUpOrDown(int currentIndex, bool incrememnt)
    {
        string direction = incrememnt ? "down" : "up";

        ButtonControl reorderUpDownBtn = new ButtonControl(driver, By.Id(id + "_row-" + currentIndex), "#" + id + "_row-" + currentIndex + "-reorder-" + direction);

        reorderUpDownBtn.Click();
    }

    /// <summary>
    /// Sort a table by a column
    /// </summary>
    /// <param name="index">Column index</param>
    public void SortTable(int index)
    {
        WaitForLoading();
        var header = m_control.FindElement(By.CssSelector("thead tr"));
        var cells = header.FindElements(By.CssSelector("th"));

        cells[index].Click();
    }

    /// <summary>
    /// Open the table's column configuration interface
    /// </summary>
    public void OpenColumnConfig()
	{
		// Click the table configuration button which can open a menu or popup,
		// depending on how many configuration interfaces are available
		configBtn.Click();

		try
		{
			// If the column configuration is the only available configuration,
			// the configuration popup should now be open to it
			TableConfigurationPage tableConfigurationPage = new TableConfigurationPage(driver);
		}
		catch
		{
			// If there are multiple table configuration options,
			// the table configuration menu should be open
			// and the column configuration button should be clicked
			columnConfigBtn.Click();
		}
	}


}
