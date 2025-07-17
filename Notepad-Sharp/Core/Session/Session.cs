using System.Collections.Generic;

namespace NotepadSharp.Core.Session
{
    /// <summary>
    /// Object model used to represent the user's current session, all open files/tabs.
    /// </summary>
    public class Session
    {
        private List<TabSession> tabSessions;

        /// <summary>
        /// Constructor for Session data structure.
        /// </summary>
        /// <param name="tabSessions"></param>
        public Session(
            List<TabSession> tabSessions /* List of Tab Session models */
            )
        {
            this.tabSessions = tabSessions;
        }

        /// <summary>
        /// Returns list of all <see cref="TabSession"/> models to be saved to disk.
        /// </summary>
        public List<TabSession> TabSessions { get {  return tabSessions; } }
    }
}
