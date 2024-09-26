using JsonIgnoreAttribute = System.Text.Json.Serialization.JsonIgnoreAttribute;
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

namespace GenioMVC.ViewModels.Agent
{
	public class Agente_ViewModel : FormViewModel<Models.Agent>, IPreparableForSerialization
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
		/// Title: "País de morada" | Type: "CE"
		/// </summary>
		public string ValCodpmora { get; set; }
		/// <summary>
		/// Title: "País de nascimento" | Type: "CE"
		/// </summary>
		public string ValCodpnasc { get; set; }

		#endregion
		/// <summary>
		/// Title: "Fotografia" | Type: "IJ"
		/// </summary>
		[ImageThumbnailJsonConverter(30, 50)]
		public GenioMVC.Models.ImageModel ValFoto { get; set; }
		/// <summary>
		/// Title: "Nome" | Type: "C"
		/// </summary>
		public string ValNome { get; set; }
		/// <summary>
		/// Title: "Data de nascimento" | Type: "D"
		/// </summary>
		public DateTime? ValDnascime { get; set; }
		/// <summary>
		/// Title: "E-mail" | Type: "C"
		/// </summary>
		public string ValEmail { get; set; }
		/// <summary>
		/// Title: "Telefone" | Type: "C"
		/// </summary>
		public string ValTelefone { get; set; }
		/// <summary>
		/// Title: "País de morada" | Type: "C"
		/// </summary>
		[ValidateSetAccess]
		public TableDBEdit<GenioMVC.Models.Pmora> TablePmoraPais { get; set; }
		/// <summary>
		/// Title: "País de nascimento" | Type: "C"
		/// </summary>
		[ValidateSetAccess]
		public TableDBEdit<GenioMVC.Models.Pnasc> TablePnascPais { get; set; }

		#region Navigations
		#endregion

		#region Auxiliar Keys for Image controls



		#endregion

		#region Extra database fields



		#endregion

		#region Fields for formulas


		#endregion

		public string ValCodagent { get; set; }


		/// <summary>
		/// FOR DESERIALIZATION ONLY
		/// A call to Init() needs to be manually invoked after this constructor
		/// </summary>
		[Obsolete("For deserialization only")]
		public Agente_ViewModel() : base(null!) { }

		public Agente_ViewModel(UserContext userContext, bool nestedForm = false) : base(userContext, "FAGENTE", nestedForm) { }

		public Agente_ViewModel(UserContext userContext, Models.Agent row, bool nestedForm = false) : base(userContext, "FAGENTE", row, nestedForm) { }

		public Agente_ViewModel(UserContext userContext, string id, bool nestedForm = false, string[]? fieldsToLoad = null) : this(userContext, nestedForm)
		{
			this.Navigation.SetValue("agent", id);
			Model = Models.Agent.Find(id, userContext, "FAGENTE", fieldsToQuery: fieldsToLoad);
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
			Models.Agent model = new Models.Agent(userContext) { Identifier = "FAGENTE" };

			var navigation = m_userContext.CurrentNavigation;
			// The "LoadKeysFromHistory" must be after the "LoadEPH" because the PHE's in the tree mark Foreign Keys to null
			// (since they cannot assign multiple values to a single field) and thus the value that comes from Navigation is lost.
			// And this makes it more like the order of loading the model when opening the form.
			model.LoadEPH("FAGENTE");
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
			Models.Agent model = Model;
			StatusMessage result = new StatusMessage(Status.OK, "");
			return result;
		}

		public StatusMessage EvaluateTableConditions(ConditionType type)
		{
			return Model.EvaluateTableConditions(type);
		}

		#endregion

		#region Mapper

