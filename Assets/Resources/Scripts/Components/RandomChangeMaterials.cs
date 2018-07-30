using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sample.Scripts.Components
{
    public class RandomChangeMaterials : MonoBehaviour
    {
        private const float kChangeInterval = 1f;
        private MeshRenderer _render;
        public List<Material> materials = new List<Material>();

        private void Start()
        {
            _render = GetComponent<MeshRenderer>();

            StartCoroutine(AutoChangeColorProcess());
        }

        private IEnumerator AutoChangeColorProcess()
        {
            while (true)
            {
                yield return new WaitForSeconds(kChangeInterval);

                var rand = Random.Range(0, materials.Count);
                _render.material = materials[rand];
            }
        }
    }
}