using JetBrains.Annotations;
using static System.Math;
using SCU = MyRandom.StandardContinuousUniform;

namespace MyRandom
{
    public sealed class Normal : IDistribution<double>
    {
        [PublicAPI] public static readonly Normal Standard = Distribution(0, 1);

        private Normal(double mean, double sigma)
        {
            Mean = mean;
            Sigma = sigma;
        }

        public double Mean  { get; }
        public double Sigma { get; }
        public double μ     => Mean;
        public double σ     => Sigma;

        
        public double Sample()
        {
            return μ + σ * StandardSample();
        }

        [NotNull]
        public static Normal Distribution(double mean, double sigma)
        {
            return new Normal(mean, sigma);
        }

        
        // Box-Muller method
        private double StandardSample()
        {
            return Sqrt(-2.0 * Log(SCU.Distribution.Sample())) * Cos(2.0 * PI * SCU.Distribution.Sample());
        }
    }
}