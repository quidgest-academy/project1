﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Principal;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Net.Http;
using System.Threading.Tasks;

using CSGenio.framework;
using CSGenio.persistence;
using CSGenio.core.persistence;
using GenioServer.security;
using Quidgest.Persistence;
using Quidgest.Persistence.GenericQuery;

// USE /[MANUAL PRO IMPORTS]/
// USE /[MANUAL PRO IMPORTS GlobalFunctions]/

namespace CSGenio.business
{
	/// <summary>
	/// Summary description for GlobalFunctions.
	/// </summary>
	public sealed partial class GlobalFunctions
	{
		/// <summary>
		/// Initializes all the manual functions.
		/// </summary>
		private static void initTodasFuncoes()
		{
			todasFuncoes = new Hashtable(8, (float)0.5);
			todasFuncoes.Add("password_alterar", 0);
			todasFuncoes.Add("password_verificaAntiga", 1);
			todasFuncoes.Add("validarAssinatura", 2);
			todasFuncoes.Add("devolverCamposAssinatura", 3);
			todasFuncoes.Add("escreverAssinatura", 4);
			todasFuncoes.Add("password_gerar", 5);
			todasFuncoes.Add("CriarDocumQweb", 6);
			todasFuncoes.Add("GetUserProfile", 7);
			//funcoes Csharp
			// Cargas
		}

		#region Funções

		#endregion

		#region MANCS


		#endregion

		private static readonly List<string> m_allManualFuntionsNames = new List<string>()
		{

		};

		public static List<string> AllManualFuntionsNames
		{
			get
			{
				return m_allManualFuntionsNames;
			}
		}

		/// <summary>
		/// Check if function can be executed from the outside (from the client-side)
		/// </summary>
		/// <param name="functionName"></param>
		/// <returns></returns>
		public static bool CheckAllowedFunctions(string functionName)
		{
			return m_allManualFuntionsNames.Contains(functionName);
		}
	}
}
