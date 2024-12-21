namespace ThingsLibrary.Device.I2c
{
    /// <summary>
    /// Laser distance time of flight ToF sensor
    /// </summary>
    public class Vl53l0xSensorGroup
    {
        public GpioController Controller { get; init; }

        public List<Vl53l0xSensor> Sensors { get; set; } = new();

        public Vl53l0xSensorGroup(GpioController gpioController)
        {            
            this.Controller = gpioController;
        }

        /// <summary>
        /// Setup the various pins
        /// </summary>
        public void Init()
        {
            foreach (var sensor in this.Sensors)
            {
                if (sensor.ShutdownPinId < 0) { continue; }

                this.Controller.OpenPin(sensor.ShutdownPinId, PinMode.Output, PinValue.High);
            }
        }

        /// <summary>
        /// Pull all shutdown pins low to put them into shutdown mode
        /// </summary>
        /// <param name="exceptPinId"></param>
        public void Shutdown(int exceptPinId = 0)
        {
            foreach(var sensor in this.Sensors)
            {
                if (sensor.ShutdownPinId < 0) { continue; }
                if (sensor.ShutdownPinId == exceptPinId) { continue; }  // skip this one?

                //pull all the shutdown pins low
                this.Controller.Write(sensor.ShutdownPinId, PinValue.Low);
            }
        }

        /// <summary>
        /// Turn all the sensors on
        /// </summary>
        public void Restore()
        {
            foreach (var sensor in this.Sensors)
            {
                if (sensor.ShutdownPinId < 0) { continue; }
                
                //pull all the shutdown pins low
                this.Controller.Write(sensor.ShutdownPinId, PinValue.High);
            }
        }
    }
}
