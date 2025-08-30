using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public string enemyTag;
    public float spawnInterval = 2f;
    public int totalEnemies = 7;
    public Transform[] spawnPoints;

    private int numEnemies = 0;
    private float timer;

    void Update()
    {
        if (numEnemies < totalEnemies)
        {
            timer += Time.deltaTime;
            if (timer >= spawnInterval)
            {
                SpawnEnemy();
                numEnemies++;
                timer = 0;
            }
        }

    }

    void SpawnEnemy()
    {
        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
        ObjectPoolAddressable.Instance.SpawnFromPool(enemyTag, point.position, Quaternion.identity);
    }
}
