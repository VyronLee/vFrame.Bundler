// ------------------------------------------------------------
//         File: OracleAsset.cs
//        Brief: Provides serialized content whose layout changes only between Editor processes.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-10-18 18:51:08
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================

using System;
using System.IO;
using UnityEngine;

namespace vFrame.Bundler.OptionOracle
{
    /// <summary>Provides stable content, a reference, and a build-scoped serialization error stimulus.</summary>
    public sealed class OracleAsset : ScriptableObject, ISerializationCallbackReceiver
    {
        public int value;
        public OracleAsset link;
        // ORACLE_LAYOUT_FIELD

        /// <summary>Emits an error only while the isolated driver owns the injection window.</summary>
        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            var gate = Environment.GetEnvironmentVariable("VFRAME_ORACLE_INJECT");
            if (!string.IsNullOrEmpty(gate) && File.Exists(gate)) {
                Debug.LogError("VFRAME_ORACLE_INJECTED: serialization value=" + value);
            }
#endif
        }

        /// <summary>Does not change loaded data or retain process state.</summary>
        public void OnAfterDeserialize() { }
    }
}