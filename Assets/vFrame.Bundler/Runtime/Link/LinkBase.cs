// ------------------------------------------------------------
//         File: LinkBase.cs
//        Brief: LinkBase.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-5 12:1
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using UnityEngine;

namespace vFrame.Bundler
{
    public abstract class LinkBase : ILink, IJsonSerializable
    {
        private readonly int _createFrame = Time.frameCount;

        [JsonSerializableProperty]
        internal int CreateFrame => _createFrame;

        [JsonSerializableProperty]
        Loader ILink.Loader { get; set; }

        [JsonSerializableProperty]
        Object ILink.Target { get; set; }

        internal abstract bool Exclusive { get; }

        internal void Retain() {
            ((ILink)this).Loader?.Retain();
        }

        internal void Release() {
            ((ILink)this).Loader?.Release();
        }
    }
}