using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ObjectPoolAddressable : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string label;
        public int size;
    }

    public static ObjectPoolAddressable Instance;
    private void Awake() => Instance = this;

    public List<Pool> pools;

    private Dictionary<GameObject, Queue<GameObject>> poolDictionary;
    private Dictionary<string, List<GameObject>> labelToPrefabs;

    public Transform DefaultParent;

    async void Start()
    {
        poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();
        labelToPrefabs = new Dictionary<string, List<GameObject>>();

        foreach (Pool pool in pools)
        {
            AsyncOperationHandle<IList<GameObject>> handle = Addressables.LoadAssetsAsync<GameObject>(
                pool.label,
                null
            );
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                List<GameObject> prefabList = new List<GameObject>();
                foreach (var prefab in handle.Result)
                {
                    prefabList.Add(prefab);

                    Queue<GameObject> objectPool = new Queue<GameObject>();
                    for (int i = 0; i < pool.size; i++)
                    {
                        GameObject obj = Instantiate(prefab, DefaultParent);
                        obj.SetActive(false);

                        var member = obj.AddComponent<PoolMember>();
                        member.OriginalPrefab = prefab;

                        objectPool.Enqueue(obj);
                    }
                    poolDictionary[prefab] = objectPool;
                }
                labelToPrefabs[pool.label] = prefabList;
            }
        }
    }

    public GameObject SpawnFromPool(string label, Vector3 position, Quaternion rotation)
    {
        if (!labelToPrefabs.ContainsKey(label)) return null;
        List<GameObject> prefabs = labelToPrefabs[label];
        GameObject prefab = prefabs[Random.Range(0, prefabs.Count)];
        return SpawnFromPool(label, prefab, position, rotation);
    }

    public GameObject SpawnFromPool(string label, GameObject prefabOverride, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(prefabOverride)) return null;
        Queue<GameObject> queue = poolDictionary[prefabOverride];
        if (queue.Count == 0) return null;
        GameObject obj = queue.Dequeue();
        obj.SetActive(true);
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        return obj;
    }

    public GameObject GetPrefabByName(string label, string prefabName)
    {
        if (!labelToPrefabs.ContainsKey(label)) return null;
        return labelToPrefabs[label].Find(p => p.name == prefabName);
    }

    public void ReturnToPool(GameObject obj)
    {
        PoolMember member = obj.GetComponent<PoolMember>();
        if (member == null || member.OriginalPrefab == null)
        {
            Destroy(obj);
            return;
        }
        obj.SetActive(false);
        poolDictionary[member.OriginalPrefab].Enqueue(obj);
    }
}

public class PoolMember : MonoBehaviour
{
    public GameObject OriginalPrefab;
}
