using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyData enemyData;

    public float maxHealth = 5;
    public float health;

    public float spawnRadius = 1.5f;
    public float forceAmount = 5f;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => PlayerPrefs.HasKey(GameData.PP_USER_DATA));
        SetEnemyData(UserData.UserDeepData.EnemiesData[0]);
    }

    public void SetEnemyData(EnemyData enemyData)
    {
        this.enemyData = enemyData;
        maxHealth = enemyData.Health;
        health = enemyData.Health;
    }

    public void TakeDamage(float d)
    {
        health -= d;
        if (health <= 0)
        {
            health = 0;
            Die();
        }
    }

    public bool IsDead() => health <= 0;

    private void Die()
    {
        DropItem(enemyData.ItemsIsDrop);
        Destroy(gameObject);
    }

    private void DropItem(List<ItemStatus> items)
    {
        for (int i = 0; i < items.Count; i++)
        {
            GameObject item = Instantiate(GameResources.instance.dropItem, transform.position, Quaternion.identity);
            item.GetComponent<Item>().SetData(items[i]);
            Vector2 randomDirection = UnityEngine.Random.insideUnitCircle.normalized;

            Rigidbody2D rb = item.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(randomDirection * forceAmount, ForceMode2D.Impulse);
                rb.linearDamping = 3f;
            }
        }
        
    }
}
