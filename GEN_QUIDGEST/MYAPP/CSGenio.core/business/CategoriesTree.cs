using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Text;
using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;

/// <summary>
/// Classe que representa as regras de construção de uma arvore de categrias. Pode ser vista como um construtor de uma arvore.
/// 
/// </summary>

namespace CSGenio.business
{
    public abstract class CategoriesTree
    {
        protected static Hashtable todasArvores;

        static CategoriesTree()
        {
            todasArvores = new Hashtable();
        }

        public static CategoriesTree createTree(string name, User user)
        {
            switch ((int)todasArvores[name])
            {
                default:
                    throw new BusinessException(null, "CategoriesTree.criarArvore", "Error creating the Categories Tree: " + name);
            }
        }

        public abstract Hashtable Tree
        {
            get;
        }

        public abstract Hashtable LevelsAreas
        {
            get;
        }

        public abstract string StringTree
        {
            get;
            set;
        }

        public abstract int LevelsCount
        {
            get;
        }

        public abstract int ViewLevel
        {
            get;
        }

        public abstract string Module
        {
            get;
        }

        [Obsolete("Use SelectQuery GenericQueryNivel0 instead")]
        public abstract QuerySelect QueryLevel0
        {
            get;
            set;
        }

        public abstract SelectQuery GenericQueryLevel0
        {
            get;
            set;
        }

        public abstract string[] NameField
        {
            get;
        }

        /// <summary>
        /// Metodo to construir a arvore (hashtable com os niveis e respectivos nodos)
        /// </summary>
        /// <returns>void</returns>
        public void buildTree(string Qyear, PersistentSupport sp)
        {
            this.Tree.Add(0, constroiPrimeiroNivel(Qyear, sp));

            for (int k = 1; k < this.LevelsAreas.Count; k++)
            {
                TreeLevel nivelAnterior = (TreeLevel)this.Tree[k - 1];
                TreeLevel novoNivel = new TreeLevel(nivelAnterior.TotalNodes, k);
                int i = 0;
                for (int j = 0; j < novoNivel.CountryNumber; j++)
                {
                    Hashtable Qresult = constroiFilhos(k, nivelAnterior.SearchNode(j), Qyear, sp);

                    IEnumerator enumera = Qresult.Values.GetEnumerator();

                    while (enumera.MoveNext())
                    {
                        novoNivel.AddNode((Nodes)enumera.Current, i);
                        i++;
                    }
                }
                this.Tree.Add(k, novoNivel);

            }
            ordenaArvore();
            buildStringTree();
        }

        /// <summary>
        /// Metodo to ordenar os nodos de cada level pelo number de filhos.
        /// </summary>
        /// <returns>void</returns>
        private void ordenaArvore()
        {
            IEnumerator enumera = this.Tree.Values.GetEnumerator();

            while (enumera.MoveNext())
            {
                TreeLevel level = (TreeLevel)enumera.Current;
                if (level.TotalNodes != 0)
                {
                    level.sortLevel();
                }
            }
        }

        /// <summary>
        /// Metodo to construir os nodos filhos de um determinado node
        /// </summary>
        /// <param name="nivel">level do node filho</param>
        /// <param name="nodo_pai">node pai</param>
        /// <returns>Hashtable com os nodos filhos do node pai</returns>
        private Hashtable constroiFilhos(int level, Nodes nodo_pai, string Qyear, PersistentSupport sp)
        {
            Area nivelAnterior = (Area)LevelsAreas[level - 1];
            Area area = (Area)LevelsAreas[level];
            Relation rel = (Relation)area.ParentTables[nivelAnterior.Alias];

            //SO 20061211 alteração do constructor da classe QuerySelect
            SelectQuery querySel = new SelectQuery()
                .Select(area.TableName, area.PrimaryKeyName)
                .Select(area.TableName, NameField[level])
                .From(area.QSystem, area.TableName, area.TableName)
                .Where(CriteriaSet.And()
                    .Equal(area.TableName, rel.SourceRelField, nodo_pai.Code));

            DataMatrix ds = sp.Execute(querySel);
            Hashtable Qresult = new Hashtable();

            for (int k = 0; k < ds.NumRows; k++)
            {
                string codigo = ds.GetString(k, area.TableName + "." + area.PrimaryKeyName);
                string name = ds.GetString(k, area.TableName + "." + NameField[level]);

                Nodes node = new Nodes(name, codigo, level);

                nodo_pai.AddBranch(k, node);
                Qresult.Add(k, node);
            }

            return Qresult;
        }

