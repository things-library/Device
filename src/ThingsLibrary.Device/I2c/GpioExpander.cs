using Iot.Device.Mcp23xxx;
using System.Device.Gpio;

namespace ThingsLibrary.Device.I2c
{
    public class GpioExpander : Mcp23x1x
    {        
        public GpioPin InterruptA { get; private set; }
        public GpioPin InterruptB { get; private set; }

        /// <summary>
        /// Contains the last fetch of GPIO states
        /// </summary>
        public ushort LastGpioStates { get; private set; }
        
        /// <summary>
        /// Last GPIO State Changes - contains a bit mask of which bits changed during the last fetch 
        /// </summary>
        public ushort LastGpioStateChanges { get; private set; }

        /// <summary>
        /// Registered PIN change handlers stored in pin specific 
        /// </summary>
        public PinChangeEventHandler[] Handlers { get; set; } = new PinChangeEventHandler[16];


        public string ErrorMessage { get; set; }

        /// <inheritdoc/>
        /// <remarks>0x20 is default (A0 = +1, A1 = +2, A2 = +4). When all pins are on the address is 0x20 it seems to be reverse.</remarks>
        public GpioExpander(I2cDevice i2cDevice) : base(new I2cAdapter(i2cDevice), -1, -1, -1, null)
        {            
            // no handlers should be attached yet at this point.. get the current cache
            _ = this.FetchStates();
        }

        #region --- Interrupts ---

        /// <summary>
        /// Try to add the interrupts
        /// </summary>
        /// <param name="interruptA">Interrupt for Port A (or Both)</param>
        /// <param name="interruptB">Interrupt for Port B</param>
        public void AttachInterrupts(GpioPin interruptA, GpioPin interruptB, bool isMirror)
        {
            try
            {
                this.InterruptA = interruptA;
                this.InterruptB = interruptB;

                if (this.InterruptA != null)
                {    
                    this.InterruptA.ValueChanged += this.InterruptA_ValueChanged;
                }

                if (this.InterruptB != null)
                {                    
                    this.InterruptB.ValueChanged += this.InterruptB_ValueChanged;
                }                
            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.Message;
            }

            //IOCON.MIRROR = bit 6 = mirrors the Interrupt pins (connects them internally)
            if (isMirror)
            {
                // get the current state so we don't false flag 
                var config = this.ReadUInt16(Register.IOCON);

                // change bit 6 (IOCON.MIRROR) to true for both Port.A and Port.B                
                config |= 0b0100_0000_0100_00;                

                this.WriteUInt16(Register.IOCON, config);
            }            
        }

        private void InterruptA_ValueChanged(object sender, PinValueChangedEventArgs e)
        {
            //TODO: only fetch the A states
            this.FetchStates();
        }

        private void InterruptB_ValueChanged(object sender, PinValueChangedEventArgs e)
        {
            //TODO: only fetch the B states?
            this.FetchStates();
        }

        #endregion

        #region --- Basic GPIO ---

        public new PinValue Read(int pinNumber) => base.Read(pinNumber);
        public new void Write(int pinNumber, PinValue value) => base.Write(pinNumber, value);
        public new void Toggle(int pinNumber) => base.Toggle(pinNumber);

        public new void SetPinMode(int pinNumber, PinMode mode) => base.SetPinMode(pinNumber, mode);
        public new void SetPinMode(int pinNumber, PinMode mode, PinValue initialValue) => base.SetPinMode(pinNumber, mode, initialValue);

        public GpioExpanderPin OpenPin(int pinNumber, PinMode mode)
        {
            var pin = new GpioExpanderPin(pinNumber, this);
            
            pin.SetPinMode(mode);

            return pin;
        }

        public GpioExpanderPin OpenPin(int pinNumber, PinMode mode, PinValue value)
        {
            var pin = this.OpenPin(pinNumber, mode);

            pin.Write(value);

            return pin;
        }

        #endregion

        /// <summary>
        /// Fetch the current states and fire off any event handers for pins that have a handler and a change detected
        /// </summary>
        /// <returns>true when any change is detected in the GPIO pins</returns>
        public bool FetchStates()
        {
            var interruptPin = this.InterruptA?.Read();
            Console.WriteLine($"Pre-Read Interrupt: {interruptPin}");

            // get the current state so we don't false flag 
            var states = this.ReadUInt16(Register.GPIO);

            // XOR operation to find the bits that changed
            var changes = (ushort)(this.LastGpioStates ^ states);

            if (changes == 0) { return false; }

            //Console.WriteLine($"Changes: {Convert.ToString(changes, 2).PadLeft(16, '0')}");

            // see which bits have handlers attached
            for (int pin = 0; pin < 16; pin++)
            {
                // no change occured?
                if ((changes & (1 << pin)) == 0) { continue; }

                // no handler listening?
                if (this.Handlers[pin] == null) { continue; }

                // figure out what sort of change we are dealing with                
                var pinChangeType = ((states & (1 << pin)) != 0) ? PinEventTypes.Rising : PinEventTypes.Falling;

                // invoke the handler
                this.Handlers[pin].Invoke(this, new PinValueChangedEventArgs(pinChangeType, pin));
            }

            // keep track of current states and changes since last time in case we want to loop through ourselves
            this.LastGpioStates = states;
            this.LastGpioStateChanges = changes;
                        

            if (Debugger.IsAttached)
            {
                interruptPin = this.InterruptA?.Read();

                Console.WriteLine($"State: {Convert.ToString(states, 2).PadLeft(this.PinCount, '0')}, Delta: {Convert.ToString(changes, 2).PadLeft(this.PinCount, '0')}, Interrupt: {interruptPin}");
            }           

            // something did change
            return true;
        }

        /// <summary>
        /// Add callback handler for Pin
        /// </summary>
        /// <param name="pinNumber">GPIO PIN</param>
        /// <param name="callback">Callback handler</param>
        /// <remarks>The interrupt signals are configured as active-low.</remarks>
        /// <exception cref="ArgumentException"></exception>
        public void AddCallback(int pinNumber, PinChangeEventHandler callback)
        {
            if (pinNumber < 0 || pinNumber >= this.Handlers.Length) { throw new ArgumentException("Invalid Pin."); }

            this.EnableInterruptOnChange(pinNumber, PinEventTypes.Rising | PinEventTypes.Falling);

            this.Handlers[pinNumber] = callback;
        }

        /// <summary>
        /// Remove callback handler for Pin
        /// </summary>
        /// <param name="pinNumber">PIN</param>
        /// <exception cref="ArgumentException"></exception>
        public void RemoveCallback(int pinNumber, PinChangeEventHandler callback)
        {
            if (pinNumber < 0 || pinNumber >= this.Handlers.Length) { throw new ArgumentException("Invalid Pin."); }

            this.DisableInterruptOnChange(pinNumber);

            this.Handlers[pinNumber] = null;
        }
    }
}
