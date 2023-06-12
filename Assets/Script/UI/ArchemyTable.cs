using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ArchemyItem
{
    public string itemName;
    public string itemDesc;
    public Sprite itemImage;

    public string[] needItemName;
    public int[] needItemNumber;

    public float itemCraftingTime;  // ���� ������ �ɸ��� �ð� (5��, 10��, 100��)

    public GameObject go_ItemPrefab;
}

public class ArchemyTable : MonoBehaviour
{
    public bool GetIsOpen()
    {
        return isOpen;
    }

    private bool isOpen = false;
    private bool isCrafting = false;    // �������� ���� ���� ����

    [SerializeField] private ArchemyItem[] archemyItems; // ������ �� �ִ� ���� ������ ����Ʈ
    private Queue<ArchemyItem> archemyItemQueue = new Queue<ArchemyItem>();
    private ArchemyItem currentCraftingItem;    // ���� �������� ���� ������

    private float craftingTime; // ���� ���� �ð�
    private float currentCraftingTime;  // ���� �ð�
    private int page = 1;   // ���� ���� ���̺��� ������
    [SerializeField] private int theNumberOfSlot;    // �� �������� ������ �ִ� ���� (4��)

    [SerializeField] private Image[] image_ArchemyItems;    // �������� ���� ���� �̹�����
    [SerializeField] private Text[] text_ArchemyItems;      // �������� ���� ���� �ؽ�Ʈ��
    [SerializeField] private Button[] btn_ArchemyItems;     // ��ư
    [SerializeField] private Slider slider_gauge;   // �����̴� ������
    [SerializeField] private Transform tf_BaseUI;   // ���̽� UI
    [SerializeField] private Transform tf_PotionAppearPos;  // ���� ���� ��ġ

    [SerializeField] private GameObject go_Liquid;   // ���� ��Ű�� ��ü ����
    [SerializeField] private Image[] image_CraftingItems;

    // �ʿ��� ������Ʈ
    [SerializeField] private ArchemyToolTip theToolTip;
    private AudioSource theAudio;
    private Inventory theInven;
    [SerializeField] private AudioClip sound_ButtonClick;
    [SerializeField] private AudioClip sound_Beep;
    [SerializeField] private AudioClip sound_Activate;
    [SerializeField] private AudioClip sound_ExitItem;

    private void PlaySE(AudioClip _clip)
    {
        theAudio.clip = _clip;
        theAudio.Play();
    }

