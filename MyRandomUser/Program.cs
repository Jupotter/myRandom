using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MyRandom;

namespace MyRandomUser
{
    class Program
    {
        static void Main(string[] args)
        {
            ColdSneeze();
        }

        private static void Animals()
        {
            var cat     = new Cat();
            var dog     = new Dog();
            var fish    = new Goldfish();
            var animals = new List<Animal>() {cat, dog, dog, fish};
            Console.WriteLine(animals.ToUniform().Histogram());
            Console.WriteLine(animals.ToUniform().ShowWeights());

            Console.WriteLine(WeightedInteger.Distribution(10, 0, 0, 11, 5).Histogram());
        }

        private static void ColdSneeze()
        {
            var                         colds = new List<Cold> {Cold.No, Cold.Yes};
            IDiscreteDistribution<Cold> cold  = colds.ToWeighted(90, 10);
            Console.WriteLine(cold.SelectMany(SneezedGivenCold).Histogram());
        }

        [NotNull]
        static IDiscreteDistribution<Sneezed>  SneezedGivenCold(Cold c)
        {
            var list = new List<Sneezed>() { Sneezed.No, Sneezed.Yes };
            return c == Cold.No ?
                       list.ToWeighted(97, 3) :
                       list.ToWeighted(15, 85);
        }
    }

    internal class Goldfish : Animal
    {
    }

    internal class Dog : Animal
    {
    }

    internal class Cat : Animal
    {
    }

    abstract class Animal
    {
    }

    enum Cold
    {
        No,
        Yes
    }

    enum Sneezed
    {
        No,
        Yes
    }
}