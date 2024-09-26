﻿using JsonIgnoreAttribute = System.Text.Json.Serialization.JsonIgnoreAttribute;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;

using CSGenio.business;
using CSGenio.framework;
using CSGenio.persistence;
using GenioMVC.Helpers;
using GenioMVC.Models.Exception;
using GenioMVC.Models.Navigation;
using Quidgest.Persistence;
using Quidgest.Persistence.GenericQuery;

namespace GenioMVC.ViewModels.Cidad
{
	public class Cidade_ViewModel : FormViewModel<Models.Cidad>, IPreparableForSerialization
	{
		[JsonIgnore]
		public override bool HasWriteConditions { get => false; }

		/// <summary>
		/// Reference for the Models MsqActive property
		/// </summary>
		[JsonIgnore]
		public bool MsqActive { get; set; } = false;

		#region Foreign keys
		/// <summary>
		/// Title: "País" | Type: "CE"
		/// </summary>
		public string ValCodpais { get; set; }

		#endregion
		/// <summary>
		/// Title: "Cidade" | Type: "C"
		/// </summary>
		public string ValCidade { get; set; }
		/// <summary>
		/// Title: "País" | Type: "C"
		/// </summary>
		[ValidateSetAccess]
		public TableDBEdit<GenioMVC.Models.Pais> TablePaisPais { get; set; }

		#region Navigations
		#endregion

		#region Auxiliar Keys for Image controls



		#endregion

		#region Extra database fields



		#endregion

		#region Fields for formulas


		#endregion

		public string ValCodcidad { get; set; }


		/// <summary>
		/// FOR DESERIALIZATION ONLY
		/// A call to Init() needs to be manually invoked after this constructor
		/// </summary>
		[Obsolete("For deserialization only")]
		public Cidade_ViewModel() : base(null!) { }

		public Cidade_ViewModel(UserContext userContext, bool nestedForm = false) : base(userContext, "FCIDADE", nestedForm) { }

		public Cidade_ViewModel(UserContext userContext, Models.Cidad row, bool nestedForm = false) : base(userContext, "FCIDADE", row, nestedForm) { }

		public Cidade_ViewModel(UserContext userContext, string id, bool nestedForm = false, string[]? fieldsToLoad = null) : this(userContext, nestedForm)
		{
			this.Navigation.SetValue("cidad", id);
			Model = Models.Cidad.Find(id, userContext, "FCIDADE", fieldsToQuery: fieldsToLoad);
			if (Model == null)
				throw new ModelNotFoundException("Model not found");
			InitModel();
		}

		protected override void InitLevels()
		{
			this.RoleToShow = CSGenio.framework.Role.ROLE_1;
			this.RoleToEdit = CSGenio.framework.Role.ROLE_1;
		}

		#region Form conditions

		public override StatusMessage InsertConditions()
		{
			return InsertConditions(m_userContext);
		}

		public static StatusMessage InsertConditions(UserContext userContext)
		{
			var m_userContext = userContext;
			StatusMessage result = new StatusMessage(Status.OK, "");
			Models.Cidad model = new Models.Cidad(userContext) { Identifier = "FCIDADE" };

			var navigation = m_userContext.CurrentNavigation;
			// The "LoadKeysFromHistory" must be after the "LoadEPH" because the PHE's in the tree mark Foreign Keys to null
			// (since they cannot assign multiple values to a single field) and thus the value that comes from Navigation is lost.
			// And this makes it more like the order of loading the model when opening the form.
			model.LoadEPH("FCIDADE");
			if (navigation != null)
				model.LoadKeysFromHistory(navigation, navigation.CurrentLevel.Level);

			var tableResult = model.EvaluateTableConditions(ConditionType.INSERT);
			result.MergeStatusMessage(tableResult);
			return result;
		}

		public override StatusMessage UpdateConditions()
		{
			StatusMessage result = new StatusMessage(Status.OK, "");
			var model = Model;

			var tableResult = model.EvaluateTableConditions(ConditionType.UPDATE);
			result.MergeStatusMessage(tableResult);
			return result;
		}

		public override StatusMessage DeleteConditions()
		{
			StatusMessage result = new StatusMessage(Status.OK, "");
			var model = Model;

			var tableResult = model.EvaluateTableConditions(ConditionType.DELETE);
			result.MergeStatusMessage(tableResult);
			return result;
		}

		public override StatusMessage ViewConditions()
		{
			var model = Model;
			StatusMessage result = model.EvaluateTableConditions(ConditionType.VIEW);
			var tableResult = model.EvaluateTableConditions(ConditionType.VIEW);
			result.MergeStatusMessage(tableResult);
			return result;
		}

