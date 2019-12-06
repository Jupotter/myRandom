using JetBrains.Annotations;

namespace MyRandom
{
    public interface IDistribution<out T>
    {
        [MustUseReturnValue]
        [NotNull]
        T Sample();
    }
}