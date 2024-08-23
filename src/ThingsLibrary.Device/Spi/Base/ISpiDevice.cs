using System.Device.Spi;

namespace ThingsLibrary.Device.Spi.Base
{
    /// <summary>
    /// SPI Device Interface
    /// </summary>
    public interface ISpiDevice
    {
        /// <summary>
        /// SPI Device
        /// </summary>
        public SpiDevice Device { get; init; }

        /// <summary>
        /// Label for the device
        /// </summary>
        public string Name { get; }
                
        /// <summary>
        /// If the device is currently enabled
        /// </summary>
        public bool IsEnabled { get; }        
    }
}
