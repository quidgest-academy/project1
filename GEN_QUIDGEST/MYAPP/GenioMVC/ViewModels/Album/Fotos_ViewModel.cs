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

namespace GenioMVC.ViewModels.Album
{
	public class Fotos_ViewModel : FormViewModel<Models.Album>, IPreparableForSerialization
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
		/// Title: "Propriedade" | Type: "CE"
		/// </summary>
		public string ValCodpropr { get; set; }

		#endregion
		/// <summary>
		/// Title: "Foto" | Type: "IJ"
		/// </summary>
		[ImageThumbnailJsonConverter(100, 50)]
		public GenioMVC.Models.ImageModel ValFoto { get; set; }
		/// <summary>
		/// Title: "Título" | Type: "C"
		/// </summary>
		public string ValTitulo { get; set; }
		/// <summary>
		/// Title: "Propriedade" | Type: "C"
		/// </summary>
		[ValidateSetAccess]
		public TableDBEdit<GenioMVC.Models.Propr> TableProprTitulo { get; set; }

		#region Navigations
		#endregion

		#region Auxiliar Keys for Image controls



		#endregion

		#region Extra database fields



		#endregion

		#region Fields for formulas


		#endregion

		public string ValCodalbum { get; set; }


		/// <summary>
		/// FOR DESERIALIZATION ONLY
		/// A call to Init() needs to be manually invoked after this constructor
		/// </summary>
		[Obsolete("For deserialization only")]
		public Fotos_ViewModel() : base(null!) { }

		public Fotos_ViewModel(UserContext userContext, bool nestedForm = false) : base(userContext, "FFOTOS", nestedForm) { }

		public Fotos_ViewModel(UserContext userContext, Models.Album row, bool nestedForm = false) : base(userContext, "FFOTOS", row, nestedForm) { }

		public Fotos_ViewModel(UserContext userContext, string id, bool nestedForm = false, string[]? fieldsToLoad = null) : this(userContext, nestedForm)
		{
			this.Navigation.SetValue("album", id);
			Model = Models.Album.Find(id, userContext, "FFOTOS", fieldsToQuery: fieldsToLoad);
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
			Models.Album model = new Models.Album(userContext) { Identifier = "FFOTOS" };

			var navigation = m_userContext.CurrentNavigation;
			// The "LoadKeysFromHistory" must be after the "LoadEPH" because the PHE's in the tree mark Foreign Keys to null
			// (since they cannot assign multiple values to a single field) and thus the value that comes from Navigation is lost.
			// And this makes it more like the order of loading the model when opening the form.
			model.LoadEPH("FFOTOS");
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
			Models.Album model = Model;
			StatusMessage result = new StatusMessage(Status.OK, "");
			return result;
		}

		public StatusMessage EvaluateTableConditions(ConditionType type)
		{
			return Model.EvaluateTableConditions(type);
		}

		#endregion

		#region Mapper

