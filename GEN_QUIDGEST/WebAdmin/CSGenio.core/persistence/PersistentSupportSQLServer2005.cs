using System.Text;
using CSGenio.business;
using System.Data;
using System;
using Quidgest.Persistence.GenericQuery;
using Quidgest.Persistence.Dialects;

namespace CSGenio.persistence
{
    /// <summary>
    /// Summary description for PersistentSupportSQLServer2005.
    /// </summary>
    /// <remarks>
    /// TODO: Esta classe está vazia mas vai ser necesário por aqui algumas diferenças na construção das queries
    /// </remarks>
    public class PersistentSupportSQLServer2005 : PersistentSupportSQLServer2000
    {
        /// <summary>
        /// Contructor
        /// </summary>
        public PersistentSupportSQLServer2005() : base() 
		{ 
			Dialect = new SqlServer2005Dialect();
		}
		
		// Este metodo foi colocado aqui como forma "rapida" de pôr os WS a funcionar... Rever assim que possivel
		/// <summary>
        /// Método to select registos entre linhas pedidas
        /// </summary>
        /// <param name="identificador">identifier da query</param>
        /// <param name="listagem">Qlisting</param>
        /// <param name="condicoes">conditions</param>
        /// <param name="linhaIncial">linha inicial</param>
        /// <param name="linhaFinal">linha final</param>
        /// <returns>Qlisting com os dados seleccionados</returns>
        [Obsolete("Use Listing seleccionarEntre(string identificador, Listing listagem, CriteriaSet condicoes, int linhaIncial, int linhaFinal) instead")]
        public Listing selectBetween(string identifier, Listing Qlisting, string conditions, int firstLine, int lastLine)
        {
            try
            {
                string[] queryGenio = (string[])controlos[identifier];
                QuerySelect querySelect = new QuerySelect(DatabaseType);
                bool distinct = false;
                if (queryGenio.Length > 3)
                    distinct = (queryGenio[3] == "false");

                if (controlosOverride.ContainsKey(identifier))
                {
                    querySelect = controlosOverride[identifier](Qlisting.User, Qlisting.Module, conditions, this);
                    //AV 2009/11/20 A query passa a só ir buscar a ordenação que vem do genio 
                    //quando não foi definida nenhuma no override
                    if (querySelect.Order.Equals(""))
                        querySelect.Order = new StringBuilder(Qlisting.Sort);
                }
                else
				{
                    querySelect.increaseQueryBetweenLines(queryGenio[0], queryGenio[1], queryGenio[2], conditions, Qlisting.Sort, firstLine, lastLine, distinct);
				}
                DataSet ds = executeQuery(querySelect.Query).DbDataSet;
                ds.Tables[0].Columns.RemoveAt(0); //tirar a coluna com a rownum
                Qlisting.DataMatrix = ds;
                Qlisting.LastFilled = ds.Tables[0].Rows.Count;
                return Qlisting;

            }
            catch (PersistenceException ex)
            {
                throw new PersistenceException(ex.UserMessage, "PersistentSupportSQLServer2005.seleccionarEntre", 
											   "Error selecting records - " + string.Format("[identificador] {0}; [listagem] {1}; [condicoes] {2}; [linhaIncial] {3}; [linhaFinal] {4}: ",
																							identifier, Qlisting.ToString(), conditions, firstLine.ToString(), lastLine.ToString()) + ex.Message, ex);
            }
            catch (Exception ex)
            {
				throw new PersistenceException(null, "PersistentSupportSQLServer2005.seleccionarEntre", 
											   "Error selecting records - " + string.Format("[identificador] {0}; [listagem] {1}; [condicoes] {2}; [linhaIncial] {3}; [linhaFinal] {4}: ",
																							identifier, Qlisting.ToString(), conditions, firstLine.ToString(), lastLine.ToString()) + ex.Message, ex);
            }
        }

        public Listing selectBetween(string identifier, Listing Qlisting, CriteriaSet conditions, int firstLine, int lastLine)
        {
            try
            {
                ControlQueryDefinition queryGenio = controlQueries[identifier];
                SelectQuery querySelect = new SelectQuery();

                if (controlQueriesOverride.ContainsKey(identifier))
                {
                    querySelect = controlQueriesOverride[identifier](Qlisting.User, Qlisting.Module, conditions, Qlisting.QuerySort, this);
                    //AV 2009/11/20 A query passa a só ir buscar a ordenação que vem do genio 
                    //quando não foi definida nenhuma no override
                    if (querySelect.OrderByFields.Count == 0)
                    {
                        foreach (ColumnSort sort in Qlisting.QuerySort)
                        {
                            querySelect.OrderByFields.Add(sort);
                        }
                    }
                }
                else
                {
                    QueryUtils.increaseQueryBetweenLines(querySelect, queryGenio.SelectFields, queryGenio.FromTable, queryGenio.WhereConditions, conditions, Qlisting.QuerySort, firstLine, lastLine, queryGenio.Distinct);
                }
                DataSet ds = Execute(querySelect).DbDataSet;
                Qlisting.DataMatrix = ds;
                Qlisting.LastFilled = ds.Tables[0].Rows.Count;
                return Qlisting;
            }
            catch (PersistenceException ex)
            {
                throw new PersistenceException(ex.UserMessage, "PersistentSupportSQLServer2005.seleccionarEntre", 
											   "Error selecting records - " + string.Format("[identificador] {0}; [listagem] {1}; [condicoes] {2}; [linhaIncial] {3}; [linhaFinal] {4}: ",
																							identifier, Qlisting.ToString(), conditions.ToString(), firstLine.ToString(), lastLine.ToString()) + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PersistenceException(null, "PersistentSupportSQLServer2005.seleccionarEntre", 
											   "Error selecting records - " + string.Format("[identificador] {0}; [listagem] {1}; [condicoes] {2}; [linhaIncial] {3}; [linhaFinal] {4}: ",
																							identifier, Qlisting.ToString(), conditions.ToString(), firstLine.ToString(), lastLine.ToString()) + ex.Message, ex);
            }
        }
    }
}
