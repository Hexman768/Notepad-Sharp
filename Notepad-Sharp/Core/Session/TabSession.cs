using System.Text;

namespace NotepadSharp.Core.Session
{
    /// <summary>
    /// Session structure representing a single Tab.
    /// </summary>
    public class TabSession
    {
        private Encoding encoding;
        private string filename;
        private string path;
        private string text;

        public TabSession(
            Encoding encoding,                     /* Encoding of given file */
            string filename,                       /* Name of given file */
            string path,                           /* Path of given file */
            string text,                           /* Text in given file */
            FastColoredTextBoxNS.Language language /* Language in given file */
            )
        {
            this.encoding = encoding;
            this.filename = filename;
            this.path = path;
            this.text = text;
        }

        /// <summary>
        /// The encoding of a given file.
        /// </summary>
        public Encoding Encoding { get { return encoding; } }

        /// <summary>
        /// The name of a given file.
        /// </summary>
        public string Filename { get { return filename; } }

        /// <summary>
        /// The path of a given file.
        /// </summary>
        public string Path { get { return path; } }

        /// <summary>
        /// The text in a given file.
        /// </summary>
        public string Text { get { return text; } }
    }
}
