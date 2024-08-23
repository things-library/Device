using Iot.Device;
using ThingsLibrary.Device.Sensor.State;
using ThingsLibrary.Device.Sensor;

namespace ThingsLibrary.Device.I2c
{
    //https://learn.adafruit.com/pmsa003i

    public class Pmsx003Sensor : Base.I2cSensor
    {
        private Pmsx003 _device { get; set; }

        //States
        public AqiState AqiState { get; init; }

        public Pmsx003Sensor(I2cBus i2cBus, int id = Pmsx003.DefaultI2cAddress, string name = "Pmsx003", bool isImperial = false) : base(i2cBus, id, name, isImperial)
        {
            // States

            this.States = new Dictionary<string, ISensorState>()
            {
                { this.AqiState.Id, this.AqiState },
            };
        }

        public override void Init()
        {
            base.Init();

            _device = new Pmsx003(this.I2cDevice);

            //this.MinReadInterval = (int)Scd4x.MeasurementPeriod.TotalMilliseconds;
            //if (this.ReadInterval < this.MinReadInterval) { throw new ArgumentException($"Read interval '{this.ReadInterval} ms' can not be less then min read interval '{this.MinReadInterval} ms' of sensor."); }

            this.IsEnabled = true;
        }

        public async override Task<bool> FetchState()
        {
            if (!this.IsEnabled) { return false; }
            if (DateTime.UtcNow < this.NextReadOn) { return false; }

            var readResult = _device.Read();
            if(readResult == null) { return false; }
            
            this.UpdatedOn = DateTime.UtcNow; 

            // see if anyone is listening
            this.StateChanged?.Invoke(this, this.States);

            // if we get here the state has changed
            return true;
        }
    }    
}
