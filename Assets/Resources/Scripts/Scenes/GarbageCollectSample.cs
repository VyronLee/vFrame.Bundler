using System.Collections;
using UnityEngine;
using Logger = vBundler.Log.Logger;

namespace Sample.Scripts.Scenes
{
    public class GarbageCollectSample : MonoBehaviour
    {
        private const string PrefabName = "Assets/Resources/Prefabs/Balls/ColorfulBall2.prefab";

        private IEnumerator Start()
        {
            BundlerFacade.Instance.SetLogLevel(Logger.LogLevel.VERBOSE);

            Debug.Log("=====================================");
            Debug.Log("Load GameObject: " + PrefabName);
            Debug.Log("=====================================");
            var asset = BundlerFacade.Instance.Load<GameObject>(PrefabName);
            var go = asset.Instantiate();
            yield return new WaitForSeconds(0.5f);

            Debug.Log("=====================================");
            Debug.Log("Destroy GameObject: " + PrefabName);
            Debug.Log("=====================================");
            Destroy(go);
            yield return new WaitForSeconds(0.5f);

            Debug.Log("=====================================");
            Debug.Log("Load GameObject again: " + PrefabName);
            Debug.Log("=====================================");
            asset = BundlerFacade.Instance.Load<GameObject>(PrefabName);
            go = asset.Instantiate();
            yield return new WaitForSeconds(10f);

            Debug.Log("=====================================");
            Debug.Log("Destroy GameObject again: " + PrefabName);
            Debug.Log("=====================================");
            Destroy(go);
        }
    }
}