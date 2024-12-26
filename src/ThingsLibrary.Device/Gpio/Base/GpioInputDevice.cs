using ThingsLibrary.Device.Sensor.Interfaces;

namespace ThingsLibrary.Device.Gpio.Base
{
    public class GpioInputDevice : GpioBase
    {
        /// <summary>
        /// If a pullup resistor should be used
        /// </summary>
        public bool IsPullUp { get; set; }

        /// <summary>
        /// If a pulldown resistor should be used
        /// </summary>
        public bool IsPullDown { get; set; }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="controller"><see cref="GpioController"/></param>
        /// <param name="pinId">Pin Number</param>
        /// <param name="name">Name of the device</param>
        /// <param name="isPullUp">If a pullup resistor should be used, false if a pull down resistor should be used, null if neither</param>
        public GpioInputDevice(GpioController controller, ushort pinId, string name, bool? isPullUp) : base(controller, pinId, name, (!isPullUp ?? true))
        {
            // do we have a value
            if (isPullUp != null)
            {
                this.IsPullUp = isPullUp.Value;
                this.IsPullDown = !isPullUp.Value;
            }            
        }

        /// <summary>
        /// Inititalize the input device
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public override void Init(bool enableDevice = true)
        {
            var pinMode = PinMode.Input;
            
            if (this.IsPullUp) { pinMode = PinMode.InputPullUp; }            
            else if (this.IsPullDown) { pinMode = PinMode.InputPullDown; }

            if (!this.Controller.IsPinModeSupported(this.Id, pinMode))
            {
                throw new ArgumentException($"Pin Mode '{pinMode}' not supported on pin # '{this.Id}'.");
            }

            //init the pin            
            this.Controller.OpenPin(this.Id, pinMode);
            
            this.IsEnabled = enableDevice;
        }

        public void Init() => this.Init(true);        
    }
}
