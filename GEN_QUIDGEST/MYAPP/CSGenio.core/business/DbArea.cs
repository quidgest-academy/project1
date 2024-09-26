using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

using CSGenio.core.messaging;
using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence;
using Quidgest.Persistence.GenericQuery;

namespace CSGenio.business
{
    /// <summary>
    /// Dados de um file na BD
    /// Name, Extensão, Versão, byte[] com o conteudo do file e o size
    /// </summary>
	public class DBFile
	{
        /// <summary>
        /// Name do file
        /// </summary>
		public string Name { get; private set; }
        /// <summary>
        /// Extensão do file
        /// </summary>
		public string Extension { get; private set; }
        /// <summary>
        /// Versão do file
        /// </summary>
		public string Version { get; private set; }
        /// <summary>
        /// Conteúdo do file
        /// </summary>
		public byte[] File { get; private set; }
        /// <summary>
        /// Size do file
        /// </summary>
		public int Size { get; private set; }
		/// <summary>
        /// Author
        /// </summary>
        public string Author { get; private set; }
        /// <summary>
        /// Date of creation
        /// </summary>
        public string CreatedAt { get; private set; }
        /// <summary>
        /// Is the document checked out?
        /// </summary>
        public bool IsCheckout { get; private set; }
        /// <summary>
        /// Document id
        /// </summary>
        public string DocumId { get; private set; }
        /// <summary>
        /// Document id
        /// </summary>
        public string Coddocums { get; private set; }
        /// <summary>
        /// Current user
        /// </summary>
        public string CurrentUser { get; private set; }
        /// <summary>
        /// Name of the editor
        /// </summary>
        public string CheckoutEditor { get; private set; }
        /// <summary>
        /// List of all versions of the document
        /// </summary>
        public SortedList<String, String> Versions { get; private set; }
        /// <summary>
        /// Is an empty file?
        /// </summary>
        public bool IsEmptyFile { get; private set; }

		public DBFile(string name, string extension, string Qversion, byte[] file, int size)
		{
			this.Name = name;
			this.Extension = extension;
			this.Version = Qversion;
			this.File = file;
			this.Size = size;
            this.IsEmptyFile = false;
		}

        public DBFile(string name, string extension, string Qversion, string filepath, int size)
        {
            this.Name = name;
            this.Extension = extension;
            this.Version = Qversion;
            this.File = System.IO.File.ReadAllBytes(System.IO.Path.Combine(Configuration.PathDocuments, filepath));
            this.Size = size;
            this.IsEmptyFile = false;
        }

        public DBFile(string coddocums, string name, string extension, string Qversion, int size, string author, string createdAt, string documId, SortedList<String, String> versions, bool isCheckout, string checkoutEditor, string currentUser)
        {
            this.Coddocums = coddocums;
			this.Name = name;
			this.Extension = extension;
			this.Version = Qversion;
			this.Size = size;
            this.Author = author;
            this.CreatedAt = createdAt;
            this.DocumId = documId;
            this.Versions = versions;
            this.IsCheckout = isCheckout;
            this.CheckoutEditor = checkoutEditor;
            this.CurrentUser = currentUser;
            this.IsEmptyFile = false;
		}

        public DBFile()
        {
            this.IsEmptyFile = true;
        }

        public string InfoDocQweb(User user)
        {
            if (IsEmptyFile)
                return "";

            string result = "";

            result += Name + "," ;
            result += GetSizeUnit() + ",";
            result += Extension + ",";
            result += Author + ",";
            result += CreatedAt + ",";
            result += Version + ",";
            result += DocumId + ",";
            if (IsCheckout)
            {
                if (user.Name.Equals(CheckoutEditor))
                    result += "||COMMIT";
                else
                    result += CheckoutEditor + "||CHECKOUT";
            }
            else
            {
                result += "|";
                int index = 0;
                foreach (var entry in Versions)
                {
                    if (index != 0)
                        result += "[";
                    result += entry.Key + ":" + entry.Value; // Version:CodDocums
                    index++;
                }
                result += "|";
            }
            result += "|";

            return result;
        }

        public Hashtable InfoDocQweb2(User user)
        {
            Hashtable res = new Hashtable();

            if (IsEmptyFile)
                return res;

            res.Add("name", Name);
            res.Add("size", GetSizeUnit());
            res.Add("ext", Extension);

            res.Add("author", Author);
            res.Add("date", CreatedAt);
            res.Add("documid", DocumId);

            if (IsCheckout)
            {
                if (user.Name.Equals(CheckoutEditor))
                    res.Add("chkstate", "COMMIT");
                else
                    res.Add("chkstate", "CHECKOUT");
                res.Add("chkuser", CheckoutEditor);

                foreach (var entry in Versions)
                {
                    res.Add("version", entry.Key);
                    res.Add("coddocums", entry.Value);
                }
            }

            return res;
        }

        public string GetSizeUnit()
        {
            string unit = " bytes";

            if (Size > 1024 * 1024)
            {
                Size /= 1024 * 1024;
                unit = " Mb";
            }
            else if (Size > 1024)
            {
                Size /= 1024;
                unit = " Kb";
            }

            return Size + unit;
        }

        public static DBFile EmptyFile()
        {
            return new DBFile();
        }
	}

	/// <summary>
	/// Classe genérica que representa uma área, todas as áreas criadas deverão extender esta
	/// classe e implementar os métodos abstractos
	/// Os atributos:
	/// - fields é uma hash cujo identifier é (alias+fieldName), cada entrada aponta to um Field
	/// todasAreas hashtable que permite obter todas as áreas
	/// </summary>
	public abstract class DbArea : Area
	{
        //iif(Qversion == 'CHECKOUT', 0, cast(iif(locate(Qversion, '.') == 0, Qversion, substring(Qversion, 1, locate(Qversion, '.') - 1)) as int))
        protected static readonly ISqlExpression DOCUMS_SORT_COLUMN1 = SqlFunctions.Iif(CriteriaSet.And().Equal("docums", "versao", "CHECKOUT"), 0, SqlFunctions.Cast(SqlFunctions.Iif(CriteriaSet.And().Equal(SqlFunctions.Locate("docums", "versao", "."), 0), new ColumnReference("docums", "versao"), SqlFunctions.Substring("docums", "versao", 1, SqlFunctions.Subtract(SqlFunctions.Locate("docums", "versao", "."), 1))), DbType.Int32));
        //iif(locate(Qversion, '.') == 0, 0, cast(substring(Qversion, locate(Qversion, '.') + 1, length(Qversion) - locate(Qversion, '.')) as int))
        protected static readonly ISqlExpression DOCUMS_SORT_COLUMN2 = SqlFunctions.Iif(CriteriaSet.And().Equal(SqlFunctions.Locate("docums", "versao", "."), 0), 0, SqlFunctions.Cast(SqlFunctions.Substring("docums", "versao", SqlFunctions.Add(SqlFunctions.Locate("docums", "versao", "."), 1), SqlFunctions.Subtract(SqlFunctions.Length("docums", "versao"), SqlFunctions.Locate("docums", "versao", "."))), DbType.Int32));

        /// <summary>
        /// Se true faz o rename dos ficheiros depois de upload, se false não faz rename
        /// </summary>
        protected static bool renfile = true;

        protected bool m_QueueMode = false;
        public bool QueueMode
        {
            get { return m_QueueMode; }
            set { m_QueueMode = value; }
        }

		/// <summary>
		/// função que preenche o Qfield com o name do file e uma ficha da table docums com o conteúdo
		/// </summary>
		/// <param name="nomeCampo">Name do Qfield</param>
		/// <param name="valorCampo">caminho to o documento</param>
		/// <param name="valorChaveDocums">key estrangeira to a table docums</param>
		public string insertNameValueFileDB(string fieldName, byte[] file, string inputFileName, object keyValueDocums, PersistentSupport sp, string Qversion, string operChange)
		{
			try
			{
                string valorLigacao = this.returnValueField(this.Alias + "." + this.PrimaryKeyName).ToString();
    		    int size = file.Length;

                string fileName = inputFileName.Substring(0, inputFileName.LastIndexOf('_'));

                int tamanhoNome = CSGenioAdocums.GetInformation().DBFields["nome"].FieldSize;
                int tamanhoExtensao = CSGenioAdocums.GetInformation().DBFields["extensao"].FieldSize;

                // a função Path.GetExtension devolve com o prefixo "." se existir extensão
                string extension = System.IO.Path.GetExtension(fileName).Trim('.');

                // truncar o name do file, se for maior do que o size definido
                if (fileName.Length > tamanhoNome)
                {
                    string name = Path.GetFileNameWithoutExtension(fileName);
                    // removem-se os characters a mais
                    name = name.Substring(0, Math.Max(name.Length - (fileName.Length - tamanhoNome), 0));
                    // acrescenta-se a extensão caso exista
                    fileName = name + ((extension.Length > 0) ? "." + extension : "");

					// depois de concatenar a extensão tem de se voltar a verificar se não ultrapassa o size limite
                    if (fileName.Length > tamanhoNome)
                        fileName = fileName.Substring(0, tamanhoNome);
                }

                // truncar a extensão, se for maior do que o size definido
                if (extension.Length > tamanhoExtensao)
                    extension = extension.Substring(0, tamanhoExtensao);

                if (keyValueDocums.Equals(""))
                {
                    keyValueDocums = sp.insertValueDocums(this, fieldName, fileName, extension, file);
                    this.insertNameValueField(Alias + "." + fieldName + "fk", keyValueDocums);
                }
                else
                     sp.changeValueDocums(keyValueDocums, file, fileName, extension, Qversion, operChange, this.TableName);

                this.insertNameValueField(Alias + "." + fieldName, fileName);

                return Information.HasVersionManagment ? keyValueDocums.ToString() : fileName;
			}
			catch (GenioException ex)
			{
				if (ex.UserMessage == null)
					throw new BusinessException("Erro ao obter o ficheiro: " + inputFileName, "DbArea.inserirNomeValorFicheiroBD", "Error getting the file " + inputFileName + ": " + ex.Message, ex);
				throw new BusinessException("Erro ao obter o ficheiro " + inputFileName + ": " + ex.UserMessage, "DbArea.inserirNomeValorFicheiroBD", "Error getting the file " + inputFileName + ": " + ex.Message, ex);
			}
			catch (Exception ex)
			{
                throw new BusinessException("Erro ao obter o ficheiro: " + inputFileName, "DbArea.inserirNomeValorFicheiroBD", "Error getting the file " + inputFileName + ": " + ex.Message, ex);
			}
		}

        /// <summary>
        /// função que obtem uma cópia do file (que está na db, na table docums) to a pasta temporária e devolve o name do file
        /// </summary>
        /// <param name="ficheiro">key do file</param>
        /// <param name="sp">suporte persistente</param>
        public static DBFile getFileDB(string coddocums, PersistentSupport sp)
        {
            if (sp == null)
				throw new BusinessException(null, "DbArea.getFileDB", "PersistentSupport is null.");

			try
			{
                string tableName = "docums";
                SelectQuery qs = new SelectQuery()
                    .Select(tableName, "nome")
                    .Select(tableName, "versao")
                    .Select(tableName, "document")
                    .Select(tableName, "docpath")
                    .From(tableName)
                    .Where(CriteriaSet.And()
                        .Equal(tableName, "coddocums", coddocums)
                        .NotEqual(tableName, "versao", "CHECKOUT"));

                ArrayList results = sp.executeReaderOneRow(qs);
                string fileName = DBConversion.ToString(results[0]);
                string Qversion = DBConversion.ToString(results[1]);

                byte[] file = null;
                if (!String.IsNullOrEmpty(results[2].ToString()) && ((byte[])results[2]).Length > 0)
                    file = DBConversion.ToBinary(results[2]);
                else if (!String.IsNullOrEmpty(results[3].ToString()))
                    file = PersistentSupport.getFileFromDisk(DBConversion.ToString(results[3]));

                if (file == null)
                {
                    throw new BusinessException("Could not find the file.", "getFileDB",
                        "Could not fetch file, make sure it exists or that the file migration routine has been run.", new NullReferenceException());
                }

                string extension = "";
                int extensionIndex = fileName.LastIndexOf(".");
                if (extensionIndex >= 0)
                    extension = fileName.Substring(extensionIndex);

                return new DBFile(fileName, extension, Qversion, file, file.Length);
			}
			catch (GenioException ex)
			{
				throw new BusinessException(ex.UserMessage, "DbArea.getFileDB", "Error getting the file " + coddocums + ": " + ex.Message, ex);
			}
			catch (Exception ex)
			{
				throw new BusinessException(null, "DbArea.getFileDB", "Error getting the file " + coddocums + ": " + ex.Message, ex);
			}
        }

		//Este método tem como objectivo ir a BD buscar o registo e actualizar os dados em memória que ainda não foram actualizados pelo interface
        //ou seja fazer o preenchimento de todos os dados da area
        public void FillOldValues(PersistentSupport sp)
        {
            //TODO: o objectivo é colocar esta função em todo o lado que faça este trabalho
            //sendo que os oldvalues terão de ser guardados como propriedade da própria area
            //evitando assim envia-los to os outros metodos
            //outra situação importante é executar esta operação antes dos override da area (change, introduce, eliminate...)
            //to que em memória tenhamos acesso aos Qvalues correctos de todos os fields.

            //ler os Qvalues da ficha antiga
            Area oldvalues = Area.createArea(this.Alias, user, module);
            sp.getRecord(oldvalues, QPrimaryKey);

            //garantir que todos os fields estão preenchidos, se o interface não forneceu um Qvalue então usamos o antigo
            //isto permite ás rotinas seguintes não ter de sistematicamente tentar fazer queries à BD
            foreach (string key in oldvalues.Fields.Keys)
            {
                if (!Fields.ContainsKey(key))
                {
                    Fields[key] = new RequestedField(oldvalues.Fields[key] as RequestedField);
                }
            }
        }

		/****************************CALCULO DE FORMULAS***************************************/

