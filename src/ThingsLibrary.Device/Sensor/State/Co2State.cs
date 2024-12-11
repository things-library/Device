namespace ThingsLibrary.Device.Sensor.State
{
    public class Co2State : SensorState
    {
        public VolumeConcentration Co2 { get; private set; } = default;

        public override double Value => this.Co2.PartsPerMillion;

        public override string ValueString() => $"{this.Value.ToString($"n{this.ValuePrecision}")} {this.UnitSymbol}";

        public void Update(VolumeConcentration? measurement, DateTimeOffset updatedOn)
        {
            // nothing to do.. trying to keep the code clean
            if (measurement is null) { return; }

            this.Co2 = measurement.Value;
            this.UpdatedOn = updatedOn;
        }

        public Co2State(string id = "CO2", string key = "co2", bool isImperial = false) : base(id, key, isImperial)
        {
            this.UnitSymbol = "ppm";
            this.ValuePrecision = 0;
        }
    }
}
