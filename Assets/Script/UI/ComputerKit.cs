using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Kit
{
    public string kitName;
    public string kitDescription;
    public string[] needItemName;
    public int[] needItemNumber;

    public GameObject go_Kit_Prefab;
}

public class ComputerKit : MonoBehaviour
{
    [SerializeField]
    private Kit[] kits;

    [SerializeField] private Transform tf_ItemAppear;    // ������ ������ ��ġ
    [SerializeField] private GameObject go_BaseUI;

    private bool isCraft = false;
    public bool isPowerOn = false;

    // �ʿ��� ������Ʈ
    private Inventory theInven;
    [SerializeField]
    private ComputerToolTip theToolTip;

    private AudioSource theAudio;
    [SerializeField] private AudioClip sound_ButtonClick;
    [SerializeField] private AudioClip sound_Beep;
    [SerializeField] private AudioClip sound_Activated;
    [SerializeField] private AudioClip sound_Output;

    void Start()
    {
        // ���콺 ��� �� �Ⱥ��̰��ϱ�
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        theInven = FindObjectOfType<Inventory>();
        theAudio = GetComponent<AudioSource>();
    }

    void Update()
    {
        if(isPowerOn)
        {
            if (Input.GetKeyDown(KeyCode.Escape)) PowerOff();
        }
    }

    public void PowerOn()
    {
        GameManager.isOpenCraftManual = true;

        isPowerOn = true;
        go_BaseUI.SetActive(true);
    }

    public void PowerOff()
    {
        GameManager.isOpenCraftManual = false;

        isPowerOn = false;
        theToolTip.HideToolTip();
        go_BaseUI.SetActive(false);
    }

    public void ShowToolTip(int _buttonNum)
    {
        theToolTip.ShowToolTip(kits[_buttonNum].kitName, kits[_buttonNum].kitDescription, kits[_buttonNum].needItemName, kits[_buttonNum].needItemNumber);
    }

    public void HideToolTip()
    {
        theToolTip.HideToolTip();
    }

    private void PlaySE(AudioClip _clip)
    {
        theAudio.clip = _clip;
        theAudio.Play();
    }

    public void ClickButton(int _slotNumber)
    {
        PlaySE(sound_ButtonClick);

        if(!isCraft)
        {
            if (!CheckIngredient(_slotNumber)) return;  // ��� üũ

            isCraft = true;
            UseIngredient(_slotNumber); // ��� ���

            StartCoroutine(CraftCoroutine(_slotNumber));    // Kit ����
        }
    }
    private bool CheckIngredient(int _slotNumber)
    {
        for (int i = 0; i < kits[_slotNumber].needItemName.Length; i++)
        {
            if(theInven.GetItemCount(kits[_slotNumber].needItemName[i]) < kits[_slotNumber].needItemNumber[i])
            {
                PlaySE(sound_Beep);

                return false;
            }
        }

        return true;
    }

    private void UseIngredient(int _slotNumber)
    {
        for (int i = 0; i < kits[_slotNumber].needItemName.Length; i++)
        {
            theInven.SetItemCount(kits[_slotNumber].needItemName[i], kits[_slotNumber].needItemNumber[i]);
        }
    }

    IEnumerator CraftCoroutine(int _slotNumber)
    {
        PlaySE(sound_Activated);

        yield return new WaitForSeconds(3f);

        PlaySE(sound_Output);

        Instantiate(kits[_slotNumber].go_Kit_Prefab, tf_ItemAppear.position, Quaternion.identity);
        isCraft = false;
    }
}