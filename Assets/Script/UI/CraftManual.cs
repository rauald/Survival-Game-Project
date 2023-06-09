using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Craft
{
    public string craftName;    // 이름
    public Sprite craftImage;   // 이미지
    public string craftDesc;    // 설명
    public string[] craftNeedItem;  // 필요한 아이템
    public int[] craftNeedItemCount;    // 필요한 아이템의 개수
    public GameObject go_Prefab;    // 실제 설치될 프리팹
    public GameObject go_PreviewPrefab; // 미리보기 프리팹
}

public class CraftManual : MonoBehaviour
{
    // 상태 변수
    private bool isActivated = false;
    private bool isPreviewActivated = false;

    [SerializeField]
    private GameObject go_BaseUI;    // 기본 베이스 UI

    private int tabNumber = 0;
    private int page = 1;
    private int selectedSlotNumber;
    private Craft[] craft_SelectedTab;

    [SerializeField]
    private Craft[] craft_fire; // 모닥불용 탭
    [SerializeField]
    private Craft[] craft_build;    // 건축물용 탭

    private GameObject go_Preview;  // 미리보기 프리팹을 담을 변수
    private GameObject go_Prefab;   // 실제 생성될 프리팹을 담을 변수

    [SerializeField]
    private Transform tf_Player;    // 플레이어 위치

    // Raycast 필요 변수 선언
    private RaycastHit hitInfo;
    [SerializeField]
    private LayerMask layerMask;
    [SerializeField]
    private float range;

    // 필요한 UI Slot 요소
    [SerializeField]
    private GameObject[] go_Slots;
    [SerializeField]
    private Image[] image_Slot;
    [SerializeField]
    private Text[] text_SlotName;
    [SerializeField]
    private Text[] text_SlotDesc;
    [SerializeField]
    private Text[] text_SlotNeedItem;

    // 필요한 컴포넌트
    private Inventory theInventory;

    void Start()
    {
        theInventory = FindObjectOfType<Inventory>();

        tabNumber = 0;
        page = 1;
        TabSlotSetting(craft_fire);
    }

    public void TabSetting(int _tabNumber)
    {
        tabNumber = _tabNumber;
        page = 1;

        switch (tabNumber)
        {
            case 0:
                // 불 세팅
                TabSlotSetting(craft_fire);
                break;
            case 1:
                // 건축 세팅
                TabSlotSetting(craft_build);
                break;
        }
    }

    private void ClearSlot()
    {
        for (int i = 0; i < go_Slots.Length; i++)
        {
            image_Slot[i].sprite = null;
            text_SlotDesc[i].text = "";
            text_SlotName[i].text = "";
            text_SlotNeedItem[i].text = "";
            go_Slots[i].SetActive(false);
        }
    }

    public void RightPageSetting()
    {
        if (page < (craft_SelectedTab.Length / go_Slots.Length) + 1)
        {
            page++;
        }
        else page = 1;

        TabSlotSetting(craft_SelectedTab);
    }

    public void LeftPageSetting()
    {
        if (page != 1)
        {
            page--;
        }
        else page = (craft_SelectedTab.Length / go_Slots.Length) + 1;

        TabSlotSetting(craft_SelectedTab);
    }

    private void TabSlotSetting(Craft[] _craft_tab)
    {
        ClearSlot();
        craft_SelectedTab = _craft_tab;

        int startSlotNumber = (page - 1) * go_Slots.Length;

        for (int i = startSlotNumber; i < craft_SelectedTab.Length; i++)
        {
            if (i == page * go_Slots.Length) break;

            go_Slots[i - startSlotNumber].SetActive(true);

            image_Slot[i - startSlotNumber].sprite = craft_SelectedTab[i].craftImage;
            text_SlotName[i - startSlotNumber].text = craft_SelectedTab[i].craftName;
            text_SlotDesc[i - startSlotNumber].text = craft_SelectedTab[i].craftDesc;

            for (int j = 0; j < craft_SelectedTab[i].craftNeedItem.Length; j++)
            {
                text_SlotNeedItem[i - startSlotNumber].text += craft_SelectedTab[i].craftNeedItem[j];
                text_SlotNeedItem[i - startSlotNumber].text += " x " + craft_SelectedTab[i].craftNeedItemCount[j] + "\n";
            }
        }
    }

