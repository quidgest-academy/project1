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
using GenioMVC.ViewModels.Album;
using Quidgest.Persistence.GenericQuery;

// USE /[MANUAL PRO INCLUDE_CONTROLLER ALBUM]/

namespace GenioMVC.Controllers
{
	public partial class AlbumController : ControllerBase
	{
		#region NavigationLocation Names

		private static readonly NavigationLocation ACTION_FOTOS_CANCEL = new("FOTOGRAFIA36807", "Fotos_Cancel", "Album") { vueRouteName = "form-FOTOS", mode = "CANCEL" };
		private static readonly NavigationLocation ACTION_FOTOS_SHOW = new("FOTOGRAFIA36807", "Fotos_Show", "Album") { vueRouteName = "form-FOTOS", mode = "SHOW" };
		private static readonly NavigationLocation ACTION_FOTOS_NEW = new("FOTOGRAFIA36807", "Fotos_New", "Album") { vueRouteName = "form-FOTOS", mode = "NEW" };
		private static readonly NavigationLocation ACTION_FOTOS_EDIT = new("FOTOGRAFIA36807", "Fotos_Edit", "Album") { vueRouteName = "form-FOTOS", mode = "EDIT" };
		private static readonly NavigationLocation ACTION_FOTOS_DUPLICATE = new("FOTOGRAFIA36807", "Fotos_Duplicate", "Album") { vueRouteName = "form-FOTOS", mode = "DUPLICATE" };
		private static readonly NavigationLocation ACTION_FOTOS_DELETE = new("FOTOGRAFIA36807", "Fotos_Delete", "Album") { vueRouteName = "form-FOTOS", mode = "DELETE" };

		#endregion

		#region Fotos private

		private void FormHistoryLimits_Fotos()
		{

		}

		#endregion

		#region Fotos_Show

// USE /[MANUAL PRO CONTROLLER_SHOW FOTOS]/

		[HttpPost]
		public ActionResult Fotos_Show_GET([FromBody]RequestIdModel requestModel)
		{
			var id = requestModel.Id;
			var model = new Fotos_ViewModel(UserContext.Current);
			var eventSink = new EventSink()
			{
				MethodName = "Fotos_Show_GET",
				AreaName = "album",
				Location = ACTION_FOTOS_SHOW,
				BeforeOp = (sink, sp) =>
				{
					FormHistoryLimits_Fotos();
// USE /[MANUAL PRO BEFORE_LOAD_SHOW FOTOS]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_SHOW FOTOS]/
				}
			};

			return GenericHandleGetFormShow(eventSink, model, id);
		}

		#endregion

		#region Fotos_New

// USE /[MANUAL PRO CONTROLLER_NEW_GET FOTOS]/
		[HttpPost]
		public ActionResult Fotos_New_GET([FromBody]RequestNewGetModel requestModel)
		{
			var id = requestModel.Id;
			var isNewLocation = requestModel.IsNewLocation;
			var prefillValues = requestModel.PrefillValues;

			var model = new Fotos_ViewModel(UserContext.Current);
			var eventSink = new EventSink()
			{
				MethodName = "Fotos_New_GET",
				AreaName = "album",
				FormName = "FOTOS",
				Location = ACTION_FOTOS_NEW,
				BeforeAll = (sink, sp) =>
				{
					FormHistoryLimits_Fotos();
				},
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_LOAD_NEW FOTOS]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_NEW FOTOS]/
				}
			};

			return GenericHandleGetFormNew(eventSink, model, id, isNewLocation, prefillValues);
		}

		//
		// POST: /Album/Fotos_New
