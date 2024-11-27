using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.FilteringMode;
using ThingsLibrary.Device.Sensor;
using ThingsLibrary.Device.Sensor.State;

// https://learn.adafruit.com/adafruit-bme680-humidity-temperature-barometic-pressure-voc-gas

namespace ThingsLibrary.Device.I2c
{
    public class Bme680Sensor : Base.I2cSensor, ISensorStates
    {
        private Bme680 _device { get; set; }
                
        public TemperatureState TemperatureState { get; init; }
        public HumidityState HumidityState { get; init; }
        public PressureState PressureState { get; init; }
        public LengthState AltitudeState { get; init; }
        public GasState GasState { get; init; }

        /// <inheritdoc/>
        /// <remarks>0x76 is default, 0x77 is secondary</remarks>
        public Bme680Sensor(I2cBus i2cBus, int id = 0x76, string name = "Bme680", bool isImperial = false) : base(i2cBus, id, name, isImperial)
        {
            this.MinReadInterval = 7; //157hz = 6.37ms

            // States
            this.TemperatureState = new TemperatureState(isImperial: isImperial);
            this.HumidityState = new HumidityState(isImperial: isImperial);
            this.PressureState = new PressureState(isImperial: isImperial);
            this.AltitudeState = new LengthState(id: "Altitude", key: "alt", isImperial: isImperial);
            this.GasState = new GasState(isImperial: isImperial);

            this.States = new Dictionary<string, ISensorState>()
            {
                { this.TemperatureState.Id, this.TemperatureState },
                { this.HumidityState.Id, this.HumidityState },
                { this.PressureState.Id, this.PressureState },
                { this.AltitudeState.Id, this.AltitudeState },
                { this.GasState.Id,  this.GasState }
            };
        }

        public override void Init()
        {
            try
            {
                base.Init();

                _device = new Bme680(this.I2cDevice);
                _device.TemperatureSampling = Sampling.Standard;
                _device.HumiditySampling = Sampling.Standard;
                _device.PressureSampling = Sampling.Standard;

                //_device.FilterMode = Bmx280FilteringMode.X16;
                //_device.StandbyTime = StandbyTime.Ms1000;

                //this.MinReadInterval = _device.GetMeasurementDuration();

                // we must enable for this device to work at all.
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
            
            var readResult = _device.Read();
            if (readResult == null) { return false; }

            var updatedOn = DateTime.UtcNow;
            var isStateChanged = false;

            // TEMPERATURE
            if (readResult.Temperature is not null)
            {
                this.TemperatureState.Update(readResult.Temperature.Value, updatedOn);
                isStateChanged = true;
            }

            // TEMPERATURE
            if (readResult.Humidity is not null)
            {
                this.HumidityState.Update(readResult.Humidity.Value, updatedOn);
                isStateChanged = true;
            }

            // PRESSURE
            if (readResult.Pressure is not null)
            {
                this.PressureState.Update(readResult.Pressure.Value, updatedOn);
                isStateChanged = true;
            }

            // ALTITUDE
            if (isStateChanged && this.TemperatureState.UpdatedOn is not null && this.PressureState.UpdatedOn is not null)
            {
                var altitude = WeatherHelper.CalculateAltitude(this.PressureState.Pressure, this.TemperatureState.Temperature);
                this.AltitudeState.Update(altitude, updatedOn);
            }

            // GAS
            if (readResult.GasResistance is not null)
            {
                this.GasState.Update(readResult.GasResistance.Value, updatedOn);
                isStateChanged = true;
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
