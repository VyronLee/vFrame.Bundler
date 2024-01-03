namespace vFrame.Bundler.Interface
{
    public interface IReference
    {
        void Retain();

        void Release();

        int GetReferences();
    }
}