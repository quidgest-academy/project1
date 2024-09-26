using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

using CSGenio.business;
using CSGenio.framework;
using CSGenio.persistence;
using GenioMVC.Models;
using GenioMVC.Models.Exception;
using GenioMVC.Models.Navigation;
using Quidgest.Persistence.GenericQuery;

using SelectList = Microsoft.AspNetCore.Mvc.Rendering.SelectList;
using JsonIgnoreAttribute = System.Text.Json.Serialization.JsonIgnoreAttribute;

namespace GenioMVC.ViewModels.Psw
{
	public class PswViewModel : FormViewModel<Models.Psw>
	{
		[JsonIgnore]
		public override bool HasWriteConditions { get => false; }

		public string ValCodpsw { get; set; }

		public string ValNome { get; set; }

		public string ValPassword { get; set; }

		public string ValConfirmPassword { get; set; }

		public string ValSalt { get; set; }
		public decimal? ValStatus { get; set; }
		public string ValPswtype { get; set; }
		public DateTime? ValDatexp { get; set; }

		public string ValEmail { get; set; }

		private static List<string> modulos = new List<string>() { "PRO" };

		public IDictionary<decimal, string> ProLevels { get; private set; }
		public decimal? PROLevel { get; set; }
		public SelectList UserPRO { get; set; }

		public PswViewModel(UserContext userContext, string? identifier = null, bool nestedForm = false) : base(userContext, "FUSER", nestedForm) { }

		public PswViewModel(UserContext userContext, string identifier, GenioMVC.Models.Psw row, bool nestedForm = false) : base(userContext, "FUSER", row, nestedForm) { }

		public PswViewModel(UserContext userContext, string id, bool nestedForm = false, string[]? fieldsToLoad = null) : this(userContext, "FUSER", nestedForm)
		{
			this.Navigation.SetValue("psw", id);
			Model = Models.Psw.Find(id, userContext, "FUSER", fieldsToQuery: fieldsToLoad);
			if (Model == null)
				throw new ModelNotFoundException("Model not found");
			InitModel();
		}

		override protected void InitLevels()
		{
			//JGF 2021.11.18 Access to this form should be managed by the menu permission
			this.RoleToShow = CSGenio.framework.Role.UNAUTHORIZED;
			this.RoleToEdit = CSGenio.framework.Role.UNAUTHORIZED;
		}

		#region User Authorization List

		public List<GenioMVC.Models.S_ua> FillAuthorizationList()
		{
			CriteriaSet cond = CriteriaSet.And();
			cond.Equal(CSGenioApsw.FldCodpsw, ValCodpsw);
			cond.Equal(CSGenioAs_ua.FldSistema, "PRO");

			List<GenioMVC.Models.S_ua> lst = GenioMVC.Models.S_ua.AllModel(m_userContext, cond);
			//Create authorization lines for modules that are not in the database yet
			var naoExistentes = modulos.FindAll(m => !lst.Exists(db_m => db_m.ValModulo == m));
			foreach (var m in naoExistentes)
			{
				User u = m_userContext.User;
				PersistentSupport sp = m_userContext.PersistentSupport;
				// [RC] 06/06/2017 We need to catch any exception here, so we can roolback the transaction
				try
				{
					sp.openTransaction();

					GenioMVC.Models.S_ua new_rowuser = new GenioMVC.Models.S_ua(m_userContext);
					new_rowuser.klass = new CSGenioAs_ua(u, u.CurrentModule);
					new_rowuser.klass.ValCodpsw = ValCodpsw;
					new_rowuser.klass.ValModulo = m;
					new_rowuser.klass.ValSistema = "PRO";
					new_rowuser.klass.ValOpermuda = u.Name;
					new_rowuser.klass.ValDatamuda = DateTime.Now;
					new_rowuser.klass.ValNivel = 0;
					new_rowuser.klass.insertPseud(sp);

					sp.closeTransaction();
				}
				catch (Exception ex)
				{
					sp.rollbackTransaction();
					throw new FrameworkException("Não foi possível obter a lista de autorizações.", "PswViewModel.FillAuthorizationList", "Error filling authorization list: " + ex.Message, ex);
				}
			}

			// If we had to create authorization lines read everything back from the database
			if (naoExistentes.Count > 0)
				lst = Models.S_ua.AllModel(m_userContext, cond);

			return lst;
		}

