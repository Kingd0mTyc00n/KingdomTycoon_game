using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public interface IItemAction
{
    public string ActionName { get; }
    bool PerformAction(GameObject character);
    bool RemoveAction(GameObject character);
}

public interface IDestroyableItem
{
    bool PerformAction(GameObject character);
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

[System.Serializable]
public class UserDeepData
{
    public List<HunterData> HuntersData;
    public List<ItemStatus> ItemsData;
    public List<EnemyData> EnemiesData;
    public int InventorySize;

    public int Coins;
}

[System.Serializable]
public class HunterData:CharacterData
{
    public float Hunger;
    public InventoryData InventoryData;
}

[System.Serializable]
public class EnemyData: CharacterData
{
    public List<ItemStatus> ItemsIsDrop;
}

public class CharacterData
{
    public int Id;
    public string Name;
    public float Damage;
    public float Speed;
    public float Armor;
    public float Health;
}   


[System.Serializable]
public class ItemData
{
    public int Id;
    public string Name;
    public string Detail;
    public string Description;
    public bool IsStackable;
    public bool IsEdible;
    public bool IsEquippable;
    public EquipmentType EquipmentType;
    public int MaxStackSize;
}

[System.Serializable]
public class ModifierData
{
    //public StatModifier StatModifier;
    public float Value;
}


[System.Serializable]
public class ItemSlot
{
    public int SlotId;
    public EquipmentType EquipmentType;
    public int QuantityOfItem;
    public ItemStatus EdibleItem;
}


[System.Serializable]
public class InventoryData
{
    public List<InventoryItem> InventoryItems;
}   


[System.Serializable]
public struct InventoryItem
{
    public int Quantity;
    public ItemStatus Item;
    public bool IsEmpty;

    public InventoryItem ChangeQuantity(int newQuantity)
    {
        return new InventoryItem { Item = this.Item, Quantity = newQuantity, };
    }

    public static InventoryItem GetEmptyItem() => new InventoryItem { Item = null, Quantity = 0, IsEmpty = true };
}


public class UserData
{
    public static UserDeepData UserDeepData;

    public static void SaveData()
    {
        PlayerPrefs.SetString(GameData.PP_USER_DATA, JsonConvert.SerializeObject(UserDeepData));
        PlayerPrefs.Save();
    }


    public static void LoadData()
    {
        string userJson = PlayerPrefs.GetString(GameData.PP_USER_DATA, "{}");
        UserDeepData = JsonConvert.DeserializeObject<UserDeepData>(userJson);

        Debug.Log(userJson);

    }
}

public class GameData
{
    public const string PP_USER_DATA = "UserData";
}

