// ------------------------------------------------------------
//         File: AudioSourceLink.cs
//        Brief: Routes loaded AudioClip assets into AudioSource.clip with automatic reference tracking.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-08-15 20:06:25
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================


using UnityEngine;

namespace vFrame.Bundler
{
    public static class AudioSourceLink
    {
        private class AudioClipLink : PropertyLink<AudioSource, AudioClip>
        {
            public override void Set(AudioSource target, AudioClip asset)
            {
                target.clip = asset;
            }
        }

        //============================================================
        // SetClip
        //============================================================
        public static void SetClip(this AudioSource target, Asset asset)
        {
            asset.SetTo<AudioSource, AudioClip, AudioClipLink>(target);
        }
        public static void SetClip(this AudioSource target, AssetAsync asset)
        {
            asset.SetTo<AudioSource, AudioClip, AudioClipLink>(target);
        }
        public static void SetClip(this AudioSource target, Asset<AudioClip> asset)
        {
            asset.SetTo<AudioSource, AudioClipLink>(target);
        }
        public static void SetClip(this AudioSource target, AssetAsync<AudioClip> asset)
        {
            asset.SetTo<AudioSource, AudioClipLink>(target);
        }
    }
}