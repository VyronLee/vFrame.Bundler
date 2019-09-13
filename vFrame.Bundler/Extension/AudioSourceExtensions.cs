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
using vFrame.Bundler.Interface;

namespace vFrame.Bundler.Extension
{
    public static class AudioSourceExtensions
    {
        public static void SetClip(this AudioSource target, IAsset asset)
        {
            asset.SetTo(target, "clip");
        }
    }
}