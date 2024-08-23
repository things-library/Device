//using Iot.Device.Common;
//using Iot.Device.DHTxx;
//using Starlight.Device.Gpio.Base;


//// https://github.com/dotnet/iot/tree/main/src/devices/Dhtxx
//// https://docs.microsoft.com/en-us/dotnet/iot/tutorials/temp-sensor

//namespace Starlight.Device.Gpio
//{
//    public sealed class TempSensorDht : GpioDevice
//    {
//        public DhtBase DhtDevice { get; set; }

//        [JsonPropertyName("type")]
//        public DhtType Type { get; set; }

//        [JsonPropertyName("temp")]
//        public double Temperature => Math.Round(this.IsMetric ? _temperature.DegreesCelsius : _temperature.DegreesFahrenheit, 2);
//        private UnitsNet.Temperature _temperature;

//        [JsonPropertyName("humidity")]
//        public double Humidity => Math.Round(_humidity.Percent, 2);
//        private UnitsNet.RelativeHumidity _humidity;

//        // calculated fields
//        [JsonPropertyName("dew_point")]
//        public double DewPoint => (this.IsMetric ? WeatherHelper.CalculateDewPoint(_temperature, _humidity).DegreesCelsius : WeatherHelper.CalculateDewPoint(_temperature, _humidity).DegreesFahrenheit);

//        [JsonPropertyName("heat_index")]
//        public double HeatIndex => (this.IsMetric ? WeatherHelper.CalculateHeatIndex(_temperature, _humidity).DegreesCelsius : WeatherHelper.CalculateHeatIndex(_temperature, _humidity).DegreesFahrenheit);

//        [JsonPropertyName("read_interval")]
//        public int ReadInterval { get; set; }

//        [JsonPropertyName("metric")]
//        public bool IsMetric { get; set; }

//        [JsonPropertyName("temp_units")]
//        public string TempUnitSymbol => (this.IsMetric ? "C" : "F");

//        [JsonIgnore]
//        public override string StateStr => $"Temperature: {this.Temperature:0.00}{this.TempUnitSymbol}, Humidity: {this.Humidity:0.0}%, Dew Point: {this.DewPoint:0.00}{this.TempUnitSymbol}, Heat Index: {this.HeatIndex:0.00}{this.TempUnitSymbol}";

//        [JsonIgnore]
//        public override string StateJson => $"{{\"id\":{this.PinId},\"temp\":{this.Temperature:0.00},\"hum\":{this.Humidity:0.0},\"dew\":{this.DewPoint:0.00},\"hIndex\":{this.HeatIndex:0.00} }}";


//        public override string ToString() => this.StateJson;


//        public TempSensorDht() : base()
//        {

//        }

//        public override void Init()
//        {
//            //ID: 0x77 (default)
//            switch (this.Type)
//            {
//                case DhtType.Dht11: { this.DhtDevice = new Dht11(this.PinId, System.Device.Gpio.PinNumberingScheme.Board, GpioDevice.Controller); break; }
//                case DhtType.Dht12: { this.DhtDevice = new Dht12(this.PinId, System.Device.Gpio.PinNumberingScheme.Board, GpioDevice.Controller); break; }
//                case DhtType.Dht21: { this.DhtDevice = new Dht21(this.PinId, System.Device.Gpio.PinNumberingScheme.Board, GpioDevice.Controller); break; }
//                case DhtType.Dht22: { this.DhtDevice = new Dht22(this.PinId, System.Device.Gpio.PinNumberingScheme.Board, GpioDevice.Controller); break; }

//                default:
//                    {
//                        throw new ArgumentException($"Unknown type '{this.Type}'");
//                    }
//            }

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

//            // Read the sensor values
//            _temperature = this.DhtDevice.Temperature;
//            _humidity = this.DhtDevice.Humidity;

//            if (this.DhtDevice.IsLastReadSuccessful)
//            {
//                this.StateChangedOn = DateTime.UtcNow;
//                this.IsChanged = true;
//            }
//            else if(this.IsChanged)
//            {
//                this.IsChanged = false;
//            }
//        }


//        public enum DhtType : short
//        {
//            Dht11 = 11,
//            Dht12 = 12,
//            Dht21 = 21,
//            Dht22 = 22,
//        }
//    }
//}
