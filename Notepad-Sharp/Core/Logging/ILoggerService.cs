namespace NotepadSharp.Core.Logging
{
    /// <summary>
    /// Interface for a logging service that can be implemented to provide 
    /// customized logging behavior.
    /// </summary>
    public interface ILoggerService
    {
        void Error(string message);
        void Info(string message);
        void Warn(string message);
    }
}
