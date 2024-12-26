using System.Threading;

using Iot.Device.Vl53L0X;

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
            // make sure that every sensor has a shutdown pin
            if(this.Sensors.Any(x => x.ShutdownPinId < 0)) { throw new ArgumentException("All sensors must have a defined shutdown pin in order to be in a group of sensors"); }

            // set up all the shutdown pins before using them            
            foreach (var sensor in this.Sensors)
            {
                this.Controller.OpenPin(sensor.ShutdownPinId, PinMode.Output, PinValue.High);
            }

            // update device addresses so they are on their own unique address  (0x00 to 0x7F)  Default: 0x29 --> 41
            byte newAddress = 42; 

            // set up all the shutdown pins before using them
            foreach (var sensor in this.Sensors)
            {
                sensor.NewDeviceId = newAddress;
                sensor.Init();
                
                newAddress++;                
            }

            // turn them all back on
            this.Restore();
        }

        /// <summary>
        /// Change the sensor address by shutting down all the others.
        /// </summary>
        /// <param name="sensor"></param>
        /// <param name="newAddress"></param>
        /// <exception cref="ArgumentException"></exception>
        public void ChangeAddress(Vl53l0xSensor sensor, byte newAddress)
        {
            if(sensor.ShutdownPinId < 0) { throw new ArgumentException($"Invalid Pin ID {sensor.ShutdownPinId}"); }

            // make sure all other devices are shutdown
            this.Shutdown(sensor.ShutdownPinId);

            // make sure that the device we are interested in is on (it should be shut down from a different shutdown process)
            this.Restore(sensor.ShutdownPinId);
            
            Thread.Sleep(100);  //? not sure how long to wait if sensor was off
            
            //set address            
            Vl53L0X.ChangeI2cAddress(sensor.I2cDevice, newAddress);
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
            foreach(var sensor in this.Sensors)
            {
                this.Restore(sensor.ShutdownPinId);
            }            
        }

        /// <summary>
        /// Turn on specific sensor
        /// </summary>
        public void Restore(int pinId)
        {
            if (pinId < 0) { return; }

            //pull all the shutdown pins low
            this.Controller.Write(pinId, PinValue.High);
        }
    }
}
