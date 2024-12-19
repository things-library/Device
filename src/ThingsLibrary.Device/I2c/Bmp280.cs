﻿using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.FilteringMode;
using ThingsLibrary.Device.Sensor.Interfaces;

// https://docs.microsoft.com/en-us/dotnet/iot/tutorials/temp-sensor
// https://learn.adafruit.com/adafruit-bmp280-barometric-pressure-plus-temperature-sensor-breakout

namespace ThingsLibrary.Device.I2c
{
    public class Bmp280Sensor : Base.I2cSensor, ISensorStates
    {
        public Bmp280 Device { get; set; }
                
        public TemperatureState TemperatureState { get; init; }
        public PressureState PressureState { get; init; }
        public LengthState AltitudeState { get; init; }

        /// <inheritdoc/>
        /// <remarks>0x77 is default, 0x76 is secondary</remarks>
        public Bmp280Sensor(I2cBus i2cBus, int id = 0x77, string name = "bmp280", bool isImperial = false) : base(i2cBus, id, name, isImperial)
        {
            this.MinReadInterval = 7; //157hz = 6.37ms

            // States
            this.States = new List<ISensorState>(3)
            {
                { this.TemperatureState = new TemperatureState(isImperial: isImperial) },
                { this.PressureState = new PressureState(isImperial: isImperial) },
                { this.AltitudeState = new LengthState(id: "Altitude", key: "alt", isImperial: isImperial) { IsDisabled = true } }  //typically don't need altitude data
            };
        }

        public override void Init()
        {
            try
            {
                base.Init();

                this.Device = new Bmp280(this.I2cDevice);
                this.Device.TemperatureSampling = Sampling.Standard;
                this.Device.PressureSampling = Sampling.Standard;

                this.Device.FilterMode = Bmx280FilteringMode.X16;
                this.Device.StandbyTime = StandbyTime.Ms1000;

                this.MinReadInterval = this.Device.GetMeasurementDuration();

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

            try
            {
                var readResult = Device.Read();
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
                    this.PressureState.Update(readResult.Pressure, updatedOn);
                    isStateChanged = true;
                }

                // ALTITUDE
                if (isStateChanged && !this.AltitudeState.IsDisabled && this.TemperatureState.UpdatedOn is not null && this.PressureState.UpdatedOn is not null)
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
            catch (Exception ex)
            {
                this.ErrorMessage = ex.Message;
                return false;
            }
        }
    }
}
