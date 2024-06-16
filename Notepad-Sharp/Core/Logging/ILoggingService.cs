namespace NotepadSharp.Core.Logging
{
    /// <summary>
    /// Interface for a logging service that can be implemented to provide 
    /// customized logging behavior.
    /// </summary>
    public interface ILoggingService
    {
        /// <summary>
        /// Displays an error message.
        /// </summary>
        /// <param name="message"></param>
        void Error(string message);

        /// <summary>
        /// Displays an informational message.
        /// </summary>
        /// <param name="message"></param>
        void Info(string message);

        /// <summary>
        /// Displays a warning message.
        /// </summary>
        /// <param name="message"></param>
        void Warn(string message);
    }
}
