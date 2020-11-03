using System.Collections.Generic;
using vFrame.Bundler.Interface;

namespace vFrame.Bundler.Utils.Pools
{
    public class DictionaryAllocator<T1, T2> : IPoolObjectAllocator<Dictionary<T1, T2>>
    {
        public Dictionary<T1, T2> Alloc() {
            return new Dictionary<T1, T2>();
        }

        public void Reset(Dictionary<T1, T2> obj) {
            obj.Clear();
        }
    }
}