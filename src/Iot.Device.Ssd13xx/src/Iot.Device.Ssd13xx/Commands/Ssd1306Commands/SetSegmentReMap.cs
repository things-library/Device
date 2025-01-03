// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace IotDevice.Ssd13xx.Commands.Ssd1306Commands
{
    /// <summary>
    /// Represents SetSegmentReMap command.
    /// </summary>
    public class SetSegmentReMap : ISsd1306Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetSegmentReMap" /> class.
        /// This command changes the mapping between the display data column address and the segment driver.
        /// It allows flexibility in OLED module design. This command only affects subsequent data input.
        /// Data already stored in GDDRAM will have no changes.
        /// </summary>
        /// <param name="columnAddress127">Column address 0 is mapped to SEG0 when FALSE.
        /// Column address 127 is mapped to SEG0 when TRUE.</param>
        public SetSegmentReMap(bool columnAddress127 = false)
        {
            ColumnAddress127 = columnAddress127;
        }

        /// <summary>
        /// The value that represents the command.
        /// </summary>
        public byte Id => (byte)(ColumnAddress127 ? 0xA1 : 0xA0);

        /// <summary>
        /// Gets or sets a value indicating whether column address 127 is mapped to SEG0.
        /// </summary>
        public bool ColumnAddress127 { get; set; }

        /// <summary>
        /// Gets the bytes that represent the command.
        /// </summary>
        /// <returns>The bytes that represent the command.</returns>
        public byte[] GetBytes()
        {
            return [Id];
        }
    }
}