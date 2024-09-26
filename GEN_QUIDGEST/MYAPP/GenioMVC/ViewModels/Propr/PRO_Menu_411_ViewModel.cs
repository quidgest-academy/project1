using JsonPropertyName = System.Text.Json.Serialization.JsonPropertyNameAttribute;
using SelectList = Microsoft.AspNetCore.Mvc.Rendering.SelectList;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.Linq;

using CSGenio.business;
using CSGenio.framework;
using GenioMVC.Helpers;
using GenioMVC.Models.Navigation;
using Quidgest.Persistence;
using Quidgest.Persistence.GenericQuery;

namespace GenioMVC.ViewModels.Propr
{
	public class PRO_Menu_411_ViewModel : ListViewModel
	{
		/// <summary>
		/// Gets or sets the object that represents the table and its elements.
		/// </summary>
		[JsonPropertyName("Table")]
		public TablePartial<GenioMVC.Models.Propr> Menu { get; set; }

		/// <inheritdoc/>
		public override string TableAlias { get => "propr"; }

		/// <inheritdoc/>
		public override string Uuid { get => "ea2707b4-7848-4b42-ab66-037c3862e1b4"; }

		/// <inheritdoc/>
		protected override string[] FieldsToSerialize { get => _fieldsToSerialize; }

		/// <inheritdoc/>
		protected override List<TableSearchColumn> SearchableColumns { get => _searchableColumns; }

		/// <summary>
		/// The primary key field.
		/// </summary>
		public string ValCodpropr { get; set; }

		/// <inheritdoc/>
		public override CriteriaSet baseConditions
		{
			get
			{
				CriteriaSet conds = CriteriaSet.And();
				conds.Equal(CSGenioApropr.FldCodagent, Navigation.GetValue("agent"));
				return conds;
			}
		}

		/// <inheritdoc/>
		public override List<Relation> relations
		{
			get
			{
				List<Relation> relations = null;
				return relations;
			}
		}


