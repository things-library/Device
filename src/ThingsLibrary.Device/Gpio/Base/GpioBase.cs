namespace ThingsLibrary.Device.Gpio.Base
{
    /// <summary>
    /// GPIO base class
    /// </summary>
    public abstract class GpioBase : IGpioDevice
    {       
        private PinValue _state;
        private readonly object _lockObj = new object();

        #region --- Events ---

        public delegate void StateChangedEventHandler(object sender, PinValue pinValue);

        /// <summary>
        /// Event when the state changes
        /// </summary>
        public StateChangedEventHandler StateChanged { get; set; }

        #endregion

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
                if (this.IsChanged) { this.IsChanged = false; }
                if (_state == value) { return; } //nothing to do, state didn't change

                lock (_lockObj)
                {
                    _state = value;
                    this.StateChangedOn = DateTime.UtcNow;

                    // only change the flag if we are in enabled state (not init)
                    if (this.IsEnabled)
                    {
                        this.IsChanged = true;
                    }
                }

                // throw events if we are enabled
                if (this.IsEnabled)
                {
                    // see if anyone is listening
                    this.StateChanged?.Invoke(this, this.State);
                }
            }
        }

        /// <inheritdoc />        
        public DateTime StateChangedOn { get; private set; } = DateTime.UtcNow;

        /// <inheritdoc />        
        public bool IsChanged { get; private set; } = false;

        /// <inheritdoc />        
        public TimeSpan StateDuration => DateTime.UtcNow.Subtract(this.StateChangedOn);

        /// <inheritdoc />        
        public bool IsEnabled { get; set; }

        /// <inheritdoc />        
        public bool IsHigh => (this.State == PinValue.High);

        /// <inheritdoc />
        public bool IsLow => (this.State == PinValue.Low);

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pinId">Board Pin Number</param>
        protected GpioBase(GpioController gpioController, ushort pinId, string name)
        {
            this.Controller = gpioController ?? throw new ArgumentNullException(nameof(gpioController));
            
            this.Id = pinId;
            this.Name = name;
        }

        /// <summary>
        /// Set up the device and enable it if requested
        /// </summary>
        public abstract void Init(bool enableDevice = true);

        /// <summary>
        /// Attempt to fetch the device state
        /// </summary>
        /// <returns>True if successful</returns>
        public void FetchState()
        {            
            // get the state off the pin
            this.State = this.Controller.Read(this.Id);            
        }

        /// <summary>
        /// Set the device to high state
        /// </summary>
        public virtual void High()
        {
            if (!this.IsEnabled) { return; }
            if (this.State == PinValue.High) { return; } //already set

            this.Controller.Write(this.Id, PinValue.High);

            this.FetchState();
        }

        /// <summary>
        /// Set the device to the low state
        /// </summary>
        public virtual void Low()
        {
            if (!this.IsEnabled) { return; }
            if (this.State == PinValue.Low) { return; } //already set

            this.Controller.Write(this.Id, PinValue.Low);

            this.FetchState();
        }

        /// <summary>
        /// State Duration String (hh:mm:ss) or (dd:hh:mm:ss)
        /// </summary>
        /// <example>01:02:05</example>
        public string StateDurationStr
        {
            get
            {
                var timeSpan = DateTime.UtcNow.Subtract(this.StateChangedOn);

                if (timeSpan.Days > 0)
                {
                    return $"{timeSpan.Days:00}:{timeSpan.Hours:00}:{timeSpan.Minutes}:{timeSpan.Seconds:00}";
                }
                else
                {
                    return $"{timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
                }
            }
        }
    }
}