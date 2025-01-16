using Iot.Device.Mcp23xxx;
using ThingsLibrary.Device.Gpio;

namespace ThingsLibrary.Device.I2c
{
    public class Mcp230xx : Base.I2cBase
    {
        public Mcp23017 Device { get; set; }
        public GpioController GpioController { get; set; }

        public string ErrorMessage { get; set; }

        //public GpioController GpioController { get; set; }
        public BoolSensor InterruptA { get; set; }
        public BoolSensor InterruptB { get; set; }

        /// <inheritdoc/>
        /// <remarks>0x20 is default (A0 = +1, A1 = +2, A2 = +4). When all pins are on the address is 0x20 it seems to be reverse.</remarks>
        public Mcp230xx(I2cBus i2cBus, int id = 0x20, string name = "mcp23xxx") : base(i2cBus, id, name)
        {
            
        }

        public void SetInterrupts(GpioController baseController, int interruptAPin, int interruptBPin)
        {
            ArgumentNullException.ThrowIfNull(baseController);

            if (interruptAPin > 0)
            {
                this.InterruptA = new BoolSensor(baseController, (ushort)interruptAPin, "Interrupt A", null);
            }

            if (interruptBPin > 0)
            {
                this.InterruptB = new BoolSensor(baseController, (ushort)interruptBPin, "Interrupt B", null);
            }
        } 

        public void SetInterrupts(BoolSensor interruptA, BoolSensor interruptB)
        {
            if(interruptA != null)
            {
                this.InterruptA = interruptA;
            }

            if(interruptB != null)
            {
                this.InterruptB = interruptB;
            }
        }

        public override void Init()
        {
            try
            {
                base.Init();
                
                this.Device = new Mcp23017(this.I2cDevice , -1, this.InterruptA?.Id ?? -1, this.InterruptB?.Id ?? -1, this.GpioController);
                //this.Device.Enable();

                this.GpioController = new GpioController(PinNumberingScheme.Logical, this.Device);

                // we must enable for this device to work at all.
                this.IsEnabled = true;
            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.Message;
            }
        }

        //public override bool FetchStates()
        //{
        //    if (!this.IsEnabled) { return false; }
        //    if (DateTimeOffset.UtcNow < this.NextReadOn) { return false; }

        //    try
        //    {
        //        return true; //TODO:
        //    }
        //    catch (Exception ex)
        //    {
        //        this.ErrorMessage = ex.Message;
                
        //        return false;
        //    }
        //}
    }
}
