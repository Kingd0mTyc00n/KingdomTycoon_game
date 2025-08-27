using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public InventoryData inventoryData;

    [Header("Page")]
    [SerializeField] private UIInventoryPage inventoryUI;

    public List<InventoryItem> initialItems = new List<InventoryItem>();

    public event Action<Dictionary<int, InventoryItem>> OnInventoryChanged;
    private Dictionary<KeyCode, Action> keyActions;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        
        PrepareUI();
        InitialzeInventory();
        PrepareInventoryData();

        initialItems = inventoryData.InventoryItems;

        keyActions = new Dictionary<KeyCode, Action>
        {
            { KeyCode.Alpha0, () => HandleKey(0) },
            { KeyCode.Alpha1, () => HandleKey(1) },
            { KeyCode.Alpha2, () => HandleKey(2) },
            { KeyCode.Alpha3, () => HandleKey(3) },
            { KeyCode.Alpha4, () => HandleKey(4) },
            { KeyCode.Alpha5, () => HandleKey(5) },
            { KeyCode.Alpha6, () => HandleKey(6) },
            { KeyCode.Alpha7, () => HandleKey(7) },
            { KeyCode.Alpha8, () => HandleKey(8) },
            { KeyCode.Alpha9, () => HandleKey(9) },
        };
    }
    private void Update()
    {

    }


    private void HandleKey(int number)
    {

    }


    public void PrepareInventoryData()
    {
        OnInventoryChanged += ChangeInventoryUI;
        foreach (InventoryItem item in initialItems)
        {
            if (item.IsEmpty)
                continue;
            AddInventoryItem(item);
        }
    }

    private void ChangeInventoryUI(Dictionary<int, InventoryItem> inventoryState)
    {
        inventoryUI.ResetAllItems();

        foreach (var item in inventoryState)
        {
            inventoryUI.UpdateData(item.Key, item.Value.ItemStatus, item.Value.Quantity);
        }
    }

    private void PrepareUI()
    {
        inventoryData = UserData.UserDeepData.inventoryData;
        inventoryUI.InitInventoryUI(inventoryData.Size);
        inventoryUI.InitChestUI(36);
        inventoryUI.OnDescriptionRequested += HandleDescriptionRequest;
        inventoryUI.OnSwapItems += HandleSwapItems;
        inventoryUI.OnStartDragging += HandleDragging;
        inventoryUI.OnItemActionRequested += HandleItemActionRequest;
    }

    #region EVENT METHODS

    public void ItemAction(int itemIndex)
    {
        InventoryItem inventoryItem = GetItemAt(itemIndex);
        IItemAction itemAction = inventoryItem.ItemStatus as IItemAction;
        if (itemAction != null)
            itemAction.PerformAction(gameObject);
    }

    public void RemoveAffectCharacterEvent(int index)
    {
        InventoryItem inventoryItem = GetItemAt(index);
        IItemAction itemAction = inventoryItem.ItemStatus as IItemAction;
        if (itemAction != null)
            itemAction.RemoveAction(gameObject);
    }

    private void HandleItemActionRequest(int itemIndex)
    {
        InventoryItem inventoryItem = GetItemAt(itemIndex);
        if (inventoryItem.IsEmpty)
            return;

        if (IsCharacterSlot(itemIndex))
            return;

        if (inventoryItem.ItemStatus.IsEdible)
        {
            IItemAction itemAction = inventoryItem.ItemStatus as IItemAction;
            if (itemAction != null)
                itemAction.PerformAction(gameObject);

            IDestroyableItem destroyableItem = inventoryItem.ItemStatus as IDestroyableItem;
            if (destroyableItem != null)
                RemoveItem(itemIndex, 1);
        }

        if (inventoryItem.ItemStatus.IsEquippable)
        {
            EquipCharacterItem(inventoryItem, itemIndex);

            IItemAction itemAction = inventoryItem.ItemStatus as IItemAction;
            if (itemAction != null)
                itemAction.PerformAction(gameObject);

            IDestroyableItem destroyableItem = inventoryItem.ItemStatus as IDestroyableItem;
            if (destroyableItem != null)
                RemoveItem(itemIndex, 1);


            //inventoryUI.UpdateStatus();
        }
    }


    private void HandleDragging(int itemIndex)
    {
        InventoryItem inventoryItem = GetItemAt(itemIndex);
        if (inventoryItem.IsEmpty)
        {
            return;
        }
        inventoryUI.CreateDraggedItem(inventoryItem.ItemStatus);
    }

    private void HandleSwapItems(int itemIndex_1, int itemIndex_2)
    {
        InventoryItem temp = inventoryData.InventoryItems[itemIndex_1];
        inventoryData.InventoryItems[itemIndex_1] = inventoryData.InventoryItems[itemIndex_2];
        inventoryData.InventoryItems[itemIndex_2] = temp;
        InfoAboutChange();
    }


    private void HandleDescriptionRequest(int itemIndex)
    {
        InventoryItem inventoryItem = GetItemAt(itemIndex);
        if (inventoryItem.IsEmpty)
        {
            inventoryUI.ResetSelection();
            return;
        }

        ItemData item = inventoryItem.ItemStatus;
        inventoryUI.UpdateDescription(itemIndex, item);

    }


    public bool IsCharacterSlot(int index)
    {
        return (index >= 0 && index <= 5);
    }

    private void UpdateInventorystate()
    {
        foreach (var item in GetCurrentInventoryState())
        {
            inventoryUI.UpdateData(item.Key, item.Value.ItemStatus, item.Value.Quantity);
        }
    }

    private void InfoAboutChange()
    {
        OnInventoryChanged?.Invoke(GetCurrentInventoryState());
    }

    public bool IsInventoryFull() => inventoryData.InventoryItems.Where(item => item.IsEmpty).Any() == false;

    public void InitialzeInventory()
    {
        inventoryData.InventoryItems = new List<InventoryItem>();

        for (int i = 0; i < inventoryData.Size; i++)
        {
            inventoryData.InventoryItems.Add(InventoryItem.GetEmptyItem());
        }
    }

    public void EquipCharacterItem(InventoryItem item, int itemIndex)
    {
        EquipItem(item.ItemStatus, itemIndex);
    }

    private void EquipItem(ItemStatus itemStatus, int itemIndex)
    {
        int targetIndex = -1;

        switch (itemStatus.EquipmentType)
        {
            case EquipmentType.Helmet: targetIndex = 0; break;
            case EquipmentType.Weapon: targetIndex = 1; break;
            case EquipmentType.Armor: targetIndex = 2; break;
            case EquipmentType.Necklace: targetIndex = 3; break;
            case EquipmentType.Boots: targetIndex = 4; break;
            case EquipmentType.Ring: targetIndex = 5; break;
        }

        if (targetIndex != -1)
        {
            if (IsSlotEmpty(targetIndex))
            {
                InventoryItem item = new InventoryItem()
                {
                    ItemStatus = itemStatus,
                    Quantity = 1
                };
                inventoryData.InventoryItems[targetIndex] = item;
            }
            else
            {
                HandleSwapItems(targetIndex, itemIndex);
            }
        }

        InfoAboutChange();
    }

    private bool IsSlotEmpty(int index)
    {
        return inventoryData.InventoryItems[index].IsEmpty;
    }

    private void AddInventoryItem(InventoryItem item)
    {
        AddItem(item.ItemStatus, item.Quantity);
    }

    public int AddItem(ItemStatus newItem, int quantity)
    {
        if (!newItem.IsStackable)
        {
            for (int i = 6; i < inventoryData.InventoryItems.Count; i++)
            {
                if (IsInventoryFull())
                    return quantity;
                while (quantity > 0 && !IsInventoryFull())
                {
                    quantity -= AddItemToFirstFreeSlot(newItem, 1);
                }
                InfoAboutChange();
                return quantity;
            }
        }
        quantity = AddStackableItem(newItem, quantity);
        InfoAboutChange();
        return quantity;
    }

    private int AddItemToFirstFreeSlot(ItemStatus newItem, int quantity)
    {
        InventoryItem item = new InventoryItem
        {
            ItemStatus = newItem,
            Quantity = quantity
        };

        for (int i = 6; i < inventoryData.InventoryItems.Count; i++)
        {
            if (inventoryData.InventoryItems[i].IsEmpty)
            {
                inventoryData.InventoryItems[i] = item;
                return quantity;
            }
        }
        return 0;
    }

    private int AddStackableItem(ItemStatus newItem, int quantity)
    {
        for (int i = 6; i < inventoryData.InventoryItems.Count; ++i)
        {
            if (inventoryData.InventoryItems[i].IsEmpty)
                continue;
            if (inventoryData.InventoryItems[i].ItemStatus.Id == newItem.Id)
            {
                int amountPossibleToTake =
                        inventoryData.InventoryItems[i].ItemStatus.MaxStackSize - inventoryData.InventoryItems[i].Quantity;

                if (quantity > amountPossibleToTake)
                {
                    inventoryData.InventoryItems[i] = inventoryData.InventoryItems[i]
                        .ChangeQuantity(inventoryData.InventoryItems[i].ItemStatus.MaxStackSize);
                    quantity -= amountPossibleToTake;
                }
                else
                {
                    inventoryData.InventoryItems[i] = inventoryData.InventoryItems[i]
                        .ChangeQuantity(inventoryData.InventoryItems[i].Quantity + quantity);

                    InfoAboutChange();
                    return 0;
                }
            }
        }
        while (quantity > 0 && IsInventoryFull() == false)
        {
            int newQuantity = Mathf.Clamp(quantity, 0, newItem.MaxStackSize);
            quantity -= newQuantity;
            AddItemToFirstFreeSlot(newItem, newQuantity);
        }
        return quantity;
    }

    public void RemoveItem(int itemIndex, int amount)
    {
        if (inventoryData.InventoryItems.Count > itemIndex)
        {
            if (inventoryData.InventoryItems[itemIndex].IsEmpty)
                return;
            int reminder = inventoryData.InventoryItems[itemIndex].Quantity - amount;
            if (reminder <= 0)
                inventoryData.InventoryItems[itemIndex] = InventoryItem.GetEmptyItem();
            else
                inventoryData.InventoryItems[itemIndex] = inventoryData.InventoryItems[itemIndex]
                    .ChangeQuantity(reminder);

            InfoAboutChange();
            inventoryUI.ResetSelection();
        }
    }

    public Dictionary<int, InventoryItem> GetCurrentInventoryState()
    {
        Dictionary<int, InventoryItem> returnValue = new Dictionary<int, InventoryItem>();

        for (int i = 0; i < inventoryData.InventoryItems.Count; i++)
        {
            if (inventoryData.InventoryItems[i].IsEmpty)
                continue;
            returnValue[i] = inventoryData.InventoryItems[i];
        }
        return returnValue;
    }

    public UIInventoryItem GetCurrentItemSelected()
    {
        foreach (UIInventoryItem item in inventoryUI.uiItems)
        {
            if (item.isSelected)
                return item;
        }
        return null;
    }

    public InventoryItem GetItemAt(int itemIndex)
    {
        return inventoryData.InventoryItems[itemIndex];
    }
    #endregion
}
