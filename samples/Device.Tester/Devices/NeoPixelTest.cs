using Device.Tester.Devices.Base;
using ThingsLibrary.Device.Spi;
using System.Drawing;

namespace Device.Tester.Devices
{
    public class NeoPixelTest : ITestDevice
    {
        public NeoPixel NeoPixel { get; set; }

        public NeoPixelTest(SpiDevice spiDevice) 
        {
            this.NeoPixel = new NeoPixel("Test Device", spiDevice, LedDriver.Ws2812b, 8);
        }

        public void Init()
        {
            this.NeoPixel.Init();
        }

        public void Loop()
        {
            this.NeoPixel.Fill(Color.Red);
            this.NeoPixel.Update();
        }
    }    
}
