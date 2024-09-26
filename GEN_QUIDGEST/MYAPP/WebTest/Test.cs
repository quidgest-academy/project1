using NUnit.Framework;
using System.Collections;

using CSGenio.business;
using CSGenio.persistence;
using CSGenio.framework;
using Quidgest.Persistence.GenericQuery;

namespace WebTest
{
    public enum ResultType
    {
        Neutral,
        Good,
        Bad,
        BadInput
    };

    /// <summary>
    ///This is a test class for Test and is intended
    ///to contain all Test Unit Tests
    ///</summary>
    // [DeploymentItem(@"..\..\Extra")] -> There is nothing similar in NUnit. It doesn't support that functionality.
    public class Test
    {

        private readonly User user;
        private PersistentSupport sp;

        //mapeamento table, n_do_registo => key primária
        Dictionary<KeyValuePair<string, string>, string> primaryKeys = new Dictionary<KeyValuePair<string, string>, string>();

        //não devo testar estas tables
        //deveria saber isto através da biblioteca GenioServer.dll, mas não encontrei
        //talvez um TODO de AreaInfo.Ishardcoded seja boa ideia
        readonly string[] hardcodedTables = { "PSW", "MEM" };

        //a GLOB tem sempre só um registo, só deve ser testada a alteração
        readonly string[] oneRegisterTables = { "GLOB" };

        public Test()
        {
            if (!System.IO.File.Exists("Webtest.ini"))
                return;

            //ler as configurações a partir de um .INI
            //não está a ser usado, por agora
            System.IO.StreamReader iniFile = new System.IO.StreamReader("Webtest.ini");

            Dictionary<string, string> iniEntries = new Dictionary<string, string>();

            while (!iniFile.EndOfStream)
            {
                string line = iniFile.ReadLine();
                int ix = line.IndexOf('=');
                if (ix != -1)
                    iniEntries.Add(line.Substring(0, ix).Trim().ToUpper(), line.Substring(ix + 1).Trim());
            }
            iniFile.Close();

            if (!iniEntries.Keys.Contains("MODULE") || !iniEntries.Keys.Contains("INFILE") || !iniEntries.Keys.Contains("CONFFILE"))
                throw new KeyNotFoundException("There are missing entries in the INI file (MODULE or INFILE or CONFFILE)");

            if (!System.IO.File.Exists(iniEntries["INFILE"]))
                return;

            if (System.IO.File.Exists(iniEntries["CONFFILE"]))
            {

                //separador de directorias (\)
                char sep = System.IO.Path.DirectorySeparatorChar;

                //Criar a pasta bin se não existir
                string bindir = AppDomain.CurrentDomain.BaseDirectory;
                //acrescentar a barra se não estiver lá.
                if (bindir[bindir.Length - 1] != sep)
                    bindir += sep;
                bindir += "bin";

                if (!System.IO.Directory.Exists(bindir))
                    System.IO.Directory.CreateDirectory(bindir);

                //é necessário copiar o file Configuracoes.Xml to a dir "bin", pois o 
                //construtor estático do Configuration baseia-se nele duma maneira hardcoded
                System.IO.File.Copy(
                    iniEntries["CONFFILE"],
                    bindir + sep + System.IO.Path.GetFileName(iniEntries["CONFFILE"]),
                    true    //overwrite
                 );

                try
                {
                    sp = PersistentSupport.getPersistentSupport(Configuration.DefaultYear);
                    user = new User("Test", "", Configuration.DefaultYear);
                    sp.openConnection();
                }
                catch (Exception ex)
                {
                    sp.closeConnection();
                    string msgEx = getExceptionMessage(ex);
                    throw new Exception("Error on authentication: " + msgEx, ex);
                }

                sp.closeConnection();

                if (!user.IsAuthorized(iniEntries["MODULE"]))
                    throw new KeyNotFoundException("The user could not get access to the specified module " + iniEntries["MODULE"]);

                user.CurrentModule = iniEntries["MODULE"];
            }
            else
                throw new System.IO.FileNotFoundException("One of the specified input files (INFILE/CONFFILE) has not been found.");
        }

