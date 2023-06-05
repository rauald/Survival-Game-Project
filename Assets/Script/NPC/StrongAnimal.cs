using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrongAnimal : Animal
{
    [SerializeField] protected int attackDamage;  // 공격 데미지
    [SerializeField] protected float attackDelay;   // 공격 딜레이
    [SerializeField] protected LayerMask targetMask;    // 타겟(플레이어) 마스크
    [SerializeField] protected float chaseTime; // 총 추격 시간
    protected float currentChaseTime;   // 계산
    [SerializeField] protected float chaseDelayTime;    // 추격 딜레이


    public void Chase(Vector3 _targetPos)
    {
        isChasing = true;
        destination = _targetPos;
        nav.speed = runSpeed;
        isWalking = false;
        isRunning = true;
        anim.SetBool("Running", isRunning);
        nav.SetDestination(destination);
    }

    public override void Damage(int _dmg, Vector3 _targetPos)
    {
        base.Damage(_dmg, _targetPos);
        if (!isDead) Chase(_targetPos);
    }

    protected IEnumerator ChaseTargetCoroutine()
    {
        currentChaseTime = 0;

        while (currentChaseTime < chaseTime)
        {
            Chase(theViewAngle.GetTargetPos());
            // 충분히 가까이 있고
            if (Vector3.Distance(transform.position, theViewAngle.GetTargetPos()) <= 3f)
            {
                if (theViewAngle.View()) // 눈 앞에 있을 경우
                {
                    Debug.Log("플레이어 공격 시도");
                    StartCoroutine(AttackCoroutine());
                }
            }
            yield return new WaitForSeconds(chaseDelayTime);
            currentChaseTime += chaseDelayTime;
        }

        isChasing = false;
        isRunning = false;
        anim.SetBool("Running", isRunning);
        nav.ResetPath();
    }

    protected IEnumerator AttackCoroutine()
    {
        isRunning = false;
        isAttacking = true;
        nav.ResetPath();
        currentChaseTime = chaseTime;
        yield return new WaitForSeconds(0.5f);
        transform.LookAt(theViewAngle.GetTargetPos());

        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(0.5f);
        RaycastHit _hit;
        if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out _hit, 3, targetMask))
        {
            Debug.Log("플레이어 적중!!");
            thePlayerStatus.DecreaseHP(attackDamage);
        }
        else
        {
            Debug.Log("플레이어 빗나감!!");
        }
        yield return new WaitForSeconds(attackDelay);
        isAttacking = false;
        StartCoroutine(ChaseTargetCoroutine());
    }
}