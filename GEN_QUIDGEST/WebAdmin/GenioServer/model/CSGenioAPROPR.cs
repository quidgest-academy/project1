

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;
using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence;
using Quidgest.Persistence.GenericQuery;
using System.Linq;

namespace CSGenio.business
{
	/// <summary>
	/// Propriedade
	/// </summary>
	public class CSGenioApropr : DbArea	{
		/// <summary>
		/// Meta-information on this area
		/// </summary>
		protected readonly static AreaInfo informacao = InicializaAreaInfo();

		public CSGenioApropr(User user, string module)
		{
			fields = new Hashtable();
            this.user = user;
            this.module = module;
			this.KeyType = CodeType.INT_KEY;
			// USE /[MANUAL PRO CONSTRUTOR PROPR]/
		}

		public CSGenioApropr(User user) : this(user, user.CurrentModule)
		{
		}

		/// <summary>
		/// Initializes the metadata relative to the fields of this area
		/// </summary>
		private static void InicializaCampos(AreaInfo info)
		{
			Field Qfield = null;
#pragma warning disable CS0168, S1481 // Variable is declared but never used
			List<ByAreaArguments> argumentsListByArea;
#pragma warning restore CS0168, S1481 // Variable is declared but never used
			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field("codpropr", FieldType.CHAVE_PRIMARIA);
			Qfield.FieldDescription = "";
			Qfield.FieldSize =  8;
			Qfield.Alias = info.Alias;
			Qfield.MQueue = false;
			Qfield.CavDesignation = "";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field("foto", FieldType.IMAGEM_JPEG);
			Qfield.FieldDescription = "Fotografia";
			Qfield.FieldSize =  3;
			Qfield.Alias = info.Alias;
			Qfield.MQueue = false;
			Qfield.CavDesignation = "FOTOGRAFIA36807";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field("titulo", FieldType.TEXTO);
			Qfield.FieldDescription = "Título";
			Qfield.FieldSize =  80;
			Qfield.Alias = info.Alias;
			Qfield.MQueue = false;
			Qfield.CavDesignation = "TITULO39021";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field("preco", FieldType.VALOR);
			Qfield.FieldDescription = "Preço";
			Qfield.FieldSize =  10;
			Qfield.Alias = info.Alias;
			Qfield.MQueue = false;
			Qfield.Decimals = 4;
			Qfield.CavDesignation = "PRECO50007";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field("codagent", FieldType.CHAVE_ESTRANGEIRA);
			Qfield.FieldDescription = "";
			Qfield.FieldSize =  8;
			Qfield.Alias = info.Alias;
			Qfield.MQueue = false;
			Qfield.CavDesignation = "";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field("tamanho", FieldType.NUMERO);
			Qfield.FieldDescription = "Tamanho (m2)";
			Qfield.FieldSize =  6;
			Qfield.Alias = info.Alias;
			Qfield.MQueue = false;
			Qfield.Decimals = 2;
			Qfield.CavDesignation = "TAMANHO__M2_40951";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field("nr_wcs", FieldType.NUMERO);
			Qfield.FieldDescription = "Número de casas de banho";
			Qfield.FieldSize =  3;
			Qfield.Alias = info.Alias;
			Qfield.MQueue = false;
			Qfield.CavDesignation = "NUMERO_DE_CASAS_DE_B39783";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field("dtconst", FieldType.DATA);
			Qfield.FieldDescription = "Data de contrução";
			Qfield.FieldSize =  8;
			Qfield.Alias = info.Alias;
			Qfield.MQueue = false;
			Qfield.CavDesignation = "DATA_DE_CONTRUCAO03489";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field("descrica", FieldType.MEMO);
			Qfield.FieldDescription = "Descrição";
			Qfield.FieldSize =  80;
			Qfield.Alias = info.Alias;
			Qfield.MQueue = false;
			Qfield.Decimals = 10;
			Qfield.CavDesignation = "DESCRICAO07528";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field("codcidad", FieldType.CHAVE_ESTRANGEIRA);
			Qfield.FieldDescription = "";
			Qfield.FieldSize =  8;
			Qfield.Alias = info.Alias;
			Qfield.MQueue = false;
			Qfield.CavDesignation = "";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field("tipoprop", FieldType.ARRAY_COD_TEXTO);
			Qfield.FieldDescription = "Tipo de construção";
			Qfield.FieldSize =  1;
			Qfield.Alias = info.Alias;
			Qfield.MQueue = false;
			Qfield.CavDesignation = "TIPO_DE_CONSTRUCAO35217";

			Qfield.Dupmsg = "";
            Qfield.ArrayName = "dbo.GetValArrayCtipocons";
            Qfield.ArrayClassName = "Tipocons";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field("tipologi", FieldType.ARRAY_COD_NUMERICO);
			Qfield.FieldDescription = "Tipologia";
			Qfield.FieldSize =  1;
			Qfield.Alias = info.Alias;
			Qfield.MQueue = false;
			Qfield.CavDesignation = "TIPOLOGIA13928";

			Qfield.Dupmsg = "";
			Qfield.ArrayName = "dbo.GetValArrayNtipologi";
            Qfield.ArrayClassName = "Tipologi";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field("idpropre", FieldType.NUMERO);
			Qfield.FieldDescription = "ID";
			Qfield.FieldSize =  10;
			Qfield.Alias = info.Alias;
			Qfield.MQueue = false;
			Qfield.CavDesignation = "ID48520";

			Qfield.Dupmsg = "";
			Qfield.DefaultValue = new DefaultValue(DefaultValue.getGreaterPlus1_int, "idpropre");
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field("idadepro", FieldType.NUMERO);
			Qfield.FieldDescription = "Idade da construção";
			Qfield.FieldSize =  4;
			Qfield.Alias = info.Alias;
			Qfield.MQueue = false;
			Qfield.CavDesignation = "IDADE_DA_CONSTRUCAO37805";

			Qfield.Dupmsg = "";
			argumentsListByArea = new List<ByAreaArguments>();
			argumentsListByArea.Add(new ByAreaArguments(new string[] {"dtconst"}, new int[] {0}, "propr", "codpropr"));
			Qfield.Formula = new InternalOperationFormula(argumentsListByArea, 1, delegate(object[] args, User user, string module, PersistentSupport sp) {
				return GlobalFunctions.Year(DateTime.Today)-GlobalFunctions.Year(((DateTime)args[0]));
			});
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field("espexter", FieldType.NUMERO);
			Qfield.FieldDescription = "Espaço exterior (m2)";
			Qfield.FieldSize =  7;
			Qfield.Alias = info.Alias;
			Qfield.MQueue = false;
			Qfield.Decimals = 2;
			Qfield.CavDesignation = "TAMANHO__M2_40951";

			Qfield.Dupmsg = "";
			argumentsListByArea = new List<ByAreaArguments>();
			argumentsListByArea.Add(new ByAreaArguments(new string[] {"tipoprop"}, new int[] {0}, "propr", "codpropr"));
			Qfield.ShowWhen = new ConditionFormula(argumentsListByArea, 1, delegate(object[] args, User user, string module, PersistentSupport sp) {
				return ((string)args[0])=="m";
			});
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field("vendida", FieldType.LOGICO);
			Qfield.FieldDescription = "Vendida";
			Qfield.FieldSize =  1;
			Qfield.Alias = info.Alias;
			Qfield.MQueue = false;
			Qfield.CavDesignation = "VENDIDA08366";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field("lucro", FieldType.VALOR);
			Qfield.FieldDescription = "Lucro";
			Qfield.FieldSize =  10;
			Qfield.Alias = info.Alias;
			Qfield.MQueue = false;
			Qfield.Decimals = 4;
			Qfield.CavDesignation = "LUCRO53291";

			Qfield.Dupmsg = "";
			argumentsListByArea = new List<ByAreaArguments>();
			argumentsListByArea.Add(new ByAreaArguments(new string[] {"vendida","preco"}, new int[] {0,1}, "propr", "codpropr"));
			argumentsListByArea.Add(new ByAreaArguments(new string[] {"perelucr"}, new int[] {2}, "agent", "codagent"));
			Qfield.Formula = new InternalOperationFormula(argumentsListByArea, 3, delegate(object[] args, User user, string module, PersistentSupport sp) {
				return ((((int)args[0])==1)?(((decimal)args[1])*((decimal)args[2])):(0));
			});
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field("localiza", FieldType.GEOGRAPHY);
			Qfield.FieldDescription = "Localização";
			Qfield.FieldSize =  50;
			Qfield.Alias = info.Alias;
			Qfield.MQueue = false;
			Qfield.CavDesignation = "LOCALIZACAO54665";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field("zzstate", FieldType.INTEIRO);
			Qfield.FieldDescription = "Estado da ficha";
			Qfield.Alias = info.Alias;
			info.RegisterFieldDB(Qfield);

		}

