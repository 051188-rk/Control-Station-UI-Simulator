namespace ControlStationSimulator.Models
{
    /// <summary>
    /// Represents a configurable control parameter with bounds and current value
    /// </summary>
    public class ControlParameter
    {
        public string Name { get; set; }
        public double CurrentValue { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public string Unit { get; set; }

        public ControlParameter(string name, double currentValue, double minValue, double maxValue, string unit)
        {
            Name = name;
            CurrentValue = currentValue;
            MinValue = minValue;
            MaxValue = maxValue;
            Unit = unit;
        }

        /// <summary>
        /// Checks if the current value is within acceptable bounds
        /// </summary>
        public bool IsInBounds()
        {
            return CurrentValue >= MinValue && CurrentValue <= MaxValue;
        }
    }
}
