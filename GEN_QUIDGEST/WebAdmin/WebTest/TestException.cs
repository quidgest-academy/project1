using CSGenio.framework;
using System;

namespace WebTest
{
    class TestException: GenioException
    {
		private static string exceptionName = "Unit Test Exception";

		/// <summary>
        /// This class represents errors that occur during unit testing.
		/// </summary>
		/// <param name="userMessage">Message that describes the current exception to the user.</param>
        /// <param name="exceptionSite">Name of the method that throws the current exception.</param>
        /// <param name="exceptionCause">Message that describes the direct cause of the current exception.</param>
        public TestException(string userMessage, string exceptionSite, string exceptionCause)
            : base(userMessage, exceptionSite, exceptionCause, null) 
		{
		}

		protected override void LogError()
        {
            LogError(exceptionName);
        }
	}
}
