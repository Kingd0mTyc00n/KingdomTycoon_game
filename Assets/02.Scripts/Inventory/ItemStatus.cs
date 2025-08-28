using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ItemStatus : ItemData, IDestroyableItem, IItemAction
{
    [SerializeField] public List<ModifierData> modifierData = new List<ModifierData>();
    public string ActionName => "Consume";

    public AudioClip actionSFX {get; private set;}

    public bool PerformAction(GameObject character)
    {
        foreach (ModifierData modifierData in modifierData)
        {
            //modifierData.StatModifier.AffectCharacter(character, modifierData.Value);
        }
        return true;
    }

    public bool RemoveAction(GameObject character)
    {
        foreach (ModifierData modifierData in modifierData)
        {
           // modifierData.StatModifier.RemoveAffectCharacter(character, modifierData.Value);
        }
        return true;
    }
}
