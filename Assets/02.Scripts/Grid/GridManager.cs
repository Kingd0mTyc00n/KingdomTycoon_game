using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int gridWidth;
    public int gridHeight;

    public GridCellSO[] gridCells;

    private MapGenerator pathGenerator;

    private void Start()
    {
        pathGenerator = new MapGenerator(gridWidth, gridHeight);
        List<Vector3> mapCells = pathGenerator.GeneratorMap();
        List<Vector3> pathCells = pathGenerator.GeneratorPath();
        List<Vector3> greyMapCells = pathGenerator.GeneratorGreyMap();
        List<Vector3> demonMapCells = pathGenerator.GeneratorDemonMap();

        LayCells(greyMapCells);
        LayDemonCells(demonMapCells);
        LayPathCells(mapCells, pathCells);
    }

    public void LayCells(List<Vector3> mapCells)
    {
        GameObject tile = new GameObject();

        foreach (Vector3 mapCell in mapCells)
        {
            tile = gridCells[2].cellPrefab;
            GameObject cell = Instantiate(tile, new Vector3(mapCell.x, mapCell.y, mapCell.y), Quaternion.identity);
            cell.transform.SetParent(transform);

        }
    }

    public void LayDemonCells(List<Vector3> mapCells)
    {
        GameObject tile = new GameObject();

        foreach (Vector3 mapCell in mapCells)
        {
            tile = gridCells[3].cellPrefab;
            GameObject cell = Instantiate(tile, new Vector3(mapCell.x, mapCell.y, mapCell.y), Quaternion.identity);
            cell.transform.SetParent(transform);

        }
    }

    public void LayPathCells(List<Vector3> mapCells, List<Vector3> pathCells)
    {
        GameObject tile = new GameObject();

        foreach (Vector3 mapCell in mapCells)
        {
            if (pathCells.Contains(mapCell))
            {
                tile = gridCells[1].cellPrefab;
            }
            else
            {
                tile = gridCells[0].cellPrefab;
            }

            GameObject cell = Instantiate(tile, new Vector3(mapCell.x, mapCell.y, mapCell.y), Quaternion.identity);
            cell.transform.SetParent(transform);

        }
    }

}
