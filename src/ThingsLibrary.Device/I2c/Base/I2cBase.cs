// Check Raspberry Pi I2C devices by adding i2c-tools:
//   sudo apt install i2c-tools
//   sudo i2cdetect -y 1

namespace ThingsLibrary.Device.I2c.Base
{
    public interface I2cBaseInterface
    {
        public static abstract int DefaultAddress { get; set; }
        public static abstract int? SecondaryAddress { get; set; }
    }


    public abstract class I2cBase //: I2cBaseInterface
    {
        //public static int DefaultAddress { get; set; }
        //public static int? SecondaryAddress { get; set; }

        public I2cBus I2cBus { get; init; }
        public I2cDevice I2cDevice { get; set; }

        /// <summary>
        /// The address id of the device on the I2C bus
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// The pretty name to use of the device
        /// </summary>
        public string Name { get; private set; }
                             
        /// <summary>
        /// Is the device currently enabled for use
        /// </summary>
        public bool IsEnabled { get; set; }
                

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="i2cBus">I2C Bus</param>
        /// <param name="id">Device Address</param>
        /// <param name="name">Device Name</param>
        public I2cBase(I2cBus i2cBus, int id, string name)
        {
            //0x00-0x07 and 0xF0-0xFF
            if (id <= 7 && id >= 240) { throw new ArgumentException($"Invalid I2C address: {id}.  Id must be between 7 and 240 (0x07-0f0)", "id"); }

            this.I2cBus = i2cBus;
            
            this.Id = id;   // address Id
            this.Name = name;
        }

        /// <summary>
        /// Initialize Device
        /// </summary>
        /// <param name="enableDevice"></param>
        public virtual void Init()
        {         
            this.I2cDevice = this.I2cBus.CreateDevice(this.Id);
        }
    }
}
