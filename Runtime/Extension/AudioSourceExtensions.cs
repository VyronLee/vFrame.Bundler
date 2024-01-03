//------------------------------------------------------------
//        File:  AudioSourceExtensions.cs
//       Brief:  AudioSourceExtensions
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 20:04
//   Copyright:  Copyright (c) 2018, VyronLee
//============================================================

using UnityEngine;

namespace vFrame.Bundler
{
    public static class AudioSourceExtensions
    {
        private class AudioClipSetter : PropertySetterProxy<AudioSource, AudioClip>
        {
            public override void Set(AudioSource target, AudioClip asset) {
                target.clip = asset;
            }
        }

        public static void SetClip(this AudioSource target, IAsset asset) {
            asset.SetTo<AudioSource, AudioClip, AudioClipSetter>(target);
        }
    }
}