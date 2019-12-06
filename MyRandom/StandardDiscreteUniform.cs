using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SCU = MyRandom.StandardContinuousUniform;
using SDU = MyRandom.StandardDiscreteUniform;

namespace MyRandom
{
    public class StandardDiscreteUniform : IDiscreteDistribution<int>
    {
        private StandardDiscreteUniform(int min, int max)
        {
            Min = min;
            Max = max;
        }

        public int Min { get; }
        public int Max { get; }

        public int Sample()
        {
            return (int) (SCU.Distribution.Sample() * (1.0 + Max - Min) + Min);
        }

        public IEnumerable<int> Support()
        {
            return Enumerable.Range(Min, 1 + Max - Min);
        }

        public int Weight(int t)
        {
            return Min <= t && t <= Max ? 1 : 0;
        }

        [NotNull]
        public static StandardDiscreteUniform Distribution(
            int min, int max)
        {
            if (min > max)
                throw new ArgumentException();
            return new StandardDiscreteUniform(min, max);
        }

        public override string ToString()
        {
            return $"StandardDiscreteUniform[{Min}, {Max}]";
        }
    }
}