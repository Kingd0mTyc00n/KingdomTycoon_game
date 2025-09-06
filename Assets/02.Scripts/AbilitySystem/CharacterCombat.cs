using System.Collections.Generic;
using UnityEngine;

public class CharacterCombat : MonoBehaviour
{
    [Header("Skill Setup")]
    public List<Ability> abilities;
    private List<AbilityInstance> abilityInstances = new List<AbilityInstance>();

    [Header("Target")]
    public Transform target;

    void Start()
    {
        foreach (var ability in abilities)
        {
            abilityInstances.Add(new AbilityInstance(ability));
        }
    }

    public void UseRandomSkill()
    {
        int randomIndex = Random.Range(0, abilityInstances.Count);
        AbilityInstance chosenAbility = abilityInstances[randomIndex];

        if (chosenAbility.CanUse())
        {
            chosenAbility.Use(transform, target);
        }
        else
        {
            Debug.Log($"{chosenAbility.ability.abilityName} is cooldown. Trying another ability...");

            foreach (var s in abilityInstances)
            {
                if (s.CanUse())
                {
                    s.Use(transform, target);
                    return;
                }
            }

            Debug.Log("Both abilities are on cooldown!");
        }
    }

    public List<AbilityInstance> GetAbilityInstances()
    {
        return abilityInstances;
    }
}
