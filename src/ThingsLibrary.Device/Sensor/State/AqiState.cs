namespace ThingsLibrary.Device.Sensor.State
{
    public class AqiState : SensorState
    {        
        public override double Value => 0;

        public override string ValueString() => $"{this.Value.ToString("0.0")} {this.UnitSymbol}";

        public void Update(Temperature temp, DateTime updatedOn)
        {
            //this.Temperature = temp;
            this.UpdatedOn = updatedOn;
        }

        public AqiState(string id = "AQI", string key = "aqi", bool isImperial = false) : base(id, key, isImperial)
        {
            //this.UnitSymbol = (isMetric ? "F" : "C");

            throw new NotImplementedException();
        }
    }
}