// USE /[MANUAL PRO CONTROLLER_NEW_POST FOTOS]/
		[HttpPost]
		public ActionResult Fotos_New([FromBody]Fotos_ViewModel model, [FromQuery]bool redirect = true)
		{
			var eventSink = new EventSink()
			{
				MethodName = "Fotos_New",
				ViewName = "Fotos",
				AreaName = "album",
				Location = ACTION_FOTOS_NEW,
				Redirect = redirect,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_SAVE_NEW FOTOS]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_SAVE_NEW FOTOS]/
				},
				BeforeException = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_LOAD_NEW_EX FOTOS]/
				},
				AfterException = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_NEW_EX FOTOS]/
				}
			};

			return GenericHandlePostFormNew(eventSink, model);
		}

		#endregion

		#region Fotos_Edit

// USE /[MANUAL PRO CONTROLLER_EDIT_GET FOTOS]/
		[HttpPost]
		public ActionResult Fotos_Edit_GET([FromBody]RequestIdModel requestModel)
		{
			var id = requestModel.Id;
			var model = new Fotos_ViewModel(UserContext.Current);
			var eventSink = new EventSink()
			{
				MethodName = "Fotos_Edit_GET",
				AreaName = "album",
				FormName = "FOTOS",
				Location = ACTION_FOTOS_EDIT,
				BeforeOp = (sink, sp) =>
				{
					FormHistoryLimits_Fotos();
// USE /[MANUAL PRO BEFORE_LOAD_EDIT FOTOS]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_EDIT FOTOS]/
				}
			};

			return GenericHandleGetFormEdit(eventSink, model, id);
		}

		//
		// POST: /Album/Fotos_Edit
// USE /[MANUAL PRO CONTROLLER_EDIT_POST FOTOS]/
		[HttpPost]
		public ActionResult Fotos_Edit([FromBody]Fotos_ViewModel model, [FromQuery]bool redirect)
		{
			var eventSink = new EventSink()
			{
				MethodName = "Fotos_Edit",
				ViewName = "Fotos",
				AreaName = "album",
				Location = ACTION_FOTOS_EDIT,
				Redirect = redirect,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_SAVE_EDIT FOTOS]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_SAVE_EDIT FOTOS]/
				},
				BeforeException = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_LOAD_EDIT_EX FOTOS]/
				},
				AfterException = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_EDIT_EX FOTOS]/
				}
			};

			return GenericHandlePostFormEdit(eventSink, model);
		}

		#endregion

		#region Fotos_Delete

// USE /[MANUAL PRO CONTROLLER_DELETE_GET FOTOS]/
		[HttpPost]
		public ActionResult Fotos_Delete_GET([FromBody]RequestIdModel requestModel)
		{
			var id = requestModel.Id;
			var model = new Fotos_ViewModel(UserContext.Current);
			var eventSink = new EventSink()
			{
				MethodName = "Fotos_Delete_GET",
				AreaName = "album",
				FormName = "FOTOS",
				Location = ACTION_FOTOS_DELETE,
				BeforeOp = (sink, sp) =>
				{
					FormHistoryLimits_Fotos();
// USE /[MANUAL PRO BEFORE_LOAD_DELETE FOTOS]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_DELETE FOTOS]/
				}
			};

			return GenericHandleGetFormDelete(eventSink, model, id);
		}

		//
		// POST: /Album/Fotos_Delete
// USE /[MANUAL PRO CONTROLLER_DELETE_POST FOTOS]/
		[HttpPost]
		public ActionResult Fotos_Delete([FromBody]RequestIdModel requestModel)
		{
			var id = requestModel.Id;
			var model = new Fotos_ViewModel (UserContext.Current, id);
			model.MapFromModel();

			var eventSink = new EventSink()
			{
				MethodName = "Fotos_Delete",
				ViewName = "Fotos",
				AreaName = "album",
				Location = ACTION_FOTOS_DELETE,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_DESTROY_DELETE FOTOS]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_DESTROY_DELETE FOTOS]/
				}
			};

			return GenericHandlePostFormDelete(eventSink, model);
		}

		public ActionResult Fotos_Delete_Redirect()
		{
			//FOR: FORM MENU GO BACK
			return RedirectToFormMenuGoBack("FOTOS");
		}

		#endregion

		#region Fotos_Duplicate

