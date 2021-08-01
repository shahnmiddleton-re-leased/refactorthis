using System;

namespace RefactorThis.Domain.Services
{
    public interface ILoggingService
    {
        public void ILog(string message);
        public void WLog(string message);
        public void ELog(string message);
    }
    public class LoggingService : ILoggingService
    {
        /// <summary>
        /// Logs to the console with log level ERROR.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void ELog(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{DateTime.Now} [ERROR] {message}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Logs to the console with log level INFO.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void ILog(string message)
        {
            Console.WriteLine($"{DateTime.Now} [INFO] {message}");
        }

        /// <summary>
        /// Logs to the console with log level WARN.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void WLog(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{DateTime.Now} [WARN] {message}");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