		protected override StatusMessage EvaluateWriteConditions(bool isApply)
		{
			Models.Cidad model = Model;
			StatusMessage result = new StatusMessage(Status.OK, "");
			return result;
		}

		public StatusMessage EvaluateTableConditions(ConditionType type)
		{
			return Model.EvaluateTableConditions(type);
		}

		#endregion

		#region Mapper

		public override void MapFromModel(Models.Cidad m)
		{
			if (m == null)
			{
				CSGenio.framework.Log.Error("Map Model (Cidad) to ViewModel (Cidade) - Model is a null reference");
				throw new ModelNotFoundException("Model not found");
			}

			try
			{
				ValCodpais = ViewModelConversion.ToString(m.ValCodpais);
				ValCidade = ViewModelConversion.ToString(m.ValCidade);
				ValCodcidad = ViewModelConversion.ToString(m.ValCodcidad);
			}
			catch (Exception)
			{
				CSGenio.framework.Log.Error("Map Model (Cidad) to ViewModel (Cidade) - Error during mapping");
				throw;
			}
		}

		/// <summary>
		/// Performs the mapping of field values from the ViewModel to the Model.
		/// </summary>
		/// <exception cref="ModelNotFoundException">Thrown if <paramref name="m"/> is null.</exception>
		public override void MapToModel()
		{
			MapToModel(this.Model);
		}

		/// <summary>
		/// Performs the mapping of field values from the ViewModel to the Model.
		/// </summary>
		/// <param name="m">The Model to be filled.</param>
		/// <exception cref="ModelNotFoundException">Thrown if <paramref name="m"/> is null.</exception>
		public override void MapToModel(Models.Cidad m)
		{
			if (m == null)
			{
				CSGenio.framework.Log.Error("Map ViewModel (Cidade) to Model (Cidad) - Model is a null reference");
				throw new ModelNotFoundException("Model not found");
			}

			try
			{

				m.ValCodpais = ViewModelConversion.ToString(ValCodpais);

				m.ValCidade = ViewModelConversion.ToString(ValCidade);

				m.ValCodcidad = ViewModelConversion.ToString(ValCodcidad);
			}
			catch (Exception)
			{
				CSGenio.framework.Log.Error($"Map ViewModel (Cidade) to Model (Cidad) - Error during mapping. All user values: {HasDisabledUserValuesSecurity}");
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
					case "cidad.codpais":
						this.ValCodpais = ViewModelConversion.ToString(_value);
						break;
					case "cidad.cidade":
						this.ValCidade = ViewModelConversion.ToString(_value);
						break;
					case "cidad.codcidad":
						this.ValCodcidad = ViewModelConversion.ToString(_value);
						break;
					default:
						Log.Error($"SetViewModelValue (Cidade) - Unexpected field identifier {fullFieldName}");
						break;
				}
			}
			catch (Exception ex)
			{
				throw new FrameworkException(Resources.Resources.PEDIMOS_DESCULPA__OC63848, "SetViewModelValue (Cidade)", "Unexpected error", ex);
			}
		}

		#endregion

		/// <summary>
		/// Reads the Model from the database based on the key that is in the history or that was passed through the parameter
		/// </summary>
		/// <param name="id">The primary key of the record that needs to be read from the database. Leave NULL to use the value from the History.</param>
		public override void LoadModel(string id = null)
		{
			try { Model = Models.Cidad.Find(id ?? Navigation.GetStrValue("cidad"), m_userContext, "FCIDADE"); }
			finally { Model ??= new Models.Cidad(m_userContext) { Identifier = "FCIDADE" }; }

			base.LoadModel();
		}

		public override void Load(NameValueCollection qs, bool editable, bool ajaxRequest = false, bool lazyLoad = false)
		{
			this.editable = editable;
			CSGenio.business.Area oldvalues = null;

			// TODO: Deve ser substituido por search do CSGenioA
			try
			{
				Model = Models.Cidad.Find(Navigation.GetStrValue("cidad"), m_userContext, "FCIDADE");
			}
			finally
			{
				if (Model == null)
					throw new ModelNotFoundException("Model not found");

				if (Navigation.CurrentLevel.FormMode == FormMode.New || Navigation.CurrentLevel.FormMode == FormMode.Duplicate)
					LoadDefaultValues();
				else
					oldvalues = Model.klass;
			}

			Model.Identifier = "FCIDADE";
			InitModel(qs, lazyLoad);

			if (Navigation.CurrentLevel.FormMode == FormMode.New || Navigation.CurrentLevel.FormMode == FormMode.Edit || Navigation.CurrentLevel.FormMode == FormMode.Duplicate)
			{
				// MH - Voltar calcular as formulas to "atualizar" os Qvalues dos fields fixos
				// Conexão deve estar aberta de fora. Podem haver formulas que utilizam funções "manuais".
				// TODO: It needs to be analyzed whether we should disable the security of field filling here. If there is any case where the field with the block condition can only be calculated after the double calculation of the formulas.
				MapToModel(Model);
				// Preencher operações internas
				Model.klass.fillInternalOperations(m_userContext.PersistentSupport, oldvalues);
				MapFromModel(Model);
			}

			// Load just the selected row primary keys for checklists.
			// Needed for submitting forms incase checklists are in collapsible zones that have not been expanded to load the checklist data.
			LoadChecklistsSelectedIDs();
		}

