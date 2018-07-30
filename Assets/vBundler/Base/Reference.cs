
namespace vBundler.Base
{
    public class Reference
    {
        protected int _references;

        public Reference()
        {
            _references = 0;
        }

        public virtual void Retain()
        {
            ++_references;
        }

        public virtual void Release()
        {
            --_references;
        }

        public virtual int GetReferences()
        {
            return _references;
        }

    }
}
