namespace vFrame.Bundler
{
    public interface IReference
    {
        void Retain();

        void Release();

        int GetReferences();
    }
}