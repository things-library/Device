﻿using Iot.Device.Pmsx003;
using ThingsLibrary.Device.Sensor.Interfaces;

namespace ThingsLibrary.Device.I2c
{
    //https://learn.adafruit.com/pmsa003i

    public class Pmsx003Sensor : Base.I2cSensor
    {
        // Part Number Definition:   (Example: PMSA003 = PMS {A0} {03})
        // PMS - sensor type
        // ## = Model/Version
        // ## - Min distinguishable particle diameter 


        /// <summary>
        /// Direct Device Object
        /// </summary>
        public Pmsx003 Device { get; set; }
                
        /// <summary>
        /// PM1.0 concentration unit: µg/𝑚3 (standard particle)
        /// </summary>
        public MassConcentrationState StandardPm10 { get; init; }

        /// <summary>
        /// PM2.5 concentration unit: µg/𝑚3 (standard particle)
        /// </summary>
        public MassConcentrationState StandardPm25 { get; init; }

        /// <summary>
        /// PM10.0 concentration unit: µg/𝑚3 (standard particle)
        /// </summary>
        public MassConcentrationState StandardPm100 { get; init; }


        // ======================================================================
        // Atmospheric Environment
        // ======================================================================
        // NOTE: Atospheric values is the raw data as it is now (whatever temperature and pressure there is currently)
        // Air being a gas is compressible which means that it changes its volume when the pressure changes so when
        // you report concentrations as mass per volume of air it is relevant at what pressure that volume is calculated.

        /// <summary>
        /// PM1.0 concentration unit：µg/𝑚3 (under atmospheric environment)
        /// </summary>
        public MassConcentrationState EnvironmentPm10 { get; init; }

        /// <summary>
        /// PM2.5 concentration unit：µg/𝑚3 (under atmospheric environment)
        /// </summary>
        public MassConcentrationState EnvironmentPm25 { get; init; }

        /// <summary>
        /// PM10.0 concentration unit：µg/𝑚3 (under atmospheric environment)
        /// </summary>
        public MassConcentrationState EnvironmentPm100 { get; init; }


        // ======================================================================
        // Particle Concentrations (Number of particles with diameter beyond xx.x µ𝑚 in 0.1L of air)
        // ======================================================================

        /// <summary>
        /// Number of particles with diameter beyond 0.3 µ𝑚 in 0.1L of air
        /// </summary>
        public ParticleState Particles03 { get; init; }

        /// <summary>
        /// Number of particles with diameter beyond 0.5 µ𝑚 in 0.1L of air
        /// </summary>
        public ParticleState Particles05 { get; init; }

        /// <summary>
        /// Number of particles with diameter beyond 1.0 µ𝑚 in 0.1L of air
        /// </summary>
        public ParticleState Particles10 { get; init; }

        /// <summary>
        /// Number of particles with diameter beyond 2.5 µ𝑚 in 0.1L of air
        /// </summary>
        public ParticleState Particles25 { get; init; }

        /// <summary>
        /// Number of particles with diameter beyond 5.0 µ𝑚 in 0.1L of air
        /// </summary>
        public ParticleState Particles50 { get; init; }

        /// <summary>
        /// Number of particles with diameter beyond 10.0 µ𝑚 in 0.1L of air
        /// </summary>
        public ParticleState Particles100 { get; init; }


