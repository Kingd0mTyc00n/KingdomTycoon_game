using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemList",menuName = "Item/ Item List")]
public class ItemDatabase : ScriptableObject
{
    public List<ItemData> items;
    public List<Equipment> equipment;

    public ItemData GetItemById(int id)
    {
        return items.Find(i => i.Id == id);
    }
    public Equipment GetEquipmentById(int id)
    {
        return equipment.Find(i => i.Id == id);
    }
}
