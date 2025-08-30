using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public CharacterData characterData;

    public float maxHealth = 5;
    public float health;

    public float spawnRadius = 1.5f;
    public float forceAmount = 5f;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => PlayerPrefs.HasKey(GameData.PP_USER_DATA));
        SetEnemyData(UserData.UserDeepData.EnemiesData[0]);
    }
     
    public void SetEnemyData(CharacterData characterData)
    {
        this.characterData = characterData;
        maxHealth = characterData.Health;
        health = characterData.Health;
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
        if (characterData is EnemyData enemyData)
        {
            DropItem(enemyData.ItemsIsDrop);
        }
        ObjectPoolAddressable.Instance.ReturnToPool(gameObject);
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
