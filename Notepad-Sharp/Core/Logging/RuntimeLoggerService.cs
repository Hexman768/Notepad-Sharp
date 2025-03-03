using System.Windows.Forms;

namespace NotepadSharp.Core.Logging
{
    /// <summary>
    /// This class is the logger service for runtime usage.
    /// Any messages that need to be relayed to the user will be 
    /// in the form of a <see cref="MessageBox"/>.
    /// </summary>
    public class RuntimeLoggerService : ILoggingService
    {
        public void Error(string message)
        {
            MessageBox.Show(message, "Error");
        }

        public void Info(string message)
        {
            MessageBox.Show(message, "Info");
        }

        public void Warn(string message)
        {
            MessageBox.Show(message, "Warning");
        }
    }
}
