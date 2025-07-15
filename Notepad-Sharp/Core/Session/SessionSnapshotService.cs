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
        /// <exception cref="System.NotImplementedException"></exception>
        public void WriteSession(Session session)
        {
            throw new System.NotImplementedException();
        }

        private void Write(Session session)
        {
            // TODO: implement the behavior to write session to user local appdata
        }
    }
}
