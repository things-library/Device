namespace ThingsLibrary.Device.Sensor.State
{
    public class HumidityState : SensorState
    {
        public RelativeHumidity Humidity { get; private set; } = default;

        public override double Value => Humidity.Percent;

        public override string ValueString() => $"{this.Value.ToString("0.0")}{this.UnitSymbol}";

        public void Update(RelativeHumidity humidity, DateTime updatedOn)
        {
            this.Humidity = humidity;
            this.UpdatedOn = updatedOn;
        }

        public HumidityState(string id = "Humidity", string key = "h", bool isImperial = false) : base(id, key, isImperial)
        {
            this.UnitSymbol = "%";
        }
    }
}
