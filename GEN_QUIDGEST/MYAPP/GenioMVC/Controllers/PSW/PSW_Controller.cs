using JsonPropertyName = System.Text.Json.Serialization.JsonPropertyNameAttribute;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Primitives;
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
using GenioMVC.Helpers;
using GenioMVC.Models;
using GenioMVC.Models.Exception;
using GenioMVC.Models.Navigation;
using GenioMVC.Resources;
using GenioMVC.ViewModels;
using GenioMVC.ViewModels.Psw;
using GenioServer.business;
using Quidgest.Persistence.GenericQuery;

// USE /[MANUAL PRO INCLUDE_CONTROLLER PSW]/

namespace GenioMVC.Controllers
{
	public partial class PswController : ControllerBase
	{
		public PswController(UserContextService userContext): base(userContext) { }
// USE /[MANUAL PRO CONTROLLER_NAVIGATION PSW]/



		private List<string> GetActionIds(CriteriaSet crs, CSGenio.persistence.PersistentSupport sp = null)
		{
			CSGenio.business.Area area = CSGenio.business.Area.createArea<CSGenioApsw>(UserContext.Current.User, UserContext.Current.User.CurrentModule);
			return base.GetActionIds(crs, sp, area);
		}

// USE /[MANUAL PRO MANUAL_CONTROLLER PSW]/


		/// <summary>
		/// Get "See more..." tree structure
		/// </summary>
		/// <returns></returns>
		public JsonResult GetTreeSeeMore([FromBody]RequestLookupModel requestModel)
		{
			var Identifier = requestModel.Identifier;
			var queryParams = requestModel.QueryParams;

			try
			{
				// We need the request values to apply filters
				var requestValues = new NameValueCollection();
				if (queryParams != null)
					foreach (var kv in queryParams)
						requestValues.Add(kv.Key, kv.Value);

				switch (string.IsNullOrEmpty(Identifier) ? "" : Identifier)
				{
					default:
						break;
				}
			}
			catch (Exception)
			{
				return Json(new { Success = false, Message = "Error" });
			}

			return Json(new { Success = false, Message = "Error" });
		}

		#region PSW Controller

		private static readonly NavigationLocation ACTION_USER_SHOW = new NavigationLocation("CONSULTA40695", "User_Show", "Psw") { vueRouteName = "form-USER", mode = "SHOW" };
		private static readonly NavigationLocation ACTION_USER_NEW = new NavigationLocation("INSERIR43365", "User_New", "Psw") { vueRouteName = "form-USER", mode = "NEW" };
		private static readonly NavigationLocation ACTION_USER_EDIT = new NavigationLocation("EDITAR11616", "User_Edit", "Psw") { vueRouteName = "form-USER", mode = "EDIT" };
		private static readonly NavigationLocation ACTION_USER_DELETE = new NavigationLocation("APAGAR04097", "User_Delete", "Psw") { vueRouteName = "form-USER", mode = "DELETE" };

		public bool NestedForm { get; set; }

		#region User_Show

		//
		// /Psw/User_new
		public ActionResult User_Show([FromBody]RequestIdModel requestModel)
		{
			var id = requestModel.Id;
			var qs = Request.Query;
			var nestedForm = qs["nestedForm"] == "true";
			var model = new PswViewModel(UserContext.Current, null, nestedForm);

			CSGenio.framework.StatusMessage permission = model.CheckPermissions(FormMode.Show);
			if (permission.Status.Equals(CSGenio.framework.Status.E))
				return PermissionError(permission.Message);

			var navigationLocationAction = ACTION_USER_SHOW.SetRoutedValues(new { Id = id });

			Navigation.SetValue("psw", id);

			try
			{
				var queryValues = new NameValueCollection();
				queryValues.AddRange(Request.Query);
				model.Load(queryValues, false, Request.IsAjaxRequest());
			}
			catch (ModelNotFoundException)
			{
				return NotFoundError(Resources.Resources.O_REGISTO_PEDIDO_NAO63869);
			}

			return JsonOK(model);
		}