		/// <summary>
		/// Initializes metadata for paths direct to other areas
		/// </summary>
		private static void InicializaRelacoes(AreaInfo info)
		{
			// Daughters Relations
			//------------------------------
			info.ChildTable = new ChildRelation[2];
			info.ChildTable[0]= new ChildRelation("contc", new String[] {"codpropr"}, DeleteProc.NA);
			info.ChildTable[1]= new ChildRelation("album", new String[] {"codpropr"}, DeleteProc.NA);

			// Mother Relations
			//------------------------------
			info.ParentTables = new Dictionary<string, Relation>();
			info.ParentTables.Add("agent", new Relation("PRO", "propropr", "propr", "codpropr", "codagent", "PRO", "proagente_imobiliario", "agent", "codagent", "codagent"));
			info.ParentTables.Add("cidad", new Relation("PRO", "propropr", "propr", "codpropr", "codcidad", "PRO", "procidad", "cidad", "codcidad", "codcidad"));
		}

		/// <summary>
		/// Initializes metadata for indirect paths to other areas
		/// </summary>
		private static void InicializaCaminhos(AreaInfo info)
		{
			// Pathways
			//------------------------------
			info.Pathways = new Dictionary<string, string>(5);
			info.Pathways.Add("cidad","cidad");
			info.Pathways.Add("agent","agent");
			info.Pathways.Add("pais","cidad");
			info.Pathways.Add("pmora","agent");
			info.Pathways.Add("pnasc","agent");
		}

