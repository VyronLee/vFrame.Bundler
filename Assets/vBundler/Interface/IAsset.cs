using UnityEngine;

namespace vBundler.Interface
{
    public interface IAsset
    {
        bool IsDone { get; set; }

        Object GetAsset();
        T GetAsset<T>() where T : Object;

        GameObject Instantiate();
        void SetTo(Component target, string propName);

        void Retain();
        void Release();
    }
}