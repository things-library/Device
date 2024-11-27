using Iot.Device.Scd4x;

namespace ThingsLibrary.Device.I2c
{
    public class Scd40Sensor : Base.I2cSensor
    {
        private Scd4x _device { get; set; }

        // States
        public Co2State Co2State { get; init; }
        public TemperatureState TemperatureState { get; init; }
        public HumidityState HumidityState { get; init; }

        // Calculated from Temp + Humidity
        public TemperatureState HeatIndexState { get; init; }
        public TemperatureState DewPointState { get; init; }


        public Scd40Sensor(I2cBus i2cBus, int id = 0x62, string name = "Scd40", bool isImperial = false) : base(i2cBus, id, name, isImperial)
        {
            // States
            this.Co2State = new Co2State(isImperial: isImperial);
            this.TemperatureState = new TemperatureState(isImperial: isImperial);
            this.HumidityState = new HumidityState(isImperial: isImperial);

            this.HeatIndexState = new TemperatureState("Heat Index", "heat", isImperial: isImperial);
            this.DewPointState = new TemperatureState("Dew Point", "dew", isImperial: isImperial);
            
            this.States = new Dictionary<string, ISensorState>()
            {
                { this.Co2State.Id, this.Co2State },
                { this.TemperatureState.Id, this.TemperatureState },
                { this.HumidityState.Id, this.HumidityState },
                { this.HeatIndexState.Id, this.HeatIndexState },
                { this.DewPointState.Id, this.DewPointState }
            };
        }

        public override void Init()
        {
            try
            {
                base.Init();

                _device = new Scd4x(this.I2cDevice);

                this.MinReadInterval = (int)Scd4x.MeasurementPeriod.TotalMilliseconds;
                if (this.ReadInterval < this.MinReadInterval)
                {
                    this.ReadInterval = this.MinReadInterval;
                    //throw new ArgumentException($"Read interval '{this.ReadInterval} ms' can not be less then min read interval '{this.MinReadInterval} ms' of sensor."); }
                }

                this.IsEnabled = true;
            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.Message;
            }
        }

        public override bool FetchState()
        {
            if (!this.IsEnabled) { return false; }
            if (DateTime.UtcNow < this.NextReadOn) { return false; }

            var readResult = _device.ReadPeriodicMeasurement();
            
            var updatedOn = DateTime.UtcNow;
            var isStateChanged = false;

            // CO2
            if (readResult.CarbonDioxide.HasValue)
            {
                this.Co2State.Update(readResult.CarbonDioxide.Value, updatedOn);
                isStateChanged = true;
            }

            // TEMPERATURE
            if (readResult.Temperature.HasValue)
            {
                this.TemperatureState.Update(readResult.Temperature.Value, updatedOn);
                isStateChanged = true;
            }

            // HUMIDITY
            if (readResult.RelativeHumidity.HasValue)
            {
                this.HumidityState.Update(readResult.RelativeHumidity.Value, updatedOn);
                isStateChanged = true;
            }

            // if all we need it temp and humidity then great!
            if (readResult.Temperature.HasValue && readResult.RelativeHumidity.HasValue)
            {
                var heatIndex = WeatherHelper.CalculateHeatIndex(this.TemperatureState.Temperature, this.HumidityState.Humidity);
                this.HeatIndexState.Update(heatIndex, updatedOn);

                var dewPoint = WeatherHelper.CalculateDewPoint(this.TemperatureState.Temperature, this.HumidityState.Humidity);
                this.DewPointState.Update(dewPoint, updatedOn);
            }


            // see if anyone is listening
            if (isStateChanged)
            {
                this.UpdatedOn = updatedOn;
                this.StateChanged?.Invoke(this, this.States);
            }

            return isStateChanged;
        }
    }
}
