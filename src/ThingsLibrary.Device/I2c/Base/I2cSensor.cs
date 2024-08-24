namespace ThingsLibrary.Device.I2c.Base
{
    public abstract class I2cSensor : I2cBase
    {
        #region --- Events ---

        public delegate void StateChangedEventHandler(object sender, Dictionary<string, ISensorState> states);

        /// <summary>
        /// Event when the state changes
        /// </summary>
        public StateChangedEventHandler StateChanged { get; set; }

        #endregion
        
        /// <summary>
        /// Collection of states for the sensor
        /// </summary>
        public Dictionary<string, ISensorState> States { get; init; }

        /// <summary>
        /// Minimum Device Read Interval
        /// </summary>
        public int MinReadInterval { get; set; }
        
        /// <summary>
        /// Read Interval (in miliseconds)
        /// </summary>
        public int ReadInterval { get; set; }

        /// <summary>
        /// Read Schedule (CRON)
        /// </summary>
        /// <example>*/15 * * * * = every 15th minute</example>
        /// <remarks><see href="https://crontab.guru/" /></remarks>
        public string ReadSchedule { get; set; }

        /// <summary>
        /// Library for CRON scheduling
        /// </summary>
        public CrontabSchedule ReadScheduler { get; set; }

        /// <summary>
        /// Next time the sensor should be read
        /// </summary>
        public DateTime NextReadOn { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Last time the sensor was read.
        /// </summary>        
        public DateTime UpdatedOn 
        {
            get => _updatedOn; 
            set
            {
                _updatedOn = value;
                this.ScheduleNextRead();
            }
        }
        private DateTime _updatedOn;
        
        /// <summary>
        /// If the output should be in metric units
        /// </summary>
        public bool IsMetric { get; set; }

        public string ErrorMessage { get; set; }

        public override string ToString() => String.Join(", ", this.States.Values.Select(x => x.ToString()));
       
        protected I2cSensor(I2cBus i2cBus, int id, string name, bool isMetric) : base(i2cBus, id, name)
        {
            this.IsMetric = isMetric;
        }

        public override void Init()
        {
            base.Init();
            
            // if there is a cron schedule parse it and have it ready to use.
            if (!string.IsNullOrEmpty(this.ReadSchedule))
            {
                this.ReadScheduler = CrontabSchedule.Parse(this.ReadSchedule);
            }
        }

        public abstract bool FetchState();

        /// <summary>
        /// Figure out when the next read event should happen
        /// </summary>
        private void ScheduleNextRead()
        {
            if (this.ReadScheduler != null)
            {
                this.NextReadOn = this.ReadScheduler.GetNextOccurrence(DateTime.UtcNow);
            }
            else
            {                
                this.NextReadOn = DateTime.UtcNow.AddMilliseconds(this.ReadInterval);
            }
        }
    }
}
