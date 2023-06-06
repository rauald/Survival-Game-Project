using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    public static bool isActivated = true;

    // 기존 위치
    private Vector3 originPos;

    // 현재 위치
    private Vector3 currentPos;

    // sway 한계
    [SerializeField]
    private Vector3 limitPos;

    // 정조준 sway 한계
    [SerializeField]
    private Vector3 fineSightLimitPos;

    // 부드러운 움직임 정도
    [SerializeField]
    private Vector3 smoothSway;

    // 필요한 컴포넌트
    [SerializeField]
    private GunController theGunController;

    // Start is called before the first frame update
    void Start()
    {
        originPos = this.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Inventory.inventoryActivated && isActivated)
        {
            TrySway();
        }
    }

    private void TrySway()
    {
        if(Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            Swaying();
        }
        else BackToOriginPos();
    }

    private void Swaying()
    {
        float _moveX = Input.GetAxis("Mouse X");
        float _moveY = Input.GetAxis("Mouse Y");

        //if (!theGunController.isFineSightMode)
        if (!theGunController.GetFineSightMode())
        {
            currentPos.Set(Mathf.Clamp(Mathf.Lerp(currentPos.x, -_moveX, smoothSway.x), -limitPos.x, limitPos.x),
                           Mathf.Clamp(Mathf.Lerp(currentPos.y, -_moveY, smoothSway.x), -limitPos.y, limitPos.y),
                           originPos.z);
        }
        else
        {
            currentPos.Set(Mathf.Clamp(Mathf.Lerp(currentPos.x, -_moveX, smoothSway.y), -fineSightLimitPos.x, fineSightLimitPos.x),
                              Mathf.Clamp(Mathf.Lerp(currentPos.y, -_moveY, smoothSway.y), -fineSightLimitPos.y, fineSightLimitPos.y),
                              originPos.z);
        }
        transform.localEulerAngles = currentPos;
    }

    private void BackToOriginPos()
    {
        currentPos = Vector3.Lerp(currentPos, originPos, smoothSway.y);
        transform.localPosition = currentPos;
    }
}
