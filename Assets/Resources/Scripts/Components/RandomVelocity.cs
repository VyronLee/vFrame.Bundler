using UnityEngine;

namespace Sample.Scripts.Components
{
    public class RandomVelocity : MonoBehaviour
    {
        private void Start()
        {
            var randx = Random.Range(-1f, 1f) * 10;
            var randz = Random.Range(5f, 10f);
            GetComponent<Rigidbody>().velocity = new Vector3(randx, 0, randz);
        }
    }
}