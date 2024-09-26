using System;
using System.Collections;
using System.Collections.Generic;

namespace CSGenio.framework
{
    /// <summary>
    /// Tipos de fields das tables
    /// </summary>
    //SO 20060814 alterei a classe, os métodos de conversão passaram to a classe Conversion
    public class FieldType
    {
        public static readonly FieldType TEXTO = new FieldType("C", FieldFormatting.CARACTERES, typeof(string));					// text (string)

		public static readonly FieldType CHAVE_ESTRANGEIRA = new FieldType("CE", FieldFormatting.CARACTERES, typeof(string));	// key estrangeira (string)
        public static readonly FieldType CHAVE_ESTRANGEIRA_GUID = new FieldType("CEG", FieldFormatting.GUID, typeof(string));	// key estrangeira (Guid)

		public static readonly FieldType CHAVE_FALSA_GUID = new FieldType("CF", FieldFormatting.GUID, typeof(string));						// key falsa (guid)
        public static readonly FieldType CHAVE_FALSA = new FieldType("CF", FieldFormatting.CARACTERES, typeof(string));			// key falsa (string) //AJATODO _ CHAVEFALSA

        public static readonly FieldType MUITAS_LINHAS = new FieldType("M", FieldFormatting.CARACTERES, typeof(string));			// muitas linhas (string)
        public static readonly FieldType MEMO = new FieldType("MO", FieldFormatting.CARACTERES, typeof(string));					// memo (string)
        public static readonly FieldType MEMO_COMP_RTF = new FieldType("MM", FieldFormatting.CARACTERES, typeof(string));		// memo sem RTF (string)
        public static readonly FieldType MEMO_RTF = new FieldType("MN", FieldFormatting.CARACTERES, typeof(string));				// memo com RTP (string)

        public static readonly FieldType CHAVE_PRIMARIA = new FieldType("+", FieldFormatting.CARACTERES, typeof(string));		// key primária (string)
        public static readonly FieldType CHAVE_PRIMARIA_GUID = new FieldType("+G", FieldFormatting.GUID, typeof(string));		// key primária (Guid)

        public static readonly FieldType PATH = new FieldType("P", FieldFormatting.CARACTERES, typeof(string));					// path (string)
        public static readonly FieldType ANO = new FieldType("Y", FieldFormatting.INTEIRO, typeof(int));						// Qyear (inteiro) AV(2010/07/05) Corrigi de characters to inteiro
        public static readonly FieldType TEMPO = new FieldType("T", FieldFormatting.TEMPO, typeof(string));					// time (string)
        public static readonly FieldType NUMERO = new FieldType("N", FieldFormatting.FLOAT, typeof(decimal));						// numérico (double)
        public static readonly FieldType VALOR = new FieldType("$", FieldFormatting.FLOAT, typeof(decimal));						// Qvalue monetário (double)
        //em C# é bool na BD é int
        public static readonly FieldType LOGICO = new FieldType("L", FieldFormatting.LOGICO, typeof(int));					// lógico (long)

        public static readonly FieldType INTEIRO = new FieldType("I", FieldFormatting.INTEIRO, typeof(int));					// lógico (long)

        public static readonly FieldType DATA = new FieldType("D", FieldFormatting.DATA, typeof(DateTime));						// data (datetime)
        public static readonly FieldType DATAHORA = new FieldType("DT", FieldFormatting.DATAHORA, typeof(DateTime));				// data e hour (datetime)
        public static readonly FieldType DATASEGUNDO = new FieldType("DS", FieldFormatting.DATASEGUNDO, typeof(DateTime));			// data e hour (datetime)

