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
	public class Propr : ModelBase
	{
		[JsonIgnore]
		public CSGenioApropr klass { get { return baseklass as CSGenioApropr; } set { baseklass = value; } }

		[Key]
		/// <summary>Field : "" Tipo: "+" Formula:  ""</summary>
		[ShouldSerialize("Propr.ValCodpropr")]
		public string ValCodpropr { get { return klass.ValCodpropr; } set { klass.ValCodpropr = value; } }

		[DisplayName("Fotografia")]
		/// <summary>Field : "Fotografia" Tipo: "IJ" Formula:  ""</summary>
		[ShouldSerialize("Propr.ValFoto")]
		[ImageThumbnailJsonConverter(75, 75)]
		public ImageModel ValFoto { get { return new ImageModel(klass.ValFoto) { Ticket = ValFotoQTicket }; } set { klass.ValFoto = value; } }
		[JsonIgnore]
		public string ValFotoQTicket = null;

		[DisplayName("Título")]
		/// <summary>Field : "Título" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("Propr.ValTitulo")]
		public string ValTitulo { get { return klass.ValTitulo; } set { klass.ValTitulo = value; } }

		[DisplayName("Preço")]
		/// <summary>Field : "Preço" Tipo: "$" Formula:  ""</summary>
		[ShouldSerialize("Propr.ValPreco")]
		[CurrencyAttribute("EUR", 4)]
		public decimal? ValPreco { get { return Convert.ToDecimal(GlobalFunctions.RoundQG(klass.ValPreco, 4)); } set { klass.ValPreco = Convert.ToDecimal(value); } }

		[DisplayName("")]
		/// <summary>Field : "" Tipo: "CE" Formula:  ""</summary>
		[ShouldSerialize("Propr.ValCodagent")]
		public string ValCodagent { get { return klass.ValCodagent; } set { klass.ValCodagent = value; } }
		private Agent _agent;
		[DisplayName("Agent")]
		[ShouldSerialize("Agent")]
		public virtual Agent Agent {
			get {
				if (!this.isEmptyModel && (_agent == null || (!string.IsNullOrEmpty(ValCodagent) && (_agent.isEmptyModel || _agent.klass.QPrimaryKey != ValCodagent))))
					_agent = Models.Agent.Find(ValCodagent, m_userContext, Identifier, _fieldsToSerialize);
				if (_agent == null)
					_agent = new Models.Agent(m_userContext, true, _fieldsToSerialize);
				return _agent;
			}
			set { _agent = value; }
		}


		[DisplayName("Tamanho (m2)")]
		/// <summary>Field : "Tamanho (m2)" Tipo: "N" Formula:  ""</summary>
		[ShouldSerialize("Propr.ValTamanho")]
		[NumericAttribute(2)]
		public decimal? ValTamanho { get { return Convert.ToDecimal(GlobalFunctions.RoundQG(klass.ValTamanho, 2)); } set { klass.ValTamanho = Convert.ToDecimal(value); } }

		[DisplayName("Número de casas de banho")]
		/// <summary>Field : "Número de casas de banho" Tipo: "N" Formula:  ""</summary>
		[ShouldSerialize("Propr.ValNr_wcs")]
		[NumericAttribute(0)]
		public decimal? ValNr_wcs { get { return Convert.ToDecimal(GlobalFunctions.RoundQG(klass.ValNr_wcs, 0)); } set { klass.ValNr_wcs = Convert.ToDecimal(value); } }

		[DisplayName("Data de contrução")]
		/// <summary>Field : "Data de contrução" Tipo: "D" Formula:  ""</summary>
		[ShouldSerialize("Propr.ValDtconst")]
		[DataType(DataType.Date)]
		[DateAttribute("D")]
		public DateTime? ValDtconst { get { return klass.ValDtconst; } set { klass.ValDtconst = value ?? DateTime.MinValue; } }

		[DisplayName("Descrição")]
		/// <summary>Field : "Descrição" Tipo: "MO" Formula:  ""</summary>
		[ShouldSerialize("Propr.ValDescrica")]
		[DataType(DataType.MultilineText)]
		public string ValDescrica { get { return klass.ValDescrica; } set { klass.ValDescrica = value; } }

		[DisplayName("")]
		/// <summary>Field : "" Tipo: "CE" Formula:  ""</summary>
		[ShouldSerialize("Propr.ValCodcidad")]
		public string ValCodcidad { get { return klass.ValCodcidad; } set { klass.ValCodcidad = value; } }
		private Cidad _cidad;
		[DisplayName("Cidad")]
		[ShouldSerialize("Cidad")]
		public virtual Cidad Cidad {
			get {
				if (!this.isEmptyModel && (_cidad == null || (!string.IsNullOrEmpty(ValCodcidad) && (_cidad.isEmptyModel || _cidad.klass.QPrimaryKey != ValCodcidad))))
					_cidad = Models.Cidad.Find(ValCodcidad, m_userContext, Identifier, _fieldsToSerialize);
				if (_cidad == null)
					_cidad = new Models.Cidad(m_userContext, true, _fieldsToSerialize);
				return _cidad;
			}
			set { _cidad = value; }
		}


		[DisplayName("Tipo de construção")]
		/// <summary>Field : "Tipo de construção" Tipo: "AC" Formula:  ""</summary>
		[ShouldSerialize("Propr.ValTipoprop")]
		[DataArray("Tipocons", GenioMVC.Helpers.ArrayType.Character)]
		public string ValTipoprop { get { return klass.ValTipoprop; } set { klass.ValTipoprop = value; } }
		[JsonIgnore]
		public SelectList ArrayValtipoprop { get { return new SelectList(CSGenio.business.ArrayTipocons.GetDictionary(), "Key", "Value", ValTipoprop); } set { ValTipoprop = value.SelectedValue as string; } }

		[DisplayName("Tipologia")]
		/// <summary>Field : "Tipologia" Tipo: "AN" Formula:  ""</summary>
		[ShouldSerialize("Propr.ValTipologi")]
		[DataArray("Tipologi", GenioMVC.Helpers.ArrayType.Numeric)]
		public decimal ValTipologi { get { return klass.ValTipologi; } set { klass.ValTipologi = value; } }
		[JsonIgnore]
		public SelectList ArrayValtipologi { get { return new SelectList(CSGenio.business.ArrayTipologi.GetDictionary(), "Key", "Value", ValTipologi); } set { ValTipologi = Convert.ToDecimal(value.SelectedValue); } }

		[DisplayName("ID")]
		/// <summary>Field : "ID" Tipo: "N" Formula:  ""</summary>
		[ShouldSerialize("Propr.ValIdpropre")]
		[NumericAttribute(0)]
		public decimal? ValIdpropre { get { return Convert.ToDecimal(GlobalFunctions.RoundQG(klass.ValIdpropre, 0)); } set { klass.ValIdpropre = Convert.ToDecimal(value); } }

		[DisplayName("Idade da construção")]
		/// <summary>Field : "Idade da construção" Tipo: "N" Formula: + "Year([Today]) - Year([PROPR->DTCONST])"</summary>
		[ShouldSerialize("Propr.ValIdadepro")]
		[NumericAttribute(0)]
		public decimal? ValIdadepro { get { return Convert.ToDecimal(GlobalFunctions.RoundQG(klass.ValIdadepro, 0)); } set { klass.ValIdadepro = Convert.ToDecimal(value); } }

		[DisplayName("Espaço exterior (m2)")]
		/// <summary>Field : "Espaço exterior (m2)" Tipo: "N" Formula:  ""</summary>
		[ShouldSerialize("Propr.ValEspexter")]
		[NumericAttribute(2)]
		public decimal? ValEspexter { get { return Convert.ToDecimal(GlobalFunctions.RoundQG(klass.ValEspexter, 2)); } set { klass.ValEspexter = Convert.ToDecimal(value); } }

		[DisplayName("Vendida")]
		/// <summary>Field : "Vendida" Tipo: "L" Formula:  ""</summary>
		[ShouldSerialize("Propr.ValVendida")]
		public bool ValVendida { get { return Convert.ToBoolean(klass.ValVendida); } set { klass.ValVendida = Convert.ToInt32(value); } }

		[DisplayName("Lucro")]
		/// <summary>Field : "Lucro" Tipo: "$" Formula: + "iif([PROPR->VENDIDA]==1,[PROPR->PRECO]*[AGENT->PERELUCR],0)"</summary>
		[ShouldSerialize("Propr.ValLucro")]
		[CurrencyAttribute("EUR", 4)]
		public decimal? ValLucro { get { return Convert.ToDecimal(GlobalFunctions.RoundQG(klass.ValLucro, 4)); } set { klass.ValLucro = Convert.ToDecimal(value); } }

		[DisplayName("Localização")]
		/// <summary>Field : "Localização" Tipo: "GG" Formula:  ""</summary>
		[ShouldSerialize("Propr.ValLocaliza")]
		[GeographicAttribute("GG")]
		public string ValLocaliza { get { return klass.ValLocaliza; } set { klass.ValLocaliza = value; } }

		[DisplayName("ZZSTATE")]
		[ShouldSerialize("Propr.ValZzstate")]
		/// <summary>Field : "ZZSTATE" Type: "INT" Formula:  ""</summary>
		public int ValZzstate { get { return klass.ValZzstate; } set { klass.ValZzstate = value; } }

		public Propr(UserContext userContext, bool isEmpty = false, string[]? fieldsToSerialize = null) : base(userContext)
		{
			klass = new CSGenioApropr(userContext.User);
			isEmptyModel = isEmpty;
			if (fieldsToSerialize != null)
				SetFieldsToSerialize(fieldsToSerialize);
		}

		public Propr(UserContext userContext, CSGenioApropr val, bool isEmpty = false, string[]? fieldsToSerialize = null) : base(userContext)
		{
			klass = val;
			isEmptyModel = isEmpty;
			if (fieldsToSerialize != null)
				SetFieldsToSerialize(fieldsToSerialize);
			FillRelatedAreas(val);
		}


		public void FillRelatedAreas(CSGenioApropr csgenioa)
		{
			if (csgenioa == null)
				return;

			foreach (RequestedField Qfield in csgenioa.Fields.Values)
			{
				switch (Qfield.Area)
				{
					case "agent":
						if (_agent == null)
							_agent = new Agent(m_userContext, true, _fieldsToSerialize);
						_agent.klass.insertNameValueField(Qfield.FullName, Qfield.Value);
						break;
					case "cidad":
						if (_cidad == null)
							_cidad = new Cidad(m_userContext, true, _fieldsToSerialize);
						_cidad.klass.insertNameValueField(Qfield.FullName, Qfield.Value);
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
		public static Propr Find(string id, UserContext userCtx, string identifier = null, string[] fieldsToSerialize = null, string[] fieldsToQuery = null)
		{
			var record = Find<CSGenioApropr>(id, userCtx, identifier, fieldsToQuery);
			return record == null ? null : new Propr(userCtx, record, false, fieldsToSerialize) { Identifier = identifier };
		}

		public static List<Propr> AllModel(UserContext userCtx, CriteriaSet args = null, string identifier = null)
		{
			return Where<CSGenioApropr>(userCtx, false, args, numRegs: -1, identifier: identifier).RowsForViewModel<Propr>((r) => new Propr(userCtx, r));
		}

// USE /[MANUAL PRO MODEL PROPR]/
	}
}
