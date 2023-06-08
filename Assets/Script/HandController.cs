using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : CloseWeaponController
{
    // Ȱ��ȭ ����
    public static bool isActivate = false;
    public static Item currentKit;  // ��ġ�Ϸ��� Ŷ (���� ���̺�)

    private bool isPreview = false;

    private GameObject go_preview;  // ��ġ�� ŰƮ ������
    private Vector3 previewPos; // ��ġ�� ŰƮ ��ġ
    [SerializeField] private float rangeAdd;    // ����� �߰� �����Ÿ�

    [SerializeField]
    private QuickSlotController theQuickSlot;

    // Update is called once per frame
    void Update()
    {
        if (isActivate && !Inventory.inventoryActivated)
        {
            if (currentKit == null)
            {
                if (QuickSlotController.go_HandItem == null) TryAttack();
                else TryEating();
            }
            else
            {
                if(!isPreview)
                {
                    InstallPreviewKit();
                }
                PreviewPostionUpdate();
                Build();
            }
        }
    }

    private void InstallPreviewKit()
    {
        isPreview = true;
        go_preview = Instantiate(currentKit.kitPreviewPrefab, transform.position, Quaternion.identity);
    }

    private void PreviewPostionUpdate()
    {
        if(Physics.Raycast(transform.position, transform.forward, out hitInfo, currentCloseWeapon.range + rangeAdd, layerMask))
        {
            previewPos = hitInfo.point;
            go_preview.transform.position = previewPos;
        }
    }

    private void Build()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            if(go_preview.GetComponent<PreviewObject>().IsBuildable())
            {
                theQuickSlot.DecreaseSelectedItem();    // ���� ������ ���� -1;
                GameObject temp = Instantiate(currentKit.kitPrefab, previewPos, Quaternion.identity);
                temp.name = currentKit.itemName;
                Destroy(go_preview);
                currentKit = null;
                isPreview = false;
            }
        }
    }

    public void Cancel()
    {
        Destroy(go_preview);
        currentKit = null;
        isPreview = false;
    }

    private void TryEating()
    {
        if (Input.GetButtonDown("Fire1") && !theQuickSlot.GetIsCoolTime())
        {
            currentCloseWeapon.anim.SetTrigger("Eat");
            theQuickSlot.DecreaseSelectedItem();
        }
    }

    protected override IEnumerator HitCoroutine()
    {
        while (isSwing)
        {
            if (CheckObject())
            {
                isSwing = !isSwing;
                Debug.Log(hitInfo.transform.name);
            }
            yield return null;
        }
    }

    public override void CloseWeaponChange(CloseWeapon _closeWeapon)
    {
        base.CloseWeaponChange(_closeWeapon);
        isActivate = true;
    }
}