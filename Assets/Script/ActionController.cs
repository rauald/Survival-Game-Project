using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionController : MonoBehaviour
{
    [SerializeField]
    private float range;    // ���� ������ �ִ� �Ÿ�

    private bool pickupActivated = false;   // ������ ���� ������ �� true

    private bool dissolveActivated = false; // ��� ��ü ������ �� true;
    private bool isDissolving = false;      // ��� ��ü��

    private bool fireLookActivated = false; // ���� �����ؼ� �ٶ� �� �� true;

    private bool lookCompouter = false; // ��ǻ�͸� �ٶ� �� true;
    private bool lookArchemyTable = false;  // ���� ���̺��� �ٶ� �� true;

    private bool lookActivatedTrap = false; // ������ ������ �ٶ󺼽� True;

    private RaycastHit hitInfo; // �浹ü ���� ����

    // ������ ���̾�� �����ϵ��� ���̾� ����ũ�� ����
    [SerializeField]
    private LayerMask layerMask;

    // �ʿ��� ������Ʈ
    [SerializeField]
    private Text actionText;
    [SerializeField]
    private Inventory theInventory;
    [SerializeField] 
    private WeaponManager theWeaponManager;
    [SerializeField]
    private QuickSlotController theQuickSlot;
    [SerializeField] 
    private Transform tf_MeatDissolveTool;
    [SerializeField]
    private ComputerKit theComputer;


    [SerializeField]
    private string sound_meat; // �Ҹ� ���

    
    // Update is called once per frame
    void Update()
    {
        CheckAction();
        TryAction();

        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 3, Color.red);
    }

    private void TryAction()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            CheckAction();
            CanPickUp();
            CanMeat();
            CanDropFire();
            CanComputerPowerOn();
            CanArchemyTableOpen();
            CanReInstallTrap();
        }
    }

    private void CanPickUp()
    {
        if(pickupActivated)
        {
            if(hitInfo.transform != null)
            {
                theInventory.AcquireItem(hitInfo.transform.GetComponent<ItemPickUp>().item);
                Destroy(hitInfo.transform.gameObject);
                InfoDisappear();
            }
        }
    }

    private void CanComputerPowerOn()
    {
        if (lookCompouter)
        {
            if (hitInfo.transform != null)
            {
                if (!hitInfo.transform.GetComponent<ComputerKit>().isPowerOn)
                {
                    hitInfo.transform.GetComponent<ComputerKit>().PowerOn();
                    InfoDisappear();
                }
            }
        }
    }

    private void CanArchemyTableOpen()
    {
        if (lookArchemyTable)
        {
            if (hitInfo.transform != null)
            {
                hitInfo.transform.GetComponent<ArchemyTable>().Window();
                InfoDisappear();
            }
        }
    }

    private void CanReInstallTrap()
    {
        if (lookActivatedTrap)
        {
            if (hitInfo.transform != null)
            {
                hitInfo.transform.GetComponent<DeadTrap>().ReInstall();
                InfoDisappear();
            }
        }
    }

    private void CanMeat()
    {
        if(dissolveActivated)
        {
            if ((hitInfo.transform.tag == "WeakAnimal" || hitInfo.transform.tag == "StrongAnimal") && hitInfo.transform.GetComponent<Animal>().isDead && !isDissolving)
            {
                isDissolving = true;
                InfoDisappear();
                // ��� ��ü �ǽ�
                StartCoroutine(MeatCoroutine());
            }
        }
    }
    
    IEnumerator MeatCoroutine()
    {
        WeaponManager.isChangeWepon = true;
        WeaponSway.isActivated = false;
        WeaponManager.currentWeaponAnim.SetTrigger("Weapon_Out");
        PlayerController.isActivated = false;

        yield return new WaitForSeconds(0.2f);
        
        WeaponManager.currentWeapon.gameObject.SetActive(false);
        tf_MeatDissolveTool.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.2f);

        SoundManager.instance.PlaySE(sound_meat);

        yield return new WaitForSeconds(1.8f);
        Debug.Log(hitInfo.transform.GetComponent<Animal>().GetItem().itemName);

        theInventory.AcquireItem(hitInfo.transform.GetComponent<Animal>().GetItem(), hitInfo.transform.GetComponent<Animal>().itemNumber);

        WeaponManager.currentWeapon.gameObject.SetActive(true);
        tf_MeatDissolveTool.gameObject.SetActive(false);

        PlayerController.isActivated = true;
        WeaponSway.isActivated = true;
        WeaponManager.isChangeWepon = false;
        isDissolving = false;
    }

    private void CanDropFire()
    {
        if(fireLookActivated)
        {
            if(hitInfo.transform.tag == "Fire" && hitInfo.transform.GetComponent<Fire>().GetIsFire())
            {
                Slot _selectedSlot = theQuickSlot.GetSelectedSlot();
                if(_selectedSlot.item != null)
                {
                    DropAnItem(_selectedSlot);
                }
            }
        }
    }

    private void DropAnItem(Slot _selectedSlot)
    {
        switch(_selectedSlot.item.itemType)
        {
            case Item.ItemType.Used:
                if(_selectedSlot.item.itemName.Contains("���"))
                {
                    Instantiate(_selectedSlot.item.itemPrefab, hitInfo.transform.position + Vector3.up, Quaternion.identity);
                    _selectedSlot.SetSlotCount(-1);
                    theQuickSlot.DecreaseSelectedItem();
                }
                break;
            case Item.ItemType.Ingredient:
                break;
        }
    }

    private void CheckAction()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hitInfo, range, layerMask))
        {
            if (hitInfo.transform.tag == "Item")
            {
                ItemInfoAppear();
            }
            else if (hitInfo.transform.tag == "WeakAnimal" || hitInfo.transform.tag == "StrongAnimal")
            {
                MeatInfoAppear();
            }
            else if (hitInfo.transform.tag == "Fire")
            {
                FireInfoAppear();
            }
            else if (hitInfo.transform.tag == "Computer")
            {
                ComputerInfoAppear();
            }
            else if (hitInfo.transform.tag == "ArchemyTable")
            {
                ArchemyInfoAppear();
            }
            else if (hitInfo.transform.tag == "Trap")
            {
                TrapInfoAppear();
            }
            else InfoDisappear();
        }
        else InfoDisappear();
    }

    private void Reset()
    {
        pickupActivated = false;
        dissolveActivated = false;
        fireLookActivated = false;
    }

    private void ItemInfoAppear()
    {
        Reset();
        pickupActivated = true;
        actionText.gameObject.SetActive(true);
        actionText.text = hitInfo.transform.GetComponent<ItemPickUp>().item.itemName + " ȹ��" + "<color=yellow>" + "(E)" + "</color>";
    }

    private void MeatInfoAppear()
    {
        if (hitInfo.transform.GetComponent<Animal>().isDead)
        {
            Reset();
            dissolveActivated = true;
            actionText.gameObject.SetActive(true);
            actionText.text = hitInfo.transform.GetComponent<Animal>().animalName + " ��ü�ϱ�" + "<color=yellow>" + "(E)" + "</color>";
        }
    }

    private void FireInfoAppear()
    {
        Reset();
        fireLookActivated = true;

        if(hitInfo.transform.GetComponent<Fire>().GetIsFire())
        {
            actionText.gameObject.SetActive(true);
            actionText.text = "���õ� ������ �ҿ� �ֱ�" + "<color=yellow>" + "(E)" + "</color>";
        }
    }

    private void ComputerInfoAppear()
    {
        if (!hitInfo.transform.GetComponent<ComputerKit>().isPowerOn)
        {
            Reset();
            lookCompouter = true;
            actionText.gameObject.SetActive(true);
            actionText.text = "��ǻ�� ���� " + "<color=yellow>" + "(E)" + "</color>";
        }
    }

    private void ArchemyInfoAppear()
    {
        if (!hitInfo.transform.GetComponent<ArchemyTable>().GetIsOpen())
        {
            Reset();
            lookArchemyTable = true;
            actionText.gameObject.SetActive(true);
            actionText.text = "���� ���̺� ���� " + "<color=yellow>" + "(E)" + "</color>";
        }
    }

    private void TrapInfoAppear()
    {
        if (hitInfo.transform.GetComponent<DeadTrap>().GetIsActivated())
        {
            Reset();
            lookActivatedTrap = true;
            actionText.gameObject.SetActive(true);
            actionText.text = "���� �缳ġ " + "<color=yellow>" + "(E)" + "</color>";
        }
    }

    private void InfoDisappear()
    {
        pickupActivated = false;
        dissolveActivated = false;
        fireLookActivated = false;
        lookCompouter = false;
        lookArchemyTable = false;
        lookActivatedTrap = false;
        actionText.gameObject.SetActive(false);
    }
}