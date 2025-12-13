using System;
using System.Collections.ObjectModel;
using System.Windows;
using ControlStationSimulator.Models;

namespace ControlStationSimulator.Services
{
    /// <summary>
    /// Manages system alerts with thread-safe observable collection
    /// </summary>
    public class AlertService
    {
        private readonly ObservableCollection<Alert> _alerts;
        private readonly object _lockObject = new object();

        public ObservableCollection<Alert> Alerts => _alerts;

        public AlertService()
        {
            _alerts = new ObservableCollection<Alert>();
        }

        /// <summary>
        /// Adds a new alert to the collection (thread-safe)
        /// </summary>
        public void AddAlert(AlertSeverity severity, string message)
        {
            var alert = new Alert(severity, message);

            // Ensure UI thread marshaling
            Application.Current?.Dispatcher.Invoke(() =>
            {
                lock (_lockObject)
                {
                    _alerts.Add(alert);

                    // Keep only last 100 alerts to prevent memory buildup
                    if (_alerts.Count > 100)
                    {
                        _alerts.RemoveAt(0);
                    }
                }
            });
        }

        /// <summary>
        /// Clears all alerts
        /// </summary>
        public void ClearAlerts()
        {
            Application.Current?.Dispatcher.Invoke(() =>
            {
                lock (_lockObject)
                {
                    _alerts.Clear();
                }
            });
        }
    }
}
