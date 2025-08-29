using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIInventoryItem : MonoBehaviour, IPointerClickHandler
{
    public ItemSlot ItemSlot;

    [Header("UI Item")]
    [SerializeField] private Image item;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private Image outline;

    public event Action<UIInventoryItem> OnItemClicked;

    public bool isEmpty = true;
    public bool isSelected = false;


    private void Start()
    {
        ResetData();
    }
    public void ResetData()
    {
        item.gameObject.SetActive(false);
        isEmpty = true;
    }

    public void SetData(ItemStatus itemData, int quantity)
    {
        ItemSlot.EdibleItem = itemData;
        ItemSlot.QuantityOfItem = quantity;

        item.gameObject.SetActive(true);
        item.sprite = GameResources.instance.iconItems[ItemSlot.EdibleItem.Id];
        quantityText.text = ItemSlot.QuantityOfItem.ToString();
        isEmpty = false;

    }

    public void Select()
    {
        outline.enabled = true;
        isSelected = true;
    }


    public void OnPointerClick(PointerEventData pointerData)
    {
        {
            OnItemClicked?.Invoke(this);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        
    }

    
}
