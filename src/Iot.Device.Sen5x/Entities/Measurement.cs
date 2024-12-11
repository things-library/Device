using System.Buffers.Binary;
using UnitsNet;

namespace Iot.Device.Sen5x.Entities
{
    /// <summary>
    /// Represents measured values from the sensor.
    /// </summary>
    public class Measurement : AbstractReadEntity
    {
        internal Measurement()
        {
            // Internal constructor, because this is a read-only entity.
        }

        internal override int ByteCount => 24;

        internal override void FromSpanByte(Span<Byte> data)
        {
            this.Pm1_0 = MassConcentration.FromMicrogramsPerCubicMeter(BinaryPrimitives.ReadUInt16BigEndian(data) / 10.0);
            this.Pm2_5 = MassConcentration.FromMicrogramsPerCubicMeter(BinaryPrimitives.ReadUInt16BigEndian(data.Slice(3)) / 10.0);
            this.Pm4_0 = MassConcentration.FromMicrogramsPerCubicMeter(BinaryPrimitives.ReadUInt16BigEndian(data.Slice(6)) / 10.0);
            this.Pm10_0 = MassConcentration.FromMicrogramsPerCubicMeter(BinaryPrimitives.ReadUInt16BigEndian(data.Slice(9)) / 10.0);
            this.Humidity = RelativeHumidity.FromPercent(BinaryPrimitives.ReadInt16BigEndian(data.Slice(12)) / 100.0);
            this.Temperature = Temperature.FromDegreesCelsius(BinaryPrimitives.ReadInt16BigEndian(data.Slice(15)) / 200.0);
            this.VocIndex = (ushort)(BinaryPrimitives.ReadInt16BigEndian(data.Slice(18)) / 10.0);
            this.NoxIndex = (ushort)(BinaryPrimitives.ReadInt16BigEndian(data.Slice(21)) / 10.0);
        }

        /// <summary>
        /// Gets or sets the Mass Concentration PM1.0 [µg/m³].
        /// </summary>
        public MassConcentration Pm1_0 { get; private set; }

        /// <summary>
        /// Gets or sets the Mass Concentration PM2.5 [µg/m³].
        /// </summary>
        public MassConcentration Pm2_5 { get; private set; }

        /// <summary>
        /// Gets or sets the Mass Concentration PM4.0 [µg/m³].
        /// </summary>
        public MassConcentration Pm4_0 { get; private set; }

        /// <summary>
        /// Gets or sets the Mass Concentration PM10 [µg/m³].
        /// </summary>
        public MassConcentration Pm10_0 { get; private set; }

        /// <summary>
        /// Gets or sets the Compensated Ambient Humidity [%RH].
        /// </summary>
        public RelativeHumidity Humidity { get; private set; }

        /// <summary>
        /// Gets or sets the Compensated Ambient Temperature [°C].
        /// </summary>
        public Temperature Temperature { get; private set; }

        /// <summary>
        /// Gets or sets the VOC Index.
        /// </summary>
        /// <remarks>
        /// The VOC Index mimics the human nose’s perception of odors with a relative intensity compared to recent history.The VOC Index is also sensitive to odorless VOCs, but it cannot discriminate between them.
        /// Sensirion's VOC Index is explained in more detail here: https://sensirion.com/media/documents/02232963/6294E043/Info_Note_VOC_Index.pdf .
        /// </remarks>
        public ushort VocIndex { get; private set; }

        /// <summary>
        /// Gets or sets the NOx Index.
        /// </summary>
        public ushort NoxIndex { get; private set; }

        /// <summary>
        /// Convenient method to quickly dump the read measurement.
        /// </summary>
        /// <returns>All measurement values in a single string.</returns>
        public override string ToString()
        {
            return $"{nameof(this.Pm1_0)}={this.Pm1_0.MicrogramsPerCubicMeter} µg/m³, " +
                $"{nameof(this.Pm2_5)}={this.Pm2_5.MicrogramsPerCubicMeter} µg/m³, " +
                $"{nameof(this.Pm4_0)}={this.Pm4_0.MicrogramsPerCubicMeter} µg/m³, " +
                $"{nameof(this.Pm10_0)}={this.Pm10_0.MicrogramsPerCubicMeter} µg/m³, " +
                $"{nameof(this.Humidity)}={this.Humidity.Percent} %RH, " +
                $"{nameof(this.Temperature)}={this.Temperature.DegreesCelsius} °C, " +
                $"{nameof(this.VocIndex)}={this.VocIndex}, " +
                $"{nameof(this.NoxIndex)}={this.NoxIndex}";
        }
    }
}
