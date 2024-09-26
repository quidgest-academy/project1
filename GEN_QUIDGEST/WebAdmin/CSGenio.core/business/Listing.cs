using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;
using System.Reflection;
using Quidgest.Persistence;

namespace CSGenio.business
{
    /// <summary>
    /// Esta classe serve to as fun��es GET, GET+, GET-, GETPOS e GETNIVELTREE.
    /// O atributo :
    /// - fields representa o name dos fields que o user deseja ver
    /// - fieldsvalues representa os Qvalues retornados. Cada entrada � identificada
    /// por um inteiro que corresponde a uma linha de resposta. Isto � se eu quiser ver 50
    /// registos, a hashtable ter� 50 registos (ou menos, depende do que existir na BD)
    /// - posicoes � um array de inteiros com 2 posi��es que tem a posi��o do name do codigo
    /// interno e o name do Qfield de ordena��o no array fields. Serve to construir a clausula
    /// ORDER BY das queries.
    /// - estadoLista serve to introduzir o status do pedido, se correu td bem(OK), se houve
    /// algum erro(E), etc.
    /// - mensagemEstado serve to introuzir a mensagem que vou enviar ao lado cliente.
    /// Por exemplo - Altera��o Efectuada ou Erro na abertura da BD, etc.
    /// </summary>
    public class Listing
    {
        /// <summary>
        /// sorting
        /// </summary>
        [Obsolete("Use IList<ColumnSort> ordenacaoQuery instead")]
        private string sorting;
        private IList<ColumnSort> ordenacaoQuery;

        /// <summary>
        /// condi��o decorrente do uso de EPHs
        /// </summary>
        private CriteriaSet condicoesEphQuery;
        /// <summary>
        /// user em sess�o
        /// </summary>
        private User user;
        /// <summary>
        /// dados lidos da BD
        /// </summary>
        private DataSet matrizDados;

        private PersistentSupport sp;
        /// <summary>
        /// �ltima linha preenchida
        /// </summary>
        private int ultPreenchida;

        /// <summary>
        /// indica se queremos saber o total de registos
        /// </summary>
        public bool obterTotal;
        /// <summary>
        /// total de registos da Qlisting
        /// </summary>
        public int TotalRecords;
		/// <summary>
        /// Aplicar o comando NoLock nas queries
        /// </summary>
		private Boolean m_noLock;

        /// <summary>
        /// Lista de fields pedidos to esta Qlisting
        /// </summary>
        public string[] RequestedFields
        {
            get { return fieldsRequested; }
            set { fieldsRequested = value; }
        }
        private string[] fieldsRequested;

        /// <summary>
        /// Area base da Qlisting (null caso n�o aplic�vel)
        /// </summary>
        private Area area;

        /// <summary>
        /// Module
        /// </summary>
        protected string module;

        /// <summary>
        /// Construtor
        /// </summary>
        [Obsolete("Use Listing(Area area, object condEphObj, IList<ColumnSort> ordenacao, string modulo, LevelAccess nivelAcesso, string identificador, User utilizador, PersistentSupport sp) instead")]
        public Listing(Area area, object condEphObj, string sorting, string module, LevelAccess accessLevel, string identifier, User user, PersistentSupport sp)
        {
            this.user = user;
            this.module = module;
            this.sorting = sorting;
            this.sp = sp;
            this.area = area;
            condicoesEphQuery = CalculateConditionsEphGeneric(area, identifier);
			//NH(2016.07.21) - The NoLock command is only applied to SQL SERVER connections
			if (sp.DatabaseType == DatabaseType.SQLSERVER2000 || sp.DatabaseType == DatabaseType.SQLSERVER2005 || sp.DatabaseType == DatabaseType.SQLSERVER2008)
                this.m_noLock = true;
            else
                this.m_noLock = false;
        }

