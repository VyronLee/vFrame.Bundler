//------------------------------------------------------------
//        File:  BundlerMessengerInspector.cs
//       Brief:  BundlerMessengerInspector
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-07-09 16:46
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using vBundler.Assets;
using vBundler.Loaders;
using vBundler.Messengers;

namespace vBundler.Editor.Inspector
{
    [CustomEditor(typeof(BundlerMessenger))]
    public class BundlerMessengerInspector : UnityEditor.Editor
    {
        private BundlerMessenger _messenger;
        private FieldInfo _assetsFieldInfo;
        private FieldInfo _targetFieldInfo;
        private FieldInfo _pathFieldInfo;

        private void OnEnable()
        {
            _messenger = (BundlerMessenger) target;

            var tBundlerMessenger = typeof(BundlerMessenger);
            _assetsFieldInfo = tBundlerMessenger.GetField("_assets", BindingFlags.Instance | BindingFlags.NonPublic);

            var tAssetBase = typeof(AssetBase);
            _targetFieldInfo = tAssetBase.GetField("_target", BindingFlags.Instance | BindingFlags.NonPublic);

            var tBundleLoaderBase = typeof(BundleLoaderBase);
            _pathFieldInfo = tBundleLoaderBase.GetField("_path", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("AssetBundle References:");

            var assets = _assetsFieldInfo.GetValue(_messenger) as List<AssetBase>;
            if (assets == null)
            {
                EditorGUILayout.EndVertical();
                return;
            }

            EditorGUILayout.BeginVertical(GUI.skin.box);

            var abNames = new HashSet<string>();

            var index = 0;
            foreach (var assetBase in assets)
            {
                var loader = _targetFieldInfo.GetValue(assetBase) as BundleLoaderBase;
                if (loader == null)
                    continue;

                var path = _pathFieldInfo.GetValue(loader) as string;
                if (string.IsNullOrEmpty(path))
                    continue;

                // Multiple assets may reference on the same loader.
                if (abNames.Contains(path))
                    continue;
                abNames.Add(path);

                EditorGUILayout.LabelField(string.Format("{0}) {1}", ++index, path));

                foreach (var loaderDependency in loader.Dependencies)
                {
                    path = _pathFieldInfo.GetValue(loaderDependency) as string;
                    if (string.IsNullOrEmpty(path))
                        continue;

                    EditorGUILayout.LabelField("    + " + path);
                }
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();
        }
    }
}