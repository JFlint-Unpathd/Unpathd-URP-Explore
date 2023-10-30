using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
//using Inventory.UI;
//using Inventory.Model;

//namespace Inventory
//{
public class InventoryController : MonoBehaviour
{
    [SerializeField]
    private UIInventoryPage inventoryUI;
    [SerializeField]
    private InventorySO inventoryData;
    
    
    private void Start()
    {
        PrepareUI();
        //inventoryData.Initialize();
    }

    private void PrepareUI()
    {
        inventoryUI.InitializeInventoryUI(inventoryData.Size);
        this.inventoryUI.OnDescriptionRequested += HandleDescriptionRequest;
        this.inventoryUI.OnSwapItems += HandleSwapItems;
        this.inventoryUI.OnStartDragging += HandleDragging;
        this.inventoryUI.OnItemActionrequested += HandleItemActionRequest;
    }


    
    private void HandleItemActionRequest(int itemIndex)
    {
        
    }

    private void HandleSwapItems(int itemIndex1, int itemIndex2)
    {
        
    }
    private void HandleDragging(int itemIndex)
    {
        
    }

    private void HandleDescriptionRequest(int itemIndex)
    {
        InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
        if(inventoryItem.IsEmpty)
        {
            inventoryUI.ResetSelection();
            return;
        }
        
        ItemSO item = inventoryItem.item;
        inventoryUI.UpdateDescription(itemIndex, item.ItemImage,
            item.name, item.Description);
    }

  
    // toggles inventory menu on and off
    public void Update() 
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            if(inventoryUI.isActiveAndEnabled == false)
            {
                inventoryUI.Show();
                foreach (var item in inventoryData.GetCurrentInventoryState())
                {
                    inventoryUI.UpdateData(item.Key, 
                    item.Value.item.ItemImage, item.Value.quantity);
                }
            }

            else
            {
                inventoryUI.Hide();
            }
        }   
    }
}
//}
