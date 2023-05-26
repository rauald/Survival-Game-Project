using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 미완성 클래스 - 추상 클래스 (함수중 하나라도 abstract 추상 함수가 있을경우 꼭 해야한다)
public abstract class CloseWeaponController : MonoBehaviour
{
    // 현재 장착된 Hand형 타입 무기
    [SerializeField]
    protected CloseWeapon currentCloseWeapon;

    // 공격중?
    protected bool isAttack = false;
    protected bool isSwing = false;

    protected RaycastHit hitInfo;

    private PlayerController thePlayerController;

    private void Start()
    {
        thePlayerController = FindObjectOfType<PlayerController>();
    }

    protected void TryAttack()
    {
        // Fire1 => 좌클릭
        if (Input.GetButton("Fire1"))
        {
            if (!isAttack)
            {
                if (CheckObject())
                {
                    if (currentCloseWeapon.isAxe && hitInfo.transform.tag == "Tree")
                    {
                        StartCoroutine(thePlayerController.TreeLookCoroutine(hitInfo.transform.GetComponent<TreeComponent>().GetTreeCenterPosition()));
                        // 코루틴 실행
                        StartCoroutine(AttackCoroutine("Chop", currentCloseWeapon.workDelayA, currentCloseWeapon.workDelayB, currentCloseWeapon.workDelay));
                        return;
                    }
                }

                StartCoroutine(AttackCoroutine("Attack", currentCloseWeapon.attackDelayA, currentCloseWeapon.attackDelayB, currentCloseWeapon.attackDelay));
            }
        }
    }

    protected IEnumerator AttackCoroutine(string _swingType, float _delayA, float _delayB, float _delayC)
    {
        isAttack = true;
        currentCloseWeapon.anim.SetTrigger(_swingType);

        yield return new WaitForSeconds(_delayA);
        isSwing = true;

        // 공격 활성화 시점
        StartCoroutine(HitCoroutine());

        yield return new WaitForSeconds(_delayB);
        isSwing = false;

        yield return new WaitForSeconds(_delayC - _delayA - _delayB);

        isAttack = false;
    }

    // 미완성 - 추상 코루틴 (자식 클래스에서 완성 시켜라)
    protected abstract IEnumerator HitCoroutine();

    protected bool CheckObject()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, currentCloseWeapon.range))
        {
            return true;
        }
        return false;
    }

    // 완성 함수 이지만 추가 편집이 가능한 함수
    public virtual void CloseWeaponChange(CloseWeapon _closeWeapon)
    {
        if (WeaponManager.currentWeapon != null) WeaponManager.currentWeapon.gameObject.SetActive(false);

        currentCloseWeapon = _closeWeapon;
        WeaponManager.currentWeapon = currentCloseWeapon.GetComponent<Transform>();
        WeaponManager.currentWeaponAnim = currentCloseWeapon.anim;

        currentCloseWeapon.transform.localPosition = Vector3.zero;
        currentCloseWeapon.gameObject.SetActive(true);
    }
}