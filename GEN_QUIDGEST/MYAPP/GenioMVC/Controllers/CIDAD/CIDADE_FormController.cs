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
using GenioMVC.ViewModels.Cidad;
using Quidgest.Persistence.GenericQuery;

// USE /[MANUAL PRO INCLUDE_CONTROLLER CIDAD]/

namespace GenioMVC.Controllers
{
	public partial class CidadController : ControllerBase
	{
		#region NavigationLocation Names

		private static readonly NavigationLocation ACTION_CIDADE_CANCEL = new("CIDADE42080", "Cidade_Cancel", "Cidad") { vueRouteName = "form-CIDADE", mode = "CANCEL" };
		private static readonly NavigationLocation ACTION_CIDADE_SHOW = new("CIDADE42080", "Cidade_Show", "Cidad") { vueRouteName = "form-CIDADE", mode = "SHOW" };
		private static readonly NavigationLocation ACTION_CIDADE_NEW = new("CIDADE42080", "Cidade_New", "Cidad") { vueRouteName = "form-CIDADE", mode = "NEW" };
		private static readonly NavigationLocation ACTION_CIDADE_EDIT = new("CIDADE42080", "Cidade_Edit", "Cidad") { vueRouteName = "form-CIDADE", mode = "EDIT" };
		private static readonly NavigationLocation ACTION_CIDADE_DUPLICATE = new("CIDADE42080", "Cidade_Duplicate", "Cidad") { vueRouteName = "form-CIDADE", mode = "DUPLICATE" };
		private static readonly NavigationLocation ACTION_CIDADE_DELETE = new("CIDADE42080", "Cidade_Delete", "Cidad") { vueRouteName = "form-CIDADE", mode = "DELETE" };

		#endregion

		#region Cidade private

		private void FormHistoryLimits_Cidade()
		{

		}

		#endregion

		#region Cidade_Show

// USE /[MANUAL PRO CONTROLLER_SHOW CIDADE]/

		[HttpPost]
		public ActionResult Cidade_Show_GET([FromBody]RequestIdModel requestModel)
		{
			var id = requestModel.Id;
			var model = new Cidade_ViewModel(UserContext.Current);
			var eventSink = new EventSink()
			{
				MethodName = "Cidade_Show_GET",
				AreaName = "cidad",
				Location = ACTION_CIDADE_SHOW,
				BeforeOp = (sink, sp) =>
				{
					FormHistoryLimits_Cidade();
// USE /[MANUAL PRO BEFORE_LOAD_SHOW CIDADE]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_SHOW CIDADE]/
				}
			};

			return GenericHandleGetFormShow(eventSink, model, id);
		}

		#endregion

		#region Cidade_New

// USE /[MANUAL PRO CONTROLLER_NEW_GET CIDADE]/
		[HttpPost]
		public ActionResult Cidade_New_GET([FromBody]RequestNewGetModel requestModel)
		{
			var id = requestModel.Id;
			var isNewLocation = requestModel.IsNewLocation;
			var prefillValues = requestModel.PrefillValues;

			var model = new Cidade_ViewModel(UserContext.Current);
			var eventSink = new EventSink()
			{
				MethodName = "Cidade_New_GET",
				AreaName = "cidad",
				FormName = "CIDADE",
				Location = ACTION_CIDADE_NEW,
				BeforeAll = (sink, sp) =>
				{
					FormHistoryLimits_Cidade();
				},
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_LOAD_NEW CIDADE]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_NEW CIDADE]/
				}
			};

			return GenericHandleGetFormNew(eventSink, model, id, isNewLocation, prefillValues);
		}

		//
		// POST: /Cidad/Cidade_New
// USE /[MANUAL PRO CONTROLLER_NEW_POST CIDADE]/
		[HttpPost]
		public ActionResult Cidade_New([FromBody]Cidade_ViewModel model, [FromQuery]bool redirect = true)
		{
			var eventSink = new EventSink()
			{
				MethodName = "Cidade_New",
				ViewName = "Cidade",
				AreaName = "cidad",
				Location = ACTION_CIDADE_NEW,
				Redirect = redirect,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_SAVE_NEW CIDADE]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_SAVE_NEW CIDADE]/
				},
				BeforeException = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_LOAD_NEW_EX CIDADE]/
				},
				AfterException = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_NEW_EX CIDADE]/
				}
			};

			return GenericHandlePostFormNew(eventSink, model);
		}

		#endregion

		#region Cidade_Edit

