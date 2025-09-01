using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class StatModifier: MonoBehaviour
{
    public abstract void AffectCharacter(GameObject character, float value);

    public abstract void RemoveAffectCharacter(GameObject character, float value);
}