        public Test(string mod)
        {
            try
            {
                sp = PersistentSupport.getPersistentSupport(Configuration.DefaultYear);
                user = new User("Test", "", Configuration.DefaultYear);
                sp.openConnection();
            }
            catch (Exception ex)
            {
                sp.closeConnection();
                string msgEx = getExceptionMessage(ex);
                throw new Exception("Error on authentication: " + msgEx, ex);
            }
            sp.closeConnection();

            if (!user.IsAuthorized(mod))
                throw new KeyNotFoundException("The user could not get access to the specified module " + mod);

            user.CurrentModule = mod;
        }

        [Test]
        public void TestConnect()
        {
            try
            {
                sp = PersistentSupport.getPersistentSupport(Configuration.DefaultYear);
                sp.openConnection();
                Assert.IsNotNull(sp);
            }
            catch (Exception ex)
            {
                sp.closeConnection();
                string msgEx = getExceptionMessage(ex);
                throw new Exception("Error on authentication: " + msgEx, ex);
            }
            sp.closeConnection();
        }


        /// <summary>
        /// Obtém do input os dados (fields,Qvalues) que queremos colocar no tuplo respectivo
        /// </summary>
        /// <param name="table"></param>
        /// <param name="lineData">número da linha</param>
        /// <returns>Um dicionario (Qfield,Qvalue). Atençãoo name do Qfield é devolvido no format TABLE.FIELD</returns>
        private Dictionary<string, string> getDataFromObjectArray(Area tableBD, string[] tableFields, object[] lineData)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            //os dados propriamente ditos só começam a partir da 4º coluna
            for (int j = 0; j < tableFields.Length; j++)
            {
                if (lineData[j] != null)    //Qvalue
                {
                    string nomecampo = tableFields[j].ToString().ToLower();
                    string Qvalue = lineData[j].ToString();
                    data.Add(tableBD.Alias + "." + nomecampo, Qvalue);
                }
            }
            return data;
        }

        /// <summary>
        /// Handlig das mensagens das excepções
        /// </summary>
        /// <param name="ex">A excepção</param>
        /// <returns>A mensagem adequada à excepção</returns>
        private string getExceptionMessage(Exception ex)
        {
            if (ex is GenioException)
                return ((GenioException)ex).UserMessage;
            else
                return ex.Message;
        }

        /// <summary>
        /// obter o name da table que se relaciona com o Qfield key estrangeira em questão
        /// </summary>
        /// <param name="tableBD">a table abaixo</param>
        /// <param name="campo">o name do Qfield key estrangeira</param>
        /// <returns>o name da table acima na relação</returns>
        public string getRelatedTable(Area tableBD, string ncampo)
        {
            Field Qfield = tableBD.DBFields[ncampo];
            if (Qfield.FieldType == FieldType.CHAVE_ESTRANGEIRA || Qfield.FieldType == FieldType.CHAVE_ESTRANGEIRA_GUID)
            {
                foreach (KeyValuePair<string, Relation> p in tableBD.ParentTables)
                {
                    Relation rel = p.Value;
                    if (rel.SourceRelField == ncampo)
                        return rel.AliasTargetTab;
                }
            }
            throw new KeyNotFoundException("Error on the structure of the tables. Could not find table relationed with " + tableBD.Alias + "." + ncampo);
        }

