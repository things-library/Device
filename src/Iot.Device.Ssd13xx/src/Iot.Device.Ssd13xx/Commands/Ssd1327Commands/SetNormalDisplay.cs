// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace IotDevice.Ssd13xx.Commands.Ssd1327Commands
{
    /// <summary>
    /// Represents SetNormalDisplay command.
    /// </summary>
    public class SetNormalDisplay : ISsd1327Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetNormalDisplay" /> class.
        /// This command sets the display to be normal.
        /// </summary>
        public SetNormalDisplay()
        {
            //nothing
        }

        /// <summary>
        /// The value that represents the command.
        /// </summary>
        public byte Id => 0xA4;

        /// <summary>
        /// Gets the bytes that represent the command.
        /// </summary>
        /// <returns>The bytes that represent the command.</returns>
        public byte[] GetBytes()
        {
            return [this.Id];
        }
    }
}