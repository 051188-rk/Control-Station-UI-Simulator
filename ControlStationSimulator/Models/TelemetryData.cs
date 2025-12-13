using System;

namespace ControlStationSimulator.Models
{
    /// <summary>
    /// Container for real-time telemetry data from the control system
    /// </summary>
    public class TelemetryData
    {
        public double Temperature { get; set; }  // °C
        public double Pressure { get; set; }     // bar
        public double PowerOutput { get; set; }  // MW
        public DateTime Timestamp { get; set; }

        public TelemetryData()
        {
            Timestamp = DateTime.Now;
        }

        public TelemetryData(double temperature, double pressure, double powerOutput)
        {
            Temperature = temperature;
            Pressure = pressure;
            PowerOutput = powerOutput;
            Timestamp = DateTime.Now;
        }
    }
}
