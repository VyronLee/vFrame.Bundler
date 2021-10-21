using UnityEngine;

namespace vFrame.Bundler.Extension
{
    public static class GameObjectExtensions
    {
        public static void DestroyEx(this GameObject obj) {
            if (!obj) {
                return;
            }

            if (Application.isPlaying) {
                Object.Destroy(obj);
            }
            else {
                Object.DestroyImmediate(obj);
            }
        }
    }
}