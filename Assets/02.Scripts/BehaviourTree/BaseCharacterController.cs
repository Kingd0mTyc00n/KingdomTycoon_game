using FishNet.Demo.AdditiveScenes;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class BaseCharacterController : MonoBehaviour
{
    [SerializeField] protected CharacterData characterData;
    [SerializeField] protected HealthBar healthBar;
    protected CharacterCombat characterCombat;

    [Header("Base Stats")]
    public float attackDamage;
    public float lastAttackTime = 0f;
    public float attackSpeed = 1f;
    public float moveSpeed = 3f;
    public float arriveTolerance = 0.1f;
    public float attackRange = 1f;

    [Header("Current State")]
    public Transform currentTarget;

    [Header("Animation & Visual")]
    public PlayerObj characterObj;

    [Header("Physics")]
    public float spawnRadius = 1.5f;
    public float forceAmount = 5f;

    protected virtual void Awake()
    {
        if (healthBar == null)
            healthBar = GetComponentInChildren<HealthBar>();
        if (characterObj == null)
            characterObj = GetComponent<PlayerObj>();
        if (characterCombat == null)
            characterCombat = GetComponent<CharacterCombat>();

    }

    protected virtual void InitializeCharacter()
    {
        if (characterData != null)
        {
            healthBar.SetMaxHealth(characterData.Health);
            attackDamage = characterData.Damage;
        }
    }

    public virtual void SetCharacterData(CharacterData data)
    {
        characterData = data;
        InitializeCharacter();
    }

    #region Health & Combat
    public bool IsHealthLow() => healthBar.IsHealthLow();
    public bool IsDead() => healthBar.currentHealth <= 0;

    public virtual void TakeDamage(float damage)
    {
        healthBar.currentHealth -= damage;
        if (healthBar.currentHealth <= 0)
        {
            healthBar.currentHealth = 0;
            Die();
        }
        healthBar.UpdateProgressBar();
    }

    protected virtual void Die()
    {
        ObjectPoolAddressable.Instance.ReturnToPool(gameObject);
    }

    protected virtual bool CanAttack()
    {
        return Time.time - lastAttackTime >= attackSpeed;
    }

    protected virtual void PerformAttack(BaseCharacterController target)
    {
        characterObj.SetAttack();
        target.TakeDamage(attackDamage);
        healthBar.UpdateProgressBar();
        lastAttackTime = Time.time;
    }
    #endregion

    #region Movement
    protected bool IsAtPosition(Vector3 targetPos)
    {
        return Vector2.Distance(transform.position, targetPos) <= arriveTolerance;
    }

    protected void MoveToPosition(Vector3 targetPos)
    {
        if (characterObj == null)
        {
            Debug.LogError($"[BaseCharacterController] characterObj is null on {gameObject.name}! Cannot move.");
            return;
        }
        characterObj.SetMovePos(targetPos);
    }

    protected void StopMovement()
    {
        if (characterObj == null)
        {
            Debug.LogError($"[BaseCharacterController] characterObj is null on {gameObject.name}! Cannot stop movement.");
            return;
        }
        characterObj.SetIdle();
    }
    #endregion

    #region Abstract Methods - Must be implemented by subclasses
    public abstract NodeState FindTarget();
    public abstract NodeState MoveToTarget();
    public abstract NodeState AttackTarget();
    #endregion

    #region Virtual Methods - Can be overridden by subclasses
    public virtual NodeState IdleMovement()
    {
        return NodeState.Failure;
    }
    #endregion
}
