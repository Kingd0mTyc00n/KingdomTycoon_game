using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Equipment : ItemData, IDestroyableItem, IItemAction
{
    public EquipmentType EquipmentType;
    [SerializeField] public List<ModifierData> modifierData = new List<ModifierData>();

    public string ActionName => "Consume";

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

public enum EquipmentType
{
    None,
    Helmet,
    Armor,
    Weapon,
    Necklace,
    Ring,
    Boots
}
