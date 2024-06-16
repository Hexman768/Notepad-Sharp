using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace NotepadSharp.Core.Utilities
{
    /// <summary>
    /// Class to send trace messages to the output window in the Visual Studio IDE.
    /// </summary>
    public class TraceTextWriter : TextWriter
    {
        public override Encoding Encoding
        {
            get
            {
                return Encoding.Unicode;
            }
        }

        /// <summary>
        /// Sends an informational trace message to the output window in the
        /// Visual Studio IDE.
        /// </summary>
        /// <param name="message"></param>
        public void Info(string message)
        {
            Trace.TraceInformation("\n" + DateTime.Now + ": " + message + "\n");
        }

        public override void Write(string text)
        {
            Trace.Write(text);
        }

        public override void WriteLine(string text)
        {
            Trace.WriteLine(text);
        }
    }
}
