namespace Iot.Device.Pmsx003
{
    /// <summary>
    /// Register shared by the Pmsx003 family.
    /// </summary>   
    internal enum Pmsx003Register : byte
    {
        // fixed register values for PMSx003 sensors
        FIXED_DIGIT_1 = 0x42,
        FIXED_DIGIT_2 = 0x4d,

        // fixed register values for Cubic PM1006
        FIXED_PM1006_1 = 0x16,      

        // REGISTERS
        DIGIT_1 = 0x01, //fixed to 0x42
        DIGIT_2 = 0x02, //fixed to 0x4d

        FRAME_HIGH = 0x02,
        FRAME_LOW = 0x03,
        
        // PM REGISTERS

        VERSION = 0x1c,
        ERROR_CODE = 0x1d,

        CRC_HIGH = 0x1e,
        CRC_LOW = 0x1f
    }
}
