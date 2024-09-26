using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;

namespace CSGenio.business
{
    /// <summary>
    /// Classe m�e que caracteriza uma formula
    /// </summary>
    public abstract class Formula
    {
        public object[] returnValueFieldsInternalFormula(Area areaField, List<ByAreaArguments> argumentsListByArea, PersistentSupport sp, FormulaDbContext fdc, int nrArguments, FunctionType tpFunction)
        {
            try
            {
                object[] fieldsValue = new object[nrArguments];
                Area area = null;
                foreach (ByAreaArguments argumentosPorArea in argumentsListByArea)
                {
                    if (argumentosPorArea.AliasName.Equals(areaField.Alias)) //se � a propria area
                    {
                        //ir buscar a key prim�ria
                        string codIntValue = areaField.QPrimaryKey;
                        if (codIntValue == "")
                            throw new BusinessException(null, "Formula.devolverValorCamposFormulaInterna", "ChavePrimaria is null.");

                        //descobrir que fields n�o est�o em mem�ria
                        List<string> selectBD = new List<string>();
                        for (int i = 0; i < argumentosPorArea.FieldNames.Length; i++)
                        {
                            object campoObj = areaField.Fields[areaField.Alias + "." + argumentosPorArea.FieldNames[i]];
                            if (campoObj == null) //se o Qfield n�o est� em mem�ria
                                selectBD.Add(argumentosPorArea.FieldNames[i]);
                        }

                        //ir buscar esses Qfield to esta �rea
                        if (selectBD.Count > 0 && (tpFunction == FunctionType.INS || tpFunction == FunctionType.DUP))
                            sp.getRecord(areaField, codIntValue, selectBD.ToArray()); //<----- Em que situa��es pode o codigo chegar aqui?

                        //agora j� podemos assumir que os fields est�o em memoria
                        for (int i = 0; i < argumentosPorArea.FieldNames.Length; i++)
                            fieldsValue[argumentosPorArea.FieldsPosition[i]] = areaField.returnValueField(areaField.Alias + "." + argumentosPorArea.FieldNames[i]);

                    }
                    else //� outra �rea
                    {
						//Aqui j� s� tenho de ler o que foi preenchido pelo FormulaDbContext, se estiver vazia usamos os Qvalues nulos to as sources das formulas
						area = fdc.GetArea(argumentosPorArea.AliasName, sp, areaField.User);
                        for (int i = 0; i < argumentosPorArea.FieldNames.Length; i++)
                            fieldsValue[argumentosPorArea.FieldsPosition[i]] = area.returnValueField(area.Alias + "." + argumentosPorArea.FieldNames[i]);
                    }
                }

                return fieldsValue;
            }
            catch (GenioException ex)
			{
				if (ex.ExceptionSite == "Formula.devolverValorCamposFormulaInterna")
					throw;
				throw new BusinessException(ex.UserMessage, "Formula.devolverValorCamposFormulaInterna", "Error getting field values in internal formula: " + ex.Message, ex);
			}
            catch (Exception ex)
            {
                throw new BusinessException(null, "Formula.devolverValorCamposFormulaInterna", "Error getting field values in internal formula: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// M�todo to obter os Qvalues necess�rios nas opera��es internas
        /// </summary>
        /// <param name="areaCampo">area do Qfield</param>
        /// <param name="argumentosPorArea">argumentosPorArea</param>
        /// <param name="sp">Suporte Persistente</param>
        /// <returns></returns>
        //[Obsolete("Please use the overload that uses FormulaDbContext for a more efficient calculation")]
        public object[] returnValueFieldsInternalFormula(Area areaField, List<ByAreaArguments> argumentsListByArea, PersistentSupport sp, int nrArguments, FunctionType tpFunction)
        {
            try
            {
                object[] fieldsValue = new object[nrArguments];
                Area area = null;
                FieldFormatting formField = FieldFormatting.CARACTERES;
                foreach (ByAreaArguments argumentosPorArea in argumentsListByArea)
                {
                    if (!argumentosPorArea.AliasName.Equals(areaField.Alias))
                        area = Area.createArea(argumentosPorArea.AliasName, areaField.User, areaField.Module);
                    else
                        area = null;

                    if (area == null)//se � a propria area
                    {
                        //ir buscar a key prim�ria
                        string codIntValue = areaField.QPrimaryKey;
                        if (codIntValue == "")
							throw new BusinessException(null, "Formula.devolverValorCamposFormulaInterna", "ChavePrimaria is null.");

                        //descobrir que fields n�o est�o em mem�ria
                        List<string> selectBD = new List<string>();
                        for (int i = 0; i < argumentosPorArea.FieldNames.Length; i++)
                        {
                            object campoObj = areaField.Fields[areaField.Alias + "." + argumentosPorArea.FieldNames[i]];
                            if (campoObj == null) //se o Qfield n�o est� em mem�ria
                                selectBD.Add(argumentosPorArea.FieldNames[i]);
                        }

                        //ir buscar esses Qfield to esta �rea
                        if (selectBD.Count > 0 && (tpFunction == FunctionType.INS || tpFunction == FunctionType.DUP))
                            sp.getRecord(areaField, codIntValue, selectBD.ToArray());

                        //agora j� podemos assumir que os fields est�o em memoria
                        for (int i = 0; i < argumentosPorArea.FieldNames.Length; i++)
                            fieldsValue[argumentosPorArea.FieldsPosition[i]] = areaField.returnValueField(areaField.Alias + "." + argumentosPorArea.FieldNames[i]);

                    }
                    else if ((area.Alias.ToUpper()).Equals("GLOB"))//se for a table glob
                    {
                        //query to ir buscar os Qvalues dos fields
                        SelectQuery qs = new SelectQuery();
                        foreach (string field in argumentosPorArea.FieldNames)
                        {
                            qs.Select(area.TableName, field);
                        }
                        qs.From(area.QSystem, area.TableName, area.TableName)
                            //.Where(CriteriaSet.And()
                                //.Equal(area.TableName, area.PrimaryKeyName, codIntValue))
                                .PageSize(1);

                        ArrayList fieldsvalues = sp.executeReaderOneRow(qs);
                        if(fieldsValue.Length == 0)
                            throw new BusinessException(null, "Formula.devolverValorCamposFormulaInterna", "No record found in glob.");

                        for (int i = 0; i < argumentosPorArea.FieldNames.Length; i++)
                        {
                            formField = ((Field)area.DBFields[argumentosPorArea.FieldNames[i]]).FieldFormat;
                            //TSX (2008-10-30) fieldsValue[argumentosPorArea.FieldsPosition[i]] = Conversion.internal2InternalValid(fieldsvalues[i], formField);
                            //prever o caso em que a query nao encontrou nenhum registo
                            if (fieldsvalues.Count == 0)
                                fieldsValue[argumentosPorArea.FieldsPosition[i]] = Field.GetValorEmpty(formField);
                            else
                                fieldsValue[argumentosPorArea.FieldsPosition[i]] = DBConversion.ToInternal(fieldsvalues[i], formField);
                        }

                    }
                    else if (!area.Alias.Equals(areaField.Alias))
                    {
                        //ir buscar a key prim�ria da table a que os fields pertencem
                        //verificar se o areaField contem a key estrangeira que corresponde � key
                        //prim�ria do Qfield que estamos a procurar se n�o contem entao tem de se ir ler � base de dados
                        string valorChaveEst;
                        //AV(2010/06/04) Os fields que eram apagados nos forms estavam a ser sobrepostos com o Qvalue na db por isso temos que testar se o Qfield est� em mem�ria
                        if (areaField.Fields[areaField.Alias + "." + argumentosPorArea.KeyName] == null)//se o Qvalue n�o est� em mem�ria ir ler � BD
                        {
                            string codIntValue = (string)areaField.returnValueField(areaField.Alias + "." + areaField.PrimaryKeyName, FieldFormatting.CARACTERES);
                            valorChaveEst = DBConversion.ToKey(sp.returnField(areaField, argumentosPorArea.KeyName, codIntValue));
                            areaField.insertNameValueField(areaField.Alias + "." + argumentosPorArea.KeyName, valorChaveEst);
                        }
                        else
                            valorChaveEst = (string)areaField.returnValueField(areaField.Alias + "." + argumentosPorArea.KeyName, FieldFormatting.CARACTERES);
                        if (valorChaveEst != "")//se a key estrangeira est� em mem�ria ou na BD (j� est� em mem�ria)
                        {
                            //query to ir buscar os Qvalues dos fields
                            sp.getRecord(area, valorChaveEst, argumentosPorArea.FieldNames);
                            //quando o registo n�o consegue ser posicionado os Qvalues est�o a nulo
                            for (int i = 0; i < argumentosPorArea.FieldNames.Length; i++)
                                fieldsValue[argumentosPorArea.FieldsPosition[i]] = area.returnValueField(area.Alias + "." + argumentosPorArea.FieldNames[i]);
                             
                        }
                        else
                        {
                            for (int i = 0; i < argumentosPorArea.FieldNames.Length; i++)
                            {
                                formField = ((Field)area.DBFields[argumentosPorArea.FieldNames[i]]).FieldFormat;
                                fieldsValue[argumentosPorArea.FieldsPosition[i]] = Field.GetValorEmpty(formField);
                            }
                        }
                    }
                }

                return fieldsValue;

            }
            catch (GenioException ex)
			{
				if (ex.ExceptionSite == "Formula.devolverValorCamposFormulaInterna")
					throw;
				throw new BusinessException(ex.UserMessage, "Formula.devolverValorCamposFormulaInterna", "Error getting field values in internal formula: " + ex.Message, ex);
			}
            catch (Exception ex)
            {
				throw new BusinessException(null, "Formula.devolverValorCamposFormulaInterna", "Error getting field values in internal formula: " + ex.Message, ex);
            }
        }
    }

    /// <summary>
    /// Classe que representa um argumento duma formula de opera��o interna ou formula condi��o
    /// </summary>
    public class ByAreaArguments
    {
        private string nomeAlias;//name da �rea a que pertencem os fields que s�o argumentos da fun��o
        private string nomeChave; //name da key prim�ria to a table dos fields que s�o argumentos 
        private int[] posicaoCampos; //posi��o dos fields que s�o argumentos da fun��o 
        private string[] fieldNames; //name dos fields que s�o argumentos da fun��o 

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="nomeCampos">Name dos fields que s�o argumentos</param>
        /// <param name="f">fun��o que corresponde � f�rmula</param>
        public ByAreaArguments(string[] fieldNames, int[] posicaoCampos, string nomeAlias, string nomeChave)
        {
            this.nomeAlias = nomeAlias;
            this.posicaoCampos = posicaoCampos;
            this.fieldNames = fieldNames;
            this.nomeChave = nomeChave;
        }


        /// <summary>
        /// Name dos fields que servem de argumentos
        /// </summary>
        public string[] FieldNames
        {
            get { return fieldNames; }
            set { fieldNames = value; }
        }

        /// <summary>
        /// Posicao dos fields que servem de argumentos
        /// </summary>
        public int[] FieldsPosition
        {
            get { return posicaoCampos; }
            set { posicaoCampos = value; }
        }

        /// <summary>
        /// Name do alias da table dos fields que servem de argumentos
        /// </summary>
        public string AliasName
        {
            get { return nomeAlias; }
            set { nomeAlias = value; }
        }

        /// <summary>
        /// Name da key da table dos fields que s�o argumentos 
        /// </summary>
        public string KeyName
        {
            get { return nomeChave; }
            set { nomeChave = value; }
        }
    }
}
