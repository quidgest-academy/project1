using System;
using CSGenio.framework;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using CSGenio.persistence;
using System.Text;
using Quidgest.Persistence;
using Quidgest.Persistence.GenericQuery;

namespace CSGenio.business
{
	/// <summary>
	/// Summary description for CSArea.
	/// </summary>
	public class CSGenioAdelega : DbArea
	{
	    /// <summary>
		/// Meta-informa��o sobre esta �rea
		/// </summary>
		protected static AreaInfo informacao = InicializaAreaInfo();

		public CSGenioAdelega(User user,string module)
		{
			fields = new Hashtable();
            this.user = user;
            this.module = module;
		}
	
		public CSGenioAdelega(User user) : this(user, user.CurrentModule)
		{
		}
	
		private static AreaInfo InicializaAreaInfo()
		{
			AreaInfo info = new AreaInfo();
			
			/*Information das areas*/
			info.TableName = "PROdelega";
			info.ShadowTabName = "";
			info.PrimaryKeyName = "coddelega";
            info.HumanKeyName = "coddelega";
			info.ShadowTabKeyName = "";
			info.Alias = "delega";
			info.IsDomain =  true;
			info.AreaDesignation = "Delega��o";
			info.AreaPluralDesignation = "Delega��es";
			info.DescriptionCav = "Delega��o";
			
			//sincroniza��o
			info.SyncIncrementalDateStart = TimeSpan.FromHours(9.0);
			info.SyncIncrementalDateEnd = TimeSpan.FromHours(23.0);
			info.SyncCompleteHour = TimeSpan.FromHours(1.0);
			info.SyncIncrementalPeriod = TimeSpan.FromHours(1);
			info.BatchSync = 100;
			info.SyncType = SyncType.Central;
					
      info.RegisterFieldDB(new Field("coddelega", FieldType.CHAVE_PRIMARIA));
	  info.DBFields["coddelega"].Alias = info.Alias;
      info.RegisterFieldDB(new Field("codpswup", FieldType.CHAVE_ESTRANGEIRA));
      info.RegisterFieldDB(new Field("codpswdw", FieldType.CHAVE_ESTRANGEIRA));
	  info.KeyType = CodeType.INT_KEY;
	  info.DBFields["codpswup"].Alias = info.Alias;
	  info.DBFields["codpswdw"].Alias = info.Alias;
      info.RegisterFieldDB(new Field("dateini", FieldType.DATA));
	  info.DBFields["dateini"].Alias = info.Alias;
      info.RegisterFieldDB(new Field("dateend", FieldType.DATA));
	  info.DBFields["dateend"].Alias = info.Alias;
      info.RegisterFieldDB(new Field("message", FieldType.TEXTO));
	  info.DBFields["message"].Alias = info.Alias;
      info.RegisterFieldDB(new Field("revoked", FieldType.LOGICO));
	  info.DBFields["revoked"].Alias = info.Alias;
      info.RegisterFieldDB(new Field("auditusr", FieldType.TEXTO));
	  info.DBFields["auditusr"].Alias = info.Alias;
      info.RegisterFieldDB(new Field("opercria", FieldType.TEXTO));
	  info.DBFields["opercria"].Alias = info.Alias;
      info.RegisterFieldDB(new Field("datacria", FieldType.DATA));
	  info.DBFields["datacria"].Alias = info.Alias;
      info.RegisterFieldDB(new Field("opermuda", FieldType.TEXTO));
	  info.DBFields["opermuda"].Alias = info.Alias;
      info.RegisterFieldDB(new Field("datamuda", FieldType.DATA));
	  info.DBFields["datamuda"].Alias = info.Alias;
      info.RegisterFieldDB(new Field("zzstate", FieldType.INTEIRO));
	  info.DBFields["zzstate"].Alias = info.Alias;

			// Rela��es Filhas
			//------------------------------

			// Rela��es M�e
			//------------------------------

			// Pathways
			//------------------------------

			// Levels de acesso
			//------------------------------
			info.QLevel = new QLevel();
			info.QLevel.Query = Role.UNAUTHORIZED;
			info.QLevel.Create = Role.UNAUTHORIZED;
			info.QLevel.AlterAlways = Role.UNAUTHORIZED;
			info.QLevel.RemoveAlways = Role.UNAUTHORIZED;

			// Automatic audit stamps in BD
            //------------------------------


			return info;
		}
		
		/// <summary>
		/// Meta-informa��o sobre esta �rea
		/// </summary>
		public override AreaInfo Information
		{
			get { return informacao; }
		}
		/// <summary>
		/// Meta-informa��o sobre esta �rea
		/// </summary>		
		public static AreaInfo GetInformation()
		{
			return informacao;
		}

		// USE /[MANUAL PRO TABAUX DELEGA]/

		
    public static FieldRef FldCoddelega { get { return m_fldCoddelega; } }
    private static FieldRef m_fldCoddelega = new FieldRef("delega", "coddelega");
    
    public string ValCoddelega
    {
        get { return (string)returnValueField(FldCoddelega); }
        set { insertNameValueField(FldCoddelega, value); }
    }
    
    public static FieldRef FldCodpswup { get { return m_fldCodpswup; } }
    private static FieldRef m_fldCodpswup = new FieldRef("delega", "codpswup");
    
    public string ValCodpswup
    {
        get { return (string)returnValueField(FldCodpswup); }
        set { insertNameValueField(FldCodpswup, value); }
    }
    
    public static FieldRef FldCodpswdw { get { return m_fldCodpswdw; } }
    private static FieldRef m_fldCodpswdw = new FieldRef("delega", "codpswdw");
    
    public string ValCodpswdw
    {
        get { return (string)returnValueField(FldCodpswdw); }
        set { insertNameValueField(FldCodpswdw, value); }
    }

    public static FieldRef FldAuditusr { get { return m_fldAuditusr; } }
    private static FieldRef m_fldAuditusr = new FieldRef("delega", "auditusr");

    public string ValAuditusr
    {
        get { return (string)returnValueField(FldAuditusr); }
        set { insertNameValueField(FldAuditusr, value); }
    }

    public static FieldRef FldDateini { get { return m_fldDateini; } }
    private static FieldRef m_fldDateini = new FieldRef("delega", "dateini");

    public DateTime ValDateini
    {
        get { return (DateTime)returnValueField(FldDateini); }
        set { insertNameValueField(FldDateini, value); }
    }

    public static FieldRef FldDateend { get { return m_fldDateend; } }
    private static FieldRef m_fldDateend = new FieldRef("delega", "dateend");

    public DateTime ValDateend
    {
        get { return (DateTime)returnValueField(FldDateend); }
        set { insertNameValueField(FldDateend, value); }
    }

    public static FieldRef FldRevoked { get { return m_fldRevoked; } }
    private static FieldRef m_fldRevoked = new FieldRef("delega", "revoked");

    public int ValRevoked
    {
        get { return (int)returnValueField(FldRevoked); }
        set { insertNameValueField(FldRevoked, value); }
    }

    public override Area insertPseud(PersistentSupport sp, string[] fieldNames, string[] fieldsvalues)
    {
        this.ValCodpswup = User.Codpsw;
        return base.insertPseud(sp, fieldNames, fieldsvalues);
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
    public static CSGenioAdelega search(PersistentSupport sp, string key, User user, string[] fields = null)
    {
        CSGenioAdelega area = new CSGenioAdelega(user, user.CurrentModule);
        if (sp.getRecord(area, key, fields))
            return area;
        return null;
    }

	}
}
