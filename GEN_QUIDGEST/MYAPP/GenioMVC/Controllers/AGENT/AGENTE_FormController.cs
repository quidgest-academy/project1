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
using GenioMVC.ViewModels.Agent;
using Quidgest.Persistence.GenericQuery;

// USE /[MANUAL PRO INCLUDE_CONTROLLER AGENT]/

namespace GenioMVC.Controllers
{
	public partial class AgentController : ControllerBase
	{
		#region NavigationLocation Names

		private static readonly NavigationLocation ACTION_AGENTE_CANCEL = new("AGENTE_IMOBILIARIO28727", "Agente_Cancel", "Agent") { vueRouteName = "form-AGENTE", mode = "CANCEL" };
		private static readonly NavigationLocation ACTION_AGENTE_SHOW = new("AGENTE_IMOBILIARIO28727", "Agente_Show", "Agent") { vueRouteName = "form-AGENTE", mode = "SHOW" };
		private static readonly NavigationLocation ACTION_AGENTE_NEW = new("AGENTE_IMOBILIARIO28727", "Agente_New", "Agent") { vueRouteName = "form-AGENTE", mode = "NEW" };
		private static readonly NavigationLocation ACTION_AGENTE_EDIT = new("AGENTE_IMOBILIARIO28727", "Agente_Edit", "Agent") { vueRouteName = "form-AGENTE", mode = "EDIT" };
		private static readonly NavigationLocation ACTION_AGENTE_DUPLICATE = new("AGENTE_IMOBILIARIO28727", "Agente_Duplicate", "Agent") { vueRouteName = "form-AGENTE", mode = "DUPLICATE" };
		private static readonly NavigationLocation ACTION_AGENTE_DELETE = new("AGENTE_IMOBILIARIO28727", "Agente_Delete", "Agent") { vueRouteName = "form-AGENTE", mode = "DELETE" };

		#endregion

		#region Agente private

		private void FormHistoryLimits_Agente()
		{

		}

		#endregion

		#region Agente_Show

// USE /[MANUAL PRO CONTROLLER_SHOW AGENTE]/

		[HttpPost]
		public ActionResult Agente_Show_GET([FromBody]RequestIdModel requestModel)
		{
			var id = requestModel.Id;
			var model = new Agente_ViewModel(UserContext.Current);
			var eventSink = new EventSink()
			{
				MethodName = "Agente_Show_GET",
				AreaName = "agent",
				Location = ACTION_AGENTE_SHOW,
				BeforeOp = (sink, sp) =>
				{
					FormHistoryLimits_Agente();
// USE /[MANUAL PRO BEFORE_LOAD_SHOW AGENTE]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_SHOW AGENTE]/
				}
			};

			return GenericHandleGetFormShow(eventSink, model, id);
		}

		#endregion

		#region Agente_New

// USE /[MANUAL PRO CONTROLLER_NEW_GET AGENTE]/
		[HttpPost]
		public ActionResult Agente_New_GET([FromBody]RequestNewGetModel requestModel)
		{
			var id = requestModel.Id;
			var isNewLocation = requestModel.IsNewLocation;
			var prefillValues = requestModel.PrefillValues;

			var model = new Agente_ViewModel(UserContext.Current);
			var eventSink = new EventSink()
			{
				MethodName = "Agente_New_GET",
				AreaName = "agent",
				FormName = "AGENTE",
				Location = ACTION_AGENTE_NEW,
				BeforeAll = (sink, sp) =>
				{
					FormHistoryLimits_Agente();
				},
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_LOAD_NEW AGENTE]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_NEW AGENTE]/
				}
			};

			return GenericHandleGetFormNew(eventSink, model, id, isNewLocation, prefillValues);
		}

		//
		// POST: /Agent/Agente_New
// USE /[MANUAL PRO CONTROLLER_NEW_POST AGENTE]/
		[HttpPost]
		public ActionResult Agente_New([FromBody]Agente_ViewModel model, [FromQuery]bool redirect = true)
		{
			var eventSink = new EventSink()
			{
				MethodName = "Agente_New",
				ViewName = "Agente",
				AreaName = "agent",
				Location = ACTION_AGENTE_NEW,
				Redirect = redirect,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_SAVE_NEW AGENTE]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_SAVE_NEW AGENTE]/
				},
				BeforeException = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_LOAD_NEW_EX AGENTE]/
				},
				AfterException = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_NEW_EX AGENTE]/
				}
			};

			return GenericHandlePostFormNew(eventSink, model);
		}

		#endregion

		#region Agente_Edit

