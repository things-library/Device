namespace ThingsLibrary.Device.Sensor
{
    /// <summary>
    /// Sensor Overall State
    /// </summary>
    public interface ISensorStates
    {
        /// <summary>
        /// Last State Fetch Timestamp
        /// </summary>
        public DateTime UpdatedOn { get; }
                
        /// <summary>
        /// Collection of States
        /// </summary>
        public Dictionary<string, ISensorState> States { get; }
    }
}
