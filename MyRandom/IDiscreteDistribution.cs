using System.Collections.Generic;
using JetBrains.Annotations;

namespace MyRandom
{
    public interface IDiscreteDistribution<T> : IDistribution<T>
    {
        [MustUseReturnValue]
        [NotNull]
        [ItemNotNull]
        IEnumerable<T> Support();

        [MustUseReturnValue]
        int Weight([NotNull] T t);
    }
}