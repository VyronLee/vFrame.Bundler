using UnityEditor;
using UnityEngine;
using Logger = vBundler.Log.Logger;

namespace vBundler.Editor
{
    public class BundlerPreference : MonoBehaviour
    {
        [PreferenceItem("Bundler")]
        public static void BundlerPreferences()
        {
            var bundleMode = EditorPrefs.GetBool(BundlerSetting.kBundlerModePreferenceKey, false);
            bundleMode = EditorGUILayout.Toggle("Bundle Mode", bundleMode);

            var logLevel = EditorPrefs.GetInt(BundlerSetting.kBundlerLogLevelPreferenceKey, Logger.LogLevel.INFO - 1);
            logLevel = EditorGUILayout.Popup("Log Level", logLevel, new[] {"Verbose", "Info", "Error"});

            if (GUI.changed)
            {
                EditorPrefs.SetBool(BundlerSetting.kBundlerModePreferenceKey, bundleMode);
                EditorPrefs.SetInt(BundlerSetting.kBundlerLogLevelPreferenceKey, logLevel);
            }
        }
    }
}