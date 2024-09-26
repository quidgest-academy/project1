﻿using JsonIgnoreAttribute = System.Text.Json.Serialization.JsonIgnoreAttribute;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

using CSGenio.business;
using CSGenio.framework;
using GenioMVC.Helpers;
using GenioMVC.Models.Navigation;
using Quidgest.Persistence.GenericQuery;

namespace GenioMVC.ViewModels
{
	public enum TableViewsManagementMode
	{
		/// <summary>
		/// The user is not allowed to change the list in any way.
		/// </summary>
		None,

		/// <summary>
		/// The user is allowed to customize the table but the changes are not saved.
		/// </summary>
		NonPersistent,

		/// <summary>
		/// The user changes are automatically saved in a single user table configuration.
		/// </summary>
		PersistOne,

		/// <summary>
		/// The user can fully create and manage multiple table configurations.
		/// </summary>
		PersistMany
	}

	public abstract class ListViewModel : ViewModelBase
	{
		private readonly UserUiSettings _userUiSettings;

		protected List<CSGenioAlstcol> userColumns;

		/// <summary>
		/// Gets the alias of the table.
		/// </summary>
		public abstract string TableAlias { get; }

		/// <summary>
		/// Gets the unique user interface descriptor.
		/// </summary>
		public abstract string Uuid { get; }

		/// <summary>
		/// Gets the searchable columns.
		/// </summary>
		protected abstract List<TableSearchColumn> SearchableColumns { get; }

		/// <summary>
		/// Gets the list base conditions.
		/// For row reordering.
		/// </summary>
		public abstract CriteriaSet baseConditions { get; }

		/// <summary>
		/// Gets the list of relations.
		/// For row reordering.
		/// </summary>
		public abstract List<Relation> relations { get; }

		/// <summary>
		/// Gets the user column configuration.
		/// </summary>
		[JsonIgnore]
		public List<CSGenioAlstcol> UserColumns
		{
			get => userColumns;
		}

		/// <summary>
		/// Gets or sets the table limits.
		/// </summary>
		[JsonIgnore]
		public List<Limit> tableLimits { get; set; }

		/// <summary>
		/// Gets or sets the data to display the table limits.
		/// </summary>
		public List<LimitDisplayData> tableLimitsDisplayData { get; set; }

		/// <summary>
		/// Gets the table views management mode.
		/// </summary>
		virtual protected TableViewsManagementMode ViewsManagementMode
		{
			get => TableViewsManagementMode.None;
		}

		/// <summary>
		/// Gets the names of the user table configurations.
		/// </summary>
		public List<string> UserTableConfigNames => _userUiSettings?.userTableConfigNames;

		/// <summary>
		/// Gets the name of the default user table configuration.
		/// </summary>
		public string UserTableConfigNameDefault => _userUiSettings?.userTableConfigDefaultName;

