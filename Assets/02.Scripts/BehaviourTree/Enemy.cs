using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHealth = 50f;
    private float health;

    void Awake()
    { 
        health = maxHealth; 
    }

    public void TakeDamage(float d)
    {
        health -= d;
        if (health <= 0) Die();
    }

    public bool IsDead() => health <= 0;

    private void Die()
    {
        Destroy(gameObject);
    }
}
