using System;

namespace CSGenio.framework
{
	/// <summary>
	/// Guarda os tipos possíveis de ligação a gestores de Base de Dados.
	/// </summary>
    public enum DatabaseType
	{
        ACCESS,
        ORACLE,
        SQLSERVER2000,
        SQLSERVER2005,
        SQLSERVER2008,
        SQLITE,
		MYSQL,
        ERRO
	}
	
	/// <summary>
    /// Guarda os tipos de key primária possiveis.
    /// </summary>
    /// 
    public enum CodeType
    {
        NOT_KEY = 0,
        STRING_KEY,
        GUID_KEY,
        INT_KEY,
    };

}
