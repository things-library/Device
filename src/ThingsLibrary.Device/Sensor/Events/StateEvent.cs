namespace ThingsLibrary.Device.Sensor.Events
{
    /// <summary>
    /// Bool State Event
    /// </summary>
    public class StateEvent : EventArgs
    {
        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; init; }

        /// <summary>
        /// Key
        /// </summary>
        public string Key { get; init; }

        /// <summary>
        /// Number of digits after the decimal, also how much the data is scaled for telemetry (0 = no scaling)
        /// </summary>
        /// <example>1 = for temp so 78.1 becomes 781 in telemetry data</example>
        public byte ValuePrecision { get; init; }

        /// <summary>
        /// Current State
        /// </summary>
        public double State { get; init; }

        /// <summary>
        /// Event Date / State Changed On
        /// </summary>
        public DateTimeOffset StateChangedOn { get; init; }


        // ================================================================================
        // HISTORY
        // ================================================================================

        /// <summary>
        /// Last State
        /// </summary>
        public double? LastState { get; init; }

        /// <summary>
        /// Last State Duration
        /// </summary>
        public TimeSpan LastStateDuration { get; init; }
    }
}
