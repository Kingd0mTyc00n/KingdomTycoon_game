using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MapGenerator
{
    private int width, height;

    private List<Vector3> mapCellIndexes;
    private List<Vector3> pathCellIndexes;
    private List<Vector3> greyCellIndexes;
    private List<Vector3> demonCellIndexes;
    private List<Vector3> rockWallCellIndexes;

    public MapGenerator(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    //colonial

    public List<Vector3> GeneratorDemonMap()
    {
        demonCellIndexes = new List<Vector3>();

        for (int x = width -1; x >= width / 2 + width / 10 + 3; x--)
        {
            for (int y = height - 1; y >= 0; y--)
            { 
                if (x <= width * 0.8f + 2 && y >= height / 2)
                    continue;

                float isoX = (x - y) * 1f / 2f;
                float isoY = (x + y) * 0.5f / 2f;
                Vector3 pos = new Vector3(isoX, isoY, 0);

                demonCellIndexes.Add(pos);
            }
        }

        return demonCellIndexes;
    }

    public List<Vector3> GeneratorGreyMap()
    {
        greyCellIndexes = new List<Vector3>();

        for (int x = width/2 + width/10; x >= 0; x--)
        {
            for (int y = height - 1; y >= 0; y--)
            {
                if (x >= width * 0.4f && y >= height / 2)
                    continue;

                float isoX = (x - y) * 1f / 2f;
                float isoY = (x + y) * 0.5f / 2f;
                Vector3 pos = new Vector3(isoX, isoY, 0);

                greyCellIndexes.Add(pos);
            }
        }

        return greyCellIndexes;
    }

    public List<Vector3> GeneratorMap()
    {
        mapCellIndexes = new List<Vector3>();

        for (int x = (width * 4)/5; x >= (width * 2)/5 + 3; x--)
        {
            for (int y = height - 1; y >= height/2 + 2; y--)
            {
                float isoX = (x - y) * 1f / 2f;
                float isoY = (x + y) * 0.5f / 2f;
                Vector3 pos = new Vector3(isoX, isoY, 0);

                mapCellIndexes.Add(pos);
            }
        }
        return mapCellIndexes;
    }

    public List<Vector3> GeneratorPath()
    {
        pathCellIndexes = new List<Vector3>();


        int left = (width/2 -2) / 3;           
        int right = (width / 2 - 2) - (width / 2 + 2) / 2;    
        int bottom = (height/2 + 2) / 2;         
        int top = (height / 2 + 2) - (height / 2 + 2) / 3;   

        for (int x = left; x <= right; x++)
        {
            float isoX = (x - top) * 1f / 2f;
            float isoY = (x + top) * 0.5f / 2f;
            pathCellIndexes.Add(new Vector3(isoX, isoY, 0));
        }

        for (int y = width - 2; y >= bottom - 5; y--)
        {
            float isoX = (right - y) * 1f / 2f;
            float isoY = (right + y) * 0.5f / 2f;
            pathCellIndexes.Add(new Vector3(isoX, isoY, 0));
        }

        for (int y = bottom - 5; y >= 1; y--)
        {
            float isoX = (right + 5 - y) * 1f / 2f;
            float isoY = (right + 5 + y) * 0.5f / 2f;
            pathCellIndexes.Add(new Vector3(isoX, isoY, 0));
        }

        for (int x = right; x >= left; x--)
        {
            float isoX = (x - bottom) * 1f / 2f;
            float isoY = (x + bottom) * 0.5f / 2f;
            pathCellIndexes.Add(new Vector3(isoX, isoY, 0));
        }

        for (int x = left; x >= 1; x--)
        {
            float isoX = (x - bottom) * 1f / 2f;
            float isoY = (x + bottom) * 0.5f / 2f;
            pathCellIndexes.Add(new Vector3(isoX, isoY, 0));
        }

        for (int y = bottom + 1; y <= top - 1; y++)
        { 
            float isoX = (left - y) * 1f / 2f;
            float isoY = (left + y) * 0.5f / 2f;
            pathCellIndexes.Add(new Vector3(isoX, isoY, 0));
        }

        for (int x = right + 5; x >= right; x--)
        {
            float isoX = (x - (bottom - 5)) * 1f / 2f;
            float isoY = (x + bottom - 5) * 0.5f / 2f;
            pathCellIndexes.Add(new Vector3(isoX, isoY, 0));
        }

        return pathCellIndexes;
    }

    public List<Vector3> GeneratorRockWall()
    {
        rockWallCellIndexes = new List<Vector3>();


        return rockWallCellIndexes;
    }

}