		private List<GenioMVC.Models.S_ua> m_authorizationList;

		public List<GenioMVC.Models.S_ua> AuthorizationList
		{
			get
			{
				if (m_authorizationList == null)
					m_authorizationList = FillAuthorizationList();

				return m_authorizationList;
			}
			private set { m_authorizationList = value; }
		}

		#endregion

		public override void New()
		{
			base.New();
			InsertAuthorization();
		}

		public override void MapFromModel(Models.Psw m)
		{
			if (m == null)
			{
				CSGenio.framework.Log.Error("Map Model (PSW) to ViewModel (PSW) - Model is a null reference");
				throw new Models.Exception.ModelNotFoundException("Model not found");
			}
			try
			{
				ValCodpsw = m.ValCodpsw;
				ValNome = m.ValNome;
				ValSalt = m.ValSalt;
				ValStatus = m.ValStatus;
				ValPswtype = m.ValPswtype;
				ValEmail = m.ValEmail;
				ValDatexp = m.ValDatexp;
			}
			catch (Exception)
			{
				CSGenio.framework.Log.Error("Map Model (PSW) to ViewModel (PSW) - Error during mapping");
				throw;
			}
		}

		/// <summary>
		/// Performs the mapping of field values from the ViewModel to the Model.
		/// </summary>
		public override void MapToModel()
		{
			MapToModel(Model);
		}

		/// <summary>
		/// Performs the mapping of field values from the ViewModel to the Model.
		/// </summary>
		/// <param name="m">The Model to be filled.</param>
		/// <exception cref="ModelNotFoundException">Thrown if <paramref name="m"/> is null.</exception>
		public override void MapToModel(Models.Psw m)
		{
			if (m == null)
			{
				CSGenio.framework.Log.Error("Map ViewModel (PSW) to Model (PSW) - Model is a null reference");
				throw new Models.Exception.ModelNotFoundException("Model not found");
			}
			try
			{
				m.ValCodpsw = ValCodpsw;
				m.ValNome = ValNome;
				m.ValPasswordDecrypted = ValPassword;
				m.ValSalt = ValSalt;
				m.ValStatus = ValStatus;
				m.ValPswtype = ValPswtype;
				m.ValEmail = ValEmail;
				m.ValDatexp = ValDatexp;
			}
			catch (Exception)
			{
				CSGenio.framework.Log.Error($"Map ViewModel (PSW) to Model (PSW) - Error during mapping. All user values: {HasDisabledUserValuesSecurity}");
				throw;
			}
		}

		/// <summary>
		/// Sets the value of a single property of the view model based on the provided table and field names.
		/// </summary>
		/// <param name="fullFieldName">The full field name in the format "table.field".</param>
		/// <param name="value">The field value.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="fullFieldName"/> is null.</exception>
		public override void SetViewModelValue(string fullFieldName, object value)
		{
			try
			{
				ArgumentNullException.ThrowIfNull(fullFieldName);
				// Obtain a valid value from JsonValueKind that can come from "prefillValues" during the pre-filling of fields during insertion
				var _value = ViewModelConversion.ToRawValue(value);

				switch (fullFieldName)
				{
					case "psw.codpsw":
						this.ValCodpsw = ViewModelConversion.ToString(_value);
						break;
					case "psw.nome":
						this.ValNome = ViewModelConversion.ToString(_value);
						break;
					case "psw.salt":
						this.ValSalt = ViewModelConversion.ToString(_value);
						break;
					case "psw.status":
						this.ValStatus = ViewModelConversion.ToNumeric(_value);
						break;
					case "psw.pswtype":
						this.ValPswtype = ViewModelConversion.ToString(_value);
						break;
					case "psw.email":
						this.ValEmail = ViewModelConversion.ToString(_value);
						break;
					case "psw.datexp":
						this.ValDatexp = ViewModelConversion.ToDateTime(_value);
						break;
					default:
						CSGenio.framework.Log.Error($"SetViewModelValue (Psw) - Unexpected field identifier {fullFieldName}");
						break;
				}
			}
			catch (Exception ex)
			{
				throw new FrameworkException(Resources.Resources.PEDIMOS_DESCULPA__OC63848, "SetViewModelValue (Psw)", "Unexpected error", ex);
			}
		}

