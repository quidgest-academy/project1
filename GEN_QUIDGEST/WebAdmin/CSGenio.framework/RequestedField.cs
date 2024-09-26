using System;

namespace CSGenio.framework
{
	/// <summary>
	/// Classe que representa um RequestedField.
	/// </summary>
	public class RequestedField
	{
		private string name;//name do Qfield
		private string nomeCompleto;//name completo, to a interface
		private string area;//name da �rea que o Qfield pertence
		private bool pertenceArea;//se o Qfield pertence � �rea
		private bool semArea;// se o Qfield n�o pertence a uma �rea
		private object Qvalue;//Qvalue do Qfield
        private FieldType fieldType;//tipo do Qfield

        /// <summary>
        /// Construtor de c�pia
        /// </summary>
        /// <param name="other">O RequestedField de onde copiar</param>
        public RequestedField(RequestedField other)
        {
            name = other.name;
            nomeCompleto = other.nomeCompleto;
            area = other.area;
            pertenceArea = other.pertenceArea;
            semArea = other.semArea;
            Qvalue = other.Qvalue;
            fieldType = other.fieldType;
        }

		/// <summary>
        /// Constructor da classe usado na inser��o
		/// </summary>
		/// <param name="nomeComp">Name do Qfield</param>
		/// <param name="alias">Alias da table</param>
		public RequestedField(string nomeComp, string alias)
		{
			string[] split = nomeComp.Split('.');
			nomeCompleto = nomeComp;
            if(split.Length == 2)
			{
				area = split[0];
				name = split[1];
				semArea = false;
				if (area.Equals(alias))
					pertenceArea = true;
				else
					pertenceArea = false;
			}
			else
			{
				area = "";
				name = nomeComp;
				nomeCompleto = nomeComp;
				semArea = true;
				pertenceArea = false;
			}
		}
		
		
		
		/// <summary>
		/// M�todo que devolve o name do Qfield
		/// </summary>
		public string Name
		{
			get { return name; }
		}
		
		/// <summary>
		/// M�todo que devolve o name completo do Qfield (por name completo
		/// subentende-se NomeArea.NomeCampo)
		/// </summary>
		public string FullName
		{
			get { return nomeCompleto; }
		}
		
		/// <summary>
		/// M�todo que devolve ou coloca o Qvalue do Qfield
		/// </summary>
		public object Value
		{
			get { return Qvalue; }
			set { Qvalue = value; }
		}
		
        //SO 20060816 mudei de formata��o to tipo de Qfield
        /// <summary>
        /// M�todo que devolve o coloca a formata��o do Qfield
        /// </summary>
        public FieldType FieldType
        {
            get { return fieldType; }
            set { fieldType = value; }
        }

		/// <summary>
		/// M�todo que devolve a �rea do Qfield
		/// </summary>
		public string Area
		{
			get { return area; }
		}
		
		/// <summary>
		/// M�todo que devolve true se o Qfield pertence � �rea e 
		/// false caso contr�rio
		/// </summary>
		public bool BelongsArea
		{
			get { return pertenceArea; }
		}
		
		/// <summary>
		/// M�todo que devolve true se o Qfield n�o tem �rea e false 
		/// caso contr�rio
		/// </summary>
		public bool WithoutArea
		{
			get { return semArea; }
		}

     
        }

}
