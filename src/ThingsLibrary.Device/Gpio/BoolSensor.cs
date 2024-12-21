using ThingsLibrary.Device.Sensor.Interfaces;

namespace ThingsLibrary.Device.Gpio
{
    /// <summary>
    /// Basic switch like sensor that is either on or off.
    /// </summary>
    public sealed class BoolSensor : Base.GpioInputDevice
    {                
        /// <summary>
        /// Specifies that a 'low' state is considered fault.
        /// </summary>
        public bool IsLowFault { get; set; }

        /// <summary>
        /// Boolean sensor..  open/close, on/off
        /// </summary>
        /// <param name="controller"><see cref="GpioController"/></param>
        /// <param name="pinId">Board Pin ID</param>
        /// <param name="isPullUp">If a pull up resistor should be used. False = pull down resistor, null = floating</param>
        public BoolSensor(GpioController controller, ushort pinId, string name, bool? isPullUp) : base(controller, pinId, name, isPullUp)
        {
            // bool sensor specific
            this.IsLowFault = !this.IsPullUp;            
        }

        /// <summary>
        /// Is the state normal (aka: not fault)
        /// </summary>
        public bool IsNormal => !this.IsFaulted;

        /// <summary>
        /// Is the state faulted based on if low state is 'fault'
        /// </summary>
        /// <remarks>Example: With a pullup resistor, the default/normal value is HIGH, goes LOW when sensor is 'normal' in NC state</remarks>
        public bool IsFaulted => (this.State == (this.IsLowFault ? PinValue.Low : PinValue.High));
                
        /// <summary>
        /// Label for the current state
        /// </summary>
        public string StateStr => (this.IsNormal ? this.BoolState.OffLabel : this.BoolState.OnLabel);


        /// <summary>
        /// Last State Fetch Timestamp
        /// </summary>
        //public DateTimeOffset UpdatedOn { get; internal set; }       

    }
}
