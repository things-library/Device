using Iot.Device.Vl53L0X;
using ThingsLibrary.Device.Sensor.Interfaces;

// https://learn.adafruit.com/adafruit-vl53l0x-micro-lidar-distance-sensor-breakout

namespace ThingsLibrary.Device.I2c
{
    /// <summary>
    /// Laser distance time of flight ToF sensor
    /// </summary>
    public class Vl53l0xSensor : Base.I2cSensor, ISensorStates
    {
        public Vl53L0X Device { get; set; }

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
        public Vl53l0xSensor(I2cBus i2cBus, int id = 0x29, string name = "vl5310x", bool isImperial = false) : base(i2cBus, id, name, isImperial)
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

                    Vl53L0X.ChangeI2cAddress(this.I2cDevice, (byte)this.NewDeviceId);
                    this.I2cDevice = this.I2cBus.CreateDevice(this.NewDeviceId);
                }

                this.Device = new Vl53L0X(this.I2cDevice)
                {
                    Precision = Precision.ShortRange,
                    MeasurementMode = MeasurementMode.Continuous
                };

                this.Device.StartContinuousMeasurement(10);
                                
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
                var distance = this.Device.Distance;
                if (distance >= (ushort)OperationRange.Maximum) { return false; }

                var updatedOn = DateTime.UtcNow;

                // DISTANCE
                var lengthUnits = Length.FromMillimeters(distance);
                this.DistanceState.Update(lengthUnits, updatedOn);

                // see if anyone is listening
                this.UpdatedOn = updatedOn;
                this.StateChanged?.Invoke(this, this.States);

                // there is always a new reading
                return true;
            }
            catch(Exception ex)
            {
                this.ErrorMessage = ex.Message;

                return false;
            }
        }
    }
}