		#endregion

		#region User_New

		//
		// GET: /Psw/User_new
		[ActionName("User_New")]
		[HttpGet]
		public ActionResult User_New()
		{
			var qs = Request.Query;
			var nestedForm = qs["nestedForm"] == "true";
			var model = new PswViewModel(UserContext.Current, null, nestedForm);
			CSGenio.framework.StatusMessage permission = model.CheckPermissions(FormMode.New);

			if (permission.Status.Equals(CSGenio.framework.Status.E))
				return PermissionError(permission.Message);

			var navigationLocationAction = ACTION_USER_NEW;

			PersistentSupport sp = UserContext.Current.PersistentSupport;
			try
			{
				if (IsNewLocation(navigationLocationAction))
				{
					sp.openTransaction();
					model.New();
					sp.closeTransaction();

					Navigation.SetValue("psw", model.ValCodpsw);

					sp.openConnection();
					model.NewLoad();
					sp.closeConnection();
				}
				else
				{
					try
					{
						var queryValues = new NameValueCollection();
						queryValues.AddRange(Request.Query);
						model.Load(queryValues, true, Request.IsAjaxRequest());
					}
					catch (ModelNotFoundException)
					{
						return NotFoundError(Resources.Resources.O_REGISTO_PEDIDO_NAO63869);
					}
				}
			}
			catch (Exception e)
			{
				sp.rollbackTransaction();
				sp.closeConnection();

				var exceptionUserMessage = Resources.Resources.PEDIMOS_DESCULPA__OC63848;
				if (e is GenioException && (e as GenioException).UserMessage != null)
					exceptionUserMessage = Translations.Get((e as GenioException).UserMessage, UserContext.Current.User.Language);

				ModelState.AddModelError("Erro", exceptionUserMessage);
				ErrorMessage(exceptionUserMessage);
				CSGenio.framework.Log.Error("User_New - GET " + e.Message);

				return RedirectToLocation(Navigation.CurrentLevel.Location);
			}

			return JsonOK(model);
		}

		[HttpPost]
		public ActionResult User_New_GET()
		{
			return User_New();
		}

		// POST: /Psw/User_New
		[HttpPost]
		public ActionResult User_New([FromBody]PswViewModel model)
		{
			long st = DateTime.Now.Ticks;
			CSGenio.framework.StatusMessage permission = model.CheckPermissions(FormMode.New);
			if (permission.Status.Equals(CSGenio.framework.Status.E))
				return PermissionError(permission.Message);

			var qs = Request.Form;
			if (Request.IsAjaxRequest() && qs.ContainsKey("partialView"))
				return JsonOK(model);

			var sp = UserContext.Current.PersistentSupport;
			try
			{
				// Removes password validation if it had not changed
				if (String.IsNullOrEmpty(model.ValPassword))
					ModelState.Remove("ValPassword");

				ValidateModel(model);

				if (!ModelState.IsValid)
					throw new BusinessException(Resources.Resources.NAO_E_POSSIVEL_GRAVA23775, "User_New", "Erro");
				
				// Validate se user já exists
				CheckUserExist(model.ValNome);

				// Create a new user
				var userFactory = new GenioServer.security.UserFactory(sp, UserContext.Current.User);
				var password = new GenioServer.security.Password(model.ValPassword, model.ValConfirmPassword);
				Psw userPsw = Psw.Find(model.ValCodpsw, UserContext.Current);
				userFactory.FillPsw(userPsw.klass, model.ValNome, model.ValEmail, phone:"", status: 0,password: password);

				sp.openTransaction();
				userPsw.Save(sp);
				model.SaveAuthorization();
				sp.closeTransaction();

				if (!Request.IsAjaxRequest())
					GetFlashMessage(model.flashMessage, Navigation.CurrentLevel.FormMode);

				// New insertion in upper table
				// MH (13/10/2017) - Deixou de ser preciso Request.IsAjaxRequest() pq os formularios passam a fazer pedidos ajax nos submits dos formulários
				if (Navigation.PreviousLevel != null && Navigation.PreviousLevel.FormMode != FormMode.List)
					Navigation.SetValue("RETURN_psw", Navigation.GetValue("psw"), true);
			}
			catch (Exception e)
			{
				sp.rollbackTransaction();
				var queryValues = new NameValueCollection();
				queryValues.AddRange(Request.Query);
				model.LoadPartial(queryValues);

				if (e is GenioException && (e as GenioException).UserMessage != null)
					ModelState.AddModelError("Erro", (e as GenioException).UserMessage);
				else
					ModelState.AddModelError("Erro", Resources.Resources.PEDIMOS_DESCULPA__OC63848);
				CSGenio.framework.Log.Error(e.Message);
				model.NestedForm = Request.IsAjaxRequest();

				if (Request.IsAjaxRequest())
					return Json(new { Success = false, Message = Resources.Resources.ERRO_AO_GUARDAR_O_RE65182 });

				return JsonOK(model);
			}

			if (CSGenio.framework.Log.IsDebugEnabled)
				CSGenio.framework.Log.Debug("Controller success " + (DateTime.Now.Ticks - st) / TimeSpan.TicksPerMillisecond + "ms");

			if (Request.IsAjaxRequest())
				return JsonOK(new { Success = true, Operation = "New", Message = Resources.Resources.REGISTO_CRIADO_COM_S18746 });

			return RedirectToLocation(Navigation.CurrentLevel.Location);
		}

