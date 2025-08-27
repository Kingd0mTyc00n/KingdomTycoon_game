using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorIsoMap : MonoBehaviour
{
    public GameObject tilePrefab;
    public int width = 10;
    public int height = 10;
    public float tileWidth = 1f;
    public float tileHeight = 0.5f;

    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float isoX = (x - y) * tileWidth / 2f;
                float isoY = (x + y) * tileHeight / 2f;

                Vector3 pos = new Vector3(isoX, isoY, 0);
                Instantiate(tilePrefab, pos, Quaternion.identity, transform);
            }
        }
    }
}
