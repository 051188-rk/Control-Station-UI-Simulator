using System;
using System.Diagnostics;

namespace ControlStationSimulator.Utilities
{
    /// <summary>
    /// Simple logging service for debugging and monitoring
    /// </summary>
    public class LoggingService
    {
        public void LogInfo(string message)
        {
            Log("INFO", message);
        }

        public void LogWarning(string message)
        {
            Log("WARNING", message);
        }

        public void LogError(string message, Exception? exception = null)
        {
            Log("ERROR", message);
            if (exception != null)
            {
                Log("ERROR", $"Exception: {exception.Message}");
                Log("ERROR", $"StackTrace: {exception.StackTrace}");
            }
        }

        private void Log(string level, string message)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var logMessage = $"[{timestamp}] [{level}] {message}";
            Debug.WriteLine(logMessage);
            Console.WriteLine(logMessage);
        }
    }
}
