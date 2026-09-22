// ------------------------------------------------------------
//         File: BundleBuildRules.cs
//        Brief: Serializable rule set that drives AssetBundle build packing:
//               main-asset packing rules, shared-dependency auto-grouping
//               rules, and JSON deserialization of the whole rule set.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 03:19:44
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================



using System;
using System.Collections.Generic;
using System.Linq;

namespace vFrame.Bundler.Editor
{
    /// <summary>
    /// Determines how assets matched by a rule are grouped into bundle files.
    /// </summary>
    public enum PackType
    {
        /// <summary>Pack each matched asset into its own single bundle file.</summary>
        PackBySingleFile = 1,

        /// <summary>Pack all matched assets into one bundle file.</summary>
        PackByAllFiles = 2,

        /// <summary>Pack matched assets together by their top-level directory.</summary>
        PackByTopDirectory = 3,

        /// <summary>Pack matched assets together by their full directory path.</summary>
        PackByAllDirectories = 4
    }

    /// <summary>
    /// Root rule set consumed by the bundle build pipeline, deserializable from JSON.
    /// </summary>
    [Serializable]
    public class BundleBuildRules
    {
        /// <summary>
        /// Rules matching assets that are loaded directly by gameplay; each match is packed
        /// according to its own <see cref="MainBundleRule.PackType"/>.
        /// </summary>
        public List<MainBundleRule> MainRules { get; } = new List<MainBundleRule>();

        /// <summary>
        /// Rules that group shared dependency assets into common bundles automatically.
        /// </summary>
        public List<AutoGroupRule> GroupRules { get; } = new List<AutoGroupRule>();

        /// <summary>
        /// Deserializes build rules from JSON text.
        /// </summary>
        /// <param name="jsonText">JSON text holding the rule set.</param>
        /// <returns>The deserialized rule set; empty lists when the JSON object lacks the rule arrays.</returns>
        /// <exception cref="BundleException">Thrown when <paramref name="jsonText"/> is not a valid JSON object.</exception>
        public static BundleBuildRules FromJson(string jsonText)
        {
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

    /// <summary>
    /// Rule that matches directly-loaded main assets and decides how they are packed.
    /// </summary>
    [Serializable]
    public class MainBundleRule
    {
        /// <summary>
        /// Name of a <see cref="PackType"/> value parsed by the build pipeline; decides how
        /// every asset matched by this rule is packed.
        /// </summary>
        public string PackType { get; set; }

        /// <summary>
        /// Project-relative path under which assets are searched for matches.
        /// </summary>
        public string SearchPath { get; set; } = "";

        /// <summary>
        /// Include regular expression; only assets whose paths match enter the AssetBundle build.
        /// </summary>
        public string Include { get; set; } = ".+";

        /// <summary>
        /// Exclude regular expression; matched assets are kept out of the AssetBundle build.
        /// </summary>
        public string Exclude { get; set; } = "";

        /// <summary>
        /// Creates a rule from a JSON object, falling back to property defaults for missing keys.
        /// </summary>
        /// <param name="jsonObject">JSON object holding the rule fields.</param>
        /// <returns>The populated rule.</returns>
        internal static MainBundleRule FromJsonObject(JsonObject jsonObject)
        {
            var ret = new MainBundleRule();
            ret.PackType = jsonObject.SafeGetValue(nameof(PackType), ret.PackType);
            ret.SearchPath = jsonObject.SafeGetValue(nameof(SearchPath), ret.SearchPath);
            ret.Include = jsonObject.SafeGetValue(nameof(Include), ret.Include);
            ret.Exclude = jsonObject.SafeGetValue(nameof(Exclude), ret.Exclude);
            return ret;
        }
    }

    /// <summary>
    /// Rule that groups shared dependency assets matching its patterns into a common bundle.
    /// </summary>
    [Serializable]
    public class AutoGroupRule
    {
        /// <summary>
        /// Include regular expression whose captured path parts determine how matched
        /// dependency assets are grouped together.
        /// </summary>
        public string Include { get; set; } = "(.+)";

        /// <summary>
        /// Exclude regular expression; matched dependency assets are left out of auto grouping.
        /// </summary>
        public string Exclude { get; set; } = "";

        /// <summary>
        /// Fallback rule applied when no explicit group rule matches; groups every dependency
        /// asset by its full path.
        /// </summary>
        public static AutoGroupRule Fallback { get; } = new AutoGroupRule { Include = "(.+)" };

        /// <summary>
        /// Creates a rule from a JSON object, falling back to property defaults for missing keys.
        /// </summary>
        /// <param name="jsonObject">JSON object holding the rule fields.</param>
        /// <returns>The populated rule.</returns>
        internal static AutoGroupRule FromJsonObject(JsonObject jsonObject)
        {
            var ret = new AutoGroupRule();
            ret.Include = jsonObject.SafeGetValue(nameof(Include), ret.Include);
            ret.Exclude = jsonObject.SafeGetValue(nameof(Exclude), ret.Exclude);
            return ret;
        }
    }
}