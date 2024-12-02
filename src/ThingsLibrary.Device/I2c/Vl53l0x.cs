using Iot.Device.Vl53L0X;

// https://learn.adafruit.com/adafruit-vl53l0x-micro-lidar-distance-sensor-breakout

namespace ThingsLibrary.Device.I2c
{
    public class Vl53l0xSensor : Base.I2cSensor, ISensorStates
    {
        public Vl53L0X Device { get; set; }
        
        public LengthState DistanceState { get; init; }

        /// <inheritdoc/>
        /// <remarks>0x77 is default</remarks>
        public Vl53l0xSensor(I2cBus i2cBus, int id = 0x29, string name = "Vl5310x", bool isImperial = false) : base(i2cBus, id, name, isImperial)
        {            
            // States
            this.DistanceState = new LengthState("Distance", "dist", isImperial: isImperial);

            this.States = new Dictionary<string, ISensorState>()
            {
                { this.DistanceState.Id, this.DistanceState }
            };
        }

        public override void Init()
        {
            try
            {
                base.Init();

                Device = new Vl53L0X(this.I2cDevice)
                {
                    Precision = Precision.ShortRange,
                    MeasurementMode = MeasurementMode.Continuous
                };

                Device.StartContinuousMeasurement(10);
                                
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

            var distance = Device.Distance;  //in ms
            if (distance >= (ushort)OperationRange.Maximum) { return false; }

            var updatedOn = DateTime.UtcNow;

            // DISTANCE
            var lengthUnits = Length.FromMillimeters(distance);
            this.DistanceState.Update(lengthUnits, updatedOn);

            // see if anyone is listening
            this.UpdatedOn = updatedOn;
            this.StateChanged?.Invoke(this, this.States);

            return true;
        }
    }
}
