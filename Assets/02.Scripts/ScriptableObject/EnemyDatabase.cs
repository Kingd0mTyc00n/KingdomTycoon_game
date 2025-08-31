using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemies", menuName = "Enemy/Enemies")]
public class EnemyDatabase : ScriptableObject
{
    public List<EnemyData> enemies;

    public EnemyData GetEnemyByName(string Name)
    {
        return enemies.Find(h => h.Name == Name);
    }
}