    public void SlotClick(int _slotNumber)
    {
        selectedSlotNumber = _slotNumber + (page - 1) * go_Slots.Length;

        if (!CheckIngredient()) return;

        go_Preview = Instantiate(craft_SelectedTab[selectedSlotNumber].go_PreviewPrefab, tf_Player.position + tf_Player.forward, Quaternion.identity);
        go_Prefab = craft_SelectedTab[selectedSlotNumber].go_Prefab;
        isPreviewActivated = true;

        GameManager.isOpenCraftManual = false;

        go_BaseUI.SetActive(false);
    }

    private bool CheckIngredient()
    {
        for (int i = 0; i < craft_SelectedTab[selectedSlotNumber].craftNeedItem.Length; i++)
        {
            if(theInventory.GetItemCount(craft_SelectedTab[selectedSlotNumber].craftNeedItem[i]) < craft_SelectedTab[selectedSlotNumber].craftNeedItemCount[i])
            return false;
        }

        return true;
    }

    private void UseIngredient()
    {
        for (int i = 0; i < craft_SelectedTab[selectedSlotNumber].craftNeedItem.Length; i++)
        {
            theInventory.SetItemCount(craft_SelectedTab[selectedSlotNumber].craftNeedItem[i], craft_SelectedTab[selectedSlotNumber].craftNeedItemCount[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab) && !isPreviewActivated)
        {
            Window();
        }
        if(isPreviewActivated)
        {
            PreviewPositionUpdate();
        }
        if(Input.GetButtonDown("Fire1"))
        {
            Build();
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Cancel();
        }
    }

    private void Build()
    {
        if(isPreviewActivated && go_Preview.GetComponent<PreviewObject>().IsBuildable())
        {
            UseIngredient();
            Instantiate(go_Prefab, go_Preview.transform.position, go_Preview.transform.rotation);
            Destroy(go_Preview);
            isActivated = false;
            isPreviewActivated = false;
            go_Preview = null;
            go_Prefab = null;
        }
    }

    private void PreviewPositionUpdate()
    {
        if(Physics.Raycast(tf_Player.position, tf_Player.forward, out hitInfo, range, layerMask))
        {
            if(hitInfo.transform != null)
            {
                Vector3 _localtion = hitInfo.point;

                if(Input.GetKeyDown(KeyCode.Q))
                {
                    go_Preview.transform.Rotate(0, -90f, 0);
                }
                else if (Input.GetKeyDown(KeyCode.E))
                {
                    go_Preview.transform.Rotate(0, +90f, 0);
                }
                _localtion.Set(Mathf.Round(_localtion.x), Mathf.Round(_localtion.y / 0.1f) * 0.1f, Mathf.Round(_localtion.z));

                go_Preview.transform.position = _localtion;
            }
        }
    }

    private void Cancel()
    {
        if (isPreviewActivated) Destroy(go_Preview);

        isActivated = false;
        isPreviewActivated = false;
        go_Preview = null;
        go_Prefab = null;

        GameManager.isOpenCraftManual = false;

        go_BaseUI.SetActive(false);

    }

    private void Window()
    {
        if (!isActivated) OpenWindow();
        else CloseWindow();
    }

    private void OpenWindow()
    {
        GameManager.isOpenCraftManual = true;
        isActivated = true;
        go_BaseUI.SetActive(true);
    }

    private void CloseWindow()
    {
        GameManager.isOpenCraftManual = false;
        isActivated = false;
        go_BaseUI.SetActive(false);
    }

}
