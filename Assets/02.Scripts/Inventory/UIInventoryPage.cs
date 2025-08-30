using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIInventoryPage : MonoBehaviour
{
    public static UIInventoryPage Instance;

    public CharacterController hunterController;

    private void Awake()
    {
        Instance = this;
    }

    [Header("UI Inventory Page")]
    [SerializeField] private UIInventoryItem itemPrefab;
    [SerializeField] private RectTransform content;

    public List<UIInventoryItem> uiItems = new List<UIInventoryItem>();

    public event Action<int> OnDescriptionRequested, OnItemActionRequested;


    private void Start()
    {
        gameObject.SetActive(false);
    }


    public void InitInventoryUI(int inventorySize)
    {
        for (int i = 0; i < inventorySize; i++)
        {
            UIInventoryItem uiItem = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
            uiItem.transform.SetParent(content);
            uiItem.transform.localScale = Vector3.one;  
            uiItem.name = "UI Item " + i.ToString();
            uiItems.Add(uiItem);
            uiItem.OnItemClicked += HandleItemClick;
        }
    }

    private void HandleItemClick(UIInventoryItem item)
    {
        
    }

    public void UpdateData(int itemIndex, ItemStatus itemInventoryData, int quantity)
    {
        if (uiItems.Count > itemIndex)
        {
            uiItems[itemIndex].SetData(itemInventoryData, quantity);
        }
    }
}
