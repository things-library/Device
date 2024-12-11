using System.Buffers.Binary;

namespace Iot.Device.Sen5x.Entities
{
    /// <summary>
    /// Represents the automatic cleaning interval configuration.
    /// </summary>
    public class AutoCleaningIntervalParameters : AbstractReadWriteEntity
    {
        internal override int ByteCount => 6;

        internal override void FromSpanByte(Span<Byte> data)
        {
            Span<Byte> buffer = new byte[4];
            buffer[0] = data[0];
            buffer[1] = data[1];
            buffer[2] = data[3];
            buffer[3] = data[4];

            this.AutoCleaningInterval = TimeSpan.FromSeconds(BinaryPrimitives.ReadInt32BigEndian(buffer));
        }

        internal override void ToSpanByte(Span<Byte> data)
        {
            Span<Byte> buffer = new byte[4];
            BinaryPrimitives.WriteInt32BigEndian(buffer, (int)this.AutoCleaningInterval.TotalSeconds);
            data[0] = buffer[0];
            data[1] = buffer[1];
            data[3] = buffer[2];
            data[4] = buffer[3];
        }

        /// <summary>
        /// Gets or sets the Auto Cleaning Interval [s].
        /// </summary>
        public TimeSpan AutoCleaningInterval { get; set; }
    }
}
