// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace IotDevice.Ssd13xx.Commands.Ssd1327Commands
{
    /// <summary>
    /// Represents SetDisplayOffset command.
    /// </summary>
    public class SetDisplayOffset : ISsd1327Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetDisplayOffset" /> class.
        /// This command specifies the mapping of the display start line to one of COM0-COM127
        /// (assuming that COM0 is the display start line then the display start line register is equal to 0).
        /// </summary>
        /// <param name="displayOffset">Display offset with a range of 0-127.</param>
        public SetDisplayOffset(byte displayOffset = 0x00)
        {
            if (displayOffset > 0x5F)
            {
                throw new ArgumentOutOfRangeException(nameof(displayOffset));
            }

            this.DisplayOffset = displayOffset;
        }

        /// <summary>
        /// Gets the value that represents the command.
        /// </summary>
        public byte Id => 0xA2;

        /// <summary>
        /// Gets display offset with a range of 0-127.
        /// </summary>
        public byte DisplayOffset { get; }

        /// <summary>
        /// Gets the bytes that represent the command.
        /// </summary>
        /// <returns>The bytes that represent the command.</returns>
        public byte[] GetBytes()
        {
            return [this.Id, this.DisplayOffset];
        }
    }
}