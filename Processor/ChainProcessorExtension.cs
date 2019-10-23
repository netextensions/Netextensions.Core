namespace Netextensions.Core.Processor
{
    public static class ChainProcessorExtension
    {
        public static IProcessor<T> Then<T>(
            this IProcessor<T> first, IProcessor<T> next) =>
            first is NullProcessor<T> ? next
            : next is NullProcessor<T> ? first
            : new ChainedProcessor<T>(first, next);

    }
}