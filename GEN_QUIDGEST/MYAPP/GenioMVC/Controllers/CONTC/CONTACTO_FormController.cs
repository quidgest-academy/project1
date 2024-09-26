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
using GenioMVC.ViewModels.Contc;
using Quidgest.Persistence.GenericQuery;

// USE /[MANUAL PRO INCLUDE_CONTROLLER CONTC]/

namespace GenioMVC.Controllers
{
	public partial class ContcController : ControllerBase
	{
		#region NavigationLocation Names

		private static readonly NavigationLocation ACTION_CONTACTO_CANCEL = new("CONTACTO_DE_CLIENTE62085", "Contacto_Cancel", "Contc") { vueRouteName = "form-CONTACTO", mode = "CANCEL" };
		private static readonly NavigationLocation ACTION_CONTACTO_SHOW = new("CONTACTO_DE_CLIENTE62085", "Contacto_Show", "Contc") { vueRouteName = "form-CONTACTO", mode = "SHOW" };
		private static readonly NavigationLocation ACTION_CONTACTO_NEW = new("CONTACTO_DE_CLIENTE62085", "Contacto_New", "Contc") { vueRouteName = "form-CONTACTO", mode = "NEW" };
		private static readonly NavigationLocation ACTION_CONTACTO_EDIT = new("CONTACTO_DE_CLIENTE62085", "Contacto_Edit", "Contc") { vueRouteName = "form-CONTACTO", mode = "EDIT" };
		private static readonly NavigationLocation ACTION_CONTACTO_DUPLICATE = new("CONTACTO_DE_CLIENTE62085", "Contacto_Duplicate", "Contc") { vueRouteName = "form-CONTACTO", mode = "DUPLICATE" };
		private static readonly NavigationLocation ACTION_CONTACTO_DELETE = new("CONTACTO_DE_CLIENTE62085", "Contacto_Delete", "Contc") { vueRouteName = "form-CONTACTO", mode = "DELETE" };

		#endregion

		#region Contacto private

		private void FormHistoryLimits_Contacto()
		{

		}

		#endregion

		#region Contacto_Show

// USE /[MANUAL PRO CONTROLLER_SHOW CONTACTO]/

		[HttpPost]
		public ActionResult Contacto_Show_GET([FromBody]RequestIdModel requestModel)
		{
			var id = requestModel.Id;
			var model = new Contacto_ViewModel(UserContext.Current);
			var eventSink = new EventSink()
			{
				MethodName = "Contacto_Show_GET",
				AreaName = "contc",
				Location = ACTION_CONTACTO_SHOW,
				BeforeOp = (sink, sp) =>
				{
					FormHistoryLimits_Contacto();
// USE /[MANUAL PRO BEFORE_LOAD_SHOW CONTACTO]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_SHOW CONTACTO]/
				}
			};

			return GenericHandleGetFormShow(eventSink, model, id);
		}

		#endregion

		#region Contacto_New

// USE /[MANUAL PRO CONTROLLER_NEW_GET CONTACTO]/
		[HttpPost]
		public ActionResult Contacto_New_GET([FromBody]RequestNewGetModel requestModel)
		{
			var id = requestModel.Id;
			var isNewLocation = requestModel.IsNewLocation;
			var prefillValues = requestModel.PrefillValues;

			var model = new Contacto_ViewModel(UserContext.Current);
			var eventSink = new EventSink()
			{
				MethodName = "Contacto_New_GET",
				AreaName = "contc",
				FormName = "CONTACTO",
				Location = ACTION_CONTACTO_NEW,
				BeforeAll = (sink, sp) =>
				{
					FormHistoryLimits_Contacto();
				},
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_LOAD_NEW CONTACTO]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_NEW CONTACTO]/
				}
			};

			return GenericHandleGetFormNew(eventSink, model, id, isNewLocation, prefillValues);
		}

		//
		// POST: /Contc/Contacto_New
