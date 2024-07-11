using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace NotepadSharp.Core.Utilities
{
    public class TraceTextWriter : TextWriter
    {
        public override Encoding Encoding
        {
            get
            {
                return Encoding.Unicode;
            }
        }

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
