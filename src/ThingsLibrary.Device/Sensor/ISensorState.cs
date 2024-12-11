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
        /// State Value
        /// </summary>
        public abstract double Value { get; }

        /// <summary>
        /// Number of digits after the decimal, also how much the data is scaled for telemetry (0 = no scaling)
        /// </summary>
        /// <example>1 = for temp so 78.1 becomes 781 in telemetry data</example>
        public byte ValuePrecision { get; }

        /// <summary>
        /// Value as a string with unit symbol
        /// </summary>
        /// <returns></returns>
        public abstract string ValueString();


        /// <summary>
        /// State Changed Timestamp
        /// </summary>
        public DateTimeOffset? UpdatedOn { get; }

        /// <summary>
        /// should this sensor state be emitted in telemetry and other outputs?
        /// </summary>
        public bool IsDisabled { get; set; }
    }
}
