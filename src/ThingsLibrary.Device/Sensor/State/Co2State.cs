namespace ThingsLibrary.Device.Sensor.State
{
    public class Co2State : SensorState
    {
        public VolumeConcentration Co2 { get; private set; } = default;

        public override double Value => this.Co2.PartsPerMillion;

        public override string ValueString() => $"{this.Value.ToString($"n{this.ValuePrecision}")} {this.UnitSymbol}";

        public void Update(VolumeConcentration co2, DateTime updatedOn)
        {
            this.Co2 = co2;
            this.UpdatedOn = updatedOn;
        }

        public Co2State(string id = "CO2", string key = "c", bool isImperial = false) : base(id, key, isImperial)
        {
            this.UnitSymbol = "ppm";
            this.ValuePrecision = 0;
        }
    }
}
