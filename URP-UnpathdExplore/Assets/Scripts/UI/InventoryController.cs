using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Inventory.UI;


public class InventoryController : MonoBehaviour
{
    [SerializeField]
    private UIInventoryPage inventoryUI;
    
    public int inventorySize = 10;
    private void Start() 
    {
        inventoryUI.InitializeInventoryUI(inventorySize);
    }


    // toggles inventory menu on and off
    public void Update() 
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            if(inventoryUI.isActiveAndEnabled == false)
            {
                inventoryUI.Show();
            }

            else
            {
                inventoryUI.Hide();
            }
        }   
    }
}
