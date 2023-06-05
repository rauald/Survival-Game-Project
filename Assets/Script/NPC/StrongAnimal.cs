using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrongAnimal : Animal
{
    [SerializeField] protected int attackDamage;  // ���� ������
    [SerializeField] protected float attackDelay;   // ���� ������
    [SerializeField] protected LayerMask targetMask;    // Ÿ��(�÷��̾�) ����ũ
    [SerializeField] protected float chaseTime; // �� �߰� �ð�
    protected float currentChaseTime;   // ���
    [SerializeField] protected float chaseDelayTime;    // �߰� ������


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
            // ����� ������ �ְ�
            if (Vector3.Distance(transform.position, theViewAngle.GetTargetPos()) <= 3f)
            {
                if (theViewAngle.View()) // �� �տ� ���� ���
                {
                    Debug.Log("�÷��̾� ���� �õ�");
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
            Debug.Log("�÷��̾� ����!!");
            thePlayerStatus.DecreaseHP(attackDamage);
        }
        else
        {
            Debug.Log("�÷��̾� ������!!");
        }
        yield return new WaitForSeconds(attackDelay);
        isAttacking = false;
        StartCoroutine(ChaseTargetCoroutine());
    }
}