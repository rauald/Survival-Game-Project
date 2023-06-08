using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComputerToolTip : MonoBehaviour
{
    [SerializeField] private GameObject go_BaseUI;

    [SerializeField] private Text kitName;
    [SerializeField] private Text kitDes;
    [SerializeField] private Text kitneedItem;

    public void ShowToolTip(string _kitName, string _kitDes, string[] _needItem, int[] _needItemNumber)
    {
        go_BaseUI.SetActive(true);

        kitName.text = _kitName;
        kitDes.text = _kitDes;

        for (int i = 0; i < _needItem.Length; i++)
        {
            kitneedItem.text += _needItem[i];
            kitneedItem.text += " x " + _needItemNumber[i].ToString() + "\n";
        }
    }

    public void HideToolTip()
    {
        go_BaseUI.SetActive(false);

        kitName.text = "";
        kitDes.text = "";
        kitneedItem.text = "";
    }
}