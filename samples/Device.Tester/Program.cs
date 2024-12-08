using Iot.Device.Ft232H;
using Iot.Device.FtCommon;
using Iot.Device.Board;

using Ft = Iot.Device.FtCommon;

using ThingsLibrary.Device.I2c;
using ThingsLibrary.Device.I2c.Base;

namespace Device.Tester
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
            
            // get the different devices that are plugged in and available
            Log.Information("Getting GPIO Controller devices...");
            var devices = Ft.FtCommon.GetDevices();
            Log.Information($"{devices.Count} found!");

            if (devices.Any())
            {
                foreach (var device in devices)
                {
                    Log.Information($"+ {device.Description} ({device.Flags}, Id: {device.Id}, Serial: {device.SerialNumber}, Type: {device.Type})");
                }
            }
            else
            {
                Log.Error("- No GPIO Controller device connected!");
                return;
            }

            // pick just the first one
            var ft232h = new Ft232HDevice(devices.First());
            var i2cBus = ft232h.CreateOrGetI2cBus(0);

            // Bus scan to show that the device(s) are plugged in and seen
            Log.Information("");
            Log.Information("Performing I2C Bus Scan...");

            var i2cDevices = i2cBus.PerformBusScan();
            Log.Information($"{i2cDevices.Count} found!");

            foreach (var i2cDeviceAddress in i2cDevices)
            {
                Log.Information($"+ Addr: {i2cDeviceAddress} (0x{i2cDeviceAddress:X})");
            }

            // TEST STACK:
            // - 0x76 - Bme688 (T,H,Vox,)
            // - 0x62 - Scd40 (T,H,CO2)
            // - 0x77 - BMP280 (T,P)
            // - 0x44 - SHT41 (T,H)

            // M5 ENV IV Sensor
            // - 0x44 - SHT41 (T,H)
            // - 0x76 - BMP280 (T,P)

            var sensors = new List<I2cSensor>();
            //sensors.Add(new Bme680Sensor(i2cBus, 0x76));
            //sensors.Add(new Scd40Sensor(i2cBus, 0x62));
            //sensors.Add(new Bmp280Sensor(i2cBus, 0x77));
            //sensors.Add(new Sht4xSensor(i2cBus, 0x44));

            sensors.Add(new Sht4xSensor(i2cBus, 0x44, "SHT41", true));
            sensors.Add(new Bmp280Sensor(i2cBus, 0x76, "BMP280", true));

            // Initialize all the sensors
            sensors.ForEach((sensor) => { sensor.Init(); });


            // DO SENSOR LOOP
            while (true)            
            {
                foreach(var sensor in sensors)
                {
                    if (sensor.FetchState())
                    {
                        Log.Information($"{sensor.Name}:");
                        foreach(var state in sensor.States)
                        {
                            Log.Information($" - {state}");
                        }
                    }
                }

                //Log.Information($"${DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}|Telem|{string.Join('|', pairs.Select(x => $"{x.Key}:{x.Value}"))}*");
                Thread.Sleep(1000);
            }

            Log.Information("Processing Complete!");
        }
    }
}