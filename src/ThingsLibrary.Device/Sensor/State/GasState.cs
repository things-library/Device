namespace ThingsLibrary.Device.Sensor.State
{
    public class GasState : SensorState
    {
        public ElectricResistance GasResistance { get; private set; } = default;

        public override double Value => this.GasResistance.Ohms;

        public override string ValueString() => $"{(this.IsImperial ? this.Value.ToString("0.00") : this.Value.ToString("0"))} {this.UnitSymbol}";

        public void Update(ElectricResistance resistance, DateTime updatedOn)
        {
            this.GasResistance = resistance;
            this.UpdatedOn = updatedOn;
        }

        public GasState(string id = "Gas", bool isImperial = false)
        {
            this.Id = id;
            this.IsImperial = isImperial;
            this.UnitSymbol = (this.IsImperial ? "in" : "mb");
        }
    }
}
