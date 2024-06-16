using NotepadSharp.Core.Utilities;
using System;

namespace NotepadSharp.Core.Logging
{
    public class EditorLoggerService : ILoggerService
    {
        private TraceTextWriter _textWriter;

        public EditorLoggerService(TraceTextWriter textWriter)
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