    private void Start()
    {
        theInven = FindObjectOfType<Inventory>();
        theAudio = GetComponent<AudioSource>();
        ClearSlot();
        PageSetting();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsFinish())
        {
            Crafting();
        }
        if (isOpen)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseWindow();
            }
        }
    }

    private bool IsFinish()
    {
        if (archemyItemQueue.Count == 0 && !isCrafting)
        {
            go_Liquid.SetActive(false);
            slider_gauge.gameObject.SetActive(false);
            return true;
        }
        else
        {
            go_Liquid.SetActive(true);
            slider_gauge.gameObject.SetActive(true);
            return false;
        }
    }

    private void Crafting()
    {
        if (!isCrafting && archemyItemQueue.Count != 0) DequeueItem();

        if(isCrafting)
        {
            currentCraftingTime += Time.deltaTime;
            slider_gauge.value = currentCraftingTime;

            if (currentCraftingTime >= craftingTime) ProductionComplete();
        }
    }

    private void DequeueItem()
    {
        PlaySE(sound_Activate);
        isCrafting = true;
        currentCraftingItem = archemyItemQueue.Dequeue();

        craftingTime = currentCraftingItem.itemCraftingTime;
        currentCraftingTime = 0;

        slider_gauge.maxValue = craftingTime;

        CraftingImageChange();
    }

    private void CraftingImageChange()
    {
        image_CraftingItems[0].gameObject.SetActive(true);

        // ������ Dequeue �� �����Ƿ� Count�� 1�� ����
        for (int i = 0; i < archemyItemQueue.Count + 1; i++)
        {
            image_CraftingItems[i].sprite = image_CraftingItems[i + 1].sprite;
            if (i + 1 == archemyItemQueue.Count + 1) image_CraftingItems[i + 1].gameObject.SetActive(false);
        }
    }

    public void Window()
    {
        isOpen = !isOpen;
        if (isOpen) OpenWindow();
        else CloseWindow();
    }

    private void OpenWindow()
    {
        GameManager.isOpenArchemyTable = true;
        isOpen = true;
        tf_BaseUI.localScale = new Vector3(1f, 1f, 1f);
    }

    private void CloseWindow()
    {
        GameManager.isOpenArchemyTable = false;
        isOpen = false;
        tf_BaseUI.localScale = new Vector3(0f, 0f, 0f);
    }

    public void ButtonClick(int _buttonNum)
    {
        PlaySE(sound_ButtonClick);

        if(archemyItemQueue.Count < 3)
        {
            int archemyItemArrayNumber = _buttonNum + ((page - 1) * theNumberOfSlot);

            // �κ��丮���� ��� �˻�
            for (int i = 0; i < archemyItems[archemyItemArrayNumber].needItemName.Length; i++)
            {
                if(theInven.GetItemCount(archemyItems[archemyItemArrayNumber].needItemName[i]) < archemyItems[archemyItemArrayNumber].needItemNumber[i])
                {
                    PlaySE(sound_Beep);
                    return;
                }
            }

            // �κ��丮 ��� ����
            for (int i = 0; i < archemyItems[archemyItemArrayNumber].needItemName.Length; i++)
            {
                theInven.SetItemCount(archemyItems[archemyItemArrayNumber].needItemName[i], archemyItems[archemyItemArrayNumber].needItemNumber[i]);
            }

            // ���� ����
            archemyItemQueue.Enqueue(archemyItems[archemyItemArrayNumber]);

            image_CraftingItems[archemyItemQueue.Count].gameObject.SetActive(true);
            image_CraftingItems[archemyItemQueue.Count].sprite = archemyItems[archemyItemArrayNumber].itemImage;
        }
        else
        {
            PlaySE(sound_Beep);
        }
    }

    private void ProductionComplete()
    {
        isCrafting = false;
        image_CraftingItems[0].gameObject.SetActive(false);

        PlaySE(sound_ExitItem);

        Instantiate(currentCraftingItem.go_ItemPrefab, tf_PotionAppearPos.position, Quaternion.identity);
    }

    public void UpButton()
    {
        PlaySE(sound_ButtonClick);

        if (page != 1) page--;
        else page = 1 + (archemyItems.Length / theNumberOfSlot);

        ClearSlot();
        PageSetting();
    }

    public void DownButton()
    {
        PlaySE(sound_ButtonClick);

        if (page < 1 + (archemyItems.Length / theNumberOfSlot)) page++;
        else page = 1;

        ClearSlot();
        PageSetting();
    }

    private void ClearSlot()
    {
        for (int i = 0; i < theNumberOfSlot; i++)
        {
            image_ArchemyItems[i].sprite = null;
            image_ArchemyItems[i].gameObject.SetActive(false);
            btn_ArchemyItems[i].gameObject.SetActive(false);
            text_ArchemyItems[i].text = "";
        }
    }

    private void PageSetting()
    {
        int pageArrayStartNumber = (page - 1) * theNumberOfSlot; // 0, 4, 8, 12 ...

        for (int i = pageArrayStartNumber; i < archemyItems.Length; i++)
        {
            if (i == page * theNumberOfSlot) break;

            image_ArchemyItems[i - pageArrayStartNumber].sprite = archemyItems[i].itemImage;
            image_ArchemyItems[i - pageArrayStartNumber].gameObject.SetActive(true);
            btn_ArchemyItems[i - pageArrayStartNumber].gameObject.SetActive(true);
            text_ArchemyItems[i - pageArrayStartNumber].text = archemyItems[i].itemName + "\n" + archemyItems[i].itemDesc;
        }
    }

    public void ShowToolTip(int _buttonNum)
    {
        int archemyItemArrayNumber = _buttonNum + ((page - 1) * theNumberOfSlot);
        theToolTip.ShowToolTip(archemyItems[archemyItemArrayNumber].needItemName, archemyItems[archemyItemArrayNumber].needItemNumber);
    }

    public void HideToolTip()
    {
        theToolTip.HideToolTip();
    }
}