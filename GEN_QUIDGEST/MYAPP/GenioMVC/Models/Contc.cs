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
	public class Contc : ModelBase
	{
		[JsonIgnore]
		public CSGenioAcontc klass { get { return baseklass as CSGenioAcontc; } set { baseklass = value; } }

		[Key]
		/// <summary>Field : "" Tipo: "+" Formula:  ""</summary>
		[ShouldSerialize("Contc.ValCodcontc")]
		public string ValCodcontc { get { return klass.ValCodcontc; } set { klass.ValCodcontc = value; } }

		[DisplayName("Data do contacto")]
		/// <summary>Field : "Data do contacto" Tipo: "D" Formula:  ""</summary>
		[ShouldSerialize("Contc.ValDtcontat")]
		[DataType(DataType.Date)]
		[DateAttribute("D")]
		public DateTime? ValDtcontat { get { return klass.ValDtcontat; } set { klass.ValDtcontat = value ?? DateTime.MinValue; } }

		[DisplayName("")]
		/// <summary>Field : "" Tipo: "CE" Formula:  ""</summary>
		[ShouldSerialize("Contc.ValCodpropr")]
		public string ValCodpropr { get { return klass.ValCodpropr; } set { klass.ValCodpropr = value; } }
		private Propr _propr;
		[DisplayName("Propr")]
		[ShouldSerialize("Propr")]
		public virtual Propr Propr {
			get {
				if (!this.isEmptyModel && (_propr == null || (!string.IsNullOrEmpty(ValCodpropr) && (_propr.isEmptyModel || _propr.klass.QPrimaryKey != ValCodpropr))))
					_propr = Models.Propr.Find(ValCodpropr, m_userContext, Identifier, _fieldsToSerialize);
				if (_propr == null)
					_propr = new Models.Propr(m_userContext, true, _fieldsToSerialize);
				return _propr;
			}
			set { _propr = value; }
		}


		[DisplayName("Nome do cliente")]
		/// <summary>Field : "Nome do cliente" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("Contc.ValCltname")]
		public string ValCltname { get { return klass.ValCltname; } set { klass.ValCltname = value; } }

		[DisplayName("Email do cliente")]
		/// <summary>Field : "Email do cliente" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("Contc.ValCltemail")]
		public string ValCltemail { get { return klass.ValCltemail; } set { klass.ValCltemail = value; } }

		[DisplayName("Telefone")]
		/// <summary>Field : "Telefone" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("Contc.ValTelefone")]
		public string ValTelefone { get { return klass.ValTelefone; } set { klass.ValTelefone = value; } }

		[DisplayName("Descrição")]
		/// <summary>Field : "Descrição" Tipo: "MO" Formula:  ""</summary>
		[ShouldSerialize("Contc.ValDescriic")]
		[DataType(DataType.MultilineText)]
		public string ValDescriic { get { return klass.ValDescriic; } set { klass.ValDescriic = value; } }

		[DisplayName("ZZSTATE")]
		[ShouldSerialize("Contc.ValZzstate")]
		/// <summary>Field : "ZZSTATE" Type: "INT" Formula:  ""</summary>
		public int ValZzstate { get { return klass.ValZzstate; } set { klass.ValZzstate = value; } }

		public Contc(UserContext userContext, bool isEmpty = false, string[]? fieldsToSerialize = null) : base(userContext)
		{
			klass = new CSGenioAcontc(userContext.User);
			isEmptyModel = isEmpty;
			if (fieldsToSerialize != null)
				SetFieldsToSerialize(fieldsToSerialize);
		}

		public Contc(UserContext userContext, CSGenioAcontc val, bool isEmpty = false, string[]? fieldsToSerialize = null) : base(userContext)
		{
			klass = val;
			isEmptyModel = isEmpty;
			if (fieldsToSerialize != null)
				SetFieldsToSerialize(fieldsToSerialize);
			FillRelatedAreas(val);
		}


		public void FillRelatedAreas(CSGenioAcontc csgenioa)
		{
			if (csgenioa == null)
				return;

			foreach (RequestedField Qfield in csgenioa.Fields.Values)
			{
				switch (Qfield.Area)
				{
					case "propr":
						if (_propr == null)
							_propr = new Propr(m_userContext, true, _fieldsToSerialize);
						_propr.klass.insertNameValueField(Qfield.FullName, Qfield.Value);
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
		public static Contc Find(string id, UserContext userCtx, string identifier = null, string[] fieldsToSerialize = null, string[] fieldsToQuery = null)
		{
			var record = Find<CSGenioAcontc>(id, userCtx, identifier, fieldsToQuery);
			return record == null ? null : new Contc(userCtx, record, false, fieldsToSerialize) { Identifier = identifier };
		}

		public static List<Contc> AllModel(UserContext userCtx, CriteriaSet args = null, string identifier = null)
		{
			return Where<CSGenioAcontc>(userCtx, false, args, numRegs: -1, identifier: identifier).RowsForViewModel<Contc>((r) => new Contc(userCtx, r));
		}

// USE /[MANUAL PRO MODEL CONTC]/
	}
}
