namespace ThingsLibrary.Device.Gpio.Base
{
    /// <summary>
    /// GPIO Output Device
    /// </summary>
    public class GpioOutputDevice : GpioBase
    {
        public bool IsNormallyLow { get; private set; } = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="controller"><see cref="GpioController"/></param>
        /// <param name="pinId">Pin Number</param>
        /// <param name="name">Name of the device</param>
        /// <param name="isNormallyLow">If the device is normally low state</param>
        public GpioOutputDevice(GpioController controller, ushort pinId, string name, bool isNormallyLow) : base(controller, pinId, name)
        {           
            this.IsNormallyLow = isNormallyLow;
        }

        /// <summary>
        /// Initialize the device
        /// </summary>
        /// <param name="enableDevice"></param>
        public override void Init(bool enableDevice = true)
        {   
            //set pin mode
            this.Controller.OpenPin(this.Id, PinMode.Output, (this.IsNormallyLow ? PinValue.Low : PinValue.High));
            
            this.IsEnabled = enableDevice;
        }

        /// <summary>
        /// Turn on the output
        /// </summary>
        public void On()
        {
            if (this.IsNormallyLow) { this.High(); }
            else { this.Low(); }
        }

        /// <summary>
        /// Turn off the output
        /// </summary>
        public void Off()
        {
            if (this.IsNormallyLow) { this.Low(); }
            else { this.High(); }
        }

        /// <summary>
        /// Is Output On
        /// </summary>
        public bool IsOn => (this.IsNormallyLow ? this.State == PinValue.High : this.State == PinValue.Low);
        
        /// <summary>
        /// Is Output Off
        /// </summary>
        public bool IsOff => (this.IsNormallyLow ? this.State == PinValue.Low : this.State == PinValue.High);
    }
}