        public static readonly FieldType DATACRIA = new FieldType("OD", FieldFormatting.DATA, typeof(DateTime));					// data (datetime)
        public static readonly FieldType DATAMUDA = new FieldType("ED", FieldFormatting.DATA, typeof(DateTime));					// data (datetime)
        public static readonly FieldType OPERCRIA = new FieldType("ON", FieldFormatting.CARACTERES, typeof(string));				// text (string)
        public static readonly FieldType OPERMUDA = new FieldType("EN", FieldFormatting.CARACTERES, typeof(string));				// text (string)
        public static readonly FieldType HORACRIA = new FieldType("OT", FieldFormatting.TEMPO, typeof(string));				// time (string)
        public static readonly FieldType HORAMUDA = new FieldType("ET", FieldFormatting.TEMPO, typeof(string));				// time (string)
        public static readonly FieldType INSTANTECRIA = new FieldType("OI", FieldFormatting.DATASEGUNDO, typeof(string));			// time com segundos (datetime)

        public static readonly FieldType ARRAY_COD_NUMERICO = new FieldType("AN", FieldFormatting.FLOAT, typeof(decimal));		//DQ 180107 - estes fields são numéricos		// array com código numérico
        public static readonly FieldType ARRAY_COD_TEXTO = new FieldType("AC", FieldFormatting.CARACTERES, typeof(string));		// array com código text
        public static readonly FieldType ARRAY_COD_LOGICO = new FieldType("AL", FieldFormatting.LOGICO, typeof(string));		// array com código text
        public static readonly FieldType IMAGEM_JPEG = new FieldType("IJ", FieldFormatting.JPEG, typeof(byte[]));				// imagem gravada na db
        public static readonly FieldType FICHEIRO_BD = new FieldType("IB", FieldFormatting.CARACTERES, typeof(string));			//AV 20090302  - name do file que fica gravado na table docums

        public static readonly FieldType FICHEIRO_BIN = new FieldType("BIN", FieldFormatting.BINARIO, typeof(byte[]));           // file binario na db

		public static readonly FieldType GEOGRAPHY = new FieldType("GG", FieldFormatting.GEOGRAPHY, typeof(byte[]));           // Qfield de coordenadas geograficas na db
		public static readonly FieldType GEO_SHAPE = new FieldType("GS", FieldFormatting.GEO_SHAPE, typeof(byte[]));
		public static readonly FieldType GEOMETRIC = new FieldType("GC", FieldFormatting.GEOMETRIC, typeof(byte[]));

        /// <summary>
        /// Encrypted field type - Password.
        /// Will have to manage two values (encrypted and decrypted) in memory at the same time.
        /// </summary>
        public static readonly FieldType PASSWORD = new FieldType("PW", FieldFormatting.ENCRYPTED, typeof(EncryptedDataType));

        /// <summary>
        /// Id do tipo de Qfield
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Formatação
        /// </summary>
        public FieldFormatting Formatting { get; }

        /// <summary>
        /// Type de dados interno
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Construtor privado to inicialização dos membros
        /// </summary>
        /// <param name="anID"></param>
        private FieldType(string anID, FieldFormatting tipoInt, Type tipo)
        {
            Id = anID;
            Formatting = tipoInt;
            Type = tipo;
        }

        /// <summary>
        /// Devolve o identifier do tipo de Qfield
        /// </summary>
        /// <returns>Identificador do tipo de Qfield</returns>
        public override string ToString()
        {
            return Id;
        }

		/// <summary>
        /// Mapeamento entre o tipo de Field e Função SQL que validate se está vazio
        /// </summary>
		private static Dictionary<string, string> EPHFieldType = new Dictionary<string, string>
        {
			{"ANO_MES_DIA", "EMPTYD"},
			{"DIA_MES_ANO", "EMPTYD"},
			{"CARACTERES", "EMPTYC"},
			{"GUID", "EMPTYG"},
			{"INTEIRO", "EMPTYN"},
			{"DATA","EMPTYD"},
			{"DATAHORA", "EMPTYD"},
			{"LOGICO", "EMPTYL"},
			{"FLOAT", "EMPTYN"},
			{"DATASEGUNDO", "EMPTYD"}
	    };

        public static string getEPHFunction(FieldFormatting fField)
        {
            EPHFieldType.TryGetValue(fField.ToString(), out string funcaoSQL);
            return funcaoSQL;
        }
    }
}