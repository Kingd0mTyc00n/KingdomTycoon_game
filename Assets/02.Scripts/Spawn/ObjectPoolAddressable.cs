using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ObjectPoolAddressable : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string label;  // label in Addressables
        public int size;      // number of objects per prefab
    }

    public static ObjectPoolAddressable Instance;
    private void Awake() => Instance = this;

    public List<Pool> pools;

    // Flatten dictionary
    private Dictionary<GameObject, Queue<GameObject>> poolDictionary;
    private Dictionary<string, List<GameObject>> labelToPrefabs;

    public Transform EnemyParentTransform; // Optional parent for spawned enemies

    async void Start()
    {
        poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();
        labelToPrefabs = new Dictionary<string, List<GameObject>>();

        foreach (Pool pool in pools)
        {
            // Load all prefabs with this label
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
                        GameObject obj = Instantiate(prefab, EnemyParentTransform);
                        obj.SetActive(false);

                        // attach PoolMember to know original prefab
                        var member = obj.AddComponent<PoolMember>();
                        member.OriginalPrefab = prefab;

                        objectPool.Enqueue(obj);
                    }
                    poolDictionary[prefab] = objectPool;
                }
                labelToPrefabs[pool.label] = prefabList;
            }
            else
            {
                Debug.LogError($"Failed to load label {pool.label}");
            }
        }
    }

    public GameObject SpawnFromPool(string label, Vector3 position, Quaternion rotation)
    {
        if (!labelToPrefabs.ContainsKey(label))
        {
            Debug.LogError($"No pool with label {label}");
            return null;
        }

        // Random prefab in this label
        List<GameObject> prefabs = labelToPrefabs[label];
        GameObject prefab = prefabs[Random.Range(0, prefabs.Count)];

        if (!poolDictionary.ContainsKey(prefab))
        {
            Debug.LogError($"No pool created for prefab {prefab.name}");
            return null;
        }

        Queue<GameObject> queue = poolDictionary[prefab];

        if (queue.Count == 0)
        {
            // optional: expand pool
            Debug.LogWarning($"Pool for {prefab.name} is empty! Max objects reached.");
            return null;
        }

        GameObject obj = queue.Dequeue();
        obj.SetActive(true);
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        return obj;
    }

    public void ReturnToPool(GameObject obj)
    {
        PoolMember member = obj.GetComponent<PoolMember>();
        if (member == null || member.OriginalPrefab == null)
        {
            Debug.LogWarning($"Object {obj.name} does not belong to any pool");
            Destroy(obj);
            return;
        }

        obj.SetActive(false);
        poolDictionary[member.OriginalPrefab].Enqueue(obj);
    }
}

public class PoolMember : MonoBehaviour
{
    public GameObject OriginalPrefab; // keep track of the prefab it came from
}
