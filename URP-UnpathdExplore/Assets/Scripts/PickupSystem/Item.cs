using Inventory.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [field: SerializeField]
    public ItemSO InventoryItem { get; private set; }

    //Quantity of item that can be picked up
    [field: SerializeField]
    public int Quantity { get; set; } = 1;

    [SerializeField]
    private AudioSource audioSource;

     //animation duration
    [SerializeField]
    private float duration = 0.3f;

    private void Start()
    {
        //get sprite of the inventory item assigned in the inspector
        GetComponent<SpriteRenderer>().sprite = InventoryItem.ItemImage;
    }

    // function called when item is picked up, collider is disable and animation is set to play
    public void DestroyItem()
    {
        GetComponent<Collider2D>().enabled = false;
        StartCoroutine(AnimateItemPickup());
    }


    private IEnumerator AnimateItemPickup()
    {
        audioSource.Play();
        //original scale
        Vector3 startScale = transform.localScale;
        // scale item down to zero before destroying
        Vector3 endScale = Vector3.zero;
        float currentTime = 0;

        while(currentTime < duration)
        {
            currentTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, endScale, currentTime / duration);
            yield return null;
        }
        Destroy(gameObject);
    }
}
