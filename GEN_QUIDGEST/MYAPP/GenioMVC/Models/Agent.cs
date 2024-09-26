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
	public class Agent : ModelBase
	{
		[JsonIgnore]
		public CSGenioAagent klass { get { return baseklass as CSGenioAagent; } set { baseklass = value; } }

		[Key]
		/// <summary>Field : "" Tipo: "+" Formula:  ""</summary>
		[ShouldSerialize("Agent.ValCodagent")]
		public string ValCodagent { get { return klass.ValCodagent; } set { klass.ValCodagent = value; } }

		[DisplayName("Fotografia")]
		/// <summary>Field : "Fotografia" Tipo: "IJ" Formula:  ""</summary>
		[ShouldSerialize("Agent.ValFoto")]
		[ImageThumbnailJsonConverter(75, 75)]
		public ImageModel ValFoto { get { return new ImageModel(klass.ValFoto) { Ticket = ValFotoQTicket }; } set { klass.ValFoto = value; } }
		[JsonIgnore]
		public string ValFotoQTicket = null;

		[DisplayName("Nome")]
		/// <summary>Field : "Nome" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("Agent.ValNome")]
		public string ValNome { get { return klass.ValNome; } set { klass.ValNome = value; } }

		[DisplayName("Data de nascimento")]
		/// <summary>Field : "Data de nascimento" Tipo: "D" Formula:  ""</summary>
		[ShouldSerialize("Agent.ValDnascime")]
		[DataType(DataType.Date)]
		[DateAttribute("D")]
		public DateTime? ValDnascime { get { return klass.ValDnascime; } set { klass.ValDnascime = value ?? DateTime.MinValue; } }

		[DisplayName("E-mail")]
		/// <summary>Field : "E-mail" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("Agent.ValEmail")]
		public string ValEmail { get { return klass.ValEmail; } set { klass.ValEmail = value; } }

		[DisplayName("Telefone")]
		/// <summary>Field : "Telefone" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("Agent.ValTelefone")]
		public string ValTelefone { get { return klass.ValTelefone; } set { klass.ValTelefone = value; } }

		[DisplayName("")]
		/// <summary>Field : "" Tipo: "CE" Formula:  ""</summary>
		[ShouldSerialize("Agent.ValCodpmora")]
		public string ValCodpmora { get { return klass.ValCodpmora; } set { klass.ValCodpmora = value; } }
		private Pmora _pmora;
		[DisplayName("Pmora")]
		[ShouldSerialize("Pmora")]
		public virtual Pmora Pmora {
			get {
				if (!this.isEmptyModel && (_pmora == null || (!string.IsNullOrEmpty(ValCodpmora) && (_pmora.isEmptyModel || _pmora.klass.QPrimaryKey != ValCodpmora))))
					_pmora = Models.Pmora.Find(ValCodpmora, m_userContext, Identifier, _fieldsToSerialize);
				if (_pmora == null)
					_pmora = new Models.Pmora(m_userContext, true, _fieldsToSerialize);
				return _pmora;
			}
			set { _pmora = value; }
		}


		[DisplayName("")]
		/// <summary>Field : "" Tipo: "CE" Formula:  ""</summary>
		[ShouldSerialize("Agent.ValCodpnasc")]
		public string ValCodpnasc { get { return klass.ValCodpnasc; } set { klass.ValCodpnasc = value; } }
		private Pnasc _pnasc;
		[DisplayName("Pnasc")]
		[ShouldSerialize("Pnasc")]
		public virtual Pnasc Pnasc {
			get {
				if (!this.isEmptyModel && (_pnasc == null || (!string.IsNullOrEmpty(ValCodpnasc) && (_pnasc.isEmptyModel || _pnasc.klass.QPrimaryKey != ValCodpnasc))))
					_pnasc = Models.Pnasc.Find(ValCodpnasc, m_userContext, Identifier, _fieldsToSerialize);
				if (_pnasc == null)
					_pnasc = new Models.Pnasc(m_userContext, true, _fieldsToSerialize);
				return _pnasc;
			}
			set { _pnasc = value; }
		}


		[DisplayName("% lucro")]
		/// <summary>Field : "% lucro" Tipo: "N" Formula:  ""</summary>
		[ShouldSerialize("Agent.ValPerelucr")]
		[NumericAttribute(2)]
		public decimal? ValPerelucr { get { return Convert.ToDecimal(GlobalFunctions.RoundQG(klass.ValPerelucr, 2)); } set { klass.ValPerelucr = Convert.ToDecimal(value); } }

		[DisplayName("Lucro")]
		/// <summary>Field : "Lucro" Tipo: "$" Formula: SR "[PROPR->LUCRO]"</summary>
		[ShouldSerialize("Agent.ValLucro")]
		[CurrencyAttribute("EUR", 4)]
		public decimal? ValLucro { get { return Convert.ToDecimal(GlobalFunctions.RoundQG(klass.ValLucro, 4)); } set { klass.ValLucro = Convert.ToDecimal(value); } }

		[DisplayName("ZZSTATE")]
		[ShouldSerialize("Agent.ValZzstate")]
		/// <summary>Field : "ZZSTATE" Type: "INT" Formula:  ""</summary>
		public int ValZzstate { get { return klass.ValZzstate; } set { klass.ValZzstate = value; } }

		public Agent(UserContext userContext, bool isEmpty = false, string[]? fieldsToSerialize = null) : base(userContext)
		{
			klass = new CSGenioAagent(userContext.User);
			isEmptyModel = isEmpty;
			if (fieldsToSerialize != null)
				SetFieldsToSerialize(fieldsToSerialize);
		}

		public Agent(UserContext userContext, CSGenioAagent val, bool isEmpty = false, string[]? fieldsToSerialize = null) : base(userContext)
		{
			klass = val;
			isEmptyModel = isEmpty;
			if (fieldsToSerialize != null)
				SetFieldsToSerialize(fieldsToSerialize);
			FillRelatedAreas(val);
		}


		public void FillRelatedAreas(CSGenioAagent csgenioa)
		{
			if (csgenioa == null)
				return;

			foreach (RequestedField Qfield in csgenioa.Fields.Values)
			{
				switch (Qfield.Area)
				{
					case "pmora":
						if (_pmora == null)
							_pmora = new Pmora(m_userContext, true, _fieldsToSerialize);
						_pmora.klass.insertNameValueField(Qfield.FullName, Qfield.Value);
						break;
					case "pnasc":
						if (_pnasc == null)
							_pnasc = new Pnasc(m_userContext, true, _fieldsToSerialize);
						_pnasc.klass.insertNameValueField(Qfield.FullName, Qfield.Value);
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
		public static Agent Find(string id, UserContext userCtx, string identifier = null, string[] fieldsToSerialize = null, string[] fieldsToQuery = null)
		{
			var record = Find<CSGenioAagent>(id, userCtx, identifier, fieldsToQuery);
			return record == null ? null : new Agent(userCtx, record, false, fieldsToSerialize) { Identifier = identifier };
		}

		public static List<Agent> AllModel(UserContext userCtx, CriteriaSet args = null, string identifier = null)
		{
			return Where<CSGenioAagent>(userCtx, false, args, numRegs: -1, identifier: identifier).RowsForViewModel<Agent>((r) => new Agent(userCtx, r));
		}

// USE /[MANUAL PRO MODEL AGENT]/
	}
}
