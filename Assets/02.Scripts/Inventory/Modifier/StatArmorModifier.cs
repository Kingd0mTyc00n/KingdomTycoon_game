using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatArmorModifier : StatModifier
{
    public override void AffectCharacter(GameObject character, float value)
    {
        //character.GetComponent<PlayerController>().characterData.Armor += value;
    }

    public override void RemoveAffectCharacter(GameObject character, float value)
    {
        //character.GetComponent<PlayerController>().characterData.Armor -= value;
    }
}
