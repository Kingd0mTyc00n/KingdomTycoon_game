using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField]
    public ItemStatus item;

    [SerializeField]
    public int Quantity;


    [SerializeField]
    private float duration = 0.7f;

    private void Start()
    {
        Sprite icon = GameController.instance.items.items[this.item.Id].icon;
        GetComponent<SpriteRenderer>().sprite = icon;
    }

    public void SetData(ItemStatus itemData)
    {
        item = itemData;
    }

    public void DestroyItem()
    {
        GetComponent<Collider2D>().enabled = false;
        StartCoroutine(AnimateItemPickup());
    }

    private IEnumerator AnimateItemPickup()
    {
        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.zero;
        float currentTime = 0;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            transform.localScale =
                Vector3.Lerp(startScale, endScale, currentTime / duration);
            yield return null;
        }
        Destroy(gameObject);
    }
}