namespace Iot.Device
{
    /// <summary>
    /// PMSx003 Series - PM2.5 AQI (Air quality sensor)
    /// </summary>
    /// <remarks> 
    /// Stable data should be got at least 30 seconds after the sensor wakeup from the sleep mode because of the fan’s performance.
    /// 
    /// Mainly output as the quality and number of each particles with different size per unit volume, the unit volume of 
    /// particle number is 0.1L and the unit of mass concentration is μg/m³. 
    /// 
    /// There are two options for digital output: passive and active.
    /// 
    /// Default mode is active after power up. In this mode sensor would send serial data to the host automatically. 
    /// The active mode is divided into two sub-modes: stable mode and fast mode. 
    /// If the concentration change is small the sensor would run at stable mode with the real interval of 2.3s. 
    /// And if the change is big the sensor would be changed to fast mode automatically with the interval of 200~800ms, 
    /// the higher of the concentration, the shorter of the interval.
    /// </remarks>
    public sealed class Pmsx003 : IDisposable
    {
        /// NOTE: This is a Plantower Sensor

        /// <summary>
        /// The default I²C address of this device.
        /// </summary>
        public const byte DefaultI2cAddress = 0x12; // PMSA003I has only one I2C address (18 or 0x12)

        public I2cDevice I2cDevice { get; set; }

        private bool IsDisposed { get; set; } = false;

        //private byte Version { get; set; }
        //private byte ErrorCode { get; set; }
        //private ushort Checksum { get; set; }         // Packet checksum

        /// <summary>
        /// Instantiates a new <see cref="Pmsx003"/>
        /// </summary>
        /// <param name="i2cDevice">The I²C device to operate on.</param>
        public Pmsx003(I2cDevice i2cDevice)
        {
            this.I2cDevice = i2cDevice ?? throw new ArgumentNullException(nameof(i2cDevice));            
        }

        /// <summary>
        /// Reads data from the device
        /// </summary>
        /// <returns><see cref="AirQuality"/> on success, null on CRC failure</returns>
        public AirQuality Read()
        {
            // Reference: https://cdn-shop.adafruit.com/product-files/4632/4505_PMSA003I_series_data_manual_English_V2.6.pdf
            //  Section 5, Register Definition

            this.I2cDevice.WriteByte((byte)0x00);

            // read all at once since we are dealing with a checksum
            Span<Byte> readBuffer = stackalloc byte[32];
            this.I2cDevice.Read(readBuffer);
            
            // calculate checksum
            if(!CrcCheck(readBuffer)) { return null; }

            // The data comes in Endian so largest bit then smallest (register mapping below instead of assumptions)
            //Debug.WriteLine($"Start Char: {ToUshort(readBuffer[0x00], readBuffer[0x01])}"); //Fixed value set as 0x42 and 0x4d
            //Debug.WriteLine($"Frame Length: {ToUshort(readBuffer[0x02], readBuffer[0x03])}");
            //Debug.WriteLine($"Version: {readBuffer[0x1c]}");            
            //Debug.WriteLine($"Checksum: {ToUshort(readBuffer[0x1e], readBuffer[0x1f])}");
            

            var airQuality = new AirQuality
            {
                // Standard Particles   
                StandardPm10 = ToUshort(readBuffer[0x04], readBuffer[0x05]),  //PM1.0 concentration unit: µg /𝑚3
                StandardPm25 = ToUshort(readBuffer[0x06], readBuffer[0x07]),  //PM2.5 concentration unit：µg /𝑚3
                StandardPm100 = ToUshort(readBuffer[0x08], readBuffer[0x09]), //PM10 concentration unit：µg /𝑚3 

                // Atmospheric Environment
                EnvironmentPm10 = ToUshort(readBuffer[0x0a], readBuffer[0x0b]),  //PM1.0 concentration unit：µg /𝑚3 (under atmospheric environment)
                EnvironmentPm25 = ToUshort(readBuffer[0x0c], readBuffer[0x0d]),  //PM2.5 concentration unit：µg /𝑚3 (under atmospheric environment)
                EnvironmentPm100 = ToUshort(readBuffer[0x0e], readBuffer[0x0f]), //PM10.0 concentration unit：µg /𝑚3 (under atmospheric environment)

                // Particle Concentrations (Number of particles with diameter beyond xx.x µ𝑚 in 0.1L of air)
                Particles03 = ToUshort(readBuffer[0x10], readBuffer[0x11]),  //  > 0.3 µ𝑚
                Particles05 = ToUshort(readBuffer[0x12], readBuffer[0x13]),  //  > 0.5 µ𝑚
                Particles10 = ToUshort(readBuffer[0x14], readBuffer[0x15]),  //  > 1.0 µ𝑚
                Particles25 = ToUshort(readBuffer[0x16], readBuffer[0x17]),  //  > 2.5 µ𝑚
                Particles50 = ToUshort(readBuffer[0x18], readBuffer[0x19]),  //  > 5.0 µ𝑚
                Particles100 = ToUshort(readBuffer[0x1a], readBuffer[0x1b])  // > 10.0 µ𝑚
            };

            return airQuality;
        }

        private bool CrcCheck(Span<Byte> readBuffer)
        {
            // Check that start bytes are correct! (these are fixed values by the manufacture)
            if (readBuffer[0] != 0x42 || readBuffer[1] != 0x4d)
            {
                return false;
            }

            // calculate checksum
            short sum = 0;
            for (byte i = 0; i < 30; i++)   //don't include the checksum in the calculation
            {
                sum += readBuffer[i];
            }

            // no point on continuing if the checksum doesn't 'check' out :)
            var checksum = (ushort)(readBuffer[0x1e] << 8 | readBuffer[0x1f]);
            
            return (sum == checksum);
        }

        private static ushort ToUshort(byte hi, byte low)
        {
            // The data comes in Endian so largest bit then smallest (register mapping below instead of assumptions)

            return (ushort)(hi << 8 | low);
        }

        /// <summary>
        /// Cleanup resources.
        /// </summary>        
        public void Dispose()
        {            
            if (this.IsDisposed) { return; }

            this.I2cDevice.Dispose();

            this.IsDisposed = true;
        }
    }
}