        /// <summary>
        /// To os fields solicitados, percorre todos os que são chaves estrangeiras, e busca no mapa de 
        /// primary keys a key respectiva de mode a preencher com o Qvalue correspondente
        /// </summary>
        /// <param name="tableBD">table na qual queremos preencher as chaves estrangeiras</param>
        /// <param name="aliasedData"></param>
        /// <returns>
        /// Uma lista de entradas de fields e Qvalues, onde as posições no mapa já estão 
        /// substituídas pelos respectivos Qvalues de chaves estrangeiras
        /// </returns>
        private Dictionary<string, string> fillForeignKeys(string sheet, int sheetLine, Area tableBD, Dictionary<string, string> aliasedData)
        {
            Dictionary<string, string> dataWithForeignKeys = new Dictionary<string, string>(aliasedData);
            foreach (KeyValuePair<string, string> p in aliasedData)
            {
                //name do Qfield sem a table agarrada atrás
                string ncampo = p.Key.Substring(p.Key.IndexOf('.') + 1);
                Field Qfield = tableBD.DBFields[ncampo] as Field;

                if (Qfield.FieldType == FieldType.CHAVE_ESTRANGEIRA || Qfield.FieldType == FieldType.CHAVE_ESTRANGEIRA_GUID)
                {
                    string tabmae = getRelatedTable(tableBD, Qfield.Name);
                    KeyValuePair<string, string> pkey = new KeyValuePair<string, string>(tabmae, aliasedData[p.Key]);

                    if (primaryKeys.Keys.Contains(pkey))
                        dataWithForeignKeys[p.Key] = primaryKeys[pkey];
                    else
                        throw new KeyNotFoundException("Sheet '" + sheet + "' at line " + sheetLine + ":Update of a foreign key that has never been inserted");
                }
            }
            return dataWithForeignKeys;
        }

       

        /// <summary>
        /// classe auxiliar to que se tenha cada uma das linhas do input
        /// de forma estruturada e adequada ao test
        /// </summary>

        class Line
        {
            /// <summary>
            /// name da Folha
            /// </summary>
            readonly string sheet;
            public string Sheet
            {
                get
                {
                    return sheet;
                }
            }

            /// <summary>
            /// número da linha na folha
            /// </summary>
            readonly int sheetLineNo;
            public int SheetLineNo
            {
                get
                {
                    return sheetLineNo;
                }
            }

            /// <summary>
            /// name da table
            /// </summary>
            readonly string table;
            public string Table
            {
                get
                {
                    return table;
                }
            }

            /// <summary>
            /// Resultado esperado: VALID|INVALID
            /// </summary>
            readonly string expectedResult;
            public string ExpectedResult
            {
                get
                {
                    return expectedResult;
                }
            }

            /// <summary>
            /// Type de operação INSERT|UPDATE|DELETE|SELECT
            /// </summary>
            readonly string operation;
            public string Operation
            {
                get
                {
                    return operation;
                }
            }

            /// <summary>
            /// N. relativo do registo. Servirá to mapeamento entre os registos e respectiva chaves primárias. 
            /// Assim, ao introduce um registo temos controlo sobre a sua key primária, permitindo-nos  
            /// referenciar o mesmo nas operações seguintes. 
            /// Além disso também nos permitirá fazer o cross-matching das chaves estrangeiras nas tables abaixo.
            /// </summary>
            readonly string recordN;
            public string RecordN
            {
                get
                {
                    return recordN;
                }
            }

            /// <summary>
            /// Os dados proprimante ditos em format de lista com entradas TABELA.CAMPO=>VALOR
            /// </summary>
            readonly Dictionary<string, string> data = new Dictionary<string, string>();
            public Dictionary<string, string> Data
            {
                get
                {
                    return data;
                }
            }

            /// <summary>
            /// Construtor. A única maneira de definirmos os fields de uma linha
            /// </summary>
            /// <param name="sht">Name da folha</param>
            /// <param name="shtLine">Nº da linha na folha</param>
            /// <param name="tab">Name da table</param>
            /// <param name="expRes">Resultado esperado</param>
            /// <param name="optn">Operação a efectuar</param>
            /// <param name="recn">Nº do registo</param>
            /// <param name="dat">Dados concretos</param>
            public Line(string sht, int shtLine, string tab, string expRes, string optn, string recn, Dictionary<string, string> dat)
            {
                sheet = sht;
                sheetLineNo = shtLine;
                table = tab;
                expectedResult = expRes;
                operation = optn;
                recordN = recn;
                data = dat;
            }
        }

        public static void AssertThrows<T>(Action action) where T : Exception
        {
            bool fail;
            try
            {
                action();
                fail = true;
            }
            catch (T)
            {
                fail = false;
            }

            if (fail)
                Assert.Fail(string.Format("Exception of type {0} should be thrown.", typeof(T)));
        }
    }
}
