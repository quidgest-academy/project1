using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Reflection;

using CSGenio.business;
using CSGenio.core.persistence;
using CSGenio.framework;
using CSGenio.persistence;
using CSGenio.reporting;
using GenioMVC.Helpers;
using GenioMVC.Models;
using GenioMVC.Models.Exception;
using GenioMVC.Models.Navigation;
using GenioMVC.Resources;
using GenioMVC.ViewModels;
using GenioMVC.ViewModels.Propr;
using Quidgest.Persistence.GenericQuery;

// USE /[MANUAL PRO INCLUDE_CONTROLLER PROPR]/

namespace GenioMVC.Controllers
{
	public partial class ProprController : ControllerBase
	{
		#region NavigationLocation Names

		private static readonly NavigationLocation ACTION_PROPRIED_CANCEL = new("PROPRIEDADE00464", "Propried_Cancel", "Propr") { vueRouteName = "form-PROPRIED", mode = "CANCEL" };
		private static readonly NavigationLocation ACTION_PROPRIED_SHOW = new("PROPRIEDADE00464", "Propried_Show", "Propr") { vueRouteName = "form-PROPRIED", mode = "SHOW" };
		private static readonly NavigationLocation ACTION_PROPRIED_NEW = new("PROPRIEDADE00464", "Propried_New", "Propr") { vueRouteName = "form-PROPRIED", mode = "NEW" };
		private static readonly NavigationLocation ACTION_PROPRIED_EDIT = new("PROPRIEDADE00464", "Propried_Edit", "Propr") { vueRouteName = "form-PROPRIED", mode = "EDIT" };
		private static readonly NavigationLocation ACTION_PROPRIED_DUPLICATE = new("PROPRIEDADE00464", "Propried_Duplicate", "Propr") { vueRouteName = "form-PROPRIED", mode = "DUPLICATE" };
		private static readonly NavigationLocation ACTION_PROPRIED_DELETE = new("PROPRIEDADE00464", "Propried_Delete", "Propr") { vueRouteName = "form-PROPRIED", mode = "DELETE" };

		#endregion

		#region Propried private

		private void FormHistoryLimits_Propried()
		{

		}

		#endregion

		#region Propried_Show

// USE /[MANUAL PRO CONTROLLER_SHOW PROPRIED]/

		[HttpPost]
		public ActionResult Propried_Show_GET([FromBody]RequestIdModel requestModel)
		{
			var id = requestModel.Id;
			var model = new Propried_ViewModel(UserContext.Current);
			var eventSink = new EventSink()
			{
				MethodName = "Propried_Show_GET",
				AreaName = "propr",
				Location = ACTION_PROPRIED_SHOW,
				BeforeOp = (sink, sp) =>
				{
					FormHistoryLimits_Propried();
// USE /[MANUAL PRO BEFORE_LOAD_SHOW PROPRIED]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_SHOW PROPRIED]/
				}
			};

			return GenericHandleGetFormShow(eventSink, model, id);
		}

		#endregion

		#region Propried_New

// USE /[MANUAL PRO CONTROLLER_NEW_GET PROPRIED]/
		[HttpPost]
		public ActionResult Propried_New_GET([FromBody]RequestNewGetModel requestModel)
		{
			var id = requestModel.Id;
			var isNewLocation = requestModel.IsNewLocation;
			var prefillValues = requestModel.PrefillValues;

			var model = new Propried_ViewModel(UserContext.Current);
			var eventSink = new EventSink()
			{
				MethodName = "Propried_New_GET",
				AreaName = "propr",
				FormName = "PROPRIED",
				Location = ACTION_PROPRIED_NEW,
				BeforeAll = (sink, sp) =>
				{
					FormHistoryLimits_Propried();
				},
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_LOAD_NEW PROPRIED]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_NEW PROPRIED]/
				}
			};

			return GenericHandleGetFormNew(eventSink, model, id, isNewLocation, prefillValues);
		}

		//
		// POST: /Propr/Propried_New
