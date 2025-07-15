namespace NotepadSharp.Core.Session
{
    /// <summary>
    /// Interface for the session snapshot writing service.
    /// </summary>
    public interface ISessionSnapshotService
    {
        /// <summary>
        /// Writes the given session snapshot to the user's local appdata directory.
        /// </summary>
        /// <param name="session"></param>
        void WriteSession(Session session);
    }
}
