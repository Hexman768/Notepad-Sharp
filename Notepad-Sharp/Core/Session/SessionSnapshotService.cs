using System.Xml;

namespace NotepadSharp.Core.Session
{
    /// <summary>
    /// Session Snapshot Writing serivce implementation.
    /// </summary>
    public class SessionSnapshotService : ISessionSnapshotService
    {
        /// <summary>
        /// Writes the given session object to a session.xml file in the user's local appdata directory.
        /// </summary>
        /// <param name="session"></param>
        public void WriteSession(Session session)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            XmlWriter writer = XmlWriter.Create("settings.xml", settings);

            writer.WriteStartDocument();

            writer.WriteComment("Auto-Generated settings file.");

            writer.WriteStartElement("");
        }

        private void Write(Session session)
        {
            // TODO: implement the behavior to write session to user local appdata
        }
    }
}
