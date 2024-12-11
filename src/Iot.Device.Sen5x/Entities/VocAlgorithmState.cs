namespace Iot.Device.Sen5x.Entities
{
    /// <summary>
    /// Represents the VOC algorithm state.
    /// </summary>
    public class VocAlgorithmState : AbstractReadWriteEntity
    {
        internal override int ByteCount => 12;

        internal override void FromSpanByte(Span<Byte> data)
        {
            this.State[0] = data[0];
            this.State[1] = data[1];
            this.State[2] = data[3];
            this.State[3] = data[4];
            this.State[4] = data[6];
            this.State[5] = data[7];
            this.State[6] = data[9];
            this.State[7] = data[10];
        }

        internal override void ToSpanByte(Span<Byte> data)
        {
            data[0] = this.State[0];
            data[1] = this.State[1];

            data[3] = this.State[2];
            data[4] = this.State[3];

            data[6] = this.State[4];
            data[7] = this.State[5];

            data[9] = this.State[6];
            data[10] = this.State[7];
        }

        /// <summary>
        /// Gets or sets the VOC algorithm state represented in an 8 byte array.
        /// </summary>
        public byte[] State { get; private set; } = new byte[8];
    }
}
