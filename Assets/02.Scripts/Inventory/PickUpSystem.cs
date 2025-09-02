using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpSystem : MonoBehaviour
{
    public static PickUpSystem instance;

    [SerializeField] private HunterInventory hunterInventory;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Item item = collision.GetComponent<Item>();
        if (item != null )
        {
            Debug.Log("Pickup item : "+ item.name);
            int reminder = hunterInventory.AddItem(item.item, item.Quantity);
            if (reminder == 0)
                item.DestroyItem();
            else
                item.Quantity = reminder;
        }
    }

    public void PickUpItem(Item item)
    {
        if (item != null)
        {
            int reminder = hunterInventory.AddItem(item.item, item.Quantity);
            if (reminder == 0)
                item.DestroyItem();
            else
                item.Quantity = reminder;
        }
    }
}
