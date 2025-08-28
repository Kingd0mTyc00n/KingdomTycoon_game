using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResources : MonoBehaviour
{
    public static GameResources instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [Header("Progress Bar Prefabs")]
    public List<GameObject> progressBars;

    [Header("Tree")]
    public List<Tree> listTree;
    public GameObject stump;
    public List<GameObject> treesPrefab;

    [Header("Minerals")]
    public List<GameObject> rocksPrefab;
    public List<GameObject> ironPrefab;
    public List<GameObject> goldPrefab;
    public List<GameObject> coalPrefab;
    public List<GameObject> diamondPrefab;

    [Header("Minerals Sprites")]
    public List<Sprite> mineralsSprites;

    [Header("Item On Drop")]
    public GameObject dropItem;

    [Header("Prefab")]
    public List<GameObject> listTreePrf;
    public List<Sprite> itemIcons;
    public List<Sprite> icons;

    [Header("Item Inventory Image")]
    public List<Sprite> iconItems;

    [Header("State Modifier")]
    public List<StatModifier> statModifiers;

    [Header("Crop Growth")]
    public List<Sprite> wheatGrowth;
}
