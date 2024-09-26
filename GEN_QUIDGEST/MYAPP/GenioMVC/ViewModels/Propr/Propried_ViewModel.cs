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

namespace GenioMVC.ViewModels.Propr
{
	public class Propried_ViewModel : FormViewModel<Models.Propr>, IPreparableForSerialization
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
		/// Title: "Agente imobiliário responsável" | Type: "CE"
		/// </summary>
		public string ValCodagent { get; set; }
		/// <summary>
		/// Title: "Cidade" | Type: "CE"
		/// </summary>
		public string ValCodcidad { get; set; }

		#endregion
		/// <summary>
		/// Title: "ID" | Type: "N"
		/// </summary>
		[ValidateSetAccess]
		public decimal? ValIdpropre { get; set; }
		/// <summary>
		/// Title: "Vendida" | Type: "L"
		/// </summary>
		public bool ValVendida { get; set; }
		/// <summary>
		/// Title: "Fotografia" | Type: "IJ"
		/// </summary>
		[ImageThumbnailJsonConverter(30, 50)]
		public GenioMVC.Models.ImageModel ValFoto { get; set; }
		/// <summary>
		/// Title: "Título" | Type: "C"
		/// </summary>
		public string ValTitulo { get; set; }
		/// <summary>
		/// Title: "Preço" | Type: "$"
		/// </summary>
		public decimal? ValPreco { get; set; }
		/// <summary>
		/// Title: "Descrição" | Type: "MO"
		/// </summary>
		public string ValDescrica { get; set; }
		/// <summary>
		/// Title: "Cidade" | Type: "C"
		/// </summary>
		[ValidateSetAccess]
		public TableDBEdit<GenioMVC.Models.Cidad> TableCidadCidade { get; set; }
		/// <summary>
		/// Title: "País" | Type: "C"
		/// </summary>
		[ValidateSetAccess]
		public string CidadPaisValPais 
		{
			get
			{
				return funcCidadPaisValPais != null ? funcCidadPaisValPais() : _auxCidadPaisValPais;
			}
			set { funcCidadPaisValPais = () => value; }
		}

		[JsonIgnore]
		public Func<string> funcCidadPaisValPais { get; set; }

		private string _auxCidadPaisValPais { get; set; }
		/// <summary>
		/// Title: "Localização" | Type: "GG"
		/// </summary>
		public string ValLocaliza { get; set; }
		/// <summary>
		/// Title: "Tipo de construção" | Type: "AC"
		/// </summary>
		public string ValTipoprop { get; set; }
		/// <summary>
		/// Title: "" | Type: "PSEUD"
		/// </summary>
		[JsonIgnore]
		public SelectList List_ValTipoprop { get; set; }
		/// <summary>
		/// Title: "Espaço exterior (m2)" | Type: "N"
		/// </summary>
		public decimal? ValEspexter { get; set; }
		/// <summary>
		/// Title: "Tipologia" | Type: "AN"
		/// </summary>
		public decimal ValTipologi { get; set; }
		/// <summary>
		/// Title: "" | Type: "PSEUD"
		/// </summary>
		[JsonIgnore]
		public SelectList List_ValTipologi { get; set; }
		/// <summary>
		/// Title: "Tamanho (m2)" | Type: "N"
		/// </summary>
		public decimal? ValTamanho { get; set; }
		/// <summary>
		/// Title: "Número de casas de banho" | Type: "N"
		/// </summary>
		public decimal? ValNr_wcs { get; set; }
		/// <summary>
		/// Title: "Data de contrução" | Type: "D"
		/// </summary>
		public DateTime? ValDtconst { get; set; }
		/// <summary>
		/// Title: "Idade da construção" | Type: "N"
		/// </summary>
		[ValidateSetAccess]
		public decimal? ValIdadepro { get; set; }
		/// <summary>
		/// Title: "Agente imobiliário responsável" | Type: "C"
		/// </summary>
		[ValidateSetAccess]
		public TableDBEdit<GenioMVC.Models.Agent> TableAgentNome { get; set; }
		/// <summary>
		/// Title: "Fotografia" | Type: "IJ"
		/// </summary>
		[ImageThumbnailJsonConverter(30, 50)]
		[ValidateSetAccess]
		public GenioMVC.Models.ImageModel AgentValFoto 
		{
			get
			{
				return funcAgentValFoto != null ? funcAgentValFoto() : _auxAgentValFoto;
			}
			set { funcAgentValFoto = () => value; }
		}

		[JsonIgnore]
		public Func<GenioMVC.Models.ImageModel> funcAgentValFoto { get; set; }

