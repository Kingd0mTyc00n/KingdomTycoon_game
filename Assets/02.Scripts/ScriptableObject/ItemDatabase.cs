using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemList",menuName = "Item/ Item List")]
public class ItemDatabase : ScriptableObject
{
    public List<ItemSO> items;
}
