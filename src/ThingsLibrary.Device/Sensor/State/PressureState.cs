namespace ThingsLibrary.Device.Sensor.State
{
    public class PressureState : SensorState
    {
        public Pressure Pressure { get; private set; } = default;

        public override double Value => (this.IsImperial ? this.Pressure.InchesOfMercury : this.Pressure.Millibars);

        public override string ValueString() => $"{(this.IsImperial ? this.Value.ToString("0.00") : this.Value.ToString("0"))} {this.UnitSymbol}";

        public void Update(Pressure pressure, DateTime updatedOn)
        {
            this.Pressure = pressure;
            this.UpdatedOn = updatedOn;
        }

        public PressureState(string id = "Pressure", bool isImperial = false)
        {
            this.Id = id;
            this.IsImperial = isImperial;
            this.UnitSymbol = (this.IsImperial ? "in" : "mb");
        }
    }
}