		public override void MapFromModel(Models.Agent m)
		{
			if (m == null)
			{
				CSGenio.framework.Log.Error("Map Model (Agent) to ViewModel (Agente) - Model is a null reference");
				throw new ModelNotFoundException("Model not found");
			}

			try
			{
				ValCodpmora = ViewModelConversion.ToString(m.ValCodpmora);
				ValCodpnasc = ViewModelConversion.ToString(m.ValCodpnasc);
				ValFoto = ViewModelConversion.ToImage(m.ValFoto);
				ValNome = ViewModelConversion.ToString(m.ValNome);
				ValDnascime = ViewModelConversion.ToDateTime(m.ValDnascime);
				ValEmail = ViewModelConversion.ToString(m.ValEmail);
				ValTelefone = ViewModelConversion.ToString(m.ValTelefone);
				ValCodagent = ViewModelConversion.ToString(m.ValCodagent);
			}
			catch (Exception)
			{
				CSGenio.framework.Log.Error("Map Model (Agent) to ViewModel (Agente) - Error during mapping");
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
		public override void MapToModel(Models.Agent m)
		{
			if (m == null)
			{
				CSGenio.framework.Log.Error("Map ViewModel (Agente) to Model (Agent) - Model is a null reference");
				throw new ModelNotFoundException("Model not found");
			}

			try
			{

				m.ValCodpmora = ViewModelConversion.ToString(ValCodpmora);

				m.ValCodpnasc = ViewModelConversion.ToString(ValCodpnasc);

				if (ValFoto == null || !ValFoto.IsThumbnail)
					m.ValFoto = ViewModelConversion.ToImage(ValFoto);

				m.ValNome = ViewModelConversion.ToString(ValNome);

				m.ValDnascime = ViewModelConversion.ToDateTime(ValDnascime);

				m.ValEmail = ViewModelConversion.ToString(ValEmail);

				m.ValTelefone = ViewModelConversion.ToString(ValTelefone);

				m.ValCodagent = ViewModelConversion.ToString(ValCodagent);
			}
			catch (Exception)
			{
				CSGenio.framework.Log.Error($"Map ViewModel (Agente) to Model (Agent) - Error during mapping. All user values: {HasDisabledUserValuesSecurity}");
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
					case "agent.codpmora":
						this.ValCodpmora = ViewModelConversion.ToString(_value);
						break;
					case "agent.codpnasc":
						this.ValCodpnasc = ViewModelConversion.ToString(_value);
						break;
					case "agent.foto":
						this.ValFoto = ViewModelConversion.ToImage(_value);
						break;
					case "agent.nome":
						this.ValNome = ViewModelConversion.ToString(_value);
						break;
					case "agent.dnascime":
						this.ValDnascime = ViewModelConversion.ToDateTime(_value);
						break;
					case "agent.email":
						this.ValEmail = ViewModelConversion.ToString(_value);
						break;
					case "agent.telefone":
						this.ValTelefone = ViewModelConversion.ToString(_value);
						break;
					case "agent.codagent":
						this.ValCodagent = ViewModelConversion.ToString(_value);
						break;
					default:
						Log.Error($"SetViewModelValue (Agente) - Unexpected field identifier {fullFieldName}");
						break;
				}
			}
			catch (Exception ex)
			{
				throw new FrameworkException(Resources.Resources.PEDIMOS_DESCULPA__OC63848, "SetViewModelValue (Agente)", "Unexpected error", ex);
			}
		}

		#endregion

		/// <summary>
		/// Reads the Model from the database based on the key that is in the history or that was passed through the parameter
		/// </summary>
		/// <param name="id">The primary key of the record that needs to be read from the database. Leave NULL to use the value from the History.</param>
		public override void LoadModel(string id = null)
		{
			try { Model = Models.Agent.Find(id ?? Navigation.GetStrValue("agent"), m_userContext, "FAGENTE"); }
			finally { Model ??= new Models.Agent(m_userContext) { Identifier = "FAGENTE" }; }

			base.LoadModel();
		}

		public override void Load(NameValueCollection qs, bool editable, bool ajaxRequest = false, bool lazyLoad = false)
		{
			this.editable = editable;
			CSGenio.business.Area oldvalues = null;

			// TODO: Deve ser substituido por search do CSGenioA
			try
			{
				Model = Models.Agent.Find(Navigation.GetStrValue("agent"), m_userContext, "FAGENTE");
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

			Model.Identifier = "FAGENTE";
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

		protected override void LoadDocumentsProperties(Models.Agent row)
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
				Model = Models.Agent.Find(Navigation.GetStrValue("agent"), m_userContext, "FAGENTE");
				if (Model == null)
				{
					Model = new Models.Agent(m_userContext) { Identifier = "FAGENTE" };
					Model.klass.QPrimaryKey = Navigation.GetStrValue("agent");
				}
				MapToModel(Model);
				LoadDocumentsProperties(Model);
			}
			// Add characteristics
			Characs = new List<string>();

			Load_Agente__pmorapais____(qs, lazyLoad);
			Load_Agente__pnascpais____(qs, lazyLoad);
// USE /[MANUAL PRO VIEWMODEL_LOADPARTIAL AGENTE]/
		}

// USE /[MANUAL PRO VIEWMODEL_NEW AGENTE]/

		// Preencher Qvalues default dos fields do form
		protected override void LoadDefaultValues()
		{
		}

		public override CrudViewModelValidationResult Validate()
		{
			CrudViewModelFieldValidator validator = new(m_userContext.User.Language);

			validator.StringLength("ValNome", Resources.Resources.NOME47814, ValNome, 80);
			validator.Required("ValNome", Resources.Resources.NOME47814, ViewModelConversion.ToString(ValNome), FieldType.TEXTO.Formatting);
			validator.StringLength("ValEmail", Resources.Resources.E_MAIL42251, ValEmail, 80);
			validator.Required("ValEmail", Resources.Resources.E_MAIL42251, ViewModelConversion.ToString(ValEmail), FieldType.TEXTO.Formatting);
			validator.StringLength("ValTelefone", Resources.Resources.TELEFONE37757, ValTelefone, 14);

			return validator.GetResult();
		}

// USE /[MANUAL PRO VIEWMODEL_SAVE AGENTE]/
		public override void Save()
		{


			base.Save();
		}

// USE /[MANUAL PRO VIEWMODEL_APPLY AGENTE]/

// USE /[MANUAL PRO VIEWMODEL_DUPLICATE AGENTE]/

// USE /[MANUAL PRO VIEWMODEL_DESTROY AGENTE]/
		public override void Destroy(string id)
		{
			Model = Models.Agent.Find(id, m_userContext, "FAGENTE");
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
		/// TablePmoraPais -> (DB)
		/// </summary>
		/// <param name="qs"></param>
		/// <param name="lazyLoad">Lazy loading of dropdown items</param>
		public void Load_Agente__pmorapais____(NameValueCollection qs, bool lazyLoad = false)
		{
			bool agente__pmorapais____DoLoad = true;
			CriteriaSet agente__pmorapais____Conds = CriteriaSet.And();
			{
				object hValue = Navigation.GetValue("pmora", true);
				if (hValue != null && !(hValue is Array) && !string.IsNullOrEmpty(Convert.ToString(hValue)))
				{
					agente__pmorapais____Conds.Equal(CSGenioApmora.FldCodpais, hValue);
					this.ValCodpmora = DBConversion.ToString(hValue);
				}
			}

			TablePmoraPais = new TableDBEdit<Models.Pmora>
			{
				IsLazyLoad = lazyLoad
			};

			if (lazyLoad)
			{
				if (Navigation.CurrentLevel.GetEntry("RETURN_pmora") != null)
				{
					this.ValCodpmora = Navigation.GetStrValue("RETURN_pmora");
					Navigation.CurrentLevel.SetEntry("RETURN_pmora", null);
				}
				FillDependant_AgenteTablePmoraPais(lazyLoad);
				//Check if foreignkey comes from history
				TablePmoraPais.FilledByHistory = Navigation.CheckFilledByHistory("pmora");
				return;
			}

			if (agente__pmorapais____DoLoad)
			{
				List<ColumnSort> sorts = new List<ColumnSort>();
				ColumnSort requestedSort = GetRequestSort(TablePmoraPais, "sTablePmoraPais", "dTablePmoraPais", qs, "pmora");
				if (requestedSort != null)
					sorts.Add(requestedSort);

				string query = "";
				if (!string.IsNullOrEmpty(qs["TablePmoraPais_tableFilters"]))
					TablePmoraPais.TableFilters = bool.Parse(qs["TablePmoraPais_tableFilters"]);
				else
					TablePmoraPais.TableFilters = false;

				query = qs["qTablePmoraPais"];

				//RS 26.07.2016 O preenchimento da lista de ajuda dos Dbedits passa a basear-se apenas no campo do próprio DbEdit
				// O interface de pesquisa rápida não fica coerente quando se visualiza apenas uma coluna mas a pesquisa faz matching com 5 ou 6 colunas diferentes
				//  tornando confuso to o user porque determinada row foi devolvida quando o Qresult não mostra como o matching foi feito
				CriteriaSet search_filters = CriteriaSet.And();
				if (!string.IsNullOrEmpty(query))
				{
					search_filters.Like(CSGenioApmora.FldPais, query + "%");
				}
				agente__pmorapais____Conds.SubSet(search_filters);

				string tryParsePage = qs["pTablePmoraPais"] != null ? qs["pTablePmoraPais"].ToString() : "1";
				int page = !string.IsNullOrEmpty(tryParsePage) ? int.Parse(tryParsePage) : 1;
				int numberItems = CSGenio.framework.Configuration.NrRegDBedit;
				int offset = (page - 1) * numberItems;

				FieldRef[] fields = new FieldRef[] { CSGenioApmora.FldCodpais, CSGenioApmora.FldPais, CSGenioApmora.FldZzstate };

// USE /[MANUAL PRO OVERRQ AGENTE_PMORAPAIS]/

				// Limitation by Zzstate
				/*
					Records that are currently being inserted or duplicated will also be included.
					Client-side persistence will try to fill the "text" value of that option.
				*/
				if (Navigation.checkFormMode("pmora", FormMode.New) || Navigation.checkFormMode("pmora", FormMode.Duplicate))
					agente__pmorapais____Conds.SubSet(CriteriaSet.Or()
						.Equal(CSGenioApmora.FldZzstate, 0)
						.Equal(CSGenioApmora.FldCodpais, Navigation.GetStrValue("pmora")));
				else
					agente__pmorapais____Conds.Criterias.Add(new Criteria(new ColumnReference(CSGenioApmora.FldZzstate), CriteriaOperator.Equal, 0));

				FieldRef firstVisibleColumn = new FieldRef("pmora", "pais");
				ListingMVC<CSGenioApmora> listing = Models.ModelBase.Where<CSGenioApmora>(m_userContext, false, agente__pmorapais____Conds, fields, offset, numberItems, sorts, "LED_AGENTE__PMORAPAIS____", true, false, firstVisibleColumn: firstVisibleColumn);

				TablePmoraPais.SetPagination(page, numberItems, listing.HasMore, listing.GetTotal, listing.TotalRecords);
				TablePmoraPais.Query = query;
				TablePmoraPais.Elements = listing.RowsForViewModel<GenioMVC.Models.Pmora>((r) => new GenioMVC.Models.Pmora(m_userContext, r, true, _fieldsToSerialize_AGENTE__PMORAPAIS____));

				//created by [ MH ] at [ 14.04.2016 ] - Foi alterada a forma de retornar a key do novo registo inserido / editado no form de apoio do DBEdit.
				//last update by [ MH ] at [ 10.05.2016 ] - Validação se key encontra-se no level atual, as chaves dos niveis anteriores devem ser ignorados.
				if (Navigation.CurrentLevel.GetEntry("RETURN_pmora") != null)
				{
					this.ValCodpmora = Navigation.GetStrValue("RETURN_pmora");
					Navigation.CurrentLevel.SetEntry("RETURN_pmora", null);
				}

				TablePmoraPais.List = new SelectList(TablePmoraPais.Elements.ToSelectList(x => x.ValPais, x => x.ValCodpais,  x => x.ValCodpais == this.ValCodpmora), "Value", "Text", this.ValCodpmora);
				FillDependant_AgenteTablePmoraPais();

				//Check if foreignkey comes from history
				TablePmoraPais.FilledByHistory = Navigation.CheckFilledByHistory("pmora");
			}
		}

		/// <summary>
		/// Get Dependant fields values -> TablePmoraPais (DB)
		/// </summary>
		/// <param name="PKey">Primary Key of Pmora</param>
		public ConcurrentDictionary<string, object> GetDependant_AgenteTablePmoraPais(string PKey)
		{
			FieldRef[] refDependantFields = [CSGenioApmora.FldCodpais, CSGenioApmora.FldPais];

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

			CSGenioApmora tempArea = new(u);

			// Fields to select
			SelectQuery querySelect = new();
			querySelect.PageSize(1);
			foreach (FieldRef field in refDependantFields)
				querySelect.Select(field);

			querySelect.From(tempArea.QSystem, tempArea.TableName, tempArea.Alias)
				.Where(wherecodition.Equal(CSGenioApmora.FldCodpais, PKey));

			string[] dependantFields = refDependantFields.Select(f => f.FullName).ToArray();
			QueryUtils.SetInnerJoins(dependantFields, null, tempArea, querySelect);

			ArrayList values = sp.executeReaderOneRow(querySelect);
			bool useDefaults = values.Count == 0;

			if (useDefaults)
				return GetViewModelFieldValues(refDependantFields);
			return GetViewModelFieldValues(refDependantFields, values);
		}

		/// <summary>
		/// Fill Dependant fields values -> TablePmoraPais (DB)
		/// </summary>
		/// <param name="lazyLoad">Lazy loading of dropdown items</param>
		public void FillDependant_AgenteTablePmoraPais(bool lazyLoad = false)
		{
			var row = GetDependant_AgenteTablePmoraPais(this.ValCodpmora);
			try
			{

				// Fill List fields
				this.ValCodpmora = ViewModelConversion.ToString(row["pmora.codpais"]);
				TablePmoraPais.Value = (string)row["pmora.pais"];
				if (GlobalFunctions.emptyG(this.ValCodpmora) == 1)
				{
					this.ValCodpmora = "";
					TablePmoraPais.Value = "";
					Navigation.ClearValue("pmora");
				}
				else if (lazyLoad)
				{
					TablePmoraPais.SetPagination(1, 0, false, false, 1);
					TablePmoraPais.List = new SelectList(new List<SelectListItem>()
					{
						new SelectListItem
						{
							Value = Convert.ToString(this.ValCodpmora),
							Text = Convert.ToString(TablePmoraPais.Value),
							Selected = true
						}
					}, "Value", "Text", this.ValCodpmora);
				}

				TablePmoraPais.Selected = this.ValCodpmora;
			}
			catch (Exception ex)
			{
				CSGenio.framework.Log.Error(string.Format("FillDependant_Error (TablePmoraPais): {0}; {1}", ex.Message, ex.InnerException != null ? ex.InnerException.Message : ""));
			}
		}

		private readonly string[] _fieldsToSerialize_AGENTE__PMORAPAIS____ = ["Pmora", "Pmora.ValCodpais", "Pmora.ValZzstate", "Pmora.ValPais"];

		/// <summary>
		/// TablePnascPais -> (DB)
		/// </summary>
		/// <param name="qs"></param>
		/// <param name="lazyLoad">Lazy loading of dropdown items</param>
		public void Load_Agente__pnascpais____(NameValueCollection qs, bool lazyLoad = false)
		{
			bool agente__pnascpais____DoLoad = true;
			CriteriaSet agente__pnascpais____Conds = CriteriaSet.And();
			{
				object hValue = Navigation.GetValue("pnasc", true);
				if (hValue != null && !(hValue is Array) && !string.IsNullOrEmpty(Convert.ToString(hValue)))
				{
					agente__pnascpais____Conds.Equal(CSGenioApnasc.FldCodpais, hValue);
					this.ValCodpnasc = DBConversion.ToString(hValue);
				}
			}

			TablePnascPais = new TableDBEdit<Models.Pnasc>
			{
				IsLazyLoad = lazyLoad
			};

			if (lazyLoad)
			{
				if (Navigation.CurrentLevel.GetEntry("RETURN_pnasc") != null)
				{
					this.ValCodpnasc = Navigation.GetStrValue("RETURN_pnasc");
					Navigation.CurrentLevel.SetEntry("RETURN_pnasc", null);
				}
				FillDependant_AgenteTablePnascPais(lazyLoad);
				//Check if foreignkey comes from history
				TablePnascPais.FilledByHistory = Navigation.CheckFilledByHistory("pnasc");
				return;
			}

			if (agente__pnascpais____DoLoad)
			{
				List<ColumnSort> sorts = new List<ColumnSort>();
				ColumnSort requestedSort = GetRequestSort(TablePnascPais, "sTablePnascPais", "dTablePnascPais", qs, "pnasc");
				if (requestedSort != null)
					sorts.Add(requestedSort);

				string query = "";
				if (!string.IsNullOrEmpty(qs["TablePnascPais_tableFilters"]))
					TablePnascPais.TableFilters = bool.Parse(qs["TablePnascPais_tableFilters"]);
				else
					TablePnascPais.TableFilters = false;

				query = qs["qTablePnascPais"];

				//RS 26.07.2016 O preenchimento da lista de ajuda dos Dbedits passa a basear-se apenas no campo do próprio DbEdit
				// O interface de pesquisa rápida não fica coerente quando se visualiza apenas uma coluna mas a pesquisa faz matching com 5 ou 6 colunas diferentes
				//  tornando confuso to o user porque determinada row foi devolvida quando o Qresult não mostra como o matching foi feito
				CriteriaSet search_filters = CriteriaSet.And();
				if (!string.IsNullOrEmpty(query))
				{
					search_filters.Like(CSGenioApnasc.FldPais, query + "%");
				}
				agente__pnascpais____Conds.SubSet(search_filters);

				string tryParsePage = qs["pTablePnascPais"] != null ? qs["pTablePnascPais"].ToString() : "1";
				int page = !string.IsNullOrEmpty(tryParsePage) ? int.Parse(tryParsePage) : 1;
				int numberItems = CSGenio.framework.Configuration.NrRegDBedit;
				int offset = (page - 1) * numberItems;

				FieldRef[] fields = new FieldRef[] { CSGenioApnasc.FldCodpais, CSGenioApnasc.FldPais, CSGenioApnasc.FldZzstate };

// USE /[MANUAL PRO OVERRQ AGENTE_PNASCPAIS]/

				// Limitation by Zzstate
				/*
					Records that are currently being inserted or duplicated will also be included.
					Client-side persistence will try to fill the "text" value of that option.
				*/
				if (Navigation.checkFormMode("pnasc", FormMode.New) || Navigation.checkFormMode("pnasc", FormMode.Duplicate))
					agente__pnascpais____Conds.SubSet(CriteriaSet.Or()
						.Equal(CSGenioApnasc.FldZzstate, 0)
						.Equal(CSGenioApnasc.FldCodpais, Navigation.GetStrValue("pnasc")));
				else
					agente__pnascpais____Conds.Criterias.Add(new Criteria(new ColumnReference(CSGenioApnasc.FldZzstate), CriteriaOperator.Equal, 0));

				FieldRef firstVisibleColumn = new FieldRef("pnasc", "pais");
				ListingMVC<CSGenioApnasc> listing = Models.ModelBase.Where<CSGenioApnasc>(m_userContext, false, agente__pnascpais____Conds, fields, offset, numberItems, sorts, "LED_AGENTE__PNASCPAIS____", true, false, firstVisibleColumn: firstVisibleColumn);

				TablePnascPais.SetPagination(page, numberItems, listing.HasMore, listing.GetTotal, listing.TotalRecords);
				TablePnascPais.Query = query;
				TablePnascPais.Elements = listing.RowsForViewModel<GenioMVC.Models.Pnasc>((r) => new GenioMVC.Models.Pnasc(m_userContext, r, true, _fieldsToSerialize_AGENTE__PNASCPAIS____));

				//created by [ MH ] at [ 14.04.2016 ] - Foi alterada a forma de retornar a key do novo registo inserido / editado no form de apoio do DBEdit.
				//last update by [ MH ] at [ 10.05.2016 ] - Validação se key encontra-se no level atual, as chaves dos niveis anteriores devem ser ignorados.
				if (Navigation.CurrentLevel.GetEntry("RETURN_pnasc") != null)
				{
					this.ValCodpnasc = Navigation.GetStrValue("RETURN_pnasc");
					Navigation.CurrentLevel.SetEntry("RETURN_pnasc", null);
				}

				TablePnascPais.List = new SelectList(TablePnascPais.Elements.ToSelectList(x => x.ValPais, x => x.ValCodpais,  x => x.ValCodpais == this.ValCodpnasc), "Value", "Text", this.ValCodpnasc);
				FillDependant_AgenteTablePnascPais();

				//Check if foreignkey comes from history
				TablePnascPais.FilledByHistory = Navigation.CheckFilledByHistory("pnasc");
			}
		}

		/// <summary>
		/// Get Dependant fields values -> TablePnascPais (DB)
		/// </summary>
		/// <param name="PKey">Primary Key of Pnasc</param>
		public ConcurrentDictionary<string, object> GetDependant_AgenteTablePnascPais(string PKey)
		{
			FieldRef[] refDependantFields = [CSGenioApnasc.FldCodpais, CSGenioApnasc.FldPais];

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

			CSGenioApnasc tempArea = new(u);

			// Fields to select
			SelectQuery querySelect = new();
			querySelect.PageSize(1);
			foreach (FieldRef field in refDependantFields)
				querySelect.Select(field);

			querySelect.From(tempArea.QSystem, tempArea.TableName, tempArea.Alias)
				.Where(wherecodition.Equal(CSGenioApnasc.FldCodpais, PKey));

			string[] dependantFields = refDependantFields.Select(f => f.FullName).ToArray();
			QueryUtils.SetInnerJoins(dependantFields, null, tempArea, querySelect);

			ArrayList values = sp.executeReaderOneRow(querySelect);
			bool useDefaults = values.Count == 0;

			if (useDefaults)
				return GetViewModelFieldValues(refDependantFields);
			return GetViewModelFieldValues(refDependantFields, values);
		}

		/// <summary>
		/// Fill Dependant fields values -> TablePnascPais (DB)
		/// </summary>
		/// <param name="lazyLoad">Lazy loading of dropdown items</param>
		public void FillDependant_AgenteTablePnascPais(bool lazyLoad = false)
		{
			var row = GetDependant_AgenteTablePnascPais(this.ValCodpnasc);
			try
			{

				// Fill List fields
				this.ValCodpnasc = ViewModelConversion.ToString(row["pnasc.codpais"]);
				TablePnascPais.Value = (string)row["pnasc.pais"];
				if (GlobalFunctions.emptyG(this.ValCodpnasc) == 1)
				{
					this.ValCodpnasc = "";
					TablePnascPais.Value = "";
					Navigation.ClearValue("pnasc");
				}
				else if (lazyLoad)
				{
					TablePnascPais.SetPagination(1, 0, false, false, 1);
					TablePnascPais.List = new SelectList(new List<SelectListItem>()
					{
						new SelectListItem
						{
							Value = Convert.ToString(this.ValCodpnasc),
							Text = Convert.ToString(TablePnascPais.Value),
							Selected = true
						}
					}, "Value", "Text", this.ValCodpnasc);
				}

				TablePnascPais.Selected = this.ValCodpnasc;
			}
			catch (Exception ex)
			{
				CSGenio.framework.Log.Error(string.Format("FillDependant_Error (TablePnascPais): {0}; {1}", ex.Message, ex.InnerException != null ? ex.InnerException.Message : ""));
			}
		}

		private readonly string[] _fieldsToSerialize_AGENTE__PNASCPAIS____ = ["Pnasc", "Pnasc.ValCodpais", "Pnasc.ValZzstate", "Pnasc.ValPais"];

		protected override object GetViewModelValue(string identifier, object modelValue)
		{
			return identifier switch
			{
				"agent.codpmora" => ViewModelConversion.ToString(modelValue),
				"agent.codpnasc" => ViewModelConversion.ToString(modelValue),
				"agent.foto" => ViewModelConversion.ToImage(modelValue),
				"agent.nome" => ViewModelConversion.ToString(modelValue),
				"agent.dnascime" => ViewModelConversion.ToDateTime(modelValue),
				"agent.email" => ViewModelConversion.ToString(modelValue),
				"agent.telefone" => ViewModelConversion.ToString(modelValue),
				"agent.codagent" => ViewModelConversion.ToString(modelValue),
				"pmora.codpais" => ViewModelConversion.ToString(modelValue),
				"pmora.pais" => ViewModelConversion.ToString(modelValue),
				"pnasc.codpais" => ViewModelConversion.ToString(modelValue),
				"pnasc.pais" => ViewModelConversion.ToString(modelValue),
				_ => modelValue
			};
		}


		/// <inheritdoc/>
		protected override void SetTicketToImageFields()
		{
			if(ValFoto != null)
				ValFoto.Ticket = Helpers.Helpers.GetFileTicket(m_userContext.User, Area.AreaAGENT, CSGenioAagent.FldFoto.Field, null, ValCodagent);
		}

		#region Charts


		#endregion

		#region Custom code

// USE /[MANUAL PRO VIEWMODEL_CUSTOM AGENTE]/

		#endregion
	}
}
