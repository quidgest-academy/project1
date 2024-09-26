using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using CSGenio.business;
using CSGenio.framework;
using CSGenio.persistence;
using GenioMVC.Helpers;
using GenioMVC.Models.Navigation;
using Quidgest.Persistence;
using Quidgest.Persistence.GenericQuery;

using SelectList = Microsoft.AspNetCore.Mvc.Rendering.SelectList;
using JsonIgnoreAttribute = System.Text.Json.Serialization.JsonIgnoreAttribute;

namespace GenioMVC.Models
{
	public class Pnasc : ModelBase
	{
		[JsonIgnore]
		public CSGenioApnasc klass { get { return baseklass as CSGenioApnasc; } set { baseklass = value; } }

		[Key]
		/// <summary>Field : "" Tipo: "+" Formula:  ""</summary>
		[ShouldSerialize("Pnasc.ValCodpais")]
		public string ValCodpais { get { return klass.ValCodpais; } set { klass.ValCodpais = value; } }

		[DisplayName("País")]
		/// <summary>Field : "País" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("Pnasc.ValPais")]
		public string ValPais { get { return klass.ValPais; } set { klass.ValPais = value; } }

		[DisplayName("ZZSTATE")]
		[ShouldSerialize("Pnasc.ValZzstate")]
		/// <summary>Field : "ZZSTATE" Type: "INT" Formula:  ""</summary>
		public int ValZzstate { get { return klass.ValZzstate; } set { klass.ValZzstate = value; } }

		public Pnasc(UserContext userContext, bool isEmpty = false, string[]? fieldsToSerialize = null) : base(userContext)
		{
			klass = new CSGenioApnasc(userContext.User);
			isEmptyModel = isEmpty;
			if (fieldsToSerialize != null)
				SetFieldsToSerialize(fieldsToSerialize);
		}

		public Pnasc(UserContext userContext, CSGenioApnasc val, bool isEmpty = false, string[]? fieldsToSerialize = null) : base(userContext)
		{
			klass = val;
			isEmptyModel = isEmpty;
			if (fieldsToSerialize != null)
				SetFieldsToSerialize(fieldsToSerialize);
			FillRelatedAreas(val);
		}


		public void FillRelatedAreas(CSGenioApnasc csgenioa)
		{
			if (csgenioa == null)
				return;

			foreach (RequestedField Qfield in csgenioa.Fields.Values)
			{
				switch (Qfield.Area)
				{
					default:
						break;
				}
			}
		}

		/// <summary>
		/// Search the row by key.
		/// </summary>
		/// <param name="id">The primary key.</param>
		/// <param name="userCtx">The user context.</param>
		/// <param name="identifier">The identifier.</param>
		/// <param name="fieldsToSerialize">The fields to serialize.</param>
		/// <param name="fieldsToQuery">The fields to query.</param>
		/// <returns>Model or NULL</returns>
		public static Pnasc Find(string id, UserContext userCtx, string identifier = null, string[] fieldsToSerialize = null, string[] fieldsToQuery = null)
		{
			var record = Find<CSGenioApnasc>(id, userCtx, identifier, fieldsToQuery);
			return record == null ? null : new Pnasc(userCtx, record, false, fieldsToSerialize) { Identifier = identifier };
		}

		public static List<Pnasc> AllModel(UserContext userCtx, CriteriaSet args = null, string identifier = null)
		{
			return Where<CSGenioApnasc>(userCtx, false, args, numRegs: -1, identifier: identifier).RowsForViewModel<Pnasc>((r) => new Pnasc(userCtx, r));
		}

// USE /[MANUAL PRO MODEL PNASC]/
	}
}
