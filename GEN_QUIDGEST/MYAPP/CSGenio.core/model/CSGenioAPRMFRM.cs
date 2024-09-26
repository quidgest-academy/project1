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
	public class CSGenioAprmfrm : DbArea
	{
	    /// <summary>
		/// Meta-informação sobre esta àrea
		/// </summary>
		protected static AreaInfo informacao = InicializaAreaInfo();

		public CSGenioAprmfrm(User user,string module)
		{
			fields = new Hashtable();
            this.user = user;
            this.module = module;
		}
	
		public CSGenioAprmfrm(User user) : this(user, user.CurrentModule)
		{
		}
	
		private static AreaInfo InicializaAreaInfo()
		{
			AreaInfo info = new AreaInfo();
			
			/*Information das areas*/
			info.TableName = "PROprmfrm";
			info.ShadowTabName = "";
			info.PrimaryKeyName = "codprmfrm";
            info.HumanKeyName = "codprmfrm";
			info.ShadowTabKeyName = "";
			info.Alias = "prmfrm";
			info.IsDomain =  true;
			info.AreaDesignation = "Permissão por form";
			info.AreaPluralDesignation = "Permissões por form";
			info.DescriptionCav = "Permissão por form";
			
			//sincronização
			info.SyncIncrementalDateStart = TimeSpan.FromHours(9.0);
			info.SyncIncrementalDateEnd = TimeSpan.FromHours(23.0);
			info.SyncCompleteHour = TimeSpan.FromHours(1.0);
			info.SyncIncrementalPeriod = TimeSpan.FromHours(1);
			info.BatchSync = 100;
			info.SyncType = SyncType.Central;
					
			info.RegisterFieldDB(new Field("codprmfrm", FieldType.CHAVE_PRIMARIA));
			info.KeyType = CodeType.INT_KEY;
			info.RegisterFieldDB(new Field("autoriza", FieldType.TEXTO));
			info.RegisterFieldDB(new Field("comprova", FieldType.TEXTO));
			info.RegisterFieldDB(new Field("mensag1", FieldType.TEXTO));
			info.RegisterFieldDB(new Field("mensag2", FieldType.TEXTO));
			info.RegisterFieldDB(new Field("mensag3", FieldType.TEXTO));
			info.RegisterFieldDB(new Field("mensag4", FieldType.TEXTO));
			info.RegisterFieldDB(new Field("prazodia", FieldType.NUMERO));
			info.RegisterFieldDB(new Field("prazohor", FieldType.NUMERO));
			info.RegisterFieldDB(new Field("prfvalid", FieldType.TEXTO));
			info.RegisterFieldDB(new Field("secompro", FieldType.LOGICO));
			info.RegisterFieldDB(new Field("sevalida", FieldType.LOGICO));

			// Relações Filhas
			//------------------------------

			// Relações Mãe
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
		/// Meta-informação sobre esta àrea
		/// </summary>
		public override AreaInfo Information
		{
			get { return informacao; }
		}
		/// <summary>
		/// Meta-informação sobre esta àrea
		/// </summary>		
		public static AreaInfo GetInformation()
		{
			return informacao;
		}

		// USE /[MANUAL PRO TABAUX PRMFRM]/

		
	}
}
