// ------------------------------------------------------------
//         File: AudioSourceLink.cs
//        Brief: Extension methods assigning loaded AudioClip assets to AudioSource.clip
//               with automatic reference tracking.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 06:29:32
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using UnityEngine;

namespace vFrame.Bundler
{
    /// <summary>
    /// Extension methods that bind loaded <see cref="AudioClip"/> assets to an <see cref="AudioSource"/> clip
    /// property through the link system, which retains the asset until the clip is replaced or the source is destroyed.
    /// </summary>
    public static class AudioSourceLink
    {
        /// <summary>
        /// Property link that assigns an <see cref="AudioClip"/> to <see cref="AudioSource"/>.clip.
        /// </summary>
        private class AudioClipLink : PropertyLink<AudioSource, AudioClip>
        {
            /// <summary>
            /// Assigns the clip to the target audio source.
            /// </summary>
            /// <param name="target">Audio source receiving the clip.</param>
            /// <param name="asset">Clip to assign.</param>
            public override void Set(AudioSource target, AudioClip asset)
            {
                target.clip = asset;
            }
        }

        //============================================================
        // SetClip
        //============================================================

        /// <summary>
        /// Binds the <see cref="AudioClip"/> held by the untyped asset handle to the target's clip,
        /// replacing any previously linked clip on the same source.
        /// </summary>
        /// <param name="target">Audio source whose clip property receives the asset.</param>
        /// <param name="asset">Loaded asset handle; its underlying object must be an <see cref="AudioClip"/>.</param>
        public static void SetClip(this AudioSource target, Asset asset)
        {
            asset.SetTo<AudioSource, AudioClip, AudioClipLink>(target);
        }

        /// <summary>
        /// Binds the <see cref="AudioClip"/> held by the untyped asynchronous asset handle to the target's clip,
        /// replacing any previously linked clip on the same source.
        /// </summary>
        /// <param name="target">Audio source whose clip property receives the asset.</param>
        /// <param name="asset">Asynchronous asset handle; its underlying object must be an <see cref="AudioClip"/>.</param>
        public static void SetClip(this AudioSource target, AssetAsync asset)
        {
            asset.SetTo<AudioSource, AudioClip, AudioClipLink>(target);
        }

        /// <summary>
        /// Binds the clip held by the typed asset handle to the target's clip,
        /// replacing any previously linked clip on the same source.
        /// </summary>
        /// <param name="target">Audio source whose clip property receives the asset.</param>
        /// <param name="asset">Loaded asset handle supplying the <see cref="AudioClip"/>.</param>
        public static void SetClip(this AudioSource target, Asset<AudioClip> asset)
        {
            asset.SetTo<AudioSource, AudioClipLink>(target);
        }

        /// <summary>
        /// Binds the clip held by the typed asynchronous asset handle to the target's clip,
        /// replacing any previously linked clip on the same source.
        /// </summary>
        /// <param name="target">Audio source whose clip property receives the asset.</param>
        /// <param name="asset">Asynchronous asset handle supplying the <see cref="AudioClip"/>.</param>
        public static void SetClip(this AudioSource target, AssetAsync<AudioClip> asset)
        {
            asset.SetTo<AudioSource, AudioClipLink>(target);
        }
    }
}