using RefactorThis.Domain.Services;

namespace RefactorThis.Domain.Helpers
{

    public static class Logger
    {
        private static ILoggingService _service;

        private static ILoggingService Service
        {
            get
            {
                if (_service == null)
                    _service = new LoggingService();

                return _service;
            }
        }

        /// <summary>
        /// Logs to the console with log level INFO.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void ILog(string message)
        {
            Service.ILog(message);
        }

        /// <summary>
        /// Logs to the console with log level WARN.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void WLog(string message)
        {
            Service.WLog(message);
        }

        /// <summary>
        /// Logs to the console with log level ERROR.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void ELog(string message)
        {
            Service.ELog(message);
        }
    }
}
