using System.Collections.Specialized;

using CSGenio.framework;
using GenioMVC.Models;
using GenioMVC.Models.Navigation;

namespace GenioMVC.ViewModels
{
	public abstract class CustomTableFormViewModel<T> : FormViewModel<T> where T : ModelBase, new()
	{
		protected CustomTableFormViewModel(UserContext userContext, string formId) : base(userContext, formId) { }

		// Loads all the information needed to present the form in insert mode
		public override void NewLoad()
		{
			this.LoadPartial(new NameValueCollection());
			LoadDefaultValues();
		}
	}
}
