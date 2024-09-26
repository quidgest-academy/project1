using System.Collections.Specialized;

using GenioMVC.Models.Navigation;

namespace GenioMVC.ViewModels
{
	public abstract class EmptyFormViewModel : ViewModelBase
	{
		public EmptyFormViewModel(UserContext userContext, bool nestedForm = false) : base(userContext)
		{
			InitLevels();
			NestedForm = nestedForm;
		}

		public void Load(NameValueCollection qs)
		{
			LoadPartial(qs);
		}

		protected abstract void InitLevels();

		public abstract void LoadPartial(NameValueCollection qs, bool lazyLoad = false);
	}
}
