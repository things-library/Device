namespace ThingsLibrary.Device.Sensor.State
{
    public class HumidityState : SensorState
    {
        public RelativeHumidity Humidity { get; private set; } = default;

        public override double Value => Humidity.Percent;

        public override string ValueString() => $"{this.Value.ToString($"n{this.ValuePrecision}")}{this.UnitSymbol}";

        public void Update(RelativeHumidity? measurement, DateTimeOffset updatedOn)
        {
            if(measurement is null) { return; }

            this.Humidity = measurement.Value;
            this.UpdatedOn = updatedOn;
        }

        public HumidityState(string id = "Humidity", string key = "h", bool isImperial = false) : base(id, key, isImperial)
        {
            this.UnitSymbol = "%";
            this.ValuePrecision = 1;
        }
    }
}
