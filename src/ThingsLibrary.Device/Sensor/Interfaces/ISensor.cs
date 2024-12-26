namespace ThingsLibrary.Device.Sensor.Interfaces
{
    /// <summary>
    /// Sensor Interface
    /// </summary>
    public interface ISensor
    {
        // ================================================================================
        // Events
        // ================================================================================
        public delegate void StatesChangedEventHandler(object sender, List<ISensorState> states);

        /// <summary>
        /// Event when the sensor states change
        /// </summary>
        public ISensor.StatesChangedEventHandler StatesChanged { get; set; }

        // ================================================================================
        // Properties
        // ================================================================================

        /// <summary>
        /// Collection of States
        /// </summary>
        public List<ISensorState> States { get; }

        /// <summary>
        /// Minimum Device Read Interval
        /// </summary>
        public int MinReadInterval { get; }

        /// <summary>
        /// Read Interval (in miliseconds)
        /// </summary>
        public int ReadInterval { get; }

        /// <summary>
        /// Last time a state actually changed
        /// </summary>
        public DateTimeOffset LastStateChanged { get; }

        /// <summary>
        /// Last State Fetch / Update Timestamp
        /// </summary>
        public DateTimeOffset UpdatedOn { get; }

        /// <summary>
        /// Last error message
        /// </summary>
        public string ErrorMessage { get; }


        // ================================================================================
        // Methods 
        // ================================================================================

        /// <summary>
        /// Initiailize the sensor
        /// </summary>
        public abstract void Init();
        
        /// <summary>
        /// Attempt to fetch all sensor states
        /// </summary>
        /// <returns></returns>
        public abstract bool FetchStates();
        
    }
}
