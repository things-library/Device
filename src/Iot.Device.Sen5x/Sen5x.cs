﻿using System.Buffers.Binary;
using System.Device.I2c;

using Iot.Device.Sen5x.Entities;
using Iot.Device.Sen5x.Extensions;

namespace Iot.Device.Sen5x
{
    /// <summary>
    /// Sen
    /// </summary>
    public class Sen5x : IDisposable
    {        
        /// <summary>
        /// The I2C address as specified in the datasheet.
        /// </summary>
        public const byte DefaultI2cAddress = 0x69;

        public I2cDevice I2cDevice { get; internal set; }

        private bool IsDisposed { get; set; } = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Sen5xSensor"/> class.
        /// </summary>
        /// <param name="i2cDevice">The I2cDevice to which this sensor is connected.</param>
        public Sen5x(I2cDevice i2cDevice)
        {
            this.I2cDevice = i2cDevice ?? throw new ArgumentNullException(nameof(i2cDevice));
        }

        /// <summary>
        /// Starts the measurement. After power up, the module is in Idle-Mode. Before any measurement values can be read, the Measurement-Mode needs to be started using this command.
        /// </summary>
        /// <exception cref="InvalidOperationException">When the I2C operation fails.</exception>
        public void StartMeasurement()
        {
            this.Write(Command.StartMeasurement, TimeSpan.FromMilliseconds(50));
        }

        /// <summary>
        /// Starts a continuous measurement without PM. Only humidity, temperature, VOC, and NOx are available in this mode. Laser and fan are switched off to keep power consumption low.
        /// </summary>
        /// <exception cref="InvalidOperationException">When the I2C operation fails.</exception>
        public void StartMeasurementWithoutPM()
        {
            this.Write(Command.StartMeasurementWithoutPM, TimeSpan.FromMilliseconds(50));
        }

        /// <summary>
        /// Stops the measurement. Use this command to return to the initial state (Idle-Mode).
        /// </summary>
        /// <exception cref="InvalidOperationException">When the I2C operation fails.</exception>
        public void StopMeasurement()
        {
            this.Write(Command.StopMeasurement, TimeSpan.FromMilliseconds(200));
        }

        /// <summary>
        /// This command can be used for polling to find out when new measurements are available.
        /// </summary>
        /// <returns>True when data is ready to be read, false otherwise.</returns>
        /// <exception cref="InvalidOperationException">When the I2C operation fails.</exception>
        public bool ReadDataReadyFlag()
        {
            var data = this.WriteRead(Command.DataReadyFlag, TimeSpan.FromMilliseconds(20), new byte[3]);

            return data[1] > 0;
        }

        /// <summary>
        /// Reads the measured values from the sensor module and resets the "Data-Ready Flag". If the sensor module is in Measurement-Mode, an updated measurement value is provided
        /// every second and the "Data-Ready Flag" is set. If no synchronized readout is desired, the "Data-Ready Flag" can be ignored. The command "Read Measured Values" always returns
        /// the latest measured values. In RHT/Gas-Only Measurement Mode, the PM output is 0xFFFF. If any value is unknown, 0xFFFF is returned.
        /// </summary>
        /// <returns>The measured values.</returns>
        /// <exception cref="InvalidOperationException">When the I2C operation fails.</exception>
        public Measurement ReadMeasurement()
        {
            return (Measurement)this.WriteRead(Command.Measurement, TimeSpan.FromMilliseconds(20), new Measurement());
        }

        /// <summary>
        /// These commands allow to compensate temperature effects of the design-in at customer side by applying a custom temperature offset to the ambient temperature.
        /// </summary>
        /// <returns>The current temperature compensation parameters.</returns>
        /// <exception cref="InvalidOperationException">When the I2C operation fails.</exception>
        public TemperatureCompensationParameters ReadTemperatureCompensationParameters()
        {
            return (TemperatureCompensationParameters)this.WriteRead(Command.TemperatureCompensationParameters, TimeSpan.FromMilliseconds(20), new TemperatureCompensationParameters());
        }

        /// <summary>
        /// These commands allow to compensate temperature effects of the design-in at customer side by applying a custom temperature offset to the ambient temperature.
        /// </summary>
        /// <param name="value">The new temperature compensation parameters to be set.</param>
        /// <exception cref="InvalidOperationException">When the I2C operation fails.</exception>
        public void WriteTemperatureCompensationParameters(TemperatureCompensationParameters value)
        {
            this.Write(Command.TemperatureCompensationParameters, TimeSpan.FromMilliseconds(20), value);
        }