        public Pmsx003Sensor(I2cBus i2cBus, int id = Pmsx003.DefaultI2cAddress, string name = "pmsx003", bool isImperial = false) : base(i2cBus, id, name, isImperial)
        {
            // States
            this.States = new List<ISensorState>(12)
            {   
                // Standard Concentration States
                {   this.StandardPm10 = new MassConcentrationState("Standard PM 1.0", "pm3_std", isImperial: isImperial) { UnitSymbol = "mcg/m3" } },
                {   this.StandardPm25 = new MassConcentrationState("Standard PM 2.5", "pm2_5_std", isImperial: isImperial) { UnitSymbol = "mcg/m3" } },
                {   this.StandardPm100 = new MassConcentrationState("Standard PM 10.0", "pm10_std", isImperial: isImperial) { UnitSymbol = "mcg/m3" } },
                
                // Environment States 
                {   this.EnvironmentPm10 = new MassConcentrationState("PM 1.0", "pm1", isImperial: isImperial) { UnitSymbol = "mcg/m3" } },
                {   this.EnvironmentPm25 = new MassConcentrationState("PM 2.5", "pm2_5", isImperial: isImperial) { UnitSymbol = "mcg/m3" } },
                {   this.EnvironmentPm100 = new MassConcentrationState("PM 10.0", "pm10", isImperial: isImperial) { UnitSymbol = "mcg/m3" } },
                
                // Particle Counts
                {   this.Particles03 = new ParticleState("Particles > 0.3 µm", "pm0_3ct", isImperial: isImperial) { UnitSymbol = "/0.1L" } },
                {   this.Particles05 = new ParticleState("Particles > 0.5 µm", "pm0_5ct", isImperial: isImperial) { UnitSymbol = "/0.1L" } },
                {   this.Particles10 = new ParticleState("Particles > 1.0 µm", "pm1ct", isImperial: isImperial) { UnitSymbol = "/0.1L" } },
                {   this.Particles25 = new ParticleState("Particles > 2.5 µm", "pm2_5ct", isImperial: isImperial) { UnitSymbol = "/0.1L" } },
                {   this.Particles50 = new ParticleState("Particles > 5.0 µm", "pm5ct", isImperial: isImperial) { UnitSymbol = "/0.1L" } },
                {   this.Particles100 = new ParticleState("Particles > 10.0 µm", "pm10ct", isImperial: isImperial) { UnitSymbol = "/0.1L" } }
            };
        }

        public override void Init()
        {
            try
            {
                base.Init();

                this.Device = new Pmsx003(this.I2cDevice);

                //this.MinReadInterval = (int)Scd4x.MeasurementPeriod.TotalMilliseconds;
                //if (this.ReadInterval < this.MinReadInterval) { throw new ArgumentException($"Read interval '{this.ReadInterval} ms' can not be less then min read interval '{this.MinReadInterval} ms' of sensor."); }
                       

                //TODO:


                this.IsEnabled = true;
            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.Message;
            }
        }

        public override bool FetchStates()
        {
            if (!this.IsEnabled) { return false; }
            if (DateTimeOffset.UtcNow < this.NextReadOn) { return false; }

            try
            {
                var readResult = this.Device.Read();
                if (readResult == null) { return false; }

                this.UpdatedOn = DateTimeOffset.UtcNow;

                this.StandardPm10.Update(readResult.StandardPm10, this.UpdatedOn);
                this.StandardPm25.Update(readResult.StandardPm25, this.UpdatedOn);
                this.StandardPm100.Update(readResult.StandardPm100, this.UpdatedOn);

                this.EnvironmentPm10.Update(readResult.EnvironmentPm10, this.UpdatedOn);
                this.EnvironmentPm25.Update(readResult.EnvironmentPm25, this.UpdatedOn);
                this.EnvironmentPm100.Update(readResult.EnvironmentPm100, this.UpdatedOn);

                this.Particles03.Update(readResult.Particles03, this.UpdatedOn);
                this.Particles05.Update(readResult.Particles03, this.UpdatedOn);
                this.Particles10.Update(readResult.Particles10, this.UpdatedOn);
                this.Particles25.Update(readResult.Particles25, this.UpdatedOn);
                this.Particles50.Update(readResult.Particles50, this.UpdatedOn);
                this.Particles100.Update(readResult.Particles100, this.UpdatedOn);

                // see if anyone is listening
                this.StatesChanged?.Invoke(this, this.States);

                // if we get here the state has changed
                return true;
            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.Message;
                return false;
            }
        }     
    }    
}
