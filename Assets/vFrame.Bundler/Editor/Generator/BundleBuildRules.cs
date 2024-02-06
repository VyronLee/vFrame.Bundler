// ------------------------------------------------------------
//         File: BundlerBuildRule.cs
//        Brief: AssetBundle build rule structure definition
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2023-12-19 16:28
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System;
using System.Collections.Generic;
using System.Linq;
using vFrame.Bundler.Exception;

namespace vFrame.Bundler
{
    public enum PackType
    {
        PackBySingleFile = 1,
        PackByAllFiles = 2,
        PackByTopDirectory = 3,
        PackByAllDirectories = 4
    }

    [Serializable]
    public class BundleBuildRules
    {
        public List<MainBundleRule> MainRules { get; } = new List<MainBundleRule>();
        public List<AutoGroupRule> GroupRules { get; } = new List<AutoGroupRule>();

        public static BundleBuildRules FromJson(string jsonText) {
            var jsonData = Json.Deserialize(jsonText) as JsonObject;
            if (null == jsonData) {
                throw new BundleException("Deserialize bundle build rules failed!");
            }

            var ret = new BundleBuildRules();
            if (jsonData.TryGetValue(nameof(MainRules), out var mainRules)) {
                var ruleList = mainRules as JsonList;
                if (ruleList != null) {
                    var rules = ruleList.Select(v => v as JsonObject)
                        .Where(v => v != null)
                        .Select(MainBundleRule.FromJsonObject);
                    ret.MainRules.AddRange(rules);
                }
            }
            if (jsonData.TryGetValue(nameof(GroupRules), out var groupRules)) {
                var ruleList = groupRules as JsonList;
                if (ruleList != null) {
                    var rules = ruleList.Select(v => v as JsonObject)
                        .Where(v => v != null)
                        .Select(AutoGroupRule.FromJsonObject);
                    ret.GroupRules.AddRange(rules);
                }
            }
            return ret;
        }
    }

    [Serializable]
    public class MainBundleRule
    {
        /// <summary>
        /// Packing method type, see PackType definition
        /// </summary>
        public string PackType { get; set; }

        /// <summary>
        /// Search path
        /// </summary>
        public string SearchPath { get; set; } = "";

        /// <summary>
        /// Regular expression, only resource files that meet the conditions will be
        /// included in the AB package
        /// </summary>
        public string Include { get; set; } = ".+";

        /// <summary>
        /// Regular expression, files that meet the conditions will be excluded and will not
        /// enter the AB package
        /// </summary>
        public string Exclude { get; set; } = "";

        internal static MainBundleRule FromJsonObject(JsonObject jsonObject) {
            var ret = new MainBundleRule();
            ret.PackType = jsonObject.SafeGetValue(nameof(PackType), ret.PackType);
            ret.SearchPath = jsonObject.SafeGetValue(nameof(SearchPath), ret.SearchPath);
            ret.Include = jsonObject.SafeGetValue(nameof(Include), ret.Include);
            ret.Exclude = jsonObject.SafeGetValue(nameof(Exclude), ret.Exclude);
            return ret;
        }
    }

    [Serializable]
    public class AutoGroupRule
    {
        public string Include { get; set; } = "(.+)";
        public string Exclude { get; set; } = "";

        public static AutoGroupRule Fallback { get; } = new AutoGroupRule { Include = "(.+)" };

        internal static AutoGroupRule FromJsonObject(JsonObject jsonObject) {
            var ret = new AutoGroupRule();
            ret.Include = jsonObject.SafeGetValue(nameof(Include), ret.Include);
            ret.Exclude = jsonObject.SafeGetValue(nameof(Exclude), ret.Exclude);
            return ret;
        }
    }
}