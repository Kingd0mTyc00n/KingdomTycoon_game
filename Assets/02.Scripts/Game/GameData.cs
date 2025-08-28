using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public interface IItemAction
{
    public string ActionName { get; }
    public AudioClip actionSFX { get; }
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
    public HunterData CharacterData;
    public InventoryData inventoryData;

    public int Coins;
}

[System.Serializable]
public class HunterData
{
    public string Name;
    public float Damage;
    public float Speed;
    public float Armor;
    public float Health;
    public float Hunger;
}


[System.Serializable]
public class ItemData
{
    public int Id;
    public string Name;
    public string Detail;
    public string Description;
    public int SellingPrice;
    public int PurchasePrice;
    public bool IsStackable;
    public bool IsEdible;
    public bool IsEquippable;
    public EquipmentType EquipmentType;
    public int MaxStackSize;
    public Sprite Icon;
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

    public int Size;

}


[System.Serializable]
public struct InventoryItem
{
    public int Quantity;
    public ItemStatus ItemStatus;
    public bool IsEmpty;

    public InventoryItem ChangeQuantity(int newQuantity)
    {
        return new InventoryItem { ItemStatus = this.ItemStatus, Quantity = newQuantity, };
    }

    public static InventoryItem GetEmptyItem() => new InventoryItem { ItemStatus = null, Quantity = 0, IsEmpty = true };
}


public class UserData
{
    public static UserDeepData UserDeepData;
    public static InventoryData InventoryData;
    public static DateTime GameDateTime;

    public static void SaveData()
    {
        PlayerPrefs.SetString(GameData.PP_USER_DATA, JsonConvert.SerializeObject(UserDeepData));
        PlayerPrefs.SetString(GameData.PP_INVENTORY_DATA, JsonConvert.SerializeObject(InventoryData));
        PlayerPrefs.SetString(GameData.PP_DATETIME_DATA, JsonConvert.SerializeObject(GameDateTime));
        PlayerPrefs.Save();
    }


    public static void LoadData()
    {
        string userJson = PlayerPrefs.GetString(GameData.PP_USER_DATA, "{}");
        UserDeepData = JsonConvert.DeserializeObject<UserDeepData>(userJson);

        Debug.Log(userJson);

        string inventoryJson = PlayerPrefs.GetString(GameData.PP_INVENTORY_DATA, "{}");
        InventoryData = JsonConvert.DeserializeObject<InventoryData>(inventoryJson);

        Debug.Log(inventoryJson);

        string dateTimeJson = PlayerPrefs.GetString(GameData.PP_DATETIME_DATA, "{}");
        GameDateTime = JsonConvert.DeserializeObject<DateTime>(dateTimeJson);

        Debug.Log(dateTimeJson);
    }
}

public class GameData
{
    public const string PP_USER_DATA = "UserData";
    public const string PP_DATETIME_DATA = "DateTimeData";
    public const string PP_INVENTORY_DATA = "InventoryData";
}

