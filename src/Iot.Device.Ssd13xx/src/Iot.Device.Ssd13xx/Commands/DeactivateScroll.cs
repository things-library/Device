// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace IotDevice.Ssd13xx.Commands
{
    /// <summary>
    /// Represents DeactivateScroll command.
    /// </summary>
    public class DeactivateScroll : ISharedCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeactivateScroll" /> class.
        /// This command stops the motion of scrolling. After sending 2Eh command to deactivate
        /// the scrolling action, the ram data needs to be rewritten.
        /// </summary>
        public DeactivateScroll()
        {
            //nothing
        }

        /// <summary>
        /// The value that represents the command.
        /// </summary>
        public byte Id => 0x2E;

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
