//------------------------------------------------------------
//        File:  BundlerException.cs
//       Brief:  Exceptions.
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 20:03
//   Copyright:  Copyright (c) 2024, VyronLee
//============================================================

namespace vFrame.Bundler.Exception
{
    public class BundleException : System.Exception
    {
        public BundleException(string message) : base(message) {
        }
    }

    public class BundleArgumentException : BundleException
    {
        public BundleArgumentException(string message) : base(message) {
        }
    }

    public class BundleArgumentNullException : BundleException
    {
        public BundleArgumentNullException(string message = "Argument cannot be null.") : base(message) {
        }
    }

    public class BundleUnsupportedEnumException : BundleException
    {
        public BundleUnsupportedEnumException(string message) : base(message) {
        }
    }

    public class BundleNotFoundException : BundleException
    {
        public BundleNotFoundException(string message) : base(message) {
        }
    }

    public class BundleNoneConfigurationException : BundleException
    {
        public BundleNoneConfigurationException(string message) : base(message) {
        }
    }

    public class BundleLoadFailedException : BundleException
    {
        public BundleLoadFailedException(string path) : base($"Could not load bundle at path: {path}") {
        }
    }

    public class BundleLoadNotFinishedException : BundleException
    {
        public BundleLoadNotFinishedException(string message) : base(message) {
        }
    }

    public class BundleAssetLoadFailedException : BundleException
    {
        public BundleAssetLoadFailedException(string path) : base($"Could not load asset at path: {path}") {
        }
    }

    public class BundleAssetNotReadyException : BundleException
    {
        public BundleAssetNotReadyException(string message) : base(message) {
        }
    }

    public class BundleSceneLoadFailedException : BundleException
    {
        public BundleSceneLoadFailedException(string message) : base(message) {
        }
    }

    public class BundleInstanceNotFoundException : BundleException
    {
        public BundleInstanceNotFoundException(string message) : base(message) {
        }
    }

    public class BundleAssetTypeNotMatchException : BundleException
    {
        public BundleAssetTypeNotMatchException(string message) : base(message) {
        }
    }

    public class BundleRuleConflictException : BundleException
    {
        public BundleRuleConflictException(string message) : base(message) {
        }
    }

    public class BundleBuildFailedException : BundleException
    {
        public BundleBuildFailedException(string message) : base(message) {
        }
    }

    public class BundleNotSupportedException : BundleException
    {
        public BundleNotSupportedException(string message) : base(message) {
        }
    }
}