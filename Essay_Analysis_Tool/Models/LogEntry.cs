using System;

namespace Essay_Analysis_Tool.Models
{
    public abstract class LogEntry
    {
        public LogEntry(string message)
        {
            Message = $"{message}\n";
            AddedAt = DateTime.UtcNow;
        }

        public string Message { get; }

        public DateTime AddedAt { get; }
    }
}