// USE /[MANUAL PRO CONTROLLER_EDIT_GET CIDADE]/
		[HttpPost]
		public ActionResult Cidade_Edit_GET([FromBody]RequestIdModel requestModel)
		{
			var id = requestModel.Id;
			var model = new Cidade_ViewModel(UserContext.Current);
			var eventSink = new EventSink()
			{
				MethodName = "Cidade_Edit_GET",
				AreaName = "cidad",
				FormName = "CIDADE",
				Location = ACTION_CIDADE_EDIT,
				BeforeOp = (sink, sp) =>
				{
					FormHistoryLimits_Cidade();
// USE /[MANUAL PRO BEFORE_LOAD_EDIT CIDADE]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_EDIT CIDADE]/
				}
			};

			return GenericHandleGetFormEdit(eventSink, model, id);
		}

		//
		// POST: /Cidad/Cidade_Edit
// USE /[MANUAL PRO CONTROLLER_EDIT_POST CIDADE]/
		[HttpPost]
		public ActionResult Cidade_Edit([FromBody]Cidade_ViewModel model, [FromQuery]bool redirect)
		{
			var eventSink = new EventSink()
			{
				MethodName = "Cidade_Edit",
				ViewName = "Cidade",
				AreaName = "cidad",
				Location = ACTION_CIDADE_EDIT,
				Redirect = redirect,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_SAVE_EDIT CIDADE]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_SAVE_EDIT CIDADE]/
				},
				BeforeException = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_LOAD_EDIT_EX CIDADE]/
				},
				AfterException = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_EDIT_EX CIDADE]/
				}
			};

			return GenericHandlePostFormEdit(eventSink, model);
		}

		#endregion

		#region Cidade_Delete

// USE /[MANUAL PRO CONTROLLER_DELETE_GET CIDADE]/
		[HttpPost]
		public ActionResult Cidade_Delete_GET([FromBody]RequestIdModel requestModel)
		{
			var id = requestModel.Id;
			var model = new Cidade_ViewModel(UserContext.Current);
			var eventSink = new EventSink()
			{
				MethodName = "Cidade_Delete_GET",
				AreaName = "cidad",
				FormName = "CIDADE",
				Location = ACTION_CIDADE_DELETE,
				BeforeOp = (sink, sp) =>
				{
					FormHistoryLimits_Cidade();
// USE /[MANUAL PRO BEFORE_LOAD_DELETE CIDADE]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_DELETE CIDADE]/
				}
			};

			return GenericHandleGetFormDelete(eventSink, model, id);
		}

		//
		// POST: /Cidad/Cidade_Delete
// USE /[MANUAL PRO CONTROLLER_DELETE_POST CIDADE]/
		[HttpPost]
		public ActionResult Cidade_Delete([FromBody]RequestIdModel requestModel)
		{
			var id = requestModel.Id;
			var model = new Cidade_ViewModel (UserContext.Current, id);
			model.MapFromModel();

			var eventSink = new EventSink()
			{
				MethodName = "Cidade_Delete",
				ViewName = "Cidade",
				AreaName = "cidad",
				Location = ACTION_CIDADE_DELETE,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_DESTROY_DELETE CIDADE]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_DESTROY_DELETE CIDADE]/
				}
			};

			return GenericHandlePostFormDelete(eventSink, model);
		}

		public ActionResult Cidade_Delete_Redirect()
		{
			//FOR: FORM MENU GO BACK
			return RedirectToFormMenuGoBack("CIDADE");
		}

		#endregion

		#region Cidade_Duplicate

// USE /[MANUAL PRO CONTROLLER_DUPLICATE_GET CIDADE]/

		[HttpPost]
		public ActionResult Cidade_Duplicate_GET([FromBody]RequestNewGetModel requestModel)
		{
			var id = requestModel.Id;
			var isNewLocation = requestModel.IsNewLocation;

			var model = new Cidade_ViewModel(UserContext.Current);
			var eventSink = new EventSink()
			{
				MethodName = "Cidade_Duplicate_GET",
				AreaName = "cidad",
				FormName = "CIDADE",
				Location = ACTION_CIDADE_DUPLICATE,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_LOAD_DUPLICATE CIDADE]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_DUPLICATE CIDADE]/
				}
			};

			return GenericHandleGetFormDuplicate(eventSink, model, id, isNewLocation);
		}

		//
		// POST: /Cidad/Cidade_Duplicate
