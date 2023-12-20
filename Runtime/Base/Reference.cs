//------------------------------------------------------------
//        File:  Reference.cs
//       Brief:  Reference of objects.
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 20:02
//   Copyright:  Copyright (c) 2018, VyronLee
//============================================================

using vFrame.Bundler.Interface;

namespace vFrame.Bundler.Base
{
    public class Reference : IReference
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