// USE /[MANUAL PRO CONTROLLER_NEW_POST PROPRIED]/
		[HttpPost]
		public ActionResult Propried_New([FromBody]Propried_ViewModel model, [FromQuery]bool redirect = true)
		{
			var eventSink = new EventSink()
			{
				MethodName = "Propried_New",
				ViewName = "Propried",
				AreaName = "propr",
				Location = ACTION_PROPRIED_NEW,
				Redirect = redirect,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_SAVE_NEW PROPRIED]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_SAVE_NEW PROPRIED]/
				},
				BeforeException = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_LOAD_NEW_EX PROPRIED]/
				},
				AfterException = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_NEW_EX PROPRIED]/
				}
			};

			return GenericHandlePostFormNew(eventSink, model);
		}

		#endregion

		#region Propried_Edit

// USE /[MANUAL PRO CONTROLLER_EDIT_GET PROPRIED]/
		[HttpPost]
		public ActionResult Propried_Edit_GET([FromBody]RequestIdModel requestModel)
		{
			var id = requestModel.Id;
			var model = new Propried_ViewModel(UserContext.Current);
			var eventSink = new EventSink()
			{
				MethodName = "Propried_Edit_GET",
				AreaName = "propr",
				FormName = "PROPRIED",
				Location = ACTION_PROPRIED_EDIT,
				BeforeOp = (sink, sp) =>
				{
					FormHistoryLimits_Propried();
// USE /[MANUAL PRO BEFORE_LOAD_EDIT PROPRIED]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_EDIT PROPRIED]/
				}
			};

			return GenericHandleGetFormEdit(eventSink, model, id);
		}

		//
		// POST: /Propr/Propried_Edit
// USE /[MANUAL PRO CONTROLLER_EDIT_POST PROPRIED]/
		[HttpPost]
		public ActionResult Propried_Edit([FromBody]Propried_ViewModel model, [FromQuery]bool redirect)
		{
			var eventSink = new EventSink()
			{
				MethodName = "Propried_Edit",
				ViewName = "Propried",
				AreaName = "propr",
				Location = ACTION_PROPRIED_EDIT,
				Redirect = redirect,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_SAVE_EDIT PROPRIED]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_SAVE_EDIT PROPRIED]/
				},
				BeforeException = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_LOAD_EDIT_EX PROPRIED]/
				},
				AfterException = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_EDIT_EX PROPRIED]/
				}
			};

			return GenericHandlePostFormEdit(eventSink, model);
		}

		#endregion

		#region Propried_Delete

// USE /[MANUAL PRO CONTROLLER_DELETE_GET PROPRIED]/
		[HttpPost]
		public ActionResult Propried_Delete_GET([FromBody]RequestIdModel requestModel)
		{
			var id = requestModel.Id;
			var model = new Propried_ViewModel(UserContext.Current);
			var eventSink = new EventSink()
			{
				MethodName = "Propried_Delete_GET",
				AreaName = "propr",
				FormName = "PROPRIED",
				Location = ACTION_PROPRIED_DELETE,
				BeforeOp = (sink, sp) =>
				{
					FormHistoryLimits_Propried();
// USE /[MANUAL PRO BEFORE_LOAD_DELETE PROPRIED]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_DELETE PROPRIED]/
				}
			};

			return GenericHandleGetFormDelete(eventSink, model, id);
		}

		//
		// POST: /Propr/Propried_Delete
// USE /[MANUAL PRO CONTROLLER_DELETE_POST PROPRIED]/
		[HttpPost]
		public ActionResult Propried_Delete([FromBody]RequestIdModel requestModel)
		{
			var id = requestModel.Id;
			var model = new Propried_ViewModel (UserContext.Current, id);
			model.MapFromModel();

			var eventSink = new EventSink()
			{
				MethodName = "Propried_Delete",
				ViewName = "Propried",
				AreaName = "propr",
				Location = ACTION_PROPRIED_DELETE,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_DESTROY_DELETE PROPRIED]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_DESTROY_DELETE PROPRIED]/
				}
			};

			return GenericHandlePostFormDelete(eventSink, model);
		}

		public ActionResult Propried_Delete_Redirect()
		{
			//FOR: FORM MENU GO BACK
			return RedirectToFormMenuGoBack("PROPRIED");
		}

		#endregion

		#region Propried_Duplicate

