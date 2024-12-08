namespace ThingsLibrary.Device.Sensor.State
{
    public class PressureState : SensorState
    {
        public Pressure Pressure { get; private set; } = default;

        public override double Value => (this.IsImperial ? this.Pressure.InchesOfMercury : this.Pressure.Millibars);

        public override string ValueString() => $"{this.Value.ToString($"n{this.ValuePrecision}")} {this.UnitSymbol}";

        public void Update(Pressure pressure, DateTime updatedOn)
        {
            this.Pressure = pressure;
            this.UpdatedOn = updatedOn;
        }

        public PressureState(string id = "Pressure", string key = "p", bool isImperial = false) : base(id, key, isImperial)
        {
            this.UnitSymbol = (this.IsImperial ? "inHg" : "mb");

            this.ValuePrecision = (byte)(this.IsImperial ? 2 : 0);
        }
    }
}
