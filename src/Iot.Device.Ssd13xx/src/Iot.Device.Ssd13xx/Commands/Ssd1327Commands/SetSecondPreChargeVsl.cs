// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace IotDevice.Ssd13xx.Commands.Ssd1327Commands
{
    /// <summary>
    /// Represents SetSecondPreChargeVsl command.
    /// </summary>
    public class SetSecondPreChargeVsl : ISsd1327Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetSecondPreChargeVsl" /> class.
        /// This command sets the first pre-charge voltage (phase 2) level of segment pins.
        /// </summary>
        /// <param name="secondPrecharge">Enable/disable second precharge.</param>
        /// <param name="externalVsl"> Switch between internal and external VSL.</param>
        public SetSecondPreChargeVsl(bool secondPrecharge = false, bool externalVsl = false)
        {
            this.Config = (byte)(secondPrecharge ? 0b0110_0010 : 0b0110_0000);
            this.Config |= (byte)(externalVsl ? 0b0110_0001 : 0b0110_0000);
        }

        /// <summary>
        /// Gets the value that represents the command.
        /// </summary>
        public byte Id => 0xD5;

        /// <summary>
        /// Gets the value that represents configuration.
        /// </summary>
        public byte Config { get; }

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