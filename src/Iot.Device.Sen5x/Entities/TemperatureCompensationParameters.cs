using System.Buffers.Binary;
using UnitsNet;

namespace Iot.Device.Sen5x.Entities
{
    /// <summary>
    /// Represents the temperature compensation parameters.
    /// </summary>
    public class TemperatureCompensationParameters : AbstractReadWriteEntity
    {
        internal override int ByteCount => 9;

        internal override void FromSpanByte(Span<Byte> data)
        {
            this.TemperatureOffset = Temperature.FromDegreesCelsius(BinaryPrimitives.ReadInt16BigEndian(data) / 200.0);
            this.NormalizedTemperatureOffsetSlope = BinaryPrimitives.ReadInt16BigEndian(data.Slice(3)) / 10000.0;
            this.TimeConstant = TimeSpan.FromSeconds(BinaryPrimitives.ReadUInt16BigEndian(data.Slice(6)));
        }

        internal override void ToSpanByte(Span<Byte> data)
        {
            BinaryPrimitives.WriteInt16BigEndian(data, (short)(this.TemperatureOffset.DegreesCelsius * 200.0));
            BinaryPrimitives.WriteInt16BigEndian(data.Slice(3), (short)(this.NormalizedTemperatureOffsetSlope * 10000.0));
            BinaryPrimitives.WriteUInt16BigEndian(data.Slice(6), (ushort)this.TimeConstant.TotalSeconds);
        }

        /// <summary>
        /// Gets or sets the temperature offset [°C] (default value: 0).
        /// </summary>
        public Temperature TemperatureOffset { get; private set; }

        /// <summary>
        /// Gets or sets the normalized temperature offset slope (default value: 0).
        /// </summary>
        public double NormalizedTemperatureOffsetSlope { get; private set; }

        /// <summary>
        /// Gets or sets the time constant in seconds (default value: 0).
        /// </summary>
        public TimeSpan TimeConstant { get; private set; }
    }
}
