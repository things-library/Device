using ThingsLibrary.DataType.Events;

namespace ThingsLibrary.Device.Gpio.Base
{
    /// <summary>
    /// GPIO base class
    /// </summary>
    public abstract class GpioBase : IGpioDevice
    {       
        private PinValue _state;
        private readonly object _lockObj = new object();

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
                lock (_lockObj)
                {
                    _state = value;
                }

                // throw events if we are enabled
                if (!this.IsEnabled) { return; }

                var updatedOn = DateTimeOffset.UtcNow;

                // we are using value because other things could be changing the state variable on different threads
                var isOn = (this.IsNormallyLow ? value == PinValue.High : value == PinValue.Low);

                this.BoolState.Update(isOn, updatedOn);
            }
        }

        /// <inheritdoc />        
        public DateTimeOffset StateChangedOn => this.BoolState.StateChangedOn;

        /// <inheritdoc />        
        public TimeSpan StateDuration() => this.BoolState.StateDuration();  //technically calculated but basically a property

        /// <inheritdoc />
        public TimeSpan LastStateDuration => this.BoolState.LastStateDuration;

        /// <inheritdoc />        
        public DateTimeOffset? UpdatedOn => this.BoolState.UpdatedOn;

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
        public bool IsNormallyLow { get; private set; } = false;
        

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
        
        /// <summary>
        /// Attempt to fetch the device state
        /// </summary>
        /// <returns>True if successful</returns>
        public virtual void FetchState()
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
        /// Convert to a telemetry sentence
        /// </summary>
        /// <param name="telemetryItem">Telemetry Item</param>
        /// <returns></returns>
        public string ToTelemetryString(string typeKey = null)
        {
            //EXAMPLES:
            //  $1724387849602|sens|r:1|s:143|p:PPE Mask|q:1|p:000*79
            //  $1724387850520|sens|r:1|q:2*33

            var telemetryEvent = this.ToTelemetryEvent(typeKey);
            
            return telemetryEvent?.ToString();
        }

        /// <summary>
        /// Convert to a Telemetry Event
        /// </summary>
        /// <returns></returns>
        public TelemetryEvent ToTelemetryEvent(string typeKey = null)
        {
            if (this.BoolState.UpdatedOn == null) { return null; }

            var telemetryEvent = new TelemetryEvent(typeKey ?? this.Name, this.BoolState.UpdatedOn.Value)
            {
                Attributes = new Dictionary<string, string>(1)
            };

            //telemetryEvent.Attributes[state.Key] = $"{this.}";

            return telemetryEvent;
        }
    }
}