// ------------------------------------------------------------
//         File: BundlerException.cs
//        Brief: Defines the Bundler exception hierarchy: a common base plus argument, lookup, loading,
//               build and rule variants.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 06:29:40
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


namespace vFrame.Bundler
{
    /// <summary>
    ///     Base type of all exceptions thrown by the bundler runtime and build pipeline.
    /// </summary>
    public class BundleException : System.Exception
    {
        /// <summary>
        ///     Initializes the exception with the specified message.
        /// </summary>
        /// <param name="message">Description of the error.</param>
        public BundleException(string message) : base(message)
        {
        }
    }

    /// <summary>
    ///     Thrown when an argument value is invalid, for example empty or out of range.
    /// </summary>
    public class BundleArgumentException : BundleException
    {
        /// <summary>
        ///     Initializes the exception with the specified message.
        /// </summary>
        /// <param name="message">Description of the invalid argument.</param>
        public BundleArgumentException(string message) : base(message)
        {
        }
    }

    /// <summary>
    ///     Thrown when a required argument is null.
    /// </summary>
    public class BundleArgumentNullException : BundleException
    {
        /// <summary>
        ///     Initializes the exception, falling back to a generic message when none is supplied.
        /// </summary>
        /// <param name="message">Description of the invalid argument.</param>
        public BundleArgumentNullException(string message = "Argument cannot be null.") : base(message)
        {
        }
    }

    /// <summary>
    ///     Thrown when an enum value has no supported handling in the current context.
    /// </summary>
    public class BundleUnsupportedEnumException : BundleException
    {
        /// <summary>
        ///     Initializes the exception with the specified message.
        /// </summary>
        /// <param name="message">Description of the unsupported enum value.</param>
        public BundleUnsupportedEnumException(string message) : base(message)
        {
        }
    }

    /// <summary>
    ///     Thrown when a requested bundle or asset cannot be located in the manifest.
    /// </summary>
    public class BundleNotFoundException : BundleException
    {
        /// <summary>
        ///     Initializes the exception with the specified message.
        /// </summary>
        /// <param name="message">Description of the missing item.</param>
        public BundleNotFoundException(string message) : base(message)
        {
        }
    }

    /// <summary>
    ///     Thrown when an asset path has no matching configuration in the manifest.
    /// </summary>
    public class BundleNoneConfigurationException : BundleException
    {
        /// <summary>
        ///     Initializes the exception with the specified message.
        /// </summary>
        /// <param name="message">Description of the unconfigured asset path.</param>
        public BundleNoneConfigurationException(string message) : base(message)
        {
        }
    }

    /// <summary>
    ///     Thrown when an AssetBundle fails to load.
    /// </summary>
    public class BundleLoadFailedException : BundleException
    {
        /// <summary>
        ///     Initializes the exception with the bundle path that failed to load.
        /// </summary>
        /// <param name="path">Path of the bundle that could not be loaded.</param>
        public BundleLoadFailedException(string path) : base($"Could not load bundle at path: {path}")
        {
        }
    }

    /// <summary>
    ///     Thrown when an operation requires a load that has not completed yet.
    /// </summary>
    public class BundleLoadNotFinishedException : BundleException
    {
        /// <summary>
        ///     Initializes the exception with the specified message.
        /// </summary>
        /// <param name="message">Description of the unfinished load.</param>
        public BundleLoadNotFinishedException(string message) : base(message)
        {
        }
    }

    /// <summary>
    ///     Thrown when an asset fails to load from its bundle.
    /// </summary>
    public class BundleAssetLoadFailedException : BundleException
    {
        /// <summary>
        ///     Initializes the exception with the asset path that failed to load.
        /// </summary>
        /// <param name="path">Path of the asset that could not be loaded.</param>
        public BundleAssetLoadFailedException(string path) : base($"Could not load asset at path: {path}")
        {
        }
    }

    /// <summary>
    ///     Thrown when an asset is accessed before its asynchronous load has finished.
    /// </summary>
    public class BundleAssetNotReadyException : BundleException
    {
        /// <summary>
        ///     Initializes the exception with the specified message.
        /// </summary>
        /// <param name="message">Description of the not-yet-ready asset.</param>
        public BundleAssetNotReadyException(string message) : base(message)
        {
        }
    }

    /// <summary>
    ///     Thrown when a scene fails to load.
    /// </summary>
    public class BundleSceneLoadFailedException : BundleException
    {
        /// <summary>
        ///     Initializes the exception with the specified message.
        /// </summary>
        /// <param name="message">Description of the failed scene load.</param>
        public BundleSceneLoadFailedException(string message) : base(message)
        {
        }
    }

    /// <summary>
    ///     Thrown when a required loaded instance cannot be found.
    /// </summary>
    public class BundleInstanceNotFoundException : BundleException
    {
        /// <summary>
        ///     Initializes the exception with the specified message.
        /// </summary>
        /// <param name="message">Description of the missing instance.</param>
        public BundleInstanceNotFoundException(string message) : base(message)
        {
        }
    }

    /// <summary>
    ///     Thrown when a loaded asset's type does not match the requested type.
    /// </summary>
    public class BundleAssetTypeNotMatchException : BundleException
    {
        /// <summary>
        ///     Initializes the exception with the specified message.
        /// </summary>
        /// <param name="message">Description of the type mismatch.</param>
        public BundleAssetTypeNotMatchException(string message) : base(message)
        {
        }
    }

    /// <summary>
    ///     Thrown when asset grouping rules conflict, for example duplicated or overlapping rules.
    /// </summary>
    public class BundleRuleConflictException : BundleException
    {
        /// <summary>
        ///     Initializes the exception with the specified message.
        /// </summary>
        /// <param name="message">Description of the conflicting rules.</param>
        public BundleRuleConflictException(string message) : base(message)
        {
        }
    }

    /// <summary>
    ///     Thrown when the AssetBundle build pipeline fails.
    /// </summary>
    public class BundleBuildFailedException : BundleException
    {
        /// <summary>
        ///     Initializes the exception with the specified message.
        /// </summary>
        /// <param name="message">Description of the build failure.</param>
        public BundleBuildFailedException(string message) : base(message)
        {
        }
    }

    /// <summary>
    ///     Thrown when an operation is not supported in the current bundler mode or context.
    /// </summary>
    public class BundleNotSupportedException : BundleException
    {
        /// <summary>
        ///     Initializes the exception with the specified message.
        /// </summary>
        /// <param name="message">Description of the unsupported operation.</param>
        public BundleNotSupportedException(string message) : base(message)
        {
        }
    }
}