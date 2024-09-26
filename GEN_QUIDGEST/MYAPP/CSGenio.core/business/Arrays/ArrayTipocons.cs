using System.Collections.Generic;

namespace CSGenio.business
{
	/// <summary>
	/// Array tipocons (Tipo de construção)
	/// </summary>
	public class ArrayTipocons : Array<string>
	{
		/// <summary>
		/// The instance
		/// </summary>
		private static readonly ArrayTipocons _instance = new ArrayTipocons();

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>
		/// The instance.
		/// </value>
		public static ArrayTipocons Instance { get => _instance; }

		/// <summary>
		/// Array code type
		/// </summary>
		public static ArrayType Type { get { return ArrayType.STRING; } }

		/// <summary>
		/// Apartamento
		/// </summary>
		public const string E_A_1 = "a";
		/// <summary>
		/// Moradia
		/// </summary>
		public const string E_M_2 = "m";
		/// <summary>
		/// Outra
		/// </summary>
		public const string E_O_3 = "o";

		/// <summary>
		/// Prevents a default instance of the <see cref="ArrayTipocons"/> class from being created.
		/// </summary>
		private ArrayTipocons() : base() {}

		/// <summary>
        /// Loads the dictionary.
        /// </summary>
        /// <returns></returns>
		protected override Dictionary<string, ArrayElement> LoadDictionary()
		{
			return new Dictionary<string, ArrayElement>()
			{
				{ E_A_1, new ArrayElement() { ResourceId = "APARTAMENTO13855", HelpId = "", Group = "" } },
				{ E_M_2, new ArrayElement() { ResourceId = "MORADIA65264", HelpId = "", Group = "" } },
				{ E_O_3, new ArrayElement() { ResourceId = "OUTRA00632", HelpId = "", Group = "" } },
			};
		}

		/// <summary>
		/// Gets the element's description.
		/// </summary>
		/// <param name="cod">The cod.</param>
		/// <returns></returns>
		public static string CodToDescricao(string cod)
		{
			return Instance.CodToDescricaoImpl(cod);
		}

		/// <summary>
		/// Gets the elements.
		/// </summary>
		/// <returns></returns>
		public static List<string> GetElements()
		{
			return Instance.GetElementsImpl();
		}

		/// <summary>
		/// Gets the element.
		/// </summary>
		/// <param name="cod">The cod.</param>
		/// <returns></returns>
		public static ArrayElement GetElement(string cod)
		{
            return Instance.GetElementImpl(cod);
        }

		/// <summary>
		/// Gets the dictionary.
		/// </summary>
		/// <returns></returns>
		public static IDictionary<string, string> GetDictionary()
		{
			return Instance.GetDictionaryImpl();
		}

		/// <summary>
		/// Gets the help identifier.
		/// </summary>
		/// <param name="cod">The cod.</param>
		/// <returns></returns>
		public static string GetHelpId(string cod)
		{
			return Instance.GetHelpIdImpl(cod);
		}
	}
}
