// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace IotDevice.Ssd13xx.Commands.Ssd1327Commands
{
    /// <summary>
    /// Represents SetDisplayClockDivideRatioOscillatorFrequency command.
    /// </summary>
    public class SetDisplayClockDivideRatioOscillatorFrequency : ISsd1327Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetDisplayClockDivideRatioOscillatorFrequency" /> class.
        /// This command sets the divide ratio to generate DCLK (Display Clock) from CLK and
        /// programs the oscillator frequency Fosc that is the source of CLK if CLS pin is pulled high.
        /// </summary>
        /// <param name="displayClockDivideRatio">Display clock divide ratio with a range of 0-15. For more information see device documentations.</param>
        /// <param name="oscillatorFrequency">Oscillator frequency with a range of 0-15 in Kilohertz. For more information see device documentations.</param>
        public SetDisplayClockDivideRatioOscillatorFrequency(byte displayClockDivideRatio = 0x00, byte oscillatorFrequency = 0x00)
        {
            if (displayClockDivideRatio > 0x0F)
            {
                throw new ArgumentOutOfRangeException(nameof(displayClockDivideRatio));
            }

            if (oscillatorFrequency > 0x0F)
            {
                throw new ArgumentOutOfRangeException(nameof(oscillatorFrequency));
            }

            this.DisplayClockDivideRatio = displayClockDivideRatio;
            this.OscillatorFrequency = oscillatorFrequency;
        }

        /// <summary>
        /// The value that represents the command.
        /// </summary>
        public byte Id => 0xB3;

        /// <summary>
        /// Gets display clock divide ratio with a range of 0-15.
        /// </summary>
        public byte DisplayClockDivideRatio { get; }

        /// <summary>
        /// Gets oscillator frequency with a range of 0-15.
        /// </summary>
        public byte OscillatorFrequency { get; }

        /// <summary>
        /// Gets the bytes that represent the command.
        /// </summary>
        /// <returns>The bytes that represent the command.</returns>
        public byte[] GetBytes()
        {
            byte displayClockDivideRatioOscillatorFrequency = (byte)((this.OscillatorFrequency << 4) | this.DisplayClockDivideRatio);

            return [this.Id, displayClockDivideRatioOscillatorFrequency];
        }
    }
}