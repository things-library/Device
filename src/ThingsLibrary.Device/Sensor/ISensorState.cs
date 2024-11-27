namespace ThingsLibrary.Device.Sensor
{
    /// <summary>
    /// Specific state (Humidity, Temperature, CO2, etc)
    /// </summary>
    public interface ISensorState
    {
        /// <summary>
        /// Sensor ID
        /// </summary>
        public string Id { get; init; }

        /// <summary>
        /// Key
        /// </summary>
        public string Key { get; init; }

        /// <summary>
        /// Output Imperial Values?
        /// </summary>
        public bool IsImperial { get; init; }

        /// <summary>
        /// Unit Symbol
        /// </summary>
        public string UnitSymbol { get; init; }

        /// <summary>
        /// State Changed Timestamp
        /// </summary>
        public DateTime? UpdatedOn { get; }

        // ============================================================
        // ABSTRACT
        // ============================================================

        /// <summary>
        /// State Value
        /// </summary>
        public abstract double Value { get; }

        /// <summary>
        /// Value as a string with unit symbol
        /// </summary>
        /// <returns></returns>
        public abstract string ValueString();                
    }
}
