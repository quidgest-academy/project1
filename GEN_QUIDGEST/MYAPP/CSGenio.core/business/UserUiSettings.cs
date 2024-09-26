using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSGenio.business
{
    public class UserUiSettings
    {
		//FOR: USER_TABLE_CONFIG (VueJS)
        public List<CSGenioAtblcfgsel> userTableConfigSelectedInfoRow { get; private set; }
        public string userTableConfigSelectedPk { get; private set; }
        public CSGenioAtblcfg userTableConfigSelectedRow { get; private set; }
        public string userTableConfigSelected { get; private set; }
        public string userTableConfigSelectedName { get; private set; }
        public List<CSGenioAtblcfg> userTableConfigs { get; private set; }
        public List<string> userTableConfigNames { get; private set; }
		public CSGenioAtblcfg userTableConfigDefaultRow { get; private set; }
        public string userTableConfigDefaultName { get; private set; }

        public CSGenioAlstusr userSettings { get; private set; }
        public List<CSGenioAlstcol> userColumns { get; private set; }
        public List<CSGenioAlstren> userRenderings { get; private set; }
        public List<CSGenioAusrwid> userWidgets { get; private set; }
		
        public string key { get; private set; }
		
        public static UserUiSettings Load(PersistentSupport sp, string uuid, User user, string UserTableConfigName = "", bool loadBase = false)
        {
			string ckey = "lstUser_" + uuid + ";" + user.Codpsw;
            UserUiSettings res;

            if (loadBase)
                res = new UserUiSettings();
            else
                res = QCache.Instance.User.Get(ckey) as UserUiSettings;

            if (res == null || (res.userTableConfigSelectedName != null && !res.userTableConfigSelectedName.Equals(UserTableConfigName)))
            {
                res = new UserUiSettings();
                res.key = ckey;

                //FOR: USER_TABLE_CONFIG (VueJS)
                //BEGIN: User table configuration

                //BEGIN: Get selected configuration
                //Get row of selected configuration info
                res.userTableConfigSelectedInfoRow = CSGenioAtblcfgsel.searchList(sp, user, CriteriaSet.And()
					.Equal(CSGenioAtblcfgsel.FldCodpsw, user.Codpsw)
					.Equal(CSGenioAtblcfgsel.FldUuid, uuid)
					.Equal(CSGenioAtblcfgsel.FldZzstate, 0))
					.ToList();
					
				//Default configuration
                if (res.userTableConfigSelectedInfoRow != null && res.userTableConfigSelectedInfoRow.Count > 0)
                {
                    res.userTableConfigDefaultRow = CSGenioAtblcfg.searchList(sp, user, CriteriaSet.And()
                        .Equal(CSGenioAtblcfg.FldCodtblcfg, res.userTableConfigSelectedInfoRow[0].ValCodtblcfg)
                        .Equal(CSGenioAtblcfg.FldZzstate, 0))
                        .FirstOrDefault();

                    res.userTableConfigDefaultName = res.userTableConfigDefaultRow?.ValName;
                }

                //User selected configuration
                if (!string.IsNullOrEmpty(UserTableConfigName))
                {
                    //Get selected configuration row
                    res.userTableConfigSelectedRow = CSGenioAtblcfg.searchList(sp, user, CriteriaSet.And()
                        .Equal(CSGenioAtblcfg.FldCodpsw, user.Codpsw)
                        .Equal(CSGenioAtblcfg.FldUuid, uuid)
                        .Equal(CSGenioAtblcfg.FldName, UserTableConfigName)
                        .Equal(CSGenioAtblcfg.FldZzstate, 0))
                        .FirstOrDefault();
                    //Get selected configuration data
                    res.userTableConfigSelected = res.userTableConfigSelectedRow?.ValConfig;
                    res.userTableConfigSelectedName = res.userTableConfigSelectedRow?.ValName;

                    //Get PK of selected configuration
                    res.userTableConfigSelectedPk = res.userTableConfigSelectedRow?.ValCodtblcfg;
                }
                //Default configuration
                else if (res.userTableConfigDefaultRow != null)
				{
					//Get selected configuration row
					res.userTableConfigSelectedRow = res.userTableConfigDefaultRow;
					
                    //Get selected configuration data
                    res.userTableConfigSelected = res.userTableConfigSelectedRow.ValConfig;
                    res.userTableConfigSelectedName = res.userTableConfigSelectedRow.ValName;
					
					//Get PK of selected configuration
                    res.userTableConfigSelectedPk = res.userTableConfigSelectedRow.ValCodtblcfg;

                }
				//END: Get selected configuration
				
                //END: User table configuration

                //BEGIN: User Settings
                res.userSettings = CSGenioAlstusr.searchList(sp, user, CriteriaSet.And()
                    .Equal(CSGenioAlstusr.FldCodpsw, user.Codpsw)
                    .Equal(CSGenioAlstusr.FldDescric, uuid)
                    .Equal(CSGenioAlstusr.FldZzstate, 0))
                    .FirstOrDefault();
					
                if(res.userSettings != null)
                {
                    res.userColumns = CSGenioAlstcol.searchList(sp, user, CriteriaSet.And()
                        .Equal(CSGenioAlstcol.FldCodlstusr, res.userSettings.ValCodlstusr)
                        .Equal(CSGenioAlstcol.FldZzstate, 0))
                        .ToList();
                    //do the sort in the client side, don't bother the database with that
                    res.userColumns.Sort((x, y) => x.ValPosicao.CompareTo(y.ValPosicao));

                    res.userRenderings = CSGenioAlstren.searchList(sp, user, CriteriaSet.And()
                        .Equal(CSGenioAlstren.FldCodlstusr, res.userSettings.ValCodlstusr)
                        .Equal(CSGenioAlstren.FldZzstate, 0))
                        .ToList();
                    //do the sort in the client side, don't bother the database with that
                    res.userRenderings.Sort((x, y) => x.ValPosicao.CompareTo(y.ValPosicao));

                    res.userWidgets = CSGenioAusrwid.searchList(sp, user, CriteriaSet.And()
                        .Equal(CSGenioAusrwid.FldCodlstusr, res.userSettings.ValCodlstusr)
                        .Equal(CSGenioAusrwid.FldZzstate, 0))
                        .ToList();
                }
                //END: User Settings

                if (string.IsNullOrEmpty(UserTableConfigName))
                    QCache.Instance.User.Put(ckey, res, TimeSpan.FromHours(1));
            }

            //BEGIN: User table configuration
            //Get all user table configurations
            res.userTableConfigs = CSGenioAtblcfg.searchList(sp, user, CriteriaSet.And()
                .Equal(CSGenioAtblcfg.FldCodpsw, user.Codpsw)
                .Equal(CSGenioAtblcfg.FldUuid, uuid)
                .Equal(CSGenioAtblcfg.FldZzstate, 0))
                .ToList();

            res.userTableConfigNames = new List<string>();
            foreach (var row in res.userTableConfigs)
            {
                res.userTableConfigNames.Add(row.ValName);
            }
            //END: User table configuration

            return res;
        }

        public static void Invalidate(string uuid, User user)
        {
            string ckey = "lstUser_" + uuid + ";" + user.Codpsw;

            QCache.Instance.User.Invalidate(ckey);
        }
    }
}