        /// <summary>
        /// Construtor
        /// </summary>
        [Obsolete("Use Listing(Area area, IList<ColumnSort> sorting, string module, string identifier, User user, PersistentSupport sp)")]
        public Listing(Area area, object condEphObj, IList<ColumnSort> sorting, string module, LevelAccess accessLevel, string identifier, User user, PersistentSupport sp)
        {
            this.user = user;
            this.module = module;
            this.ordenacaoQuery = sorting;
            bool addPrimaryKeySort = true;
            if (ordenacaoQuery.Count > 0 && ordenacaoQuery[0].Expression is ColumnReference)
            {
                ColumnReference cref = (ColumnReference)ordenacaoQuery[0].Expression;
                if (cref.TableAlias == area.Alias && cref.ColumnName == area.PrimaryKeyName)
                {
                    addPrimaryKeySort = false;
                }
            }

            if (addPrimaryKeySort)
            {
                this.ordenacaoQuery.Add(new ColumnSort(new ColumnReference(area.Alias, area.PrimaryKeyName), SortOrder.Ascending));
            }
            this.sp = sp;
            this.area = area;
            condicoesEphQuery = CalculateConditionsEphGeneric(area, identifier);

			//NH(2016.07.21) - The NoLock command is only applied to SQL SERVER connections
			if (sp.DatabaseType == DatabaseType.SQLSERVER2000 || sp.DatabaseType == DatabaseType.SQLSERVER2005 || sp.DatabaseType == DatabaseType.SQLSERVER2008)
                this.m_noLock = true;
            else
                this.m_noLock = false;
        }

        /// <summary>
        /// Construtor
        /// </summary>
        public Listing(Area area, IList<ColumnSort> sorting, string module, string identifier, User user, PersistentSupport sp)
        {
            this.user = user;
            this.module = module;
            this.ordenacaoQuery = sorting;
            bool addPrimaryKeySort = true;
            if (ordenacaoQuery.Count > 0 && ordenacaoQuery[0].Expression is ColumnReference)
            {
                ColumnReference cref = (ColumnReference)ordenacaoQuery[0].Expression;
                if (cref.TableAlias == area.Alias && cref.ColumnName == area.PrimaryKeyName)
                {
                    addPrimaryKeySort = false;
                }
            }

            if (addPrimaryKeySort)
            {
                this.ordenacaoQuery.Add(new ColumnSort(new ColumnReference(area.Alias, area.PrimaryKeyName), SortOrder.Ascending));
            }
            this.sp = sp;
            this.area = area;
            condicoesEphQuery = CalculateConditionsEphGeneric(area, identifier);

            //NH(2016.07.21) - The NoLock command is only applied to SQL SERVER connections
            if (sp.DatabaseType == DatabaseType.SQLSERVER2000 || sp.DatabaseType == DatabaseType.SQLSERVER2005 || sp.DatabaseType == DatabaseType.SQLSERVER2008)
                this.m_noLock = true;
            else
                this.m_noLock = false;
        }

		public Boolean NoLock
		{
			get
            {
                return m_noLock;
            }
            set
            {
                if (sp.DatabaseType != DatabaseType.SQLSERVER2000  && sp.DatabaseType != DatabaseType.SQLSERVER2005 && sp.DatabaseType != DatabaseType.SQLSERVER2008 && value==true)
                {
                    throw new BusinessException(null, "Listing.NoLock", "Current connection has unknown type: " + sp.DatabaseType.ToString());
                }

                m_noLock = value;
            }
        }

        /// <summary>
        /// Calcula uma condi��o em SQL que vai aplicar a uma area os filtros indicados nos EPH
        /// </summary>
        /// <param name="area">A area base</param>
        /// <param name="identificador">O identifier to aplicar overrides de eph</param>
        /// <returns>A condi��o de SQL</returns>
        public static CriteriaSet CalculateConditionsEphGeneric(Area area, string identifier)
        {
            CriteriaSet res = CriteriaSet.And();

            List<EPHOfArea> ephsDaArea = area.CalculateAreaEphs(area.User.Ephs, identifier, false);

            foreach (EPHOfArea eph in ephsDaArea)
            {
                if (eph.Relation2 != null)
                {
                    CriteriaSet inner_cs = new CriteriaSet(CriteriaSetOperator.And);
                    if(eph.Eph.OR_EPH1_EPH2)
                        inner_cs = new CriteriaSet(CriteriaSetOperator.Or);

                    // First relation can be empty
                    if (eph.Relation != null)
                        AuxAdicionaCondicaoOutraArea(inner_cs, area, eph.Eph, eph.ValuesList, eph.Relation);

                    // In order to reuse code, we create a second EPH field from data in area's EPH
                    EPHField EPH2 = new EPHField(eph.Eph.Name, eph.Eph.Table2, eph.Eph.Field2, eph.Eph.Operator2, eph.Eph.Propagate);
                    AuxAdicionaCondicaoOutraArea(inner_cs, area, EPH2, eph.ValuesList, eph.Relation2);
                    res.SubSet(inner_cs);
                }
                else if (eph.Relation != null)
                {
                    AuxAdicionaCondicaoOutraArea(res, area, eph.Eph, eph.ValuesList, eph.Relation);
                }
                else
                {
                    AuxAdicionaCondicaoMesmaArea(res, area, eph.Eph, eph.ValuesList);
                }
            }

            return res;
        }

