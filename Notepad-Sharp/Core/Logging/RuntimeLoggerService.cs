using System;

namespace NotepadSharp.Core.Logging
{
    /// <summary>
    /// This class is the logger service for runtime usage.
    /// Any messages that need to be relayed to the user will be 
    /// in the form of a <see cref="MessageBox"/>.
    /// </summary>
    public class RuntimeLoggerService : ILoggerService
    {
        public void Error(string message)
        {
            throw new NotImplementedException();
        }

        public void Info(string message)
        {
            throw new NotImplementedException();
        }

        public void Warn(string message)
        {
            throw new NotImplementedException();
        }
    }
}