        /// <summary>
        /// The temperature compensation algorithm is optimized for a cold start by default, i.e., it is assumed that the "Start Measurement" commands are called on a device not yet warmed up by previous measurements.
        /// If the measurement is started on a device that is already warmed up, this parameter can be used to improve the initial accuracy of the ambient temperature output. This parameter can be gotten and
        /// set in any state of the device, but it is applied only the next time starting a measurement, i.e., when sending a "Start Measurement" command.So, the parameter needs to be written before a warm-start
        /// measurement is started.
        /// </summary>
        /// <returns>The warm start parameter.</returns>
        /// <exception cref="InvalidOperationException">When the I2C operation fails.</exception>
        public WarmStartParameters ReadWarmStartParameter()
        {
            return (WarmStartParameters)this.WriteRead(Command.WarmStartParameters, TimeSpan.FromMilliseconds(20), new WarmStartParameters());
        }

        /// <summary>
        /// The temperature compensation algorithm is optimized for a cold start by default, i.e., it is assumed that the "Start Measurement" commands are called on a device not yet warmed up by previous measurements.
        /// If the measurement is started on a device that is already warmed up, this parameter can be used to improve the initial accuracy of the ambient temperature output. This parameter can be gotten and
        /// set in any state of the device, but it is applied only the next time starting a measurement, i.e., when sending a "Start Measurement" command.So, the parameter needs to be written before a warm-start
        /// measurement is started.
        /// </summary>
        /// <param name="value">The new warm start parameter to be set.</param>
        /// <exception cref="InvalidOperationException">When the I2C operation fails.</exception>
        public void WriteWarmStartParameter(WarmStartParameters value)
        {
            this.Write(Command.WarmStartParameters, TimeSpan.FromMilliseconds(20), value);
        }

        /// <summary>
        /// The VOC algorithm can be customized by tuning 6 different parameters. Note that this command is available only in idle mode. In measure mode, this command has no effect. In addition, it has no effect if at least one parameter is outside the specified range.
        /// </summary>
        /// <returns>The algorithm tuning parametes for VOC.</returns>
        /// <exception cref="InvalidOperationException">When the I2C operation fails.</exception>
        public AlgorithmTuningParameters ReadVocAlgorithmTuningParameters()
        {
            return (AlgorithmTuningParameters)this.WriteRead(Command.VocAlgorithmTuningParameters, TimeSpan.FromMilliseconds(20), new AlgorithmTuningParameters());
        }

        /// <summary>
        /// The VOC algorithm can be customized by tuning 6 different parameters. Note that this command is available only in idle mode. In measure mode, this command has no effect. In addition, it has no effect if at least one parameter is outside the specified range.
        /// </summary>
        /// <param name="value">The new algorithm tuning parameters for VOC.</param>
        /// <exception cref="InvalidOperationException">When the I2C operation fails.</exception>
        public void WriteVocAlgorithmTuningParameters(AlgorithmTuningParameters value)
        {
            this.Write(Command.VocAlgorithmTuningParameters, TimeSpan.FromMilliseconds(20), value);
        }

        /// <summary>
        /// The NOx algorithm can be customized by tuning 6 different parameters. Note that this command is available only in idle mode. In measure mode, this command has no effect. In addition, it has no effect if at least one parameter is outside the specified range.
        /// </summary>
        /// <returns>The algorithm tuning parametes for NOx.</returns>
        /// <exception cref="InvalidOperationException">When the I2C operation fails.</exception>
        public AlgorithmTuningParameters ReadNoxAlgorithmTuningParameters()
        {
            return (AlgorithmTuningParameters)this.WriteRead(Command.NoxAlgorithmTuningParameters, TimeSpan.FromMilliseconds(20), new AlgorithmTuningParameters());
        }

        /// <summary>
        /// The NOx algorithm can be customized by tuning 6 different parameters. Note that this command is available only in idle mode. In measure mode, this command has no effect. In addition, it has no effect if at least one parameter is outside the specified range.
        /// </summary>
        /// <param name="value">The new algorithm tuning parameters for NOx.</param>
        /// <exception cref="InvalidOperationException">When the I2C operation fails.</exception>
        public void WriteNoxAlgorithmTuningParameters(AlgorithmTuningParameters value)
        {
            this.Write(Command.NoxAlgorithmTuningParameters, TimeSpan.FromMilliseconds(20), value);
        }