		private GenioMVC.Models.ImageModel _auxAgentValFoto { get; set; }
		/// <summary>
		/// Title: "E-mail" | Type: "C"
		/// </summary>
		[ValidateSetAccess]
		public string AgentValEmail 
		{
			get
			{
				return funcAgentValEmail != null ? funcAgentValEmail() : _auxAgentValEmail;
			}
			set { funcAgentValEmail = () => value; }
		}

		[JsonIgnore]
		public Func<string> funcAgentValEmail { get; set; }

		private string _auxAgentValEmail { get; set; }

		#region Navigations
		#endregion

		#region Auxiliar Keys for Image controls



		#endregion

		#region Extra database fields



		#endregion

		#region Fields for formulas


		#endregion

		public string ValCodpropr { get; set; }


		/// <summary>
		/// FOR DESERIALIZATION ONLY
		/// A call to Init() needs to be manually invoked after this constructor
		/// </summary>
		[Obsolete("For deserialization only")]
		public Propried_ViewModel() : base(null!) { }

		public Propried_ViewModel(UserContext userContext, bool nestedForm = false) : base(userContext, "FPROPRIED", nestedForm) { }

		public Propried_ViewModel(UserContext userContext, Models.Propr row, bool nestedForm = false) : base(userContext, "FPROPRIED", row, nestedForm) { }

		public Propried_ViewModel(UserContext userContext, string id, bool nestedForm = false, string[]? fieldsToLoad = null) : this(userContext, nestedForm)
		{
			this.Navigation.SetValue("propr", id);
			Model = Models.Propr.Find(id, userContext, "FPROPRIED", fieldsToQuery: fieldsToLoad);
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
			Models.Propr model = new Models.Propr(userContext) { Identifier = "FPROPRIED" };

			var navigation = m_userContext.CurrentNavigation;
			// The "LoadKeysFromHistory" must be after the "LoadEPH" because the PHE's in the tree mark Foreign Keys to null
			// (since they cannot assign multiple values to a single field) and thus the value that comes from Navigation is lost.
			// And this makes it more like the order of loading the model when opening the form.
			model.LoadEPH("FPROPRIED");
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
			Models.Propr model = Model;
			StatusMessage result = new StatusMessage(Status.OK, "");
			return result;
		}

		public StatusMessage EvaluateTableConditions(ConditionType type)
		{
			return Model.EvaluateTableConditions(type);
		}

		#endregion

		#region Mapper

