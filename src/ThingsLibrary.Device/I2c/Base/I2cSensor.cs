using System.Text;
using ThingsLibrary.DataType.Events;
using ThingsLibrary.Schema.Library;
using ThingsLibrary.Schema.Library.Extensions;

using SMath = System.Math;

namespace ThingsLibrary.Device.I2c.Base
{
    public abstract class I2cSensor : I2cBase
    {
        #region --- Events ---

        public delegate void StateChangedEventHandler(object sender, List<ISensorState> states);

        /// <summary>
        /// Event when the state changes
        /// </summary>
        public StateChangedEventHandler StateChanged { get; set; }

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
        public DateTime NextReadOn { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Last time the sensor was read.
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
        /// If the output should be in metric units
        /// </summary>
        public bool IsImperial { get; internal set; }
                
        /// <summary>
        /// Any Error that has occured initalizing or reading from the sensor
        /// </summary>
        public string ErrorMessage { get; internal set; }

        public override string ToString() => String.Join(", ", this.States.Select(x => x.ToString()));
       
        protected I2cSensor(I2cBus i2cBus, int id, string name, bool isImperial) : base(i2cBus, id, name)
        {
            this.IsImperial = isImperial;
        }

        public override void Init()
        {
            base.Init();            
        }

        public abstract bool FetchState();

        /// <summary>
        /// Figure out when the next read event should happen
        /// </summary>
        private void ScheduleNextRead()
        {
            this.NextReadOn = DateTime.UtcNow.AddMilliseconds(this.ReadInterval);
        }


        private long ScaleValue(double value, byte precision)
        {
            // Scale the value by 10 raised to the power of precision
            double scaledValue = value * SMath.Pow(10, precision);
                        
            // Round the result to the nearest whole number and cast it to long
            return (long)SMath.Round(scaledValue);
        }

        ///// <summary>
        ///// Convert to a telemetry sentence
        ///// </summary>
        ///// <param name="telemetryItem">Telemetry Item</param>
        ///// <returns></returns>
        //public string ToTelemetryString(string typeKey = null)
        //{
        //    //EXAMPLES:
        //    //  $1724387849602|sens|r:1|s:143|p:PPE Mask|q:1|p:000*79
        //    //  $1724387850520|sens|r:1|q:2*33

        //    var sentence = new StringBuilder();

        //    // time + sentence ID 
        //    sentence.Append($"${this.UpdatedOn.ToUnixTimeMilliseconds()}|{typeKey ?? this.Name}");

        //    // add the states
        //    foreach (var state in this.States)
        //    {
        //        // nothing to do?
        //        if (state.IsDisabled) { continue; }
        //        if (state.UpdatedOn != this.UpdatedOn) { continue; }

        //        sentence.Append($"|{state.Key}:{this.ScaleValue(state.Value, state.ValuePrecision)}");
        //    }

        //    //Add checksum
        //    sentence.AppendChecksum();

        //    return sentence.ToString();
        //}

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

                telemetryEvent.Attributes[state.Key] = $"{this.ScaleValue(state.Value, state.ValuePrecision)}";
            }

            return telemetryEvent;
        }
    }
}