		private static void AuxAdicionaCondicaoOutraArea(CriteriaSet cs, Area area, EPHField ephArea, object[] listaValores, Relation myrelacao)
        {
            AreaInfo tabelaEPH = Area.GetInfoArea(ephArea.Table);
            Field campoEPH = tabelaEPH.DBFields[ephArea.Field];

            string arorigem = myrelacao.AliasSourceTab;
            string crorigem = myrelacao.SourceRelField;
            string fieldKey = myrelacao.SourceRelField;

            // EPH is set on a primary-key field
            if (ephArea.Field.ToLower().Equals(myrelacao.TargetRelField))
            {
                object[] safeValues = new object[listaValores.Length];
                for (int i = 0; i < listaValores.Length; i++)
                {
                    safeValues[i] = QueryUtils.ToValidDbValue(listaValores[i], campoEPH);
                }

                if (ephArea.Operator == "EN")
                {
                    CriteriaSet mda = new CriteriaSet(CriteriaSetOperator.Or);

                    if (campoEPH.isKey())
                    {
                        mda.Equal(new ColumnReference(arorigem, crorigem), null);
                    }
                    else
                    {
                        FieldFormatting cFormat = campoEPH.FieldFormat;
                        string funcaoSQL = FieldType.getEPHFunction(cFormat);
                        mda.Equal(SqlFunctions.Custom(funcaoSQL, new ColumnReference(arorigem, crorigem)), 1);
                    }
                    mda.In(arorigem, crorigem, safeValues);
                    cs.SubSet(mda);
                }
                else
                    cs.In(arorigem, crorigem, safeValues);
            }
            // EPH is set on value field (non primary-key)
            else
            {
                if (ephArea.Propagate)
                {
                    // Assume the path between current area and the EPH's area exists
                    arorigem = ephArea.Table;
                    crorigem = ephArea.Field;
                    fieldKey = tabelaEPH.PrimaryKeyName;
                }

                if (ephArea.Operator == "EN")
                {
                    object[] safeValues = new object[listaValores.Length];
                    for (int i = 0; i < listaValores.Length; i++)
                    {
                        safeValues[i] = QueryUtils.ToValidDbValue(listaValores[i], campoEPH);
                    }

                    CriteriaSet mda = new CriteriaSet(CriteriaSetOperator.Or);

                    if (campoEPH.isKey())
                    {
                        mda.Equal(new ColumnReference(arorigem, crorigem), null);
                    }
                    else
                    {
                        crorigem = campoEPH.Name;
                        FieldFormatting cFormat = campoEPH.FieldFormat;
                        string funcaoSQL = FieldType.getEPHFunction(cFormat);
                        
						mda.Equal(SqlFunctions.Custom(funcaoSQL, new ColumnReference(myrelacao.AliasTargetTab, crorigem)), 1);
                    }
                    
                    mda.In(myrelacao.AliasTargetTab, crorigem, safeValues);
					cs.SubSet(mda);
                }
                else
                {
                    //condi��es de OR (Valores ou EMPTY)
                    CriteriaSet mda = new CriteriaSet(CriteriaSetOperator.Or);

                    SelectQuery innerQuery = new SelectQuery()
                        .Select(tabelaEPH.TableName, tabelaEPH.PrimaryKeyName)
                        .From(tabelaEPH.QSystem, tabelaEPH.TableName, tabelaEPH.TableName);

                    CriteriaSet innerConditions = CriteriaSet.Or();

                    // Tree structure EPH
                    if (ephArea.Operator == "L" || ephArea.Operator == "LN")
                    {
                        for (int i = 0; i < listaValores.Length; i++)
                        {
                            // Use the EPH field as a prefix
                            innerConditions.Like(tabelaEPH.TableName, ephArea.Field,
                                listaValores[i].ToString() + "%");
                        }
                        // MH - Eph em �rvore ou NULL
                        if (ephArea.Operator == "LN")
                        {
                            if (campoEPH.isKey())
                            {
                                mda.Equal(new ColumnReference(arorigem, crorigem), null);
                            }
                            else
                            {
                                FieldFormatting cFormat = campoEPH.FieldFormat;
                                string funcaoSQL = FieldType.getEPHFunction(cFormat);
                                mda.Equal(SqlFunctions.Custom(funcaoSQL, new ColumnReference(arorigem, crorigem)), 1);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < listaValores.Length; i++)
                        {
                            innerConditions.Criterias.Add(new Criteria(
                                new ColumnReference(tabelaEPH.TableName, ephArea.Field),
                                QueryUtils.ParseEphOperator(ephArea.Operator),
                                listaValores[i]));
                        }
                    }

                    innerQuery.Where(innerConditions);
                    // The inner query fetches the PKey, not the value field
                    mda.In(arorigem, fieldKey, innerQuery);
                    cs.SubSet(mda);
                }
            }
        }

        private static void AuxAdicionaCondicaoMesmaArea(CriteriaSet cs, Area area, EPHField ephArea, object[] listaValores)
        {
            if (ephArea.Operator == "=" || ephArea.Operator == "EN" )
            {
                //se o operador for "=" podemos usar o IN
                object[] safeValues = new object[listaValores.Length];
                for (int i = 0; i < listaValores.Length; i++)
                {
                    safeValues[i] = QueryUtils.ToValidDbValue(listaValores[i], area.DBFields[ephArea.Field]);
                }
                if (ephArea.Operator == "=")
                    cs.In(area.Alias, ephArea.Field, safeValues);
                else
                {
                    CriteriaSet mda = new CriteriaSet(CriteriaSetOperator.Or);

                    Field campoEPH = area.DBFields[ephArea.Field];
                    if (campoEPH.isKey())
                    {
                        mda.Equal(new ColumnReference(area.Alias, ephArea.Field), null);
                    }
                    else
                    {
                        FieldFormatting cFormat = campoEPH.FieldFormat;
                        string funcaoSQL = FieldType.getEPHFunction(cFormat);
                        mda.Equal(SqlFunctions.Custom(funcaoSQL, new ColumnReference(area.Alias, ephArea.Field)), 1);
                    }
                    mda.In(area.Alias, ephArea.Field, safeValues);
                    cs.SubSet(mda);
                }
            }
            else
            {
                CriteriaSet innerConditions = CriteriaSet.Or();

                if (ephArea.Operator == "L" || ephArea.Operator == "LN")
                {
                    // Use the EPH field as a prefix
                    for (int i = 0; i < listaValores.Length; i++)
                    {
                        innerConditions.Like(area.Alias, ephArea.Field,
                            listaValores[i].ToString() + "%");
                    }
                    // MH - Eph em �rvore ou NULL
                    if(ephArea.Operator == "LN")
                    {
                        Field campoEPH = area.DBFields[ephArea.Field];
                        if (campoEPH.isKey())
                        {
                            innerConditions.Equal(new ColumnReference(area.Alias, ephArea.Field), null);
                        }
                        else
                        {
                            FieldFormatting cFormat = campoEPH.FieldFormat;
                            string funcaoSQL = FieldType.getEPHFunction(cFormat);
                            innerConditions.Equal(SqlFunctions.Custom(funcaoSQL, new ColumnReference(area.Alias, ephArea.Field)), 1);
                        }
                    }
                }
				else if (ephArea.Operator == "EN")
                {
                    object[] safeValues = new object[listaValores.Length];
                    for (int i = 0; i < listaValores.Length; i++)
                    {
                        safeValues[i] = QueryUtils.ToValidDbValue(listaValores[i], area.DBFields[ephArea.Field]);
                    }
                    Field campoEPH = area.DBFields[ephArea.Field];
                    if (campoEPH.isKey())
                    {
                        innerConditions.Equal(new ColumnReference(area.Alias, ephArea.Field), null);
                    }
                    else
                    {
                        FieldFormatting cFormat = campoEPH.FieldFormat;
                        string funcaoSQL = FieldType.getEPHFunction(cFormat);
                        innerConditions.Equal(SqlFunctions.Custom(funcaoSQL, new ColumnReference(area.Alias, ephArea.Field)), 1);
                    }
                    innerConditions.In(area.Alias, ephArea.Field, safeValues);
                }
                else
                {
                    for (int i = 0; i < listaValores.Length; i++)
                    {
                        innerConditions.Criterias.Add(new Criteria(
                            new ColumnReference(area.Alias, ephArea.Field),
                            QueryUtils.ParseEphOperator(ephArea.Operator),
                            listaValores[i]));
                    }
                }

                cs.SubSet(innerConditions);
            }
        }

        /// <summary>
        /// M�todo que preenche o name das colunas da matriz
        /// </summary>
        /// <param name="nomesColunas">string com o name das colunas</param>
        /// <returns>Devolve uma arraylist com os pares (Qfield,area)</returns>
        public Dictionary<string, Area> preencheColunasMatriz_Areas(string columnNames, string module)
        {
            Dictionary<string, Area> areas = new Dictionary<string, Area>();
            Area area = null;
            string[] arrayCampos = columnNames.Split(',');
            for (int i = 0; i < arrayCampos.Length; i++)
            {
                string[] tableField = arrayCampos[i].Split('.');
                if (tableField.Length == 2)
                {

                    if (!areas.ContainsKey(tableField[0]))//se a �rea ainda n�o foi adicionada
                    {
                        //SO 2007.05.29
                        area = Area.createArea(tableField[0], User, module);
                        areas.Add(tableField[0], area);//adicionar a �rea
                    }
                    else
                        area = (Area)areas[tableField[0]];
                }
            }
            return areas;
        }

        /// <summary>
        /// Fun��o to select v�rios registos da BD -GET
        /// </summary>
        /// <param name="identificador"></param>
        /// <param name="condicao"></param>
        /// <returns>uma listagens com os fields</returns>
        public Listing select(string identifier, CriteriaSet condition, int nrRecords)
        {
            try
            {
                Listing Qlisting;
                Type funcObj = typeof(GenioServer.framework.OverrideQuery);
                //public static Listing OVERRIDE_IDENTIFICADOR(Listing Qlisting, CriteriaSet condition, User user, int nrRecords, PersistentSupport sp)
                MethodInfo funcOver = funcObj.GetMethod(identifier);
                if (funcOver != null)
                {
                    object[] parameters = new object[5];
                    parameters[0] = condition;//CriteriaSet
                    parameters[1] = user;//User
                    parameters[2] = sp;//PersistentSupport
                    parameters[3] = nrRecords;//int
                    parameters[4] = this;//Listing

                    Qlisting = (Listing)funcOver.Invoke(this, parameters);
                }
                else
                {
                    if (area == null || area.Information.PersistenceType == PersistenceType.Database || area.Information.PersistenceType == PersistenceType.View)
                    {
                        Qlisting = sp.select(identifier, this, condition, nrRecords, m_noLock);
                    }
                    else //if (area.Information.PersistenceType == PersistenceType.Codebase)
                    {
                        //invocar o metodo estatico searchList por reflection
                        IDictionary<string, PersistentSupport.ControlQueryDefinition> controlos = PersistentSupport.getControlQueries();
                        PersistentSupport.ControlQueryDefinition aux = controlos[identifier];
                        condition.SubSet(aux.WhereConditions);
                        IList res = InvokeSearchList(condition);
                        if (res == null)
                            return null;

                        Qlisting = AreaList2Listagem(res);
                    }
                }
                return Qlisting;
            }
            catch (PersistenceException ex)
            {
                throw new BusinessException(ex.UserMessage, "Listing.seleccionar", "Error selecting database records: " + ex.Message, ex);
            }
            catch (TargetInvocationException ex)
            {
                throw new BusinessException(null, "Listing.seleccionar", "Error selecting database records: " + ex.Message, ex);
                /*if (ex.InnerException is FrameworkException)
                    throw (FrameworkException)ex.InnerException;
                else if (ex.InnerException is BusinessException)
                    throw (BusinessException)ex.InnerException;
                else if (ex.InnerException is PersistenceException)
                    throw (PersistenceException)ex.InnerException;
                else
                    throw ex.InnerException;*/
            }
        }

		public Listing anotherSelect(string identifier, string[] fieldsRequested, IList<ColumnSort> sorts, CriteriaSet condition, int nrRecords,  int offset)
        {
            try
            {
                Listing Qlisting;
                Type funcObj = typeof(GenioServer.framework.OverrideQuery);
                //public static Listing OVERRIDE_IDENTIFICADOR(Listing Qlisting, CriteriaSet condition, User user, int nrRecords, PersistentSupport sp)
                MethodInfo funcOver = funcObj.GetMethod(identifier);
                if (funcOver != null)
                {
                    object[] parameters = new object[5];
                    parameters[0] = condition;//CriteriaSet
                    parameters[1] = user;//User
                    parameters[2] = sp;//PersistentSupport
                    parameters[3] = nrRecords;//int
                    parameters[4] = this;//Listing

                    Qlisting = (Listing)funcOver.Invoke(this, parameters);
                }
                else
                {
                    if (area == null || area.Information.PersistenceType == PersistenceType.Database || area.Information.PersistenceType == PersistenceType.View)
                    {
                        sp.PrepareQuerySelect(identifier, fieldsRequested, sorts, false, nrRecords, offset, area);

                        Qlisting = sp.anotherSelect(identifier, this, condition, nrRecords, offset, area);
                    }
                    else //if (area.Information.PersistenceType == PersistenceType.Codebase)
                    {
                        //invocar o metodo estatico searchList por reflection
                        IDictionary<string, PersistentSupport.ControlQueryDefinition> controlos = PersistentSupport.getControlQueries();
                        PersistentSupport.ControlQueryDefinition aux = controlos[identifier];
                        condition.SubSet(aux.WhereConditions);
                        IList res = InvokeSearchList(condition);
                        if (res == null)
                            return null;

                        Qlisting = AreaList2Listagem(res);
                    }
                }
                return Qlisting;
            }
            catch (PersistenceException ep)
            {
                throw new BusinessException(null, "Listing.anotherSelect", "Error selecting database records: " + ep.Message, ep);
            }
            catch (TargetInvocationException ex)
            {
                throw new BusinessException(null, "Listing.anotherSelect", "Error selecting database records: " + ex.Message, ex);
                /*if (ex.InnerException is FrameworkException)
                    throw (FrameworkException)ex.InnerException;
                else if (ex.InnerException is BusinessException)
                    throw (BusinessException)ex.InnerException;
                else if (ex.InnerException is PersistenceException)
                    throw (PersistenceException)ex.InnerException;
                else
                    throw ex.InnerException;*/
            }
        }

        private IList InvokeSearchList(CriteriaSet condition)
        {
            Type areaType = area.GetType();
            MethodInfo listMethod = areaType.GetMethod("searchList",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new Type[] { typeof(PersistentSupport), typeof(User), typeof(CriteriaSet), typeof(string[]), typeof(bool), typeof(bool) },
                null
                );
            IList res = listMethod.Invoke(null, new object[] {
                            sp,
                            user,
                            condition,
                            fieldsRequested, false, false
                        }) as IList;
            return res;
        }

        private Listing AreaList2Listagem(IList res)
        {
            Listing Qlisting;
            DataTable dt = new DataTable();
            foreach (string campopedido in fieldsRequested)
            {
                string[] tabelacampo = campopedido.Split('.');
                string table = tabelacampo[0];
                string Qfield = tabelacampo[1];

                var cp = Area.GetInfoArea(table).DBFields[Qfield];
                dt.Columns.Add(campopedido, cp.FieldType.Type);
            }

            foreach (IArea row in res)
            {
                DataRow dr = dt.NewRow();
                for (int col = 0; col < fieldsRequested.Length; col++)
                {
                    var cp = row.Fields[fieldsRequested[col]] as RequestedField;
                    dr[col] = cp.Value;
                }
                dt.Rows.Add(dr);
            }

            //ordernar o dataset
            if (ordenacaoQuery.Count > 0)
            {
                DataView dv = new DataView(dt);
                StringBuilder sort = new StringBuilder(dv.Sort);

                foreach (ColumnSort sortCol in QuerySort)
                {
                    string colname = "";
                    if (sortCol.ColumnIndex != null)
                    {
                        int index = (int)sortCol.ColumnIndex - 1;
                        colname = dt.Columns[index].ColumnName;
                    }
                    else if (sortCol.Expression is ColumnReference)
                    {
                        ColumnReference col = (ColumnReference)sortCol.Expression;
                        colname = col.TableAlias + "." + col.ColumnName;
                    }
                    else
                        continue;

                    if (!string.IsNullOrEmpty(sort.ToString()))
                        sort.Append(", ");
                    sort.AppendFormat("{0} {1}", colname, sortCol.Order == SortOrder.Descending ? " DESC" : " ASC");
                }
                dv.Sort = sort.ToString();
                dt = dv.ToTable();
            }

            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            DataMatrix = ds;
            LastFilled = dt.Rows.Count;

            if (obterTotal)
                TotalRecords = dt.Rows.Count;

            Qlisting = this;
            return Qlisting;
        }


        public Listing selectLevel(Area areaLevel, IList<SelectField> fieldsRequested, CriteriaSet condition, string parentKeyCond)
        {
            try
            {
                Listing Qlisting = sp.selectLevel(areaLevel, fieldsRequested, this, condition, parentKeyCond);
                return Qlisting;
            }
            catch (PersistenceException ex)
            {
                throw new BusinessException(ex.UserMessage, "Listing.seleccionarNivel", "Error selecting database records: " + ex.Message, ex);
            }
        }

        public Listing selectMore(string identifier, CriteriaSet condition, int nrRecords, int lastRead, string primaryKey)
        {
            try
            {
                Listing Qlisting;
                Type funcObj = typeof(GenioServer.framework.OverrideQuery);
                MethodInfo funcOver = funcObj.GetMethod(identifier + "_MAIS");
                if (funcOver != null)
                {
                    object[] parameters = new object[7];
                    parameters[0] = condition;//CriteriaSet
                    parameters[1] = user;//User
                    parameters[2] = sp;//PersistentSupport
                    parameters[3] = nrRecords;//int
                    parameters[4] = this;//Listing
                    parameters[5] = lastRead;
                    parameters[6] = primaryKey;

                    Qlisting = (Listing)funcOver.Invoke(this, parameters);
                }
                else
                {
                    if (area == null || area.Information.PersistenceType == PersistenceType.Database || area.Information.PersistenceType == PersistenceType.View)
                    {
                        Qlisting = sp.selectMore(identifier, this, condition, nrRecords, lastRead, primaryKey);
                    }
                    else //if (area.Information.PersistenceType == PersistenceType.Codebase)
                    {
                        //invocar o metodo estatico searchList por reflection
                        IDictionary<string, PersistentSupport.ControlQueryDefinition> controlos = PersistentSupport.getControlQueries();
                        PersistentSupport.ControlQueryDefinition aux = controlos[identifier];
                        condition.SubSet(aux.WhereConditions);
                        IList res = InvokeSearchList(condition);
                        if (res == null)
                            return null;

                        Qlisting = AreaList2Listagem(res);
                    }
                }
                return Qlisting;

            }
            catch (PersistenceException ex)
            {
                throw new BusinessException(ex.UserMessage, "Listing.seleccionarMais", "Error selecting database records: " + ex.Message, ex);
            }
        }


        public int getRecordPos(IArea area, string primaryKeyValue, CriteriaSet conditions, string identifier)
        {
            try
            {
                int posRegisto = sp.getRecordPos(User, Module, area, ordenacaoQuery, primaryKeyValue, conditions, identifier);
                return posRegisto;
            }
            catch (PersistenceException ex)
            {
                throw new BusinessException(ex.UserMessage, "Listing.getRecordPos", "Error getting previous record: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Devolve o elemento que est� a servir de ordena��o
        /// </summary>
        /// <param name="campoOrdenacao">Field de ordena��o</param>
        /// <param name="nrLinha">N�mero da linha pretendida</param>
        /// <returns>Devolve o elemento ordena��o da linha pretendida</returns>
        public object returnElementOrder(string sortingField, int nrLine)
        {
            //verificar se a coluna exists
            if (matrizDados.Tables[0].Columns[sortingField] != null)
            {
                return matrizDados.Tables[0].Rows[nrLine][sortingField];
            }
            else//significa que nao exists coluna de ordena��o
                return null;
        }

        [Obsolete("Use IList<ColumnSort> OrdenacaoQuery instead")]
        public string Sort
        {
            get { return sorting; }
            set { sorting = value; }
        }

        public IList<ColumnSort> QuerySort
        {
            get { return ordenacaoQuery; }
            set { ordenacaoQuery = value; }
        }


        public User User
        {
            get { return user; }

        }


        public CriteriaSet EphQueryConditions
        {
            get { return condicoesEphQuery; }
            set { condicoesEphQuery = value; }
        }

        public DataSet DataMatrix
        {
            get { return matrizDados; }
            set { matrizDados = value; }
        }

        /// <summary>
        /// M�todo que devolve ou coloca o n�mero da �ltima linha preenchida
        /// </summary>
        public int LastFilled
        {
            get { return ultPreenchida; }
            set { ultPreenchida = value; }
        }

        /// <summary>
        /// M�todo que devolve o module
        /// </summary>
        public string Module
        {
            get { return module; }
        }

        /// <summary>
        /// RS(29.05.2007) Modifica o size da matriz subjacente da lista
        /// </summary>
        /// <param name="numLinhas"></param>
        /*public void ResetNrRegistos(int numLinhas)
        {
            if (this.nrLinhas >= numLinhas)
                return; //a matriz ja tem o size q precisamos

            this.nrLinhas = numLinhas;
            matrizDados = new object[numLinhas, nrColunas];
        }*/
    }

    public class ListingMVC<A>
    {
        private const int DefaultNumRegs = 10;

        private List<A> elements;

        private int numRegs;

        private int offset;

        private FieldRef[] fields;

        private bool distinct;

        private bool noLock;

        private User user;

        private string rowselect;

		private CriteriaSet pagingposEPHs;

        private int currentpage;

        public string identifier;

        public int totalRegistos;

        public IList<TableJoin> Joins { get; set; }

        public User User
        {
            get { return user; }
        }

        public bool Distinct
        {
            get { return distinct; }
        }

        public bool NoLock
        {
            get { return noLock; }
            set { noLock = value; }
        }

        public FieldRef[] RequestFields
        {
            get { return fields; }
        }

        public string[] RequestFieldsAsStringArray
        {
            get {
                string[] array = new string[fields.Length];
                int index = 0;
                foreach(FieldRef f in fields)
                    array[index++] = f.FullName;
                return array;
            }
        }

        private IList<ColumnSort> sorts;

        public IList<ColumnSort> Sorts
        {
            get { return sorts; }
        }

        public CriteriaSet PagingPosEPHs
        {
            get { return pagingposEPHs; }
            set { pagingposEPHs = value; }
        }

        public ListingMVC(FieldRef[] fields, IList<ColumnSort> sorts, int offset, int numRegs, bool distinct, User user, bool noLock, string identifier = "", bool getTotal = false, string selectrow ="", CriteriaSet pagingPosEPHs = null)
        {
            this.fields = fields;
            this.sorts = sorts;
            this.offset = offset;
            if (numRegs == 0)
                this.numRegs = DefaultNumRegs;
            else
                this.numRegs = numRegs;
            this.distinct = distinct;
            this.user = user;
            this.noLock = noLock;
            this.identifier = identifier;
            this.getTotal = getTotal;
            this.rowselect = selectrow;
            this.pagingposEPHs = pagingPosEPHs;
        }

        public int Offset
        {
            get { return offset; }
            set { this.offset = value; }
        }

        public string RowSelect
        {
            get { return rowselect; }
            set { this.rowselect = value; }
        }

        public int CurrentPage
        {
            get { return currentpage; }
            set { this.currentpage = value; }
        }

        public int NumRegs
        {
            get { return numRegs; }
            set { this.numRegs = value; }
        }

        public List<A> Rows
        {
            get { return elements; }
            set { this.elements = value; }
        }

        protected bool getTotal;
        /// <summary>
        /// Indicates whether we want to know the total number of records
        /// </summary>
        public bool GetTotal
        {
            get { return getTotal; }
        }

        public int TotalRecords
        {
            get { return totalRegistos; }
            set { this.totalRegistos = value; }
        }

        /// <summary>
        /// Indicates if exist more pages
        /// One more record is always selected to check if exist more pages
        /// </summary>
        public bool HasMore {
            get {
				return (Rows != null && numRegs != -1) ? Rows.Count > NumRegs : false;
            }
        }

        public List<T> RowsForViewModel<T>() where T : new()
        {
            return RowsForViewModel<T>(false);
        }

        public List<T> RowsForViewModel<T> (bool fillAreasRel) where T: new() {
            return RowsForViewModel<T>(fillAreasRel, null);
        }

        public List<T> RowsForViewModel<T>(bool fillAreasRel, string[] _fieldsToSerialize) where T : new()
        {
            int count = (numRegs > 0 && elements.Count > numRegs) ? numRegs : elements.Count;

            List<T> list = new List<T>(count);
            Type type = typeof(T);

            T[] array = new T[count];
            for (int i = 0; i < count; i++)
            {
               var args = new object[] { elements[i] };
                if (fillAreasRel)
                    args = new object[] { elements[i], true };
                if (_fieldsToSerialize != null)
                    args = new object[] { elements[i], true, _fieldsToSerialize };
                array[i] = (T)Activator.CreateInstance(type, args);
            }
            list.AddRange(array);

            return list;
        }

        public List<T> RowsForViewModel<T>(Func<A, T> constructor)
        {
			int count = (elements != null) ? ((numRegs > 0 && elements.Count > numRegs) ? numRegs : elements.Count) : 0;
            List<T> list = new List<T>(count);

            for (int i = 0; i < count; i++)
                list.Add(constructor(elements[i]));

            return list;
        }
    }
}
