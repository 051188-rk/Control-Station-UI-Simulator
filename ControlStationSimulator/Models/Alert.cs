using System;

namespace ControlStationSimulator.Models
{
    /// <summary>
    /// Severity level of an alert
    /// </summary>
    public enum AlertSeverity
    {
        INFO,
        WARNING,
        CRITICAL
    }

    /// <summary>
    /// Represents a system alert with severity, message, and timestamp
    /// </summary>
    public class Alert
    {
        public AlertSeverity Severity { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }

        public Alert(AlertSeverity severity, string message)
        {
            Severity = severity;
            Message = message;
            Timestamp = DateTime.Now;
        }

        public override string ToString()
        {
            return $"[{Timestamp:HH:mm:ss}] [{Severity}] {Message}";
        }
    }
}
