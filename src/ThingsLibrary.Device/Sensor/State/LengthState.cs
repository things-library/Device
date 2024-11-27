namespace ThingsLibrary.Device.Sensor.State
{
    public class LengthState : SensorState
    {
        public Length Altitude { get; private set; } = default;

        public override double Value => (this.IsImperial ? this.Altitude.Feet : this.Altitude.Meters);

        public override string ValueString() => $"{this.Value.ToString("0.000")} {this.UnitSymbol}";

        public void Update(Length length, DateTime updatedOn)
        {
            this.Altitude = length;
            this.UpdatedOn = updatedOn;
        }

        public LengthState(string id = "Length", string key = "l", bool isImperial = false) : base(id, key, isImperial)
        {
            this.UnitSymbol = (this.IsImperial ? "ft" : "m");
        }
    }
}
