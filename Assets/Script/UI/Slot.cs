using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Item item;   // 획득한 아이템
    public int itemCount;   // 획득한 아이템 개수
    public Image itemImage;

    [SerializeField]
    private bool isQuickSlot;    // 퀵슬롯 여부 판단
    [SerializeField]
    private int quickSlotNumber;    // 퀵슬롯 넘버

    // 필요한 컴포넌트
    [SerializeField]
    private Text text_count;
    [SerializeField]
    private GameObject go_CountImage;

    private ItemEffectDatabase theItemEffectDatabase;
    [SerializeField]
    private RectTransform baseRect;  // 인벤토리 영역
    [SerializeField]
    private RectTransform quickSlotBaseRect;    // 퀵슬롯의 영역
    private InputNumber theInputNumber;

    private void Start()
    {
        theItemEffectDatabase = FindObjectOfType<ItemEffectDatabase>();
        theInputNumber = FindObjectOfType<InputNumber>();
    }

    // 이미지 투명도 조절
    private void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
    }

    // 아이템 획득
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

    // 아이템 개수 조정
    public void SetSlotCount(int _count)
    {
        itemCount += _count;
        text_count.text = itemCount.ToString();

        if (itemCount <= 0) ClearSlot();
    }
    
    // 슬롯 초기화
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

    // 드래그가 끝나면 호출
    public void OnEndDrag(PointerEventData eventData)
    {
        // x y Min 최소 x y Max 최대
        // baseRect 인벤토리 영역 | xMin < 내부 < xMax | yMin < 내부 < yMax
        // quickSlotRect 퀵슬롯 영역 | xMin < 내부 < xMax | localY - yMax < 내부 < localY - yMin
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
    // 슬롯이 바뀔때
    public void OnDrop(PointerEventData eventData)
    {
        if (DragSlot.instance.dragSlot != null)
        {
            ChangeSlot();

            // 인벤토리 -> 퀵슬롯 or 퀵슬롯 -> 퀵슬롯)
            if (isQuickSlot)
            {
                theItemEffectDatabase.IsAcitvatedQuickSlot(quickSlotNumber);   // 활성화된 퀵슬롯? 교체 작업
            }
            else // 인벤토리 -> 인벤토리 or 퀵슬롯 -> 인벤토리
            {
                if (DragSlot.instance.dragSlot.isQuickSlot)  // 퀵슬롯 -> 인벤토리
                {
                    theItemEffectDatabase.IsAcitvatedQuickSlot(DragSlot.instance.dragSlot.quickSlotNumber);   // 활성화된 퀵슬롯? 교체작업
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
    // 마우스가 슬롯에 들어갈 때 발동
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(item != null) theItemEffectDatabase.ShowToolTip(item, transform.position);
    }
    // 마우스가 슬롯에서 빠져 나갈 때 발동
    public void OnPointerExit(PointerEventData eventData)
    {
        theItemEffectDatabase.HideToopTip();
    }
}