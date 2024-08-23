namespace Device.Tester.Devices.Base
{
    public interface ITestDevice
    {
        public abstract void Init();

        public abstract void Loop();
    }
}
