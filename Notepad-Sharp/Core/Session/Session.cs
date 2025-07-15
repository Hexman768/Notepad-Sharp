using System.Text;

namespace NotepadSharp.Core.Session
{
    /// <summary>
    /// Object model used to represent the user's current session, all open files/tabs.
    /// </summary>
    public class Session
    {
        private Encoding encoding;
        private string filename;
        private string path;
        private string text;

        /// <summary>
        /// Constructs an instance of Session, used for session snapshots.
        /// </summary>
        /// <param name="encoding"></param>
        /// <param name="filename"></param>
        /// <param name="path"></param>
        /// <param name="text"></param>
        public Session(Encoding encoding, string filename, string path, string text)
        {
            this.encoding = encoding;
            this.filename = filename;
            this.path = path;
            this.text = text;
        }

        public Encoding Encoding
        {
            get { return encoding; }
            set { encoding = value; }
        }

        public string Filename
        {
            get {  return filename; }
            set { filename = value; }
        }

        public string Path
        { 
            get { return path; } 
            set { path = value; }
        }

        public string Text
        {
            get { return text; }
            set { text = value; }
        }
    }
}
