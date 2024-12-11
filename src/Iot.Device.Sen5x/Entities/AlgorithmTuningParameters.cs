using System.Buffers.Binary;

namespace Iot.Device.Sen5x.Entities
{
    /// <summary>
    /// Represents the algorithm tuning parameters for either VOC or NOx.
    /// </summary>
    public class AlgorithmTuningParameters : AbstractReadWriteEntity
    {
        internal override int ByteCount => 18;

        internal override void FromSpanByte(Span<Byte> data)
        {
            this.IndexOffset = BinaryPrimitives.ReadInt16BigEndian(data);
            this.LearningTimeOffset = TimeSpan.FromHours(BinaryPrimitives.ReadInt16BigEndian(data.Slice(3)));
            this.LearningTimeGain = TimeSpan.FromHours(BinaryPrimitives.ReadInt16BigEndian(data.Slice(6)));
            this.GatingMaxDuration = TimeSpan.FromMinutes(BinaryPrimitives.ReadInt16BigEndian(data.Slice(9)));
            this.InitialStandardDeviation = BinaryPrimitives.ReadInt16BigEndian(data.Slice(12));
            this.GainFactor = BinaryPrimitives.ReadInt16BigEndian(data.Slice(15));
        }

        internal override void ToSpanByte(Span<Byte> data)
        {
            BinaryPrimitives.WriteInt16BigEndian(data, this.IndexOffset);
            BinaryPrimitives.WriteInt16BigEndian(data.Slice(3), (short)this.LearningTimeOffset.TotalHours);
            BinaryPrimitives.WriteInt16BigEndian(data.Slice(6), (short)this.LearningTimeGain.TotalHours);
            BinaryPrimitives.WriteInt16BigEndian(data.Slice(9), (short)this.GatingMaxDuration.TotalMinutes);
            BinaryPrimitives.WriteInt16BigEndian(data.Slice(12), this.InitialStandardDeviation);
            BinaryPrimitives.WriteInt16BigEndian(data.Slice(15), this.GainFactor);
        }

        /// <summary>
        /// Gets or sets the index offset.
        /// </summary>
        public short IndexOffset { get; set; }

        /// <summary>
        /// Gets or sets the Learning Time Offset Hours. Note that the smallest counted unit is hours.
        /// </summary>
        public TimeSpan LearningTimeOffset { get; set; }

        /// <summary>
        /// Gets or sets the Learning Time Gain Hours. Note that the smallest counted unit is hours.
        /// </summary>
        public TimeSpan LearningTimeGain { get; set; }

        /// <summary>
        /// Gets or sets the Gating Max Duration Minutes. Note that the smallest counted unit is minutes.
        /// </summary>
        public TimeSpan GatingMaxDuration { get; set; }

        /// <summary>
        /// Gets or sets the initial (estimate for) standard deviation.
        /// </summary>
        public short InitialStandardDeviation { get; set; }

        /// <summary>
        /// Gets or sets the Gain Factor.
        /// </summary>
        public short GainFactor { get; set; }
    }
}