		/// <summary>
		/// Initializes metadata for triggers and formula arguments
		/// </summary>
		private static void InicializaFormulas(AreaInfo info)
		{
			// Formulas
			//------------------------------
			//Actualiza as seguintes somas relacionadas:
			info.RelatedSumArgs = new List<RelatedSumArgument>();
			info.RelatedSumArgs.Add( new RelatedSumArgument("propr", "agent", "lucro", "lucro", '+', true));



			info.InternalOperationFields = new string[] {
			 "idadepro","lucro"
			};

			info.SequentialDefaultValues = new string[] {
			 "idpropre"
			};





			//Write conditions
			List<ConditionFormula> conditions = new List<ConditionFormula>();

			// [PROPR->PRECO]>0
			{
			List<ByAreaArguments> argumentsListByArea = new List<ByAreaArguments>();
			argumentsListByArea= new List<ByAreaArguments>();
			argumentsListByArea.Add(new ByAreaArguments(new string[] {"preco"},new int[] {0},"propr","codpropr"));
			ConditionFormula writeCondition = new ConditionFormula(argumentsListByArea, 1, delegate(object []args,User user,string module,PersistentSupport sp) {
				return ((decimal)args[0])>0;
			});
			writeCondition.ErrorWarning = "O preço deverá ser maior que zero!";
            writeCondition.Type =  ConditionType.ERROR;
            writeCondition.Validate = true;
			writeCondition.Field = info.DBFields["preco"];
			conditions.Add(writeCondition);
			}
			info.WriteConditions = conditions.Where(c=> c.IsWriteCondition()).ToList();
			info.CrudConditions = conditions.Where(c=> c.IsCrudCondition()).ToList();

		}

