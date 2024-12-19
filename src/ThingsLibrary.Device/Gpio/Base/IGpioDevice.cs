namespace ThingsLibrary.Device.Gpio.Base
{
    /// <summary>
    /// GPIO Device Interface
    /// </summary>
    public interface IGpioDevice
    {
        /// <summary>
        /// GPIO Controller
        /// </summary>
        public GpioController Controller { get; init; }

        /// <summary>
        /// Pin # on the board
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Label for the device
        /// </summary>
        public string Name { get; }
                
        /// <summary>
        /// Set the state
        /// </summary>
        public PinValue State { get; }
       
        /// <summary>
        /// When the state last changed
        /// </summary>
        public DateTimeOffset StateChangedOn { get; }

        /// <summary>
        /// How long was the last state before this one
        /// </summary>
        public TimeSpan LastStateDuration { get; }

        /// <summary>
        /// Keep track anytime we have pulled the state even if it hasn't changed
        /// </summary>
        public DateTimeOffset? UpdatedOn { get; }
                
        /// <summary>
        /// If the device is currently enabled
        /// </summary>
        public bool IsEnabled { get; }

        /// <summary>
        /// Is State High
        /// </summary>
        public bool IsHigh { get; }

        /// <summary>
        /// Is State Low
        /// </summary>
        public bool IsLow { get; }


        /// <summary>
        /// How long has it been the current state
        /// </summary>
        public TimeSpan StateDuration();
    }
}
