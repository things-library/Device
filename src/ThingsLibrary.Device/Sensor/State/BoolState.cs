using ThingsLibrary.Device.Sensor.Events;

namespace ThingsLibrary.Device.Sensor.State
{        
    /// <summary>
    /// Basic boolean state for keeping track of two states.
    /// </summary>
    public class BoolState : SensorState
    {
        /// <summary>
        /// Current State
        /// </summary>
        public bool IsOn => (this.CurrentState != 0);

        /// <summary>
        /// Label for the normal / off state
        /// </summary>
        public string OffLabel { get; private set; } = "Off";

        /// <summary>
        /// Label for the faulted / on state
        /// </summary>
        public string OnLabel { get; private set; } = "On";
                
        /// <summary>
        /// Last State pretty label
        /// </summary>
        public string LastStateLabel => (this.IsOn ? this.OffLabel : this.OnLabel);

        /// <summary>
        /// State Label to show the user
        /// </summary>
        public string StateLabel => (this.IsOn ? this.OnLabel : this.OffLabel);
                

        /// <inheritdoc />
        public void Update(bool isOn, DateTimeOffset updatedOn)
        {
            this.UpdatedOn = updatedOn;

            // is the state changing?
            if (isOn != this.IsOn)
            {
                //capture the duration of the previous state since we know we are changing
                this.LastState = (this.IsOn ? 1 : 0); // we haven't changed it yet
                this.LastStateDuration = this.StateDuration(updatedOn);

                this.CurrentState = (this.IsOn ? 1 : 0);
                this.StateChangedOn = updatedOn;

                // did the state change during this process?
                if (this.StateChangedOn == UpdatedOn)
                {
                    // see if anyone is listening (use provided value to be thread safe)
                    this.StateChanged?.Invoke(this, this.ToEvent());
                }
            }
        }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="key">Key</param>
        /// <param name="offLabel">Display label when Off</param>
        /// <param name="onLabel">Display label when on</param>
        public BoolState(string id = "Switch", string key = "sw", string offLabel = "Off", string onLabel = "On") : base(id, key, false)
        {
            this.OffLabel = offLabel;
            this.OnLabel = onLabel;

            // make sure 
            var updatedOn = DateTimeOffset.UtcNow;

            this.StateChangedOn = updatedOn;
            this.UpdatedOn = updatedOn;
        }
    }
}
