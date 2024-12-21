namespace ThingsLibrary.Device.I2c.Base
{
    public interface I2cBaseInterface
    {
        public static abstract int DefaultAddress { get; set; }
        public static abstract int? SecondaryAddress { get; set; }
    }
}