		public override int GetCount(User user)
		{
			CSGenio.persistence.PersistentSupport sp = m_userContext.PersistentSupport;
			var areaBase = CSGenio.business.Area.createArea("propr", user, "PRO");

			//gets eph conditions to be applied in listing
			CriteriaSet conditions = CSGenio.business.Listing.CalculateConditionsEphGeneric(areaBase, "ML411");
			conditions.Equal(CSGenioApropr.FldZzstate, 0); //valid zzstate only

			//Menu fixed limits and relations:

			

			// Checks for foreign tables in fields and conditions
FieldRef[] fields = new FieldRef[] { CSGenioApropr.FldCodpropr, CSGenioApropr.FldZzstate, CSGenioApropr.FldCodagent, CSGenioAagent.FldCodagent, CSGenioAagent.FldEmail, CSGenioApropr.FldTitulo, CSGenioApropr.FldFoto, CSGenioApropr.FldPreco };

			ListingMVC<CSGenioApropr> listing = new ListingMVC<CSGenioApropr>(fields, null, 1, 1, false, user, true, string.Empty, true);
			SelectQuery qs = sp.getSelectQueryFromListingMVC(conditions, listing);

			//Menu relations:
			if (qs.FromTable == null)
				qs.From(areaBase.QSystem, areaBase.TableName, areaBase.Alias);


			if (!qs.Joins.Select(x => x.Table).Select(y=>y.TableAlias).Contains(CSGenio.business.Area.AreaAGENT.Alias))
				qs.Join(CSGenio.business.Area.AreaAGENT, TableJoinType.Inner).On(CriteriaSet.And().Equal(CSGenioAagent.FldCodagent, CSGenioApropr.FldCodagent));

			//operation: Count menu records
			return CSGenio.persistence.DBConversion.ToInteger(sp.ExecuteScalar(CSGenio.persistence.QueryUtils.buildQueryCount(qs)));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PRO_Menu_411_ViewModel" /> class.
		/// </summary>
		/// <param name="userContext">The current user request context</param>
		public PRO_Menu_411_ViewModel(UserContext userContext) : base(userContext)
		{
			this.RoleToShow = CSGenio.framework.Role.ROLE_3;
		}

		/// <inheritdoc/>
		public override List<Exports.QColumn> GetColumnsToExport(bool ajaxRequest = false)
		{
			var columns = new List<Exports.QColumn>()
			{
				new Exports.QColumn(CSGenioAagent.FldEmail, FieldType.TEXTO, Resources.Resources.E_MAIL42251, 30, 0, true),
				new Exports.QColumn(CSGenioApropr.FldTitulo, FieldType.TEXTO, Resources.Resources.TITULO39021, 30, 0, true),
				!ajaxRequest ? new Exports.QColumn(CSGenioApropr.FldFoto, FieldType.IMAGEM_JPEG, Resources.Resources.FOTOGRAFIA36807, 3, 0, true):null,
				new Exports.QColumn(CSGenioApropr.FldPreco, FieldType.VALOR, Resources.Resources.PRECO50007, 10, 2, true),
			};

			columns.RemoveAll(item => item == null);
			return columns;
		}

		public void LoadToExport(out ListingMVC<CSGenioApropr> listing, out CriteriaSet conditions, out List<Exports.QColumn> columns, NameValueCollection requestValues, bool ajaxRequest = false)
		{
			CSGenio.framework.TableConfiguration.TableConfiguration tableConfig = new CSGenio.framework.TableConfiguration.TableConfiguration();

			LoadToExport(out listing, out conditions, out columns, tableConfig, requestValues, ajaxRequest);
		}

		public void LoadToExport(out ListingMVC<CSGenioApropr> listing, out CriteriaSet conditions, out List<Exports.QColumn> columns, CSGenio.framework.TableConfiguration.TableConfiguration tableConfig, NameValueCollection requestValues, bool ajaxRequest = false)
		{
			listing = null;
			conditions = null;
			columns = this.GetExportColumns(tableConfig.ColumnConfiguration);

			// Store number of records to reset it after loading
			int rowsPerPage = tableConfig.RowsPerPage;
			tableConfig.RowsPerPage = -1;

			Load(tableConfig, requestValues, ajaxRequest, true, ref listing, ref conditions);

			// Reset number of records to original value
			tableConfig.RowsPerPage = rowsPerPage;
		}

		/// <summary>
		/// Loads the viewmodel to export a template.
		/// </summary>
		/// <param name="columns">The columns.</param>
		public void LoadToExportTemplate(out List<Exports.QColumn> columns)
		{
			columns = new List<Exports.QColumn>()
			{
				new Exports.QColumn(CSGenioApropr.FldFoto, FieldType.IMAGEM_JPEG, Resources.Resources.FOTOGRAFIA36807, 3, 0, true),
				new Exports.QColumn(CSGenioApropr.FldTitulo, FieldType.TEXTO, Resources.Resources.TITULO39021, 80, 0, true),
				new Exports.QColumn(CSGenioApropr.FldPreco, FieldType.VALOR, Resources.Resources.PRECO50007, 9, 4, true),
				new Exports.QColumn(CSGenioApropr.FldTamanho, FieldType.NUMERO, Resources.Resources.TAMANHO__M2_40951, 5, 2, true),
				new Exports.QColumn(CSGenioApropr.FldNr_wcs, FieldType.NUMERO, Resources.Resources.NUMERO_DE_CASAS_DE_B39783, 3, 0, true),
				new Exports.QColumn(CSGenioApropr.FldDtconst, FieldType.DATA, Resources.Resources.DATA_DE_CONTRUCAO03489, 8, 0, true),
				new Exports.QColumn(CSGenioApropr.FldDescrica, FieldType.MEMO, Resources.Resources.DESCRICAO07528, 80, 10, true),
				new Exports.QColumn(CSGenioApropr.FldTipoprop, FieldType.ARRAY_COD_TEXTO, Resources.Resources.TIPO_DE_CONSTRUCAO35217, 1, 0, true),
				new Exports.QColumn(CSGenioApropr.FldTipologi, FieldType.ARRAY_COD_NUMERICO, Resources.Resources.TIPOLOGIA13928, 1, 0, true),
				new Exports.QColumn(CSGenioApropr.FldIdpropre, FieldType.NUMERO, Resources.Resources.ID48520, 10, 0, true),
				new Exports.QColumn(CSGenioApropr.FldEspexter, FieldType.NUMERO, Resources.Resources.ESPACO_EXTERIOR__M2_04786, 6, 2, true),
				new Exports.QColumn(CSGenioApropr.FldVendida, FieldType.LOGICO, Resources.Resources.VENDIDA08366, 1, 0, true),
				new Exports.QColumn(CSGenioApropr.FldLocaliza, FieldType.GEOGRAPHY, Resources.Resources.LOCALIZACAO54665, 50, 0, true),
				new Exports.QColumn(CSGenioAagent.FldEmail, FieldType.TEXTO, Resources.Resources.E_MAIL42251, 30, 0, true),
			};
		}

		/// <inheritdoc/>
		public override CriteriaSet BuildCriteriaSet(NameValueCollection requestValues, out bool tableReload, CriteriaSet crs = null, bool isToExport = false)
		{
			CSGenio.framework.TableConfiguration.TableConfiguration tableConfig = new();
			return BuildCriteriaSet(tableConfig, requestValues, out tableReload, crs, isToExport);
		}

		/// <inheritdoc/>
		public override CriteriaSet BuildCriteriaSet(CSGenio.framework.TableConfiguration.TableConfiguration tableConfig, NameValueCollection requestValues, out bool tableReload, CriteriaSet crs = null, bool isToExport = false)
		{
			User u = m_userContext.User;
			tableReload = true;

			if (crs == null)
				crs = CriteriaSet.And();


			if (Menu == null)
				Menu = new TablePartial<GenioMVC.Models.Propr>();
			Menu.SetFilters(false, false);


			//FOR: MENU LIST SORTING
			Dictionary<string, OrderedDictionary> allSortOrders = new Dictionary<string, OrderedDictionary>();
			allSortOrders.Add("PROPR.TITULO", new OrderedDictionary());
			allSortOrders["PROPR.TITULO"].Add("PROPR.TITULO", "A");


			crs.SubSets.Add(ProcessSearchFilters(Menu, GetSearchColumns(true), tableConfig));


			//Subfilters
			CriteriaSet subfilters = CriteriaSet.And();


			crs.SubSets.Add(subfilters);




			// Limitations
			// Limit "DB"
			crs.Equal(CSGenioApropr.FldCodagent, Navigation.GetValue("agent"));

			if (isToExport)
			{
				// EPH
				crs = Models.Propr.AddEPH<CSGenioApropr>(ref u, crs, "ML411");

				// Export only records with ZZState == 0
				crs.Equal(CSGenioApropr.FldZzstate, 0);

				return crs;
			}

			// Limitation by Zzstate
			if (!Navigation.checkFormMode("PROPR", FormMode.New)) // TODO: Check in Duplicate mode
				crs = extendWithZzstateCondition(crs, CSGenioApropr.FldZzstate, null);


			if (tableReload)
			{
				string QMVC_POS_RECORD = Navigation.GetStrValue("QMVC_POS_RECORD_propr");
				Navigation.DestroyEntry("QMVC_POS_RECORD_propr");
				if (!string.IsNullOrEmpty(QMVC_POS_RECORD))
					crs.Equals(Models.Propr.AddEPH<CSGenioApropr>(ref u, null, "ML411"));
			}

			return crs;
		}

		/// <summary>
		/// Loads the list with the specified number of rows.
		/// </summary>
		/// <param name="numberListItems">The number of rows to load.</param>
		/// <param name="ajaxRequest">Whether the request was initiated via AJAX.</param>
		public void Load(int numberListItems, bool ajaxRequest = false)
		{
			Load(numberListItems, new NameValueCollection(), ajaxRequest);
		}

		/// <summary>
		/// Loads the list with the specified number of rows.
		/// </summary>
		/// <param name="numberListItems">The number of rows to load.</param>
		/// <param name="requestValues">The request values.</param>
		/// <param name="ajaxRequest">Whether the request was initiated via AJAX.</param>
		/// <param name="conditions">The conditions.</param>
		public void Load(int numberListItems, NameValueCollection requestValues, bool ajaxRequest = false, CriteriaSet conditions = null)
		{
			ListingMVC<CSGenioApropr> listing = null;

			Load(numberListItems, requestValues, ajaxRequest, false, ref listing, ref conditions);
		}

		/// <summary>
		/// Loads the list with the specified number of rows.
		/// </summary>
		/// <param name="numberListItems">The number of rows to load.</param>
		/// <param name="requestValues">The request values.</param>
		/// <param name="ajaxRequest">Whether the request was initiated via AJAX.</param>
		/// <param name="isToExport">Whether the list is being loaded to be exported</param>
		/// <param name="Qlisting">The rows.</param>
		/// <param name="conditions">The conditions.</param>
		public void Load(int numberListItems, NameValueCollection requestValues, bool ajaxRequest, bool isToExport, ref ListingMVC<CSGenioApropr> Qlisting, ref CriteriaSet conditions)
		{
			CSGenio.framework.TableConfiguration.TableConfiguration tableConfig = new CSGenio.framework.TableConfiguration.TableConfiguration();

			tableConfig.RowsPerPage = numberListItems;

			Load(tableConfig, requestValues, ajaxRequest, isToExport, ref Qlisting, ref conditions);
		}

		/// <summary>
		/// Loads the table with the specified configuration.
		/// </summary>
		/// <param name="tableConfig">The table configuration object</param>
		/// <param name="requestValues">The request values.</param>
		/// <param name="ajaxRequest">Whether the request was initiated via AJAX.</param>
		/// <param name="isToExport">Whether the list is being loaded to be exported</param>
		/// <param name="conditions">The conditions.</param>
		public void Load(CSGenio.framework.TableConfiguration.TableConfiguration tableConfig, NameValueCollection requestValues, bool ajaxRequest, bool isToExport = false, CriteriaSet conditions = null)
		{
			ListingMVC<CSGenioApropr> listing = null;

			Load(tableConfig, requestValues, ajaxRequest, isToExport, ref listing, ref conditions);
		}

		/// <summary>
		/// Loads the table with the specified configuration.
		/// </summary>
		/// <param name="tableConfig">The table configuration object</param>
		/// <param name="requestValues">The request values.</param>
		/// <param name="ajaxRequest">Whether the request was initiated via AJAX.</param>
		/// <param name="isToExport">Whether the list is being loaded to be exported</param>
		/// <param name="Qlisting">The rows.</param>
		/// <param name="conditions">The conditions.</param>
		public void Load(CSGenio.framework.TableConfiguration.TableConfiguration tableConfig, NameValueCollection requestValues, bool ajaxRequest, bool isToExport, ref ListingMVC<CSGenioApropr> Qlisting, ref CriteriaSet conditions)
		{
			User u = m_userContext.User;
			Menu = new TablePartial<GenioMVC.Models.Propr>();

			CriteriaSet pro_menu_411Conds = CriteriaSet.And();

			bool tableReload = true;

			//FOR: MENU LIST SORTING
			Dictionary<string, OrderedDictionary> allSortOrders = new Dictionary<string, OrderedDictionary>();
			allSortOrders.Add("PROPR.TITULO", new OrderedDictionary());
			allSortOrders["PROPR.TITULO"].Add("PROPR.TITULO", "A");




			int numberListItems = tableConfig.RowsPerPage;
			var pageNumber = ajaxRequest ? tableConfig.Page : 1;

			// Added to avoid 0 or -1 pages when setting number of records to -1 to disable pagination
			if (pageNumber < 1)
				pageNumber = 1;

			List<ColumnSort> sorts = GetRequestSorts(this.Menu, tableConfig.ColumnOrderBy, "propr", allSortOrders);

			if (sorts == null || sorts.Count == 0)
			{
				sorts = new List<ColumnSort>();
				sorts.Add(new ColumnSort(new ColumnReference(CSGenioApropr.FldTitulo), SortOrder.Ascending));

			}

FieldRef[] fields = new FieldRef[] { CSGenioApropr.FldCodpropr, CSGenioApropr.FldZzstate, CSGenioApropr.FldCodagent, CSGenioAagent.FldCodagent, CSGenioAagent.FldEmail, CSGenioApropr.FldTitulo, CSGenioApropr.FldFoto, CSGenioApropr.FldPreco };


			//columns by users list (TemplateDBEditViewModel)
			//TODO: Get columns based on table configuration or merge this into the main column sort logic
			userColumns = UserUiSettings.Load(m_userContext.PersistentSupport, Uuid, m_userContext.User).userColumns;
			FieldRef firstVisibleColumn = null;

			if (sorts == null)
				if (userColumns != null)
				{
					CSGenioAlstcol col = userColumns.FirstOrDefault(x => x.ValVisivel == 1);

					if (col != null)
					{
						string table = col.ValTabela.ToLower();
						string field = col.ValCampo.ToLower(); //may contain Table.ValField
						if (field.Contains("."))
						{
							field = field.Substring(table.Length + 4); //remove table name and .Val from ValCampo data. i.e: "Pesso.ValNome", pesso lenght will remove "Pesso" and then +4 for the fixed ".Val"
						}
						else
						{
							field = field.Substring(3); //remove table Val from ValCampo data. i.e: "ValNome", Substring(3) will remove "Val"
						}

						firstVisibleColumn = new FieldRef(table, field);
					}
				}
				else
					firstVisibleColumn = new FieldRef("agent", "email");


			// Limitations
			if (this.tableLimits == null)
				this.tableLimits = new List<Limit>();
			//Comparer to check if limit is already present in tableLimits
			LimitComparer limitComparer = new LimitComparer();

			//Tooltip for EPHs affecting this viewmodel list
			{
				Limit limit = new Limit();
				limit.TipoLimite = LimitType.EPH;
				CSGenioApropr model_limit_area = new CSGenioApropr(m_userContext.User);
				List<Limit> area_EPH_limits = EPH_Limit_Filler(ref limit, model_limit_area, "ML411");
				if (area_EPH_limits.Count > 0)
					this.tableLimits.AddRange(area_EPH_limits);
			}

			// Tooltips: Making a tooltip for each valid limitation: 1 Limit(s) detected.
			// Limit origin: menu 

			//Limit type: "DB"
			//Current Area = "PROPR"
			//1st Area Limit: "AGENT"
			//1st Area Field: "CODAGENT"
			//1st Area Value: ""
			{
				Limit limit = new Limit();
				limit.TipoLimite = LimitType.DB;
				limit.NaoAplicaSeNulo = false;
				CSGenioAagent model_limit_area = new CSGenioAagent(m_userContext.User);
				string limit_field = "codagent", limit_field_value = "";
				object this_limit_field = Navigation.GetStrValue(limit_field_value);
				Limit_Filler(ref limit, model_limit_area, limit_field, limit_field_value, this_limit_field, LimitAreaType.AreaLimita);
				if (!this.tableLimits.Contains(limit, limitComparer)) //to avoid repetitions (i.e: DB and EPH applying same limit)
					this.tableLimits.Add(limit);
			}

			if (conditions == null)
				conditions = CriteriaSet.And();

			conditions.SubSets.Add(pro_menu_411Conds);
			pro_menu_411Conds = BuildCriteriaSet(tableConfig, requestValues, out bool hasAllRequiredLimits, conditions, isToExport);
			tableReload &= hasAllRequiredLimits;

// USE /[MANUAL PRO OVERRQ 411]/

			if (isToExport)
			{
				if (!tableReload)
					return;

				Qlisting = Models.ModelBase.Where<CSGenioApropr>(m_userContext, false, pro_menu_411Conds, fields, (pageNumber - 1) * numberListItems, numberListItems, sorts, "ML411", true, firstVisibleColumn: firstVisibleColumn);

// USE /[MANUAL PRO OVERRQLSTEXP 411]/

				return;
			}

			if (tableReload)
			{
// USE /[MANUAL PRO OVERRQLIST 411]/

				string QMVC_POS_RECORD = Navigation.GetStrValue("QMVC_POS_RECORD_propr");
				Navigation.DestroyEntry("QMVC_POS_RECORD_propr");
				CriteriaSet m_PagingPosEPHs = null;

				if (!string.IsNullOrEmpty(QMVC_POS_RECORD))
				{
					var m_iCurPag = m_userContext.PersistentSupport.getPagingPos(CSGenioApropr.GetInformation(), QMVC_POS_RECORD, sorts, pro_menu_411Conds, m_PagingPosEPHs, firstVisibleColumn: firstVisibleColumn);
					if (m_iCurPag != -1)
						pageNumber = ((m_iCurPag - 1) / numberListItems) + 1;
				}

				ListingMVC<CSGenioApropr> listing = Models.ModelBase.Where<CSGenioApropr>(m_userContext, false, pro_menu_411Conds, fields, (pageNumber - 1) * numberListItems, numberListItems, sorts, "ML411", true, true, QMVC_POS_RECORD, m_PagingPosEPHs, firstVisibleColumn);

				if (listing.CurrentPage > 0)
					pageNumber = listing.CurrentPage;

				//Added to avoid 0 or -1 pages when setting number of records to -1 to disable pagination
				if (pageNumber < 1)
					pageNumber = 1;

				//Set document field values to objects
				SetDocumentFields(listing);

				Menu.Elements = MapPRO_Menu_411(listing);

				Menu.Identifier = "ML411";
				Menu.Slots = new Dictionary<string, List<object>>();

				// Last updated by [CJP] at [2015.02.03]
				// Adds the identifier to each element
				foreach (var element in Menu.Elements)
					element.Identifier = "ML411";

				Menu.SetPagination(pageNumber, listing.NumRegs, listing.HasMore, listing.GetTotal, listing.TotalRecords);
			}

			//Set table limits display property
			FillTableLimitsDisplayData();

			// Store table configuration so it gets sent to the client-side to be processed
			CurrentTableConfig = tableConfig;
		}

		private List<Models.Propr> MapPRO_Menu_411(ListingMVC<CSGenioApropr> Qlisting)
		{
			var Elements = new List<Models.Propr>();
			int i = 0;

			if (Qlisting.Rows != null)
			{
				foreach (var row in Qlisting.Rows)
				{
					if (Qlisting.NumRegs > 0 && i >= Qlisting.NumRegs) // Copiado da versão antiga do RowsToViewModels
						break;
					Elements.Add(MapPRO_Menu_411(row));
					i++;
				}
			}

			return Elements;
		}

		/// <summary>
		/// Maps a single CSGenioApropr row
		/// to a Models.Propr object.
		/// </summary>
		/// <param name="row">The row.</param>
		private Models.Propr MapPRO_Menu_411(CSGenioApropr row)
		{
			var model = new Models.Propr(m_userContext, true, _fieldsToSerialize);
			if (row == null) return model;

			foreach (RequestedField Qfield in row.Fields.Values)
			{
				switch (Qfield.Area)
				{
					case "propr":
						model.klass.insertNameValueField(Qfield.FullName, Qfield.Value); break;
					case "agent":
						model.Agent.klass.insertNameValueField(Qfield.FullName, Qfield.Value); break;
					default:
						break;
				}
			}

			SetTicketToImageFields(model);
			return model;
		}

		/// <summary>
		/// Checks the loaded model for pending rows (zzsttate not 0).
		/// </summary>
		public bool CheckForZzstate()
		{
			if (Menu?.Elements == null)
				return false;

			return Menu.Elements.Any(row => row.ValZzstate != 0);
		}


		/// <summary>
		/// Sets the document field values to objects.
		/// </summary>
		/// <param name="listing">The rows.</param>
		private void SetDocumentFields(ListingMVC<CSGenioApropr> listing)
		{
			if (listing.Rows == null)
				return;

			foreach (CSGenioApropr row in listing.Rows)
			{
			}
		}

		#region Custom code
// USE /[MANUAL PRO VIEWMODEL_CUSTOM PRO_MENU_411]/
		#endregion

		private static readonly string[] _fieldsToSerialize =
		{
			"Propr", "Propr.ValCodpropr", "Propr.ValZzstate", "Agent", "Agent.ValEmail", "Propr.ValTitulo", "Propr.ValFoto", "Propr.ValPreco", "Propr.ValCodagent", "Propr.ValCodcidad"
		};

		private static readonly List<TableSearchColumn> _searchableColumns = new List<TableSearchColumn>
		{
			new TableSearchColumn("Agent_ValEmail", CSGenioAagent.FldEmail, typeof(string)),
			new TableSearchColumn("ValTitulo", CSGenioApropr.FldTitulo, typeof(string), defaultSearch : true),
			new TableSearchColumn("ValPreco", CSGenioApropr.FldPreco, typeof(decimal?))
		};


		protected void SetTicketToImageFields(Models.Propr row)
		{
			if(row == null)
				return;
	
			row.ValFotoQTicket = Helpers.Helpers.GetFileTicket(m_userContext.User, Area.AreaPROPR, CSGenioApropr.FldFoto.Field, null, row.ValCodpropr);
		}
	}
}