		/// <summary>
		/// função que preenche os Qvalues default
		/// </summary>
		/// <param name="sp">suporte persistente que esta a ser utilizado</param>
		/// <returns>devolve a area com os Qvalues default preenchidos</returns>
        public Area fillValuesDefault(PersistentSupport sp, FunctionType tpFunction)
        {
            try
            {
                if (this.DefaultValues != null)
                {
                    FormulaDbContext fdc = new FormulaDbContext(this);
                    fdc.AddDefaults();

                    string[] valoresDefault = this.DefaultValues;
                    for (int i = 0; i < valoresDefault.Length; i++)
                    {
                        Field Qfield = (Field)this.DBFields[valoresDefault[i]];
                        RequestedField reqField = null;
                        bool hasEmptyValue = true;

                        if (this.Fields.ContainsKey(Alias + "." + valoresDefault[i]))
                        {
                            reqField = (RequestedField)this.Fields[Alias + "." + Qfield.Name];
                            hasEmptyValue = Qfield.isEmptyValue(reqField.Value);
                        }
                        // Ignore fields with default of the type "FIXO" when this field is already filled.
                        if (Qfield.DefaultValue.tpDefault == DefaultValue.DefaultType.FIXO && !hasEmptyValue)
                            continue;

                        // Skip fields that are not empty when duplicating records
                        if (tpFunction == FunctionType.DUP && !hasEmptyValue)
                            continue;

                        object valorDefault = Qfield.DefaultValue.calculateFormulaDefault(this, sp, fdc, tpFunction);
                        insertNameValueField(Alias + "." + valoresDefault[i], valorDefault);
                    }
                }
                return this;
            }
            catch (GenioException ex)
            {
                throw new BusinessException(ex.UserMessage, "DbArea.preencherValoresDefault", "Error computing default values: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new BusinessException(null, "DbArea.preencherValoresDefault", "Error computing default values: " + ex.Message, ex);
            }
        }

		public void calculateTemporarySequentials()
        {
            Dictionary<string, object> CamposNegativos = new Dictionary<string, object>();
            foreach (var Qfield in Fields.Values)
            {
                RequestedField campoPedido = (RequestedField)Qfield;
                if (!DBFields.ContainsKey(campoPedido.Name))
                    continue;

                if (DBFields[campoPedido.Name] != null)
                {
                    Field campoBD = (Field)DBFields[campoPedido.Name];
                    object condition = null;
                    //If it's a sequential number in the DB a random Qvalue is written
                    //We only do this if the field value is empty, we want to maintain the value otherwise
                    if (campoBD.DefaultValue != null && campoBD.DefaultValue.tpDefault.Equals(DefaultValue.DefaultType.PRE_DEF_BD))
                    {
                        object valorObj = QueryUtils.getRandomValue(campoBD);
                        condition = DBConversion.FromInternal(valorObj, campoBD.FieldType.Formatting);
                    }
                    if (condition != null)
                    {
						if (condition.GetType() == typeof(string))
                            condition = (object)condition.ToString().Replace("'",""); //test CHN
                        CamposNegativos.Add(campoBD.Alias + "." + campoBD.Name, condition);
                    }
                }
            }

            foreach (KeyValuePair<string, object> Qfield in CamposNegativos)
                insertNameValueField(Qfield.Key, Qfield.Value);
        }

        private void preencherValorSequencial(PersistentSupport sp, Field Qfield, Area oldValues)
        {

            if (Zzstate == 1) //Extreme case, when creating PseudoNew row, no calculation is allowed
                return;
            bool noChanges = oldValues == null ? true : false; //if no oldValues we assume there were no changes

            bool needsToBeCalculated = false;
            bool savePseudoNew =  oldValues != null && oldValues.Zzstate == 1 && Zzstate == 0 ? true : false; // PseudoNew Save or Manual Calls without oldvalues


            object sequentialFieldValue = returnValueField(Alias + "." + Qfield.Name); //Qvalue do Qfield sequencial

            object nDupPrefValue = null; //Qvalue do prefixo de não duplicação (caso exista)
            FieldFormatting formCampoPrefNDup = FieldFormatting.CARACTERES;
            if (Qfield.PrefNDup != null)
            {
                nDupPrefValue = returnValueField(Alias + "." + Qfield.PrefNDup);
                formCampoPrefNDup = returnFormattingDBField(Qfield.PrefNDup);
            }

            bool isChanged = false; //Assume it's false as most cases will be
            bool InvalidValue = false; //Assume it's false as most cases will be
            bool prefixChanged = false; //Assume it's false as most cases will be

            //Check if field has a valid number
            if (Qfield.isEmptyValue(sequentialFieldValue)) //Empty or null not valid
            {
                InvalidValue = true;
            }
            else
            {
                //check for negatives
                //Check if field is Date
                if ((sequentialFieldValue is DateTime || sequentialFieldValue is DateTime?)) //DateTime Field
                {
                    if (Convert.ToDateTime(sequentialFieldValue) > DateTime.MinValue) //minDate is the same as negative date
                        InvalidValue = true;
                }
                else
                {
                    if (Convert.ToDecimal(sequentialFieldValue) < 0) //negative number
                        InvalidValue = true;
                }
            }
            //-------------------------

            //Check if already Calculated and value has not changed
            if (!InvalidValue && !noChanges) //check for changes only in valid value and with oldValues
            {
                object oldSequencialValue = oldValues.returnValueField(Alias + "." + Qfield.Name);
                if (!sequentialFieldValue.Equals(oldSequencialValue))
                    isChanged = true;
            }
            //-------------------------

            //check if prefix changed
            if (!InvalidValue && Qfield.PrefNDup != null && !noChanges) // check for changes in prefix only in valid value and with oldValues
            {
                object oldValorPrefNDup = oldValues.returnValueField(Alias + "." + Qfield.PrefNDup);
                if (nDupPrefValue is string && oldValorPrefNDup is string)
                {
                    // MH (30/10/2017) - Houve caso em que o prefixo de não duplicação, que vinha dum Qfield de CE com formula artitmética, tinha "case" diferente.
                    if (!((string)nDupPrefValue).Equals(((string)oldValorPrefNDup), StringComparison.InvariantCultureIgnoreCase))
                        prefixChanged = true;
                }
                else if ((nDupPrefValue == null && oldValorPrefNDup != null) || (nDupPrefValue != null && !nDupPrefValue.Equals(oldValorPrefNDup)))
                    prefixChanged = true;
            }
            //-------------------------


            //-------------------------

            //Check if record is being inserted or edited
            //We can check this by taking a look a the zzstate value,
            //This will only be one when the record is being inserted (or duplicated)
            //When duplicating, oldValues is usually null, so we set it to 1 as a default
            int zzstate = 1;
            if (oldValues != null)
                zzstate = oldValues.Zzstate;

            //Decide if it needs to be Calculated
            //We only want to validade the prefix if the record is not being inserted
            //Since there is no such things as "changing the prefix" on a new record
            //And when duplicating we want to maintain the same values
            if (InvalidValue || (prefixChanged && zzstate == 0)) //Invalid values are not allowed and prefix changes forces calculation
            {
                needsToBeCalculated = true;
            }
            else if (isChanged) //Check if changed value already exists, or if from manual entry value cannot be trusted
            {
                object primaryKeyValue = returnValueField(Alias + "." + PrimaryKeyName);
                if (Qfield.DefaultValue.existsSequentialValue(this, primaryKeyValue, Qfield.PrefNDup, nDupPrefValue, formCampoPrefNDup, sequentialFieldValue, Qfield.FieldFormat, sp))
                    needsToBeCalculated = true;
            }
            //-------------------------


            //Do the calculation
            if (needsToBeCalculated)
            {
                object valorSequencial = Qfield.DefaultValue.calculateSequentialFormula(this, Qfield.PrefNDup, nDupPrefValue, formCampoPrefNDup, sp);
                if (Qfield.FieldFormat == FieldFormatting.CARACTERES)
                {
                    valorSequencial = valorSequencial.ToString().PadLeft(Qfield.FieldSize);
                }
                if (Qfield.FieldFormat == FieldFormatting.DATA)
                {
                    if (valorSequencial.ToString() == "1")
                    {
                        if (Convert.ToDateTime(sequentialFieldValue) > DateTime.MinValue)
                            valorSequencial = sequentialFieldValue;
                        else
                            valorSequencial = DateTime.MinValue;
                    }
                }
                if (Qfield.FieldFormat == FieldFormatting.FLOAT)
                {
                    //If the order ISN'T bigger than all the others or smaller (1 <)
                    //Try to insert anyways (results in duplication error if unique)
                    if (!(Convert.ToDecimal(sequentialFieldValue) >= Convert.ToDecimal(valorSequencial) || Convert.ToDecimal(sequentialFieldValue) < 1))
                    {
                        insertNameValueField(Alias + "." + Qfield.Name, sequentialFieldValue);
                    }
                }
                insertNameValueField(Alias + "." + Qfield.Name, valorSequencial);
            }
        }

		/// <summary>
        /// Applys automatic formatting rules to fields
        /// </summary>
        public void formatFields()
        {
            foreach (Field Qfield in Information.DBFieldsList)
            {
                //right alignment
                if (Qfield.AlignRightPad)
                {
                    string value = returnValueField(Alias + "." + Qfield.Name) as string;
                    if (!string.IsNullOrEmpty(value))
                    {
                        value = value.PadLeft(Qfield.FieldSize);
                        insertNameValueField(Alias + "." + Qfield.Name, value);
                    }
                }
            }
        }

        /// <summary>
        /// Preenche todas as formulas internas, ou seja, formulas que só alteram a ficha que
        /// está a ser actualizada. O cálculo é feito pela sequencia em que os fields foram
        /// registados de forma a dar hipótese de respeitar dependencias entre eles.
        /// </summary>
        /// <param name="sp">Um suporte persistente com uma conexão aberta</param>
        /// <param name="oldvalues">Os Qvalues anteriores do registo ou null caso estejasmos numa inserção</param>
        public void fillInternalOperations(PersistentSupport sp, Area oldvalues)
        {
			FormulaDbContext fdc = new FormulaDbContext(this);
            fdc.AddInternalOperations();
            fdc.AddReplicas();
            fdc.AddDefaults();

            foreach (Field Qfield in Information.DBFieldsList)
            {

                //RS 18.05.2017 updates onde a UserRecord está a false deitam fora todos os calculos de numeros sequenciais, to não estragar o calculo da ficha principal
                // pela gravação das fichas abaixo que potencialmente podem ter SR ou outras formulas de propagação.
                // Assim sendo, não vale a pena tentar calcular um number que vai ser deitado fora.
                if (UserRecord && Qfield.DefaultValue != null && SequentialDefaultValues != null && Array.Exists(SequentialDefaultValues, X => X == Qfield.Name))
                {
                    if (Qfield.DefaultValue.tpDefault == DefaultValue.DefaultType.OP_INT)
                    {
                        // Field with formula default (DG and DF) that has dependency of the sequential field.
                        object valorDefault = Qfield.DefaultValue.calculateFormulaDefault(this, sp, fdc, FunctionType.INS);
                        insertNameValueField(Alias + "." + Qfield.Name, valorDefault);
                    }
                    else
                    {
                        preencherValorSequencial(sp, Qfield, oldvalues);
                    }
                    continue;
                }

                //RS(2017.03.13) Suport for FillWhen in the server side. It was only calculating in the interface side, forcing the interface to include this field.
                if (Qfield.FillWhen != null)
                {
                    object[] fieldsValue = Qfield.FillWhen.returnValueFieldsInternalFormula(this, Qfield.FillWhen.ByAreaArguments, sp, Qfield.FillWhen.ParameterCount, FunctionType.ALT);
                    if (!Qfield.FillWhen.calculateFormulaCondition(fieldsValue, user, user.CurrentModule, sp))
                        insertNameValueField(Alias + "." + Qfield.Name, null);
                }

                if (Qfield.Formula == null)
                    continue;

                if (Qfield.Formula is ReplicaFormula)
                {
                    preencherReplica(sp, Qfield, fdc);
                    continue;
                }
                if (Qfield.Formula is QueryTableFormula)
                {
                    preencherConsultaTabela(sp, Qfield);
                    continue;
                }
                if (Qfield.Formula is InternalOperationFormula)
                {
                    preencherOperacaoInterna(sp, Qfield, fdc, oldvalues);
                    continue;
                }
                if (Qfield.Formula is EndPeriodFormula)
                {
                    preencherFimPeriodo(sp, Qfield);
                    continue;
                }
            }
		}

		/// <summary>
        /// Verifica se a condição to não recalcular está activa.
        /// </summary>
        /// <param name="sp">Suporte persistente com a conexão aberta</param>
		/// <returns>true se pode calcular</returns>
		private bool verificaRecalculo(PersistentSupport sp)
        {
            ConditionFormula condition = Information.ForbidsRecalculationIf;
            if (condition != null)
            {
                object[] Qvalues = condition.returnValueFieldsInternalFormula(this, condition.ByAreaArguments, sp, condition.ParameterCount, FunctionType.ALT);
				return condition.calculateFormulaCondition(Qvalues, user, module, sp);
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Recalcula uma réplica (++)
        /// </summary>
        /// <param name="sp">Suporte persistente com a conexão aberta</param>
        /// <param name="campo">O Qfield a ser recalculado</param>
        private void preencherReplica(PersistentSupport sp, Field Qfield, FormulaDbContext fdc)
        {
            ReplicaFormula formula = Qfield.Formula as ReplicaFormula;
            System.Diagnostics.Debug.Assert(formula != null);

            Area target = fdc.GetRelation(formula.Alias, sp, User);
            object replica = target.returnValueField(target.Alias + "." + formula.Field);
            insertNameValueField(Alias + "." + Qfield.Name, replica);

            /*
            Relation relacao = (Relation)this.ParentTables[formula.Alias];
            object fieldValue = returnValueField(Alias + "." + relacao.SourceRelField, FieldFormatting.CARACTERES);
            if (!Field.isEmptyValue(fieldValue, FieldFormatting.CARACTERES))
            {
                //DQ 01/09/2006 -> usar campoRelDestino em vez de source: a source é a table que aponta, o target a table que é apontada.
                object replica = formula.getReplicaValue(relacao.DestinationSystem, relacao.TargetTable, relacao.TargetRelField, (string)fieldValue, sp);
                insertNameValueField(Alias + "." + Qfield.Name, replica);
            }
            else //se o Qfield é nulo então atribuir o Qvalue de vazio
                insertNameValueField(Alias + "." + Qfield.Name, Qfield.GetValorEmpty());
            */
        }

        /// <summary>
        /// Recalcula uma consulta a table (CT)
        /// </summary>
        /// <param name="sp">Suporte persistente com a conexão aberta</param>
        /// <param name="campo">O Qfield a ser recalculado</param>
        private void preencherConsultaTabela(PersistentSupport sp, Field Qfield)
        {
            QueryTableFormula formula = Qfield.Formula as QueryTableFormula;
            System.Diagnostics.Debug.Assert(formula != null);

            object valorDataPreenchida = returnValueField(Alias + "." + formula.FilledDateFields);
            Field filledDateField = (Field)this.DBFields[formula.FilledDateFields];
            //se o Qvalue do Qfield é null
            if (valorDataPreenchida != null)
            {
                //se a formula tem Qfield que agrupa
                if (formula.IsGroup)
                {
                    //Qvalue do Qfield que agrupa
                    object campoAgruparObj = Fields[Alias + "." + formula.FilledGroupField];
                    if (campoAgruparObj != null)//se não exists Qvalue, não há nada to change
                    {
                        RequestedField campoAgrupar = (RequestedField)campoAgruparObj;
                        if (formula.IsGroup2)
                        {
                            //Qvalue do Qfield que agrupa
                            object campoAgruparObj2 = Fields[Alias + "." + formula.Filled2GroupField];
                            if (campoAgruparObj2 != null)//se não exists Qvalue, não há nada to change
                            {
                                RequestedField campoAgrupar2 = (RequestedField)campoAgruparObj2;
                                object ct = formula.getGroupedCTValue(valorDataPreenchida, filledDateField.FieldFormat, campoAgrupar.Value, campoAgrupar.FieldType.Formatting, campoAgrupar2.Value, campoAgrupar2.FieldType.Formatting, sp);
                                insertNameValueField(Alias + "." + Qfield.Name, ct);
                            }
                        }
                        else
						{
                            object ct = formula.getGroupedCTValue(valorDataPreenchida, filledDateField.FieldFormat, campoAgrupar.Value, campoAgrupar.FieldType.Formatting, sp);
                            insertNameValueField(Alias + "." + Qfield.Name, ct);
                        }
                    }
                }
                else//se não exists Qfield a agrupar
                {
                    object ct = formula.getCTValue(valorDataPreenchida, filledDateField.FieldFormat, sp);
                    insertNameValueField(Alias + "." + Qfield.Name, ct);
                }
            }
        }

        /// <summary>
        /// Recalcula uma formula aritmética (+, +H)
        /// </summary>
        /// <param name="sp">Suporte persistente com a conexão aberta</param>
        /// <param name="campo">O Qfield a ser recalculado</param>
		/// <param name="fdc">O contexto de agregação das fontes das formulas</param>
        private void preencherOperacaoInterna(PersistentSupport sp, Field Qfield, FormulaDbContext fdc, Area oldvalues)
        {
            InternalOperationFormula formula = Qfield.Formula as InternalOperationFormula;
            System.Diagnostics.Debug.Assert(formula != null);

            bool shouldRecalc = shouldForceRecalc(formula, oldvalues);

            if (!shouldRecalc && !verificaRecalculo(sp)) // table-level recalif
            {
                shouldRecalc = false;
            }
            else if (!shouldRecalc && Qfield.RecalculatesIf != null) // field-level recalcif
            {
                object[] Qvalues = Qfield.RecalculatesIf.returnValueFieldsInternalFormula(this, Qfield.RecalculatesIf.ByAreaArguments, sp, Qfield.RecalculatesIf.ParameterCount, FunctionType.ALT);
                shouldRecalc = Qfield.RecalculatesIf.calculateFormulaCondition(Qvalues, user, module, sp);
            }
            else // recalcif conditions are not defined or should recalc anyway
            {
                shouldRecalc = true;
            }

            if (shouldRecalc)
            {
                object fieldValue = formula.calculateInternalFormula(this, sp, fdc, FunctionType.ALT);
                if (fieldValue != null)
                    insertNameValueField(Alias + "." + Qfield.Name, fieldValue);
            }
        }

        /// <summary>
        /// Determine if the field value should be recalculated
        /// </summary>
        /// <param name="formula">The formula to analyze</param>
        /// <param name="oldvalues">The record's previous Qvalues or null if we are in an insert</param>
        private bool shouldForceRecalc(InternalOperationFormula formula, Area oldvalues)
        {
            /* Determine if the field value should be recalculated
             * --
             * Approach:
             * - Always recalc if input arguments have changed
             * - If not, performing recalc or not depends on recalcif
             * - Finally, recalcs if the conditions are not defined
             */

            if (oldvalues == null)
                return true;

            foreach (ByAreaArguments area in formula.ByAreaArguments)
            {
                foreach (string argument in area.FieldNames)
                {
                    string fieldname;
                    if (area.AliasName == "glob")
                    {
                        // GLOB field used in formula
                        // not forcing recalc due to lack of context
                        continue;
                    }
                    else if (area.AliasName != Alias)
                    {
                        // Argument from a different area
                        // we can only check if the value of the FK changed
                        Relation rel = Information.ParentTables[area.AliasName];
                        fieldname = Alias + "." + rel.SourceRelField;
                    }
                    else
                    {
                        // Argument from this area
                        // we check if the value changed
                        fieldname = Alias + "." + argument;
                    }

                    object oldvalue = oldvalues.returnValueField(fieldname);
                    object newvalue = returnValueField(fieldname);

                    if ((oldvalue == null && newvalue != null) || (oldvalue != null && !oldvalue.Equals(newvalue)))
                    {
                        // Value changed, forcing recalc
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Recalcula uma formula de fim de periodo (FP)
        /// </summary>
        /// <param name="sp">Suporte persistente com a conexão aberta</param>
        /// <param name="campo">O Qfield a ser recalculado</param>
		private void preencherFimPeriodo(PersistentSupport sp, Field Qfield)
        {
            EndPeriodFormula formula = Qfield.Formula as EndPeriodFormula;
            System.Diagnostics.Debug.Assert(formula != null);

            object primaryKeyValue = QPrimaryKey;
            object start = returnValueField(Alias + "." + formula.DateField);
            object grouping = null;
            Field campoGrupo = null;
            if (formula.GroupField != null)
            {
                campoGrupo = DBFields[formula.GroupField] as Field;
                grouping = returnValueField(Alias + "." + formula.GroupField);
            }

            object fim;
            // evita-se fazer mais uma query, quando já se sabe que temos de limpar o Qvalue do Qfield de fim
            if (Qfield.isEmptyValue(start) || (campoGrupo != null && campoGrupo.isEmptyValue(grouping)))
                // se o start é vazio, tem de limpar o fim!
                fim = Qfield.GetValorEmpty();
            else
                //ler qual é o fim de periodo dada a data de start
                fim = formula.readEndPeriod(sp, this, start, grouping, primaryKeyValue);
            insertNameValueField(Alias + "." + Qfield.Name, fim);
        }

        /// <summary>
        /// Propaga os ultimos Qvalues to as fichas acima
        /// Calcula que fichas devem ser actualizadas
        /// </summary>
        /// <param name="sp">O suporte persistente</param>
        /// <param name="oldvalues">Os Qvalues originais do registo to optimizar e poder lidar com mudanças da relação</param>
        /// <param name="deleted">True quando o registo foi apagado, false quando foi actualizado</param>
        private void propagarUltimosValores(PersistentSupport sp, Area oldvalues, bool deleted)
        {
            if (LastValueArgs == null)
                return;
            foreach (LastValueArgument arg in LastValueArgs)
            {
                //ver se a relação mudou
                Relation rel = Information.ParentTables[arg.AliasRUV];
                object oldrel = oldvalues.returnValueField(Alias + "." + rel.SourceRelField);
                object newrel = returnValueField(Alias + "." + rel.SourceRelField);

                //se a ficha é pseudo-nova então é como se o Qvalue antigo não existisse
                if (Zzstate != 0)
                    oldrel = "";
                //se a ficha foi apagada é como se a nova relação não existisse
                if (deleted)
                    newrel = "";

                // TODO - se:
                // o U1 não tem condição
                // e a relação não mudar
                // e o Qvalue da data não mudar
                // e nenhum dos Qvalues dos fields consultados tiver sido alterado
                // não é necessária nenhuma propagação

                FieldFormatting formatacaoRelacao = DBFields[rel.SourceRelField].FieldFormat;

                //se a nova relação não é nula é preciso actualizar a nova
                if (!Field.isEmptyValue(newrel, formatacaoRelacao))
                    auxActualizarUltimoValor(sp, arg, rel.SourceRelField, rel.TargetRelField, newrel, formatacaoRelacao);

                //se a relação mudou e a antiga não era nula é preciso actualizar a antiga
                if (!Field.isEmptyValue(oldrel, formatacaoRelacao) && !oldrel.Equals(newrel))
                    auxActualizarUltimoValor(sp, arg, rel.SourceRelField, rel.TargetRelField, oldrel, formatacaoRelacao);
            }
        }

        /// <summary>
        /// Actualiza a ficha de target
        /// </summary>
        /// <param name="sp">O suporte persistente</param>
        /// <param name="arg">Metadados sobre o ultimo Qvalue</param>
        /// <param name="campoRelacao">O Qfield que estabeleca a relação com a table acima</param>
        /// <param name="valorRelacao">O Qvalue da key to a table acima</param>
        /// <param name="formatacaoRelacao">A formatação do tipo de Qfield da relação</param>
        private void auxActualizarUltimoValor(PersistentSupport sp, LastValueArgument arg, string relationalField, string TargetRelField, object relationValue, FieldFormatting formatacaoRelacao)
        {
            FieldFormatting formatacaoDataConsultada = DBFields[arg.ConsultedDateFields].FieldFormat;

            //ler a data de encerramento da ficha acima
            //TODO: arg.EndDateField

            //descobrir quais o Qvalues mais recentes
            ArrayList valoresConsultados = arg.ReadLvr(sp, this, relationalField, relationValue, formatacaoRelacao, formatacaoDataConsultada, TargetRelField);

            //posicionamos a área da relação
            DbArea areaRuv = (DbArea)Area.createArea(arg.AliasRUV, user, module);
            areaRuv.UserRecord = false;
            sp.getRecord(areaRuv, relationValue);
            int i = 0;
            //so actualizamos quando existem mudanças de Qvalue
            bool updateNeeded = false;

            //se encontramos uma ficha
            foreach (string Qfield in arg.ConsultedFields)
            {
                if (valoresConsultados.Count > 0)
                {
                    FieldFormatting formatting = DBFields[Qfield].FieldFormat;
                    object Qvalue = DBConversion.ToInternal(valoresConsultados[i], formatting);
                    if (!Qvalue.Equals(areaRuv.returnValueField(areaRuv.Alias + "." + arg.LVRFields[i])))
                        updateNeeded = true;
                    areaRuv.insertNameValueField(areaRuv.Alias + "." + arg.LVRFields[i], Qvalue);
                }
                else //temos de limpar os Qvalues
                {
                    areaRuv.insertNameValueField(areaRuv.Alias + "." + arg.LVRFields[i], null);
                    updateNeeded = true;
                }
                i++;
            }

            //actualizar a area da relacao caso necessário
            if (updateNeeded)
                areaRuv.update(sp);
        }

        /// <summary>
        /// Actualiza as fichas afectadas pela alteração do fim de periodo
        /// </summary>
        /// <param name="sp">O suporte persistente</param>
        /// <param name="oldvalues">Os Qvalues antigos do registo</param>
        /// <param name="deleted">True se o registo está a ser apagado</param>
        private void propagarFimPeriodo(PersistentSupport sp, Area oldvalues, bool deleted)
        {
            if (this.EndofPeriodFields == null)
                return;

            foreach (string campoFp in EndofPeriodFields)
            {
                Field Qfield = (Field)this.DBFields[campoFp];
				Field campoAgrupar = null;
                EndPeriodFormula formula = (EndPeriodFormula)Qfield.Formula;

                //ver se temos de actualizar o registo anterior ao actual
                object primaryKeyValue = QPrimaryKey;

                //obter os Qvalues actuais
                object start = returnValueField(Alias + "." + formula.DateField);
                object grouping = null;
                if (formula.GroupField != null)
                {
                    grouping = returnValueField(Alias + "." + formula.GroupField);
                    campoAgrupar = (Field)this.DBFields[formula.GroupField];
                }

                //obter os Qvalues antigos
                object oldinicio = oldvalues.returnValueField(Alias + "." + formula.DateField);
                object oldagrupamento = null;
                if (formula.GroupField != null)
                    oldagrupamento = oldvalues.returnValueField(Alias + "." + formula.GroupField);

                //se a ficha está a ser apagada então é como se os Qvalues novos fossem zero
                if (deleted)
                {
                    start = Qfield.GetValorEmpty();
                    grouping = (campoAgrupar != null) ? campoAgrupar.GetValorEmpty() : null;
                }

                //se a ficha não é nova entao é como se os Qvalues antigos fossem zero
                if (oldvalues.Zzstate == 1)
                {
                    oldinicio = Qfield.GetValorEmpty();
                    oldagrupamento = (campoAgrupar != null) ? campoAgrupar.GetValorEmpty() : null;
                }

                //se a data e o grouping forem iguais não é preciso propagar
                if (start.Equals(oldinicio) && (formula.GroupField == null || (oldagrupamento != null && oldagrupamento.Equals(grouping))))
                    continue;

                //actualiza a ficha que ficou atras dos novos Qvalues
                if (!Qfield.isEmptyValue(start))
                {
                    string chaveAnterior = formula.getPreviousRecord(sp, this, start, grouping);
                    if (!string.IsNullOrEmpty(chaveAnterior))
                        auxActualizaFimPeriodo(sp, campoFp, formula, chaveAnterior);
                }

                //actualiza a ficha que estava atras dos Qvalues antigos
                if (!Qfield.isEmptyValue(oldinicio))
                {
                    string chaveAnterior = formula.getPreviousRecord(sp, this, oldinicio, oldagrupamento);
                    if (!string.IsNullOrEmpty(chaveAnterior))
                        auxActualizaFimPeriodo(sp, campoFp, formula, chaveAnterior);
                }
            }
        }

        private void auxActualizaFimPeriodo(PersistentSupport sp, string campoFp, EndPeriodFormula formula, object chaveAnterior)
        {
			DbArea outra = (DbArea)Area.createArea(Alias, user, module);
            outra.UserRecord = false;
            sp.getRecord(outra, chaveAnterior);
            // enquanto o cálculo de FP's não estiver protegido com um test se o Qvalue de start mudou ou não
            // as próximas 6 instruções até ao update são desnecessárias, porque o Qvalue é recalculado pelas fórmulas internas
            // ao gravar a ficha de target (a query to ler o FP vai ser feita duas vezes)
            object start = outra.returnValueField(Alias + "." + formula.DateField);
            object grouping = null;
            if (formula.GroupField != null)
                grouping = outra.returnValueField(Alias + "." + formula.GroupField);

            object fim = formula.readEndPeriod(sp, this, start, grouping, chaveAnterior);
            outra.insertNameValueField(Alias + "." + campoFp, fim);

            outra.update(sp);
        }

		/// <summary>
        /// Actualiza as fichas afectadas pela alteração de fields somados por uma SR
		/// </summary>
        /// <param name="sp">O suporte persistente</param>
        /// <param name="oldvalues">Os Qvalues antigos do registo</param>
        /// <param name="deleted">True se o registo está a ser apagado</param>
        public void propagateLinkedSum(PersistentSupport sp, Area oldValues, bool delete)
        {
            if (RelatedSumArgs == null)//fields argumentos de fórmulas do tipo soma relacionada
                return;

            //com zzstate a 1 não somamos nada
            // MH (27/01/2020) - Invalid records (zzstate equal to 11), which were not accounted in reindexing, should not propagate SR upwards, otherwise, the values of that record are incorrect.
            // Imagine the following relational model C(zzstate = 1)->B(zzstate = 11 | On delete of C)->A(zzstate = 1).
            // By eliminating B, C propagates formula up to A.
            if (Zzstate == 1 || Zzstate == 11)
                return;

            Dictionary<string, DbArea> areasPosicionadas = new Dictionary<string, DbArea>();

            // JMN (17/07/2020) - HACK: There is a scenario where a group of SRs affects the same row but have different areas associated.
            // In this scenario, SR values computed for the first area were being overriden by the values from the second area SR.
            // This happens because we read all the information before updating the values, causing the second area SR to use the old values and overriding the values set by the first area SR.
            // The fix consists in dealing with each area separately and applying the updates once SRs from that area are computed.
            var linkedSumAlias = RelatedSumArgs.Select(x => x.AliasSR).Distinct();
            foreach (string aliasSR in linkedSumAlias)
            {
                foreach (RelatedSumArgument argSR in RelatedSumArgs)
                {
                    if (!argSR.AliasSR.Equals(aliasSR))
                        continue;

                    AreaInfo source = Area.GetInfoArea(argSR.AliasSource);
                    Relation relacao = source.ParentTables[argSR.AliasSR];

                    FieldFormatting formatacaoRel = returnFormattingDBField(relacao.SourceRelField);
                    object valorRel = returnValueField(Alias + "." + relacao.SourceRelField);
                    object oldValorRel = oldValues.returnValueField(Alias + "." + relacao.SourceRelField);

                    decimal novoValor;
                    decimal oldValor;
                    if (argSR.IsField) //se for um Qfield vai buscar o Qvalue senão é uma contagem
                    {
                        novoValor = Convert.ToDecimal(returnValueField(Alias + "." + argSR.ArgField));
                        oldValor = Convert.ToDecimal(oldValues.returnValueField(Alias + "." + argSR.ArgField));
                    }
                    else //a contagem é feita com um number fixo (tipicamente 1.0)
                    {
                        novoValor = decimal.Parse(argSR.ArgField);
                        oldValor = novoValor;
                    }

                    //se antes a ficha era pseudo-nova então é como se o Qvalue antigo fosse 0
                    if (oldValues.Zzstate == 1)
                    {
                        oldValor = 0;
                        oldValorRel = "";
                    }

                    //se a ficha vai ser apagada então é como se o Qvalue novo fosse 0
                    if (delete)
                    {
                        novoValor = 0;
                        valorRel = "";
                    }

                    //calcula a diferença a causar na relação antiga e na relação nova
                    decimal olddiff = -oldValor;
                    decimal newdiff = novoValor;
                    //se a relação ficou igual agregamos tudo no newdiff
                    if (oldValorRel.Equals(valorRel))
                    {
                        newdiff -= oldValor;
                        olddiff = 0;
                    }

                    //ao novo Qvalue da relação adicionamos a diferença
                    if (newdiff != 0 && !Field.isEmptyValue(valorRel, formatacaoRel))
                        AuxUpdateSr(sp, argSR, relacao, valorRel, newdiff, areasPosicionadas);

                    //ao Qvalue antigo da relação
                    if (olddiff != 0 && !Field.isEmptyValue(oldValorRel, formatacaoRel))
                        AuxUpdateSr(sp, argSR, relacao, oldValorRel, olddiff, areasPosicionadas);
                }

                // at the end, update all records on areasPosicionadas
                foreach (DbArea a in areasPosicionadas.Values)
                    a.update(sp);

                // reset areasPosicionadas after updating affected rows
                areasPosicionadas.Clear();
            }
        }

        private void AuxUpdateSr(PersistentSupport sp, RelatedSumArgument argSR, Relation relacao, object valorRel, decimal diff, Dictionary<string, DbArea> areasPosicionadas)
        {
            DbArea outra = null;
            areasPosicionadas.TryGetValue(relacao.AliasTargetTab + "_" + valorRel.ToString(), out outra);
            if (outra == null)
            {
                outra = (DbArea)Area.createArea(relacao.AliasTargetTab, user, module);
                outra.UserRecord = false;
                sp.getRecord(outra, valorRel, true);
                areasPosicionadas.Add(relacao.AliasTargetTab + "_" + valorRel.ToString(), outra);
            }
            decimal valorSR = Convert.ToDecimal(outra.returnValueField(outra.Alias + "." + argSR.SRField));
            if (argSR.Signal == '+')
                valorSR += diff;
            else
                valorSR -= diff;
            outra.insertNameValueField(outra.Alias + "." + argSR.SRField, valorSR);
        }

        //created by [AJA] at [2014.06.06] - concatenha linhas
        //last updated by [RG] at [2016.10.13]
        //last reviewed by [    ] at [    .  .  ]
        /// <summary>
        /// Actualiza as fichas afectadas pela alteração de fields concatenados por uma List Aggregate
        /// </summary>
        /// <param name="sp">O suporte persistente</param>
        /// <param name="oldvalues">Os Qvalues antigos do registo</param>
        /// <param name="deleted">True se o registo está a ser apagado</param>
        public void propagateListAggregate(PersistentSupport sp, Area oldValues, bool delete)
        {
            if (ArgsListAggregate == null)//fields argumentos de fórmulas do tipo soma relacionada
                return;

            //com zzstate a 1 não somamos nada
            if (Zzstate == 1)
                return;

            foreach (ListAggregateArgument argLG in ArgsListAggregate)
            {
                AreaInfo source = Area.GetInfoArea(argLG.AliasSource);
                Relation relacao = source.ParentTables[argLG.AliasLG];

                FieldFormatting formatacaoRel = returnFormattingDBField(relacao.SourceRelField);
                object valorRel = returnValueField(Alias + "." + relacao.SourceRelField);
                object oldValorRel = oldValues.returnValueField(Alias + "." + relacao.SourceRelField);

                object novoValorLG = returnValueField(Alias + "." + argLG.ArgField);
                object oldValorLG = oldValues.returnValueField(Alias + "." + argLG.ArgField);

                object novoValorOrdenacao = returnValueField(Alias + "." + argLG.SortField);
                object oldValorOrdenacao = oldValues.returnValueField(Alias + "." + argLG.SortField);

                // To the new value of the relationship, we add the difference.
                if (!Equals(valorRel, oldValorRel) || !Equals(novoValorLG, oldValorLG) || !Equals(novoValorOrdenacao, oldValorOrdenacao)
                // We must always process records that change from the pseudo new state (zzstate 1 or 11) to the valid one and those that are removed.
                    || delete || oldValues.Zzstate != 0)
                {
                    //AJA 2016-04-06 - Verifica se a relação exists. Se estiver fazia não calcula a formula.
                    if (!Field.isEmptyValue(valorRel, formatacaoRel))
                        AuxUpdateLG(sp, argLG, relacao, valorRel);
                    if (!delete && !Field.isEmptyValue(oldValorRel, formatacaoRel) && !Equals(valorRel, oldValorRel))
                        AuxUpdateLG(sp, argLG, relacao, oldValorRel);
                }
            }
        }

        private void AuxUpdateLG(PersistentSupport sp, ListAggregateArgument argLG, Relation relacao, object valorRel)
        {
            DbArea outra = (DbArea)Area.createArea(relacao.AliasTargetTab, user, module);
            outra.UserRecord = false;
            sp.getRecord(outra, valorRel);

            var sql = sp.getLGQuery(this.TableName, argLG, relacao, valorRel);
            object Qvalue = sp.executeScalar(sql);
            string valorLG = "";
            if (Qvalue != DBNull.Value)
                valorLG = (string)Qvalue;

            outra.insertNameValueField(outra.Alias + "." + argLG.LGField, valorLG);
            outra.update(sp);
        }

		/// <summary>
		/// Função to propagar réplicas
		/// </summary>
		/// <param name="valorChavePrimaria">Value da key primária da ficha que vai ser propagada</param>
		/// <param name="sp">suporte persistente que esta a ser utilizado</param>
		public void propagateReplicas(PersistentSupport sp, Area oldValues)
		{
			try
			{
				if (Information.FieldsParametersReplicas == null)
                    return;

                //we group all the updates to a table into this dictionary
                Dictionary<string, UpdateQuery> updates = new Dictionary<string, UpdateQuery>();

                for (int i = 0; i < Information.FieldsParametersReplicas.Length; i++)
                {
                    Field campoReplica = DBFields[Information.FieldsParametersReplicas[i]];
                    object valorReplica = returnValueField(Alias + "." + campoReplica.Name);
                    object oldReplica = oldValues.returnValueField(Alias + "." + campoReplica.Name);
                    //só propaga se o Qvalue mudou
                    if (!valorReplica.Equals(oldReplica))
                    {
                        foreach (ReplicaDestination target in campoReplica.ReplicaDestinationList)
                        {
                            UpdateQuery uq = null;
                            updates.TryGetValue(target.ReplicaDestinationTable+"_"+target.ForeignKey, out uq);
                            if (uq == null)
                            {
                                // MH (25/09/2017) - Alterado to utilizar "destino.TabelaDestinoReplica" em vez do "Alias"  no Where do UpdateQuery.
                                // Alias referencia a table atual e não a table que vamos change. Ex: Alias: "factura" e TargetTable: "linhas da fatura".
                                uq = new UpdateQuery().Update(target.ReplicaDestinationSystem, target.ReplicaDestinationTable)
                                    .Where(CriteriaSet.And().Equal(target.ReplicaDestinationTable, target.ForeignKey, QPrimaryKey));
                                updates.Add(target.ReplicaDestinationTable+"_"+target.ForeignKey, uq);
                            }

                            uq.Set(target.ReplicaTargetFields, ((campoReplica.FieldType == FieldType.CHAVE_FALSA_GUID || campoReplica.FieldType == FieldType.CHAVE_PRIMARIA_GUID || campoReplica.FieldType == FieldType.CHAVE_ESTRANGEIRA_GUID) && String.Equals(valorReplica, "")) ?
                                    null :
                                    valorReplica);
                        }
                    }
                }

                //Then we do the actual updates to the database
                foreach (UpdateQuery uq in updates.Values)
                    sp.Execute(uq);
			}
			catch (GenioException ex)
			{
				throw new BusinessException(ex.UserMessage, "DbArea.propagarReplicas", "Error propagating replicas: " + ex.Message, ex);
			}
		}

        /// <summary>
        /// Garante que os registos de somas parciais (ST) estão criados
        /// </summary>
        /// <param name="sp">suporte persistente que esta a ser utilizado</param>
        /// <returns>cria o registo na table que agrupa os fields usados na fórmula ST</returns>
        private void criarRegistosST(PersistentSupport sp)
        {
            try
            {
                if (this.Information.SumCreateRecords != null)
                {
                    bool camposPreenchidos = true;
                    SumsCreatesRecords somaCriaRegistos;
                    object[] valoresOrigem;
                    string[] camposDestinoCompletos;

                    for (int i = 0; i < this.Information.SumCreateRecords.Length; i++)
                    {
                        somaCriaRegistos = this.Information.SumCreateRecords[i];
                        valoresOrigem = new object[somaCriaRegistos.STSourceFields.Length];
                        camposDestinoCompletos = new string[somaCriaRegistos.STSourceFields.Length];
                        for (int j = 0; j < somaCriaRegistos.STSourceFields.Length; j++)
                        {
                            valoresOrigem[j] = returnValueField(Alias + "." + somaCriaRegistos.STSourceFields[j]);
                            camposDestinoCompletos[j] = somaCriaRegistos.AliasTargetTab + "." + somaCriaRegistos.STTargetFields[j];
                            //se algum dos fields de source estiver vazio não vamos criar a ficha
                            if (((Field)DBFields[somaCriaRegistos.STSourceFields[j]]).isEmptyValue(valoresOrigem[j]))
                            {
                                camposPreenchidos = false;
                                break;
                            }
                        }
                        if (camposPreenchidos)
                        {
                            //se o registo ainda não exists então introduce uma nova ficha com a soma dos Qvalues
                            if (!sp.exists(somaCriaRegistos.STTargetFields, somaCriaRegistos.TargetTable, valoresOrigem))
                            {
                                // Create-se uma ficha na table a cima, correndo as regras de negócio
                                // to garantir que as possíveis propagações do registo são feitas
                                // (por oposição ao insertDirect).
                                // É necessário ter cuidado com o encadeamento de fórmulas, porque
                                // esta lógica pode gerar ciclos infinitos. Caso aconteça tem de se
                                // re-pensar as definições ou até mesmo a forma de propagação das ST
								DbArea areaST = (DbArea)Area.createArea(somaCriaRegistos.AliasTargetTab, User, Module);
                                areaST.UserRecord = false;
                                for (int j = 0; j < valoresOrigem.Length; j++)
                                    areaST.insertNameValueField(camposDestinoCompletos[j], valoresOrigem[j]);
                                areaST.insert(sp);
                                insertNameValueField(this.Alias + "." + somaCriaRegistos.TargetRelField, areaST.QPrimaryKey);
                            }
                            // o registo já exists e só precisamos de actualizar
                            else
                            {
                                ArrayList camposChave = sp.returnFieldsListConditions(new string[] { somaCriaRegistos.TargetIntKey }, somaCriaRegistos.TargetTable, somaCriaRegistos.STTargetFields, valoresOrigem);
                                insertNameValueField(this.Alias + "." + somaCriaRegistos.TargetRelField, camposChave[0]);
                            }
                        }
                    }
                }
            }
            catch (GenioException ex)
			{
				throw new BusinessException(ex.UserMessage, "DbArea.criarRegistosST", "Error creating partial sum records: " + ex.Message, ex);
			}
            catch (Exception ex)
            {
                throw new BusinessException(null, "DbArea.criarRegistosST", "Error creating partial sum records: " + ex.Message, ex);
            }
        }

        /************************FIM DO CALCULO DE FORMULAS***************************************/

		/// <summary>
		/// função que apaga os ficheiros externos referenciados pela ficha
		/// </summary>
		/// <param name="sp">suporte persistente que esta a ser utilizado</param>
		public void deleteExternalFiles(PersistentSupport sp)
		{
			try
			{
				object primaryKeyValue = QPrimaryKey;
				IEnumerator enumficheiros = this.Fields.Values.GetEnumerator();
				while (enumficheiros.MoveNext())
				{
					RequestedField Qfield = (RequestedField)enumficheiros.Current;
					if (Qfield.FieldType.Equals(FieldType.PATH))
					{
						object fieldValue = sp.returnField(this, Qfield.Name, primaryKeyValue);
						FileInfo originalFile = new FileInfo(Configuration.PathDocuments + "\\" + fieldValue);
						if (originalFile.Exists)
							originalFile.Delete();
					}
				}
			}
			catch (GenioException ex)
			{
				throw new BusinessException("Erro ao apagar os ficheiros.", "DbArea.apagarFicheirosExternos", "Error deleting external files: " + ex.Message, ex);
			}
			catch (Exception ex)
			{
				throw new BusinessException("Erro ao apagar os ficheiros.", "DbArea.apagarFicheirosExternos", "Error deleting external files: " + ex.Message, ex);
			}
		}

		/// <summary>
		/// função que apaga os ficheiros referenciados pela ficha e que estão na table DOCUMS
		/// </summary>
		/// <param name="sp">suporte persistente que esta a ser utilizado</param>
		public void deleteFilesDB(PersistentSupport sp)
		{
			try
			{
				object primaryKeyValue = QPrimaryKey;
				if (Information.DocumsForeignKeys != null)
					for (int j = 0; j < Information.DocumsForeignKeys.Count; j++)
					{
						object fieldValue = sp.returnField(this, Information.DocumsForeignKeys[j], primaryKeyValue);

						sp.deleteRecordDocums("documid",fieldValue);
					}
			}
			catch (GenioException ex)
			{
				if (ex.UserMessage == null)
					throw new BusinessException("Erro ao apagar os ficheiros.", "DbArea.apagarFicheirosBD", "Error deleting database files: " + ex.Message, ex);
				else
					throw new BusinessException("Erro ao apagar os ficheiros: " + ex.UserMessage, "DbArea.apagarFicheirosBD", "Error deleting database files: " + ex.Message, ex);
			}
			catch (Exception ex)
			{
				throw new BusinessException("Erro ao apagar os ficheiros.", "DbArea.apagarFicheirosBD", "Error deleting database files: " + ex.Message, ex);
			}
		}

		/// <summary>
		/// Função to verificar se houve alterações que implicam a criação do histórico
		/// </summary>
		/// <param name="area">area que vai ser actualizada</param>
		/// <param name="utilizador">User em sessão</param>
		/// <param name="tabelaCriaHist">name da table de criahist</param>
		public bool verifyChangesHistory(Area areaDb, User user, History history)
		{
			try
			{
				IEnumerator enumficheiros = areaDb.Fields.Values.GetEnumerator();
				while (enumficheiros.MoveNext())
				{
					RequestedField Qfield = (RequestedField)enumficheiros.Current;
					if (((Field)DBFields[Qfield.Name]).CreateHist == history.CreateHistTables && Fields.ContainsKey(Alias + "." + Qfield.Name))
					{
						if (!Qfield.Value.Equals(((RequestedField)Fields[Alias + "." + Qfield.Name]).Value))
							return true;
					}
				}
				return false;
			}
			catch (GenioException ex)
			{
				throw new BusinessException(ex.UserMessage, "DbArea.verificarAlteracoesHistorico", "Error checking history changes: " + ex.Message, ex);
			}
		}

		/// <summary>
		/// Função to a criação do histórico
		/// </summary>
		/// <param name="utilizador">User em sessão</param>
		/// <param name="sp">suporte persistente que esta a ser utilizado</param>
		public void createHistory(PersistentSupport sp, Area oldvalues)
		{
			try
			{
                if (HistoryList == null)
                    return;

				//Area areaDb = Area.createArea(Alias, User, Module);
				History history;

				for (int i = 0; i < HistoryList.Count; i++)
				{
					history = HistoryList[i];
					int totalCamposCriaHist = history.CreateHistFields.Length;

					string[] nomesCamposBd = new string[totalCamposCriaHist];
					string[] nomesCamposCriaHist = new string[totalCamposCriaHist];
					object[] fieldsvalues = new object[totalCamposCriaHist];
					for (int j = 0; j < totalCamposCriaHist; j++)
					{
						nomesCamposBd[j] = Alias + "." + history.CreateHistFields[j];
						nomesCamposCriaHist[j] = history.CreateHistTables + "." + history.CreateHistFields[j];
                        if (Fields.ContainsKey(Alias + "." + history.CreateHistFields[j]))
                            fieldsvalues[j] = ((RequestedField)Fields[Alias + "." + history.CreateHistFields[j]]).Value;
					}
                    bool pseudoToNew = oldvalues.Zzstate == 1 && this.Zzstate == 0;

                    //The record is not pseudo or is no longer a pseudo
                    if (this.Zzstate == 0 && verifyChangesHistory(oldvalues, user, history) || pseudoToNew)
					{
						string tabelaCriaHist = history.CreateHistTables;
						Area areaHist = Area.createArea(tabelaCriaHist, User, Module);

						//introduce fields da table de histórico
						areaHist.insertNamesFields(new string[] { });
						for (int j = 0; j < totalCamposCriaHist; j++)
                            areaHist.insertNameValueField(nomesCamposCriaHist[j], fieldsvalues[j]);

						//introduce Qfield da key primária
						RequestedField campoPedido = new RequestedField(tabelaCriaHist + "." + areaHist.PrimaryKeyName, tabelaCriaHist);
						FieldType fieldType = ((Field)areaHist.DBFields[campoPedido.Name]).FieldType;
						campoPedido.FieldType = fieldType;
						areaHist.Fields.Add(tabelaCriaHist + "." + areaHist.PrimaryKeyName, campoPedido);
						areaHist.insertNameValueField(tabelaCriaHist + "." + PrimaryKeyName, QPrimaryKey);
						areaHist.insertPseud(sp, new string[] { }, new string[] { });
						areaHist.change(sp, (CriteriaSet)null);
					}
				}
			}
			catch (GenioException ex)
			{
				throw new BusinessException(ex.UserMessage, "DbArea.criarHistorico", "Error creating history: " + ex.Message, ex);
			}
		}

		/// <summary>
		/// Método que permite eliminate um registo, pressupoe uma ligação à base de dados aberta
		/// </summary>
		/// <param name="sp">Suporte Persistente</param>
		/// <returns></returns>
		public override StatusMessage eliminate(PersistentSupport sp)
		{
            try
            {
				if (Log.IsDebugEnabled) Log.Debug(string.Format("Area.eliminar [area] {0}", Alias));
                object valorCodigoObj = QPrimaryKey;
                if (valorCodigoObj == null)
					// RMR(2017-03-27) - Whenever the record didn't exit, the system would block and no more work was allowed
					return StatusMessage.OK("Registo não encontrado.");

                delete(sp, this);
                return StatusMessage.OK("Deletion successful");
            }
            catch (GenioException ex)
            {
                throw new BusinessException(ex.UserMessage, "DbArea.eliminar " + Alias, "Error deleting record in DbArea: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new BusinessException(null, "DbArea.eliminar " + Alias, "Error deleting record in DbArea: " + ex.Message, ex);
            }
		}

        public override StatusMessage eliminateDependent(PersistentSupport sp, Area rootRecord)
        {
            try
            {
                //Sometimes the dependent record is deleted by other dependency, so we have to do this check to avoid deleting something that doesn't exist
                if (!sp.Exists(PrimaryKeyName, TableName, QPrimaryKey))
                    return StatusMessage.OK("Record not found. Deletion skipped");

                delete(sp, rootRecord);
                return StatusMessage.OK("Deletion successful");
            }
            catch (GenioException ex)
            {
                throw new BusinessException(ex.UserMessage, "DbArea.eliminar " + Alias, "Error deleting record in DbArea: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new BusinessException(null, "DbArea.eliminar " + Alias, "Error deleting record in DbArea: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Método to eliminate um registo
        /// </summary>
        /// <param name="sp">Persistent support</param>
        /// <param name="sp">The root record that originated the deletion. Pass self otherwise</param>
        private void delete(PersistentSupport sp, Area rootRecord)
        {
            //ler os Qvalues da ficha antiga
            Area oldvalues = Area.createArea(this.Alias, user, module);
            sp.getRecord(oldvalues, QPrimaryKey);
            //durante o apagar os Qvalues novos são sempre iguais aos antigos
            CloneFrom(oldvalues);

            if (UserRecord)
            {
                //verificar os direitos de acesso se não estivermos a cancelar uma ficha pseudo-nova
                if (Zzstate == 0)
                {
                    //check validations to delete a record
                    var result = CanDelete(sp);
                    if (!result.Status.Equals(Status.OK))
                        throw new InvalidAccessException(result, ConditionType.DELETE);
				}
            }

            beforeEliminate(sp);

            DeleteDependencies(sp, rootRecord, oldvalues);

			if (Zzstate == 0)
            {
				insertQueue(sp, "D", null, null); // à imagem do que é feito no backoffice, a queue é enviada imediatamente antes de ser apagado o registo.
                MessageQueue(sp, "D", null);
            }

            // CX 2011.10.18: Os ficheiros têm de ser apagados antes da ficha, senão não se consegue obter o Qvalue da key to apagar os ficheiros.
            deleteFilesDB(sp);
            sp.deleteRecord(this, QPrimaryKey);
            // CX 2011.10.18: Os ficheiros externos não devem ser apagados pois podem ser utilizados noutras fichas.
            // From qq das maneiras o código existente não os estava a apagar pois a ficha estava a ser apagada primeiro
            // e já não era possível obter o name do file a apagar.
            //deleteExternalFiles(sp);

            afterEliminate(sp);

            // Inserts a record with the copy of the values into the shadow table
            if (!string.IsNullOrEmpty(ShadowTabName))
                sp.requestTabShadow(this, User.Name, FunctionType.ELI);

            if (Zzstate == 0)
            {
				propagateLinkedSum(sp, oldvalues, true);
				propagarUltimosValores(sp, oldvalues, true);
				propagarFimPeriodo(sp, oldvalues, true);
                propagateListAggregate(sp, oldvalues, true); //AJA concatena linhas
            }
            //  updateOnDelete
        }

        /// <summary>
        /// Find and delete all dependencies from this record. Throw exception if it's not possible.
        /// </summary>
        /// <param name="sp">Persistent Support</param>
        /// <param name="rootRecord">The record where the deletion request was originated</param>
        /// <param name="oldvalues">Old record values</param>
        private void DeleteDependencies(PersistentSupport sp, Area rootRecord, Area oldvalues)
        {
            if (rootRecord == this)
            {
                //Check if it can be deleted
                CheckDependencies(sp);
            }

            ChildRelation[] tabsFilha = ChildTable;
            if (tabsFilha != null)
            {
                // MH (27/01/2020) - Indicates whether we will need to update the fields values after deleting the records from the child table.
                // Because of the propagation of formulas, such as SR.
                var requireUpdateValues = false;
                int nrFilhos = tabsFilha.Length;
                for (int i = 0; i < nrFilhos; i++)
                {
                    //SO 2007.05.29
                    Area childTable = Area.createArea(tabsFilha[i].ChildArea, User, User.CurrentModule);

                    //2014.03.19 AP - Ignora tables Personalizadas
                    //TODO: Criar Um método onde possa ser avaliada a validação da table personalizada
                    if (childTable.Information.PersistenceType.Equals(CSGenio.business.PersistenceType.Codebase) || childTable.Information.PersistenceType.Equals(CSGenio.business.PersistenceType.View))
                        continue;

                    childTable.UserRecord = false;
                    ArrayList filhos = sp.existsChild(tabsFilha[i].RelatedFields, childTable, QPrimaryKey);
                    if (filhos.Count != 0)
                    {
                        if (tabsFilha[i].ProcWhenDelete.Equals(DeleteProc.AP) || (tabsFilha[i].ProcWhenDelete.Equals(DeleteProc.AN) && rootRecord.Zzstate == 1))
                        {
                            for (int j = 0; j < filhos.Count; j++)
                            {
                                childTable.insertNameValueField(childTable.Alias + "." + childTable.PrimaryKeyName, filhos[j].ToString());
                                //vai recursivamente apagar as fichas filhas
                                childTable.eliminateDependent(sp, rootRecord);
                                requireUpdateValues = true;
                            }
                        }
                        else
                        {
                            if (tabsFilha[i].ProcWhenDelete.Equals(DeleteProc.DM) ||
                            tabsFilha[i].ProcWhenDelete.Equals(DeleteProc.NA) && tabsFilha[i].ChildArea.Equals(this.Alias)) // Use case: Domain A, Area B of Domain A. Table A refer B. In cases of deletion A records, delete all references to A in other records as well.
                            {
                                for (int j = 0; j < tabsFilha[i].RelatedFields.Length; j++)//20061122
                                {
                                    //TODO: isto não está a respeitar as regras de business das formulas internas das tables actualizadas
                                    sp.deleteRelationship(childTable, tabsFilha[i].RelatedFields[j].ToString(), QPrimaryKey);
                                }
                            }
                            else
                            {
                                string strMsg = Translations.Get("O registo não pode ser eliminado porque existem registos relacionados.", user.Language);
                                string strTable = Translations.Get("Tabela", user.Language);
                                string srtDesig = Translations.Get(childTable.AreaDesignation, user.Language);
                                string strMsgUser = strMsg + " (" + strTable + ": " + srtDesig + ")";
                                throw new BusinessException(strMsgUser, "DbArea.apagar", "The record with code " + QPrimaryKey + " of the table " + this.Alias.ToUpper() + " has related records and can't be deleted. The related table: " + childTable.Alias.ToUpper());
                            }
                        }
                    }
                }

                if (requireUpdateValues)
                {
                    // MH (27/01/2020) - Read the old values from the database, which may be updated by propagation of the formulas.
                    // For example: C-> B-> A, where A has the SR of B and B has the SR of C.
                    // If B also has On delete rule of the C, the value of the SR in table A will be incorrect.
                    oldvalues.removeCalculatedFields();
                    sp.getRecord(oldvalues, QPrimaryKey);
                    // During deletion, new values are always the same as in the database.
                    CloneFrom(oldvalues);
                }
            }
        }

        /// <summary>
        /// Checks it there are errors in the StatusMessage and throws a MultiException in that case
        /// </summary>
        /// <param name="Qresult">The result SM we will merge all information</param>
        /// <param name="validationResults">The SM with the results of the validations</param>
        /// <param name="location">Where this is being called in the code</param>
        private void CheckErrorMessages(StatusMessage Qresult, StatusMessage validationResults, string location)
        {
            if (validationResults.Status == Status.E)
            {
                throw new FieldValidationException(validationResults, location);
            }
            Qresult.MergeStatusMessage(validationResults);
        }

        /// <summary>
        /// Checks if this record can be changed.
        /// Checks both change conditions and table access rights
        /// </summary>
        /// <returns>A status message with all the cumulated results</returns>
        public StatusMessage CanChange(PersistentSupport sp)
        {
            var result = EvaluateCrudConditions(sp, user, ConditionType.UPDATE);
            if (!accessRightsToChange())
            {
                var message = Translations.Get("Não tem permissões para alterar o registo.", User.Language);
                result.MergeStatusMessage(StatusMessage.Error(message));
            }
            return result;
        }

        /// <summary>
        /// Checks if this record can be inserted.
        /// Checks both change conditions and table access rights
        /// </summary>
        /// <returns>A status message with all the cumulated results</returns>
        public StatusMessage CanInsert(PersistentSupport sp)
        {
            var result = EvaluateCrudConditions(sp, user, ConditionType.INSERT);
            if (!AccessRightsToCreate())
            {
                var message = Translations.Get("Não tem permissões para criar o registo.", User.Language);
                result.MergeStatusMessage(StatusMessage.Error(message));
            }
            return result;
        }

        /// <summary>
        /// Checks if this record can be deleted.
        /// Checks both change conditions and table access rights
        /// </summary>
        /// <returns>A status message with all the cumulated results</returns>
        public StatusMessage CanDelete(PersistentSupport sp)
        {
            var result = EvaluateCrudConditions(sp, user, ConditionType.DELETE);
            if (!accessRightsToDelete())
            {
                var message = Translations.Get("Não tem permissões para apagar o registo.", User.Language);
                result.MergeStatusMessage(StatusMessage.Error(message));
            }
            return result;
        }

        private FunctionType ConditionToFunctionType(ConditionType type)
        {
            switch(type)
            {
                case ConditionType.VIEW:
                    return FunctionType.GET;
                case ConditionType.UPDATE:
                    return FunctionType.ALT;
                case ConditionType.DELETE:
                    return FunctionType.ELI;
                case ConditionType.INSERT:
                    return FunctionType.INS;
                default:
                    return FunctionType.ALT;
            }
        }

        /// <summary>
        /// Evaluates all crud conditions of a certain type and returns a status message with the result
        /// </summary>
        /// <param name="sp">Persistent support for conditions that need to get values</param>
        /// <param name="user">The user info to get the localization</param>
        /// <param name="type">The condition type we want to check</param>
        /// <returns>A status message with the cumulated results</returns>
        public StatusMessage EvaluateCrudConditions(PersistentSupport sp, User user, ConditionType type)
        {
            StatusMessage result = StatusMessage.OK();
            var conditions = Information.CrudConditions.Where(c=> c.Type == type);
            //Update and delete conditions also view data, so they must also obey the View conditions
            if (type == ConditionType.UPDATE || type == ConditionType.DELETE)
                conditions = Information.CrudConditions.Where(c => c.Type == ConditionType.VIEW).Union(conditions);

            if (conditions != null && conditions.Any())
            {
                using(new ScopedPersistentSupport(sp))
                {
                    foreach (var condition in conditions)
                    {
                        try
                        {
                            bool condResult = condition.ExecuteCondition(this, sp, ConditionToFunctionType(type));
                            if (!condResult)
                            {
                                var status = StatusMessage.Error(condition.GetMessage(user));
                                result.MergeStatusMessage(status);
                            }
                        }
                        catch (GenioException exc)
                        {
                            Log.Error($"Error evaluating table condition in table {this.AreaDesignation}:{exc.UserMessage}");
                        }
                    }
                }
            }

            return result;
        }


        /// <summary>
        /// Validates all duplication conditions returns a boolean with the result
        /// </summary>
        /// <param name="sp">Persistent support for conditions that need to get values</param>
        /// <param name="user">The user info to get the localization</param>
        /// <param name="condArea">The area that corresponds to the one where the condition is defined</param>
        /// <returns>A boolean representing their validity</returns>
        public bool ValidateDupConditions(PersistentSupport sp, string condArea)
        {
            IEnumerable<DupConditionFormula> conditions = Information.DupConditions;

            foreach (DupConditionFormula condition in conditions)
            {
                //We only want to validade the conditions for the area where they are
                //declared, due to the way they are defined
                if (condition.CondArea != condArea)
                    continue;

                try
                {
                    bool res = condition.ExecuteCondition(this, sp, FunctionType.ALT);

                    if (!res)
                        return false;
                }
                catch (GenioException exc)
                {
                    Log.Error($"Error evaluating table condition in table {this.AreaDesignation}:{exc.UserMessage}");
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Testa se o user tem direitos de Acesso to apagar
        /// </summary>
        /// <returns>true se o user pode apagar, false caso contrário</returns>
        public bool accessRightsToDelete()
        {
            return AccessRightsToDelete(User);
        }

        /// <summary>
        /// Check if there are any delete dependencis that prevent the record from being deleted
        /// </summary>
        /// <exception>Throws a BusinessException if the record cannot be deleted</exception>
        private void CheckDependencies(PersistentSupport sp)
        {
            var areas = FindDeleteDependencies(sp, new List<ChildRelation>(), this);
            if (areas.Any())
            {
                string strMsg = Translations.Get("O registo não pode ser eliminado porque existem registos relacionados.", user.Language);
                string strTable = Translations.Get("Tabela", user.Language);
                string srtDesig = String.Join(",", areas.Select(a => Translations.Get(a.AreaDesignation, user.Language)));

                string strMsgUser = strMsg + " (" + strTable + ": " + srtDesig + ")";
                throw new BusinessException(strMsgUser, "DbArea.apagar", "The record with code " + QPrimaryKey + " of the table " + this.Alias.ToUpper() + " has related records and can't be deleted. The related tables: " + string.Join(",", areas.Select(a=>a.Alias)));
            }
        }

        /// <summary>
        /// From a list of relations, check if there are any dependencies that stop the record deletion and return them
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="prevRelations">List of </param>
        /// <param name="rootZzstate"></param>
        /// <returns></returns>
        private List<AreaInfo> FindDeleteDependencies(PersistentSupport sp,List<ChildRelation> prevRelations, DbArea rootRecord)
        {
            List<AreaInfo> areas = new List<AreaInfo>();
            if (ChildTable == null || ChildTable.Length == 0)
                return areas;

            foreach (ChildRelation relation in ChildTable)
            {
                string deleteProc = relation.ProcWhenDelete.toString();
                var childInfo = GetInfoArea(relation.ChildArea);
                var isTreeRelation = childInfo.TableName == this.Information.TableName;

                if (deleteProc == DeleteProc.CLEAR //Clear relations are always possible
                    || childInfo.PersistenceType != PersistenceType.Database)  //Views and code tables should not be verified here
                    continue;

                //Clone the list of previous relations, add self and check for dependencies
                var relations = new List<ChildRelation>(prevRelations);
                relations.Add(relation);
                /*
                    Due to the low frequency of occurrence, we will not apply in-depth validation to tree structures.
                    We let proceed with the removal and it's will throw an error if has a dependent record that can't be removed (as the old behavior).
                 */
                if (isTreeRelation || !rootRecord.HasDependencies(sp, relations))
                    continue;

                if (deleteProc == DeleteProc.DONT_DELETE ||
                    (deleteProc == DeleteProc.DELETE_IF_NEW && rootRecord.Zzstate == 0))
                {
                    //This dependencies will stop the deletion
                    areas.Add(childInfo);
                }
                else if (deleteProc == DeleteProc.DELETE_RECORD ||
                    (deleteProc == DeleteProc.DELETE_IF_NEW && rootRecord.Zzstate != 0))
                {
                    //Recursively check if the child records can be deleted
                    DbArea childArea = (DbArea)createArea(relation.ChildArea, user, module);
                    var dependencies = childArea.FindDeleteDependencies(sp, relations, rootRecord);
                    areas.AddRange(dependencies);
                }
            }

            return areas;
        }

        /// <summary>
        /// Check if there is any records by following this relation path
        /// </summary>
        /// <param name="relations">A relation path from this record to the last leaf</param>
        /// <returns>True if there are any records</returns>
        private bool HasDependencies(PersistentSupport sp, List<ChildRelation> relations)
        {
            SelectQuery query = new SelectQuery();
            query.Select(SqlFunctions.Count("1"), "numRecords");
            query.From(this.TableName, this.Alias);
            string parentAlias = this.Alias;
            string parentKey = this.PrimaryKeyName;
            foreach (var relation in relations)
            {
                var area = GetInfoArea(relation.ChildArea);
                var criteriaSet = CriteriaSet.Or();
                foreach (var foreignKey in relation.RelatedFields)
                    criteriaSet.Equal(parentAlias, parentKey, area.Alias, foreignKey);

                query.Join(area.TableName, area.Alias, TableJoinType.Inner).On(criteriaSet);
            }
            query.Where(CriteriaSet.And().Equal(parentAlias, parentKey, QPrimaryKey));

            //JGF 2021.06.16 MySQL databases return bigint for counts so the return value may be either an int or a long.
            // Due to C# boxing and unboxing we must do this ugly multiplexing
            var valCount = sp.ExecuteScalar(query);
            if (valCount is int)
                return ((int) valCount) > 0;
            else
                return ((long) valCount) > 0;

        }

		/// <summary>
		/// Método que permite change um registo
		/// pressupoe uma ligação à BD
		/// </summary>
		/// <param name="sp">Suporte Persistente</param>
		/// <param name="condition">Condição de alteração</param>
		/// <returns></returns>
		[Obsolete("Use StatusMessage change(PersistentSupport sp, CriteriaSet condition) instead")]
		public virtual StatusMessage change(PersistentSupport sp, string condition)
		{
			StatusMessage Qresult = StatusMessage.GetAggregator();

            try
            {
				if (Log.IsDebugEnabled) Log.Debug(string.Format("Area.alterar [area] {0}", Alias));

                //ler a key primária
                string codIntValue = QPrimaryKey;
                //não podemos change o registo sem uma key primária
                if (string.IsNullOrEmpty(codIntValue))
                    throw new BusinessException(null, "DbArea.alterar", "ChavePrimaria is null.");

                //ler os Qvalues da ficha antiga
                Area oldvalues = Area.createArea(this.Alias, user, module);
                sp.getRecord(oldvalues, codIntValue);

                //garantir que todos os fields estão preenchidos, se o interface não forneceu um Qvalue então usamos o antigo
                //isto permite ás rotinas seguintes não ter de sistematicamente tentar fazer queries à BD
                foreach (string key in oldvalues.Fields.Keys)
                    if (!Fields.ContainsKey(key))
                        Fields[key] = new RequestedField(oldvalues.Fields[key] as RequestedField);

                //Acontece nos pedidos GET1 com em dbedits com fields dependentes
                removeFieldsOtherAreas(); //TODO: Não devia ser possivel a área chegar a este estado

                if (UserRecord)
                {
					//carimbar a ficha caso tenham existido mudanças
					if (Zzstate == 0)
						fillStampChange();

					//mudar o estado do zzstate
					Zzstate = 0;

					//verificar permissões de escrita
					if (!accessRightsToChange())
						throw new BusinessException("Não tem permissões para alterar o registo.", "DbArea.alterar", "No permissions to change records.");
                }

                //calcular formulas internas (+, +H, ++, CT, FP)
                //os numeros sequenciais são recalculados sempre que se detecta que estão a vazio ou que não são únicos
                fillInternalOperations(sp, oldvalues);

                // validar o registo
                // (RS 2011.06.30) Quando não é o user a gravar a ficha não devemos validar as outras fichas porque podem ainda estar incompletas
                // No entanto isto pode deixar gravar fichas num estado invalido. Tem de se avaliar se devemos só evitar esta validação no caso de fichas
                // pseudo-novas, ou seja, onde o oldzzstate != 0
                if (UserRecord)
                {
                    var validationResults = Validation.validateFieldsChange(this, sp, user);
                    CheckErrorMessages(Qresult, validationResults, "DbArea.alterar");
                }

                // Os registos ST são propagados imediatamente antes de gravar o registo
                // caso contrário obrigava a re-gravar esta ficha depois do change to
                // actualizar os Qvalues das chaves estrangeiras to a area da ST
                criarRegistosST(sp);

                Dictionary<string, RequestedField> tempValoresSequenciais = new Dictionary<string, RequestedField>();

                if (Zzstate != 0)
                {
                    // antes de change um registo que não é gravado por um user, tem de se
                    // remover os números sequênciais to continuarem com o Qvalue negativo na BD
                    if (SequentialDefaultValues != null)
                    {
                        foreach (string camposeq in SequentialDefaultValues)
                        {
                            tempValoresSequenciais.Add(Alias + "." + camposeq, (RequestedField)fields[Alias + "." + camposeq]);
                            removeFieldValue(Alias + "." + camposeq);
                        }
                    }
                }

                //persistir o registo, optimizando to só gravar as mudanças
                sp.change(this);

                foreach (KeyValuePair<string, RequestedField> Qfield in tempValoresSequenciais)
                    fields[Qfield.Key] = Qfield.Value;

                //propagar alterações to outros registos
                propagateReplicas(sp, oldvalues);
                propagarUltimosValores(sp, oldvalues, false);
                propagateLinkedSum(sp, oldvalues, false);
                propagarFimPeriodo(sp, oldvalues, false);
                propagateListAggregate(sp, oldvalues, false); //concatena linhas

                //criar fichas de shadow e de history
                createHistory(sp, oldvalues);

                //enviar mensagem de message queueing
				if (Zzstate == 0)
                {
					insertQueue(sp, oldvalues.Zzstate == 0 ? "U" : "C", oldvalues, null);
                    MessageQueue(sp, oldvalues.Zzstate == 0 ? "U" : "C", oldvalues);
                }
			}
			catch (GenioException ex)
			{
				if (ex.ExceptionSite == "DbArea.alterar")
					throw;
				throw new BusinessException(ex.UserMessage, "DbArea.alterar " + Alias, "Error changing record in DbArea: " + ex.Message, ex);
			}
            catch (Exception ex)
            {
                throw new BusinessException(null, "DbArea.alterar " + Alias, "Error changing record in DbArea: " + ex.Message, ex);
            }

            return Qresult.MergeStatusMessage(StatusMessage.OK("Alteração bem sucedida."));
		}

		public override StatusMessage change(PersistentSupport sp, CriteriaSet condition)
		{
			StatusMessage Qresult = StatusMessage.GetAggregator();
			StatusMessage validationResults = null;
            try
            {
				if (Log.IsDebugEnabled) Log.Debug(string.Format("Area.alterar [area] {0}", Alias));

                //ler a key primária
                string codIntValue = QPrimaryKey;
                //não podemos change o registo sem uma key primária
                if (string.IsNullOrEmpty(codIntValue))
				{
                    throw new BusinessException(null, "DbArea.alterar", "ChavePrimaria is null.");
				}

                //ler os Qvalues da ficha antiga
                Area oldvalues = Area.createArea(this.Alias, user, module);
                sp.getRecord(oldvalues, codIntValue, true);

                //garantir que todos os fields estão preenchidos, se o interface não forneceu um Qvalue então usamos o antigo
                //isto permite ás rotinas seguintes não ter de sistematicamente tentar fazer queries à BD
                foreach (string key in oldvalues.Fields.Keys)
				{
                    if (!Fields.ContainsKey(key))
					{
                        Fields[key] = new RequestedField(oldvalues.Fields[key] as RequestedField);
					}
				}

                //Acontece nos pedidos GET1 com em dbedits com fields dependentes
                removeFieldsOtherAreas(); //TODO: Não devia ser possivel a área chegar a este estado

                if (UserRecord)
                {
					//carimbar a ficha caso tenham existido mudanças
					if (Zzstate == 0)
						fillStampChange();

					//mudar o estado do zzstate
					Zzstate = 0;

					 //check validations to change a record
                    var result = CanChange(sp);
                    if (!result.Status.Equals(Status.OK))
                        throw new InvalidAccessException(result, ConditionType.UPDATE);
                }

                //Formatação automática de campos
                formatFields();

                //calcular formulas internas (+, +H, ++, CT, FP)
                //os numeros sequenciais são recalculados sempre que se detecta que estão a vazio ou que não são únicos
				fillInternalOperations(sp, oldvalues);

                // validar o registo
                // (RS 2011.06.30) Quando não é o user a gravar a ficha não devemos validar as outras fichas porque podem ainda estar incompletas
                // No entanto isto pode deixar gravar fichas num estado invalido. Tem de se avaliar se devemos só evitar esta validação no caso de fichas
                // pseudo-novas, ou seja, onde o oldzzstate != 0
                if (UserRecord)
                {
					validationResults = Validation.validateFieldsChange(this, sp, user);
                    CheckErrorMessages(Qresult, validationResults, "DbArea.alterar");
                }

				//Validações e cálculos custom
                Qresult.MergeStatusMessage(beforeUpdate(sp, oldvalues));
                if (Qresult.Status.Equals(Status.E))
                {
                    throw new BusinessException(Qresult.Message, "DbArea.alterar", "Error updating record: " + Qresult.Message);
                }

                // Os registos ST são propagados imediatamente antes de gravar o registo
                // caso contrário obrigava a re-gravar esta ficha depois do change to
                // actualizar os Qvalues das chaves estrangeiras to a area da ST
				criarRegistosST(sp);

                Dictionary<string, RequestedField> tempValoresSequenciais = new Dictionary<string, RequestedField>();

                if (Zzstate != 0)
                {
                    // antes de change um registo que não é gravado por um user, tem de se
                    // remover os números sequênciais to continuarem com o Qvalue negativo na BD
                    if (SequentialDefaultValues != null)
                    {
                        foreach (string camposeq in SequentialDefaultValues)
                        {
                            tempValoresSequenciais.Add(Alias + "." + camposeq, (RequestedField)fields[Alias + "." + camposeq]);
                            removeFieldValue(Alias + "." + camposeq);
                        }
                    }
                }

                //persistir o registo, optimizando to só gravar as mudanças
				sp.change(this);

                foreach (KeyValuePair<string, RequestedField> Qfield in tempValoresSequenciais)
                    fields[Qfield.Key] = Qfield.Value;

                //propagar alterações to outros registos
                propagateReplicas(sp, oldvalues);
                propagarUltimosValores(sp, oldvalues, false);
                propagateLinkedSum(sp, oldvalues, false);
                propagarFimPeriodo(sp, oldvalues, false);
                propagateListAggregate(sp, oldvalues, false); //concatena linhas

                //criar fichas de shadow e de history
                createHistory(sp, oldvalues);

                //enviar mensagem de message queueing
				if (Zzstate == 0)
                {
					insertQueue(sp, oldvalues.Zzstate == 0 ? "U" : "C", oldvalues, null);
                    MessageQueue(sp, oldvalues.Zzstate == 0 ? "U" : "C", oldvalues);
                }

                //Validações e cálculos custom
                Qresult.MergeStatusMessage(afterUpdate(sp, oldvalues));
                if (Qresult.Status.Equals(Status.E))
                {
                    throw new BusinessException(Qresult.Message, "DbArea.alterar", "Error updating record: " + Qresult.Message);
                }
			}
			catch (GenioException ex)
			{
				if (ex.ExceptionSite == "DbArea.alterar")
					throw;
				throw new BusinessException(ex.UserMessage, "DbArea.alterar " + Alias, "Error changing record in DbArea: " + ex.Message, ex);
			}
            catch (Exception ex)
            {
                throw new BusinessException(null, "DbArea.alterar " + Alias, "Error changing record in DbArea: " + ex.Message, ex);
            }

            if (Qresult.Status != Status.W)
            {
				if (validationResults != null && validationResults.Status == Status.OK && !string.IsNullOrEmpty(validationResults.Message))
					return validationResults;

                Qresult.MergeStatusMessage(StatusMessage.OK());
            }
            return Qresult;
		}

		/// <summary>
        /// Função que verifica se um registo pode ser alterado
		/// </summary>
		/// <param name="sp">Suporte Persistente</param>
        /// <returns>true se pode change, false caso contrário</returns>
        public bool accessRightsToChange()
        {
            return user.GetModuleRoles(module).Any(role => QLevel.CanChange(role));
        }

        /// <summary>
        /// Checks if a user has access rights to create a record in this area.
        /// </summary>
        /// <returns></returns>
        public bool AccessRightsToCreate()
        {
            return User.GetModuleRoles(module).Any(role => QLevel.CanCreate(role));
        }

        [Obsolete("Please use inserir(sp) instead")]
        public StatusMessage insertPseud(PersistentSupport sp, bool shadow)
        {
            return inserir_WS(sp);
        }

		/// <summary>
		/// Método que permite introduce um registo na base de dados,
		/// pressupoe uma ligação aberta
		/// </summary>
		/// <param name="sp">Suporte Persistente</param>
        /// <returns></returns>
        public override Area insertPseud(PersistentSupport sp)
        {
			return insertPseud(sp, new string[] { }, new string[] { });
        }

		/// <summary>
		/// Método que permite introduce um registo na base de dados como uma ficha pseudo-nova (zzstate = 1)
		/// </summary>
		/// <param name="sp">Suporte Persistente</param>
		/// <param name="condicao">Condição de seleção dos registos</param>
		/// <returns></returns>
        public override Area insertPseud(PersistentSupport sp, string[] fieldNames, string[] fieldsvalues)
		{
            try
            {
				if (Log.IsDebugEnabled) Log.Debug(string.Format("Area.inserir [area] {0}", Alias));

                beforeInsert(sp);

                // key primária
                string codInt = sp.codIntInsertion(this, false);

                // AV(2010/09/20) As fichas novas deixam de ter registos com fields NULL e passam a ter com os Qvalues vazios apropriados
                removeCalculatedFields();
                createEmptyFields();

                Zzstate = 1;
                QPrimaryKey = codInt;

                if (UserRecord)
                {
					//check validations to insert a record
                    var result = CanInsert(sp);
                    if (!result.Status.Equals(Status.OK))
                        throw new InvalidAccessException(result, ConditionType.INSERT);

					//1 - preencher carimbo
					fillStampInsert();
                }

                //2 - prencher fields sequenciais
                calculateTemporarySequentials();
                //3 - preencher Qvalues default
				fillValuesDefault(sp, FunctionType.INS);
                //4 - Qvalues enviados pelo cliente
                addNamesValuesFields(fieldNames, fieldsvalues);
                //5 - preencher operações internas
				fillInternalOperations(sp, null);

				base.insertPseud(sp, fieldNames, fieldsvalues);

                afterInsert(sp);
            }
            catch (GenioException ex)
			{
				if (ex.ExceptionSite == "DbArea.inserir")
					throw;
				throw new BusinessException(ex.UserMessage, "DbArea.inserir " + Alias, "Error inserting record in DbArea: " + ex.Message, ex);
			}
            catch (Exception ex)
            {
                throw new BusinessException(null, "DbArea.inserir " + Alias, "Error inserting record in DbArea: " + ex.Message, ex);
            }

            return this;
		}

        /// <summary>
        /// Insere um novo registo com os dados actuais correndo as regras de negócio.
        /// A key primária ficará preenchida no fim da execução da operação
        /// </summary>
        /// <param name="sp">O suporte de persistence</param>
        public void insert(PersistentSupport sp)
        {
            inserir_WS(sp);
        }

        /// <summary>
        /// Insere um novo registo com os dados actuais sem correr as regras de negócio
        /// A key primária ficará preenchida no fim da execução da operação
        /// </summary>
        /// <param name="sp">O suporte de persistence</param>
        public void insertDirect(PersistentSupport sp)
        {
            sp.insertPseud(this);
        }

        /// <summary>
        /// Actualiza um registo existente com os dados actuais correndo as regras de negócio.
        /// </summary>
        /// <param name="sp">O suporte de persistence</param>
        public void update(PersistentSupport sp)
        {
            change(sp, (CriteriaSet)null);
        }

        /// <summary>
        /// Actualiza um registo existente com os dados actuais sem correr as regras de negócio.
        /// </summary>
        /// <param name="sp">O suporte de persistence</param>
        public void updateDirect(PersistentSupport sp)
        {
            sp.change(this);
        }

        /// <summary>
        /// Actualiza um registo existente com os dados actuais com validação das regras de business escolhidas no Genio.
        /// </summary>
        /// <param name="sp">O suporte de persistence</param>
        public override void apply(PersistentSupport sp, bool isGoingBack = false)
        {
            //ler os Qvalues da ficha antiga
            if (string.IsNullOrEmpty(QPrimaryKey))
            {
                throw new BusinessException(null, "DbArea.apply", "ChavePrimaria is null.");
            }
            Area oldvalues = Area.createArea(this.Alias, user, module);
            sp.getRecord(oldvalues, QPrimaryKey);

            if (!isGoingBack)
            {
                //garantir que todos os fields estão preenchidos, se o interface não forneceu um Qvalue então usamos o antigo
                //isto permite ás rotinas seguintes não ter de sistematicamente tentar fazer queries à BD
                foreach (string key in oldvalues.Fields.Keys)
                {
                    if (!Fields.ContainsKey(key))
                    {
                        Fields[key] = new RequestedField(oldvalues.Fields[key] as RequestedField);
                    }
                }
            }

            //Acontece nos pedidos GET1 com em dbedits com fields dependentes
            removeFieldsOtherAreas(); //TODO: Não devia ser possivel a área chegar a este estado

            //Fill changed operators
            if (Zzstate == 0)
                fillStampChange();

            fillInternalOperations(sp, oldvalues);

            var validationResults = Validation.validateFieldsChange(this, sp, User, true);
            if (Zzstate != 0)
                calculateTemporarySequentials();

            CheckErrorMessages(StatusMessage.GetAggregator(), validationResults, "DbArea.apply");
            updateDirect(sp);

			// Propagate changes to other records
            propagateReplicas(sp, oldvalues);
            propagarUltimosValores(sp, oldvalues, false);
            propagateLinkedSum(sp, oldvalues, false);
            propagarFimPeriodo(sp, oldvalues, false);
            // To test in the future
            //propagateListAggregate(sp, oldvalues, false);
        }

        /// <summary>
        /// Apaga um registo existente com os dados actuais correndo as regras de negócio.
        /// </summary>
        /// <param name="sp">O suporte de persistence</param>
        public void delete(PersistentSupport sp)
        {
            eliminate(sp);
        }

        /// <summary>
        /// Apaga um registo existente com os dados actuais sem correr as regras de negócio.
        /// </summary>
        /// <param name="sp">O suporte de persistence</param>
        public void deleteDirect(PersistentSupport sp)
        {
            string valchave = (string)returnValueField(Information.Alias + "." + Information.PrimaryKeyName);
            sp.deleteRecord(this, valchave);
        }

		[Obsolete("Please use inserir_WS(sp) instead")]
        public StatusMessage inserir_WS(string module, bool shadow, PersistentSupport sp)
        {
            return inserir_WS(sp);
        }

		/// <summary>
		/// Método to introduce um registo que fica imediatamente disponivel (zztate=0)
		/// </summary>
		/// <param name="modulo">módulo</param>
		/// <returns>o status e a mensagem resposta da inserção</returns>
		public override StatusMessage inserir_WS(PersistentSupport sp)
		{
            StatusMessage Qresult = StatusMessage.GetAggregator();

            try
			{
                //guardar os Qvalues default que ja possam ter sido preenchidos
                Hashtable temp = new Hashtable();
                foreach (DictionaryEntry cpvl in Fields)
                {
                    if (DefaultValues != null && Array.Exists(DefaultValues, X => (Alias + "." + X) == cpvl.Key.ToString()))
                        temp.Add(cpvl.Key, ((RequestedField)cpvl.Value).Value);
                    else if (SequentialDefaultValues != null && Array.Exists(SequentialDefaultValues, X => (Alias + "." + X) == cpvl.Key.ToString()))
                        temp.Add(cpvl.Key, ((RequestedField)cpvl.Value).Value);
                }

                //INSERT PHASE
                //--------------------------------------------------------------------
                if (Log.IsDebugEnabled) Log.Debug(string.Format("Area.inserir [area] {0}", Alias));

                // key primária
                string codInt = sp.codIntInsertion(this, false);

                // AV(2010/09/20) As fichas novas deixam de ter registos com fields NULL e passam a ter com os Qvalues vazios apropriados
                removeCalculatedFields();
                createEmptyFields();

                Zzstate = 1;
                QPrimaryKey = codInt;

                if (UserRecord)
                {
                    //check validations to insert a record
                    var result = CanInsert(sp);
                    if (!result.Status.Equals(Status.OK))
                        throw new InvalidAccessException(result, ConditionType.INSERT);

                    //1 - preencher carimbo
                    fillStampInsert();
					//NH(2021.12.22) - Removed change stamp in the method "inserir_WS"
					//fillStampChange(); //To já mantenho o carimbo alt to ficar a mesma semântica do metodo anterior. Reavaliar no futuro.
                }

                //Formatação automática de campos
                formatFields();

                //3 - preencher Qvalues default
                fillValuesDefault(sp, FunctionType.INS);
                //--------------------------------------------------------------------

                //devolver os Qvalues dos defaults que vieram do exterior
                foreach (DictionaryEntry cpvl in temp)
                    ((RequestedField)Fields[cpvl.Key]).Value = cpvl.Value;

                //Aqui queremos sempre garantir que o zzstate passa a 0
                Zzstate = 0;

                //UPDATE PHASE
                //--------------------------------------------------------------------

                //ler os Qvalues da ficha antiga (não ha nada to ler, é o equivalente a uma ficha vazia
                Area oldvalues = Area.createArea(this.Alias, user, module);
                //simular a alteração de uma ficha vazia to uma ficha preenchida
                oldvalues.Zzstate = 1;

                //garantir que todos os fields estão preenchidos, se o interface não forneceu um Qvalue então usamos o antigo
                //isto permite ás rotinas seguintes não ter de sistematicamente tentar fazer queries à BD
                foreach (string key in oldvalues.Fields.Keys)
                {
                    if (!Fields.ContainsKey(key))
                    {
                        Fields[key] = new RequestedField(oldvalues.Fields[key] as RequestedField);
                    }
                }

                //calcular formulas internas (+, +H, ++, CT, FP)
                //os numeros sequenciais são recalculados sempre que se detecta que estão a vazio ou que não são únicos
                fillInternalOperations(sp, oldvalues);

                // validar o registo
                // (RS 2011.06.30) Quando não é o user a gravar a ficha não devemos validar as outras fichas porque podem ainda estar incompletas
                // No entanto isto pode deixar gravar fichas num estado invalido. Tem de se avaliar se devemos só evitar esta validação no caso de fichas
                // pseudo-novas, ou seja, onde o oldzzstate != 0
                if (UserRecord && NeedsValidation)
                {
                    var validationResults = Validation.validateFieldsChange(this, sp, user);
                    CheckErrorMessages(Qresult, validationResults, "DbArea.alterar");
                }

                //Validações e cálculos custom
                Qresult.MergeStatusMessage(beforeUpdate(sp, oldvalues));
                if (Qresult.Status.Equals(Status.E))
                {
                    throw new BusinessException(Qresult.Message, "DbArea.alterar", "Error updating record: " + Qresult.Message);
                }

                // Os registos ST são propagados imediatamente antes de gravar o registo
                // caso contrário obrigava a re-gravar esta ficha depois do change to
                // actualizar os Qvalues das chaves estrangeiras to a area da ST
                criarRegistosST(sp);

                sp.insertPseud(this);

                //propagar alterações to outros registos
                // propagateReplicas(sp, oldvalues); //<-- Numa insercao nunca precisamos de propagar replicas pois não existem fichas abaixo
                propagarUltimosValores(sp, oldvalues, false);
                propagateLinkedSum(sp, oldvalues, false);
                propagarFimPeriodo(sp, oldvalues, false);
                propagateListAggregate(sp, oldvalues, false); //concatena linhas

                //criar fichas de shadow e de history
                createHistory(sp, oldvalues);

                //enviar mensagem de message queueing
				insertQueue(sp, "C", null, null);
                MessageQueue(sp, "C", null);
                //--------------------------------------------------------------------

                //Validações e cálculos custom
                Qresult.MergeStatusMessage(afterUpdate(sp, oldvalues));
                if (Qresult.Status.Equals(Status.E))
                {
                    throw new BusinessException(Qresult.Message, "DbArea.alterar", "Error updating record: " + Qresult.Message);
                }
            }
            catch (GenioException ex)
			{
				throw new BusinessException(ex.UserMessage, "DbArea.inserir_WS", "Error inserting record in DbArea: " + ex.Message, ex, ex.ErrorStack);
			}

            if (Qresult.Status != Status.W)
            {
                Qresult.MergeStatusMessage(StatusMessage.OK("Inserção bem sucedida."));
            }
            return Qresult;
        }

		/// <summary>
		/// Função que permite duplicate um registo
		/// pressupoe a existência de uma ligação à BD
		/// </summary>
		/// <param name="sp">Suporte Persistente</param>
		/// <param name="condition">Condição de seleção</param>
		/// <returns></returns>
        [Obsolete("Use Area duplicate(PersistentSupport sp, CriteriaSet condition) instead")]
		public virtual Area duplicate(PersistentSupport sp, string condition)
		{
			string[] split = condition.Split('=');
            if (split.Length < 2)
                throw new BusinessException(null, "Area.duplicar", "Invalid condition: " + condition);
            string codeValue = split[1].Trim('\'');

			try
			{
				if (Log.IsDebugEnabled) Log.Debug(string.Format("Area.duplicar [area] {0}", Alias));

                //ir buscar o registo to duplicate
                sp.getRecord(this, codeValue);
                string codInt = sp.codIntInsertion(this, false);
                zeroDuplicar();
                QPrimaryKey = codInt;
                Zzstate = 1;

                //1 - preencher carimbo
                if (UserRecord)
                {
                    //TODO: validar direitos de acesso
                    fillStampInsert();
                }

                //2 - prencher fields sequenciais
                calculateTemporarySequentials();

                //4 - fill defaults on empty fields
                fillValuesDefault(sp, FunctionType.DUP);

                //5 - operações internas que dependem de números sequenciais
                fillInternalOperations(sp, null);

                //6 - duplicate os documentos na db
                sp.duplicateFilesDB(this, codInt, false);
                sp.insertPseud(this);

                //duplicate as fichas relacionadas
                List<FieldRef> fieldsToUpdate = tambemDuplica(sp, codeValue.ToString());

                // Reload formula fields (SR and UV) when cascade duplicate
                reloadFormulaModelFields(sp, fieldsToUpdate);
            }
            catch (GenioException ex)
			{
                throw new BusinessException(ex.UserMessage, "DbArea.duplicar " + Alias, "Error duplicating record in DbArea: " + ex.Message, ex);
			}
            catch (Exception ex)
            {
                throw new BusinessException(null, "DbArea.duplicar " + Alias, "Error duplicating record in DbArea: " + ex.Message, ex);
			}

			return this;
		}

        public override Area duplicate(PersistentSupport sp, CriteriaSet condition)
        {
            if (condition == null || condition.Criterias.Count == 0)
            {
                throw new BusinessException(null, "DbArea.duplicar " + Alias, "Invalid condition: " + condition);
            }

            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("Area.duplicar [area] {0}", Alias));

                //ir buscar o registo to duplicate
                object codeValue = condition.Criterias[0].RightTerm;
                sp.getRecord(this, codeValue);
                string codInt = sp.codIntInsertion(this, false);

                //zerar os fields declarados com zeroAduplicar
                zeroDuplicar();

                QPrimaryKey = codInt;
                Zzstate = 1;

                //1 - preencher carimbo
                if (UserRecord)
                {
                    //TODO: validar direitos de acesso
                    fillStampInsert();
                }

                //2 - prencher fields sequenciais
                calculateTemporarySequentials();

                //4 - fill defaults on empty fields
                fillValuesDefault(sp, FunctionType.DUP);

                //5 - operações internas que dependem de números sequenciais
                fillInternalOperations(sp, null);

                beforeDuplicate(sp);

                //6 - duplicate os documentos na db
                sp.duplicateFilesDB(this, codInt, false);
                sp.insertPseud(this);

                //duplicate as fichas relacionadas
                List<FieldRef> fieldsToUpdate = tambemDuplica(sp, codeValue.ToString());

                // Reload formula fields (SR and UV) when cascade duplicate
                reloadFormulaModelFields(sp, fieldsToUpdate);

                afterDuplicate(sp);
            }
            catch (GenioException ex)
            {
                throw new BusinessException(ex.UserMessage, "DbArea.duplicar " + Alias, "Error duplicating record in DbArea: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new BusinessException(null, "DbArea.duplicar " + Alias, "Error duplicating record in DbArea: " + ex.Message, ex);
            }

            return this;
        }

        /// <summary>
        /// Zera todos os fields que tem definido o zeroAduplicar
        /// </summary>
        private void zeroDuplicar()
        {
            IEnumerator enumCampos = Fields.Values.GetEnumerator();
            List<string> camposToZero = new List<string>();
            var camposSR = new List<string>(this.RelatedSumFields ?? new string[] { }); // To simplificar o código na validação e não ter que lidar com array vazio
            while (enumCampos.MoveNext())
            {
                RequestedField campoPedido = (RequestedField)enumCampos.Current;
                if (DBFields[campoPedido.Name] != null)
                {
                    Field campoBD = (Field)DBFields[campoPedido.Name];
                    //RMR(2017-06-01) - Whenever a record is duplicated, every DATAMUDA/HORAMUDA/OPERMUDA should also be reseted
                    if (campoBD.ZeroDuplication || campoBD.FieldType == FieldType.DATAMUDA || campoBD.FieldType == FieldType.OPERMUDA || campoBD.FieldType == FieldType.HORAMUDA
                        //MH(2018-03-14) - The target fields of the SRs must be reseted
                        || camposSR.Contains(campoBD.Name)
                        //MH(2023-03-17) - The password fields can never be duplicated!
                        || campoBD.FieldType == FieldType.PASSWORD)
                        camposToZero.Add(campoBD.Alias + "." + campoBD.Name);
                }
            }

            for (int i = 0; i < camposToZero.Count; i++)
                insertNameValueField(camposToZero[i], null);
        }

        /// <summary>
        /// Reloads the formula model fields with DB data that have formulas
        /// based on field affected by cascade duplicade
        /// </summary>
        /// <param name="sp">The persistent support object.</param>
        /// <param name="modelFieldsToUpdate">The list of fields to update.</param>
        private void reloadFormulaModelFields(PersistentSupport sp, List<FieldRef> modelFieldsToUpdate)
        {
            if(modelFieldsToUpdate.Count() < 1) return;

            // Group fields by table
            var fieldGroups = modelFieldsToUpdate.GroupBy(field => field.Area).ToList();

            foreach (var fieldGroup in fieldGroups)
            {
                // Initialize the DbArea for the current table
                DbArea fieldArea = (DbArea)Area.createArea(fieldGroup.Key, User, User.CurrentModule);

                // Fetch all field records for the current table in one go
                var fieldNames = fieldGroup.Select(f => f.Field).ToArray();
                sp.getRecord(fieldArea, QPrimaryKey, fieldNames);

                // Replace DB value in the model fields
                foreach(var field in fieldGroup)
                {
                    string key = (field.Area + "." + field.Field).ToLower();
                    Fields[key] = fieldArea.Fields[key];
                }
            }
        }

        private List<FieldRef> loadFieldsToUpdate(DbArea area)
        {
            /*
            * In this method we only need to reload the values of the Formula
            * fields that are related to the child table duplicated values, since
            * these are calculated with cascade duplicate and the value is updated
            * in the database but not in the modal.
            *
            * The other formulas like Replicas and End of Period don't need to be included
            * here because we fetch the formula is calculated over the parent, and not over
            * the children.
            */
            List<FieldRef> fieldsToUpdate = new List<FieldRef>();

            // SR
            area.RelatedSumArgs?.ForEach(rel =>
                fieldsToUpdate.Add(new FieldRef(rel.AliasSR, rel.SRField))
            );

            //UV
            area.LastValueArgs?.ForEach(rel =>
            {
                foreach (var field in rel.LVRFields)
                    fieldsToUpdate.Add(new FieldRef(rel.AliasRUV, field));
            });

            // List Aggregate
            area.ArgsListAggregate?.ForEach(rel => fieldsToUpdate.Add(new FieldRef(rel.AliasLG, rel.LGField)));

            return fieldsToUpdate;
        }

        private List<FieldRef> tambemDuplica(PersistentSupport sp, string codIntValue)
        {
            List<FieldRef> modelFieldsToUpdate = new List<FieldRef>();

            if (DuplicationRelations != null)
            {
                //registo das chaves que já foram duplicadas to cada area
                //assim sabemos sempre 1) que registos já foram duplicados 2) qual a antiga e nova key desses registos
                Dictionary<string, Dictionary<string, string>> areasDuplicadas = new Dictionary<string, Dictionary<string, string>>();

                //inicializamos com a key deste proprio registo (que já foi duplicada)
                var fichasDuplicadas = new Dictionary<string, string>();
                areasDuplicadas.Add(this.TableName, fichasDuplicadas);
                fichasDuplicadas.Add(codIntValue, QPrimaryKey);

                //Calcular a lista em cascata das relações a percorrer
                List<Relation> cascata = CalcularCascataDuplicacao();

                foreach (Relation relacao in cascata)
                {
                    //criar um novo mapeamento to esta table
                    //convém determinar se já exists primeiro to o caso de existirem relações exclusivas em losango
                    areasDuplicadas.TryGetValue(relacao.SourceTable, out fichasDuplicadas);
                    if (fichasDuplicadas == null)
                    {
                        fichasDuplicadas = new Dictionary<string, string>();
                        areasDuplicadas.Add(relacao.SourceTable, fichasDuplicadas);
                    }

                    //pegar em cada elemento duplicado da table de source e duplicate todos os elementos
                    //eventualmente isto pode ser optimizado to uma só query
                    //mas é preciso pesar se o WHERE cod IN (carradas de Qvalues) conpensa
                    var filhasParaDuplicar = areasDuplicadas[relacao.TargetTable];
                    string condArea = relacao.AliasTargetTab.ToUpper();
                    foreach (var filha in filhasParaDuplicar)
                    {
                        ArrayList duplicacoes = sp.existsChild(relacao.SourceRelField, relacao.SourceIntKey, relacao.SourceSystem, relacao.SourceTable, relacao.AliasSourceTab, filha.Key);
                        DbArea areaChild = (DbArea)Area.createArea(relacao.AliasSourceTab, User, User.CurrentModule);

                        // Load fields to update on the parent table
                        modelFieldsToUpdate = loadFieldsToUpdate(areaChild);

                        //RMR(2022-11-11) - If it has more child record to duplicate after, it cannot enforce conditions
                        areaChild.NeedsValidation = false;
                        if (cascata.Where(x=>x.TargetTable == relacao.SourceTable).Count() == 0)
                            areaChild.NeedsValidation = true;

                        foreach (var dup in duplicacoes)
                        {
							if (!fichasDuplicadas.ContainsKey(dup.ToString()))
                            {
                                //Fetch current records
                                //We do this to make sure the duplicate condition is validated
                                //based on the original values and not the duplicated ones
                                sp.getRecord(areaChild, dup);

                                if (areaChild.ValidateDupConditions(sp, condArea)) //Validate Duplicate Conditions
                                    if (areaChild.duplicarFilha(sp, dup.ToString(), areasDuplicadas)) //Duplicate Record
                                        fichasDuplicadas.Add(dup.ToString(), areaChild.QPrimaryKey);
                            }
                        }
                    }
                }
            }

            return modelFieldsToUpdate;
        }

		private List<Relation> CalcularCascataDuplicacao()
        {
            //TODO: Calculate this at generation time, where we already have the tables ordering available
            List<Relation> res = new List<Relation>();
            List<string> visitados = new List<string>();
            int cabeca = 0;
            visitados.Add(this.Alias);

            while (cabeca < visitados.Count)
            {
                var node = visitados[cabeca];
                var info = Area.GetInfoArea(node);

                if (info.DuplicationRelations != null)
                {
                    res.AddRange(info.DuplicationRelations);
                    foreach (var source in info.DuplicationRelations)
                        if (!visitados.Contains(source.AliasSourceTab))
                            visitados.Add(source.AliasSourceTab);
                }
                cabeca++;
            }

            return res;
		}

        /// <summary>
        /// Função que permite duplicate uma table filha
        /// </summary>
        /// <param name="area">Name da área filha a ser duplicada</param>
        /// <param name="valorCodInt">Qvalue do código interno da área filha a ser duplicada</param>
        /// <param name="campoMae">Name da key primária da table mãe</param>
        /// <param name="valorCampoMae">Qvalue da key primária da table mãe</param>
        /// <returns>true se a filha foi duplicada, false caso contrário</returns>
        private bool duplicarFilha(PersistentSupport sp, string codIntValue, Dictionary<string, Dictionary<string, string>> areasDuplicadas)
        {
            //TODO: falta o suporte to a duplicação em cascata
            sp.getRecord(this, codIntValue);
            string codInt = sp.codIntInsertion(this, false);

            // Last updated by [CJP] at [2016.06.01]
            // Não deve duplicate os registos filhos com ZZSTATE != 0
            if (Zzstate != 0)
                return false;

            //Como é uma duplicação em cascata temos de considerar que é como se fosse o proprio user a introduce as fichas abaixo.
            //RMR(2022-11-11) - Removed force to true because this is decided in the "tambemDuplica" function, in case it has child to duplicate with conditions
            //UserRecord = true;

            //actualizar chaves estrangeiras dos Qvalues antigos to os novos
            foreach (var r in this.ParentTables)
            {
                var acima = r.Value.TargetTable;
                if (areasDuplicadas.ContainsKey(acima))
                {
                    string nomeCe = Alias + "." + r.Value.SourceRelField;
                    string valorCeAntigo = this.returnValueField(nomeCe).ToString();

                    areasDuplicadas[acima].TryGetValue(valorCeAntigo, out string valorCeNovo);
                    if (valorCeNovo != null)
                        this.insertNameValueField(nomeCe, valorCeNovo);
                }
            }

            //zerar os fields declarados com zeroAduplicar
            zeroDuplicar();
            QPrimaryKey = codInt;

            //1 - preencher carimbo
            fillStampInsert();

            //No need to fill the sequencial fields with negative values
            //Since we always want to keep the value when duplicating,
            //Even if the value is empty or invalid, it will be handled next

            //4 - fill defaults on empty fields
            fillValuesDefault(sp, FunctionType.DUP);

            //5 - operações internas que dependem de números sequenciais
            fillInternalOperations(sp, null);
            //Duplicate docums
            string newcodDocums = sp.duplicateFilesDB(this, codInt, false);
            
            //RS 24.04.2017 Passa a efectuar todas as regras de business durante a duplicação.
            insert(sp);

            //This is not the best way to update the field "chave" from Docums table.
            //May be, we should not use this field because it creates a bidirectionl relationship with other tables.
            //There is one place where the field "chave" is used, but it could be unused if we refactory the content of document ticket. 
            if (!string.IsNullOrEmpty(newcodDocums))
            {
                UpdateQuery uq = new UpdateQuery()
                .Update("docums")
                .Set("chave", QPrimaryKey)
                .Where(CriteriaSet.And()
                    .Equal("docums", "coddocums", newcodDocums));

                sp.Execute(uq);
            } 

            return true;
        }

        public virtual bool checkoutDocums(PersistentSupport sp, string docField, out string newcodDocums)
        {
            string documField = Alias + "." + docField + "fk";
			string valorLigacao = returnValueField(documField).ToString();
            string codtable = QPrimaryKey;

            newcodDocums = "";
            DataMatrix resultados = this.returnValuesDocums(
				sp,
                new SelectField[] { new SelectField(SqlFunctions.Count(0), "count") },
                CriteriaSet.And()
                    .Equal("docums", "versao", "CHECKOUT")
                    .Equal("docums", "documid", valorLigacao),
				null,
                docField);

            if (DBConversion.ToInteger(resultados.GetDirect(0, 0)) > 0)
                return false;

            newcodDocums = sp.duplicateFilesDB(this, codtable, true, documField);

            UpdateQuery uq = new UpdateQuery()
                .Update("docums")
                .Set("opercria", user.Name)
                .Where(CriteriaSet.And()
                    .Equal("docums", "coddocums", newcodDocums));

			sp.Execute(uq);

            return true;
        }

        public bool removeDocums(PersistentSupport sp, string docField)
        {
            string valorLigacao = returnValueField(Alias + "." + docField + "fk").ToString();
            if (string.IsNullOrEmpty(valorLigacao))
                return true;

            DataMatrix resultados = this.returnValuesDocums(
                sp,
                new[] { new SelectField(SqlFunctions.Count(0), "count") },
                CriteriaSet.And()
                    .Equal("docums", "versao", "CHECKOUT")
                    .Equal("docums", "documid", valorLigacao),
                null,
                docField);

            if (DBConversion.ToInteger(resultados.GetDirect(0, 0)) > 0)
                return false;

            sp.deleteRecordDocums("documid", valorLigacao);
            insertNameValueField(Alias + "." + docField, null);
            insertNameValueField(Alias + "." + docField + "fk", null);

            return true;
        }

        public virtual DBFile infoDocum(PersistentSupport sp, string docField)
        {
            string connectionVal = returnValueField(Alias + "." + docField + "fk").ToString();
            return infoDocum(sp, docField, connectionVal, true);
        }

        public virtual DBFile infoDocum(PersistentSupport sp, string docField, string documentId, bool isForeignKey)
        {
            //          file.gif,215.0 bytes,gif,@web,11/11/2010 16:30:56,1
            //            |1:23209sd23b3gb3gb33b3212b[1.1:23209s231121b33b32152h[2:23209sd25211231323b32123
            //            |[CHECKOUT]

            DataMatrix resultados = this.returnValuesDocums(
                sp,
                new[]
                {
                    new SelectField(new ColumnReference("docums", "nome"), "nome"),
                    new SelectField(new ColumnReference("docums", "tamanho"), "tamanho"),
                    new SelectField(new ColumnReference("docums", "extensao"), "extensao"),
                    new SelectField(new ColumnReference("docums", "opercria"), "opercria"),
                    new SelectField(new ColumnReference("docums", "datacria"), "datacria"),
                    new SelectField(new ColumnReference("docums", "versao"), "versao"),
                    new SelectField(new ColumnReference("docums", "coddocums"), "coddocums"),
                    new SelectField(new ColumnReference("docums", "documid"), "documid")
                },
                CriteriaSet.And()
                    .NotEqual("docums", "versao", "CHECKOUT")
                    .Equal("docums", isForeignKey ? "documid" : "coddocums", documentId),
                new[]
                {
                    new ColumnSort(DOCUMS_SORT_COLUMN1, SortOrder.Descending),
                    new ColumnSort(DOCUMS_SORT_COLUMN2, SortOrder.Descending)
                },
                docField,
                isForeignKey);

            if (resultados == null)
                return DBFile.EmptyFile();

            if (resultados.NumRows == 0)
                return DBFile.EmptyFile();

			string name = resultados.GetDirect(0, 0).ToString(); //name
            int size = DBConversion.ToInteger(resultados.GetDirect(0, 1)); //size
            string fileType = resultados.GetDirect(0, 2).ToString(); //extension
            string author = resultados.GetDirect(0, 3).ToString(); //opercria
            string createdAt = resultados.GetDirect(0, 4).ToString(); //datacria
            string version = resultados.GetDirect(0, 5).ToString(); //Qversion
            string coddocums = resultados.GetDirect(0,6).ToString(); //coddocums
            string documId = resultados.GetDirect(0, 7).ToString(); //Identificador do documento
            SortedList<string, string> versions = new SortedList<string, string>(new VersionComparer());
            bool isCheckout = false;
            string checkoutEditor = "";
            string currentUser = user.Name;
            resultados = this.returnValuesDocums(
                sp,
                new[]
                {
                    new SelectField(new ColumnReference("docums", "versao"), "versao"),
                    new SelectField(new ColumnReference("docums", "coddocums"), "coddocums"),
                    new SelectField(new ColumnReference("docums", "opercria"), "opercria")
                },
                CriteriaSet.And()
                    .Equal("docums", isForeignKey ? "documid" : "coddocums", documentId),
                new[]
                {
                    new ColumnSort(DOCUMS_SORT_COLUMN1, SortOrder.Ascending),
                    new ColumnSort(DOCUMS_SORT_COLUMN2, SortOrder.Ascending)
                },
                docField,
                isForeignKey);

            for (int i = 0; i < resultados.NumRows; i++)
            {
                string Qversion = resultados.GetDirect(i, 0).ToString();
                string codDocums = DBConversion.ToString(resultados.GetDirect(i, 1));
                string opercria = DBConversion.ToString(resultados.GetDirect(i, 2));

                if (Qversion.ToUpper().Equals("CHECKOUT"))
                {
                    coddocums = codDocums;
                    isCheckout = true;
                    checkoutEditor = opercria;
                }
                else
                    versions.Add(Qversion, codDocums);
            }

            return new DBFile(coddocums, name, fileType, version, size, author, createdAt, documId, versions, isCheckout, checkoutEditor, currentUser);
        }

        public bool deleteLastDocums(PersistentSupport sp, string docField)
        {
            string valorLigacao = returnValueField(Alias + "." + docField + "fk") as string;

            DataMatrix resultados = this.returnValuesDocums(
                sp,
                new[]
                {
                    new SelectField(new ColumnReference("docums", "coddocums"), "coddocums"),
                    new SelectField(new ColumnReference("docums", "nome"), "nome")
                },
                CriteriaSet.And()
                    .NotEqual("docums", "versao", "CHECKOUT")
                    .Equal("docums", "documid", valorLigacao),
                new[]
                {
                    new ColumnSort(DOCUMS_SORT_COLUMN1, SortOrder.Descending),
                    new ColumnSort(DOCUMS_SORT_COLUMN2, SortOrder.Descending)
                },
                docField);

            try
            {
                sp.openTransaction();

                sp.deleteRecordDocums("coddocums", resultados.GetDirect(0, 0).ToString());
                //actualiza name file pelo penultimo
                this.insertNameValueField(this.Alias + "." + docField, resultados.GetDirect(1, 1).ToString());
                this.change(sp, (CriteriaSet)null);

                sp.closeTransaction();
                return true;
            }
            catch
            {
                sp.rollbackTransaction();
                return false;
            }
        }

        public void deleteHistoryDocums(PersistentSupport sp, string docField)
        {
            deleteHistoryDocums(sp, docField, null);
        }

        public void deleteHistoryDocums(PersistentSupport sp, string docField, string currentVersion)
        {
            string valorLigacao = returnValueField(Alias + "." + docField + "fk") as string;

            DataMatrix resultados = this.returnValuesDocums(
                sp,
                new[]
                {
                    new SelectField(new ColumnReference("docums", "coddocums"), "coddocums"),
                    new SelectField(new ColumnReference("docums", "versao"), "versao")
                },
                CriteriaSet.And()
                    .NotEqual("docums", "versao", "CHECKOUT")
                    .Equal("docums", "documid", valorLigacao),
                new[]
                {
                    new ColumnSort(new ColumnReference(CSGenioAdocums.FldDatacria), SortOrder.Descending)
                },
                docField);

            if (resultados.NumRows <= 1)
                return;

            string versionToKeep = currentVersion ?? resultados.GetDirect(0, 1).ToString();

            for (int i = 0; i < resultados.NumRows; i++)
            {
                string version = resultados.GetDirect(i, 1).ToString();
                if (version != versionToKeep)
                    sp.deleteRecordDocums("coddocums", resultados.GetDirect(i, 0).ToString());
            }
        }

        public string[] returnLastVersionDocum(PersistentSupport sp, string docField)
        {
            string valorLigacao = returnValueField(Alias + "." + docField + "fk") as string;
            if (string.IsNullOrEmpty(valorLigacao))
                return null;

            string []result = new string[2];

            //versão mais recente
            DataMatrix resultados = this.returnValuesDocums(
                sp,
                new[] { new SelectField(new ColumnReference("docums", "versao"), "versao") },
                CriteriaSet.And()
                    .NotEqual("docums", "versao", "CHECKOUT")
                    .Equal("docums", "documid", valorLigacao),
                new[]
                {
                    new ColumnSort(DOCUMS_SORT_COLUMN1, SortOrder.Descending),
                    new ColumnSort(DOCUMS_SORT_COLUMN2, SortOrder.Descending)
                },
                docField);
            result[0] = resultados.NumRows > 0 ? DBConversion.ToString(resultados.GetDirect(0, 0)) : null;
            //codigo da key primária da versão checkout
            resultados = this.returnValuesDocums(
                sp,
                new[] { new SelectField(new ColumnReference("docums", "coddocums"), "coddocums") },
                CriteriaSet.And()
                    .Equal("docums", "versao",  "CHECKOUT")
                    .Equal("docums", "documid", valorLigacao),
                null,
                docField);

            //number da versão - key primária, isto to mostrar o major e o minor e a key primária to actualizar o file
            result[1] = resultados.NumRows > 0 ? DBConversion.ToString(resultados.GetDirect(0, 0)) : null;
            return result;
        }

        public DBFile returnLastVersionFileDocum(PersistentSupport sp, string docField)
        {
            string valorLigacao = returnValueField(Alias + "." + docField + "fk").ToString();
            if (string.IsNullOrEmpty(valorLigacao))
                return null;

            //versão mais recente
            DataMatrix resultados = this.returnValuesDocums(
                sp,
                new[] { new SelectField(new ColumnReference("docums", "coddocums"), "coddocums") },
                CriteriaSet.And()
                    .Equal("docums", "documid", valorLigacao),
                new[]
                {
                    new ColumnSort(DOCUMS_SORT_COLUMN1, SortOrder.Descending),
                    new ColumnSort(DOCUMS_SORT_COLUMN2, SortOrder.Descending)
                },
                docField);
            //codigo da key primária da versão checkout
            object coddocums = resultados.GetDirect(0, 0);
            return getFileDB(DBConversion.ToString(coddocums), sp);
        }

        public void submitDocum(PersistentSupport sp, string docField, byte[] file, string fileName, string mode, string Qversion)
        {
            int pos = fileName.LastIndexOf('_') + 1;
            string coddocums = fileName.Substring(pos, fileName.Length - pos);

            if (mode.Equals("DESBL"))
                sp.deleteRecordDocums("coddocums", coddocums);
            else if (mode.Equals("SUBM"))
            {
                insertNameValueFileDB(docField, file, fileName, coddocums, sp, Qversion, user.Name);
                this.updateDirect(sp);
            }
            else if (mode.Equals("GRAVAR"))
            {
                insertNameValueFileDB(docField, file, fileName, coddocums, sp, "CHECKOUT", user.Name);
                this.updateDirect(sp);
            }
        }

        public void discardDocum(PersistentSupport sp, string docField)
        {
            string coddocums = returnValueField(Alias + "." + docField + "fk") as string;
            sp.deleteRecordDocums("coddocums", coddocums);
        }

        public void commitDocum(PersistentSupport sp, string docField, byte[] file, string fileName, string Qversion)
        {
            string coddocums = returnValueField(Alias + "." + docField + "fk") as string;
            insertNameValueFileDB(docField, file, fileName, coddocums, sp, Qversion, user.Name);
            this.updateDirect(sp);
        }

        public void updateDocum(PersistentSupport sp, string docField, byte[] file, string fileName, string Qversion)
        {
            commitDocum(sp, docField, file, fileName, "CHECKOUT");
        }

        public DataMatrix returnValuesDocums(PersistentSupport sp, SelectField[] Qvalues, CriteriaSet condition, ColumnSort[] order, string docField)
        {
            return returnValuesDocums(sp, Qvalues, condition, order, docField, true);
        }

        public DataMatrix returnValuesDocums(PersistentSupport sp, SelectField[] Qvalues, CriteriaSet condition, ColumnSort[] order, string docField, bool isForeignKey)
        {
            return sp.returnValuesDocums(this, docField, isForeignKey, Qvalues, condition, order);
        }

		/// <summary>
		/// Método que permite eliminate e introduce vários registos
		/// </summary>
		/// <param name="sp">Suporte persistente</param>
		/// <param name="fields">fields</param>
		/// <param name="Qvalues">Qvalues</param>
		/// <param name="condition">condition</param>
		/// <returns>mensagem de Qresult</returns>
        [Obsolete("Use StatusMessage eliminar_inserir_Varios(PersistentSupport sp, string[] fields, List<string[]> Qvalues, CriteriaSet condition) instead")]
		public virtual StatusMessage eliminar_inserir_Varios(PersistentSupport sp, string[] fields, List<string[]> Qvalues, string condition)
		{
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("Area.eliminar_inserir_varios [area] {0}", Alias));

                //ir obter todos os registos que existiam na table
                QuerySelect qs = new QuerySelect(sp.DatabaseType);
                qs.Select = new StringBuilder(PrimaryKeyName);
                qs.setFromWithAlias(TableName, Alias);
                qs.Where = new StringBuilder(condition);
                qs.buildQuery();
                ArrayList chavesPrimarias = sp.executeReaderOneColumn(qs.Query);
                for (int i = 0; i < chavesPrimarias.Count; i++)
                {
                    chavesPrimarias[i] = DBConversion.ToKey(chavesPrimarias[i]);
                }
                StatusMessage Qresult = StatusMessage.GetAggregator();
                string primaryKeyValue = "";
                foreach (string[] valoresRegisto in Qvalues)
                {

                    insertNamesValuesFields(fields, valoresRegisto);
                    primaryKeyValue = QPrimaryKey;
                    if (!primaryKeyValue.Equals(""))//se não é vazio é porque já estava seleccionado
                    {
                        //remover da lista porque os que vão ficar na lista são os que
                        //irão ser apagados
                        chavesPrimarias.Remove(primaryKeyValue);
                    }
                    else
                    {
                        //key primária
                        string codInt = sp.codIntInsertion(this, false);

                        //AV(2010/09/20) As fichas novas deixam de ter registos com fields NULL e passam a ter com os Qvalues vazios apropriados
                        createEmptyFields();

                        sp.fillAreaInsert(this, user.Name, codInt, "", 1);

                        //antes de introduce
                        //1 - preencher carimbo
                        fillStampInsert();

                        //3 - preencher operações internas
                        fillInternalOperations(sp, null);

                        sp.insertPseud(this);

                        // verificar os direitos de acesso
                        if (accessRightsToChange())
                        {
                            var validationResults = Validation.validateFieldsChange(this, sp, user);
                            CheckErrorMessages(Qresult, validationResults, "DbArea.eliminar_inserir_Varios");

                            sp.change(this);
                        }
                        else
                            throw new PersistenceException("Não tem permissões para alterar os registos.", "DbArea.eliminar_inserir_Varios", "The user has no permissions to change the records.");
                    }
                }
                //verificar se sobraram chaves na lista to serem apagadas
                if (chavesPrimarias.Count > 0)
                {
                    foreach (object keyValue in chavesPrimarias)
                    {
                        Fields = new Hashtable();
                        QPrimaryKey = keyValue as string;
                        eliminate(sp);
                    }
                }
                Qresult.MergeStatusMessage(StatusMessage.OK("Eliminação e inserção múltipla bem sucedida."));
                return Qresult;
            }
            catch (GenioException ex)
            {
				if (ex.ExceptionSite == "DbArea.eliminar_inserir_Varios")
					throw;
                throw new PersistenceException(ex.UserMessage, "DbArea.eliminar_inserir_Varios", "Error in multiple deletion and insertion in DbArea: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PersistenceException(null, "DbArea.eliminar_inserir_Varios", "Error in multiple deletion and insertion in DbArea: " + ex.Message, ex);
            }
		}

        public virtual StatusMessage eliminar_inserir_Varios(PersistentSupport sp, string[] fields, List<string[]> Qvalues, CriteriaSet condition)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("Area.eliminar_inserir_varios [area] {0}", Alias));

                //ir obter todos os registos que existiam na table
                SelectQuery qs = new SelectQuery()
                    .Select(Alias, PrimaryKeyName)
                    .From(QSystem, TableName, Alias)
                    .Where(condition);

                ArrayList chavesPrimarias = sp.executeReaderOneColumn(qs);
                for (int i = 0; i < chavesPrimarias.Count; i++)
                {
                    chavesPrimarias[i] = DBConversion.ToKey(chavesPrimarias[i]);
                }

                string primaryKeyValue = "";
                foreach (string[] valoresRegisto in Qvalues)
                {

                    insertNamesValuesFields(fields, valoresRegisto);
                    primaryKeyValue = QPrimaryKey;
                    if (!primaryKeyValue.Equals(""))//se não é vazio é porque já estava seleccionado
                    {
                        //remover da lista porque os que vão ficar na lista são os que
                        //irão ser apagados
                        chavesPrimarias.Remove(primaryKeyValue);
                    }
                    else
                    {
                        //key primária
                        string codInt = sp.codIntInsertion(this, false);

                        //AV(2010/09/20) As fichas novas deixam de ter registos com fields NULL e passam a ter com os Qvalues vazios apropriados
                        createEmptyFields();

                        sp.fillAreaInsert(this, user.Name, codInt, "", 1);

                        //antes de introduce
                        //1 - preencher carimbo
                        fillStampInsert();

						//2 - prencher fields sequenciais
						calculateTemporarySequentials();

                        //3 - preencher operações internas
                        fillInternalOperations(sp, null);

						//4 - preenche Qvalues default
                        fillValuesDefault(sp, FunctionType.INS);

                        sp.insertPseud(this);
                        change(sp, (CriteriaSet)null);
                    }
                }
                //verificar se sobraram chaves na lista to serem apagadas
                if (chavesPrimarias.Count > 0)
                {
                    foreach (object keyValue in chavesPrimarias)
                    {
                        Fields = new Hashtable();
                        QPrimaryKey = keyValue as string;
                        eliminate(sp);
                    }
                }
                return StatusMessage.OK("Eliminação e inserção múltipla bem sucedida.");
            }
            catch (GenioException ex)
            {
                throw new BusinessException(ex.UserMessage, "DbArea.eliminar_inserir_Varios", "Error in multiple deletion and insertion in DbArea: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PersistenceException(null, "DbArea.eliminar_inserir_Varios", "Error in multiple deletion and insertion in DbArea: " + ex.Message, ex);
            }
        }

		public virtual bool ValidateMQ(PersistentSupport persistentSupport, string operation, string queue)
		{
			return true;
		}

		/// <summary>
        /// Função que validate se pelo menos um Qfield (sinalizado to ser enviado na queue) foi alterado.
        /// </summary>
        /// <param name="oldValues"></param>
        /// <returns></returns>
        public virtual bool ValidateChangeRecordMQ(Area oldValues)
        {
            foreach (Field Qfield in this.DBFields.Values)//percorre area corrente
            {
                if (Qfield.MQueue)
                {
                    string campoNome = this.Alias + "." + Qfield.Name;
                    if (!oldValues.returnValueField(campoNome).Equals(this.returnValueField(campoNome)))
                        return true;
                }
            }
            return false;
        }


        private void MessageQueue(PersistentSupport sp, string operation, Area oldValues)
        {
            if(!Configuration.Messaging.Enabled)
                return;

            var meta = MessagingService.Metadata;
            foreach(var pub in meta.Publishers)
            {
                //check if the publication is enabled
                if (!Configuration.Messaging.EnabledPublications.Contains(pub.Id))
                    continue;

                //if we are inside a queue processor don't resend publications that are involved in service loops
                if (sp.QueueMode && pub.NoReexport) 
                    continue;

                //check if this table is part of this publication
                var mt = pub.Tables.Find(t => t.Areas.Contains(this.Alias));
                if (mt == null)
                    continue;
                //anex tables do no send themselves, only as a result of other tables being sent
                if (mt.IsAnex)
                    continue;
                //check conditions
                if (!CheckTableFilter(mt, this, sp))
                    continue;

                //delete operation is very simple so we take that out of the way
                if (operation == "D")
                {
                    sp.DeferMessageDelete(pub, mt, this);
                    continue;
                }

                //check if any of the fields that this publisher uses have changed
                if (operation == "U" && oldValues != null)
                {
                    bool changed = mt.Fields
                        .Select(fld => Alias + "." + fld)
                        .Any(fld => !oldValues.returnValueField(fld).Equals(this.returnValueField(fld)));
                    if (!changed)
                        continue;
                }

                //check if we have any parent row that is still zzstate pending
                //this can be made much more efficient if we have access to the current history stack
                if (CheckPendingParents(sp, pub))
                    continue;

                //add the message to the transaction context so it can be sent during the commit phase
                sp.DeferMessageUpdate(pub, mt, this);

                //Anex tables are always sent together with the main one
                MessageAnexes(sp, pub);

                //if this row changed to zzstate 0 during this change
                // then send all its child records that are part of the publication
                if (this.Zzstate == 0 && oldValues != null && oldValues.Zzstate != 0)
                    MessageChildren(sp, pub);
            }
        }

        private void MessageAnexes(PersistentSupport sp, PublisherMetadata pub)
        {
            //above table anexes
            foreach (var rel in this.Information.ParentTables)
            {
                var anex = pub.Tables.Find(x => x.IsAnex && x.Areas.Contains(rel.Key));
                if (anex != null)
                {
                    string fk = returnValueField(this.Alias + "." + rel.Value.SourceRelField) as string;
                    if (DBFields[rel.Value.SourceRelField].isEmptyValue(fk))
                        continue;

                    Area areaUp = Area.createArea(rel.Key, user, user.CurrentModule);
                    sp.getRecord(areaUp, fk, anex.Fields.ToArray());
                    if (!CheckTableFilter(anex, areaUp, sp))
                        continue;
                    sp.DeferMessageUpdate(pub, anex, areaUp);
                }
            }

            //below table anexes
            foreach (var rel in this.Information.ChildTable)
            {
                var child = pub.Tables.Find(x => x.IsAnex && x.Areas.Contains(rel.ChildArea));
                if (child != null)
                {
                    var criteria = CriteriaSet.Or();
                    foreach (var foreignKey in rel.RelatedFields)
                        criteria.Equal(rel.ChildArea, foreignKey, QPrimaryKey);

                    var rows = Area.searchList(rel.ChildArea, sp, user, criteria, child.Fields.ToArray());
                    foreach (var row in rows)
                    {
                        if (!CheckTableFilter(child, row, sp))
                            continue;
                        sp.DeferMessageUpdate(pub, child, row);
                    }
                }
            }
        }

        private void MessageChildren(PersistentSupport sp, PublisherMetadata pub)
        {
            foreach (var rel in this.Information.ChildTable)
            {
                var child = pub.Tables.Find(x => !x.IsAnex && x.Areas.Contains(rel.ChildArea));
                if (child != null)
                {
                    var criteria = CriteriaSet.Or();
                    foreach (var foreignKey in rel.RelatedFields)
                        criteria.Equal(rel.ChildArea, foreignKey, QPrimaryKey);

                    var rows = Area.searchList(rel.ChildArea, sp, user, criteria, child.Fields.ToArray());
                    foreach (var row in rows)
                    {
                        if (!CheckTableFilter(child, row, sp))
                            continue;
                        sp.DeferMessageUpdate(pub, child, row);
                    }
                }
            }
        }

        private bool CheckTableFilter(PublisherTable mt, Area area, PersistentSupport sp)
        {
            if (mt.Filter == null)
                return true;

            FormulaDbContext fdc = new FormulaDbContext(area);
            //If the area we are running this formula on is different from the area the formula was defined on
            // we need to convert the formula into something that can run in this new area.
            //Since InternalOperationFormula fetches relations from the phisical FK rather than relations
            // we can get away to just cloning and remaping the base area of the formula to the current area.
            var formula = mt.Filter;
            if(mt.Table != area.Alias)
            {
                formula = new InternalOperationFormula(
                    formula.ByAreaArguments.Select(a => new ByAreaArguments(
                        a.FieldNames, 
                        a.FieldsPosition,
                        a.AliasName == mt.Table ? area.Alias : a.AliasName, //switch the base area
                        a.KeyName
                        )).ToList(),
                    formula.ParameterCount,
                    formula.function
                    );
            }
            fdc.AddFormulaSources(formula);
            return (bool)formula.calculateInternalFormula(area, sp, fdc, FunctionType.ALT);
        }

        private bool CheckPendingParents(PersistentSupport sp, PublisherMetadata pub)
        {
            foreach (var rel in this.Information.ParentTables)
                if (pub.Tables.Exists(x => !x.IsAnex && x.Table == rel.Key))
                {
                    string fk = returnValueField(this.Alias + "." + rel.Value.SourceRelField) as string;
                    if (DBFields[rel.Value.SourceRelField].isEmptyValue(fk))
                        continue;

                    //fetch zzstate
                    Area areaUp = Area.createArea(rel.Key, user, user.CurrentModule);
                    sp.getRecord(areaUp, fk, new string[] { "zzstate" });
                    if (areaUp.Zzstate != 0)
                        return true;
                }
            return false;
        }

        /// <summary>
        /// Método to enviar queues(estas ficam na db, posteriormente deverão ser enviadas pelo integrador)
        /// </summary>
        /// <param name="sp">suporte persistente</param>
        /// <param name="operacao">Type de Operação, C - Create, U - Update, D - Delete </param>
		/// <param name="oldValues">Valores originais to validar se algum Qfield mudou. Passar a null caso seja uma inserção ou quisermos forçar o envio</param>
		/// <param name="queueId">Id da queue a enviar ou passar a null to enviar todas as queues desta area</param>
        /// <returns>o status e a mensagem resposta da inserção</returns>
        public virtual void insertQueue(PersistentSupport sp, string operation, Area oldValues, string queueId)
        {
            //TODO - Actualmente as queues só estão a ser criadas no Area.Alterar e Area.Apagar, verificar por casos praticos se faz sentido estender isto
            try
            {
                if (this.Information.QueuesList == null || Configuration.MessageQueueing == null || Configuration.MessageQueueing.Queues == null)
                    return;

				if (operation.Equals("U") && oldValues != null && !ValidateChangeRecordMQ(oldValues))
                    return;

                foreach (var queue in this.Information.QueuesList)
                {
					if (queueId != null && queueId != queue.Name)
						continue; //Se só queremos enviar to uma queue específica. Por exemplo nas exportações.

                    //TODO: isto não vai permitir enviar to multiplas queues da mesma table, devia ser feito o FindAdd e iterado
                    var configQueue = Configuration.MessageQueueing.Queues.Find(x => x.queue == queue.Name && x.Qyear == sp.SchemaMapping.Name);
					if (configQueue == null)
                        continue;//Se a queue não estiver declarada no Configuracoes.xml

					string queue_guid = Guid.NewGuid().ToString("N");

                    if (!this.ValidateMQ(sp, operation, queue.Name))
                        continue;//Se a queue não cumpre as regras de envio

					if (sp.QueueMode && queue.DoNotReexport)
                        continue;//Se estiver em mode queue e tiver definido na queue to não reexportar

                    XmlDocument xml = new XmlDocument();
                    XmlElement xmlMainElem; // nós de 1º level
                    XmlElement xmlNTable; // nós de 2º level - Tabelas 1N e N1
                    XmlElement xmlNTableField; // nós de 3º level - fields das tables 1N e N1

                    XmlElement xml_root = xml.DocumentElement;

                    XmlDeclaration xml_decl;
                    if (configQueue.Unicode)
                        xml_decl = xml.CreateXmlDeclaration("1.0", "UTF-16", null);
                    else
                        xml_decl = xml.CreateXmlDeclaration("1.0", "ISO-8859-1", null);

                    xmlMainElem = xml.CreateElement("", "mqrec", "");
                    xml.InsertBefore(xml_decl, xml_root);
                    xmlMainElem.SetAttribute("table", queue.DomainTable.ToUpper());
                    xmlMainElem.SetAttribute("guid", queue_guid);
                    xmlMainElem.SetAttribute("sistema", Configuration.Program);
                    xmlMainElem.SetAttribute("queue", queue.Name);
                    xmlMainElem.SetAttribute("tp", operation);
                    xmlMainElem.SetAttribute("year", DBConversion.ToString(user.Year));

                    foreach (Field Qfield in this.DBFields.Values)//percorre area corrente
                    {
                        if (Qfield.MQueue)
                        {
                            xmlMainElem.AppendChild(MQXml.FieldAdd(xml,
                                                                   Qfield.Name,
                                                                   Qfield,
                                                                   this.returnValueField(this.Alias + "." + Qfield.Name)));
                            xml.AppendChild(xmlMainElem);
                        }
                    }

                    //percorre areas N1
                    xmlNTable = xml.CreateElement("mqreln1");
                    foreach (var area_N1 in queue.TablesN1)
                    {
                        Area mq_area_N1 = Area.createArea(area_N1.Table.ToLower(),this.user, this.module);
                        string key = DBConversion.ToString(this.returnValueField(Alias + "." + area_N1.Field));
                        if (!string.IsNullOrEmpty(key))
                        {
                            sp.getRecord(mq_area_N1, key);

                            xmlNTableField = xml.CreateElement("mqrec");
                            xmlNTableField.SetAttribute("table", area_N1.DomainTable);
                            foreach (Field Qfield in mq_area_N1.DBFields.Values)
                            {
                                if (Qfield.MQueue)
                                {
                                    xmlNTableField.AppendChild(MQXml.FieldAdd(xml,
                                                                              Qfield.Name,
                                                                              Qfield,
                                                                              mq_area_N1.returnValueField(mq_area_N1.Alias + "." + Qfield.Name)));
                                }
                            }
                            xmlNTable.AppendChild(xmlNTableField);
                        }//fechar table
                    }
                    xmlMainElem.AppendChild(xmlNTable);

                    //percorre areas 1N
                    xmlNTable = xml.CreateElement("mqrel1n");
                    foreach (var area_1N in queue.Tables1N)
                    {
                        Area mq_area_1N = Area.createArea(area_1N.Table.ToLower(),this.user, this.module);
                        string key = QPrimaryKey;
                        if (!string.IsNullOrEmpty(key))
                        {
                            string sql = "";
                            List<string> cp_list = new List<string>();
                            int x = 0;
                            foreach (Field Qfield in mq_area_1N.DBFields.Values) // Create dinamicamente query com todos os fields necessarios
                            {
                                if (Qfield.MQueue)
                                {
                                    cp_list.Add(Qfield.Name);
                                    if (sql.Length > 0) sql += ", ";
                                    sql += Qfield.Name;
                                    x++;
                                }
                            }
                            //if (sql.EndsWith(", ")) sql = sql.Remove(sql.LastIndexOf(", ")); // deverá ser mais performante que a solução acima... TODO: testar

                            sql = "SELECT " + sql + " FROM " + Configuration.Program + mq_area_1N.Alias + " WHERE " + area_1N.Field + " = '" + key + "' AND zzstate = 0";

                            DataMatrix list_mq_area_1N = sp.executeQuery(sql);
                            for (int i = 0; i < list_mq_area_1N.NumRows; i++)
	                        {
                                xmlNTableField = xml.CreateElement("mqrec");
                                xmlNTableField.SetAttribute("table", area_1N.DomainTable);
                                for (int n = 0; n < list_mq_area_1N.NumCols; n++)
			                    {
                                    xmlNTableField.AppendChild(MQXml.FieldAdd(xml,
                                                                              cp_list[n],
                                                                              mq_area_1N.DBFields[cp_list[n]],
                                                                              list_mq_area_1N.GetString(i,n)));
                                }
                                xmlNTable.AppendChild(xmlNTableField);
                            }
                        } // fechar table
                    }
                    xmlMainElem.AppendChild(xmlNTable);
                    xml.AppendChild(xmlMainElem);

                    StringWriter sw = new StringWriter();
                    XmlTextWriter tx = new XmlTextWriter(sw);
                    xml.WriteTo(tx);

                    byte[] message = configQueue.Unicode?
						Encoding.Unicode.GetBytes(sw.ToString()):
                        Encoding.Default.GetBytes(sw.ToString());

					//gzip compress the message (compatible with sql2016 DECOMPRESS() function)
					using (var compressStream = new MemoryStream())
                    using (var compressor = new System.IO.Compression.GZipStream(compressStream, System.IO.Compression.CompressionMode.Compress))
                    {
                        compressor.Write(message, 0, message.Length);
						compressor.Close();
                        message = compressStream.ToArray();
                    }

                    //---------------------------------- TEST (this code does not take into account transaction rollbacks)
                    //if (configQueue.UsesMsmq)
                    //    MQXml.SendMSMQ(configQueue.path, message);
                    //----------------------------------

                    //TODO: isto não vai permitir enviar to multiplas queues da mesma table
                    RecordQueueSend(sp, operation, queue_guid, configQueue.queue, configQueue.channelId, message);
                }
            }
            catch (GenioException ex)
            {
				throw new BusinessException(ex.UserMessage, "DbArea.inserirQueue", "Error inserting queue: " + ex.Message, ex);
            }
        }

        public StatusMessage SendTask(PersistentSupport sp, string task, List<KeyValuePair<string, object>> arguments)
        {
            try
			{
                string queue_guid = Guid.NewGuid().ToString("N");
                var configQueue = Configuration.MessageQueueing.Queues.Find(x => x.queue == task && x.Qyear == sp.SchemaMapping.Name);
                if (configQueue == null)
                    throw new BusinessException(null, "DbArea.SendTask", "Queue configuration is null.");

                string s = MQXml.GetTaskXml(sp.SchemaMapping.Name, queue_guid, task, arguments);
                byte[] message = configQueue.Unicode ?
                        Encoding.Unicode.GetBytes(s) :
                        Encoding.Default.GetBytes(s);

                //---------------------------------- TEST (this code does not take into account transaction rollbacks)
                //if (configQueue.UsesMsmq)
                //    MQXml.SendMSMQ(configQueue.path, message);
                //----------------------------------

                return RecordQueueSend(sp, "F", queue_guid, task, configQueue.channelId, message);
            }
            catch (GenioException ex)
            {
				throw new BusinessException(ex.UserMessage, "DbArea.SendTask", "Error sending task: " + ex.Message, ex);
            }
        }

        private StatusMessage RecordQueueSend(PersistentSupport sp, string operation, string queue_guid, string queue, string channelId, byte[] message)
        {
            Area mqqueue = new CSGenioAmqqueues(user, this.module);
            mqqueue.insertNameValueField("mqqueues.ano", user.Year);
            mqqueue.insertNameValueField("mqqueues.username", user.Name);
            mqqueue.insertNameValueField("mqqueues.tabela", this.Alias);
            mqqueue.insertNameValueField("mqqueues.tabelacod", QPrimaryKey);
            mqqueue.insertNameValueField("mqqueues.queuekey", queue_guid);
            mqqueue.insertNameValueField("mqqueues.queue", message);
            mqqueue.insertNameValueField("mqqueues.queueid", queue);
            mqqueue.insertNameValueField("mqqueues.channelid", channelId);
            mqqueue.insertNameValueField("mqqueues.mqstatus", "0");//SendFAIL = 0;// quando a queue deu erro a ser enviada to o servidor de queues.
			mqqueue.insertNameValueField("mqqueues.sendnumber", "0");
            mqqueue.insertNameValueField("mqqueues.datastatus", DateTime.Now);
			mqqueue.insertNameValueField("mqqueues.datacria", DateTime.Now);
            mqqueue.insertNameValueField("mqqueues.operacao", operation);

            mqqueue.Zzstate = 0;
            mqqueue.QPrimaryKey = sp.codIntInsertion(mqqueue, false);
			sp.DeferQueueToCommit(mqqueue);
            return StatusMessage.OK();
        }

		/// <summary>
        /// Check if a record exist
        /// </summary>
        /// <param name="key">Record key</param>
        /// <param name="areadInfo">Info area</param>
        /// <param name="sp">DB conecntion</param>
        /// <returns></returns>
        protected static bool RecordExist(string key, AreaInfo areadInfo, PersistentSupport sp)
        {
            SelectQuery query = new SelectQuery()
                .Select(SqlFunctions.Count(1), "count")
                .From(areadInfo.TableName, areadInfo.Alias)
                .Where(CriteriaSet.And()
                .Equal(areadInfo.Alias, areadInfo.PrimaryKeyName, key));


            return DBConversion.ToInteger(sp.ExecuteScalar(query)) > 0;
        }
	}

    /// <summary>
    /// Version comparer that orders the document version by descending version.
    /// </summary>
    public class VersionComparer : IComparer<String>
    {
        public int Compare(string v1, string v2)
        {
			if (string.IsNullOrEmpty(v1))
                v1 = "0";
            if (string.IsNullOrEmpty(v2))
                v2 = "0";
            // First split by "."
            string[] versionParts1 = v1.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            string[] versionParts2 = v2.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            // Parse to int
            int[] version1ToInt = new int[versionParts1.Length];
			for(int i = 0; i < versionParts1.Length; i++)
				version1ToInt[i] = int.Parse(versionParts1[i]);
            int[] version2ToInt = new int[versionParts2.Length];
			for(int i = 0; i < versionParts2.Length; i++)
				version2ToInt[i] = int.Parse(versionParts2[i]);
            // Take the numeric part and compare
            int numericPart1 = version1ToInt[0];
            int numericPart2 = version2ToInt[0];
            int compare = numericPart1.CompareTo(numericPart2);
            if (compare != 0)
                return compare;
            if (version1ToInt.Length == 1 && version2ToInt.Length == 1)
                // No decimal parts, they are equal
                return compare;
            // Compare decimal part
            int decimalPart1 = 0;
            if (version1ToInt.Length == 2)
                decimalPart1 = version1ToInt[1];
            int decimalPart2 = 0;
            if (version2ToInt.Length == 2)
                decimalPart2 = version2ToInt[1];
            return decimalPart1.CompareTo(decimalPart2);
        }
    }
}
