using UnityEngine;

public enum AbilitySpawnType
{
    OnCaster,
    OnTarget
}

[CreateAssetMenu(fileName = "NewAbility", menuName = "Ability System/Ability")]
public class Ability : ScriptableObject
{
    public string abilityName;
    public float damage;
    public float cooldown;
    public GameObject effectPrefab;

    public AbilitySpawnType spawnType = AbilitySpawnType.OnCaster;

    public virtual void Activate(Transform caster, Transform target)
    {
        if (effectPrefab == null) return;

        Vector3 spawnPosition = caster.position;
        Quaternion rotation = caster.rotation;

        switch (spawnType)
        {
            case AbilitySpawnType.OnTarget:
                if (target != null)
                {
                    spawnPosition = target.position;
                    rotation = Quaternion.identity;
                }
                break;

            case AbilitySpawnType.OnCaster:
                if (target != null)
                {
                    spawnPosition = caster.position + caster.forward * 1.5f;
                    rotation = Quaternion.LookRotation(caster.forward);
                }
                break;
        }

        GameObject effect = Instantiate(effectPrefab, spawnPosition, rotation);
        effect.transform.localScale = Vector3.one * 0.5f;

        // Sử dụng base class để handle cả Hunter và Enemy
        var characterController = target.GetComponent<BaseCharacterController>();
        if (characterController != null)
            characterController.TakeDamage(damage);

    }
}
