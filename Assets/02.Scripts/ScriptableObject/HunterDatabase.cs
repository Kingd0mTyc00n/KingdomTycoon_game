using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HunterList",menuName = "Game/Hunter List")]
public class HunterDatabase : ScriptableObject
{
    public List<HunterDataSO> hunters;
}
