using Device.Tester.Devices;
using Device.Tester.Devices.Base;
using Iot.Device.Bmxx80;
using Iot.Device.Ft232H;
using Iot.Device.FtCommon;
using ThingsLibrary.Device.Spi;
using System;

namespace Device.Tester
{
    public static class Program
    {

        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            Log.Information("Getting GPIO Controller devices...");
            var devices = FtCommon.GetDevices();
            Log.Information("GPIO Devices: {GpioDeviceCount} found!", devices.Count);

            if (devices.Any())
            {
                foreach (var device in devices)
                {
                    Log.Information($"  {device.Description}");
                    Log.Information($"    Flags: {device.Flags}");
                    Log.Information($"    Id: {device.Id}");
                    Log.Information($"    LocId: {device.LocId}");
                    Log.Information($"    Serial number: {device.SerialNumber}");
                    Log.Information($"    Type: {device.Type}");
                }
            }
            else
            {
                Log.Error("No GPIO Controller device connected!");
                return;
            }

            //SpiConnectionSettings settings = new(0, 3) { ClockFrequency = 1_000_000, DataBitLength = 8, ChipSelectLineActiveState = PinValue.Low };
            //var spi = new Ft232HDevice(devices[0]).CreateSpiDevice(settings);

            var ft232h = new Ft232HDevice(devices.First());
            var settings = new SpiConnectionSettings(0, 3)
            {
                ClockFrequency = 2_400_000,
                Mode = SpiMode.Mode0,
                DataBitLength = 8
            };

            var spiDevice = ft232h.CreateSpiDevice(settings);

            ITestDevice testDevice = new NeoPixelTest(spiDevice);

            testDevice.Init();

            while (true)
            {
                testDevice.Loop();

                
                Thread.Sleep(1000);
            }

            Log.Information("Processing Complete!");
        }
    }
}
