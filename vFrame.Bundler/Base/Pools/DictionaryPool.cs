using System.Collections.Generic;

namespace vFrame.Bundler.Base.Pools
{
    internal class DictionaryPool<T1, T2> : ObjectPool<Dictionary<T1, T2>, DictionaryAllocator<T1, T2>>
    {
    }
}