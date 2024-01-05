using UnityEngine;

namespace vFrame.Bundler
{
    public abstract class PropertySetterProxy<T1, T2> : PropertyProxy where T1 : Component where T2 : Object
    {
        public abstract void Set(T1 target, T2 asset);
    }
}