		public override void MapFromModel(Models.Propr m)
		{
			if (m == null)
			{
				CSGenio.framework.Log.Error("Map Model (Propr) to ViewModel (Propried) - Model is a null reference");
				throw new ModelNotFoundException("Model not found");
			}

			try
			{
				ValCodagent = ViewModelConversion.ToString(m.ValCodagent);
				ValCodcidad = ViewModelConversion.ToString(m.ValCodcidad);
				ValIdpropre = ViewModelConversion.ToNumeric(m.ValIdpropre);
				ValVendida = ViewModelConversion.ToLogic(m.ValVendida);
				ValFoto = ViewModelConversion.ToImage(m.ValFoto);
				ValTitulo = ViewModelConversion.ToString(m.ValTitulo);
				ValPreco = ViewModelConversion.ToNumeric(m.ValPreco);
				ValDescrica = ViewModelConversion.ToString(m.ValDescrica);
				ValLocaliza = ViewModelConversion.ToString(m.ValLocaliza);
				ValTipoprop = ViewModelConversion.ToString(m.ValTipoprop);
				ValEspexter = ViewModelConversion.ToNumeric(m.ValEspexter);
				ValTipologi = ViewModelConversion.ToNumeric(m.ValTipologi);
				ValTamanho = ViewModelConversion.ToNumeric(m.ValTamanho);
				ValNr_wcs = ViewModelConversion.ToNumeric(m.ValNr_wcs);
				ValDtconst = ViewModelConversion.ToDateTime(m.ValDtconst);
				ValIdadepro = ViewModelConversion.ToNumeric(m.ValIdadepro);
				funcAgentValFoto = () => ViewModelConversion.ToImage(m.Agent.ValFoto);
				funcAgentValEmail = () => ViewModelConversion.ToString(m.Agent.ValEmail);
				ValCodpropr = ViewModelConversion.ToString(m.ValCodpropr);
			}
			catch (Exception)
			{
				CSGenio.framework.Log.Error("Map Model (Propr) to ViewModel (Propried) - Error during mapping");
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
		public override void MapToModel(Models.Propr m)
		{
			if (m == null)
			{
				CSGenio.framework.Log.Error("Map ViewModel (Propried) to Model (Propr) - Model is a null reference");
				throw new ModelNotFoundException("Model not found");
			}

			try
			{

				m.ValCodagent = ViewModelConversion.ToString(ValCodagent);

				m.ValCodcidad = ViewModelConversion.ToString(ValCodcidad);

				m.ValVendida = ViewModelConversion.ToLogic(ValVendida);

				if (ValFoto == null || !ValFoto.IsThumbnail)
					m.ValFoto = ViewModelConversion.ToImage(ValFoto);

				m.ValTitulo = ViewModelConversion.ToString(ValTitulo);

				m.ValPreco = ViewModelConversion.ToNumeric(ValPreco);

				m.ValDescrica = ViewModelConversion.ToString(ValDescrica);

				m.ValLocaliza = ViewModelConversion.ToString(ValLocaliza);

				m.ValTipoprop = ViewModelConversion.ToString(ValTipoprop);

				m.ValEspexter = ViewModelConversion.ToNumeric(ValEspexter);

				m.ValTipologi = ViewModelConversion.ToNumeric(ValTipologi);

				m.ValTamanho = ViewModelConversion.ToNumeric(ValTamanho);

				m.ValNr_wcs = ViewModelConversion.ToNumeric(ValNr_wcs);

				m.ValDtconst = ViewModelConversion.ToDateTime(ValDtconst);

				m.ValCodpropr = ViewModelConversion.ToString(ValCodpropr);

				/*
					At this moment, in the case of runtime calculation of server-side formulas, to improve performance and reduce database load,
						the values coming from the client-side will be accepted as valid, since they will not be saved and are only being used for calculation.
				*/
				if (!HasDisabledUserValuesSecurity)
					return;


				m.ValIdpropre = ViewModelConversion.ToNumeric(ValIdpropre);

				m.ValIdadepro = ViewModelConversion.ToNumeric(ValIdadepro);
			}
			catch (Exception)
			{
				CSGenio.framework.Log.Error($"Map ViewModel (Propried) to Model (Propr) - Error during mapping. All user values: {HasDisabledUserValuesSecurity}");
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
					case "propr.codagent":
						this.ValCodagent = ViewModelConversion.ToString(_value);
						break;
					case "propr.codcidad":
						this.ValCodcidad = ViewModelConversion.ToString(_value);
						break;
					case "propr.vendida":
						this.ValVendida = ViewModelConversion.ToLogic(_value);
						break;
					case "propr.foto":
						this.ValFoto = ViewModelConversion.ToImage(_value);
						break;
					case "propr.titulo":
						this.ValTitulo = ViewModelConversion.ToString(_value);
						break;
					case "propr.preco":
						this.ValPreco = ViewModelConversion.ToNumeric(_value);
						break;
					case "propr.descrica":
						this.ValDescrica = ViewModelConversion.ToString(_value);
						break;
					case "propr.localiza":
						this.ValLocaliza = ViewModelConversion.ToString(_value);
						break;
					case "propr.tipoprop":
						this.ValTipoprop = ViewModelConversion.ToString(_value);
						break;
					case "propr.espexter":
						this.ValEspexter = ViewModelConversion.ToNumeric(_value);
						break;
					case "propr.tipologi":
						this.ValTipologi = ViewModelConversion.ToNumeric(_value);
						break;
					case "propr.tamanho":
						this.ValTamanho = ViewModelConversion.ToNumeric(_value);
						break;
					case "propr.nr_wcs":
						this.ValNr_wcs = ViewModelConversion.ToNumeric(_value);
						break;
					case "propr.dtconst":
						this.ValDtconst = ViewModelConversion.ToDateTime(_value);
						break;
					case "propr.codpropr":
						this.ValCodpropr = ViewModelConversion.ToString(_value);
						break;
					default:
						Log.Error($"SetViewModelValue (Propried) - Unexpected field identifier {fullFieldName}");
						break;
				}
			}
			catch (Exception ex)
			{
				throw new FrameworkException(Resources.Resources.PEDIMOS_DESCULPA__OC63848, "SetViewModelValue (Propried)", "Unexpected error", ex);
			}
		}

		#endregion

		/// <summary>
		/// Reads the Model from the database based on the key that is in the history or that was passed through the parameter
		/// </summary>
		/// <param name="id">The primary key of the record that needs to be read from the database. Leave NULL to use the value from the History.</param>
		public override void LoadModel(string id = null)
		{
			try { Model = Models.Propr.Find(id ?? Navigation.GetStrValue("propr"), m_userContext, "FPROPRIED"); }
			finally { Model ??= new Models.Propr(m_userContext) { Identifier = "FPROPRIED" }; }

			base.LoadModel();
		}

		public override void Load(NameValueCollection qs, bool editable, bool ajaxRequest = false, bool lazyLoad = false)
		{
			this.editable = editable;
			CSGenio.business.Area oldvalues = null;

			// TODO: Deve ser substituido por search do CSGenioA
			try
			{
				Model = Models.Propr.Find(Navigation.GetStrValue("propr"), m_userContext, "FPROPRIED");
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

			Model.Identifier = "FPROPRIED";
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

		protected override void LoadDocumentsProperties(Models.Propr row)
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
				Model = Models.Propr.Find(Navigation.GetStrValue("propr"), m_userContext, "FPROPRIED");
				if (Model == null)
				{
					Model = new Models.Propr(m_userContext) { Identifier = "FPROPRIED" };
					Model.klass.QPrimaryKey = Navigation.GetStrValue("propr");
				}
				MapToModel(Model);
				LoadDocumentsProperties(Model);
			}
			// Add characteristics
			Characs = new List<string>();

			Load_Propriedcidadcidade__(qs, lazyLoad);
			Load_Propriedagentnome____(qs, lazyLoad);
// USE /[MANUAL PRO VIEWMODEL_LOADPARTIAL PROPRIED]/
		}

// USE /[MANUAL PRO VIEWMODEL_NEW PROPRIED]/

		// Preencher Qvalues default dos fields do form
		protected override void LoadDefaultValues()
		{
		}

		public override CrudViewModelValidationResult Validate()
		{
			CrudViewModelFieldValidator validator = new(m_userContext.User.Language);

			validator.StringLength("ValTitulo", Resources.Resources.TITULO39021, ValTitulo, 80);
			validator.StringLength("CidadPaisValPais", Resources.Resources.PAIS58483, CidadPaisValPais, 50);
			validator.StringLength("AgentValEmail", Resources.Resources.E_MAIL42251, AgentValEmail, 80);

			return validator.GetResult();
		}

// USE /[MANUAL PRO VIEWMODEL_SAVE PROPRIED]/
		public override void Save()
		{


			base.Save();
		}

// USE /[MANUAL PRO VIEWMODEL_APPLY PROPRIED]/

// USE /[MANUAL PRO VIEWMODEL_DUPLICATE PROPRIED]/

// USE /[MANUAL PRO VIEWMODEL_DESTROY PROPRIED]/
		public override void Destroy(string id)
		{
			Model = Models.Propr.Find(id, m_userContext, "FPROPRIED");
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
		/// TableCidadCidade -> (DB)
		/// </summary>
		/// <param name="qs"></param>
		/// <param name="lazyLoad">Lazy loading of dropdown items</param>
		public void Load_Propriedcidadcidade__(NameValueCollection qs, bool lazyLoad = false)
		{
			bool propriedcidadcidade__DoLoad = true;
			CriteriaSet propriedcidadcidade__Conds = CriteriaSet.And();
			{
				object hValue = Navigation.GetValue("cidad", true);
				if (hValue != null && !(hValue is Array) && !string.IsNullOrEmpty(Convert.ToString(hValue)))
				{
					propriedcidadcidade__Conds.Equal(CSGenioAcidad.FldCodcidad, hValue);
					this.ValCodcidad = DBConversion.ToString(hValue);
				}
			}

			TableCidadCidade = new TableDBEdit<Models.Cidad>
			{
				IsLazyLoad = lazyLoad
			};

			if (lazyLoad)
			{
				if (Navigation.CurrentLevel.GetEntry("RETURN_cidad") != null)
				{
					this.ValCodcidad = Navigation.GetStrValue("RETURN_cidad");
					Navigation.CurrentLevel.SetEntry("RETURN_cidad", null);
				}
				FillDependant_PropriedTableCidadCidade(lazyLoad);
				//Check if foreignkey comes from history
				TableCidadCidade.FilledByHistory = Navigation.CheckFilledByHistory("cidad");
				return;
			}

			if (propriedcidadcidade__DoLoad)
			{
				List<ColumnSort> sorts = new List<ColumnSort>();
				ColumnSort requestedSort = GetRequestSort(TableCidadCidade, "sTableCidadCidade", "dTableCidadCidade", qs, "cidad");
				if (requestedSort != null)
					sorts.Add(requestedSort);

				string query = "";
				if (!string.IsNullOrEmpty(qs["TableCidadCidade_tableFilters"]))
					TableCidadCidade.TableFilters = bool.Parse(qs["TableCidadCidade_tableFilters"]);
				else
					TableCidadCidade.TableFilters = false;

				query = qs["qTableCidadCidade"];

				//RS 26.07.2016 O preenchimento da lista de ajuda dos Dbedits passa a basear-se apenas no campo do próprio DbEdit
				// O interface de pesquisa rápida não fica coerente quando se visualiza apenas uma coluna mas a pesquisa faz matching com 5 ou 6 colunas diferentes
				//  tornando confuso to o user porque determinada row foi devolvida quando o Qresult não mostra como o matching foi feito
				CriteriaSet search_filters = CriteriaSet.And();
				if (!string.IsNullOrEmpty(query))
				{
					search_filters.Like(CSGenioAcidad.FldCidade, query + "%");
				}
				propriedcidadcidade__Conds.SubSet(search_filters);

				string tryParsePage = qs["pTableCidadCidade"] != null ? qs["pTableCidadCidade"].ToString() : "1";
				int page = !string.IsNullOrEmpty(tryParsePage) ? int.Parse(tryParsePage) : 1;
				int numberItems = CSGenio.framework.Configuration.NrRegDBedit;
				int offset = (page - 1) * numberItems;

				FieldRef[] fields = new FieldRef[] { CSGenioAcidad.FldCodcidad, CSGenioAcidad.FldCidade, CSGenioAcidad.FldZzstate };

// USE /[MANUAL PRO OVERRQ PROPRIED_CIDADCIDADE]/

				// Limitation by Zzstate
				/*
					Records that are currently being inserted or duplicated will also be included.
					Client-side persistence will try to fill the "text" value of that option.
				*/
				if (Navigation.checkFormMode("cidad", FormMode.New) || Navigation.checkFormMode("cidad", FormMode.Duplicate))
					propriedcidadcidade__Conds.SubSet(CriteriaSet.Or()
						.Equal(CSGenioAcidad.FldZzstate, 0)
						.Equal(CSGenioAcidad.FldCodcidad, Navigation.GetStrValue("cidad")));
				else
					propriedcidadcidade__Conds.Criterias.Add(new Criteria(new ColumnReference(CSGenioAcidad.FldZzstate), CriteriaOperator.Equal, 0));

				FieldRef firstVisibleColumn = new FieldRef("cidad", "cidade");
				ListingMVC<CSGenioAcidad> listing = Models.ModelBase.Where<CSGenioAcidad>(m_userContext, false, propriedcidadcidade__Conds, fields, offset, numberItems, sorts, "LED_PROPRIEDCIDADCIDADE__", true, false, firstVisibleColumn: firstVisibleColumn);

				TableCidadCidade.SetPagination(page, numberItems, listing.HasMore, listing.GetTotal, listing.TotalRecords);
				TableCidadCidade.Query = query;
				TableCidadCidade.Elements = listing.RowsForViewModel<GenioMVC.Models.Cidad>((r) => new GenioMVC.Models.Cidad(m_userContext, r, true, _fieldsToSerialize_PROPRIEDCIDADCIDADE__));

				//created by [ MH ] at [ 14.04.2016 ] - Foi alterada a forma de retornar a key do novo registo inserido / editado no form de apoio do DBEdit.
				//last update by [ MH ] at [ 10.05.2016 ] - Validação se key encontra-se no level atual, as chaves dos niveis anteriores devem ser ignorados.
				if (Navigation.CurrentLevel.GetEntry("RETURN_cidad") != null)
				{
					this.ValCodcidad = Navigation.GetStrValue("RETURN_cidad");
					Navigation.CurrentLevel.SetEntry("RETURN_cidad", null);
				}

				TableCidadCidade.List = new SelectList(TableCidadCidade.Elements.ToSelectList(x => x.ValCidade, x => x.ValCodcidad,  x => x.ValCodcidad == this.ValCodcidad), "Value", "Text", this.ValCodcidad);
				FillDependant_PropriedTableCidadCidade();

				//Check if foreignkey comes from history
				TableCidadCidade.FilledByHistory = Navigation.CheckFilledByHistory("cidad");
			}
		}

		/// <summary>
		/// Get Dependant fields values -> TableCidadCidade (DB)
		/// </summary>
		/// <param name="PKey">Primary Key of Cidad</param>
		public ConcurrentDictionary<string, object> GetDependant_PropriedTableCidadCidade(string PKey)
		{
			FieldRef[] refDependantFields = [CSGenioAcidad.FldCodcidad, CSGenioAcidad.FldCidade, CSGenioApais.FldCodpais, CSGenioApais.FldPais];

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

			CSGenioAcidad tempArea = new(u);

			// Fields to select
			SelectQuery querySelect = new();
			querySelect.PageSize(1);
			foreach (FieldRef field in refDependantFields)
				querySelect.Select(field);

			querySelect.From(tempArea.QSystem, tempArea.TableName, tempArea.Alias)
				.Where(wherecodition.Equal(CSGenioAcidad.FldCodcidad, PKey));

			string[] dependantFields = refDependantFields.Select(f => f.FullName).ToArray();
			QueryUtils.SetInnerJoins(dependantFields, null, tempArea, querySelect);

			ArrayList values = sp.executeReaderOneRow(querySelect);
			bool useDefaults = values.Count == 0;

			if (useDefaults)
				return GetViewModelFieldValues(refDependantFields);
			return GetViewModelFieldValues(refDependantFields, values);
		}

		/// <summary>
		/// Fill Dependant fields values -> TableCidadCidade (DB)
		/// </summary>
		/// <param name="lazyLoad">Lazy loading of dropdown items</param>
		public void FillDependant_PropriedTableCidadCidade(bool lazyLoad = false)
		{
			var row = GetDependant_PropriedTableCidadCidade(this.ValCodcidad);
			try
			{
				this.funcCidadPaisValPais = () => (string)row["pais.pais"];

				// Fill List fields
				this.ValCodcidad = ViewModelConversion.ToString(row["cidad.codcidad"]);
				TableCidadCidade.Value = (string)row["cidad.cidade"];
				if (GlobalFunctions.emptyG(this.ValCodcidad) == 1)
				{
					this.ValCodcidad = "";
					TableCidadCidade.Value = "";
					Navigation.ClearValue("cidad");
				}
				else if (lazyLoad)
				{
					TableCidadCidade.SetPagination(1, 0, false, false, 1);
					TableCidadCidade.List = new SelectList(new List<SelectListItem>()
					{
						new SelectListItem
						{
							Value = Convert.ToString(this.ValCodcidad),
							Text = Convert.ToString(TableCidadCidade.Value),
							Selected = true
						}
					}, "Value", "Text", this.ValCodcidad);
				}

				TableCidadCidade.Selected = this.ValCodcidad;
			}
			catch (Exception ex)
			{
				CSGenio.framework.Log.Error(string.Format("FillDependant_Error (TableCidadCidade): {0}; {1}", ex.Message, ex.InnerException != null ? ex.InnerException.Message : ""));
			}
		}

		private readonly string[] _fieldsToSerialize_PROPRIEDCIDADCIDADE__ = ["Cidad", "Cidad.ValCodcidad", "Cidad.ValZzstate", "Cidad.ValCidade"];

		/// <summary>
		/// TableAgentNome -> (DB)
		/// </summary>
		/// <param name="qs"></param>
		/// <param name="lazyLoad">Lazy loading of dropdown items</param>
		public void Load_Propriedagentnome____(NameValueCollection qs, bool lazyLoad = false)
		{
			bool propriedagentnome____DoLoad = true;
			CriteriaSet propriedagentnome____Conds = CriteriaSet.And();
			{
				object hValue = Navigation.GetValue("agent", true);
				if (hValue != null && !(hValue is Array) && !string.IsNullOrEmpty(Convert.ToString(hValue)))
				{
					propriedagentnome____Conds.Equal(CSGenioAagent.FldCodagent, hValue);
					this.ValCodagent = DBConversion.ToString(hValue);
				}
			}

			TableAgentNome = new TableDBEdit<Models.Agent>
			{
				IsLazyLoad = lazyLoad
			};

			if (lazyLoad)
			{
				if (Navigation.CurrentLevel.GetEntry("RETURN_agent") != null)
				{
					this.ValCodagent = Navigation.GetStrValue("RETURN_agent");
					Navigation.CurrentLevel.SetEntry("RETURN_agent", null);
				}
				FillDependant_PropriedTableAgentNome(lazyLoad);
				//Check if foreignkey comes from history
				TableAgentNome.FilledByHistory = Navigation.CheckFilledByHistory("agent");
				return;
			}

			if (propriedagentnome____DoLoad)
			{
				List<ColumnSort> sorts = new List<ColumnSort>();
				ColumnSort requestedSort = GetRequestSort(TableAgentNome, "sTableAgentNome", "dTableAgentNome", qs, "agent");
				if (requestedSort != null)
					sorts.Add(requestedSort);

				string query = "";
				if (!string.IsNullOrEmpty(qs["TableAgentNome_tableFilters"]))
					TableAgentNome.TableFilters = bool.Parse(qs["TableAgentNome_tableFilters"]);
				else
					TableAgentNome.TableFilters = false;

				query = qs["qTableAgentNome"];

				//RS 26.07.2016 O preenchimento da lista de ajuda dos Dbedits passa a basear-se apenas no campo do próprio DbEdit
				// O interface de pesquisa rápida não fica coerente quando se visualiza apenas uma coluna mas a pesquisa faz matching com 5 ou 6 colunas diferentes
				//  tornando confuso to o user porque determinada row foi devolvida quando o Qresult não mostra como o matching foi feito
				CriteriaSet search_filters = CriteriaSet.And();
				if (!string.IsNullOrEmpty(query))
				{
					search_filters.Like(CSGenioAagent.FldNome, query + "%");
				}
				propriedagentnome____Conds.SubSet(search_filters);

				string tryParsePage = qs["pTableAgentNome"] != null ? qs["pTableAgentNome"].ToString() : "1";
				int page = !string.IsNullOrEmpty(tryParsePage) ? int.Parse(tryParsePage) : 1;
				int numberItems = CSGenio.framework.Configuration.NrRegDBedit;
				int offset = (page - 1) * numberItems;

				FieldRef[] fields = new FieldRef[] { CSGenioAagent.FldCodagent, CSGenioAagent.FldNome, CSGenioAagent.FldEmail, CSGenioAagent.FldZzstate };

// USE /[MANUAL PRO OVERRQ PROPRIED_AGENTNOME]/

				// Limitation by Zzstate
				/*
					Records that are currently being inserted or duplicated will also be included.
					Client-side persistence will try to fill the "text" value of that option.
				*/
				if (Navigation.checkFormMode("agent", FormMode.New) || Navigation.checkFormMode("agent", FormMode.Duplicate))
					propriedagentnome____Conds.SubSet(CriteriaSet.Or()
						.Equal(CSGenioAagent.FldZzstate, 0)
						.Equal(CSGenioAagent.FldCodagent, Navigation.GetStrValue("agent")));
				else
					propriedagentnome____Conds.Criterias.Add(new Criteria(new ColumnReference(CSGenioAagent.FldZzstate), CriteriaOperator.Equal, 0));

				FieldRef firstVisibleColumn = new FieldRef("agent", "email");
				ListingMVC<CSGenioAagent> listing = Models.ModelBase.Where<CSGenioAagent>(m_userContext, false, propriedagentnome____Conds, fields, offset, numberItems, sorts, "LED_PROPRIEDAGENTNOME____", true, false, firstVisibleColumn: firstVisibleColumn);

				TableAgentNome.SetPagination(page, numberItems, listing.HasMore, listing.GetTotal, listing.TotalRecords);
				TableAgentNome.Query = query;
				TableAgentNome.Elements = listing.RowsForViewModel<GenioMVC.Models.Agent>((r) => new GenioMVC.Models.Agent(m_userContext, r, true, _fieldsToSerialize_PROPRIEDAGENTNOME____));

				//created by [ MH ] at [ 14.04.2016 ] - Foi alterada a forma de retornar a key do novo registo inserido / editado no form de apoio do DBEdit.
				//last update by [ MH ] at [ 10.05.2016 ] - Validação se key encontra-se no level atual, as chaves dos niveis anteriores devem ser ignorados.
				if (Navigation.CurrentLevel.GetEntry("RETURN_agent") != null)
				{
					this.ValCodagent = Navigation.GetStrValue("RETURN_agent");
					Navigation.CurrentLevel.SetEntry("RETURN_agent", null);
				}

				TableAgentNome.List = new SelectList(TableAgentNome.Elements.ToSelectList(x => x.ValNome, x => x.ValCodagent,  x => x.ValCodagent == this.ValCodagent), "Value", "Text", this.ValCodagent);
				FillDependant_PropriedTableAgentNome();

				//Check if foreignkey comes from history
				TableAgentNome.FilledByHistory = Navigation.CheckFilledByHistory("agent");
			}
		}

		/// <summary>
		/// Get Dependant fields values -> TableAgentNome (DB)
		/// </summary>
		/// <param name="PKey">Primary Key of Agent</param>
		public ConcurrentDictionary<string, object> GetDependant_PropriedTableAgentNome(string PKey)
		{
			FieldRef[] refDependantFields = [CSGenioAagent.FldCodagent, CSGenioAagent.FldNome, CSGenioAagent.FldFoto, CSGenioAagent.FldEmail];

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

			CSGenioAagent tempArea = new(u);

			// Fields to select
			SelectQuery querySelect = new();
			querySelect.PageSize(1);
			foreach (FieldRef field in refDependantFields)
				querySelect.Select(field);

			querySelect.From(tempArea.QSystem, tempArea.TableName, tempArea.Alias)
				.Where(wherecodition.Equal(CSGenioAagent.FldCodagent, PKey));

			string[] dependantFields = refDependantFields.Select(f => f.FullName).ToArray();
			QueryUtils.SetInnerJoins(dependantFields, null, tempArea, querySelect);

			ArrayList values = sp.executeReaderOneRow(querySelect);
			bool useDefaults = values.Count == 0;

			if (useDefaults)
				return GetViewModelFieldValues(refDependantFields);
			return GetViewModelFieldValues(refDependantFields, values);
		}

		/// <summary>
		/// Fill Dependant fields values -> TableAgentNome (DB)
		/// </summary>
		/// <param name="lazyLoad">Lazy loading of dropdown items</param>
		public void FillDependant_PropriedTableAgentNome(bool lazyLoad = false)
		{
			var row = GetDependant_PropriedTableAgentNome(this.ValCodagent);
			try
			{
				this.funcAgentValFoto = () => (GenioMVC.Models.ImageModel)row["agent.foto"];
				this.funcAgentValEmail = () => (string)row["agent.email"];

				// Fill List fields
				this.ValCodagent = ViewModelConversion.ToString(row["agent.codagent"]);
				TableAgentNome.Value = (string)row["agent.nome"];
				if (GlobalFunctions.emptyG(this.ValCodagent) == 1)
				{
					this.ValCodagent = "";
					TableAgentNome.Value = "";
					Navigation.ClearValue("agent");
				}
				else if (lazyLoad)
				{
					TableAgentNome.SetPagination(1, 0, false, false, 1);
					TableAgentNome.List = new SelectList(new List<SelectListItem>()
					{
						new SelectListItem
						{
							Value = Convert.ToString(this.ValCodagent),
							Text = Convert.ToString(TableAgentNome.Value),
							Selected = true
						}
					}, "Value", "Text", this.ValCodagent);
				}

				TableAgentNome.Selected = this.ValCodagent;
			}
			catch (Exception ex)
			{
				CSGenio.framework.Log.Error(string.Format("FillDependant_Error (TableAgentNome): {0}; {1}", ex.Message, ex.InnerException != null ? ex.InnerException.Message : ""));
			}
		}

		private readonly string[] _fieldsToSerialize_PROPRIEDAGENTNOME____ = ["Agent", "Agent.ValCodagent", "Agent.ValZzstate", "Agent.ValEmail", "Agent.ValNome"];

		protected override object GetViewModelValue(string identifier, object modelValue)
		{
			return identifier switch
			{
				"propr.codagent" => ViewModelConversion.ToString(modelValue),
				"propr.codcidad" => ViewModelConversion.ToString(modelValue),
				"propr.idpropre" => ViewModelConversion.ToNumeric(modelValue),
				"propr.vendida" => ViewModelConversion.ToLogic(modelValue),
				"propr.foto" => ViewModelConversion.ToImage(modelValue),
				"propr.titulo" => ViewModelConversion.ToString(modelValue),
				"propr.preco" => ViewModelConversion.ToNumeric(modelValue),
				"propr.descrica" => ViewModelConversion.ToString(modelValue),
				"pais.pais" => ViewModelConversion.ToString(modelValue),
				"propr.localiza" => ViewModelConversion.ToString(modelValue),
				"propr.tipoprop" => ViewModelConversion.ToString(modelValue),
				"propr.espexter" => ViewModelConversion.ToNumeric(modelValue),
				"propr.tipologi" => ViewModelConversion.ToNumeric(modelValue),
				"propr.tamanho" => ViewModelConversion.ToNumeric(modelValue),
				"propr.nr_wcs" => ViewModelConversion.ToNumeric(modelValue),
				"propr.dtconst" => ViewModelConversion.ToDateTime(modelValue),
				"propr.idadepro" => ViewModelConversion.ToNumeric(modelValue),
				"agent.foto" => ViewModelConversion.ToImage(modelValue),
				"agent.email" => ViewModelConversion.ToString(modelValue),
				"propr.codpropr" => ViewModelConversion.ToString(modelValue),
				"cidad.codcidad" => ViewModelConversion.ToString(modelValue),
				"cidad.cidade" => ViewModelConversion.ToString(modelValue),
				"pais.codpais" => ViewModelConversion.ToString(modelValue),
				"agent.codagent" => ViewModelConversion.ToString(modelValue),
				"agent.nome" => ViewModelConversion.ToString(modelValue),
				_ => modelValue
			};
		}


		/// <inheritdoc/>
		protected override void SetTicketToImageFields()
		{
			if(ValFoto != null)
				ValFoto.Ticket = Helpers.Helpers.GetFileTicket(m_userContext.User, Area.AreaPROPR, CSGenioApropr.FldFoto.Field, null, ValCodpropr);
			if(AgentValFoto != null)
				AgentValFoto.Ticket = Helpers.Helpers.GetFileTicket(m_userContext.User, Area.AreaAGENT, CSGenioAagent.FldFoto.Field, null, ValCodagent);
		}

		#region Charts


		#endregion

		#region Custom code

// USE /[MANUAL PRO VIEWMODEL_CUSTOM PROPRIED]/

		#endregion
	}
}
