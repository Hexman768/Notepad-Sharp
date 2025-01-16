using NotepadSharp.Core.Utilities;
using System;

namespace NotepadSharp.Core.Logging
{
    /// <summary>
    /// This is the logger service that provides trace messages to 
    /// the output box in the Visual Studio IDE while the application is 
    /// running in debug mode.
    /// </summary>
    public class DebugLoggerService : ILoggerService
    {
        private TraceTextWriter _textWriter;

        /// <summary>
        /// Constructs the <see cref="DebugLoggerService"/>.
        /// </summary>
        /// <param name="textWriter"></param>
        public DebugLoggerService(TraceTextWriter textWriter)
        {
            _textWriter = textWriter;
        }

        private void Write(string text, Exception exception)
        {
            if (text != null)
            {
                _textWriter.Write(text);
            }
            if (exception != null)
            {
                _textWriter.Write(exception.ToString());
            }
        }

        public void Error(string message)
        {
            Write(message, null);
        }

        public void Warn(string message)
        {
            Write(message, null);
        }

        public void Info(string message)
        {
            _textWriter.Info(message);
        }
    }
}