// USE /[MANUAL PRO CONTROLLER_NEW_POST CONTACTO]/
		[HttpPost]
		public ActionResult Contacto_New([FromBody]Contacto_ViewModel model, [FromQuery]bool redirect = true)
		{
			var eventSink = new EventSink()
			{
				MethodName = "Contacto_New",
				ViewName = "Contacto",
				AreaName = "contc",
				Location = ACTION_CONTACTO_NEW,
				Redirect = redirect,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_SAVE_NEW CONTACTO]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_SAVE_NEW CONTACTO]/
				},
				BeforeException = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_LOAD_NEW_EX CONTACTO]/
				},
				AfterException = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_NEW_EX CONTACTO]/
				}
			};

			return GenericHandlePostFormNew(eventSink, model);
		}

		#endregion

		#region Contacto_Edit

// USE /[MANUAL PRO CONTROLLER_EDIT_GET CONTACTO]/
		[HttpPost]
		public ActionResult Contacto_Edit_GET([FromBody]RequestIdModel requestModel)
		{
			var id = requestModel.Id;
			var model = new Contacto_ViewModel(UserContext.Current);
			var eventSink = new EventSink()
			{
				MethodName = "Contacto_Edit_GET",
				AreaName = "contc",
				FormName = "CONTACTO",
				Location = ACTION_CONTACTO_EDIT,
				BeforeOp = (sink, sp) =>
				{
					FormHistoryLimits_Contacto();
// USE /[MANUAL PRO BEFORE_LOAD_EDIT CONTACTO]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_EDIT CONTACTO]/
				}
			};

			return GenericHandleGetFormEdit(eventSink, model, id);
		}

		//
		// POST: /Contc/Contacto_Edit
// USE /[MANUAL PRO CONTROLLER_EDIT_POST CONTACTO]/
		[HttpPost]
		public ActionResult Contacto_Edit([FromBody]Contacto_ViewModel model, [FromQuery]bool redirect)
		{
			var eventSink = new EventSink()
			{
				MethodName = "Contacto_Edit",
				ViewName = "Contacto",
				AreaName = "contc",
				Location = ACTION_CONTACTO_EDIT,
				Redirect = redirect,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_SAVE_EDIT CONTACTO]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_SAVE_EDIT CONTACTO]/
				},
				BeforeException = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_LOAD_EDIT_EX CONTACTO]/
				},
				AfterException = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_EDIT_EX CONTACTO]/
				}
			};

			return GenericHandlePostFormEdit(eventSink, model);
		}

		#endregion

		#region Contacto_Delete

// USE /[MANUAL PRO CONTROLLER_DELETE_GET CONTACTO]/
		[HttpPost]
		public ActionResult Contacto_Delete_GET([FromBody]RequestIdModel requestModel)
		{
			var id = requestModel.Id;
			var model = new Contacto_ViewModel(UserContext.Current);
			var eventSink = new EventSink()
			{
				MethodName = "Contacto_Delete_GET",
				AreaName = "contc",
				FormName = "CONTACTO",
				Location = ACTION_CONTACTO_DELETE,
				BeforeOp = (sink, sp) =>
				{
					FormHistoryLimits_Contacto();
// USE /[MANUAL PRO BEFORE_LOAD_DELETE CONTACTO]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_DELETE CONTACTO]/
				}
			};

			return GenericHandleGetFormDelete(eventSink, model, id);
		}

		//
		// POST: /Contc/Contacto_Delete
// USE /[MANUAL PRO CONTROLLER_DELETE_POST CONTACTO]/
		[HttpPost]
		public ActionResult Contacto_Delete([FromBody]RequestIdModel requestModel)
		{
			var id = requestModel.Id;
			var model = new Contacto_ViewModel (UserContext.Current, id);
			model.MapFromModel();

			var eventSink = new EventSink()
			{
				MethodName = "Contacto_Delete",
				ViewName = "Contacto",
				AreaName = "contc",
				Location = ACTION_CONTACTO_DELETE,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_DESTROY_DELETE CONTACTO]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_DESTROY_DELETE CONTACTO]/
				}
			};

			return GenericHandlePostFormDelete(eventSink, model);
		}

		public ActionResult Contacto_Delete_Redirect()
		{
			//FOR: FORM MENU GO BACK
			return RedirectToFormMenuGoBack("CONTACTO");
		}

		#endregion

		#region Contacto_Duplicate

