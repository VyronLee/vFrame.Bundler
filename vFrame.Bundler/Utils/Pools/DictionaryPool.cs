using System.Collections.Generic;

namespace vFrame.Bundler.Utils.Pools
{
    public class DictionaryPool<T1, T2> : ObjectPool<Dictionary<T1, T2>, DictionaryAllocator<T1, T2>>
    {

    }
}