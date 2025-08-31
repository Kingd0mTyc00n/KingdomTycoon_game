using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HunterInventory : MonoBehaviour
{
    public InventoryData inventoryData;

    [SerializeField] private UIInventoryPage inventoryUI;

    public List<InventoryItem> items = new List<InventoryItem>();

    public event Action<Dictionary<int, InventoryItem>> OnInventoryChanged;


    private IEnumerator Start()
    {
        yield return new WaitUntil(() => PlayerPrefs.HasKey(GameData.PP_USER_DATA));
        InitialzeInventory();
        SetInventoryData(UserData.UserDeepData.HuntersData[0].InventoryData);
        SetupInventory();
        PrepareInventoryData();

        items = inventoryData.InventoryItems;
    }

    public void SetInventoryData(InventoryData inventoryData)
    {
        this.inventoryData = inventoryData;
        inventoryUI.InitInventoryUI(UserData.UserDeepData.InventorySize);
        //inventoryUI.OnDescriptionRequested += HandleDescriptionRequest;
        inventoryUI.OnItemActionRequested += HandleItemActionRequest;
    }


    private void SetupInventory()
    {
        
    }


    public void InitialzeInventory()
    {
        inventoryData.InventoryItems = new List<InventoryItem>();

        for (int i = 0; i < UserData.UserDeepData.InventorySize; i++)
        {
            inventoryData.InventoryItems.Add(InventoryItem.GetEmptyItem());
        }
    }


    private void HandleKey(int number)
    {

    }


    public void PrepareInventoryData()
    {
        OnInventoryChanged += ChangeInventoryUI;
        foreach (InventoryItem item in items)
        {
            if (item.IsEmpty)
                continue;
            AddInventoryItem(item);
        }
    }

    private void ChangeInventoryUI(Dictionary<int, InventoryItem> inventoryState)
    {
        foreach (var item in inventoryState)
        {
            inventoryUI.UpdateData(item.Key, item.Value.Item, item.Value.Quantity);
        }
    }


    #region EVENT METHODS

    public void ItemAction(int itemIndex)
    {
        InventoryItem inventoryItem = GetItemAt(itemIndex);
        IItemAction itemAction = inventoryItem.Item as IItemAction;
        if (itemAction != null)
            itemAction.PerformAction(gameObject);
    }

    public void RemoveAffectCharacterEvent(int index)
    {
        InventoryItem inventoryItem = GetItemAt(index);
        IItemAction itemAction = inventoryItem.Item as IItemAction;
        if (itemAction != null)
            itemAction.RemoveAction(gameObject);
    }

    private void HandleItemActionRequest(int itemIndex)
    {
        InventoryItem inventoryItem = GetItemAt(itemIndex);
        if (inventoryItem.IsEmpty)
            return;

        if (IsEquipmentSlot(itemIndex))
            return;

        if (inventoryItem.Item.ItemType == ItemType.Edible)
        {
            IItemAction itemAction = inventoryItem.Item as IItemAction;
            if (itemAction != null)
                itemAction.PerformAction(gameObject);

            IDestroyableItem destroyableItem = inventoryItem.Item as IDestroyableItem;
            if (destroyableItem != null)
                RemoveItem(itemIndex, 1);
        }

        if (inventoryItem.Item.ItemType == ItemType.Equippable)
        {
            var equipment = GameController.instance.items.GetEquipmentById(inventoryItem.Item.Id);
            EquipCharacterItem(equipment, itemIndex);

            IItemAction itemAction = inventoryItem.Item as IItemAction;
            if (itemAction != null)
                itemAction.PerformAction(gameObject);

            IDestroyableItem destroyableItem = inventoryItem.Item as IDestroyableItem;
            if (destroyableItem != null)
                RemoveItem(itemIndex, 1);


            //inventoryUI.UpdateStatus();
        }
    }



    private void HandleSwapItems(int itemIndex_1, int itemIndex_2)
    {
        InventoryItem temp = inventoryData.InventoryItems[itemIndex_1];
        inventoryData.InventoryItems[itemIndex_1] = inventoryData.InventoryItems[itemIndex_2];
        inventoryData.InventoryItems[itemIndex_2] = temp;
        InfoAboutChange();
    }


    /*private void HandleDescriptionRequest(int itemIndex)
    {
        InventoryItem inventoryItem = GetItemAt(itemIndex);
        if (inventoryItem.IsEmpty)
        {
            inventoryUI.ResetSelection();
            return;
        }

        ItemData item = inventoryItem.ItemStatus;
        inventoryUI.UpdateDescription(itemIndex, item);

    }*/


    public bool IsEquipmentSlot(int index)
    {
        return (index >= 0 && index <= 5);
    }

    private void UpdateInventorystate()
    {
        foreach (var item in GetCurrentInventoryState())
        {
            inventoryUI.UpdateData(item.Key, item.Value.Item, item.Value.Quantity);
        }
    }

    private void InfoAboutChange()
    {
        OnInventoryChanged?.Invoke(GetCurrentInventoryState());
    }

    public bool IsInventoryFull() => inventoryData.InventoryItems.Where(item => item.IsEmpty).Any() == false;


    public void EquipCharacterItem(Equipment item, int itemIndex)
    {
        EquipItem(item, itemIndex);
    }

    private void EquipItem(Equipment itemStatus, int itemIndex)
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
                    Item = itemStatus,
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
        AddItem(item.Item, item.Quantity);
    }

    public int AddItem(ItemData newItem, int quantity)
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

    private int AddItemToFirstFreeSlot(ItemData newItem, int quantity)
    {
        InventoryItem item = new InventoryItem
        {
            Item = newItem,
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

    private int AddStackableItem(ItemData newItem, int quantity)
    {
        for (int i = 6; i < inventoryData.InventoryItems.Count; ++i)
        {
            if (inventoryData.InventoryItems[i].IsEmpty)
                continue;
            if (inventoryData.InventoryItems[i].Item.Id == newItem.Id)
            {
                int amountPossibleToTake =
                        inventoryData.InventoryItems[i].Item.MaxStackSize - inventoryData.InventoryItems[i].Quantity;

                if (quantity > amountPossibleToTake)
                {
                    inventoryData.InventoryItems[i] = inventoryData.InventoryItems[i]
                        .ChangeQuantity(inventoryData.InventoryItems[i].Item.MaxStackSize);
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
