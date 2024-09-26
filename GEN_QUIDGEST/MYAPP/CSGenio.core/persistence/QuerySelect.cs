using System;
using System.Collections;
using System.Collections.Generic;
using CSGenio.business;
using CSGenio.framework;
using System.Text;

namespace CSGenio.persistence
{
    /// <summary>
    /// Esta classe serve to criar query de select
    /// </summary>
	[Obsolete("Use Query.SelectQuery instead")]
    public class QuerySelect : Query
    {
        /// <summary>
        /// Fields que vão ser seleccionados
        /// </summary>
        StringBuilder select;

        /// <summary>
        /// Tabelas a que pertencem os fields que vão ser seleccionados
        /// </summary>
        string from;

        /// <summary>
        /// Condições de seleção
        /// </summary>
        StringBuilder where;

        /// <summary>
        /// Ordenação dos dados resultantes da seleção
        /// </summary>
        StringBuilder order;

        /// <summary>
        /// Group by do query
        /// </summary>
        string groupBy;

        /// <summary>
        /// string com o query a ser executado
        /// </summary>
        string query;

        /// <summary>
        /// número de registos por query
        /// </summary>
        int nrRecords;

        /// <summary>
        /// true to fazer select distinct
        /// </summary>
        bool distinct;

        /// <summary>
        /// Type de base de dados
        /// </summary>
        DatabaseType connectionType;

        /// <summary>
        /// constructor da classe
        /// </summary>
        public QuerySelect(DatabaseType dbType) { connectionType = dbType; }

        /// <summary>
        /// Lista com os fields a serem devolvidos pela query
        /// </summary>
        private List<string> camposSelect;

        /// <summary>
        /// Get do camposSelect
        /// </summary>
        public List<string> SelectFields
        {
            get { return camposSelect; }
        }

        private string primaryKey;

        /// <summary>
        /// Name da key primária
        /// </summary>
        public string QPrimaryKey
        {
            get { return primaryKey; }
            set { primaryKey = value; }
        }


        /// <summary>
        /// To acrescentar um Qfield a ser seleccionado
        /// </summary>
        /// <param name="campo"></param>
        public void addSelect(string Qfield)
        {
            if (select == null)
                select = new StringBuilder(Qfield);
            else
                select.Append(", " + Qfield);
        }

        /// <summary>
        /// To acrescentar um array de fields que serão seleccionado
        /// </summary>
        /// <param name="campo"></param>
        public void addSelect(string[] fields)
        {
            if (select != null)
                select.Append(", ");
            else
                select = new StringBuilder();

            for (int i = 0; i < fields.Length; i++)
                select.Append(fields[i] + ", ");
            if (select.Length >= 2)
                select.Remove(select.Length - 2, 2);
        }

        /// <summary>
        /// Método to adicionar uma condição ao query
        /// </summary>
        /// <param name="nomeCampo">name do Qfield</param>
        /// <param name="valorCampo">Qvalue do Qfield</param>
        /// <param name="formatCampo">formatação do Qfield</param>
        /// <param name="conector">conector</param>
        public void addWhere(string fieldName, object fieldValue, FieldFormatting formatField, string conector)
        {
            string condition = "";
            if (fieldValue == null || fieldValue == DBNull.Value)
                condition = fieldName + " IS NULL";
            else
                condition = createCondition(fieldName, fieldValue, formatField, "=");

            if (condition != null && !condition.Equals(""))
            {
                if (where == null)
                    where = new StringBuilder(condition);
                else
                    where.Append(" " + conector + " " + condition);
                if (connectionType == DatabaseType.ORACLE)
                    where.Replace("= ''", "is null");
            }
        }

        /// <summary>
        /// Método to adicionar uma condição ao query
        /// </summary>
        /// <param name="nomeCampo">name do Qfield</param>
        /// <param name="valorCampo">Qvalue do Qfield</param>
        /// <param name="formatCampo">formatação do Qfield</param>
        /// <param name="operacao">operation</param>
        /// <param name="conector">conector tipicamente AND ou OR</param>
        public void addWhere(string fieldName, object fieldValue, FieldFormatting formatField, string operation, string conector)
        {
            string condition = createCondition(fieldName, fieldValue, formatField, operation);
            if (condition != null && !condition.Equals(""))
            {
                if (where == null)
                    where = new StringBuilder(condition);
                else
                    where.Append(" " + conector + " " + condition);
                //se a BD é Oracle em oracle
                if (connectionType == DatabaseType.ORACLE)
                    where.Replace("= ''", "is null");
            }
        }

