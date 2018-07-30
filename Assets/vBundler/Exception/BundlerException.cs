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

    public class BundleMixLoadingRequestException : System.Exception
    {
        public BundleMixLoadingRequestException(string message) : base(message)
        {
        }
    }
}