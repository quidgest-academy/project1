﻿using CSGenio.framework;

namespace CSGenio.persistence
{
    public static class PersistenceFactoryExtension
    {
        /// <summary>
        /// Use this class as the factory method for persistent support allocation
        /// </summary>
        public static void Use()
        {
            PersistentSupport.RegisterSpFactory(PersistenceFactoryExtension.getPersistentSupport);
        }

        /// <summary>
        /// Last updated by [CJP] at [2016.07.06]
        /// Método to retornar a subclasse de suporte persistente
        /// </summary>
        /// <returns>Devolve uma instancia de PersistentSupport</returns>
        private static PersistentSupport getPersistentSupport(DatabaseType dbType)
        {
            try
            {
                PersistentSupport res;
                switch (dbType)
                {
                    case DatabaseType.ORACLE:
                        res = new PersistentSupportOracle19();
                        break;
                    case DatabaseType.SQLSERVER2000:
                        res = new PersistentSupportSQLServer2000();
                        break;
                    case DatabaseType.SQLSERVER2005:
                    case DatabaseType.SQLSERVER2008:
                        res = new PersistentSupportSQLServer2005();
                        break;
                    case DatabaseType.SQLITE:
                        res = new PersistentSupportSQLite();
                        break;
                    case DatabaseType.MYSQL:
                        res = new PersistentSupportMySql();
                        break;
                    default:
                        throw new PersistenceException("Não foi possível estabelecer ligação à base de dados.", "PersistentSupport.getPersistentSupport", "Unknown database type: " + dbType);
                }
                //res.DatabaseType = dbType;
                return res;
            }
            catch (FrameworkException ex)
            {
                if (ex.UserMessage == null)
                    throw new PersistenceException("Não foi possível estabelecer ligação à base de dados.", "PersistentSupport.getPersistentSupport", "Error getting persistent support: " + ex.Message, ex);
                else
                    throw new PersistenceException("Não foi possível estabelecer ligação à base de dados." + ex.UserMessage, "PersistentSupport.getPersistentSupport", "Error getting persistent support: " + ex.Message, ex);
            }
        }
    }
}
