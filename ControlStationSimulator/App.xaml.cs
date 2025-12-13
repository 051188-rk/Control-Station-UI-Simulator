using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using ControlStationSimulator.Services;
using ControlStationSimulator.Utilities;
using ControlStationSimulator.ViewModels;
using ControlStationSimulator.Views;

namespace ControlStationSimulator
{
    /// <summary>
    /// Application entry point with dependency injection configuration
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider? _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Configure dependency injection
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();

            // Setup global exception handling
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            DispatcherUnhandledException += OnDispatcherUnhandledException;

            // Create and show main window
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            var viewModel = _serviceProvider.GetRequiredService<MainViewModel>();
            mainWindow.DataContext = viewModel;
            mainWindow.Show();

            // Log application startup
            var logger = _serviceProvider.GetRequiredService<LoggingService>();
            logger.LogInfo("Control Station UI Simulator started");
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Register services as singletons
            services.AddSingleton<LoggingService>();
            services.AddSingleton<AlertService>();
            services.AddSingleton<SystemStateService>();
            services.AddSingleton<TelemetrySimulationService>();
            services.AddSingleton<ControlEngineService>();

            // Register ViewModels
            services.AddSingleton<MainViewModel>();

            // Register Views
            services.AddSingleton<MainWindow>();
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var logger = _serviceProvider?.GetService<LoggingService>();
            logger?.LogError("Unhandled exception", e.ExceptionObject as Exception);
            
            MessageBox.Show(
                "A critical error occurred. Please check the logs.",
                "Critical Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            var logger = _serviceProvider?.GetService<LoggingService>();
            logger?.LogError("Dispatcher unhandled exception", e.Exception);
            
            MessageBox.Show(
                $"An error occurred: {e.Exception.Message}",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            
            e.Handled = true;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Cleanup
            var telemetryService = _serviceProvider?.GetService<TelemetrySimulationService>();
            telemetryService?.Dispose();

            var logger = _serviceProvider?.GetService<LoggingService>();
            logger?.LogInfo("Control Station UI Simulator shutdown");

            _serviceProvider?.Dispose();
            base.OnExit(e);
        }
    }
}
