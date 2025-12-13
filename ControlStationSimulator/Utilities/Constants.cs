namespace ControlStationSimulator.Utilities
{
    /// <summary>
    /// Application-wide constants and default values
    /// </summary>
    public static class Constants
    {
        // Telemetry update interval
        public const int TelemetryUpdateIntervalMs = 1000; // 1 second

        // Default telemetry ranges
        public const double DefaultTemperatureBase = 50.0;     // °C
        public const double TemperatureVariation = 15.0;       // °C

        public const double DefaultPressureBase = 1.0;         // bar
        public const double PressureVariation = 0.3;           // bar

        public const double DefaultPowerOutputBase = 50.0;     // MW
        public const double PowerOutputVariation = 10.0;       // MW

        // Default threshold limits
        public const double DefaultMaxTemperature = 80.0;      // °C
        public const double DefaultMaxPressure = 1.5;          // bar

        // Alert messages
        public const string AlertTemperatureExceeded = "Temperature threshold exceeded!";
        public const string AlertPressureExceeded = "Pressure threshold exceeded!";
        public const string AlertSystemStarted = "System started successfully";
        public const string AlertSystemStopped = "System stopped";
        public const string AlertSystemFaulted = "System entered FAULT state";
        public const string AlertSystemReset = "System reset to IDLE";
        public const string AlertConfigApplied = "Configuration applied successfully";
    }
}
