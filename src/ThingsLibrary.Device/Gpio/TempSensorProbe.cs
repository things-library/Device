//using Iot.Device.OneWire;
//using Starlight.Device.Gpio.Base;

//// https://github.com/dotnet/iot/tree/main/src/devices/OneWire
//// https://docs.microsoft.com/en-us/dotnet/iot/tutorials/temp-sensor

////NOTE: Add the following to /boot/config.txt to enable 1-wire protocol. The default gpio is 4 (pin 7).
////         dtoverlay=w1-gpio,gpiopin=17

////Supported Dallas DS18B20, MAX31820

//namespace Starlight.Device.Gpio
//{
//    public sealed class TempSensorProbe : GpioDevice
//    {
//        public OneWireThermometerDevice Device { get; set; }

//        [JsonPropertyName("bus")]
//        public string BusId { get; set; }

//        [JsonPropertyName("device")]
//        public string DeviceId { get; set; }

//        [JsonPropertyName("temp")]
//        public double Temperature => Math.Round(this.IsMetric ? _temperature.DegreesCelsius : _temperature.DegreesFahrenheit, 2);
//        private UnitsNet.Temperature _temperature;
                
//        [JsonPropertyName("read_interval")]
//        public int ReadInterval { get; set; }

//        [JsonPropertyName("metric")]
//        public bool IsMetric { get; set; }

//        [JsonPropertyName("temp_units")]
//        public string TempUnitSymbol => (this.IsMetric ? "C" : "F");

//        [JsonIgnore]
//        public override string StateStr => $"Temperature: {this.Temperature:0.00}{this.TempUnitSymbol}";

//        [JsonIgnore]
//        public override string StateJson => $"{{\"id\":{this.PinId},\"temp\":{this.Temperature:0.00} }}";


//        public override string ToString() => this.StateJson;


//        public TempSensorProbe() : base()
//        {

//        }

//        public override void Init()
//        {
//            //GpioBase.Controller.OpenPin(this.PinId, PinMode.Output);

//            ////var bus = new OneWireBus()
//            //foreach (var device in OneWireThermometerDevice.EnumerateDevices())
//            //{
//            //    this.Devices.Add(new OneWireThermometerDevice(device.BusId, device.DeviceId));
//            //}

//            if(this.ReadInterval == 0)
//            {
//                this.ReadInterval = 10 * 1000; // 10 seconds
//            }

//            // read to confirm
//            this.FetchState();
//        }

//        public override void FetchState()
//        {
//            // nothing to do?
//            if (!this.IsEnabled)
//            {
//                if (this.IsChanged) { this.IsChanged = false; }
//                return;
//            }

//            //unable to do that since we are in cooldown phase?
//            if (this.ReadInterval > 0 && DateTime.UtcNow < this.StateChangedOn.AddMilliseconds(this.ReadInterval))
//            {
//                this.IsChanged = false;
//                return;
//            }

//            _temperature = this.Device.ReadTemperature();

//            this.StateChangedOn = DateTime.UtcNow;
//            this.IsChanged = true;
//        }
//    }
//}
