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
        public DateTime? UpdatedOn { get; set; }

        // ============================================================
        // ABSTRACT
        // ============================================================

        /// <inheritdoc />
        public abstract double Value { get; }

        /// <inheritdoc />
        public abstract string ValueString();


        public override string ToString() => this.ValueString();


        public SensorState(string id, string key, bool isImperical)
        {
            this.Id = id;
            this.Key = key;
            this.IsImperial = isImperical;
        }
    }
}
