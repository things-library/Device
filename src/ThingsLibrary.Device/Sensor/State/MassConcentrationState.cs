namespace ThingsLibrary.Device.Sensor.State
{
    public class MassConcentrationState : SensorState
    {        
        public MassConcentration ParticleCount { get; set; }

        public override double Value => this.ParticleCount.Value; //pass through
                
        public override string ValueString() => $"{this.Value.ToString($"n{this.ValuePrecision}")} {this.UnitSymbol}";

        public void Update(MassConcentration? measurement, DateTimeOffset updatedOn)
        {
            // nothing to do
            if (measurement is null) { return; }

            this.ParticleCount = measurement.Value;
            this.UpdatedOn = updatedOn;
        }

        public MassConcentrationState(string id = "Particles", string key = "parts", bool isImperial = false) : base(id, key, isImperial)
        {
            //particles are particles
            this.UnitSymbol = "ppm";
            this.ValuePrecision = 0;            
        }
    }
}
