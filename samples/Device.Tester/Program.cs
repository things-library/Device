using Iot.Device.Ft232H;
using Iot.Device.FtCommon;
using Iot.Device.Board;

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

            // TEST STACK:
            // - 0x76 - Bme688 (T,H,Vox,)
            // - 0x62 - Scd40 (T,H,CO2)
            // - 0x77 - BMP280 (T,P)
            // - 0x44 - SHT41 (T,H)
            
            var ft232h = new Ft232HDevice(devices.First());
            
            var i2cBus = ft232h.CreateOrGetI2cBus(0);
            var i2cDevices = i2cBus.PerformBusScan();
            foreach (var i2cDeviceAddress in i2cDevices)
            {
                Log.Information($"+ Addr: {i2cDeviceAddress} (0x{i2cDeviceAddress:X})");
            }

            
            var sensors = new List<I2cSensor>();
            sensors.Add(new Bme680Sensor(i2cBus, 0x76));
            sensors.Add(new Scd40Sensor(i2cBus, 0x62));
            sensors.Add(new Bmp280Sensor(i2cBus, 0x77));
            sensors.Add(new Sht4xSensor(i2cBus, 0x44));

            // Initialize all the sensors
            Parallel.ForEach(sensors, (sensor) => { sensor.Init(); });


            while (true)            
            {
                foreach(var sensor in sensors)
                {
                    if (sensor.FetchState())
                    {

                    }
                }
                
                //Log.Information($"${DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}|Telem|{string.Join('|', pairs.Select(x => $"{x.Key}:{x.Value}"))}*");
                Thread.Sleep(1000);
            }

            Log.Information("Processing Complete!");
        }
    }
}