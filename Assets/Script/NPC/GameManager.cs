
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool canPlayerMove = true;    // �÷��̾��� ������ ��

    public static bool isOpenInventory = false; // �κ��丮 Ȱ��ȭ
    public static bool isOpenCraftManual = false; // ���� �޴�â Ȱ��ȭ
    public static bool isOpenArchemyTable = false;  // ���� ���̺� â Ȱ��ȭ
    public static bool isOpenComputer = false;  // ��ǻ�� â Ȱ��ȭ 

    // Update is called once per frame
    void Update()
    {
        if (isOpenInventory || isOpenCraftManual || isOpenArchemyTable || isOpenComputer)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            canPlayerMove = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            canPlayerMove = true;
        }
    }
}