		#endregion

		#region User_Edit

		//
		// GET: /Psw/User_Edit
		[HttpGet]
		[ActionName("User_Edit")]
		public ActionResult User_Edit([FromBody]RequestIdModel requestModel)
		{
			var id = requestModel.Id;
			var qs = Request.Query;
			var nestedForm = qs["nestedForm"] == "true";
			string partialView = qs["partialView"].FirstOrDefault("User");

			var navigationLocationAction = ACTION_USER_EDIT.SetRoutedValues(new { Id = id });

			Navigation.SetValue("psw", id);

			var model = new PswViewModel(UserContext.Current, null, nestedForm);

			try
			{
				var queryValues = new NameValueCollection();
				queryValues.AddRange(Request.Query);
				model.Load(queryValues, true, Request.IsAjaxRequest());
			}
			catch (ModelNotFoundException)
			{
				return NotFoundError(Resources.Resources.O_REGISTO_PEDIDO_NAO63869);
			}

			CSGenio.framework.StatusMessage permission = model.CheckPermissions(FormMode.Edit);

			if (permission.Status.Equals(CSGenio.framework.Status.E))
				return PermissionError(permission.Message);
			return JsonOK(model);
		}

		[HttpPost]
		public ActionResult User_Edit_GET([FromBody]RequestIdModel requestModel)
		{
			return User_Edit(requestModel);
		}