// USE /[MANUAL PRO CONTROLLER_EDIT_GET AGENTE]/
		[HttpPost]
		public ActionResult Agente_Edit_GET([FromBody]RequestIdModel requestModel)
		{
			var id = requestModel.Id;
			var model = new Agente_ViewModel(UserContext.Current);
			var eventSink = new EventSink()
			{
				MethodName = "Agente_Edit_GET",
				AreaName = "agent",
				FormName = "AGENTE",
				Location = ACTION_AGENTE_EDIT,
				BeforeOp = (sink, sp) =>
				{
					FormHistoryLimits_Agente();
// USE /[MANUAL PRO BEFORE_LOAD_EDIT AGENTE]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_EDIT AGENTE]/
				}
			};

			return GenericHandleGetFormEdit(eventSink, model, id);
		}

		//
		// POST: /Agent/Agente_Edit
// USE /[MANUAL PRO CONTROLLER_EDIT_POST AGENTE]/
		[HttpPost]
		public ActionResult Agente_Edit([FromBody]Agente_ViewModel model, [FromQuery]bool redirect)
		{
			var eventSink = new EventSink()
			{
				MethodName = "Agente_Edit",
				ViewName = "Agente",
				AreaName = "agent",
				Location = ACTION_AGENTE_EDIT,
				Redirect = redirect,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_SAVE_EDIT AGENTE]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_SAVE_EDIT AGENTE]/
				},
				BeforeException = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_LOAD_EDIT_EX AGENTE]/
				},
				AfterException = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_EDIT_EX AGENTE]/
				}
			};

			return GenericHandlePostFormEdit(eventSink, model);
		}

		#endregion

		#region Agente_Delete

// USE /[MANUAL PRO CONTROLLER_DELETE_GET AGENTE]/
		[HttpPost]
		public ActionResult Agente_Delete_GET([FromBody]RequestIdModel requestModel)
		{
			var id = requestModel.Id;
			var model = new Agente_ViewModel(UserContext.Current);
			var eventSink = new EventSink()
			{
				MethodName = "Agente_Delete_GET",
				AreaName = "agent",
				FormName = "AGENTE",
				Location = ACTION_AGENTE_DELETE,
				BeforeOp = (sink, sp) =>
				{
					FormHistoryLimits_Agente();
// USE /[MANUAL PRO BEFORE_LOAD_DELETE AGENTE]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_DELETE AGENTE]/
				}
			};

			return GenericHandleGetFormDelete(eventSink, model, id);
		}

		//
		// POST: /Agent/Agente_Delete
// USE /[MANUAL PRO CONTROLLER_DELETE_POST AGENTE]/
		[HttpPost]
		public ActionResult Agente_Delete([FromBody]RequestIdModel requestModel)
		{
			var id = requestModel.Id;
			var model = new Agente_ViewModel (UserContext.Current, id);
			model.MapFromModel();

			var eventSink = new EventSink()
			{
				MethodName = "Agente_Delete",
				ViewName = "Agente",
				AreaName = "agent",
				Location = ACTION_AGENTE_DELETE,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_DESTROY_DELETE AGENTE]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_DESTROY_DELETE AGENTE]/
				}
			};

			return GenericHandlePostFormDelete(eventSink, model);
		}

		public ActionResult Agente_Delete_Redirect()
		{
			//FOR: FORM MENU GO BACK
			return RedirectToFormMenuGoBack("AGENTE");
		}

		#endregion

		#region Agente_Duplicate

// USE /[MANUAL PRO CONTROLLER_DUPLICATE_GET AGENTE]/

		[HttpPost]
		public ActionResult Agente_Duplicate_GET([FromBody]RequestNewGetModel requestModel)
		{
			var id = requestModel.Id;
			var isNewLocation = requestModel.IsNewLocation;

			var model = new Agente_ViewModel(UserContext.Current);
			var eventSink = new EventSink()
			{
				MethodName = "Agente_Duplicate_GET",
				AreaName = "agent",
				FormName = "AGENTE",
				Location = ACTION_AGENTE_DUPLICATE,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_LOAD_DUPLICATE AGENTE]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_DUPLICATE AGENTE]/
				}
			};

			return GenericHandleGetFormDuplicate(eventSink, model, id, isNewLocation);
		}

		//
		// POST: /Agent/Agente_Duplicate
