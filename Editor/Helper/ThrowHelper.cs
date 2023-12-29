// ------------------------------------------------------------
//         File: ThrowHelper.cs
//        Brief: ThrowHelper.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2023-12-25 22:58
//    Copyright: Copyright (c) 2023, VyronLee
// ============================================================

using System.Collections.Generic;
using System.Linq;
using vFrame.Bundler.Exception;

namespace vFrame.Bundler.Editor.Helper
{
    internal static class ThrowHelper
    {
        public static void ThrowArgumentException(string message) {
            throw new BundleArgumentException(message);
        }

        public static void ThrowIfNull(object param, string variable) {
            if (null != param) {
                return;
            }
            throw new BundleArgumentException($"Variable ${variable} cannot be null!");
        }

        public static void ThrowIfNullOrEmpty<T>(IEnumerable<T> param, string variable) {
            if (null != param && param.Any()) {
                return;
            }
            throw new BundleArgumentException($"Variable ${variable} cannot be null or empty!");
        }

        public static void ThrowUnsupportedEnum<T>(T value) {
            throw new BundleUnsupportedEnumException($"Unsupported enum value: {value}!");
        }

        public static void ThrowUndesiredException(string message) {
            throw new BundleException(message);
        }

        public static string Variables(params string[] args) {
            if (null == args || args.Length <= 0) {
                return "";
            }
            return string.Join(".", args);
        }
    }
}