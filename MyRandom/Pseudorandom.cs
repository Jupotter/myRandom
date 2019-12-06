using System;
using System.Diagnostics;
using System.Threading;
using JetBrains.Annotations;

namespace MyRandom
{
    public static class Pseudorandom
    {
        [NotNull] private static readonly ThreadLocal<Random> Prng =
            new ThreadLocal<Random>(() => new Random(BetterRandom.NextInt()));

        public static int NextInt()
        {
            Debug.Assert(Prng.Value != null, "prng.Value != null");
            return Prng.Value.Next();
        }

        public static double NextDouble()
        {
            Debug.Assert(Prng.Value != null, "prng.Value != null");
            return Prng.Value.NextDouble();
        }
    }
}