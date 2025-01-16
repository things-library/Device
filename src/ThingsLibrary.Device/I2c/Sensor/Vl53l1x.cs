﻿using Iot.Device.Vl53L1X;

// 

namespace ThingsLibrary.Device.I2c.Sensor
{
    /// <summary>
    /// Laser distance time of flight ToF sensor
    /// </summary>
    /// <seealso cref="https://learn.adafruit.com/adafruit-vl53l0x-micro-lidar-distance-sensor-breakout" />
    public class Vl53l1xSensor : Base.I2cSensor
    {
        public Vl53L1X Device { get; set; }

        /// <summary>
        /// If reassigning the device ID during initilization
        /// </summary>
        public int NewDeviceId { get; set; } = -1;  // -1 = not assigned

        /// <summary>
        /// Shutdown Pin, when pulled low the sensor goes into shutdown mode.
        /// </summary>
        public int ShutdownPinId { get; set; } = -1; //-1 = not assigned

        /// <summary>
        /// Used for putting the sensors in shutdown mode
        /// </summary>
        public Vl53l0xSensorGroup SensorGroup { get; set; }

        /// <summary>
        /// Distance Measurement
        /// </summary>
        public LengthState DistanceState { get; init; }

        /// <inheritdoc/>
        /// <remarks>0x77 is default</remarks>
        public Vl53l1xSensor(I2cBus i2cBus, int id = 0x29, string name = "vl5311x", bool isImperial = false) : base(i2cBus, id, name, isImperial)
        {
            // States
            this.States = new List<ISensorState>()
            {
                { this.DistanceState = new LengthState("Distance", "d", isImperial: isImperial) }
            };
        }

        public override void Init()
        {
            try
            {
                base.Init();

                // are we reassigning the ID using the shutdown PIN?
                if (this.NewDeviceId > 0)
                {
                    //TODO: shutdown all other sensors that we know about using the shutdown pins

                    Vl53L1X.ChangeI2CAddress(this.I2cDevice, (byte)this.NewDeviceId);
                    this.I2cDevice = this.I2cBus.CreateDevice(this.NewDeviceId);
                }

                this.Device = new Vl53L1X(this.I2cDevice)
                {
                    Precision = Precision.Short,
                    Roi = new Roi(4, 4)
                };

                //this.Device.StartContinuousMeasurement(10);

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
                var distance = this.Device.GetDistance();
                //    if (distance >= (ushort)OperationRange.Maximum) { return false; }

                var updatedOn = DateTime.UtcNow;

                // DISTANCE            
                this.DistanceState.Update(distance, updatedOn);

                // see if anyone is listening
                this.UpdatedOn = updatedOn;
                this.StatesChanged?.Invoke(this, this.States);

                // there is always a new reading
                return true;
            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.Message;

                return false;
            }
        }
    }
}
