using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIInventoryPage : MonoBehaviour
{
    public static UIInventoryPage Instance;

    public HunterInventory hunterInventory;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    [Header("UI Inventory Page")]
    [SerializeField] private UIInventoryItem itemPrefab;
    [SerializeField] private RectTransform content;

    public List<UIInventoryItem> uiItems = new List<UIInventoryItem>();

    public event Action<int> OnDescriptionRequested, OnItemActionRequested;


    private IEnumerator Start()
    {
        yield return new WaitUntil(() => PlayerPrefs.HasKey(GameData.PP_USER_DATA));
        InitInventoryUI(UserData.UserDeepData.InventorySize);
        gameObject.SetActive(false);
    }

    public void OpenSelectedHunterInventory()
    {
        if (HunterInventory.CurrentSelectedHunter == null)
        {
            Debug.LogWarning("No hunter selected!");
            return;
        }

        ShowInventory(HunterInventory.CurrentSelectedHunter);
    }

    public void ShowInventory(HunterInventory hunter)
    {
        gameObject.SetActive(true);

        uiItems.ForEach(item => item.ResetData()); 

        hunterInventory = hunter;

        foreach (var item in hunter.GetCurrentInventoryState())
        {
            UpdateData(item.Key, item.Value.Item, item.Value.Quantity);
        }
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

    public void UpdateData(int itemIndex, ItemData itemInventoryData, int quantity)
    {
        if (uiItems.Count > itemIndex)
        {
            uiItems[itemIndex].SetData(itemInventoryData, quantity);
        }
    }
}
