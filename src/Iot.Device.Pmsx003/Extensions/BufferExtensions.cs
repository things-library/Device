namespace Iot.Device.Pmsx003.Extensions
{
    public static class BufferExtensions
    {
        public static ushort ToUshort(this Span<byte> readBuffer, byte hi)
        {
            // The data comes in Endian so largest bit then smallest (register mapping below instead of assumptions)

            return (ushort)(readBuffer[hi] << 8 | readBuffer[hi + 1]);
        }
    }
}
