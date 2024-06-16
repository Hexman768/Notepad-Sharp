using NotepadSharp.Core.Logging;

namespace NotepadSharp.Core.Services
{
    /// <summary>
    /// Class for easy logging.
    /// </summary>
    public static class LoggingService
    {
        private static ILoggerService _service = ServiceSingleton.GetRequiredService<EditorLoggerService>();

        public static void Error(string message)
        {
            _service.Error(message);
        }

        public static void Info(string message)
        {
            _service.Info(message);
        }

        public static void Warn(string message)
        {
            _service.Warn(message);
        }
    }
}
