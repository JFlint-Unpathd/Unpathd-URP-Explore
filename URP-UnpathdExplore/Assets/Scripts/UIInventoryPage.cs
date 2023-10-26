using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.UI
{
public class UIInventoryPage : MonoBehaviour
{
    [SerializeField]
    private UIInventoryItem itemPrefab;
    [SerializeField]
    private RectTransform contentPanel;
    [SerializeField]
    private UIInventoryDescription itemDescription;
    [SerializeField]
    private MouseFollower mouseFollower;

    List<UIInventoryItem> listOfUIItems = new List<UIInventoryItem>();

    public Sprite image;
    public int quantity;
    public string title, description;


    private void Awake()
    {
        Hide();
        mouseFollower.Toggle(false);
        itemDescription.ResetDescription();
    }

    public void InitializeInventoryUI(int inventorySize)
    {
        for (int i = 0; i < inventorySize; i++)
        {
            UIInventoryItem uiItem = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
            uiItem.transform.SetParent(contentPanel);
            listOfUIItems.Add(uiItem);

            uiItem.OnItemClicked += HandleItemSelection;
            uiItem.OnItemBeginDrag += HandleBeginDrag;
            uiItem.OnItemDroppedOn += HandleSwap;
            uiItem.OnIteEndDrag += HandleEndDrag;
            uiItem.OnRightMouseBtnClick += HandleShowItemActions;
        }
    }

        private void HandleShowItemActions(UIInventoryItem item)
        {
            
        }

        private void HandleSwap(UIInventoryItem item)
        {
            
        }

        private void HandleBeginDrag(UIInventoryItem item)
        {
            mouseFollower.Toggle(true);
            mouseFollower.SetData(image, quantity);
        }

        private void HandleEndDrag(UIInventoryItem item)
        {
            mouseFollower.Toggle(false);
        }

        private void HandleItemSelection(UIInventoryItem item)
        {
            itemDescription.SetDescription(image, title, description);
            listOfUIItems[0].Select();
        }

        public void Show()
        {
            gameObject.SetActive(true);
            itemDescription.ResetDescription();
            listOfUIItems[0].SetData(image, quantity);
        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }
}
}
