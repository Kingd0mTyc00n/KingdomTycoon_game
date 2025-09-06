using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

public enum HunterClass
{
    Ranger,
    Sorcerer,
}

[System.Serializable]
public class UserDeepData
{
    public List<HunterData> HuntersData;
    public List<ItemData> ItemsData;
    public List<Equipment> EquipmentData;

    public int InventorySize;

    public int Coins;
    public int Gems;
    public int Soul;
    public int Fool;
    public int Shard;

    public int HuntersNum;
}

[System.Serializable]
public class HunterData : CharacterData
{
    public float Hunger;

    public int Exp;
    public int DPSParam;
    public int ATK;
    public int Defense;
    public int Crit;
    public int ATKSPD;
    public float Evasion;
    public HunterType HunterType;
    public int Level;
    public string SpritePath;
    public InventoryData InventoryData;

    public static HunterData CreateFromSO(HunterData template)
    {
        return new HunterData
        {
            Id = template.Id,
            Name = template.Name,
            Damage = template.Damage,
            Speed = template.Speed,
            Armor = template.Armor,
            Health = template.Health,

            Hunger = template.Hunger,
            Exp = 0,
            DPSParam = template.DPSParam,
            ATK = template.ATK,
            Defense = template.Defense,
            Crit = template.Crit,
            ATKSPD = template.ATKSPD,
            Evasion = template.Evasion,
            HunterType = template.HunterType,
            Level = 1,

            SpritePath = template.SpritePath,

            InventoryData = new InventoryData()
        };
    }
}

public enum HunterType
{
    Common,
    Rare,
    Epic,
    Legendary
}

[System.Serializable]
public class EnemyData : CharacterData
{
    public List<ItemData> ItemsIsDrop;
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
    public ItemType ItemType;
    public int MaxStackSize;
    public Sprite icon;
}

public enum ItemType
{
    Material,
    Edible,
    Equippable
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
    public ItemData Item;
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
    public ItemData Item;
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