		// POST: /Psw/User_Edit
		[HttpPost]
		public ActionResult User_Edit([FromBody]PswViewModel model)
		{
			long st = DateTime.Now.Ticks;
			CSGenio.framework.StatusMessage permission = model.CheckPermissions(FormMode.New);
			if (permission.Status.Equals(CSGenio.framework.Status.E))
				return PermissionError(permission.Message);

			var sp = UserContext.Current.PersistentSupport;
			try
			{
				ValidateModel(model);

				if (!ModelState.IsValid)
					throw new BusinessException(Resources.Resources.NAO_E_POSSIVEL_GRAVA23775, "User_Edit", "Erro");
				
				// Validate se user já exists
				CheckUserExist(model.ValNome, model.ValCodpsw);

				Psw psw = Psw.Find(model.ValCodpsw, UserContext.Current);
				var userFactory = new GenioServer.security.UserFactory(UserContext.Current.PersistentSupport, UserContext.Current.User);
				if (!string.IsNullOrEmpty(model.ValPassword))
					userFactory.ChangePassword(psw.klass, model.ValPassword, model.ValConfirmPassword);

				psw.ValNome = model.ValNome;
				psw.ValEmail = model.ValEmail;

				sp.openTransaction();
				model.flashMessage = psw.Save(sp);
				model.SaveAuthorization();
				sp.closeTransaction();

				if (!Request.IsAjaxRequest())
					GetFlashMessage(model.flashMessage, Navigation.CurrentLevel.FormMode);

				// New insertion in upper table
				if (Navigation.PreviousLevel != null && Navigation.PreviousLevel.FormMode != FormMode.List)
					Navigation.SetValue("RETURN_psw", Navigation.GetValue("psw"), true);
			}
			catch (Exception e)
			{
				sp.rollbackTransaction();
				var queryValues = new NameValueCollection();
				queryValues.AddRange(Request.Query);
				model.LoadPartial(queryValues);

				if (e is GenioException && (e as GenioException).UserMessage != null)
					ModelState.AddModelError("Erro", (e as GenioException).UserMessage);
				else
					ModelState.AddModelError("Erro", Resources.Resources.PEDIMOS_DESCULPA__OC63848);
				CSGenio.framework.Log.Error(e.Message);
				model.NestedForm = Request.IsAjaxRequest();

				if (Request.IsAjaxRequest())
					return Json(new { Success = false, Message = Resources.Resources.ERRO_AO_GUARDAR_O_RE65182 });

				return JsonOK(model);
			}

			if (CSGenio.framework.Log.IsDebugEnabled)
				CSGenio.framework.Log.Debug("Controller success " + (DateTime.Now.Ticks - st) / TimeSpan.TicksPerMillisecond + "ms");
			if (Request.IsAjaxRequest())
				return Json(new { Success = true, Operation = "Edit", Message = Resources.Resources.ALTERACOES_EFETUADAS10166 });

			return RedirectToLocation(Navigation.CurrentLevel.Location);
		}

		#endregion

		#region User_Delete

		//
		// GET: /Psw/User_Delete
		[HttpGet]
		[ActionName("User_Delete")]
		public ActionResult User_Delete([FromBody]RequestIdModel requestModel)
		{
			var id = requestModel.Id;
			var qs = Request.Query;
			var nestedForm = qs["nestedForm"] == "true";
			string partialView = qs["partialView"].FirstOrDefault("User");

			var navigationLocationAction = ACTION_USER_DELETE.SetRoutedValues(new { Id = id });

			if (IsNewLocation(navigationLocationAction))
				Navigation.SetValue("psw", id);

			var model = new PswViewModel(UserContext.Current);

			try
			{
				var queryValues = new NameValueCollection();
				queryValues.AddRange(Request.Query);
				model.Load(queryValues, false, Request.IsAjaxRequest());
			}
			catch (ModelNotFoundException)
			{
				return NotFoundError(Resources.Resources.O_REGISTO_PEDIDO_NAO63869);
			}

			CSGenio.framework.StatusMessage permission = model.CheckPermissions(FormMode.Delete);

			if (permission.Status.Equals(CSGenio.framework.Status.E))
				return PermissionError(permission.Message);
			return JsonOK(model);
		}

		[HttpPost]
		public ActionResult User_Delete_GET([FromBody]RequestIdModel requestModel)
		{
			return User_Delete(requestModel);
		}

