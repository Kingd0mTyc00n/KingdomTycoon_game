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

    [Header("Item On Drop")]
    public GameObject dropItem;

    [Header("Item Inventory Image")]
    public List<Sprite> iconItems;

    [Header("State Modifier")]
    public List<StatModifier> statModifiers;

}