		protected override void FillExtraProperties()
		{
		}

		protected override void LoadDocumentsProperties(Models.Cidad row)
		{
		}

		/// <summary>
		/// Load Partial
		/// </summary>
		/// <param name="lazyLoad">Lazy loading of dropdown items</param>
		public override void LoadPartial(NameValueCollection qs, bool lazyLoad = false)
		{
			// MH [bugfix] - Quando o POST da ficha falha, ao recaregar a view os documentos na BD perdem alguma informação (ex: name do file)
			if (Model == null)
			{
				// Precisamos fazer o Find to obter as chaves dos documentos que já foram anexados
				// TODO: Conseguir passar estas chaves no POST to poder retirar o Find.
				Model = Models.Cidad.Find(Navigation.GetStrValue("cidad"), m_userContext, "FCIDADE");
				if (Model == null)
				{
					Model = new Models.Cidad(m_userContext) { Identifier = "FCIDADE" };
					Model.klass.QPrimaryKey = Navigation.GetStrValue("cidad");
				}
				MapToModel(Model);
				LoadDocumentsProperties(Model);
			}
			// Add characteristics
			Characs = new List<string>();

			Load_Cidade__pais_pais____(qs, lazyLoad);
// USE /[MANUAL PRO VIEWMODEL_LOADPARTIAL CIDADE]/
		}

// USE /[MANUAL PRO VIEWMODEL_NEW CIDADE]/

		// Preencher Qvalues default dos fields do form
		protected override void LoadDefaultValues()
		{
		}

		public override CrudViewModelValidationResult Validate()
		{
			CrudViewModelFieldValidator validator = new(m_userContext.User.Language);

			validator.StringLength("ValCidade", Resources.Resources.CIDADE42080, ValCidade, 50);
			validator.Required("ValCidade", Resources.Resources.CIDADE42080, ViewModelConversion.ToString(ValCidade), FieldType.TEXTO.Formatting);

			return validator.GetResult();
		}

// USE /[MANUAL PRO VIEWMODEL_SAVE CIDADE]/
		public override void Save()
		{


			base.Save();
		}

// USE /[MANUAL PRO VIEWMODEL_APPLY CIDADE]/

// USE /[MANUAL PRO VIEWMODEL_DUPLICATE CIDADE]/

// USE /[MANUAL PRO VIEWMODEL_DESTROY CIDADE]/
		public override void Destroy(string id)
		{
			Model = Models.Cidad.Find(id, m_userContext, "FCIDADE");
			if (Model == null)
				throw new ModelNotFoundException("Model not found");
			this.flashMessage = Model.Destroy();
		}

		/// <summary>
		/// Load selected row primary keys for all checklists
		/// </summary>
		public void LoadChecklistsSelectedIDs()
		{
		}

