namespace ThingsLibrary.Device.Sensor.State
{
    public class LengthState : SensorState
    {
        public Length Length { get; private set; } = default;

        public override double Value => (this.IsImperial ? this.Length.Feet : this.Length.Meters);

        public override string ValueString() => $"{this.Value.ToString($"n{this.ValuePrecision}")} {this.UnitSymbol}";

        public void Update(Length? measurement, DateTimeOffset updatedOn)
        {
            // nothing to do.. trying to keep the code clean
            if (measurement is null) { return; }

            this.Length = measurement.Value;
            this.UpdatedOn = updatedOn;
        }

        public LengthState(string id = "Length", string key = "l", bool isImperial = false, byte telemetryScaleFactor = 0) : base(id, key, isImperial)
        {
            this.UnitSymbol = (this.IsImperial ? "ft" : "m");
            this.ValuePrecision = (byte)(this.IsImperial ? 2 : 3);
        }
    }
}
