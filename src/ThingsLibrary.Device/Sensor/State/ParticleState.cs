namespace ThingsLibrary.Device.Sensor.State
{
    public class ParticleState : SensorState
    {        
        public double ParticleCount { get; set; }

        public override double Value => this.ParticleCount; //pass through
                
        public override string ValueString() => $"{this.Value.ToString($"n{this.ValuePrecision}")} {this.UnitSymbol}";

        public void Update(double? measurement, DateTimeOffset updatedOn)
        {
            // nothing to do.. trying to keep the code clean
            if (measurement is null) { return; }

            this.ParticleCount = measurement.Value;
            this.UpdatedOn = updatedOn;
        }

        public ParticleState(string id = "Particles", string key = "parts", bool isImperial = false) : base(id, key, isImperial)
        {
            //particles are particles
            this.UnitSymbol = "ppm";
            this.ValuePrecision = 0;
        }
    }
}