        /// <summary>
        /// By default, the RH/T acceleration algorithm is optimized for a sensor which is positioned in free air. If the sensor is integrated into another
        /// device, the ambient RH/T output values might not be optimal due to different thermal behavior. This parameter can be used to read the RH/T
        /// acceleration behavior for the actual use-case, leading in an improvement of the ambient RH/T output accuracy.
        /// </summary>
        /// <returns>The parameters for the RH/T acceleration mode.</returns>
        /// <exception cref="InvalidOperationException">When the I2C operation fails.</exception>
        public RhtAccelerationModeParameters ReadRhtAccelerationMode()
        {
            return (RhtAccelerationModeParameters)this.WriteRead(Command.RhtAccelerationModeParameters, TimeSpan.FromMilliseconds(20), new RhtAccelerationModeParameters());
        }

        /// <summary>
        /// By default, the RH/T acceleration algorithm is optimized for a sensor which is positioned in free air. If the sensor is integrated into another
        /// device, the ambient RH/T output values might not be optimal due to different thermal behavior. This parameter can be used to adapt the RH/T
        /// acceleration behavior for the actual use-case, leading in an improvement of the ambient RH/T output accuracy.
        /// </summary>
        /// <param name="value">The new RH/T acceleration mode parameters.</param>
        /// <exception cref="InvalidOperationException">When the I2C operation fails.</exception>
        public void WriteRhtAccelerationMode(RhtAccelerationModeParameters value)
        {
            this.Write(Command.RhtAccelerationModeParameters, TimeSpan.FromMilliseconds(20), value);
        }

        /// <summary>
        /// Allows to backup and restore the VOC algorithm state to resume operation after a short interruption, skipping initial learning phase. By default,
        /// the VOC algorithm resets its state to initial values each time a measurement is started, even if the measurement was stopped only for a short time.
        /// So, the VOC index output value needs a long time until it is stable again. This can be avoided by restoring the previously memorized algorithm
        /// state before starting the measure mode.
        /// </summary>
        /// <returns>The current VOC algorithm state.</returns>
        /// <exception cref="InvalidOperationException">When the I2C operation fails.</exception>
        public VocAlgorithmState ReadVocAlgorithmState()
        {
            return (VocAlgorithmState)this.WriteRead(Command.VocAlgorithmState, TimeSpan.FromMilliseconds(20), new VocAlgorithmState());
        }

        /// <summary>
        /// Allows to backup and restore the VOC algorithm state to resume operation after a short interruption, skipping initial learning phase. By default,
        /// the VOC algorithm resets its state to initial values each time a measurement is started, even if the measurement was stopped only for a short time.
        /// So, the VOC index output value needs a long time until it is stable again. This can be avoided by restoring the previously memorized algorithm
        /// state before starting the measure mode.
        /// </summary>
        /// <param name="value">The new VOC algorithm state to write.</param>
        /// <exception cref="InvalidOperationException">When the I2C operation fails.</exception>
        public void WriteVocAlgorithmState(VocAlgorithmState value)
        {
            this.Write(Command.VocAlgorithmState, TimeSpan.FromMilliseconds(20), value);
        }

        /// <summary>
        /// Starts the fan-cleaning manually. This command can only be executed in Measurement-Mode.
        /// </summary>
        /// <exception cref="InvalidOperationException">When the I2C operation fails.</exception>
        public void StartFanCleaning()
        {
            this.Write(Command.StartFanCleaning, TimeSpan.FromMilliseconds(20));
        }

        /// <summary>
        /// Reads the interval [s] of the periodic fan-cleaning.
        /// </summary>
        /// <returns>The current auto cleaning interval parameters.</returns>
        /// <exception cref="InvalidOperationException">When the I2C operation fails.</exception>
        public AutoCleaningIntervalParameters ReadAutoCleaningInterval()
        {
            return (AutoCleaningIntervalParameters)this.WriteRead(Command.AutoCleaningIntervalParameters, TimeSpan.FromMilliseconds(20), new AutoCleaningIntervalParameters());
        }

        /// <summary>
        /// Writes the interval [s] of the periodic fan-cleaning. Please note that since this configuration is volatile, it will be reverted to the default value after a device reset.
        /// </summary>
        /// <param name="value">The new auto cleaning interval to be set.</param>
        /// <exception cref="InvalidOperationException">When the I2C operation fails.</exception>
        public void WriteAutoCleaningInterval(AutoCleaningIntervalParameters value)
        {
            this.Write(Command.AutoCleaningIntervalParameters, TimeSpan.FromMilliseconds(20), value);
        }