// USE /[MANUAL PRO CONTROLLER_DUPLICATE_GET FOTOS]/

		[HttpPost]
		public ActionResult Fotos_Duplicate_GET([FromBody]RequestNewGetModel requestModel)
		{
			var id = requestModel.Id;
			var isNewLocation = requestModel.IsNewLocation;

			var model = new Fotos_ViewModel(UserContext.Current);
			var eventSink = new EventSink()
			{
				MethodName = "Fotos_Duplicate_GET",
				AreaName = "album",
				FormName = "FOTOS",
				Location = ACTION_FOTOS_DUPLICATE,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_LOAD_DUPLICATE FOTOS]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_DUPLICATE FOTOS]/
				}
			};

			return GenericHandleGetFormDuplicate(eventSink, model, id, isNewLocation);
		}

		//
		// POST: /Album/Fotos_Duplicate
// USE /[MANUAL PRO CONTROLLER_DUPLICATE_POST FOTOS]/
		[HttpPost]
		public ActionResult Fotos_Duplicate([FromBody]Fotos_ViewModel model, [FromQuery]bool redirect = true)
		{
			var eventSink = new EventSink()
			{
				MethodName = "Fotos_Duplicate",
				ViewName = "Fotos",
				AreaName = "album",
				Location = ACTION_FOTOS_DUPLICATE,
				Redirect = redirect,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_SAVE_DUPLICATE FOTOS]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_SAVE_DUPLICATE FOTOS]/
				},
				BeforeException = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_LOAD_DUPLICATE_EX FOTOS]/
				},
				AfterException = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_DUPLICATE_EX FOTOS]/
				}
			};

			return GenericHandlePostFormDuplicate(eventSink, model);
		}

		#endregion

		#region Fotos_Cancel

		//
		// GET: /Album/Fotos_Cancel
// USE /[MANUAL PRO CONTROLLER_CANCEL_GET FOTOS]/
		public ActionResult Fotos_Cancel()
		{
			if (Navigation.CurrentLevel.FormMode == FormMode.New || Navigation.CurrentLevel.FormMode == FormMode.Duplicate)
			{
				PersistentSupport sp = UserContext.Current.PersistentSupport;
				try
				{
					var model = new GenioMVC.Models.Album(UserContext.Current);
					model.klass.QPrimaryKey = Navigation.GetStrValue("album");

// USE /[MANUAL PRO BEFORE_CANCEL FOTOS]/

					sp.openTransaction();
					model.Destroy();
					sp.closeTransaction();

// USE /[MANUAL PRO AFTER_CANCEL FOTOS]/

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

				Navigation.SetValue("ForcePrimaryRead_album", "true", true);
			}

			Navigation.ClearValue("album");

			return JsonOK(new { Success = true, currentNavigationLevel = Navigation.CurrentLevel.Level });
		}

		#endregion


		//
		// GET: /Album/Fotos_ProprValTitulo
		// POST: /Album/Fotos_ProprValTitulo
		[ActionName("Fotos_ProprValTitulo")]
		public ActionResult Fotos_ProprValTitulo([FromBody]RequestLookupModel requestModel)
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
			Fotos_ProprValTitulo_ViewModel model = new Fotos_ProprValTitulo_ViewModel(UserContext.Current);

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


		// POST: /Album/Fotos_SaveEdit
		[HttpPost]
		public ActionResult Fotos_SaveEdit([FromBody]Fotos_ViewModel model)
		{
			var eventSink = new EventSink()
			{
				MethodName = "Fotos_SaveEdit",
				ViewName = "Fotos",
				AreaName = "album",
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_APPLY_EDIT FOTOS]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_APPLY_EDIT FOTOS]/
				}
			};

			return GenericHandlePostFormApply(eventSink, model);
		}
	}
}