		public override void LoadModel(string id = null)
		{
			try { Model = Models.Psw.Find(id ?? Navigation.GetStrValue("psw"), m_userContext); }
			finally { Model ??= new Models.Psw(m_userContext); }
		}

		public override void Load(NameValueCollection qs, bool editable, bool ajaxRequest = false, bool lazyLoad = false)
		{
			this.editable = editable;
			GenioMVC.Models.Psw row = GenioMVC.Models.Psw.Find(Navigation.GetStrValue("psw"), m_userContext);
			this.MapFromModel(row);
			LoadPartial(qs, ajaxRequest);
		}

		public override void LoadPartial(NameValueCollection qs, bool ajaxRequest = false)
		{
			//add characteristics
			Characs = new List<string>();

			GenioMVC.Models.S_ua selectedAuth = null;
			ProLevels = new Dictionary<decimal, string>();
			ProLevels.Add(LevelAccess.DESAUTORIZADO.LevelValue, Resources.Resources.DESAUTORIZADO34584);
			ProLevels.Add(LevelAccess.NV1.LevelValue, Resources.Resources.CONSULTA40695);
			ProLevels.Add(LevelAccess.NV2.LevelValue, Resources.Resources.ADMINISTRATIVA24751);
			ProLevels.Add(LevelAccess.NV3.LevelValue, Resources.Resources.AGENTE44214);
			ProLevels.Add(LevelAccess.NV99.LevelValue, Resources.Resources.ADMINISTRADOR57294);

			selectedAuth = this.AuthorizationList.FirstOrDefault(x => x.ValModulo == "PRO");
			PROLevel = selectedAuth == null ? LevelAccess.DESAUTORIZADO.LevelValue : selectedAuth.ValNivel;
			this.UserPRO = new SelectList(ProLevels, "Key", "Value", PROLevel);
		}

		public void InsertAuthorization()
		{
			User user = m_userContext.User;
			PersistentSupport sp = m_userContext.PersistentSupport;
			CSGenio.business.CSGenioAs_ua userauth;

			userauth = new CSGenioAs_ua(user)
			{
				ValCodpsw = QPrimaryKey,
				ValModulo = "PRO",
				ValDatacria = DateTime.Now,
				ValOpercria = user.Name,
				ValSistema = "PRO"
			};
			userauth.insertPseud(sp);
		}

		public override CrudViewModelValidationResult Validate()
		{
			CrudViewModelFieldValidator validator = new(m_userContext.User.Language);

			validator.Required("ValNome", Resources.Resources.UTILIZADOR52387, ValNome);

			return validator.GetResult();
		}

		public void SaveAuthorization()
		{
			var Valcodua = "";
			User u = m_userContext.User;
			PersistentSupport sp = m_userContext.PersistentSupport;
			CSGenio.business.CSGenioAs_ua userauth;


			Valcodua = AuthorizationList.Find(m => m.ValModulo == "PRO").ValCodua;
			userauth = CSGenio.business.CSGenioAs_ua.search(sp, Valcodua, u);
			userauth.ValDatamuda = DateTime.Now;
			userauth.ValModulo = "PRO";
			userauth.ValNivel = Convert.ToDecimal(PROLevel);
			userauth.ValRole = PROLevel.ToString();
			userauth.change(sp, (CriteriaSet)null);

			StatusMessage msg = userauth.change(sp, (CriteriaSet)null);

			if (flashMessage == null)
				flashMessage = msg;
		}

		public override void Destroy(string id)
		{
			Model = Models.Psw.Find(id, m_userContext);
			if (Model == null)
				throw new ModelNotFoundException("Model not found");
			this.flashMessage = Model.Destroy();
		}

		#region Required methods - Empties

		protected override void LoadDefaultValues() { /* Method intentionally left empty. */ }

		protected override StatusMessage EvaluateWriteConditions(bool isApply) => null;

		protected override void LoadDocumentsProperties(Models.Psw model) { /* Method intentionally left empty. */ }

		public override StatusMessage ViewConditions() => null;

		public override StatusMessage InsertConditions() => null;

		public override StatusMessage UpdateConditions() => null;

		public override StatusMessage DeleteConditions() => null;

		#endregion
	}
}
