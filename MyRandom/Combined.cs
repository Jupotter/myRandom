using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;

namespace MyRandom
{
    public sealed class Combined<TA, TR> : IDiscreteDistribution<TR>
    {
        [NotNull] private readonly Func<TA, IDiscreteDistribution<TR>> likelihood;
        [NotNull] private readonly IDiscreteDistribution<TA>          prior;
        [NotNull] private readonly List<TR>                           support;

        private Combined([NotNull] IDiscreteDistribution<TA> prior,
                         [NotNull] Func<TA, IDiscreteDistribution<TR>> likelihood)
        {
            this.prior = prior;
            this.likelihood = likelihood;
            IEnumerable<TR> q = from a in prior.Support()
                               from b in this.likelihood(a)?.Support()
                               select b;
            support = q.Distinct().ToList();
        }

        public IEnumerable<TR> Support()
        {
            return support.Select(x => x);
        }

        public TR Sample()
        {
            var post = likelihood(prior.Sample());
            Debug.Assert(post != null, nameof(post) + " != null");
            return post.Sample();
        }

        public int Weight(TR r)
        {
            return 0;
        }

        [NotNull]
        public static IDiscreteDistribution<TR> Distribution([NotNull] IDiscreteDistribution<TA> prior,
                                                            [NotNull] Func<TA, IDiscreteDistribution<TR>> likelihood)
        {
            return new Combined<TA, TR>(prior, likelihood);
        }
    }
}