		public override void MapFromModel(Models.Album m)
		{
			if (m == null)
			{
				CSGenio.framework.Log.Error("Map Model (Album) to ViewModel (Fotos) - Model is a null reference");
				throw new ModelNotFoundException("Model not found");
			}

			try
			{
				ValCodpropr = ViewModelConversion.ToString(m.ValCodpropr);
				ValFoto = ViewModelConversion.ToImage(m.ValFoto);
				ValTitulo = ViewModelConversion.ToString(m.ValTitulo);
				ValCodalbum = ViewModelConversion.ToString(m.ValCodalbum);
			}
			catch (Exception)
			{
				CSGenio.framework.Log.Error("Map Model (Album) to ViewModel (Fotos) - Error during mapping");
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
		public override void MapToModel(Models.Album m)
		{
			if (m == null)
			{
				CSGenio.framework.Log.Error("Map ViewModel (Fotos) to Model (Album) - Model is a null reference");
				throw new ModelNotFoundException("Model not found");
			}

			try
			{

				m.ValCodpropr = ViewModelConversion.ToString(ValCodpropr);

				if (ValFoto == null || !ValFoto.IsThumbnail)
					m.ValFoto = ViewModelConversion.ToImage(ValFoto);

				m.ValTitulo = ViewModelConversion.ToString(ValTitulo);

				m.ValCodalbum = ViewModelConversion.ToString(ValCodalbum);
			}
			catch (Exception)
			{
				CSGenio.framework.Log.Error($"Map ViewModel (Fotos) to Model (Album) - Error during mapping. All user values: {HasDisabledUserValuesSecurity}");
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
					case "album.codpropr":
						this.ValCodpropr = ViewModelConversion.ToString(_value);
						break;
					case "album.foto":
						this.ValFoto = ViewModelConversion.ToImage(_value);
						break;
					case "album.titulo":
						this.ValTitulo = ViewModelConversion.ToString(_value);
						break;
					case "album.codalbum":
						this.ValCodalbum = ViewModelConversion.ToString(_value);
						break;
					default:
						Log.Error($"SetViewModelValue (Fotos) - Unexpected field identifier {fullFieldName}");
						break;
				}
			}
			catch (Exception ex)
			{
				throw new FrameworkException(Resources.Resources.PEDIMOS_DESCULPA__OC63848, "SetViewModelValue (Fotos)", "Unexpected error", ex);
			}
		}

		#endregion

		/// <summary>
		/// Reads the Model from the database based on the key that is in the history or that was passed through the parameter
		/// </summary>
		/// <param name="id">The primary key of the record that needs to be read from the database. Leave NULL to use the value from the History.</param>
		public override void LoadModel(string id = null)
		{
			try { Model = Models.Album.Find(id ?? Navigation.GetStrValue("album"), m_userContext, "FFOTOS"); }
			finally { Model ??= new Models.Album(m_userContext) { Identifier = "FFOTOS" }; }

			base.LoadModel();
		}

		public override void Load(NameValueCollection qs, bool editable, bool ajaxRequest = false, bool lazyLoad = false)
		{
			this.editable = editable;
			CSGenio.business.Area oldvalues = null;

			// TODO: Deve ser substituido por search do CSGenioA
			try
			{
				Model = Models.Album.Find(Navigation.GetStrValue("album"), m_userContext, "FFOTOS");
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

			Model.Identifier = "FFOTOS";
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

		protected override void LoadDocumentsProperties(Models.Album row)
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
				Model = Models.Album.Find(Navigation.GetStrValue("album"), m_userContext, "FFOTOS");
				if (Model == null)
				{
					Model = new Models.Album(m_userContext) { Identifier = "FFOTOS" };
					Model.klass.QPrimaryKey = Navigation.GetStrValue("album");
				}
				MapToModel(Model);
				LoadDocumentsProperties(Model);
			}
			// Add characteristics
			Characs = new List<string>();

			Load_Fotos___proprtitulo__(qs, lazyLoad);
// USE /[MANUAL PRO VIEWMODEL_LOADPARTIAL FOTOS]/
		}

// USE /[MANUAL PRO VIEWMODEL_NEW FOTOS]/

		// Preencher Qvalues default dos fields do form
		protected override void LoadDefaultValues()
		{
		}

		public override CrudViewModelValidationResult Validate()
		{
			CrudViewModelFieldValidator validator = new(m_userContext.User.Language);

			validator.StringLength("ValTitulo", Resources.Resources.TITULO39021, ValTitulo, 50);

			return validator.GetResult();
		}

// USE /[MANUAL PRO VIEWMODEL_SAVE FOTOS]/
		public override void Save()
		{


			base.Save();
		}

// USE /[MANUAL PRO VIEWMODEL_APPLY FOTOS]/

// USE /[MANUAL PRO VIEWMODEL_DUPLICATE FOTOS]/

// USE /[MANUAL PRO VIEWMODEL_DESTROY FOTOS]/
		public override void Destroy(string id)
		{
			Model = Models.Album.Find(id, m_userContext, "FFOTOS");
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
		/// TableProprTitulo -> (DB)
		/// </summary>
		/// <param name="qs"></param>
		/// <param name="lazyLoad">Lazy loading of dropdown items</param>
		public void Load_Fotos___proprtitulo__(NameValueCollection qs, bool lazyLoad = false)
		{
			bool fotos___proprtitulo__DoLoad = true;
			CriteriaSet fotos___proprtitulo__Conds = CriteriaSet.And();
			{
				object hValue = Navigation.GetValue("propr", true);
				if (hValue != null && !(hValue is Array) && !string.IsNullOrEmpty(Convert.ToString(hValue)))
				{
					fotos___proprtitulo__Conds.Equal(CSGenioApropr.FldCodpropr, hValue);
					this.ValCodpropr = DBConversion.ToString(hValue);
				}
			}

			TableProprTitulo = new TableDBEdit<Models.Propr>
			{
				IsLazyLoad = lazyLoad
			};

			if (lazyLoad)
			{
				if (Navigation.CurrentLevel.GetEntry("RETURN_propr") != null)
				{
					this.ValCodpropr = Navigation.GetStrValue("RETURN_propr");
					Navigation.CurrentLevel.SetEntry("RETURN_propr", null);
				}
				FillDependant_FotosTableProprTitulo(lazyLoad);
				//Check if foreignkey comes from history
				TableProprTitulo.FilledByHistory = Navigation.CheckFilledByHistory("propr");
				return;
			}

			if (fotos___proprtitulo__DoLoad)
			{
				List<ColumnSort> sorts = new List<ColumnSort>();
				ColumnSort requestedSort = GetRequestSort(TableProprTitulo, "sTableProprTitulo", "dTableProprTitulo", qs, "propr");
				if (requestedSort != null)
					sorts.Add(requestedSort);
				sorts.Add(new ColumnSort(new ColumnReference(CSGenioApropr.FldTitulo), SortOrder.Ascending));

				string query = "";
				if (!string.IsNullOrEmpty(qs["TableProprTitulo_tableFilters"]))
					TableProprTitulo.TableFilters = bool.Parse(qs["TableProprTitulo_tableFilters"]);
				else
					TableProprTitulo.TableFilters = false;

				query = qs["qTableProprTitulo"];

				//RS 26.07.2016 O preenchimento da lista de ajuda dos Dbedits passa a basear-se apenas no campo do próprio DbEdit
				// O interface de pesquisa rápida não fica coerente quando se visualiza apenas uma coluna mas a pesquisa faz matching com 5 ou 6 colunas diferentes
				//  tornando confuso to o user porque determinada row foi devolvida quando o Qresult não mostra como o matching foi feito
				CriteriaSet search_filters = CriteriaSet.And();
				if (!string.IsNullOrEmpty(query))
				{
					search_filters.Like(CSGenioApropr.FldTitulo, query + "%");
				}
				fotos___proprtitulo__Conds.SubSet(search_filters);

				string tryParsePage = qs["pTableProprTitulo"] != null ? qs["pTableProprTitulo"].ToString() : "1";
				int page = !string.IsNullOrEmpty(tryParsePage) ? int.Parse(tryParsePage) : 1;
				int numberItems = CSGenio.framework.Configuration.NrRegDBedit;
				int offset = (page - 1) * numberItems;

				FieldRef[] fields = new FieldRef[] { CSGenioApropr.FldCodpropr, CSGenioApropr.FldTitulo, CSGenioApropr.FldZzstate };

// USE /[MANUAL PRO OVERRQ FOTOS_PROPRTITULO]/

				// Limitation by Zzstate
				/*
					Records that are currently being inserted or duplicated will also be included.
					Client-side persistence will try to fill the "text" value of that option.
				*/
				if (Navigation.checkFormMode("propr", FormMode.New) || Navigation.checkFormMode("propr", FormMode.Duplicate))
					fotos___proprtitulo__Conds.SubSet(CriteriaSet.Or()
						.Equal(CSGenioApropr.FldZzstate, 0)
						.Equal(CSGenioApropr.FldCodpropr, Navigation.GetStrValue("propr")));
				else
					fotos___proprtitulo__Conds.Criterias.Add(new Criteria(new ColumnReference(CSGenioApropr.FldZzstate), CriteriaOperator.Equal, 0));

				FieldRef firstVisibleColumn = new FieldRef("propr", "titulo");
				ListingMVC<CSGenioApropr> listing = Models.ModelBase.Where<CSGenioApropr>(m_userContext, false, fotos___proprtitulo__Conds, fields, offset, numberItems, sorts, "LED_FOTOS___PROPRTITULO__", true, false, firstVisibleColumn: firstVisibleColumn);

				TableProprTitulo.SetPagination(page, numberItems, listing.HasMore, listing.GetTotal, listing.TotalRecords);
				TableProprTitulo.Query = query;
				TableProprTitulo.Elements = listing.RowsForViewModel<GenioMVC.Models.Propr>((r) => new GenioMVC.Models.Propr(m_userContext, r, true, _fieldsToSerialize_FOTOS___PROPRTITULO__));

				//created by [ MH ] at [ 14.04.2016 ] - Foi alterada a forma de retornar a key do novo registo inserido / editado no form de apoio do DBEdit.
				//last update by [ MH ] at [ 10.05.2016 ] - Validação se key encontra-se no level atual, as chaves dos niveis anteriores devem ser ignorados.
				if (Navigation.CurrentLevel.GetEntry("RETURN_propr") != null)
				{
					this.ValCodpropr = Navigation.GetStrValue("RETURN_propr");
					Navigation.CurrentLevel.SetEntry("RETURN_propr", null);
				}

				TableProprTitulo.List = new SelectList(TableProprTitulo.Elements.ToSelectList(x => x.ValTitulo, x => x.ValCodpropr,  x => x.ValCodpropr == this.ValCodpropr), "Value", "Text", this.ValCodpropr);
				FillDependant_FotosTableProprTitulo();

				//Check if foreignkey comes from history
				TableProprTitulo.FilledByHistory = Navigation.CheckFilledByHistory("propr");
			}
		}

		/// <summary>
		/// Get Dependant fields values -> TableProprTitulo (DB)
		/// </summary>
		/// <param name="PKey">Primary Key of Propr</param>
		public ConcurrentDictionary<string, object> GetDependant_FotosTableProprTitulo(string PKey)
		{
			FieldRef[] refDependantFields = [CSGenioApropr.FldCodpropr, CSGenioApropr.FldTitulo];

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

			CSGenioApropr tempArea = new(u);

			// Fields to select
			SelectQuery querySelect = new();
			querySelect.PageSize(1);
			foreach (FieldRef field in refDependantFields)
				querySelect.Select(field);

			querySelect.From(tempArea.QSystem, tempArea.TableName, tempArea.Alias)
				.Where(wherecodition.Equal(CSGenioApropr.FldCodpropr, PKey));

			string[] dependantFields = refDependantFields.Select(f => f.FullName).ToArray();
			QueryUtils.SetInnerJoins(dependantFields, null, tempArea, querySelect);

			ArrayList values = sp.executeReaderOneRow(querySelect);
			bool useDefaults = values.Count == 0;

			if (useDefaults)
				return GetViewModelFieldValues(refDependantFields);
			return GetViewModelFieldValues(refDependantFields, values);
		}

		/// <summary>
		/// Fill Dependant fields values -> TableProprTitulo (DB)
		/// </summary>
		/// <param name="lazyLoad">Lazy loading of dropdown items</param>
		public void FillDependant_FotosTableProprTitulo(bool lazyLoad = false)
		{
			var row = GetDependant_FotosTableProprTitulo(this.ValCodpropr);
			try
			{

				// Fill List fields
				this.ValCodpropr = ViewModelConversion.ToString(row["propr.codpropr"]);
				TableProprTitulo.Value = (string)row["propr.titulo"];
				if (GlobalFunctions.emptyG(this.ValCodpropr) == 1)
				{
					this.ValCodpropr = "";
					TableProprTitulo.Value = "";
					Navigation.ClearValue("propr");
				}
				else if (lazyLoad)
				{
					TableProprTitulo.SetPagination(1, 0, false, false, 1);
					TableProprTitulo.List = new SelectList(new List<SelectListItem>()
					{
						new SelectListItem
						{
							Value = Convert.ToString(this.ValCodpropr),
							Text = Convert.ToString(TableProprTitulo.Value),
							Selected = true
						}
					}, "Value", "Text", this.ValCodpropr);
				}

				TableProprTitulo.Selected = this.ValCodpropr;
			}
			catch (Exception ex)
			{
				CSGenio.framework.Log.Error(string.Format("FillDependant_Error (TableProprTitulo): {0}; {1}", ex.Message, ex.InnerException != null ? ex.InnerException.Message : ""));
			}
		}

		private readonly string[] _fieldsToSerialize_FOTOS___PROPRTITULO__ = ["Propr", "Propr.ValCodpropr", "Propr.ValZzstate", "Propr.ValTitulo"];

		protected override object GetViewModelValue(string identifier, object modelValue)
		{
			return identifier switch
			{
				"album.codpropr" => ViewModelConversion.ToString(modelValue),
				"album.foto" => ViewModelConversion.ToImage(modelValue),
				"album.titulo" => ViewModelConversion.ToString(modelValue),
				"album.codalbum" => ViewModelConversion.ToString(modelValue),
				"propr.codpropr" => ViewModelConversion.ToString(modelValue),
				"propr.titulo" => ViewModelConversion.ToString(modelValue),
				_ => modelValue
			};
		}


		/// <inheritdoc/>
		protected override void SetTicketToImageFields()
		{
			if(ValFoto != null)
				ValFoto.Ticket = Helpers.Helpers.GetFileTicket(m_userContext.User, Area.AreaALBUM, CSGenioAalbum.FldFoto.Field, null, ValCodalbum);
		}

		#region Charts


		#endregion

		#region Custom code

// USE /[MANUAL PRO VIEWMODEL_CUSTOM FOTOS]/

		#endregion
	}
}
