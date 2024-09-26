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
using GenioMVC.ViewModels.Agent;
using GenioServer.business;
using Quidgest.Persistence.GenericQuery;

// USE /[MANUAL PRO INCLUDE_CONTROLLER AGENT]/

namespace GenioMVC.Controllers
{
	public partial class AgentController : ControllerBase
	{
		public AgentController(UserContextService userContext): base(userContext) { }
// USE /[MANUAL PRO CONTROLLER_NAVIGATION AGENT]/



		private List<string> GetActionIds(CriteriaSet crs, CSGenio.persistence.PersistentSupport sp = null)
		{
			CSGenio.business.Area area = CSGenio.business.Area.createArea<CSGenioAagent>(UserContext.Current.User, UserContext.Current.User.CurrentModule);
			return base.GetActionIds(crs, sp, area);
		}

// USE /[MANUAL PRO MANUAL_CONTROLLER AGENT]/


		[HttpPost]
		public JsonResult ReloadDBEdit([FromBody]RequestReloadDBEditModel requestModel)
		{
			var Identifier = requestModel.Identifier ?? "";
			var qs = new NameValueCollection();
			qs.AddRange(Request.Query);
			// The value of the lookup search field comes in 'Values'
			if (requestModel.Values != null)
				qs.AddRange(requestModel.Values);
			this.IsStateReadonly = true;

			dynamic result = null;
			Models.Agent row = null;

			try
			{
				row = Models.Agent.Find(Navigation.GetStrValue("agent"), UserContext.Current);
			}
			catch (Exception)
			{
				CSGenio.framework.Log.Error("ReloadDBEdit - " + Identifier + " Not found Model agent");
			}

			if (row == null)
			{
				row = new Models.Agent(UserContext.Current);
				row.klass.QPrimaryKey = Navigation.GetStrValue("agent");
			}

			// Only the last reload request is accepted.
			var requestNumber = Request.Headers["ReloadDBEditRequestNumber"];
			if (requestNumber != StringValues.Empty)
				Response.Headers["ReloadDBEditRequestNumber"] = requestNumber.First();

			try
			{
				switch (string.IsNullOrEmpty(Identifier) ? "" : Identifier)
				{
					case "AGENTE__PMORAPAIS____":	// Field (DB)
						{
							row.LoadKeysFromHistory(Navigation, Navigation.CurrentLevel.Level, false, true, true, true);
							var model = new Agente_ViewModel(UserContext.Current) { editable = false };
							model.MapFromModel(row);
							model.Load_Agente__pmorapais____(qs);
							result = model.TablePmoraPais;
						}
						break;
					case "AGENTE__PNASCPAIS____":	// Field (DB)
						{
							row.LoadKeysFromHistory(Navigation, Navigation.CurrentLevel.Level, false, true, true, true);
							var model = new Agente_ViewModel(UserContext.Current) { editable = false };
							model.MapFromModel(row);
							model.Load_Agente__pnascpais____(qs);
							result = model.TablePnascPais;
						}
						break;
					default:
						break;
				}
			}
			catch (Exception)
			{
				return JsonERROR("On Reload form field: " + Identifier);
			}

			if (result != null)
				return JsonOK(new { List = result.List, TotalRows = result.Pagination.TotalRows, Selected = result.Selected, Value = result.Value });
			return JsonERROR("Not found any valid result");
		}

		[HttpPost]
		public JsonResult GetDependants([FromBody]RequestDependantsModel requestModel)
		{
			var Identifier = requestModel.Identifier;
			var Selected = requestModel.Selected;

			ConcurrentDictionary<string, object> values = null;
			this.IsStateReadonly = true;

			try
			{
				// Only the last reload request is accepted.
				var requestNumber = Request.Headers["GetDependantsRequestNumber"];
				if (requestNumber != StringValues.Empty)
					Response.Headers["GetDependantsRequestNumber"] = requestNumber.First();

				UserContext.Current.PersistentSupport.openConnection();
				switch (string.IsNullOrEmpty(Identifier) ? "" : Identifier)
				{
					case "AGENTE__PMORAPAIS____":	// Field (DB)
						values = new Agente_ViewModel(UserContext.Current).GetDependant_AgenteTablePmoraPais(Selected);
						break;
					case "AGENTE__PNASCPAIS____":	// Field (DB)
						values = new Agente_ViewModel(UserContext.Current).GetDependant_AgenteTablePnascPais(Selected);
						break;
					default: break;
				}

				if (values == null || !values.Any())
					return JsonERROR("List is empty");

				// Remove DateTime.MinValue
				foreach (KeyValuePair<string, object> field in values)
					if (field.Value is DateTime && (DateTime)field.Value == DateTime.MinValue)
						values.TryUpdate(field.Key, "", DateTime.MinValue);

				// TODO: Sanitize HTML content
				return JsonOK(values);
			}
			catch (Exception)
			{
				return JsonERROR("On Get Dependants - " + Identifier);
			}
			finally
			{
				UserContext.Current.PersistentSupport.closeConnection();
			}
		}


		/// <summary>
		/// Recalculate formulas of the "Agente" form. (++, CT, SR, CL and U1)
		/// </summary>
		/// <param name="formData">Current form data</param>
		/// <returns></returns>
		[HttpPost]
		public JsonResult RecalculateFormulas_Agente([FromBody]Agente_ViewModel formData)
		{
			return GenericRecalculateFormulas(formData, "agent",
				(primaryKey) => Models.Agent.Find(primaryKey, UserContext.Current, "FAGENTE"),
				(model) => formData.MapToModel(model as Models.Agent)
			);
		}

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
	}
}