		//
		// POST: /Psw/User_Delete
		[HttpPost]
		public ActionResult User_Delete([FromBody]PswViewModel model)
		{
			string id = model.ValCodpsw;
			model = new PswViewModel(UserContext.Current, id, false);
			model.MapFromModel();

			long st = DateTime.Now.Ticks;
			CSGenio.framework.StatusMessage permission = model.CheckPermissions(FormMode.New);
			if (permission.Status.Equals(CSGenio.framework.Status.E))
				return PermissionError(permission.Message);

			var sp = UserContext.Current.PersistentSupport;

			try
			{
				sp.openTransaction();
				model.Destroy();
				sp.closeTransaction();

				if (!Request.IsAjaxRequest())
					GetFlashMessage(model.flashMessage, Navigation.CurrentLevel.FormMode);
			}
			catch (Exception e)
			{
				sp.rollbackTransaction();
				sp.closeConnection();

				var queryValues = new NameValueCollection();
				queryValues.AddRange(Request.Query);
				model.LoadPartial(queryValues);

				ModelState.AddModelError(string.Empty, CSGenio.framework.Translations.Get(e.Message, UserContext.Current.User.Language));
				CSGenio.framework.Log.Error(e.Message);
				if (Request.IsAjaxRequest())
					return Json(new { Success = false, Operation = "Delete", Message = Resources.Resources.ERRO_AO_APAGAR_O_REG38939 });

				return JsonOK(model);
			}

			if (CSGenio.framework.Log.IsDebugEnabled)
				CSGenio.framework.Log.Debug("Controller success " + (DateTime.Now.Ticks - st) / TimeSpan.TicksPerMillisecond + "ms");

			if (Request.IsAjaxRequest())
				return Json(new { Success = true, Operation = "Delete", Message = Resources.Resources.REGISTO_APAGADO_COM_64671 });

			return RedirectToLocation(Navigation.CurrentLevel.Location);
		}

		#endregion

		#region User_Cancel

		//
		// GET: /Psw/User_Cancel
		public ActionResult User_Cancel()
		{
			if ((Navigation.CurrentLevel.FormMode == FormMode.New || Navigation.CurrentLevel.FormMode == FormMode.Duplicate) && !Request.IsAjaxRequest())
			{
				PersistentSupport sp = UserContext.Current.PersistentSupport;

				try
				{
					var model = new GenioMVC.Models.Psw(UserContext.Current);
					model.klass.QPrimaryKey = Navigation.GetStrValue("psw");

					sp.openTransaction();
					model.Destroy();
					sp.closeTransaction();
				}
				catch (Exception e)
				{
					sp.rollbackTransaction();
					sp.closeConnection();
					ClearMessages();

					ErrorMessage(CSGenio.framework.Translations.Get(e.Message, UserContext.Current.User.Language));
					return RedirectToLocation(Navigation.CurrentLevel.Location);
				}
			}

			Navigation.ClearValue("psw");

			return RedirectToLocation(Navigation.CurrentLevel.Location);
		}

		#endregion

		private void CheckUserExist(string Username, string CodUser = null)
		{
			//verificar se já exists um user com o mesmo name
			SelectQuery userQuery = new SelectQuery()
				.Select(CSGenioApsw.FldCodpsw)
				.From("USERLOGIN", "psw")
				.PageSize(1);

			CriteriaSet where = new CriteriaSet(CriteriaSetOperator.And);
			where.Equal(CSGenioApsw.FldNome, Username);
			where.Equal(CSGenioApsw.FldZzstate, 0);

			if (!string.IsNullOrEmpty(CodUser)) //!= de introduce
				where.NotEqual(CSGenioApsw.FldCodpsw, CodUser);

			userQuery.Where(where);
			var userExist = UserContext.Current.PersistentSupport.ExecuteScalar(userQuery);

			if (userExist != null)
			{
				//replace de %s to o format do c#
				var regex = new System.Text.RegularExpressions.Regex(System.Text.RegularExpressions.Regex.Escape("%s"));
				var msg = regex.Replace(Resources.Resources.A_FICHA_COM_O_VALOR_35649, "{0}", 1);
				msg = regex.Replace(msg, "{1}", 1);

				throw new BusinessException(String.Format(msg, Username, Resources.Resources.UTILIZADOR52387), "CheckUserExist", "Erro");
			}
		}

		#endregion
	}
}
