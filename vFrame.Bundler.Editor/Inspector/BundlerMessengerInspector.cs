//------------------------------------------------------------
//        File:  BundlerMessengerInspector.cs
//       Brief:  BundlerMessengerInspector
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-07-09 16:46
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using vFrame.Bundler.Assets;
using vFrame.Bundler.Loaders;
using vFrame.Bundler.Messengers;

namespace vFrame.Bundler.Editor.Inspector
{
    [CustomEditor(typeof(BundlerMessenger))]
    public class BundlerMessengerInspector : UnityEditor.Editor
    {
        private BundlerMessenger _messenger;
        private FieldInfo _assetsFieldInfo;
        private FieldInfo _typedAssetsFieldInfo;
        private SerializedProperty _instanceId;

        private void OnEnable()
        {
            _messenger = (BundlerMessenger) target;

            _instanceId = serializedObject.FindProperty("_instanceId");

            var tBundlerMessenger = typeof(BundlerMessenger);
            _assetsFieldInfo = tBundlerMessenger.GetField("_assets", BindingFlags.Instance | BindingFlags.NonPublic);
            _typedAssetsFieldInfo = tBundlerMessenger.GetField("_typedAssets", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.PropertyField(_instanceId);

            var assets = _assetsFieldInfo.GetValue(_messenger) as HashSet<AssetBase>;
            var typedAssets = _typedAssetsFieldInfo.GetValue(_messenger) as Dictionary<Type, AssetBase>;

            EditorGUILayout.BeginVertical(GUI.skin.box);

            var uniqueAssets = new HashSet<AssetBase>();
            if (null != assets) {
                foreach (var asset in assets) {
                    uniqueAssets.Add(asset);
                }
            }

            if (null != typedAssets) {
                foreach (var kv in typedAssets) {
                    uniqueAssets.Add(kv.Value);
                }
            }

            void DrawLoader(BundleLoaderBase loader, int indentLevel) {
                var prevIndent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = indentLevel;
                EditorGUILayout.LabelField(loader.AssetBundlePath);
                EditorGUI.indentLevel = prevIndent;

                if (null == loader.Dependencies)
                    return;

                foreach (var loaderDependency in loader.Dependencies) {
                    DrawLoader(loaderDependency, indentLevel + 1);
                }
            }

            foreach (var asset in uniqueAssets) {
                EditorGUILayout.LabelField(asset.AssetPath);

                if (null != asset.Loader) {
                    DrawLoader(asset.Loader, 2);
                }
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();
        }
    }
}