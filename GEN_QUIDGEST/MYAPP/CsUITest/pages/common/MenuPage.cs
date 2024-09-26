using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quidgest.uitests.pages.common
{
    public class MenuPage:PageObject
    {
        protected string id;  

        protected IWebElement page => driver.FindElement(By.ClassName(this.id));

        public ListControl List => new ListControl(driver, By.Id("form-container"), "form .q-table-list");

        public CardsControl Cards => new CardsControl(driver, By.Id("form-container"), "form .q-table-list");

        public MenuPage(IWebDriver driver) : base(driver) {}
    }
}
