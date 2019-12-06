using JetBrains.Annotations;

namespace MyRandom
{
    public sealed class StandardContinuousUniform : IDistribution<double>
    {
        [NotNull] public static readonly StandardContinuousUniform Distribution = new StandardContinuousUniform();

        private StandardContinuousUniform()
        {
        }

        [MustUseReturnValue]
        public double Sample() => Pseudorandom.NextDouble();
    }
}