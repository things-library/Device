// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Device.I2c;

using Ssd1327Cmnds = IotDevice.Ssd13xx.Commands.Ssd1327Commands;

namespace IotDevice.Ssd13xx
{
    /// <summary>
    /// Represents SSD1327 OLED display.
    /// </summary>
    public class Ssd1327 : Ssd13xx
    {
        private const byte CommandMode = 0x80;
        private const byte DataMode = 0x40;
        private const int CleaningBufferSize = 48 * 96;

        /// <summary>
        /// Initializes a new instance of the <see cref="Ssd1327" /> class.
        /// </summary>
        /// <param name="i2cDevice">The I2C device used for communication.</param>
        public Ssd1327(I2cDevice i2cDevice) : base(i2cDevice)
        {
            //nothing
        }

        /// <summary>
        /// Sets column address.
        /// </summary>
        /// <param name="startAddress">Start address.</param>
        /// <param name="endAddress">End address.</param>
        public void SetColumnAddress(byte startAddress = 0x08, byte endAddress = 0x37) => this.SendCommand(new Ssd1327Cmnds.SetColumnAddress(startAddress, endAddress));

        /// <summary>
        /// Sets row address.
        /// </summary>
        /// <param name="startAddress">Start address.</param>
        /// <param name="endAddress">End address.</param>
        public void SetRowAddress(byte startAddress = 0x00, byte endAddress = 0x5f) => this.SendCommand(new Ssd1327Cmnds.SetRowAddress(startAddress, endAddress));

        /// <summary>
        /// Clears the display.
        /// </summary>
        public void ClearDisplay()
        {
            this.SendCommand(new Commands.SetDisplayOff());
            this.SetColumnAddress();
            this.SetRowAddress();
            
            byte[] data = new byte[CleaningBufferSize];
            this.SendData(data);
            this.SendCommand(new Commands.SetDisplayOn());
        }

        /// <summary>
        /// Send a command to the display controller.
        /// </summary>
        /// <param name="command">The command to send to the display controller.</param>
        public void SendCommand(byte command)
        {
            Span<Byte> writeBuffer = [CommandMode, command];

            this.I2cDevice.Write(writeBuffer);
        }

        /// <summary>
        /// Sends command to the device.
        /// </summary>
        /// <param name="command">Command being send.</param>
        public void SendCommand(Commands.ISsd1327Command command) => SendCommand((Commands.ICommand)command);

        /// <summary>
        /// Sends command to the device.
        /// </summary>
        /// <param name="command">Command being send.</param>
        public override void SendCommand(Commands.ISharedCommand command) => SendCommand(command);

        /// <summary>
        /// Send data to the display controller.
        /// </summary>
        /// <param name="data">The data to send to the display controller.</param>
        public void SendData(byte data)
        {
            Span<Byte> writeBuffer = [DataMode, data];

            this.I2cDevice.Write(writeBuffer);
        }

        private void SendCommand(Commands.ICommand command)
        {
            byte[] commandBytes = command.GetBytes();

            if (commandBytes.Length == 0)
            {
                throw new ArgumentException(nameof(command), "Argument is either null or there were no bytes to send.");
            }

            foreach (var item in commandBytes)
            {
                this.SendCommand(item);
            }
        }
    }
}
