﻿using Iot.Device.Sht4x;
using ThingsLibrary.Device.Sensor.Interfaces;

// https://docs.microsoft.com/en-us/dotnet/iot/tutorials/temp-sensor
// https://learn.adafruit.com/adafruit-bmp280-barometric-pressure-plus-temperature-sensor-breakout

namespace ThingsLibrary.Device.I2c.Sensor
{
    public class Sht4xSensor : Base.I2cSensor
    {
        public Sht4x Device { get; set; }

        public TemperatureState TemperatureState { get; init; }
        public HumidityState HumidityState { get; init; }

        /// <inheritdoc/>
        /// <remarks>0x44 is default</remarks>
        public Sht4xSensor(I2cBus i2cBus, int id = Sht4x.DefaultI2cAddress, string name = "sht4x", bool isImperial = false) : base(i2cBus, id, name, isImperial)
        {
            this.MinReadInterval = 7; //157hz = 6.37ms

            // States
            this.States = new List<ISensorState>(2)
            {
                { this.TemperatureState = new TemperatureState(isImperial: isImperial) },
                { this.HumidityState = new HumidityState(isImperial: isImperial) }
            };
        }

        public override void Init()
        {
            try
            {
                base.Init();

                this.Device = new Sht4x(this.I2cDevice);

                //this.MinReadInterval = _device.GetMeasurementDuration();

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
                var (humidity, temp) = this.Device.ReadHumidityAndTemperature();
                if (humidity is null || temp is null) { return false; }

                var updatedOn = DateTimeOffset.UtcNow;
                var isStateChanged = false;

                // TEMPERATURE
                if (temp is not null)
                {
                    this.TemperatureState.Update(temp.Value, updatedOn);
                    isStateChanged = true;
                }

                // HUMIDITY
                if (humidity is not null)
                {
                    this.HumidityState.Update(humidity.Value, updatedOn);
                    isStateChanged = true;
                }

                // see if anyone is listening
                if (isStateChanged)
                {
                    this.UpdatedOn = updatedOn;
                    this.StatesChanged?.Invoke(this, this.States);
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
