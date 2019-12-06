using System.Linq;
using JetBrains.Annotations;

namespace MyRandom
{
    public sealed class IrwinHall : IDistribution<double>
    {
        private IrwinHall(int n)
        {
            N = n;
        }

        public int N { get; }

        [MustUseReturnValue]
        public double Sample()
        {
            return StandardContinuousUniform.Distribution.Samples().Take(N).Sum();
        }

        [NotNull]
        
        public static IrwinHall Distribution(int n)
        {
            return new IrwinHall(n);
        }
    }
}