using Iot.Device.Card.CreditCardProcessing;
using Iot.Device.Ws28xx;
using System.CodeDom;
using System.Drawing;
using System.Threading;

namespace ThingsLibrary.Device.Spi
{
    public enum LedDriver
    {
        Ws2812b,
        Ws2815b,
        Sk6812
    }

    /// <summary>
    /// WS28xx LED driver device
    /// </summary>
    /// <seealso cref="https://github.com/dotnet/iot/blob/main/src/devices/Ws28xx/README.md"/>
    public class NeoPixel : Base.SpiBase
    {
        /// <summary>
        /// LED Driver Chipset
        /// </summary>
        public LedDriver LedDriver { get; init; }

        /// <summary>
        /// Pixel Count
        /// </summary>
        public int PixelCount { get; init; }

        /// <summary>
        /// If the white channel is the alpha channel
        /// </summary>
        public bool SupportsSeparateWhite { get; set; }

        /// <summary>
        /// Device Drivers
        /// </summary>
        private Ws28xx Neo { get; set; }
        //Ws28xx neo = new Ws2812b(spi, Count);
        //Ws28xx neo = new Ws2815b(spi, ledCount);
        //Ws28xx neo = new Sk6812(Spi, ledCount);

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Device Name</param>
        /// <param name="busId">BUS ID</param>
        /// <param name="pixelCount">Pixel Count</param>
        /// <exception cref="ArgumentException"></exception>
        public NeoPixel(string name, SpiDevice spiDevice, LedDriver ledDriver, int pixelCount) : base(name, spiDevice)
        {
            if (pixelCount <= 0) { throw new ArgumentException("Invalid pixel count."); }

            this.LedDriver = ledDriver;
            this.PixelCount = pixelCount;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Device Name</param>
        /// <param name="busId">BUS ID</param>
        /// <param name="pixelCount">Pixel Count</param>
        /// <exception cref="ArgumentException"></exception>
        public NeoPixel(string name, int busId, LedDriver ledDriver, int pixelCount) : base(name, busId)
        {
            if(pixelCount <= 0) { throw new ArgumentException("Invalid pixel count."); }

            this.LedDriver = ledDriver;
            this.PixelCount = pixelCount;
        }

        /// <inheritdoc/>
        public override void Init(bool enableDevice = true)
        {
            this.Neo = this.LedDriver switch
            {
                LedDriver.Ws2812b => new Ws2812b(this.Device, this.PixelCount),
                LedDriver.Ws2815b => new Ws2815b(this.Device, this.PixelCount),
                LedDriver.Sk6812 => new Sk6812(this.Device, this.PixelCount),
                _ => throw new ArgumentException("Invalid LED Driver type.")
            };

            if(this.Neo is Sk6812) { this.SupportsSeparateWhite = true; }

            this.IsEnabled = enableDevice;
        }

        /// <summary>
        /// Set a specific pixel to a color
        /// </summary>
        /// <param name="pixelId">Pixel ID</param>
        /// <param name="color">Color</param>
        public void Set(int pixelId, Color color)
        {
            this.Neo.Image.SetPixel(pixelId, 0, color);
        }

        /// <summary>
        /// Set a specific pixel to a color
        /// </summary>
        /// <param name="pixelId">Pixel ID</param>
        /// <param name="red">Red</param>
        /// <param name="green">Green</param>
        /// <param name="blue">Blue</param>
        public void Set(int pixelId, int red, int green, int blue)
        {
            this.Set(pixelId, this.GetColor(red, green, blue));
        }       

        /// <summary>
        /// Sends the data to the pixels to display
        /// </summary>
        public void Update()
        {
            this.Neo.Update();
            Thread.Sleep(25);
        }

        /// <summary>
        /// Set the same color for a series of pixels
        /// </summary>
        /// <param name="color">Color</param>
        /// <param name="pixelStartId">Start</param>
        /// <param name="pixelEndId">End (default: -1 which is to end)</param>
        public void Fill(Color color, int pixelStartId = 0, int pixelEndId = -1)
        {
            if (pixelEndId < 0) { pixelEndId = this.PixelCount - 1; }

            for (int x = pixelStartId; x <= pixelEndId; x++)
            {
                this.Set(x, color);
            }
        }

        /// <summary>
        /// Set a series of pixels to the same color from Start to End.  If end = -1 then it automatically goes to the end
        /// </summary>
        /// <param name="red">Red</param>
        /// <param name="green">Green</param>
        /// <param name="blue">Blue</param>
        /// <param name="startId">Start Pixel</param>
        /// <param name="endId">End Pixel (default: -1)</param>
        public void Fill(int red, int green, int blue, int startId = 0, int endId = -1)
        {
            var color = this.GetColor(red, green, blue);

            this.Fill(color, startId, endId);
        }       

        private Color GetColor(int red, int green, int blue)
        {
            if (this.SupportsSeparateWhite)
            {
                return Color.FromArgb(0, red, green, blue);
            }
            else
            {
                return Color.FromArgb(red, green, blue);
            }
        }

    }
}
