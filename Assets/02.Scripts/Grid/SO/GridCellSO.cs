using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public enum CellType
{
    Land,
    Grass
}

[CreateAssetMenu(fileName = "GridCell", menuName = "Grid/ Grid Cell")]
public class GridCellSO : ScriptableObject
{
    public CellType cellType;
    public GameObject cellPrefab;
}