		/// <summary>
		/// static CSGenioApropr()
		/// </summary>
		private static AreaInfo InicializaAreaInfo()
		{
			AreaInfo info = new AreaInfo();

			// Area meta-information
			info.QSystem="PRO";
			info.TableName="propropr";
			info.ShadowTabName="";
			info.ShadowTabKeyName="";

			info.PrimaryKeyName="codpropr";
			info.HumanKeyName="titulo,".TrimEnd(',');
			info.Alias="propr";
			info.IsDomain = true;
			info.PersistenceType = PersistenceType.Database;
			info.AreaDesignation="Propriedade";
			info.AreaPluralDesignation="Propriedades";
			info.DescriptionCav="PROPRIEDADE00464";

			info.KeyType = CodeType.INT_KEY;

			//sincronização
			info.SyncIncrementalDateStart = TimeSpan.FromHours(8);
			info.SyncIncrementalDateEnd = TimeSpan.FromHours(23);
			info.SyncCompleteHour = TimeSpan.FromHours(0.5);
			info.SyncIncrementalPeriod = TimeSpan.FromHours(1);
			info.BatchSync = 100;
			info.SyncType = SyncType.Central;
            info.SolrList = new List<string>();
        	info.QueuesList = new List<GenioServer.business.QueueGenio>();





			//RS 22.03.2011 I separated in submetodos due to performance problems with the JIT in 64bits
			// that in very large projects took 2 minutes on the first call.
			// After a Microsoft analysis of the JIT algortimo it was revealed that it has a
			// complexity O(n*m) where n are the lines of code and m the number of variables of a function.
			// Tests have revealed that splitting into subfunctions cuts the JIT time by more than half by 64-bit.
			//------------------------------
			InicializaCampos(info);

			//------------------------------
			InicializaRelacoes(info);

			//------------------------------
			InicializaCaminhos(info);

			//------------------------------
			InicializaFormulas(info);

			// Automatic audit stamps in BD
            //------------------------------

            // Documents in DB
            //------------------------------

            // Historics
            //------------------------------

			// Duplication
			//------------------------------

			// Ephs
			//------------------------------
			info.Ephs=new Hashtable();

			// Table minimum roles and access levels
			//------------------------------
            info.QLevel = new QLevel();
            info.QLevel.Query = Role.AUTHORIZED;
            info.QLevel.Create = Role.AUTHORIZED;
            info.QLevel.AlterAlways = Role.AUTHORIZED;
            info.QLevel.RemoveAlways = Role.AUTHORIZED;

      		return info;
		}

		/// <summary>
		/// Meta-information about this area
		/// </summary>
		public override AreaInfo Information
		{
			get { return informacao; }
		}
		/// <summary>
		/// Meta-information about this area
		/// </summary>
		public static AreaInfo GetInformation()
		{
			return informacao;
		}

		/// <summary>Field : "" Tipo: "+" Formula:  ""</summary>
		public static FieldRef FldCodpropr { get { return m_fldCodpropr; } }
		private static FieldRef m_fldCodpropr = new FieldRef("propr", "codpropr");

		/// <summary>Field : "" Tipo: "+" Formula:  ""</summary>
		public string ValCodpropr
		{
			get { return (string)returnValueField(FldCodpropr); }
			set { insertNameValueField(FldCodpropr, value); }
		}


		/// <summary>Field : "Fotografia" Tipo: "IJ" Formula:  ""</summary>
		public static FieldRef FldFoto { get { return m_fldFoto; } }
		private static FieldRef m_fldFoto = new FieldRef("propr", "foto");

		/// <summary>Field : "Fotografia" Tipo: "IJ" Formula:  ""</summary>
		public byte[] ValFoto
		{
			get { return (byte[])returnValueField(FldFoto); }
			set { insertNameValueField(FldFoto, value); }
		}


		/// <summary>Field : "Título" Tipo: "C" Formula:  ""</summary>
		public static FieldRef FldTitulo { get { return m_fldTitulo; } }
		private static FieldRef m_fldTitulo = new FieldRef("propr", "titulo");

		/// <summary>Field : "Título" Tipo: "C" Formula:  ""</summary>
		public string ValTitulo
		{
			get { return (string)returnValueField(FldTitulo); }
			set { insertNameValueField(FldTitulo, value); }
		}


