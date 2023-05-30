using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemEffect
{
    public string itemName; // ������ �̸� (Ű��)
    [Tooltip("HP, SP, DP, HUNGRY, THIRSTY, SATISFY�� �����մϴ�")]
    public string[] part; // ����
    public int[] num; // ��ġ
}
public class ItemEffectDatabase : MonoBehaviour
{
    [SerializeField]
    private ItemEffect[] itemEffects;

    // �ʿ��� ������Ʈ
    [SerializeField]
    private WeaponManager theWeaponManager;
    [SerializeField]
    private StatusController thePlayerStatus;
    [SerializeField]
    private SlotToolTip theSlotToopTip;
    [SerializeField]
    private QuickSlotController theQuickSlotController;

    private const string HP = "HP", SP = "SP", DP = "DP", HUNGRY = "HUNGRY", THIRSTY = "THIRSTY", SATISFY = "SATISFY";

    // QuickSlotController ¡�˴ٸ�
    public void IsAcitvatedQuickSlot(int _num)
    {
        theQuickSlotController.IsActivatedQuickSlot(_num);
    }

    // SlotToolTip ¡�˴ٸ�
    public void ShowToolTip(Item _item, Vector3 _pos)
    {
        theSlotToopTip.ShowToolTip(_item, _pos);
    }

    // SlotToolTip ¡�˴ٸ�
    public void HideToopTip()
    {
        theSlotToopTip.HideToolTip();
    }

    public void UseItem(Item _item)
    {
        if(_item.itemType == Item.ItemType.Equipment)
        {
            StartCoroutine(theWeaponManager.ChangeWeaponCoroutine(_item.weaponType, _item.itemName));
        }
        else if (_item.itemType == Item.ItemType.Used)
        {
            for (int i = 0; i < itemEffects.Length; i++)
            {
                if(itemEffects[i].itemName == _item.itemName)
                {
                    for (int j = 0; j < itemEffects[i].part.Length; j++)
                    {
                        switch(itemEffects[i].part[j])
                        {
                            case HP:
                                thePlayerStatus.IncreaseHP(itemEffects[i].num[j]);
                                break;
                            case SP:
                                thePlayerStatus.IncreaseSP(itemEffects[i].num[j]);
                                break;
                            case DP:
                                thePlayerStatus.IncreaseDP(itemEffects[i].num[j]);
                                break;
                            case HUNGRY:
                                thePlayerStatus.IncreaseHungry(itemEffects[i].num[j]);
                                break;
                            case THIRSTY:
                                thePlayerStatus.IncreaseThirsty(itemEffects[i].num[j]);
                                break;
                            case SATISFY:
                                break;
                            default:
                                Debug.Log("������ Status ���� HP, SP, DP, HUNGRY, THIRSTY, SATISFY�� �����մϴ�");
                                break;
                        }
                        Debug.Log(_item.itemName + " �� ����߽��ϴ�.");
                    }
                    return;
                }
            }
            Debug.Log("itemEffectData�� ��ġ�ϴ� itemnAME �����ϴ�.");
        }
    }
}