		/// <summary>
		/// TablePaisPais -> (DB)
		/// </summary>
		/// <param name="qs"></param>
		/// <param name="lazyLoad">Lazy loading of dropdown items</param>
		public void Load_Cidade__pais_pais____(NameValueCollection qs, bool lazyLoad = false)
		{
			bool cidade__pais_pais____DoLoad = true;
			CriteriaSet cidade__pais_pais____Conds = CriteriaSet.And();
			{
				object hValue = Navigation.GetValue("pais", true);
				if (hValue != null && !(hValue is Array) && !string.IsNullOrEmpty(Convert.ToString(hValue)))
				{
					cidade__pais_pais____Conds.Equal(CSGenioApais.FldCodpais, hValue);
					this.ValCodpais = DBConversion.ToString(hValue);
				}
			}

			TablePaisPais = new TableDBEdit<Models.Pais>
			{
				IsLazyLoad = lazyLoad
			};

			if (lazyLoad)
			{
				if (Navigation.CurrentLevel.GetEntry("RETURN_pais") != null)
				{
					this.ValCodpais = Navigation.GetStrValue("RETURN_pais");
					Navigation.CurrentLevel.SetEntry("RETURN_pais", null);
				}
				FillDependant_CidadeTablePaisPais(lazyLoad);
				//Check if foreignkey comes from history
				TablePaisPais.FilledByHistory = Navigation.CheckFilledByHistory("pais");
				return;
			}

			if (cidade__pais_pais____DoLoad)
			{
				List<ColumnSort> sorts = new List<ColumnSort>();
				ColumnSort requestedSort = GetRequestSort(TablePaisPais, "sTablePaisPais", "dTablePaisPais", qs, "pais");
				if (requestedSort != null)
					sorts.Add(requestedSort);
				sorts.Add(new ColumnSort(new ColumnReference(CSGenioApais.FldPais), SortOrder.Ascending));

				string query = "";
				if (!string.IsNullOrEmpty(qs["TablePaisPais_tableFilters"]))
					TablePaisPais.TableFilters = bool.Parse(qs["TablePaisPais_tableFilters"]);
				else
					TablePaisPais.TableFilters = false;

				query = qs["qTablePaisPais"];

				//RS 26.07.2016 O preenchimento da lista de ajuda dos Dbedits passa a basear-se apenas no campo do próprio DbEdit
				// O interface de pesquisa rápida não fica coerente quando se visualiza apenas uma coluna mas a pesquisa faz matching com 5 ou 6 colunas diferentes
				//  tornando confuso to o user porque determinada row foi devolvida quando o Qresult não mostra como o matching foi feito
				CriteriaSet search_filters = CriteriaSet.And();
				if (!string.IsNullOrEmpty(query))
				{
					search_filters.Like(CSGenioApais.FldPais, query + "%");
				}
				cidade__pais_pais____Conds.SubSet(search_filters);

				string tryParsePage = qs["pTablePaisPais"] != null ? qs["pTablePaisPais"].ToString() : "1";
				int page = !string.IsNullOrEmpty(tryParsePage) ? int.Parse(tryParsePage) : 1;
				int numberItems = CSGenio.framework.Configuration.NrRegDBedit;
				int offset = (page - 1) * numberItems;

				FieldRef[] fields = new FieldRef[] { CSGenioApais.FldCodpais, CSGenioApais.FldPais, CSGenioApais.FldZzstate };

// USE /[MANUAL PRO OVERRQ CIDADE_PAISPAIS]/

				// Limitation by Zzstate
				/*
					Records that are currently being inserted or duplicated will also be included.
					Client-side persistence will try to fill the "text" value of that option.
				*/
				if (Navigation.checkFormMode("pais", FormMode.New) || Navigation.checkFormMode("pais", FormMode.Duplicate))
					cidade__pais_pais____Conds.SubSet(CriteriaSet.Or()
						.Equal(CSGenioApais.FldZzstate, 0)
						.Equal(CSGenioApais.FldCodpais, Navigation.GetStrValue("pais")));
				else
					cidade__pais_pais____Conds.Criterias.Add(new Criteria(new ColumnReference(CSGenioApais.FldZzstate), CriteriaOperator.Equal, 0));

				FieldRef firstVisibleColumn = new FieldRef("pais", "pais");
				ListingMVC<CSGenioApais> listing = Models.ModelBase.Where<CSGenioApais>(m_userContext, false, cidade__pais_pais____Conds, fields, offset, numberItems, sorts, "LED_CIDADE__PAIS_PAIS____", true, false, firstVisibleColumn: firstVisibleColumn);

				TablePaisPais.SetPagination(page, numberItems, listing.HasMore, listing.GetTotal, listing.TotalRecords);
				TablePaisPais.Query = query;
				TablePaisPais.Elements = listing.RowsForViewModel<GenioMVC.Models.Pais>((r) => new GenioMVC.Models.Pais(m_userContext, r, true, _fieldsToSerialize_CIDADE__PAIS_PAIS____));

				//created by [ MH ] at [ 14.04.2016 ] - Foi alterada a forma de retornar a key do novo registo inserido / editado no form de apoio do DBEdit.
				//last update by [ MH ] at [ 10.05.2016 ] - Validação se key encontra-se no level atual, as chaves dos niveis anteriores devem ser ignorados.
				if (Navigation.CurrentLevel.GetEntry("RETURN_pais") != null)
				{
					this.ValCodpais = Navigation.GetStrValue("RETURN_pais");
					Navigation.CurrentLevel.SetEntry("RETURN_pais", null);
				}

				TablePaisPais.List = new SelectList(TablePaisPais.Elements.ToSelectList(x => x.ValPais, x => x.ValCodpais,  x => x.ValCodpais == this.ValCodpais), "Value", "Text", this.ValCodpais);
				FillDependant_CidadeTablePaisPais();

				//Check if foreignkey comes from history
				TablePaisPais.FilledByHistory = Navigation.CheckFilledByHistory("pais");
			}
		}

