using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item",menuName = "Item/ Item Data")]
public class ItemSO : ScriptableObject
{
    public ItemStatus itemData;
    public Sprite icon;
}
