using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.FilteringMode;

// https://docs.microsoft.com/en-us/dotnet/iot/tutorials/temp-sensor
// https://learn.adafruit.com/adafruit-bmp280-barometric-pressure-plus-temperature-sensor-breakout

namespace ThingsLibrary.Device.I2c
{
    public class Bmp280Sensor : Base.I2cSensor, ISensorStates
    {
        private Bmp280 _device { get; set; }
                
        public TemperatureState TemperatureState { get; init; }
        public PressureState PressureState { get; init; }
        public AltitudeState AltitudeState { get; init; }

        /// <inheritdoc/>
        /// <remarks>0x77 is default, 0x76 is secondary</remarks>
        public Bmp280Sensor(I2cBus i2cBus, int id = 0x77, string name = "Bmp280", bool isImperial = false) : base(i2cBus, id, name, isImperial)
        {
            this.MinReadInterval = 7; //157hz = 6.37ms

            // States
            this.TemperatureState = new TemperatureState(isImperial: isImperial);
            this.PressureState = new PressureState(isImperial: isImperial);
            this.AltitudeState = new AltitudeState(isImperial: isImperial);

            this.States = new Dictionary<string, ISensorState>()
            {
                { this.TemperatureState.Id, this.TemperatureState },
                { this.PressureState.Id, this.PressureState },
                { this.AltitudeState.Id, this.AltitudeState }
            };
        }

        public override void Init()
        {
            try
            {
                base.Init();

                _device = new Bmp280(this.I2cDevice);
                _device.TemperatureSampling = Sampling.Standard;
                _device.PressureSampling = Sampling.Standard;

                _device.FilterMode = Bmx280FilteringMode.X16;
                _device.StandbyTime = StandbyTime.Ms1000;

                this.MinReadInterval = _device.GetMeasurementDuration();

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
