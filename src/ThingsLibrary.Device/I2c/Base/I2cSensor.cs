using ThingsLibrary.DataType.Events;

using SMath = System.Math;

namespace ThingsLibrary.Device.I2c.Base
{
    public abstract class I2cSensor : I2cBase, ISensor
    {
        #region --- Events ---

        /// <summary>
        /// Event when the state changes
        /// </summary>
        public ISensor.StatesChangedEventHandler StatesChanged { get; set; }

        #endregion
        
        /// <summary>
        /// Collection of states for the sensor
        /// </summary>
        public List<ISensorState> States { get; init; }

        /// <summary>
        /// Minimum Device Read Interval
        /// </summary>
        public int MinReadInterval { get; set; }
        
        /// <summary>
        /// Read Interval (in miliseconds)
        /// </summary>
        public int ReadInterval { get; set; }
                       
        /// <summary>
        /// Next time the sensor should be read
        /// </summary>
        public DateTimeOffset NextReadOn { get; set; } = DateTimeOffset.UtcNow; // until told otherwise

        /// <summary>
        /// Keep track of the last time a state actually changed
        /// </summary>
        public DateTimeOffset LastStateChanged { get; set; } = DateTimeOffset.UtcNow;

        /// <summary>
        /// Last time the sensor fetched / updated
        /// </summary>        
        public DateTimeOffset UpdatedOn 
        {
            get => _updatedOn; 
            set
            {
                _updatedOn = value;
                this.ScheduleNextRead();
            }
        }
        private DateTimeOffset _updatedOn;

        
        /// <summary>
        /// If the output should be in imperical units
        /// </summary>
        public bool IsImperial { get; internal set; }
                
        /// <summary>
        /// Any Error that has occured initalizing or reading from the sensor
        /// </summary>
        public string ErrorMessage { get; internal set; }
        
        public override string ToString() => String.Join(", ", this.States.Select(x => x.ToString()));
       
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="i2cBus">I2C Bus</param>
        /// <param name="id">ID</param>
        /// <param name="name">Name</param>
        /// <param name="isImperial">Use imperical units</param>
        protected I2cSensor(I2cBus i2cBus, int id, string name, bool isImperial) : base(i2cBus, id, name)
        {
            this.IsImperial = isImperial;
        }

        /// <inheritdoc />
        public override void Init()
        {
            //nothing other then base 

            base.Init();            
        }

        /// <inheritdoc />
        public abstract bool FetchStates();

        /// <summary>
        /// Figure out when the next read event should happen
        /// </summary>
        private void ScheduleNextRead()
        {
            this.NextReadOn = DateTimeOffset.UtcNow.AddMilliseconds(this.ReadInterval);
        }


        private long ScaleValue(double value, byte precision)
        {
            // Scale the value by 10 raised to the power of precision
            var scaledValue = value * SMath.Pow(10, precision);
                        
            // Round the result to the nearest whole number and cast it to long
            return (long)SMath.Round(scaledValue);
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

            return telemetryEvent.ToString();
        }

        /// <summary>
        /// Convert to a Telemetry Event
        /// </summary>
        /// <returns></returns>
        public TelemetryEvent ToTelemetryEvent(string typeKey = null)
        {
            var telemetryEvent = new TelemetryEvent(typeKey ?? this.Name, this.UpdatedOn) 
            { 
                Attributes = new Dictionary<string, string>(this.States.Count) 
            };
            
            foreach (var state in this.States)
            {
                if (state.IsDisabled) { continue; }

                telemetryEvent.Attributes[state.Key] = $"{this.ScaleValue(state.CurrentState, state.ValuePrecision)}";
            }

            return telemetryEvent;
        }
    }
}
