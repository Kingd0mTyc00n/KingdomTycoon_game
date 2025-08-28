using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInventoryDescription : MonoBehaviour
{
    [SerializeField]
    private Image item;
    [SerializeField]
    private TextMeshProUGUI detailText;
    [SerializeField]
    private TextMeshProUGUI description;
    [SerializeField] public Button removeBtn;
    int currentSlotIndex;

    public void ResetDescription()
    {
        gameObject.SetActive(false);
        item.gameObject.SetActive(false);
        detailText.text = "";
        description.text = "";
    }

    public void SetDescriptionToItem(ItemData itemData, int index)
    {
        gameObject.SetActive(true);
        item.gameObject.SetActive(true);
        item.sprite = GameResources.instance.iconItems[itemData.Id];
        detailText.text = itemData.Name + "\n \n" + itemData.Detail;
        description.text = itemData.Description;
        currentSlotIndex = index;

        if (InventoryManager.Instance.IsCharacterSlot(index))
        {
            removeBtn.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "REMOVE";
            removeBtn.gameObject.SetActive(true);
        }
        else
        {
            if (itemData.IsEquippable)
            {
                removeBtn.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "EQUIP";
                removeBtn.gameObject.SetActive(true);
            }
        }
        if (itemData.IsEdible)
        {
            removeBtn.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "EAT";
            removeBtn.gameObject.SetActive(true);
        }                                   
        
        if (!itemData.IsEdible && !itemData.IsEquippable)
        {
            removeBtn.gameObject.SetActive(false);
        }
    }

    public void EquipItem()
    {
        InventoryManager.Instance.ItemAction(currentSlotIndex);

        InventoryItem inventoryItem = InventoryManager.Instance.GetItemAt(currentSlotIndex);
    }

    public void RemoveEquipment()
    {
        InventoryManager.Instance.RemoveAffectCharacterEvent(currentSlotIndex);

        InventoryItem inventoryItem = InventoryManager.Instance.GetItemAt(currentSlotIndex);
        IDestroyableItem destroyableItem = inventoryItem.ItemStatus as IDestroyableItem;
        if (destroyableItem != null)
            InventoryManager.Instance.RemoveItem(currentSlotIndex, 1);
        InventoryManager.Instance.AddItem(UIInventoryPage.Instance.uiItems[currentSlotIndex].ItemSlot.EdibleItem, 1);

        UIInventoryPage.Instance.UpdateStatus();
    }
}
