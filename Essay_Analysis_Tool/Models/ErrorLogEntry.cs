using System;

namespace Essay_Analysis_Tool.Models
{
    public class ErrorLogEntry : LogEntry
    {
        public ErrorLogEntry(string message)
            : base(message)
        {
        }
    }
}
