using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;
using System.Collections.Generic;
using System.Collections;

namespace CSGenio.business
{
    /// <summary>
    /// Aggregates the sources of formulas for each area in order to minimize the number of querys needed
    /// to calculate those formulas
    /// </summary>
    public class FormulaDbContext
    {
        Dictionary<string, HashSet<string>> camposPorArea = new Dictionary<string, HashSet<string>>();
        Dictionary<string, string> chavePorArea = new Dictionary<string, string>();
        Dictionary<string, Area> areasLidas = new Dictionary<string, Area>();
        Area areaBase;
        /// <summary>
        /// Creates a new FormulaDbContext
        /// </summary>
        /// <param name="area">The base area for the context</param>
        public FormulaDbContext(Area area)
        {
            areaBase = area;
        }

        public void AddFormulaSources(InternalOperationFormula f)
        {
            foreach (var arg in f.ByAreaArguments)
            {
                if (!camposPorArea.ContainsKey(arg.AliasName))
                {
                    camposPorArea.Add(arg.AliasName, new HashSet<string>());
                    chavePorArea.Add(arg.AliasName, arg.KeyName);
                }
                foreach (var c in arg.FieldNames)
                    camposPorArea[arg.AliasName].Add(c);
            }
        }

        /// <summary>
        /// Makes the context aware of all the default formula sources
        /// </summary>
        public void AddDefaults()
        {
            if (areaBase.DefaultValues != null)
            {
                string[] valoresDefault = areaBase.DefaultValues;
                for (int i = 0; i < valoresDefault.Length; i++)
                {
                    Field Qfield = (Field)areaBase.DBFields[valoresDefault[i]];
                    if (Qfield.DefaultValue.tpDefault == DefaultValue.DefaultType.OP_INT)
                    {
                        var f = Qfield.DefaultValue.DefaultFormula as InternalOperationFormula;
                        AddFormulaSources(f);
                    }
                }
            }
        }

        /// <summary>
        /// Makes the coxtext aware of all the arithmetic formula sources
        /// </summary>
        public void AddInternalOperations()
        {
            foreach (Field Qfield in areaBase.Information.DBFieldsList)
            {
                if (Qfield.Formula is InternalOperationFormula)
                {
                    var f = Qfield.Formula as InternalOperationFormula;
                    AddFormulaSources(f);
                }
            }
        }


        /// <summary>
        /// Makes the context aware of all the replica formula sources
        /// </summary>
        public void AddReplicas()
        {
            foreach (Field Qfield in areaBase.Information.DBFieldsList)
            {
                if (Qfield.Formula is ReplicaFormula)
                {
                    var f = Qfield.Formula as ReplicaFormula;
                    var _tabela = f.Alias;
                    if (!camposPorArea.ContainsKey(f.Alias))
                    {
                        camposPorArea.Add(f.Alias, new HashSet<string>());
                        chavePorArea.Add(f.Alias, areaBase.ParentTables[f.Alias].SourceRelField); //TODO: Here we can use the CE of the relationship
                    }
                    camposPorArea[f.Alias].Add(f.Field);
                }
            }
        }

        /// <summary>
        /// Get the data from an area by its physical relationship, if already cached returns the the data
        /// </summary>
        /// <param name="rel">The relationship you want to get data from</param>
        /// <param name="sp">Persistent support with the current transaction</param>
        /// <param name="u">The User</param>
        /// <returns>The area with the formula source fields filled</returns>
        public Area GetRelation(string rel, PersistentSupport sp, User u)
        {
            if (!areasLidas.ContainsKey(rel))
            {
                Relation relacao = areaBase.ParentTables[rel];
                string area = relacao.AliasTargetTab;
                Area a = Area.createArea(area, u, u.CurrentModule);
                string nomeChave = chavePorArea[rel];
                string valorChaveEst = GetValorChaveEstrangeira(nomeChave, sp, a);

                //If the foreign key is in memory or in BD (already in memory)
                if (valorChaveEst != "")
                {
                    //Query to go fetch the values of the fields
                    var fields = new string[camposPorArea[rel].Count];
                    camposPorArea[rel].CopyTo(fields, 0);
                    sp.getRecord(a, valorChaveEst, fields);
                }
                //else values are empty

                //Caching this joint read
                areasLidas.Add(rel, a);
            }
            return areasLidas[rel];
        }