// USE /[MANUAL PRO CONTROLLER_DUPLICATE_GET PROPRIED]/

		[HttpPost]
		public ActionResult Propried_Duplicate_GET([FromBody]RequestNewGetModel requestModel)
		{
			var id = requestModel.Id;
			var isNewLocation = requestModel.IsNewLocation;

			var model = new Propried_ViewModel(UserContext.Current);
			var eventSink = new EventSink()
			{
				MethodName = "Propried_Duplicate_GET",
				AreaName = "propr",
				FormName = "PROPRIED",
				Location = ACTION_PROPRIED_DUPLICATE,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_LOAD_DUPLICATE PROPRIED]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_DUPLICATE PROPRIED]/
				}
			};

			return GenericHandleGetFormDuplicate(eventSink, model, id, isNewLocation);
		}

		//
		// POST: /Propr/Propried_Duplicate
// USE /[MANUAL PRO CONTROLLER_DUPLICATE_POST PROPRIED]/
		[HttpPost]
		public ActionResult Propried_Duplicate([FromBody]Propried_ViewModel model, [FromQuery]bool redirect = true)
		{
			var eventSink = new EventSink()
			{
				MethodName = "Propried_Duplicate",
				ViewName = "Propried",
				AreaName = "propr",
				Location = ACTION_PROPRIED_DUPLICATE,
				Redirect = redirect,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_SAVE_DUPLICATE PROPRIED]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_SAVE_DUPLICATE PROPRIED]/
				},
				BeforeException = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_LOAD_DUPLICATE_EX PROPRIED]/
				},
				AfterException = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_DUPLICATE_EX PROPRIED]/
				}
			};

			return GenericHandlePostFormDuplicate(eventSink, model);
		}

		#endregion

		#region Propried_Cancel

		//
		// GET: /Propr/Propried_Cancel
