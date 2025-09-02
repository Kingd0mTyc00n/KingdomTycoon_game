using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HunterList",menuName = "Game/Hunter List")]
public class HunterDatabase : ScriptableObject
{
    public List<HunterData> hunters = new List<HunterData>();

    public HunterData GetHunterByName(string Name)
    {
        return hunters.Find(h => h.Name == Name);
    }

    
}
