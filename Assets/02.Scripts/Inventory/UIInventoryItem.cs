using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIInventoryItem : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDropHandler, IDragHandler
{
    public ItemSlot ItemSlot;

    [Header("UI Item")]
    [SerializeField] private Image item;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private Image outline;

    public event Action<UIInventoryItem> OnItemClicked, OnItemBeginDrag, OnItemEndDrag, OnItemDroppedOn, OnRightMouseBtnClick;

    public bool isEmpty = true;
    public bool isSelected = false;


    private void Start()
    {
        ResetData();
        Deselect();
    }
    public void ResetData()
    {
        item.gameObject.SetActive(false);
        isEmpty = true;
    }


    public void Deselect()
    {
        outline.enabled = false;
        isSelected = false;
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

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isEmpty)
            return;

        OnItemBeginDrag?.Invoke(this);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnItemEndDrag?.Invoke(this);
    }

    public void OnDrop(PointerEventData eventData)
    {
        OnItemDroppedOn?.Invoke(this);
        Debug.Log("On Drop " + eventData);
    }

    public void OnPointerClick(PointerEventData pointerData)
    {
        if (pointerData.button == PointerEventData.InputButton.Right)
        {
            OnRightMouseBtnClick?.Invoke(this);
        }
        else
        {
            OnItemClicked?.Invoke(this);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        
    }

    
}