// USE /[MANUAL PRO CONTROLLER_CANCEL_GET PROPRIED]/
		public ActionResult Propried_Cancel()
		{
			if (Navigation.CurrentLevel.FormMode == FormMode.New || Navigation.CurrentLevel.FormMode == FormMode.Duplicate)
			{
				PersistentSupport sp = UserContext.Current.PersistentSupport;
				try
				{
					var model = new GenioMVC.Models.Propr(UserContext.Current);
					model.klass.QPrimaryKey = Navigation.GetStrValue("propr");

// USE /[MANUAL PRO BEFORE_CANCEL PROPRIED]/

					sp.openTransaction();
					model.Destroy();
					sp.closeTransaction();

// USE /[MANUAL PRO AFTER_CANCEL PROPRIED]/

				}
				catch (Exception e)
				{
					sp.rollbackTransaction();
					sp.closeConnection();
					ClearMessages();

					var exceptionUserMessage = Resources.Resources.PEDIMOS_DESCULPA__OC63848;
					if (e is GenioException && (e as GenioException).UserMessage != null)
						exceptionUserMessage = Translations.Get((e as GenioException).UserMessage, UserContext.Current.User.Language);
					return JsonERROR(exceptionUserMessage);
				}

				Navigation.SetValue("ForcePrimaryRead_propr", "true", true);
			}

			Navigation.ClearValue("propr");

			return JsonOK(new { Success = true, currentNavigationLevel = Navigation.CurrentLevel.Level });
		}

		#endregion


		//
		// GET: /Propr/Propried_CidadValCidade
		// POST: /Propr/Propried_CidadValCidade
		[ActionName("Propried_CidadValCidade")]
		public ActionResult Propried_CidadValCidade([FromBody]RequestLookupModel requestModel)
		{
			var queryParams = requestModel.QueryParams;

			int perPage = CSGenio.framework.Configuration.NrRegDBedit;
			string rowsPerPageOptionsString = "";

			// If there was a recent operation on this table then force the primary persistence server to be called and ignore the read only feature
			if (string.IsNullOrEmpty(Navigation.GetStrValue("ForcePrimaryRead_cidad")))
				UserContext.Current.SetPersistenceReadOnly(true);
			else
			{
				Navigation.DestroyEntry("ForcePrimaryRead_cidad");
				UserContext.Current.SetPersistenceReadOnly(false);
			}

			var requestValues = new NameValueCollection();
			if (queryParams != null)
			{
				// Add to request values
				foreach (var kv in queryParams)
					requestValues.Add(kv.Key, kv.Value);
			}

			IsStateReadonly = true;
			Propried_CidadValCidade_ViewModel model = new Propried_CidadValCidade_ViewModel(UserContext.Current);

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

			model.setModes(Request.Query["m"].ToString());
			model.Load(tableConfig, requestValues, Request.IsAjaxRequest());

			return JsonOK(model);
		}

		//
		// GET: /Propr/Propried_AgentValNome
		// POST: /Propr/Propried_AgentValNome
		[ActionName("Propried_AgentValNome")]
		public ActionResult Propried_AgentValNome([FromBody]RequestLookupModel requestModel)
		{
			var queryParams = requestModel.QueryParams;

			int perPage = CSGenio.framework.Configuration.NrRegDBedit;
			string rowsPerPageOptionsString = "";

			// If there was a recent operation on this table then force the primary persistence server to be called and ignore the read only feature
			if (string.IsNullOrEmpty(Navigation.GetStrValue("ForcePrimaryRead_agent")))
				UserContext.Current.SetPersistenceReadOnly(true);
			else
			{
				Navigation.DestroyEntry("ForcePrimaryRead_agent");
				UserContext.Current.SetPersistenceReadOnly(false);
			}

			var requestValues = new NameValueCollection();
			if (queryParams != null)
			{
				// Add to request values
				foreach (var kv in queryParams)
					requestValues.Add(kv.Key, kv.Value);
			}

			IsStateReadonly = true;
			Propried_AgentValNome_ViewModel model = new Propried_AgentValNome_ViewModel(UserContext.Current);

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

			model.setModes(Request.Query["m"].ToString());
			model.Load(tableConfig, requestValues, Request.IsAjaxRequest());

			return JsonOK(model);
		}

		//
		// GET: /Propr/Propried_ValField001
		// POST: /Propr/Propried_ValField001
		[ActionName("Propried_ValField001")]
		public ActionResult Propried_ValField001([FromBody]RequestLookupModel requestModel)
		{
			var queryParams = requestModel.QueryParams;

			int perPage = 10;
			string rowsPerPageOptionsString = "";

			// If there was a recent operation on this table then force the primary persistence server to be called and ignore the read only feature
			if (string.IsNullOrEmpty(Navigation.GetStrValue("ForcePrimaryRead_album")))
				UserContext.Current.SetPersistenceReadOnly(true);
			else
			{
				Navigation.DestroyEntry("ForcePrimaryRead_album");
				UserContext.Current.SetPersistenceReadOnly(false);
			}

			var requestValues = new NameValueCollection();
			if (queryParams != null)
			{
				// Add to request values
				foreach (var kv in queryParams)
					requestValues.Add(kv.Key, kv.Value);
			}

			Propried_ValField001_ViewModel model = new Propried_ValField001_ViewModel(UserContext.Current);

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

			model.setModes(Request.Query["m"].ToString());
			model.Load(tableConfig, requestValues, Request.IsAjaxRequest());

			return JsonOK(model);
		}

		//
		// GET: /Propr/Propried_ValField002
		// POST: /Propr/Propried_ValField002
		[ActionName("Propried_ValField002")]
		public ActionResult Propried_ValField002([FromBody]RequestLookupModel requestModel)
		{
			var queryParams = requestModel.QueryParams;

			int perPage = 3;
			string rowsPerPageOptionsString = "";

			// If there was a recent operation on this table then force the primary persistence server to be called and ignore the read only feature
			if (string.IsNullOrEmpty(Navigation.GetStrValue("ForcePrimaryRead_contc")))
				UserContext.Current.SetPersistenceReadOnly(true);
			else
			{
				Navigation.DestroyEntry("ForcePrimaryRead_contc");
				UserContext.Current.SetPersistenceReadOnly(false);
			}

			var requestValues = new NameValueCollection();
			if (queryParams != null)
			{
				// Add to request values
				foreach (var kv in queryParams)
					requestValues.Add(kv.Key, kv.Value);
			}

			Propried_ValField002_ViewModel model = new Propried_ValField002_ViewModel(UserContext.Current);

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

			model.setModes(Request.Query["m"].ToString());
			model.Load(tableConfig, requestValues, Request.IsAjaxRequest());

			return JsonOK(model);
		}


		// POST: /Propr/Propried_SaveEdit
		[HttpPost]
		public ActionResult Propried_SaveEdit([FromBody]Propried_ViewModel model)
		{
			var eventSink = new EventSink()
			{
				MethodName = "Propried_SaveEdit",
				ViewName = "Propried",
				AreaName = "propr",
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_APPLY_EDIT PROPRIED]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_APPLY_EDIT PROPRIED]/
				}
			};

			return GenericHandlePostFormApply(eventSink, model);
		}
	}
}
