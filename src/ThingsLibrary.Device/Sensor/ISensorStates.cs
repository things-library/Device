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
        public DateTimeOffset UpdatedOn { get; }
                
        /// <summary>
        /// Collection of States
        /// </summary>
        public List<ISensorState> States { get; }
    }
}
