using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIInventoryPage : MonoBehaviour
{
    public static UIInventoryPage Instance;

    HunterController hunterController;

    private void Awake()
    {
        Instance = this;
    }

    [Header("UI Inventory Page")]
    [SerializeField] private UIInventoryItem itemPrefab;
    [SerializeField] private RectTransform content;
    [SerializeField] private RectTransform chestContent;
    [SerializeField] public ItemInventoryDescription itemDescription;
    [SerializeField] private MouseFollower mouseFollower;
    [SerializeField] private TextMeshProUGUI statusText;

    public List<UIInventoryItem> uiItems = new List<UIInventoryItem>();
    public List<UIInventoryItem> chestUIItem = new List<UIInventoryItem>();

    [SerializeField] private int currentlyDraggedItemIndex = -1;

    public event Action<int> OnDescriptionRequested, OnItemActionRequested, OnStartDragging;

    public event Action<int, int> OnSwapItems;


    private void Start()
    {
        gameObject.SetActive(false);
        mouseFollower.Toggle(false);
        DeselectAllItem();

        for (int i = 0; i < 16; i++)
        {
            uiItems[i].OnItemClicked += HandleItemSelection;
            uiItems[i].OnItemBeginDrag += HandleBeginDrag;
            uiItems[i].OnItemDroppedOn += HandleSwap;
            uiItems[i].OnItemEndDrag += HandleEndDrag;
            uiItems[i].OnRightMouseBtnClick += HandleShowItemActions;
        }

    }


    private void OnEnable()
    {
        UpdateStatus(hunterController);
    }

    private void OnDisable()
    {
        DeselectAllItem();
        itemDescription.gameObject.SetActive(false);
    }

    public void InitInventoryUI(int inventorySize)
    {
        for (int i = 10; i < inventorySize; i++)
        {
            UIInventoryItem uiItem = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
            uiItem.transform.SetParent(content);
            uiItem.name = "UI Item " + i.ToString();
            uiItems.Add(uiItem);
            uiItem.OnItemClicked += HandleItemSelection;
            uiItem.OnItemBeginDrag += HandleBeginDrag;
            uiItem.OnItemDroppedOn += HandleSwap;
            uiItem.OnItemEndDrag += HandleEndDrag;
            uiItem.OnRightMouseBtnClick += HandleShowItemActions;
        }
    }

    public void InitChestUI(int size)
    {
        for (int i = 0; i < size; i++)
        {
            UIInventoryItem item = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
            item.transform.SetParent(chestContent);
            item.name = "Chest Slot " + i.ToString();
            chestUIItem.Add(item);
            item.OnItemClicked += HandleItemSelection;
            item.OnItemBeginDrag += HandleBeginDrag;
            item.OnItemDroppedOn += HandleSwap;
            item.OnItemEndDrag += HandleEndDrag;
            item.OnRightMouseBtnClick += HandleShowItemActions;
        }
    }

    public void UpdateStatus(HunterController hunter)
    {
        HunterData hunterData = hunter.GetHunterData();
        statusText.text = string.Format("Name:  {0} \nHealth: {1} \nDamage: {2} \nArmor: {3} \nSpeed: {4}"
            , hunterData.Name
            , hunterData.Health
            , hunterData.Damage
            , hunterData.Armor
            , hunterData.Speed);
    }

    public void UpdateData(int itemIndex, ItemStatus itemInventoryData, int quantity)
    {
        if (uiItems.Count > itemIndex)
        {
            uiItems[itemIndex].SetData(itemInventoryData, quantity);
        }
    }


    public void UpdateDescription(int itemIndex, ItemData itemInventoryData)
    {
        itemDescription.SetDescriptionToItem(itemInventoryData, itemIndex);
        DeselectAllItem();
        uiItems[itemIndex].Select();

        /*switch (itemInventoryData.Id)
        {
            case 1:
                PlayerController.instance.ChangeItem(ItemType.Pickaxe);
                PlayerController.instance.ChangePlayerState(State.Idle);
                break;
            case 2:
                PlayerController.instance.ChangeItem(ItemType.Axe);
                PlayerController.instance.ChangePlayerState(State.Idle);
                break;
            case 3:
                PlayerController.instance.ChangeItem(ItemType.Shovel);
                PlayerController.instance.ChangePlayerState(State.Idle);
                break;
            case 15:
                PlayerController.instance.ChangePlayerState(State.Carry);
                PlayerController.instance.ChangeCarriedCrop(CarriedCropType.Beetroot);
                break;
            case 4:
                PlayerController.instance.ChangeItem(ItemType.Bucket);
                break;
        }*/
    }

    private void HandleShowItemActions(UIInventoryItem uiInventoryItem)
    {
        int index = uiItems.IndexOf(uiInventoryItem);
        if (index == -1)
        {
            return;
        }
        OnItemActionRequested?.Invoke(index);
    }
    private void HandleSwap(UIInventoryItem uiInventoryItem)
    {
        int targetIndex = uiItems.IndexOf(uiInventoryItem);

        if (targetIndex == -1 || currentlyDraggedItemIndex == -1)
        {
            return;
        }

        if (uiInventoryItem.ItemSlot.EquipmentType != uiItems[currentlyDraggedItemIndex].ItemSlot.EdibleItem.EquipmentType && uiInventoryItem.ItemSlot.EquipmentType != EquipmentType.None)
        {
            return;
        }

        OnSwapItems?.Invoke(currentlyDraggedItemIndex, targetIndex);
        HandleItemSelection(uiInventoryItem);

        var insInventory = InventoryManager.Instance;

        if (insInventory.IsCharacterSlot(targetIndex))
        {
            insInventory.ItemAction(targetIndex);
            if (!uiItems[currentlyDraggedItemIndex].isEmpty)
                insInventory.RemoveAffectCharacterEvent(currentlyDraggedItemIndex);
        }

        if (insInventory.IsCharacterSlot(currentlyDraggedItemIndex))
            insInventory.RemoveAffectCharacterEvent(targetIndex);


        //UpdateStatus();
    }



    private void HandleBeginDrag(UIInventoryItem uiInventoryItem)
    {
        int index = uiItems.IndexOf(uiInventoryItem);

        if (index == -1)
            return;
        currentlyDraggedItemIndex = index;

        HandleItemSelection(uiInventoryItem);
        OnStartDragging?.Invoke(index);

    }

    private void HandleEndDrag(UIInventoryItem uiInventoryItem)
    {
        ResetDraggedItem();
    }


    public void ResetSelection()
    {
        itemDescription.ResetDescription();
        DeselectAllItem();
    }

    public void DeselectAllItem()
    {
        foreach (UIInventoryItem uiInventoryItem in uiItems)
        {
            uiInventoryItem.Deselect();
        }

       // PlayerController.instance.ChangeItem(ItemType.None);
    }

    //call when disable page
    private void ResetDraggedItem()
    {
        mouseFollower.Toggle(false);
        currentlyDraggedItemIndex = -1;
    }


    public void CreateDraggedItem(ItemStatus itemInventoryData)
    {
        mouseFollower.Toggle(true);
        mouseFollower.SetItemDataOnDrag(itemInventoryData);
    }

    private void HandleItemSelection(UIInventoryItem uiInventoryItem)
    {
        int index = uiItems.IndexOf(uiInventoryItem);

        if (index == -1)
            return;

        OnDescriptionRequested?.Invoke(index);
    }

    internal void ResetAllItems()
    {
        foreach (var item in uiItems)
        {
            item.ResetData();
            item.Deselect();
        }
    }
}
