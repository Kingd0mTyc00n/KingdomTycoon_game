using UnityEngine;

public class AbilityInstance
{
    public Ability ability;
    private float lastUsedTime = 0f;

    public AbilityInstance(Ability ability)
    {
        this.ability = ability;
        lastUsedTime = -999f;
    }

    public bool CanUse()
    {
        return Time.time - lastUsedTime >= ability.cooldown;
    }

    public void Use(Transform caster, Transform target)
    {
        if (CanUse())
        {
            ability.Activate(caster, target);
            lastUsedTime = Time.time;
        }
        else
        {
            Debug.Log($"{ability.abilityName} đang cooldown!");
        }
    }
}
