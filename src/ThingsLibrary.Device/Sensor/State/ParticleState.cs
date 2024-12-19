namespace ThingsLibrary.Device.Sensor.State
{
    public class ParticleState : SensorState
    {   
        /// <inheritdoc />
        public void Update(double? measurement, DateTimeOffset updatedOn)
        {
            // nothing to do?
            if (this.IsDisabled) { return; }
            if (measurement is null) { return; }

            base.Update(measurement.Value, updatedOn);
        }

        public ParticleState(string id = "Particles", string key = "parts", bool isImperial = false) : base(id, key, isImperial)
        {
            //particles are particles
            this.UnitSymbol = "ppm";
            this.ValuePrecision = 0;
        }
    }
}
