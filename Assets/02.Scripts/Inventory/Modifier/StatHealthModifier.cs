using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatHealthModifier : StatModifier
{
    public override void AffectCharacter(GameObject character, float value)
    {
        /*HealthBar healthBar = character.GetComponent<HealthBar>();
        if (healthBar != null)
        {
            healthBar.Heal(value);
        }*/
    }

    public override void RemoveAffectCharacter(GameObject character, float value)
    {
        throw new System.NotImplementedException();
    }
}
