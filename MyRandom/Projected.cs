using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace MyRandom
{
    public sealed class Projected<TA, TR> : IDiscreteDistribution<TR>
    {
        [NotNull] private readonly Func<TA, TR>              projection;
        [NotNull] private readonly IDiscreteDistribution<TA> underlying;
        [NotNull] private readonly Dictionary<TR, int>       weights;

        private Projected([NotNull] IDiscreteDistribution<TA> underlying, [NotNull] Func<TA, TR> projection)
        {
            this.underlying = underlying;
            this.projection = projection;
            weights = underlying.Support().GroupBy(projection, underlying.Weight)
                                .ToDictionary(g => g.Key, g => g.Sum());
        }

        public TR Sample()
        {
            return projection(underlying.Sample());
        }

        public IEnumerable<TR> Support()
        {
            return weights.Keys;
        }

        public int Weight(TR r)
        {
            return weights.GetValueOrDefault(r, 0);
        }

        [NotNull]
        public static IDiscreteDistribution<TR> Distribution([NotNull] IDiscreteDistribution<TA> underlying,
                                                             [NotNull] Func<TA, TR> projection)
        {
            var result = new Projected<TA, TR>(underlying, projection);
            if (result.Support().Count() == 1)
                return Singleton<TR>.Distribution(result.Support().First());
            return result;
        }
    }
}