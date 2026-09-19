using Unity.Collections;

namespace NotBura.Packages
{
    public static class NativeContainerExtensions
    {
        public static NotQuery<NativeListIterator<T>, T> AsQuery<T>(this in NativeList<T> source)
            where T : unmanaged
        {
            return new(new(source));
        }

        public static NativeArray<T> ToNativeArray<TIterator, T>(this NotQuery<TIterator, T> source, AllocatorManager.AllocatorHandle allocator)
            where TIterator : struct, INotQueryIterator<T>
            where T : unmanaged
        {
            using var _buffer = new NativeList<T>(SafeTemporaryAllocator());

            var _iterator = source.Iterator;
            while (_iterator.TryMoveNext(out var _current))
            {
                _buffer.Add(_current);
            }

            return _buffer.ToArray(allocator);
        }

        private static AllocatorManager.AllocatorHandle SafeTemporaryAllocator()
        {
#if ENABLE_UNITASK
            return Cysharp.Threading.Tasks.PlayerLoopHelper.IsMainThread
                ? Allocator.Temp
                : Allocator.Persistent;
#else
            return Allocator.Persistent;
#endif
        }
    }
}
