using System.Device.Gpio;
using System.Device.I2c;

using Iot.Device.Ft232H;
using Iot.Device.Board;

using Ft = Iot.Device.FtCommon;

using ThingsLibrary.DataType.Extensions;
using ThingsLibrary.Device.Gpio;
using ThingsLibrary.Device.I2c;
using ThingsLibrary.Device.I2c.Base;
using ThingsLibrary.Device.Sensor.Events;
using Iot.Device.Mcp23xxx;

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
            Log.Information("================================================================================");
            Log.Information("Getting GPIO Controller devices...");
            var devices = Ft.FtCommon.GetDevices();

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
            var ftDevice = new Ft232HDevice(devices.First());

            //var gpioController = ftDevice.CreateGpioController();
            //GpioTests(gpioController);

            var i2cBus = ftDevice.CreateOrGetI2cBus(0);
            McpTests(i2cBus);
            
            //I2cTests(i2cBus);
        }

        public static void GpioTests(GpioController gpioController)
        {
            var sensors = new List<BoolSensor>();
            sensors.Add(new BoolSensor(gpioController, 0, "Motion", false));

            Log.Information("Initializing {SensorCount} Sensors...", sensors.Count);

            // Initialize all the sensors
            sensors.ForEach((sensor) => 
            { 
                sensor.Init(); 
                
            });

            sensors[0].BoolState.StateChanged += StateChanged;

            Log.Information("================================================================================");
            Log.Information("Reading Sensor Telemetry...");
                        
            // DO SENSOR LOOP
            while (true)
            {
                foreach (var sensor in sensors)
                {
                    if (!sensor.IsEnabled)
                    {
                        Log.Information($"{sensor.Name}: Not Enabled");
                        continue;
                    }

                    sensor.FetchStates();

                    Log.Information($"{sensor.Name}: {sensor.StateStr} ({sensor.BoolState.StateDuration().ToHHMMSS()})");
                }

                Thread.Sleep(2000);
            }
        }

        public static void StateChanged(object sender, StateEvent e)
        {
            //this.CurrentState.ToString($"n{this.ValuePrecision}")}

            if(e.LastState != null)
            {
                Log.Information($"{e.Id}: {e.State} (Last: {e.LastState?.ToString($"n{e.ValuePrecision}")}, Dur: {e.LastStateDuration.ToHHMMSS()}");
                
            }
            else
            {
                Log.Information($"{e.Id}: {e.State}");
            }
        }

        public static void SwitchTests(Mcp230xx mcp)
        {
            // set up the input/output modes
            mcp.Device.WriteByte(Register.IODIR, 0b1111_1111, Port.PortA); // Switch Matrix            
            mcp.Device.WriteByte(Register.GPPU, 0b1111_1111, Port.PortA); // pullups

            byte value;
            while (true)
            {
                value = mcp.Device.ReadByte(Register.GPIO, Port.PortA);

                Console.WriteLine($"B:{Convert.ToString(value, 2)}");
                Thread.Sleep(500);
            }
        }

        public static void LedTests(Mcp230xx mcp)
        {
            // set up the input/output modes
            mcp.Device.WriteByte(Register.IODIR, 0b0000_0000, Port.PortA); // Switch Matrix            
            mcp.Device.WriteByte(Register.IODIR, 0b000_0000, Port.PortB); // LEDs
            mcp.Device.WriteByte(Register.GPPU, 0b0000_0000, Port.PortB); // pullups

            for (byte i = 1; i <= 6; i++)
            {
                ShowSelection(mcp.Device, i);
                Thread.Sleep(200);
            }

            for (byte i = 6; i > 0; i--)
            {
                ShowSelection(mcp.Device, i);
                Thread.Sleep(200);
            }
            
            // COLS
            mcp.Device.WriteByte(Register.GPIO, 0b0000_1001, Port.PortB);
            Thread.Sleep(400);
            mcp.Device.WriteByte(Register.GPIO, 0b0001_0010, Port.PortB);
            Thread.Sleep(400);
            mcp.Device.WriteByte(Register.GPIO, 0b0010_0100, Port.PortB);
            Thread.Sleep(400);
            mcp.Device.WriteByte(Register.GPIO, 0b0001_0010, Port.PortB);
            Thread.Sleep(400);
            mcp.Device.WriteByte(Register.GPIO, 0b0000_1001, Port.PortB);
            Thread.Sleep(400);

            // ROWS
            mcp.Device.WriteByte(Register.GPIO, 0b0000_0111, Port.PortB);
            Thread.Sleep(400);
            mcp.Device.WriteByte(Register.GPIO, 0b0011_1000, Port.PortB);
            Thread.Sleep(400);
            mcp.Device.WriteByte(Register.GPIO, 0b0000_0111, Port.PortB);
            Thread.Sleep(400);

            mcp.Device.WriteByte(Register.GPIO, 0b1111_1111, Port.PortB);
            Thread.Sleep(1000);
            mcp.Device.WriteByte(Register.GPIO, 0b0000_0000, Port.PortB);
            Thread.Sleep(500);
        }

        public static void McpTests(I2cBus i2cBus)
        {
            var mcp = new Mcp230xx(i2cBus, 0x27);
            mcp.Init();

            //// set up the input/output modes
            //mcp.Device.WriteByte(Register.IODIR, 0b0000_0000, Port.PortA); // Switch Matrix            
            //mcp.Device.WriteByte(Register.IODIR, 0b1111_1111, Port.PortB); // LEDs
            //mcp.Device.WriteByte(Register.GPPU, 0b1111_1111, Port.PortB); // pullups

            ////mcp.Device.WriteByte(Register.GPIO, 0b1111_0000, Port.PortB);

            //byte value;
            //while (true)
            //{
            //    value = mcp.Device.ReadByte(Register.GPIO, Port.PortB);

            //    Console.WriteLine($"B:{Convert.ToString(value, 2)}");
            //    Thread.Sleep(500);
            //}


            //SwitchTests(mcp);

            while (true)
            {
                LedTests(mcp);
            }
        }        

       

        //public static void ShowSelection(Mcp23017 mcp)
        //{
        //    mcp.WriteByte(Register.GPIO, 0b0000_0000, Port.PortB);


        //    byte result = 0;
        //    result |= (byte)(bool1 ? 1 << 0 : 0);
        //    result |= (byte)(bool2 ? 1 << 1 : 0);
        //    result |= (byte)(bool3 ? 1 << 2 : 0);
        //    result |= (byte)(bool4 ? 1 << 3 : 0);
        //    result |= (byte)(bool5 ? 1 << 4 : 0);
        //    result |= (byte)(bool6 ? 1 << 5 : 0);
        //    result |= (byte)(bool7 ? 1 << 6 : 0);
        //    result |= (byte)(bool8 ? 1 << 7 : 0);

        //    mcp.WriteByte(Register.GPIO, 0b0000_0000, Port.PortB);
        //}


        public static void ShowSelection(Mcp23017 mcp, byte id)
        {            
            mcp.WriteByte(Register.GPIO, (byte)(1 << (byte)(id - 1)), Port.PortB);
        }

        public static void ShowRow(byte row)
        {
            
        }


        public static void I2cTests(I2cBus i2cBus) 
        {         
            // Bus scan to show that the device(s) are plugged in and seen
            Log.Information("================================================================================");
            Log.Information("Performing I2C Bus Scan...");

            var i2cDevices = i2cBus.PerformBusScan();            
            foreach (var i2cDeviceAddress in i2cDevices)
            {
                Log.Information($"+ Addr: {i2cDeviceAddress} (0x{i2cDeviceAddress:X})");
            }

            // TEST STACK:
            // - 0x76 - Bme688 (T,H,Vox,)
            // - 0x62 - Scd40 (T,H,CO2)
            // - 0x77 - BMP280 (T,P)
            // - 0x44 - SHT41 (T,H)
            // - 0x29 - Vl53l0xSensor (D)
            // - 0x12 - Pmsx003 (AQI)
            // - 0x69 - Sen5x (AQI)

            // M5 ENV IV Sensor
            // - 0x44 - SHT41 (T,H)
            // - 0x76 - BMP280 (T,P)
            

            var sensors = new List<I2cSensor>();
            //sensors.Add(new Bme680Sensor(i2cBus, 0x76, "BME680", true));
            //sensors.Add(new Scd40Sensor(i2cBus, 0x62));
            //sensors.Add(new Bmp280Sensor(i2cBus, 0x77));
            //sensors.Add(new Sht4xSensor(i2cBus, 0x44));
            //sensors.Add(new Vl53l0xSensor(i2cBus));
            //sensors.Add(new Vl53l1xSensor(i2cBus));
            
            //sensors.Add(new Pmsx003Sensor(i2cBus));
            //sensors.Add(new Sen5xSensor(i2cBus));
                        
            // M5 ENV IV Sensor Module
            //sensors.Add(new Sht4xSensor(i2cBus, 0x44, "SHT41", true));
            //sensors.Add(new Bmp280Sensor(i2cBus, 0x76, "BMP280", true));

            Log.Information("Initializing {SensorCount} Sensors...", sensors.Count);

            // Initialize all the sensors
            sensors.ForEach((sensor) => { sensor.Init(); });

            // show any errors 
            foreach (var sensor in sensors)
            {
                if (sensor.IsEnabled) { continue; }
                if (string.IsNullOrEmpty(sensor.ErrorMessage)) { continue; }

                Log.Information($"{sensor.Name}: {sensor.ErrorMessage} (Not Enabled)");
            }

            Log.Information("================================================================================");
            Log.Information("Reading Sensor Telemetry...");
                        
            // DO SENSOR LOOP
            while (true)            
            {
                foreach (var sensor in sensors)
                {                    
                    if (!sensor.IsEnabled) { continue; }

                    if (sensor.FetchStates())
                    {
                        //Log.Information($"{sensor.Name}:");
                        Log.Information(sensor.ToTelemetryString());                        
                    }
                }

                Thread.Sleep(1000);
            }
        }
    }
}