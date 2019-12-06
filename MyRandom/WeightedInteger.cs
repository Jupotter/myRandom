using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace MyRandom
{
    public sealed class WeightedInteger : IDiscreteDistribution<int>
    {
        [NotNull] private readonly IDistribution<IDistribution<int>> rows;
        [NotNull] private readonly List<int>                         weights;

        private WeightedInteger([NotNull] IEnumerable<int> weights)
        {
            this.weights = weights.ToList();
            int s = this.weights.Sum();
            int n = this.weights.Count;
            var distributions = new IDistribution<int>[n];
            var lows  = new Dictionary<int, int>();
            var highs = new Dictionary<int, int>();

            for (var i = 0; i < n; i += 1)
            {
                int w = this.weights[i] * n;
                if (w == s)
                    distributions[i] = Singleton<int>.Distribution(i);
                else if (w < s)
                    lows.Add(i, w);
                else
                    highs.Add(i, w);
            }

            while (lows.Any())
            {
                KeyValuePair<int, int> low = lows.First();
                lows.Remove(low.Key);
                KeyValuePair<int, int> high = highs.First();
                highs.Remove(high.Key);
                int lowNeeds = s - low.Value;
                distributions[low.Key] = Bernoulli.Distribution(low.Value, lowNeeds)
                                                  .Select(x => x == 0 ? low.Key : high.Key);
                int newHigh = high.Value - lowNeeds;
                if (newHigh == s)
                    distributions[high.Key] =
                        Singleton<int>.Distribution(high.Key);
                else if (newHigh < s)
                    lows[high.Key] = newHigh;
                else
                    highs[high.Key] = newHigh;
            }

            rows = distributions.ToUniform();
        }

        public IEnumerable<int> Support()
        {
            return Enumerable.Range(0, weights.Count).Where(x => weights[x] != 0);
        }

        public int Weight(int i)
        {
            return 0 <= i && i < weights.Count ? weights[i] : 0;
        }

        public int Sample()
        {
            return rows.Sample().Sample();
        }

        [NotNull]
        public static IDiscreteDistribution<int> Distribution([NotNull] params int[] weights)
        {
            return Distribution((IEnumerable<int>) weights);
        }

        [NotNull]
        public static IDiscreteDistribution<int> Distribution([NotNull] IEnumerable<int> weights)
        {
            List<int> w = weights.ToList();
            if (w.Any(x => x < 0) || !w.Any(x => x > 0))
                throw new ArgumentException();
            if (w.Count == 1)
                return Singleton<int>.Distribution(0);
            if (w.Count == 2)
                return Bernoulli.Distribution(w[0], w[1]);
            return new WeightedInteger(w);
        }
    }
}