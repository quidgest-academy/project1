using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Entity;
using System.Linq;

using CSGenio.business;
using CSGenio.framework;
using CSGenio.persistence;
using CSGenio.reporting;
using CSGenio.core.persistence;
using GenioMVC.Helpers;
using GenioMVC.Models;
using GenioMVC.Models.Exception;
using GenioMVC.Models.Navigation;
using GenioMVC.Resources;
using GenioMVC.ViewModels.Propr;
using Quidgest.Persistence.GenericQuery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// USE /[MANUAL PRO INCLUDE_CONTROLLER PROPR]/

namespace GenioMVC.Controllers
{
	public partial class ProprController : ControllerBase
	{
		private static readonly NavigationLocation ACTION_PRO_MENU_21 = new NavigationLocation("PROPRIEDADES45924", "PRO_Menu_21", "Propr") { vueRouteName = "menu-PRO_21" };
		private static readonly NavigationLocation ACTION_PRO_MENU_411 = new NavigationLocation("PROPRIEDADES45924", "PRO_Menu_411", "Propr") { vueRouteName = "menu-PRO_411" };
		private static readonly NavigationLocation ACTION_PRO_MENU_511 = new NavigationLocation("PROPRIEDADES45924", "PRO_Menu_511", "Propr") { vueRouteName = "menu-PRO_511" };


		//
		// GET: /Propr/PRO_Menu_21
		[ActionName("PRO_Menu_21")]
		[HttpPost]
		public ActionResult PRO_Menu_21([FromBody]RequestMenuModel requestModel)
		{
			var queryParams = requestModel.QueryParams;
			var allSelected = requestModel.AllSelected;

			int perPage = CSGenio.framework.Configuration.NrRegDBedit;
			string rowsPerPageOptionsString = "";

			PRO_Menu_21_ViewModel model = new PRO_Menu_21_ViewModel(UserContext.Current);

			// Determine which table configuration to use and load it
			CSGenio.framework.TableConfiguration.TableConfiguration tableConfig = TableConfigurationIO.DetermineTableConfig(
				UserContext.Current.PersistentSupport,
				UserContext.Current.User,
				model.Uuid,
				requestModel?.TableConfiguration,
				requestModel?.UserTableConfigName,
				(bool)requestModel?.LoadDefaultView
			);

			// Determine rows per page
			tableConfig.RowsPerPage = CSGenio.framework.TableConfiguration.TableConfigurationHelpers.DetermineRowsPerPage(tableConfig.RowsPerPage, perPage, rowsPerPageOptionsString);

			bool isHomePage = RouteData.Values.ContainsKey("isHomePage") ? (bool)RouteData.Values["isHomePage"] : false;
			if (isHomePage)
				Navigation.SetValue("HomePage", "PRO_Menu_21");

			//If there was a recent operation on this table then force the primary persistence server to be called and ignore the read only feature
			if (string.IsNullOrEmpty(Navigation.GetStrValue("ForcePrimaryRead_propr")))
				UserContext.Current.SetPersistenceReadOnly(true);
			else
			{
				Navigation.DestroyEntry("ForcePrimaryRead_propr");
				UserContext.Current.SetPersistenceReadOnly(false);
			}
			CSGenio.framework.StatusMessage result = model.CheckPermissions(FormMode.List);
			if (result.Status.Equals(CSGenio.framework.Status.E))
				return PermissionError(result.Message);

			NameValueCollection querystring = new NameValueCollection();
			if (queryParams != null && queryParams.Count > 0)
				querystring.AddRange(queryParams);

			if (!isHomePage &&
				(Navigation.CurrentLevel == null || !ACTION_PRO_MENU_21.IsSameAction(Navigation.CurrentLevel.Location)) &&
				Navigation.CurrentLevel.Location.Action != ACTION_PRO_MENU_21.Action)
				CSGenio.framework.Audit.registAction(UserContext.Current.User, Resources.Resources.MENU01948 + " " + Navigation.CurrentLevel.Location.ShortDescription());
			else if (isHomePage)
			{
				CSGenio.framework.Audit.registAction(UserContext.Current.User, Resources.Resources.MENU01948 + " " + ACTION_PRO_MENU_21.ShortDescription());
				Navigation.SetValue("HomePageContainsList", true);
			}



// USE /[MANUAL PRO MENU_GET 21]/


			model.Load(tableConfig, querystring, Request.IsAjaxRequest());

			if (model.CheckForZzstate())
				WarningMessage(Resources.Resources.ATENCAO__TEM_FICHAS_40812);


			return JsonOK(model);
		}

