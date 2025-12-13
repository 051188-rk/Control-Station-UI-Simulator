using ControlStationSimulator.Models;
using ControlStationSimulator.Utilities;

namespace ControlStationSimulator.Services
{
    /// <summary>
    /// Control logic engine that validates configurations and evaluates threshold rules
    /// </summary>
    public class ControlEngineService
    {
        private readonly AlertService _alertService;
        private readonly SystemStateService _stateService;
        private readonly LoggingService _logger;

        private double _maxTemperature = Constants.DefaultMaxTemperature;
        private double _maxPressure = Constants.DefaultMaxPressure;

        public double MaxTemperature
        {
            get => _maxTemperature;
            set => _maxTemperature = value;
        }

        public double MaxPressure
        {
            get => _maxPressure;
            set => _maxPressure = value;
        }

        public ControlEngineService(
            AlertService alertService,
            SystemStateService stateService,
            LoggingService logger)
        {
            _alertService = alertService;
            _stateService = stateService;
            _logger = logger;
        }

        /// <summary>
        /// Validates configuration setpoints
        /// </summary>
        public bool ValidateConfiguration(double maxTemp, double maxPress)
        {
            if (maxTemp <= 0 || maxTemp > 200)
            {
                _alertService.AddAlert(AlertSeverity.WARNING, "Invalid temperature setpoint (must be 0-200°C)");
                return false;
            }

            if (maxPress <= 0 || maxPress > 10)
            {
                _alertService.AddAlert(AlertSeverity.WARNING, "Invalid pressure setpoint (must be 0-10 bar)");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Applies new configuration setpoints
        /// </summary>
        public void ApplyConfiguration(double maxTemp, double maxPress)
        {
            _maxTemperature = maxTemp;
            _maxPressure = maxPress;
            _alertService.AddAlert(AlertSeverity.INFO, Constants.AlertConfigApplied);
            _logger.LogInfo($"Configuration applied: MaxTemp={maxTemp}°C, MaxPress={maxPress} bar");
        }

        /// <summary>
        /// Evaluates telemetry against threshold rules and triggers alerts
        /// </summary>
        public void EvaluateTelemetry(TelemetryData telemetry)
        {
            // Only evaluate if system is running
            if (_stateService.CurrentState != SystemState.RUNNING)
                return;

            bool faultDetected = false;

            // Check temperature threshold
            if (telemetry.Temperature > _maxTemperature)
            {
                _alertService.AddAlert(AlertSeverity.CRITICAL, 
                    $"{Constants.AlertTemperatureExceeded} Current: {telemetry.Temperature:F1}°C, Max: {_maxTemperature:F1}°C");
                _logger.LogWarning($"Temperature threshold exceeded: {telemetry.Temperature:F1}°C > {_maxTemperature:F1}°C");
                faultDetected = true;
            }

            // Check pressure threshold
            if (telemetry.Pressure > _maxPressure)
            {
                _alertService.AddAlert(AlertSeverity.CRITICAL, 
                    $"{Constants.AlertPressureExceeded} Current: {telemetry.Pressure:F2} bar, Max: {_maxPressure:F2} bar");
                _logger.LogWarning($"Pressure threshold exceeded: {telemetry.Pressure:F2} bar > {_maxPressure:F2} bar");
                faultDetected = true;
            }

            // Transition to FAULT state if any threshold exceeded
            if (faultDetected)
            {
                if (_stateService.TransitionTo(SystemState.FAULT))
                {
                    _alertService.AddAlert(AlertSeverity.CRITICAL, Constants.AlertSystemFaulted);
                }
            }
        }
    }
}
