using Iot.Device.Ft232H;
using Iot.Device.Board;

using Ft = Iot.Device.FtCommon;
using Dv = IotDevice.Ssd13xx;

namespace Iot.Device.Ssd13xx.Samples
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            // get the different devices that are plugged in and available
            Console.Write("Getting GPIO Controller devices...");
            var devices = Ft.FtCommon.GetDevices();
            Console.WriteLine($"{devices.Count} found!");

            if (devices.Any())
            {
                foreach (var device in devices)
                {
                    Console.WriteLine($"+ {device.Description} ({device.Flags}, Id: {device.Id}, Serial: {device.SerialNumber}, Type: {device.Type})");                    
                }
            }
            else
            {
                Console.WriteLine("- No GPIO Controller device connected!");
                return;
            }

            // pick just the first one
            var ft232h = new Ft232HDevice(devices.First());
            var i2cBus = ft232h.CreateOrGetI2cBus(0);

            // Bus scan to show that the device(s) are plugged in and seen
            Console.WriteLine();
            Console.Write("Performing I2C Bus Scan...");

            var i2cDevices = i2cBus.PerformBusScan();
            Console.WriteLine($"{i2cDevices.Count} found!");

            foreach (var i2cDeviceAddress in i2cDevices)
            {
                Console.WriteLine($"+ Addr: {i2cDeviceAddress} (0x{i2cDeviceAddress:X})");
            }
            
            // get the display i2c device
            var i2cDevice = i2cBus.CreateDevice(Dv.Ssd1306.DefaultI2cAddress);

            Console.WriteLine();
            Console.WriteLine("Performing OLED Tests...");

            //Tested with 128x64 and 128x32 OLEDs
            using var display = new Dv.Ssd1306(i2cDevice, Dv.DisplayResolution.OLED128x64);

            display.ClearScreen();
            display.Font = new IotDevice.Ssd13xx.Fonts.BasicFont8x8();
            display.DrawString(2, 2, "nF IOT!", 2);//large size 2 font
            display.DrawString(2, 32, "nanoFramework", 1, true);//centered text
            display.Show();

            Thread.Sleep(2000);

            display.ClearScreen();
            display.Font = new DoubleByteFont();
            display.DrawString(2, 2, "功夫＄", 2, false);
            display.DrawString(2, 34, "８９ＡＢ功夫＄", 1, true);
            display.Show();

            Thread.Sleep(2000);

            display.ClearScreen();
            display.DrawLineHorizontal(10, 5, 50);
            display.DrawLineVertical(10, 5, 50);
            display.DrawRectangle(0, 0, display.Width, display.Height);
            display.DrawRectangleFilled(20, 20, 80, 20);

            display.Show();

            Thread.Sleep(2000);

            display.ClearScreen();
            display.DrawRectangleRounded(0, 0, display.Width, display.Height, 4);
            
            display.Show();

            Thread.Sleep(2000);

            // clear 
            display.ClearScreen();

            Console.WriteLine("Complete.");
        }
    }
}
