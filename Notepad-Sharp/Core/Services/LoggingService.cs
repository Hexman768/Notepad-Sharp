namespace NotepadSharp.Core.Services
{
    /// <summary>
    /// Service Class for easy logging.
    /// </summary>
    public static class LoggingService
    {
        private static Logging.ILoggingService _service = ServiceSingleton.GetRequiredService<Logging.ILoggingService>();

        /// <summary>
        /// Sends an error message to a logging service.
        /// </summary>
        /// <param name="message"></param>
        public static void
            Error(
            string message /* Message to be logged */
            )
        {
            _service.Error(message);
        }

        /// <summary>
        /// Sends an informational message to a logging service.
        /// </summary>
        /// <param name="message"></param>
        public static void
            Info(
            string message /* Message to be logged */
            )
        {
            _service.Info(message);
        }

        /// <summary>
        /// Sends a warning message to a logging service.
        /// </summary>
        /// <param name="message"></param>
        public static void
            Warn
            (
            string message /* Message to be logged */
            )
        {
            _service.Warn(message);
        }
    }
}