        /// <summary>
        /// This command returns the product name SEN5x (SEN50, SEN54 or SEN55).
        /// </summary>
        /// <returns>The product name SEN5x (SEN50, SEN54 or SEN55).</returns>
        /// <exception cref="InvalidOperationException">When the I2C operation fails.</exception>
        public string ReadProductName()
        {
            return ((StringValue)this.WriteRead(Command.ProductName, TimeSpan.FromMilliseconds(20), new StringValue(32))).Text;
        }

        /// <summary>
        /// This command returns the requested serial number.
        /// </summary>
        /// <returns>The serial number.</returns>
        /// <exception cref="InvalidOperationException">When the I2C operation fails.</exception>
        public string ReadSerialNumber()
        {
            return ((StringValue)this.WriteRead(Command.SerialNumber, TimeSpan.FromMilliseconds(20), new StringValue(32))).Text;
        }

        /// <summary>
        /// Get firmware version. There is no Major/Minor, only a single value.
        /// </summary>
        /// <returns>The firmware version.</returns>
        /// <exception cref="InvalidOperationException">When the I2C operation fails.</exception>
        public byte ReadFirmwareVersion()
        {
            var data = this.WriteRead(Command.FirmwareVersion, TimeSpan.FromMilliseconds(20), new byte[3]);
            return data[0];
        }

        /// <summary>
        /// Use this command to read the Device Status Register.
        /// </summary>
        /// <returns>The device status.</returns>
        /// <exception cref="InvalidOperationException">When the I2C operation fails.</exception>
        public DeviceStatus ReadDeviceStatus()
        {
            return (DeviceStatus)this.WriteRead(Command.ReadDeviceStatus, TimeSpan.FromMilliseconds(20), new DeviceStatus());
        }

        /// <summary>
        /// Clears all flags in device status register.
        /// </summary>
        /// <exception cref="InvalidOperationException">When the I2C operation fails.</exception>
        public void ClearDeviceStatus()
        {
            this.Write(Command.ClearDeviceStatus, TimeSpan.FromMilliseconds(20));
        }

        /// <summary>
        /// Device software reset command. After calling this command, the module is in the same state as after a power reset.
        /// </summary>
        /// <exception cref="InvalidOperationException">When the I2C operation fails.</exception>
        public void DeviceReset()
        {
            this.Write(Command.DeviceReset, TimeSpan.FromMilliseconds(100));
        }

        private void Write(Command cmd, TimeSpan commandExecutionTime)
        {
            Span<Byte> data = new byte[2];
            BinaryPrimitives.WriteUInt16BigEndian(data, (ushort)cmd);
            this.I2cDevice.Write(data);

            Thread.Sleep(commandExecutionTime);            
        }

        private AbstractReadEntity WriteRead(Command cmd, TimeSpan commandExecutionTime, AbstractReadEntity entity)
        {
            Span<Byte> data = new byte[2];
            BinaryPrimitives.WriteUInt16BigEndian(data, (ushort)cmd);
            this.I2cDevice.Write(data);
            
            Thread.Sleep(commandExecutionTime);
            Span<Byte> response = new byte[entity.ByteCount];
            this.I2cDevice.Read(response);                

            response.VerifyCrc();
            entity.FromSpanByte(response);

            return entity;
        }

        private Span<Byte> WriteRead(Command cmd, TimeSpan commandExecutionTime, Span<Byte> response)
        {
            if (response.Length == 0)
            {
                throw new ArgumentException("Response object has no memory space provided.");
            }

            Span<Byte> data = new byte[2];
            BinaryPrimitives.WriteUInt16BigEndian(data, (ushort)cmd);
            
            this.I2cDevice.Write(data);
            
            Thread.Sleep(commandExecutionTime);
            
            this.I2cDevice.Read(response);
                
            response.VerifyCrc();            

            return response;
        }

        private void Write(Command cmd, TimeSpan commandExecutionTime, AbstractReadWriteEntity entity)
        {
            Span<Byte> data = new byte[2 + entity.ByteCount];
            BinaryPrimitives.WriteUInt16BigEndian(data, (ushort)cmd);
            
            Span<Byte> entityPart = data.Slice(2);
            entity.ToSpanByte(entityPart);
            entityPart.UpdateCrc();
            this.I2cDevice.Write(data);
            
            Thread.Sleep(commandExecutionTime);
        }

        /// <summary>
        /// Cleanup resources.
        /// </summary>        
        public void Dispose()
        {
            if (this.IsDisposed) { return; }

            if (this.I2cDevice != null)
            {
                this.I2cDevice.Dispose();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                this.I2cDevice = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }

            GC.SuppressFinalize(this);

            this.IsDisposed = true;
        }
    }
}
