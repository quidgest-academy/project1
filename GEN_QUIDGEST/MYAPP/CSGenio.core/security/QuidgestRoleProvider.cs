using System;
using System.Collections.Generic;
using System.Security.Principal;
using CSGenio.framework;
using CSGenio.business;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;
using System.ComponentModel;

namespace GenioServer.security
{
    [Description("Retrieves the roles of a given user from the application database.")]
    [DisplayName("Application Database roles")]
    class QuidgestRoleProvider : IRoleProvider
    {
        public CSGenioApsw GetUser(string userName, User user, PersistentSupport sp)
        {
            var list = CSGenioApsw.searchList(sp, user, CriteriaSet.And().Equal(CSGenioApsw.FldNome, userName));
            if (list.Count == 0)
                return null;
            else
                return list[0];
        }

        public CSGenioApsw GetUserFromEmail(string email, User user, PersistentSupport sp)
        {
            var pswList = CSGenioApsw.searchList(sp, user, CriteriaSet.And().Equal(CSGenioApsw.FldEmail, email));
            if (pswList.Count == 0)
                return null;
            else if (pswList.Count >= 2)
            {                
                Log.Error($"Found more than one user with email {email}");
            }

            return pswList[0];
        }

		public IPrincipal GetUserRoles(IIdentity identity)
        {
            List<string> roles = new List<string>();

            IList<string> anos = new List<string>(Configuration.Years);
            if (Configuration.Years.Count == 0)
            {
                anos.Add(Configuration.DefaultYear);
            }

            foreach (string Qyear in anos)
            {
                PersistentSupport sp = PersistentSupport.getPersistentSupport(Qyear, identity.Name);
                sp.openConnection();

                SelectQuery select = new SelectQuery()
                    .Select(CSGenioAuserauthorization.FldModulo, "modulo")
                    .Select(CSGenioAuserauthorization.FldNivel, "nivel")
                    .Select(CSGenioAuserauthorization.FldRole, "role")
                    .From(Area.AreaPSW)
                    .Join(Area.AreaUSERAUTHORIZATION).On(CriteriaSet.And().Equal(CSGenioApsw.FldCodpsw, CSGenioAuserauthorization.FldCodpsw))
                    .Where(CriteriaSet.And()
                        .Equal(CSGenioApsw.FldNome, identity.Name)
                        .Equal(CSGenioAuserauthorization.FldSistema, Configuration.Program)
                        );

                string[] modulos = new CSGenioApsw(null, null).getModules(); //passar isto a static

                DataMatrix data = sp.Execute(select);
                sp.closeConnection();

                bool temModulo = false;
                for (int row = 0; row < data.NumRows; row++)
                {
                    string module = data.GetString(row, "modulo");
                    string role = data.GetString(row, "role");
                    int level = data.GetInteger(row, "nivel");
                    if (Array.IndexOf(modulos, module) != -1)
                    {
                        if(!String.IsNullOrEmpty(role))
                        {
                            temModulo = true;
                            roles.Add(Qyear + "." + module + "." + role);
                        }
                        else if(level > 0)
                        {
                            temModulo = true;
                            roles.Add(Qyear + "." + module + "." + level.ToString());
                        }
                    }
                }
                //so tem acesso ao Qyear se tiver pelo menos um módulo
                if (temModulo)
                    roles.Add(Qyear);
            }

            return new GenericPrincipal(identity, roles.ToArray());
        }
    }
}