        /// <summary>
        /// Metodo to construir o primeiro level da arvore
        /// </summary>
        /// <returns>TreeLevel primeiro level da arvore</returns>
        private TreeLevel constroiPrimeiroNivel(string Qyear, PersistentSupport sp)
        {
            Area area = (Area)this.LevelsAreas[0];

            DataMatrix ds = sp.Execute(this.GenericQueryLevel0);

            TreeLevel primeiroNivel = new TreeLevel(0, 0);
            for (int k = 0; k < ds.NumRows; k++)
            {
                string codigo = ds.GetString(k, area.TableName + "." + area.PrimaryKeyName);
                string name = ds.GetString(k, area.TableName + "." + NameField[0]);

                Nodes node = new Nodes(name, codigo, 0);
                primeiroNivel.AddNode(node, k);

            }
            return primeiroNivel;
        }

        /// <summary>
        /// Metodo to por condições EPH e outras.
        /// </summary>
        /// <returns>void</returns>
        [Obsolete("Use void poeCondicao(User utilizador, CriteriaSet condicao) instead")]
        public void putCondition(User user, string condition)
        {
            Area area = (Area)this.LevelsAreas[0];

            string module = this.Module;
            //So 2007.05.31 alterei to passar a usar a classe LevelAccess
            string level = user.LevelModules[module].ToString();
            if (area.Ephs != null)//adicionar as condições das ephs se existirem
            {
                object ephsAreaObj = area.Ephs[new Par(module, level)];
                if (ephsAreaObj != null)
                {
                    string[] ephsArea = (string[])ephsAreaObj;
                    Hashtable ephsUtilizador = user.Ephs;
                    for (int i = 0; i < ephsArea.Length; i++)
                        if (ephsUtilizador.ContainsKey(module + "_" + ephsArea[i]))
                        {
                            if (condition.Equals(""))
                            {
                                condition = ephsUtilizador[module + "_" + ephsArea[i]].ToString();
                            }
                            else
                            {
                                condition += " AND " + ephsUtilizador[module + "_" + ephsArea[i]].ToString();
                            }
                        }
                }
            }

            this.QueryLevel0.addWhere(condition, "");
        }

        public void putCondition(User user, CriteriaSet condition)
        {
            QueryUtils.addWhere(this.GenericQueryLevel0, condition, CriteriaSetOperator.And);
        }

        /// <summary>
        /// Metodo to construir a string que representa a arvore.
        /// </summary>
        /// <returns>void</returns>
        public void buildStringTree()
        {
            string strArvore = "";

            TreeLevel level = (TreeLevel)this.Tree[0];

            for (int k = 0; k < level.TotalNodes; k++)
            {
                Nodes node = level.SearchNode(k);

                if (!node.HasBranches())
                {
                    strArvore += node.Code + "[";

                    for (int j = 1; j < this.LevelsCount; j++)
                    {
                        strArvore += "[";
                    }

                    strArvore += node.Name + "[" + node.QLevel + "{";

                }
                else
                {
                    for (int j = 0; j < this.LevelsCount; j++)
                    {
                        strArvore += "[";
                    }
                    strArvore += node.Name + "[" + node.QLevel + "{";
                    for (int i = 0; i < node.TotalBranches; i++)
                    {
                        strArvore += StringNiveisInferiores(node.searchBranch(i));
                    }
                }
            }

            int comp = strArvore.Length - 1;
            if (strArvore.EndsWith("{"))
                strArvore = strArvore.Remove(comp);
            this.StringTree = strArvore;
        }

        /// <summary>
        /// Metodo (auxiliar) to construir a string que representa a arvore.
        /// </summary>
        /// <returns>string string que representa um level inferior</returns>
        private string StringNiveisInferiores(Nodes node)
        {
            string constroi = "";

            if (!node.HasBranches())
            {
                for (int j = 0; j < node.QLevel; j++)
                {
                    constroi += "[";
                }

                constroi += node.Code;

                for (int k = node.QLevel; k < this.LevelsCount; k++)
                {
                    constroi += "[";
                }

                constroi += node.Name + "[" + node.QLevel + "{";
            }
            else
            {
                for (int m = 0; m < this.LevelsCount; m++)
                {
                    constroi += "[";
                }
                constroi += "[" + node.Name + "[" + node.QLevel + "{";
                for (int i = 0; i < node.TotalBranches; i++)
                {
                    constroi += StringNiveisInferiores(node.searchBranch(i));
                }
            }
            return constroi;
        }
    }
}
