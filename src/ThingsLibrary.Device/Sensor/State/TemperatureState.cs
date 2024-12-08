﻿namespace ThingsLibrary.Device.Sensor.State
{
    public class TemperatureState : SensorState
    {
        public Temperature Temperature { get; private set; } = default;

        public override double Value => (this.IsImperial ? Temperature.DegreesFahrenheit : Temperature.DegreesCelsius);

        public override string ValueString() => $"{this.Value.ToString($"n{this.ValuePrecision}")} {this.UnitSymbol}";

        public void Update(Temperature temp, DateTime updatedOn)
        {
            this.Temperature = temp;
            this.UpdatedOn = updatedOn;
        }

        public TemperatureState(string id = "Temperature", string key = "t", bool isImperial = false) : base(id, key, isImperial)
        {
            this.UnitSymbol = (this.IsImperial ? "F" : "C");

            this.ValuePrecision = 1; //78.3f => 783
        }
    }
}
