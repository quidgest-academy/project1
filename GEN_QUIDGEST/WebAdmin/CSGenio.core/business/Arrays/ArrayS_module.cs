﻿using System.Collections.Generic;

namespace CSGenio.business
{
	/// <summary>
	/// Array s_module (Module)
	/// </summary>
	public class ArrayS_module : ModulesDynamicArray
	{
		/// <summary>
		/// The instance
		/// </summary>
		private static readonly ArrayS_module _instance = new ArrayS_module();

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>
		/// The instance.
		/// </value>
		public static ArrayS_module Instance { get => _instance; }

		/// <summary>
		/// Array code type
		/// </summary>
		public static ArrayType Type { get { return ArrayType.STRING; } }

		/// <summary>
		/// Prevents a default instance of the <see cref="ArrayS_module"/> class from being created.
		/// </summary>
		private ArrayS_module() : base() {}

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
		
		/// <summary>
		/// Serializes this instance.
		/// </summary>
		public static List<object> Serialize(string lang)
		{
			return Instance.SerializeImpl(lang);
		}
	}
}