		/// <summary>Field : "Preço" Tipo: "$" Formula:  ""</summary>
		public static FieldRef FldPreco { get { return m_fldPreco; } }
		private static FieldRef m_fldPreco = new FieldRef("propr", "preco");

		/// <summary>Field : "Preço" Tipo: "$" Formula:  ""</summary>
		public decimal ValPreco
		{
			get { return (decimal)returnValueField(FldPreco); }
			set { insertNameValueField(FldPreco, value); }
		}


		/// <summary>Field : "" Tipo: "CE" Formula:  ""</summary>
		public static FieldRef FldCodagent { get { return m_fldCodagent; } }
		private static FieldRef m_fldCodagent = new FieldRef("propr", "codagent");

		/// <summary>Field : "" Tipo: "CE" Formula:  ""</summary>
		public string ValCodagent
		{
			get { return (string)returnValueField(FldCodagent); }
			set { insertNameValueField(FldCodagent, value); }
		}


		/// <summary>Field : "Tamanho (m2)" Tipo: "N" Formula:  ""</summary>
		public static FieldRef FldTamanho { get { return m_fldTamanho; } }
		private static FieldRef m_fldTamanho = new FieldRef("propr", "tamanho");

		/// <summary>Field : "Tamanho (m2)" Tipo: "N" Formula:  ""</summary>
		public decimal ValTamanho
		{
			get { return (decimal)returnValueField(FldTamanho); }
			set { insertNameValueField(FldTamanho, value); }
		}


		/// <summary>Field : "Número de casas de banho" Tipo: "N" Formula:  ""</summary>
		public static FieldRef FldNr_wcs { get { return m_fldNr_wcs; } }
		private static FieldRef m_fldNr_wcs = new FieldRef("propr", "nr_wcs");

		/// <summary>Field : "Número de casas de banho" Tipo: "N" Formula:  ""</summary>
		public decimal ValNr_wcs
		{
			get { return (decimal)returnValueField(FldNr_wcs); }
			set { insertNameValueField(FldNr_wcs, value); }
		}


		/// <summary>Field : "Data de contrução" Tipo: "D" Formula:  ""</summary>
		public static FieldRef FldDtconst { get { return m_fldDtconst; } }
		private static FieldRef m_fldDtconst = new FieldRef("propr", "dtconst");

		/// <summary>Field : "Data de contrução" Tipo: "D" Formula:  ""</summary>
		public DateTime ValDtconst
		{
			get { return (DateTime)returnValueField(FldDtconst); }
			set { insertNameValueField(FldDtconst, value); }
		}


		/// <summary>Field : "Descrição" Tipo: "MO" Formula:  ""</summary>
		public static FieldRef FldDescrica { get { return m_fldDescrica; } }
		private static FieldRef m_fldDescrica = new FieldRef("propr", "descrica");

		/// <summary>Field : "Descrição" Tipo: "MO" Formula:  ""</summary>
		public string ValDescrica
		{
			get { return (string)returnValueField(FldDescrica); }
			set { insertNameValueField(FldDescrica, value); }
		}


		/// <summary>Field : "" Tipo: "CE" Formula:  ""</summary>
		public static FieldRef FldCodcidad { get { return m_fldCodcidad; } }
		private static FieldRef m_fldCodcidad = new FieldRef("propr", "codcidad");

		/// <summary>Field : "" Tipo: "CE" Formula:  ""</summary>
		public string ValCodcidad
		{
			get { return (string)returnValueField(FldCodcidad); }
			set { insertNameValueField(FldCodcidad, value); }
		}


		/// <summary>Field : "Tipo de construção" Tipo: "AC" Formula:  ""</summary>
		public static FieldRef FldTipoprop { get { return m_fldTipoprop; } }
		private static FieldRef m_fldTipoprop = new FieldRef("propr", "tipoprop");

		/// <summary>Field : "Tipo de construção" Tipo: "AC" Formula:  ""</summary>
		public string ValTipoprop
		{
			get { return (string)returnValueField(FldTipoprop); }
			set { insertNameValueField(FldTipoprop, value); }
		}


