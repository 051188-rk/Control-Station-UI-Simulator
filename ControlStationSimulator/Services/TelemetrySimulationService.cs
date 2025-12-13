using System;
using System.Timers;
using ControlStationSimulator.Models;
using ControlStationSimulator.Utilities;

namespace ControlStationSimulator.Services
{
    /// <summary>
    /// Simulates real-time telemetry data with deterministic patterns
    /// </summary>
    public class TelemetrySimulationService : IDisposable
    {
        private readonly Timer _timer;
        private readonly LoggingService _logger;
        private double _elapsedSeconds = 0;
        private bool _isRunning = false;

        public event EventHandler<TelemetryData>? TelemetryUpdated;

        public TelemetrySimulationService(LoggingService logger)
        {
            _logger = logger;
            _timer = new Timer(Constants.TelemetryUpdateIntervalMs);
            _timer.Elapsed += OnTimerElapsed;
        }

        /// <summary>
        /// Starts the telemetry simulation
        /// </summary>
        public void Start()
        {
            if (_isRunning)
                return;

            _isRunning = true;
            _elapsedSeconds = 0;
            _timer.Start();
            _logger.LogInfo("Telemetry simulation started");
        }

        /// <summary>
        /// Stops the telemetry simulation
        /// </summary>
        public void Stop()
        {
            if (!_isRunning)
                return;

            _isRunning = false;
            _timer.Stop();
            _logger.LogInfo("Telemetry simulation stopped");
        }

        /// <summary>
        /// Generates deterministic telemetry values using sine/cosine waves
        /// </summary>
        private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            _elapsedSeconds += Constants.TelemetryUpdateIntervalMs / 1000.0;

            // Deterministic simulation using trigonometric functions
            // Temperature: sine wave with period ~60 seconds
            double temperature = Constants.DefaultTemperatureBase + 
                               Constants.TemperatureVariation * Math.Sin(2 * Math.PI * _elapsedSeconds / 60.0);

            // Pressure: cosine wave with period ~45 seconds
            double pressure = Constants.DefaultPressureBase + 
                            Constants.PressureVariation * Math.Cos(2 * Math.PI * _elapsedSeconds / 45.0);

            // Power Output: combined wave with period ~30 seconds
            double powerOutput = Constants.DefaultPowerOutputBase + 
                               Constants.PowerOutputVariation * Math.Sin(2 * Math.PI * _elapsedSeconds / 30.0) *
                               Math.Cos(2 * Math.PI * _elapsedSeconds / 40.0);

            var telemetryData = new TelemetryData(temperature, pressure, powerOutput);

            // Raise event on background thread (subscribers must marshal to UI thread)
            TelemetryUpdated?.Invoke(this, telemetryData);
        }

        public void Dispose()
        {
            _timer?.Stop();
            _timer?.Dispose();
        }
    }
}
