using Essay_Analysis_Tool.Models;
using System;

namespace Essay_Analysis_Tool.Business
{
    /// <summary>
    /// A Service class for extracting the logic to log
    /// information to a text file in the documents folder.
    /// </summary>
    public class LoggingService
    {
        public event EventHandler<InfoLogEntry> InfoAdded;
        public event EventHandler<WarnLogEntry> WarningAdded;
        public event EventHandler<ErrorLogEntry> ErrorAdded;

        public void Add(InfoLogEntry entry)
        {
            InfoAdded?.Invoke(this, entry);
        }

        public void Add(WarnLogEntry entry)
        {
            WarningAdded?.Invoke(this, entry);
        }

        public void Add(ErrorLogEntry entry)
        {
            ErrorAdded?.Invoke(this, entry);
        }
    }
}
