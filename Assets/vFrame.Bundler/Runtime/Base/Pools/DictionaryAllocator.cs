using System.Collections.Generic;

namespace vFrame.Bundler
{
    internal class DictionaryAllocator<T1, T2> : IPoolObjectAllocator<Dictionary<T1, T2>>
    {
        public Dictionary<T1, T2> Alloc() {
            return new Dictionary<T1, T2>();
        }

        public void Reset(Dictionary<T1, T2> obj) {
            obj.Clear();
        }
    }
}