using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static bool inventoryActivated = false;

    // ÇÊ¿äÇÑ ÄÄÆ÷³ÍÆ®
    [SerializeField]
    private GameObject go_InventoryBase;
    [SerializeField]
    private GameObject go_SlotParent;
    [SerializeField]
    private GameObject go_QuickSlotParent;
    [SerializeField]
    private QuickSlotController theQuickSlot;

    // ½½·Ôµé
    private Slot[] slots;   // ÀÎº¥Åä¸® ½½·Ôµé
    private Slot[] quickSlot;   // Äü½½·Ô
    private bool isNotPut;

    private int slotNumber;

    // Start is called before the first frame update
    void Start()
    {
        slots = go_SlotParent.GetComponentsInChildren<Slot>();
        quickSlot = go_QuickSlotParent.GetComponentsInChildren<Slot>();
    }

    // Update is called once per frame
    void Update()
    {
        TryOpenInventory();
    }

    private void TryOpenInventory()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            inventoryActivated = !inventoryActivated;

            if (inventoryActivated) OpenInventory();
            else CloseInventory();
        }
    }

    private void OpenInventory()
    {
        go_InventoryBase.SetActive(true);
    }

    private void CloseInventory()
    {
        go_InventoryBase.SetActive(false);
    }

    public void AcquireItem(Item _item, int _count = 1)
    {
        PutSlot(quickSlot, _item, _count);
        if (!isNotPut)
            theQuickSlot.IsActivatedQuickSlot(slotNumber);

        if (isNotPut) PutSlot(slots, _item, _count);

        if(isNotPut)
        {
            Debug.Log("Äü½½·Ô°ú ÀÎº¥Åä¸®°¡ ²ËÃ¡½À´Ï´Ù.");
        }
    }

    private void PutSlot(Slot[] _slots, Item _item, int _count)
    {
        if (Item.ItemType.Equipment != _item.itemType)
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i].item != null)
                {
                    if (_slots[i].item.itemName == _item.itemName)
                    {
                        slotNumber = i;
                        _slots[i].SetSlotCount(_count);
                        isNotPut = false;
                        return;
                    }
                }
            }
        }

        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i].item == null)
            {
                _slots[i].Additem(_item, _count);
                isNotPut = false;
                return;
            }
        }

        isNotPut = true;
    }
}