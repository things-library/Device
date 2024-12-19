namespace ThingsLibrary.Device.Sensor.State
{
    public class TemperatureState : SensorState
    {
        /// <summary>
        /// Temperature
        /// </summary>
        public Temperature Temperature { get; private set; } = default;

        /// <inheritdoc />
        public void Update(Temperature? measurement, DateTimeOffset updatedOn)
        {
            // nothing to do?
            if (this.IsDisabled) { return; }
            if (measurement is null) { return; }

            this.Temperature = measurement.Value;

            var state = (this.IsImperial ? measurement.Value.DegreesFahrenheit : measurement.Value.DegreesCelsius);
            base.Update(state, updatedOn);
        }

        public TemperatureState(string id = "Temperature", string key = "t", bool isImperial = false) : base(id, key, isImperial)
        {
            this.UnitSymbol = (this.IsImperial ? "F" : "C");
            this.ValuePrecision = 1; //78.3f => 783
        }
    }
}
