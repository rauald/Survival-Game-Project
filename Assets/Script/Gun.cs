using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public string gunName;          // ���� �̸�
    public float range;             // �� ���� �Ÿ�
    public float accuracy;          // �� ��Ȯ�� (���Ÿ�)
    public float fireRate;          // ���� �ӵ� (�������� ����)
    public float reloadTime;        // ������ �ӵ�

    public int damage;              // ���� ������

    public int reloadBulletCount;   // �Ѿ� ������ ����
    public int currentBulletCount;  // ���� ź������ ���� �ִ� �Ѿ��� ����
    public int maxBulletCount;      // �ִ� ���� ���� �Ѿ� ����
    public int carryBulletCount;    // ���� �����ϰ� �ִ� �Ѿ� ����

    public float retroActionForce;  // �ݵ� ����
    public float retroActionFineSightForce;    // �����ؽ� �ݵ� ����

    public Vector3 fineSightOriginPos;

    public Animator anim;

    public ParticleSystem muzzleFlash;      // �Ѿ� ����Ʈ

    public AudioClip fire_sound;    // �Ѿ� ����
}