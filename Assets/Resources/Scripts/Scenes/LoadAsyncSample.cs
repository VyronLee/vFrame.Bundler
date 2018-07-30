using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using vBundler.Interface;

namespace Sample.Scripts.Scenes
{
    public class LoadAsyncSample : MonoBehaviour
    {
        private const int kMaxBalls = 200;
        private const float kCreateBallsInterval = 0.3f;
        private readonly List<GameObject> balls = new List<GameObject>();

        private readonly List<string> prefabs = new List<string>
        {
            "Assets/Resources/Prefabs/Balls/BlueBall.prefab",
            "Assets/Resources/Prefabs/Balls/GreenBall.prefab",
            "Assets/Resources/Prefabs/Balls/RedBall.prefab",
            "Assets/Resources/Prefabs/Balls/ColorfulBall.prefab",
            "Assets/Resources/Prefabs/Balls/ColorfulBall2.prefab"
        };

        private void Start()
        {
            StartCoroutine(CreateBallsAsync());
        }

        private IEnumerator CreateBallsAsync()
        {
            while (true)
            {
                yield return CreateBallAsync();
                yield return new WaitForSeconds(kCreateBallsInterval);
            }
        }

        private IEnumerator CreateBallAsync()
        {
            var randIdx = Random.Range(0, prefabs.Count);
            IAsset asset = null;
            yield return BundlerFacade.Instance.LoadAsync<GameObject>(prefabs[randIdx], v => asset = v);
            var ball = asset.Instantiate();
            ball.transform.position += (Vector3.up + Vector3.back) * 2;

            if (balls.Count > kMaxBalls)
            {
                Destroy(balls[0]);
                balls.RemoveAt(0);
            }

            balls.Add(ball);
        }
    }
}