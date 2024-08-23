namespace ThingsLibrary.Device.Sensor.State
{
    public class Co2State : SensorState
    {
        public VolumeConcentration Co2 { get; private set; } = default;

        public override double Value => this.Co2.PartsPerMillion;

        public override string ValueString() => $"{this.Value.ToString("0")} {this.UnitSymbol}";

        public void Update(VolumeConcentration co2, DateTime updatedOn)
        {
            this.Co2 = co2;
            this.UpdatedOn = updatedOn;
        }

        public Co2State(string id = "CO2", bool isImperial = false)
        {
            this.Id = id;
            this.IsImperial = isImperial;
            this.UnitSymbol = "ppm";
        }
    }
}
