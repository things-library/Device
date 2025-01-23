namespace ThingsLibrary.Device.I2c
{
    public class GpioExpanderPin
    {
        public int PinNumber { get; private set; }
        public GpioExpander Driver { get; private set; }

        public GpioExpanderPin(int pinNumber, GpioExpander driver) : base()
        {
            this.PinNumber = pinNumber;
            this.Driver = driver;
        }

        public PinValue Read() => this.Driver.Read(this.PinNumber);
        public void Toggle() => this.Driver.Toggle(this.PinNumber);
        public void Write(PinValue value) => this.Driver.Write(this.PinNumber, value);
        public void SetPinMode(PinMode mode) => this.Driver.SetPinMode(this.PinNumber, mode);
        public void SetPinMode(PinMode mode, PinValue value) => this.Driver.SetPinMode(this.PinNumber, mode, value);


        /// <summary>
        /// Occurs when the value of the general-purpose I/O (GPIO) pin changes, either because of an external stimulus when the pin is configured as an input, or when a value is written to the pin when the pin in configured as an output.
        /// </summary>
        public virtual event PinChangeEventHandler ValueChanged
        {
            add
            {
                this.Driver.AddCallback(this.PinNumber, value);
            }

            remove
            {
                this.Driver.RemoveCallback(this.PinNumber, value);
            }
        }
    }
}
