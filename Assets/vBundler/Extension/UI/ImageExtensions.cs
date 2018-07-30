using UnityEngine.UI;
using vBundler.Interface;

namespace vBundler.Extension.UI
{
    public static class ImageExtensions
    {
        public static void SetSprite(this Image target, IAsset asset)
        {
            asset.SetTo(target, "sprite");
        }

        public static void SetOverrideSprite(this Image target, IAsset asset)
        {
            asset.SetTo(target, "overrideSprite");
        }

        public static void SetMaterial(this Image target, IAsset asset)
        {
            asset.SetTo(target, "material");
        }
    }
}