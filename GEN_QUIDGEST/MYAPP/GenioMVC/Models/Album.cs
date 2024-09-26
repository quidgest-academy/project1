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
	public class Album : ModelBase
	{
		[JsonIgnore]
		public CSGenioAalbum klass { get { return baseklass as CSGenioAalbum; } set { baseklass = value; } }

		[Key]
		/// <summary>Field : "" Tipo: "+" Formula:  ""</summary>
		[ShouldSerialize("Album.ValCodalbum")]
		public string ValCodalbum { get { return klass.ValCodalbum; } set { klass.ValCodalbum = value; } }

		[DisplayName("Foto")]
		/// <summary>Field : "Foto" Tipo: "IJ" Formula:  ""</summary>
		[ShouldSerialize("Album.ValFoto")]
		[ImageThumbnailJsonConverter(75, 75)]
		public ImageModel ValFoto { get { return new ImageModel(klass.ValFoto) { Ticket = ValFotoQTicket }; } set { klass.ValFoto = value; } }
		[JsonIgnore]
		public string ValFotoQTicket = null;

		[DisplayName("Título")]
		/// <summary>Field : "Título" Tipo: "C" Formula:  ""</summary>
		[ShouldSerialize("Album.ValTitulo")]
		public string ValTitulo { get { return klass.ValTitulo; } set { klass.ValTitulo = value; } }

		[DisplayName("")]
		/// <summary>Field : "" Tipo: "CE" Formula:  ""</summary>
		[ShouldSerialize("Album.ValCodpropr")]
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


		[DisplayName("ZZSTATE")]
		[ShouldSerialize("Album.ValZzstate")]
		/// <summary>Field : "ZZSTATE" Type: "INT" Formula:  ""</summary>
		public int ValZzstate { get { return klass.ValZzstate; } set { klass.ValZzstate = value; } }

		public Album(UserContext userContext, bool isEmpty = false, string[]? fieldsToSerialize = null) : base(userContext)
		{
			klass = new CSGenioAalbum(userContext.User);
			isEmptyModel = isEmpty;
			if (fieldsToSerialize != null)
				SetFieldsToSerialize(fieldsToSerialize);
		}

		public Album(UserContext userContext, CSGenioAalbum val, bool isEmpty = false, string[]? fieldsToSerialize = null) : base(userContext)
		{
			klass = val;
			isEmptyModel = isEmpty;
			if (fieldsToSerialize != null)
				SetFieldsToSerialize(fieldsToSerialize);
			FillRelatedAreas(val);
		}


		public void FillRelatedAreas(CSGenioAalbum csgenioa)
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
		public static Album Find(string id, UserContext userCtx, string identifier = null, string[] fieldsToSerialize = null, string[] fieldsToQuery = null)
		{
			var record = Find<CSGenioAalbum>(id, userCtx, identifier, fieldsToQuery);
			return record == null ? null : new Album(userCtx, record, false, fieldsToSerialize) { Identifier = identifier };
		}

		public static List<Album> AllModel(UserContext userCtx, CriteriaSet args = null, string identifier = null)
		{
			return Where<CSGenioAalbum>(userCtx, false, args, numRegs: -1, identifier: identifier).RowsForViewModel<Album>((r) => new Album(userCtx, r));
		}

// USE /[MANUAL PRO MODEL ALBUM]/
	}
}