        /// <summary>
        /// Gets the data from an area, if already cached returns that data
        /// </summary>
        /// <param name="area">The area to which you want to fetch data</param>
        /// <param name="sp">Persistent support with the current transaction</param>
        /// <param name="u">The User</param>
        /// <returns>The area with the formula source fields filled</returns>
        public Area GetArea(string area, PersistentSupport sp, User u)
        {
            if (!areasLidas.ContainsKey(area))
            {
                Area a = Area.createArea(area, u, u.CurrentModule);
                string nomeChave = chavePorArea[area];
                string valorChaveEst = GetValorChaveEstrangeira(nomeChave, sp, a);

                //If the foreign key is in memory or in BD (already in memory)
                if (valorChaveEst != "")
                {
                    //Query to go fetch the values of the fields
                    var fields = new string[camposPorArea[area].Count];
                    camposPorArea[area].CopyTo(fields, 0);
                    sp.getRecord(a, valorChaveEst, fields);
                }
                //else values are empty

                //Caching this joint reading
                areasLidas.Add(area, a);
            }
            return areasLidas[area];
        }

        /// <summary>
        /// Caches the data from an area.
        /// </summary>
        /// <param name="area">The area.</param>
        public void SetArea(Area area)
        {
            if (!areasLidas.ContainsKey(area.Alias))
                areasLidas.Add(area.Alias, area);
            else
                areasLidas[area.Alias] = area;
        }

		// Cache the Glob internal code (it should never change)
		private static string m_codglob = null;
        private static object m_codglobLock = new object();

        private string GetValorChaveEstrangeira(string nomeChave, PersistentSupport sp, Area a)
        {
            //Go fetch the primary key of the table to which the fields belong
            //Verify that the areaField contains the foreign key that corresponds to the primary key
            //field we're looking for, if it doesn't count then you have to go read the database
            string valorChaveEst;
            
            if (a.Alias == "glob")
            {
                lock(m_codglobLock)
                {
                    if (m_codglob == null)
                    {
                        //Go fetch the value of the primary key from the GLOB table
                        SelectQuery qs = new SelectQuery()
                            .Select(a.TableName, a.PrimaryKeyName)
                            .From(a.QSystem, a.TableName, a.TableName)
                            .PageSize(1);

                        object Qresult = sp.ExecuteScalar(qs);
                        if (Qresult != null)
                            m_codglob = DBConversion.ToKey(Qresult);
                        else
                            throw new BusinessException(null, "FormulaDbContext.GetArea", "No record found in glob.");
                    }
					valorChaveEst = m_codglob;
                }
            }
            else
            {
                // AV (2010/06/04) The fields that were erased in the forms were to be overlapped with the value in the DB so we have to test whether the field is in memory
                if (areaBase.Fields[areaBase.Alias + "." + nomeChave] == null)//If the value is not in memory go read to BD
                {
                    // TODO: The code should never have to come through here, if it passes we may have a problem of efficiency. You Must always read all the fields at once to the head. Review this if block.
                    string codIntValue = (string)areaBase.returnValueField(areaBase.Alias + "." + areaBase.PrimaryKeyName, FieldFormatting.CARACTERES);
                    if (codIntValue == "")
                        codIntValue = null;
                    valorChaveEst = DBConversion.ToKey(sp.returnField(areaBase, nomeChave, codIntValue));
                    areaBase.insertNameValueField(areaBase.Alias + "." + nomeChave, valorChaveEst);
                }
                else
                    valorChaveEst = (string)areaBase.returnValueField(areaBase.Alias + "." + nomeChave, FieldFormatting.CARACTERES);
            }

            return valorChaveEst;
        }
    }
}
