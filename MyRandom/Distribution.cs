using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace MyRandom
{
    public static class Distribution
    {
        [Pure]
        [NotNull]
        [ItemNotNull]
        public static IEnumerable<T> Samples<T>([NotNull] this IDistribution<T> d)
        {
            while (true)
                yield return d.Sample();
            // ReSharper disable once IteratorNeverReturns
        }

        [NotNull]
        [Pure]
        public static IDiscreteDistribution<T> ToUniform<T>([NotNull] [ItemNotNull] this IEnumerable<T> items)

        {
            var list = items.ToList();
            return from i in StandardDiscreteUniform.Distribution(0, list.Count - 1)
                   select list[i];
        }

        [NotNull]
        [Pure]
        public static IDiscreteDistribution<TR> Select<TA, TR>([NotNull] this IDiscreteDistribution<TA> d,
                                                               [NotNull] Func<TA, TR> projection)
        {
            var dict = d.Support()
                        .GroupBy(projection, d.Weight)
                        .ToDictionary(g => g.Key, g => g.Sum());
            var rs = dict.Keys.ToList();
            return Projected<int, TR>.Distribution(
                WeightedInteger.Distribution(
                    rs.Select(r => dict[r])),
                i => rs[i]);
        }

        [Pure]
        [NotNull]
        public static IDiscreteDistribution<T> ToWeighted<T>([NotNull] this IEnumerable<T> items,
                                                             [NotNull] IEnumerable<int> weights)
        {
            var list = items.ToList();
            return WeightedInteger.Distribution(weights)
                                  .Select(i => list[i]);
        }

        [Pure]
        [NotNull]
        public static IDiscreteDistribution<T> ToWeighted<T>([NotNull] this IEnumerable<T> items,
                                                             [NotNull] params int[] weights)
        {
            return items.ToWeighted((IEnumerable<int>) weights);
        }

        [NotNull]
        [Pure]
        public static IDiscreteDistribution<T> Where<T>([NotNull] this IDiscreteDistribution<T> d,
                                                        [NotNull] Func<T, bool> predicate)
        {
            var s = d.Support().Where(predicate).ToList();
            return s.ToWeighted(s.Select(d.Weight));
        }

        [NotNull]
        public static IDiscreteDistribution<R> SelectMany<A, R>([NotNull] this IDiscreteDistribution<A> prior,
                                                                [NotNull] Func<A, IDiscreteDistribution<R>> likelihood)
        {
            return Combined<A, R>.Distribution(prior, likelihood);
        }

        [NotNull]
        [Pure]
        public static string Histogram<T>([NotNull] this IDiscreteDistribution<T> d)
        {
            return d.Samples().DiscreteHistogram();
        }

        [NotNull]
        [Pure]
        public static string Histogram([NotNull] this IDistribution<double> d, double low, double high)
        {
            return d.Samples().Histogram(low, high);
        }

        [NotNull]
        [Pure]
        public static string Histogram([NotNull] this IEnumerable<double> d, double low, double high)
        {
            const int width       = 40;
            const int height      = 20;
            const int sampleCount = 100000;
            var       buckets     = new int[width];
            foreach (double c in d.Take(sampleCount))
            {
                var bucket = (int) (buckets.Length * (c - low) / (high - low));
                if (0 <= bucket && bucket < buckets.Length)
                    buckets[bucket] += 1;
            }

            int    max   = buckets.Max();
            double scale = max < height ? 1.0 : (double) height / max;
            return string.Join("",
                               Enumerable.Range(0, height).Select(
                                   r => string.Join("", buckets.Select(
                                                        b => b * scale > height - r ? '*' : ' ')) + "\n"))
                   + new string('-', width) + "\n";
        }

        [NotNull]
        [Pure]
        public static string DiscreteHistogram<T>([NotNull] this IEnumerable<T> d)
        {
            const int sampleCount = 100000;
            const int width       = 40;
            Dictionary<T, int> dict = d.Take(sampleCount)
                                       .GroupBy(x => x)
                                       .ToDictionary(g => g.Key, g => g.Count());
            int labelMax = dict.Keys.Select(x => x.ToString().Length)
                               .Max();
            List<T> sup = typeof(IComparable<T>).IsAssignableFrom(typeof(T))
                              ? dict.Keys.OrderBy(x => x).ToList()
                              : dict.OrderBy(x => x.Value).Select(x => x.Key).ToList();
            int    max   = dict.Values.Max();
            double scale = max < width ? 1.0 : (double) width / max;
            return string.Join("\n", sup.Select(s => $"{ToLabel(s)}|{Bar(s)}"));

            string ToLabel(T t)
            {
                return t.ToString().PadLeft(labelMax);
            }

            string Bar(T t)
            {
                return new string('*', (int) (dict[t] * scale));
            }
        }

        [NotNull]
        [Pure]
        public static string ShowWeights<T>([NotNull] this IDiscreteDistribution<T> d)
        {
            int labelMax = d.Support()
                            .Select(x => x.ToString().Length)
                            .Max();
            return string.Join("\n", d.Support()
                                      .Select(s => $"{ToLabel(s)}:{d.Weight(s)}"));

            string ToLabel(T t)
            {
                return t.ToString().PadLeft(labelMax);
            }
        }
    }
}