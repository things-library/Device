namespace ThingsLibrary.Device.Gpio
{
    /// <summary>
    /// Basic switch like sensor that is either on or off.
    /// </summary>
    public sealed class BoolSensor : Base.GpioInputDevice
    {
        /// <summary>
        /// Label to use for a normal state
        /// </summary>
        /// <example>Closed</example>
        public string NormalLabel { get; set; } = "Off";
        
        /// <summary>
        /// Label to use for a fault state
        /// </summary>
        /// <example>Open</example>
        public string FaultLabel { get; set; } = "On";

        /// <summary>
        /// Specifies that a 'low' state is considered fault.
        /// </summary>
        public bool IsLowFault { get; set; }

        /// <summary>
        /// Boolean sensor..  open/close, on/off
        /// </summary>
        /// <param name="controller"><see cref="GpioController"/></param>
        /// <param name="pinId">Board Pin ID</param>
        /// <param name="isLowFault">Define what state is considered a 'fault' state based on how sensor is being used.</param>
        /// <param name="isPullUp">If a pull up resistor should be used.</param>
        /// <param name="isPullDown">If a pulldown resistor should be used.</param>
        public BoolSensor(GpioController controller, ushort pinId, string name, bool isPullUp, bool isPullDown, bool isLowFault) : base(controller, pinId, name, isPullUp, isPullDown)
        {
            // bool sensor specific
            this.IsLowFault = isLowFault;
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
        /// Label for normal state
        /// </summary>
        public string NormalStateLabel { get; set; } = "Off";

        /// <summary>
        /// Label for faulted state (not normal state)
        /// </summary>
        public string FaultedStateLabel { get; set; } = "On";

        /// <summary>
        /// Label for the current state
        /// </summary>
        public string StateStr => (this.IsNormal ? this.NormalStateLabel : this.FaultedStateLabel);                
    }
}
