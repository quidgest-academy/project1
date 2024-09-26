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
using GenioMVC.ViewModels.Pais;
using Quidgest.Persistence.GenericQuery;

// USE /[MANUAL PRO INCLUDE_CONTROLLER PAIS]/

namespace GenioMVC.Controllers
{
	public partial class PaisController : ControllerBase
	{
		#region NavigationLocation Names

		private static readonly NavigationLocation ACTION_PAIS_CANCEL = new("PAIS58483", "Pais_Cancel", "Pais") { vueRouteName = "form-PAIS", mode = "CANCEL" };
		private static readonly NavigationLocation ACTION_PAIS_SHOW = new("PAIS58483", "Pais_Show", "Pais") { vueRouteName = "form-PAIS", mode = "SHOW" };
		private static readonly NavigationLocation ACTION_PAIS_NEW = new("PAIS58483", "Pais_New", "Pais") { vueRouteName = "form-PAIS", mode = "NEW" };
		private static readonly NavigationLocation ACTION_PAIS_EDIT = new("PAIS58483", "Pais_Edit", "Pais") { vueRouteName = "form-PAIS", mode = "EDIT" };
		private static readonly NavigationLocation ACTION_PAIS_DUPLICATE = new("PAIS58483", "Pais_Duplicate", "Pais") { vueRouteName = "form-PAIS", mode = "DUPLICATE" };
		private static readonly NavigationLocation ACTION_PAIS_DELETE = new("PAIS58483", "Pais_Delete", "Pais") { vueRouteName = "form-PAIS", mode = "DELETE" };

		#endregion

		#region Pais private

		private void FormHistoryLimits_Pais()
		{

		}

		#endregion

		#region Pais_Show

// USE /[MANUAL PRO CONTROLLER_SHOW PAIS]/

		[HttpPost]
		public ActionResult Pais_Show_GET([FromBody]RequestIdModel requestModel)
		{
			var id = requestModel.Id;
			var model = new Pais_ViewModel(UserContext.Current);
			var eventSink = new EventSink()
			{
				MethodName = "Pais_Show_GET",
				AreaName = "pais",
				Location = ACTION_PAIS_SHOW,
				BeforeOp = (sink, sp) =>
				{
					FormHistoryLimits_Pais();
// USE /[MANUAL PRO BEFORE_LOAD_SHOW PAIS]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_SHOW PAIS]/
				}
			};

			return GenericHandleGetFormShow(eventSink, model, id);
		}

		#endregion

		#region Pais_New

// USE /[MANUAL PRO CONTROLLER_NEW_GET PAIS]/
		[HttpPost]
		public ActionResult Pais_New_GET([FromBody]RequestNewGetModel requestModel)
		{
			var id = requestModel.Id;
			var isNewLocation = requestModel.IsNewLocation;
			var prefillValues = requestModel.PrefillValues;

			var model = new Pais_ViewModel(UserContext.Current);
			var eventSink = new EventSink()
			{
				MethodName = "Pais_New_GET",
				AreaName = "pais",
				FormName = "PAIS",
				Location = ACTION_PAIS_NEW,
				BeforeAll = (sink, sp) =>
				{
					FormHistoryLimits_Pais();
				},
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_LOAD_NEW PAIS]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_NEW PAIS]/
				}
			};

			return GenericHandleGetFormNew(eventSink, model, id, isNewLocation, prefillValues);
		}

		//
		// POST: /Pais/Pais_New
// USE /[MANUAL PRO CONTROLLER_NEW_POST PAIS]/
		[HttpPost]
		public ActionResult Pais_New([FromBody]Pais_ViewModel model, [FromQuery]bool redirect = true)
		{
			var eventSink = new EventSink()
			{
				MethodName = "Pais_New",
				ViewName = "Pais",
				AreaName = "pais",
				Location = ACTION_PAIS_NEW,
				Redirect = redirect,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_SAVE_NEW PAIS]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_SAVE_NEW PAIS]/
				},
				BeforeException = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_LOAD_NEW_EX PAIS]/
				},
				AfterException = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_NEW_EX PAIS]/
				}
			};

			return GenericHandlePostFormNew(eventSink, model);
		}

		#endregion

		#region Pais_Edit

// USE /[MANUAL PRO CONTROLLER_EDIT_GET PAIS]/
		[HttpPost]
		public ActionResult Pais_Edit_GET([FromBody]RequestIdModel requestModel)
		{
			var id = requestModel.Id;
			var model = new Pais_ViewModel(UserContext.Current);
			var eventSink = new EventSink()
			{
				MethodName = "Pais_Edit_GET",
				AreaName = "pais",
				FormName = "PAIS",
				Location = ACTION_PAIS_EDIT,
				BeforeOp = (sink, sp) =>
				{
					FormHistoryLimits_Pais();
// USE /[MANUAL PRO BEFORE_LOAD_EDIT PAIS]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_EDIT PAIS]/
				}
			};

			return GenericHandleGetFormEdit(eventSink, model, id);
		}

		//
		// POST: /Pais/Pais_Edit
// USE /[MANUAL PRO CONTROLLER_EDIT_POST PAIS]/
		[HttpPost]
		public ActionResult Pais_Edit([FromBody]Pais_ViewModel model, [FromQuery]bool redirect)
		{
			var eventSink = new EventSink()
			{
				MethodName = "Pais_Edit",
				ViewName = "Pais",
				AreaName = "pais",
				Location = ACTION_PAIS_EDIT,
				Redirect = redirect,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_SAVE_EDIT PAIS]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_SAVE_EDIT PAIS]/
				},
				BeforeException = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_LOAD_EDIT_EX PAIS]/
				},
				AfterException = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_EDIT_EX PAIS]/
				}
			};

			return GenericHandlePostFormEdit(eventSink, model);
		}

		#endregion

		#region Pais_Delete