		/// <summary>
		/// The current table configuration.
		/// </summary>
		public CSGenio.framework.TableConfiguration.TableConfiguration CurrentTableConfig { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ListViewModel" /> class.
		/// </summary>
		/// <param name="userContext">The current user request context</param>
		public ListViewModel(UserContext userContext) : base(userContext)
		{
			if (ViewsManagementMode == TableViewsManagementMode.PersistOne ||
				ViewsManagementMode == TableViewsManagementMode.PersistMany)
			{
				_userUiSettings = UserUiSettings.Load(
					userContext.PersistentSupport,
					Uuid,
					userContext.User,
					Navigation.GetStrValue("UserTableConfigName"),
					false
				);
			}
		}

		/// <summary>
		/// Sets the table limits display property.
		/// </summary>
		protected void FillTableLimitsDisplayData()
		{
			// Nothing to do if table has no limits.
			if (this.tableLimits == null || this.tableLimits.Count < 1)
				return;

			this.tableLimitsDisplayData = new List<LimitDisplayData>();

			string userLanguage = m_userContext.User.Language;
			CSGenio.persistence.PersistentSupport sp = m_userContext.PersistentSupport;

			// Iterate limits and set display data.
			foreach (Limit limit in this.tableLimits)
			{
				LimitDisplayData limitDisplayData = new LimitDisplayData();
				limitDisplayData.Type = Enum.GetName(typeof(LimitType), limit.TipoLimite);

				string Area = "",
					AreaPlural = "",
					Field = "",
					Value = "";

				FillAreaFieldDisplayData(
					limit.AreaLimita,
					limit.CampoLimita,
					ref Area,
					ref AreaPlural,
					ref Field,
					ref Value,
					TableAlias,
					userLanguage,
					sp
				);

				limitDisplayData.Area = Area;
				limitDisplayData.AreaPlural = AreaPlural;
				limitDisplayData.Field = Field;
				limitDisplayData.Value = Value;

				string AreaN = "",
					AreaNPlural = "",
					FieldN = "",
					ValueN = "";

				FillAreaFieldDisplayData(
					limit.AreaLimitaN,
					limit.CampoLimitaN,
					ref AreaN,
					ref AreaNPlural,
					ref FieldN,
					ref ValueN,
					TableAlias,
					userLanguage,
					sp
				);

				limitDisplayData.AreaN = AreaN;
				limitDisplayData.AreaNPlural = AreaNPlural;
				limitDisplayData.FieldN = FieldN;
				limitDisplayData.ValueN = ValueN;

				string AreaToCompare = "",
					AreaToComparePlural = "",
					FieldToCompare = "",
					ValueToCompare = "";

				FillAreaFieldDisplayData(
					limit.AreaComparar,
					limit.CampoComparar,
					ref AreaToCompare,
					ref AreaToComparePlural,
					ref FieldToCompare,
					ref ValueToCompare,
					TableAlias,
					userLanguage,
					sp
				);

				limitDisplayData.AreaToCompare = AreaToCompare;
				limitDisplayData.AreaToComparePlural = AreaToComparePlural;
				limitDisplayData.FieldToCompare = FieldToCompare;
				limitDisplayData.ValueToCompare = ValueToCompare;

				if (limit.AreaLimita != null)
				{
					// Between dates (min).
					if (limit.AreaLimita.Fields.ContainsKey(limit.AreaLimita.Alias + "." + "minLim"))
					{
						string minLimValue = limit.AreaLimita.Fields[
							limit.AreaLimita.Alias + "." + "minLim"
						].ToString();

						limitDisplayData.ValueMin = GenioMVC.Models.AuditModel.GetHumanValue(
							sp,
							limit.AreaLimita.Information,
							limit.CampoLimita,
							minLimValue,
							m_userContext.User.Language
						);
					}

					// Between dates (max).
					if (limit.AreaLimita.Fields.ContainsKey(limit.AreaLimita.Alias + "." + "maxLim"))
					{
						string maxLimValue = limit.AreaLimita.Fields[
							limit.AreaLimita.Alias + "." + "maxLim"
						].ToString();

						limitDisplayData.ValueMax = GenioMVC.Models.AuditModel.GetHumanValue(
							sp,
							limit.AreaLimita.Information,
							limit.CampoLimita,
							maxLimValue,
							m_userContext.User.Language
						);
					}
				}

				limitDisplayData.OperatorType = limit.TipoLimiteOperator ?? "";

				// OperatorThreshold for SU limit.
				switch (limit.TipoLimiteSU)
				{
					case OperationType.LESS:
						limitDisplayData.OperatorThreshold = "<";
						break;
					case OperationType.LESSEQUAL:
						limitDisplayData.OperatorThreshold = "<=";
						break;
					case OperationType.GREAT:
						limitDisplayData.OperatorThreshold = ">";
						break;
					case OperationType.GREATEQUAL:
						limitDisplayData.OperatorThreshold = ">=";
						break;
					case OperationType.DIFF:
						limitDisplayData.OperatorThreshold = "<>";
						break;
					case OperationType.EQUAL:
					default:
						limitDisplayData.OperatorThreshold = "=";
						break;
				}

				limitDisplayData.ManualHTMLText = limit.ManualHTMLText ?? "";
				limitDisplayData.ApplyOnlyIfExists = limit.NaoAplicaSeNulo.ToString();

				// Add limit data to array.
				this.tableLimitsDisplayData.Add(limitDisplayData);
			}
		}

		/// <summary>
		/// Fills the area field display data.
		/// </summary>
		/// <param name="LimitArea">The limit area.</param>
		/// <param name="LimitField">The limit field.</param>
		/// <param name="Area">The area.</param>
		/// <param name="AreaPlural">The area plural.</param>
		/// <param name="Field">The field.</param>
		/// <param name="Value">The value.</param>
		/// <param name="TableAlias">The table alias.</param>
		/// <param name="userLanguage">The user language.</param>
		/// <param name="sp">The persistent support.</param>
		private void FillAreaFieldDisplayData(
			CSGenio.business.Area LimitArea,
			Field LimitField,
			ref string Area,
			ref string AreaPlural,
			ref string Field,
			ref string Value,
			string TableAlias,
			string userLanguage,
			CSGenio.persistence.PersistentSupport sp
		)
		{
			if (LimitArea != null)
			{
				// Area
				if (LimitArea.Alias != TableAlias)
				{
					// Naming with translations
					string Designation = CSGenio.framework.Translations.Get(
						LimitArea.AreaDesignation,
						userLanguage
					);
					string PluralDesignation = CSGenio.framework.Translations.Get(
						LimitArea.AreaPluralDesignation,
						userLanguage
					);
					string Alias = CSGenio.framework.Translations.Get(
						LimitArea.Alias,
						userLanguage
					);

					Area = !string.IsNullOrEmpty(Designation) ? Designation : Alias;
					AreaPlural = !string.IsNullOrEmpty(PluralDesignation)
						? PluralDesignation
						: Alias;
				}

				// Field
				if (LimitField != null)
				{
					string FieldName = LimitField.Name;
					string[] HumanFields = LimitArea.Information.HumanKeyName.Split(',');

					if (
						FieldName != LimitArea.Information.PrimaryKeyName
						&& (!HumanFields.Contains(FieldName) || LimitArea.Alias == TableAlias)
					)
					{
						//Naming with Translations
						//CampoLimita
						string Description = CSGenio.framework.Translations.Get(
							LimitArea.DBFields[FieldName].FieldDescription,
							userLanguage
						);
						string Name = LimitArea.DBFields[FieldName].Name;

						Field = !string.IsNullOrEmpty(Description) ? Description : Name;
					}
					else if (
						LimitArea.Alias == TableAlias
						&& FieldName == LimitArea.Information.PrimaryKeyName
						&& CSGenio.business.GlobalFunctions.emptyC(
							LimitArea.Information.HumanKeyName
						) == 0
					) //special case
					{
						//Naming with Translations
						//CampoLimita (as humankey)
						string HumanKeyDescription = CSGenio.framework.Translations.Get(
							LimitArea.DBFields[
								LimitArea.Information.HumanKeyName.Split(',')[0]
							].FieldDescription,
							userLanguage
						);
						string HumanKeyName = LimitArea.DBFields[
							LimitArea.Information.HumanKeyName.Split(',')[0]
						].Name;

						Field = !string.IsNullOrEmpty(HumanKeyDescription)
							? HumanKeyDescription
							: HumanKeyName;
					}
					//Value
					if (LimitArea.Fields.ContainsKey(LimitArea.Alias + "." + FieldName))
					{
						string FieldValue = (
							(CSGenio.framework.RequestedField)
								LimitArea.Fields[LimitArea.Alias + "." + FieldName]
							).Value.ToString();

						Value = GenioMVC.Models.AuditModel.GetHumanValue(
							sp,
							LimitArea.Information,
							LimitField,
							FieldValue,
							m_userContext.User.Language
						);
					}
				}
			}
		}

		/// <summary>
		/// Gets the available search columns,
		/// with respect to the user column configuration.
		/// </summary>
		/// <param name="includeInvisibleFields">Whether to include invisible fields.</param>
		public List<TableSearchColumn> GetSearchColumns(bool includeInvisibleFields = false)
		{
			// If the user has some hidden columns we should not search in them
			if (includeInvisibleFields)
				return SearchableColumns;

			//JGF 2021.09.01 Moved this line nearer the usage, it was going to the server a lot needlessly
			var userColumns = UserUiSettings
				.Load(m_userContext.PersistentSupport, Uuid, m_userContext.User)
				.userColumns;

			return SearchableColumns.Where(tsc => IsColumnVisible(tsc, userColumns)).ToList();
		}

		/// <summary>
		/// Gets the list of columns to export.
		/// </summary>
		/// <param name="ajaxRequest">Whether the request was initiated via AJAX.</param>
		public abstract List<Exports.QColumn> GetColumnsToExport(bool ajaxRequest = false);

		/// <summary>
		/// Builds the list CriteriaSet with all the limits, filters and conditions
		/// </summary>
		/// <param name="requestValues">Parameters from the request</param>
		/// <param name="tableReload">[Quick fix] Indicates whether the data list should be loaded. If set to false within the method, it signals that the data list should not display rows due to unmet mandatory limits.</param>
		/// <param name="crs">Pass a CriteriaSet by reference to be modified</param>
		/// <param name="isToExport">If the  table is to be exported</param>
		public abstract CriteriaSet BuildCriteriaSet(NameValueCollection requestValues, out bool tableReload, CriteriaSet crs = null, bool isToExport = false);

		/// <summary>
		/// Builds the list CriteriaSet with all the limits, filters and conditions
		/// </summary>
		/// <param name="tableConfig">Table configuration object</param>
		/// <param name="requestValues">Parameters from the request</param>
		/// <param name="tableReload">[Quick fix] Indicates whether the data list should be loaded. If set to false within the method, it signals that the data list should not display rows due to unmet mandatory limits.</param>
		/// <param name="crs">Pass a CriteriaSet by reference to be modified</param>
		/// <param name="isToExport">If the  table is to be exported</param>
		/// <inheritdoc/>
		public abstract CriteriaSet BuildCriteriaSet(CSGenio.framework.TableConfiguration.TableConfiguration tableConfig, NameValueCollection requestValues, out bool tableReload, CriteriaSet crs = null, bool isToExport = false);

		/// <summary>
		/// Gets the list of columns to export, accounting for the column configuration.
		/// </summary>
		/// <param name="ColumnConfiguration">Column order and visibility</param>
		public List<Exports.QColumn> GetExportColumns(List<CSGenio.framework.TableConfiguration.ColumnConfiguration> ColumnConfiguration)
		{
			List<Exports.QColumn> defaultColumns = this.GetColumnsToExport(false);
			List<Exports.QColumn> configuredColumns = new List<Exports.QColumn>();

			// If configuration is defined, get visible columns from the default configuration
			if (ColumnConfiguration == null)
				return defaultColumns.Where((col) => col.Visible == true).ToList();

			// Get column data with the order and visibility set in the column configuration
			foreach (CSGenio.framework.TableConfiguration.ColumnConfiguration currentConfiguredColumn in ColumnConfiguration)
			{
				if (currentConfiguredColumn == null || currentConfiguredColumn.Name == null)
					continue;

				// Get the full column name (table.column)
				string currentColumnName;
				string currentTableName;
				int sepIdx = currentConfiguredColumn.Name.IndexOf(".");

				// If the name only has the column name, set the table name as the name of this table
				if (sepIdx == -1)
					currentTableName = this.TableAlias.ToLower();
				// If the name has the table and column names, use the part before the '.' as the table name
				else
					currentTableName = currentConfiguredColumn.Name.Substring(0, sepIdx).ToLower();

				// Use the part of the name after the '.' as the column name
				currentColumnName = currentConfiguredColumn.Name.Substring(sepIdx + 4).ToLower();

				// Get the column that has the matching name
				Exports.QColumn currentColumn = defaultColumns.Find((col) => col.Name.Equals(currentTableName + '.' + currentColumnName));

				if (currentColumn == null)
					continue;

				// Set the visibility to match the configuration of this column
				currentColumn.Visible = currentConfiguredColumn.Visibility == 1;

				// If the column is visible, add it to the list of columns to export
				if (currentColumn.Visible)
					configuredColumns.Add(currentColumn);
			}

			return configuredColumns;
		}

		/// <summary>
		/// Counts the total number of records in the data underlying this list
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public abstract int GetCount(User user);

		/// <summary>
		/// Instantiates a new ListViewModel given its name
		/// </summary>
		/// <param name="userContext">The current user context</param>
		/// <param name="controller">Base of the list</param>
		/// <param name="action">Id of the list</param>
		/// <returns>The instantiated ListViewModel</returns>
		public static ListViewModel CreateListViewModel(UserContext userContext, string controller, string action)
		{
			string viewmodelStr = string.Format("GenioMVC.ViewModels.{0}.{1}_ViewModel", controller, action);
			var viewmodelType = Type.GetType(viewmodelStr, false, true) ?? throw new InvalidOperationException($"Could not instantiate a ListViewModel for {controller}/{action}");
			var newViewmodel = Activator.CreateInstance(viewmodelType, userContext);
			return newViewmodel as ListViewModel ?? throw new InvalidOperationException($"Could not instantiate a ListViewModel for {controller}/{action}");
		}
	}
}
