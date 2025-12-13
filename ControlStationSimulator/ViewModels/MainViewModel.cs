using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ControlStationSimulator.Models;
using ControlStationSimulator.Services;
using ControlStationSimulator.Utilities;

namespace ControlStationSimulator.ViewModels
{
    /// <summary>
    /// Main ViewModel for the control station UI - orchestrates all services and UI state
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly TelemetrySimulationService _telemetryService;
        private readonly ControlEngineService _controlEngine;
        private readonly AlertService _alertService;
        private readonly SystemStateService _stateService;
        private readonly LoggingService _logger;

        // Telemetry properties
        private double _temperature;
        private double _pressure;
        private double _powerOutput;

        // State properties
        private SystemState _currentSystemState;
        private string _stateDisplayText = "IDLE";
        private Brush _stateColor = Brushes.Gray;

        // Configuration properties
        private double _maxTemperature;
        private double _maxPressure;

        // Commands
        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand ApplyConfigCommand { get; }
        public ICommand ResetCommand { get; }

        // Observable collections
        public ObservableCollection<Alert> Alerts { get; }

        #region Properties

        public double Temperature
        {
            get => _temperature;
            set => SetProperty(ref _temperature, value);
        }

        public double Pressure
        {
            get => _pressure;
            set => SetProperty(ref _pressure, value);
        }

        public double PowerOutput
        {
            get => _powerOutput;
            set => SetProperty(ref _powerOutput, value);
        }

        public SystemState CurrentSystemState
        {
            get => _currentSystemState;
            set => SetProperty(ref _currentSystemState, value);
        }

        public string StateDisplayText
        {
            get => _stateDisplayText;
            set => SetProperty(ref _stateDisplayText, value);
        }

        public Brush StateColor
        {
            get => _stateColor;
            set => SetProperty(ref _stateColor, value);
        }

        public double MaxTemperature
        {
            get => _maxTemperature;
            set => SetProperty(ref _maxTemperature, value);
        }

        public double MaxPressure
        {
            get => _maxPressure;
            set => SetProperty(ref _maxPressure, value);
        }

        #endregion

        public MainViewModel(
            TelemetrySimulationService telemetryService,
            ControlEngineService controlEngine,
            AlertService alertService,
            SystemStateService stateService,
            LoggingService logger)
        {
            _telemetryService = telemetryService;
            _controlEngine = controlEngine;
            _alertService = alertService;
            _stateService = stateService;
            _logger = logger;

            // Initialize collections
            Alerts = _alertService.Alerts;

            // Initialize configuration with defaults
            _maxTemperature = Constants.DefaultMaxTemperature;
            _maxPressure = Constants.DefaultMaxPressure;

            // Initialize commands
            StartCommand = new RelayCommand(ExecuteStart, CanExecuteStart);
            StopCommand = new RelayCommand(ExecuteStop, CanExecuteStop);
            ApplyConfigCommand = new RelayCommand(ExecuteApplyConfig);
            ResetCommand = new RelayCommand(ExecuteReset, CanExecuteReset);

            // Subscribe to events
            _telemetryService.TelemetryUpdated += OnTelemetryUpdated;
            _stateService.StateChanged += OnStateChanged;

            // Initialize state
            UpdateStateDisplay(_stateService.CurrentState);

            _logger.LogInfo("MainViewModel initialized");
        }

        #region Command Implementations

        private bool CanExecuteStart()
        {
            return _stateService.CurrentState == SystemState.IDLE;
        }

        private void ExecuteStart()
        {
            if (_stateService.TransitionTo(SystemState.RUNNING))
            {
                _telemetryService.Start();
                _alertService.AddAlert(AlertSeverity.INFO, Constants.AlertSystemStarted);
                _logger.LogInfo("System started by user");
            }
        }

        private bool CanExecuteStop()
        {
            return _stateService.CurrentState == SystemState.RUNNING;
        }

        private void ExecuteStop()
        {
            if (_stateService.TransitionTo(SystemState.IDLE))
            {
                _telemetryService.Stop();
                _alertService.AddAlert(AlertSeverity.INFO, Constants.AlertSystemStopped);
                _logger.LogInfo("System stopped by user");
            }
        }

        private void ExecuteApplyConfig()
        {
            if (_controlEngine.ValidateConfiguration(MaxTemperature, MaxPressure))
            {
                _controlEngine.ApplyConfiguration(MaxTemperature, MaxPressure);
            }
        }

        private bool CanExecuteReset()
        {
            return _stateService.CurrentState == SystemState.FAULT;
        }

        private void ExecuteReset()
        {
            if (_stateService.TransitionTo(SystemState.IDLE))
            {
                _telemetryService.Stop();
                _alertService.AddAlert(AlertSeverity.INFO, Constants.AlertSystemReset);
                _logger.LogInfo("System reset by user");
            }
        }

        #endregion

        #region Event Handlers

        private void OnTelemetryUpdated(object? sender, TelemetryData telemetry)
        {
            // Marshal to UI thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                Temperature = telemetry.Temperature;
                Pressure = telemetry.Pressure;
                PowerOutput = telemetry.PowerOutput;

                // Evaluate telemetry against thresholds
                _controlEngine.EvaluateTelemetry(telemetry);
            });
        }

        private void OnStateChanged(object? sender, SystemState newState)
        {
            // Marshal to UI thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                CurrentSystemState = newState;
                UpdateStateDisplay(newState);

                // Refresh command states
                CommandManager.InvalidateRequerySuggested();
            });
        }

        private void UpdateStateDisplay(SystemState state)
        {
            StateDisplayText = state.ToString();

            StateColor = state switch
            {
                SystemState.IDLE => Brushes.Gray,
                SystemState.RUNNING => Brushes.LimeGreen,
                SystemState.FAULT => Brushes.Red,
                _ => Brushes.Gray
            };
        }

        #endregion
    }
}
