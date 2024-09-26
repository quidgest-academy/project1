﻿using System;
using System.Data;
using System.Data.OracleClient;
using CSGenio.framework;
using Quidgest.Persistence.Dialects;
using Quidgest.Persistence.GenericQuery;

namespace CSGenio.persistence
{
    /// <summary>
    /// Summary description for PersistentSupportOracle.
    /// </summary>

    public class PersistentSupportOracle : PersistentSupport
    {
        public override IDbDataParameter CreateParameter()
        {
            return new OracleParameter();
        }

        public override IDbDataParameter CreateParameter(object value)
        {
            if (value is Guid || (value is Guid? && value != null))
            {
                return base.CreateParameter(((Guid)value).ToByteArray());
            }

            return base.CreateParameter(value);
        }

		// schema sobre o qual as queries s�o excutadas
        private string schema_bd = null;
        // Define se as pesquisas nas BD s�o case insensitive(apenas to ORACLE), por pr�-defini��o est� desactivado
        private bool insensitive = false;

        /// <summary>
        /// Contructor
        /// </summary>
        public PersistentSupportOracle()
        {
			Dialect = new Oracle10gDialect();
        }

        protected override void BuildConnection(DataSystemXml dataSystem, string login, string password, int connectionTimeout = 0)
        {
            schema_bd = Configuration.GetProperty("SCHEMA_BD", null);
            insensitive = (Configuration.GetProperty("INSENSITIVE", null) ?? "0") == "1";

            string cs = "Data Source=(DESCRIPTION=(ADDRESS_LIST="
                + "(ADDRESS=(PROTOCOL=TCP" + (dataSystem.Schemas[0].ConnEncrypt ? "S" : "") + ")(HOST=" + dataSystem.Server + ")(PORT=" + dataSystem.Port + ")))"
                + "(CONNECT_DATA=";
            if (!String.IsNullOrEmpty(dataSystem.ServiceName))
                cs += "(SERVICE_NAME=" + dataSystem.ServiceName + ")";

            if (!String.IsNullOrEmpty(dataSystem.Service))
                cs += "(SID=" + dataSystem.Service + ")";

            if (login == null)
            {
                cs += "));" + "User Id=" + dataSystem.LoginDecode() + ";Password=" + dataSystem.PasswordDecode() + ";";
            }
            else
            {
                cs += "));" + "User Id=" + login + ";Password=" + password + ";";                
            }
#pragma warning disable 618
            Connection = new OracleConnection(cs);
#pragma warning restore 618
        }

        /// <summary>
        /// Fun��o que abre uma conex�o
        /// </summary>
        public override void openConnection()
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                {
                    Log.Debug("abrirConexao.");
					string QueryCaseInc;
                    Connection.Open();
					IDbCommand comand;
					
                    //JG 270307 querys to alterara propriedade da conec��o to CASE INCENSITIVE   
                    // RR 27-06-2011
                    // os alter session seguintes fazem com que certos �ndices n�o sejam utilizados
                    // o que origina a degrada��o da performance
					if(insensitive)
					{
						QueryCaseInc = "ALTER SESSION SET NLS_SORT=BINARY_AI";
						comand = CreateCommand(QueryCaseInc);
						comand.ExecuteNonQuery();

						QueryCaseInc = "ALTER SESSION SET NLS_COMP=LINGUISTIC";
						comand = CreateCommand(QueryCaseInc);
						comand.ExecuteNonQuery();
					}

                    // se for definido o schema sobre o qual as queries s�o executadas
                    if (!String.IsNullOrEmpty(schema_bd))
                    {
                        string querySchema_bd = "ALTER SESSION SET CURRENT_SCHEMA = " + schema_bd;
                        comand = CreateCommand(querySchema_bd);
                        comand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new PersistenceException("N�o foi poss�vel estabelecer liga��o � base de dados.", "PersistentSupportOracle.abrirConexao", "Error opening connection: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Instancia um novo adaptador de Sql
        /// </summary>
        /// <param name="query">A query de inicializa��o do adaptador</param>
        /// <returns>Um adaptador de sql</returns>
        public override IDbDataAdapter CreateAdapter(string query)
        {
            IDbCommand command = CreateCommand(query);
#pragma warning disable 618
            return new OracleDataAdapter((OracleCommand)command);
#pragma warning restore 618
        }

        /// <summary>
        /// Obtem uma nova key prim�ria to um determinado objecto
        /// </summary>
        /// <param name="id_objecto">O objecto to o qual se quer gerar uma key, tipicamente o name da table</param>
        /// <param name="tamanho">O size da key a gerar to o caso de codigo internos</param>
        /// <param name="formato">O format de key a gerar</param>
        /// <returns>Uma key prim�ria �nica</returns>
        public override string generatePrimaryKey(string id_object, int size, CodeType format)
        {
            if (format.Equals(CodeType.GUID_KEY))
                return Guid.NewGuid().ToString();
#pragma warning disable 618
            OracleCommand command = CreateCommand("updateCod") as OracleCommand;
#pragma warning restore 618
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("p_Id_objecto", OracleType.VarChar).Value = id_object.ToUpper();
            command.Parameters.Add("p_Blksize", OracleType.Int32).Value = 1;
            command.Parameters.Add("codigo", OracleType.Cursor).Direction = ParameterDirection.Output;

            //um stored procedure em oracle que retorna um cursor tem de ser lido por um datareader
            //http://msdn.microsoft.com/en-us/library/ms971506.aspx
            OracleDataReader dr = command.ExecuteReader();
            int codigoNovo = 0;
            if (dr.Read())
                codigoNovo = (int)((decimal)dr[0]);

            if (codigoNovo < 1)
            {
                throw new PersistenceException(null, "PersistentSupportOracle.generatePrimaryKey", 
				                               "The primary key generated for object with id " + id_object + ", with size " + size + " and format " + format.ToString() + " is invalid: " + codigoNovo.ToString());
                // closeConnection();
                // return null;
            }

			if (format.Equals(CodeType.STRING_KEY))
            {
                return codigoNovo.ToString().PadLeft(size);
            }

            return codigoNovo.ToString();
        }

        public override IDbConnection GetConnectionToServer()
        {
            throw new NotImplementedException();
        }

        public override bool CheckIfDatabaseExists(string database)
        {
            throw new NotImplementedException();
        }

        public override void Drop(string schema)
        {
            throw new NotImplementedException();
        }
    }
}
