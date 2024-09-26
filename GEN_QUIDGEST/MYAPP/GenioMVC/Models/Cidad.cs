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
	public class Cidad : ModelBase
	{
		[JsonIgnore]
		public CSGenioAcidad klass { get { return baseklass as CSGenioAcidad; } set { baseklass = value; } }

		[Key]
		/// <summary>Field : "" Tipo: "+" Formula:  ""</summary>
		[ShouldSerialize("Cidad.ValCodcidad")]
		public string ValCodcidad { get { return klass.ValCodcidad; } set { klass.ValCodcidad = value; } }

		[DisplayName("Cidade")]
		/// <summary>Field : "Cidade" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("Cidad.ValCidade")]
		public string ValCidade { get { return klass.ValCidade; } set { klass.ValCidade = value; } }

		[DisplayName("")]
		/// <summary>Field : "" Tipo: "CE" Formula:  ""</summary>
		[ShouldSerialize("Cidad.ValCodpais")]
		public string ValCodpais { get { return klass.ValCodpais; } set { klass.ValCodpais = value; } }
		private Pais _pais;
		[DisplayName("Pais")]
		[ShouldSerialize("Pais")]
		public virtual Pais Pais {
			get {
				if (!this.isEmptyModel && (_pais == null || (!string.IsNullOrEmpty(ValCodpais) && (_pais.isEmptyModel || _pais.klass.QPrimaryKey != ValCodpais))))
					_pais = Models.Pais.Find(ValCodpais, m_userContext, Identifier, _fieldsToSerialize);
				if (_pais == null)
					_pais = new Models.Pais(m_userContext, true, _fieldsToSerialize);
				return _pais;
			}
			set { _pais = value; }
		}


		[DisplayName("ZZSTATE")]
		[ShouldSerialize("Cidad.ValZzstate")]
		/// <summary>Field : "ZZSTATE" Type: "INT" Formula:  ""</summary>
		public int ValZzstate { get { return klass.ValZzstate; } set { klass.ValZzstate = value; } }

		public Cidad(UserContext userContext, bool isEmpty = false, string[]? fieldsToSerialize = null) : base(userContext)
		{
			klass = new CSGenioAcidad(userContext.User);
			isEmptyModel = isEmpty;
			if (fieldsToSerialize != null)
				SetFieldsToSerialize(fieldsToSerialize);
		}

		public Cidad(UserContext userContext, CSGenioAcidad val, bool isEmpty = false, string[]? fieldsToSerialize = null) : base(userContext)
		{
			klass = val;
			isEmptyModel = isEmpty;
			if (fieldsToSerialize != null)
				SetFieldsToSerialize(fieldsToSerialize);
			FillRelatedAreas(val);
		}


		public void FillRelatedAreas(CSGenioAcidad csgenioa)
		{
			if (csgenioa == null)
				return;

			foreach (RequestedField Qfield in csgenioa.Fields.Values)
			{
				switch (Qfield.Area)
				{
					case "pais":
						if (_pais == null)
							_pais = new Pais(m_userContext, true, _fieldsToSerialize);
						_pais.klass.insertNameValueField(Qfield.FullName, Qfield.Value);
						break;
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
		public static Cidad Find(string id, UserContext userCtx, string identifier = null, string[] fieldsToSerialize = null, string[] fieldsToQuery = null)
		{
			var record = Find<CSGenioAcidad>(id, userCtx, identifier, fieldsToQuery);
			return record == null ? null : new Cidad(userCtx, record, false, fieldsToSerialize) { Identifier = identifier };
		}

		public static List<Cidad> AllModel(UserContext userCtx, CriteriaSet args = null, string identifier = null)
		{
			return Where<CSGenioAcidad>(userCtx, false, args, numRegs: -1, identifier: identifier).RowsForViewModel<Cidad>((r) => new Cidad(userCtx, r));
		}

// USE /[MANUAL PRO MODEL CIDAD]/
	}
}
