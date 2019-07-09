//------------------------------------------------------------
//        File:  BundlerException.cs
//       Brief:  Exceptions.
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 20:03
//   Copyright:  Copyright (c) 2018, VyronLee
//============================================================

namespace vBundler.Exception
{
    public class BundleNotFoundException : System.Exception
    {
        public BundleNotFoundException(string message) : base(message)
        {
        }
    }

    public class BundleNoneConfigurationException : System.Exception
    {
        public BundleNoneConfigurationException(string message) : base(message)
        {
        }
    }

    public class BundleLoadFailedException : System.Exception
    {
        public BundleLoadFailedException(string message) : base(message)
        {
        }
    }

    public class BundleAssetLoadFailedException : System.Exception
    {
        public BundleAssetLoadFailedException(string message) : base(message)
        {
        }
    }

    public class BundleAssetNotReadyException : System.Exception
    {
        public BundleAssetNotReadyException(string message) : base(message)
        {
        }
    }

    public class BundleSceneLoadFailedException : System.Exception
    {
        public BundleSceneLoadFailedException(string message) : base(message)
        {
        }
    }

    public class BundleInstanceNotFoundException : System.Exception
    {
        public BundleInstanceNotFoundException(string message) : base(message)
        {
        }
    }

    public class BundleAssetTypeNotMatchException : System.Exception
    {
        public BundleAssetTypeNotMatchException(string message) : base(message)
        {
        }
    }
}