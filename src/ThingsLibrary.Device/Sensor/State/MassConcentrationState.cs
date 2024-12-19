﻿namespace ThingsLibrary.Device.Sensor.State
{
    public class MassConcentrationState : SensorState
    {        
        /// <summary>
        /// Particle Count
        /// </summary>
        public MassConcentration ParticleCount { get; set; }

        /// <inheritdoc />
        public void Update(MassConcentration? measurement, DateTimeOffset updatedOn)
        {
            // nothing to do?
            if (this.IsDisabled) { return; }
            if (measurement is null) { return; }

            this.ParticleCount = measurement.Value;
            this.Update(measurement.Value, updatedOn);
        }


        public MassConcentrationState(string id = "Particles", string key = "parts", bool isImperial = false) : base(id, key, isImperial)
        {
            //particles are particles
            this.UnitSymbol = "ppm";
            this.ValuePrecision = 0;            
        }
    }
}
