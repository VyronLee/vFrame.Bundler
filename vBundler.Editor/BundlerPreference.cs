//------------------------------------------------------------
//        File:  BundlerPreference.cs
//       Brief:  BundlerPreference
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-07-09 10:40
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================
using UnityEditor;
using UnityEngine;
using Logger = vBundler.Logs.Logger;

namespace vBundler.Editor
{
    public class BundlerPreference : MonoBehaviour
    {
        [PreferenceItem("vBundler")]
        public static void BundlerPreferences()
        {
            var bundleMode = EditorPrefs.GetBool(BundlerDefaultBuildSettings.kModePreferenceKey, false);
            bundleMode = EditorGUILayout.Toggle("Bundle Mode", bundleMode);

            var logLevel = EditorPrefs.GetInt(BundlerDefaultBuildSettings.kLogLevelPreferenceKey, Logger.LogLevel.INFO - 1);
            logLevel = EditorGUILayout.Popup("Log Level", logLevel, new[] {"Verbose", "Info", "Error"});

            if (GUI.changed)
            {
                EditorPrefs.SetBool(BundlerDefaultBuildSettings.kModePreferenceKey, bundleMode);
                EditorPrefs.SetInt(BundlerDefaultBuildSettings.kLogLevelPreferenceKey, logLevel);
            }
        }
    }
}