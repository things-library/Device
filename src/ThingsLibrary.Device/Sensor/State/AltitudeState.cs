namespace ThingsLibrary.Device.Sensor.State
{
    public class AltitudeState : SensorState
    {
        public Length Altitude { get; private set; } = default;

        public override double Value => (this.IsImperial ? this.Altitude.Feet : this.Altitude.Meters);

        public override string ValueString() => $"{this.Value.ToString("0.0")} {this.UnitSymbol}";

        public void Update(Length length, DateTime updatedOn)
        {
            this.Altitude = length;
            this.UpdatedOn = updatedOn;
        }

        public AltitudeState(string id = "Altitude", bool isImperial = false)
        {
            this.Id = id;
            this.IsImperial = isImperial;
            this.UnitSymbol = (isImperial ? "ft" : "m");
        }
    }
}
