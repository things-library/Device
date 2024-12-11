using System.Buffers.Binary;

namespace Iot.Device.Sen5x.Entities
{
    /// <summary>
    /// Represents the warm start parameters.
    /// </summary>
    public class WarmStartParameters : AbstractReadWriteEntity
    {
        internal override int ByteCount => 3;

        internal override void FromSpanByte(Span<Byte> data)
        {
            this.WarmStartBehavior = BinaryPrimitives.ReadUInt16BigEndian(data);
        }

        internal override void ToSpanByte(Span<Byte> data)
        {
            BinaryPrimitives.WriteUInt16BigEndian(data, this.WarmStartBehavior);
        }

        /// <summary>
        /// Gets or sets the warm start behavior as a value in the range from 0 (cold start, default value) to 65535 (warm start).
        /// </summary>
        public ushort WarmStartBehavior { get; private set; }
    }
}
