using System;
using System.Diagnostics;
using System.Threading;
using JetBrains.Annotations;
using CRNG = System.Security.Cryptography.RandomNumberGenerator;

namespace MyRandom
{
    public static class BetterRandom
    {
        [NotNull] private static readonly ThreadLocal<CRNG> Crng = new ThreadLocal<CRNG>(CRNG.Create);

        [NotNull] private static readonly ThreadLocal<byte[]> Bytes =
            new ThreadLocal<byte[]>(() => new byte[sizeof(int)]);

        public static int NextInt()
        {
            Debug.Assert(Crng.Value  != null, "Crng.Value != null");
            Debug.Assert(Bytes.Value != null, "Bytes.Value != null");

            Crng.Value.GetBytes(Bytes.Value);
            return BitConverter.ToInt32(Bytes.Value, 0) & int.MaxValue;
        }

        // ReSharper disable RedundantCast
        // ReSharper disable CompareOfFloatsByEqualityOperator
        public static double NextDouble()
        {
            long x = NextInt() & 0x001FFFFF;
            x <<= 31;
            x |= (long) NextInt();
            double       n = x;
            const double d = 1L << 52;
            return n / d;
        }
    }
}