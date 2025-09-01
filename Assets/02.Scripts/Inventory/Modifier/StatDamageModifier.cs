using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatDamageModifier : StatModifier
{
    public override void AffectCharacter(GameObject character, float value)
    {
        //character.GetComponent<PlayerController>().characterData.Damage += value;
    }

    public override void RemoveAffectCharacter(GameObject character, float value)
    {
        //character.GetComponent<PlayerController>().characterData.Damage -= value;
    }
}
