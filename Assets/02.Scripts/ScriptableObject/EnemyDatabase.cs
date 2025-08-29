using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemies", menuName = "Enemy/Enemies")]
public class EnemyDatabase : ScriptableObject
{
    public List<EnemyDataSO> enemies;
}
