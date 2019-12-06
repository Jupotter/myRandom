using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace MyRandom
{
    public class Singleton<T> : IDiscreteDistribution<T>
    {
        [NotNull] private readonly T t;

        private Singleton([NotNull] T t)
        {
            this.t = t;
        }

        
        public T Sample()
        {
            return t;
        }

        
        public IEnumerable<T> Support()
        {
            yield return t;
        }

        
        public int Weight(T i)
        {
            return EqualityComparer<T>.Default.Equals(t, i) ? 1 : 0;
        }

        
        [NotNull]
        public static Singleton<T> Distribution([NotNull] T t)
        {
            return new Singleton<T>(t);
        }

        public override string ToString()
        {
            return $"Singleton[{t}]";
        }
    }

    public sealed class Bernoulli :
        IDiscreteDistribution<int>
    {
        private Bernoulli(int zero, int one)
        {
            Zero = zero;
            One = one;
        }

        public int Zero { get; }
        public int One  { get; }

        
        public int Sample()
        {
            return StandardContinuousUniform.Distribution.Sample() <= (double) Zero / (Zero + One)
                       ? 0 : 1;
        }

        
        public IEnumerable<int> Support()
        {
            return Enumerable.Range(0, 2);
        }

        
        public int Weight(int x)
        {
            return x == 0 ? Zero : x == 1 ? One : 0;
        }

        
        [NotNull]
        public static IDiscreteDistribution<int> Distribution(int zero, int one)
        {
            if (zero < 0 || one < 0 || zero == 0 && one == 0)
                throw new ArgumentException();
            if (zero == 0) return Singleton<int>.Distribution(1);
            if (one  == 0) return Singleton<int>.Distribution(0);
            return new Bernoulli(zero, one);
        }

        public override string ToString()
        {
            return $"Bernoulli[{Zero}, {One}]";
        }
    }
}