		//
		// GET: /Propr/PRO_Menu_411
		[ActionName("PRO_Menu_411")]
		[HttpPost]
		public ActionResult PRO_Menu_411([FromBody]RequestMenuModel requestModel)
		{
			var queryParams = requestModel.QueryParams;
			var allSelected = requestModel.AllSelected;

			int perPage = CSGenio.framework.Configuration.NrRegDBedit;
			string rowsPerPageOptionsString = "";

			PRO_Menu_411_ViewModel model = new PRO_Menu_411_ViewModel(UserContext.Current);

			// Determine which table configuration to use and load it
			CSGenio.framework.TableConfiguration.TableConfiguration tableConfig = TableConfigurationIO.DetermineTableConfig(
				UserContext.Current.PersistentSupport,
				UserContext.Current.User,
				model.Uuid,
				requestModel?.TableConfiguration,
				requestModel?.UserTableConfigName,
				(bool)requestModel?.LoadDefaultView
			);

			// Determine rows per page
			tableConfig.RowsPerPage = CSGenio.framework.TableConfiguration.TableConfigurationHelpers.DetermineRowsPerPage(tableConfig.RowsPerPage, perPage, rowsPerPageOptionsString);

			bool isHomePage = RouteData.Values.ContainsKey("isHomePage") ? (bool)RouteData.Values["isHomePage"] : false;
			if (isHomePage)
				Navigation.SetValue("HomePage", "PRO_Menu_411");

			//If there was a recent operation on this table then force the primary persistence server to be called and ignore the read only feature
			if (string.IsNullOrEmpty(Navigation.GetStrValue("ForcePrimaryRead_propr")))
				UserContext.Current.SetPersistenceReadOnly(true);
			else
			{
				Navigation.DestroyEntry("ForcePrimaryRead_propr");
				UserContext.Current.SetPersistenceReadOnly(false);
			}
			CSGenio.framework.StatusMessage result = model.CheckPermissions(FormMode.List);
			if (result.Status.Equals(CSGenio.framework.Status.E))
				return PermissionError(result.Message);

			NameValueCollection querystring = new NameValueCollection();
			if (queryParams != null && queryParams.Count > 0)
				querystring.AddRange(queryParams);

			if (!isHomePage &&
				(Navigation.CurrentLevel == null || !ACTION_PRO_MENU_411.IsSameAction(Navigation.CurrentLevel.Location)) &&
				Navigation.CurrentLevel.Location.Action != ACTION_PRO_MENU_411.Action)
				CSGenio.framework.Audit.registAction(UserContext.Current.User, Resources.Resources.MENU01948 + " " + Navigation.CurrentLevel.Location.ShortDescription());
			else if (isHomePage)
			{
				CSGenio.framework.Audit.registAction(UserContext.Current.User, Resources.Resources.MENU01948 + " " + ACTION_PRO_MENU_411.ShortDescription());
				Navigation.SetValue("HomePageContainsList", true);
			}


			if (!String.IsNullOrEmpty(querystring["agent"]))
				Navigation.SetValue("agent", querystring["agent"]);


// USE /[MANUAL PRO MENU_GET 411]/

			// Table List Export - check if user is exporting the Qlisting
			if (querystring["ExportList"] != null && Convert.ToBoolean(querystring["ExportList"]) && querystring["ExportType"] != null)
			{
				string exportType = querystring["ExportType"];
				string file = "PRO_Menu_411_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + "." + exportType;
				ListingMVC<CSGenioApropr> listing = null;
				CriteriaSet conditions = null;
				List<CSGenio.framework.Exports.QColumn> columns = null;
				model.LoadToExport(out listing, out conditions, out columns, tableConfig, querystring, Request.IsAjaxRequest());

				// Validate export format
				if (querystring["ExportValidate"] == "true")
				{
					bool isValidExport = new CSGenio.framework.Exports(UserContext.Current.User).ExportListValidation(listing, conditions, columns, exportType);
					return Json(new { ValidFormat = isValidExport });
				}

				byte[] fileBytes = null;
// USE /[MANUAL PRO OVERRQEXPORT 411]/
				fileBytes = new CSGenio.framework.Exports(UserContext.Current.User).ExportList(listing, conditions, columns, exportType, file,ACTION_PRO_MENU_411.Name);

				QCache.Instance.ExportFiles.Put(file, fileBytes);
				return Json(GetJsonForDownloadExportFile(file, querystring["ExportType"]));
			}
			if (querystring["ImportList"] != null && Convert.ToBoolean(querystring["ImportList"]) && querystring["ImportType"] != null)
			{
				string importType =  querystring["ImportType"];
				string file = "PRO_Menu_411_Template" + "." + importType;
				List<CSGenio.framework.Exports.QColumn> columns = null;
				model.LoadToExportTemplate(out columns);
				byte[] fileBytes = null;

				fileBytes = new CSGenio.framework.Exports(UserContext.Current.User).ExportTemplate(columns, importType, file,ACTION_PRO_MENU_411.Name);

				QCache.Instance.ExportFiles.Put(file, fileBytes);
				return Json(GetJsonForDownloadExportFile(file, importType));
			}

			model.Load(tableConfig, querystring, Request.IsAjaxRequest());

			if (model.CheckForZzstate())
				WarningMessage(Resources.Resources.ATENCAO__TEM_FICHAS_40812);


			return JsonOK(model);
		}