// USE /[MANUAL PRO CONTROLLER_DUPLICATE_POST AGENTE]/
		[HttpPost]
		public ActionResult Agente_Duplicate([FromBody]Agente_ViewModel model, [FromQuery]bool redirect = true)
		{
			var eventSink = new EventSink()
			{
				MethodName = "Agente_Duplicate",
				ViewName = "Agente",
				AreaName = "agent",
				Location = ACTION_AGENTE_DUPLICATE,
				Redirect = redirect,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_SAVE_DUPLICATE AGENTE]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_SAVE_DUPLICATE AGENTE]/
				},
				BeforeException = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_LOAD_DUPLICATE_EX AGENTE]/
				},
				AfterException = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_DUPLICATE_EX AGENTE]/
				}
			};

			return GenericHandlePostFormDuplicate(eventSink, model);
		}

		#endregion

		#region Agente_Cancel

		//
		// GET: /Agent/Agente_Cancel
// USE /[MANUAL PRO CONTROLLER_CANCEL_GET AGENTE]/
		public ActionResult Agente_Cancel()
		{
			if (Navigation.CurrentLevel.FormMode == FormMode.New || Navigation.CurrentLevel.FormMode == FormMode.Duplicate)
			{
				PersistentSupport sp = UserContext.Current.PersistentSupport;
				try
				{
					var model = new GenioMVC.Models.Agent(UserContext.Current);
					model.klass.QPrimaryKey = Navigation.GetStrValue("agent");

// USE /[MANUAL PRO BEFORE_CANCEL AGENTE]/

					sp.openTransaction();
					model.Destroy();
					sp.closeTransaction();

// USE /[MANUAL PRO AFTER_CANCEL AGENTE]/

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

				Navigation.SetValue("ForcePrimaryRead_agent", "true", true);
			}

			Navigation.ClearValue("agent");

			return JsonOK(new { Success = true, currentNavigationLevel = Navigation.CurrentLevel.Level });
		}

		#endregion


		//
		// GET: /Agent/Agente_PmoraValPais
		// POST: /Agent/Agente_PmoraValPais
		[ActionName("Agente_PmoraValPais")]
		public ActionResult Agente_PmoraValPais([FromBody]RequestLookupModel requestModel)
		{
			var queryParams = requestModel.QueryParams;

			int perPage = CSGenio.framework.Configuration.NrRegDBedit;
			string rowsPerPageOptionsString = "";

			// If there was a recent operation on this table then force the primary persistence server to be called and ignore the read only feature
			if (string.IsNullOrEmpty(Navigation.GetStrValue("ForcePrimaryRead_pmora")))
				UserContext.Current.SetPersistenceReadOnly(true);
			else
			{
				Navigation.DestroyEntry("ForcePrimaryRead_pmora");
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
			Agente_PmoraValPais_ViewModel model = new Agente_PmoraValPais_ViewModel(UserContext.Current);

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
		// GET: /Agent/Agente_PnascValPais
		// POST: /Agent/Agente_PnascValPais
		[ActionName("Agente_PnascValPais")]
		public ActionResult Agente_PnascValPais([FromBody]RequestLookupModel requestModel)
		{
			var queryParams = requestModel.QueryParams;

			int perPage = CSGenio.framework.Configuration.NrRegDBedit;
			string rowsPerPageOptionsString = "";

			// If there was a recent operation on this table then force the primary persistence server to be called and ignore the read only feature
			if (string.IsNullOrEmpty(Navigation.GetStrValue("ForcePrimaryRead_pnasc")))
				UserContext.Current.SetPersistenceReadOnly(true);
			else
			{
				Navigation.DestroyEntry("ForcePrimaryRead_pnasc");
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
			Agente_PnascValPais_ViewModel model = new Agente_PnascValPais_ViewModel(UserContext.Current);

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


		// POST: /Agent/Agente_SaveEdit
		[HttpPost]
		public ActionResult Agente_SaveEdit([FromBody]Agente_ViewModel model)
		{
			var eventSink = new EventSink()
			{
				MethodName = "Agente_SaveEdit",
				ViewName = "Agente",
				AreaName = "agent",
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_APPLY_EDIT AGENTE]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_APPLY_EDIT AGENTE]/
				}
			};

			return GenericHandlePostFormApply(eventSink, model);
		}
	}
}
