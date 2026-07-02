using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class ObjectPool
    {
        private readonly GameObject prefab;
        private readonly int preWarmCount;
        private readonly Stack<GameObject> pool = new();

        public ObjectPool(GameObject prefab, int preWarmCount)
        {
            this.prefab = prefab;
            this.preWarmCount = preWarmCount;
        }

        public void PreWarm()
        {
            for (int i = 0; i < preWarmCount; i++)
            {
                var obj = Object.Instantiate(prefab);
                obj.SetActive(false);
                pool.Push(obj);
            }
        }

        public GameObject Checkout(Vector2 position, Quaternion rotation)
        {
            GameObject obj;
            if (pool.Count > 0)
            {
                obj = pool.Pop();
            }
            else
            {
                Debug.LogWarning($"ObjectPool exhausted for '{prefab.name}', falling back to Instantiate");
                obj = Object.Instantiate(prefab);
            }

            obj.transform.SetPositionAndRotation(position, rotation);
            obj.SetActive(true);
            return obj;
        }

        public void Return(GameObject obj)
        {
            obj.SetActive(false);
            pool.Push(obj);
        }

        public void ReturnAll()
        {
            while (pool.Count > 0)
            {
                var obj = pool.Pop();
                Object.Destroy(obj);
            }
        }
    }
}
