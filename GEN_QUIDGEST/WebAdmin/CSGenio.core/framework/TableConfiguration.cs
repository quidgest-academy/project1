using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CSGenio.framework.TableConfiguration
{
    public class ToStringArrayConverter : System.Text.Json.Serialization.JsonConverter<string[]>
    {
        public override string[] Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            List<string> stringList = new List<string>();
            object[] array;
            try
            {
                // Deserialize to an object array
                array = JsonSerializer.Deserialize<object[]>(JsonElement.ParseValue(ref reader));
            }
            catch (Exception ex) 
            {
                Log.Error(ex.Message);
                array = new object[0];
            }

            // Convert all values to strings
            foreach (object item in array) 
            {
                stringList.Add(item.ToString());
            }

            return stringList.ToArray();
        }

        public override void Write(Utf8JsonWriter writer, string[] value, JsonSerializerOptions options)
        {
            string serializedArray;
            try 
            {
                // Serialize array
                serializedArray = JsonSerializer.Serialize(value);
            }
            catch (Exception ex) 
            {
                Log.Error(ex.Message);
                serializedArray = "[]";
            }

            // Write value as raw so it is an array of strings
            writer.WriteRawValue(serializedArray);
        }
    }

    public class SearchFilter
    {
        [JsonPropertyName("name")]
		public string Name { get; set; }

        [JsonPropertyName("active")]
		public bool Active { get; set; }

        [JsonPropertyName("conditions")]
		public SearchFilterCondition[] Conditions { get; set; }
    }

    public class SearchFilterCondition
    {
        [JsonPropertyName("name")]
		public string Name { get; set; }

        [JsonPropertyName("active")]
		public bool Active { get; set; }

        [JsonPropertyName("field")]
		public string Field { get; set; }

        [JsonPropertyName("operator")]
		public string Operator { get; set; }

        [JsonPropertyName("values")]
		[System.Text.Json.Serialization.JsonConverter(typeof(ToStringArrayConverter))]
        public string[] Values { get; set; }
    }

    public class ActiveFilter
    {
        [JsonPropertyName("date")]
		public string Date { get; set; }

        [JsonPropertyName("active")]
		public bool Active { get; set; }

        [JsonPropertyName("inactive")]
		public bool Inactive { get; set; }

        [JsonPropertyName("future")]
		public bool Future { get; set; }
    }

    public class ColumnOrderBy
    {
        [JsonPropertyName("columnName")]
		public string ColumnName { get; set; }

        [JsonPropertyName("sortOrder")]
		public string SortOrder { get; set; }
    }

    public class ColumnConfiguration
    {
        [JsonPropertyName("name")]
		public string Name { get; set; }

        [JsonPropertyName("order")]
		public int Order { get; set; }

        [JsonPropertyName("visibility")]
		public int Visibility { get; set; }
    }

    public class ColumnSizing
    {
        [JsonPropertyName("tableSize")]
		public string TableSize { get; set; }

        [JsonPropertyName("columnSizes")]
		public Dictionary<string, string> ColumnSizes { get; set; }
    }

    public class TableConfiguration
    {
		[JsonPropertyName("name")]
        public string Name { get; set; }

		[JsonPropertyName("columnConfiguration")]
        public List<ColumnConfiguration> ColumnConfiguration { get; set; }

        [JsonPropertyName("advancedFilters")]
		public List<SearchFilter> AdvancedFilters { get; set; }

        [JsonPropertyName("columnFilters")]
		public Dictionary<string, SearchFilter> ColumnFilters { get; set; }

        [JsonPropertyName("searchBarFilters")]
		public Dictionary<string, SearchFilter> SearchBarFilters { get; set; }

        [JsonPropertyName("staticFilters")]
		public Dictionary<string, string> StaticFilters { get; set; } = new Dictionary<string, string>();

        [JsonPropertyName("activeFilter")]
		public ActiveFilter ActiveFilter { get; set; }

        [JsonPropertyName("columnOrderBy")]
		public ColumnOrderBy ColumnOrderBy { get; set; }

        [JsonPropertyName("defaultSearchColumn")]
		public string DefaultSearchColumn { get; set; }

        [JsonPropertyName("columnSizes")]
		public ColumnSizing ColumnSizes { get; set; }

        [JsonPropertyName("lineBreak")]
		public bool LineBreak { get; set; }

        [JsonPropertyName("rowsPerPage")]
		public int RowsPerPage { get; set; }

        [JsonPropertyName("page")]
		public int Page { get; set; } = 1;

        [JsonPropertyName("query")]
		public string Query { get; set; }

        // Advanced filters, column filters, and searchbar filters merged
        [JsonIgnore]
        public List<SearchFilter> SearchFilters
        {
            get
            {
                List<SearchFilter> searchFilters = new List<SearchFilter>();

                if (AdvancedFilters != null)
                    searchFilters.AddRange(AdvancedFilters);

                if (ColumnFilters != null)
                    searchFilters.AddRange(ColumnFilters.Values);

                if (SearchBarFilters != null)
                    searchFilters.AddRange(SearchBarFilters.Values);

                return searchFilters;
            }
        }
    }
	
	public class TableConfigurationHelpers
	{
		/// <summary>
		/// Determine the number of rows per page to use based on the tables options and configuration
		/// </summary>
		/// <param name="tableConfigRowsPerPage">Number of rows per page in table configuration</param>
		/// <param name="defaultRowsPerPage">Default number of rows per page</param>
		/// <param name="rowsPerPageOptionsString">Rows per page options as a string of values separated by commas</param>
		/// <returns>The number of rows per page.</returns>
		public static int DetermineRowsPerPage(int tableConfigRowsPerPage, int defaultRowsPerPage, string rowsPerPageOptionsString)
		{
			List<int> rowsPerPageOptions = new List<int>();

			// Split string into array of string values
			string[] optionsStrArr = string.IsNullOrEmpty(rowsPerPageOptionsString) ? new string[0] : rowsPerPageOptionsString.Split(',');
			int res;

			// Convert string values to integers and add to list
			foreach (string str in optionsStrArr)
			{
				if (int.TryParse(str, out res))
					rowsPerPageOptions.Add(res);
			}

			// If rows per page is the default or a value in the defined options, use it
			if (tableConfigRowsPerPage == defaultRowsPerPage
				|| (rowsPerPageOptions != null
				&& rowsPerPageOptions.Contains(tableConfigRowsPerPage)))
				return tableConfigRowsPerPage;

			// If not, use the default
			return defaultRowsPerPage;
		}
	}
}
