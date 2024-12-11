namespace ThingsLibrary.Device.Sensor
{
    /// <summary>
    /// Specific element state (Humidity, Temperature, CO2, etc)
    /// </summary>
    public abstract class SensorState : ISensorState
    {
        /// <inheritdoc />
        public string Id { get; init; }

        /// <inheritdoc />
        public string Key { get; init; }

        /// <inheritdoc />        
        public bool IsImperial { get; init; }
        
        /// <inheritdoc />
        public string UnitSymbol { get; init; }

        /// <inheritdoc />
        public DateTimeOffset? UpdatedOn { get; set; }

        /// <inheritdoc />
        public byte ValuePrecision { get; internal set; }

        /// <inheritdoc />
        public abstract double Value { get; }

        /// <inheritdoc />
        public abstract string ValueString();


        public override string ToString() => this.ValueString();

        /// <inheritdoc />
        public bool IsDisabled { get; set; }

        public SensorState(string id, string key, bool isImperical)
        {
            this.Id = id;
            this.Key = key;
            this.IsImperial = isImperical;
        }
    }
}
