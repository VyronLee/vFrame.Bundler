using System.Collections;

namespace vFrame.Bundler.Interface
{
    internal interface IAsyncProcessor
    {
        bool IsSetup { get; set; }
        int ThreadHandle { get; set; }
        IEnumerator OnAsyncProcess();
    }
}