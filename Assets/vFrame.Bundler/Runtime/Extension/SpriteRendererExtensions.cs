//------------------------------------------------------------
//        File:  SpriteRendererExtensions.cs
//       Brief:  SpriteRendererExtensions
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2020-05-21 16:20
//   Copyright:  Copyright (c) 2020, VyronLee
//============================================================

using UnityEngine;
using vFrame.Bundler.Base;
using vFrame.Bundler.Interface;

namespace vFrame.Bundler.Extension
{
    public static class SpriteRendererExtensions
    {
        private class SpriteSetter : PropertySetterProxy<SpriteRenderer, Sprite>
        {
            public override void Set(SpriteRenderer target, Sprite asset) {
                target.sprite = asset;
            }
        }

        public static void SetSprite(this SpriteRenderer target, IAsset asset) {
            asset.SetTo<SpriteRenderer, Sprite, SpriteSetter>(target);
        }
    }
}