		/// <summary>Field : "Tipologia" Tipo: "AN" Formula:  ""</summary>
		public static FieldRef FldTipologi { get { return m_fldTipologi; } }
		private static FieldRef m_fldTipologi = new FieldRef("propr", "tipologi");

		/// <summary>Field : "Tipologia" Tipo: "AN" Formula:  ""</summary>
		public decimal ValTipologi
		{
			get { return (decimal)returnValueField(FldTipologi); }
			set { insertNameValueField(FldTipologi, value); }
		}


		/// <summary>Field : "ID" Tipo: "N" Formula:  ""</summary>
		public static FieldRef FldIdpropre { get { return m_fldIdpropre; } }
		private static FieldRef m_fldIdpropre = new FieldRef("propr", "idpropre");

		/// <summary>Field : "ID" Tipo: "N" Formula:  ""</summary>
		public decimal ValIdpropre
		{
			get { return (decimal)returnValueField(FldIdpropre); }
			set { insertNameValueField(FldIdpropre, value); }
		}


		/// <summary>Field : "Idade da construção" Tipo: "N" Formula: + "Year([Today]) - Year([PROPR->DTCONST])"</summary>
		public static FieldRef FldIdadepro { get { return m_fldIdadepro; } }
		private static FieldRef m_fldIdadepro = new FieldRef("propr", "idadepro");

		/// <summary>Field : "Idade da construção" Tipo: "N" Formula: + "Year([Today]) - Year([PROPR->DTCONST])"</summary>
		public decimal ValIdadepro
		{
			get { return (decimal)returnValueField(FldIdadepro); }
			set { insertNameValueField(FldIdadepro, value); }
		}


		/// <summary>Field : "Espaço exterior (m2)" Tipo: "N" Formula:  ""</summary>
		public static FieldRef FldEspexter { get { return m_fldEspexter; } }
		private static FieldRef m_fldEspexter = new FieldRef("propr", "espexter");

		/// <summary>Field : "Espaço exterior (m2)" Tipo: "N" Formula:  ""</summary>
		public decimal ValEspexter
		{
			get { return (decimal)returnValueField(FldEspexter); }
			set { insertNameValueField(FldEspexter, value); }
		}


		/// <summary>Field : "Vendida" Tipo: "L" Formula:  ""</summary>
		public static FieldRef FldVendida { get { return m_fldVendida; } }
		private static FieldRef m_fldVendida = new FieldRef("propr", "vendida");

		/// <summary>Field : "Vendida" Tipo: "L" Formula:  ""</summary>
		public int ValVendida
		{
			get { return (int)returnValueField(FldVendida); }
			set { insertNameValueField(FldVendida, value); }
		}


		/// <summary>Field : "Lucro" Tipo: "$" Formula: + "iif([PROPR->VENDIDA]==1,[PROPR->PRECO]*[AGENT->PERELUCR],0)"</summary>
		public static FieldRef FldLucro { get { return m_fldLucro; } }
		private static FieldRef m_fldLucro = new FieldRef("propr", "lucro");

		/// <summary>Field : "Lucro" Tipo: "$" Formula: + "iif([PROPR->VENDIDA]==1,[PROPR->PRECO]*[AGENT->PERELUCR],0)"</summary>
		public decimal ValLucro
		{
			get { return (decimal)returnValueField(FldLucro); }
			set { insertNameValueField(FldLucro, value); }
		}


		/// <summary>Field : "Localização" Tipo: "GG" Formula:  ""</summary>
		public static FieldRef FldLocaliza { get { return m_fldLocaliza; } }
		private static FieldRef m_fldLocaliza = new FieldRef("propr", "localiza");

		/// <summary>Field : "Localização" Tipo: "GG" Formula:  ""</summary>
		public string ValLocaliza
		{
			get { return (string)returnValueField(FldLocaliza); }
			set { insertNameValueField(FldLocaliza, value); }
		}


		/// <summary>Field : "ZZSTATE" Type: "INT" Formula:  ""</summary>
		public static FieldRef FldZzstate { get { return m_fldZzstate; } }
		private static FieldRef m_fldZzstate = new FieldRef("propr", "zzstate");



