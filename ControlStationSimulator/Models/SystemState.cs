namespace ControlStationSimulator.Models
{
    /// <summary>
    /// Represents the operational state of the control system
    /// </summary>
    public enum SystemState
    {
        /// <summary>
        /// System is idle and not executing control logic
        /// </summary>
        IDLE,

        /// <summary>
        /// System is actively running and monitoring telemetry
        /// </summary>
        RUNNING,

        /// <summary>
        /// System has encountered a fault condition and requires reset
        /// </summary>
        FAULT
    }
}