		//
		// POST: /Propr/PRO_Menu_411_UploadFile
		[HttpPost]
		public ActionResult PRO_Menu_411_UploadFile(string importType, string qqfile) {
			PRO_Menu_411_ViewModel model = new PRO_Menu_411_ViewModel(UserContext.Current);

			PersistentSupport sp = UserContext.Current.PersistentSupport;
			List<CSGenioApropr> rows = new List<CSGenioApropr>();
			List<String> results = new List<String>();

			try
			{
				var file = Request.Form.Files[0];
				byte[] fileBytes = new byte[file.Length];
				var mem = new MemoryStream(fileBytes);
				file.CopyTo(mem);

				List<CSGenio.framework.Exports.QColumn> columns = null;
				model.LoadToExportTemplate(out columns);

				rows = new CSGenio.framework.Exports( UserContext.Current.User).ImportList<CSGenioApropr>(columns, importType, fileBytes);

				sp.openTransaction();
				int lineNumber = 0;
				foreach (CSGenioApropr importRow in rows)
				{
					try
					{
						lineNumber++;
						importRow.ValidateIfIsNull = true;
						importRow.insertPseud(UserContext.Current.PersistentSupport);
						importRow.change(UserContext.Current.PersistentSupport, (CriteriaSet)null);
					}
					catch (GenioException ex)
					{
						string lineNumberMsg = String.Format(Resources.Resources.ERROR_IN_LINE__0__45377 + " ", lineNumber);
						ex.UserMessage = lineNumberMsg + ex.UserMessage;
						throw;
					}
				}
				sp.closeTransaction();

				results.Add(string.Format(Resources.Resources._0__LINHAS_IMPORTADA15937, rows.Count));

				return Json(new { success = true, lines = results, msg = Resources.Resources.FICHEIRO_IMPORTADO_C51013 });
			}
			catch (GenioException e)
			{
				sp.rollbackTransaction();
				sp.closeConnection();
				CSGenio.framework.Log.Error(e.Message);
				results.Add(e.UserMessage);

				return Json(new { success = false, errors = results, msg = Resources.Resources.ERROR_IMPORTING_FILE09339 });
			}
		}

		//
		// GET: /Propr/PRO_Menu_511
		[ActionName("PRO_Menu_511")]
		[HttpPost]
		public ActionResult PRO_Menu_511([FromBody]RequestMenuModel requestModel)
		{
			var queryParams = requestModel.QueryParams;
			var allSelected = requestModel.AllSelected;

			int perPage = CSGenio.framework.Configuration.NrRegDBedit;
			string rowsPerPageOptionsString = "";

			PRO_Menu_511_ViewModel model = new PRO_Menu_511_ViewModel(UserContext.Current);

			// Determine which table configuration to use and load it
			CSGenio.framework.TableConfiguration.TableConfiguration tableConfig = TableConfigurationIO.DetermineTableConfig(
				UserContext.Current.PersistentSupport,
				UserContext.Current.User,
				model.Uuid,
				requestModel?.TableConfiguration,
				requestModel?.UserTableConfigName,
				(bool)requestModel?.LoadDefaultView
			);

			// Determine rows per page
			tableConfig.RowsPerPage = CSGenio.framework.TableConfiguration.TableConfigurationHelpers.DetermineRowsPerPage(tableConfig.RowsPerPage, perPage, rowsPerPageOptionsString);

			bool isHomePage = RouteData.Values.ContainsKey("isHomePage") ? (bool)RouteData.Values["isHomePage"] : false;
			if (isHomePage)
				Navigation.SetValue("HomePage", "PRO_Menu_511");

			//If there was a recent operation on this table then force the primary persistence server to be called and ignore the read only feature
			if (string.IsNullOrEmpty(Navigation.GetStrValue("ForcePrimaryRead_propr")))
				UserContext.Current.SetPersistenceReadOnly(true);
			else
			{
				Navigation.DestroyEntry("ForcePrimaryRead_propr");
				UserContext.Current.SetPersistenceReadOnly(false);
			}
			CSGenio.framework.StatusMessage result = model.CheckPermissions(FormMode.List);
			if (result.Status.Equals(CSGenio.framework.Status.E))
				return PermissionError(result.Message);

			NameValueCollection querystring = new NameValueCollection();
			if (queryParams != null && queryParams.Count > 0)
				querystring.AddRange(queryParams);

			if (!isHomePage &&
				(Navigation.CurrentLevel == null || !ACTION_PRO_MENU_511.IsSameAction(Navigation.CurrentLevel.Location)) &&
				Navigation.CurrentLevel.Location.Action != ACTION_PRO_MENU_511.Action)
				CSGenio.framework.Audit.registAction(UserContext.Current.User, Resources.Resources.MENU01948 + " " + Navigation.CurrentLevel.Location.ShortDescription());
			else if (isHomePage)
			{
				CSGenio.framework.Audit.registAction(UserContext.Current.User, Resources.Resources.MENU01948 + " " + ACTION_PRO_MENU_511.ShortDescription());
				Navigation.SetValue("HomePageContainsList", true);
			}


			Navigation.SetValue("propr.vendida", "1");

// USE /[MANUAL PRO MENU_GET 511]/


			model.Load(tableConfig, querystring, Request.IsAjaxRequest());

			if (model.CheckForZzstate())
				WarningMessage(Resources.Resources.ATENCAO__TEM_FICHAS_40812);


			return JsonOK(model);
		}



	}
}
