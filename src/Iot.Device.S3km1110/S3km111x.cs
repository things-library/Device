using System;

namespace Iot.Device.S3km1110
{
    /// <summary>
    /// Human Micro-Motion Detection Sensor, 24GHz mmWave Radar, S3KM1110, FMCW Technology
    /// </summary>
    /// <remarks>115200bps (default)</remarks>    
    public class S3km111x
    {
        // https://github.com/2Grey/s3km1110/blob/main/src/s3km1110.cpp
        // https://www.waveshare.com/hmmd-mmwave-sensor.htm
        // https://www.waveshare.com/wiki/HMMD_mmWave_Sensor

        public const byte FRAME_COMMAND_SIZE = 2;
        public const byte FRAME_LENGTH_SIZE = 2; 

        public const byte RADAR_COMMAND_READ_FIRMWARE_VERSION = 0x00;
        //        // 0x01    // Write register
        //        // 0x02    // Read register
        public const byte RADAR_COMMAND_RADAR_SET_CONFIG = 0x07;
        public const byte RADAR_COMMAND_RADAR_READ_CONFIG = 0x08;
        public const byte RADAR_COMMAND_READ_SERIAL_NUMBER = 0x11;
        public const byte RADAR_COMMAND_SET_MODE = 0x12;
        public const byte RADAR_COMMAND_READ_MODE = 0x13;
        //        // 0x24    // Enter factory test mode
        //        // 0x25    // Exit factory test mode
        //        // 0x26    // Send factory test results
        public const byte RADAR_COMMAND_OPEN_COMMAND_MODE = 0xFF;
        public const byte RADAR_COMMAND_CLOSE_COMMAND_MODE = 0xFE;

        public const byte RADAR_CONFIG_DETECTION_DISTANE_MIN = 0x00;
        public const byte RADAR_CONFIG_DETECTION_DISTANE_MAX = 0x01;
        public const byte RADAR_CONFIG_TARGET_ACTIVE_FRAMES = 0x02;
        public const byte RADAR_CONFIG_TARGET_INACTIVE_FRAMES = 0x03;
        public const byte RADAR_CONFIG_DISAPPEARANCE_DELAY = 0x04;
        
        public const byte RADAR_MODE_DEBUG = 0x00;
        public const byte RADAR_MODE_REPORT = 0x04;
        public const byte RADAR_MODE_RUNNING = 0x64;

        ~S3km111x()
        {
            //_uartRadar = null;
            //_uartDebug = null;
            //firmwareVersion = null;
            //serialNumber = null;
            //radarConfiguration = null;
        }

        //private string IntToHex(int value, byte byteCount)
        //{
        //    var width = byteCount * 2;
        //    var result = new string[width];
            
        //    uint32_t littleIndian = __htonl(value);
            
        //    //snprintf(result, width + 1, "%08x", littleIndian);
            
        //    return result;
        //}
    }
}
