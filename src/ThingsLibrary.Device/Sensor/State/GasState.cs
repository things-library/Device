namespace ThingsLibrary.Device.Sensor.State
{
    public class GasState : SensorState
    {
        public ElectricResistance GasResistance { get; private set; } = default;

        public override double Value => this.GasResistance.Ohms;

        public override string ValueString() => $"{this.Value.ToString($"n{this.ValuePrecision}")} {this.UnitSymbol}";

        public void Update(ElectricResistance? measurement, DateTimeOffset updatedOn)
        {
            // nothing to do.. trying to keep the code clean
            if (measurement is null) { return; }

            this.GasResistance = measurement.Value;
            this.UpdatedOn = updatedOn;
        }

        public GasState(string id = "Gas", string key = "g", bool isImperial = false) : base(id, key, isImperial)
        {
            this.UnitSymbol = (this.IsImperial ? "in" : "mb");
        }
    }
}