        /// <summary>
        /// Método to adicionar uma condição ao query
        /// </summary>
        /// <param name="condicao">condição</param>
        /// <param name="conector">conector</param>
        public void addWhere(string condition, string conector)
        {
            if (condition != null && !condition.Equals(""))
            {
                if (where == null)
                    where = new StringBuilder(condition);
                else
                    where.Append(" " + conector + " " + condition);

                //se a BD é Oracle em oracle
                if (connectionType == DatabaseType.ORACLE)
                    where.Replace("= ''", "is null");
            }
        }

        /// <summary>
        /// Método to adicionar conditions ( é usado na função exists filha)
        /// </summary>
        /// <param name="campos">fields</param>
        /// <param name="valor">Qvalue do Qfield</param>
        /// <param name="formatacao">formatação do Qfield</param>
        public void addWhere(string[] fields, object Qvalue, FieldFormatting formatting)
        {
            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i] != null && !fields[i].Equals(""))
                {
                    if (where == null)
                        where = new StringBuilder(createCondition(fields[i], Qvalue, formatting, "="));
                    else
                        where.Append(" OR " + createCondition(fields[i], Qvalue, formatting, "="));
                }
            }
            if (connectionType == DatabaseType.ORACLE)
                where.Replace("= ''", "is null");
        }

        /// <summary>
        /// To acrescentar uma ordenação
        /// </summary>
        /// <param name="ordenacao">ordenação</param>
        public void addOrder(string sorting)
        {
            //if (order == null)
            if (order == null || order.Length == 0) // RR se o objecto ja exists, mas tá vazio não mete virgula
                order = new StringBuilder(sorting);
            else
                order.Append(", " + sorting);
        }

        /// <summary>
        /// Aumenta a clausula FROM com tables onde há relações directas
        /// </summary>
        /// <param name="tabelas">tables directamente relaccionadas com a area</param>
        /// <param name="area">area table sobre a qual está a ser feita a query</param>	
        public void setFromTabDirect(List<Relation> relations, IArea area)
        {
            from = " " + area.TableName;
            if (connectionType == DatabaseType.ORACLE)
                from += " ";
            else
                from += " AS ";
            from += area.Alias;

            foreach (Relation r in relations)
            {
                from += " LEFT OUTER JOIN " + r.TargetTable;
                if (connectionType == DatabaseType.ORACLE)
                    from += " ";
                else
                    from += " AS ";
                from += r.AliasTargetTab +
                    " ON " + r.AliasSourceTab + "." + r.SourceRelField +
                    "=" + r.AliasTargetTab + "." + r.TargetRelField;
            }
        }        

        /// <summary>
        /// Parser do Select. Constrói uma lista com os nomes dos fields a serem lidos pela query.
        /// </summary>
        private void parseSelect()
        {
            camposSelect = new List<string>();
            //parse ao select da query select
            //vamos assumir que a string select está na forma area.campo1 "area.campo1", area.campo2 "area.campo2", etc...
            string[] aux = Select.ToString().Split(',');
            int i = 0;
            while (i < aux.Length)
            {
                string[] subaux = aux[i].Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                camposSelect.Add(subaux[0]);
                i = i + 1;
            }
        }

        /// <summary>
        /// Constroi um query select
        /// </summary>
        public void buildQuery()
        {
            parseSelect();
            if (where != null && where.ToString().Contains("LIKE"))
                where.Replace("*", "%");
            if (nrRecords != 0)
            {
                if (connectionType == DatabaseType.ORACLE)
                {
                    if (where != null && where.Length != 0)
                        if (order != null && order.Length != 0)
                            query = "SELECT * FROM (SELECT " + (distinct == true ? " DISTINCT " : "") + select.ToString() + " FROM " + from.ToString() + " WHERE " + where.ToString() + " ORDER BY " + order.ToString() + ") WHERE ROWNUM<=" + nrRecords;
                        else
                            query = "SELECT " + (distinct == true ? " DISTINCT " : "") + select.ToString() + " FROM " + from.ToString() + " WHERE ROWNUM<=" + nrRecords + " AND " + where.ToString();
                    else
                        if (order != null && order.Length != 0)
                            query = "SELECT " + (distinct == true ? " DISTINCT " : "") + select.ToString() + " FROM " + from.ToString() + " WHERE ROWNUM<=" + nrRecords + " ORDER BY " + order.ToString();
                        else
                            query = "SELECT " + (distinct == true ? " DISTINCT " : "") + select.ToString() + " FROM " + from.ToString() + " WHERE ROWNUM<=" + nrRecords;
                }
                else
                {
                    if (where != null && where.Length != 0)
                        if (order != null && order.Length != 0)
                            query = "SELECT" + (distinct == true ? " DISTINCT " : "") + " TOP " + nrRecords + " " + select.ToString() + " FROM " + from.ToString() + " WHERE " + where.ToString() + " ORDER BY " + order.ToString();
                        else
                            query = "SELECT" + (distinct == true ? " DISTINCT " : "") + " TOP " + nrRecords + " " + select.ToString() + " FROM " + from.ToString() + " WHERE " + where.ToString();
                    else
                        if (order != null && order.Length != 0)
                            query = "SELECT" + (distinct == true ? " DISTINCT " : "") + " TOP " + nrRecords + " " + select.ToString() + " FROM " + from.ToString() + " ORDER BY " + order.ToString();
                        else
                            query = "SELECT" + (distinct == true ? " DISTINCT " : "") + " TOP " + nrRecords + " " + select.ToString() + " FROM " + from.ToString();
                }
            }
            else
            {
                if (where != null && where.Length != 0)
                    if (groupBy != null && groupBy.Length != 0)
                        if (order != null && order.Length != 0)
                            query = "SELECT " + (distinct == true ? " DISTINCT " : "") + select.ToString() + " FROM " + from.ToString() + " WHERE " + where.ToString() + " GROUP BY " + groupBy + " ORDER BY " + order.ToString();
                        else
                            query = "SELECT " + (distinct == true ? " DISTINCT " : "") + select.ToString() + " FROM " + from.ToString() + " WHERE " + where.ToString() + " GROUP BY " + groupBy;
                    else
                        if (order != null && order.Length != 0)
                            query = "SELECT " + (distinct == true ? " DISTINCT " : "") + select.ToString() + " FROM " + from.ToString() + " WHERE " + where.ToString() + " ORDER BY " + order.ToString();
                        else
                            query = "SELECT " + (distinct == true ? " DISTINCT " : "") + select.ToString() + " FROM " + from.ToString() + " WHERE " + where.ToString();
                else
                    if (groupBy != null && groupBy.Length != 0)
                        if (order != null && order.Length != 0)
                            query = "SELECT " + (distinct == true ? " DISTINCT " : "") + select.ToString() + " FROM " + from.ToString() + " GROUP BY " + groupBy + " ORDER BY " + order.ToString();
                        else
                            query = "SELECT " + (distinct == true ? " DISTINCT " : "") + select.ToString() + " FROM " + from.ToString() + " GROUP BY " + groupBy;
                    else
                        if (order != null && order.Length != 0)
                            query = "SELECT " + (distinct == true ? " DISTINCT " : "") + select.ToString() + " FROM " + from.ToString() + " ORDER BY " + order.ToString();
                        else
                            query = "SELECT " + (distinct == true ? " DISTINCT " : "") + select.ToString() + " FROM " + from.ToString();
            }
        }

        /// <summary>
        /// Constroi um query select
        /// </summary>
        public void buildQuery(string primaryKey)
        {
            parseSelect();
            if (where != null && where.ToString().Contains("LIKE"))
                where.Replace("*", "%");
            if (Order == null || order.Length == 0 || !returnFieldsOrder().Contains(primaryKey))
                addOrder(primaryKey + " ASC");

            // RR 26-01-2011 - Troquei a order dos if's to uma melhor legibilidade
            if (connectionType == DatabaseType.ORACLE)
            {
                if (nrRecords != 0)
                {
                    // RR 26-01-2011
                    // Este código está repetido em vários lados devido à estrutura desta classe
                    // Quando se fizer uma re-estruturação de fundo, devem-se evitar ao máximo estas repetições 

                    // query que obtém os Qvalues da key primária, já com a ordenação final da query
                    query =
                        "SELECT " + primaryKey + "\"" + primaryKey + "\" " +
                        "FROM " + from.ToString() + " " +
                        ((where != null && where.Length > 0) ? "WHERE " + where.ToString() + " " : "") +
                        "ORDER BY " + order.ToString();

                    // query que acrescenta o rownum to a paginação
                    query = "SELECT \"" + primaryKey + "\", ROWNUM RNUM FROM (" + query + ")";
                    // query que faz a paginação com base no rownum
                    query = "SELECT \"" + primaryKey + "\" FROM (" + query + ") WHERE RNUM <= " + nrRecords.ToString();
                    // query que vai realmente obter os dados dos registos limitados pelas querys anteriores (o order by é repetido aqui porque é mesmo necessário)
                    query = "SELECT " + select.ToString() + " FROM " + from.ToString() + " WHERE " + primaryKey + " IN (" + query + ") ORDER BY " + order.ToString();
                }
                else
                {
                    if (where != null && where.Length != 0)
                    {
                        query = "SELECT * FROM (SELECT tabela.*,ROWNUM rnum FROM (SELECT " + select.ToString() + " FROM " + from.ToString() + " WHERE " + where.ToString();
                        query += (distinct == true ? " GROUP BY " + returnFieldsOrder() : "");
                        query += " ORDER BY " + order.ToString() + ") tabela ORDER BY rnum)";
                    }
                    else
                    {
                        query = "SELECT * FROM (SELECT tabela.*,ROWNUM rnum FROM (SELECT " + select.ToString() + " FROM " + from.ToString();
                        query += (distinct == true ? " GROUP BY " + returnFieldsOrder() : "");
                        query += " ORDER BY " + order.ToString() + ") tabela ORDER BY rnum)";
                    }
                }
            }
            else // Configuration.DatabaseType != ORACLE
            {
                if (nrRecords != 0)
                {
                    if (where != null && where.Length != 0)
                    {
                        query = "SELECT " + select.ToString() + " FROM " + from.ToString() + " WHERE " + where.ToString() + " AND " + primaryKey + " IN ";
                        query += "(SELECT TOP " + nrRecords + " " + primaryKey + " FROM " + from.ToString() + " WHERE " + where.ToString();
                        query += " ORDER BY " + order.ToString() + ")" + (distinct == true ? " GROUP BY " + returnFieldsOrder() : "") + " ORDER BY " + order.ToString();
                    }
                    else
                    {
                        query = "SELECT " + select.ToString() + " FROM " + from.ToString() + " WHERE " + primaryKey + " IN ";
                        query += "(SELECT TOP " + nrRecords + " " + primaryKey + " FROM " + from.ToString();
                        query += (distinct == true ? " GROUP BY " + primaryKey + ", " + returnFieldsOrder() : "") + " ORDER BY " + order.ToString();
                    }
                }
                else
                {
                    if (where != null && where.Length != 0)
                    {
                        query = "SELECT " + select.ToString() + " FROM " + from.ToString() + " WHERE " + where.ToString();
                        query += (distinct == true ? " GROUP BY " + returnFieldsOrder() : "") + " ORDER BY " + order.ToString();
                    }
                    else
                    {
                        query = "SELECT " + select.ToString() + " FROM " + from.ToString();
                        query += (distinct == true ? " GROUP BY " + primaryKey + ", " + returnFieldsOrder() : "") + " ORDER BY " + order.ToString();
                    }
                }
            }
        }

        /// <summary>
        /// Método que constroi um query select tendo em conta a ultima linha lida ( método usado to paginar)
        /// </summary>
        public void buildQuery(int lastRead, string primaryKey)
        {
            parseSelect();
            if (where != null && where.ToString().Contains("LIKE"))
                where.Replace("*", "%");
            if (Order == null || order.Length == 0 || !returnFieldsOrder().Contains(primaryKey))
                addOrder(primaryKey + " ASC");
            if (nrRecords != 0)
            {
                if (connectionType == DatabaseType.ORACLE)
                {
                    // RR 26-01-2011
                    // Este código está repetido em vários lados devido à estrutura desta classe
                    // Quando se fizer uma re-estruturação de fundo, devem-se evitar ao máximo estas repetições 

                    // query que obtém os Qvalues da key primária, já com a ordenação final da query
                    query =
                        "SELECT " + primaryKey + "\"" + primaryKey + "\" " +
                        "FROM " + from.ToString() + " " +
                        ((where != null && where.Length > 0) ? "WHERE " + where.ToString() + " " : "") +
                        "ORDER BY " + order.ToString();

                    // query que acrescenta o rownum to a paginação
                    query = "SELECT \"" + primaryKey + "\", ROWNUM RNUM FROM (" + query + ")";
                    // query que faz a paginação com base no rownum
                    query = "SELECT \"" + primaryKey + "\" FROM (" + query + ") WHERE RNUM BETWEEN " + (lastRead + 1).ToString() + " AND " + (nrRecords + lastRead).ToString();
                    // query que vai realmente obter os dados dos registos limitados pelas querys anteriores (o order by é repetido aqui porque é mesmo necessário)
                    query = "SELECT " + select.ToString() + " FROM " + from.ToString() + " WHERE " + primaryKey + " IN (" + query + ") ORDER BY " + order.ToString();
                }
                else // Configuration.DatabaseType != ORACLE
                {

                    if (where != null && where.Length != 0)
                    {
                        query = "SELECT " + select.ToString() + " FROM " + from.ToString() + " WHERE " + where.ToString() + " AND " + primaryKey + " IN ";
                        query += "(SELECT TOP " + nrRecords + " " + primaryKey + " FROM " + from.ToString() + " WHERE " + where.ToString();
                        query += " AND " + primaryKey + " NOT IN (SELECT TOP " + lastRead + " " + primaryKey + " FROM " + from.ToString() + " WHERE " + where.ToString();
                        query += (distinct == true ? " GROUP BY " + returnFieldsOrder() : "") + " ORDER BY " + order.ToString() + ")" + (distinct == true ? " GROUP BY " + returnFieldsOrder() : "") + " ORDER BY " + order.ToString() + " ) ORDER BY " + order.ToString();
                    }
                    else
                    {
                        query = "SELECT " + select.ToString() + " FROM " + from.ToString() + " WHERE " + primaryKey + " IN ";
                        query += "(SELECT TOP " + nrRecords + " " + primaryKey + " FROM " + from.ToString() + " WHERE ";
                        query += primaryKey + " NOT IN (SELECT TOP " + lastRead + " " + primaryKey + " FROM " + from.ToString();
                        query += (distinct == true ? " GROUP BY " + primaryKey + ", " + returnFieldsOrder() : "") + " ORDER BY " + order.ToString() + ")" + (distinct == true ? " GROUP BY " + primaryKey + ", " + returnFieldsOrder() : "") + " ORDER BY " + order.ToString() + " ) ORDER BY " + order.ToString();
                    }
                }
            }
        }

        /// <summary>
        /// To construir um query count 
        /// </summary>
        /// <returns>uma string que corresponde à interrogação SQL a executar</returns>
        public string buildQueryCount()
        {
            if (where != null && where.ToString().Contains("LIKE"))
                where.Replace("*", "%");

            string query = "";
            if (where != null && where.Length != 0)
                query = "SELECT count(1) FROM " + from.ToString() + " WHERE " + where.ToString();
            else
                query = "SELECT count(1) FROM " + from.ToString();
            return query;
        }

        /// <summary>
        /// Função to aumentar a query quando temos um pedido get
        /// </summary>
        /// <param name="select">select do query</param>
        /// <param name="from">from do query</param>
        /// <param name="where">where do query</param>
        /// <param name="tipoLigacao">tipo de BD</param>
        /// <param name="numRegistos">número de registos a serem lidos</param>
        /// <param name="condicoes">conditions que vêm do interface</param>
        /// <param name="orderPedido">ordenação</param>
        /// <param name="selectDistinct">true to select distinct</param>
        public void increaseQuery(string select, string from, string where, DatabaseType connectionType, int numRecords, string conditions, string orderRequest, bool selectDistinct)
        {
            this.select = new StringBuilder(select);
            this.from = from;
            addWhere(where.Replace('*', '%'), " AND ");
            addWhere(conditions.Replace('*', '%'), " AND ");
            this.order = new StringBuilder(orderRequest);
            this.nrRecords = numRecords;
            this.distinct = selectDistinct;
            parseSelect();
        }

        /// <summary>
        /// Função to aumentar os queries geradas pelo GENIO
        /// </summary>
        /// <param name="select">select do query</param>
        /// <param name="from">from do query</param>
        /// <param name="where">where do query</param>
        /// <param name="condicoes">string que corresponde às condições separadas por AND</param>
        /// <param name="ordenacao">string que corresponde à ordenação</param>
        /// <param name="linhaInicial">linha incial</param>
        /// <param name="linhaFinal">linha final</param>
        /// <param name="selectDistinct">table</param>
        /// <param name="selectDistinct">true to select distinct</param>
        public string increaseQueryBetweenLines(string select, string from, string where, string conditions, string sorting, int firstLine, int lastLine, bool selectDistinct)
        {
            this.select = new StringBuilder(select);
            this.from = from;
            addWhere(where.Replace('*', '%'), " AND ");
                        addWhere(conditions.Replace('*', '%'), "AND");
            this.order = new StringBuilder(sorting);
            this.distinct = selectDistinct;

            if (this.where != null && this.where.Length!=0)
            {
                this.query = "SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY " + order.ToString() + ") AS 'RowNumber'," + select.ToString();

                if (lastLine == 0)
                    this.query += " FROM " + from + " WHERE " + this.where.ToString() + " ) as QueryResult WHERE RowNumber >= " + firstLine.ToString();
                else
                    this.query += " FROM " + from + " WHERE " + this.where.ToString() + " ) as QueryResult WHERE RowNumber BETWEEN " + firstLine.ToString() + " AND " + lastLine.ToString();
            }
            else
            {
                this.query = "SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY " + order.ToString() + ") AS 'RowNumber', " + select.ToString();

                if (lastLine == 0)
                    this.query += " FROM " + from + " ) as QueryResult WHERE RowNumber >= " + firstLine.ToString();
                else
                    this.query += " FROM " + from + " ) as QueryResult WHERE RowNumber BETWEEN " + firstLine.ToString() + " AND " + lastLine.ToString();
            }
            return this.query;
        }

        /// <summary>
        /// Método usado to criar o where do query que permite obter a posição do último registo lido
        /// </summary>
        /// <param name="ordenacao">string recebida da interface com a ordenação</param>
        /// <param name="tabela">name da table</param>
        /// <param name="chavePrimaria">name da key primária</param>
        /// <param name="valorChavePrimaria">Qvalue da key primária</param>
        /// <param name="tipoLigacao">tipo de BD</param>
        public void setWhereGetPos(string sorting, string table, string alias, string primaryKey, string primaryKeyValue, DatabaseType connectionType)
        {
            if (this.Where != null && this.Where.Length != 0)
                this.Where.Append(" AND ");
            else
                this.Where = new StringBuilder();

            //Qfield de ordenação
            //só está implementado to os casos em que temos uma única colunca de ordenação
            //se existirem Qvalues repetidos aparecem todos
            /*select count(codtpflt)
            from GIPtpflt
            where tpflt < (select tpflt from GIPtpflt where codtpflt='    65') */
            string[] sortingField = sorting.Split(',');
            //só considera o 1º

            if (sortingField.Length == 0)
                throw new PersistenceException(null, "QuerySelect.setWhereGetPos", "No fields to order by.");

            string[] campoTpOrdenacao = sortingField[0].Trim().Split(' ');
            campoTpOrdenacao[0] = campoTpOrdenacao[0].Trim();
            campoTpOrdenacao[1] = campoTpOrdenacao[1].Trim();
            if (connectionType == DatabaseType.ORACLE)
            {
                if (campoTpOrdenacao[1].Equals(SortingType.ASC.ToString()))
                    this.where.Append(campoTpOrdenacao[0] + "<(select " + campoTpOrdenacao[0] + " from " + table + " " + alias + " where " + primaryKey + " = '" + primaryKeyValue + "')");
                else
                    this.where.Append(campoTpOrdenacao[0] + ">(select " + campoTpOrdenacao[0] + " from " + table + " " + alias + " where " + primaryKey + " = '" + primaryKeyValue + "')");
            }
            else
            {
                if (campoTpOrdenacao[1].Equals(SortingType.ASC.ToString()))
                    this.where.Append(campoTpOrdenacao[0] + "<(select " + campoTpOrdenacao[0] + " from " + table + " as " + alias + " where " + primaryKey + " = '" + primaryKeyValue + "')");
                else
                    this.where.Append(campoTpOrdenacao[0] + ">(select " + campoTpOrdenacao[0] + " from " + table + " as " + alias + " where " + primaryKey + " = '" + primaryKeyValue + "')");
            }
        }

        /// <summary>
        /// Fields usados no order da query
        /// </summary>
        public string returnFieldsOrder()
        {
            string camposOrder = order.ToString().ToUpper();
            camposOrder = camposOrder.Replace("DESC", "");
            camposOrder = camposOrder.Replace("ASC", "");
            return camposOrder.ToLower();
        }

        /// <summary>
        /// Método to criar o from do query sem alias 
        /// </summary>
        public void setFromWithoutAlias(string from)
        {
            this.from = from;
        }

        /// <summary>
        /// Método to criar o from do query com alias
        /// </summary>
        /// <param name="tabelas">tables</param>
        /// <param name="alias">alias</param>
        public void setFromWithAlias(string[] tables, string[] alias)
        {
            if (connectionType == DatabaseType.ORACLE)
            {
                for (int i = 0; i < tables.Length; i++)
                    from += tables[i] + " " + alias[i] + ",";
            }
            else
            {
                for (int i = 0; i < tables.Length; i++)
                    from += tables[i] + " AS [" + alias[i] + "],";
            }
            if (from.Length >= 1)
                from = from.Remove(from.Length - 1, 1);
        }

        /// <summary>
        /// Método to criar o from do query com alias
        /// </summary>
        /// <param name="tabela">table</param>
        /// <param name="alias">alias</param>
        public void setFromWithAlias(string table, string alias)
        {
            if (connectionType == DatabaseType.ORACLE)
                from = table + " " + alias;
            else
                from = table + " AS [" + alias + "]";
        }

        /// <summary>
        /// Função que retorna o Qvalue do From
        /// </summary>
        /// <returns>o from do query</returns>
        public string getFrom()
        {
            return from;
        }

        /// <summary>
        /// get e set dos fields a serem seleccionados
        /// </summary>
        public StringBuilder Select
        {
            get { return select; }
            set { select = value; }
        }

        /// <summary>
        /// get e set do from
        /// </summary>
        public string From
        {
            get { return from; }
            set { from = value; }
        }

        /// <summary>
        /// get e set das conditions do query
        /// </summary>
        public StringBuilder Where
        {
            get { return where; }
            set { where = value; }
        }

        /// <summary>
        /// get e set da ordenação no query
        /// </summary>
        public StringBuilder Order
        {
            get { return order; }
            set { order = value; }
        }
        /// <summary>
        /// string que corresponde ao query
        /// </summary>
        public string Query
        {
            get { return query; }
            set { query = value; }
        }

        /// <summary>
        /// string que corresponde ao nr de registos por query
        /// </summary>
        public int RecordCount
        {
            get { return nrRecords; }
            set { nrRecords = value; }
        }

        /// <summary>
        /// string que corresponde ao group by de um query
        /// </summary>
        public string GroupBy
        {
            get { return groupBy; }
            set { groupBy = value; }
        }

        /// <summary>
        /// bool que indica se a query faz um select distinct
        /// </summary>
        public bool Distinct
        {
            get { return distinct; }
            set { distinct = value; }
        }
    }
}