// USE /[MANUAL PRO CONTROLLER_DUPLICATE_POST CIDADE]/
		[HttpPost]
		public ActionResult Cidade_Duplicate([FromBody]Cidade_ViewModel model, [FromQuery]bool redirect = true)
		{
			var eventSink = new EventSink()
			{
				MethodName = "Cidade_Duplicate",
				ViewName = "Cidade",
				AreaName = "cidad",
				Location = ACTION_CIDADE_DUPLICATE,
				Redirect = redirect,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_SAVE_DUPLICATE CIDADE]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_SAVE_DUPLICATE CIDADE]/
				},
				BeforeException = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_LOAD_DUPLICATE_EX CIDADE]/
				},
				AfterException = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_DUPLICATE_EX CIDADE]/
				}
			};

			return GenericHandlePostFormDuplicate(eventSink, model);
		}

		#endregion

		#region Cidade_Cancel

		//
		// GET: /Cidad/Cidade_Cancel
// USE /[MANUAL PRO CONTROLLER_CANCEL_GET CIDADE]/
		public ActionResult Cidade_Cancel()
		{
			if (Navigation.CurrentLevel.FormMode == FormMode.New || Navigation.CurrentLevel.FormMode == FormMode.Duplicate)
			{
				PersistentSupport sp = UserContext.Current.PersistentSupport;
				try
				{
					var model = new GenioMVC.Models.Cidad(UserContext.Current);
					model.klass.QPrimaryKey = Navigation.GetStrValue("cidad");

// USE /[MANUAL PRO BEFORE_CANCEL CIDADE]/

					sp.openTransaction();
					model.Destroy();
					sp.closeTransaction();

// USE /[MANUAL PRO AFTER_CANCEL CIDADE]/

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

				Navigation.SetValue("ForcePrimaryRead_cidad", "true", true);
			}

			Navigation.ClearValue("cidad");

			return JsonOK(new { Success = true, currentNavigationLevel = Navigation.CurrentLevel.Level });
		}

		#endregion


		//
		// GET: /Cidad/Cidade_PaisValPais
		// POST: /Cidad/Cidade_PaisValPais
		[ActionName("Cidade_PaisValPais")]
		public ActionResult Cidade_PaisValPais([FromBody]RequestLookupModel requestModel)
		{
			var queryParams = requestModel.QueryParams;

			int perPage = CSGenio.framework.Configuration.NrRegDBedit;
			string rowsPerPageOptionsString = "";

			// If there was a recent operation on this table then force the primary persistence server to be called and ignore the read only feature
			if (string.IsNullOrEmpty(Navigation.GetStrValue("ForcePrimaryRead_pais")))
				UserContext.Current.SetPersistenceReadOnly(true);
			else
			{
				Navigation.DestroyEntry("ForcePrimaryRead_pais");
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
			Cidade_PaisValPais_ViewModel model = new Cidade_PaisValPais_ViewModel(UserContext.Current);

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
		// GET: /Cidad/Cidade_ValField001
		// POST: /Cidad/Cidade_ValField001
		[ActionName("Cidade_ValField001")]
		public ActionResult Cidade_ValField001([FromBody]RequestLookupModel requestModel)
		{
			var queryParams = requestModel.QueryParams;

			int perPage = CSGenio.framework.Configuration.NrRegDBedit;
			string rowsPerPageOptionsString = "";

			// If there was a recent operation on this table then force the primary persistence server to be called and ignore the read only feature
			if (string.IsNullOrEmpty(Navigation.GetStrValue("ForcePrimaryRead_propr")))
				UserContext.Current.SetPersistenceReadOnly(true);
			else
			{
				Navigation.DestroyEntry("ForcePrimaryRead_propr");
				UserContext.Current.SetPersistenceReadOnly(false);
			}

			var requestValues = new NameValueCollection();
			if (queryParams != null)
			{
				// Add to request values
				foreach (var kv in queryParams)
					requestValues.Add(kv.Key, kv.Value);
			}

			Cidade_ValField001_ViewModel model = new Cidade_ValField001_ViewModel(UserContext.Current);

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


		// POST: /Cidad/Cidade_SaveEdit
		[HttpPost]
		public ActionResult Cidade_SaveEdit([FromBody]Cidade_ViewModel model)
		{
			var eventSink = new EventSink()
			{
				MethodName = "Cidade_SaveEdit",
				ViewName = "Cidade",
				AreaName = "cidad",
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_APPLY_EDIT CIDADE]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_APPLY_EDIT CIDADE]/
				}
			};

			return GenericHandlePostFormApply(eventSink, model);
		}
	}
}
