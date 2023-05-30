using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Item item;   // ȹ���� ������
    public int itemCount;   // ȹ���� ������ ����
    public Image itemImage;

    [SerializeField]
    private bool isQuickSlot;    // ������ ���� �Ǵ�
    [SerializeField]
    private int quickSlotNumber;    // ������ �ѹ�

    // �ʿ��� ������Ʈ
    [SerializeField]
    private Text text_count;
    [SerializeField]
    private GameObject go_CountImage;

    private ItemEffectDatabase theItemEffectDatabase;
    [SerializeField]
    private RectTransform baseRect;  // �κ��丮 ����
    [SerializeField]
    private RectTransform quickSlotBaseRect;    // �������� ����
    private InputNumber theInputNumber;

    private void Start()
    {
        theItemEffectDatabase = FindObjectOfType<ItemEffectDatabase>();
        theInputNumber = FindObjectOfType<InputNumber>();
    }

    // �̹��� ���� ����
    private void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
    }

    // ������ ȹ��
    public void Additem(Item _item, int _count =1)
    {
        item = _item;
        itemCount = _count;
        itemImage.sprite = item.itemImage;

        if(item.itemType != Item.ItemType.Equipment)
        {
            go_CountImage.SetActive(true);
            text_count.text = itemCount.ToString();
        }
        else
        {
            text_count.text = "0";
            go_CountImage.SetActive(false);
        }

        SetColor(1);
    }

    public int GetQuickSlotNumber()
    {
        return quickSlotNumber;
    }

    // ������ ���� ����
    public void SetSlotCount(int _count)
    {
        itemCount += _count;
        text_count.text = itemCount.ToString();

        if (itemCount <= 0) ClearSlot();
    }
    
    // ���� �ʱ�ȭ
    private void ClearSlot()
    {
        item = null;
        itemCount = 0;
        itemImage.sprite = null;
        SetColor(0);

        text_count.text = "0";
        go_CountImage.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            if(item != null)
            {
                theItemEffectDatabase.UseItem(item);
                if(item.itemType == Item.ItemType.Used) SetSlotCount(-1);
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null && Inventory.inventoryActivated)
        {
            DragSlot.instance.dragSlot = this;
            DragSlot.instance.DragSetImage(itemImage);

            DragSlot.instance.transform.position = eventData.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(item != null) DragSlot.instance.transform.position = eventData.position;
    }

    // �巡�װ� ������ ȣ��
    public void OnEndDrag(PointerEventData eventData)
    {
        // x y Min �ּ� x y Max �ִ�
        // baseRect �κ��丮 ���� | xMin < ���� < xMax | yMin < ���� < yMax
        // quickSlotRect ������ ���� | xMin < ���� < xMax | localY - yMax < ���� < localY - yMin
        if (!((DragSlot.instance.transform.localPosition.x > baseRect.rect.xMin && DragSlot.instance.transform.localPosition.x < baseRect.rect.xMax
            && DragSlot.instance.transform.localPosition.y > baseRect.rect.yMin && DragSlot.instance.transform.localPosition.y < baseRect.rect.yMax)
            || 
            (DragSlot.instance.transform.localPosition.x > quickSlotBaseRect.rect.xMin && DragSlot.instance.transform.localPosition.x < quickSlotBaseRect.rect.xMax
            && DragSlot.instance.transform.localPosition.y > quickSlotBaseRect.transform.localPosition.y - quickSlotBaseRect.rect.yMax && DragSlot.instance.transform.localPosition.y < quickSlotBaseRect.transform.localPosition.y - quickSlotBaseRect.rect.yMin)))
        {
            if (DragSlot.instance.dragSlot != null)
            {
                theInputNumber.Call();
            }
        }
        else
        {
            DragSlot.instance.SetColor(0);
            DragSlot.instance.dragSlot = null;
        }
    }
    // ������ �ٲ�
    public void OnDrop(PointerEventData eventData)
    {
        if (DragSlot.instance.dragSlot != null)
        {
            ChangeSlot();

            // �κ��丮 -> ������ or ������ -> ������)
            if (isQuickSlot)
            {
                theItemEffectDatabase.IsAcitvatedQuickSlot(quickSlotNumber);   // Ȱ��ȭ�� ������? ��ü �۾�
            }
            else // �κ��丮 -> �κ��丮 or ������ -> �κ��丮
            {
                if (DragSlot.instance.dragSlot.isQuickSlot)  // ������ -> �κ��丮
                {
                    theItemEffectDatabase.IsAcitvatedQuickSlot(DragSlot.instance.dragSlot.quickSlotNumber);   // Ȱ��ȭ�� ������? ��ü�۾�
                }
            }
        }
    }

    private void ChangeSlot()
    {
        Item _tempItem = item;
        int _tempItemCount = itemCount;

        Additem(DragSlot.instance.dragSlot.item, DragSlot.instance.dragSlot.itemCount);

        if(_tempItem != null) DragSlot.instance.dragSlot.Additem(_tempItem, _tempItemCount);
        else DragSlot.instance.dragSlot.ClearSlot();
    }
    // ���콺�� ���Կ� �� �� �ߵ�
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(item != null) theItemEffectDatabase.ShowToolTip(item, transform.position);
    }
    // ���콺�� ���Կ��� ���� ���� �� �ߵ�
    public void OnPointerExit(PointerEventData eventData)
    {
        theItemEffectDatabase.HideToopTip();
    }
}