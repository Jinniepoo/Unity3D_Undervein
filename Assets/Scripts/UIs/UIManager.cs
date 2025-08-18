using Diablo.InventorySystem.Items;
using Diablo.InventorySystem.UIs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public ItemDatabaseObject itemDatabase;

    public StaticInventoryUI equipmentUI;
    public DynamicInventoryUI inventoryUI;

    private PlayerInputActions inputActions;

    private void Start()
    {
        if (inventoryUI != null) { inventoryUI.gameObject.SetActive(false); }
        if (equipmentUI != null) { equipmentUI.gameObject.SetActive(false); }
    }

    private void Awake()
    {
        Instance = this;

        inputActions = new PlayerInputActions();

        inputActions.UI.ToggleInventory.performed += ctx => ToggleInventory();
        inputActions.UI.ToggleEquipment.performed += ctx => ToggleEquipment();
        inputActions.UI.CloseTabs.performed += ctx => CloseTabs();
    }
    private void OnEnable()
    {
        inputActions.UI.Enable();
    }

    private void OnDisable()
    {
        inputActions.UI.Disable();
    }

    private void ToggleInventory()
    {
        if (inventoryUI != null)
        {
            inventoryUI.gameObject.SetActive(!inventoryUI.gameObject.activeSelf);
        }
    }

    private void ToggleEquipment()
    {
        if (equipmentUI != null)
        {
            equipmentUI.gameObject.SetActive(!equipmentUI.gameObject.activeSelf);
        }
    }

    private void CloseTabs()
    {
        if (inventoryUI != null || equipmentUI != null) { 
            equipmentUI.gameObject.SetActive(false);
            inventoryUI.gameObject.SetActive(false);
        }
    }
    private void Update()
    {
       
    }
}
