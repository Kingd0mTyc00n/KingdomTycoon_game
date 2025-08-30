using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ObjectPoolAddressable : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public string address;
        public int size;
    }

    public static ObjectPoolAddressable Instance;
    private void Awake() => Instance = this;

    public List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    async void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(pool.address);
                await handle.Task;

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    GameObject obj = Instantiate(handle.Result);
                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                }
                else
                {
                    Debug.LogError($"Failed to load {pool.address}");
                }
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            return null;
        }

        Queue<GameObject> queue = poolDictionary[tag];

        if (queue.Count == 0)
        {
            return null;
        }

        GameObject obj = queue.Dequeue();
        obj.SetActive(true);
        obj.transform.position = position;
        obj.transform.rotation = rotation;

        queue.Enqueue(obj); 

        return obj;
    }

    public void ReturnToPool(string tag, GameObject obj)
    {
        obj.SetActive(false);
        poolDictionary[tag].Enqueue(obj);
    }
}
