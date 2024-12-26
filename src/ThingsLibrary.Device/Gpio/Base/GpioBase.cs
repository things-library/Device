namespace ThingsLibrary.Device.Gpio.Base
{
    /// <summary>
    /// GPIO base class
    /// </summary>
    public abstract class GpioBase : IGpioDevice
    {    
        /// <summary>
        /// Keep track of the bool state
        /// </summary>
        public BoolState BoolState { get; init; }
       
        #region --- Interface Properties ---

        /// <inheritdoc />        
        public GpioController Controller { get; init; }

        /// <inheritdoc />        
        public int Id { get; init; }

        /// <inheritdoc />        
        public string Name { get; init; }

        /// <inheritdoc />        
        public PinValue State
        {
            get => _state;
            set
            {
                // nothing is changing?
                if (_state == value) { return; }

                _state = value;
                this.StateChangedOn = DateTimeOffset.UtcNow;
            }
        }
        private PinValue _state;

        /// <inheritdoc />        
        public DateTimeOffset StateChangedOn { get; set; }

        /// <inheritdoc />        
        public bool IsEnabled { get; set; }

        /// <inheritdoc />        
        public bool IsHigh => (this.State == PinValue.High);

        /// <inheritdoc />
        public bool IsLow => (this.State == PinValue.Low);

        #endregion

        /// <summary>
        /// What state is considered 'normal' or default
        /// </summary>
        public bool IsNormallyLow { get; init; } = false;
        
        /// <summary>
        /// Last error message
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pinId">Board Pin Number</param>
        protected GpioBase(GpioController gpioController, int pinId, string name, bool isNormallyLow)
        {
            this.Controller = gpioController ?? throw new ArgumentNullException(nameof(gpioController));
            
            this.Id = pinId;
            this.Name = name;
            this.IsNormallyLow = isNormallyLow;
        }

        /// <summary>
        /// Set up the device and enable it if requested
        /// </summary>
        public abstract void Init(bool enableDevice = true);
    }
}