// USE /[MANUAL PRO CONTROLLER_DELETE_GET PAIS]/
		[HttpPost]
		public ActionResult Pais_Delete_GET([FromBody]RequestIdModel requestModel)
		{
			var id = requestModel.Id;
			var model = new Pais_ViewModel(UserContext.Current);
			var eventSink = new EventSink()
			{
				MethodName = "Pais_Delete_GET",
				AreaName = "pais",
				FormName = "PAIS",
				Location = ACTION_PAIS_DELETE,
				BeforeOp = (sink, sp) =>
				{
					FormHistoryLimits_Pais();
// USE /[MANUAL PRO BEFORE_LOAD_DELETE PAIS]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_DELETE PAIS]/
				}
			};

			return GenericHandleGetFormDelete(eventSink, model, id);
		}

		//
		// POST: /Pais/Pais_Delete
// USE /[MANUAL PRO CONTROLLER_DELETE_POST PAIS]/
		[HttpPost]
		public ActionResult Pais_Delete([FromBody]RequestIdModel requestModel)
		{
			var id = requestModel.Id;
			var model = new Pais_ViewModel (UserContext.Current, id);
			model.MapFromModel();

			var eventSink = new EventSink()
			{
				MethodName = "Pais_Delete",
				ViewName = "Pais",
				AreaName = "pais",
				Location = ACTION_PAIS_DELETE,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_DESTROY_DELETE PAIS]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_DESTROY_DELETE PAIS]/
				}
			};

			return GenericHandlePostFormDelete(eventSink, model);
		}

		public ActionResult Pais_Delete_Redirect()
		{
			//FOR: FORM MENU GO BACK
			return RedirectToFormMenuGoBack("PAIS");
		}

		#endregion

		#region Pais_Duplicate

// USE /[MANUAL PRO CONTROLLER_DUPLICATE_GET PAIS]/

		[HttpPost]
		public ActionResult Pais_Duplicate_GET([FromBody]RequestNewGetModel requestModel)
		{
			var id = requestModel.Id;
			var isNewLocation = requestModel.IsNewLocation;

			var model = new Pais_ViewModel(UserContext.Current);
			var eventSink = new EventSink()
			{
				MethodName = "Pais_Duplicate_GET",
				AreaName = "pais",
				FormName = "PAIS",
				Location = ACTION_PAIS_DUPLICATE,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_LOAD_DUPLICATE PAIS]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_DUPLICATE PAIS]/
				}
			};

			return GenericHandleGetFormDuplicate(eventSink, model, id, isNewLocation);
		}

		//
		// POST: /Pais/Pais_Duplicate
// USE /[MANUAL PRO CONTROLLER_DUPLICATE_POST PAIS]/
		[HttpPost]
		public ActionResult Pais_Duplicate([FromBody]Pais_ViewModel model, [FromQuery]bool redirect = true)
		{
			var eventSink = new EventSink()
			{
				MethodName = "Pais_Duplicate",
				ViewName = "Pais",
				AreaName = "pais",
				Location = ACTION_PAIS_DUPLICATE,
				Redirect = redirect,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_SAVE_DUPLICATE PAIS]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_SAVE_DUPLICATE PAIS]/
				},
				BeforeException = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_LOAD_DUPLICATE_EX PAIS]/
				},
				AfterException = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_LOAD_DUPLICATE_EX PAIS]/
				}
			};

			return GenericHandlePostFormDuplicate(eventSink, model);
		}

		#endregion

		#region Pais_Cancel

		//
		// GET: /Pais/Pais_Cancel
// USE /[MANUAL PRO CONTROLLER_CANCEL_GET PAIS]/
		public ActionResult Pais_Cancel()
		{
			if (Navigation.CurrentLevel.FormMode == FormMode.New || Navigation.CurrentLevel.FormMode == FormMode.Duplicate)
			{
				PersistentSupport sp = UserContext.Current.PersistentSupport;
				try
				{
					var model = new GenioMVC.Models.Pais(UserContext.Current);
					model.klass.QPrimaryKey = Navigation.GetStrValue("pais");

// USE /[MANUAL PRO BEFORE_CANCEL PAIS]/

					sp.openTransaction();
					model.Destroy();
					sp.closeTransaction();

// USE /[MANUAL PRO AFTER_CANCEL PAIS]/

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

				Navigation.SetValue("ForcePrimaryRead_pais", "true", true);
			}

			Navigation.ClearValue("pais");

			return JsonOK(new { Success = true, currentNavigationLevel = Navigation.CurrentLevel.Level });
		}

		#endregion



		// POST: /Pais/Pais_SaveEdit
		[HttpPost]
		public ActionResult Pais_SaveEdit([FromBody]Pais_ViewModel model)
		{
			var eventSink = new EventSink()
			{
				MethodName = "Pais_SaveEdit",
				ViewName = "Pais",
				AreaName = "pais",
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL PRO BEFORE_APPLY_EDIT PAIS]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL PRO AFTER_APPLY_EDIT PAIS]/
				}
			};

			return GenericHandlePostFormApply(eventSink, model);
		}
	}
}
