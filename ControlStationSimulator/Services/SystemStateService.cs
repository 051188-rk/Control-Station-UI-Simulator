using System;
using ControlStationSimulator.Models;
using ControlStationSimulator.Utilities;

namespace ControlStationSimulator.Services
{
    /// <summary>
    /// Manages system state transitions with validation
    /// </summary>
    public class SystemStateService
    {
        private SystemState _currentState = SystemState.IDLE;
        private readonly object _stateLock = new object();
        private readonly LoggingService _logger;

        public event EventHandler<SystemState>? StateChanged;

        public SystemState CurrentState
        {
            get
            {
                lock (_stateLock)
                {
                    return _currentState;
                }
            }
        }

        public SystemStateService(LoggingService logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Attempts to transition to a new state with validation
        /// </summary>
        /// <returns>True if transition was successful, false otherwise</returns>
        public bool TransitionTo(SystemState newState)
        {
            lock (_stateLock)
            {
                if (!IsValidTransition(_currentState, newState))
                {
                    _logger.LogWarning($"Invalid state transition from {_currentState} to {newState}");
                    return false;
                }

                var previousState = _currentState;
                _currentState = newState;

                _logger.LogInfo($"State transition: {previousState} -> {newState}");
                StateChanged?.Invoke(this, newState);

                return true;
            }
        }

        /// <summary>
        /// Validates if a state transition is allowed
        /// </summary>
        private bool IsValidTransition(SystemState from, SystemState to)
        {
            // Valid transitions:
            // IDLE -> RUNNING
            // RUNNING -> IDLE
            // RUNNING -> FAULT
            // FAULT -> IDLE (reset)

            return (from, to) switch
            {
                (SystemState.IDLE, SystemState.RUNNING) => true,
                (SystemState.RUNNING, SystemState.IDLE) => true,
                (SystemState.RUNNING, SystemState.FAULT) => true,
                (SystemState.FAULT, SystemState.IDLE) => true,
                _ => false
            };
        }
    }
}
