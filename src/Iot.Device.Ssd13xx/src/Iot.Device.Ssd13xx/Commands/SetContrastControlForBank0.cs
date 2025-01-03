// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace IotDevice.Ssd13xx.Commands
{
    /// <summary>
    /// Represents SetContrastControlForBank0 command.
    /// </summary>
    public class SetContrastControlForBank0 : ISharedCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetContrastControlForBank0" /> class.
        /// This command sets the Contrast Setting of the display.
        /// The chip has 256 contrast steps from 0 to 255.
        /// The segment output current increases as the contrast step value increases.
        /// </summary>
        /// <param name="contrastSetting">Contrast setting with a range of 0-255.</param>
        public SetContrastControlForBank0(byte contrastSetting = 0x7F)
        {
            this.ContrastSetting = contrastSetting;
        }

        /// <summary>
        /// The value that represents the command.
        /// </summary>
        public byte Id => 0x81;

        /// <summary>
        /// Gets contrast setting with a range of 0-255.
        /// </summary>
        public byte ContrastSetting { get; }

        /// <summary>
        /// Gets the bytes that represent the command.
        /// </summary>
        /// <returns>The bytes that represent the command.</returns>
        public byte[] GetBytes()
        {
            return [Id, ContrastSetting];
        }
    }
}