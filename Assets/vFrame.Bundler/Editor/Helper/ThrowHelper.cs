// ------------------------------------------------------------
//         File: ThrowHelper.cs
//        Brief: Editor-side helpers that throw Bundler exceptions and build dotted variable names for error messages.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:07:46
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System.Collections.Generic;
using System.Linq;

namespace vFrame.Bundler.Editor
{
    /// <summary>
    ///     Editor-side helpers that throw Bundler-specific exceptions and build dot-separated variable-name
    ///     strings for use in error messages.
    /// </summary>
    internal static class ThrowHelper
    {
        /// <summary>
        ///     Throws a <see cref="BundleArgumentException"/> carrying the specified message.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <exception cref="BundleArgumentException">Always thrown by this method.</exception>
        public static void ThrowArgumentException(string message)
        {
            throw new BundleArgumentException(message);
        }

        /// <summary>
        ///     Throws if the given parameter value is null.
        /// </summary>
        /// <param name="param">Parameter value to check.</param>
        /// <param name="variable">Name of the checked parameter, used in the exception message.</param>
        /// <exception cref="BundleArgumentException"><paramref name="param"/> is null.</exception>
        public static void ThrowIfNull(object param, string variable)
        {
            if (null != param) {
                return;
            }
            throw new BundleArgumentException($"Variable ${variable} cannot be null!");
        }

        /// <summary>
        ///     Throws if the given sequence is null or contains no elements.
        /// </summary>
        /// <typeparam name="T">Element type of the sequence.</typeparam>
        /// <param name="param">Sequence to check.</param>
        /// <param name="variable">Name of the checked parameter, used in the exception message.</param>
        /// <exception cref="BundleArgumentException"><paramref name="param"/> is null or empty.</exception>
        public static void ThrowIfNullOrEmpty<T>(IEnumerable<T> param, string variable)
        {
            if (null != param && param.Any()) {
                return;
            }
            throw new BundleArgumentException($"Variable ${variable} cannot be null or empty!");
        }

        /// <summary>
        ///     Throws to report an enum value not supported by the current pipeline.
        /// </summary>
        /// <typeparam name="T">Type of the enum value.</typeparam>
        /// <param name="value">Unsupported enum value, included in the exception message.</param>
        /// <exception cref="BundleUnsupportedEnumException">Always thrown by this method.</exception>
        public static void ThrowUnsupportedEnum<T>(T value)
        {
            throw new BundleUnsupportedEnumException($"Unsupported enum value: {value}!");
        }

        /// <summary>
        ///     Throws to report that an unreachable or undesired code path was reached.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <exception cref="BundleException">Always thrown by this method.</exception>
        public static void ThrowUndesiredException(string message)
        {
            throw new BundleException(message);
        }

        /// <summary>
        ///     Joins variable-name parts into one dot-separated identifier for exception messages.
        /// </summary>
        /// <param name="args">Variable-name parts, ordered from outermost to innermost.</param>
        /// <returns>
        ///     Dot-separated name such as "buildRules.MainRules", or an empty string when no parts are given.
        /// </returns>
        public static string Variables(params string[] args)
        {
            if (null == args || args.Length <= 0) {
                return "";
            }
            return string.Join(".", args);
        }
    }
}