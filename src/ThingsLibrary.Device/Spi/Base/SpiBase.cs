using System.Device.Spi;

namespace ThingsLibrary.Device.Spi.Base
{
    /// <summary>
    /// GPIO base class
    /// </summary>
    public abstract class SpiBase : ISpiDevice
    {   
        #region --- Interface Properties ---

        /// <inheritdoc />        
        public SpiDevice Device { get; init; }

        /// <inheritdoc />        
        public string Name { get; init; }

        /// <inheritdoc />        
        public bool IsEnabled { get; set; }

        #endregion
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pinId">Board Pin Number</param>
        protected SpiBase(string name, int busId, int chipSelectId = -1)
        {            
            this.Name = name;

            var settings = new SpiConnectionSettings(busId, chipSelectId)
            {
                ClockFrequency = 2_400_000,
                Mode = SpiMode.Mode0,
                DataBitLength = 8
            };

            this.Device = SpiDevice.Create(settings);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="settings">SPI Settings</param>
        protected SpiBase(string name, SpiConnectionSettings settings)
        {
            this.Name = name;
            
            this.Device = SpiDevice.Create(settings);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="spiDevice">SPI Device</param>
        protected SpiBase(string name, SpiDevice spiDevice)
        {
            this.Device = spiDevice ?? throw new ArgumentNullException(nameof(spiDevice));

            this.Name = name;
        }

        /// <summary>
        /// Set up the device and enable it if requested
        /// </summary>
        /// <param name="enableDevice">Enable device after initilization?</param>
        public abstract void Init(bool enableDevice = true);        
    }
}