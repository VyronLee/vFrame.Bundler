using System.Collections;

namespace vBundler.Interface
{
    public interface IAsync : IEnumerator
    {
        bool IsDone { get; }
    }
}