		/// <summary>Field : "ZZSTATE" Type: "INT"</summary>
		public int ValZzstate
		{
			get { return (int)returnValueField(FldZzstate); }
			set { insertNameValueField(FldZzstate, value); }
		}

        /// <summary>
        /// Obtains a partially populated area with the record corresponding to a primary key
        /// </summary>
        /// <param name="sp">Persistent support from where to get the registration</param>
        /// <param name="key">The value of the primary key</param>
        /// <param name="user">The context of the user</param>
        /// <param name="fields">The fields to be filled in the area</param>
        /// <returns>An area with the fields requests of the record read or null if the key does not exist</returns>
        /// <remarks>Persistence operations should not be used on a partially positioned register</remarks>
        public static CSGenioApropr search(PersistentSupport sp, string key, User user, string[] fields = null)
        {
			if (string.IsNullOrEmpty(key))
				return null;

		    CSGenioApropr area = new CSGenioApropr(user, user.CurrentModule);

            if (sp.getRecord(area, key, fields))
                return area;
			return null;
        }


		public static string GetkeyFromControlledRecord(PersistentSupport sp, string ID, User user)
		{
			if (informacao.ControlledRecords != null)
				return informacao.ControlledRecords.GetPrimaryKeyFromControlledRecord(sp, user, ID);
			return String.Empty;
		}



        /// <summary>
        /// Search for all records of this area that comply with a condition
        /// </summary>
        /// <param name="sp">Persistent support from where to get the list</param>
        /// <param name="user">The context of the user</param>
        /// <param name="where">The search condition for the records. Use null to get all records</param>
        /// <param name="fields">The fields to be filled in the area</param>
        /// <returns>A list of area records with all fields populated</returns>
        /// <remarks>Persistence operations should not be used on a partially positioned register</remarks>
        [Obsolete("Use List<CSGenioApropr> searchList(PersistentSupport sp, User user, CriteriaSet where, string []fields) instead")]
        public static List<CSGenioApropr> searchList(PersistentSupport sp, User user, string where, string []fields = null)
        {
            return sp.searchListWhere<CSGenioApropr>(where, user, fields);
        }


        /// <summary>
        /// Search for all records of this area that comply with a condition
        /// </summary>
        /// <param name="sp">Persistent support from where to get the list</param>
        /// <param name="user">The context of the user</param>
        /// <param name="where">The search condition for the records. Use null to get all records</param>
        /// <param name="fields">The fields to be filled in the area</param>
        /// <param name="distinct">Get distinct from fields</param>
        /// <param name="noLock">NOLOCK</param>
        /// <returns>A list of area records with all fields populated</returns>
        /// <remarks>Persistence operations should not be used on a partially positioned register</remarks>
        public static List<CSGenioApropr> searchList(PersistentSupport sp, User user, CriteriaSet where, string[] fields = null, bool distinct = false, bool noLock = false)
        {
				return sp.searchListWhere<CSGenioApropr>(where, user, fields, distinct, noLock);
        }



       	/// <summary>
        /// Search for all records of this area that comply with a condition
        /// </summary>
        /// <param name="sp">Persistent support from where to get the list</param>
        /// <param name="user">The context of the user</param>
        /// <param name="where">The search condition for the records. Use null to get all records</param>
        /// <param name="listing">List configuration</param>
        /// <returns>A list of area records with all fields populated</returns>
        /// <remarks>Persistence operations should not be used on a partially positioned register</remarks>
        public static void searchListAdvancedWhere(PersistentSupport sp, User user, CriteriaSet where, ListingMVC<CSGenioApropr> listing)
        {
			sp.searchListAdvancedWhere<CSGenioApropr>(where, listing);
        }




		/// <summary>
		/// Check if a record exist
		/// </summary>
		/// <param name="key">Record key</param>
		/// <param name="sp">DB conecntion</param>
		/// <returns>True if the record exist</returns>
		public static bool RecordExist(string key, PersistentSupport sp) => DbArea.RecordExist(key, informacao, sp);







		// USE /[MANUAL PRO TABAUX PROPR]/

 

                   

	}
}
