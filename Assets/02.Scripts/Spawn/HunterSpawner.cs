using System.Collections.Generic;
using UnityEngine;

public class HunterSpawner : MonoBehaviour
{

    public static HunterSpawner Instance;

    public string hunterLabel;
    public Transform spawnPoint;
    public Transform townTransform;
    public int maxHunter = 6;
    public float spawnInterval = 1f;

    public int hunterNum = 0;
    private float timer = 0f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (hunterNum >= maxHunter) return;
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnNextHunterFromGacha();
            timer = 0f;
        }
    }

    private Queue<HunterData> gachaQueue = new Queue<HunterData>();

    public void AddHunterFromGacha(HunterData hunterData)
    {
        gachaQueue.Enqueue(hunterData);
    }

    private void SpawnNextHunterFromGacha()
    {
        if (hunterNum >= maxHunter) return;
        if (spawnPoint == null) return;
        if (gachaQueue.Count == 0) return;

        HunterData hunterData = gachaQueue.Dequeue();

        GameObject prefab = ObjectPoolAddressable.Instance.GetPrefabByName(
            hunterData.Name,
            hunterData.Name
        );

        if (prefab == null) return;
        Debug.Log("Spawning Hunter from Gacha");

        GameObject hunter = ObjectPoolAddressable.Instance.SpawnFromPool(
            hunterData.Name,
            prefab,
            spawnPoint.position,
            Quaternion.identity
        );

        if (hunter == null) return;

        hunterNum++;

        var hunterComp = hunter.GetComponent<HunterController>();
        if (hunterComp != null)
        {
            Debug.Log("Setting Hunter Data and Sending to Town");
            hunterComp.SetCharacterData(hunterData);
            hunterComp.SetTown(townTransform);
            hunterComp.GoToTownAndHeal();
        }
    }

    public void OnHunterDied(GameObject hunter)
    {
        ObjectPoolAddressable.Instance.ReturnToPool(hunter);
        hunterNum = Mathf.Max(0, hunterNum - 1);
    }
}