		/// <summary>
		/// Get Dependant fields values -> TablePaisPais (DB)
		/// </summary>
		/// <param name="PKey">Primary Key of Pais</param>
		public ConcurrentDictionary<string, object> GetDependant_CidadeTablePaisPais(string PKey)
		{
			FieldRef[] refDependantFields = [CSGenioApais.FldCodpais, CSGenioApais.FldPais];

			var returnEmptyDependants = false;
			CriteriaSet wherecodition = CriteriaSet.And();

			// Return default values
			if (GlobalFunctions.emptyG(PKey) == 1)
				returnEmptyDependants = true;

			// Check if the limit(s) is filled if exists
			// - - - - - - - - - - - - - - - - - - - - -

			if (returnEmptyDependants)
				return GetViewModelFieldValues(refDependantFields);

			PersistentSupport sp = m_userContext.PersistentSupport;
			User u = m_userContext.User;

			CSGenioApais tempArea = new(u);

			// Fields to select
			SelectQuery querySelect = new();
			querySelect.PageSize(1);
			foreach (FieldRef field in refDependantFields)
				querySelect.Select(field);

			querySelect.From(tempArea.QSystem, tempArea.TableName, tempArea.Alias)
				.Where(wherecodition.Equal(CSGenioApais.FldCodpais, PKey));

			string[] dependantFields = refDependantFields.Select(f => f.FullName).ToArray();
			QueryUtils.SetInnerJoins(dependantFields, null, tempArea, querySelect);

			ArrayList values = sp.executeReaderOneRow(querySelect);
			bool useDefaults = values.Count == 0;

			if (useDefaults)
				return GetViewModelFieldValues(refDependantFields);
			return GetViewModelFieldValues(refDependantFields, values);
		}

		/// <summary>
		/// Fill Dependant fields values -> TablePaisPais (DB)
		/// </summary>
		/// <param name="lazyLoad">Lazy loading of dropdown items</param>
		public void FillDependant_CidadeTablePaisPais(bool lazyLoad = false)
		{
			var row = GetDependant_CidadeTablePaisPais(this.ValCodpais);
			try
			{

				// Fill List fields
				this.ValCodpais = ViewModelConversion.ToString(row["pais.codpais"]);
				TablePaisPais.Value = (string)row["pais.pais"];
				if (GlobalFunctions.emptyG(this.ValCodpais) == 1)
				{
					this.ValCodpais = "";
					TablePaisPais.Value = "";
					Navigation.ClearValue("pais");
				}
				else if (lazyLoad)
				{
					TablePaisPais.SetPagination(1, 0, false, false, 1);
					TablePaisPais.List = new SelectList(new List<SelectListItem>()
					{
						new SelectListItem
						{
							Value = Convert.ToString(this.ValCodpais),
							Text = Convert.ToString(TablePaisPais.Value),
							Selected = true
						}
					}, "Value", "Text", this.ValCodpais);
				}

				TablePaisPais.Selected = this.ValCodpais;
			}
			catch (Exception ex)
			{
				CSGenio.framework.Log.Error(string.Format("FillDependant_Error (TablePaisPais): {0}; {1}", ex.Message, ex.InnerException != null ? ex.InnerException.Message : ""));
			}
		}

		private readonly string[] _fieldsToSerialize_CIDADE__PAIS_PAIS____ = ["Pais", "Pais.ValCodpais", "Pais.ValZzstate", "Pais.ValPais"];

		protected override object GetViewModelValue(string identifier, object modelValue)
		{
			return identifier switch
			{
				"cidad.codpais" => ViewModelConversion.ToString(modelValue),
				"cidad.cidade" => ViewModelConversion.ToString(modelValue),
				"cidad.codcidad" => ViewModelConversion.ToString(modelValue),
				"pais.codpais" => ViewModelConversion.ToString(modelValue),
				"pais.pais" => ViewModelConversion.ToString(modelValue),
				_ => modelValue
			};
		}



		#region Charts


		#endregion

		#region Custom code

// USE /[MANUAL PRO VIEWMODEL_CUSTOM CIDADE]/

		#endregion
	}
}
