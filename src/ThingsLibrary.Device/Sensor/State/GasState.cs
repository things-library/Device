namespace ThingsLibrary.Device.Sensor.State
{
    public class GasState : SensorState
    {
        /// <summary>
        /// Gas Resistance
        /// </summary>
        public ElectricResistance GasResistance { get; private set; } = default;

        /// <inheritdoc />
        public void Update(ElectricResistance? measurement, DateTimeOffset updatedOn)
        {
            // nothing to do?
            if (this.IsDisabled) { return; }
            if (measurement is null) { return; }

            this.GasResistance = measurement.Value;
            this.Update(measurement.Value.Ohms, updatedOn);            
        }


        public GasState(string id = "Gas", string key = "g", bool isImperial = false) : base(id, key, isImperial)
        {
            this.UnitSymbol = (this.IsImperial ? "in" : "mb");
        }
    }
}
