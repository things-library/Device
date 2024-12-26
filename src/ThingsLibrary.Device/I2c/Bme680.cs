using Iot.Device.Bmxx80;
using ThingsLibrary.Device.Sensor.Interfaces;

// https://learn.adafruit.com/adafruit-bme680-humidity-temperature-barometic-pressure-voc-gas

namespace ThingsLibrary.Device.I2c
{
    public class Bme680Sensor : Base.I2cSensor
    {
        public Bme680 Device { get; set; }
                
        public TemperatureState TemperatureState { get; init; }
        public HumidityState HumidityState { get; init; }
        public PressureState PressureState { get; init; }
        public LengthState AltitudeState { get; init; }
        public GasState GasState { get; init; }

        /// <inheritdoc/>
        /// <remarks>0x76 is default, 0x77 is secondary</remarks>
        public Bme680Sensor(I2cBus i2cBus, int id = 0x76, string name = "bme680", bool isImperial = false) : base(i2cBus, id, name, isImperial)
        {
            this.MinReadInterval = 7; //157hz = 6.37ms

            // States   
            this.States = new List<ISensorState>(5)
            {
                { this.TemperatureState = new TemperatureState(isImperial: isImperial) },
                { this.HumidityState = new HumidityState(isImperial: isImperial) },
                { this.PressureState = new PressureState(isImperial: isImperial) },
                { this.AltitudeState = new LengthState(id: "Altitude", key: "alt", isImperial: isImperial) { IsDisabled = true } }, //typically don't need altitude data
                { this.GasState = new GasState(isImperial: isImperial) }
            };
        }

        public override void Init()
        {
            try
            {
                base.Init();

                this.Device = new Bme680(this.I2cDevice);
                this.Device.TemperatureSampling = Sampling.Standard;
                this.Device.HumiditySampling = Sampling.Standard;
                this.Device.PressureSampling = Sampling.Standard;

                //_device.FilterMode = Bmx280FilteringMode.X16;
                //_device.StandbyTime = StandbyTime.Ms1000;

                //this.MinReadInterval = this.Device.GetMeasurementDuration();

                // we must enable for this device to work at all.
                this.IsEnabled = true;
            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.Message;
            }
        }

        public override bool FetchStates()
        {
            if (!this.IsEnabled) { return false; }
            if (DateTimeOffset.UtcNow < this.NextReadOn) { return false; }

            try
            {
                var readResult = this.Device.Read();
                if (readResult == null) { return false; }

                var updatedOn = DateTime.UtcNow;
                var isStateChanged = false;

                // TEMPERATURE
                if (readResult.Temperature is not null)
                {
                    this.TemperatureState.Update(readResult.Temperature, updatedOn);
                    isStateChanged = true;
                }

                // TEMPERATURE
                if (readResult.Humidity is not null)
                {
                    this.HumidityState.Update(readResult.Humidity, updatedOn);
                    isStateChanged = true;
                }

                // PRESSURE
                if (readResult.Pressure is not null)
                {
                    this.PressureState.Update(readResult.Pressure, updatedOn);
                    isStateChanged = true;
                }

                // GAS
                if (readResult.GasResistance is not null)
                {
                    this.GasState.Update(readResult.GasResistance, updatedOn);
                    isStateChanged = true;
                }

                // CALCULATE ALTITUDE
                if (isStateChanged && !this.AltitudeState.IsDisabled && !this.TemperatureState.IsDisabled && !this.PressureState.IsDisabled)
                {
                    var altitude = WeatherHelper.CalculateAltitude(this.PressureState.Pressure, this.TemperatureState.Temperature);
                    this.AltitudeState.Update(altitude, updatedOn);
                }


                // see if anyone is listening
                if (isStateChanged)
                {
                    this.UpdatedOn = updatedOn;
                    this.StatesChanged?.Invoke(this, this.States);
                }

                return isStateChanged;
            }
            catch(Exception ex)
            {
                this.ErrorMessage = ex.Message;
                return false;
            }
        }
    }
}
