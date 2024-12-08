// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace IotDevice.Ssd13xx.Commands.Ssd1327Commands
{
    /// <summary>
    /// Represents SetReMap command.
    /// </summary>
    public class SetReMap : ISsd1327Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetReMap" /> class.
        /// Re-map setting in Graphic Display Data RAM(GDDRAM).
        /// </summary>
        /// <param name="columnAddressRemap">Should remap column addresses.</param>
        /// <param name="nibbleRemap">Is nibble remap enabled.</param>
        /// <param name="verticalMode">Is vertical mode enabled.</param>
        /// <param name="comRemap">Is com remap enabled.</param>
        /// <param name="comSplitOddEven">IS com split odd even enabled.</param>
        public SetReMap(
            bool columnAddressRemap = false,
            bool nibbleRemap = true,
            bool verticalMode = true,
            bool comRemap = false,
            bool comSplitOddEven = true)
        {
            Config = 0b0000_0000;
            if (columnAddressRemap)
            {
                this.Config |= 0b0000_0001;
            }

            if (nibbleRemap)
            {
                this.Config |= 0b0000_0010;
            }

            if (verticalMode)
            {
                this.Config |= 0b0000_0100;
            }

            if (comRemap)
            {
                this.Config |= 0b0001_0000;
            }

            if (comSplitOddEven)
            {
                this.Config |= 0b0100_0000;
            }
        }

        /// <summary>
        /// Gets the value that represents the command.
        /// </summary>
        public byte Id => 0xA0;

        /// <summary>
        /// Gets or sets ReMap Config.
        /// </summary>
        public byte Config { get; set; }

        /// <summary>
        /// Gets the bytes that represent the command.
        /// </summary>
        /// <returns>The bytes that represent the command.</returns>
        public byte[] GetBytes()
        {
            return [this.Id, this.Config];
        }
    }
}
