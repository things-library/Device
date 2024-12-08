namespace ThingsLibrary.Device.Sensor.State
{
    public class GasState : SensorState
    {
        public ElectricResistance GasResistance { get; private set; } = default;

        public override double Value => this.GasResistance.Ohms;

        public override string ValueString() => $"{this.Value.ToString($"n{this.ValuePrecision}")} {this.UnitSymbol}";

        public void Update(ElectricResistance resistance, DateTime updatedOn)
        {
            this.GasResistance = resistance;
            this.UpdatedOn = updatedOn;
        }

        public GasState(string id = "Gas", string key = "g", bool isImperial = false) : base(id, key, isImperial)
        {
            this.UnitSymbol = (this.IsImperial ? "in" : "mb");
        }
    }
}
