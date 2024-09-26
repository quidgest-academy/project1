using Microsoft.AspNetCore.Mvc;
using System.Collections.Specialized;

using CSGenio.business;
using CSGenio.framework;
using GenioMVC.Models.Navigation;
using GenioMVC.ViewModels.UserAdmin;
using Quidgest.Persistence.GenericQuery;

namespace GenioMVC.Controllers
{
	public class UserAdminController : ControllerBase
	{
		private static readonly NavigationLocation ACTION_ADMIN = new NavigationLocation("UTILIZADORES39761", "Index", "UserAdmin");

		public UserAdminController(UserContextService userContext) : base(userContext) { }

		//
		// UserAdmin/
		public ActionResult Index()
		{
			UserAdminViewModel model = new UserAdminViewModel(UserContext.Current);
			CSGenio.framework.StatusMessage result = model.CheckPermissions(FormMode.List);

			if (result.Status.Equals(CSGenio.framework.Status.E))
				return PermissionError(result.Message);

			NameValueCollection querystring = new NameValueCollection();
			if (Request.Form.Count > 0)
				querystring.AddRange(Request.Form);
			else
				querystring.AddRange(Request.Query);

			//verificar se o user clicou to exportar os dados da Qlisting
			if (querystring["ExportList"] != null && Convert.ToBoolean(querystring["ExportList"]) && querystring["ExportType"] != null)
			{
				string file = "Utilizadores_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + "." + querystring["ExportType"];
				ListingMVC<CSGenioApsw> listing = null;
				CriteriaSet conditions = null;
				List<CSGenio.framework.Exports.QColumn> columns = null;
				model.LoadToExport(out listing, out conditions, out columns, querystring, Request.IsAjaxRequest());
				byte[] fileBytes = new CSGenio.framework.Exports(UserContext.Current.User).ExportList(listing, conditions, columns, querystring["ExportType"], file);
				QCache.Instance.ExportFiles.Put(file, fileBytes);
				return Json(GetJsonForDownloadExportFile(file, querystring["ExportType"]));
			}

			model.Load(CSGenio.framework.Configuration.NrRegDBedit, querystring, Request.IsAjaxRequest());

			return JsonOK(model);
		}
	}
}