// USE /[MANUAL PRO CONTROLLER_DUPLICATE_GET CONTACTO]/

		[HttpPost]
		public ActionResult Contacto_Duplicate_GET([FromBody]RequestNewGetModel requestModel)
		{
			var id = requestModel.Id;
			var isNewLocation = requestModel.IsNewLocation;

			var model = new Contacto_ViewModel(UserContext.Current);
			var eventSink = new EventSink()
			{
				MethodName = "Contacto_Duplicate_GET",
				AreaName = "contc",
				FormName = "CONTACTO",
				Location = ACTION_CONTACTO_DUPLICATE,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_LOAD_DUPLICATE CONTACTO]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_DUPLICATE CONTACTO]/
				}
			};

			return GenericHandleGetFormDuplicate(eventSink, model, id, isNewLocation);
		}

		//
		// POST: /Contc/Contacto_Duplicate
// USE /[MANUAL PRO CONTROLLER_DUPLICATE_POST CONTACTO]/
		[HttpPost]
		public ActionResult Contacto_Duplicate([FromBody]Contacto_ViewModel model, [FromQuery]bool redirect = true)
		{
			var eventSink = new EventSink()
			{
				MethodName = "Contacto_Duplicate",
				ViewName = "Contacto",
				AreaName = "contc",
				Location = ACTION_CONTACTO_DUPLICATE,
				Redirect = redirect,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_SAVE_DUPLICATE CONTACTO]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_SAVE_DUPLICATE CONTACTO]/
				},
				BeforeException = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_LOAD_DUPLICATE_EX CONTACTO]/
				},
				AfterException = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_DUPLICATE_EX CONTACTO]/
				}
			};

			return GenericHandlePostFormDuplicate(eventSink, model);
		}

		#endregion

		#region Contacto_Cancel

		//
		// GET: /Contc/Contacto_Cancel
// USE /[MANUAL PRO CONTROLLER_CANCEL_GET CONTACTO]/
		public ActionResult Contacto_Cancel()
		{
			if (Navigation.CurrentLevel.FormMode == FormMode.New || Navigation.CurrentLevel.FormMode == FormMode.Duplicate)
			{
				PersistentSupport sp = UserContext.Current.PersistentSupport;
				try
				{
					var model = new GenioMVC.Models.Contc(UserContext.Current);
					model.klass.QPrimaryKey = Navigation.GetStrValue("contc");

// USE /[MANUAL PRO BEFORE_CANCEL CONTACTO]/

					sp.openTransaction();
					model.Destroy();
					sp.closeTransaction();

// USE /[MANUAL PRO AFTER_CANCEL CONTACTO]/

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

				Navigation.SetValue("ForcePrimaryRead_contc", "true", true);
			}

			Navigation.ClearValue("contc");

			return JsonOK(new { Success = true, currentNavigationLevel = Navigation.CurrentLevel.Level });
		}

		#endregion


		//
		// GET: /Contc/Contacto_ProprValTitulo
		// POST: /Contc/Contacto_ProprValTitulo
		[ActionName("Contacto_ProprValTitulo")]
		public ActionResult Contacto_ProprValTitulo([FromBody]RequestLookupModel requestModel)
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

			IsStateReadonly = true;
			Contacto_ProprValTitulo_ViewModel model = new Contacto_ProprValTitulo_ViewModel(UserContext.Current);

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


		// POST: /Contc/Contacto_SaveEdit
		[HttpPost]
		public ActionResult Contacto_SaveEdit([FromBody]Contacto_ViewModel model)
		{
			var eventSink = new EventSink()
			{
				MethodName = "Contacto_SaveEdit",
				ViewName = "Contacto",
				AreaName = "contc",
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_APPLY_EDIT CONTACTO]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_APPLY_EDIT CONTACTO]/
				}
			};

			return GenericHandlePostFormApply(eventSink, model);
		}
	}
}
