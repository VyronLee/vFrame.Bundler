// ------------------------------------------------------------
//         File: LinkBase.cs
//        Brief: Base asset link binding a loaded target to its owning Loader; forwards reference
//               counting to the Loader, while subclasses decide exclusivity.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:04:47
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using UnityEngine;

namespace vFrame.Bundler
{
    /// <summary>
    /// Base type of asset links. A link binds an object owned by a <see cref="Loader"/> (the asset
    /// itself or an instance created from it) and forwards <see cref="Retain"/>/<see cref="Release"/>
    /// calls to that loader, keeping it alive as long as the link exists.
    /// </summary>
    public abstract class LinkBase : ILink, IJsonSerializable
    {
        /// <summary>
        /// Frame count at which this link was created; serialized for the profiler, which sorts
        /// and displays links by creation frame.
        /// </summary>
        [JsonSerializableProperty]
        internal int CreateFrame { get; } = Time.frameCount;

        /// <summary>
        /// The loader that owns this link and receives its reference counts.
        /// </summary>
        [JsonSerializableProperty]
        Loader ILink.Loader { get; set; }

        /// <summary>
        /// The linked object: the loaded asset itself, or an instance created from it.
        /// </summary>
        [JsonSerializableProperty]
        Object ILink.Target { get; set; }

        /// <summary>
        /// Whether this link is the only one allowed on its target; determined by the concrete link type.
        /// </summary>
        internal abstract bool Exclusive { get; }

        /// <summary>
        /// Adds a reference through the owning loader, keeping the linked asset alive.
        /// </summary>
        internal void Retain()
        {
            ((ILink)this).Loader?.Retain();
        }

        /// <summary>
        /// Removes one reference through the owning loader; the loader may unload once it reaches zero.
        /// </summary>
        internal void Release()
        {
            ((ILink)this).Loader?.Release();
        }

        /// <summary>
        /// Returns a debug description including the link type, creation frame, exclusivity, target and loader.
        /// </summary>
        /// <returns>A string summarizing this link's state.</returns>
        public override string ToString()
        {
            return $"[@TypeName: {GetType().Name}, CreateFrame: {CreateFrame}, Exclusive: {Exclusive}, Target: {((ILink)this).Target}, Loader: {((ILink)this).Loader}]";
        }
    }
}