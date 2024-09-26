using System.Collections.Generic;

namespace CSGenio.framework
{
    /// <summary>
    /// Wrapper sobre a implementação de logs no servidor
    /// </summary>
    /// <remarks>
    /// O overhead desta classe é justificado por:
    /// Existem programas que não querem a dependencia do log4net (gerador de dll),
    /// Além disso assim pode ser mais fácil mudar a implementação do logger do servidor no futuro.
    /// São razões fracas mas é mais fácil fazer isto que continuar a poluir o codigo com #if !log4net
    /// </remarks>
    public static class Log
    {
#if !log4net
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#endif

        /// <summary>
        /// EventTracking flag to enable or disable event tracking feature for debugging purposes
        /// </summary>
        public static bool EventTracking { get; private set; } = false;

        /// <summary>
        /// Adds an error message to the log file.
        /// </summary>
        /// <remarks>
        /// Utilizes log4net's ThreadContext to store a thread-specific list of errors.
        /// This is activated only when the EventTracking feature is enabled and is specifically for debugging purposes.
        /// It allows you to capture errors that occurred during a specific request and send them to the client-side.
        /// Note: In MVC applications, the ThreadContext is cleared at the beginning and end of each request cycle.
        /// </remarks>
        /// <param name="msg">The error message to be logged.</param>
        public static void Error(string msg)
        {
#if !log4net
            // Log the error message to file
            log.Error(msg);

            // Check if the event tracking feature is active for debugging
            if (EventTracking)
            {
                // ThreadContext allows for storing contextual information tied to a specific thread.
                // This enables logging of multiple events in a sequence safely, per thread.
                // https://logging.apache.org/log4net/release/manual/contexts.html

                // Retrieve the error list from the current thread's context
                var errorList = log4net.ThreadContext.Properties["ErrorList"] as List<string>;
                // Initialize the error list if it does not exist
                if (errorList == null)
                {
                    errorList = new List<string>();
                    log4net.ThreadContext.Properties["ErrorList"] = errorList;
                }

                // Append the new error message to the thread-specific error list
                errorList.Add(msg);

                // Limit the error list to the last 20 entries for memory efficiency
                if (errorList.Count > 20)
                {
                    errorList.RemoveAt(0);
                }
            }
#endif
        }

        /// <summary>
        /// Adiciona uma mensagem de trace ao file de log
        /// </summary>
        /// <param name="msg">A mensagem</param>
        /// <remarks>
        /// So deve ser activada em ambiente de desenvolvimento. Sempre que a mensagem for
        ///  composta através de métodos dispendiosos esta chamada deve ser protegida com um if
        ///  a IsDebugEnabled
        /// </remarks>
        public static void Debug(string msg)
        {
#if !log4net
            log.Debug(msg);
#endif
        }

        /// <summary>
        /// Verifica se queremos mesmo tentar por mensagens de tracing
        /// Evita o peso de construir a mensagem à custa de um if extra
        /// </summary>
        public static bool IsDebugEnabled
        {
            get
            {
#if !log4net
                return log.IsDebugEnabled;
#else
                return false;
#endif
            }
        }

        /// <summary>
        /// Inicializa um marcador de estado desta thread de processamento
        /// Permite às mensagens de erro subsequentes saber em que contexto foram invocadas
        /// </summary>
        /// <param name="context">O contexto a inicializar</param>
        /// <param name="value">O Qvalue a por no contexto</param>
        /// <example>
        /// Um bom sitio to usar é to marcar o user que está no contexto
        /// </example>
        /// <remarks>
        /// Em ASP.Net tem de se ter cuidado com thread agility:
        /// http://blog.marekstoj.com/2011/12/log4net-contextual-properties-and.html
        /// </remarks>		
        public static void SetContext(string context, object value)
        {
#if !log4net
            log4net.ThreadContext.Properties[context] = value;
            log4net.LogicalThreadContext.Properties[context] = value;
#endif
        }

        /// <summary>
        /// Enable or disable event tracking feature for debugging.
        /// </summary>
        /// <param name="isActive">Flag to activate or deactivate event tracking.</param>
        public static void SetEventTracking(bool isActive)
        {
            // Sets the global flag that controls whether event tracking is active for debugging
            EventTracking = isActive;
        }

        /// <summary>
        /// Retrieves a list of errors specific to the current thread context.
        /// </summary>
        /// <returns>A List of string containing the errors.</returns>
        public static List<string> GetThreadErrors()
        {
#if !log4net
            // Fetches the error list from the current thread context for debugging when EventTracking is active
            return log4net.ThreadContext.Properties["ErrorList"] as List<string>;
#else
            return null;
#endif
        }

        /// <summary>
        /// Clears the error cache for the current thread context.
        /// </summary>
        /// <remarks>
        /// This ensures that old error messages do not persist beyond the current lifecycle.
        /// Particularly useful to reset the state at the end of a request in MVC applications.
        /// </remarks>
        public static void ClearThreadErrorsCache()
        {
#if !log4net
            // Nullifies the error list in the current thread context, effectively clearing it
            log4net.ThreadContext.Properties["ErrorList"] = null;
#endif
        }
    }
}